using System.ComponentModel;
using System.Windows;
using System.Text.RegularExpressions;
using Caliburn.Micro;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Users;

namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// ViewModel which handles the interaction for the RegisterViewModel
    /// </summary>
    public class RegisterViewModel : Screen, IDataErrorInfo
    {
        #region Variables
        
        // password rules
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
        
        /// <value>
        /// Property which is bound to the 'Email' TextBox in the register ViewModel, contains the text contained
        /// within this TextBox.
        /// </value>
        public string Email { get; set; }

        /// <value>
        /// Property which is bound to the 'password' PasswordBox in the register ViewModel, contains the text contained
        /// within this PasswordBox.
        /// </value>
        public string Password { get; set; }

        /// <value>
        /// Property which is bound to the 'confirm password' PasswordBox in the register ViewModel, contains the text contained
        /// within this PasswordBox.
        /// </value>
        public string PasswordConfirm { get; set; }

        /// <value>
        /// Property which is bound to the 'FirstName' TextBox in the register ViewModel, contains the text contained
        /// within this TextBox. It is important to note that specifying a FirstName property isn't required
        /// to register an account.
        /// </value>
        public string FirstName { get; set; }

        /// <value>
        /// Property which is bound to the 'LastName' TextBox in the register ViewModel, contains the text contained
        /// within this TextBox. It is important to note that specifying a LastName property isn't required
        /// to register an account.
        /// </value>
        public string LastName { get; set; }
        
        /// <value>
        /// Boolean indicating that an account has been registered.
        /// </value>
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
        
        /// <summary>
        /// Operator overload for verifying whether the inserted data is valid.
        /// The user registering will be notified of any mistakes too.
        /// </summary>
        /// <param name="propertyName"></param>
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


        /// <summary>
        /// Validates whether the email given is valid.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>A boolean determining if the mail is valid</returns>
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

        /// <summary>
        /// Validates whether the given password adheres to all requirements for a successful registration.
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Boolean indicating if the password is valid</returns>
        public bool ValidatePassword(string password)
        {
            return password.Length >= _minimumLength && CountUpperCharacters(password) >= _upperLength && CountLowerCharacters(password) >= _lowerLength
                && CountNumericCharacters(password) >= _numericLength && CountSpecialCharacters(password) >= _specialChar;
        }

        /// <summary>
        /// Validates whether the given passwords are equal.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="confirmationPassword"></param>
        /// <returns>Boolean indicating if the passwords are equal</returns>
        public bool ComparePasswordEquality(string password, string confirmationPassword)
        {
            return (password.Equals(confirmationPassword));
        }

        #region Validation helpers

        /// <summary>
        /// Helper method which counts all uppercase characters in the given input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>An integer containing the count of uppercase characters</returns>
        private static int CountUpperCharacters(string input) =>
            Regex.Matches(input, "[A-Z]").Count;

        /// <summary>
        /// Helper method which counts all lowercase characters in the given input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>An integer containing the count of lowercase characters</returns>
        private static int CountLowerCharacters(string input) =>
            Regex.Matches(input, "[a-z]").Count;

        /// <summary>
        /// Helper method which counts all numeric characters in the given input.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>An integer containing the count of numeric characters</returns>
        private static int CountNumericCharacters(string input) =>
            Regex.Matches(input, "[0-9]").Count;


        /// <summary>
        /// Helper method which counts all special characters in the given input
        /// </summary>
        /// <param name="input"></param>
        /// <returns>An integer containing the count of special characters</returns>
        private static int CountSpecialCharacters(string input) =>
            Regex.Matches(input, @"[^0-9a-zA-Z\._]").Count;

        #endregion

        #endregion

        #region Registration event handling
        
        /// <summary>
        /// Method for registering a new account, encrypts the password and validates the given input.
        /// Messages are given depending on the outcome of the registration. 
        /// </summary>
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

        /// <summary>
        /// Set the button background and accent colors on activation of the ViewModel
        /// </summary>
        protected override void OnActivate()
        {
            ButtonBackgroundColor = SettingsConstant.BUTTON_BACKGROUND_COLOR;
            ButtonAccentColor = SettingsConstant.ACCENT_COLOR;
        }

        #endregion
    }
}
