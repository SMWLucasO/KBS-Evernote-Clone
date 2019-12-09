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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Users;

namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// The root viewmodel, this is the viewmodel which handles the main application screen.
    /// </summary>
    public class NoteFeverViewModel : Conductor<object>
    {
        #region Properties

        /// <value>
        /// This contains the user object
        /// </value>
        public static User LoginUser;

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

        #region Notebook loading

        /// <summary>
        /// This loads all the notebooks from the filesystem (and from the database as well, if UserID != 1)
        /// </summary>
        /// <param name="initialLoad">If this is true, it loads the SelectedNotebook and SelectedNote</param>
        private void LoadNotebooks(bool initialLoad = false)
        {
            // Load all Notebooks
            Notebooks = Notebook.Load(LoginUser.Id);

            // if we're doing the initial loading, load the note and notebook if there is not already a selected note/notebook.
            SelectFirst();
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
                SelectedNote = (Note)notebook?.Notes.Where((note) => !((Note)note).IsDeleted).FirstOrDefault();
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
            foreach (TreeViewItem treeViewItem in AddNotebooksToFolderStructure(GetFolders()))
            {
                rootTreeViewItem.Items.Add(treeViewItem);
            }


            // Clear the NotebooksTreeView and add the folder and notebook structure (also save the currently selected folder, and select it again)
            if (SelectedTreeViewItem != null)
            {
                SelectPath(ref rootTreeViewItem, GetPath(SelectedTreeViewItem) + "/" + GetHeader(SelectedTreeViewItem));
            }

            NotebooksTreeView.Clear();
            NotebooksTreeView.Add(rootTreeViewItem);
        }

        /// <summary>
        /// Set the IsExpanded property of every TreeViewItem that is included in the given path
        /// </summary>
        /// <param name="rootTreeViewItem">The root TreeViewItem</param>
        /// <param name="path">Path that should be expanded</param>
        private void SelectPath(ref TreeViewItem rootTreeViewItem, string path)
        {
            rootTreeViewItem.IsExpanded = true;
            TreeViewItem currentNode = null;

            foreach (string directory in path.Split('/'))
            {
                if (currentNode != null)
                {
                    currentNode.IsExpanded = true;
                }

                if (currentNode == null)
                {
                    if (rootTreeViewItem.Items.Cast<TreeViewItem>().Any(treeViewItem => GetHeader(treeViewItem) == directory))
                    {
                        currentNode = rootTreeViewItem.Items.Cast<TreeViewItem>().First(treeViewItem => GetHeader(treeViewItem) == directory);
                    }
                    else
                    {
                        break;
                    }

                }
                else if (currentNode.Items.Cast<TreeViewItem>().Any(treeViewItem => GetHeader(treeViewItem) == directory))
                {
                    currentNode = currentNode.Items.Cast<TreeViewItem>().First(treeViewItem => GetHeader(treeViewItem) == directory);
                }

            }
        }

        /// <summary>
        /// Returns a list of NotebookLocations that are all subfolders of notebookLocation
        /// </summary>
        /// <param name="notebookLocation">Notebook Location from which all subfolders should be returned</param>
        /// <returns>Returns null if a notebook is part of the subfolders, else returns a list with all subfolders</returns>
        private List<NotebookLocation> GetSubFolders(NotebookLocation notebookLocation)
        {
            TreeViewItem currentNode = null;

            foreach (string directory in notebookLocation.Path.Split('/'))
            {
                if (currentNode == null)
                {
                    if (NotebooksTreeView[0].Items.Cast<TreeViewItem>().Any(treeViewItem => GetHeader(treeViewItem) == directory))
                    {
                        currentNode = NotebooksTreeView[0].Items.Cast<TreeViewItem>().First(treeViewItem => GetHeader(treeViewItem) == directory);
                    }
                    else
                    {
                        break;
                    }
                }
                else if (currentNode.Items.Cast<TreeViewItem>()
                    .Any(treeViewItem => GetHeader(treeViewItem) == directory))
                {
                    currentNode = currentNode.Items.Cast<TreeViewItem>()
                        .First(treeViewItem => GetHeader(treeViewItem) == directory);
                }
                else
                {
                    return null;
                }
            }

            bool notebookIsPartOfSubfolders = false;
            List<NotebookLocation> notebookLocations = RecursiveGetSubFolders(currentNode, ref notebookIsPartOfSubfolders);

            return notebookIsPartOfSubfolders ? null : notebookLocations;
        }

        /// <summary>
        /// This method is used by GetSubFolders to recursively check for all subfolders.
        /// It sets the notebookIsPartOfSubfolders equal to true if a notebook is found in the subfolders.
        /// </summary>
        /// <param name="rootTreeViewItem">The current TreeViewItem from which all subfolders should be returned</param>
        /// <param name="notebookIsPartOfSubfolders">This is set to true if a notebook is found in the subfolders</param>
        /// <returns>Returns list of all NotebookLocations (subfolders) from rootTreeViewItem</returns>
        private List<NotebookLocation> RecursiveGetSubFolders(TreeViewItem rootTreeViewItem, ref bool notebookIsPartOfSubfolders)
        {
            if (notebookIsPartOfSubfolders)
                return null;

            List<NotebookLocation> notebookLocations = new List<NotebookLocation>();

            if (rootTreeViewItem.Items.Count > 0)
            {
                foreach (TreeViewItem treeViewItem in rootTreeViewItem.Items.Cast<TreeViewItem>())
                {
                    if (treeViewItem.ContextMenu == NotebookContext)
                    {
                        notebookIsPartOfSubfolders = true;
                    }

                    notebookLocations.Add(new NotebookLocation {Path = GetPath(treeViewItem)});

                    List<NotebookLocation> tmpNotebookLocations =
                        RecursiveGetSubFolders(treeViewItem, ref notebookIsPartOfSubfolders);

                    if (tmpNotebookLocations != null)
                        notebookLocations.AddRange(tmpNotebookLocations);
                }
            }
            else
            {
                notebookLocations.Add(new NotebookLocation {Path = GetPath(rootTreeViewItem)});
            }

            if (notebookLocations.Count > 0)
            {
                return notebookLocations;
            }

            return null;
        }

        /// <summary>
        /// Returns all folders as TreeViewItems
        /// </summary>
        /// <returns>Returns a list with TreeViewItems</returns>
        private List<TreeViewItem> GetFolders()
        {
            List<NotebookLocation> notebookLocations = NotebookLocation.Load(LoginUser.Id);
            List<TreeViewItem> treeViewItems = new List<TreeViewItem>();
            foreach (string path in notebookLocations.Select(notebookLocation => notebookLocation.Path))
            {
                TreeViewItem currentNode = null;
                foreach (string directory in path.Split('/'))
                {
                    if (currentNode == null)
                    {
                        if (treeViewItems.Any(treeViewItem => GetHeader(treeViewItem) == directory))
                        {
                            currentNode = treeViewItems.First(treeViewItem => GetHeader(treeViewItem) == directory);
                        }

                        else
                        {
                            treeViewItems.Add(currentNode = CreateTreeNode(directory, FolderContext));
                        }

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

        /// <summary>
        /// Adds the notebooks to the folder structure and returns it
        /// </summary>
        /// <param name="treeViewItems">The folder structure all notebooks should be added to</param>
        /// <returns>Returns a list of TreeViewItems as a representation of the folder and notebook structure</returns>
        private List<TreeViewItem> AddNotebooksToFolderStructure(List<TreeViewItem> treeViewItems)
        {
            LoadNotebooks();
            foreach (Notebook notebook in Notebooks)
            {
                if (!notebook.IsDeleted)
                {

                    TreeViewItem currentNode = null;
                    foreach (string directory in notebook.Path.Path.Split('/'))
                    {
                        if (currentNode == null)
                        {
                            currentNode = treeViewItems.First(treeViewItem => GetHeader(treeViewItem) == directory);
                        }

                        else
                        {
                            currentNode = currentNode.Items.Cast<TreeViewItem>().First(treeViewItem => GetHeader(treeViewItem) == directory);
                        }
                    }

                    currentNode?.Items.Add(CreateTreeNode(notebook.Title, NotebookContext));
                }
            }

            return treeViewItems;
        }

        /// <summary>
        /// Create and return a tree node that has the required fields 
        /// </summary>
        /// <param name="header">Display name</param>
        /// <param name="contextMenu">This is the menu that is shown when right clicked</param>
        /// <param name="notebookId">The id of the notebook (if it is a notebook)</param>
        /// <returns>TreeViewItem that represents a notebook or folder</returns>
        private TreeViewItem CreateTreeNode(string header, ContextMenu contextMenu, int notebookId = -1)
        {
            return new TreeViewItem
            {
                Header = CreateTreeHeader(header, contextMenu, notebookId),
                Foreground = Brushes.White,
                IsExpanded = false,
                FontSize = 14,
                ContextMenu = contextMenu
            };
        }

        /// <summary>
        /// Create a header for a TreeViewItem
        /// </summary>
        /// <param name="header">Display name</param>
        /// <param name="contextMenu">This is the menu that is shown when right clicked</param>
        /// <param name="notebookId">The id of the notebook (if it is a notebook)</param>
        /// <returns>Returns a StackPanel containing all the required data</returns>
        private StackPanel CreateTreeHeader(string header, ContextMenu contextMenu, int notebookId = -1)
        {
            StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal };

            Image image = new Image
            {
                Height = 16,
                Width = 16
            };

            if (contextMenu == FolderContext || contextMenu == RootContext)
            {
                image.Source = FolderImage;
            }
            else if (contextMenu == NotebookContext)
            {
                image.Source = NotebookImage;
            }

            stackPanel.Children.Add(image);

            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.Add(header);
            stackPanel.Children.Add(textBlock);

            if (contextMenu == NotebookContext)
            {
                Label label = new Label
                {
                    Content = notebookId + "",
                    Visibility = Visibility.Collapsed
                };
                stackPanel.Children.Add(label);
            }

            return stackPanel;
        }

        
        public void RemoveFolder(object sender, RoutedEventArgs routedEventArgs)
        {
            if (routedEventArgs.Source is MenuItem menuItem)
            {
                if (menuItem.Parent is ContextMenu contextMenu)
                {
                    TreeViewItem treeViewItem = contextMenu.PlacementTarget as TreeViewItem;
                    string path = GetPath(treeViewItem);
                    
                    NotebookLocation notebookLocation = new NotebookLocation {Path=path};
                    List<NotebookLocation> subLocations = GetSubFolders(notebookLocation);
                    NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();

                    if (subLocations != null)
                    {
                        foreach (NotebookLocation subLocation in subLocations)
                        {
                            subLocation.Delete(notebookLocationRepository);
                        }
                    }

                    notebookLocation.Delete(notebookLocationRepository);
                    LoadNotebooksTreeView();
                }
            }
        }

        /// <summary>
        /// Add a folder to the folder structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
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
                        if (!NotebookLocation.AddNewNotebookLocation(
                            new NotebookLocation() {Path = path + "/" + newFolderName}, LoginUser.Id))
                        {
                            MessageBox.Show("Something happened while adding the folder, does it already exist?", "NoteFever | Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else // TODO fix refresh (for now, delete and add)
                        {
                            LoadNotebooksTreeView();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add a folder to the root
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddFolderToRoot(object sender, RoutedEventArgs e)
        {
            string newFolderName = GetUserInput("Create new folder", "What do you want your new folder to be called:");
            if (newFolderName != null)
            {
                if (!NotebookLocation.AddNewNotebookLocation(
                    new NotebookLocation {Path = newFolderName}, LoginUser.Id))
                {
                    MessageBox.Show("Something happened while adding the folder, does it already exist?", "NoteFever | Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else // TODO fix refresh (for now, delete and add)
                {
                    LoadNotebooksTreeView();
                }
            }
        }

        /// <summary>
        /// Returns a string that represents the path of a TreeViewItem
        /// </summary>
        /// <param name="treeViewItem">The TreeViewItem from which the path should be extracted</param>
        /// <param name="isNotebook">A boolean that indicates if the TreeViewItem is a notebook or not</param>
        /// <returns>Returns the path of a TreeViewItem (folder) or the folder which the notebook is located in</returns>
        private string GetPath(TreeViewItem treeViewItem, bool isNotebook = false)
        {
            if (GetHeader(treeViewItem) == "My Notebooks")
            {
                return "";
            }

            string path = isNotebook ? "" : GetHeader(treeViewItem);
            while (treeViewItem.Parent is TreeViewItem item)
            {
                treeViewItem = item;
                if (GetHeader(treeViewItem) == "My Notebooks")
                {
                    break;
                }

                path = GetHeader(treeViewItem) + "/" + path;
            }
            return isNotebook ? path.Substring(0, path.Length - 1) : path;
        }

        // @joris add summary & comments in code
        private int GetNotebookId(TreeViewItem treeViewItem)
        {
            if (treeViewItem.Header is StackPanel stackPanel)
            {
                if (stackPanel.Children[2] is Label label)
                {
                    return Convert.ToInt32(label.Content);
                }
            }

            return -1;
        }

        private bool IsNotebook(TreeViewItem treeViewItem)
            => treeViewItem.ContextMenu == NotebookContext;

        // @joris add summary & comments in code
        private string GetHeader(TreeViewItem treeViewItem)
        {
            if (treeViewItem.Header is StackPanel stackPanel)
            {
                if (stackPanel.Children[1] is TextBlock textBlock)
                {
                    return textBlock.Text;
                }
            }

            return treeViewItem.Header.ToString();
        }

        #endregion

        #region Treeview context menu, including pop-ups

        /// <summary>
        /// Add a notebook to the folder structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddNotebook(object sender, RoutedEventArgs routedEventArgs)
        {
            if (routedEventArgs.Source is MenuItem menuItem)
            {
                if (menuItem.Parent is ContextMenu contextMenu)
                {
                    string path = GetPath(contextMenu.PlacementTarget as TreeViewItem);
                    string newNotebookName = GetUserInput("Create new notebook", "What do you want your new notebook to be called:");

                    if (newNotebookName != null)
                    {
                        NotebookLocation notebookLocation = NotebookLocation.GetNotebookLocationByPath(path, LoginUser.Id);
                        Notebook notebook = new Notebook() { UserId = LoginUser.Id, LocationId = notebookLocation.Id, Title = newNotebookName, CreationDate = DateTime.Now.Date, LastUpdated = DateTime.Now, Path = notebookLocation };

                        if (!notebook.Save(LoginUser.Id))
                        {
                            MessageBox.Show("Something happened while adding the notebook, does it already exist?", "NoteFever | Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        }
                        else // TODO fix refresh (for now, delete and add)
                        {
                            LoadNotebooksTreeView();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes the notebook from the folder structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveNotebook(object sender, RoutedEventArgs e)
        {
            Notebook notebook = SelectedNotebook;
            NoteRepository noteRepository = new NoteRepository();
            
            //Deleted all the notes in the notebook
            List<INote> notesToRemove = notebook.Notes;

            if (notesToRemove.Count > 0)
            {
                foreach (Note note in notesToRemove)
                {
                    note.IsDeleted = true;
                    noteRepository.Update(note);
                }
                
                notebook.IsDeleted = true;
                notebook.Update();
            }
            else
            {
                notebook.DeletePermanently();
            }

            

            SelectedNotebook = null;
            LoadNoteViewIfNoteExists();

            LoadNotebooksTreeView();
        }

        /// <summary>
        /// Add a note to the currently selected notebook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddNote(object sender, RoutedEventArgs e) =>
            OpenNewNotePopupView();

        /// <summary>
        /// Returns what the user has submitted in the popup window
        /// </summary>
        /// <param name="dialogTitle">The title of the popup window</param>
        /// <param name="dialogValueRequestText">The text above the TextBox (what should the user input)</param>
        /// <param name="minCharacters">The minimum required characters</param>
        /// <param name="maxCharacters">The maximum characters that can be inputted</param>
        /// <returns></returns>
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

        /// <summary>
        /// Handles the submit button from the UserInput popup window
        /// </summary>
        /// <param name="valueRequestViewModel"></param>
        public void HandleSubmit(ValueRequestViewModel valueRequestViewModel) =>
            (valueRequestViewModel.GetView() as Window)?.Close();

        /// <summary>
        /// Handles the cancel button from the UserInput popup window
        /// </summary>
        /// <param name="valueRequestViewModel"></param>
        public void HandleCancel(ValueRequestViewModel valueRequestViewModel)
        {
            valueRequestViewModel.Value = null;
            (valueRequestViewModel.GetView() as Window)?.Close();
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
                        IEnumerable<INote> notes = SelectedNotebook.Notes.Where((note) => !((Note)note).IsDeleted);
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

                // load the selected note and the note elements, afterwards, activate the view.
                NotebookViewModel.NewNoteViewModel?.LoadNote();
                NotebookViewModel.NotebookNotesMenu.LoadNotesIntoNotebookMenu(showDeletedNotes);

                ActivateItem(NotebookViewModel);
            }
            else
            {
                DeactivateItem(NotebookViewModel, true);
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

                // get all notes from all notebooks, where the notes haven't been deleted
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

            LoginUser = loginViewModel.user;
            Constant.User = LoginUser;
            
            if (!suppressSynchronize) 
                Synchronize();
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
            LoadNoteViewIfNoteExists();
        }


        /// <summary>
        /// Select the first notebook inside a path with a certain title
        /// </summary>
        /// <param name="path">The path that should contain the notebook</param>
        /// <param name="title">The title of the notebook that we want to select</param>
        public void SelectNotebook(string path, string title)
        {
            SelectedNotebook = Notebooks.FirstOrDefault(notebook => notebook.Path.Path == path && notebook.Title == title);
            LoadNoteViewIfNoteExists();
        }

        #endregion

        /// <summary>
        /// This synchronizes all the folders, notebooks and notes from the user
        /// </summary>
        public void Synchronize()
        {
            LoadNotebooksTreeView();
            SelectedNotebook = Notebooks.FirstOrDefault(notebooks => notebooks.FsName == SelectedNotebook?.FsName);
            SelectedNote = SelectedNotebook?.Notes.Cast<Note>().FirstOrDefault(notes => notes.Title == SelectedNote?.Title && notes.LastUpdated == SelectedNote?.LastUpdated);
            LoadNoteViewIfNoteExists();
        }

        #region Events

        /// <summary>
        /// This handles the change of a folder or notebook (TreeViewItem)
        /// </summary>
        /// <param name="routedPropertyChangedEventArgs"></param>
        public void TreeViewSelectedItemChanged(RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            if (routedPropertyChangedEventArgs.NewValue is TreeViewItem treeViewItem)
            {
                SelectedTreeViewItem = treeViewItem;

                if (!IsNotebook(treeViewItem))
                {
                    return;
                }

                int notebookId = GetNotebookId(treeViewItem);
                if (notebookId != -1)
                {
                    SelectNotebook(notebookId);
                }

                else
                {
                    SelectNotebook(GetPath(treeViewItem, true), GetHeader(treeViewItem));
                }

            }
        }

        /// <summary>
        /// Whenever we know that there is a note selected,
        /// we want to switch to the note user control, which will display it with all its data.
        /// </summary>
        protected override void OnActivate()
        {
            // Show login popup
            OpenLoginPopupView(true);

            // If user closed login window without logging in or clicking the 'Use locally' button, close application
            if (LoginUser == null)
            {
                Environment.Exit(0);
            }


            // First load context menu's
            RootContext.Items.Add(CreateMenuItem("Add Folder", AddFolderToRoot));
            
            FolderContext.Items.Add(CreateMenuItem("Add Folder", AddFolder));
            FolderContext.Items.Add(CreateMenuItem("Add Notebook", AddNotebook));
            FolderContext.Items.Add(CreateMenuItem("Remove", RemoveFolder));
            
            NotebookContext.Items.Add(CreateMenuItem("Add Note", AddNote));
            NotebookContext.Items.Add(CreateMenuItem("Remove", RemoveNotebook));

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