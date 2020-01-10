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
        public string ButtonAccentColor
        {
            get => _buttonAccentColor;
            set
            {
                _buttonAccentColor = value;
                NotifyOfPropertyChange(nameof(ButtonAccentColor));
            }
        }

        /// <value>
        /// The property that is used to update the background color of the 'Editor' button
        /// </value>
        public string EditorButtonBackgroundColor
        {
            get => _editorBtnBackGround;
            set
            {
                _editorBtnBackGround = value;
                NotifyOfPropertyChange(nameof(EditorButtonBackgroundColor));
            }
        }

        /// <value>
        /// The property that is used to update the background color of the 'Standards' button
        /// </value>
        public string StandardsButtonBackgroundColor
        {
            get => _standardsBtnBackGround;
            set
            {
                _standardsBtnBackGround = value;
                NotifyOfPropertyChange(nameof(StandardsButtonBackgroundColor));
            }
        }

        /// <value>
        /// The property that is used to update the background color of the 'Layout' button
        /// </value>
        public string LayoutButtonBackgroundColor
        {
            get => _layoutBtnBackGround;
            set
            {
                _layoutBtnBackGround = value;
                NotifyOfPropertyChange(nameof(LayoutButtonBackgroundColor));
            }
        }

        /// <value>
        /// The property that is used to update the background color of the 'Language' button
        /// </value>
        public string LanguageButtonBackgroundColor
        {
            get => _languageBtnBackGround;
            set
            {
                _languageBtnBackGround = value;
                NotifyOfPropertyChange(nameof(LanguageButtonBackgroundColor));
            }
        }

        #endregion

        #endregion

        #region Variables

        /// <value>
        /// The variable that contains the background color of the 'Synchronize' button
        /// </value>
        private string _buttonAccentColor;

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
            ButtonAccentColor = SettingsConstant.ACCENT_COLOR;
        }

        /// <summary>
        /// Refresh settings (only load from local).
        /// We have to do this, because the user may change some settings but not click on Apply Changes button
        /// </summary>
        /// <param name="close"></param>
        protected override void OnDeactivate(bool close) =>
            Setting.Load(true);

        #endregion

        #region Logic for changing background and activating tab
        
        /// <summary>
        /// Sets default background of all buttons
        /// Changes the background of the given button.
        /// </summary>
        /// <param name="selectedButton">The button the background should be changed of</param>
        private void ChangeBackground(ref string selectedButton)
        {
            EditorButtonBackgroundColor = SettingsConstant.BUTTON_BACKGROUND_COLOR;
            StandardsButtonBackgroundColor = SettingsConstant.BUTTON_BACKGROUND_COLOR;
            LayoutButtonBackgroundColor = SettingsConstant.BUTTON_BACKGROUND_COLOR;
            LanguageButtonBackgroundColor = SettingsConstant.BUTTON_BACKGROUND_COLOR;

            if (selectedButton != null)
            {
                selectedButton = SettingsConstant.ACCENT_COLOR;
            }

            // This will update the GUI layout
            NotifyOfPropertyChange(nameof(EditorButtonBackgroundColor));
            NotifyOfPropertyChange(nameof(StandardsButtonBackgroundColor));
            NotifyOfPropertyChange(nameof(LayoutButtonBackgroundColor));
            NotifyOfPropertyChange(nameof(LanguageButtonBackgroundColor));
        }

        /// <summary>
        /// Retrieves the new colors from the settings and applies the changes.
        /// </summary>
        public void UpdateLayout()
        {
            EditorButtonBackgroundColor = SettingsConstant.BUTTON_BACKGROUND_COLOR;
            StandardsButtonBackgroundColor = SettingsConstant.BUTTON_BACKGROUND_COLOR;
            LayoutButtonBackgroundColor = SettingsConstant.BUTTON_BACKGROUND_COLOR;
            LanguageButtonBackgroundColor = SettingsConstant.BUTTON_BACKGROUND_COLOR;
            ButtonAccentColor = SettingsConstant.ACCENT_COLOR;

            if (_selectedView == nameof(_editorBtnBackGround))
            {
                EditorButtonBackgroundColor = SettingsConstant.ACCENT_COLOR;
            }
            if (_selectedView == nameof(_standardsBtnBackGround))
            {
                StandardsButtonBackgroundColor = SettingsConstant.ACCENT_COLOR;
            }
            if (_selectedView == nameof(_layoutBtnBackGround))
            {
                LayoutButtonBackgroundColor = SettingsConstant.ACCENT_COLOR;
            }
            if (_selectedView == nameof(_languageBtnBackGround))
            {
                LanguageButtonBackgroundColor = SettingsConstant.ACCENT_COLOR;
            }

            if (EditorSettingsViewModel != null)
            {
                EditorSettingsViewModel.BackgroundColorSettings = SettingsConstant.BACKGROUND_COLOR_SETTINGS;
            }
            if (StandardsSettingsViewModel != null)
            {
                StandardsSettingsViewModel.BackgroundColorSettings = SettingsConstant.BACKGROUND_COLOR_SETTINGS;
            }
            if (LayoutSettingsViewModel != null)
            {
                LayoutSettingsViewModel.BackgroundColorSettings = SettingsConstant.BACKGROUND_COLOR_SETTINGS;
            }
            if (LanguageSettingsViewModel != null)
            {
                LanguageSettingsViewModel.BackgroundColorSettings = SettingsConstant.BACKGROUND_COLOR_SETTINGS;
            }

            _noteFeverViewModel.UpdateColors();
        }

        /// <summary>
        /// Change the tab to the new tab
        /// </summary>
        /// <param name="newViewModel">The type of the new settings window</param>
        public void ChangeTab(Type newViewModel)
        {
            SettingViewModel screen;

            if (newViewModel == typeof(EditorViewModel))
            {
                if (EditorSettingsViewModel == null)
                {
                    screen = (EditorSettingsViewModel = (EditorViewModel) Activator.CreateInstance(newViewModel));
                }
                else
                {
                    screen = EditorSettingsViewModel;
                }
            }
            else if (newViewModel == typeof(StandardsViewModel))
            {
                if (StandardsSettingsViewModel == null)
                {
                    screen = (StandardsSettingsViewModel = (StandardsViewModel) Activator.CreateInstance(newViewModel));
                }
                else
                {
                    screen = StandardsSettingsViewModel;
                }
            }
            else if (newViewModel == typeof(LayoutViewModel))
            {
                if (LayoutSettingsViewModel == null)
                {
                    screen = (LayoutSettingsViewModel = (LayoutViewModel) Activator.CreateInstance(newViewModel));
                }
                else
                {
                    screen = LayoutSettingsViewModel;
                }
            }
            else
            {
                if (LanguageSettingsViewModel == null)
                {
                    screen = (LanguageSettingsViewModel = (LanguageViewModel) Activator.CreateInstance(newViewModel));
                }
                else
                {
                    screen = LanguageSettingsViewModel;
                }
            }

            screen.BackgroundColorSettings = SettingsConstant.BACKGROUND_COLOR_SETTINGS;
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
            {
                OpenEditorSettings();
            }
            if (_selectedView == nameof(_standardsBtnBackGround))
            {
                OpenStandardsSettings();
            }
            if (_selectedView == nameof(_layoutBtnBackGround))
            {
                OpenLayoutSettings();
            }
            if (_selectedView == nameof(_languageBtnBackGround))
            {
                OpenLanguageSettings();
            }

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
