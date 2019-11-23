using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// The root viewmodel, this is the viewmodel which handles the main application screen.
    /// </summary>
    public class NoteFeverViewModel : Conductor<object>
    {
        protected override void OnActivate()
        {
            // Only do this when a note has been opened, otherwise the right side should still be empty.
            ActivateItem(new NewNoteViewModel());
        }

        /// <summary>
        /// 
        /// </summary>
        public void NewNote()
        {
            IWindowManager windowManager = new WindowManager();
            
            dynamic settings = new ExpandoObject();
            settings.Height = 600;
            settings.Width = 800;
            settings.SizeToContent = SizeToContent.Manual;

            NewNoteViewModel newNoteViewModel = new NewNoteViewModel();
            windowManager.ShowDialog(newNoteViewModel, null, settings);
        }

    }
}
