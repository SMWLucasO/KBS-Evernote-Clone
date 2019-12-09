using EvernoteCloneLibrary.Database;
using EvernoteCloneLibrary.Notebooks.Location;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace EvernoteCloneLibrary.Notebooks
{
    /// <summary>
    /// The repository(pattern) responsible for notebooks in the database.
    /// </summary>
    public class NotebookRepository : IRepository<NotebookModel>
    {
        /// <summary>
        /// The method for inserting a Notebook record, where the class members are columns and the class member values are column values.
        /// </summary>
        /// <param name="toInsert">The model to be inserted into the table</param>
        /// <returns>bool to determine if the note was inserted</returns>
        public bool Insert(NotebookModel toInsert)
        {
            if (toInsert != null)
            {
                if (string.IsNullOrEmpty(toInsert.Title))
                {
                    toInsert.Title = "Nameless notebook";
                }

                Dictionary<string, object> parameters = GenerateQueryParameters(toInsert);

                int id = DataAccess.Instance.ExecuteAndReturnId(
                    "INSERT INTO [Notebook] ([UserID], [LocationID], [Title], [CreationDate], [LastUpdated], [Deleted])"
                        + " VALUES (@UserID, @LocationID, @Title, @CreationDate, @LastUpdated, @Deleted)", parameters);

                if (id != -1)
                {
                    toInsert.Id = id;
                }

                return id != -1;
            }
            return false;
        }

        /// <summary>
        /// The method for selecting Notebook records which satisfy the conditions.
        /// </summary>
        /// <param name="conditions">These parameters may NOT be user-typed, injection is possible.</param>
        /// <param name="parameters">Bindings for the conditions.</param>
        /// <returns>an enumerable containing all the notebooks selected from the database.</returns>
        public IEnumerable<NotebookModel> GetBy(string[] conditions, Dictionary<string, object> parameters)
        {
            List<Notebook> generatedNotebooks = new List<Notebook>();
            SqlDataReader fetchedSqlDataReader = DataAccess.Instance.ExecuteAndRead("Notebook", conditions, parameters);

            while (fetchedSqlDataReader.Read())
            {
                NoteRepository noteRepository = new NoteRepository();
                NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();

                Notebook notebook = new Notebook()
                {
                    Id = (int)fetchedSqlDataReader["Id"],
                    LocationId = (int)fetchedSqlDataReader["LocationID"],
                    UserId = (int)fetchedSqlDataReader["UserID"],
                    Title = (string)fetchedSqlDataReader["Title"],
                    CreationDate = (DateTime)fetchedSqlDataReader["CreationDate"],
                    LastUpdated = (DateTime)fetchedSqlDataReader["LastUpdated"],
                    IsDeleted = (bool)fetchedSqlDataReader["Deleted"],
                    Notes = new List<INote>(),
                };

                foreach (NoteModel model in noteRepository.GetBy(
                            new string[] { "NotebookID = @Id" },
                            new Dictionary<string, object>() {
                                { "@Id", (int) fetchedSqlDataReader["Id"] }
                            }))
                {
                    notebook.Notes.Add((INote)model);
                }

                foreach (NotebookLocationModel model in notebookLocationRepository.GetBy(
                            new string[] { "Id = @Id" },
                            new Dictionary<string, object>() {
                                { "@Id", (int) fetchedSqlDataReader["LocationID"] }
                            }
                        ))
                {
                    notebook.Path = new NotebookLocation()
                    {
                        Id = model.Id,
                        Path = model.Path
                    };

                    break;
                }

                // Make the notebook known to the note
                notebook.Notes.ForEach((el) => ((Note)el).NoteOwner = notebook);

                // Add the generated notebook with its notes to the list to be returned.
                generatedNotebooks.Add(notebook);
            }

            // We always have to close the sql connection, because it does not get closed otherwise.
            // This only goes for 'DataAccess#ExecuteAndRead'
            DataAccess.Instance.CloseSqlConnection();
            fetchedSqlDataReader.Close();

            return generatedNotebooks.AsEnumerable();
        }

        /// <summary>
        /// The method for updating the Notebook record which the specified model represents.
        /// </summary>
        /// <param name="toUpdate">The model which is to be updated</param>
        /// <returns>bool to determine if the update was a success</returns>
        public bool Update(NotebookModel toUpdate)
        {
            if (toUpdate != null)
            {
                if (string.IsNullOrEmpty(toUpdate.Title))
                {
                    toUpdate.Title = "Nameless title";
                }


                Dictionary<string, object> parameters = GenerateQueryParameters(toUpdate);
                parameters.Add("@Id", toUpdate.Id);

                return DataAccess.Instance.Execute("UPDATE [Notebook] SET [UserID] = @UserID, [LocationID] = @LocationID, "
                    + "[Title] = @Title, [CreationDate] = @CreationDate, [LastUpdated] = @LastUpdated, [Deleted] = @Deleted WHERE Id = @Id", parameters);
            }
            return false;
        }

        /// <summary>
        /// The method for deleting the Notebook record which the specified model represents.
        /// </summary>
        /// <param name="toDelete"></param>
        /// <returns></returns>
        public bool Delete(NotebookModel toDelete)
        {
            if (toDelete != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    { "@Id", toDelete.Id }
                };

                return DataAccess.Instance.Execute("DELETE FROM [Notebook] WHERE Id = @Id", parameters);
            }
            return false;
        }

        /// <summary>
        /// A helper method to generate the query parameters.
        /// </summary>
        /// <param name="toExtractFrom">The NotebookModel which data will be extracted from</param>
        /// <returns></returns>
        public Dictionary<string, object> GenerateQueryParameters(NotebookModel toExtractFrom)
        {
            if (toExtractFrom != null)
            {
                return new Dictionary<string, object>()
                {
                    { "@UserID", toExtractFrom.UserId },
                    { "@LocationID", toExtractFrom.LocationId },
                    { "@Title", toExtractFrom.Title },
                    { "@CreationDate", toExtractFrom.CreationDate },
                    { "@LastUpdated", toExtractFrom.LastUpdated },
                    { "@Deleted", toExtractFrom.IsDeleted }
                };
            }
            return null;
        }
    }
}
