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

using System.Collections.Generic;

namespace Savaged.BlackNotepad.Models
{
    /// <summary>
    /// Manifest document persisting the historical timeline and metadata for a specific file.
    /// Serialized as JSON in the document's history folder.
    /// </summary>
    public class HistoryManifest
    {
        /// <summary>
        /// Gets or sets the schema version for the manifest format.
        /// </summary>
        public int SchemaVersion { get; set; } = 1;

        /// <summary>
        /// Gets or sets the original disk location of the document.
        /// </summary>
        public string OriginalFilePath { get; set; }

        /// <summary>
        /// Gets or sets the chronological collection of historical versions for this document.
        /// </summary>
        public List<HistoryVersion> Versions { get; set; } = new List<HistoryVersion>();
    }
}
