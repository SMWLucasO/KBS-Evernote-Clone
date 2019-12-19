using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EvernoteCloneGUI.ViewModels.Controls.Settings;
using EvernoteCloneGUI.Views.Settings;
using EvernoteCloneLibrary.Constants;

namespace EvernoteCloneGUI.ViewModels.Settings
{
    public class ColorsViewModel : Screen
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
            ((SettingsViewModel) Parent).UpdateColors();
        }
        
        #endregion
        
        #region Load Textboxes

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
            if (view is ColorsView colorsView)
            {
                ButtonBackground = colorsView.ButtonBackground;
                ButtonBackgroundActive = colorsView.ButtonBackgroundActive;
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
