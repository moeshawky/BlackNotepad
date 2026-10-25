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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Savaged.BlackNotepad.Models;
using Savaged.BlackNotepad.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackNotepad.Test.UnitTests
{
    /// <summary>
    /// Unit tests verifying functionality, deduplication, adaptive compression, and integrity of FileHistoryService.
    /// </summary>
    [TestClass]
    public class FileHistoryServiceTests
    {
        private string _tempDir;
        private string _historyDir;
        private FileHistoryService _historyService;

        /// <summary>
        /// Initializes test directories and services prior to each test run.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), "HistoryTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_tempDir);
            _historyDir = Path.Combine(_tempDir, "HistoryStore");
            Directory.CreateDirectory(_historyDir);
            _historyService = new FileHistoryService(_historyDir);
        }

        /// <summary>
        /// Cleans up test directories after each test run.
        /// </summary>
        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_tempDir))
            {
                try
                {
                    Directory.Delete(_tempDir, true);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Verifies that saving snapshots creates chronological history versions and preserves recoverable content.
        /// </summary>
        [TestMethod]
        public async Task SaveSnapshotAsync_CreatesHistoryAndRetrievesContent()
        {
            string filePath = Path.Combine(_tempDir, "sample.txt");
            var snapshot1 = new FileSaveSnapshot(filePath, "Version 1 text content", DateTime.UtcNow.AddMinutes(-5), SaveReason.ManualSave);
            var snapshot2 = new FileSaveSnapshot(filePath, "Version 2 modified content", DateTime.UtcNow, SaveReason.ManualSave);

            await _historyService.SaveSnapshotAsync(snapshot1);
            await _historyService.SaveSnapshotAsync(snapshot2);

            var history = await _historyService.GetHistoryAsync(filePath);
            Assert.AreEqual(2, history.Count);

            string content1 = await _historyService.GetVersionContentAsync(filePath, history[0].VersionId);
            string content2 = await _historyService.GetVersionContentAsync(filePath, history[1].VersionId);

            Assert.AreEqual("Version 1 text content", content1);
            Assert.AreEqual("Version 2 modified content", content2);
        }

        /// <summary>
        /// Verifies that saving identical content consecutively does not record a redundant version.
        /// </summary>
        [TestMethod]
        public async Task SaveSnapshotAsync_DeduplicatesIdenticalConsecutiveSaves()
        {
            string filePath = Path.Combine(_tempDir, "sample_dedup.txt");
            var snapshot1 = new FileSaveSnapshot(filePath, "Identical content", DateTime.UtcNow.AddMinutes(-1), SaveReason.ManualSave);
            var snapshot2 = new FileSaveSnapshot(filePath, "Identical content", DateTime.UtcNow, SaveReason.ManualSave);

            await _historyService.SaveSnapshotAsync(snapshot1);
            await _historyService.SaveSnapshotAsync(snapshot2);

            var history = await _historyService.GetHistoryAsync(filePath);
            Assert.AreEqual(1, history.Count);
        }

        /// <summary>
        /// Verifies that empty document content is correctly saved and restored.
        /// </summary>
        [TestMethod]
        public async Task SaveSnapshotAsync_HandlesEmptyContent()
        {
            string filePath = Path.Combine(_tempDir, "empty.txt");
            var snapshot = new FileSaveSnapshot(filePath, string.Empty, DateTime.UtcNow, SaveReason.ManualSave);

            await _historyService.SaveSnapshotAsync(snapshot);

            var history = await _historyService.GetHistoryAsync(filePath);
            Assert.AreEqual(1, history.Count);

            string content = await _historyService.GetVersionContentAsync(filePath, history[0].VersionId);
            Assert.AreEqual(string.Empty, content);
        }

        /// <summary>
        /// Verifies that external disk changes are captured as ExternalBaseline revisions.
        /// </summary>
        [TestMethod]
        public async Task RecordExternalBaselineIfNeededAsync_RecordsExternalChanges()
        {
            string filePath = Path.Combine(_tempDir, "external.txt");
            File.WriteAllText(filePath, "External initial content");

            await _historyService.RecordExternalBaselineIfNeededAsync(filePath, "External initial content");

            var history = await _historyService.GetHistoryAsync(filePath);
            Assert.AreEqual(1, history.Count);
            Assert.AreEqual(SaveReason.ExternalBaseline, history[0].Reason);

            // Re-checking identical content does not duplicate
            await _historyService.RecordExternalBaselineIfNeededAsync(filePath, "External initial content");
            history = await _historyService.GetHistoryAsync(filePath);
            Assert.AreEqual(1, history.Count);

            // Checking modified content records a new external baseline
            await _historyService.RecordExternalBaselineIfNeededAsync(filePath, "External modified content");
            history = await _historyService.GetHistoryAsync(filePath);
            Assert.AreEqual(2, history.Count);
            Assert.AreEqual(SaveReason.ExternalBaseline, history[1].Reason);
        }

        /// <summary>
        /// Verifies that DeleteHistoryAsync removes the history storage directory.
        /// </summary>
        [TestMethod]
        public async Task DeleteHistoryAsync_RemovesHistoryStore()
        {
            string filePath = Path.Combine(_tempDir, "to_delete.txt");
            var snapshot = new FileSaveSnapshot(filePath, "Some content", DateTime.UtcNow, SaveReason.ManualSave);
            await _historyService.SaveSnapshotAsync(snapshot);

            var historyBefore = await _historyService.GetHistoryAsync(filePath);
            Assert.AreEqual(1, historyBefore.Count);

            await _historyService.DeleteHistoryAsync(filePath);

            var historyAfter = await _historyService.GetHistoryAsync(filePath);
            Assert.AreEqual(0, historyAfter.Count);
        }

        /// <summary>
        /// Verifies that manifest with duplicate version IDs does not crash during version reconstruction and deduplicates safely.
        /// </summary>
        [TestMethod]
        public async Task GetVersionContentAsync_DuplicateVersionIds_DeduplicatesSafelyWithoutThrowing()
        {
            string filePath = Path.Combine(_tempDir, "dup_version.txt");
            var snapshot = new FileSaveSnapshot(filePath, "Initial base content", DateTime.UtcNow, SaveReason.ManualSave);
            await _historyService.SaveSnapshotAsync(snapshot);

            var history = await _historyService.GetHistoryAsync(filePath);
            Assert.AreEqual(1, history.Count);
            string baseVersionId = history[0].VersionId;

            // Find history folder for filePath
            string[] subDirs = Directory.GetDirectories(_historyDir);
            Assert.AreEqual(1, subDirs.Length);
            string historySubDir = subDirs[0];
            string manifestPath = Path.Combine(historySubDir, "manifest.json");

            string manifestJson = File.ReadAllText(manifestPath, Encoding.UTF8);
            var manifest = JsonConvert.DeserializeObject<HistoryManifest>(manifestJson);

            // Inject a duplicate version with same VersionId
            var dupVersion = new HistoryVersion
            {
                VersionId = baseVersionId,
                TimestampUtc = DateTime.UtcNow,
                Reason = SaveReason.AutoSave,
                Sha256 = history[0].Sha256,
                ContentLength = history[0].ContentLength,
                StoredSizeBytes = history[0].StoredSizeBytes,
                CompressionType = history[0].CompressionType,
                PayloadFileName = history[0].PayloadFileName,
                Depth = 0
            };
            manifest.Versions.Add(dupVersion);

            File.WriteAllText(manifestPath, JsonConvert.SerializeObject(manifest, Formatting.Indented), Encoding.UTF8);

            // Retrieving content should not throw ArgumentException from ToDictionary
            string content = await _historyService.GetVersionContentAsync(filePath, baseVersionId);
            Assert.AreEqual("Initial base content", content);
        }

        /// <summary>
        /// Verifies that a circular reference in the delta version chain is detected and throws an InvalidOperationException.
        /// </summary>
        [TestMethod]
        public async Task GetVersionContentAsync_CycleInDeltaChain_ThrowsInvalidOperationException()
        {
            string filePath = Path.Combine(_tempDir, "cycle_test.txt");
            var snapshot = new FileSaveSnapshot(filePath, "Base content", DateTime.UtcNow, SaveReason.ManualSave);
            await _historyService.SaveSnapshotAsync(snapshot);

            string[] subDirs = Directory.GetDirectories(_historyDir);
            Assert.AreEqual(1, subDirs.Length);
            string historySubDir = subDirs[0];
            string manifestPath = Path.Combine(historySubDir, "manifest.json");

            string manifestJson = File.ReadAllText(manifestPath, Encoding.UTF8);
            var manifest = JsonConvert.DeserializeObject<HistoryManifest>(manifestJson);

            // Create circular delta references: v1 -> v2 and v2 -> v1
            var delta1 = new HistoryVersion
            {
                VersionId = "delta_v1",
                BaseVersionId = "delta_v2",
                CompressionType = "DeltaMsDelta",
                PayloadFileName = "dummy1.bin"
            };
            var delta2 = new HistoryVersion
            {
                VersionId = "delta_v2",
                BaseVersionId = "delta_v1",
                CompressionType = "DeltaMsDelta",
                PayloadFileName = "dummy2.bin"
            };
            manifest.Versions.Add(delta1);
            manifest.Versions.Add(delta2);

            File.WriteAllText(manifestPath, JsonConvert.SerializeObject(manifest, Formatting.Indented), Encoding.UTF8);

            bool cycleExceptionThrown = false;
            try
            {
                await _historyService.GetVersionContentAsync(filePath, "delta_v1");
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("Delta cycle detected"))
                {
                    cycleExceptionThrown = true;
                }
            }

            Assert.IsTrue(cycleExceptionThrown, "Expected InvalidOperationException indicating a delta cycle was detected.");
        }

        /// <summary>
        /// Verifies that corrupted JSON in manifest.json is gracefully handled by returning an empty history list.
        /// </summary>
        [TestMethod]
        public async Task GetHistoryAsync_CorruptedManifestJson_RecoversGracefully()
        {
            string filePath = Path.Combine(_tempDir, "corrupted_manifest.txt");
            var snapshot = new FileSaveSnapshot(filePath, "Valid content", DateTime.UtcNow, SaveReason.ManualSave);
            await _historyService.SaveSnapshotAsync(snapshot);

            string[] subDirs = Directory.GetDirectories(_historyDir);
            Assert.AreEqual(1, subDirs.Length);
            string historySubDir = subDirs[0];
            string manifestPath = Path.Combine(historySubDir, "manifest.json");

            // Overwrite manifest with invalid/corrupted JSON
            File.WriteAllText(manifestPath, "{ corrupted invalid json content !!!", Encoding.UTF8);

            var history = await _historyService.GetHistoryAsync(filePath);
            Assert.IsNotNull(history);
            Assert.AreEqual(0, history.Count);
        }

        /// <summary>
        /// Verifies that MsDeltaCodec handles null and 0-byte input buffers safely without unhandled native errors.
        /// </summary>
        [TestMethod]
        public void MsDeltaCodec_NullOrZeroByteBuffers_ReturnsNull()
        {
            byte[] nonEmpty = new byte[] { 1, 2, 3, 4 };
            byte[] empty = new byte[0];

            // CreateDelta edge cases
            Assert.IsNull(MsDeltaCodec.CreateDelta(null, nonEmpty));
            Assert.IsNull(MsDeltaCodec.CreateDelta(nonEmpty, null));
            Assert.IsNull(MsDeltaCodec.CreateDelta(null, null));
            Assert.IsNull(MsDeltaCodec.CreateDelta(empty, nonEmpty));
            Assert.IsNull(MsDeltaCodec.CreateDelta(nonEmpty, empty));
            Assert.IsNull(MsDeltaCodec.CreateDelta(empty, empty));

            // ApplyDelta edge cases
            Assert.IsNull(MsDeltaCodec.ApplyDelta(null, nonEmpty));
            Assert.IsNull(MsDeltaCodec.ApplyDelta(nonEmpty, null));
            Assert.IsNull(MsDeltaCodec.ApplyDelta(null, null));
            Assert.IsNull(MsDeltaCodec.ApplyDelta(empty, nonEmpty));
            Assert.IsNull(MsDeltaCodec.ApplyDelta(nonEmpty, empty));
            Assert.IsNull(MsDeltaCodec.ApplyDelta(empty, empty));
        }
    }
}
