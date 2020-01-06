using EvernoteCloneLibrary.Notebooks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EvernoteCloneLibraryTests.Notebooks
{
    [TestFixture, Order(7)]
    class NotebookRepositoryTests
    {
        private static readonly List<Notebook> InsertedNotebooks = new List<Notebook>();

        [Order(1)]
        [TestCase("", true)]
        [TestCase(null, true)]
        [TestCase("Works", true)]
        public void Insert_ShouldReturn(string title, bool expected)
        {
            // Arrange
            Notebook notebook = new Notebook
            {
                UserId = 3,
                LocationId = 1, // default id for testing
                Title = title,
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
            Assert.That(expected, Is.EqualTo(actual));
        }

        [Order(2)]
        [TestCase("", true)]
        [TestCase(null, true)]
        [TestCase("ATitle", true)]
        public void Update_ShouldReturn(string title, bool expected)
        {
            // Arrange
            NotebookRepository repository = new NotebookRepository();

            // Act and Assert
            foreach (Notebook notebook in InsertedNotebooks)
            {
                notebook.Title = title;
                Assert.That(repository.Update(notebook), Is.EqualTo(expected));
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
