namespace EvernoteCloneLibrary.Constants
{
    /// <summary>
    /// This class contains all the constants that relate to Database
    /// </summary>
    public class DatabaseConstant
    {
        #region Test

        public const string TEST_DATABASE_HOST = "127.0.0.1";
        public const int    TEST_DATABASE_PORT = 1433;
        public const string TEST_DATABASE_USERNAME = "sa";
        public const string TEST_DATABASE_PASSWORD = "Ikw1lmijnd@ta";
        public const string TEST_DATABASE_CATALOG = "NoteFever_EvernoteClone_Test";
        public const string TEST_DATABASE_INTEGRATED_SECURITY = "False";

        #endregion
        
        #region Production
        
        public const string DATABASE_HOST = "127.0.0.1";
        public const int    DATABASE_PORT = 1433;
        public const string DATABASE_USERNAME = "sa";
        public const string DATABASE_PASSWORD = "Ikw1lmijnd@ta";
        public const string DATABASE_CATALOG = "NoteFever_EvernoteClone";
        public const string DATABASE_INTEGRATED_SECURITY = "False";
        
        #endregion
    }
}