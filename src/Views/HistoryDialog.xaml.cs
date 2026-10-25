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

using Savaged.BlackNotepad.ViewModels;
using System.Windows;

namespace Savaged.BlackNotepad.Views
{
    /// <summary>
    /// Interaction logic for HistoryDialog.xaml.
    /// Displays historical revisions of a document and allows the user to restore past content.
    /// </summary>
    public partial class HistoryDialog : Dialog
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryDialog"/> class.
        /// </summary>
        public HistoryDialog()
        {
            InitializeComponent();
        }

        private void OnRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is IHistoryDialogViewModel vm)
            {
                vm.Restore();
            }
            DialogResult = true;
        }
    }
}
