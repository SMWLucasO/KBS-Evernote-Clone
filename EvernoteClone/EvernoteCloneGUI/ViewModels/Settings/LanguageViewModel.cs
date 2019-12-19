using System.Windows.Controls;
using EvernoteCloneGUI.ViewModels.Controls.Settings;
using EvernoteCloneGUI.Views.Settings;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Settings.Locales;

namespace EvernoteCloneGUI.ViewModels.Settings
{
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
        public void LoadLanguageComboBox()
        {
            // For all languages, add them to the ComboBox
            foreach (Locale locale in Locale.GetAllLocales())
                ComboBoxHelper.AddItemToComboBox(ref LanguageComboBox, locale.Language, nameof(SettingsConstant.LANGUAGE));
            
            // If used offline (or if something else happens) and no languages are added, add standard language
            ComboBoxHelper.AddItemToComboBox(ref LanguageComboBox, nameof(SettingsConstant.LANGUAGE));
            
            // Select standard language
            ComboBoxHelper.SelectComboBoxItemByTag(ref LanguageComboBox, SettingsConstant.LANGUAGE);
            
            LanguageViewModel tmp = new LanguageViewModel();
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
        
        

        #endregion
    }
}
