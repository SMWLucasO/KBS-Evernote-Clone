using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.Settings.Locales
{
    /// <summary>
    /// The repository(pattern) responsible for locales in the database.
    /// </summary>
    public class LocaleRepository : IRepository<LocaleModel>
    {
        /// <summary>
        /// The method for inserting a Locale record, where the class members are columns and the class member values are column values.
        /// </summary>
        /// <param name="toInsert">The model to be inserted into the table</param>
        /// <returns>bool to determine if the note was inserted</returns>
        public bool Insert(LocaleModel toInsert)
        {
            if (toInsert != null)
            {
                if (string.IsNullOrWhiteSpace(toInsert.Language) || string.IsNullOrWhiteSpace(toInsert.Locale))
                {
                    return false;
                }

                Dictionary<string, object> parameters = GenerateQueryParameters(toInsert);

                int id = DataAccess.Instance.ExecuteAndReturnId(
                    "INSERT INTO [Locale] ([Locale], [Language])"
                        + " VALUES (@Locale, @Language)", parameters);

                if (id != -1)
                {
                    toInsert.Id = id;
                }

                return id != -1;
            }
            return false;
        }

        /// <summary>
        /// The method for selecting Locale records which satisfy the conditions.
        /// </summary>
        /// <param name="conditions">These parameters may NOT be user-typed, injection is possible.</param>
        /// <param name="parameters">Bindings for the conditions.</param>
        /// <returns>an enumerable containing all the locales selected from the database.</returns>
        public IEnumerable<LocaleModel> GetBy(string[] conditions, Dictionary<string, object> parameters)
        {
            List<Locale> locales = new List<Locale>();
            SqlDataReader fetchedSqlDataReader = DataAccess.Instance.ExecuteAndRead("Locale", conditions, parameters);

            while (fetchedSqlDataReader.Read())
            {
                Locale locale = new Locale
                {
                    Id = (int)fetchedSqlDataReader["Id"],
                    Locale = (string)fetchedSqlDataReader["Locale"],
                    Language = (string)fetchedSqlDataReader["Language"]
                };

                // Add the setting to the settings variable
                locales.Add(locale);
            }

            // We always have to close the sql connection, because it does not get closed otherwise.
            // This only goes for 'DataAccess#ExecuteAndRead'
            DataAccess.Instance.CloseSqlConnection();
            fetchedSqlDataReader.Close();

            return locales.AsEnumerable();
        }

        /// <summary>
        /// Returns all Locale records from database
        /// </summary>
        /// <returns>Returns a enumerable of LocaleModel</returns>
        public IEnumerable<LocaleModel> GetAll()
        {
            List<Locale> locales = new List<Locale>();
            SqlDataReader fetchedSqlDataReader = DataAccess.Instance.ExecuteAndRead("Locale");

            while (fetchedSqlDataReader.Read())
            {
                Locale locale = new Locale
                {
                    Id = (int)fetchedSqlDataReader["Id"],
                    Locale = (string)fetchedSqlDataReader["Locale"],
                    Language = (string)fetchedSqlDataReader["Language"]
                };

                // Add the setting to the settings variable
                locales.Add(locale);
            }

            // We always have to close the sql connection, because it does not get closed otherwise.
            // This only goes for 'DataAccess#ExecuteAndRead'
            DataAccess.Instance.CloseSqlConnection();
            fetchedSqlDataReader.Close();

            return locales.AsEnumerable();
        }

        /// <summary>
        /// The method for updating the Locale record which the specified model represents.
        /// </summary>
        /// <param name="toUpdate">The model which is to be updated</param>
        /// <returns>bool to determine if the update was a success</returns>
        public bool Update(LocaleModel toUpdate)
        {
            if (toUpdate != null)
            {
                if (string.IsNullOrWhiteSpace(toUpdate.Language) || string.IsNullOrWhiteSpace(toUpdate.Locale) || toUpdate.Id == -1)
                {
                    return false;
                }
                
                Dictionary<string, object> parameters = GenerateQueryParameters(toUpdate);
                parameters.Add("@Id", toUpdate.Id);

                return DataAccess.Instance.Execute("UPDATE [Locale] SET [Language] = @Language, [Locale] = @Locale "
                    + "WHERE Id = @Id", parameters);
            }
            return false;
        }

        /// <summary>
        /// The method for deleting the Locale record which the specified model represents.
        /// </summary>
        /// <param name="toDelete">The LocaleModel that should be deleted from the database</param>
        /// <returns>a boolean that indicates if the Delete was successful or not</returns>
        public bool Delete(LocaleModel toDelete)
        {
            if (toDelete != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    { "@Id", toDelete.Id }
                };

                return DataAccess.Instance.Execute("DELETE FROM [Locale] WHERE Id = @Id", parameters);
            }
            return false;
        }

        /// <summary>
        /// A helper method to generate the query parameters.
        /// </summary>
        /// <param name="toExtractFrom">The LocaleModel which data will be extracted from</param>
        /// <returns>The parameters</returns>
        public Dictionary<string, object> GenerateQueryParameters(LocaleModel toExtractFrom)
        {
            if (toExtractFrom != null)
            {
                return new Dictionary<string, object>
                {
                    { "@Locale", toExtractFrom.Locale },
                    { "@Language", toExtractFrom.Language }
                };
            }
            return null;
        }
    }
}