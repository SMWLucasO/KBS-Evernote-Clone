using EvernoteCloneLibrary.Database;
using System;

namespace EvernoteCloneLibrary.Notebooks
{
    /// <summary>
    /// The class representation of the 'Notebook' table.
    /// </summary>
    public class NotebookModel : IModel
    {
        public int Id { get; set; } = -1;
        public int UserId { get; set; }
        public int LocationId { get; set; } = -1;
        public virtual string Title { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsDeleted { get; set; }
    }
}
