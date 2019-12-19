using System;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Caliburn.Micro;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Users;

// TODO: see next comments @Chino
// Password 1: Test123
// Password 2: Test123!
// Password 1: Test123!
// Error message under Password 2
// TODO add summeries
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
        private string _buttonBackground;
        
        /// <value>
        /// The background of active buttons
        /// </value>
        private string _buttonBackGroundActive;
        
        #endregion
        
        #region Properties
        
        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        /// <value>
        /// The background color of all buttons
        /// </value>
        public string ButtonBackground
        {
            get => _buttonBackground;
            set
            {
                _buttonBackground = value;
                NotifyOfPropertyChange(nameof(ButtonBackground));
            }
        }

        /// <value>
        /// The background color of all active buttons
        /// </value>
        public string ButtonBackgroundActive
        {
            get => _buttonBackGroundActive;
            set
            {
                _buttonBackGroundActive = value;
                NotifyOfPropertyChange(nameof(ButtonBackgroundActive));
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
                    if (string.IsNullOrEmpty(Email) || IsValidEmail(Email) == false)
                    {
                        result = "Please enter your email!";
                    }

                }

                if (propertyName == "Password")
                {
                    if (!string.IsNullOrWhiteSpace(Password))
                    {
                        if (ValidatePassword(Password) == false)
                        {
                            result = "Please enter valid password!";
                        }
                    }
                }


                if (propertyName == "PasswordConfirm")
                {
                    if (!string.IsNullOrWhiteSpace(PasswordConfirm))
                    {
                        if (ComparePasswordEquality(PasswordConfirm, Password) == false)
                        {
                            result = "Password are not equal!";
                        }
                    }
                }

                return result;
            }
        }

        public string Error =>
            "An unknown error occured!";

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
                if (User.Register(Email, tbPassword, FirstName, LastName))
                {
                    MessageBox.Show("Registration successful!");
                    (GetView() as Window)?.Close();
                }
                else
                {
                    MessageBox.Show("Registration failed! Please try again.");
                }
            }
            else
            {
                MessageBox.Show("Please fill in the fields with errors");
            }
        }

        #endregion

        #region Events

        protected override void OnActivate()
        {
            ButtonBackground = SettingsConstant.BUTTON_BACKGROUND;
            ButtonBackgroundActive = SettingsConstant.BUTTON_BACKGROUND_ACTIVE;
        }

        #endregion
    }
}
