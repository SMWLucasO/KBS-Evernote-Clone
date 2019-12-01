using EvernoteCloneLibrary.Notebooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Users
{
    public class User : UserModel
    {
        public List<Notebook> Notebooks { get; set; }


        public static bool Login(string Username, string Password)
        {
            return false;
        }

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
