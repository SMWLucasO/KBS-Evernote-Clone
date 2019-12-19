using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.Settings
{
    /// <summary>
    /// The repository(pattern) responsible for settings in the database.
    /// </summary>
    public class SettingRepository : IRepository<SettingModel>
    {
        /// <summary>
        /// The method for inserting a Setting record, where the class members are columns and the class member values are column values.
        /// </summary>
        /// <param name="toInsert">The model to be inserted into the table</param>
        /// <returns>bool to determine if the note was inserted</returns>
        public bool Insert(SettingModel toInsert)
        {
            if (toInsert != null)
            {
                if (string.IsNullOrWhiteSpace(toInsert.KeyWord))
                {
                    return false;
                }

                Dictionary<string, object> parameters = GenerateQueryParameters(toInsert);

                int id = DataAccess.Instance.ExecuteAndReturnId(
                    "INSERT INTO [Setting] ([UserID], [Keyword], [SettingValue])"
                        + " VALUES (@UserID, @Keyword, @SettingValue)", parameters);

                if (id != -1)
                {
                    toInsert.Id = id;
                }

                return id != -1;
            }
            return false;
        }

        /// <summary>
        /// The method for selecting Setting records which satisfy the conditions.
        /// </summary>
        /// <param name="conditions">These parameters may NOT be user-typed, injection is possible.</param>
        /// <param name="parameters">Bindings for the conditions.</param>
        /// <returns>an enumerable containing all the settings selected from the database.</returns>
        public IEnumerable<SettingModel> GetBy(string[] conditions, Dictionary<string, object> parameters)
        {
            List<Settings.Setting> settings = new List<Settings.Setting>();
            SqlDataReader fetchedSqlDataReader = DataAccess.Instance.ExecuteAndRead("Setting", conditions, parameters);

            while (fetchedSqlDataReader.Read())
            {
                Settings.Setting setting = new Settings.Setting
                {
                    Id = (int)fetchedSqlDataReader["Id"],
                    UserId = (int)fetchedSqlDataReader["UserID"],
                    KeyWord = (string)fetchedSqlDataReader["Keyword"],
                    SettingValue = (string)fetchedSqlDataReader["SettingValue"]
                };

                // Add the setting to the settings variable
                settings.Add(setting);
            }

            // We always have to close the sql connection, because it does not get closed otherwise.
            // This only goes for 'DataAccess#ExecuteAndRead'
            DataAccess.Instance.CloseSqlConnection();
            fetchedSqlDataReader.Close();

            return settings.AsEnumerable();
        }

        /// <summary>
        /// The method for updating the Setting record which the specified model represents.
        /// </summary>
        /// <param name="toUpdate">The model which is to be updated</param>
        /// <returns>bool to determine if the update was a success</returns>
        public bool Update(SettingModel toUpdate)
        {
            if (toUpdate != null)
            {
                if (string.IsNullOrWhiteSpace(toUpdate.KeyWord))
                {
                    return false;
                }
                
                Dictionary<string, object> parameters = GenerateQueryParameters(toUpdate);

                return DataAccess.Instance.Execute("UPDATE [Setting] SET [SettingValue] = @SettingValue "
                                                   + " WHERE UserID = @UserID AND Keyword = @Keyword", parameters);
            }
            return false;
        }

        /// <summary>
        /// The method for deleting the Setting record which the specified model represents.
        /// </summary>
        /// <param name="toDelete">The SettingModel that should be deleted from the database</param>
        /// <returns>a boolean that indicates if the Delete was successful or not</returns>
        public bool Delete(SettingModel toDelete)
        {
            if (toDelete != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    { "@Id", toDelete.Id }
                };

                return DataAccess.Instance.Execute("DELETE FROM [Setting] WHERE Id = @Id", parameters);
            }
            return false;
        }

        /// <summary>
        /// A helper method to generate the query parameters.
        /// </summary>
        /// <param name="toExtractFrom">The SettingModel which data will be extracted from</param>
        /// <returns>The parameters</returns>
        public Dictionary<string, object> GenerateQueryParameters(SettingModel toExtractFrom)
        {
            if (toExtractFrom != null)
            {
                return new Dictionary<string, object>
                {
                    { "@UserID", toExtractFrom.UserId },
                    { "@Keyword", toExtractFrom.KeyWord },
                    { "@SettingValue", toExtractFrom.SettingValue }
                };
            }
            return null;
        }
    }
}