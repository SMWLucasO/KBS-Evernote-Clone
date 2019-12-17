using Caliburn.Micro;
using EvernoteCloneGUI.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneGUI.ViewModels
{
    public class SettingsViewModel : Conductor<object>.Collection.OneActive
    {
        #region Properties

        #region ViewModels
        public EditorViewModel EditorSettingsViewModel { get; set; }
        public LayoutViewModel LayoutSettingsViewModel { get; set; }
        public ColorsViewModel ColorsSettingsViewModel { get; set; }
        public LanguageViewModel LanguageSettingsViewModel { get; set; }
        #endregion

        #region Background's
        public string EditorButtonBackground { get; set; }
        public string LayoutButtonBackground { get; set; }
        public string ColorsButtonBackground { get; set; }
        public string LanguageButtonBackground { get; set; }
        #endregion

        #endregion

        public SettingsViewModel()
        {
            EditorButtonBackground = "#404040";
            LayoutButtonBackground = "#404040";
            ColorsButtonBackground = "#404040";
            LanguageButtonBackground = "#404040";

            OpenLanguageSettings();
        }

        public void OpenEditorSettings()
        {
            EditorButtonBackground = "#0052cc";

            if (EditorSettingsViewModel == null)
            {
                EditorSettingsViewModel = new EditorViewModel();
            }

            ActivateItem(EditorSettingsViewModel);
        }

        public void OpenLayoutSettings()
        {
            LayoutButtonBackground = "#0052cc";

            if (LayoutSettingsViewModel == null)
            {
                LayoutSettingsViewModel = new LayoutViewModel();
            }

            ActivateItem(LayoutSettingsViewModel);
        }

        public void OpenColorsSettings()
        {
            ColorsButtonBackground = "#0052cc";

            if (ColorsSettingsViewModel == null)
            {
                ColorsSettingsViewModel = new ColorsViewModel();
            }

            ActivateItem(ColorsSettingsViewModel);
        }

        public void OpenLanguageSettings()
        {
            LanguageButtonBackground = "#0052cc";

            if (LanguageSettingsViewModel == null)
            {
                LanguageSettingsViewModel = new LanguageViewModel();
            }

            ActivateItem(LanguageSettingsViewModel);
        }
    }
}
