using GalaSoft.MvvmLight.Ioc;
using Microsoft.Win32;
using Savaged.BlackNotepad.Services;
using Savaged.BlackNotepad.ViewModels;
using Savaged.BlackNotepad.Views;
using System;
using System.Windows;
using System.Windows.Threading;

namespace Savaged.BlackNotepad
{
    public partial class App : Application
    {
        private void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            SimpleIoc.Default
                .Register<IFontColourLookupService, FontColourLookupService>();
            SimpleIoc.Default
                .Register<IFontFamilyLookupService, FontFamilyLookupService>();
            SimpleIoc.Default
                .Register<IFontZoomLookupService, FontZoomLookupService>();

            SimpleIoc.Default
                .Register<IViewStateService, ViewStateService>();

            SimpleIoc.Default
                .Register<IFileModelService, FileModelService>();


            SimpleIoc.Default
                .Register<IGoToDialogViewModel, GoToDialogViewModel>();
            SimpleIoc.Default
                .Register<IFindDialogViewModel, FindDialogViewModel>();
            SimpleIoc.Default
                .Register<IReplaceDialogViewModel, ReplaceDialogViewModel>();
            SimpleIoc.Default.Register<OpenFileDialog>();
            SimpleIoc.Default.Register<SaveFileDialog>();

            SimpleIoc.Default
                .Register<IDialogService, DialogService>();

            SimpleIoc.Default
                .Register<IThemeService, ThemeService>();

            SimpleIoc.Default.Register<MainViewModel>();

            var mainView = new MainWindow(SimpleIoc.Default.GetInstance<IThemeService>())
            {
                DataContext = SimpleIoc.Default
                .GetInstance<MainViewModel>()
            };
            mainView.Show();
        }

        private void OnDispatcherUnhandledException(
            object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                $"BlackNotepad encountered an unexpected error " +
                $"and must close.\n\n{e.Exception.Message}\n\n" +
                $"{e.Exception.GetType().Name}",
                "BlackNotepad - Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            e.Handled = true;
            Shutdown();
        }
    }
}
