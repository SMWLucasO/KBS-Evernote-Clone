using Caliburn.Micro;
using EvernoteCloneGUI.ViewModels.Settings;
using System;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Settings;

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
        /// Contains the StandardsViewModel that will be shown after the corresponding button is clicked.
        /// </value>
        public StandardsViewModel StandardsSettingsViewModel { get; set; }

        /// <value>
        /// Contains the LayoutViewModel that will be shown after the corresponding button is clicked.
        /// </value>
        public LayoutViewModel LayoutSettingsViewModel { get; set; }

        /// <value>
        /// Contains the LanguageViewModel that will be shown after the corresponding button is clicked.
        /// </value>
        public LanguageViewModel LanguageSettingsViewModel { get; set; }

        #endregion

        #region Background's

        /// <value>
        /// The property that is used to update the background color of the 'Synchronize' button
        /// </value>
        public string ButtonBackgroundActive
        {
            get => _buttonBackgroundActive;
            set
            {
                _buttonBackgroundActive = value;
                NotifyOfPropertyChange(nameof(ButtonBackgroundActive));
            }
        }

        /// <value>
        /// The property that is used to update the background color of the 'Editor' button
        /// </value>
        public string EditorButtonBackground
        {
            get => _editorBtnBackGround;
            set
            {
                _editorBtnBackGround = value;
                NotifyOfPropertyChange(nameof(EditorButtonBackground));
            }
        }

        /// <value>
        /// The property that is used to update the background color of the 'Standards' button
        /// </value>
        public string StandardsButtonBackground
        {
            get => _standardsBtnBackGround;
            set
            {
                _standardsBtnBackGround = value;
                NotifyOfPropertyChange(nameof(StandardsButtonBackground));
            }
        }

        /// <value>
        /// The property that is used to update the background color of the 'Layout' button
        /// </value>
        public string LayoutButtonBackground
        {
            get => _layoutBtnBackGround;
            set
            {
                _layoutBtnBackGround = value;
                NotifyOfPropertyChange(nameof(LayoutButtonBackground));
            }
        }

        /// <value>
        /// The property that is used to update the background color of the 'Language' button
        /// </value>
        public string LanguageButtonBackground
        {
            get => _languageBtnBackGround;
            set
            {
                _languageBtnBackGround = value;
                NotifyOfPropertyChange(nameof(LanguageButtonBackground));
            }
        }

        #endregion

        #endregion

        #region Variables

        /// <value>
        /// The variable that contains the background color of the 'Synchronize' button
        /// </value>
        private string _buttonBackgroundActive;

        /// <value>
        /// The variable that contains the background color of the 'Editor' button
        /// </value>
        private string _editorBtnBackGround;

        /// <value>
        /// The variable that contains the background color of the 'Standards' button
        /// </value>
        private string _standardsBtnBackGround;

        /// <value>
        /// The variable that contains the background color of the 'Layout' button
        /// </value>
        private string _layoutBtnBackGround;

        /// <value>
        /// The variable that contains the background color of the 'Language' button
        /// </value>
        private string _languageBtnBackGround;

        /// <value>
        /// The NoteFeverViewModel
        /// </value>
        private NoteFeverViewModel _noteFeverViewModel;

        /// <value>
        /// The view that is selected
        /// </value>
        private string _selectedView;

        #endregion

        #region Constructor

        public SettingsViewModel(NoteFeverViewModel noteFeverViewModel) =>
            _noteFeverViewModel = noteFeverViewModel;

        #endregion

        #region Events

        /// <summary>
        /// When this screen is activated, open the language settings tab (by default)
        /// </summary>
        protected override void OnActivate()
        {
            OpenLanguageSettings();
            ButtonBackgroundActive = SettingsConstant.BUTTON_BACKGROUND_ACTIVE;
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
            EditorButtonBackground = SettingsConstant.BUTTON_BACKGROUND;
            StandardsButtonBackground = SettingsConstant.BUTTON_BACKGROUND;
            LayoutButtonBackground = SettingsConstant.BUTTON_BACKGROUND;
            LanguageButtonBackground = SettingsConstant.BUTTON_BACKGROUND;

            if (selectedButton != null)
            {
                selectedButton = SettingsConstant.BUTTON_BACKGROUND_ACTIVE;
            }

            // This will update the GUI layout
            NotifyOfPropertyChange(nameof(EditorButtonBackground));
            NotifyOfPropertyChange(nameof(StandardsButtonBackground));
            NotifyOfPropertyChange(nameof(LayoutButtonBackground));
            NotifyOfPropertyChange(nameof(LanguageButtonBackground));
        }

        public void UpdateLayout()
        {
            EditorButtonBackground = SettingsConstant.BUTTON_BACKGROUND;
            StandardsButtonBackground = SettingsConstant.BUTTON_BACKGROUND;
            LayoutButtonBackground = SettingsConstant.BUTTON_BACKGROUND;
            LanguageButtonBackground = SettingsConstant.BUTTON_BACKGROUND;
            ButtonBackgroundActive = SettingsConstant.BUTTON_BACKGROUND_ACTIVE;

            if (_selectedView == nameof(_editorBtnBackGround))
                EditorButtonBackground = SettingsConstant.BUTTON_BACKGROUND_ACTIVE;
            if (_selectedView == nameof(_standardsBtnBackGround))
                StandardsButtonBackground = SettingsConstant.BUTTON_BACKGROUND_ACTIVE;
            if (_selectedView == nameof(_layoutBtnBackGround))
                LayoutButtonBackground = SettingsConstant.BUTTON_BACKGROUND_ACTIVE;
            if (_selectedView == nameof(_languageBtnBackGround))
                LanguageButtonBackground = SettingsConstant.BUTTON_BACKGROUND_ACTIVE;

            _noteFeverViewModel.UpdateColors();
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
            else if (newViewModel == typeof(StandardsViewModel))
            {
                if (StandardsSettingsViewModel == null)
                    screen = (StandardsSettingsViewModel = (StandardsViewModel)Activator.CreateInstance(newViewModel));
                else
                    screen = StandardsSettingsViewModel;
            }
            else if (newViewModel == typeof(LayoutViewModel))
            {
                if (LayoutSettingsViewModel == null)
                    screen = (LayoutSettingsViewModel = (LayoutViewModel)Activator.CreateInstance(newViewModel));
                else
                    screen = LayoutSettingsViewModel;
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
        /// This synchronizes and refreshes all the settings (with database if user is logged in)
        /// </summary>
        public void Synchronize()
        {
            Setting.Load();

            EditorSettingsViewModel = null;
            StandardsSettingsViewModel = null;
            
            if (_selectedView == nameof(_editorBtnBackGround))
                OpenEditorSettings();
            if (_selectedView == nameof(_standardsBtnBackGround))
                OpenStandardsSettings();
            if (_selectedView == nameof(_layoutBtnBackGround))
                OpenLayoutSettings();
            if (_selectedView == nameof(_languageBtnBackGround))
                OpenLanguageSettings();
            
            _noteFeverViewModel.UpdateColors();
        }

        /// <summary>
        /// This changes the background of the 'Editor' button
        /// This also activates the 'Editor' tab
        /// </summary>
        public void OpenEditorSettings()
        {
            _selectedView = nameof(_editorBtnBackGround);
            ChangeBackground(ref _editorBtnBackGround);
            ChangeTab(typeof(EditorViewModel));
        }

        /// <summary>
        /// This changes the background of the 'Standards' button
        /// This also activates the 'Standards' tab
        /// </summary>
        public void OpenStandardsSettings()
        {
            _selectedView = nameof(_standardsBtnBackGround);
            ChangeBackground(ref _standardsBtnBackGround);
            ChangeTab(typeof(StandardsViewModel));
        }

        /// <summary>
        /// This changes the background of the 'Layout' button
        /// This also activates the 'Layout' tab
        /// </summary>
        public void OpenLayoutSettings()
        {
            _selectedView = nameof(_layoutBtnBackGround);
            ChangeBackground(ref _layoutBtnBackGround);
            ChangeTab(typeof(LayoutViewModel));
        }

        /// <summary>
        /// This changes the background of the 'Language' button
        /// This also activates the 'Language' tab
        /// </summary>
        public void OpenLanguageSettings()
        {
            _selectedView = nameof(_languageBtnBackGround);
            ChangeBackground(ref _languageBtnBackGround);
            ChangeTab(typeof(LanguageViewModel));
        }
        
        #endregion
    }
}
