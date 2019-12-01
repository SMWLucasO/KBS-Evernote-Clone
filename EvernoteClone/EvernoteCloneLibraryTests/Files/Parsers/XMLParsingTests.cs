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
        [Order(1)]
        [TestCase(1, "test1.enex", Author = "Lucas Ouwens", Description = "Even though there's just one note, it should still be generated.")]
        [TestCase(10, "test2.enex", Author = "Lucas Ouwens", Description = "Many notes should not cause problems for the generating of the file")]
        [TestCase(1000, "test3.enex", Author = "Lucas Ouwens", Description = "A gigantic amount of notes should not cause any issues.")]
        [TestCase(0, "test4.enex", Author = "Lucas Ouwens", Description = "Even though there's no notes generated, the notebook should still be saved.")]
        public void Export_ShouldExportNotebooks(int notesToGenerate, string Filename)
        {

            // Arrange
            IParseable toParse = ObjectGenerator.GenerateTestableNotebook(notesToGenerate);
            // Act
            bool actual = XMLExporter.Export(Constant.TEST_NOTEBOOK_STORAGE_PATH, Filename, toParse);
            // Assert
            Assert.That(actual, Is.True);
        }

        [Order(2)]
        [TestCase(-1, null, Author = "Lucas Ouwens", Description = "Object to parse and filename may not be null for the export to be successful")]
        [TestCase(-1, "Test.enex", Author = "Lucas Ouwens", Description = "Object to parse may not be null for the export to be successful")]
        [TestCase(3, "Test", Author = "Lucas Ouwens", Description = "A file must have an extension to be exported.")]
        [TestCase(5, "", Author = "Lucas Ouwens", Description = "A file needs to have a name to be exported")]
        public void Export_ShouldNotExport(int NotesToGenerate, string Filename)
        {
            // Arrange
            IParseable toParse = ObjectGenerator.GenerateTestableNotebook(NotesToGenerate);

            // Act
            bool actual = XMLExporter.Export(Constant.TEST_NOTEBOOK_STORAGE_PATH, Filename, toParse);

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
            List<Notebook> loaded = XMLImporter.ImportNotebooks(Constant.TEST_NOTEBOOK_STORAGE_PATH);

            // Assert
            Assert.IsNotNull(loaded);
        }

        [Order(2)]
        [TestCase("Folder/That/Doesnot/Exist/")]
        [TestCase(null)]
        [TestCase("SomeExistingDirectory/")]
        public void Import_ShouldNotImport_IsNull(string Path)
        {
            // Arrange and Act
            List<Notebook> actual = XMLImporter.ImportNotebooks(Path);

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
    }
}
