using GalaSoft.MvvmLight.Ioc;
using Microsoft.Win32;
using Savaged.BlackNotepad.ViewsInterfaces;
using Savaged.BlackNotepad.ViewModels;
using Savaged.BlackNotepad.Views;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;

namespace Savaged.BlackNotepad.Services
{
    public class DialogService : IDialogService
    {
        private Dialog __visibleExclusiveDialog;

        public event EventHandler<IDialogDoneEventArgs> DialogDone =
            delegate { };

        private void RaiseDialogDone(
            Dialog sender, IDialogDoneEventArgs e)
        {
            DialogDone?.Invoke(sender, e);
        }

        private Dialog VisibleExclusiveDialog
        {
            get => __visibleExclusiveDialog;
            set
            {
                __visibleExclusiveDialog = value;
                __visibleExclusiveDialog.Closing
                    += OnVisibleExclusiveDialogClosing;
            }
        }

        private void OnVisibleExclusiveDialogClosing(
            object sender, CancelEventArgs e)
        {
            if (__visibleExclusiveDialog != null)
            {
                __visibleExclusiveDialog.Closing -=
                    OnVisibleExclusiveDialogClosing;
                __visibleExclusiveDialog = null;
            }
        }

        /// <summary>
        /// Resolves a dialog ViewModel instance from the MvvmLight SimpleIoc container.
        /// Throws if the requested type is not registered or resolves to null.
        /// </summary>
        /// <typeparam name="T">The dialog ViewModel interface type to resolve.</typeparam>
        /// <returns>The registered ViewModel instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the requested ViewModel type is not registered in SimpleIoc.</exception>
        public T GetDialogViewModel<T>() 
            where T : IDialogViewModel
        {
            var value = SimpleIoc.Default.GetInstance<T>();
            if (value is null)
            {
                throw new InvalidOperationException(
                    $"No ViewModel of type '{typeof(T).Name}' is registered in SimpleIoc. " +
                    "Ensure the ViewModel is registered before calling GetDialogViewModel.");
            }
            return value;
        }

        /// <summary>
        /// Resolves a file dialog instance from the MvvmLight SimpleIoc container.
        /// Throws if the requested type is not registered or resolves to null.
        /// </summary>
        /// <typeparam name="T">The FileDialog-derived type to resolve (e.g., OpenFileDialog, SaveFileDialog).</typeparam>
        /// <returns>The registered FileDialog instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the requested FileDialog type is not registered in SimpleIoc.</exception>
        public T GetFileDialog<T>() where T : FileDialog
        {
            var value = SimpleIoc.Default.GetInstance<T>();
            if (value is null)
            {
                throw new InvalidOperationException(
                    $"No FileDialog of type '{typeof(T).Name}' is registered in SimpleIoc. " +
                    "Ensure the FileDialog is registered before calling GetFileDialog.");
            }
            return value;
        }

        public void Show(IDialogViewModel vm)
        {
            var dialog = GetDialog(vm);
            dialog.DialogDone -= OnDialogDone;
            dialog.DialogDone += OnDialogDone;
            dialog.Show();
        }

        private void OnDialogDone(object sender, IDialogDoneEventArgs e)
        {
            if (sender is Dialog dialog)
            {
                RaiseDialogDone(dialog, e);
                dialog.Hide();
            }
        }

        public bool? ShowDialog(IDialogViewModel vm)
        {
            var dialog = GetDialog(vm);
            dialog.IsModal = true;
            var result = dialog.ShowDialog();
            return result;
        }

        public bool? ShowDialog(
            string msg, 
            string title, 
            bool yesNoButtons = false, 
            bool yesNoCancelButtons = false)
        {
            var btns = MessageBoxButton.OK;
            if (yesNoButtons)
            {
                btns = MessageBoxButton.YesNo;
            }
            if (yesNoCancelButtons)
            {
                btns = MessageBoxButton.YesNoCancel;
            }
            var result = MessageBox.Show(msg, title, btns);
            if (result == MessageBoxResult.Yes)
            {
                return true;
            }
            if (result == MessageBoxResult.No)
            {
                return false;
            }
            return null;
        }

        /// <summary>
        /// Resolves a Dialog instance by extracting the dialog name from the ViewModel type,
        /// then locates the matching Dialog type via reflection. Throws if no matching type exists.
        /// </summary>
        /// <param name="vm">The dialog ViewModel whose type name maps to a Dialog type.</param>
        /// <returns>The resolved Dialog instance with DataContext set.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no Dialog type matches the ViewModel name.</exception>
        private Dialog GetDialog(IDialogViewModel vm)
        {
            var vmName = vm.GetType().Name;
            var dialogName = vmName
                .Substring(0, vmName.IndexOf("ViewModel"));

            var value = GetDialog(dialogName);
            // Defensive guard: GetDialog(string) now throws, but null-check retained
            // as safety net in case reflection returns an unexpected null.
            if (value is null)
            {
                throw new InvalidOperationException(
                    $"No Dialog type found matching '{dialogName}'.");
            }
            value.DataContext = vm;

            if (vm is IExclusiveDialogViewModel)
            {
                if (VisibleExclusiveDialog is null)
                {
                    VisibleExclusiveDialog = value;
                }
                else if (VisibleExclusiveDialog != value)
                {
                    VisibleExclusiveDialog.Hide();
                    VisibleExclusiveDialog = value;
                }
            }
            return value;
        }

        /// <summary>
        /// Resolves a Dialog type by name using reflection over the entry assembly.
        /// Searches for types whose base type is Dialog or whose grandparent type is Dialog.
        /// </summary>
        /// <param name="dialogName">The short name of the Dialog type to locate (without "ViewModel" suffix).</param>
        /// <returns>The instantiated Dialog with DataContext set.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no Dialog type matches the given name — this is a programming error, not a runtime condition.</exception>
        private Dialog GetDialog(string dialogName)
        {
            Dialog value = null;
            foreach (var t in Assembly.GetEntryAssembly().GetTypes())
            {
                if (t.BaseType == typeof(Dialog)
                   || t.BaseType?.BaseType == typeof(Dialog))
                {
                    if (t.Name == dialogName)
                    {
                        value = (Dialog)Activator.CreateInstance(t);
                        break;
                    }
                }
            }
            if (value is null)
            {
                throw new InvalidOperationException(
                    $"No Dialog type found matching '{dialogName}'. " +
                    "Ensure a Dialog-derived class with this name exists in the entry assembly.");
            }
            return value;
        }
    }
}
