using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using EvernoteCloneLibrary.Settings.Locales;
using NUnit.Framework;

namespace EvernoteCloneLibraryTests.Settings.Locales
{
    [TestFixture, Order(10)]
    class LocaleRepositoryTests
    {
        private static readonly List<Locale> InsertedLocales = new List<Locale>();

        [Order(1)]
        [TestCase("TEST", "TEST_value", true)]
        [TestCase("TSET", "TEST_value_valid", true)]
        [TestCase("TSET", "TEST_value", false)]
        [TestCase("LocalUserID", "TEST_value", false)]
        [TestCase("WRONG USER ID", "TEST_value", false)]
        public void Insert_ShouldReturn(string locale, string language, bool expectedResult)
        {
            // Arrange
            Locale toBeInsertedLocale = new Locale
            {
                Locale = locale,
                Language = language
            };
            
            LocaleRepository settingRepository = new LocaleRepository();
            bool actualResult = false;
            
            // Act
            if (expectedResult == false)
                Assert.Throws<SqlException>(() => settingRepository.Insert(toBeInsertedLocale));
            else 
                actualResult = settingRepository.Insert(toBeInsertedLocale);
            
            if (actualResult)
                InsertedLocales.Add(toBeInsertedLocale);
            
            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Order(2)]
        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("NEW language", true)]
        public void Update_ShouldReturn(string newSettingValue, bool expectedResult)
        {
            // Arrange
            LocaleRepository settingRepository = new LocaleRepository();
            
            // Act and Assert
            Locale locale = InsertedLocales[0];
            locale.Language = newSettingValue;

            Assert.AreEqual(expectedResult, settingRepository.Update(locale));
        }

        [Order(3)]
        [Test]
        public void Read_ShouldReturn()
        {
            // Arrange
            LocaleRepository settingRepository = new LocaleRepository();
            
            // Act and Assert
            foreach (Locale setting in InsertedLocales)
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
            LocaleRepository settingRepository = new LocaleRepository();
            
            // Act and Assert
            foreach (Locale setting in InsertedLocales)
            {
                Assert.IsTrue(settingRepository.Delete(setting));
            }
        }
    }
}