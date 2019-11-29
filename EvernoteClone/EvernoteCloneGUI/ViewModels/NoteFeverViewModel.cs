using Caliburn.Micro;
using EvernoteCloneGUI.Views;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Location;
using EvernoteCloneLibrary.Notebooks.Location.LocationUser;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public Notebook SelectedNotebook = null;
        public Note SelectedNote = null;

        public NotebookViewModel NotebookViewModel { get; set; }
        
        public ObservableCollection<TreeViewItem> NotebooksTreeView { set; get; } = new ObservableCollection<TreeViewItem>(new List<TreeViewItem> { });

        public ContextMenu FolderContext = new ContextMenu();
        public ContextMenu NotebookContext = new ContextMenu();

        public NoteFeverViewModel()
        {
            MenuItem addFolder = new MenuItem();
            addFolder.Header = "Add Folder";
            addFolder.Click += AddFolder;

            MenuItem addNotebook = new MenuItem();
            addNotebook.Header = "Add Notebook";
            addNotebook.Click += AddNotebook;

            FolderContext.Items.Add(addFolder);
            FolderContext.Items.Add(addNotebook);
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

            // TODO: IF the user is logged in (there should be a property here with the user), insert the UserID.
            // Temporary try/catch until issue is fixed with exceptions.
            try
            {
                Notebooks = Notebook.Load();
                if (Notebooks != null && Notebooks.Count > 0)
                {
                    Notebook tempNotebook = Notebooks.First();
                    if (tempNotebook != null && tempNotebook.Notes != null && tempNotebook.Notes.Count > 0)
                    {
                        Note tempNote = (Note)tempNotebook.Notes.First();
                        if (tempNote != null)
                        {
                            SelectedNotebook = tempNotebook;
                            SelectedNote = tempNote;
                        }
                    }

                }
            }
            catch (Exception) { };


            // Only do this when a note has been opened, otherwise the right side should still be empty.
            LoadNoteViewIfNoteExists();

            // Load all folders, notebooks and add them all to the view
            foreach (TreeViewItem treeViewItem in LoadNotebooks(LoadFolders()).Items.Cast<TreeViewItem>())
                NotebooksTreeView.Add(treeViewItem);

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
            if(Notebooks != null)
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
                foreach (Notebook notebook in Notebooks)
                {
                    foreach (Note note in notebook.Notes)
                    {
                        notes.Add(note);
                    }
                }

                allNotesNotebook.Notes = notes;
                
                if(allNotesNotebook.Notes.Count > 0)
                {
                    SelectedNotebook = allNotesNotebook;
                    SelectedNote = (Note)allNotesNotebook.Notes.First();
                    if (SelectedNote != null)
                    {
                        LoadNoteViewIfNoteExists();
                    }
                }
            }
        }
        
        public TreeView LoadFolders()
        {
            // Load all LocationUser records from UserID
            LocationUserRepository locationUserRepository = new LocationUserRepository();
            List<LocationUser> locationUserRecords = locationUserRepository.GetBy(
                new string[] { "UserID = @UserID" },
                new Dictionary<string, object>() { { "@UserID", 3 } } // TODO: change this!!!!
                ).Select((el) => ((LocationUser)el)).ToList();

            NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
            List<NotebookLocation> notebookLocationsFromDatabase = new List<NotebookLocation>();
            foreach (LocationUser locationUser in locationUserRecords)
            {
                var _notebookLocation = notebookLocationRepository.GetBy(
                    new string[] { "Id = @Id" },
                    new Dictionary<string, object>() { { "@Id", locationUser.LocationID } }
                    ).Select((el) => ((NotebookLocation)el)).ToList()[0];
                notebookLocationsFromDatabase.Add(_notebookLocation);
            }

            TreeView treeView = new TreeView();
            TreeViewItem currentNode = null;

            foreach (string path in notebookLocationsFromDatabase.Select(notebookLocation => notebookLocation.Path))
            {
                currentNode = null;
                foreach (string directory in path.Split('/'))
                {
                    if (currentNode == null)
                    {
                        if (treeView.Items.Cast<TreeViewItem>().Any(item => item.Header.ToString() == directory))
                        {
                            currentNode = treeView.Items.Cast<TreeViewItem>().First(item => item.Header.ToString() == directory);
                        }
                        else
                        {
                            currentNode = CreateTreeNode(directory, FolderContext);
                            treeView.Items.Add(currentNode);
                        }
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

            return treeView;
        }

        public TreeView LoadNotebooks(TreeView treeView)
        {
            TreeViewItem currentNode = null;
            List<Notebook> notebooks = Notebook.Load(3);
            foreach (Notebook notebook in notebooks)
            {
                NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
                string path = notebookLocationRepository.GetBy(
                    new string[] { "Id = @Id" },
                    new Dictionary<string, object>() { { "@Id", notebook.LocationID } }
                    ).Select((el) => ((NotebookLocation)el)).ToList()[0].Path;

                currentNode = null;
                foreach (string directory in path.Split('/'))
                {
                    if (currentNode == null)
                        currentNode = treeView.Items.Cast<TreeViewItem>().First(item => item.Header.ToString() == directory);
                    else
                        currentNode = currentNode.Items.Cast<TreeViewItem>().First(item => item.Header.ToString() == directory);
                }

                currentNode.Items.Add(CreateTreeNode(notebook.Title, NotebookContext));
            }

            return treeView;
        }

        private TreeViewItem CreateTreeNode(string Header, ContextMenu contextMenu)
        {
            TreeViewItem treeViewItem = new TreeViewItem();
            treeViewItem.Header = Header;
            treeViewItem.Foreground = Brushes.White;
            treeViewItem.IsExpanded = false;
            treeViewItem.FontSize = 14;
            treeViewItem.ContextMenu = contextMenu;
            return treeViewItem;
        }

        public void AddFolder(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.Source as MenuItem;
            if (menuItem != null)
            {
                ContextMenu contextMenu = menuItem.Parent as ContextMenu;
                string path = GetPath(contextMenu.PlacementTarget as TreeViewItem);

                // TODO show window that asks for a name
                string newFolderName = new Random().Next().ToString();
                NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
                NotebookLocationModel notebookLocationModel = new NotebookLocationModel() { Path = path + "/" + newFolderName };
                if (notebookLocationRepository.Insert(notebookLocationModel))
                {
                    LocationUserRepository locationUserRepository = new LocationUserRepository();
                    locationUserRepository.Insert(new LocationUserModel() { LocationID = notebookLocationModel.Id, UserID = 3 }); // TODO change userid
                }

                // TODO fix refresh (for now, delete and add)
                NotebooksTreeView.Clear();
                foreach (TreeViewItem treeViewItem in LoadNotebooks(LoadFolders()).Items.Cast<TreeViewItem>())
                    NotebooksTreeView.Add(treeViewItem);
            }
        }

        public void AddNotebook(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = e.Source as MenuItem;
            if (menuItem != null)
            {
                ContextMenu contextMenu = menuItem.Parent as ContextMenu;
                string path = GetPath(contextMenu.PlacementTarget as TreeViewItem);

                // TODO show window that asks for a name
                string newNotebookName = new Random().Next().ToString();

                NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
                int locationID = notebookLocationRepository.GetBy(
                        new string[] { "Path = @Path" },
                        new Dictionary<string, object>() { { "@Path", path } }
                        ).Select((el) => ((NotebookLocation)el)).ToList()[0].Id;

                NotebookRepository notebookRepository = new NotebookRepository();
                NotebookModel notebookModel = new NotebookModel() { UserID = 3, LocationID = locationID, Title = newNotebookName, CreationDate = DateTime.Now.Date, LastUpdated = DateTime.Now };
                notebookRepository.Insert(notebookModel);

                // TODO fix refresh (for now, delete and add)
                NotebooksTreeView.Clear();
                foreach (TreeViewItem treeViewItem in LoadNotebooks(LoadFolders()).Items.Cast<TreeViewItem>())
                    NotebooksTreeView.Add(treeViewItem);
            }
        }

        public void AddNote(object sender, RoutedEventArgs e)
        {
            // TODO Change this!
            MenuItem menuItem = e.Source as MenuItem;
            if (menuItem != null)
            {
                ContextMenu contextMenu = menuItem.Parent as ContextMenu;
                string path = GetPath(contextMenu.PlacementTarget as TreeViewItem);
            }
        }

        private string GetPath(TreeViewItem treeViewItem)
        {
            string path = treeViewItem.Header.ToString();
            while (treeViewItem.Parent is TreeViewItem)
            {
                treeViewItem = treeViewItem.Parent as TreeViewItem;
                path = treeViewItem.Header.ToString() + "/" + path;
            }

            // TODO remove this write line, this is only to show it works
            Console.WriteLine(path);
            return path;
        }

    }
}
