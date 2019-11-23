using Caliburn.Micro;
using EvernoteCloneGUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EvernoteCloneGUI
{
    /// <summary>
    /// The class which handles the bootstrapping for the GUI application. 
    /// It handles all preparation for showing the screen.
    /// </summary>
    public class Bootstrapper : BootstrapperBase
    {

        public Bootstrapper()
        {
            Initialize();
        }

        /// <summary>
        /// An event called on the startup of the application.
        /// </summary>
        /// <param name="sender">The object that called the event</param>
        /// <param name="e">The startup event arguments</param>
        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // The 'MainViewModel' is basically the start-location for our entire application
            // thus we start it when the application is started.
            DisplayRootViewFor<NoteFeverViewModel>();
        }

    }
}
