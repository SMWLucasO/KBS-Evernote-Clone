using System;
using System.Collections.Generic;
using System.Text;
using EvernoteCloneLibrary.Users;
using NUnit.Framework;
using System.Linq;


namespace EvernoteCloneLibraryTests.Users
{
    [TestFixture]
    public class UserRepositoryTest
    {
        
        [TestCase("yrs@hotmail.com", "Appeltje123!", "Nice", "GoodHustle")]
        
        public void insertNewUser(string Username, string Password, string FirstName, string LastName)
        {
            
            //Arrange

            UserRepository userRepository = new UserRepository();
            UserModel users = new UserModel()
            {
                Username = Username,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName,
                CreationDate = DateTime.Now.Date

            };

            //Act            
             var result = userRepository.Insert(users);

            //Assert

            Assert.IsTrue(result);

        }

        [TestCase]

        public void 
    }

}
