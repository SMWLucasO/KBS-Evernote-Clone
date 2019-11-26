using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EvernoteCloneLibrary.Database;
using Renci.SshNet;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ssh_connection ssh_Connection = new ssh_connection(1);
            SshConnection sshConnection = new SshConnection();
            Console.WriteLine("Yay");
            Console.ReadLine();
        }
    }

    internal class ssh_connection
    {
        private SshClient client;
        public ssh_connection()
        {
            var host = "145.44.234.54";
            var port = 22;
            var username = "student";
            var passphrase = "";
            var key = File.ReadAllText(@"C:\git\KBS-Evernote-Clone\EvernoteClone\open-ssh");

            var buf = new MemoryStream(Encoding.UTF8.GetBytes(key));
            var privateKeyFile = new PrivateKeyFile(buf, passphrase);
            var connectionInfo = new ConnectionInfo(host, port, username,
                new PrivateKeyAuthenticationMethod(username, privateKeyFile));

            client = new SshClient(connectionInfo);
        }

        public ssh_connection(int num)
        {
            var host = "145.44.234.54";
            var port = 22;
            var username = "student";
            var passphrase = "";
            var key = File.ReadAllText(@"C:\git\KBS-Evernote-Clone\EvernoteClone\open-ssh");

            var buf = new MemoryStream(Encoding.UTF8.GetBytes(key));
            var privateKeyFile = new PrivateKeyFile(buf, passphrase);
            var connectionInfo = new ConnectionInfo(host, port, username,
                new PrivateKeyAuthenticationMethod(username, privateKeyFile));

            client = new SshClient(connectionInfo);

            if (num == 1)
            {

                client.Connect();
                accesDatabase();
            }
        }

        public void startSsh()
        {
            client.Connect();
        }

        public void stopSsh()
        {
            client.Disconnect();
        }

        public string sendCommand(string command)
        {
            var output = client.RunCommand(command);
            System.Threading.Thread.Sleep(1000);
            return output.Result;
        }

        public void accesDatabase()
        {
            Console.WriteLine(sendCommand("docker exec -it evernote /bin/bash"));
            Console.WriteLine(sendCommand("mysql -uever -pikwilmijndata"));
            Console.WriteLine(sendCommand("show databases;"));
        }
    }
}
