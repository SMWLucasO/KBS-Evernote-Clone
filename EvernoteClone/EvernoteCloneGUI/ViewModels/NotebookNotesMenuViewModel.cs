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

        // Examples for the text display on the view
        public string NotebookName { get; set; } = "Nameless notebook";
        public string NotebookNoteCount { get; set; } = "0 notes";

        public Notebook Notebook { get; set; }

        public ObservableCollection<NoteElementViewModel> NoteElementViews { get; set; }

        public NotebookNotesMenuViewModel()
        {
        }

        public void SearchNoteInNotebook(TextChangedEventArgs EventArgs)
        {
            // TODO:
            // if >2 characters typed: search the NoteElementViews, clear the visual part and 
            // load the NoteElementViews which adhere to the given search-criteria
        }

        public void LoadNotesIntoNotebookMenu()
        {
            NoteElementViews = GenerateNoteElementsFromNotebook(Notebook);
        }

        /// <summary>
        /// Method which generates the ViewModel objects to be inserted in the NotebookNotesMenuViewModel.
        /// </summary>
        /// <param name="Notebook"></param>
        /// <returns></returns>
        public ObservableCollection<NoteElementViewModel> GenerateNoteElementsFromNotebook(Notebook Notebook)
        {
            ObservableCollection<NoteElementViewModel> noteElementViewModels = 
                new ObservableCollection<NoteElementViewModel>();

            if (Notebook != null)
            {
                foreach (Note note in Notebook.Notes)
                {
                    noteElementViewModels.Add(new NoteElementViewModel()
                    {
                        Note = note,
                        Title = note.Title ?? "",
                        NoteCreationDate = note.CreationDate.Date.ToString() ?? "Unknown"
                    }
                    );
                }
            }
            return noteElementViewModels;
        }

    }
}
