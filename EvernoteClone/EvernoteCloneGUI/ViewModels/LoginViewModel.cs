using Caliburn.Micro;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Notes;
using EvernoteCloneLibrary.Users;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Google.Apis;
using System.Collections;


namespace EvernoteCloneGUI.ViewModels
{
    class LoginViewModel : Window
    {
        private string emailLogin;
        private string passwordLog;
        public User user=null;


        #region Register window
        /// <summary>
        /// Show register window when the button is clicked to register.
        /// </summary>
        public void Register()
        {
            IWindowManager windowManager = new WindowManager();

            RegisterViewModel registerViewModel = new RegisterViewModel();
            windowManager.ShowDialog(registerViewModel, null);
            
        }
        #endregion

        #region Properties for password and email

        /// <summary>
        /// Properties that are used to check whether someone can login with their credentials.
        /// </summary>
        public string EmailLogin
        {
            get { return emailLogin; }
            set { emailLogin = value; }
        }

        
        public string PasswordLogin
        {
            get { return passwordLog; }
            set { passwordLog = value; }
        }

        #endregion

        #region Handles login click event

        /// <summary>
        /// Passess username and password, which is encrypted. Checks if the credentials are valid. 
        /// If they are valid it shows a message that it was successful, else an error message.
        /// </summary>
        public void Login()
        {
            
            string usernameLogin = EmailLogin; 
            string passwordLogin = User.Encryption(PasswordLogin.ToString());
            user = (User)User.Login(usernameLogin, passwordLogin);

            if (user != null)
            {
                MessageBox.Show("Succes!");
                
            }
            else 
            {
                MessageBox.Show("Failed!");

            }
            
        }
        #endregion

        #region Local variables
        /// <summary>
        /// Local variables needed to connect to client for Google Oauth2.0.
        /// </summary>
        const string clientID = "IsSecret";
        const string clientSecret = "IsSecret";
        const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";
        #endregion

        #region Get ports
        /// <summary>
        /// This method is used so that webpages can be served internally
        /// and can be run in multiple instance on the same machine
        /// </summary>
        /// <returns></returns>
        public static int GetRandomUnusedPort()
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
        /// Handles the full communcation to get authorization with Google.
        /// </summary>
        /// <returns></returns>
        public async Task GoogleLoginAsync()
        {
            string state = randomDataBase64url(32);
            string code_verifier = randomDataBase64url(32);
            string code_challenge = base64urlencodeNoPadding(sha256(code_verifier));
            const string code_challenge_method = "S256";

            // Creates a redirect URI using an available port on the loopback address.
            string redirectURI = string.Format("http://{0}:{1}/", IPAddress.Loopback, GetRandomUnusedPort());
            output("redirect URI: " + redirectURI);

            // Creates an HttpListener to listen for requests on that redirect URI.
            var http = new HttpListener();
            http.Prefixes.Add(redirectURI);
            output("Listening..");
            http.Start();

            // Creates the OAuth 2.0 authorization request.
            string authorizationRequest = string.Format("{0}?response_type=code&scope=openid%20email%20profile&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                authorizationEndpoint,
                System.Uri.EscapeDataString(redirectURI),
                clientID,
                state,
                code_challenge,
                code_challenge_method);

            // Opens request in the browser.
            System.Diagnostics.Process.Start(authorizationRequest);

            // Waits for the OAuth authorization response.
            var context = await http.GetContextAsync();

            // Brings this app back to the foreground.
            this.Activate();

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
                Console.WriteLine("HTTP server stopped.");
            });

            // Checks for errors.
            if (context.Request.QueryString.Get("error") != null)
            {
                output(String.Format("OAuth authorization error: {0}.", context.Request.QueryString.Get("error")));
                return;
            }
            if (context.Request.QueryString.Get("code") == null
                || context.Request.QueryString.Get("state") == null)
            {
                output("Malformed authorization response. " + context.Request.QueryString);
                return;
            }

            // Extracts the code
            var code = context.Request.QueryString.Get("code");
            var incoming_state = context.Request.QueryString.Get("state");

            // Compares the received state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incoming_state != state)
            {
                output(String.Format("Received request with invalid state ({0})", incoming_state));
                return;
            }
            output("Authorization code: " + code);

