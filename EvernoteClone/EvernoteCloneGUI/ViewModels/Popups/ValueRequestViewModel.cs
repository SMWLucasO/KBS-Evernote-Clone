using Caliburn.Micro;
using System;

namespace EvernoteCloneGUI.ViewModels.Popups
{
    /// <summary>
    /// ViewModel which handles interaction with the ValueRequestView, a dialog which requests a piece of data.
    /// </summary>
    public class ValueRequestViewModel : Screen
    {
        /// <summary>
        /// Delegate for the events which handle the dialog results
        /// </summary>
        /// <param name="viewModel"></param>
        public delegate void DialogResultHandler(ValueRequestViewModel viewModel);

        /// <value>
        /// The event called when the submit button gets clicked.
        /// </value>
        public event DialogResultHandler Submission;

        /// <value>
        /// The event called when the cancel button gets clicked.
        /// </value>
        public event DialogResultHandler Cancellation;

        /// <value>
        /// The title for the dialog which has been opened.
        /// </value>
        public string DialogTitle { get; set; }

        /// <value>
        /// Text for the textblock above the textbox.
        /// </value>
        public string DialogValueRequestText { get; set; }

        /// <value>
        /// The input of the user
        /// </value>
        public string Value { get; set; } = "";

        /// <summary>
        /// Event which gets called when the 'submit' button gets clicked.
        /// </summary>
        /// <param name="eventArgs"></param>
        public void OnSubmit(EventArgs eventArgs) =>
            Submission?.Invoke(this);

        /// <summary>
        /// Event which gets called when the 'cancel' button gets clicked.
        /// </summary>
        /// <param name="eventArgs"></param>
        public void OnCancel(EventArgs eventArgs) =>
            Cancellation?.Invoke(this);
    }
}
