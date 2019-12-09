using Caliburn.Micro;
using EvernoteCloneLibrary.Notebooks.Notes;
using EvernoteCloneLibrary.Utils;
using System;
using System.Windows;
using System.Windows.Controls;

namespace EvernoteCloneGUI.ViewModels
{
    /// <summary>
    /// ViewModel which handles all interaction with the note elements in the NotebookNotesMenuView
    /// </summary>
    public class NoteElementViewModel : PropertyChangedBase
    {
        private string _title;
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
                if (ValidationUtil.AreNotNull(Container.NotebookViewModel, Container.NotebookViewModel.NotebookNotesMenu))
                {
                    showDeletedNotes = Container.NotebookViewModel.NotebookNotesMenu.ShowDeletedNotes;
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
            if (args.Source is ContextMenu menu && Note != null)
            {
                // Load the appropriate context menu
                // If this note is a deleted note, we want a context menu to delete it permanently or restore it, otherwise we only want a 'remove'
                // context menu element
                if (Note.IsDeleted && menu.Items.Count != 2 && Container.NotebookViewModel.NotebookNotesMenu.ShowDeletedNotes)
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

                        // If the note we are deleting is the currently selected note, we delete it.
                        if (Container.SelectedNote.Equals(Note))
                        {
                            Container.SelectedNote = null;
                        }

                        // Delete the note entirely (from database & local)
                        Note.DeletePermanently();

                        // just in case
                        if (Note.NoteOwner.Notes.Contains(Note))
                        {
                            // TODO delete notebook perm if the notebook IsDeleted is also true & this was the last note of it.
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

                                }
                            }

                        }

                        // Refresh the view
                        Container.OpenDeletedNotesView();
                    };

                    menu.Items.Add(restoreNoteMenuItem);
                    menu.Items.Add(permanentNoteDeletionMenuItem);

                }
                else if (!Note.IsDeleted && menu.Items.Count != 1 && !Container.NotebookViewModel.NotebookNotesMenu.ShowDeletedNotes)
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
                        Container.NotebookViewModel.NotebookNotesMenu.LoadNotesIntoNotebookMenu();

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
                Container.LoadNotebooksTreeView();

            }
        }
    }
}
