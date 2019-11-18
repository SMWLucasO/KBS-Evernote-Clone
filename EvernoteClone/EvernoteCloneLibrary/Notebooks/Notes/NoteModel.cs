using EvernoteCloneLibrary.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebooks.Notes
{
    /// <summary>
    /// The class(OOP) representation of the 'Note' table.
    /// </summary>
    public class NoteModel : IModel
    {
        public int Id { get; set; } = -1;
        public int NotebookID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdated { get; set; }

    }
}
