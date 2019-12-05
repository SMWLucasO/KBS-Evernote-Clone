using EvernoteCloneLibrary.Database;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace EvernoteCloneLibrary.Notebooks.Location
{
    public class NotebookLocationRepository : IRepository<NotebookLocationModel>
    {
        /// <summary>
        /// The method for inserting a NotebookLocation record, where the class members are columns and the class member values are column values.
        /// </summary>
        /// <param name="toInsert">The model to be inserted into the table</param>
        /// <returns>bool to determine if the note was inserted</returns>
        public bool Insert(NotebookLocationModel toInsert)
        {
            if (toInsert != null)
            {
                Dictionary<string, object> parameters = GenerateQueryParameters(toInsert);

                int id = DataAccess.Instance.ExecuteAndReturnId("INSERT INTO [NotebookLocation] ([Path])"
                        + " VALUES (@Path)", parameters);

                if (id != -1)
                    toInsert.Id = id;
                return id != -1;
            }
            return false;
        }

        /// <summary>
        /// The method for selecting NotebookLocation records which satisfy the conditions.
        /// </summary>
        /// <param name="conditions">These parameters may NOT be user-typed, injection is possible.</param>
        /// <param name="parameters">Bindings for the conditions.</param>
        /// <returns>an enumerable containing all the notes selected from the database.</returns>
        public IEnumerable<NotebookLocationModel> GetBy(string[] conditions, Dictionary<string, object> parameters)
        {
            List<NotebookLocationModel> generatedModels = new List<NotebookLocationModel>();

            // Query the database using the specified data
            SqlDataReader sqlDataReader = DataAccess.Instance.ExecuteAndRead("NotebookLocation", conditions, parameters);

            while (sqlDataReader.Read())
            {
                // Generate a model for each row of the NotebookLocations table.
                generatedModels.Add(new NotebookLocation()
                {
                    Id = (int)sqlDataReader["Id"],
                    Path = (string)sqlDataReader["Path"]
                });
            }

            // We always have to close the sql connection, because it does not get closed otherwise.
            DataAccess.Instance.CloseSqlConnection();
            sqlDataReader.Close();

            return generatedModels.AsEnumerable();
        }

        /// <summary>
        /// The method for updating the NotebookLocationModel record which the specified model represents.
        /// </summary>
        /// <param name="toUpdate">The model which is to be updated</param>
        /// <returns>bool to determine if the update was a success</returns>
        public bool Update(NotebookLocationModel toUpdate)
        {
            // TODO: Make sure the note is actually from the author before saving it. @Lucas don't think this should be here...
            if (toUpdate != null)
            {
                if (!(string.IsNullOrEmpty(toUpdate.Path)))
                {
                    Dictionary<string, object> parameters = GenerateQueryParameters(toUpdate);
                    parameters.Add("@Id", toUpdate.Id);

                    return DataAccess.Instance.Execute("UPDATE [NotebookLocation] SET [Path] = @Path WHERE Id = @Id",
                        parameters);
                }
            }
            return false;
        }

        /// <summary>
        /// The method for deleting the NotebookLocation record which the specified model represents.
        /// </summary>
        /// <param name="toDelete"></param>
        /// <returns></returns>
        public bool Delete(NotebookLocationModel toDelete)
        {
            if (toDelete != null)
            {
                Dictionary<string, object> parameter = new Dictionary<string, object>()
                {
                    { "@Id", toDelete.Id }
                };

                return DataAccess.Instance.Execute("DELETE FROM [NotebookLocation] WHERE Id = @Id", parameter);
            }
            return false;
        }
        
        /// <summary>
        /// A helper method to generate the query parameters.
        /// </summary>
        /// <param name="toExtractFrom">The NotebookLocationModel which data will be extracted from</param>
        /// <returns></returns>
        public Dictionary<string, object> GenerateQueryParameters(NotebookLocationModel toExtractFrom)
        {
            if (toExtractFrom != null)
            {
                return new Dictionary<string, object>() {
                    { "@Path", toExtractFrom.Path },
                };
            }
            return null;
        }
    }
}