using EvernoteCloneLibrary.Notebooks;
using System;
using System.Collections.Generic;

namespace EvernoteCloneLibrary.Users
{
    public class User : UserModel
    {
        public List<Notebook> Notebooks { get; set; }

        public static User Login(string username, string password) =>
            null;


        //Inserts user data in database
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
