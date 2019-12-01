using Caliburn.Micro;
using EvernoteCloneGUI.Views;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Location;
using EvernoteCloneLibrary.Notebooks.Location.LocationUser;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

// TODO add summary

namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// The root viewmodel, this is the viewmodel which handles the main application screen.
    /// </summary>
    public class NoteFeverViewModel : Conductor<object>
    {

        // Notebook information for viewing things
        public List<Notebook> Notebooks { get; private set; }
            = new List<Notebook>();

        public Notebook SelectedNotebook;
        public Note SelectedNote;

        private int UserID = -1; // TODO change this!!!

        public NotebookViewModel NotebookViewModel { get; set; }
        public ObservableCollection<TreeViewItem> NotebooksTreeView { get; } = new ObservableCollection<TreeViewItem>(new List<TreeViewItem>());

        public ContextMenu RootContext = new ContextMenu();
        public ContextMenu FolderContext = new ContextMenu();
        public ContextMenu NotebookContext = new ContextMenu();

        public MenuItem CreateMenuItem(string Header, RoutedEventHandler CustomEventHandler)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Header = Header;
            menuItem.Click += CustomEventHandler;
            return menuItem;
        }

        // <User object stuff here>
        // <ReplaceThis>
        // </User object stuff here>

        /// <summary>
        /// Whenever we know that there is a note selected,
        /// we want to switch to the note user control, which will display it with all its data.
        /// </summary>
        protected override void OnActivate()
        {
            // First load contextmenu's
            RootContext.Items.Add(CreateMenuItem("Add Folder", AddFolderToRoot));
            FolderContext.Items.Add(CreateMenuItem("Add Folder", AddFolder));
            FolderContext.Items.Add(CreateMenuItem("Add Notebook", AddNotebook));
            NotebookContext.Items.Add(CreateMenuItem("Add Note", AddNote));

            // TODO: IF the user is logged in (there should be a property here with the user), insert the UserID.
            // Temporary try/catch until issue is fixed with exceptions.
            try
            {
                Notebooks = Notebook.Load();
                Notebook tempNotebook = Notebooks?.First();
                Note tempNote = (Note) tempNotebook?.Notes.First();
                if (tempNote != null)
                {
                    SelectedNotebook = tempNotebook;
                    SelectedNote = tempNote;
                }
            }
            catch (Exception)
            {
                // ignored
            }


            // Only do this when a note has been opened, otherwise the right side should still be empty.
            LoadNoteViewIfNoteExists();

            // Load all folders, notebooks and add them all to the view
            LoadNotebooksTreeView();

        }

        public void LoadNoteViewIfNoteExists()
        {

            if (SelectedNote != null && SelectedNotebook != null)
            {

                // Create the notebook view with the required data.
                NotebookViewModel = new NotebookViewModel()
                {
                    NewNoteViewModel = new NewNoteViewModel(true)
                    {
                        Note = SelectedNote,
                        NoteOwner = SelectedNotebook,
                        Parent = this
                    },
                    NotebookNotesMenu = new NotebookNotesMenuViewModel()
                    {
                        Notebook = SelectedNotebook,
                        NotebookName = SelectedNotebook.Title,
                        NotebookNoteCount = $"{SelectedNotebook.Notes.Count} note(s)",
                        Parent = this
                    }
                };



                NotebookViewModel.NewNoteViewModel.LoadNote();
                NotebookViewModel.NotebookNotesMenu.LoadNotesIntoNotebookMenu();

                ActivateItem(NotebookViewModel);

            }
        }

        /// <summary>
        /// Open the Window responsible for the creation of new notes.
        /// </summary>
        public void NewNote()
        {
            // it will get confusing if I don't use an '==' here (It is placed for readability purposes)
            if(SelectedNotebook == null || SelectedNotebook.IsNotNoteOwner == false)
            {
                IWindowManager windowManager = new WindowManager();

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
            } else
            {
                MessageBox.Show("Cannot add new notes whilst in 'all notes' mode.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Method which opens the view containing all the user's notes.
        /// </summary>
        public void OpenAllNotes()
        {
            Notebook allNotesNotebook = new Notebook()
            {
                Id = -1,
                LocationID = -1,
                UserID = -1,
                Title = "All notes",
                LastUpdated = DateTime.Now,
                CreationDate = DateTime.Now.Date,
                IsNotNoteOwner = true
            };
            List<INote> notes = new List<INote>();
            foreach(Notebook notebook in Notebooks)
            {
                foreach(Note note in notebook.Notes)
                {
                    notes.Add(note);
                }
            }

            allNotesNotebook.Notes = notes;

            SelectedNotebook = allNotesNotebook;
            SelectedNote = (Note) allNotesNotebook.Notes.First();

            LoadNoteViewIfNoteExists();
        }

        public void LoadNotebooksTreeView()
        {
            NotebooksTreeView.Clear();
            TreeViewItem rootTreeViewItem = CreateTreeNode("My Notebooks", RootContext);
            foreach (TreeViewItem treeViewItem in LoadNotebooks(LoadFolders()))
                rootTreeViewItem.Items.Add(treeViewItem);
            NotebooksTreeView.Add(rootTreeViewItem);
        }

        private List<TreeViewItem> LoadFolders()
        {
            List<NotebookLocation> notebookLocations = NotebookLocation.Load(UserID); // TODO: change UserID
            List<TreeViewItem> treeViewItems = new List<TreeViewItem>();
            foreach (string path in notebookLocations.Select(notebookLocation => notebookLocation.Path))
            {
                TreeViewItem currentNode = null;
                foreach (string directory in path.Split('/'))
                {
                    if (currentNode == null)
                    {
                        if (treeViewItems.Any(treeViewItem => treeViewItem.Header.ToString() == directory))
                            currentNode = treeViewItems.First(treeViewItem => treeViewItem.Header.ToString() == directory);
                        else
                            treeViewItems.Add(currentNode = CreateTreeNode(directory, FolderContext));
                    }
                    else if (currentNode.Items.Cast<TreeViewItem>().Any(item => item.Header.ToString() == directory))
                    {
                        currentNode = currentNode.Items.Cast<TreeViewItem>().First(item => item.Header.ToString() == directory);
                    }
                    else
                    {
                        var newNode = CreateTreeNode(directory, FolderContext);
                        currentNode.Items.Add(newNode);
                        currentNode = newNode;
                    }
                }
            }

            return treeViewItems;
        }

        private List<TreeViewItem> LoadNotebooks(List<TreeViewItem> treeViewItems)
        {
            List<Notebook> notebooks = Notebook.Load(3);
            foreach (Notebook notebook in notebooks)
            {
                TreeViewItem currentNode = null;
                foreach (string directory in notebook.Path.Path.Split('/'))
                {
                    if (currentNode == null)
                        currentNode = treeViewItems.First(treeViewItem => treeViewItem.Header.ToString() == directory);
                    else
                        currentNode = currentNode.Items.Cast<TreeViewItem>().First(treeViewItem => treeViewItem.Header.ToString() == directory);
                }

                currentNode?.Items.Add(CreateTreeNode(notebook.Title, NotebookContext));
            }

            return treeViewItems;
        }

        private TreeViewItem CreateTreeNode(string Header, ContextMenu ContextMenu)
        {
            TreeViewItem treeViewItem = new TreeViewItem();
            treeViewItem.Header = Header;
            treeViewItem.Foreground = Brushes.White;
            treeViewItem.IsExpanded = false;
            treeViewItem.FontSize = 14;
            treeViewItem.ContextMenu = ContextMenu;
            return treeViewItem;
        }

        public void AddFolder(object sender, RoutedEventArgs e)
        {
            if (e.Source is MenuItem menuItem)
            {
                if (menuItem.Parent is ContextMenu contextMenu)
                {
                    string path = GetPath(contextMenu.PlacementTarget as TreeViewItem);

                    // TODO show window that asks for a name
                    string newFolderName = "[F] "+new Random().Next();
                    NotebookLocation.AddNewNotebookLocation(new NotebookLocation() { Path = path + "/" + newFolderName }, UserID); // TODO: change UserID AND do something with return value
                }

                // TODO fix refresh (for now, delete and add)
                LoadNotebooksTreeView();
            }
        }

        public void AddFolderToRoot(object sender, RoutedEventArgs e)
        {
            // TODO show window that asks for a name
            string newFolderName = "[RF] "+new Random().Next();
            NotebookLocation.AddNewNotebookLocation(new NotebookLocation() { Path = newFolderName }, UserID); // TODO: change UserID AND do something with return value

            // TODO fix refresh (for now, delete and add)
            LoadNotebooksTreeView();
        }

        public void AddNotebook(object sender, RoutedEventArgs e)
        {
            if (e.Source is MenuItem menuItem)
            {
                ContextMenu contextMenu = menuItem.Parent as ContextMenu;
                if (contextMenu != null)
                {
                    string path = GetPath(contextMenu.PlacementTarget as TreeViewItem);

                    // TODO show window that asks for a name (or notebook!!!)
                    string newNotebookName = "[NB] "+new Random().Next();
                    NotebookLocation notebookLocation = NotebookLocation.GetNotebookLocationByPath(path);

                    Notebook notebook = new Notebook() { UserID = UserID, LocationID = notebookLocation.Id, Title = newNotebookName, CreationDate = DateTime.Now.Date, LastUpdated = DateTime.Now, Path = notebookLocation };
                    notebook.Save(UserID); // TODO pass good UserID
                }

                // TODO fix refresh (for now, delete and add)
                LoadNotebooksTreeView();
            }
        }

        public void AddNote(object sender, RoutedEventArgs e)
        {
            // TODO Change this!
            if (e.Source is MenuItem menuItem)
            {
                ContextMenu contextMenu = menuItem.Parent as ContextMenu;
                if (contextMenu != null)
                {
                    string path = GetPath(contextMenu.PlacementTarget as TreeViewItem);
                }
            }
        }

        private string GetPath(TreeViewItem treeViewItem)
        {
            if (treeViewItem.Header.ToString() == "My Notebooks")
                return "";

            string path = treeViewItem.Header.ToString();
            while (treeViewItem.Parent is TreeViewItem item)
            {
                treeViewItem = item;
                if (treeViewItem.Header.ToString() == "My Notebooks")
                    break;
                path = treeViewItem.Header + "/" + path;
            }
            return path;
        }
    }
}
