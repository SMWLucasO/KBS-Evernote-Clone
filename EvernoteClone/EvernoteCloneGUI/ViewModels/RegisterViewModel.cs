using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;




namespace EvernoteCloneGUI.ViewModels
{
    public class RegisterViewModel : IDataErrorInfo
    {
        private string firstName;
        private string lastName;
        private string email;
        private string password;
        private string passwordConfirm;
        


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

        public string PasswordConfirm 
        {
            get { return passwordConfirm; }
            set { passwordConfirm = value; }
        }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName;}
            set { lastName = value;}
        }



        #endregion

        #region Show error messages
        //Show error message
        public string this[string PropertyName]
        {           
            get
            {
                
                string result = null;
                
                if(PropertyName == "Email")
                {
                    if (string.IsNullOrEmpty(Email) || isValidEmail(Email) == false)
                        result = "Please enter email!";
                }
                if (PropertyName == "Password")
                {
                    
                    if (string.IsNullOrEmpty(Password))
                        result = "Please enter Password!";
                }
                if (PropertyName == "PasswordConfirm")
                {
                    if (PasswordConfirm != null)
                        if (!PasswordConfirm.ToString().Equals(Password.ToString()))
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



        public void Register()
        {
            string tbFirstName = FirstName;
            string tbLastName = LastName;
            string tbEmail = Email;
            string tbPassword = Password.ToString();
            string cPassword = PasswordConfirm.ToString();

            
        }
                
    }
}
