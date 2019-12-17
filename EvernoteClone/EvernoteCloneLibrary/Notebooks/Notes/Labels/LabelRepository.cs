using EvernoteCloneLibrary.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebooks.Notes.Labels
{
    public class LabelRepository : IRepository<LabelModel>
    {
        public bool Delete(LabelModel toDelete)
        {
            if (toDelete != null)
            {
                Dictionary<string, object> parameter = new Dictionary<string, object>
                {
                    { "@Id", toDelete.Id }
                };

                return DataAccess.Instance.Execute("DELETE FROM [Label] WHERE Id = @Id", parameter);
            }
            return false;
        }

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

        public IEnumerable<LabelModel> GetBy(string[] conditions, Dictionary<string, object> parameters)
        {
            List<LabelModel> generatedModels = new List<LabelModel>();

            // Query the database using the specified data
            SqlDataReader sqlDataReader = DataAccess.Instance.ExecuteAndRead("Label", conditions, parameters);

            while (sqlDataReader.Read())
            {
                // Generate a model for each row of the Notes table.
                generatedModels.Add(new Label()
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

        public bool Insert(LabelModel toInsert)
        {
            if (toInsert != null)
            {
                if (CheckIfDataIsCorrect(toInsert))
                {
                    if (string.IsNullOrEmpty(toInsert.Title))
                    {
                        toInsert.Title = "Label";
                    }


                    Dictionary<string, object> parameters = GenerateQueryParameters(toInsert);

                    int id = DataAccess.Instance.ExecuteAndReturnId("INSERT INTO [Label] ([Title])"
                            + " VALUES (@Title)", parameters);

                    if (id != -1)
                    {
                        toInsert.Id = id;
                    }

                    return id != -1;
                }
            }
            return false;
        }

        private static bool CheckIfDataIsCorrect(LabelModel labelModel) =>
            !(string.IsNullOrEmpty(labelModel.Title));

        public bool Update(LabelModel toUpdate)
        {
            if (toUpdate != null)
            {
                if (CheckIfDataIsCorrect(toUpdate))
                {
                    Dictionary<string, object> parameters = GenerateQueryParameters(toUpdate);
                    parameters.Add("@Id", toUpdate.Id);

                    return DataAccess.Instance.Execute("UPDATE [Label] SET [Title] = @Title WHERE Id = @Id",
                        parameters);
                }
            }
            return false;
        }
    }
}
