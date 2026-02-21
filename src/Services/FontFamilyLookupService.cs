using Savaged.BlackNotepad.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Savaged.BlackNotepad.Services
{
    public class FontFamilyLookupService :
        LookupServiceBase<FontFamilyModel>, IFontFamilyLookupService
    {
        private const string _default = "Arial Unicode MS";
        private readonly FontFamilyModel _defaultModel;

        public FontFamilyLookupService() : base()
        {
            _defaultModel = new FontFamilyModel(_default, _default);

            // Bolt: Optimized startup performance by loading system fonts asynchronously.
            // This prevents the UI thread from blocking during application launch.
            Task.Run(() =>
            {
                var fonts = new List<FontFamilyModel>();
                // Fonts.SystemFontFamilies can be slow as it enumerates installed fonts
                foreach (var fontFamily in Fonts.SystemFontFamilies)
                {
                    var name = fontFamily.ToString();
                    fonts.Add(new FontFamilyModel(name, name));
                }

                // Marshal to UI thread to update the ObservableCollection
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    // Sort alphabetically for better UX
                    foreach (var font in fonts.OrderBy(f => f.Value))
                    {
                        Index.Add(font);
                    }
                });
            });
        }

        public override FontFamilyModel GetDefault()
        {
            var value = Index.FirstOrDefault(f => f.Key == _default);
            return value ?? _defaultModel;
        }
    }
}
