using EvernoteCloneLibrary.Database;
using System;

namespace EvernoteCloneLibrary.Notebooks.Notes
{
    /// <summary>
    /// The class(OOP) representation of the 'Note' table.
    /// </summary>
    public class NoteModel : IModel
    {
        public int Id { get; set; } = -1;
        public int NotebookId { get; set; } = -1;
        public virtual string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string Author { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; }

    }
}
