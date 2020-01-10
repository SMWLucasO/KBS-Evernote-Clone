using EvernoteCloneLibrary.Constants;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Timers;

namespace EvernoteCloneLibrary.Database
{
    public class SshConnection
    {
        /// <summary>
        /// The Singleton class container.
        /// This is the variable that contains the only instance of this class.
        /// </summary>
        public static readonly SshConnection Instance = new SshConnection();

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
        private readonly Timer _keepSshAliveTimer = new Timer();

        /// <summary>
        /// We need a static constructor so that the C# compiler won't complain,
        /// <see href="https://csharpindepth.com/Articles/Singleton"> as stated in this article.</see>
        /// (Fourth version paragraph)
        /// </summary>
        static SshConnection() { }

        /// <summary>
        /// The SshConnection class needs to be private to block any other object from being created
        /// </summary>
        private SshConnection()
        {
            _keepSshAliveTimer.Interval = (Constant.TEST_MODE ? SshConstant.TEST_SSH_KEEP_ALIVE : SshConstant.SSH_KEEP_ALIVE);
            _keepSshAliveTimer.AutoReset = false;
            _keepSshAliveTimer.Elapsed += OnTimedEvent;
        }

        /// <summary>
        /// Create new ssh connection if not still connected
        /// </summary>
        /// <returns></returns>
        public uint GetSshPort()
        {
            if (!_keepSshAliveTimer.Enabled)
            {
                (_sshClient, _localPort) = ConnectSsh(
                    sshHostName: (Constant.TEST_MODE ?
                        SshConstant.TEST_SSH_HOST :
                        SshConstant.SSH_HOST),
                    sshUserName: (Constant.TEST_MODE ?
                        SshConstant.TEST_SSH_USERNAME :
                        SshConstant.SSH_USERNAME),
                    sshKeyFile: (Constant.TEST_MODE ?
                        (SshConstant.TEST_SSH_USE_PUBLIC_KEY ? SshConstant.TEST_SSH_KEY_PATH : null) :
                        (SshConstant.SSH_USE_PUBLIC_KEY ? SshConstant.SSH_KEY_PATH : null)),
                    sshPassPhrase: (Constant.TEST_MODE ?
                        (SshConstant.TEST_SSH_USE_PUBLIC_KEY ? SshConstant.TEST_SSH_KEY_PASSPHRASE : null) :
                        (SshConstant.SSH_USE_PUBLIC_KEY ? SshConstant.SSH_KEY_PASSPHRASE : null)),
                    sshPassword: (Constant.TEST_MODE ?
                        (SshConstant.TEST_SSH_USE_PUBLIC_KEY ? null : SshConstant.TEST_SSH_PASSWORD) :
                        (SshConstant.SSH_USE_PUBLIC_KEY ? null : SshConstant.SSH_PASSWORD)),
                    databaseServer: (Constant.TEST_MODE ?
                        DatabaseConstant.TEST_DATABASE_HOST :
                        DatabaseConstant.DATABASE_HOST),
                    databasePort: (Constant.TEST_MODE ?
                        DatabaseConstant.TEST_DATABASE_PORT :
                        DatabaseConstant.DATABASE_PORT));
            }
            else
                _keepSshAliveTimer.Stop();
            _keepSshAliveTimer.Start();

            return _localPort;
        }

        /// <summary>
        /// When this event is called, _sshClient is disconnected.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnTimedEvent(object source, ElapsedEventArgs e) =>
            _sshClient.Disconnect();

        /// <summary>
        /// Returns true of the _sshClient is still alive_
        /// </summary>
        /// <returns></returns>
        public static bool IsAlive() =>
            _sshClient.IsConnected;

        /// <summary>
        /// This function makes a connection to the ssh server and forwards a port so we can use the SqlServer
        /// <see href="https://mysqlconnector.net/tutorials/connect-ssh/">This function is copied from here</see>
        /// </summary>
        /// <param name="sshHostName">The hostname of the Ssh server</param>
        /// <param name="sshUserName">The username needed for authentication</param>
        /// <param name="sshPassword">The password needed for authentication</param>
        /// <param name="sshKeyFile">The sshKeyFile needed for authentication</param>
        /// <param name="sshPassPhrase">The passPhrase needed for authentication (when using a key file)</param>
        /// <param name="sshPort">The port the Ssh server listens to</param>
        /// <param name="databaseServer">The database we need to connect to (host name)</param>
        /// <param name="databasePort">The database port we need to connect to</param>
        /// <returns></returns>
        private static (SshClient SshClient, uint Port) ConnectSsh(string sshHostName, string sshUserName, string sshPassword = null,
            string sshKeyFile = null, string sshPassPhrase = null, int sshPort = 22, string databaseServer = "localhost", int databasePort = 3306)
        {
            // check arguments
            if (string.IsNullOrWhiteSpace(sshHostName))
            {
                throw new ArgumentException($"{nameof(sshHostName)} must be specified.", nameof(sshHostName));
            }

            if (string.IsNullOrWhiteSpace(sshHostName))
            {
                throw new ArgumentException($"{nameof(sshUserName)} must be specified.", nameof(sshUserName));
            }

            if (string.IsNullOrWhiteSpace(sshPassword) && string.IsNullOrWhiteSpace(sshKeyFile))
            {
                throw new ArgumentException($"One of {nameof(sshPassword)} and {nameof(sshKeyFile)} must be specified.");
            }

            if (string.IsNullOrWhiteSpace(databaseServer))
            {
                throw new ArgumentException($"{nameof(databaseServer)} must be specified.", nameof(databaseServer));
            }


            // define the authentication methods to use (in order)
            var authenticationMethods = new List<AuthenticationMethod>();
            if (!string.IsNullOrWhiteSpace(sshKeyFile))
            {
                authenticationMethods.Add(new PrivateKeyAuthenticationMethod(sshUserName,
                                    new PrivateKeyFile(sshKeyFile, string.IsNullOrWhiteSpace(sshPassPhrase) ? null : sshPassPhrase)));
            }

            if (!string.IsNullOrWhiteSpace(sshPassword))
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
