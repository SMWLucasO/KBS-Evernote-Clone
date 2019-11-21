using EvernoteCloneLibrary.Files.Parsers;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Notes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace EvernoteCloneLibraryTests.Files.Parsers
{

    [TestFixture, Order(1)]
    class XMLExporterTests
    {

        const string FileLocation = "tests/local/";

        [Order(1)]
        [TestCase(1, "test1.enex", Author = "Lucas Ouwens", Description = "Even though there's just one note, it should still be generated.")]
        [TestCase(10, "test2.enex", Author = "Lucas Ouwens", Description = "Many notes should not cause problems for the generating of the file")]
        [TestCase(1000, "test3.enex", Author = "Lucas Ouwens", Description = "A gigantic amount of notes should not cause any issues.")]
        [TestCase(0, "test4.enex", Author = "Lucas Ouwens", Description = "Even though there's no notes generated, the notebook should still be saved.")]
        public void Export_ShouldExportNotebooks(int notesToGenerate, string Filename)
        {

            // Arrange
            IParseable toParse = GenerateTestableNotebook(notesToGenerate);
            // Act
            bool actual = XMLExporter.Export(FileLocation, Filename, toParse);
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
            IParseable toParse = GenerateTestableNotebook(NotesToGenerate);

            // Act
            bool actual = XMLExporter.Export(FileLocation, Filename, toParse);

            // Assert
            Assert.That(actual, Is.False);
        }

        private static IParseable GenerateTestableNotebook(int notes)
        {
            if (notes == -1) return null;

            List<INote> innerNotes = new List<INote>();
            Notebook notebook = new Notebook()
            {
                Id = 1,
                LocationID = 1,
                Title = $"Notebook #{1}",
                CreationDate = DateTime.Now.Date,
                LastUpdated = DateTime.Now,
            };

            for (int j = 0; j < notes; j++)
            {
                Note note = new Note()
                {
                    Id = j,
                    NotebookID = 1,
                    Title = $"Test #{j}",
                    Author = $"Some {j}",
                    Content = $"ASDF {j}",
                    CreationDate = DateTime.Now.Date,
                    LastUpdated = DateTime.Now
                };

                innerNotes.Add(note);
            }

            notebook.Notes = innerNotes;

            return notebook;
        }
    }

    [TestFixture, Order(2)]
    class XMLImporterTests
    {
        const string FileLocation = "tests/local/";

        [Order(1)]
        [TestCase("test1.enex", 1, Author = "Lucas Ouwens", Description = "All files exported should be reloadable by the import method (1 note)")]
        [TestCase("test2.enex", 10, Author = "Lucas Ouwens", Description = "All files exported should be reloadable by the import method (10 notes)")]
        [TestCase("test3.enex", 1000, Author = "Lucas Ouwens", Description = "All files exported should be reloadable by the import method (1000 notes)")]
        [TestCase("test4.enex", 0, Author = "Lucas Ouwens", Description = "All files exported should be reloadable by the import method (0 notes)")]
        public void Import_ShouldImport(string Filename, int expectedNotes)
        {
            // Act
            IParseable loaded = XMLImporter.Import(FileLocation, Filename);

            // Assert
            Assert.IsNotNull(loaded);
            Assert.That(((Notebook)loaded).Notes.Count, Is.EqualTo(expectedNotes));
        }

        [Order(2)]
        [TestCase(null, Author = "Lucas Ouwens", Description = "If there is a null value given, then nothing should be able to get imported.")]
        [TestCase("", Author = "Lucas Ouwens", Description = "If it is an empty string, there should be nothing to import.")]
        [TestCase("FileWithoutExtension", Author = "Lucas Ouwens", Description = "A file without extension is not a file, thus it should not work.")]
        public void Import_ShouldNotImport(string Filename)
        {
            // Act
            IParseable actual = XMLImporter.Import(FileLocation, Filename);

            // Assert
            Assert.IsNull(actual);
        }

        [Order(3)]
        [TestCase("File_That_Doesnt_Exist.enex", Author = "Lucas Ouwens", Description = "A file that does not exist should just throw an exception.")]
        public void Import_ShouldThrowFileNotFoundException(string Filename)
        {
            // Act and Assert
            Assert.Throws(typeof(FileNotFoundException), () => XMLImporter.Import(FileLocation, Filename));
        }

        [OneTimeTearDown]
        public void ImportExport_ClearGeneratedFiles()
        {
            // Clean up the testing location.
            foreach (string file in Directory.GetFiles(FileLocation))
            {
                File.Delete(file);
            }
        }
    }
}
