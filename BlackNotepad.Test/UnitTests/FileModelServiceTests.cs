using Microsoft.VisualStudio.TestTools.UnitTesting;
using Savaged.BlackNotepad.Lookups;
using Savaged.BlackNotepad.Models;
using Savaged.BlackNotepad.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BlackNotepad.Test.UnitTests
{
    [TestClass]
    public class FileModelServiceTests
    {
        [TestMethod]
        public void New_ReturnsNonNullFileModel()
        {
            var service = new FileModelService();

            var result = service.New();

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void New_ReturnsUntitledFileModel()
        {
            var service = new FileModelService();

            var result = service.New();

            Assert.AreEqual("Untitled", result.Name);
            Assert.IsFalse(result.IsDirty);
            Assert.AreEqual(string.Empty, result.Content);
        }

        [TestMethod]
        public async Task LoadAsync_NullLocation_ReturnsEmptyContentFileModel()
        {
            var service = new FileModelService();

            var result = await service.LoadAsync(null);

            Assert.IsNotNull(result);
            Assert.AreEqual(string.Empty, result.Content);
            Assert.IsFalse(result.IsDirty);
        }

        [TestMethod]
        public async Task LoadAsync_EmptyLocation_ReturnsEmptyContentFileModel()
        {
            var service = new FileModelService();

            var result = await service.LoadAsync(string.Empty);

            Assert.IsNotNull(result);
            Assert.AreEqual(string.Empty, result.Content);
            Assert.IsFalse(result.IsDirty);
        }

        [TestMethod]
        public async Task LoadAsync_WhitespaceLocation_ReturnsEmptyContentFileModel()
        {
            var service = new FileModelService();

            var result = await service.LoadAsync("   ");

            Assert.IsNotNull(result);
            Assert.AreEqual(string.Empty, result.Content);
            Assert.IsFalse(result.IsDirty);
        }

        [TestMethod]
        public async Task SaveAsync_CreatesTempFileThenReplacesOriginal()
        {
            var service = new FileModelService();
            var tempDir = Path.Combine(Path.GetTempPath(), "BlackNotepadTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var location = Path.Combine(tempDir, "test.txt");
                File.WriteAllText(location, "original content");

                var fileModel = new FileModel
                {
                    Location = location,
                    Content = "new content"
                };

                await service.SaveAsync(fileModel);

                Assert.AreEqual("new content", File.ReadAllText(location));
                Assert.IsFalse(File.Exists(location + ".tmp"), "Temp file should be cleaned up.");
                Assert.IsFalse(File.Exists(location + ".bak"), "Backup file should be cleaned up.");
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task SaveAsync_CleansUpTempFileOnSuccess()
        {
            var service = new FileModelService();
            var tempDir = Path.Combine(Path.GetTempPath(), "BlackNotepadTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var location = Path.Combine(tempDir, "test.txt");
                File.WriteAllText(location, "original content");

                var fileModel = new FileModel
                {
                    Location = location,
                    Content = "new content"
                };

                await service.SaveAsync(fileModel);

                Assert.IsFalse(File.Exists(location + ".tmp"), "Temp file should not exist after successful save.");
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task SaveAsync_CleansUpTempFileOnFailure()
        {
            var service = new FileModelService();
            var tempDir = Path.Combine(Path.GetTempPath(), "BlackNotepadTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            string location = null;
            try
            {
                location = Path.Combine(tempDir, "test.txt");
                File.WriteAllText(location, "original content");
                File.SetAttributes(location, FileAttributes.ReadOnly);

                var fileModel = new FileModel
                {
                    Location = location,
                    Content = "new content"
                };

                await Assert.ThrowsExceptionAsync<UnauthorizedAccessException>(async () =>
                {
                    await service.SaveAsync(fileModel);
                });

                Assert.IsFalse(File.Exists(location + ".tmp"), "Temp file should be cleaned up after failure.");
            }
            finally
            {
                if (location != null && File.Exists(location))
                {
                    File.SetAttributes(location, FileAttributes.Normal);
                }
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task LoadAsync_WithRealFile_ReturnsCorrectContent()
        {
            var service = new FileModelService();
            var tempDir = Path.Combine(Path.GetTempPath(), "BlackNotepadTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var location = Path.Combine(tempDir, "test.txt");
                File.WriteAllText(location, "Hello, World!");

                var result = await service.LoadAsync(location);

                Assert.AreEqual("Hello, World!", result.Content);
                Assert.AreEqual(location, result.Location);
                Assert.IsFalse(result.IsDirty);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task LoadAsync_WithCRLF_DetectsCRLF()
        {
            var service = new FileModelService();
            var tempDir = Path.Combine(Path.GetTempPath(), "BlackNotepadTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var location = Path.Combine(tempDir, "test.txt");
                File.WriteAllBytes(location, new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x0D, 0x0A, 0x57, 0x6F, 0x72, 0x6C, 0x64 });

                var result = await service.LoadAsync(location);

                Assert.AreEqual(LineEndings.CRLF, result.LineEnding);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task LoadAsync_WithLF_DetectsLF()
        {
            var service = new FileModelService();
            var tempDir = Path.Combine(Path.GetTempPath(), "BlackNotepadTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var location = Path.Combine(tempDir, "test.txt");
                File.WriteAllBytes(location, new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x0A, 0x57, 0x6F, 0x72, 0x6C, 0x64 });

                var result = await service.LoadAsync(location);

                Assert.AreEqual(LineEndings.LF, result.LineEnding);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task LoadAsync_WithCR_DetectsCR()
        {
            var service = new FileModelService();
            var tempDir = Path.Combine(Path.GetTempPath(), "BlackNotepadTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var location = Path.Combine(tempDir, "test.txt");
                File.WriteAllBytes(location, new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F, 0x0D, 0x57, 0x6F, 0x72, 0x6C, 0x64 });

                var result = await service.LoadAsync(location);

                Assert.AreEqual(LineEndings.CR, result.LineEnding);
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }

        [TestMethod]
        public async Task LoadAsync_NonExistentFile_ThrowsFileNotFoundException()
        {
            var service = new FileModelService();
            var tempDir = Path.Combine(Path.GetTempPath(), "BlackNotepadTest_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);
            try
            {
                var location = Path.Combine(tempDir, "nonexistent.txt");

                await Assert.ThrowsExceptionAsync<FileNotFoundException>(async () =>
                {
                    await service.LoadAsync(location);
                });
            }
            finally
            {
                Directory.Delete(tempDir, true);
            }
        }
    }
