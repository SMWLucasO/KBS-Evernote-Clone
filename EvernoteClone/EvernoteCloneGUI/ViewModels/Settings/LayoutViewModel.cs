using System.Windows.Controls;
using EvernoteCloneGUI.ViewModels.Controls.Settings;
using EvernoteCloneGUI.Views.Settings;
using EvernoteCloneLibrary.Constants;

namespace EvernoteCloneGUI.ViewModels.Settings
{
    /// <summary>
    /// Contains all logic specifically for Layout settings
    /// </summary>
    public class LayoutViewModel : SettingViewModel
    {
        #region Variables

        /// <value>
        /// The TextBox that contains the Button Background Color
        /// </value>
        public TextBox ButtonBackgroundColor;
        
        /// <value>
        /// The TextBox that contains the Active Button Background Color
        /// </value>
        public TextBox ButtonAccentColor;
        
        /// <value>
        /// The TextBox that contains the Background Color for Settings view
        /// </value>
        public TextBox BackgroundColorSettingsTextBox;
        
        #endregion
        
        #region Load Textboxes

        /// <summary>
        /// Loads all settings into TextBoxes
        /// </summary>
        public void LoadTextBoxes()
        {
            TextBoxHelper.SetTextBox(ref ButtonBackgroundColor, nameof(SettingsConstant.BUTTON_BACKGROUND_COLOR));
            TextBoxHelper.SetTextBox(ref ButtonAccentColor, nameof(SettingsConstant.ACCENT_COLOR));
            TextBoxHelper.SetTextBox(ref BackgroundColorSettingsTextBox, nameof(SettingsConstant.BACKGROUND_COLOR_SETTINGS));
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
                ButtonBackgroundColor = colorsView.ButtonBackgroundColor;
                ButtonAccentColor = colorsView.ButtonAccentColor;
                BackgroundColorSettingsTextBox = colorsView.BackgroundColorSettingsTextBox;
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
