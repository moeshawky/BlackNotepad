using System;
using Savaged.BlackNotepad.Services;
using Savaged.BlackNotepad.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Savaged.BlackNotepad.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        private bool _isClosing;
        private readonly DispatcherTimer _scrollSyncTimer;
        private readonly IThemeService _themeService;

        public MainWindow(IThemeService themeService)
        {
            InitializeComponent();
            _themeService = themeService;
            _scrollSyncTimer = new DispatcherTimer
            {
                Interval = System.TimeSpan.FromMilliseconds(100)
            };
            _scrollSyncTimer.Tick += OnScrollSyncTick;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _viewModel = DataContext as MainViewModel;
            _viewModel.GoToRequested += OnGoToRequested;
            _viewModel.FocusRequested += OnFocusRequested;
            _viewModel.TimeDateRequested += OnTimeDateRequested;
            _scrollSyncTimer.Start();

            if (_themeService != null && _viewModel?.ViewState != null)
            {
                _themeService.ApplyTheme(_viewModel.ViewState.SelectedThemeMode);
            }
        }

        private void OnTimeDateRequested()
        {
            ContentText.AppendText(System.DateTime.Now.ToString("HH:mm yyyy-MM-dd"));
        }

        private void OnGoToRequested(
            int start, 
            int selectionLength,
            int line)
        {
            if (start > 0)
            {
                if (selectionLength > 0)
                {
                    ContentText.Select(start, selectionLength);
                }
                else
                {
                    ContentText.CaretIndex = start;
                }
                ContentText.ScrollToLine(line);
                Focus();
            }
        }

        private void OnFocusRequested()
        {
            Focus();
        }

        private void OnContentTextSelectionChanged(
            object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.SelectedText = ContentText.SelectedText;

                var selectionStart = ContentText.SelectionStart;
                var line = ContentText
                    .GetLineIndexFromCharacterIndex(selectionStart);
                var column = selectionStart - 
                    ContentText.GetCharacterIndexFromLineIndex(line);
                _viewModel.CaretLine = line;
                _viewModel.CaretColumn = column;
                _viewModel.IndexOfCaret = selectionStart;
            }
        }

        private async void OnClosing(object sender, CancelEventArgs e)
        {
            if (_viewModel != null && !_isClosing)
            {
                _scrollSyncTimer.Stop();
                _isClosing = await _viewModel.OnClosing();                
                if (_isClosing)
                {
                    _viewModel.GoToRequested -= OnGoToRequested;
                    _viewModel.FocusRequested -= OnFocusRequested;
                    _viewModel.TimeDateRequested -= OnTimeDateRequested;
                    Application.Current.Shutdown();
                }
            }
            e.Cancel = true;
        }

        private void OnContentTextPreviewDragOver(object sender, DragEventArgs e)
        {
            if (_viewModel != null && _viewModel.CanExecuteDragDrop
                && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private async void OnContentTextPreviewDrop(
            object sender, DragEventArgs e)
        {
            if (_viewModel != null && _viewModel.CanExecuteDragDrop)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
                {
                    var fileLocation = (string[])e.Data
                        .GetData(DataFormats.FileDrop);

                    await _viewModel.Open(fileLocation[0]);
                    e.Handled = true;
                    return;
                }
            }
            e.Handled = false;
        }

        private void OnUndoMenuItemClick(object sender, RoutedEventArgs e)
        {
            ContentText.Undo(); 
        }

        private void OnCutMenuItemClick(object sender, RoutedEventArgs e)
        {
            ContentText.Cut();
        }

        private void OnCopyMenuItemClick(object sender, RoutedEventArgs e)
        {
            ContentText.Copy();
        }

        private void OnPasteMenuItemClick(object sender, RoutedEventArgs e)
        {
            ContentText.Paste();
        }

        private void OnSelectAllMenuItemClick(object sender, RoutedEventArgs e)
        {
            ContentText.SelectAll();
        }

        private void OnScrollSyncTick(object sender, EventArgs e)
        {
            if (_viewModel == null)
            {
                return;
            }
            _viewModel.LineNumberCount = ContentText.LineCount;
            var scrollViewer = FindChildScrollViewer(ContentText);
            if (scrollViewer != null)
            {
                _viewModel.LineScrollOffset = scrollViewer.VerticalOffset;
            }
        }

        private static ScrollViewer FindChildScrollViewer(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is ScrollViewer viewer)
                {
                    return viewer;
                }
                var result = FindChildScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
