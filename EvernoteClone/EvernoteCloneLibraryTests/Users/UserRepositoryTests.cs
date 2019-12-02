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


        public static List<int> StoredUserIDs
            = new List<int>();

        [TestCase("yrs@hotmail.com", "Appeltje123!", "Nice", "GoodHustle"), Order(1)]
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

        // TODO: @Chono implement the tests given below:

        public void GetBy_ShouldReturn()
        {

        }

        public void Update_ShouldReturn()
        {

        }

        public void Delete_ShouldReturn()
        {

        }

    }

}
