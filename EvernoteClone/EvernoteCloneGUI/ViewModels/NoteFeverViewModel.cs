using Caliburn.Micro;
using EvernoteCloneGUI.Views;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EvernoteCloneLibrary.Users;

// TODO add summary
namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// The root viewmodel, this is the viewmodel which handles the main application screen.
    /// </summary>
    public class NoteFeverViewModel : Conductor<object>
    {
        #region Properties

        LoginViewModel loginViewModel = new LoginViewModel();
        User loginUser = null;

        /// <value>
        /// Notebooks contains all the local notebooks (and online notebooks, if UserID != 1)
        /// </value>
        public List<Notebook> Notebooks { get; private set; } = new List<Notebook>();

        /// <value>
        /// SelectedNotebook contains the currently selected Notebook
        /// </value>
        public Notebook SelectedNotebook { get; set; }

        /// <value>
        /// SelectedNote contains the currently selected Note
        /// </value>
        public Note SelectedNote { get; set; }

        /// <value>
        /// UserID contains the Id of the currently logged in user (TODO change with User object)
        /// </value>
        private int _userId = -1; // TODO change this!!!

        /// <value>
        /// NotebookViewModel contains the (only???) instance of the NotebookViewModel
        /// That is used to display all the notes inside a Notebook
        /// </value>
        public NotebookViewModel NotebookViewModel { get; set; }

        /// <value>
        /// NotebooksTreeView contains the whole folder structure together with all the notebooks
        /// </value>
        public ObservableCollection<TreeViewItem> NotebooksTreeView { get; } = new ObservableCollection<TreeViewItem>(new List<TreeViewItem>());

        /// <value>
        /// RootContext contains a ContextMenu with a button to Add Folders to the root item
        /// </value>
        public readonly ContextMenu RootContext = new ContextMenu();

        /// <value>
        /// FolderContext contains a ContextMenu with 2 buttons, one to Add Folders and one to Add Notebooks, to the selected folder
        /// </value>
        public readonly ContextMenu FolderContext = new ContextMenu();

        /// <value>
        /// NotebookContext contains a ContextMenu with a button to add Notes to the selected Notebook
        /// </value>
        public readonly ContextMenu NotebookContext = new ContextMenu();

        /// <value>
        /// FolderImage contains the image used to display a folder in the treeview
        /// </value>
        public BitmapImage FolderImage { get; } = new BitmapImage(new Uri("pack://application:,,,/EvernoteCloneGUI;component/Resources/folder.png"));

        /// <value>
        /// NotebookImage contains the image used to display a notebook in the treeview
        /// </value>
        public BitmapImage NotebookImage { get; } = new BitmapImage(new Uri("pack://application:,,,/EvernoteCloneGUI;component/Resources/journal.png"));

        /// <summary>
        /// SelectedTreeViewItem contains the currently selected tree view item
        /// </summary>
        public TreeViewItem SelectedTreeViewItem;

        #endregion

        // <User object stuff here>
        // <ReplaceThis>
        // </User object stuff here>

        #region Notebook loading

        /// <summary>
        /// This loads all the notebooks from the filesystem (and from the database as well, if UserID != 1)
        /// </summary>
        /// <param name="initialLoad">If this is true, it loads the SelectedNotebook and SelectedNote</param>
        private void LoadNotebooks(bool initialLoad = false)
        {
            // Load all Notebooks
            Notebooks = Notebook.Load(_userId);

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

        #endregion

        #region Treeview impl. for notes and notebooks

        /// <summary>
        /// This method creates a menu item (for a ContextMenu)
        /// </summary>
        /// <param name="header">The display header</param>
        /// <param name="customEventHandler">A method that handles the button_click event</param>
        /// <returns>Returns a MenuItem</returns>
        public MenuItem CreateMenuItem(string header, RoutedEventHandler customEventHandler)
        {
            MenuItem menuItem = new MenuItem { Header = header };
            menuItem.Click += customEventHandler;
            return menuItem;
        }

        /// <summary>
        /// (Re)loads the folder structure and notebooks
        /// </summary>
        public void LoadNotebooksTreeView()
        {
            // Create a root TreeViewItem
            TreeViewItem rootTreeViewItem = CreateTreeNode("My Notebooks", RootContext);

            // Load all Folders (LoadFolders) and attach Notebooks to them (LoadNotebooksIntoFolderStructure)
            // Now, loop over them all, and add them to the root TreeViewItem
            foreach (TreeViewItem treeViewItem in LoadNotebooksIntoFolderStructure(LoadFolders()))
                rootTreeViewItem.Items.Add(treeViewItem);

            // Clear the NotebooksTreeView and add the folder and notebook structure (also save the currently selected folder, and select it again)
            if (SelectedTreeViewItem != null)
                SelectPath(ref rootTreeViewItem, GetPath(SelectedTreeViewItem) + "/" + GetHeader(SelectedTreeViewItem));

            NotebooksTreeView.Clear();
            NotebooksTreeView.Add(rootTreeViewItem);
        }

        private void SelectPath(ref TreeViewItem rootTreeViewItem, string path)
        {
            rootTreeViewItem.IsExpanded = true;
            TreeViewItem currentNode = null;

            foreach (string directory in path.Split('/'))
            {
                if (currentNode != null)
                    currentNode.IsExpanded = true;

                if (currentNode == null)
                {
                    if (rootTreeViewItem.Items.Cast<TreeViewItem>().Any(treeViewItem => GetHeader(treeViewItem) == directory))
                        currentNode = rootTreeViewItem.Items.Cast<TreeViewItem>().First(treeViewItem => GetHeader(treeViewItem) == directory);
                    else
                        break;
                }
                else if (currentNode.Items.Cast<TreeViewItem>().Any(treeViewItem => GetHeader(treeViewItem) == directory))
                    currentNode = currentNode.Items.Cast<TreeViewItem>().First(treeViewItem => GetHeader(treeViewItem) == directory);
            }
        }

        private List<TreeViewItem> LoadFolders()
        {
            List<NotebookLocation> notebookLocations = NotebookLocation.Load(_userId); // TODO: change UserID
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

        private TreeViewItem CreateTreeNode(string header, ContextMenu contextMenu, int notebookId = -1)
        {
            TreeViewItem treeViewItem = new TreeViewItem();
            treeViewItem.Header = CreateTreeHeader(header, contextMenu, notebookId);
            treeViewItem.Foreground = Brushes.White;
            treeViewItem.IsExpanded = false;
            treeViewItem.FontSize = 14;
            treeViewItem.ContextMenu = contextMenu;
            return treeViewItem;
        }

        private StackPanel CreateTreeHeader(string header, ContextMenu contextMenu, int notebookId = -1)
        {
            StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

            Image image = new Image();
            image.Height = 16;
            image.Width = 16;

            if (contextMenu == FolderContext || contextMenu == RootContext)
                image.Source = FolderImage;
            else if (contextMenu == NotebookContext)
                image.Source = NotebookImage;
            stackPanel.Children.Add(image);

            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.Add(header);
            stackPanel.Children.Add(textBlock);

            if (contextMenu == NotebookContext)
            {
                Label label = new Label();
                label.Content = notebookId + "";
                label.Visibility = Visibility.Collapsed;
                stackPanel.Children.Add(label);
            }

            return stackPanel;
        }

        public void AddFolder(object sender, RoutedEventArgs routedEventArgs)
        {
            if (routedEventArgs.Source is MenuItem menuItem)
            {
                if (menuItem.Parent is ContextMenu contextMenu)
                {
                    string path = GetPath(contextMenu.PlacementTarget as TreeViewItem);
                    string newFolderName = GetUserInput("Create new folder", "What do you want your new folder to be called:");

                    if (newFolderName != null)
                    {
                        NotebookLocation.AddNewNotebookLocation(new NotebookLocation() { Path = path + "/" + newFolderName }, _userId); // TODO: change UserID AND do something with return value

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
                NotebookLocation.AddNewNotebookLocation(new NotebookLocation() { Path = newFolderName }, _userId); // TODO maybe do something with return value?

                // TODO fix refresh (for now, delete and add)
                LoadNotebooksTreeView();
            }
        }

        private string GetPath(TreeViewItem treeViewItem, bool isNotebook = false)
        {
            if (GetHeader(treeViewItem) == "My Notebooks")
                return "";

            string path = isNotebook ? "" : GetHeader(treeViewItem);
            while (treeViewItem.Parent is TreeViewItem item)
            {
                treeViewItem = item;
                if (GetHeader(treeViewItem) == "My Notebooks")
                    break;
                path = GetHeader(treeViewItem) + "/" + path;
            }
            return isNotebook ? path.Substring(0, path.Length - 1) : path;
        }

        private int GetNotebookId(TreeViewItem treeViewItem)
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

        #endregion

        #region Treeview context menu, including pop-ups

        public void AddNotebook(object sender, RoutedEventArgs e)
        {
            if (e.Source is MenuItem menuItem)
            {
                if (menuItem.Parent is ContextMenu contextMenu)
                {
                    string path = GetPath(contextMenu.PlacementTarget as TreeViewItem);
                    string newNotebookName = GetUserInput("Create new notebook", "What do you want your new notebook to be called:");

                    if (newNotebookName != null)
                    {
                        NotebookLocation notebookLocation = NotebookLocation.GetNotebookLocationByPath(path, _userId);
                        Notebook notebook = new Notebook() { UserId = _userId, LocationId = notebookLocation.Id, Title = newNotebookName, CreationDate = DateTime.Now.Date, LastUpdated = DateTime.Now, Path = notebookLocation };
                        notebook.Save(_userId);

                        // TODO fix refresh (for now, delete and add)
                        LoadNotebooksTreeView();
                    }
                }
            }
        }

        public void AddNote(object sender, RoutedEventArgs e) =>
            OpenNewNotePopupView();

        public string GetUserInput(string dialogTitle, string dialogValueRequestText, int minCharacters = 2, int maxCharacters = 64)
        {
            IWindowManager windowManager = new WindowManager();

            ValueRequestViewModel valueRequestViewModel = new ValueRequestViewModel
            {
                Parent = this,
                DialogTitle = dialogTitle,
                DialogValueRequestText = dialogValueRequestText
            };
            valueRequestViewModel.Submission += HandleSubmit;
            valueRequestViewModel.Cancellation += HandleCancel;

            windowManager.ShowDialog(valueRequestViewModel);

            // If valueRequestViewModel.Value == null, cancel button is pressed
            if (valueRequestViewModel.Value != null)
            {
                while (!((valueRequestViewModel.Value = valueRequestViewModel.Value.Trim()).Length >=
                       minCharacters
                       && valueRequestViewModel.Value.Length <= maxCharacters))
                {
                    valueRequestViewModel.Value = "";
                    if (MessageBox.Show($"Text should be between {minCharacters} and {maxCharacters} characters long.",
                            "NoteFever | Error", MessageBoxButton.OKCancel, MessageBoxImage.Error) ==
                        MessageBoxResult.Cancel)
                        break;
                }
            }

            return string.IsNullOrWhiteSpace(valueRequestViewModel.Value) ? null : valueRequestViewModel.Value.Trim();
        }

        public void HandleSubmit(ValueRequestViewModel valueRequestViewModel) =>
            (valueRequestViewModel.GetView() as Window)?.Close();
        public void HandleCancel(ValueRequestViewModel valueRequestViewModel)
        {
            valueRequestViewModel.Value = null;
            (valueRequestViewModel.GetView() as Window)?.Close();
        }

        #endregion

        #region Loading and opening views
        public void LoadNoteViewIfNoteExists(bool showDeletedNotes = false)
        {
            if (SelectedNotebook != null)
            {
                string notebookCountString = "0 note(s)";
                NewNoteViewModel newNoteViewModel = null;


                if (!showDeletedNotes)
                {
                    if (SelectedNotebook.Notes.Count > 0)
                    {
                        IEnumerable<INote> notes = SelectedNotebook.Notes.Where((note) => !((Note)note).IsDeleted);
                        if (notes.ToList().Count > 0)
                        {
                            notebookCountString = $"{notes.ToList().Count} note(s)";
                            SelectedNote = (Note)notes.First();
                        }
                        else
                        {
                            SelectedNote = null;
                        }
                    }
                    else
                    {
                        SelectedNote = null;
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
                NotebookViewModel = new NotebookViewModel()
                {
                    NewNoteViewModel = newNoteViewModel,
                    NotebookNotesMenu = new NotebookNotesMenuViewModel()
                    {
                        Notebook = SelectedNotebook,
                        NotebookName = SelectedNotebook.Title,
                        NotebookNoteCount = notebookCountString,
                        Parent = this
                    }
                };

                NotebookViewModel.NewNoteViewModel?.LoadNote();
                NotebookViewModel.NotebookNotesMenu.LoadNotesIntoNotebookMenu(showDeletedNotes);

                ActivateItem(NotebookViewModel);
            }
        }

        /// <summary>
        /// Open the Window responsible for the creation of new notes.
        /// </summary>
        public void OpenNewNotePopupView()
        {
            // it will get confusing if I don't use an '==' here (It is placed for readability purposes)
            if (SelectedNotebook == null || SelectedNotebook.IsNotNoteOwner == false)
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
            }
            else
                MessageBox.Show("Cannot add new notes whilst not in a notebook", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void OpenRegisterView()
        {
            IWindowManager windowManagerUser = new WindowManager();

            dynamic size = new ExpandoObject();
            size.Height = 600;
            size.Width = 800;
            size.SizeToContent = SizeToContent.Manual;

            RegisterView registerView = new RegisterView();
            windowManagerUser.ShowDialog(registerView, null, size);
        }

        /// <summary>
        /// Method which opens the view containing all the user's notes.
        /// </summary>
        public void OpenAllNotesView()
        {
            if (Notebooks != null)
            {
                Notebook allNotesNotebook = new Notebook()
                {
                    Id = -1,
                    LocationId = -1,
                    UserId = -1,
                    Title = "All notes",
                    LastUpdated = DateTime.Now,
                    CreationDate = DateTime.Now.Date,
                    IsNotNoteOwner = true
                };

                List<INote> notes = new List<INote>();
                foreach (Notebook notebook in Notebooks)
                {
                    // We want to retrieve all notes which are not deleted.
                    notes.AddRange(notebook.RetrieveNoteList((note) => (!((NoteModel)note).IsDeleted)));
                }

                allNotesNotebook.Notes = notes;

                if (!(ValidateAndLoadNotebookView(allNotesNotebook)))
                {
                    MessageBox.Show("There are no notes to view.", "Note Fever", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        /// <summary>
        /// Open the view for all deleted notes
        /// </summary>
        public void OpenDeletedNotesView()
        {
            Notebook trashNotebook = new Notebook()
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
        public void OpenLoginPopupView()
        {
            IWindowManager windowManager = new WindowManager();

            LoginViewModel loginViewModel = new LoginViewModel();
            windowManager.ShowDialog(loginViewModel, null);
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

        public void SelectNotebook(int notebookId)
        {
            SelectedNotebook = Notebooks.First(notebook => notebook.Id == notebookId);
            LoadNoteViewIfNoteExists();
        }


        public void SelectNotebook(string path, string title)
        {
            SelectedNotebook = Notebooks.First(notebook => notebook.Path.Path == path && notebook.Title == title);
            LoadNoteViewIfNoteExists();
        }

        #endregion

        public void Login()
        {
            IWindowManager windowManager = new WindowManager();

            LoginViewModel loginViewModel = new LoginViewModel();
            windowManager.ShowDialog(loginViewModel, null);

            loginUser = loginViewModel.user;
            _userId = loginUser?.Id ?? -1;
        }

        #region Events

        public void TreeView_SelectedItemChanged(RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            if (routedPropertyChangedEventArgs.NewValue is TreeViewItem treeViewItem)
            {
                SelectedTreeViewItem = treeViewItem;

                if (!IsNotebook(treeViewItem))
                    return;

                int notebookId = GetNotebookId(treeViewItem);
                if (notebookId != -1)
                    SelectNotebook(notebookId);
                else
                    SelectNotebook(GetPath(treeViewItem, true), GetHeader(treeViewItem));
            }
        }

        /// <summary>
        /// Whenever we know that there is a note selected,
        /// we want to switch to the note user control, which will display it with all its data.
        /// </summary>
        protected override void OnActivate()
        {
            Login();
            if (loginUser == null)
            {
                Environment.Exit(0);
            }

            // Set UserID equal to user input, this is for testing purposes only!
            //var userInputReturn = GetUserInput("UserID",
            //    "Input UserID for testing purposes! -1 is offline, 3 is online:", 1, 2);
            //if (userInputReturn == null)
            //    Environment.Exit(0);
            //_userId = int.Parse(userInputReturn);


            // First load contextmenu's
            RootContext.Items.Add(CreateMenuItem("Add Folder", AddFolderToRoot));
            FolderContext.Items.Add(CreateMenuItem("Add Folder", AddFolder));
            FolderContext.Items.Add(CreateMenuItem("Add Notebook", AddNotebook));
            NotebookContext.Items.Add(CreateMenuItem("Add Note", AddNote));

            // Load all folders, notebooks and add them all to the view
            LoadNotebooksTreeView();

            // Load Notebooks
            LoadNotebooks(true);

            // Only do this when a note has been opened, otherwise the right side should still be empty.
            LoadNoteViewIfNoteExists();
        }

        #endregion
    }
}