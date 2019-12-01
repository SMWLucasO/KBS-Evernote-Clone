namespace EvernoteCloneLibrary.Constants
{
    /// <summary>
    /// All constants.
    /// </summary>
    public static class Constant
    {
        // A determinant for whether we are in the test mode.
        public const bool   TEST_MODE = true;

        // Storage path for local use (TEST mode)
        public const string TEST_NOTEBOOK_STORAGE_PATH = "test/local/notebooks";
        public const string TEST_USERDATA_STORAGE_PATH = "test/local/userdata";
        // Storage constants (TEST mode, ssh)
        public const bool   TEST_SSH_USE_PUBLIC_KEY = false;
        public const string TEST_SSH_HOST = "127.0.0.1";
        public const string TEST_SSH_USERNAME = "";
        public const string TEST_SSH_PASSWORD = "";
        public const string TEST_SSH_KEY_PATH = @"";
        public const string TEST_SSH_KEY_PASSPHRASE = null;
        public const int    TEST_SSH_KEEP_ALLIVE = 2000; // in milliseconds
        // Storage constants (TEST mode, database)
        public const string TEST_DATABASE_HOST = "127.0.0.1";
        public const int    TEST_DATABASE_PORT = 1433;
        public const string TEST_DATABASE_USERNAME = "";
        public const string TEST_DATABASE_PASSWORD = "";
        public const string TEST_DATABASE_CATALOG = "";
        public const string TEST_DATABASE_INTEGRATED_SECURITY = "False";

        // Storage path for local use (PRODUCTION mode)
        public const string PRODUCTION_NOTEBOOK_STORAGE_PATH = "production/local";
        public const string PRODUCTION_USERDATA_STORAGE_PATH = "production/local/userdata";
        // Storage constants (PRODUCTION mode, ssh)
        public const bool   SSH_USE_PUBLIC_KEY = false;
        public const string SSH_HOST = "";
        public const string SSH_USERNAME = "";
        public const string SSH_PASSWORD = "";
        public const string SSH_KEY_PATH = @"";
        public const string SSH_KEY_PASSPHRASE = null;
        public const int    SSH_KEEP_ALLIVE = 2000; // in milliseconds
        // Storage constants (PRODUCTION mode, database)
        public const string DATABASE_HOST = "127.0.0.1";
        public const int    DATABASE_PORT = 1433;
        public const string DATABASE_USERNAME = "";
        public const string DATABASE_PASSWORD = "";
        public const string DATABASE_CATALOG = "";
        public const string DATABASE_INTEGRATED_SECURITY = "False";
    }
}
