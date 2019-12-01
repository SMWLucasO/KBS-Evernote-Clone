using System.Collections.Generic;

namespace EvernoteCloneLibrary.Extensions
{
    public static class ListExtensions
    {
        public static bool AddIfNotPresent<T>(this List<T> list, T toAdd)
        {
            if (list.Contains(toAdd))
                return false;
            list.Add(toAdd);
            return true;
        }
    }   
}