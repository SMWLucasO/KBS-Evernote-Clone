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
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        private int UserID = 3; // TODO change this!!!

        public NotebookViewModel NotebookViewModel { get; set; }
        public ObservableCollection<TreeViewItem> NotebooksTreeView { get; } = new ObservableCollection<TreeViewItem>(new List<TreeViewItem>());

        public ContextMenu RootContext = new ContextMenu();
        public ContextMenu FolderContext = new ContextMenu();
        public ContextMenu NotebookContext = new ContextMenu();

        public BitmapImage FolderImage { get; } = new BitmapImage(new Uri("pack://application:,,,/EvernoteCloneGUI;component/Resources/folder.png"));
        public BitmapImage NotebookImage { get; } = new BitmapImage(new Uri("pack://application:,,,/EvernoteCloneGUI;component/Resources/journal.png"));
        //public BitmapImage NotebookImage { get; } = new BitmapImage(new Uri(@"/Resources/ok-01.png", UriKind.Relative));

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

            // Load Notebooks
            LoadNotebooks(true);

            // Only do this when a note has been opened, otherwise the right side should still be empty.
            LoadNoteViewIfNoteExists();

            // Load all folders, notebooks and add them all to the view
            LoadNotebooksTreeView();

        }

        private void LoadNotebooks(bool initialLoad = false)
        {
            // TODO: IF the user is logged in (there should be a property here with the user), insert the UserID.
            Notebooks = Notebook.Load(UserID);

            if (Notebooks.Count > 0)
            {
                Notebook tempNotebook = Notebooks.First();
                Note tempNote = null;

                if (tempNotebook.Notes.Count > 0)
                    tempNote = (Note)tempNotebook.Notes.First();

                if (initialLoad && SelectedNotebook == null)
                    SelectedNotebook = tempNotebook;
                if (tempNote != null && initialLoad && SelectedNote == null)
                    SelectedNote = tempNote;
            }
        }

        public void LoadNoteViewIfNoteExists()
        {

            if (SelectedNotebook != null)
            {
                NewNoteViewModel newNoteViewModel = null;
                if (SelectedNote != null)
                {
                    newNoteViewModel = new NewNoteViewModel(true)
                    {
                        Note = SelectedNote,
                        NoteOwner = SelectedNotebook,
                        Parent = this
                    };
                }
                // Create the notebook view with the required data.
                NotebookViewModel = new NotebookViewModel()
                {
                    NewNoteViewModel = newNoteViewModel,
                    NotebookNotesMenu = new NotebookNotesMenuViewModel()
                    {
                        Notebook = SelectedNotebook,
                        NotebookName = SelectedNotebook.Title,
                        NotebookNoteCount = $"{SelectedNotebook.Notes.Count} note(s)",
                        Parent = this
                    }
                };



                NotebookViewModel.NewNoteViewModel?.LoadNote();
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
            foreach (TreeViewItem treeViewItem in LoadNotebooksIntoFolderStructure(LoadFolders()))
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
                        if (treeViewItems.Any(treeViewItem => GetHeader(treeViewItem) == directory))
                            currentNode = treeViewItems.First(treeViewItem => GetHeader(treeViewItem) == directory);
                        else
                            treeViewItems.Add(currentNode = CreateTreeNode(directory, FolderContext));
                    }
                    else if (currentNode.Items.Cast<TreeViewItem>().Any(treeViewItem => GetHeader(treeViewItem) == directory))
                    {
                        currentNode = currentNode.Items.Cast<TreeViewItem>().First(treeViewItem => GetHeader(treeViewItem) == directory);
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

        private List<TreeViewItem> LoadNotebooksIntoFolderStructure(List<TreeViewItem> treeViewItems)
        {
            LoadNotebooks();
            foreach (Notebook notebook in Notebooks)
            {
                TreeViewItem currentNode = null;
                foreach (string directory in notebook.Path.Path.Split('/'))
                {
                    if (currentNode == null)
                        currentNode = treeViewItems.First(treeViewItem => GetHeader(treeViewItem) == directory);
                    else
                        currentNode = currentNode.Items.Cast<TreeViewItem>().First(treeViewItem => GetHeader(treeViewItem) == directory);
                }

                currentNode?.Items.Add(CreateTreeNode(notebook.Title, NotebookContext));
            }

            return treeViewItems;
        }

        private TreeViewItem CreateTreeNode(string Header, ContextMenu ContextMenu, int NotebookID = -1)
        {
            TreeViewItem treeViewItem = new TreeViewItem();
            treeViewItem.Header = CreateTreeHeader(Header, ContextMenu, NotebookID);
            treeViewItem.Foreground = Brushes.White;
            treeViewItem.IsExpanded = false;
            treeViewItem.FontSize = 14;
            treeViewItem.ContextMenu = ContextMenu;
            return treeViewItem;
        }

        private StackPanel CreateTreeHeader(string Header, ContextMenu ContextMenu, int NotebookID = -1)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            Image image = new Image();
            image.Height = 16;
            image.Width = 16;

            if (ContextMenu == FolderContext || ContextMenu == RootContext)
                image.Source = FolderImage;
            else if (ContextMenu == NotebookContext)
                image.Source = NotebookImage;
            stackPanel.Children.Add(image);

            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.Add(Header);
            stackPanel.Children.Add(textBlock);

            if (ContextMenu == NotebookContext)
            {
                Label label = new Label();
                label.Content = NotebookID + "";
                label.Visibility = Visibility.Collapsed;
                stackPanel.Children.Add(label);
            }

            return stackPanel;
        }

        public void AddFolder(object sender, RoutedEventArgs e)
        {
            if (e.Source is MenuItem menuItem)
            {
                if (menuItem.Parent is ContextMenu contextMenu)
                {
                    string path = GetPath(contextMenu.PlacementTarget as TreeViewItem);
                    string newFolderName = GetUserInput("Create new folder", "What do you want your new folder to be called:");

                    if (newFolderName != null)
                    {
                        NotebookLocation.AddNewNotebookLocation(new NotebookLocation() { Path = path + "/" + newFolderName }, UserID); // TODO: change UserID AND do something with return value

                        // TODO fix refresh (for now, delete and add)
                        LoadNotebooksTreeView();
                    }
                }
            }
        }

        public void AddFolderToRoot(object sender, RoutedEventArgs e)
        {
            string newFolderName = GetUserInput("Create new folder", "What do you want your new folder to be called:");
            if (newFolderName != null)
            {
                NotebookLocation.AddNewNotebookLocation(new NotebookLocation() { Path = newFolderName }, UserID); // TODO maybe do something with return value?

                // TODO fix refresh (for now, delete and add)
                LoadNotebooksTreeView();
            }
        }

        public void AddNotebook(object sender, RoutedEventArgs e)
        {
            if (e.Source is MenuItem menuItem)
            {
                ContextMenu contextMenu = menuItem.Parent as ContextMenu;
                if (contextMenu != null)
                {
                    string path = GetPath(contextMenu.PlacementTarget as TreeViewItem);
                    string newNotebookName = GetUserInput("Create new notebook", "What do you want your new notebook to be called:");

                    if (newNotebookName != null)
                    {
                        NotebookLocation notebookLocation = NotebookLocation.GetNotebookLocationByPath(path, UserID);
                        Notebook notebook = new Notebook() { UserID = UserID, LocationID = notebookLocation.Id, Title = newNotebookName, CreationDate = DateTime.Now.Date, LastUpdated = DateTime.Now, Path = notebookLocation };
                        notebook.Save(UserID);

                        // TODO fix refresh (for now, delete and add)
                        LoadNotebooksTreeView();
                    }
                }
            }
        }

        public void AddNote(object sender, RoutedEventArgs e) =>
            NewNote();

        public string GetUserInput(string DialogTitle, string DialogValueRequestText)
        {
            IWindowManager windowManager = new WindowManager();

            ValueRequestViewModel valueRequestViewModel = new ValueRequestViewModel
            {
                Parent = this,
                DialogTitle = DialogTitle,
                DialogValueRequestText = DialogValueRequestText
            };
            valueRequestViewModel.Submission += HandleSubmit;
            valueRequestViewModel.Cancellation += HandleCancel;

            windowManager.ShowDialog(valueRequestViewModel);

            return string.IsNullOrWhiteSpace(valueRequestViewModel.Value) ? null : valueRequestViewModel.Value.Trim();
        }

        public void HandleSubmit(ValueRequestViewModel valueRequestViewModel) =>
            (valueRequestViewModel.GetView() as Window).Close();
        public void HandleCancel(ValueRequestViewModel valueRequestViewModel)
        {
            valueRequestViewModel.Value = null;
            (valueRequestViewModel.GetView() as Window).Close();
        }

        private string GetPath(TreeViewItem treeViewItem, bool IsNotebook = false)
        {
            if (GetHeader(treeViewItem) == "My Notebooks")
                return "";

            string path = IsNotebook ? "" : GetHeader(treeViewItem);
            while (treeViewItem.Parent is TreeViewItem item)
            {
                treeViewItem = item;
                if (GetHeader(treeViewItem) == "My Notebooks")
                    break;
                path = GetHeader(treeViewItem) + "/" + path;
            }
            return IsNotebook ? path.Substring(0, path.Length - 1) : path;
        }

        private int GetNotebookID(TreeViewItem treeViewItem)
        {
            if (treeViewItem.Header is StackPanel stackPanel)
                if (stackPanel.Children[2] is Label label)
                    return Convert.ToInt32(label.Content);
            return -1;
        }

        private bool IsNotebook(TreeViewItem treeViewItem)
            => treeViewItem.ContextMenu == NotebookContext;

        private string GetHeader(TreeViewItem treeViewItem)
        {
            if (treeViewItem.Header is StackPanel stackPanel)
                if (stackPanel.Children[1] is TextBlock textBlock)
                    return textBlock.Text;
            return treeViewItem.Header.ToString();
        }

        public void TreeView_SelectedItemChanged(RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            if (routedPropertyChangedEventArgs.NewValue is TreeViewItem treeViewItem)
            {
                if (!IsNotebook(treeViewItem))
                    return;

                int notebookID = GetNotebookID(treeViewItem);
                if (notebookID != -1)
                    SelectNotebook(notebookID);
                else
                    SelectNotebook(GetPath(treeViewItem, true), GetHeader(treeViewItem));
            }
        }

        public void SelectNotebook(int NotebookID)
        {
            SelectedNotebook = Notebooks.First(notebook => notebook.Id == NotebookID);
            LoadNoteViewIfNoteExists();
        }

        public void SelectNotebook(string Path, string Title)
        {
            SelectedNotebook = Notebooks.First(notebook => notebook.Path.Path == Path && notebook.Title == Title);
            LoadNoteViewIfNoteExists();
        }
    }
}