            // Starts the code exchange at the Token Endpoint.
            performCodeExchange(code, code_verifier, redirectURI);

        }
        #endregion

        #region Exchange auth code for tokens
        /// <summary>
        /// Exchanges the authorization codes that are return to tokens
        /// Tokens are needed to make a call to a Google API
        /// </summary>
        /// <param name="code"></param>
        /// <param name="code_verifier"></param>
        /// <param name="redirectURI"></param>
        async void performCodeExchange(string code, string code_verifier, string redirectURI)
        {
            output("Exchanging code for tokens...");

            // Setup token request
            string tokenRequestURI = "https://www.googleapis.com/oauth2/v4/token";
            string tokenRequestBody = string.Format("code={0}&redirect_uri={1}&client_id={2}&code_verifier={3}&client_secret={4}&grant_type=authorization_code",
                code,
                System.Uri.EscapeDataString(redirectURI),
                clientID,
                code_verifier,
                clientSecret
                );

            // Sends the actually token request
            HttpWebRequest tokenRequest = (HttpWebRequest)WebRequest.Create(tokenRequestURI);
            tokenRequest.Method = "POST";
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            tokenRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            byte[] _byteVersion = Encoding.ASCII.GetBytes(tokenRequestBody);
            tokenRequest.ContentLength = _byteVersion.Length;
            Stream stream = tokenRequest.GetRequestStream();
            await stream.WriteAsync(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            try
            {
                // Receive the token.
                WebResponse tokenResponse = await tokenRequest.GetResponseAsync();
                using (StreamReader reader = new StreamReader(tokenResponse.GetResponseStream()))
                {
                    // Reads the output of the token.
                    string responseText = await reader.ReadToEndAsync();
                    output(responseText);

                    //Converts the JSON output and puts this in a string dictionary.
                    Dictionary<string, string> tokenEndpointDecoded = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseText);

                    string access_token = tokenEndpointDecoded["access_token"];
                    userinfoCall(access_token);
                }
            }
            //Handles error when tokens are not received.
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null)
                    {
                        output("HTTP: " + response.StatusCode);
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            // reads response body
                            string responseText = await reader.ReadToEndAsync();
                            output(responseText);
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
        /// <param name="access_token"></param>
        async void userinfoCall(string access_token)
        {
            //Local variables used to insert a new Google account if needed.
            string[] userInfo;
            ArrayList userData = new ArrayList();
            string username;
            string googleId;
            string firstName;
            string lastName;
            string googlePassword;
            bool isGoogleAccount = true;
            
            

            //Prepartion to make the API call.
            string userinfoRequestURI = "https://www.googleapis.com/userinfo/v2/me";

            //Sends the API call and formats it.
            HttpWebRequest userinfoRequest = (HttpWebRequest)WebRequest.Create(userinfoRequestURI);
            userinfoRequest.Method = "GET";
            userinfoRequest.Headers.Add(string.Format("Authorization: Bearer {0}", access_token));
            userinfoRequest.ContentType = "application/x-www-form-urlencoded";
            userinfoRequest.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            //Receive the info that was requested
            WebResponse userinfoResponse = await userinfoRequest.GetResponseAsync();
            using (StreamReader userinfoResponseReader = new StreamReader(userinfoResponse.GetResponseStream()))
            {
                //Reads the full info and adds the details to a list. 
                string userinfoResponseText = await userinfoResponseReader.ReadToEndAsync();

                char test = '"';
                userInfo = userinfoResponseText.Split(test);

                foreach(string k in userInfo)
                {
                    userData.Add(k);
                }

                //Getting specific user information from the list
                var arUserMail = userData[7];
                var arName = userData[13];
                var arLastName = userData[21];
                var arGoogleId = userData[3];

                //Converting information objects to string
                username = Convert.ToString(arUserMail);
                firstName = Convert.ToString(arName);
                lastName = Convert.ToString(arLastName);
                googleId = Convert.ToString(arGoogleId);
                googlePassword = makeGooglePassword(googleId);
                googlePassword = User.Encryption(googlePassword);

                //Check if the Google user already is registeren to the application or not.
                user = (User)User.Login(username,googlePassword);

                if (user == null)
                {
                    User.Register(username, googlePassword, firstName, lastName, isGoogleAccount);
                    user = (User)User.Login(username,googlePassword);
                }
            }
        }

        #region Extra info
        /// <summary>
        /// Appends the given string to the on-screen log, and the debug console.
        /// </summary>
        public void output(string output)
        {
            //textBoxOutput.Text = textBoxOutput.Text + output + Environment.NewLine;
            Console.WriteLine(output);
        }

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        public static string randomDataBase64url(uint length)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);
            return base64urlencodeNoPadding(bytes);
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string.
        /// </summary>
        public static byte[] sha256(string inputStirng)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(inputStirng);
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(bytes);
        }

        /// <summary>
        /// Base64url no-padding encodes the given input buffer.
        /// </summary>
        public static string base64urlencodeNoPadding(byte[] buffer)
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
    }
}
