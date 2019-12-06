using Caliburn.Micro;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Notes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// ViewModel which handles all interaction in the note menu window.
    /// </summary>
    public class NotebookNotesMenuViewModel : Conductor<NoteElementViewModel>.Collection.AllActive
    {

        public bool ShowDeletedNotes { get; set; }

        #region Databound properties and their 'behind the scenes' instance variables
        private string _notebookNoteCount = "0 note(s)";
        private ObservableCollection<NoteElementViewModel> _noteElementViews;

        // These properties need to make changes happen in the view.
        public string NotebookName
        {
            get => Notebook.Title;
            set
            {
                Notebook.Title = value;
                NotifyOfPropertyChange(() => NotebookName);
            }
        }

        public string NotebookNoteCount
        {
            get => _notebookNoteCount;
            set
            {
                _notebookNoteCount = value;
                NotifyOfPropertyChange(() => NotebookNoteCount);
            }
        }

        public ObservableCollection<NoteElementViewModel> NoteElementViews
        {
            get => _noteElementViews;
            set
            {
                _noteElementViews = value;
                NotifyOfPropertyChange(() => NoteElementViews);
            }
        }

        #endregion

        public Notebook Notebook { get; set; }

        #region Events

        /// <summary>
        /// Event that searches for notes within a notebook when more than 2 characters (which are not whitespace)
        /// are typed into the searchbar.
        /// </summary>
        /// <param name="eventArgs"></param>
        public void SearchNoteInNotebook(TextChangedEventArgs eventArgs)
        {
            if (eventArgs.Source is TextBox searchBar)
            {
                // Acceptance criteria specifies that the text should have at least 2 characters.
                if (searchBar.Text.Trim().Length >= 2 &&
                    !(string.IsNullOrWhiteSpace(searchBar.Text) || string.IsNullOrEmpty(searchBar.Text)))
                {
                    if (Notebook != null && Notebook.Notes != null)
                    {
                        // Acceptance criteria specifies that it should search for all notes that contain the piece of text 
                        // in the following data: title, author, tags, content.
                        string searchFor = searchBar.Text.ToLower().Trim();
                        List<INote> returnedNotes = new List<INote>();

                        // Check if the contents we are searching for is contained within the content, title, author or tags of the note.

                        foreach (Note note in Notebook.Notes.Cast<Note>())
                        {
                            if (note.Content != null && note.Content.ToLower().Contains(searchFor)
                                || note.Title != null && note.Title.ToLower().Contains(searchFor)
                                || note.Author != null && note.Author.ToLower().Contains(searchFor))
                            {
                                returnedNotes.Add(note);
                            }
                            else if (note.Tags != null)
                            {
                                foreach (string tag in note.Tags)
                                {
                                    if (tag.ToLower().Contains(searchFor))
                                    {
                                        returnedNotes.Add(note);
                                        break;
                                    }
                                }
                            }
                        }

                        // Generate 
                        NoteElementViews = GenerateNoteElementsFromNotebook(returnedNotes);
                    }
                }
                else
                {
                    // We have to make sure that all notes are visible again once the searching is done.
                    NoteElementViews = GenerateNoteElementsFromNotebook(Notebook.Notes);
                }

                // Update the note count to show the current situation.
                NotebookNoteCount = $"{NoteElementViews.Count} note(s)";

            }
        }

        #endregion

        #region Helper methods

        public void LoadAllNotesIntoNotebookMenu(List<INote> notes)
        {
            if (notes != null)
            {
                NoteElementViews = GenerateNoteElementsFromNotebook(notes);
            }
        }

        public void LoadNotesIntoNotebookMenu(bool showDeletedNotes = false)
        {
            this.ShowDeletedNotes = showDeletedNotes;
            NoteElementViews = GenerateNoteElementsFromNotebook(Notebook.Notes);
        }

        /// <summary>
        /// Method which generates the ViewModel objects to be inserted in the NotebookNotesMenuViewModel.
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        public ObservableCollection<NoteElementViewModel> GenerateNoteElementsFromNotebook(List<INote> notes)
        {
            ObservableCollection<NoteElementViewModel> noteElementViewModels = new ObservableCollection<NoteElementViewModel>();
            
            if (notes != null)
            {
                if (Parent is NoteFeverViewModel noteFeverViewModel)
                {
                    foreach (Note note in notes.Cast<Note>())
                    {
                        if (!note.IsDeleted || ShowDeletedNotes)
                        {
                            NoteElementViewModel noteElementView = new NoteElementViewModel()
                            {
                                Container = noteFeverViewModel,
                                Note = note,
                                Title = note.Title ?? "",
                                NoteCreationDate = note.CreationDate.Date.ToString("dd-MM-yyyy") ?? "Unknown"
                            };

                            if (note.Equals(noteFeverViewModel.SelectedNote) && noteFeverViewModel.NotebookViewModel != null)
                                noteFeverViewModel.NotebookViewModel.SelectedNoteElement = noteElementView;
                            noteElementViewModels.Add(noteElementView);
                        }

                    }
                }
            }
            return noteElementViewModels;
        }

        #endregion
    }
}
