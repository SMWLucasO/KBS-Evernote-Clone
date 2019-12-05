using Caliburn.Micro;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Notes;
using EvernoteCloneLibrary.Users;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EvernoteCloneGUI.ViewModels
{
    class LoginViewModel
    {
        private string emailLogin;
        private string passwordLog;
        public User user=null;
        public void Register()
        {
            IWindowManager windowManager = new WindowManager();

            RegisterViewModel registerViewModel = new RegisterViewModel();
            windowManager.ShowDialog(registerViewModel, null);
            
        }

        public string EmailLogin
        {
            get { return emailLogin; }
            set { emailLogin = value; }
        }

        //Password property
        public string PasswordLogin
        {
            get { return passwordLog; }
            set { passwordLog = value; }
        }

        public void Login()
        {
            
            string usernameLogin = EmailLogin;
            Console.WriteLine("Pass: "+ passwordLog.ToString());
            string passwordLogin = User.Encryption(PasswordLogin.ToString());
            user = (User)User.Login(usernameLogin, passwordLogin);

            if (user != null)
            {
                MessageBox.Show("gelukt");
            }
            else 
            {
                MessageBox.Show("false");

            }
            
        }

    }
}
