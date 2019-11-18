using EvernoteCloneLibrary.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="ToInsert">The model to be inserted into the table</param>
        /// <returns>bool to determine if the note was inserted</returns>
        public bool Insert(NoteModel ToInsert)
        {
            Dictionary<string, object> parameters = GenerateQueryParameters(ToInsert);

            return DataAccess.Instance.Execute("INSERT INTO [Note] ([NotebookID], [Title], [Content], [Author], [CreationDate], [LastUpdated])"
                    + " VALUES (@NotebookID, @Title, @Content, @Author, @CreationDate, @LastUpdated)", parameters);
        }

        /// <summary>
        /// The method for selecting Note records which satisfy the conditions.
        /// </summary>
        /// <param name="conditions">These parameters may NOT be user-typed, injection is possible.</param>
        /// <param name="Parameters">Bindings for the conditions.</param>
        /// <returns>an enumerable containing all the notes selected from the database.</returns>
        public IEnumerable<NoteModel> GetBy(string[] Conditions, Dictionary<string, object> Parameters)
        {
            List<NoteModel> generatedModels = new List<NoteModel>();

            // Query the database using the specified data
            SqlDataReader sqlDataReader = DataAccess.Instance.ExecuteAndRead("Note", Conditions, Parameters);

            while (sqlDataReader.Read())
            {
                // Generate a model for each row of the Notes table.
                generatedModels.Add(new Note()
                {
                    Id = (int)sqlDataReader["Id"],
                    NotebookID = (int)sqlDataReader["NotebookID"],
                    Title = (string)sqlDataReader["Title"],
                    Content = (string)sqlDataReader["Content"],
                    Author = (string)sqlDataReader["Author"],
                    CreationDate = (DateTime)sqlDataReader["CreationDate"],
                    LastUpdated = (DateTime)sqlDataReader["LastUpdated"]
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
        /// <param name="ToUpdate">The model which is to be updated</param>
        /// <returns>bool to determine if the update was a success</returns>
        public bool Update(NoteModel ToUpdate)
        {
            // TODO: Make sure the note is actually from the author before saving it.

            Dictionary<string, object> parameters = GenerateQueryParameters(ToUpdate);
            parameters.Add("@Id", ToUpdate.Id);


            return DataAccess.Instance.Execute("UPDATE [Note] SET [NotebookID] = @NotebookID, [Title] = @Title, [Content] = @Content, "
                + "[Author] = @Author, [CreationDate] = @CreationDate, [LastUpdated] = @LastUpdated WHERE Id = @Id",
                parameters);
        }

        /// <summary>
        /// The method for deleting the Note record which the specified model represents.
        /// </summary>
        /// <param name="ToDelete"></param>
        /// <returns></returns>
        public bool Delete(NoteModel ToDelete)
        {
            // TODO verify that the note is actually from the one deleting it
            Dictionary<string, object> Parameter = new Dictionary<string, object>()
            {
                { "@Id", ToDelete.Id }
            };

            return DataAccess.Instance.Execute("DELETE FROM [Note] WHERE Id = @Id", Parameter);
        }


        /// <summary>
        /// A helper method to generate the query parameters.
        /// </summary>
        /// <param name="ToExtractFrom">The NoteModel which data will be extracted from</param>
        /// <returns></returns>
        public Dictionary<string, object> GenerateQueryParameters(NoteModel ToExtractFrom)
        {
            if (ToExtractFrom != null)
            {
                return new Dictionary<string, object>() {
                { "@NotebookID", ToExtractFrom.NotebookID },
                { "@Title", ToExtractFrom.Title },
                { "@Content",  ToExtractFrom.Content },
                { "@Author", ToExtractFrom.Author },
                { "@CreationDate", ToExtractFrom.CreationDate.Date },
                { "@LastUpdated", ToExtractFrom.LastUpdated }
            };
            }

            return null;
        }

    }
}
