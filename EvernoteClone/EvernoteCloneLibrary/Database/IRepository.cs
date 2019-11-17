using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="Parameters"></param>
        /// <returns></returns>
        IEnumerable<T> GetBy(string[] conditions, Dictionary<string, object> Parameters);

        /// <summary>
        /// A method signature used for inserting a new record into the table
        /// </summary>
        /// <param name="ToInsert"></param>
        /// <returns></returns>
        bool Insert(T ToInsert);
        
        /// <summary>
        /// A method signature used for updating an existing record 
        /// </summary>
        /// <param name="ToUpdate"></param>
        /// <returns></returns>
        bool Update(T ToUpdate);
        
        /// <summary>
        /// A method signature for deleting an existing record
        /// </summary>
        /// <param name="ToDelete"></param>
        /// <returns></returns>
        bool Delete(T ToDelete);

    }
}
