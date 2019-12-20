using System.Windows.Controls;
using EvernoteCloneGUI.ViewModels.Controls.Settings;
using EvernoteCloneGUI.Views.Settings;
using EvernoteCloneLibrary.Constants;

namespace EvernoteCloneGUI.ViewModels.Settings
{
    /// <summary>
    /// Contains all logic specifically for Standards settings
    /// </summary>
    public class StandardsViewModel : SettingViewModel
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

            if (!Loaded)
            {
                LoadTextBoxes();
                Loaded = true;
            }
        }

        #endregion
    }
}
