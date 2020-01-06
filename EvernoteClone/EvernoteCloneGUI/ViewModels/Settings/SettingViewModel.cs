using System.Windows;
using Caliburn.Micro;
using EvernoteCloneGUI.ViewModels.Controls.Settings;

namespace EvernoteCloneGUI.ViewModels.Settings
{
    /// <summary>
    /// This class handles a lot of standard functionality all ...ViewModels should have that are used by SettingsViewModel.cs
    /// </summary>
    public abstract class SettingViewModel : Screen
    {
        #region Properties

        /// <value>
        /// The property that is used to update the background of the grid of each view that inherits from this class
        /// </value>
        public string BackgroundColorSettings
        {
            get => _backgroundColorSettings;
            set
            {
                _backgroundColorSettings = value;
                NotifyOfPropertyChange(nameof(BackgroundColorSettings));
            }
        }

        #endregion

        #region Variables

        /// <value>
        /// This bool indicates if this instance of the view is loaded once before or not.
        /// </value>
        protected bool Loaded = false;

        /// <value>
        /// The variable that contains the background color of the grid
        /// </value>
        private string _backgroundColorSettings;

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

        #region Events
        
        /// <summary>
        /// Fired when the text is changed inside a TextBox
        /// </summary>
        /// <param name="sender">The TextBox</param>
        public void TextBoxTextChanged(object sender) =>
            TextBoxHelper.TextBoxTextChanged(sender);
        
        /// <summary>
        /// Fired when the index of the selected item changes
        /// </summary>
        /// <param name="sender">The newly selected ComboBoxItem</param>
        public void ComboBoxSelectedIndexChanged(object sender) =>
            ComboBoxHelper.ComboBoxSelectedIndexChanged(sender);

        #endregion
    }
}