using EvernoteCloneLibrary.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace EvernoteCloneLibrary.SharedNotes
{
    public class SharedNoteRepository : IRepository<SharedNoteModel>
    {
        /// <summary>
        /// Method responsible for remove the shared note. 
        /// </summary>
        /// <param name="toDelete"></param>
        /// <returns></returns>
        public bool Delete(SharedNoteModel toDelete)
        {
            if (toDelete != null)
            {
                Dictionary<string, object> parameter = new Dictionary<string, object>()
                {
                    { "UserId", toDelete.UserId}
                };

                return DataAccess.Instance.Execute("DELETE FROM [SharedNote] WHERE Id = @UserId", parameter);
            }
            return false;
        }

        /// <summary>
        /// A helper method to generate the query parameters.
        /// </summary>
        /// <param name="toExtractFrom"></param>
        /// <returns></returns>
        public Dictionary<string, object> GenerateQueryParameters(SharedNoteModel toExtractFrom)
        {
            if (toExtractFrom != null)
            {
                return new Dictionary<string, object>() 
                {
                    { "@NoteId", toExtractFrom.NoteId },
                    { "@UserId", toExtractFrom.UserId }
                };
            }
            return null;
        }

        /// <summary>
        /// Method that selects the shared user by finding the id of the note and user.
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<SharedNoteModel> GetBy(string[] conditions, Dictionary<string, object> parameters)
        {
            List<SharedNoteModel> generatedModels = new List<SharedNoteModel>();

            // Query the database using the specified data
            SqlDataReader sqlDataReader = DataAccess.Instance.ExecuteAndRead("SharedNote", conditions, parameters);

            while (sqlDataReader.Read())
            {
                // Generate a model for each row of the NotebookLocations table.
                generatedModels.Add(new SharedNote()
                {
                    NoteId = (int)sqlDataReader["NoteId"],
                    UserId = (int)sqlDataReader["UserId"]
                    
                });
            }

            // We always have to close the sql connection, because it does not get closed otherwise.
            DataAccess.Instance.CloseSqlConnection();
            sqlDataReader.Close();

            return generatedModels.AsEnumerable();
        }

        /// <summary>
        /// Method responsible for inserting the note in the database and adding it
        /// to the correct user.
        /// </summary>
        /// <param name="toInsert"></param>
        /// <returns></returns>
        public bool Insert(SharedNoteModel toInsert)
        {
            if (toInsert != null)
            {
                Dictionary<string, object> parameters = GenerateQueryParameters(toInsert);

                bool insert = DataAccess.Instance.Execute("INSERT INTO [SharedNote] ([NoteId], [UserId])"
                        + " VALUES (@NoteId, @UserId)", parameters);

                return insert;
            }
            return false;
        }

        /// <summary>
        /// Updates the user id of the note incase the note is shared again.
        /// </summary>
        /// <param name="toUpdate"></param>
        /// <returns></returns>
        public bool Update(SharedNoteModel toUpdate)
        {
            if (toUpdate != null)
            {
                if (toUpdate.UserId != -1)
                {
                    Dictionary<string, object> parameters = GenerateQueryParameters(toUpdate);
                    parameters.Add("@UserId", toUpdate.UserId);

                    return DataAccess.Instance.Execute("UPDATE [SharedNote] SET [UserId] = @UserId WHERE Id = @UserId",
                        parameters);
                }
            }
            return false;
        }
    }
}
