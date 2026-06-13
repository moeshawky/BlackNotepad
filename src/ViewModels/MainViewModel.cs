using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using Newtonsoft.Json;
using Savaged.BlackNotepad.Extensions;
using Savaged.BlackNotepad.Lookups;
using Savaged.BlackNotepad.Models;
using Savaged.BlackNotepad.Services;
using Savaged.BlackNotepad.ViewsInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Deployment.Application;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Savaged.BlackNotepad.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly IFileModelService _fileModelService;
        private readonly IThemeService _themeService;
        private readonly IList<string> _busyRegister;
        private readonly OpenFileDialog _openFileDialog;
        private readonly SaveFileDialog _saveFileDialog;
        private readonly IViewStateService _viewStateService;
        private readonly IList<FontZoomModel> _fontZoomIndex;
        private readonly int _defaultZoom;
        private readonly IFindDialogViewModel _findDialog;
        private readonly IReplaceDialogViewModel _replaceDialog;
        private readonly DispatcherTimer _autoSaveTimer;
        private FileModel _selectedItem;
        private string _selectedText;
        private string _textSought;
        private bool _isFontZoomMin;
        private bool _isFontZoomMax;
        private int _caretLine;
        private int _caretColumn;
        private int _indexOfCaret;
        private int _findNextCount;
        private bool _isFindWrapAround;
        private bool _isFindMatchCase;
        private bool _isReadyForReplacement;

        public MainViewModel(
            IDialogService dialogService,
            IViewStateService viewStateService,
            IThemeService themeService,
            IFontColourLookupService fontColourLookupService,
            IFontFamilyLookupService fontFamilyLookupService,
            IFontZoomLookupService fontZoomLookupService,
            IFileModelService fileModelService)
        {
            if (dialogService is null)
            {
                throw new ArgumentNullException(nameof(dialogService));
            }
            if (viewStateService is null)
            {
                throw new ArgumentNullException(nameof(viewStateService));
            }
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
            if (fileModelService is null)
            {
                throw new ArgumentNullException(nameof(fileModelService));
            }

            _fileModelService = fileModelService;

            _dialogService = dialogService;

            _themeService = themeService;

            const string filter = "Text Documents|*.txt|All files (*.*)|*.*";
            _openFileDialog = _dialogService.GetFileDialog<OpenFileDialog>();
            if (_openFileDialog != null)
            {
                _openFileDialog.Filter = filter;
            }

            _saveFileDialog = _dialogService.GetFileDialog<SaveFileDialog>();
            if (_saveFileDialog != null)
            {
                _saveFileDialog.Filter = filter;
            }

            _viewStateService = viewStateService;
            ViewState = _viewStateService.Open();

            FontColours = fontColourLookupService.GetIndex();
            ApplySelectedOnFontColour();

            FontFamilyNames = fontFamilyLookupService.GetIndex();
            if (FontFamilyNames is INotifyCollectionChanged observable)
            {
                observable.CollectionChanged += (s, e) => ApplySelectedOnFontFamily();
            }
            ApplySelectedOnFontFamily();

            _fontZoomIndex = fontZoomLookupService.GetIndex();
            _defaultZoom = fontZoomLookupService.GetDefault().Zoom;
            _isFontZoomMin =
                ViewState.SelectedFontZoom.Key == _fontZoomIndex.First().Key;
            _isFontZoomMax =
                ViewState.SelectedFontZoom.Key == _fontZoomIndex.Last().Key;

            _busyRegister = new List<string>();

            SelectedItem = new FileModel();

            NewCmd = new RelayCommand(OnNew, () => CanExecute);
            OpenCmd = new RelayCommand(OnOpen, () => CanExecuteOpen);
            SaveCmd = new RelayCommand(OnSave, () => CanExecute);
            SaveAsCmd = new RelayCommand(OnSaveAs, () => CanExecute);
            ExitCmd = new RelayCommand(OnExit, () => CanExecute);
            OpenRecentCmd = new RelayCommand<string>(OnOpenRecent, (s) => CanExecute);
            FindCmd = new RelayCommand(OnFind, () => CanExecuteFind);
            FindNextCmd = new RelayCommand(OnFindNext, () => CanExecuteFindNext);
            EscCmd = new RelayCommand(OnEsc, () => CanExecuteEsc);
            ReplaceCmd = new RelayCommand(OnReplace, () => CanExecuteReplace);
            GoToCmd = new RelayCommand(OnGoTo, () => CanExecute);
            TimeDateCmd = new RelayCommand(OnTimeDate, () => CanExecute);
            WordWrapCmd = new RelayCommand(OnWordWrap, () => CanExecute);
            ZoomInCmd = new RelayCommand(OnZoomIn, () => CanExecuteZoomIn);
            ZoomOutCmd = new RelayCommand(OnZoomOut, () => CanExecuteZoomOut);
            RestoreDefaultZoomCmd = new RelayCommand(
                OnRestoreDefaultZoom, () => CanExecute);
            StatusBarCmd = new RelayCommand(OnStatusBar, () => CanExecute);
            HelpCmd = new RelayCommand(OnHelp, () => CanExecute);
            AboutCmd = new RelayCommand(OnAbout, () => CanExecute);
            FontColourCmd = new RelayCommand<FontColourModel>(
                OnFontColour, (b) => CanExecute);
            FontFamilyCmd = new RelayCommand<FontFamilyModel>(
                OnFontFamily, (b) => CanExecute);
            PrettifyJsonCmd = new RelayCommand(
                OnPrettifyJson, () => CanExecutePrettifyJson);
            PrintPreviewCmd = new RelayCommand(OnPrintPreview, () => CanExecute);
            ThemeModeCmd = new RelayCommand<ThemeMode>(
                OnThemeMode, (m) => CanExecute);

            _autoSaveTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(60)
            };
            _autoSaveTimer.Tick += OnAutoSaveTick;
            if (ViewState.AutoSaveEnabled)
            {
                _autoSaveTimer.Start();
            }

            _findDialog = _dialogService
                .GetDialogViewModel<IFindDialogViewModel>();
            if (_findDialog != null)
            {
                _findDialog.FindNextRaisedByDialog +=
                    OnFindNextRaisedByDialog;
                _findDialog.ReplaceCmd = ReplaceCmd;
                _findDialog.GoToCmd = GoToCmd;
                _findDialog.PropertyChanged += OnFindDialogPropertyChanged;
            }           

            _replaceDialog = _dialogService
                .GetDialogViewModel<IReplaceDialogViewModel>();
            if (_replaceDialog != null)
            {
                _replaceDialog.FindNextRaisedByDialog += OnFindNextRaisedByDialog;
                _replaceDialog.ReplaceRaisedByDialog += OnReplaceRaisedByDialog;
                _replaceDialog.ReplaceAllRaisedByDialog += OnReplaceAllRaisedByDialog;
                _replaceDialog.FindCmd = FindCmd;
                _replaceDialog.GoToCmd = GoToCmd;
                _replaceDialog.PropertyChanged += OnFindDialogPropertyChanged;
            }
        }

        public async Task<bool> OnClosing()
        {
            _autoSaveTimer.Stop();
            _viewStateService.Save(ViewState);

            var saveChanges = SaveChangesConfirmation();
            if (saveChanges == true)
            {
                await SaveAsync();
            }
            else if (saveChanges is null)
            {
                return false;
            }            
            return true;
        }

        public async Task Open(string location)
        {
            StartLongOperation();
            try
            {
                SelectedItem = await _fileModelService
                    .LoadAsync(location);

                RaisePropertyChanged(nameof(Title));
                RaisePropertyChanged(nameof(WordCount));
                AddToRecentFiles(location);
            }
            catch (System.IO.FileNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine($"File not found: {location}");
            }
            catch (System.IO.IOException ex)
            {
                System.Diagnostics.Debug.WriteLine($"IO error loading file: {ex.Message}");
            }
            catch (UnauthorizedAccessException)
            {
                System.Diagnostics.Debug.WriteLine($"Access denied: {location}");
            }
            finally
            {
                EndLongOpertation();
            }
        }

        public string Title => $"{SelectedItem?.Name} - Black Notepad";

        public bool IsBusy => _busyRegister.Count > 0;

        public ViewStateModel ViewState { get; }

        public IList<FontColourModel> FontColours { get; }

        public IList<FontFamilyModel> FontFamilyNames { get; }

        public FileModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != null)
                {
                    SelectedItem.PropertyChanged -=
                            OnSelectedItemPropertyChanged;
                }

                Set(ref _selectedItem, value);

                if (value != null)
                {
                    SelectedItem.PropertyChanged += 
                        OnSelectedItemPropertyChanged;

                    RaisePropertyChanged(nameof(Title));
                }
            }
        }

        public string TextSought
        {
            get => _textSought;
            set => Set(ref _textSought, value);
        }

        public string SelectedText
        {
            get => _selectedText;
            set
            {
                Set(ref _selectedText, value);
                RaisePropertyChanged(nameof(IsCutOrCopyEnabled));
                RaisePropertyChanged(nameof(IsPasteEnabled));
            }
        }

        public int CaretLine
        {
            get => _caretLine;
            set
            {
                Set(ref _caretLine, value + 1);
                RaisePropertyChanged(nameof(CaretPosition));
            }
        }

        public int CaretColumn
        {
            get => _caretColumn;
            set
            {
                Set(ref _caretColumn, value + 1);
                RaisePropertyChanged(nameof(CaretPosition));
            }
        }

        public int IndexOfCaret
        {
            get => _indexOfCaret;
            set => Set(ref _indexOfCaret, value);
        }

        public (int Column, int Line) CaretPosition =>
            (CaretColumn, CaretLine);

        public RelayCommand NewCmd { get; }
        public RelayCommand OpenCmd { get; }
        public RelayCommand SaveCmd { get; }
        public RelayCommand SaveAsCmd { get; }
        public RelayCommand ExitCmd { get; }
        public RelayCommand<string> OpenRecentCmd { get; }
        public RelayCommand FindCmd { get; }
        public RelayCommand FindNextCmd { get; }
        public RelayCommand EscCmd { get; }
        public RelayCommand ReplaceCmd { get; }
        public RelayCommand GoToCmd { get; }
        public RelayCommand TimeDateCmd { get; }
        public RelayCommand WordWrapCmd { get; }
        public RelayCommand ZoomInCmd { get; }
        public RelayCommand ZoomOutCmd { get; }
        public RelayCommand RestoreDefaultZoomCmd { get; }
        public RelayCommand StatusBarCmd { get; }
        public RelayCommand HelpCmd { get; }
        public RelayCommand AboutCmd { get; }
        public RelayCommand<FontColourModel> FontColourCmd { get; }
        public RelayCommand<FontFamilyModel> FontFamilyCmd { get; }
        public RelayCommand PrettifyJsonCmd { get; }
        public RelayCommand PrintPreviewCmd { get; }
        public RelayCommand<ThemeMode> ThemeModeCmd { get; }

        /// <summary>
        /// Returns the number of whitespace-separated words in the current document content.
        /// </summary>
        /// <value>Word count as an integer. Returns 0 when content is null or empty.</value>
        public int WordCount
        {
            get
            {
                var content = SelectedItem?.Content;
                if (string.IsNullOrWhiteSpace(content))
                {
                    return 0;
                }
                return content.Split(
                    (char[])null, StringSplitOptions.RemoveEmptyEntries).Length;
            }
        }

        /// <summary>
        /// Returns the total number of lines in the current document content,
        /// used to populate the line number gutter.
        /// </summary>
        /// <value>Line count as an integer. Returns 1 when content is null or empty.</value>
        public int LineNumberCount
        {
            get => _lineNumberCount;
            set => Set(ref _lineNumberCount, value);
        }

        private int _lineNumberCount = 1;

        private double _lineScrollOffset;

        /// <summary>
        /// Gets or sets the vertical scroll offset in pixels for the line number gutter.
        /// Synchronized with the main TextBox's vertical scroll position from the View layer.
        /// </summary>
        /// <value>Non-negative double representing the vertical offset in device-independent pixels.</value>
        public double LineScrollOffset
        {
            get => _lineScrollOffset;
            set => Set(ref _lineScrollOffset, value);
        }

        public bool CanExecute => !IsBusy;

        public bool CanExecuteOpen => CanExecute;

        public bool CanExecuteZoomIn => CanExecute && !_isFontZoomMax;

        public bool CanExecuteZoomOut => CanExecute && !_isFontZoomMin;

        public bool CanExecuteFind => CanExecute &&
            !string.IsNullOrEmpty(SelectedItem?.Content);

        public bool CanExecuteFindNext => CanExecute &&
            !string.IsNullOrEmpty(TextSought);

        public bool CanExecuteEsc => CanExecute &&
            _findNextCount > 0;

        public bool CanExecuteReplace => CanExecute;

        public bool CanExecuteDragDrop => CanExecute &&
            !SelectedItem.IsDirty;

        public bool CanExecutePrettifyJson => CanExecute &&
            SelectedItem.HasContent;


        public bool IsUndoEnabled => !IsBusy &&
            SelectedItem.IsDirty;

        public bool IsCutOrCopyEnabled => !IsBusy &&
            !string.IsNullOrEmpty(SelectedText);

        public bool IsPasteEnabled => !IsBusy &&
            Clipboard.ContainsText();

        public bool IsSelectAllEnabled => !IsBusy &&
            SelectedItem.HasContent;

        public Action<int, int, int> GoToRequested = delegate { };

        public Action FocusRequested = delegate { };

        public Action TimeDateRequested = delegate { };

        private void RaiseGoToRequested(
            int caretIndex, int selectionLength, int line)
        {
            GoToRequested?
                .Invoke(caretIndex, selectionLength, line);
        }

        private void RaiseFocusRequested()
        {
            FocusRequested?.Invoke();
        }

        private void StartLongOperation(
            [CallerMemberName]string caller = "")
        {
            _busyRegister.Add(caller);
            RaisePropertyChanged(nameof(IsBusy));
        }

        private void EndLongOpertation(
            [CallerMemberName]string caller = "")
        {
            _busyRegister.Remove(caller);
            RaisePropertyChanged(nameof(IsBusy));
        }

        private async void OnNew()
        {
            try
            {
                await New();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"New failed: {ex.Message}");
            }
        }

        private async Task New()
        {
            var saveChanges = SaveChangesConfirmation();
            if (saveChanges == true)
            {
                await SaveAsync();
            }
            else if (saveChanges is null)
            {
                return;
            }
            SelectedItem = _fileModelService.New();
            RaisePropertyChanged(nameof(Title));
        }

        private async void OnOpen()
        {
            try
            {
                var saveChanges = SaveChangesConfirmation();
                if (saveChanges == true)
                {
                    await SaveAsync();
                }
                else if (saveChanges is null)
                {
                    return;
                }
                var result = _openFileDialog.ShowDialog();
                if (result == true)
                {
                    await Open(_openFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Open failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Opens a file directly from a path provided by the recent files menu,
        /// bypassing the file dialog.
        /// </summary>
        /// <param name="path">Full file path to open. If null or empty, the operation is ignored.</param>
        private async void OnOpenRecent(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            var saveChanges = SaveChangesConfirmation();
            if (saveChanges == true)
            {
                await SaveAsync();
            }
            else if (saveChanges is null)
            {
                return;
            }
            await Open(path);
        }

        private async void OnSave()
        {
            await SaveAsync();
        }
        private async Task SaveAsync()
        {
            if (!SelectedItem.IsNew)
            {
                StartLongOperation();
                try
                {
                    await _fileModelService.SaveAsync(SelectedItem);
                    RaisePropertyChanged(nameof(Title));
                }
                catch (System.IO.IOException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"IO error saving file: {ex.Message}");
                }
                catch (UnauthorizedAccessException)
                {
                    System.Diagnostics.Debug.WriteLine($"Access denied saving file: {SelectedItem.Location}");
                }
                finally
                {
                    EndLongOpertation();
                }
            }
            else
            {
                await SaveAsAsync();
            }
        }

        private async void OnSaveAs()
        {
            try
            {
                await SaveAsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveAs failed: {ex.Message}");
            }
        }
        private async Task SaveAsAsync()
        {
            var result = _saveFileDialog.ShowDialog();
            if (result == true)
            {
                SelectedItem.Location = _saveFileDialog.FileName;
                await SaveAsync();
            }
        }

        private async void OnExit()
        {
            await OnClosing();
            Application.Current.Shutdown();
        }

        private void OnFind()
        {
            if (SelectedText != null)
            {
                _findDialog.TextSought = SelectedText;
            }
            _dialogService.Show(_findDialog);
            _findNextCount = 0;
        }

        private void OnFindNext()
        {
            FindNext();
        }

        private void OnFindNextRaisedByDialog(
            object sender, FindNextEventArgs e)
        {
            _isFindWrapAround = e.IsFindWrapAround;
            _isFindMatchCase = e.IsFindMatchCase;

            FindNext();
            RaiseFocusRequested();
        }

        private void FindNext()
        {
            // Bolt: Optimized FindNext to avoid large string allocations and use IndexOf with comparison.
            // Also fixed a bug where matches at index 0 were ignored.
            var textSought = TextSought;
            var allText = SelectedItem.Content;

            if (string.IsNullOrEmpty(allText) || string.IsNullOrEmpty(textSought))
            {
                return;
            }

            var comparison = _isFindMatchCase
                ? StringComparison.CurrentCulture
                : StringComparison.CurrentCultureIgnoreCase;

            if (allText.IndexOf(textSought, comparison) < 0)
            {
                return;
            }

            var isFindDirectionUp = _findDialog?.IsFindDirectionUp == true;
            int indexOfTextFound = -1;

            if (isFindDirectionUp)
            {
                if (IndexOfCaret == 0)
                {
                    if (_isFindWrapAround)
                    {
                        IndexOfCaret = allText.Length;
                        FindNext();
                        return;
                    }
                    else
                    {
                        return;
                    }
                }

                if (IndexOfCaret > 0)
                {
                    // Search backwards from IndexOfCaret
                    int startIndex = IndexOfCaret - 1;
                    int count = IndexOfCaret;

                    if (startIndex >= 0)
                    {
                        indexOfTextFound = allText.LastIndexOf(textSought, startIndex, count, comparison);
                    }
                }
            }
            else
            {
                // Check if we need to wrap
                int lastOccurrence = allText.LastIndexOf(textSought, comparison);
                if (IndexOfCaret >= lastOccurrence && lastOccurrence != -1)
                {
                    if (_isFindWrapAround)
                    {
                        IndexOfCaret = 0;
                        FindNext();
                        return;
                    }
                    else
                    {
                        return;
                    }
                }

                int startOfTextToSearch;
                if (_findNextCount > 0)
                {
                    startOfTextToSearch = IndexOfCaret + textSought.Length;
                }
                else
                {
                    startOfTextToSearch = IndexOfCaret;
                }

                if (startOfTextToSearch < allText.Length)
                {
                    indexOfTextFound = allText.IndexOf(textSought, startOfTextToSearch, comparison);
                }
            }

            if (indexOfTextFound >= 0)
            {
                _findNextCount++;
                RaiseGoToRequested(
                    indexOfTextFound,
                    textSought.Length,
                    allText.LineOfIndexOrDefault(
                        indexOfTextFound,
                        SelectedItem.LineEnding));
            }
        }

        private void OnEsc()
        {
            TextSought = string.Empty;
            FindNext();
        }

        private void OnReplace()
        {
            if (SelectedText != null)
            {
                _replaceDialog.TextSought = SelectedText;
            }
            _dialogService.Show(_replaceDialog);
            _findNextCount = 0;
            _isReadyForReplacement = false;
        }

        private void OnReplaceRaisedByDialog(
            object sender, FindNextEventArgs e)
        {
            _isFindWrapAround = e.IsFindWrapAround;
            _isFindMatchCase = e.IsFindMatchCase;

            Replace();
            RaiseFocusRequested();
        }

        private void OnReplaceAllRaisedByDialog(
            object sender, FindNextEventArgs e)
        {
            _isFindWrapAround = e.IsFindWrapAround;
            _isFindMatchCase = e.IsFindMatchCase;

            ReplaceAll();
            RaiseFocusRequested();
        }

        private void Replace()
        {
            if (_findDialog?.IsFindDirectionUp == true)
            {
                throw new NotSupportedException();
            }
            if (!ValidReplace())
            {
                return;
            }
            if (!_isReadyForReplacement)
            {
                FindNext();
                _isReadyForReplacement = _findNextCount > 0;
            }
            else
            {
                if (string.IsNullOrEmpty(SelectedItem.Content))
                {
                    return;
                }

                var allText = _isFindMatchCase ?
                    SelectedItem.Content : 
                    SelectedItem.Content?.ToLower();

                var replacement = _replaceDialog?.ReplacementText;

                var sought = _isFindMatchCase ?
                    TextSought : TextSought?.ToLower();

                var textPrior = string.Empty;
                if (allText.Contains(sought))
                {
                    textPrior = SelectedItem.Content?
                        .Substring(0, IndexOfCaret);

                    var endOfTextAfter =
                        IndexOfCaret + sought.Length;

                    var textAfter = SelectedItem.Content?
                        .Substring(endOfTextAfter);

                    allText = $"{textPrior}{replacement}{textAfter}";
                }
                SelectedItem.Content = allText;

                var indexOfTextFound = textPrior.Length +
                    replacement.Length;

                RaiseGoToRequested(
                    indexOfTextFound, 
                    0, 
                    allText.LineOfIndexOrDefault(
                        indexOfTextFound, SelectedItem.LineEnding));

                _isReadyForReplacement = false;
            }
        }

        private void ReplaceAll()
        {
            if (!ValidReplace())
            {
                return;
            }
            var text = SelectedItem.Content;
            var replacement = _replaceDialog?.ReplacementText;
            // Bolt: Optimized case-sensitive replace to use string.Replace (~20x faster)
            // and fixed inverted logic for case-insensitive replace.
            if (_isFindMatchCase)
            {
                text = text.Replace(TextSought, replacement);
            }
            else
            {
                // Escape search text for literal matching and replacement text for $ literals
                text = Regex.Replace(
                    text,
                    Regex.Escape(TextSought),
                    replacement.Replace("$", "$$"),
                    RegexOptions.IgnoreCase);
            }
            SelectedItem.Content = text;
        }

        private bool ValidReplace()
        {
            if (_replaceDialog?.ReplacementText == null
                || string.IsNullOrEmpty(SelectedItem?.Content)
                || string.IsNullOrEmpty(TextSought))
            {
                return false;
            }
            return true;
        }

        private void OnGoTo()
        {
            var vm = _dialogService
                .GetDialogViewModel<IGoToDialogViewModel>();
            vm.LineNumber = CaretLine;
            var result = _dialogService.ShowDialog(vm);
            if (result == true)
            {
                GoTo(vm.LineNumber);
            }
        }

        private void GoTo(int lineNumberSought)
        {
            var lineEndingChar = '\r';
            if (SelectedItem.LineEnding == LineEndings.LF)
            {
                lineEndingChar = '\n';
            }
            var lineCharToInclude = 0;
            if (SelectedItem.LineEnding == LineEndings.CRLF)
            {
                lineCharToInclude = 1;
            }
            var text = SelectedItem.Content;
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var linesCounted = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == lineEndingChar
                    || i == text.Length - 1)
                {
                    linesCounted++;
                }
            }

            if (lineNumberSought < 1)
            {
                lineNumberSought = 1;
            }
            else if (lineNumberSought > linesCounted)
            {
                lineNumberSought = linesCounted;
            }

            var charsInLine = 0;
            linesCounted = 0;
            for (int i = 0; i < text.Length; i++)
            {
                charsInLine++;
                if (text[i] == lineEndingChar
                    || i == text.Length - 1)
                {
                    linesCounted++;
                    if (lineNumberSought == linesCounted)
                    {
                        var lineStartPosition =
                            i + lineCharToInclude - charsInLine;

                        RaiseGoToRequested(
                            lineStartPosition, 0, lineNumberSought);
                        break;
                    }
                    charsInLine = 0;
                }
            }
        }

        private void OnTimeDate()
        {
            TimeDateRequested?.Invoke();
        }

        private void OnWordWrap()
        {
            ViewState.IsWrapped = !ViewState.IsWrapped;
        }

        /// <summary>
        /// Ctrl+Plus
        /// </summary>
        private void OnZoomIn()
        {
            var current = ViewState.SelectedFontZoom;
            if (current is null)
            {
                RestoreDefaultZoom();
            }
            FontZoomModel value = null;
            var isCurrentFound = false;
            foreach (var fontZoom in _fontZoomIndex)
            {
                if (isCurrentFound)
                {
                    value = fontZoom;
                    current.IsSelected = false;
                    value.IsSelected = true;
                    break;
                }
                isCurrentFound = fontZoom.Key == current.Key;
            }
            if (value is null)
            {
                value = current;
                _isFontZoomMax = true;
            }
            _isFontZoomMin = false;
            ViewState.SelectedFontZoom = value;
        }

        /// <summary>
        /// Ctrl+Minus
        /// </summary>
        private void OnZoomOut()
        {
            var current = ViewState.SelectedFontZoom;
            if (current is null)
            {
                RestoreDefaultZoom();
            }
            FontZoomModel value = null;
            foreach (var fontZoom in _fontZoomIndex)
            {
                if (fontZoom.Key == current.Key)
                {
                    current.IsSelected = false;
                    break;
                }
                value = fontZoom;
            }
            if (value is null)
            {
                value = current;
                _isFontZoomMin = true;
            }
            _isFontZoomMax = false;
            value.IsSelected = true;
            ViewState.SelectedFontZoom = value;
        }

        private void OnRestoreDefaultZoom()
        {
            RestoreDefaultZoom();
        }
        private void RestoreDefaultZoom()
        {
            FontZoomModel @default = null;
            foreach (var fontZoom in _fontZoomIndex)
            {
                if (fontZoom.Key == _defaultZoom)
                {
                    @default = fontZoom;
                    @default.IsSelected = true;
                }
                else
                {
                    fontZoom.IsSelected = false;
                }
            }
            ViewState.SelectedFontZoom = @default;
            _isFontZoomMax = _isFontZoomMin = false;
        }

        private void OnStatusBar()
        {
            ViewState.IsStatusBarVisible = !ViewState.IsStatusBarVisible;
        }

        private void OnHelp()
        {
            StartLongOperation();

            // No longer allowed by MS Store -> Process.Start("https://savaged.github.io/BlackNotepad/");
            _dialogService.ShowDialog(
                "Visit https://savaged.github.io/BlackNotepad/", "Help");

            EndLongOpertation();
        }

        private void OnAbout()
        {
            var productVersion = string.Empty;
            try
            {
                productVersion = ApplicationDeployment.CurrentDeployment
                    .CurrentVersion.ToString();
            }
            catch (InvalidDeploymentException)
            {
                productVersion = Assembly.GetExecutingAssembly().GetName()
                    .Version.ToString();
            }
            _dialogService.ShowDialog(
                "A black 'version' of the classic Microsoft Windows " +
                $"Notepad application{Environment.NewLine}" +
                $"{Environment.NewLine}BlackNotepad v{productVersion}", 
                "About");
        }

        private void OnFontColour(FontColourModel selected)
        {
            ViewState.SelectedFontColour = selected;
            ApplySelectedOnFontColour();
        }

        private void OnThemeMode(ThemeMode mode)
        {
            ViewState.SelectedThemeMode = mode;
            _themeService.ApplyTheme(mode);
        }

        private void ApplySelectedOnFontColour()
        {
            ApplySelectedOnList(
                FontColours, ViewState.SelectedFontColour);
            RaisePropertyChanged(nameof(FontColours));
        }

        private void ApplySelectedOnList<T>(IList<T> list, T selection)
            where T : ISelectionModel
        {
            foreach (var item in list)
            {
                if (item.Equals(selection))
                {
                    item.IsSelected = true;
                }
                else
                {
                    item.IsSelected = false;
                }
            }
        }

        private void OnFontFamily(FontFamilyModel selected)
        {
            ViewState.SelectedFontFamily = selected;
            ApplySelectedOnFontFamily();
        }

        private void ApplySelectedOnFontFamily()
        {
            ApplySelectedOnList(
                FontFamilyNames, ViewState.SelectedFontFamily);
            RaisePropertyChanged(nameof(FontFamilyNames));
        }

        private bool? SaveChangesConfirmation()
        {
            bool? value = null;
            if (SelectedItem.IsDirty)
            {
                value = _dialogService.ShowDialog(
                    $"Do you want to save changes to {SelectedItem.Name}?",
                    "Black Notepad",
                    yesNoCancelButtons: true);
            }
            else
            {
                value = false;
            }
            return value;
        }

        private void OnSelectedItemPropertyChanged(
            object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SelectedItem.Content):
                    RaisePropertyChanged(nameof(IsUndoEnabled));
                    RaisePropertyChanged(nameof(IsSelectAllEnabled));
                    RaisePropertyChanged(nameof(WordCount));
                    break;
                case nameof(SelectedItem.LineEnding):
                    break;
            }
        }

        private void OnFindDialogPropertyChanged(
            object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(TextSought):
                    if (sender is FindDialogViewModel vm)
                    {
                        TextSought = vm?.TextSought;
                    }
                    break;
            }
        }

        private void OnPrettifyJson()
        {
            if (SelectedItem is null || !SelectedItem.HasContent)
            {
                return;
            }
            StartLongOperation();
            try
            {
                dynamic parsed = JsonConvert
                    .DeserializeObject(SelectedItem.Content);
                SelectedItem.Content = JsonConvert
                    .SerializeObject(parsed, Formatting.Indented);
            }
            catch (JsonException)
            {
                System.Diagnostics.Debug.WriteLine("Failed to prettify JSON: invalid JSON content");
            }
            finally
            {
                EndLongOpertation();
            }
        }

        /// <summary>
        /// Adds the specified file path to the top of the Recent Files list.
        /// Removes any duplicate entry first. Limits the list to 10 items.
        /// </summary>
        /// <param name="path">Full file path to add. Ignored if null or whitespace.</param>
        private void AddToRecentFiles(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }
            var recentFiles = ViewState.RecentFiles;
            for (int i = recentFiles.Count - 1; i >= 0; i--)
            {
                if (string.Equals(recentFiles[i], path,
                    StringComparison.OrdinalIgnoreCase))
                {
                    recentFiles.RemoveAt(i);
                }
            }
            recentFiles.Insert(0, path);
            while (recentFiles.Count > 10)
            {
                recentFiles.RemoveAt(recentFiles.Count - 1);
            }
        }

        /// <summary>
        /// Handles the auto-save timer tick. Saves the document automatically
        /// when it is dirty and has a saved file location.
        /// </summary>
        /// <param name="sender">The DispatcherTimer that raised the tick.</param>
        /// <param name="e">Timer event arguments. Unused.</param>
        private async void OnAutoSaveTick(object sender, EventArgs e)
        {
            if (SelectedItem != null && SelectedItem.IsDirty
                && !SelectedItem.IsNew
                && !string.IsNullOrEmpty(SelectedItem.Location))
            {
                try
                {
                    await SaveAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Auto-save failed: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Opens the system Print dialog to preview and print the current document.
        /// </summary>
        private void OnPrintPreview()
        {
            if (SelectedItem is null || !SelectedItem.HasContent)
            {
                return;
            }
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                var flowDoc = new System.Windows.Documents.FlowDocument(
                    new System.Windows.Documents.Paragraph(
                        new System.Windows.Documents.Run(SelectedItem.Content)));
                var documentPaginator =
                    ((System.Windows.Documents.IDocumentPaginatorSource)flowDoc)
                    .DocumentPaginator;
                printDialog.PrintDocument(documentPaginator, Title);
            }
        }
    }
}
