using EvernoteCloneLibrary.Database;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace EvernoteCloneLibrary.Notebooks.Location.LocationUser
{
    public class LocationUserRepository : IRepository<LocationUserModel>
    {
        /// <summary>
        /// The method for inserting a LocationUser record, where the class members are columns and the class member values are column values.
        /// </summary>
        /// <param name="ToInsert">The model to be inserted into the table</param>
        /// <returns>bool to determine if the note was inserted</returns>
        public bool Insert(LocationUserModel ToInsert)
        {
            if (ToInsert != null)
            {
                Dictionary<string, object> parameters = GenerateQueryParameters(ToInsert);

                return DataAccess.Instance.Execute("INSERT INTO [LocationUser] ([LocationID], [UserID]) VALUES (@LocationID, @UserID)", parameters);
            }
            return false;
        }

        /// <summary>
        /// The method for selecting LocationUser records which satisfy the conditions.
        /// </summary>
        /// <param name="Conditions">These parameters may NOT be user-typed, injection is possible.</param>
        /// <param name="Parameters">Bindings for the conditions.</param>
        /// <returns>an enumerable containing all the notes selected from the database.</returns>
        public IEnumerable<LocationUserModel> GetBy(string[] Conditions, Dictionary<string, object> Parameters)
        {
            List<LocationUserModel> generatedModels = new List<LocationUserModel>();

            // Query the database using the specified data
            SqlDataReader sqlDataReader = DataAccess.Instance.ExecuteAndRead("LocationUser", Conditions, Parameters);

            while (sqlDataReader.Read())
            {
                // Generate a model for each row of the LocationUser table.
                generatedModels.Add(new LocationUser()
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
        /// This will always return false, because this is a 'koppel tabel' and this will never be updated (only inserted, deleted or viewed)
        /// </summary>
        /// <param name="ToUpdate">The model which is to be updated</param>
        /// <returns>bool to determine if the update was a success (will always be false)</returns>
        public bool Update(LocationUserModel ToUpdate) => false;

        /// <summary>
        /// The method for deleting the LocationUser record which the specified model represents.
        /// </summary>
        /// <param name="ToDelete"></param>
        /// <returns></returns>
        public bool Delete(LocationUserModel ToDelete)
        {
            if (ToDelete != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    { "@LocationID", ToDelete.LocationId },
                    { "@UserID", ToDelete.UserId }
                };

                return DataAccess.Instance.Execute("DELETE FROM [LocationUser] WHERE LocationID = @LocationID AND UserID = @UserID", parameters);
            }

            return false;
        }

        /// <summary>
        /// A helper method to generate the query parameters.
        /// </summary>
        /// <param name="ToExtractFrom">The LocationUserModel which data will be extracted from</param>
        /// <returns></returns>
        public Dictionary<string, object> GenerateQueryParameters(LocationUserModel ToExtractFrom)
        {
            if (ToExtractFrom != null)
            {
                return new Dictionary<string, object>() {
                    { "@LocationID", ToExtractFrom.LocationId },
                    { "@UserID", ToExtractFrom.UserId }
                };
            }

            return null;
        }
    }
}
