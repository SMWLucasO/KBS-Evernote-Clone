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
        /// <param name="Objects"></param>
        /// <returns></returns>
        private static bool ValidateMultiple(Func<object, bool> Validator, params object[] Objects)
        {
            bool result = false;
            
            foreach (object obj in Objects)
                result = Validator?.Invoke(obj) ?? false;
            return result;
        }

        /// <summary>
        /// Method which validates whether or not an array of objects is null
        /// </summary>
        /// <param name="Objects"></param>
        /// <returns></returns>
        public static bool AreNotNull(params object[] Objects) =>
            ValidateMultiple((obj) => obj != null, Objects);

        /// <summary>
        /// Method which validates where or not an array of strings is empty or null
        /// </summary>
        /// <param name="Strings"></param>
        /// <returns></returns>
        public static bool AreNullOrEmpty(params string[] Strings) =>
            ValidateMultiple((obj) => string.IsNullOrEmpty((string)obj), Strings);

        /// <summary>
        /// Check whether an object is null or not
        /// </summary>
        /// <param name="Object"></param>
        /// <returns></returns>
        public static bool IsNotNull(object Object)
            => Object != null;
    }
}
