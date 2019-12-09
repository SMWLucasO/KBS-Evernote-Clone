using EvernoteCloneLibrary.Notebooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Users
{
    public class User : UserModel
    {
        public List<Notebook> Notebooks { get; set; }

        /// <summary>
        /// Log the user in
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static UserModel Login(string username, string password)
        {
            UserRepository userRepositoryLogin = new UserRepository();
            UserModel userModelLogin = new UserModel()
            {
                Username = username,
                Password = password
            };
            return userRepositoryLogin.CompareDB(userModelLogin);
        }

        #region Password encrypter

        /// <summary>
        /// Encrypt the password of the user
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string Encryption(String password)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] encrypt;
            UTF8Encoding encode = new UTF8Encoding();

            //encrypt the given password string into Encrypted data  
            encrypt = md5.ComputeHash(encode.GetBytes(password));
            StringBuilder encryptdata = new StringBuilder();

            //Create a new string by using the encrypted data  
            for (int i = 0; i < encrypt.Length; i++)
            {
                encryptdata.Append(encrypt[i].ToString());
            }
            return encryptdata.ToString();
        }

        #endregion


        /// <summary>
        /// Store the account in the database
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public static bool Register(string username, string password, string firstName, string lastName)
        {
            UserRepository userRepository = new UserRepository();
            UserModel userModel = new UserModel()
            {
                Username = username,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                CreationDate = DateTime.Now.Date
            };
            return userRepository.Insert(userModel);
        }
    }
}
