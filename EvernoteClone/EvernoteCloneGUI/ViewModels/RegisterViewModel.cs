using System;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Caliburn.Micro;
using EvernoteCloneLibrary.Users;

// TODO: see next comments
// Password 1: Test123
// Password 2: Test123!
// Password 1: Test123!
// Error message under Password 2
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
        
        #endregion
        
        #region Properties
        
        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

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
                    if (PasswordConfirm != null)
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
            throw new NotImplementedException();

        #endregion
        #region Validation


        bool IsValidEmail(string email)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == email;
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
                    MessageBox.Show("Registration succesful!");
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
    }
}
