using EvernoteCloneLibrary.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.SharedNotes
{
    public class SharedNoteModel : IModel
    {
        public int UserId { get; set; }
        public int NoteId { get; set; }

    }
}
