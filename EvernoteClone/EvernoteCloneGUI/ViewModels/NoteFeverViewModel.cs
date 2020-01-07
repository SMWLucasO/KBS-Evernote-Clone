using Caliburn.Micro;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Location;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EvernoteCloneGUI.Properties;
using EvernoteCloneGUI.ViewModels.Controls;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.SharedNotes;
using EvernoteCloneLibrary.Users;
using EvernoteCloneLibrary.Settings;
using EvernoteCloneLibrary;


namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// The root viewmodel, this is the viewmodel which handles the main application screen.
    /// </summary>
    public class NoteFeverViewModel : Conductor<object>
    {
        #region Properties

        /// <value>
        /// The background color of all buttons
        /// </value>
        public string ButtonBackgroundColor
        {
            get => _buttonBackgroundColor;
            set
            {
                _buttonBackgroundColor = value;
                NotifyOfPropertyChange(nameof(ButtonBackgroundColor));
            }
        }

        /// <value>
        /// The background color of all active buttons
        /// </value>
        public string ButtonAccentColor
        {
            get => _buttonAccentColor;
            set
            {
                _buttonAccentColor = value;
                NotifyOfPropertyChange(nameof(ButtonAccentColor));
            }
        }
        
        /// <value>
        /// The only instance of the NoteFeverTreeViewModel
        /// </value>
        public static NoteFeverTreeViewModel NoteFeverTreeViewModel { get; private set; }

        /// <value>
        /// This contains the user object
        /// </value>
        public static User LoginUser;

        /// <value>
        /// NotebooksTreeView contains the whole folder structure together with all the notebooks
        /// </value>
        public static ObservableCollection<TreeViewItem> NotebooksTreeView { get; } = new ObservableCollection<TreeViewItem>(new List<TreeViewItem>());
        
        /// <value>
        /// Notebooks contains all the local notebooks (and online notebooks, if UserID != 1)
        /// </value>
        public List<Notebook> Notebooks { get; private set; } = new List<Notebook>();
        
        /// <summary>
        /// A notebook that contains all shared notes
        /// </summary>
        public Notebook SharedNotebook { get; private set; }

        /// <value>
        /// SelectedNotebook contains the currently selected Notebook
        /// </value>
        public Notebook SelectedNotebook { get; set; }

        /// <value>
        /// SelectedNote contains the currently selected Note
        /// </value>
        public Note SelectedNote { get; set; }

        /// <value>
        /// NotebookViewModel contains the (only???) instance of the NotebookViewModel
        /// That is used to display all the notes inside a Notebook
        /// </value>
        public NotebookViewModel NotebookViewModelProp { get; set; }

        #endregion

        #region Variables

        /// <value>
        /// The background of buttons
        /// </value>
        private string _buttonBackgroundColor;
        
        /// <value>
        /// The background of active buttons
        /// </value>
        private string _buttonAccentColor;

        #endregion

        #region Notebook loading

        /// <summary>
        /// This loads all the notebooks from the filesystem (and from the database as well, if UserID != 1)
        /// </summary>
        /// <param name="initialLoad">If this is true, it loads the SelectedNotebook and SelectedNote</param>
        public void LoadNotebooks(bool initialLoad = false)
        {
            // Load all Notebooks
            Notebooks = Notebook.Load();

            // if we're doing the initial loading, load the note and notebook if there is not already a selected note/notebook.
            SelectFirst();
        }

        public void LoadSharedNotebook()
        {
            SharedNotebook = new Notebook
            {
                Id = -1,
                LocationId = -1,
                UserId = -1,
                Title = "Shared notes",
                LastUpdated = DateTime.Now,
                CreationDate = DateTime.Now,
                IsNotNoteOwner = true,
                IsSharedNotebook = true
            };

            List<SharedNote> sharedNotes = SharedNote.GetAllSharedNotes();
            foreach (SharedNote sharedNote in sharedNotes)
            {
                Note note = Note.GetNoteFromDatabaseById(sharedNote.NoteId);
                note.NoteOwner = SharedNotebook;
                SharedNotebook.Notes.Add(note);
            }
        }

        /// <summary>
        /// This selects the first notebook (if Notebooks not null) and the first note inside the notebook we just selected.
        /// </summary>
        private void SelectFirst()
        {
            SelectFirstNotebook();
            SelectFirstNote(SelectedNotebook);
        }

        /// <summary>
        /// Select the first notebook (if Notebooks not null)
        /// </summary>
        private void SelectFirstNotebook()
        {
            if (Notebooks.Count > 0)
            {
                SelectedNotebook = Notebooks.Where((notebook) => !(notebook.IsDeleted)).FirstOrDefault();
            }
        }


        /// <summary>
        /// Select the first note from the selected notebook
        /// </summary>
        private void SelectFirstNoteFromSelectedNotebook() =>
            SelectFirstNote(SelectedNotebook);

        /// <summary>
        /// Select the first note from the given notebook
        /// </summary>
        /// <param name="notebook">The notebook from which the note should be selected</param>
        private void SelectFirstNote(Notebook notebook)
        {
            if (notebook?.Notes.Count > 0)
            {
                SelectedNote = (Note)(notebook?.Notes).FirstOrDefault(note => !((Note)note).IsDeleted);
            }
            else
            {
                SelectedNote = null;
            }
        }
        
        #endregion

        #region Loading and opening views

        /// <summary>
        /// Load the note view using the viewmodel's note and notebook
        /// </summary>
        /// <param name="showDeletedNotes"></param>
        public void LoadNoteViewIfNoteExists(bool showDeletedNotes = false)
        {
            if (SelectedNotebook != null)
            {
                string notebookCountString = "0 note(s)";
                NewNoteViewModel newNoteViewModel = null;


                // if we aren't showing deleted notes, we need to alter the note count to only show the count of those not deleted
                if (!showDeletedNotes)
                {
                    if (SelectedNotebook.Notes.Count > 0)
                    {
                        IEnumerable<INote> notes = SelectedNotebook.Notes.Where(note => !((Note)note).IsDeleted);
                        notebookCountString = $"{notes.ToList().Count} note(s)";
                    }
                }
                else
                {
                    notebookCountString = $"{SelectedNotebook.Notes.Count} note(s)";
                }

                if (SelectedNote != null)
                {
                    newNoteViewModel = new NewNoteViewModel(true)
                    {
                        Note = SelectedNote,
                        NoteOwner = SelectedNote.NoteOwner,
                        Parent = this
                    };
                }

                // Create the notebook view with the required data.
                NotebookViewModelProp = new NotebookViewModel
                {
                    NewNoteViewModel = newNoteViewModel,
                    NotebookNotesMenu = new NotebookNotesMenuViewModel
                    {
                        Notebook = SelectedNotebook,
                        NotebookName = SelectedNotebook.Title,
                        NotebookNoteCount = notebookCountString,
                        Parent = this
                    }
                };

                // load the selected note and the note elements, afterwards, activate the view.
                NotebookViewModelProp.NewNoteViewModel?.LoadNote();
                NotebookViewModelProp.NotebookNotesMenu.LoadNotesIntoNotebookMenu(showDeletedNotes);
                
                ActivateItem(NotebookViewModelProp);
            }
            else
            {
                DeactivateItem(NotebookViewModelProp, true);
            }
        }

        /// <summary>
        /// Open the Window responsible for the creation of new notes.
        /// </summary>
        public void OpenNewNotePopupView()
        {
            // check if a notebook is selected, and it is not a special notebook (all notes, bin, etc.)
            // if so, show a pop-up where a new note can be created.
            if (SelectedNotebook != null && SelectedNotebook.IsNotNoteOwner == false)
            {
                IWindowManager windowManager = new WindowManager();

                // settings object which contains the width, height and sizing for the view.
                dynamic settings = new ExpandoObject();
                settings.Height = 600;
                settings.Width = 800;
                settings.SizeToContent = SizeToContent.Manual;

                NewNoteViewModel newNoteViewModel = new NewNoteViewModel
                {
                    Parent = this,
                    NoteOwner = SelectedNotebook
                };

                windowManager.ShowDialog(newNoteViewModel, null, settings);
            }
            else
            {
                MessageBox.Show("Cannot add new notes whilst not in a notebook", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Method which opens the view containing all the user's notes.
        /// </summary>
        public void OpenAllNotesView()
        {
            if (Notebooks != null)
            {
                Notebook allNotesNotebook = new Notebook
                {
                    Id = -1,
                    LocationId = -1,
                    UserId = -1,
                    Title = "All notes",
                    LastUpdated = DateTime.Now,
                    CreationDate = DateTime.Now.Date,
                    IsNotNoteOwner = true
                };

                // get all notes from all notebooks, where the notes haven't been deleted
                List<INote> notes = new List<INote>();
                foreach (Notebook notebook in Notebooks)
                {
                    // We want to retrieve all notes which are not deleted.
                    notes.AddRange(notebook.RetrieveNoteList(note => !((NoteModel)note).IsDeleted));
                }

                allNotesNotebook.Notes = notes;

                if (!(ValidateAndLoadNotebookView(allNotesNotebook)))
                {
                    MessageBox.Show("There are no notes to view.", "Note Fever", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// Method which opens the view containing all the user's SharedNotes.
        /// </summary>
        public void OpenSharedNotesView()
        {
            LoadSharedNotebook();

            if (!ValidateAndLoadNotebookView(SharedNotebook))
            {
                MessageBox.Show("There are no shared notes to view.", "Note Fever", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Open the view for all deleted notes
        /// </summary>
        public void OpenDeletedNotesView()
        {
            Notebook trashNotebook = new Notebook
            {
                Id = -1,
                LocationId = -1,
                UserId = -1,
                Title = "Bin",
                LastUpdated = DateTime.Now,
                CreationDate = DateTime.Now.Date,
                IsNotNoteOwner = true
            };

            List<INote> deletedNotes = new List<INote>();

            // get the deleted notes of all notebooks
            foreach (Notebook notebook in Notebooks)
            {
                deletedNotes.AddRange(
                    notebook.RetrieveNoteList((note) => ((NoteModel)note).IsDeleted).Cast<INote>()
                    );
            }

            trashNotebook.Notes = deletedNotes;
            if (!(ValidateAndLoadNotebookView(trashNotebook, true)))
            {
                MessageBox.Show("There are no deleted notes.", "Note Fever", MessageBoxButton.OK, MessageBoxImage.Information);

            }

        }

        /// <summary>
        /// Open the view for the login popup
        /// </summary>
        public void OpenLoginPopupView(bool suppressSynchronize = false)
        {
            IWindowManager windowManager = new WindowManager();

            LoginViewModel loginViewModel = new LoginViewModel(LoginUser);
            windowManager.ShowDialog(loginViewModel);

            LoginUser = loginViewModel.User;
            Constant.User = LoginUser;

            if (!suppressSynchronize)
                Synchronize();
            else
                LoadSettings();
        }

        /// <summary>
        /// Open the view if the notebook has notes
        /// </summary>
        /// <param name="notebook"></param>
        /// <param name="showDeletedNotes"></param>
        private bool ValidateAndLoadNotebookView(Notebook notebook, bool showDeletedNotes = false)
        {
            if (notebook != null)
            {
                SelectedNotebook = notebook;
                if (notebook.Notes.Count > 0)
                {
                    SelectedNote = (Note)notebook.Notes.First();
                }
                else
                {
                    SelectedNote = null;
                }

                LoadNoteViewIfNoteExists(showDeletedNotes);
                return SelectedNote != null;
            }

            return false;
        }

        /// <summary>
        /// Select the notebook with the Id equal to notebookId
        /// </summary>
        /// <param name="notebookId">The Id of the requested notebook</param>
        public void SelectNotebook(int notebookId)
        {
            SelectedNotebook = Notebooks.FirstOrDefault(notebook => notebook.Id == notebookId);
            SelectFirstNote(SelectedNotebook);
            LoadNoteViewIfNoteExists();
        }


        /// <summary>
        /// Select the first notebook inside a path with a certain title
        /// </summary>
        /// <param name="notebookLocation">The path that should contain the notebook</param>
        /// <param name="title">The title of the notebook that we want to select</param>
        public void SelectNotebook(NotebookLocation notebookLocation, string title)
        {
            SelectedNotebook = Notebooks.FirstOrDefault(notebook => notebook.Path.Path == notebookLocation.Path && notebook.Title == title);
            SelectFirstNote(SelectedNotebook);
            LoadNoteViewIfNoteExists();
        }

        #endregion

        #region Button click events
        
        /// <summary>
        /// This synchronizes all the folders, notebooks and notes from the user
        /// </summary>
        public void Synchronize()
        {
            NoteFeverTreeViewModel.LoadNotebooksTreeView();
            SelectedNotebook = Notebooks.FirstOrDefault(notebooks => notebooks.FsName == SelectedNotebook?.FsName);
            SelectedNote = SelectedNotebook?.Notes.Cast<Note>().FirstOrDefault(notes => notes.Title == SelectedNote?.Title && notes.LastUpdated == SelectedNote?.LastUpdated);
            LoadNoteViewIfNoteExists();
            LoadSettings();
        }
        
        /// <summary>
        /// This will open the SettingsView when this button is clicked
        /// </summary>
        public void OpenSettingsView()
        {
            IWindowManager windowManager = new WindowManager();
            
            SettingsViewModel settingsViewModel = new SettingsViewModel(this);
            windowManager.ShowDialog(settingsViewModel);
        }
        
        #endregion

        #region Events

        /// <summary>
        /// Whenever we know that there is a note selected,
        /// we want to switch to the note user control, which will display it with all its data.
        /// </summary>
        protected override void OnActivate()
        {
            MessageBox.Show(Environment.CurrentDirectory);
            LanguageChanger.UpdateResxFile(Properties.Settings.Default.LastSelectedLanguage);

            // Show login popup
            OpenLoginPopupView(true);

            // If user closed login window without logging in or clicking the 'Use locally' button, close application
            if (LoginUser == null)
            {
                Environment.Exit(0);
            }

            NoteFeverTreeViewModel = new NoteFeverTreeViewModel(this);

            // Load Notebooks
            LoadNotebooks(true);

            // Only do this when a note has been opened, otherwise the right side should still be empty.
            LoadNoteViewIfNoteExists();
        }

        /// <summary>
        /// This handles the change of a folder or notebook (TreeViewItem)
        /// </summary>
        /// <param name="routedPropertyChangedEventArgs"></param>
        public void TreeViewSelectedItemChanged(RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            NoteFeverTreeViewModel.TreeViewSelectedItemChanged(routedPropertyChangedEventArgs);
        }

        #endregion

        #region HelperMethods

        public void UpdateColors()
        {
            ButtonBackgroundColor = SettingsConstant.BUTTON_BACKGROUND_COLOR;
            ButtonAccentColor = SettingsConstant.ACCENT_COLOR;
        }

        public void LoadSettings()
        {
            // Load settings (if exist)
            Setting.Load();

            ButtonBackgroundColor = SettingsConstant.BUTTON_BACKGROUND_COLOR;
            ButtonAccentColor = SettingsConstant.ACCENT_COLOR;
        }

        #endregion
    }
}