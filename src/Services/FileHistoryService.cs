// Copyright (C) 2026 Moe S. / BlackNotepad Contributors
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

using GalaSoft.MvvmLight.Ioc;
using Newtonsoft.Json;
using Savaged.BlackNotepad.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Savaged.BlackNotepad.Services
{
    /// <summary>
    /// Core local history storage service providing adaptive FULL (GZip) and DELTA (msdelta.dll) compression,
    /// DPAPI encryption, atomic manifest commits, and bounded retention management.
    /// </summary>
    public class FileHistoryService : IFileHistoryService
    {
        private const int MaxDeltaDepth = 16;
        private const int MaxDeltaPayloadBytes = 16 * 1024 * 1024; // 16 MiB
        private const double DeltaSavingsThreshold = 0.80; // Delta must be < 80% of GZip size (20% savings)
        private const int MaxVersionsRetention = 10000;
        private const long MaxTotalSizeBytesRetention = 64L * 1024 * 1024; // 64 MiB
        private static readonly TimeSpan MaxRetentionAge = TimeSpan.FromDays(30);

        private readonly string _baseHistoryDirectory;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _dirLocks = new ConcurrentDictionary<string, SemaphoreSlim>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHistoryService"/> class with default settings.
        /// </summary>
        [PreferredConstructor]
        public FileHistoryService()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileHistoryService"/> class.
        /// </summary>
        /// <param name="baseHistoryDirectory">Optional custom root history directory path. If null, defaults to LocalApplicationData/BlackNotepad/History.</param>
        public FileHistoryService(string baseHistoryDirectory = null)
        {
            _baseHistoryDirectory = !string.IsNullOrWhiteSpace(baseHistoryDirectory)
                ? baseHistoryDirectory
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BlackNotepad", "History");
        }

        /// <summary>
        /// Saves an immutable document snapshot to the local history store.
        /// </summary>
        /// <param name="snapshot">The immutable snapshot to record.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task SaveSnapshotAsync(FileSaveSnapshot snapshot)
        {
            if (snapshot == null || string.IsNullOrWhiteSpace(snapshot.Location))
            {
                return;
            }

            string historyDir = GetHistoryDirectory(snapshot.Location);
            if (historyDir == null)
            {
                return;
            }

            var dirLock = GetLockForDir(historyDir);
            await dirLock.WaitAsync().ConfigureAwait(false);
            try
            {
                Directory.CreateDirectory(historyDir);
                var manifest = LoadManifest(historyDir, snapshot.Location);

                byte[] targetBytes = Encoding.UTF8.GetBytes(snapshot.Content ?? string.Empty);
                string contentSha256 = ComputeSha256Hex(targetBytes);

                // Deduplication guard: ignore redundant duplicate saves if content is identical to last recorded version
                if (manifest.Versions.Count > 0)
                {
                    var lastVersion = manifest.Versions[manifest.Versions.Count - 1];
                    if (string.Equals(lastVersion.Sha256, contentSha256, StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                }

                byte[] fullGzipBytes = CompressGzip(targetBytes);
                string compressionType = "FullGzip";
                byte[] payloadBytes = fullGzipBytes;
                string baseVersionId = null;
                int depth = 0;

                // Adaptive compression: evaluate MSDelta if depth < 16, uncompressed size <= 16MiB, and delta savings > 20%
                if (manifest.Versions.Count > 0)
                {
                    var previousVersion = manifest.Versions[manifest.Versions.Count - 1];
                    if (previousVersion.Depth < MaxDeltaDepth && targetBytes.Length <= MaxDeltaPayloadBytes)
                    {
                        try
                        {
                            byte[] baseBytes = GetVersionBytes(historyDir, manifest, previousVersion);
                            if (baseBytes != null)
                            {
                                byte[] deltaBytes = MsDeltaCodec.CreateDelta(baseBytes, targetBytes);
                                if (deltaBytes != null && deltaBytes.Length < (fullGzipBytes.Length * DeltaSavingsThreshold))
                                {
                                    compressionType = "DeltaMsDelta";
                                    payloadBytes = deltaBytes;
                                    baseVersionId = previousVersion.VersionId;
                                    depth = previousVersion.Depth + 1;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Delta compression attempt failed, falling back to FullGzip: {ex.Message}");
                        }
                    }
                }

                // DPAPI protection with graceful fallback
                byte[] protectedPayload = ProtectPayload(payloadBytes, out bool isEncrypted);

                string versionId = Guid.NewGuid().ToString("N");
                string payloadFileName = $"{versionId}.bin";
                string payloadPath = Path.Combine(historyDir, payloadFileName);
                string tmpPayloadPath = Path.Combine(historyDir, $"{payloadFileName}.tmp");

                // Transaction order: physically write payload file first
                File.WriteAllBytes(tmpPayloadPath, protectedPayload);
                if (File.Exists(payloadPath))
                {
                    File.Delete(payloadPath);
                }
                File.Move(tmpPayloadPath, payloadPath);

                var version = new HistoryVersion
                {
                    VersionId = versionId,
                    TimestampUtc = snapshot.TimestampUtc,
                    Reason = snapshot.Reason,
                    Sha256 = contentSha256,
                    ContentLength = (snapshot.Content ?? string.Empty).Length,
                    StoredSizeBytes = protectedPayload.Length,
                    CompressionType = compressionType,
                    BaseVersionId = baseVersionId,
                    Depth = depth,
                    PayloadFileName = payloadFileName,
                    IsEncrypted = isEncrypted
                };

                manifest.Versions.Add(version);

                // Apply segment retention pruning
                ApplySegmentRetention(historyDir, manifest);

                // Atomically rewrite manifest.json
                SaveManifestAtomic(historyDir, manifest);
            }
            finally
            {
                dirLock.Release();
            }
        }

        /// <summary>
        /// Compares the current content on disk with the most recent historical version and records an external baseline version if changes are detected.
        /// </summary>
        /// <param name="fileLocation">The absolute file path on disk.</param>
        /// <param name="diskContent">The content currently stored on disk.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RecordExternalBaselineIfNeededAsync(string fileLocation, string diskContent)
        {
            if (string.IsNullOrWhiteSpace(fileLocation) || diskContent == null)
            {
                return;
            }

            string historyDir = GetHistoryDirectory(fileLocation);
            if (historyDir == null)
            {
                return;
            }

            var dirLock = GetLockForDir(historyDir);
            await dirLock.WaitAsync().ConfigureAwait(false);
            try
            {
                var manifest = LoadManifest(historyDir, fileLocation);
                byte[] diskBytes = Encoding.UTF8.GetBytes(diskContent);
                string diskSha256 = ComputeSha256Hex(diskBytes);

                if (manifest.Versions.Count > 0)
                {
                    var lastVersion = manifest.Versions[manifest.Versions.Count - 1];
                    if (string.Equals(lastVersion.Sha256, diskSha256, StringComparison.OrdinalIgnoreCase))
                    {
                        return; // Disk content matches HEAD version, no external baseline needed
                    }
                }
            }
            finally
            {
                dirLock.Release();
            }

            var snapshot = new FileSaveSnapshot(fileLocation, diskContent, DateTime.UtcNow, SaveReason.ExternalBaseline);
            await SaveSnapshotAsync(snapshot).ConfigureAwait(false);
        }

        /// <summary>
        /// Retrieves the chronological history versions for a given file location.
        /// </summary>
        /// <param name="fileLocation">The path to the file on disk.</param>
        /// <returns>A list of history versions, ordered chronologically.</returns>
        public async Task<IReadOnlyList<HistoryVersion>> GetHistoryAsync(string fileLocation)
        {
            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return new List<HistoryVersion>().AsReadOnly();
            }

            string historyDir = GetHistoryDirectory(fileLocation);
            if (historyDir == null || !Directory.Exists(historyDir))
            {
                return new List<HistoryVersion>().AsReadOnly();
            }

            var dirLock = GetLockForDir(historyDir);
            await dirLock.WaitAsync().ConfigureAwait(false);
            try
            {
                var manifest = LoadManifest(historyDir, fileLocation);
                return manifest.Versions.AsReadOnly();
            }
            finally
            {
                dirLock.Release();
            }
        }

        /// <summary>
        /// Retrieves the uncompressed text content of a specific historical version.
        /// </summary>
        /// <param name="fileLocation">The path to the file on disk.</param>
        /// <param name="versionId">The unique identifier of the version to retrieve.</param>
        /// <returns>The uncompressed text content of the version, or null if not found.</returns>
        public async Task<string> GetVersionContentAsync(string fileLocation, string versionId)
        {
            if (string.IsNullOrWhiteSpace(fileLocation) || string.IsNullOrWhiteSpace(versionId))
            {
                return null;
            }

            string historyDir = GetHistoryDirectory(fileLocation);
            if (historyDir == null || !Directory.Exists(historyDir))
            {
                return null;
            }

            var dirLock = GetLockForDir(historyDir);
            await dirLock.WaitAsync().ConfigureAwait(false);
            try
            {
                var manifest = LoadManifest(historyDir, fileLocation);
                var version = manifest.Versions.FirstOrDefault(v => v.VersionId == versionId);
                if (version == null)
                {
                    return null;
                }

                byte[] contentBytes = GetVersionBytes(historyDir, manifest, version);
                if (contentBytes == null)
                {
                    return null;
                }

                // Verify integrity
                string actualSha256 = ComputeSha256Hex(contentBytes);
                if (!string.Equals(actualSha256, version.Sha256, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"Integrity verification failed for version {versionId}. Expected {version.Sha256}, got {actualSha256}.");
                }

                return Encoding.UTF8.GetString(contentBytes);
            }
            finally
            {
                dirLock.Release();
            }
        }

        /// <summary>
        /// Deletes the local history store for a given file.
        /// </summary>
        /// <param name="fileLocation">The path to the file on disk.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteHistoryAsync(string fileLocation)
        {
            if (string.IsNullOrWhiteSpace(fileLocation))
            {
                return;
            }

            string historyDir = GetHistoryDirectory(fileLocation);
            if (historyDir == null || !Directory.Exists(historyDir))
            {
                return;
            }

            var dirLock = GetLockForDir(historyDir);
            await dirLock.WaitAsync().ConfigureAwait(false);
            try
            {
                Directory.Delete(historyDir, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to delete history directory '{historyDir}': {ex.Message}");
            }
            finally
            {
                dirLock.Release();
            }
        }

        private SemaphoreSlim GetLockForDir(string dir)
        {
            return _dirLocks.GetOrAdd(dir.ToLowerInvariant(), _ => new SemaphoreSlim(1, 1));
        }

        private string GetHistoryDirectory(string fileLocation)
        {
            try
            {
                string normalizedPath = Path.GetFullPath(fileLocation).Trim().ToLowerInvariant();
                using (var sha = SHA256.Create())
                {
                    byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(normalizedPath));
                    string folderName = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    return Path.Combine(_baseHistoryDirectory, folderName);
                }
            }
            catch
            {
                return null;
            }
        }

        private static HistoryManifest LoadManifest(string historyDir, string originalFilePath)
        {
            string manifestPath = Path.Combine(historyDir, "manifest.json");
            if (File.Exists(manifestPath))
            {
                try
                {
                    string json = File.ReadAllText(manifestPath, Encoding.UTF8);
                    var manifest = JsonConvert.DeserializeObject<HistoryManifest>(json);
                    if (manifest != null && manifest.Versions != null)
                    {
                        return manifest;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error reading manifest.json in '{historyDir}': {ex.Message}");
                }
            }

            return new HistoryManifest
            {
                SchemaVersion = 1,
                OriginalFilePath = originalFilePath,
                Versions = new List<HistoryVersion>()
            };
        }

        private static void SaveManifestAtomic(string historyDir, HistoryManifest manifest)
        {
            string manifestPath = Path.Combine(historyDir, "manifest.json");
            string tmpPath = Path.Combine(historyDir, $"manifest.{Guid.NewGuid():N}.tmp");
            string json = JsonConvert.SerializeObject(manifest, Formatting.Indented);

            File.WriteAllText(tmpPath, json, Encoding.UTF8);
            if (File.Exists(manifestPath))
            {
                string bakPath = Path.Combine(historyDir, $"manifest.{Guid.NewGuid():N}.bak");
                try
                {
                    File.Replace(tmpPath, manifestPath, bakPath);
                    if (File.Exists(bakPath))
                    {
                        File.Delete(bakPath);
                    }
                }
                catch
                {
                    File.Copy(tmpPath, manifestPath, true);
                    try { File.Delete(tmpPath); } catch { }
                }
            }
            else
            {
                File.Move(tmpPath, manifestPath);
            }
        }

        private static byte[] GetVersionBytes(string historyDir, HistoryManifest manifest, HistoryVersion version)
        {
            if (string.Equals(version.CompressionType, "FullGzip", StringComparison.OrdinalIgnoreCase))
            {
                return ReadAndDecompressFull(historyDir, version);
            }

            if (string.Equals(version.CompressionType, "DeltaMsDelta", StringComparison.OrdinalIgnoreCase))
            {
                var deltaChain = new List<HistoryVersion>();
                var current = version;
                var versionMap = manifest.Versions
                    .Where(v => v != null && v.VersionId != null)
                    .GroupBy(v => v.VersionId)
                    .ToDictionary(g => g.Key, g => g.First());

                var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                while (current != null && string.Equals(current.CompressionType, "DeltaMsDelta", StringComparison.OrdinalIgnoreCase))
                {
                    if (current.VersionId != null && !visited.Add(current.VersionId))
                    {
                        throw new InvalidOperationException($"Delta cycle detected in version history at version {current.VersionId}");
                    }

                    deltaChain.Add(current);
                    if (current.BaseVersionId != null && versionMap.TryGetValue(current.BaseVersionId, out var baseVer))
                    {
                        current = baseVer;
                    }
                    else
                    {
                        break;
                    }
                }

                if (current == null || !string.Equals(current.CompressionType, "FullGzip", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"Delta reconstruction failed: Base FullGzip version missing for version {version.VersionId}");
                }

                byte[] reconstructed = ReadAndDecompressFull(historyDir, current);
                deltaChain.Reverse(); // Process from oldest delta forward to target version

                foreach (var deltaVersion in deltaChain)
                {
                    byte[] deltaPayload = ReadAndUnprotect(historyDir, deltaVersion);
                    reconstructed = MsDeltaCodec.ApplyDelta(reconstructed, deltaPayload);
                    if (reconstructed == null)
                    {
                        throw new InvalidOperationException($"msdelta.dll failed to apply delta for version {deltaVersion.VersionId}");
                    }
                }

                return reconstructed;
            }

            throw new NotSupportedException($"Unsupported compression type: {version.CompressionType}");
        }

        private static byte[] ReadAndDecompressFull(string historyDir, HistoryVersion version)
        {
            byte[] payload = ReadAndUnprotect(historyDir, version);
            return DecompressGzip(payload);
        }

        private static byte[] ReadAndUnprotect(string historyDir, HistoryVersion version)
        {
            string filePath = Path.Combine(historyDir, version.PayloadFileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"History payload file not found: {filePath}");
            }

            byte[] protectedData = File.ReadAllBytes(filePath);
            return UnprotectPayload(protectedData, version.IsEncrypted);
        }

        private static byte[] ProtectPayload(byte[] data, out bool isEncrypted)
        {
            try
            {
                byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
                isEncrypted = true;
                return encrypted;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DPAPI ProtectedData.Protect failed, using plaintext payload: {ex.Message}");
                isEncrypted = false;
                return data;
            }
        }

        private static byte[] UnprotectPayload(byte[] data, bool isEncrypted)
        {
            if (!isEncrypted)
            {
                return data;
            }

            try
            {
                return ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DPAPI ProtectedData.Unprotect failed, returning payload as-is: {ex.Message}");
                return data;
            }
        }

        private static byte[] CompressGzip(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                using (var gzip = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    gzip.Write(data, 0, data.Length);
                }
                return ms.ToArray();
            }
        }

        private static byte[] DecompressGzip(byte[] compressed)
        {
            using (var ms = new MemoryStream(compressed))
            using (var gzip = new GZipStream(ms, CompressionMode.Decompress))
            using (var outMs = new MemoryStream())
            {
                gzip.CopyTo(outMs);
                return outMs.ToArray();
            }
        }

        private static string ComputeSha256Hex(byte[] data)
        {
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// Applies segment-based retention pruning to maintain bounded history storage.
        /// Prunes full segments starting from the oldest when age, version count, or total size limits are exceeded.
        /// Preserves delta chain integrity by only removing complete segments that are superseded by later full checkpoints.
        /// </summary>
        private static void ApplySegmentRetention(string historyDir, HistoryManifest manifest)
        {
            if (manifest.Versions == null || manifest.Versions.Count == 0)
            {
                return;
            }

            DateTime cutoffUtc = DateTime.UtcNow - MaxRetentionAge;

            long totalBytes = 0;
            for (int i = 0; i < manifest.Versions.Count; i++)
            {
                totalBytes += manifest.Versions[i].StoredSizeBytes;
            }

            while (true)
            {
                // Find all segment start indices (Depth == 0)
                int firstSegmentEnd = -1;
                int baselineCount = 0;
                for (int i = 0; i < manifest.Versions.Count; i++)
                {
                    if (manifest.Versions[i].Depth == 0)
                    {
                        baselineCount++;
                        if (baselineCount == 2)
                        {
                            firstSegmentEnd = i;
                            break;
                        }
                    }
                }

                // If only 1 segment exists, we cannot prune it without losing historical baseline continuity
                if (baselineCount <= 1)
                {
                    break;
                }

                var firstSegmentLastVersion = manifest.Versions[firstSegmentEnd - 1];

                bool isExpired = firstSegmentLastVersion.TimestampUtc < cutoffUtc;
                bool isCountExceeded = manifest.Versions.Count > MaxVersionsRetention;
                bool isSizeExceeded = totalBytes > MaxTotalSizeBytesRetention;

                if (!isExpired && !isCountExceeded && !isSizeExceeded)
                {
                    break; // Within all retention limits
                }

                // Prune first segment
                for (int i = 0; i < firstSegmentEnd; i++)
                {
                    var ver = manifest.Versions[i];
                    totalBytes -= ver.StoredSizeBytes;
                    try
                    {
                        string path = Path.Combine(historyDir, ver.PayloadFileName);
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to delete pruned payload {ver.PayloadFileName}: {ex.Message}");
                    }
                }

                manifest.Versions.RemoveRange(0, firstSegmentEnd);
            }
        }
    }
}
