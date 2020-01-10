using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.Labels
{
    /// <summary>
    /// This class handles all communication with the database
    /// </summary>
    public class LabelRepository : IRepository<LabelModel>
    {
        /// <summary>
        /// This method can be used to delete a label from the database
        /// </summary>
        /// <param name="toDelete"></param>
        /// <returns></returns>
        public bool Delete(LabelModel toDelete)
        {
            if (toDelete != null)
            {
                Dictionary<string, object> parameter = new Dictionary<string, object>
                {
                    { "@Id", toDelete.Id }
                };

                return DataAccess.Instance.Execute(
                    "DELETE FROM [Label] WHERE Id = @Id", 
                    parameter);
            }
            return false;
        }

        /// <summary>
        /// This method retrieves all labels that meet the requirements
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IEnumerable<LabelModel> GetBy(string[] conditions, Dictionary<string, object> parameters)
        {
            List<LabelModel> generatedModels = new List<LabelModel>();

            // Query the database using the specified data
            SqlDataReader sqlDataReader = DataAccess.Instance.ExecuteAndRead("Label", conditions, parameters);

            while (sqlDataReader.Read())
            {
                // Generate a model for each row of the Labels table.
                generatedModels.Add(new Label
                {
                    Id = (int)sqlDataReader["Id"],
                    Title = (string)sqlDataReader["Title"],
                });
            }

            // We always have to close the sql connection, because it does not get closed otherwise.
            DataAccess.Instance.CloseSqlConnection();
            sqlDataReader.Close();

            return generatedModels.AsEnumerable();
        }

        /// <summary>
        /// Inserts a new label to the database
        /// </summary>
        /// <param name="toInsert"></param>
        /// <returns></returns>
        public bool Insert(LabelModel toInsert)
        {
            if (toInsert != null)
            {
                if (CheckIfDataIsCorrect(toInsert))
                {
                    if (string.IsNullOrEmpty(toInsert.Title))
                    {
                        toInsert.Title = SettingsConstant.DEFAULT_LABEL_TITLE;
                    }


                    Dictionary<string, object> parameters = GenerateQueryParameters(toInsert);

                    int id = DataAccess.Instance.ExecuteAndReturnId(
                        "INSERT INTO [Label] ([Title]) VALUES (@Title)",
                        parameters);

                    if (id != -1)
                    {
                        toInsert.Id = id;
                    }

                    return id != -1;
                }
            }
            return false;
        }

        /// <summary>
        /// This checks if the label is good to be inserted (if it is a valid label)
        /// </summary>
        /// <param name="labelModel"></param>
        /// <returns></returns>
        private static bool CheckIfDataIsCorrect(LabelModel labelModel) =>
            !string.IsNullOrEmpty(labelModel.Title);

        /// <summary>
        /// This updates a label
        /// </summary>
        /// <param name="toUpdate"></param>
        /// <returns></returns>
        public bool Update(LabelModel toUpdate)
        {
            if (toUpdate != null)
            {
                if (CheckIfDataIsCorrect(toUpdate))
                {
                    Dictionary<string, object> parameters = GenerateQueryParameters(toUpdate);
                    parameters.Add("@Id", toUpdate.Id);

                    return DataAccess.Instance.Execute(
                        "UPDATE [Label] SET [Title] = @Title WHERE Id = @Id",
                        parameters);
                }
            }
            return false;
        }
        
        /// <summary>
        /// Generate the query parameters from a label(Model)
        /// </summary>
        /// <param name="toExtractFrom"></param>
        /// <returns></returns>
        public Dictionary<string, object> GenerateQueryParameters(LabelModel toExtractFrom)
        {
            if (toExtractFrom != null)
            {
                return new Dictionary<string, object>
                {
                    { "@Title", toExtractFrom.Title }
                };
            }
            return null;
        }
    }
}