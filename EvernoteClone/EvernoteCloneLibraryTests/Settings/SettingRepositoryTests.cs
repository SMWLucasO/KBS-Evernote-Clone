using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EvernoteCloneLibrary.Settings;
using NUnit.Framework;

namespace EvernoteCloneLibraryTests.Settings
{
    [TestFixture, Order(9)]
    class SettingRepositoryTests
    {
        private static readonly List<Setting> InsertedSettings = new List<Setting>();

        [Order(1)]
        [TestCase(3, "TEST", "TEST_value", true)]
        [TestCase(3, "NEW_TEST", "TEST_value", true)]
        [TestCase(-1, "LocalUserID", "TEST_value", false)]
        [TestCase(0, "WRONG USER ID", "TEST_value", false)]
        public void Insert_ShouldReturn(int userId, string keyWord, string value, bool expectedResult)
        {
            // Arrange
            Setting setting = new Setting
            {
                UserId = userId,
                KeyWord = keyWord,
                SettingValue = value
            };
            
            SettingRepository settingRepository = new SettingRepository();
            bool actualResult = false;
            
            // Act
            if (expectedResult == false)
                Assert.Throws<SqlException>(() => settingRepository.Insert(setting));
            else 
                actualResult = settingRepository.Insert(setting);
            
            if (actualResult)
                InsertedSettings.Add(setting);
            
            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Order(2)]
        [TestCase("", true)]
        [TestCase(null, false)]
        [TestCase("NEW TITLE", true)]
        public void Update_ShouldReturn(string newSettingValue, bool expectedResult)
        {
            // Arrange
            SettingRepository settingRepository = new SettingRepository();
            
            // Act and Assert
            foreach (Setting setting in InsertedSettings)
            {
                setting.SettingValue = newSettingValue;

                if (newSettingValue == null)
                {
                    Assert.Throws<SqlException>(() => settingRepository.Update(setting));
                }
                else
                {
                    Assert.AreEqual(expectedResult, settingRepository.Update(setting));
                }
            }
        }

        [Order(3)]
        [Test]
        public void Read_ShouldReturn()
        {
            // Arrange
            SettingRepository settingRepository = new SettingRepository();
            
            // Act and Assert
            foreach (Setting setting in InsertedSettings)
            {
                int result = settingRepository.GetBy(
                    new[] { "Id = @Id" },
                    new Dictionary<string, object> { { "@Id", setting.Id } }).ToList().First().Id;
                Assert.AreEqual(setting.Id, result);
            }
        }

        [Order(4)]
        [Test]
        public void Delete_ShouldReturn()
        {
            // Arrange
            SettingRepository settingRepository = new SettingRepository();
            
            // Act and Assert
            foreach (Setting setting in InsertedSettings)
            {
                Assert.IsTrue(settingRepository.Delete(setting));
            }
        }
    }
}