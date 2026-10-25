// Copyright (C) 2026 Moe S. / BlackNotepad Contributors
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

using GalaSoft.MvvmLight.CommandWpf;
using Savaged.BlackNotepad.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Savaged.BlackNotepad.ViewModels
{
    /// <summary>
    /// View model for the Previous Versions dialog, allowing inspection and restoration of historical revisions.
    /// </summary>
    public class HistoryDialogViewModel : ActionDialogViewModelBase, IHistoryDialogViewModel
    {
        private readonly IFileHistoryService _historyService;
        private string _fileLocation;
        private HistoryVersionItemViewModel _selectedVersion;
        private string _previewContent = string.Empty;
        private bool _isRestored;
        private string _restoredContent;
        private bool _isLoadingPreview;
        private bool _previewLoadSuccess;

        /// <summary>
        /// Gets the collection of historical versions available for inspection.
        /// </summary>
        public ObservableCollection<HistoryVersionItemViewModel> Versions { get; } = new ObservableCollection<HistoryVersionItemViewModel>();

        /// <summary>
        /// Gets or sets the currently selected historical version.
        /// </summary>
        public HistoryVersionItemViewModel SelectedVersion
        {
            get => _selectedVersion;
            set
            {
                if (Set(ref _selectedVersion, value))
                {
                    RaisePropertyChanged(nameof(IsActionEnabled));
                    LoadPreviewAsync(_selectedVersion);
                }
            }
        }

        /// <summary>
        /// Gets the text preview of the currently selected version.
        /// </summary>
        public string PreviewContent
        {
            get => _previewContent;
            private set => Set(ref _previewContent, value);
        }

        /// <summary>
        /// Gets a value indicating whether a version was selected and confirmed for restoration.
        /// </summary>
        public bool IsRestored
        {
            get => _isRestored;
            private set => Set(ref _isRestored, value);
        }

        /// <summary>
        /// Gets the content payload to restore into the active document buffer.
        /// </summary>
        public string RestoredContent
        {
            get => _restoredContent;
            private set => Set(ref _restoredContent, value);
        }

        /// <summary>
        /// Gets a value indicating whether the restore action is permitted.
        /// Requires an active selection, preview loading to be complete, and preview retrieval to have succeeded.
        /// </summary>
        public override bool IsActionEnabled => SelectedVersion != null && !_isLoadingPreview && _previewLoadSuccess;

        /// <summary>
        /// Gets the command to confirm restoration of the selected version.
        /// </summary>
        public RelayCommand RestoreCmd { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryDialogViewModel"/> class.
        /// </summary>
        /// <param name="historyService">The history service providing version data.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="historyService"/> is null.</exception>
        public HistoryDialogViewModel(IFileHistoryService historyService)
        {
            _historyService = historyService ?? throw new ArgumentNullException(nameof(historyService));
            RestoreCmd = new RelayCommand(Restore, () => IsActionEnabled);
        }

        /// <summary>
        /// Asynchronously initializes the dialog with historical versions for the specified file.
        /// </summary>
        /// <param name="fileLocation">The absolute file path on disk.</param>
        /// <returns>A task representing the initialization operation.</returns>
        public async Task InitializeAsync(string fileLocation)
        {
            _fileLocation = fileLocation;
            IsRestored = false;
            RestoredContent = null;
            PreviewContent = string.Empty;
            _previewLoadSuccess = false;
            Versions.Clear();

            if (string.IsNullOrWhiteSpace(_fileLocation))
            {
                return;
            }

            var history = await _historyService.GetHistoryAsync(_fileLocation).ConfigureAwait(true);
            // Reverse chronological order: newest version first for fast access
            foreach (var item in history.Reverse())
            {
                Versions.Add(new HistoryVersionItemViewModel(item));
            }

            if (Versions.Count > 0)
            {
                SelectedVersion = Versions[0];
            }
            else
            {
                RaisePropertyChanged(nameof(IsActionEnabled));
            }
        }

        /// <summary>
        /// Confirms restoration of the currently previewed content if preview retrieval succeeded.
        /// </summary>
        public void Restore()
        {
            if (SelectedVersion == null || !_previewLoadSuccess)
            {
                RestoredContent = null;
                return;
            }

            RestoredContent = PreviewContent ?? string.Empty;
            IsRestored = true;
        }

        private async void LoadPreviewAsync(HistoryVersionItemViewModel version)
        {
            if (version == null)
            {
                PreviewContent = string.Empty;
                _previewLoadSuccess = false;
                return;
            }

            _isLoadingPreview = true;
            _previewLoadSuccess = false;
            RaisePropertyChanged(nameof(IsActionEnabled));
            RestoreCmd.RaiseCanExecuteChanged();

            try
            {
                string content = await _historyService.GetVersionContentAsync(_fileLocation, version.VersionId).ConfigureAwait(true);
                // Ensure selection hasn't changed while awaiting
                if (SelectedVersion == version)
                {
                    PreviewContent = content ?? string.Empty;
                    _previewLoadSuccess = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load preview for version {version.VersionId}: {ex.Message}");
                if (SelectedVersion == version)
                {
                    PreviewContent = $"[Error loading version content: {ex.Message}]";
                    _previewLoadSuccess = false;
                }
            }
            finally
            {
                _isLoadingPreview = false;
                RaisePropertyChanged(nameof(IsActionEnabled));
                RestoreCmd.RaiseCanExecuteChanged();
            }
        }
    }
}
