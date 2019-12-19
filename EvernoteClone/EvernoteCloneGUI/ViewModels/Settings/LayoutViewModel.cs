using System.Windows.Controls;
using EvernoteCloneGUI.ViewModels.Controls.Settings;
using EvernoteCloneGUI.Views.Settings;
using EvernoteCloneLibrary.Constants;

namespace EvernoteCloneGUI.ViewModels.Settings
{
    public class LayoutViewModel : SettingViewModel
    {
        #region Variables

        /// <value>
        /// The TextBox that contains the Button Background Color
        /// </value>
        public TextBox ButtonBackground;
        
        /// <value>
        /// The TextBox that contains the Active Button Background Color
        /// </value>
        public TextBox ButtonBackgroundActive;
        
        #endregion
        
        #region Load Textboxes

        /// <summary>
        /// Loads all settings into TextBoxes
        /// </summary>
        public void LoadTextBoxes()
        {
            TextBoxHelper.SetTextBox(ref ButtonBackground, nameof(SettingsConstant.BUTTON_BACKGROUND));
            TextBoxHelper.SetTextBox(ref ButtonBackgroundActive, nameof(SettingsConstant.BUTTON_BACKGROUND_ACTIVE));
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
            if (view is LayoutView colorsView)
            {
                ButtonBackground = colorsView.ButtonBackground;
                ButtonBackgroundActive = colorsView.ButtonBackgroundActive;
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
