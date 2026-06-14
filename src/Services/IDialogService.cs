using Microsoft.Win32;
using Savaged.BlackNotepad.ViewModels;
using Savaged.BlackNotepad.ViewsInterfaces;
using System;

namespace Savaged.BlackNotepad.Services
{
    public interface IDialogService
    {
        /// <summary>
        /// Resolves a file dialog instance from the service container.
        /// </summary>
        /// <typeparam name="T">The FileDialog-derived type to resolve.</typeparam>
        /// <returns>The registered FileDialog instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the requested type is not registered.</exception>
        T GetFileDialog<T>() where T : FileDialog;

        /// <summary>
        /// Resolves a dialog ViewModel instance from the service container.
        /// </summary>
        /// <typeparam name="T">The IDialogViewModel interface type to resolve.</typeparam>
        /// <returns>The registered ViewModel instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the requested type is not registered.</exception>
        T GetDialogViewModel<T>() 
            where T : IDialogViewModel;

        /// <summary>
        /// Shows a modeless dialog for the given ViewModel.
        /// </summary>
        /// <param name="vm">The ViewModel to display.</param>
        void Show(IDialogViewModel vm);

        /// <summary>
        /// Shows a modal dialog for the given ViewModel and returns the result.
        /// </summary>
        /// <param name="vm">The ViewModel to display.</param>
        /// <returns>True if accepted, false if declined, null if closed without result.</returns>
        bool? ShowDialog(IDialogViewModel vm);

        /// <summary>
        /// Shows a message box dialog with the specified message and title.
        /// </summary>
        /// <param name="msg">The message to display.</param>
        /// <param name="title">The dialog title.</param>
        /// <param name="yesNoButtons">If true, shows Yes/No buttons instead of OK.</param>
        /// <param name="yesNoCancelButtons">If true, shows Yes/No/Cancel buttons.</param>
        /// <returns>True for Yes, false for No, null for Cancel.</returns>
        bool? ShowDialog(
            string msg,
            string title,
            bool yesNoButtons = false,
            bool yesNoCancelButtons = false);

        /// <summary>
        /// Raised when a dialog completes its interaction.
        /// </summary>
        event EventHandler<IDialogDoneEventArgs> DialogDone;
    }
}