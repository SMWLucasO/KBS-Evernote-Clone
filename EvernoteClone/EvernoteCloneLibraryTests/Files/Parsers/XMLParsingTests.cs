using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Files.Parsers;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibraryTests.TestHelpers;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace EvernoteCloneLibraryTests.Files.Parsers
{

    [TestFixture, Order(1)]
    class XmlExporterTests
    {

        [TestCase(Constant.TESTS_STORAGE_PATH, "file.enex", 1)]
        [TestCase(Constant.TESTS_STORAGE_PATH, "file2.enex", 3)]
        public void Export_ShouldExport(string filePath, string filename, int notes)
        {
            // Arrange
            IParseable parseable = ObjectGenerator.GenerateTestableNotebook(notes);


            // Act
            bool actual = XmlExporter.Export(filePath, filename, parseable);

            // Assert
            Assert.IsTrue(actual);
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
        public void Export_ShouldNotExport(string filePath, string filename, int notes)
        {
            // Arrange
            IParseable parseable = ObjectGenerator.GenerateTestableNotebook(notes);


            // Act
            bool actual = XmlExporter.Export(filePath, filename, parseable);

            // Assert
            Assert.IsFalse(actual);
        }

    }

    [TestFixture, Order(2)]
    class XmlImporterTests
    {

        [Order(1)]
        [Test]
        public void Import_ShouldImportNotebooks()
        {
            // Act
            List<Notebook> loaded = XmlImporter.TryImportNotebooks(Constant.TESTS_STORAGE_PATH);

            // Assert
            Assert.IsNotNull(loaded);
            Assert.IsTrue(loaded.Count > 0);
        }

        [Order(2)]
        [TestCase("Folder/That/Doesnot/Exist/")]
        [TestCase(null)]
        [TestCase("SomeExistingDirectory/")]
        public void Import_ShouldNotImport_IsNull(string path)
        {
            // Arrange and Act
            List<Notebook> actual = XmlImporter.TryImportNotebooks(path);

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
            if(Directory.Exists(Constant.TESTS_STORAGE_PATH))
            {
                Directory.Delete(Constant.TESTS_STORAGE_PATH, true);
            }
        }

    }
}
