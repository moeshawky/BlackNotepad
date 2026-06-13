using Savaged.BlackNotepad.Models;

namespace Savaged.BlackNotepad.Services
{
    /// <summary>
    /// Interface for theme management service.
    /// Handles switching between Dark, Light, and System themes.
    /// </summary>
    public interface IThemeService
    {
        /// <summary>
        /// Applies the specified theme to the application.
        /// </summary>
        /// <param name="mode">The theme mode to apply.</param>
        void ApplyTheme(ThemeMode mode);

        /// <summary>
        /// Gets the effective theme mode, resolving System to Dark or Light
        /// based on the current Windows theme setting.
        /// </summary>
        /// <returns>The resolved ThemeMode (Dark or Light).</returns>
        ThemeMode GetEffectiveTheme();
    }
}
