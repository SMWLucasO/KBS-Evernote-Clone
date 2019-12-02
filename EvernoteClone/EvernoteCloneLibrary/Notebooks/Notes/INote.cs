using EvernoteCloneLibrary.Files.Parsers;

namespace EvernoteCloneLibrary.Notebooks.Notes
{
    /// <summary>
    /// An interface in case of a future implementation of different types of notes.
    /// </summary>
    public interface INote : IParseable
    {
        Notebook NoteOwner { get; set; }
    }
}
