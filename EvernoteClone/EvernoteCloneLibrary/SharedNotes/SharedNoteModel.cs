using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.SharedNotes
{
    /// <summary>
    /// The class representation of the 'SharedNote' table.
    /// </summary>
    public class SharedNoteModel : IModel
    {
        public int UserId { get; set; }
        public int NoteId { get; set; }
    }
}
