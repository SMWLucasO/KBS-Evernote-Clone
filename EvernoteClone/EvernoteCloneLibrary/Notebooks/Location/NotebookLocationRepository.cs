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
        /// <param name="ToInsert">The model to be inserted into the table</param>
        /// <returns>bool to determine if the note was inserted</returns>
        public bool Insert(NotebookLocationModel ToInsert)
        {
            if (ToInsert != null)
            {
                Dictionary<string, object> parameters = GenerateQueryParameters(ToInsert);

                int id = DataAccess.Instance.ExecuteAndReturnId("INSERT INTO [NotebookLocation] ([Path])"
                        + " VALUES (@Path)", parameters);

                if (id != -1)
                    ToInsert.Id = id;
                return id != -1;
            }
            return false;
        }

        /// <summary>
        /// The method for selecting NotebookLocation records which satisfy the conditions.
        /// </summary>
        /// <param name="Conditions">These parameters may NOT be user-typed, injection is possible.</param>
        /// <param name="Parameters">Bindings for the conditions.</param>
        /// <returns>an enumerable containing all the notes selected from the database.</returns>
        public IEnumerable<NotebookLocationModel> GetBy(string[] Conditions, Dictionary<string, object> Parameters)
        {
            List<NotebookLocationModel> generatedModels = new List<NotebookLocationModel>();

            // Query the database using the specified data
            SqlDataReader sqlDataReader = DataAccess.Instance.ExecuteAndRead("NotebookLocation", Conditions, Parameters);

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
        /// <param name="ToUpdate">The model which is to be updated</param>
        /// <returns>bool to determine if the update was a success</returns>
        public bool Update(NotebookLocationModel ToUpdate)
        {
            // TODO: Make sure the note is actually from the author before saving it. @Lucas don't think this should be here...
            if (ToUpdate != null)
            {
                if (!(string.IsNullOrEmpty(ToUpdate.Path)))
                {
                    Dictionary<string, object> parameters = GenerateQueryParameters(ToUpdate);
                    parameters.Add("@Id", ToUpdate.Id);

                    return DataAccess.Instance.Execute("UPDATE [NotebookLocation] SET [Path] = @Path WHERE Id = @Id",
                        parameters);
                }
            }
            return false;
        }

        /// <summary>
        /// The method for deleting the NotebookLocation record which the specified model represents.
        /// </summary>
        /// <param name="ToDelete"></param>
        /// <returns></returns>
        public bool Delete(NotebookLocationModel ToDelete)
        {
            if (ToDelete != null)
            {
                Dictionary<string, object> Parameter = new Dictionary<string, object>()
                {
                    { "@Id", ToDelete.Id }
                };

                return DataAccess.Instance.Execute("DELETE FROM [NotebookLocation] WHERE Id = @Id", Parameter);
            }
            return false;
        }
        
        /// <summary>
        /// A helper method to generate the query parameters.
        /// </summary>
        /// <param name="ToExtractFrom">The NotebookLocationModel which data will be extracted from</param>
        /// <returns></returns>
        public Dictionary<string, object> GenerateQueryParameters(NotebookLocationModel ToExtractFrom)
        {
            if (ToExtractFrom != null)
            {
                return new Dictionary<string, object>() {
                    { "@Path", ToExtractFrom.Path },
                };
            }
            return null;
        }
    }
}