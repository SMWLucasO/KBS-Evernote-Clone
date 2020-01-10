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

        /// <value>
        /// The note which will be moved to the new notebook if everything goes through.
        /// </value>
        public Note PotentialMoveCandidate { get; set; }

        #region  databound properties

        /// <value>
        /// The notebooks which can be chosen within the viewmodel
        /// </value>
        private List<Notebook> _notebooks;

        /// <value>
        /// Gets and sets the notebooks which can be chosen within the viewmodel.
        /// Calls the NotifyOfPropertyChange event when set to a new value, to update the ComboBox which it is bound to.
        /// </value>
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

        /// <value>
        /// The currently selected notebook within the NotebookPicker.
        /// </value>
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
                    MessageBox.Show(string.Format(Properties.Settings.Default.NotebookPickerViewModelMoved, PotentialMoveCandidate.Title, SelectedNotebook.Path.Path, SelectedNotebook.Title,
                        Properties.Settings.Default.NotebookPickerViewModelTitle, MessageBoxButton.OK, MessageBoxImage.Information));
                    TryClose(true);
                }
                else
                {
                    MessageBox.Show(Properties.Settings.Default.NotebookPickerViewModelNotMoved,
                        Properties.Settings.Default.NotebookPickerViewModelTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
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
                Notebook notePreviousNotebook = PotentialMoveCandidate.NoteOwner;

                PotentialMoveCandidate.NoteOwner.Notes.Remove(PotentialMoveCandidate);
                PotentialMoveCandidate.NoteOwner = SelectedNotebook;
                
                SelectedNotebook.Notes.Add(PotentialMoveCandidate);

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
