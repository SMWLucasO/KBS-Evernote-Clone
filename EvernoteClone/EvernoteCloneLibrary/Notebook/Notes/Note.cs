using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebook.Notes
{
    public class Note : NoteModel
    {
        public override string ToString() => $"{Title} by {Author}";
    }
}
