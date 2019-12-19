using Caliburn.Micro;
using EvernoteCloneLibrary.Notebooks.Notes;
using EvernoteCloneLibrary.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using EvernoteCloneGUI.ViewModels.Controls;
using Microsoft.VisualBasic;
using EvernoteCloneLibrary.Users;
using System.Collections.Generic;
using EvernoteCloneLibrary.SharedNotes;

namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// ViewModel which handles all interaction with the note elements in the NotebookNotesMenuView
    /// </summary>
    public class NoteElementViewModel : PropertyChangedBase
    {
        private string _title;
        public User _user { get; private set; }

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }
        public string NoteCreationDate { get; set; }

        public Note Note { get; set; }

        public NoteFeverViewModel Container { get; set; }

        public void LoadOnClick(EventArgs clickedEventArgs)
        {
            if (Container?.SelectedNote != null)
            {
                // When we click on the NoteElementView, it gets called from the class which was clicked
                // thus we need to get the currently selected note (so, the one which is currently being displayed
                // before switching) and check if it was modified, and if so, notify that changes may be lost.
                Note currentlySelectedNote = Container.SelectedNote;
                if (!(currentlySelectedNote.Content.Equals(currentlySelectedNote.NewContent)))
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure? Unsaved changes will be lost.", "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.Yes)
                    {
                        // Reset contents and switch note views.
                        currentlySelectedNote.NewContent = currentlySelectedNote.Content;
                        SwitchNoteView();
                    }
                }
                else
                {
                    SwitchNoteView();
                }

            }
            else
            {
                SwitchNoteView();
            }
        }

        /// <summary>
        /// Switches to the editor view of the note object specified by this object
        /// </summary>
        private void SwitchNoteView()
        {
            if (Container != null)
            {
                Container.SelectedNote = Note;

                // validate whether we should show deleted notes
                bool showDeletedNotes = false;
                if (ValidationUtil.AreNotNull(Container.NotebookViewModelProp, Container.NotebookViewModelProp.NotebookNotesMenu))
                {
                    showDeletedNotes = Container.NotebookViewModelProp.NotebookNotesMenu.ShowDeletedNotes;
                }

                // load the note into view.
                Container.LoadNoteViewIfNoteExists(showDeletedNotes);
            }

        }

        /// <summary>
        /// Generate and load the ContextMenu for the note element.
        /// </summary>
        /// <param name="args"></param>
        public void LoadNoteContext(RoutedEventArgs args)
        {
            // TODO redo this
            if (args.Source is ContextMenu menu && Note != null)
            {
                // Load the appropriate context menu
                // If this note is a deleted note, we want a context menu to delete it permanently or restore it, otherwise we only want a 'remove'
                // context menu element
                if (Note.IsDeleted && menu.Items.Count != 2 && Container.NotebookViewModelProp.NotebookNotesMenu.ShowDeletedNotes)
                {
                    // Clear the previous items, because it might already have previous ones.
                    menu.Items.Clear();
                    MenuItem restoreNoteMenuItem = new MenuItem
                    {
                        Header = "Restore"
                    };

                    // Set the 'IsDeleted' flag to false, same goes for the notebook if this flag was set to true.
                    restoreNoteMenuItem.Click += (sender, arg) =>
                    {
                        // update the note and note's state.
                        UpdateNoteDeletion(false);
                        if (Container.SelectedNote.Equals(Note))
                        {
                            Container.SelectedNote = null;
                        }

                        // Refresh the view
                        Container.OpenDeletedNotesView();
                    };

                    MenuItem permanentNoteDeletionMenuItem = new MenuItem
                    {
                        Header = "Delete permanently"
                    };

                    // Register 'delete permanently' click event, which deletes the note perm, and the notebook too if it is 
                    // the last note inside of said notebook.
                    permanentNoteDeletionMenuItem.Click += (sender, arg) =>
                    {
                        if (MessageBox.Show("Are you sure that you want to permanently delete this note?", "Note Fever | Warning", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            // If the note we are deleting is the currently selected note, we delete it.
                            if (Container.SelectedNote.Equals(Note))
                            {
                                Container.SelectedNote = null;
                            }

                            // Delete the note entirely (from database & local)
                            Note.DeletePermanently();

                            if (Note.NoteOwner.Notes.Contains(Note))
                            {
                                
                                if (Note.NoteOwner.IsDeleted)
                                {
                                    // Honestly should not be possible, but you never know.
                                    if (Container.SelectedNotebook.Equals(Note.NoteOwner))
                                    {
                                        Container.SelectedNotebook = null;
                                    }

                                    if (Note.NoteOwner.Notes.Count == 1)
                                    {
                                        // Remove notebook from the synchronizable notebook list
                                        Container.Notebooks.Remove(Note.NoteOwner);

                                        // Delete notebook from local storage and database
                                        Note.NoteOwner.DeletePermanently();

                                        // remove it from the noteowner
                                        Note.NoteOwner.Notes.Remove(Note);

                                    }
                                }

                                Note.NoteOwner.Notes.Remove(Note);
                            }

                            // Refresh the view
                            Container.OpenDeletedNotesView();
                        }
                        
                    };

                    menu.Items.Add(restoreNoteMenuItem);
                    menu.Items.Add(permanentNoteDeletionMenuItem);

                }
                else if (!Note.IsDeleted && menu.Items.Count != 1 && !Container.NotebookViewModelProp.NotebookNotesMenu.ShowDeletedNotes)
                {
                    menu.Items.Clear();
                    MenuItem removeNoteMenuItem = new MenuItem()
                    {
                        Header = "Remove"
                    };

                    removeNoteMenuItem.Click += (sender, arg) =>
                    {
                        // Set IsDeleted to true and update this in the database.
                        UpdateNoteDeletion(true);

                        // reload the notebook with the new notes
                        Container.NotebookViewModelProp.NotebookNotesMenu.LoadNotesIntoNotebookMenu();

                        // Remove note from text editor if that is the one we're removing.
                        if (Container.SelectedNote != null && Container.SelectedNote.Equals(Note))
                        {
                            Container.SelectedNote = null;
                        }

                        // Load...
                        // We know for sure that we do not want to load deleted notes, thus we can just insert false.
                        Container.LoadNoteViewIfNoteExists(false);
                    };

                    menu.Items.Add(removeNoteMenuItem);
                }

                // Makes new menu item share.
                MenuItem shareNote = new MenuItem
                {
                    Header = "Share Note"
                };

                shareNote.Click += ShareNote;
                menu.Items.Add(shareNote);
            }
        }

        /// <summary>
        /// Makes a new note inserts them into list of notes of shared user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        public void ShareNote(object sender, RoutedEventArgs arg)
        {
            UserRepository userRepositoryLogin = new UserRepository();
            Note sharedNote = Note;
            string userInput = "";
            sharedNote.Id = -1;
            sharedNote.NotebookId = -1;

            // Checks in field that is insert is not empty.
            userInput = Interaction.InputBox("Share Note", "Please enter a valid username", userInput);
            if (string.IsNullOrEmpty(userInput))
            {
                MessageBox.Show("Field cannot be empty");
                return;
            }
            

            // If field has been filled, it will check if it exist in database of users.
            // If the user exist it will add the note to the new user. 
            User sharedUser = (User)userRepositoryLogin.CheckIfUserExists(userInput);
            if (sharedUser != null)
            {
                
                new NoteRepository().Insert(sharedNote);
                SharedNote.SaveNewRecord(sharedNote.Id, sharedUser.Id);
            }
            else
            {
                MessageBox.Show("Username does not exist");
            }
        }

        /// <summary>
        /// Update the note and notebook deletion status.
        /// If we are restoring a note and the notebook was deleted, we restore the notebook too.
        /// </summary>
        /// <param name="deleted"></param>
        public void UpdateNoteDeletion(bool deleted)
        {
            if (Note.NoteOwner != null && !(Note.NoteOwner.IsNotNoteOwner))
            {
                Note.IsDeleted = deleted;
                if (!deleted && Note.NoteOwner.IsDeleted)
                {
                    Note.NoteOwner.IsDeleted = false;
                }


                Note.NoteOwner.Save();
                
                // reload notebook treeview
                NoteFeverViewModel.NoteFeverTreeViewModel.LoadNotebooksTreeView();
            }
        }
    }
}
