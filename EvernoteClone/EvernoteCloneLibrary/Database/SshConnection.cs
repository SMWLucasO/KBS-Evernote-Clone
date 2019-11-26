using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Database
{
    public class SshConnection
    {
        public SshConnection()
        {
            var sshHost = "145.44.234.54";
            var sshUsername = "student";
            var sshKeyFilePath = @"C:\git\KBS-Evernote-Clone\EvernoteClone\open-ssh";

            var databasePort = 1433;
            var databaseHost = "127.0.0.1";
            var databaseUsername = "ever";
            var databasePassword = "ikwilmijndata";
            var databaseCatalog = "NoteFever_EvernoteClone";
            var databaseSecurity = true;

            var (sshClient, localPort) = ConnectSsh(sshHostName: sshHost,
                sshUserName: sshUsername,
                sshKeyFile: sshKeyFilePath,
                databaseServer: databaseHost,
                databasePort: databasePort);

            using (sshClient)
            {
                string connectionString = $"Data Source={databaseHost},{localPort};" +
                    $"Initial Catalog={databaseCatalog};" +
                    $"User ID={databaseUsername};" +
                    $"Password={databasePassword}" +
                    $"Integrated Security={char.ToUpper(databaseSecurity.ToString()[0]) + databaseSecurity.ToString().Substring(1)}";

                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
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
        private (SshClient SshClient, uint Port) ConnectSsh(string sshHostName, string sshUserName, string sshPassword = null,
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
