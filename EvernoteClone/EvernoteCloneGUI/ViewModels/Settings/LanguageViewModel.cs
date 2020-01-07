using System.Collections.Generic;
using System.Windows.Controls;
using EvernoteCloneGUI.ViewModels.Controls.Settings;
using EvernoteCloneGUI.Views.Settings;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Settings.Locales;

namespace EvernoteCloneGUI.ViewModels.Settings
{
    /// <summary>
    /// Contains all logic specifically for Language settings
    /// </summary>
    public class LanguageViewModel : SettingViewModel
    {
        #region Variables

        /// <value>
        /// The ComboBox that contains all languages
        /// </value>
        public ComboBox LanguageComboBox;
        
        #endregion
        
        #region Language ComboBox

        /// <summary>
        /// Loads all settings into ComboBox
        /// </summary>
        private void LoadLanguageComboBox()
        {
            List<Locale> locales = Locale.GetAllLocales();

            if (locales.Count > 0)
            {
                // For all languages, add them to the ComboBox
                foreach (Locale locale in locales)
                    ComboBoxHelper.AddItemToComboBox(ref LanguageComboBox, locale, nameof(SettingsConstant.LANGUAGE),
                        locale.Language);

                // If used offline (or if something else happens) and no languages are added, add standard language

                // Select standard language
                ComboBoxHelper.SelectComboBoxItemByTag(ref LanguageComboBox, Locale.GetLocaleByLocale(SettingsConstant.LANGUAGE));
            }
            else
            {
                LanguageComboBox.IsEnabled = false;
            }
        }
        
        #endregion
        
        #region Button Handlers

        public override void ApplyChanges()
        {
            base.ApplyChanges();
            
            Properties.Settings.Default.Save();
            LanguageChanger.UpdateResxFile();
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
            
            if (!Loaded)
            {
                LoadLanguageComboBox();
                Loaded = true;
            }
        }

        /// <summary>
        /// Fired when the index of the selected item changes
        /// </summary>
        /// <param name="sender">The newly selected ComboBoxItem</param>
        public new void ComboBoxSelectedIndexChanged(object sender)
        {
            ComboBoxHelper.ComboBoxSelectedIndexChanged(sender);

            Properties.Settings.Default.LastSelectedLanguage = SettingsConstant.LANGUAGE;
        }

        #endregion
    }
}
