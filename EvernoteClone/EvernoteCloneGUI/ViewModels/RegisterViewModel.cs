using System;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using EvernoteCloneLibrary.Users;

// TODO: see next comments
// Password 1: Test123
// Password 2: Test123!
// Password 1: Test123!
// Error message under Password 2
namespace EvernoteCloneGUI.ViewModels
{
    public class RegisterViewModel : IDataErrorInfo
    {
        #region Variables
        // TODO change variable names according to coding conventions
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _password;
        private string _passwordConfirm;
        private static int _minimumLength = 5;
        private static int _upperLength = 1;
        private static int _lowerLength = 1;
        private static int _specialChar = 1;
        private static int _numericLength = 2;
        #endregion
        #region Properties
        //Email property
        public string Email
        {
            get => _email;
            set => _email = value;
        }

        //Password property
        public string Password
        {
            get => _password;
            set => _password = value;
        }

        //Password confirm property
        public string PasswordConfirm
        {
            get => _passwordConfirm;
            set => _passwordConfirm = value;
        }

        //First name property
        public string FirstName
        {
            get => _firstName;
            set => _firstName = value;
        }

        //Last name property
        public string LastName
        {
            get => _lastName;
            set => _lastName = value;
        }
        #endregion
        #region Show error messages
        //Show error message
        public string this[string propertyName]
        {
            get
            {
                string result = null;

                // Email
                if (propertyName == "Email")
                    if (string.IsNullOrEmpty(Email) || IsValidEmail(Email) == false)
                        result = "Please enter your email!";
                
                // Password
                if (propertyName == "Password")
                    if (!string.IsNullOrWhiteSpace(Password))
                        if (ValidatePassword(Password) == false)
                            result = "Please enter valid password!";
                
                // PasswordConfirm
                if (propertyName == "PasswordConfirm")
                    if (PasswordConfirm != null)
                        if (PasswordTheSame(PasswordConfirm,Password) == false)
                            result = "Password are not equal!";

                return result;
            }
        }

        //Throws Error
        public string Error => 
            throw new NotImplementedException();
        #endregion
        #region Method to check if email is valid
        //Check if email is valid 
        bool IsValidEmail(string email)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == email;
            }
            catch { return false; }
        }
        #endregion
        #region Validates password
        //method to validate inserted password
        public bool ValidatePassword(string password)
        {
            if (password.Length < _minimumLength)
                return false;
            if (UpperCount(password) < _upperLength)
                return false;
            if (LowerCount(password) < _lowerLength)
                return false;
            if (NumericCount(password) < _numericLength)
                return false;
            if (SpecialCharCount(password) < _specialChar)
                return false;
            return true;
        }

        // Checks if password are the same
        public bool PasswordTheSame (string password1, string password2)
        {
            if (password1.Equals(password2))
                return true;
            return false;
        }
        #endregion
        #region Methods to count specifics in password
        //Counts uppercase characters in password
        private static int UpperCount(string password) =>
            Regex.Matches(password, "[A-Z]").Count;

        //Counts lowercase characters in password
        private static int LowerCount(string password) =>
            Regex.Matches(password, "[a-z]").Count;

        //Counts numeric characters in password
        private static int NumericCount(string password) =>
            Regex.Matches(password, "[0-9]").Count;

        //Counts special characters in password

        private static int SpecialCharCount(string password) =>
            Regex.Matches(password, @"[^0-9a-zA-Z\._]").Count;
        #endregion
        #region Password encrypter
        //Encrypt password using md5 hasing algorithm
        public string Encryption(string password)
        {
            //encrypt the given password string into Encrypted data  
            byte[] encrypt = new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(password));
            StringBuilder encryptData = new StringBuilder();

            //Create a new string by using the encrypted data  
            for (int i = 0; i < encrypt.Length; i++)
                encryptData.Append(encrypt[i].ToString());
            return encryptData.ToString();
        }
        #endregion
        #region Register button
        //Register button event
        public void Register()
        {
            string tbFirstName = FirstName;
            string tbLastName = LastName;
            string tbEmail = Email;
            string tbPassword = Encryption(Password);

            if (IsValidEmail(Email) && ValidatePassword(Password) && PasswordTheSame(PasswordConfirm, Password))
            {
                if (User.Register(tbEmail, tbPassword, tbFirstName, tbLastName))
                    MessageBox.Show("Registration succesful!");
                else
                    MessageBox.Show("Registration failed! Please try again.");
            }
            else
                MessageBox.Show("Please fill the fields with errors");

        }
        #endregion
    }
}
