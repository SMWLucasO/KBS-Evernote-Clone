using System;
using System.Windows.Input;

namespace EvernoteCloneGUI.ViewModels.Commands.KeyGestures
{
    /// <summary>
    /// Command for inserting a table (alt + t)
    /// </summary>
    public class InsertTableCommand : ICommand
    {
        public NewNoteViewModel NewNoteViewModel { get; set; }
        
        public event EventHandler CanExecuteChanged;
        
        public bool CanExecute(object parameter)
            => true;
        

        /// <summary>
        /// Method which is called when the key gesture is executed, inserts a table (if gone through with)
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            NewNoteViewModel.OnInsertTable();
        }

    }
}