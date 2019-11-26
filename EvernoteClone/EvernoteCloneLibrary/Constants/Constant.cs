namespace EvernoteCloneLibrary.Constants
{
    /// <summary>
    /// All constants.
    /// </summary>
    public static class Constant
    {
        // A determinant for whether we are in the test mode.
        public const bool TEST_MODE = true;

        // Storage constants (TEST mode)
        public const string TEST_CONNECTION_STRING = "Data Source=.;Initial Catalog=NoteFever_EvernoteClone;Integrated Security=True";
        public const string TEST_STORAGE_PATH = "tests/local/";

        // Storage constants (PRODUCTION mode, ssh)
        public const bool   SSH_USE_PUBLIC_KEY = true;
        public const string SSH_HOST = "145.44.234.54";
        public const string SSH_USERNAME = "student";
        public const string SSH_PASSWORD = "";
        public const string SSH_KEY_PATH = @"C:\git\KBS-Evernote-Clone\EvernoteClone\open-ssh";

        // Storage constants (PRODUCTION mode, database)
        public const string DATABASE_HOST = "127.0.0.1";
        public const int    DATABASE_PORT = 1433;
        public const string DATABASE_USERNAME = "ever";
        public const string DATABASE_PASSWORD = "";
        public const string DATABASE_CATALOG = "NoteFever_EvernoteClone";
        public const string DATABASE_INTEGRATED_SECURITY = "False";
    }
}
