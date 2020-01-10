namespace EvernoteCloneLibrary.Constants
{
    /// <summary>
    /// This class contains all the constants that needed to make a Ssh connection
    /// </summary>
    public class SshConstant
    {
        #region Test
        
        public const bool   TEST_SSH_USE_PUBLIC_KEY = false;
        public const string TEST_SSH_HOST = "145.44.234.54";
        public const string TEST_SSH_USERNAME = "student";
        public const string TEST_SSH_PASSWORD = "Ug5#ItC@14";
        public const string TEST_SSH_KEY_PATH = @"C:\git\KBS-Evernote-Clone\EvernoteClone\open-ssh";
        public const string TEST_SSH_KEY_PASSPHRASE = null;
        public const int    TEST_SSH_KEEP_ALIVE = 2000; // in milliseconds
        
        #endregion
        
        #region Production
        
        public const bool   SSH_USE_PUBLIC_KEY = false;
        public const string SSH_HOST = "145.44.234.54";
        public const string SSH_USERNAME = "student";
        public const string SSH_PASSWORD = "Ug5#ItC@14";
        public const string SSH_KEY_PATH = @"C:\git\KBS-Evernote-Clone\EvernoteClone\open-ssh";
        public const string SSH_KEY_PASSPHRASE = null;
        public const int    SSH_KEEP_ALIVE = 2000; // in milliseconds
        
        #endregion
    }
}