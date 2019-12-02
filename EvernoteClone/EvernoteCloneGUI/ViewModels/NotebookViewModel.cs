using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// Intermediary ViewModel which couples the NewNoteView and NotebookNotesMenu together.
    /// </summary>
    public class NotebookViewModel : Conductor<object>.Collection.AllActive
    {

        public NotebookNotesMenuViewModel NotebookNotesMenu { get; set; }
        public NewNoteViewModel NewNoteViewModel { get; set; }
        public NoteElementViewModel SelectedNoteElement { get; set; }
 
        public NotebookViewModel()
        {
            NotebookNotesMenu = new NotebookNotesMenuViewModel();
            NewNoteViewModel = new NewNoteViewModel();
            ActivateItem(NotebookNotesMenu);
            ActivateItem(NewNoteViewModel);
        }


    }
}
