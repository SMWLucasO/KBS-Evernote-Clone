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


        public static UserModel Login(string Username, string Password)
        {
            UserRepository userRepositoryLogin = new UserRepository();
            UserModel userModelLogin = new UserModel()
            {
                Username = Username,
                Password = Password
            };
            return userRepositoryLogin.CompareDB(userModelLogin);
        }

        #region Password encrypter

        //Encrypt password md5
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


        //Inserts user data in database
        public static bool Register(string Username, string Password, string FirstName, string LastName)
        {
            UserRepository userRepository = new UserRepository();
            UserModel userModel = new UserModel()
            {
                Username = Username,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName,
                CreationDate = DateTime.Now.Date
            };
            return userRepository.Insert(userModel);
        }
    }
}
