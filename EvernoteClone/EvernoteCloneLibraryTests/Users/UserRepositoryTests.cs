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
        private static List<UserModel> storedUsers = new List<UserModel>
        {
            new UserModel
            {
                Username = "yr7s@hotmail.com",
                Password = User.Encryption("Appeltje123!"),
                FirstName = "Nice",
                LastName = "GoodHustle",
                CreationDate = DateTime.Now.Date
            },
            new UserModel
            {
                Username = "test@email.example",
                Password = User.Encryption("SupersterkWachtwoord123!"),
                FirstName = "FName",
                LastName = "LName",
                CreationDate = DateTime.Now.Date
            }
        };

        [Order(1)]
        [TestCaseSource(nameof(storedUsers))]
        public void Insert_ShouldInsert(UserModel user)
        {
            //Arrange
            UserRepository userRepository = new UserRepository();

            //Act            
            bool result = userRepository.Insert(user);

            //Assert
            Assert.IsTrue(result);
        }

        [Order(2)]
        [TestCaseSource(nameof(storedUsers))]
        public void Update_ShouldReturn(UserModel user)
        {
            //Arrange
            UserRepository userRepository = new UserRepository();
            user.FirstName = user.FirstName + "UPDATE";
            user.LastName = user.LastName + "UPDATE";

            //Act
            bool result = userRepository.Update(user);

            // Assert
            Assert.IsTrue(result);
        }
        
        [Order(3)]
        [TestCaseSource(nameof(storedUsers))]
        public void GetBy_ShouldReturn(UserModel expectedUser)
        {
            //Arrange
            UserRepository userRepository = new UserRepository();
            
            // Act
            UserModel actualUser = userRepository.GetBy(
                new[] {"Id = @Id"},
                new Dictionary<string, object> {{"Id", expectedUser.Id}}
            ).ToList().First();

            //Act and Assert
            Assert.AreEqual(expectedUser.Id, actualUser.Id);
        }

        [Order(4)]
        [TestCaseSource(nameof(storedUsers))]
        public void Delete_ShouldReturn(UserModel user)
        {
            // Arrange
            UserRepository userRepository = new UserRepository();

            // Act
            var result = userRepository.Delete(user);
            
            // Assert
            Assert.IsTrue(result);
        }
    }
}
