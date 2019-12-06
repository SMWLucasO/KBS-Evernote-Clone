using NUnit.Framework;
using System;
using System.Collections.Generic;
using EvernoteCloneLibrary.Notebooks.Location;

// TODO maybe rewrite this test (look at NotebookLocationRepositoryTests)
namespace EvernoteCloneLibraryTests.Notebooks.Location
{
    [TestFixture]
    class NotebookLocationTests
    {
        static readonly List<NotebookLocation> _insertedNotebookLocations = new List<NotebookLocation>();
        static Random _random = new Random();

        [Order(1)]
        [TestCase(-1, "AddNewNotebookLocation_Should_Return_Boolean", -1, true)]
        [TestCase(9999, "NewNotebookLocation_WithId_9999", -1, true)]
        [TestCase(-1, null, -1, true)]
        [TestCase(99998, null, -1, true)]
        [TestCase(-1, "AddNewNotebookLocation_Should_Return_Boolean", 3, true)]
        [TestCase(9997, "NewNotebookLocation_WithId_9999", 3, true)]
        [TestCase(-1, null, 3, true)]
        [TestCase(99996, null, 3, true)]
        public void AddNewNotebookLocation_ShouldReturn_Boolean(int id, string path, int userId, bool expectedResult)
        {
            // Arrange
            NotebookLocation newNotebookLocation = new NotebookLocation
            {
                Id = id,
                Path = path
            };

            // Act
            int _tmpId = NotebookLocation.AddNewNotebookLocationAndGetId(newNotebookLocation, userId);
            
            // If actualResult equals true, it has been added to the database.
            if (_tmpId != -1 && userId != -1)
                _insertedNotebookLocations.Add(newNotebookLocation);
            
            // Assert
            Assert.AreEqual(expectedResult, _tmpId != 0);
        }

        [Order(2)]
        [TestCase(-1, false)]
        [TestCase(3, true)]
        public void GetAllNotebookLocationsFromDatabase(int userId, bool expectedResult)
        {
            // Act
            List<NotebookLocation> notebookLocations = NotebookLocation.GetAllNotebookLocationsFromDatabase(userId);
            bool actualResult = notebookLocations.Count > 0;

            // Assert
            Assert.AreEqual(expectedResult, actualResult);
        }

        [Order(3)]
        [TestCase(-1)]
        [TestCase(3)]
        public void GetNotebooklocationById_ShouldReturn_NotebookLocation(int userId)
        {   
            // Act and Assert
            foreach (NotebookLocation notebookLocation in _insertedNotebookLocations)
            {
                Assert.AreEqual(
                    notebookLocation.Id,
                    NotebookLocation.GetNotebookLocationById(notebookLocation.Id, userId)?.Id ?? -1);
            }
        }
        
        [Order(4)]
        [TestCase(-1)]
        [TestCase(3)]
        public void GetNotebooklocationByPath_ShouldReturn_NotebookLocation(int userId)
        {   
            // Act and Assert
            foreach (NotebookLocation notebookLocation in _insertedNotebookLocations)
            {
                Assert.AreEqual(
                    notebookLocation.Id,
                    NotebookLocation.GetNotebookLocationByPath(notebookLocation.Path, userId)?.Id ?? -1);
            }
        }
    }
}