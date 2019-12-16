using System;
using NUnit.Framework;
using System.Collections.Generic;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Notebooks.Location;
using EvernoteCloneLibrary.Users;
using EvernoteCloneLibraryTests.TestHelpers;

namespace EvernoteCloneLibraryTests.Notebooks.Location
{
    [TestFixture, Order(4)]
    class NotebookLocationTests
    {
        private static readonly List<Tuple<NotebookLocation, int>> ToBeInsertedNotebookLocations = new List<Tuple<NotebookLocation, int>>
        {
            new Tuple<NotebookLocation, int>(
                ObjectGenerator.GenerateNotebookLocation("AddNewNotebookLocation_Should_Return_Boolean"),
                -1
            ),
            new Tuple<NotebookLocation, int>(
                ObjectGenerator.GenerateNotebookLocation("NewNotebookLocation_WithId_9999"),
                -1
            ),
            new Tuple<NotebookLocation, int>(
                ObjectGenerator.GenerateNotebookLocation(null),
                -1
            ),
            new Tuple<NotebookLocation, int>(
                ObjectGenerator.GenerateNotebookLocation("AddNewNotebookLocation_Should_Return_Boolean"),
                3
            ),
            new Tuple<NotebookLocation, int>(
                ObjectGenerator.GenerateNotebookLocation("NewNotebookLocation_WithId_9999"),
                3
            ),
            new Tuple<NotebookLocation, int>(
                ObjectGenerator.GenerateNotebookLocation(null),
                3
            )
        };

        [SetUp]
        public void Setup()
        {
            Constant.User = new User { Id=-1, Username = "LocalTest"};
        }
        
        [Order(1)]
        [TestCaseSource(nameof(ToBeInsertedNotebookLocations))]
        public void AddNewNotebookLocation_ShouldReturn_Boolean(Tuple<NotebookLocation, int> testCase)
        {
            // Arrange
            Constant.User.Id = testCase.Item2;

            // Act
            int actualResult = NotebookLocation.AddNewNotebookLocationAndGetId(testCase.Item1);
                
            // Assert
            if (testCase.Item2 == -1)
                Assert.IsTrue(actualResult == -1);
            else
                Assert.IsTrue(actualResult != -1);
        }

        [Order(2)]
        [TestCaseSource(nameof(ToBeInsertedNotebookLocations))]
        public void GetAllNotebookLocationsFromDatabase(Tuple<NotebookLocation, int> testCase)
        {
            // Arrange
            Constant.User.Id = testCase.Item2;
            
            // Act
            List<NotebookLocation> notebookLocations = NotebookLocation.GetAllNotebookLocationsFromDatabase();

            // Assert
            if (testCase.Item2 == -1)
                Assert.AreEqual(null, notebookLocations);
            else
                Assert.IsTrue(notebookLocations.Contains(testCase.Item1));
        }

        [Order(3)]
        [TestCaseSource(nameof(ToBeInsertedNotebookLocations))]
        public void GetNotebookLocationById_ShouldReturn_NotebookLocation(Tuple<NotebookLocation, int> testCase)
        {   
            // Arrange
            Constant.User.Id = testCase.Item2;
            
            // Act
            NotebookLocation notebookLocation = NotebookLocation.GetNotebookLocationById(testCase.Item1.Id);
            
            // Assert
            Assert.AreEqual(testCase.Item1.Id, notebookLocation.Id);
        }
        
        [Order(4)]
        [TestCaseSource(nameof(ToBeInsertedNotebookLocations))]
        public void GetNotebookLocationByPath_ShouldReturn_NotebookLocation(Tuple<NotebookLocation, int> testCase)
        {   
            // Arrange
            Constant.User.Id = testCase.Item2;
            
            // Act
            NotebookLocation notebookLocation = NotebookLocation.GetNotebookLocationByPath(testCase.Item1.Path);
            
            // Arrange
            Assert.AreEqual(testCase.Item1.Path, notebookLocation.Path);
        }
        
        [Order(5)]
        [TestCaseSource(nameof(ToBeInsertedNotebookLocations))]
        public void DeleteNotebookLocations_ShouldReturn_Boolean(Tuple<NotebookLocation, int> testCase)
        {
            // Arrange
            Constant.User.Id = testCase.Item2;
            
            // Act
            bool result = testCase.Item1.DeleteFromDatabase();
            
            // Assert
            if (testCase.Item2 == -1)
                Assert.IsFalse(result);
            else
                Assert.IsTrue(result);
        }
    }
}