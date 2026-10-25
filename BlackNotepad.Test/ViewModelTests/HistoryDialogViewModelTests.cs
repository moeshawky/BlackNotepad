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
using Moq;
using Savaged.BlackNotepad.Models;
using Savaged.BlackNotepad.Services;
using Savaged.BlackNotepad.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BlackNotepad.Test.ViewModelTests
{
    /// <summary>
    /// Unit tests verifying Previous Versions dialog ViewModel behavior, preview error containment, and restore guards.
    /// </summary>
    [TestClass]
    public class HistoryDialogViewModelTests
    {
        private Mock<IFileHistoryService> _mockHistoryService;

        /// <summary>
        /// Initializes test mocks before each test method runs.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            _mockHistoryService = new Mock<IFileHistoryService>();
        }

        /// <summary>
        /// Verifies that InitializeAsync with empty file location does not query history.
        /// </summary>
        [TestMethod]
        public async Task InitializeAsync_EmptyLocation_DoesNotQueryHistory()
        {
            var vm = new HistoryDialogViewModel(_mockHistoryService.Object);
            await vm.InitializeAsync(string.Empty);

            Assert.AreEqual(0, vm.Versions.Count);
            Assert.IsFalse(vm.IsActionEnabled);
            _mockHistoryService.Verify(s => s.GetHistoryAsync(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Verifies that InitializeAsync populates versions in reverse chronological order and selects the newest version.
        /// </summary>
        [TestMethod]
        public async Task InitializeAsync_ValidHistory_PopulatesAndSelectsNewest()
        {
            var v1 = new HistoryVersion { VersionId = "v1", TimestampUtc = DateTime.UtcNow.AddMinutes(-10) };
            var v2 = new HistoryVersion { VersionId = "v2", TimestampUtc = DateTime.UtcNow };

            _mockHistoryService.Setup(s => s.GetHistoryAsync("C:\\test.txt"))
                .ReturnsAsync(new List<HistoryVersion> { v1, v2 });
            _mockHistoryService.Setup(s => s.GetVersionContentAsync("C:\\test.txt", "v2"))
                .ReturnsAsync("v2 content");

            var vm = new HistoryDialogViewModel(_mockHistoryService.Object);
            await vm.InitializeAsync("C:\\test.txt");

            Assert.AreEqual(2, vm.Versions.Count);
            Assert.AreEqual("v2", vm.SelectedVersion.VersionId);
            Assert.AreEqual("v2 content", vm.PreviewContent);
            Assert.IsTrue(vm.IsActionEnabled);
        }

        /// <summary>
        /// Verifies that Restore sets RestoredContent and IsRestored when preview load succeeded.
        /// </summary>
        [TestMethod]
        public async Task Restore_SuccessfulPreview_RestoresContent()
        {
            var v1 = new HistoryVersion { VersionId = "v1", TimestampUtc = DateTime.UtcNow };
            _mockHistoryService.Setup(s => s.GetHistoryAsync("C:\\test.txt"))
                .ReturnsAsync(new List<HistoryVersion> { v1 });
            _mockHistoryService.Setup(s => s.GetVersionContentAsync("C:\\test.txt", "v1"))
                .ReturnsAsync("Restorable text content");

            var vm = new HistoryDialogViewModel(_mockHistoryService.Object);
            await vm.InitializeAsync("C:\\test.txt");

            Assert.IsTrue(vm.IsActionEnabled);
            vm.Restore();

            Assert.IsTrue(vm.IsRestored);
            Assert.AreEqual("Restorable text content", vm.RestoredContent);
        }

        /// <summary>
        /// Verifies G-ERR-1 remediation: When preview loading fails with an exception,
        /// IsActionEnabled remains false and Restore() does NOT assign the error string to RestoredContent.
        /// </summary>
        [TestMethod]
        public async Task LoadPreviewAsync_WhenServiceThrows_DisablesActionAndPreventsRestoringErrorMessage()
        {
            var v1 = new HistoryVersion { VersionId = "corrupted_v1", TimestampUtc = DateTime.UtcNow };
            _mockHistoryService.Setup(s => s.GetHistoryAsync("C:\\test.txt"))
                .ReturnsAsync(new List<HistoryVersion> { v1 });
            _mockHistoryService.Setup(s => s.GetVersionContentAsync("C:\\test.txt", "corrupted_v1"))
                .ThrowsAsync(new IOException("Disk read error"));

            var vm = new HistoryDialogViewModel(_mockHistoryService.Object);
            await vm.InitializeAsync("C:\\test.txt");

            // Preview content shows the error notification
            Assert.IsTrue(vm.PreviewContent.Contains("[Error loading version content:"));

            // But IsActionEnabled MUST be false (G-ERR-1)
            Assert.IsFalse(vm.IsActionEnabled, "IsActionEnabled must be false when preview failed to load.");
            Assert.IsFalse(vm.RestoreCmd.CanExecute(null), "Restore command CanExecute must be false when preview failed to load.");

            // Attempting to call Restore directly must not overwrite document with error text
            vm.Restore();

            Assert.IsFalse(vm.IsRestored, "IsRestored must remain false on failed preview.");
            Assert.IsNull(vm.RestoredContent, "RestoredContent must remain null and not be overwritten with error message.");
        }
    }
}
