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
    /// Metadata record for a single historical version of a document.
    /// </summary>
    public class HistoryVersion
    {
        /// <summary>
        /// Gets or sets the unique identifier for this version.
        /// </summary>
        public string VersionId { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp when this version was committed.
        /// </summary>
        public DateTime TimestampUtc { get; set; }

        /// <summary>
        /// Gets or sets the trigger reason for creating this version.
        /// </summary>
        public SaveReason Reason { get; set; }

        /// <summary>
        /// Gets or sets the hex-encoded SHA-256 hash of the uncompressed text content.
        /// </summary>
        public string Sha256 { get; set; }

        /// <summary>
        /// Gets or sets the character count of the uncompressed text content.
        /// </summary>
        public long ContentLength { get; set; }

        /// <summary>
        /// Gets or sets the byte size of the stored payload file on disk.
        /// </summary>
        public long StoredSizeBytes { get; set; }

        /// <summary>
        /// Gets or sets the compression format: "FullGzip" or "DeltaMsDelta".
        /// </summary>
        public string CompressionType { get; set; }

        /// <summary>
        /// Gets or sets the base version identifier if delta compression was used; null for full baseline.
        /// </summary>
        public string BaseVersionId { get; set; }

        /// <summary>
        /// Gets or sets the delta depth from the nearest full baseline checkpoint (0 for full baseline).
        /// </summary>
        public int Depth { get; set; }

        /// <summary>
        /// Gets or sets the file name of the binary payload in the history storage folder.
        /// </summary>
        public string PayloadFileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether DPAPI encryption was applied to the payload.
        /// </summary>
        public bool IsEncrypted { get; set; }
    }
}
