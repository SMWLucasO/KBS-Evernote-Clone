using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.SharedNotes
{
    public class SharedNote : SharedNoteModel
    {
        /// <summary>
        /// Takes id of note and from the user. Inserts these two values in the database.
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool SaveNewRecord(int noteId, int userId)
        {
            // Gets the two ID of a note and user and inserts them into the database.
            SharedNoteRepository sharedNoteRepository = new SharedNoteRepository();
            SharedNoteModel sharedNoteModel = new SharedNoteModel()
            {
                NoteId = noteId,
                UserId = userId
            };
            return sharedNoteRepository.Insert(sharedNoteModel);
        }
    }
}
