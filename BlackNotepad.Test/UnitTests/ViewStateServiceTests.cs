using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Savaged.BlackNotepad.Models;
using Savaged.BlackNotepad.Services;
using System;
using System.IO;

namespace BlackNotepad.Test.UnitTests
{
    [TestClass]
    public class ViewStateServiceTests
    {
        private Mock<IFontColourLookupService> _mockColourLookup;
        private Mock<IFontFamilyLookupService> _mockFamilyLookup;
        private Mock<IFontZoomLookupService> _mockZoomLookup;

        [TestInitialize]
        public void Init()
        {
            _mockColourLookup = new Mock<IFontColourLookupService>();
            _mockColourLookup.Setup(s => s.GetDefault())
                .Returns(new FontColourModel { IsSelected = true });

            _mockFamilyLookup = new Mock<IFontFamilyLookupService>();
            _mockFamilyLookup.Setup(s => s.GetDefault())
                .Returns(new FontFamilyModel { IsSelected = true });

            _mockZoomLookup = new Mock<IFontZoomLookupService>();
            _mockZoomLookup.Setup(s => s.GetDefault())
                .Returns(new FontZoomModel { IsSelected = true });
        }

        [TestMethod]
        public void Open_NullConstructorArgs_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => new ViewStateService(null, _mockFamilyLookup.Object, _mockZoomLookup.Object));
            Assert.ThrowsException<ArgumentNullException>(
                () => new ViewStateService(_mockColourLookup.Object, null, _mockZoomLookup.Object));
            Assert.ThrowsException<ArgumentNullException>(
                () => new ViewStateService(_mockColourLookup.Object, _mockFamilyLookup.Object, null));
        }

        [TestMethod]
        public void Open_FileNotFound_ReturnsWithDefaults()
        {
            var service = CreateService();

            var result = service.Open();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.SelectedFontZoom);
            Assert.IsNotNull(result.SelectedFontColour);
            Assert.IsNotNull(result.SelectedFontFamily);
        }

        [TestMethod]
        public void Open_CorruptJson_ReturnsWithDefaults()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, "this is not valid json {{{");
                var service = CreateServiceWithFile(tempFile);

                var result = service.Open();

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.SelectedFontZoom);
                Assert.IsNotNull(result.SelectedFontColour);
                Assert.IsNotNull(result.SelectedFontFamily);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void Open_EmptyFile_ReturnsWithDefaults()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                File.WriteAllText(tempFile, "");
                var service = CreateServiceWithFile(tempFile);

                var result = service.Open();

                Assert.IsNotNull(result);
                Assert.IsNotNull(result.SelectedFontZoom);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void Open_NullSelectedFontZoom_FillsWithDefault()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                var json = "{\"SelectedFontZoom\":null,\"SelectedFontColour\":{\"IsSelected\":true},\"SelectedFontFamily\":{\"IsSelected\":true}}";
                File.WriteAllText(tempFile, json);
                var service = CreateServiceWithFile(tempFile);

                var result = service.Open();

                Assert.IsNotNull(result.SelectedFontZoom);
                Assert.IsTrue(result.SelectedFontZoom.IsSelected);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void Open_NullSelectedFontColour_FillsWithDefault()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                var json = "{\"SelectedFontZoom\":{\"IsSelected\":true},\"SelectedFontColour\":null,\"SelectedFontFamily\":{\"IsSelected\":true}}";
                File.WriteAllText(tempFile, json);
                var service = CreateServiceWithFile(tempFile);

                var result = service.Open();

                Assert.IsNotNull(result.SelectedFontColour);
                Assert.IsTrue(result.SelectedFontColour.IsSelected);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void Open_NullSelectedFontFamily_FillsWithDefault()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                var json = "{\"SelectedFontZoom\":{\"IsSelected\":true},\"SelectedFontColour\":{\"IsSelected\":true},\"SelectedFontFamily\":null}";
                File.WriteAllText(tempFile, json);
                var service = CreateServiceWithFile(tempFile);

                var result = service.Open();

                Assert.IsNotNull(result.SelectedFontFamily);
                Assert.IsTrue(result.SelectedFontFamily.IsSelected);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void Open_InvalidThemeMode_ResetsToDark()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                var json = "{\"SelectedFontZoom\":{\"IsSelected\":true},\"SelectedFontColour\":{\"IsSelected\":true},\"SelectedFontFamily\":{\"IsSelected\":true},\"SelectedThemeMode\":999}";
                File.WriteAllText(tempFile, json);
                var service = CreateServiceWithFile(tempFile);

                var result = service.Open();

                Assert.AreEqual(ThemeMode.Dark, result.SelectedThemeMode);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void Open_ValidJson_ReturnsDeserializedState()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                var json = "{\"SelectedFontZoom\":{\"IsSelected\":true,\"Key\":150},\"SelectedFontColour\":{\"IsSelected\":true},\"SelectedFontFamily\":{\"IsSelected\":true},\"SelectedThemeMode\":1,\"IsWrapped\":true}";
                File.WriteAllText(tempFile, json);
                var service = CreateServiceWithFile(tempFile);

                var result = service.Open();

                Assert.IsTrue(result.IsWrapped);
                Assert.AreEqual(150, result.SelectedFontZoom.Key);
                Assert.AreEqual(ThemeMode.Light, result.SelectedThemeMode);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void Open_AllFieldsNull_ReturnsFullyDefaultedState()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                var json = "{\"SelectedFontZoom\":null,\"SelectedFontColour\":null,\"SelectedFontFamily\":null,\"SelectedThemeMode\":0}";
                File.WriteAllText(tempFile, json);
                var service = CreateServiceWithFile(tempFile);

                var result = service.Open();

                Assert.IsNotNull(result.SelectedFontZoom);
                Assert.IsNotNull(result.SelectedFontColour);
                Assert.IsNotNull(result.SelectedFontFamily);
                Assert.AreEqual(ThemeMode.Dark, result.SelectedThemeMode);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void Save_WritesJsonToFile()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                File.Delete(tempFile);
                var service = CreateServiceWithFile(tempFile);
                var state = new ViewStateModel(
                    _mockColourLookup.Object.GetDefault(),
                    _mockFamilyLookup.Object.GetDefault(),
                    _mockZoomLookup.Object.GetDefault());

                service.Save(state);

                Assert.IsTrue(File.Exists(tempFile));
                var savedJson = File.ReadAllText(tempFile);
                Assert.IsTrue(savedJson.Contains("SelectedFontZoom"));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        [TestMethod]
        public void Save_OpenRoundTrip_PreservesState()
        {
            var tempFile = Path.GetTempFileName();
            try
            {
                File.Delete(tempFile);
                var service = CreateServiceWithFile(tempFile);
                var state = new ViewStateModel(
                    _mockColourLookup.Object.GetDefault(),
                    _mockFamilyLookup.Object.GetDefault(),
                    _mockZoomLookup.Object.GetDefault());
                state.IsWrapped = true;
                state.SelectedThemeMode = ThemeMode.Light;

                service.Save(state);
                var loaded = service.Open();

                Assert.AreEqual(ThemeMode.Light, loaded.SelectedThemeMode);
                Assert.IsTrue(loaded.IsWrapped);
            }
            finally
            {
                File.Delete(tempFile);
                File.Delete(tempFile + ".tmp");
            }
        }

        private ViewStateService CreateService()
        {
            return new ViewStateService(
                _mockColourLookup.Object,
                _mockFamilyLookup.Object,
                _mockZoomLookup.Object);
        }

        private ViewStateService CreateServiceWithFile(string filePath)
        {
            var service = new ViewStateService(
                _mockColourLookup.Object,
                _mockFamilyLookup.Object,
                _mockZoomLookup.Object);

            var field = typeof(ViewStateService).GetField(
                "_fileLocation",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(service, filePath);

            return service;
        }
    }
}
