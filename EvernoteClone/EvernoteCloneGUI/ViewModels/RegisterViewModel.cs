using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using EvernoteCloneLibrary.Users;

namespace EvernoteCloneGUI.ViewModels
{
    public class RegisterViewModel : IDataErrorInfo
    {
        private string firstName;
        private string lastName;
        private string email;
        private string password;
        private string passwordConfirm;
        private bool isEnabled;
        private static int minimumLength = 5;
        private static int upperLength = 1;
        private static int lowerLength = 1;
        private static int specialChar = 1;
        private static int numericLength = 2;



        #region Properties
        //Email property
        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        //Password property
        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        //Password confirm property
        public string PasswordConfirm
        {
            get { return passwordConfirm; }
            set { passwordConfirm = value; }
        }

        //First name property
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        //Last name property
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }




        #endregion

        #region Show error messages
        //Show error message
        public string this[string PropertyName]
        {
            get
            {

                string result = null;

                if (PropertyName == "Email")
                {
                    if (string.IsNullOrEmpty(Email) || isValidEmail(Email) == false)
                        result = "Please enter your email!";
                }
                if (PropertyName == "Password")
                {
                    try
                    {
                        if (ValidatePassword(Password) == false)
                        {
                            result = "Please enter valid password!";
                        }
                    }
                    catch (Exception e)
                    {
                        result = "";
                    }
                }
                if (PropertyName == "Password")
                {
                    try
                    {
                        if (ValidatePassword(Password) == false)
                        {
                            result = "Please enter valid password!";
                        }
                    }
                    catch(Exception e)
                    {
                        result = "";
                    }
                }
                if (PropertyName == "PasswordConfirm")
                {
                    if (PasswordConfirm != null)
                        if (PasswordTheSame(PasswordConfirm,Password) == false)
                            result = "Password are not equal!";
                }

               
                return result;
            }
        }

        //Throws Error
        public string Error
        {

            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Method to check if email is valid
        //Check if email is valid 
        bool isValidEmail(string email)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == email;
            }
            catch
            {
                return false;
            }
        }

        #endregion
        #region Validates password

        //method to validate inserted password
        public bool ValidatePassword(string Password)
        {
            if (Password.Length < minimumLength)
                return false;
            if (UpperCount(Password) < upperLength)
                return false;
            if (LowerCount(Password) < lowerLength)
                return false;
            if (NumericCount(Password) < numericLength)
                return false;
            if (SpecialCharCount(Password) < specialChar)
                return false;
            return true;
        }

        // Checks if password are the same
        public bool PasswordTheSame (string password1, string password2)
        {
            if (password1.ToString().Equals(password2.ToString()))
            {
                return true;
            } else
            {
                return false;
            }
        }

        #endregion
        #region Methods to count specifics in password

        //Counts uppercase characters in password
        private static int UpperCount(string Password)
        {
            return Regex.Matches(Password, "[A-Z]").Count;
        }

        //Counts lowercase characters in password
        private static int LowerCount(string Password)
        {
            return Regex.Matches(Password, "[a-z]").Count;
        }

        //Counts numeric characters in password
        private static int NumericCount(string Password)
        {
            return Regex.Matches(Password, "[0-9]").Count;
        }

        //Counts special characters in password

        private static int SpecialCharCount(string Password)
        {
            return Regex.Matches(Password, @"[^0-9a-zA-Z\._]").Count;
        }

        #endregion
 
        

        #region Register button
        //Register button event
        public void Register()
        {
            User user = new User();
            string tbFirstName = FirstName;
            string tbLastName = LastName;
            string tbEmail = Email;
            string tbPassword = User.Encryption(Password.ToString());

            if (isValidEmail(Email) && ValidatePassword(Password) && PasswordTheSame(PasswordConfirm, Password))
            {
                if (User.Register(tbEmail, tbPassword, tbFirstName, tbLastName))
                {

                    MessageBox.Show("Registration succesful!");
                }
                else
                {
                    MessageBox.Show("Registration failed! Please try again. (F's in chat)");
                }
            }
            else
            {
                MessageBox.Show("Please fill the fields with errors");
            }

        }
        #endregion
    }
}
