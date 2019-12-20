using Caliburn.Micro;
using System;
using System.Windows;
using System.Windows.Controls;

namespace EvernoteCloneGUI.ViewModels.Popups
{
    /// <summary>
    /// Class handling interaction with the 'TableRowColumnSpecifierView'
    /// </summary>
    public class TableRowColumnSpecifierViewModel : Screen
    {

        private bool _canSubmit = true;

        public bool Submitted = false;
        public bool Cancelled = false;

        #region Databound properties

        public uint ColumnCount { get; set; } = 1;
        public uint RowCount { get; set; } = 1;

        #endregion

        #region Event handling

        /// <summary>
        /// Output of the textboxes should only contain positive (nonzero) integers, thus we validate the output.
        /// </summary>
        /// <param name="args"></param>
        public void ValidateNumericTextInput(TextChangedEventArgs args)
        {
            try
            {
                if (args.Source is TextBox inputBox)
                {
                    // We need to validate if the input is correct, otherwise we need to make it unable to submit.
                    int output = int.Parse(inputBox.Text);

                    // the user may only submit if the amount of columns and rows is more than 0 (a table cannot have 0 rows or columns)
                    _canSubmit = output > 0 && ColumnCount > 0 && RowCount > 0;
                }

            }
            catch (Exception)
            {
                _canSubmit = false;
            }
        }

        /// <summary>
        /// Method handling the 'submit' button, validates if submission is possible.
        /// </summary>
        public void OnSubmit()
        {
            if (!_canSubmit)
            {
                MessageBox.Show("Please make sure that both the rows and columns are greater than 0", "Note Fever | Warning",
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                Submitted = true;
                TryClose(true);
            }

        }

        /// <summary>
        /// Method handling the 'Cancel' button.
        /// </summary>
        public void OnCancel()
        {
            Cancelled = true;
            TryClose(false);
        }

        #endregion

    }
}
