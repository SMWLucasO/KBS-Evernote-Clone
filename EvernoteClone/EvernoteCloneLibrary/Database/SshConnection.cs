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
        /// The SshConnection class needs to be privae to block any other object from being created
        /// </summary>
        private SshConnection()
        {
            _keepSshAliveTimer.Interval = (Constant.TEST_MODE ? Constant.TEST_SSH_KEEP_ALLIVE : Constant.SSH_KEEP_ALLIVE);
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
                    SshHostName: (Constant.TEST_MODE ? Constant.TEST_SSH_HOST : Constant.SSH_HOST),
                    SshUserName: (Constant.TEST_MODE ? Constant.TEST_SSH_USERNAME : Constant.SSH_USERNAME),
                    SshKeyFile: (Constant.TEST_MODE ?
                        (Constant.TEST_SSH_USE_PUBLIC_KEY ? Constant.TEST_SSH_KEY_PATH : null) :
                        (Constant.SSH_USE_PUBLIC_KEY ? Constant.SSH_KEY_PATH : null)),
                    SshPassPhrase: (Constant.TEST_MODE ?
                        (Constant.TEST_SSH_USE_PUBLIC_KEY ? Constant.TEST_SSH_KEY_PASSPHRASE : null) :
                        (Constant.SSH_USE_PUBLIC_KEY ? Constant.SSH_KEY_PASSPHRASE : null)),
                    SshPassword: (Constant.TEST_MODE ?
                        (Constant.TEST_SSH_USE_PUBLIC_KEY ? null : Constant.TEST_SSH_PASSWORD) :
                        (Constant.SSH_USE_PUBLIC_KEY ? null : Constant.SSH_PASSWORD)),
                    DatabaseServer: (Constant.TEST_MODE ? Constant.TEST_DATABASE_HOST : Constant.DATABASE_HOST),
                    DatabasePort: (Constant.TEST_MODE ? Constant.TEST_DATABASE_PORT : Constant.DATABASE_PORT));
            }
            else
                _keepSshAliveTimer.Stop();
            _keepSshAliveTimer.Start();

            return _localPort;
        }

        /// <summary>
        /// When this event is called, _sshClient is disconnected.
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Event"></param>
        private static void OnTimedEvent(object Source, ElapsedEventArgs Event) => 
            _sshClient.Disconnect();

        /// <summary>
        /// Returns true of the _sshClient is still allive_
        /// </summary>
        /// <returns></returns>
        public static bool IsAlive() => 
            _sshClient.IsConnected;

        /// <summary>
        /// This function makes a connection to the ssh server and forwards a port so we can use the SqlServer
        /// <see href="https://mysqlconnector.net/tutorials/connect-ssh/">This function is copied from here</see>
        /// </summary>
        /// <param name="SshHostName"></param>
        /// <param name="SshUserName"></param>
        /// <param name="SshPassword"></param>
        /// <param name="SshKeyFile"></param>
        /// <param name="SshPassPhrase"></param>
        /// <param name="SshPort"></param>
        /// <param name="DatabaseServer"></param>
        /// <param name="DatabasePort"></param>
        /// <returns></returns>
        public static (SshClient SshClient, uint Port) ConnectSsh(string SshHostName, string SshUserName, string SshPassword = null,
            string SshKeyFile = null, string SshPassPhrase = null, int SshPort = 22, string DatabaseServer = "localhost", int DatabasePort = 3306)
        {
            // check arguments
            if (string.IsNullOrEmpty(SshHostName))
                throw new ArgumentException($"{nameof(SshHostName)} must be specified.", nameof(SshHostName));
            if (string.IsNullOrEmpty(SshHostName))
                throw new ArgumentException($"{nameof(SshUserName)} must be specified.", nameof(SshUserName));
            if (string.IsNullOrEmpty(SshPassword) && string.IsNullOrEmpty(SshKeyFile))
                throw new ArgumentException($"One of {nameof(SshPassword)} and {nameof(SshKeyFile)} must be specified.");
            if (string.IsNullOrEmpty(DatabaseServer))
                throw new ArgumentException($"{nameof(DatabaseServer)} must be specified.", nameof(DatabaseServer));

            // define the authentication methods to use (in order)
            var authenticationMethods = new List<AuthenticationMethod>();
            if (!string.IsNullOrEmpty(SshKeyFile))
                authenticationMethods.Add(new PrivateKeyAuthenticationMethod(SshUserName,
                    new PrivateKeyFile(SshKeyFile, string.IsNullOrEmpty(SshPassPhrase) ? null : SshPassPhrase)));
            if (!string.IsNullOrEmpty(SshPassword))
                authenticationMethods.Add(new PasswordAuthenticationMethod(SshUserName, SshPassword));

            // connect to the SSH server
            var sshClient = new SshClient(new ConnectionInfo(SshHostName, SshPort, SshUserName, authenticationMethods.ToArray()));
            sshClient.Connect();

            // forward a local port to the database server and port, using the SSH server
            var forwardedPort = new ForwardedPortLocal("127.0.0.1", DatabaseServer, (uint)DatabasePort);
            sshClient.AddForwardedPort(forwardedPort);
            forwardedPort.Start();

            return (sshClient, forwardedPort.BoundPort);
        } 
    }
}
