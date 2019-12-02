using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

// TODO create check for '/' in Value (in case of creating folder) and trimming the result (just) before using Value (after submit, cancel or close)
namespace EvernoteCloneGUI.ViewModels
{

    /// <summary>
    /// ViewModel which handles interaction with the ValueRequestView, a dialog which requests a piece of data.
    /// </summary>
    public class ValueRequestViewModel : Screen
    {
        /// <summary>
        /// Delegate for the events which handle the dialog results
        /// </summary>
        /// <param name="ViewModel"></param>
        public delegate void DialogResultHandler(ValueRequestViewModel ViewModel);


        /// <summary>
        /// When the submit button gets clicked, this event will get called.
        /// </summary>
        public event DialogResultHandler Submission;

        /// <summary>
        /// When the cancel button gets clicked, this event will get called.
        /// </summary>
        public event DialogResultHandler Cancellation;

        /// <summary>
        /// The title for the dialog which has been opened.
        /// </summary>
        public string DialogTitle { get; set; } = "No title has been specified.";

        /// <summary>
        /// Text for the textblock above the textbox.
        /// </summary>
        public string DialogValueRequestText { get; set; } = "No request text has been given.";

        /// <summary>
        /// The variable containing the user input
        /// </summary>
        public string Value { get; set; } = "";

        /// <summary>
        /// Event which gets called when the 'submit' button gets clicked.
        /// </summary>
        /// <param name="EventArgs"></param>
        public void OnSubmit(EventArgs EventArgs)
        {
            Submission?.Invoke(this);
        }

        /// <summary>
        /// Event which gets called when the 'cancel' button gets clicked.
        /// </summary>
        /// <param name="EventArgs"></param>
        public void OnCancel(EventArgs EventArgs)
        {
            Cancellation?.Invoke(this);     
        }


    }
}
