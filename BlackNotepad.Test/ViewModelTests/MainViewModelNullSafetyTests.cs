using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Savaged.BlackNotepad.Models;
using Savaged.BlackNotepad.Services;
using Savaged.BlackNotepad.ViewModels;
using Savaged.BlackNotepad.ViewsInterfaces;
using System;
using System.Collections.Generic;

namespace BlackNotepad.Test.ViewModelTests
{
    [TestClass]
    public class MainViewModelNullSafetyTests : TestBase
    {
        private IFontColourLookupService _fontColourLookup;
        private IFontFamilyLookupService _fontFamilyLookup;
        private IFontZoomLookupService _fontZoomLookup;

        [TestInitialize]
        public void NullSafetyInit()
        {
            _fontColourLookup = new FontColourLookupService();
            _fontFamilyLookup = new FontFamilyLookupService();
            _fontZoomLookup = new FontZoomLookupService();
        }

        [TestMethod]
        public void OnZoomIn_WithNullSelectedFontZoom_DoesNotThrow()
        {
            var viewStateWithNull = new ViewStateModel(
                _fontColourLookup.GetDefault(),
                _fontFamilyLookup.GetDefault(),
                _fontZoomLookup.GetDefault());
            viewStateWithNull.SelectedFontZoom = null;

            var mockViewStateService = new Mock<IViewStateService>();
            mockViewStateService.Setup(s => s.Open()).Returns(viewStateWithNull);

            var vm = CreateMainViewModel(viewStateService: mockViewStateService.Object);

            try
            {
                vm.ZoomInCmd.Execute(null);
            }
            catch (NullReferenceException)
            {
                Assert.Fail("OnZoomIn threw NullReferenceException when SelectedFontZoom was null");
            }
        }

        [TestMethod]
        public void OnZoomOut_WithNullSelectedFontZoom_DoesNotThrow()
        {
            var viewStateWithNull = new ViewStateModel(
                _fontColourLookup.GetDefault(),
                _fontFamilyLookup.GetDefault(),
                _fontZoomLookup.GetDefault());
            viewStateWithNull.SelectedFontZoom = null;

            var mockViewStateService = new Mock<IViewStateService>();
            mockViewStateService.Setup(s => s.Open()).Returns(viewStateWithNull);

            var vm = CreateMainViewModel(viewStateService: mockViewStateService.Object);

            try
            {
                vm.ZoomOutCmd.Execute(null);
            }
            catch (NullReferenceException)
            {
                Assert.Fail("OnZoomOut threw NullReferenceException when SelectedFontZoom was null");
            }
        }

        [TestMethod]
        public void OnZoomIn_WithValidState_ChangesZoomLevel()
        {
            var vm = CreateMainViewModel();
            var initialZoom = vm.ViewState.SelectedFontZoom.Key;

            vm.ZoomInCmd.Execute(null);

            Assert.AreNotEqual(initialZoom, vm.ViewState.SelectedFontZoom.Key,
                "Zoom level should change after ZoomIn");
        }

        [TestMethod]
        public void OnZoomOut_WithValidState_ChangesZoomLevel()
        {
            var vm = CreateMainViewModel();
            vm.ZoomInCmd.Execute(null);
            var zoomedIn = vm.ViewState.SelectedFontZoom.Key;

            vm.ZoomOutCmd.Execute(null);

            Assert.AreNotEqual(zoomedIn, vm.ViewState.SelectedFontZoom.Key,
                "Zoom level should change after ZoomOut");
        }

        [TestMethod]
        public void RestoreDefaultZoom_WithNullState_RestoresToDefault()
        {
            var viewStateWithNull = new ViewStateModel(
                _fontColourLookup.GetDefault(),
                _fontFamilyLookup.GetDefault(),
                _fontZoomLookup.GetDefault());
            viewStateWithNull.SelectedFontZoom = null;

            var mockViewStateService = new Mock<IViewStateService>();
            mockViewStateService.Setup(s => s.Open()).Returns(viewStateWithNull);

            var vm = CreateMainViewModel(viewStateService: mockViewStateService.Object);

            vm.RestoreDefaultZoomCmd.Execute(null);

            Assert.IsNotNull(vm.ViewState.SelectedFontZoom);
            Assert.AreEqual(vm.ViewState.SelectedFontZoom.Key, 100,
                "Default zoom should be 100");
        }

        [TestMethod]
        public void ViewState_AlwaysHasNonNullFontZoom_AfterConstruction()
        {
            Assert.IsNotNull(MainVm.ViewState.SelectedFontZoom);
            Assert.IsNotNull(MainVm.ViewState.SelectedFontColour);
            Assert.IsNotNull(MainVm.ViewState.SelectedFontFamily);
        }

        private MainViewModel CreateMainViewModel(
            IViewStateService viewStateService = null,
            IFileModelService fileModelService = null,
            IDialogService dialogService = null,
            IThemeService themeService = null)
        {
            IViewStateService vsService;
            if (viewStateService != null)
            {
                vsService = viewStateService;
            }
            else
            {
                var mockVs = new Mock<IViewStateService>();
                mockVs.Setup(s => s.Open())
                    .Returns(new ViewStateModel(
                        _fontColourLookup.GetDefault(),
                        _fontFamilyLookup.GetDefault(),
                        _fontZoomLookup.GetDefault()));
                vsService = mockVs.Object;
            }

            var mockFmService = new Mock<IFileModelService>();
            mockFmService.Setup(s => s.LoadAsync(It.IsAny<string>()))
                .Returns(System.Threading.Tasks.Task.FromResult(new FileModel()));
            mockFmService.Setup(s => s.SaveAsync(It.IsAny<FileModel>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            var mockDlgService = new Mock<IDialogService>();
            mockDlgService.Setup(s => s.GetFileDialog<OpenFileDialog>())
                .Returns(new Microsoft.Win32.OpenFileDialog());
            mockDlgService.Setup(s => s.GetFileDialog<SaveFileDialog>())
                .Returns(new Microsoft.Win32.SaveFileDialog());
            mockDlgService.Setup(s => s.GetDialogViewModel<IGoToDialogViewModel>())
                .Returns(new Mock<IGoToDialogViewModel>().Object);
            mockDlgService.Setup(s => s.GetDialogViewModel<IFindDialogViewModel>())
                .Returns(new FindDialogViewModel());
            mockDlgService.Setup(s => s.GetDialogViewModel<IReplaceDialogViewModel>())
                .Returns(new ReplaceDialogViewModel());
            mockDlgService.Setup(s => s.ShowDialog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns((bool?)true);
            mockDlgService.Setup(s => s.Show(It.IsAny<IDialogViewModel>()));
            mockDlgService.Setup(s => s.ShowDialog(It.IsAny<IDialogViewModel>()))
                .Returns(true);

            var dlgService = dialogService ?? mockDlgService.Object;
            var thService = themeService ?? new Mock<IThemeService>().Object;
            var fmService = fileModelService ?? mockFmService.Object;

            return new MainViewModel(
                dlgService,
                vsService,
                thService,
                _fontColourLookup,
                _fontFamilyLookup,
                _fontZoomLookup,
                fmService);
        }
    }
}
