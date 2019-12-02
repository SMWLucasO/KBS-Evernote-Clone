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

// TODO add summary
namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// The root viewmodel, this is the viewmodel which handles the main application screen.
    /// </summary>
    public class NoteFeverViewModel : Conductor<object>
    {
        /// <value>
        /// Notebooks contains all the local notebooks (and online notebooks, if UserID != 1)
        /// </value>
        public List<Notebook> Notebooks { get; private set; } = new List<Notebook>();

        /// <value>
        /// SelectedNotebook contains the currently selected Notebook
        /// </value>
        public Notebook SelectedNotebook;
        
        /// <value>
        /// SelectedNote contains the currently selected Note
        /// </value>
        public Note SelectedNote;

        /// <value>
        /// UserID contains the Id of the currently logged in user (TODO change with User object)
        /// </value>
        private int UserID = -1; // TODO change this!!!

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

        /// <summary>
        /// This method creates a menu item (for a ContextMenu)
        /// </summary>
        /// <param name="Header">The display header</param>
        /// <param name="CustomEventHandler">A method that handles the button_click event</param>
        /// <returns>Returns a MenuItem</returns>
        public MenuItem CreateMenuItem(string Header, RoutedEventHandler CustomEventHandler)
        {
            MenuItem menuItem = new MenuItem {Header = Header};
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

            // Load all folders, notebooks and add them all to the view
            LoadNotebooksTreeView();

            // Load Notebooks
            LoadNotebooks(true);

            // Only do this when a note has been opened, otherwise the right side should still be empty.
            LoadNoteViewIfNoteExists();
        }

        /// <summary>
        /// This loads all the notebooks from the filesystem (and from the database as well, if UserID != 1)
        /// </summary>
        /// <param name="initialLoad">If this is true, it loads the SelectedNotebook and SelectedNote</param>
        private void LoadNotebooks(bool initialLoad = false)
        {   
            // Load all Notebooks
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

        /// <summary>
        /// This loads the NoteView if there are notes inside the Notebook
        /// </summary>
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
            } 
            else
                MessageBox.Show("Cannot add new notes whilst in 'all notes' mode.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void User()
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
                    foreach (Note note in notebook.Notes.Cast<Note>())
                        notes.Add(note);

                allNotesNotebook.Notes = notes;
                
                if(allNotesNotebook.Notes.Count > 0)
                {
                    SelectedNotebook = allNotesNotebook;
                    SelectedNote = (Note)allNotesNotebook.Notes.First();
                    if (SelectedNote != null)
                        LoadNoteViewIfNoteExists();
                }
            }
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
            
            /*
            // Clear the NotebooksTreeView and add the folder and notebook structure (also save the currently selected folder, and select it again)
            string path, notebookTitle = "";
            bool isNotebook = IsNotebook(SelectedTreeViewItem);
            if (isNotebook)
            {
                notebookTitle = GetHeader(SelectedTreeViewItem);
                path = GetPath(SelectedTreeViewItem, true);
            }
            else
                path = GetPath(SelectedTreeViewItem);
            SelectPath(ref rootTreeViewItem, path, notebookTitle);
*/
            NotebooksTreeView.Clear();
            NotebooksTreeView.Add(rootTreeViewItem);
        }

        private void SelectPath(ref TreeViewItem rootTreeViewItem, string path, string notebookTitle)
        {
            if (string.IsNullOrWhiteSpace(notebookTitle))
            {
                string[] splittedPath = path.Split('/');
            }
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

        private List<TreeViewItem> LoadNotebooksIntoFolderStructure(List<TreeViewItem> TreeViewItems)
        {
            LoadNotebooks();
            foreach (Notebook notebook in Notebooks)
            {
                TreeViewItem currentNode = null;
                foreach (string directory in notebook.Path.Path.Split('/'))
                {
                    if (currentNode == null)
                        currentNode = TreeViewItems.First(treeViewItem => GetHeader(treeViewItem) == directory);
                    else
                        currentNode = currentNode.Items.Cast<TreeViewItem>().First(treeViewItem => GetHeader(treeViewItem) == directory);
                }

                currentNode?.Items.Add(CreateTreeNode(notebook.Title, NotebookContext));
            }

            return TreeViewItems;
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
            StackPanel stackPanel = new StackPanel {Orientation = Orientation.Horizontal};

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

        public void AddFolder(object Sender, RoutedEventArgs RoutedEventArgs)
        {
            if (RoutedEventArgs.Source is MenuItem menuItem)
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
                if (menuItem.Parent is ContextMenu contextMenu)
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

        public string GetUserInput(string DialogTitle, string DialogValueRequestText, int MinCharacters = 2, int MaxCharacters = 64)
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
            
            // If valueRequestViewModel.Value == null, cancel button is pressed
            if (valueRequestViewModel.Value != null)
            {
                while (!((valueRequestViewModel.Value = valueRequestViewModel.Value.Trim()).Length >=
                       MinCharacters
                       && valueRequestViewModel.Value.Length <= MaxCharacters))
                {
                    MessageBox.Show($"Text should be between {MinCharacters} and {MaxCharacters} characters long.",
                        "NoteFever | Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    valueRequestViewModel.Value = "";
                }
            }

            return string.IsNullOrWhiteSpace(valueRequestViewModel.Value) ? null : valueRequestViewModel.Value.Trim();
        }

        public void HandleSubmit(ValueRequestViewModel ValueRequestViewModel) =>
            (ValueRequestViewModel.GetView() as Window)?.Close();
        public void HandleCancel(ValueRequestViewModel ValueRequestViewModel)
        {
            ValueRequestViewModel.Value = null;
            (ValueRequestViewModel.GetView() as Window)?.Close();
        }

        private string GetPath(TreeViewItem TreeViewItem, bool IsNotebook = false)
        {
            if (GetHeader(TreeViewItem) == "My Notebooks")
                return "";

            string path = IsNotebook ? "" : GetHeader(TreeViewItem);
            while (TreeViewItem.Parent is TreeViewItem item)
            {
                TreeViewItem = item;
                if (GetHeader(TreeViewItem) == "My Notebooks")
                    break;
                path = GetHeader(TreeViewItem) + "/" + path;
            }
            return IsNotebook ? path.Substring(0, path.Length - 1) : path;
        }

        private int GetNotebookID(TreeViewItem TreeViewItem)
        {
            if (TreeViewItem.Header is StackPanel stackPanel)
                if (stackPanel.Children[2] is Label label)
                    return Convert.ToInt32(label.Content);
            return -1;
        }

        private bool IsNotebook(TreeViewItem TreeViewItem)
            => TreeViewItem.ContextMenu == NotebookContext;

        private string GetHeader(TreeViewItem TreeViewItem)
        {
            if (TreeViewItem.Header is StackPanel stackPanel)
                if (stackPanel.Children[1] is TextBlock textBlock)
                    return textBlock.Text;
            return TreeViewItem.Header.ToString();
        }

        public void TreeView_SelectedItemChanged(RoutedPropertyChangedEventArgs<object> RoutedPropertyChangedEventArgs)
        {
            if (RoutedPropertyChangedEventArgs.NewValue is TreeViewItem treeViewItem)
            {
                SelectedTreeViewItem = treeViewItem;
                
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

        public void Login()
        {
            IWindowManager windowManager = new WindowManager();     

            LoginViewModel loginViewModel = new LoginViewModel();
            windowManager.ShowDialog(loginViewModel, null);
        }
        
        public void SelectNotebook(string Path, string Title)
        {
            SelectedNotebook = Notebooks.First(notebook => notebook.Path.Path == Path && notebook.Title == Title);
            LoadNoteViewIfNoteExists();
        }
    }
}
