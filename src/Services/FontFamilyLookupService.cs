using Savaged.BlackNotepad.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Savaged.BlackNotepad.Services
{
    public class FontFamilyLookupService :
        LookupServiceBase<FontFamilyModel>, IFontFamilyLookupService
    {
        private const string _default = "Arial Unicode MS";
        private readonly FontFamilyModel _defaultModel;

        /// <summary>
        /// Loads system fonts synchronously into the Index collection.
        /// Fonts.SystemFontFamilies enumerates installed fonts and may be slow
        /// on systems with many fonts installed.
        /// </summary>
        public FontFamilyLookupService() : base()
        {
            _defaultModel = new FontFamilyModel(_default, _default);

            var fonts = new List<FontFamilyModel>();
            foreach (var fontFamily in Fonts.SystemFontFamilies)
            {
                var name = fontFamily.ToString();
                fonts.Add(new FontFamilyModel(name, name));
            }

            foreach (var font in fonts.OrderBy(f => f.Value))
            {
                Index.Add(font);
            }
        }

        /// <summary>
        /// Returns the default FontFamilyModel matching _default key.
        /// Falls back to _defaultModel if the default font is not installed.
        /// </summary>
        /// <returns>The default FontFamilyModel. Never null.</returns>
        public override FontFamilyModel GetDefault()
        {
            var value = Index.FirstOrDefault(f => f.Key == _default);
            return value ?? _defaultModel;
        }
    }
}
