using EvernoteCloneLibrary.Constants;

namespace EvernoteCloneLibrary
{
    /// <summary>
    /// This class contains some general static methods that are used on multiple occasions and can't really get their own class.
    /// </summary>
    public static class StaticMethods
    {
        /// <summary>
        /// Returns the storage path for all data
        /// </summary>
        /// <returns>string</returns>
        public static string GetUserDataStoragePath()
        {
            string path = Constant.TEST_MODE ? Constant.TEST_USERDATA_STORAGE_PATH : Constant.PRODUCTION_USERDATA_STORAGE_PATH;
            string[] splittedPath = path.Split('<', '>');

            if (splittedPath.Length == 3)
            {
                splittedPath[1] = Constant.User.Username;

                return splittedPath[0] + splittedPath[1] + splittedPath[2];
            }

            return null;
        }
        
        /// <summary>
        /// Get the storage path for saving notes and notebooks locally.
        /// </summary>
        /// <returns></returns>
        public static string GetNotebookStoragePath()
        {
            string path = Constant.TEST_MODE ? Constant.TEST_NOTEBOOK_STORAGE_PATH : Constant.PRODUCTION_NOTEBOOK_STORAGE_PATH;
            string[] splittedPath = path.Split('<', '>');

            if (splittedPath.Length == 3)
            {
                splittedPath[1] = Constant.User.Username;

                return splittedPath[0] + splittedPath[1] + splittedPath[2];
            }

            return null;
        }
    }
}