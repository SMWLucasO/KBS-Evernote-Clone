using System;
using System.Windows.Input;

namespace EvernoteCloneGUI.ViewModels.Commands.KeyGestures
{
    
    /// <summary>
    /// Command for changing the text color (alt + c)
    /// </summary>
    public class TextColorCommand : ICommand
    {
        public NewNoteViewModel NewNoteViewModel { get; set; }
        
        public event EventHandler CanExecuteChanged;
        
        public bool CanExecute(object parameter)
            => true;

        /// <summary>
        /// Method which is called when the key gesture is executed, sets the text color. (if gone through with)
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            NewNoteViewModel.OnSetTextColor();
        }

    }
}