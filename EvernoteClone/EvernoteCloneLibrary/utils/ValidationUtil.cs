using System;

namespace EvernoteCloneLibrary.Utils
{
    /// <summary>
    /// An util class for all validation methods
    /// </summary>
    public static class ValidationUtil
    {
        /// <summary>
        /// Method to validate multiple objects with a specified validator
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        private static bool ValidateMultiple(Func<object, bool> validator, params object[] objects)
        {
            bool result = true;

            foreach (object obj in objects)
            {
                bool outputResult = validator?.Invoke(obj) ?? false;
                
                if (!outputResult)
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Method which validates whether or not an array of objects is null
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static bool AreNotNull(params object[] objects) =>
            ValidateMultiple(obj => obj != null, objects);

        /// <summary>
        /// Method which validates where or not an array of strings is empty or null
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static bool AreNullOrEmpty(params string[] strings) =>
            ValidateMultiple(obj => string.IsNullOrEmpty((string)obj), strings);
        
        /// <summary>
        /// Method which validates where or not an array of strings is whitespace or null or empty
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        public static bool AreNullOrWhiteSpace(params string[] strings) =>
            ValidateMultiple(obj => string.IsNullOrWhiteSpace((string)obj), strings);

        /// <summary>
        /// Check whether an object is null or not
        /// </summary>
        /// <param name="specifiedObject"></param>
        /// <returns></returns>
        public static bool IsNotNull(object specifiedObject)
            => specifiedObject != null;
    }
}
