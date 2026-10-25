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

using GalaSoft.MvvmLight;
using Savaged.BlackNotepad.Models;
using System;

namespace Savaged.BlackNotepad.ViewModels
{
    /// <summary>
    /// Item view model representing a single historical revision in the Previous Versions list.
    /// </summary>
    public class HistoryVersionItemViewModel : ViewModelBase
    {
        /// <summary>
        /// Gets the underlying history version record.
        /// </summary>
        public HistoryVersion Model { get; }

        /// <summary>
        /// Gets the unique identifier for this version.
        /// </summary>
        public string VersionId => Model?.VersionId;

        /// <summary>
        /// Gets the UTC timestamp when this version was created.
        /// </summary>
        public DateTime TimestampUtc => Model?.TimestampUtc ?? DateTime.MinValue;

        /// <summary>
        /// Gets the user-friendly local timestamp string.
        /// </summary>
        public string DisplayDate => TimestampUtc != DateTime.MinValue
            ? TimestampUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
            : string.Empty;

        /// <summary>
        /// Gets the save reason for this version.
        /// </summary>
        public SaveReason Reason => Model?.Reason ?? SaveReason.ManualSave;

        /// <summary>
        /// Gets a human-readable description of the save reason.
        /// </summary>
        public string DisplayReason
        {
            get
            {
                switch (Reason)
                {
                    case SaveReason.ManualSave:
                        return "Manual Save";
                    case SaveReason.AutoSave:
                        return "Auto Save";
                    case SaveReason.SaveAs:
                        return "Save As";
                    case SaveReason.ExternalBaseline:
                        return "External Baseline";
                    case SaveReason.Restore:
                        return "Restored";
                    default:
                        return Reason.ToString();
                }
            }
        }

        /// <summary>
        /// Gets a formatted string representation of the stored payload size.
        /// </summary>
        public string DisplaySize
        {
            get
            {
                if (Model == null) return "0 B";
                long bytes = Model.StoredSizeBytes;
                if (bytes < 1024) return $"{bytes} B";
                if (bytes < 1024 * 1024) return $"{(bytes / 1024.0):F1} KB";
                return $"{(bytes / (1024.0 * 1024.0)):F2} MB";
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryVersionItemViewModel"/> class.
        /// </summary>
        /// <param name="model">The underlying history version record.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is null.</exception>
        public HistoryVersionItemViewModel(HistoryVersion model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
        }
    }
}
