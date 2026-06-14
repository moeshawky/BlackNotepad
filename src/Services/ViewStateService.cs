using Newtonsoft.Json;
using Savaged.BlackNotepad.Models;
using System;
using System.IO;

namespace Savaged.BlackNotepad.Services
{
    public class ViewStateService : IViewStateService
    {
        private readonly string _fileLocation;
        private readonly IFontColourLookupService _fontColourLookupService;
        private readonly IFontFamilyLookupService _fontFamilyLookupService;
        private readonly IFontZoomLookupService _fontZoomLookupService;

        public ViewStateService(
            IFontColourLookupService fontColourLookupService,
            IFontFamilyLookupService fontFamilyLookupService,
            IFontZoomLookupService fontZoomLookupService)
        {
            if (fontColourLookupService is null)
            {
                throw new ArgumentNullException(nameof(fontColourLookupService));
            }
            if (fontFamilyLookupService is null)
            {
                throw new ArgumentNullException(nameof(fontFamilyLookupService));
            }
            if (fontZoomLookupService is null)
            {
                throw new ArgumentNullException(nameof(fontZoomLookupService));
            }
            _fontColourLookupService = fontColourLookupService;
            _fontFamilyLookupService = fontFamilyLookupService;
            _fontZoomLookupService = fontZoomLookupService;

            var localAppData = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData);
            _fileLocation = 
                $"{localAppData}\\BlackNotepad.ViewState.json";
        }

        /// <summary>
        /// Loads the persisted view state from the JSON file on disk.
        /// If the file is missing, corrupt, or contains null fields,
        /// returns a ViewStateModel initialized with service defaults.
        /// </summary>
        /// <returns>A non-null ViewStateModel with all properties populated.
        /// Never returns null or a model with null font properties.</returns>
        /// <remarks>
        /// The fallback defaults below are EMERGENCY RECOVERY values, not configuration.
        /// When the state file is missing, contains invalid JSON, or has null font properties,
        /// these safe defaults ensure the application remains usable. The user's previous
        /// preferences are lost in this case, but the app will not crash. On the next Save(),
        /// the recovery defaults will be persisted, overwriting any corrupt state file.
        /// </remarks>
        public ViewStateModel Open()
        {
            ViewStateModel value;
            var fileInfo = new FileInfo(_fileLocation);
            if (fileInfo.Exists)
            {
                try
                {
                    value = JsonConvert.DeserializeObject<ViewStateModel>(
                        File.ReadAllText(_fileLocation));
                }
                catch (JsonException)
                {
                    value = null;
                }
            }
            else
            {
                value = new ViewStateModel(
                    _fontColourLookupService.GetDefault(),
                    _fontFamilyLookupService.GetDefault(),
                    _fontZoomLookupService.GetDefault());
            }
            if (value is null)
            {
                // EMERGENCY RECOVERY: Deserialized to null (corrupt or empty JSON).
                // These defaults restore the app to a usable state. User preferences
                // are lost; on next Save(), these recovery values will be persisted.
                value = new ViewStateModel(
                    _fontColourLookupService.GetDefault(),
                    _fontFamilyLookupService.GetDefault(),
                    _fontZoomLookupService.GetDefault());
                return value;
            }
            // EMERGENCY RECOVERY: Individual null fields below indicate partial
            // deserialization (e.g., JSON missing a key). These stub defaults keep
            // the app alive; user will see generic font settings until next Save().
            if (value.SelectedFontZoom is null)
            {
                value.SelectedFontZoom = new FontZoomModel
                    { IsSelected = true };
            }
            if (value.SelectedFontColour is null)
            {
                value.SelectedFontColour = new FontColourModel
                    { IsSelected = true };
            }
            if (value.SelectedFontFamily is null)
            {
                value.SelectedFontFamily = new FontFamilyModel
                    { IsSelected = true };
            }
            // EMERGENCY RECOVERY: Invalid ThemeMode value (e.g., from a future version
            // or manual edit of the JSON). Fall back to Dark theme.
            if (!Enum.IsDefined(typeof(ThemeMode), value.SelectedThemeMode))
            {
                value.SelectedThemeMode = ThemeMode.Dark;
            }
            return value;
        }

        public void Save(ViewStateModel viewState)
        {
            var json = JsonConvert.SerializeObject(viewState);

            var tempLocation = _fileLocation + ".tmp";
            File.WriteAllText(tempLocation, json);

            File.Move(tempLocation, _fileLocation);
        }
    }
}
