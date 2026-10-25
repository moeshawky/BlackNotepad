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

using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Savaged.BlackNotepad.ViewModels
{
    /// <summary>
    /// View model interface for the Previous Versions dialog.
    /// </summary>
    public interface IHistoryDialogViewModel : IActionDialogViewModel
    {
        /// <summary>
        /// Gets the collection of available historical versions for the document.
        /// </summary>
        ObservableCollection<HistoryVersionItemViewModel> Versions { get; }

        /// <summary>
        /// Gets or sets the currently selected historical version.
        /// </summary>
        HistoryVersionItemViewModel SelectedVersion { get; set; }

        /// <summary>
        /// Gets the text preview of the currently selected historical version.
        /// </summary>
        string PreviewContent { get; }

        /// <summary>
        /// Gets a value indicating whether the user confirmed a restoration action.
        /// </summary>
        bool IsRestored { get; }

        /// <summary>
        /// Gets the text content to restore into the active document.
        /// </summary>
        string RestoredContent { get; }

        /// <summary>
        /// Asynchronously initializes the dialog with historical versions for the specified file.
        /// </summary>
        /// <param name="fileLocation">The absolute file path on disk.</param>
        /// <returns>A task representing the initialization operation.</returns>
        Task InitializeAsync(string fileLocation);

        /// <summary>
        /// Executes the restore action using the currently selected version.
        /// </summary>
        void Restore();
    }
}
