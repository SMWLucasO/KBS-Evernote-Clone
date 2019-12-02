namespace EvernoteCloneLibrary.Constants
{
    /// <summary>
    /// All constants.
    /// </summary>
    public static class Constant
    {
        // A determinant for whether we are in the test mode.
        public const bool TEST_MODE = true;

        // Storage path for local use (TEST mode)
        public const string TEST_STORAGE_PATH = "test/local";
        // Storage constants (TEST mode, ssh)
        public const bool TEST_SSH_USE_PUBLIC_KEY = false;
        public const string TEST_SSH_HOST = "145.44.234.54";
        public const string TEST_SSH_USERNAME = "student";
        public const string TEST_SSH_PASSWORD = "Ug5#ItC@14";
        public const string TEST_SSH_KEY_PATH = @"C:\git\KBS-Evernote-Clone\EvernoteClone\open-ssh";
        public const string TEST_SSH_KEY_PASSPHRASE = null;
        public const int TEST_SSH_KEEP_ALLIVE = 2000; // in milliseconds
        // Storage constants (TEST mode, database)
        public const string TEST_DATABASE_HOST = "127.0.0.1";
        public const int TEST_DATABASE_PORT = 1433;
        public const string TEST_DATABASE_USERNAME = "sa";
        public const string TEST_DATABASE_PASSWORD = "Ikw1lmijnd@ta";
        public const string TEST_DATABASE_CATALOG = "NoteFever_EvernoteClone_Test";
        public const string TEST_DATABASE_INTEGRATED_SECURITY = "False";

        // Storage path for local use (PRODUCTION mode)
        public const string PRODUCTION_STORAGE_PATH = "production/local";
        // Storage constants (PRODUCTION mode, ssh)
        public const bool SSH_USE_PUBLIC_KEY = false;
        public const string SSH_HOST = "145.44.234.54";
        public const string SSH_USERNAME = "student";
        public const string SSH_PASSWORD = "Ug5#ItC@14";
        public const string SSH_KEY_PATH = @"C:\git\KBS-Evernote-Clone\EvernoteClone\open-ssh";
        public const string SSH_KEY_PASSPHRASE = null;
        public const int SSH_KEEP_ALLIVE = 2000; // in milliseconds
        // Storage constants (PRODUCTION mode, database)
        public const string DATABASE_HOST = "127.0.0.1";
        public const int DATABASE_PORT = 1433;
        public const string DATABASE_USERNAME = "sa";
        public const string DATABASE_PASSWORD = "Ikw1lmijnd@ta";
        public const string DATABASE_CATALOG = "NoteFever_EvernoteClone";
        public const string DATABASE_INTEGRATED_SECURITY = "False";
    }
}
