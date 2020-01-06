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
        /// Passes username and password to check if they're in the database
        /// if so, login will give a success message else a failed message.
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
            return userRepositoryLogin.Login(userModelLogin);
        }

        #region Password encrypter

        /// <summary>
        /// Passess a password, gets encrypted through a md5 method and returns this in a string.
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


        #region Handles sign-up account
        /// <summary>
        /// First method is to sign-up a normal account. Passes username, password,firstname and lastname. 
        /// Second method is to sign-up a google account passess same variables with a boolean to set the value to true for google accounts. 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public static bool Register(string username, string password, string firstName, string lastName, bool isGoogle = false)
        {
            UserRepository userRepository = new UserRepository();
            UserModel userModel = new UserModel()
            {
                Username = username,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                IsGoogleAccount = isGoogle,
                CreationDate = DateTime.Now.Date
            };
            return userRepository.Insert(userModel);
        }
        #endregion
    }
}
