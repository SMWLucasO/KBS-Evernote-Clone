using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Files.Parsers;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Notes;
using EvernoteCloneLibraryTests.TestHelpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace EvernoteCloneLibraryTests.Files.Parsers
{

    [TestFixture, Order(1)]
    class XMLExporterTests
    {

        [TestCase(Constant.TEST_NOTEBOOK_STORAGE_PATH + "/testcases/", "file.enex", 1)]
        [TestCase(Constant.TEST_NOTEBOOK_STORAGE_PATH + "/testcases/", "file2.enex", 3)]
        public void Export_ShouldExport(string FilePath, string Filename, int notes)
        {
            // Arrange
            IParseable parseable = ObjectGenerator.GenerateTestableNotebook(notes);


            // Act
            bool actual = XMLExporter.Export(FilePath, Filename, parseable);

            // Assert
            Assert.That(actual, Is.True);
        }

        [TestCase("", "", -1)]
        [TestCase(null, null, -1)]
        [TestCase(null, "", -1)]
        [TestCase("", null, -1)]
        [TestCase(null, null, 3)]
        [TestCase("", "", 3)]
        [TestCase("", null, 3)]
        [TestCase(null, "", 3)]
        [TestCase("somepath/", "filewithoutextension", 3)]
        public void Export_ShouldNotExport(string FilePath, string Filename, int notes)
        {
            // Arrange
            IParseable parseable = ObjectGenerator.GenerateTestableNotebook(notes);


            // Act
            bool actual = XMLExporter.Export(FilePath, Filename, parseable);

            // Assert
            Assert.That(actual, Is.False);
        }

    }

    [TestFixture, Order(2)]
    class XMLImporterTests
    {

        [Order(1)]
        [Test]
        public void Import_ShouldImportNotebooks()
        {
            // Act
            List<Notebook> loaded = XMLImporter.TryImportNotebooks(Constant.TEST_NOTEBOOK_STORAGE_PATH + "/testcases/");

            // Assert
            Assert.IsNotNull(loaded);
            Assert.IsTrue(loaded.Count > 0);
        }

        [Order(2)]
        [TestCase("Folder/That/Doesnot/Exist/")]
        [TestCase(null)]
        [TestCase("SomeExistingDirectory/")]
        public void Import_ShouldNotImport_IsNull(string Path)
        {
            // Arrange and Act
            List<Notebook> actual = XMLImporter.TryImportNotebooks(Path);

            // Assert
            Assert.IsNull(actual);
        }

        [OneTimeSetUp]
        public void Generate_TestFolder()
        {
            if (!(Directory.Exists("SomeExistingDirectory/")))
            {
                Directory.CreateDirectory("SomeExistingDirectory/");
            }
        }

        [OneTimeTearDown]
        public void Remove_TestCaseFolder()
        {
            if(Directory.Exists(Constant.TEST_NOTEBOOK_STORAGE_PATH + "/testcases/"))
            {
                Directory.Delete(Constant.TEST_NOTEBOOK_STORAGE_PATH + "/testcases/", true);
            }
        }

    }
}
