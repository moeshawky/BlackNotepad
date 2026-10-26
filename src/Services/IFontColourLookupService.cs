using Savaged.BlackNotepad.Models;
using System.Collections.Generic;

namespace Savaged.BlackNotepad.Services
{
    /// <summary>
    /// Service for querying available font colors, default color, and theme-adapted counterpart colors.
    /// </summary>
    public interface IFontColourLookupService
    {
        /// <summary>
        /// Gets the default font color model.
        /// </summary>
        /// <returns>Default font color model.</returns>
        FontColourModel GetDefault();

        /// <summary>
        /// Gets the default font color model for the specified theme mode.
        /// </summary>
        /// <param name="mode">The effective theme mode.</param>
        /// <returns>Default font color model suited for the theme.</returns>
        FontColourModel GetDefault(ThemeMode mode);

        /// <summary>
        /// Gets the full index of available font colors.
        /// </summary>
        /// <returns>List of font color models.</returns>
        IList<FontColourModel> GetIndex();

        /// <summary>
        /// Gets a tactful counterpart font color model for the specified theme mode,
        /// preserving color family aesthetics while ensuring high contrast and legibility.
        /// </summary>
        /// <param name="current">The current font color model.</param>
        /// <param name="targetMode">The target effective theme mode (Dark or Light).</param>
        /// <returns>The adapted font color model.</returns>
        FontColourModel GetCounterpart(FontColourModel current, ThemeMode targetMode);
    }
}
