using Caliburn.Micro;
using EvernoteCloneLibrary.Notebooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Notebooks.Notes;
using EvernoteCloneLibrary.SharedNotes;

namespace EvernoteCloneGUI.ViewModels.Popups
{
    /// <summary>
    /// Class for handling interaction with the NotebookPickerView
    /// </summary>
    public class NotebookPickerViewModel : Screen
    {

        public Note PotentialMoveCandidate { get; set; }

        #region  databound properties

        private List<Notebook> _notebooks;

        public List<Notebook> Notebooks
        {
            get
            {
                return _notebooks;
            }
            set
            {

                _notebooks = value;
                NotifyOfPropertyChange(() => Notebooks);
            }
        }

        public Notebook SelectedNotebook { get; set; }
        
        
        #endregion
        
        public NotebookPickerViewModel()
        {
            _notebooks = new List<Notebook>();
        }
        
        #region Events
        
        /// <summary>
        /// Handler for the 'Submit' button in the NotebookPickerView
        /// </summary>
        public void OnSubmit()
        {
            if (SelectedNotebook != null && !(SelectedNotebook.IsDeleted  || SelectedNotebook.IsNotNoteOwner))
            {
                if (UpdateNoteNotebook())
                {
                    MessageBox.Show($"The note {PotentialMoveCandidate.Title} has been moved to the" +
                                    $" notebook {SelectedNotebook.Path.Path}/{SelectedNotebook.Title}",
                        "Note Fever | Notice", MessageBoxButton.OK, MessageBoxImage.Information);
                    TryClose(true);
                }
                else
                {
                    MessageBox.Show("Failed to move the note to the specified notebook.",
                        "Note Fever | Notice", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            
            
            
            TryClose(false);
        }
        
        /// <summary>
        /// Handler for the 'Cancel' button in the NotebookPickerView
        /// </summary>
        public void OnCancel()
        {
            TryClose(false);
        }
        
        #endregion

        /// <summary>
        /// Update the notebooks, then save them.
        /// </summary>
        /// <returns></returns>
        private bool UpdateNoteNotebook()
        {
            PotentialMoveCandidate.NotebookId = SelectedNotebook.Id;

            if (!PotentialMoveCandidate.NoteOwner.IsSharedNotebook)
            {
                SelectedNotebook.Notes.Add(PotentialMoveCandidate);

                Notebook notePreviousNotebook = PotentialMoveCandidate.NoteOwner;
                PotentialMoveCandidate.NoteOwner = SelectedNotebook;

                return SelectedNotebook.Save() && notePreviousNotebook.Save();
            }

            SharedNote.RemoveRecord(PotentialMoveCandidate.Id, Constant.User.Id);
            
            PotentialMoveCandidate.NoteOwner.Notes.Remove(PotentialMoveCandidate);
            SelectedNotebook.Notes.Add(PotentialMoveCandidate);
            PotentialMoveCandidate.NoteOwner = SelectedNotebook;

            return SelectedNotebook.Save();
        }
    }
}
