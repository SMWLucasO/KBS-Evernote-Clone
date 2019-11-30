using Caliburn.Micro;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EvernoteCloneGUI.ViewModels
{
    public class NotebookNotesMenuViewModel : Conductor<NoteElementViewModel>.Collection.AllActive
    {

        private string _notebookNoteCount = "0 note(s)";
        private ObservableCollection<NoteElementViewModel> _noteElementViews;

        // These properties need to make changes happen in the view.
        public string NotebookName
        {
            get
            {
                return Notebook.Title;
            }
            set
            {
                Notebook.Title = value;
                NotifyOfPropertyChange(() => NotebookName);
            }
        }

        public string NotebookNoteCount
        {
            get
            {
                return _notebookNoteCount;
            }
            set
            {
                _notebookNoteCount = value;
                NotifyOfPropertyChange(() => NotebookNoteCount);
            }
        }

        public ObservableCollection<NoteElementViewModel> NoteElementViews
        {
            get
            {
                return _noteElementViews;
            }
            set
            {
                _noteElementViews = value;
                NotifyOfPropertyChange(() => NoteElementViews);
            }
        }

        public Notebook Notebook { get; set; }

        

        public NotebookNotesMenuViewModel()
        {
        }

        /// <summary>
        /// Event that searches for notes within a notebook when more than 2 characters (which are not whitespace)
        /// are typed into the searchbar.
        /// </summary>
        /// <param name="EventArgs"></param>
        public void SearchNoteInNotebook(TextChangedEventArgs EventArgs)
        {
            if (EventArgs.Source is TextBox searchBar)
            {
                if (searchBar.Text.Trim().Length > 2 &&
                    !(string.IsNullOrWhiteSpace(searchBar.Text) || string.IsNullOrEmpty(searchBar.Text)))
                {
                    string searchFor = searchBar.Text.Trim();
                    NoteElementViews = GenerateNoteElementsFromNotebook(
                            Notebook.Notes.Where(note =>
                                ((Note)note).Title.ToLower().StartsWith(searchFor)
                                || ((Note)note).Title.ToLower().EndsWith(searchFor)
                                || ((Note)note).Author.ToLower().StartsWith(searchFor)
                                || ((Note)note).Author.ToLower().EndsWith(searchFor)
                                ).ToList()
                        );

                }
                else
                {
                    NoteElementViews = GenerateNoteElementsFromNotebook(Notebook.Notes);
                }

                NotebookNoteCount = $"{NoteElementViews.Count} note(s)";

            }

            // TODO:
            // if >2 characters typed: search the NoteElementViews, clear the visual part and 
            // load the NoteElementViews which adhere to the given search-criteria
        }

        public void LoadNotesIntoNotebookMenu()
        {
            NoteElementViews = GenerateNoteElementsFromNotebook(Notebook.Notes);
        }

        /// <summary>
        /// Method which generates the ViewModel objects to be inserted in the NotebookNotesMenuViewModel.
        /// </summary>
        /// <param name="Notes"></param>
        /// <returns></returns>
        public ObservableCollection<NoteElementViewModel> GenerateNoteElementsFromNotebook(List<INote> Notes)
        {

            ObservableCollection<NoteElementViewModel> noteElementViewModels =
                new ObservableCollection<NoteElementViewModel>();

            if (Notes != null)
            {
                if (Parent is NoteFeverViewModel noteFeverViewModel)
                {
                    foreach (Note note in Notes)
                    {

                        NoteElementViewModel noteElementView = new NoteElementViewModel()
                        {
                            Container = noteFeverViewModel,
                            Note = note,
                            Title = note.Title ?? "",
                            NoteCreationDate = note.CreationDate.Date.ToString("dd-MM-yyyy") ?? "Unknown"
                        };

                        if (note.Equals(noteFeverViewModel.SelectedNote) 
                            && noteFeverViewModel.NotebookViewModel != null)
                        {
                            noteFeverViewModel.NotebookViewModel.SelectedNoteElement = noteElementView;
                        }

                        noteElementViewModels.Add(noteElementView);
                    }
                }   
            }
            return noteElementViewModels;
        }


    }
}
