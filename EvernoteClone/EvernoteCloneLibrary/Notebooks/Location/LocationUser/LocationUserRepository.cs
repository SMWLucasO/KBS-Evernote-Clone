using EvernoteCloneLibrary.Database;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace EvernoteCloneLibrary.Notebooks.Location.LocationUser
{
    /// <summary>
    /// This class handles all communication between the database
    /// </summary>
    public class LocationUserRepository : IRepository<LocationUserModel>
    {
        /// <summary>
        /// The method for inserting a LocationUser record, where the class members are columns and the class member values are column values.
        /// </summary>
        /// <param name="toInsert">The model to be inserted into the table</param>
        /// <returns>bool to determine if the note was inserted</returns>
        public bool Insert(LocationUserModel toInsert)
        {
            if (toInsert != null)
            {
                Dictionary<string, object> parameters = GenerateQueryParameters(toInsert);
                
                // Check if the path already exists, if so, set toInsert.Id equal to the already existing NotebookLocation id
                List<LocationUserModel> notebookLocations = 
                    GetBy(new[] {"LocationID = @LocationID", "UserID = @UserID"}, parameters).ToList();
                
                if (notebookLocations.Count > 0)
                {
                    return true;
                }

                return DataAccess.Instance.Execute(
                    "INSERT INTO [LocationUser] ([LocationID], [UserID]) VALUES (@LocationID, @UserID)", 
                    parameters);
            }
            return false;
        }

        /// <summary>
        /// The method for selecting LocationUser records which satisfy the conditions.
        /// </summary>
        /// <param name="conditions">These parameters may NOT be user-typed, injection is possible.</param>
        /// <param name="parameters">Bindings for the conditions.</param>
        /// <returns>an enumerable containing all the notes selected from the database.</returns>
        public IEnumerable<LocationUserModel> GetBy(string[] conditions, Dictionary<string, object> parameters)
        {
            List<LocationUserModel> generatedModels = new List<LocationUserModel>();

            // Query the database using the specified data
            SqlDataReader sqlDataReader = DataAccess.Instance.ExecuteAndRead("LocationUser", conditions, parameters);

            while (sqlDataReader.Read())
            {
                // Generate a model for each row of the LocationUser table.
                generatedModels.Add(new LocationUser
                { 
                    LocationId = (int)sqlDataReader["LocationID"],
                    UserId = (int)sqlDataReader["UserID"]
                });
            }

            // We always have to close the sql connection, because it does not get closed otherwise.
            DataAccess.Instance.CloseSqlConnection();
            sqlDataReader.Close();

            return generatedModels.AsEnumerable();
        }

        /// <summary>
        /// This will always return false, because this is a 'koppel' table and this will never be updated (only inserted, deleted or viewed)
        /// </summary>
        /// <param name="toUpdate">The model which is to be updated</param>
        /// <returns>bool to determine if the update was a success (will always be false)</returns>
        public bool Update(LocationUserModel toUpdate) => 
            false;

        /// <summary>
        /// The method for deleting the LocationUser record which the specified model represents.
        /// </summary>
        /// <param name="toDelete"></param>
        /// <returns></returns>
        public bool Delete(LocationUserModel toDelete)
        {
            if (toDelete != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@LocationID", toDelete.LocationId },
                    { "@UserID", toDelete.UserId }
                };

                return DataAccess.Instance.Execute(
                    "DELETE FROM [LocationUser] WHERE LocationID = @LocationID AND UserID = @UserID", 
                    parameters);
            }

            return false;
        }

        /// <summary>
        /// A helper method to generate the query parameters.
        /// </summary>
        /// <param name="toExtractFrom">The LocationUserModel which data will be extracted from</param>
        /// <returns></returns>
        public Dictionary<string, object> GenerateQueryParameters(LocationUserModel toExtractFrom)
        {
            if (toExtractFrom != null)
            {
                return new Dictionary<string, object>
                {
                    { "@LocationID", toExtractFrom.LocationId },
                    { "@UserID", toExtractFrom.UserId }
                };
            }

            return null;
        }
    }
}
