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

        private bool _isLoaded;
        private readonly object _lock = new object();

        public FontFamilyLookupService() : base()
        {
        }

        public override IList<FontFamilyModel> GetIndex()
        {
            if (!_isLoaded)
            {
                lock (_lock)
                {
                    if (!_isLoaded)
                    {
                        foreach (var fontFamily in Fonts.SystemFontFamilies)
                        {
                            var name = fontFamily.ToString();
                            Index.Add(new FontFamilyModel(name, name));
                        }
                        _isLoaded = true;
                    }
                }
            }
            return base.GetIndex();
        }

        public override FontFamilyModel GetDefault()
        {
            if (!_isLoaded)
            {
                // Bolt: Fast path for default to avoid full font scan if not needed
                return new FontFamilyModel(_default, _default);
            }
            var value = Index.Where(f => f.Key == _default).FirstOrDefault();
            return value;
        }
    }
}
