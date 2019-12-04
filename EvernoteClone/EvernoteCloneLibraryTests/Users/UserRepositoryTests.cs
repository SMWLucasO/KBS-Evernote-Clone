using System;
using System.Collections.Generic;
using System.Text;
using EvernoteCloneLibrary.Users;
using NUnit.Framework;
using System.Linq;
using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibraryTests.Users
{

    [TestFixture]
    public class UserRepositoryTest
    {


        public static List<int> StoredUserIDs
            = new List<int>();

        [TestCase("yr7s@hotmail.com", "Appeltje123!", "Nice", "GoodHustle"), Order(1)]
        public void Insert_ShouldInsert(string Username, string Password, string FirstName, string LastName)
        {
            //Arrange
            UserRepository userRepository = new UserRepository();
            UserModel user = new UserModel()
            {
                Username = Username,
                Password = Password,
                FirstName = FirstName,
                LastName = LastName,
                CreationDate = DateTime.Now.Date

            };

            //Act            
            var result = userRepository.Insert(user);

            StoredUserIDs.Add(user.Id);

            //Assert
            Assert.IsTrue(result);
        }

        
        [TestCase, Order(2)]
        public void GetBy_ShouldReturn()
        {

            //Arrange
            UserRepository user = new UserRepository();
            UserModel user2 = new UserModel()
            {
                Username = "Lol@hotmail.com",
                Password = "Pk",
                FirstName = "dk",
                LastName = "Nice",
                CreationDate = DateTime.Now.Date

            };

            //Act and Assert
            foreach (int users in StoredUserIDs)
            {
                Assert.That(user.GetBy(new string[] { "id = @Id" }, new Dictionary<string, object> { { "Id", user2.Id } }).ToList().First().Id, Is.EqualTo(users));
            }
        }

        [Test, Order(3)]
        public void Update_ShouldReturn()
        {

            //Arrange
            bool Expected = true;
            UserRepository userRepository = new UserRepository();
            UserModel user = new UserModel()
            {
                Username = "Lol@hotmail.com",
                Password = "Pk",
                FirstName = "dk",
                LastName = "Nice",
                CreationDate = DateTime.Now.Date

            };

            //Act and Assert
            userRepository.Insert(user);

            foreach (int users in StoredUserIDs)
            {

                Assert.That(userRepository.Update(user), Is.EqualTo(Expected));
            }


        }

        [Test, Order(4)]
        public void Delete_ShouldReturn()
        {

            //Arrange
            bool Expected = true;
            UserRepository userRepository = new UserRepository();
            UserModel user = new UserModel()
            {
                Username = "Lol@hotmail.com",
                Password = "Pk",
                FirstName = "dk",
                LastName = "Nice",
                CreationDate = DateTime.Now.Date

            };

            //Act and Assert
            var result = userRepository.Insert(user);


            Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    {"@Id", user.Id}
                };
            foreach (int users in StoredUserIDs)
            {

                Assert.That(userRepository.Delete(user), Is.EqualTo(Expected));
            }
        }

    }

}
