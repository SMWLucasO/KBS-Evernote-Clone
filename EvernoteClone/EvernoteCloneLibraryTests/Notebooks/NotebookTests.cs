using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Files.Parsers;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibraryTests.Files.Parsers;
using EvernoteCloneLibraryTests.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using EvernoteCloneLibrary.Users;

namespace EvernoteCloneLibraryTests.Notebooks
{
    [TestFixture, Order(8)]
    class NotebookTests
    {
        [SetUp]
        public void Setup()
        {
            Constant.User = new User { Id=3, Username = "LocalTest"};
        }

        // To run some of these test-cases properly, we need a user with a UserID of '3'
        // More testcases should be added once we have more objects to work with.
        [Order(1)]
        [TestCase(-1, 1, true)]
        [TestCase(3, 1, true)]
        public void Save_ShouldReturn(int userId, int notesToGenerate, bool expected)
        {
            // Arrange
            Constant.User.Id = userId;
            Notebook notebook = (Notebook)ObjectGenerator.GenerateTestableNotebook(notesToGenerate);

            // Act
            bool actual = notebook.Save();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));

        }

        [Order(2)]
        [TestCase(-1, true, Description = "If the user ID is '-1', it means it should just get the local notebooks.")]
        [TestCase(3, false, Description = "If the user ID is '3', it means the notes from user 3 will also be loaded.")]
        public void Load_ShouldReturn(int userId, bool shouldJustBeFs)
        {
            // Arrange
            Constant.User.Id = userId;
            List<Notebook> notebooksFromFs = XmlImporter.TryImportNotebooks(GetNotebookStoragePath());
            
            // Act
            List<Notebook> actual = Notebook.Load();
            
            // Assert
            if (shouldJustBeFs)
            {
                Assert.AreEqual(notebooksFromFs.Count, actual.Count);
            }
            else
            {
                CollectionAssert.AreNotEquivalent(notebooksFromFs, actual);
            }
        }

        [Order(3)]
        [TestCase(3, null, "Nameless notebook (3)")]
        [TestCase(0, "", "Nameless notebook")]
        [TestCase(3, "Notebook1", "Notebook1 (3)")]
        [TestCase(0, "Notebook", "Notebook")]
        public void ToString_ShouldReturn(int notesToGenerate, string notebookTitle, string expected)
        {
            // Arrange
            Notebook generatedNotebook = (Notebook)ObjectGenerator.GenerateTestableNotebook(notesToGenerate);
            generatedNotebook.Title = notebookTitle;

            // Act
            string actual = generatedNotebook.ToString();

            // Assert
            Assert.That(expected, Is.EqualTo(actual));
        }

        [Order(4)]
        [OneTimeTearDown]
        public void ImportExport_ClearGeneratedFiles()
        {
            // Clean up the testing location.
            foreach (string file in Directory.GetFiles(GetNotebookStoragePath()))
            {
                File.Delete(file);
            }
        }

        private static string GetNotebookStoragePath()
        {
            string path = Constant.TEST_MODE ? Constant.TEST_NOTEBOOK_STORAGE_PATH : Constant.PRODUCTION_NOTEBOOK_STORAGE_PATH;
            string[] splittedPath = path.Split('<', '>');

            if (splittedPath.Length == 3)
            {
                splittedPath[1] = Constant.User.Username;

                return splittedPath[0] + splittedPath[1] + splittedPath[2];
            }

            return null;
        }
    }
}
