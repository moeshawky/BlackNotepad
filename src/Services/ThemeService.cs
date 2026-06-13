using Savaged.BlackNotepad.Models;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Savaged.BlackNotepad.Services
{
    /// <summary>
    /// Theme management service that handles switching between Dark, Light, and System themes.
    /// Manages application-level resource dictionaries for theme colors.
    /// </summary>
    public class ThemeService : IThemeService
    {
        private const string DarkThemeKey = "DarkThemeBrush";
        private const string LightThemeKey = "LightThemeBrush";

        /// <summary>
        /// Applies the specified theme to the application.
        /// </summary>
        /// <param name="mode">The theme mode to apply.</param>
        public void ApplyTheme(ThemeMode mode)
        {
            var effectiveMode = mode == ThemeMode.System ? GetEffectiveTheme() : mode;
            ApplyThemeResources(effectiveMode);
        }

        /// <summary>
        /// Gets the effective theme mode, resolving System to Dark or Light
        /// based on the current Windows theme setting.
        /// </summary>
        /// <returns>The resolved ThemeMode (Dark or Light).</returns>
        public ThemeMode GetEffectiveTheme()
        {
            try
            {
                var key = Microsoft.Win32.Registry.GetValue(
                    @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                    "AppsUseLightTheme",
                    null);

                if (key is int value)
                {
                    return value == 0 ? ThemeMode.Dark : ThemeMode.Light;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to read system theme: {ex.Message}");
            }

            return ThemeMode.Dark;
        }

        /// <summary>
        /// Applies theme resources to the application based on the effective theme mode.
        /// </summary>
        /// <param name="mode">The effective theme mode (Dark or Light).</param>
        private void ApplyThemeResources(ThemeMode mode)
        {
            var app = Application.Current;
            if (app == null) return;

            var mergedDicts = app.Resources.MergedDictionaries;
            if (mergedDicts == null || mergedDicts.Count == 0) return;

            var resourceDict = mergedDicts[0];

            if (mode == ThemeMode.Dark)
            {
                resourceDict["MainBackgroundBrush"] = new SolidColorBrush(Colors.Black);
                resourceDict["StandOutBackgroundBrush"] = new SolidColorBrush(Colors.DarkGray);
                resourceDict["MainBorderBrush"] = new SolidColorBrush(Colors.SlateGray);
                resourceDict["MenuBackgroundBrush"] = new SolidColorBrush(Colors.DarkGray);
                resourceDict["TextForegroundBrush"] = new SolidColorBrush(Colors.White);
            }
            else
            {
                resourceDict["MainBackgroundBrush"] = new SolidColorBrush(Colors.White);
                resourceDict["StandOutBackgroundBrush"] = new SolidColorBrush(Colors.LightGray);
                resourceDict["MainBorderBrush"] = new SolidColorBrush(Colors.Gray);
                resourceDict["MenuBackgroundBrush"] = new SolidColorBrush(Colors.LightGray);
                resourceDict["TextForegroundBrush"] = new SolidColorBrush(Colors.Black);
            }
        }
    }
}
