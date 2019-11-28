using EvernoteCloneLibrary.Constants;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Timers;

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
        /// A variable that contains a ongoing connection with the ssh server.
        /// </summary>
        private static SshClient _sshClient;

        /// <summary>
        /// A variable that contains the forwarded port from the ssh server (used for databaseconnection)
        /// </summary>
        private static uint _localPort;

        /// <summary>
        /// This timer is used to close the _sshClient after a certain amount of seconds
        /// </summary>
        private Timer _keepSSHAlliveTimer = new Timer();

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
            _keepSSHAlliveTimer.Interval = (Constant.TEST_MODE ? Constant.TEST_SSH_KEEP_ALLIVE : Constant.SSH_KEEP_ALLIVE);
            _keepSSHAlliveTimer.AutoReset = false;
            _keepSSHAlliveTimer.Elapsed += OnTimedEvent;
        }

        /// <summary>
        /// Create new ssh connection if not still connected
        /// </summary>
        /// <returns></returns>
        public uint GetSshPort()
        {
            if (!_keepSSHAlliveTimer.Enabled)
            {
                (_sshClient, _localPort) = ConnectSsh(
                    sshHostName: (Constant.TEST_MODE ? Constant.TEST_SSH_HOST : Constant.SSH_HOST),
                    sshUserName: (Constant.TEST_MODE ? Constant.TEST_SSH_USERNAME : Constant.SSH_USERNAME),
                    sshKeyFile: (Constant.TEST_MODE ?
                        (Constant.TEST_SSH_USE_PUBLIC_KEY ? Constant.TEST_SSH_KEY_PATH : null) :
                        (Constant.SSH_USE_PUBLIC_KEY ? Constant.SSH_KEY_PATH : null)),
                    sshPassPhrase: (Constant.TEST_MODE ?
                        (Constant.TEST_SSH_USE_PUBLIC_KEY ? Constant.TEST_SSH_KEY_PASSPHRASE : null) :
                        (Constant.SSH_USE_PUBLIC_KEY ? Constant.SSH_KEY_PASSPHRASE : null)),
                    sshPassword: (Constant.TEST_MODE ?
                        (Constant.TEST_SSH_USE_PUBLIC_KEY ? null : Constant.TEST_SSH_PASSWORD) :
                        (Constant.SSH_USE_PUBLIC_KEY ? null : Constant.SSH_PASSWORD)),
                    databaseServer: (Constant.TEST_MODE ? Constant.TEST_DATABASE_HOST : Constant.DATABASE_HOST),
                    databasePort: (Constant.TEST_MODE ? Constant.TEST_DATABASE_PORT : Constant.DATABASE_PORT));
            }
            else
                _keepSSHAlliveTimer.Stop();

            _keepSSHAlliveTimer.Start();

            return _localPort;
        }

        /// <summary>
        /// When this event is called, _sshClient is disconnected.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnTimedEvent(object source, ElapsedEventArgs e) => _sshClient.Disconnect();

        /// <summary>
        /// Returns true of the _sshClient is still allive_
        /// </summary>
        /// <returns></returns>
        public static bool IsAllive() => _sshClient.IsConnected;

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
