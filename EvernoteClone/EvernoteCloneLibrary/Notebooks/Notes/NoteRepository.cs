using EvernoteCloneLibrary.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace EvernoteCloneLibrary.Notebooks.Notes
{
    /// <summary>
    /// The repository(pattern) responsible for notes in the database.
    /// </summary>
    public class NoteRepository : IRepository<NoteModel>
    {
        /// <summary>
        /// The method for inserting a Note record, where the class members are columns and the class member values are column values.
        /// </summary>
        /// <param name="toInsert">The model to be inserted into the table</param>
        /// <returns>bool to determine if the note was inserted</returns>
        public bool Insert(NoteModel toInsert)
        {
            if (toInsert != null)
            {
                if (CheckIfDataIsCorrect(toInsert))
                {
                    if (string.IsNullOrEmpty(toInsert.Title))
                        toInsert.Title = "Nameless note";

                    Dictionary<string, object> parameters = GenerateQueryParameters(toInsert);

                    int id = DataAccess.Instance.ExecuteAndReturnId("INSERT INTO [Note] ([NotebookID], [Title], [Content], [Author], [CreationDate], [LastUpdated], [Deleted])"
                            + " VALUES (@NotebookID, @Title, @Content, @Author, @CreationDate, @LastUpdated, @Deleted)", parameters);
                    
                    if(id != -1)
                        toInsert.Id = id;
                    return id != -1;
                }
            }
            return false;
        }

        private static bool CheckIfDataIsCorrect(NoteModel noteModel) =>
            !(string.IsNullOrEmpty(noteModel.Author));

        /// <summary>
        /// The method for selecting Note records which satisfy the conditions.
        /// </summary>
        /// <param name="conditions">These parameters may NOT be user-typed, injection is possible.</param>
        /// <param name="parameters">Bindings for the conditions.</param>
        /// <returns>an enumerable containing all the notes selected from the database.</returns>
        public IEnumerable<NoteModel> GetBy(string[] conditions, Dictionary<string, object> parameters)
        {
            List<NoteModel> generatedModels = new List<NoteModel>();

            // Query the database using the specified data
            SqlDataReader sqlDataReader = DataAccess.Instance.ExecuteAndRead("Note", conditions, parameters);

            while (sqlDataReader.Read())
            {
                // Generate a model for each row of the Notes table.
                generatedModels.Add(new Note()
                {
                    Id = (int)sqlDataReader["Id"],
                    NotebookId = (int)sqlDataReader["NotebookID"],
                    Title = (string)sqlDataReader["Title"],
                    Content = (string)sqlDataReader["Content"],
                    NewContent = (string)sqlDataReader["Content"],
                    Author = (string)sqlDataReader["Author"],
                    CreationDate = (DateTime)sqlDataReader["CreationDate"],
                    LastUpdated = (DateTime)sqlDataReader["LastUpdated"],
                    IsDeleted = (bool) sqlDataReader["Deleted"]
                });
            }

            // We always have to close the sql connection, because it does not get closed otherwise.
            DataAccess.Instance.CloseSqlConnection();
            sqlDataReader.Close();

            return generatedModels.AsEnumerable();
        }

        /// <summary>
        /// The method for updating the Note record which the specified model represents.
        /// </summary>
        /// <param name="toUpdate">The model which is to be updated</param>
        /// <returns>bool to determine if the update was a success</returns>
        public bool Update(NoteModel toUpdate)
        {
            // TODO: Make sure the note is actually from the author before saving it.
            if (toUpdate != null)
            {
                if (CheckIfDataIsCorrect(toUpdate))
                {
                    Dictionary<string, object> parameters = GenerateQueryParameters(toUpdate);
                    parameters.Add("@Id", toUpdate.Id);

                    return DataAccess.Instance.Execute("UPDATE [Note] SET [NotebookID] = @NotebookID, [Title] = @Title, [Content] = @Content, "
                        + "[Author] = @Author, [CreationDate] = @CreationDate, [LastUpdated] = @LastUpdated, [Deleted] = @Deleted WHERE Id = @Id",
                        parameters);
                }
            }
            return false;
        }

        /// <summary>
        /// The method for deleting the Note record which the specified model represents.
        /// </summary>
        /// <param name="toDelete"></param>
        /// <returns></returns>
        public bool Delete(NoteModel toDelete)
        {
            if(toDelete != null)
            {
                Dictionary<string, object> parameter = new Dictionary<string, object>
                {
                    { "@Id", toDelete.Id }
                };

                return DataAccess.Instance.Execute("DELETE FROM [Note] WHERE Id = @Id", parameter);
            }
            return false;
        }

        /// <summary>
        /// A helper method to generate the query parameters.
        /// </summary>
        /// <param name="toExtractFrom">The NoteModel which data will be extracted from</param>
        /// <returns></returns>
        public Dictionary<string, object> GenerateQueryParameters(NoteModel toExtractFrom)
        {
            if (toExtractFrom != null)
            {
                return new Dictionary<string, object> {
                    { "@NotebookID", toExtractFrom.NotebookId },
                    { "@Title", toExtractFrom.Title },
                    { "@Content",  toExtractFrom.Content },
                    { "@Author", toExtractFrom.Author },
                    { "@CreationDate", toExtractFrom.CreationDate.Date },
                    { "@LastUpdated", toExtractFrom.LastUpdated },
                    { "@Deleted", toExtractFrom.IsDeleted }
                };
            }
            return null;
        }
    }
}
