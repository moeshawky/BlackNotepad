using GalaSoft.MvvmLight;

namespace Savaged.BlackNotepad.Models
{
    public class ViewStateModel : ObservableObject
    {
        private bool _isWrapped;
        private bool _isStatusBarVisible;
        private FontZoomModel _selectedFontZoom = new FontZoomModel { IsSelected = true };
        private FontColourModel _selectedFontColour = new FontColourModel { IsSelected = true };
        private FontFamilyModel _selectedFontFamily = new FontFamilyModel { IsSelected = true };

        /// <summary>
        /// Parameterless constructor for JSON deserialization.
        /// Initializes fields to safe defaults so properties are never null
        /// when the parameterized constructor is bypassed by Newtonsoft.Json.
        /// </summary>
        public ViewStateModel() { }

        /// <summary>
        /// Creates a ViewStateModel with explicit defaults for font colour, family, and zoom.
        /// </summary>
        /// <param name="defaultFontColour">Default font colour model. If null, a new FontColourModel is used.</param>
        /// <param name="defaultFontFamily">Default font family model. If null, a new FontFamilyModel is used.</param>
        /// <param name="defaultFontZoom">Default font zoom model. If null, a new FontZoomModel is used.</param>
        public ViewStateModel(
            FontColourModel defaultFontColour,
            FontFamilyModel defaultFontFamily,
            FontZoomModel defaultFontZoom)
        {
            SelectedFontColour = 
                defaultFontColour ?? new FontColourModel { IsSelected = true };
            SelectedFontFamily = 
                defaultFontFamily ?? new FontFamilyModel { IsSelected = true };
            SelectedFontZoom = 
                defaultFontZoom ?? new FontZoomModel { IsSelected = true };
        }

        public bool IsWrapped
        {
            get => _isWrapped;
            set => Set(ref _isWrapped, value);
        }

        public bool IsStatusBarVisible
        {
            get => _isStatusBarVisible;
            set => Set(ref _isStatusBarVisible, value);
        }

        public FontZoomModel SelectedFontZoom
        {
            get => _selectedFontZoom;
            set
            {
                Set(ref _selectedFontZoom, value);
            }
        }          

        public FontColourModel SelectedFontColour
        {
            get => _selectedFontColour;
            set
            {
                value.IsSelected = true;
                Set(ref _selectedFontColour, value);
            }
        }

        public FontFamilyModel SelectedFontFamily
        {
            get => _selectedFontFamily;
            set
            {
                value.IsSelected = true;
                Set(ref _selectedFontFamily, value);
            }
        }
    }
}
