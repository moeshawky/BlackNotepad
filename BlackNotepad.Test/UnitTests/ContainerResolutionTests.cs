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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using Savaged.BlackNotepad.Services;
using Savaged.BlackNotepad.ViewModels;
using System.Reflection;

namespace BlackNotepad.Test.UnitTests
{
    /// <summary>
    /// Integration tests verifying SimpleIoc registration and resolution of all services
    /// and ViewModels matching production configuration in <c>App.xaml.cs</c>.
    /// </summary>
    [TestClass]
    public class ContainerResolutionTests
    {
        /// <summary>
        /// Resets and configures the SimpleIoc container with all production registrations.
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            ResetAndRegisterContainer();
        }

        /// <summary>
        /// Cleans up and resets the SimpleIoc container after each test execution.
        /// </summary>
        [TestCleanup]
        public void TearDown()
        {
            SimpleIoc.Default.Reset();
        }

        /// <summary>
        /// Configures the SimpleIoc container matching the exact registration sequence in App.xaml.cs.
        /// </summary>
        private static void ResetAndRegisterContainer()
        {
            SimpleIoc.Default.Reset();

            SimpleIoc.Default
                .Register<IFontColourLookupService, FontColourLookupService>();
            SimpleIoc.Default
                .Register<IFontFamilyLookupService, FontFamilyLookupService>();
            SimpleIoc.Default
                .Register<IFontZoomLookupService, FontZoomLookupService>();

            SimpleIoc.Default
                .Register<IViewStateService, ViewStateService>();

            SimpleIoc.Default
                .Register<IFileHistoryService, FileHistoryService>();

            SimpleIoc.Default
                .Register<IFileModelService, FileModelService>();

            SimpleIoc.Default
                .Register<IGoToDialogViewModel, GoToDialogViewModel>();
            SimpleIoc.Default
                .Register<IFindDialogViewModel, FindDialogViewModel>();
            SimpleIoc.Default
                .Register<IReplaceDialogViewModel, ReplaceDialogViewModel>();
            SimpleIoc.Default
                .Register<IHistoryDialogViewModel, HistoryDialogViewModel>();
            SimpleIoc.Default.Register<OpenFileDialog>();
            SimpleIoc.Default.Register<SaveFileDialog>();

            SimpleIoc.Default
                .Register<IDialogService, DialogService>();

            SimpleIoc.Default
                .Register<IThemeService, ThemeService>();

            SimpleIoc.Default.Register<MainViewModel>();
        }

        /// <summary>
        /// Verifies that all services and ViewModels registered in App.xaml.cs resolve without exception.
        /// </summary>
        [TestMethod]
        public void Resolve_AllRegisteredServicesAndViewModels_ResolvesSuccessfully()
        {
            Assert.IsNotNull(SimpleIoc.Default.GetInstance<IFontColourLookupService>());
            Assert.IsNotNull(SimpleIoc.Default.GetInstance<IFontFamilyLookupService>());
            Assert.IsNotNull(SimpleIoc.Default.GetInstance<IFontZoomLookupService>());
            Assert.IsNotNull(SimpleIoc.Default.GetInstance<IViewStateService>());
            Assert.IsNotNull(SimpleIoc.Default.GetInstance<IFileHistoryService>());
            Assert.IsNotNull(SimpleIoc.Default.GetInstance<IFileModelService>());
            Assert.IsNotNull(SimpleIoc.Default.GetInstance<IGoToDialogViewModel>());
            Assert.IsNotNull(SimpleIoc.Default.GetInstance<IFindDialogViewModel>());
            Assert.IsNotNull(SimpleIoc.Default.GetInstance<IReplaceDialogViewModel>());
            Assert.IsNotNull(SimpleIoc.Default.GetInstance<IHistoryDialogViewModel>());
            Assert.IsNotNull(SimpleIoc.Default.GetInstance<IDialogService>());
            Assert.IsNotNull(SimpleIoc.Default.GetInstance<IThemeService>());
            Assert.IsNotNull(SimpleIoc.Default.GetInstance<MainViewModel>());
        }

        /// <summary>
        /// Verifies that the resolved IFileModelService has an active IFileHistoryService instance injected,
        /// ensuring C5 document history recording capability is preserved in production.
        /// </summary>
        [TestMethod]
        public void Resolve_FileModelService_HasActiveHistoryService()
        {
            var fileModelService = SimpleIoc.Default.GetInstance<IFileModelService>();
            Assert.IsNotNull(fileModelService);
            Assert.IsInstanceOfType(fileModelService, typeof(FileModelService));

            var historyField = typeof(FileModelService).GetField("_historyService", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(historyField, "_historyService field should exist on FileModelService");

            var historyService = historyField.GetValue(fileModelService);
            Assert.IsNotNull(historyService, "FileModelService must have an active IFileHistoryService injected by SimpleIoc");
            Assert.IsInstanceOfType(historyService, typeof(FileHistoryService));

            var resolvedHistory = SimpleIoc.Default.GetInstance<IFileHistoryService>();
            Assert.AreSame(resolvedHistory, historyService, "Resolved IFileHistoryService should match the instance injected into FileModelService");
        }

        /// <summary>
        /// Verifies that IFileHistoryService resolves directly using the parameterless preferred constructor.
        /// </summary>
        [TestMethod]
        public void Resolve_FileHistoryService_ResolvesWithDefaultConstructor()
        {
            var historyService = SimpleIoc.Default.GetInstance<IFileHistoryService>();
            Assert.IsNotNull(historyService);
            Assert.IsInstanceOfType(historyService, typeof(FileHistoryService));
        }

        /// <summary>
        /// Verifies that IHistoryDialogViewModel resolves and receives the active IFileHistoryService.
        /// </summary>
        [TestMethod]
        public void Resolve_HistoryDialogViewModel_InjectsActiveHistoryService()
        {
            var historyDialogVm = SimpleIoc.Default.GetInstance<IHistoryDialogViewModel>();
            Assert.IsNotNull(historyDialogVm);
            Assert.IsInstanceOfType(historyDialogVm, typeof(HistoryDialogViewModel));

            var historyField = typeof(HistoryDialogViewModel).GetField("_historyService", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(historyField, "_historyService field should exist on HistoryDialogViewModel");

            var historyService = historyField.GetValue(historyDialogVm);
            Assert.IsNotNull(historyService, "HistoryDialogViewModel must have an active IFileHistoryService injected by SimpleIoc");
            Assert.IsInstanceOfType(historyService, typeof(FileHistoryService));

            var resolvedHistory = SimpleIoc.Default.GetInstance<IFileHistoryService>();
            Assert.AreSame(resolvedHistory, historyService, "Resolved IFileHistoryService should match the instance injected into HistoryDialogViewModel");
        }

        /// <summary>
        /// Verifies that MainViewModel resolves through SimpleIoc with all child dependencies injected.
        /// </summary>
        [TestMethod]
        public void Resolve_MainViewModel_ResolvesWithAllDependenciesInjected()
        {
            var vm = SimpleIoc.Default.GetInstance<MainViewModel>();
            Assert.IsNotNull(vm);
            Assert.IsNotNull(vm.ViewState);
        }
    }
}
