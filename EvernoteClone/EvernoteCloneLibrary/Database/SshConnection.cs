using EvernoteCloneLibrary.Constants;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EvernoteCloneLibrary.Database
{
    //TODO: remove everything in this class except the ConnectSsh method!
    public class SshConnection
    {
        /// <summary>
        /// The Singleton class container.
        /// This is the variable that contains the only instance of this class.
        /// </summary>
        private static SshConnection _instance;

        /// <summary>
        /// The Singleton accessor to this class.
        /// The only way to access this class'object is through here.
        /// </summary>
        /// <returns></returns>
        public static SshConnection Instance()
        {
            if (_instance == null)
                _instance = new SshConnection();
            return _instance;
        }

        /// <summary>
        /// The ongoing connection with the database
        /// </summary>
        private SqlConnection _connection;

        /// <summary>
        /// We need a static constructor so that the C# compiler won't complain,
        /// <see href="https://csharpindepth.com/Articles/Singleton"> as stated in this article.</see>
        /// (Fourth version paragraph)
        /// </summary>
        static SshConnection() { }

        /// <summary>
        /// The SshConnection class needs to be privae to block any other object from being created
        /// </summary>
        private SshConnection()
        {
            var (sshClient, localPort) = SshConnection.ConnectSsh(
                sshHostName: Constant.SSH_HOST,
                sshUserName: Constant.SSH_USERNAME,
                sshKeyFile: Constant.SSH_KEY_PATH,
                databaseServer: Constant.DATABASE_HOST,
                databasePort: Constant.DATABASE_PORT);

            using (sshClient)
            {
                string connectionString = $"" +
                    $"Server=tcp:{Constant.DATABASE_HOST},{localPort};" +
                    $"Database={Constant.DATABASE_CATALOG};" +
                    $"UID={Constant.DATABASE_USERNAME};" +
                    $"Password={Constant.DATABASE_PASSWORD};" +
                    $"Integrated Security={Constant.DATABASE_INTEGRATED_SECURITY}";

                using (_connection = new SqlConnection(connectionString))
                {
                    _connection.Open();
                }
            }
        }

        /// <summary>
        /// This function makes a connection to the ssh server and forwards a port so we can use the SqlServer
        /// <see href="https://mysqlconnector.net/tutorials/connect-ssh/">This function is copied from here</see>
        /// </summary>
        /// <param name="sshHostName"></param>
        /// <param name="sshUserName"></param>
        /// <param name="sshPassword"></param>
        /// <param name="sshKeyFile"></param>
        /// <param name="sshPassPhrase"></param>
        /// <param name="sshPort"></param>
        /// <param name="databaseServer"></param>
        /// <param name="databasePort"></param>
        /// <returns></returns>
        public static (SshClient SshClient, uint Port) ConnectSsh(string sshHostName, string sshUserName, string sshPassword = null,
            string sshKeyFile = null, string sshPassPhrase = null, int sshPort = 22, string databaseServer = "localhost", int databasePort = 3306)
        {
            // check arguments
            if (string.IsNullOrEmpty(sshHostName))
                throw new ArgumentException($"{nameof(sshHostName)} must be specified.", nameof(sshHostName));
            if (string.IsNullOrEmpty(sshHostName))
                throw new ArgumentException($"{nameof(sshUserName)} must be specified.", nameof(sshUserName));
            if (string.IsNullOrEmpty(sshPassword) && string.IsNullOrEmpty(sshKeyFile))
                throw new ArgumentException($"One of {nameof(sshPassword)} and {nameof(sshKeyFile)} must be specified.");
            if (string.IsNullOrEmpty(databaseServer))
                throw new ArgumentException($"{nameof(databaseServer)} must be specified.", nameof(databaseServer));

            // define the authentication methods to use (in order)
            var authenticationMethods = new List<AuthenticationMethod>();
            if (!string.IsNullOrEmpty(sshKeyFile))
            {
                authenticationMethods.Add(new PrivateKeyAuthenticationMethod(sshUserName,
                    new PrivateKeyFile(sshKeyFile, string.IsNullOrEmpty(sshPassPhrase) ? null : sshPassPhrase)));
            }
            if (!string.IsNullOrEmpty(sshPassword))
            {
                authenticationMethods.Add(new PasswordAuthenticationMethod(sshUserName, sshPassword));
            }

            // connect to the SSH server
            var sshClient = new SshClient(new ConnectionInfo(sshHostName, sshPort, sshUserName, authenticationMethods.ToArray()));
            sshClient.Connect();

            // forward a local port to the database server and port, using the SSH server
            var forwardedPort = new ForwardedPortLocal("127.0.0.1", databaseServer, (uint)databasePort);
            sshClient.AddForwardedPort(forwardedPort);
            forwardedPort.Start();

            return (sshClient, forwardedPort.BoundPort);
        }
    }
}
