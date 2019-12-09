using Caliburn.Micro;
using EvernoteCloneLibrary.Users;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using System.Collections;

namespace EvernoteCloneGUI.ViewModels
{
    class LoginViewModel : Screen
    {
        #region Properties
        
        /// <value>
        /// This contains the email
        /// </value>
        public string EmailLogin { get; set; }
        
        /// <value>
        /// This contains the password
        /// </value>
        public string PasswordLogin { get; set; }

        /// <value>
        /// This contains the user object (null if not logged in)
        /// </value>
        public User user { get; private set; }
        
        /// <value>
        /// This contains the clientId which is used for the Google API
        /// </value>
        private const string _clientId = "IsSecret";
        
        /// <value>
        /// This contains the clientSecret which is also used for the Google API
        /// </value>
        private const string _clientSecret = "IsSecret";
        
        /// <value>
        /// The endpoint used for authorization (google)
        /// </value>
        private const string _authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";

        #endregion

        #region Register window
        
        /// <summary>
        /// Show register window when the button is clicked to register.
        /// </summary>
        public void Register()
        {
            IWindowManager windowManager = new WindowManager();

            RegisterViewModel registerViewModel = new RegisterViewModel();
            windowManager.ShowDialog(registerViewModel);
        }
        
        #endregion

        #region Handles login click event

        /// <summary>
        /// Passes username and password, which is encrypted. Checks if the credentials are valid. 
        /// If they are valid it shows a message that it was successful, else an error message.
        /// </summary>
        public void Login()
        {
            
            string usernameLogin = EmailLogin; 
            string passwordLogin = User.Encryption(PasswordLogin);
            user = (User)User.Login(usernameLogin, passwordLogin);

            if (user != null)
            {
                MessageBox.Show("You've been logged in with success!");
                (GetView() as Window)?.Close();
            }
            else 
            {
                MessageBox.Show("Password or Username is not correct. Please check again.");
            }
            
        }
        
        #endregion

        #region Get ports
        
        /// <summary>
        /// This method is used so that webpages can be served internally
        /// and can be run in multiple instance on the same machine
        /// </summary>
        /// <returns></returns>
        private static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        #endregion

        #region Request authorization
        
        /// <summary>
        /// Handles the full communication to get authorization with Google.
        /// </summary>
        /// <returns></returns>
        public async Task GoogleLoginAsync()
        {
            string state = RandomDataBase64Url(32);
            string codeVerifier = RandomDataBase64Url(32);
            string codeChallenge = Base64UrlencodeNoPadding(Sha256(codeVerifier));
            const string codeChallengeMethod = "S256";

            // Creates a redirect URI using an available port on the loopback address.
            string redirectUri = $"http://{IPAddress.Loopback}:{GetRandomUnusedPort()}/";
            Output("redirect URI: " + redirectUri);

            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectUri);
            Output("Listening..");
            http.Start();

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest =
                $"{_authorizationEndpoint}?response_type=code&scope=openid%20email%20profile&redirect_uri={System.Uri.EscapeDataString(redirectUri)}&client_id={_clientId}&state={state}&code_challenge={codeChallenge}&code_challenge_method={codeChallengeMethod}";

            // Opens request in the browser.
            System.Diagnostics.Process.Start(authorizationRequest);

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Brings this app back to the foreground.
            // TODO make this work!
            //this.Activate();

            // Sends an HTTP response to the browser.
            var response = context.Response;
            string responseString = string.Format("<html><head><meta http-equiv='refresh' content='10;url=https://google.com'></head><body>Please return to the app.</body></html>");
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var responseOutput = response.OutputStream;
            Task responseTask = responseOutput.WriteAsync(buffer, 0, buffer.Length).ContinueWith((task) =>
            {
                responseOutput.Close();
                http.Stop();
                Console.WriteLine(@"HTTP server stopped.");
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                Output($"OAuth authorization error: {context.Request.QueryString.Get("error")}.");
                return;
            }
            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                Output("Malformed authorization response. " + context.Request.QueryString);
                return;
            }

            // Extracts the code
            var code = context.Request.QueryString.Get("code");
            var incomingState = context.Request.QueryString.Get("state");

            // Compares the received state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incomingState != state)
            {
                Output($"Received request with invalid state ({incomingState})");
                return;
            }
            Output("Authorization code: " + code);

            // Starts the code exchange at the Token Endpoint.
            PerformCodeExchange(code, codeVerifier, redirectUri);

        }
        
        #endregion

        #region Exchange auth code for tokens
        
