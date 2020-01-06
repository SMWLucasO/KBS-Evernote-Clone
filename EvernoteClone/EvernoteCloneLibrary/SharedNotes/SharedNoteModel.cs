using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.SharedNotes
{
    public class SharedNoteModel : IModel
    {
        public int UserId { get; set; }
        public int NoteId { get; set; }
    }
}
