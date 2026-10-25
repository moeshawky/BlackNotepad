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

namespace Savaged.BlackNotepad.Models
{
    /// <summary>
    /// Specifies the reason or context for saving document history.
    /// </summary>
    public enum SaveReason
    {
        /// <summary>
        /// Explicit save initiated by the user (e.g., File -> Save or Ctrl+S).
        /// </summary>
        ManualSave,

        /// <summary>
        /// Background periodic save initiated by the auto-save timer.
        /// </summary>
        AutoSave,

        /// <summary>
        /// Save initiated via Save As with a new file destination.
        /// </summary>
        SaveAs,

        /// <summary>
        /// Baseline captured when an external process modified the file on disk prior to saving.
        /// </summary>
        ExternalBaseline,

        /// <summary>
        /// Save state created after restoring an earlier historical version.
        /// </summary>
        Restore
    }
}
