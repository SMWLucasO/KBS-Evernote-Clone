using Caliburn.Micro;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Windows;

namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// The root viewmodel, this is the viewmodel which handles the main application screen.
    /// </summary>
    public class NoteFeverViewModel : Conductor<object>
    {

        // Notebook information for viewing things
        public List<Notebook> Notebooks { get; private set; }
        public Note SelectedNote = null;

        // <User object stuff here>
        // <ReplaceThis>
        // </User object stuff here>

        /// <summary>
        /// Whenever we know that there is a note selected,
        /// we want to switch to the note user control, which will display it with all its data.
        /// </summary>
        protected override void OnActivate()
        {
            
            // TODO: IF the user is logged in (there should be a property here with the user), insert the UserID.
            // Temporary try/catch until issue is fixed with exceptions.
            try
            {
                Notebooks = Notebook.Load();
                if (Notebooks != null)
                {

                    Note tempNote = (Note)Notebooks.First().Notes.First();
                    if (tempNote != null)
                    {
                        SelectedNote = tempNote;
                    }
                }
            }
            catch (Exception) { };


            // Only do this when a note has been opened, otherwise the right side should still be empty.
            LoadNoteViewIfNoteExists();


        }

        public void LoadNoteViewIfNoteExists()
        {
            
            if (SelectedNote != null)
            {

                NewNoteViewModel newNoteViewModel = new NewNoteViewModel(true)
                {
                    Note = SelectedNote
                };

                newNoteViewModel.LoadNote();

                ActivateItem(newNoteViewModel);

            }
        }

        /// <summary>
        /// Open the Window responsible for the creation of new notes.
        /// </summary>
        public void NewNote()
        {
            IWindowManager windowManager = new WindowManager();

            dynamic settings = new ExpandoObject();
            settings.Height = 600;
            settings.Width = 800;
            settings.SizeToContent = SizeToContent.Manual;

            NewNoteViewModel newNoteViewModel = new NewNoteViewModel
            {
                Parent = this
            };
            windowManager.ShowDialog(newNoteViewModel, null, settings);
        }

    }
}
