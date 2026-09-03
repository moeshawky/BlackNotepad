using Savaged.BlackNotepad.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Savaged.BlackNotepad.Services
{
    public class FontFamilyLookupService :
        LookupServiceBase<FontFamilyModel>, IFontFamilyLookupService
    {
        private const string _default = "Arial Unicode MS";
        private readonly FontFamilyModel _defaultModel;
        private bool _loaded;

        /// <summary>
        /// Seeds only the default entry so construction stays cheap.
        /// Full system font enumeration is deferred to LoadAsync, since
        /// Fonts.SystemFontFamilies touches every installed font and
        /// stalls application startup on the UI thread.
        /// </summary>
        public FontFamilyLookupService() : base()
        {
            _defaultModel = new FontFamilyModel(_default, _default);
            Index.Add(_defaultModel);
        }

        /// <summary>
        /// Enumerates and sorts installed fonts on a pool thread, then adds
        /// them to the Index on the calling (UI) thread. Safe to call once;
        /// repeat calls are no-ops. MainViewModel already re-applies the
        /// persisted font selection as entries arrive.
        /// </summary>
        public async Task LoadAsync()
        {
            if (_loaded)
            {
                return;
            }
            _loaded = true;

            var fonts = await Task.Run(() =>
            {
                var found = new List<FontFamilyModel>();
                foreach (var fontFamily in Fonts.SystemFontFamilies)
                {
                    var name = fontFamily.ToString();
                    if (name != _default)
                    {
                        found.Add(new FontFamilyModel(name, name));
                    }
                }
                return found.OrderBy(f => f.Value).ToList();
            });

            foreach (var font in fonts)
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
