using EvernoteCloneLibrary.Files.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebooks.Notes
{
    /// <summary>
    /// An interface in case of a future implementation of different types of notes.
    /// </summary>
    public interface INote : IParseable
    {
        public Notebook NoteOwner { get; set; }
    }
}
