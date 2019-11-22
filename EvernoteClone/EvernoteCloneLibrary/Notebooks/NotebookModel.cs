using EvernoteCloneLibrary.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebooks
{
    /// <summary>
    /// The class representation of the 'Notebook' table.
    /// </summary>
    public class NotebookModel : IModel
    {
        public int Id { get; set; } = -1;
        public int UserID { get; set; }
        public int LocationID { get; set; }
        public virtual string Title { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
