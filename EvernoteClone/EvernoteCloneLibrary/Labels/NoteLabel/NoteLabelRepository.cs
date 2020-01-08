using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary.Labels.NoteLabel
{
    /// <summary>
    /// This class handles all communication with the database
    /// </summary>
    public class NoteLabelRepository : IRepository<NoteLabelModel>
    {
        /// <summary>
        /// The method for inserting a LocationUser record, where the class members are columns and the class member values are column values.
        /// </summary>
        /// <param name="toInsert">The model to be inserted into the table</param>
        /// <returns>bool to determine if the note was inserted</returns>
        public bool Insert(NoteLabelModel toInsert)
        {
            if (toInsert != null)
            {
                Dictionary<string, object> parameters = GenerateQueryParameters(toInsert);
                
                // Check if the path already exists, if so, set toInsert.Id equal to the already existing NotebookLocation id
                List<NoteLabelModel> noteLabels = 
                    GetBy(new[] {"NoteID = @NoteID", "LabelID = @LabelID"}, parameters).ToList();

                if (noteLabels.Count > 0)
                {
                    return true;
                }

                return DataAccess.Instance.Execute(
                    "INSERT INTO [NoteLabel] ([NoteID], [LabelID]) VALUES (@NoteID, @LabelID)",
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
        public IEnumerable<NoteLabelModel> GetBy(string[] conditions, Dictionary<string, object> parameters)
        {
            List<NoteLabelModel> generatedModels = new List<NoteLabelModel>();

            // Query the database using the specified data
            SqlDataReader sqlDataReader = DataAccess.Instance.ExecuteAndRead("NoteLabel", conditions, parameters);

            while (sqlDataReader.Read())
            {
                // Generate a model for each row of the NoteLabel table.
                generatedModels.Add(new NoteLabelModel
                { 
                    NoteId = (int)sqlDataReader["NoteID"],
                    LabelId = (int)sqlDataReader["LabelID"]
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
        /// <param name="toUpdate">The model which is to be updated</param>
        /// <returns>bool to determine if the update was a success (will always be false)</returns>
        public bool Update(NoteLabelModel toUpdate) => 
            false;

        /// <summary>
        /// The method for deleting the NoteLabel record which the specified model represents.
        /// </summary>
        /// <param name="toDelete"></param>
        /// <returns>A boolean indicating whether it was deleted with success (true) or not (false)</returns>
        public bool Delete(NoteLabelModel toDelete)
        {
            if (toDelete != null)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@NoteID", toDelete.NoteId },
                    { "@LabelID", toDelete.LabelId }
                };

                return DataAccess.Instance.Execute(
                    "DELETE FROM [NoteLabel] WHERE NoteID = @NoteID AND LabelID = @LabelID",
                    parameters);
            }

            return false;
        }

        /// <summary>
        /// A helper method to generate the query parameters.
        /// </summary>
        /// <param name="toExtractFrom">The NoteLabelModel which data will be extracted from</param>
        /// <returns>The generated parameters for the query to be executed</returns>
        public Dictionary<string, object> GenerateQueryParameters(NoteLabelModel toExtractFrom)
        {
            if (toExtractFrom != null)
            {
                return new Dictionary<string, object> 
                {
                    { "@NoteID", toExtractFrom.NoteId },
                    { "@LabelID", toExtractFrom.LabelId }
                };
            }

            return null;
        }
    }
}