        /// <summary>
        /// Exchanges the authorization codes that are return to tokens
        /// Tokens are needed to make a call to a Google API
        /// </summary>
        /// <param name="code"></param>
        /// <param name="codeVerifier"></param>
        /// <param name="redirectUri"></param>
        async void PerformCodeExchange(string code, string codeVerifier, string redirectUri)
        {
            Output("Exchanging code for tokens...");

            // Setup token request
            string tokenRequestURI = "https://www.googleapis.com/oauth2/v4/token";
            string tokenRequestBody =
                $"code={code}&redirect_uri={System.Uri.EscapeDataString(redirectUri)}&client_id={_clientId}&code_verifier={codeVerifier}&client_secret={_clientSecret}&grant_type=authorization_code";

            // Sends the actually token request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(byteVersion, 0, byteVersion.Length);
            stream.Close();

            try
            {
                // Receive the token.
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream() ?? throw new NullReferenceException()))
                {
                    // Reads the output of the token.
                    string responseText = await reader.ReadToEndAsync();
                    Output(responseText);

                    //Converts the JSON output and puts this in a string dictionary.
                    Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    string accessToken = tokenEndpointDecoded["access_token"];
                    UserInfoCall(accessToken);
                }
            }
            
            //Handles error when tokens are not received.
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    if (ex.Response is HttpWebResponse response)
                    {
                        Output("HTTP: " + response.StatusCode);
                        using (StreamReader reader = new StreamReader(response.GetResponseStream() ?? throw new NullReferenceException()))
                        {
                            // reads response body
                            string responseText = await reader.ReadToEndAsync();
                            Output(responseText);
                        }
                    }

                }
            }
        }
        #endregion

        #region Make API call for Google user
        
        /// <summary>
        /// Makes the whole API call to get user information. You have to explicitly tell the application
        /// what user information you wanna receive. 
        /// </summary>
        /// <param name="accessToken"></param>
        private async void UserInfoCall(string accessToken)
        {
            // Local variables used to insert a new Google account if needed.
            ArrayList userData = new ArrayList();
            const bool isGoogleAccount = true;
            
            

            // Preparation to make the API call.
            string userInfoRequestUri = "https://www.googleapis.com/userinfo/v2/me";

            //Sends the API call and formats it.
            HttpWebRequest userInfoRequest = (HttpWebRequest)WebRequest.Create(userInfoRequestUri);
            userInfoRequest.Method = "GET";
            userInfoRequest.Headers.Add($"Authorization: Bearer {accessToken}");
            userInfoRequest.ContentType = "application/x-www-form-urlencoded";
            userInfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            //Receive the info that was requested
            WebResponse userInfoResponse = await userInfoRequest.GetResponseAsync();
            using (StreamReader userInfoResponseReader = new StreamReader(userInfoResponse.GetResponseStream() ?? throw new NullReferenceException()))
            {
                //Reads the full info and adds the details to a list. 
                string userInfoResponseText = await userInfoResponseReader.ReadToEndAsync();
                
                var userInfo = userInfoResponseText.Split('"');

                foreach(string k in userInfo)
                {
                    userData.Add(k);
                }

                // Getting specific user information from the list
                var arUserMail = userData[7];
                var arName = userData[13];
                var arLastName = userData[21];
                var arGoogleId = userData[3];

                //Converting information objects to string
                var username = Convert.ToString(arUserMail);
                var firstName = Convert.ToString(arName);
                var lastName = Convert.ToString(arLastName);
                var googleId = Convert.ToString(arGoogleId);
                var googlePassword = makeGooglePassword(googleId);
                googlePassword = User.Encryption(googlePassword);

                //Check if the Google user already is registered to the application or not.
                user = (User)User.Login(username,googlePassword);

                if (user == null)
                {
                    User.Register(username, googlePassword, firstName, lastName, isGoogleAccount);
                    user = (User)User.Login(username,googlePassword);
                }

                if (user != null)
                {
                    if (user.Id != -1)
                    {
                        MessageBox.Show("You've been logged in with a Google account.","NoteFever | Google login", MessageBoxButton.OK, MessageBoxImage.Information);
                        (GetView() as Window)?.Close();
                    }
                }
            }
        }

        #region Extra info
        /// <summary>
        /// Appends the given string to the on-screen log, and the debug console.
        /// </summary>
        public void Output(string output)
        {
            Console.WriteLine(output);
        }

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        public static string RandomDataBase64Url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return Base64UrlencodeNoPadding(bytes);
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        public static byte[] Sha256(string inputString)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputString);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        /// <summary>
        /// Base64url no-padding encodes the given input buffer.
        /// </summary>
        public static string Base64UrlencodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);

            // Converts base64 to base64url.
            base64 = base64.Replace("+", "-");
            base64 = base64.Replace("/", "_");
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }
        #endregion
        #endregion

        #region Make new password
        
        /// <summary>
        /// Receives unique google ID passes this through to make it the password
        /// for this account.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string makeGooglePassword (string password)
        {
           return password + "!@#45";
        }

        #endregion

        #region For local use only
        
        public void UseLocally()
        {
            user = new User
            {
                Id = -1
            };
            
            (GetView() as Window)?.Close();
        }
        
        #endregion
    }
}
