using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using EvernoteCloneLibrary.Notebooks.Location;

namespace EvernoteCloneLibraryTests.Notebooks.Location
{
    [TestFixture, Order(3)]
    class NotebookLocationRepositoryTests
    {
        private static List<NotebookLocation> _testNotebookLocations = new List<NotebookLocation>
        {
            new NotebookLocation {Path = "TestNBL_1"},
            new NotebookLocation {Path = "TestNBL_2"},
            new NotebookLocation {Path = null},
            new NotebookLocation {Path = "TestNBL_4"},
            new NotebookLocation {Path = "TestNBL_5"},
        };
        private static readonly Random Random = new Random();
        private static bool _updateToNull = false;

        [Order(1)]
        [TestCaseSource(nameof(_testNotebookLocations))]
        public void Insert_ShouldReturn_Boolean(NotebookLocation notebookLocation)
        {
            // Arrange
            NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
            
            // Act
            bool actualResult = notebookLocationRepository.Insert(notebookLocation);
            
            // Assert
            Assert.IsTrue(actualResult);
        }

        [Order(2)]
        [TestCaseSource(nameof(_testNotebookLocations))]
        public void Update_ShouldReturn(NotebookLocation toUpdate)
        {
            // Arrange
            NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
            toUpdate.Path = _updateToNull ? null : Random.Next().ToString();
            
            // Act
            bool actualResult = notebookLocationRepository.Update(toUpdate);
            
            // Assert
            Assert.IsTrue(actualResult);
        }

        [Order(3)]
        [TestCaseSource(nameof(_testNotebookLocations))]
        public void GetBy_ShouldReturn(NotebookLocation notebookLocation)
        {
            // Arrange
            NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
            
            // Act and Assert
            Assert.AreEqual(notebookLocation.Id, 
                notebookLocationRepository.GetBy(
                    new[] { "Id = @Id" },
                    new Dictionary<string, object> { { "@Id", notebookLocation.Id } }
                ).ToList().First().Id);
        }

        [Order(4)]
        [TestCaseSource(nameof(_testNotebookLocations))]
        public void Delete_ShouldReturn_True(NotebookLocation notebookLocation)
        {
            // Arrange
            NotebookLocationRepository notebookLocationRepository = new NotebookLocationRepository();
            
            // Act and Assert
            Assert.IsTrue(notebookLocationRepository.Delete(notebookLocation));
        }
    }
}