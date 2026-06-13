using Savaged.BlackNotepad.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Savaged.BlackNotepad.Converters
{
    /// <summary>
    /// Converts a ThemeMode enum value to a boolean indicating whether it matches
    /// the parameter theme mode string. Used for menu item check marks.
    /// </summary>
    class ThemeModeConverter : IValueConverter
    {
        /// <summary>
        /// Converts a ThemeMode value to a boolean indicating if it matches the parameter.
        /// </summary>
        /// <param name="value">The ThemeMode value to compare.</param>
        /// <param name="targetType">The target type (bool).</param>
        /// <param name="parameter">The theme mode name string (Dark, Light, or System).</param>
        /// <param name="culture">Culture info.</param>
        /// <returns>True if the ThemeMode matches the parameter; false otherwise.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ThemeMode mode && parameter is string modeName)
            {
                if (Enum.TryParse<ThemeMode>(modeName, out var targetMode))
                {
                    return mode == targetMode;
                }
            }
            return false;
        }

        /// <summary>
        /// Not supported. Throws NotSupportedException.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
