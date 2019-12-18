using EvernoteCloneLibrary.Users;

namespace EvernoteCloneLibrary.Constants
{
    /// <summary>
    /// All constants.
    /// </summary>
    public static class Constant
    {
        /// <value>
        /// This is used as an indication if the application is in test mode
        /// </value>
        public const bool   TEST_MODE = true;

        /// <value>
        /// This contains the path to the test cases storage
        /// </value>
        public const string TESTS_STORAGE_PATH = "test/local/testcases";

        /// <value>
        /// This contains a string that is the relative path were all notebook data should be stored
        /// <username> is replaced with the currently logged in user
        /// </value>
        public const string TEST_NOTEBOOK_STORAGE_PATH = "test/local/<username>/notebooks";

        /// <value>
        /// This contains a string that is the relative path were all user data should be stored
        /// <username> is replaced with the currently logged in user
        /// </value>
        public const string TEST_USERDATA_STORAGE_PATH = "test/local/<username>/userdata";

        /// <value>
        /// This contains a string that is the relative path were all notebook data should be stored
        /// <username> is replaced with the currently logged in user
        /// </value>
        public const string PRODUCTION_NOTEBOOK_STORAGE_PATH = "production/local/<username>/notebooks";
        
        /// <value>
        /// This contains a string that is the relative path were all user data should be stored
        /// <username> is replaced with the currently logged in user
        /// </value>
        public const string PRODUCTION_USERDATA_STORAGE_PATH = "production/local/<username>/userdata";

        /// <value>
        /// This contains a User object. This represents the logged in user
        /// We get the username, id and such from this object.
        /// </value>
        public static User User;
    }
}
