using Caliburn.Micro;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneGUI.ViewModels
{
    public class NoteElementViewModel : PropertyChangedBase
    {

        public string Title { get; set; }
        public string NoteCreationDate { get; set; }

        public Note Note { get; set; }

        public void LoadOnClick(EventArgs ClickedEventArgs)
        {

        }


    }
}
