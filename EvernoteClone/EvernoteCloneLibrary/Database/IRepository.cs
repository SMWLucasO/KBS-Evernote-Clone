using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Database
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : IModel
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        IEnumerable<T> GetBy(string[] conditions, Dictionary<string, object> Parameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToInsert"></param>
        /// <returns></returns>
        bool Insert(T ToInsert);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToUpdate"></param>
        /// <returns></returns>
        bool Update(T ToUpdate);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToDelete"></param>
        /// <returns></returns>
        bool Delete(T ToDelete);

    }
}
