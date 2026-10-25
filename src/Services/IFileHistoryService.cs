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

using Savaged.BlackNotepad.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Savaged.BlackNotepad.Services
{
    /// <summary>
    /// Service for persisting, managing, and retrieving local document version history.
    /// </summary>
    public interface IFileHistoryService
    {
        /// <summary>
        /// Saves a point-in-time snapshot of the document to the local history store.
        /// </summary>
        /// <param name="snapshot">The immutable snapshot containing file path, content, and metadata.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SaveSnapshotAsync(FileSaveSnapshot snapshot);

        /// <summary>
        /// Compares the current content on disk with the most recent historical version and records an external baseline version if changes are detected.
        /// </summary>
        /// <param name="fileLocation">The absolute file path on disk.</param>
        /// <param name="diskContent">The content currently stored on disk.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task RecordExternalBaselineIfNeededAsync(string fileLocation, string diskContent);

        /// <summary>
        /// Retrieves the chronological history versions for a given file location.
        /// </summary>
        /// <param name="fileLocation">The path to the file on disk.</param>
        /// <returns>A list of history versions, ordered chronologically.</returns>
        Task<IReadOnlyList<HistoryVersion>> GetHistoryAsync(string fileLocation);

        /// <summary>
        /// Retrieves the uncompressed text content of a specific historical version.
        /// </summary>
        /// <param name="fileLocation">The path to the file on disk.</param>
        /// <param name="versionId">The unique identifier of the version to retrieve.</param>
        /// <returns>The uncompressed text content of the version, or null if not found.</returns>
        Task<string> GetVersionContentAsync(string fileLocation, string versionId);

        /// <summary>
        /// Deletes the local history store for a given file.
        /// </summary>
        /// <param name="fileLocation">The path to the file on disk.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task DeleteHistoryAsync(string fileLocation);
    }
}
