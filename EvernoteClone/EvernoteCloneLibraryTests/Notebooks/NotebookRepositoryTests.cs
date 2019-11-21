using EvernoteCloneLibrary.Notebooks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EvernoteCloneLibraryTests.Notebooks
{
    [TestFixture, Order(3)]
    class NotebookRepositoryTests
    {
        static List<Notebook> InsertedNotebooks = new List<Notebook>();

        [Order(1)]
        [TestCase("", true)]
        [TestCase(null, true)]
        [TestCase("Works", true)]
        public void Insert_ShouldReturn(string Title, bool Expected)
        {
            // Arrange
            Notebook notebook = new Notebook()
            {
                UserID = 3,
                LocationID = 1, // default id for testing
                Title = Title,
                CreationDate = DateTime.Now.Date,
                LastUpdated = DateTime.Now
            };

            NotebookRepository notebookRepository = new NotebookRepository();

            // Act
            bool actual = notebookRepository.Insert(notebook);

            // Should only add the Id if it got inserted.
            if (actual)
                InsertedNotebooks.Add(notebook);

            // Assert
            Assert.That(Expected, Is.EqualTo(actual));
        }

        [Order(2)]
        [TestCase("", true)]
        [TestCase(null, true)]
        [TestCase("ATitle", true)]
        public void Update_ShouldReturn(string Title, bool Expected)
        {
            // arrange
            NotebookRepository repository = new NotebookRepository();

            // act and assert
            foreach (Notebook notebook in InsertedNotebooks)
            {
                notebook.Title = Title;
                Assert.That(repository.Update(notebook), Is.EqualTo(Expected));
            }

        }

        [Order(3)]
        [Test]
        public void Read_ShouldReturn()
        {
            // Arrange
            NotebookRepository notebookRepository = new NotebookRepository();

            // Act and Assert
            foreach (Notebook notebook in InsertedNotebooks)
            {
                Assert.That(
                    notebookRepository.GetBy(
                        new string[] { "Id = @Id" },
                        new Dictionary<string, object> { { "@Id", notebook.Id } }).ToList().First().Id,
                    Is.EqualTo(notebook.Id));
            }
        }

        [Order(4)]
        [Test]
        public void Delete_ShouldReturn()
        {
            // Arrange
            NotebookRepository noteRepository = new NotebookRepository();

            // Act and Assert
            foreach (Notebook notebook in InsertedNotebooks)
            {
                Assert.That(noteRepository.Delete(notebook), Is.True);
            }
        }
    }
}
