using System.Collections.Generic;

namespace EvernoteCloneLibrary.Database
{
    /// <summary>
    /// This interface specifies everything which a repository should adhere to.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : IModel
    {
        
        /// <summary>
        /// A method signature for getting models by specified conditions
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IEnumerable<T> GetBy(string[] conditions, Dictionary<string, object> parameters);

        /// <summary>
        /// A method signature used for inserting a new record into the table
        /// </summary>
        /// <param name="toInsert"></param>
        /// <returns></returns>
        bool Insert(T toInsert);
        
        /// <summary>
        /// A method signature used for updating an existing record 
        /// </summary>
        /// <param name="toUpdate"></param>
        /// <returns></returns>
        bool Update(T toUpdate);
        
        /// <summary>
        /// A method signature for deleting an existing record
        /// </summary>
        /// <param name="toDelete"></param>
        /// <returns></returns>
        bool Delete(T toDelete);

        /// <summary>
        /// A helper method to generate the query parameters.
        /// </summary>
        /// <param name="toExtractFrom">The model which data will be extracted from</param>
        /// <returns></returns>
        Dictionary<string, object> GenerateQueryParameters(T toExtractFrom);
    }
}
