using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using EvernoteCloneGUI.ViewModels.Controls.Settings;
using EvernoteCloneGUI.Views.Settings;
using EvernoteCloneLibrary.Constants;

namespace EvernoteCloneGUI.ViewModels.Settings
{
    public class StandardsViewModel: Screen
    {
        #region Variables

        /// <value>
        /// The TextBox that contains the Default Note Title
        /// </value>
        public TextBox DefaultNoteTitle;
        
        /// <value>
        /// The TextBox that contains the Default Notebook Title
        /// </value>
        public TextBox DefaultNotebookTitle;
        
        /// <value>
        /// This bool indicates if this instance of the view is loaded once before or not.
        /// </value>
        private bool _loaded = false;
        
        #endregion
        
        #region Button handlers
        
        /// <summary>
        /// Saves all changes locally
        /// </summary>
        public void ApplyChanges()
        {
            if (EvernoteCloneLibrary.Settings.Setting.SaveSettings())
            {
                MessageBox.Show("Settings successfully saved!", "NoteFever | Settings", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Something went wrong while trying to save settings.", "NoteFever | Settings", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Update colors of Settings view
            ((SettingsViewModel) Parent).UpdateLayout();
        }
        
        #endregion
        
        #region Load Textboxes

        /// <summary>
        /// Loads all settings into TextBoxes
        /// </summary>
        public void LoadTextBoxes()
        {
            TextBoxHelper.SetTextBox(ref DefaultNoteTitle, nameof(SettingsConstant.DEFAULT_NOTE_TITLE));
            TextBoxHelper.SetTextBox(ref DefaultNotebookTitle, nameof(SettingsConstant.DEFAULT_NOTEBOOK_TITLE));
        }

        #endregion
        
        #region Events

        /// <summary>
        /// When the view is attached, prepare the settings view for usage
        /// </summary>
        /// <param name="view"></param>
        /// <param name="context"></param>
        protected override void OnViewAttached(object view, object context)
        {
            if (view is StandardsView colorsView)
            {
                DefaultNoteTitle = colorsView.DefaultNoteTitle;
                DefaultNotebookTitle = colorsView.DefaultNotebookTitle;
            }

            if (!_loaded)
            {
                LoadTextBoxes();
                _loaded = true;
            }
        }
        
        /// <summary>
        /// Fired when the text is changed inside a TextBox
        /// </summary>
        /// <param name="sender">The TextBox</param>
        public void TextBoxTextChanged(object sender) =>
            TextBoxHelper.TextBoxTextChanged(sender);

        #endregion
    }
}
