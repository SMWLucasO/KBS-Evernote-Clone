using Caliburn.Micro;
using EvernoteCloneGUI.ViewModels.Popups;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Location;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EvernoteCloneGUI.ViewModels.Controls
{
    public class NoteFeverTreeViewModel
    {
        #region Properties

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
        
        /// <value>
        /// SelectedTreeViewItem contains the currently selected tree view item
        /// </value>
        public TreeViewItem SelectedTreeViewItem;
        
        /// <value>
        /// _noteFeverViewModel contains the (only) instance of NoteFeverViewModel.
        /// </value>
        private static NoteFeverViewModel _noteFeverViewModel;
        
        #endregion

        #region Constructor

        /// <summary>
        /// This is the constructor of NoteFeverTreeViewModel. This class needs an instance of NoteFeverViewModel to function.
        /// In the constructor method it builds all the necessary context menu's and Loads the TreeView.
        /// </summary>
        /// <param name="noteFeverViewModel"></param>
        public NoteFeverTreeViewModel(NoteFeverViewModel noteFeverViewModel)
        {
            _noteFeverViewModel = noteFeverViewModel;
            
            // First create/build context menu's
            RootContext.Items.Add(CreateMenuItem(Properties.Settings.Default.NoteFeverTreeViewModelAddFolder, AddFolderToRoot));
            
            FolderContext.Items.Add(CreateMenuItem(Properties.Settings.Default.NoteFeverTreeViewModelAddFolder, AddFolder));
            FolderContext.Items.Add(CreateMenuItem(Properties.Settings.Default.NoteFeverTreeViewModelAddNotebook, AddNotebook));
            FolderContext.Items.Add(CreateMenuItem(Properties.Settings.Default.NoteFeverTreeViewModelRemove, RemoveFolder));
            
            NotebookContext.Items.Add(CreateMenuItem(Properties.Settings.Default.NoteFeverTreeViewModelAddNote, AddNote));
            NotebookContext.Items.Add(CreateMenuItem(Properties.Settings.Default.NoteFeverTreeViewModelRemove, RemoveNotebook));

            // Load all folders, notebooks and add them all to the view
            LoadNotebooksTreeView();
        }

        #endregion

        #region TreeView Builder

        /// <summary>
        /// (Re)loads the folder structure and notebooks
        /// </summary>
        /// <param name="pathToBeSelected"></param>
        /// <param name="withoutSynchronize"></param>
        public void LoadNotebooksTreeView(string pathToBeSelected = null, bool withoutSynchronize = false)
        {
            // Create a root TreeViewItem
            TreeViewItem rootTreeViewItem = CreateTreeNode(Properties.Settings.Default.NoteFeverTreeViewModelMyNotebooks, RootContext, null);

            // Load all Folders (LoadFolders) and attach Notebooks to them (LoadNotebooksIntoFolderStructure)
            // Now, loop over them all, and add them to the root TreeViewItem
            foreach (TreeViewItem treeViewItem in AddNotebooksToFolders(GetFolders(withoutSynchronize), withoutSynchronize))
            {
                rootTreeViewItem.Items.Add(treeViewItem);
            }

            // Select the currently selected folder again in the new TreeView
            if (SelectedTreeViewItem != null)
            {
                if (pathToBeSelected == null)
                {
                    SelectPath(ref rootTreeViewItem,
                        GetPath(SelectedTreeViewItem, IsNotebook(SelectedTreeViewItem))?.Path);
                }
                else
                {
                    SelectPath(ref rootTreeViewItem,
                        pathToBeSelected);
                }
            }

            // Clear the NotebooksTreeView and add the folder and notebook structure
            NoteFeverViewModel.NotebooksTreeView.Clear();
            NoteFeverViewModel.NotebooksTreeView.Add(rootTreeViewItem);
        }

        #endregion

        #region TreeView Helpers
        
        /// <summary>
        /// Returns a NotebookLocation that represents the path of a TreeViewItem
        /// </summary>
        /// <param name="treeViewItem">The TreeViewItem from which the path should be extracted</param>
        /// <param name="isNotebook">A boolean that indicates if the TreeViewItem is a notebook or not</param>
        /// <returns>Returns a NotebookLocation object containing the folder path (or path to the notebook, if it is a notebook)</returns>
        private static NotebookLocation GetPath(HeaderedItemsControl treeViewItem, bool isNotebook = false)
        {
            // Check if header of treeViewItem is 'My Notebooks', if this is true, it is the root TreeViewItem and thus we should return null.
            if (treeViewItem.Header.ToString() == Properties.Settings.Default.NoteFeverTreeViewModelMyNotebooks)
            {
                return null;
            }

            if (!isNotebook)
            {
                return (NotebookLocation) treeViewItem.Tag;
            }
            
            // If isNotebook equals true, we should get the parent (which should be a folder) and return it's Tag (NotebookLocation)
            if (treeViewItem.Parent is TreeViewItem folder)
            {
                return (NotebookLocation) folder.Tag;
            }

            return (NotebookLocation)treeViewItem.Tag;
        }

        /// <summary>
        /// Returns the id of the notebook selected.
        /// </summary>
        /// <param name="treeViewItem">The selected 'notebook'</param>
        /// <returns>Notebook Id</returns>
        private int GetNotebookId(TreeViewItem treeViewItem) =>
            ((Notebook)treeViewItem.Tag).Id;
        
        /// <summary>
        /// Returns the header of a folder or notebook that is selected
        /// </summary>
        /// <param name="treeViewItem">The selected item</param>
        /// <returns>The header as a string</returns>
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

        /// <summary>
        /// Set the IsExpanded property of every TreeViewItem that is included in the given path
        /// </summary>
        /// <param name="rootTreeViewItem">The root TreeViewItem</param>
        /// <param name="path">Path that should be expanded</param>
        private void SelectPath(ref TreeViewItem rootTreeViewItem, string path)
        {
            rootTreeViewItem.IsExpanded = true;
            TreeViewItem currentNode = null;

            if (path == null)
            {
                return;
            }

            // Foreach folder in path
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

            if (currentNode != null)
            {
                currentNode.IsExpanded = true;
            }
        }
        
        /// <summary>
        /// Checks if the selected item is a Notebook
        /// </summary>
        /// <param name="treeViewItem">The selected item</param>
        /// <returns>Returns a boolean indicating if the selected item is a Notebook or not</returns>
        private bool IsNotebook(TreeViewItem treeViewItem)
            => treeViewItem.ContextMenu == NotebookContext;
        
        #region Get SubFolders
        
        /// <summary>
        /// Returns a list of NotebookLocations that are all subfolders of notebookLocation
        /// </summary>
        /// <param name="notebookLocation">Notebook Location from which all subfolders should be returned</param>
        /// <returns>Returns null if a notebook is part of the subfolders, else returns a list with all subfolders</returns>
        private List<NotebookLocation> GetSubFolders(NotebookLocation notebookLocation)
        {
            TreeViewItem currentNode = null;

            // Foreach folder in path
            foreach (string directory in notebookLocation.Path.Split('/'))
            {
                if (currentNode == null)
                {
                    if (NoteFeverViewModel.NotebooksTreeView[0].Items.Cast<TreeViewItem>().Any(treeViewItem => GetHeader(treeViewItem) == directory))
                    {
                        currentNode = NoteFeverViewModel.NotebooksTreeView[0].Items.Cast<TreeViewItem>().First(treeViewItem => GetHeader(treeViewItem) == directory);
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

            // This boolean is used to check if there are notebooks in the (sub)folders.
            // If this is the case the folder shouldn't be deleted.
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
            {
                return null;
            }
                

            List<NotebookLocation> notebookLocations = new List<NotebookLocation>();

            if (rootTreeViewItem.Items.Count > 0)
            {
                foreach (TreeViewItem treeViewItem in rootTreeViewItem.Items.Cast<TreeViewItem>())
                {
                    if (treeViewItem.ContextMenu == NotebookContext)
                    {
                        notebookIsPartOfSubfolders = true;
                    }

                    notebookLocations.Add((NotebookLocation)treeViewItem.Tag);

                    List<NotebookLocation> tmpNotebookLocations =
                        RecursiveGetSubFolders(treeViewItem, ref notebookIsPartOfSubfolders);

                    if (tmpNotebookLocations != null)
                    {
                        notebookLocations.AddRange(tmpNotebookLocations);
                    }
                        
                }
            }
            else
            {
                notebookLocations.Add((NotebookLocation)rootTreeViewItem.Tag);
            }

            if (notebookLocations.Count > 0)
            {
                return notebookLocations;
            }

            return null;
        }
        
        #endregion
        
        #endregion
        
        #region TreeViewItem Helpers
        
        /// <summary>
        /// Create and return a tree node that has the required fields 
        /// </summary>
        /// <param name="header">Display name</param>
        /// <param name="contextMenu">This is the menu that is shown when right clicked</param>
        /// <param name="tag">An object that should be either a Notebook or a NotebookLocation</param>
        /// <returns>TreeViewItem that represents a notebook or folder</returns>
        private TreeViewItem CreateTreeNode(string header, ContextMenu contextMenu, object tag)
        {
            return new TreeViewItem
            {
                Header = CreateTreeHeader(header, contextMenu),
                Tag = tag,
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
        /// <returns>Returns a StackPanel containing all the required data</returns>
        private StackPanel CreateTreeHeader(string header, ContextMenu contextMenu)
        {
            // This StackPanel contains the image and header
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

            return stackPanel;
        }
        
        #endregion
        
        #region TreeView(item) Builder
        
        /// <summary>
        /// Returns all folders as TreeViewItems
        /// </summary>
        /// <param name="withoutSynchronize">This boolean indicates whether everything should be synchronized or not</param>
        /// <returns>Returns a list with TreeViewItems</returns>
        private List<TreeViewItem> GetFolders(bool withoutSynchronize = false)
        {
            List<NotebookLocation> notebookLocations = NotebookLocation.Load(withoutSynchronize);
            List<TreeViewItem> treeViewItems = new List<TreeViewItem>();
            
            // For all NotebookLocations create a treeViewItem
            foreach (NotebookLocation notebookLocation in notebookLocations)
            {
                TreeViewItem currentNode = null;
                foreach (string directory in notebookLocation.Path.Split('/'))
                {
                    if (currentNode == null)
                    {
                        if (treeViewItems.Any(treeViewItem => GetHeader(treeViewItem) == directory))
                        {
                            currentNode = treeViewItems.First(treeViewItem => GetHeader(treeViewItem) == directory);
                        }

                        else
                        {
                            treeViewItems.Add(currentNode = CreateTreeNode(directory, FolderContext, notebookLocation));
                        }

                    }
                    else if (currentNode.Items.Cast<TreeViewItem>().Any(treeViewItem => GetHeader(treeViewItem) == directory))
                    {
                        currentNode = currentNode.Items.Cast<TreeViewItem>().First(treeViewItem => GetHeader(treeViewItem) == directory);
                    }
                    else
                    {
                        var newNode = CreateTreeNode(directory, FolderContext, notebookLocation);
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
        /// <param name="withoutSynchronize">This boolean indicates whether everything should be synchronized or not</param>
        /// <returns>Returns a list of TreeViewItems as a representation of the folder and notebook structure</returns>
        private List<TreeViewItem> AddNotebooksToFolders(List<TreeViewItem> treeViewItems, bool withoutSynchronize = false)
        {
            _noteFeverViewModel.LoadNotebooks(false, withoutSynchronize);
            
            // For all Notebooks add them to the corresponding path (folder)
            foreach (Notebook notebook in _noteFeverViewModel.Notebooks)
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

                    currentNode?.Items.Add(CreateTreeNode(notebook.Title, NotebookContext, notebook));
                }
            }

            return treeViewItems;
        }
        
        #endregion
        
        #region TreeView ContextMenu's Handlers
        
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
                    NotebookLocation notebookLocation = GetPath(contextMenu.PlacementTarget as TreeViewItem);

                    if (notebookLocation.Path.Split('/').Length >= 10)
                    {
                        MessageBox.Show(Properties.Settings.Default.NoteFeverTreeViewModelNoMoreThan, Properties.Settings.Default.MessageBoxTitleError, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    
                    string newFolderName = GetUserInput(Properties.Settings.Default.NoteFeverTreeViewModelNewFolderTitle, Properties.Settings.Default.NoteFeverTreeViewModelNewFolder);

                    if (newFolderName != null)
                    {
                        if (!NotebookLocation.AddNewNotebookLocation(
                            new NotebookLocation {Path = notebookLocation.Path + "/" + newFolderName }))
                        {
                            MessageBox.Show(Properties.Settings.Default.NoteFeverTreeViewModelErrorWhileNewFolder, Properties.Settings.Default.MessageBoxTitleError, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            LoadNotebooksTreeView(notebookLocation.Path + "/" + newFolderName, true);
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
            string newFolderName = GetUserInput(Properties.Settings.Default.NoteFeverTreeViewModelNewFolderTitle, Properties.Settings.Default.NoteFeverTreeViewModelNewFolder);
            
            if (newFolderName != null)
            {
                if (!NotebookLocation.AddNewNotebookLocation(
                    new NotebookLocation {Path = newFolderName}))
                {
                    MessageBox.Show(Properties.Settings.Default.NoteFeverTreeViewModelErrorWhileNewFolder, Properties.Settings.Default.MessageBoxTitleError, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    LoadNotebooksTreeView(null, true);
                }
            }
        }

        /// <summary>
        /// Add a notebook to the folder structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        public void AddNotebook(object sender, RoutedEventArgs routedEventArgs)
        {
            if (routedEventArgs.Source is MenuItem menuItem)
            {
                if (menuItem.Parent is ContextMenu contextMenu)
                {
                    NotebookLocation notebookLocation = GetPath(contextMenu.PlacementTarget as TreeViewItem);
                    string newNotebookName = GetUserInput(Properties.Settings.Default.NoteFeverTreeViewModelNewNotebookTitle, Properties.Settings.Default.NoteFeverTreeViewModelNewNotebook);

                    if (newNotebookName != null)
                    {
                        Notebook notebook = new Notebook
                        {
                            UserId = NoteFeverViewModel.LoginUser.Id, 
                            LocationId = notebookLocation.Id, 
                            Title = newNotebookName, 
                            CreationDate = DateTime.Now.Date, 
                            LastUpdated = DateTime.Now, 
                            Path = notebookLocation
                        };

                        if (!notebook.Save())
                        {
                            MessageBox.Show(Properties.Settings.Default.NoteFeverTreeViewModelErrorWhileNewNotebook, Properties.Settings.Default.MessageBoxTitleError, MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        else
                        {
                            LoadNotebooksTreeView(null, false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add a note to the currently selected notebook
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddNote(object sender, RoutedEventArgs e) =>
            _noteFeverViewModel.OpenNewNotePopupView();

        /// <summary>
        /// Removes the notebook from the folder structure
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RemoveNotebook(object sender, RoutedEventArgs e)
        {
            Notebook notebook = _noteFeverViewModel.SelectedNotebook;
            NoteRepository noteRepository = new NoteRepository();
            
            // Deleted all the notes in the notebook
            List<INote> notesToRemove = notebook.Notes;

            if (notesToRemove.Count > 0)
            {
                foreach (Note note in notesToRemove.Cast<Note>())
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

            _noteFeverViewModel.SelectedNotebook = null;
            _noteFeverViewModel.LoadNoteViewIfNoteExists();

            LoadNotebooksTreeView();
        }
        
        /// <summary>
        /// Remove the selected folder
        /// </summary>
        /// <param name="sender">The object that fired the event</param>
        /// <param name="routedEventArgs">Containing information about the fired event</param>
        public void RemoveFolder(object sender, RoutedEventArgs routedEventArgs)
        {
            if (routedEventArgs.Source is MenuItem menuItem)
            {
                if (menuItem.Parent is ContextMenu contextMenu)
                {
                    TreeViewItem treeViewItem = contextMenu.PlacementTarget as TreeViewItem;
                    NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
                    
                    NotebookLocation notebookLocation = GetPath(treeViewItem);
                    List<NotebookLocation> subLocations = GetSubFolders(notebookLocation);

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
        
        #endregion
        
        #region ContextMenu Helpers
        
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
        
        #endregion
        
        #region Get user input
        
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
                while (valueRequestViewModel.Value.ToCharArray().Contains('/') ||
                       (valueRequestViewModel.Value = valueRequestViewModel.Value.Trim()).Length < minCharacters
                       && valueRequestViewModel.Value.Length > maxCharacters)
                {
                    if (MessageBox.Show(
                            string.Format(Properties.Settings.Default.NoteFeverTreeViewModelTextBetween, minCharacters, maxCharacters),
                            Properties.Settings.Default.MessageBoxTitleError, 
                            MessageBoxButton.OKCancel, 
                            MessageBoxImage.Error) 
                        == MessageBoxResult.Cancel)
                    {
                        break;
                    }
                    
                    windowManager.ShowDialog(valueRequestViewModel);
                    if (valueRequestViewModel.Value != null)
                    {
                        break;
                    }
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
                    _noteFeverViewModel.SelectNotebook(notebookId);
                }
                else
                {
                    _noteFeverViewModel.SelectNotebook(GetPath(treeViewItem, true), GetHeader(treeViewItem));
                }
            }
        }
        
        #endregion
    }
}