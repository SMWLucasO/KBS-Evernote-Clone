using System;
using System.Windows.Input;

namespace EvernoteCloneGUI.ViewModels.Commands.KeyGestures
{
    
    /// <summary>
    /// Command for the saving of the note (ctrl + s)
    /// </summary>
    public class SaveCommand : ICommand
    {
        public NewNoteViewModel NewNoteViewModel { get; set; }
        
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
            => true;

        /// <summary>
        /// Method which is called when the key gesture is executed, saves the note.
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            NewNoteViewModel.NotifyUserOfSave();
        }
    }
}