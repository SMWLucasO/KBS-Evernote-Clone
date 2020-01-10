namespace EvernoteCloneLibrary.Constants
{
    /// <summary>
    /// This class contains all info needed for Google authentication
    /// </summary>
    public class GoogleConstant
    {
        /// <value>
        /// This contains the clientId which is used for the Google API
        /// </value>
        public const string CLIENT_ID = "299435995922-pgpo06947h7aij34klp5g5l0ktku26tv.apps.googleusercontent.com";
        
        /// <value>
        /// This contains the clientSecret which is also used for the Google API
        /// </value>
        public const string CLIENT_SECRET = "c-ZFoxmjPXFcXUI9sXpAzI8N";
        
        /// <value>
        /// The endpoint used for authorization (google)
        /// </value>
        public const string AUTHORIZATION_ENDPOINT = "https://accounts.google.com/o/oauth2/v2/auth";
        
        /// <value>
        /// This contains a string that is the relative path were all logs should be stored
        /// <username> is replaced with the currently logged in user
        /// </value>
        public const string TEST_LOG_STORAGE_PATH = "test/local/logs/google";
        
        /// <value>
        /// This contains a string that is the relative path were all logs should be stored
        /// <username> is replaced with the currently logged in user
        /// </value>
        public const string PRODUCTION_LOG_STORAGE_PATH = "production/local/logs/google";
    }
}