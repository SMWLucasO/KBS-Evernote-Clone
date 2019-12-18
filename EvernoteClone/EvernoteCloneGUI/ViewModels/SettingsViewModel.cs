using Caliburn.Micro;
using EvernoteCloneGUI.ViewModels.Settings;
using System;

namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// This is a ViewModel that makes the 'Tab' Control work
    /// </summary>
    public class SettingsViewModel : Conductor<Screen>.Collection.OneActive
    {
        #region Properties

        #region ViewModels

        /// <value>
        /// Contains the EditorViewModel that will be shown after the corresponding button is clicked.
        /// </value>
        public EditorViewModel EditorSettingsViewModel { get; set; }

        /// <value>
        /// Contains the LayoutViewModel that will be shown after the corresponding button is clicked.
        /// </value>
        public LayoutViewModel LayoutSettingsViewModel { get; set; }

        /// <value>
        /// Contains the ColorsViewModel that will be shown after the corresponding button is clicked.
        /// </value>
        public ColorsViewModel ColorsSettingsViewModel { get; set; }

        /// <value>
        /// Contains the LanguageViewModel that will be shown after the corresponding button is clicked.
        /// </value>
        public LanguageViewModel LanguageSettingsViewModel { get; set; }

        #endregion

        #region Background's

        /// <value>
        /// The property that is used to update the background color of the 'Editor' button
        /// </value>
        public string EditorButtonBackground { get => _editorBtnBackGround; set => _editorBtnBackGround = value; }

        /// <value>
        /// The property that is used to update the background color of the 'Layout' button
        /// </value>
        public string LayoutButtonBackground { get => _layoutBtnBackGround; set => _layoutBtnBackGround = value; }

        /// <value>
        /// The property that is used to update the background color of the 'Colors' button
        /// </value>
        public string ColorsButtonBackground { get => _colorsBtnBackGround; set => _colorsBtnBackGround = value; }

        /// <value>
        /// The property that is used to update the background color of the 'Language' button
        /// </value>
        public string LanguageButtonBackground { get => _languageBtnBackGround; set => _languageBtnBackGround = value; }

        #endregion

        #endregion

        #region Variables

        /// <value>
        /// The variable that contains the background color of the 'Editor' button
        /// </value>
        private string _editorBtnBackGround;

        /// <value>
        /// The variable that contains the background color of the 'Layout' button
        /// </value>
        private string _layoutBtnBackGround;

        /// <value>
        /// The variable that contains the background color of the 'Colors' button
        /// </value>
        private string _colorsBtnBackGround;

        /// <value>
        /// The variable that contains the background color of the 'Language' button
        /// </value>
        private string _languageBtnBackGround;

        #endregion

        #region Events

        /// <summary>
        /// When this screen is activated, open the language settings tab (by default)
        /// </summary>
        protected override void OnActivate()
        {
            base.OnActivate();
            OpenLanguageSettings();
        }

        #endregion

        #region Logic for changing background and activating tab
        
        /// <summary>
        /// Sets default background of all buttons
        /// Changes the background of the given button.
        /// </summary>
        /// <param name="selectedButton">The button the background should be changed of</param>
        private void ChangeBackground(ref string selectedButton)
        {
            EditorButtonBackground = "#404040";
            LayoutButtonBackground = "#404040";
            ColorsButtonBackground = "#404040";
            LanguageButtonBackground = "#404040";

            if (selectedButton != null)
            {
                selectedButton = "#0052cc";
            }

            NotifyOfPropertyChange(nameof(EditorButtonBackground));
            NotifyOfPropertyChange(nameof(LayoutButtonBackground));
            NotifyOfPropertyChange(nameof(ColorsButtonBackground));
            NotifyOfPropertyChange(nameof(LanguageButtonBackground));
        }

        /// <summary>
        /// Change the tab to the new tab
        /// </summary>
        /// <param name="newViewModel">The type of the new settings window</param>
        public void ChangeTab(Type newViewModel)
        {
            Screen screen;

            if (newViewModel == typeof(EditorViewModel))
            {
                if (EditorSettingsViewModel == null)
                    screen = (EditorSettingsViewModel = (EditorViewModel)Activator.CreateInstance(newViewModel));
                else
                    screen = EditorSettingsViewModel;
            }
            else if (newViewModel == typeof(LayoutViewModel))
            {
                if (LayoutSettingsViewModel == null)
                    screen = (LayoutSettingsViewModel = (LayoutViewModel)Activator.CreateInstance(newViewModel));
                else
                    screen = LayoutSettingsViewModel;
            }
            else if (newViewModel == typeof(ColorsViewModel))
            {
                if (ColorsSettingsViewModel == null)
                    screen = (ColorsSettingsViewModel = (ColorsViewModel)Activator.CreateInstance(newViewModel));
                else
                    screen = ColorsSettingsViewModel;
            }
            else
            {
                if (LanguageSettingsViewModel == null)
                    screen = (LanguageSettingsViewModel = (LanguageViewModel)Activator.CreateInstance(newViewModel));
                else
                    screen = LanguageSettingsViewModel;
            }

            ActivateItem(screen);
        }
        
        #endregion

        #region Button handlers
        
        /// <summary>
        /// This changes the background of the 'Editor' button
        /// This also activates the 'Editor' tab
        /// </summary>
        public void OpenEditorSettings()
        {
            ChangeBackground(ref _editorBtnBackGround);
            ChangeTab(typeof(EditorViewModel));
        }

        /// <summary>
        /// This changes the background of the 'Layout' button
        /// This also activates the 'Layout' tab
        /// </summary>
        public void OpenLayoutSettings()
        {
            ChangeBackground(ref _layoutBtnBackGround);
            ChangeTab(typeof(LayoutViewModel));
        }

        /// <summary>
        /// This changes the background of the 'Colors' button
        /// This also activates the 'Colors' tab
        /// </summary>
        public void OpenColorsSettings()
        {
            ChangeBackground(ref _colorsBtnBackGround);
            ChangeTab(typeof(ColorsViewModel));
        }

        /// <summary>
        /// This changes the background of the 'Language' button
        /// This also activates the 'Language' tab
        /// </summary>
        public void OpenLanguageSettings()
        {
            ChangeBackground(ref _languageBtnBackGround);
            ChangeTab(typeof(LanguageViewModel));
        }
        
        #endregion
    }
}
