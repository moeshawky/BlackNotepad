using Savaged.BlackNotepad.Lookups;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Savaged.BlackNotepad.Converters
{
    public class LineEndingEnumToDisplayNameConverter : IValueConverter
    {
        /// <summary>
        /// Converts a LineEndings enum value to a display string.
        /// </summary>
        /// <param name="value">The LineEndings value to convert. Must be a LineEndings enum instance.</param>
        /// <param name="targetType">Target type. Expected: typeof(string).</param>
        /// <param name="parameter">Unused. Pass null.</param>
        /// <param name="culture">Culture for formatting. Currently unused.</param>
        /// <returns>
        /// "Windows (CRLF)" for CRLF, "Unix (LF)" for LF, "Classic Mac (CR)" for CR,
        /// or string.Empty if value is null, not a LineEndings enum, or targetType is not string.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null 
                || value.GetType() != typeof(LineEndings)
                || targetType != typeof(string))
            {
                return string.Empty;
            }
            if (value is LineEndings l)
            {
                if (l == LineEndings.CRLF)
                {
                    return "Windows (CRLF)";
                }
                if (l == LineEndings.LF)
                {
                    return "Unix (LF)";
                }
                if (l == LineEndings.CR)
                {
                    return "Classic Mac (CR)";
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
