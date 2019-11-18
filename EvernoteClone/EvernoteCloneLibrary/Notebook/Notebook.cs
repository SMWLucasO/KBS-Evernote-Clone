using EvernoteCloneLibrary.Notebook.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebook
{
    public class Notebook : NotebookModel
    {
        public List<Note> Notes { get; set; }

        /// <summary>
        /// When there's more than 0 notes: [TheTitle] (n)
        /// Otherwise: [TheTitle]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Title + (Notes.Count > 0 ? $" ({Notes.Count})" : "");
    }
}
