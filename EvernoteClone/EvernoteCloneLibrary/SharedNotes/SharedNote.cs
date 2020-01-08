using System.Collections.Generic;
using System.Linq;
using EvernoteCloneLibrary.Constants;

namespace EvernoteCloneLibrary.SharedNotes
{
    /// <summary>
    /// This handles all logic for SharedNotes
    /// </summary>
    public class SharedNote : SharedNoteModel
    {
        /// <summary>
        /// Takes id of note and from the user. Inserts these two values in the database.
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="userId">The userId of the User that it is shared to</param>
        /// <returns></returns>
        public static bool SaveNewRecord(int noteId, int userId)
        {
            // Gets the two ID of a note and user and inserts them into the database.
            SharedNoteRepository sharedNoteRepository = new SharedNoteRepository();
            SharedNoteModel sharedNoteModel = new SharedNoteModel
            {
                NoteId = noteId,
                UserId = userId
            };
            
            return sharedNoteRepository.Insert(sharedNoteModel);
        }

        /// <summary>
        /// Inserts a new sharedNoteModel
        /// </summary>
        /// <param name="sharedNoteModel"></param>
        /// <returns></returns>
        public static bool SaveNewRecord(SharedNoteModel sharedNoteModel)
        {
            SharedNoteRepository sharedNoteRepository = new SharedNoteRepository();
            return sharedNoteRepository.Insert(sharedNoteModel);
        }

        /// <summary>
        /// Takes id of note and from the user. Deletes these two values in the database.
        /// </summary>
        /// <param name="noteId"></param>
        /// <param name="userId">The userId of the User that it is shared to</param>
        /// <returns></returns>
        public static bool RemoveRecord(int noteId, int userId)
        {
            // Gets the two ID of a note and user and inserts them into the database.
            SharedNoteRepository sharedNoteRepository = new SharedNoteRepository();
            SharedNoteModel sharedNoteModel = new SharedNoteModel
            {
                NoteId = noteId,
                UserId = userId
            };
            
            return sharedNoteRepository.Delete(sharedNoteModel);
        }
        
        /// <summary>
        /// Remove a sharedNoteModel record from the database
        /// </summary>
        /// <param name="sharedNoteModel"></param>
        /// <returns></returns>
        public static bool RemoveRecord(SharedNoteModel sharedNoteModel)
        {
            SharedNoteRepository sharedNoteRepository = new SharedNoteRepository();
            return sharedNoteRepository.Delete(sharedNoteModel);
        }

        /// <summary>
        /// Returns all SharedNote records that are linked to logged in User
        /// </summary>
        /// <returns>A list of SharedNotes</returns>
        public static List<SharedNote> GetAllSharedNotes() =>
            new SharedNoteRepository().GetBy(
                new[] { "UserID = @UserID" },
                new Dictionary<string, object> { { "@UserID", Constant.User.Id } }
                ).Cast<SharedNote>().ToList();
    }
}
