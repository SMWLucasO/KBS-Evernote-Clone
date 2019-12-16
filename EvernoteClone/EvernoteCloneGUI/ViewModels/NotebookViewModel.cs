using Caliburn.Micro;

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

        public sealed override void ActivateItem(object item) =>
            base.ActivateItem(item);
    }
}
