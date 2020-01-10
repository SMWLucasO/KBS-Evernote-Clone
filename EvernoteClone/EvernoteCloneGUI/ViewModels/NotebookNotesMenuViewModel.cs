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
    /// This ViewModel represents a notebook containing multiple notes.
    /// </summary>
    public class NotebookNotesMenuViewModel : Conductor<NoteElementViewModel>.Collection.AllActive
    {

        /// <value>
        /// Boolean indicating whether deleted notes should be shown.
        /// </value>
        public bool ShowDeletedNotes { get; set; }

        #region Databound properties and their 'behind the scenes' instance variables
        
        /// <value>
        /// instance variable containing a string representation of the amount of notes in the specified notebook. 
        /// </value>
        private string _notebookNoteCount = "0 " + Properties.Settings.Default.NotebookNotesMenuViewNotes;
        
        /// <value>
        /// Instance variable containing all visual representations of the notes within the notebook.
        /// </value>
        private ObservableCollection<NoteElementViewModel> _noteElementViews;

        /// <value>
        /// Getter and setter for the name of the notebook currently open.
        /// </value>
        public string NotebookName
        {
            get => Notebook.Title;
            set
            {
                Notebook.Title = value;
                NotifyOfPropertyChange(() => NotebookName);
            }
        }

        /// <value>
        /// Getter/setter for the string representation of the notebook note count.
        /// When this property is set, it will be modified on the screen, too.
        /// </value>
        public string NotebookNoteCount
        {
            get => _notebookNoteCount;
            set
            {
                _notebookNoteCount = value;
                NotifyOfPropertyChange(() => NotebookNoteCount);
            }
        }

        /// <value>
        /// Getter/setter for all the visual representations of the notebook's notes.
        /// </value>
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

        /// <value>
        /// The notebook which this ViewModel represents visually.
        /// </value>
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
                    !string.IsNullOrWhiteSpace(searchBar.Text))
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
                            else if (note.Labels != null)
                            {
                                foreach (string tag in note.Labels)
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
                NotebookNoteCount = $"{NoteElementViews.Count} " + Properties.Settings.Default.NotebookNotesMenuViewNotes;

            }
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Load the specified notes into the notebook notes menu
        /// </summary>
        /// <param name="notes"></param>
        public void LoadAllNotesIntoNotebookMenu(List<INote> notes)
        {
            if (notes != null)
            {
                NoteElementViews = GenerateNoteElementsFromNotebook(notes);
            }
        }

        /// <summary>
        /// Load all notes of the notebook into the notebook notes menu, whilst also determining wheter it should 
        /// show deleted notes too.
        /// </summary>
        /// <param name="showDeletedNotes"></param>
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
            ObservableCollection<NoteElementViewModel> noteElementViewModels 
                = new ObservableCollection<NoteElementViewModel>();
            
            // Generate Note elements, if the bool for showing deleted notes is true, we will show those too. 
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
                            
                            // Set the default selected note element for the notebook view
                            if (note.Equals(noteFeverViewModel.SelectedNote) && noteFeverViewModel.NotebookViewModelProp != null)
                            {
                                noteFeverViewModel.NotebookViewModelProp.SelectedNoteElement = noteElementView;
                            }
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
