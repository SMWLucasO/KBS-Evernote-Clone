using System.Collections.Generic;

namespace EvernoteCloneLibrary.Extensions
{
    /// <summary>
    /// This class contains all Extension methods for Lists
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// This method only adds the given item when it does not already exist in the list
        /// </summary>
        /// <param name="list">The list the item should be added to</param>
        /// <param name="toAdd">The item that should be added to the list</param>
        /// <typeparam name="T">The type of the list (and item)</typeparam>
        /// <returns></returns>
        public static bool AddIfNotPresent<T>(this List<T> list, T toAdd)
        {
            if (list.Contains(toAdd))
            {
                return false;
            }

            list.Add(toAdd);
            return true;
        }
    }
}