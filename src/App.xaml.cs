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
        private async void OnApplicationStartup(object sender, StartupEventArgs e)
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

            var vm = SimpleIoc.Default.GetInstance<MainViewModel>();
            var mainView = new MainWindow(SimpleIoc.Default.GetInstance<IThemeService>())
            {
                DataContext = vm
            };
            mainView.Show();

            try
            {
                // Deferred until after first paint: enumerating installed
                // fonts stalls startup on the UI thread for little benefit,
                // since the menu fills in as entries arrive.
                await SimpleIoc.Default
                    .GetInstance<IFontFamilyLookupService>()
                    .LoadAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Failed to load system fonts: {ex.Message}");
            }

            if (e.Args.Length > 0 && System.IO.File.Exists(e.Args[0]))
            {
                try
                {
                    await vm.Open(e.Args[0]);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Failed to open startup file: {ex.Message}");
                }
            }
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
