using Caliburn.Micro;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Notes;
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

        public void Register()
        {
            IWindowManager windowManager = new WindowManager();

            RegisterViewModel registerViewModel = new RegisterViewModel();
            windowManager.ShowDialog(registerViewModel, null);
        }

    }
}
