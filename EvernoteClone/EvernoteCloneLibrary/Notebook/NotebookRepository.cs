using EvernoteCloneLibrary.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebook
{
    /// <summary>
    /// The repository(pattern) responsible for notebooks in the database.
    /// </summary>
    public class NotebookRepository : IRepository<NotebookModel>
    {

        /// <summary>
        /// The method for inserting a Notebook record, where the class members are columns and the class member values are column values.
        /// </summary>
        /// <param name="ToInsert">The model to be inserted into the table</param>
        /// <returns>bool to determine if the note was inserted</returns>
        public bool Insert(NotebookModel ToInsert)
        {
            Dictionary<string, object> Parameters = GenerateQueryParameters(ToInsert);

            return DataAccess.Instance.Execute("INSERT INTO [Notebook] ([UserID], [LocationID], [Title], [CreationDate], [LastUpdated])"
                    + " VALUES (@UserID, @LocationID, @Title, @CreationDate, @LastUpdated)", Parameters);
        }

        /// <summary>
        /// The method for selecting Notebook records which satisfy the conditions.
        /// </summary>
        /// <param name="conditions">These parameters may NOT be user-typed, injection is possible.</param>
        /// <param name="Parameters">Bindings for the conditions.</param>
        /// <returns>an enumerable containing all the notebooks selected from the database.</returns>
        public IEnumerable<NotebookModel> GetBy(string[] Conditions, Dictionary<string, object> Parameters)
        {
            List<Notebook> generatedNotebooks = new List<Notebook>();
            SqlDataReader fetchedSqlDataReader = DataAccess.Instance.ExecuteAndRead("Notebook", Conditions, Parameters);

            while (fetchedSqlDataReader.Read())
            {
                generatedNotebooks.Add(new Notebook()
                {
                    Id = (int)fetchedSqlDataReader["Id"],
                    UserID = (int)fetchedSqlDataReader["UserID"],
                    Title = (string)fetchedSqlDataReader["Title"],
                    CreationDate = (DateTime)fetchedSqlDataReader["CreationDate"],
                    LastUpdated = (DateTime)fetchedSqlDataReader["LastUpdated"]
                });
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
        /// <param name="ToUpdate">The model which is to be updated</param>
        /// <returns>bool to determine if the update was a success</returns>
        public bool Update(NotebookModel ToUpdate)
        {
            Dictionary<string, object> parameters = GenerateQueryParameters(ToUpdate);
            parameters.Add("@Id", ToUpdate.Id);

            return DataAccess.Instance.Execute("UPDATE [Notebook] SET [UserID] = @UserID, [LocationID] = @LocationID, "
                + "[Title] = @Title, [CreationDate] = @CreationDate, [LastUpdated] = @LastUpdated WHERE Id = @Id", parameters);

        }

        /// <summary>
        /// The method for deleting the Notebook record which the specified model represents.
        /// </summary>
        /// <param name="ToDelete"></param>
        /// <returns></returns>
        public bool Delete(NotebookModel ToDelete)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                { "@Id", ToDelete.Id }
            };

            return DataAccess.Instance.Execute("DELETE FROM [Notebook] WHERE Id = @Id", parameters);

        }

        /// <summary>
        /// A helper method to generate the query parameters.
        /// </summary>
        /// <param name="ToExtractFrom">The NotebookModel which data will be extracted from</param>
        /// <returns></returns>
        public Dictionary<string, object> GenerateQueryParameters(NotebookModel ToExtractFrom)
        {
            if (ToExtractFrom != null)
            {
                return new Dictionary<string, object>()
                {
                    { "@UserID", ToExtractFrom.UserID },
                    { "@LocationID", ToExtractFrom.LocationID },
                    { "@Title", ToExtractFrom.Title },
                    { "@CreationDate", ToExtractFrom.CreationDate },
                    { "@LastUpdated", ToExtractFrom.LastUpdated }
                };
            }

            return null;
        }
    }
}
