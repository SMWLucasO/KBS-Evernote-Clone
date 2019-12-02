using Caliburn.Micro;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Windows;

namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// ViewModel which handles all interaction with the note elements in the NotebookNotesMenuView
    /// </summary>
    public class NoteElementViewModel : PropertyChangedBase
    {
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }
        public string NoteCreationDate { get; set; }

        public Note Note { get; set; }

        public NoteFeverViewModel Container { get; set; }

        public void LoadOnClick(EventArgs ClickedEventArgs)
        {
            if (Container?.SelectedNote != null)
            {
                // When we click on the NoteElementView, it gets called from the class which was clicked
                // thus we need to get the currently selected note (so, the one which is currently being displayed
                // before switching) and check if it was modified, and if so, notify that changes may be lost.
                Note currentlySelectedNote = Container.SelectedNote;
                if (!(currentlySelectedNote.Content.Equals(currentlySelectedNote.NewContent)))
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure? Unsaved changes will be lost.", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        // Reset contents and switch note views.
                        currentlySelectedNote.NewContent = currentlySelectedNote.Content;
                        SwitchNoteView();
                    }
                }
                else
                    SwitchNoteView();
            }
            else
                SwitchNoteView();
            
        }

        private void SwitchNoteView()
        {
            if(Container != null)
            {
                Container.SelectedNote = Note;
                Container.LoadNoteViewIfNoteExists();
            }
            
        }
    }
}
