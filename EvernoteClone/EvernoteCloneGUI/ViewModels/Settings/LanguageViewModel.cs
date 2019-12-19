using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using EvernoteCloneGUI.ViewModels.Controls.Settings;
using EvernoteCloneGUI.Views.Settings;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Settings.Locales;

namespace EvernoteCloneGUI.ViewModels.Settings
{
    public class LanguageViewModel : Screen
    {
        #region Variables

        /// <value>
        /// The ComboBox that contains all languages
        /// </value>
        public ComboBox LanguageComboBox;

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
        
        #region Language ComboBox

        /// <summary>
        /// Loads all settings into ComboBox
        /// </summary>
        public void LoadLanguageComboBox()
        {
            // For all languages, add them to the ComboBox
            foreach (Locale locale in Locale.GetAllLocales())
                ComboBoxHelper.AddItemToComboBox(ref LanguageComboBox, locale.Language, nameof(SettingsConstant.LANGUAGE));
            
            // If used offline (or if something else happens) and no languages are added, add standard language
            ComboBoxHelper.AddItemToComboBox(ref LanguageComboBox, nameof(SettingsConstant.LANGUAGE));
            
            // Select standard language
            ComboBoxHelper.SelectComboBoxItemByTag(ref LanguageComboBox, SettingsConstant.LANGUAGE);
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
            if (view is LanguageView languageView)
            {
                LanguageComboBox = languageView.LanguageComboBox;
            }
            
            if (!_loaded)
            {
                LoadLanguageComboBox();
                _loaded = true;
            }
        }

        /// <summary>
        /// Fired when the index of the selected item changes
        /// </summary>
        /// <param name="sender">The newly selected ComboBoxItem</param>
        public void ComboBoxSelectedIndexChanged(object sender) =>
            ComboBoxHelper.ComboBoxSelectedIndexChanged(sender);
        
        #endregion
    }
}
