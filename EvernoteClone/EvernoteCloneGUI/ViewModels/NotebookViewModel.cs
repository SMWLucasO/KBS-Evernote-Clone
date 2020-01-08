using Caliburn.Micro;

namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// Intermediary ViewModel which couples the NewNoteView and NotebookNotesMenu together.
    /// </summary>
    public class NotebookViewModel : Conductor<object>.Collection.AllActive
    {

        /// <value>
        /// The viewmodel(a menu, in this case.) which contains all the notes for this notebook
        /// </value>
        public NotebookNotesMenuViewModel NotebookNotesMenu { get; set; }
        
        /// <value>
        /// The text editor
        /// </value>
        public NewNoteViewModel NewNoteViewModel { get; set; }
        
        /// <value>
        /// The currently selected note element
        /// </value>
        public NoteElementViewModel SelectedNoteElement { get; set; }
 
        public NotebookViewModel()
        {
            NotebookNotesMenu = new NotebookNotesMenuViewModel();
            NewNoteViewModel = new NewNoteViewModel();
            
            // Display the viewmodels on the page
            ActivateItem(NotebookNotesMenu);
            ActivateItem(NewNoteViewModel);
        }

        public sealed override void ActivateItem(object item) =>
            base.ActivateItem(item);
    }
}
