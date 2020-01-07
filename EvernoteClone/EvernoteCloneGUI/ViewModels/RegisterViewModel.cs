using System.ComponentModel;
using System.Windows;
using System.Text.RegularExpressions;
using Caliburn.Micro;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Users;

namespace EvernoteCloneGUI.ViewModels
{
    public class RegisterViewModel : Screen, IDataErrorInfo
    {
        #region Variables
        
        private static readonly int _minimumLength = 5;
        private static readonly int _upperLength = 1;
        private static readonly int _lowerLength = 1;
        private static readonly int _specialChar = 1;
        private static readonly int _numericLength = 2;
        
        /// <value>
        /// The background of buttons
        /// </value>
        private string _buttonBackgroundColorColor;
        
        /// <value>
        /// The background of active buttons
        /// </value>
        private string _buttonAccentColor;
        
        #endregion
        
        #region Properties
        
        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        public bool Registered { get; set; }
        
        /// <value>
        /// The background color of all buttons
        /// </value>
        public string ButtonBackgroundColor
        {
            get => _buttonBackgroundColorColor;
            set
            {
                _buttonBackgroundColorColor = value;
                NotifyOfPropertyChange(nameof(ButtonBackgroundColor));
            }
        }

        /// <value>
        /// The background color of all active buttons
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

        #endregion
        
        #region Show error messages
        
        public string this[string propertyName]
        {
            get
            {
                string result = null;

                if (propertyName == "Email")
                {
                    if (string.IsNullOrWhiteSpace(Email) || IsValidEmail(Email) == false)
                    {
                        result = Properties.Settings.Default.RegisterViewModelPleaseEmail;
                    }

                }

                if (propertyName == "Password")
                {
                    if (!string.IsNullOrWhiteSpace(Password))
                    {
                        if (ValidatePassword(Password) == false)
                        {
                            result = Properties.Settings.Default.RegisterViewModelPleasePassword;
                        }
                    }
                }


                if (propertyName == "PasswordConfirm")
                {
                    if (!string.IsNullOrWhiteSpace(PasswordConfirm))
                    {
                        if (ComparePasswordEquality(PasswordConfirm, Password) == false)
                        {
                            result = Properties.Settings.Default.RegisterViewModelNotEqual;
                        }
                    }
                }

                return result;
            }
        }

        public string Error =>
            Properties.Settings.Default.RegisterViewModelUnknown;

        #endregion

        #region Validation


        bool IsValidEmail(string email)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(email);

                string[] splittedAtDotAfterAt = email.Split('@')[1].Split('.');
                bool containsValidDomain = splittedAtDotAfterAt.Length > 1 
                                           && !splittedAtDotAfterAt[splittedAtDotAfterAt.Length-1].EndsWith(".");
                
                return address.Address == email && containsValidDomain;
            }
            catch { return false; }
        }

        public bool ValidatePassword(string password)
        {
            return password.Length >= _minimumLength && CountUpperCharacters(password) >= _upperLength && CountLowerCharacters(password) >= _lowerLength
                && CountNumericCharacters(password) >= _numericLength && CountSpecialCharacters(password) >= _specialChar;
        }

        public bool ComparePasswordEquality(string password, string confirmationPassword)
        {
            return (password.Equals(confirmationPassword));
        }

        #region Validation helpers

        private static int CountUpperCharacters(string password) =>
            Regex.Matches(password, "[A-Z]").Count;

        private static int CountLowerCharacters(string password) =>
            Regex.Matches(password, "[a-z]").Count;

        private static int CountNumericCharacters(string password) =>
            Regex.Matches(password, "[0-9]").Count;


        private static int CountSpecialCharacters(string password) =>
            Regex.Matches(password, @"[^0-9a-zA-Z\._]").Count;

        #endregion

        #endregion

        #region Registration event handling
        
        public void Register()
        {
            string tbPassword = User.Encryption(Password);

            if (IsValidEmail(Email) && ValidatePassword(Password) && ComparePasswordEquality(PasswordConfirm, Password))
            {
                if (new UserRepository().CheckIfUserExists(Email) == null)
                {
                    if (User.Register(Email, tbPassword, FirstName, LastName))
                    {
                        MessageBox.Show(Properties.Settings.Default.RegisterViewModelRegisterSuccessful,
                            Properties.Settings.Default.MessageBoxTitleSuccessful);
                        DefaultSettingsConstant.CopyDefaults();
                        Registered = true;
                        (GetView() as Window)?.Close();
                    }
                    else
                    {
                        MessageBox.Show(Properties.Settings.Default.RegisterViewModelRegistrationFailed,
                            Properties.Settings.Default.MessageBoxTitleFailed, MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
                    }
                }
                else
                {
                    MessageBox.Show(Properties.Settings.Default.RegisterViewModelAlreadyExists,
                        Properties.Settings.Default.MessageBoxTitleFailed, MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show(Properties.Settings.Default.RegisterViewModelFieldsWithErrors, Properties.Settings.Default.MessageBoxTitleWarning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        #endregion

        #region Events

        protected override void OnActivate()
        {
            ButtonBackgroundColor = SettingsConstant.BUTTON_BACKGROUND_COLOR;
            ButtonAccentColor = SettingsConstant.ACCENT_COLOR;
        }

        #endregion
    }
}
