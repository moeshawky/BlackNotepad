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

using System;

namespace Savaged.BlackNotepad.Models
{
    /// <summary>
    /// Represents an immutable point-in-time snapshot of document content and save metadata.
    /// Capturing this snapshot before acquiring locks eliminates TOCTOU races caused by concurrent user keystrokes.
    /// </summary>
    public class FileSaveSnapshot
    {
        /// <summary>
        /// Gets the absolute disk path of the document.
        /// </summary>
        public string Location { get; }

        /// <summary>
        /// Gets the immutable document text content at the moment the snapshot was created.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Gets the UTC timestamp when the snapshot was captured.
        /// </summary>
        public DateTime TimestampUtc { get; }

        /// <summary>
        /// Gets the trigger reason for this save operation.
        /// </summary>
        public SaveReason Reason { get; }

        /// <summary>
        /// Initializes a new immutable snapshot of document content and save context.
        /// </summary>
        /// <param name="location">The file path being saved.</param>
        /// <param name="content">The document content captured at snapshot time.</param>
        /// <param name="timestampUtc">The UTC timestamp of the snapshot.</param>
        /// <param name="reason">The reason for the save operation.</param>
        public FileSaveSnapshot(string location, string content, DateTime timestampUtc, SaveReason reason)
        {
            Location = location;
            Content = content ?? string.Empty;
            TimestampUtc = timestampUtc;
            Reason = reason;
        }
    }
}
