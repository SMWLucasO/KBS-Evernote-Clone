using EvernoteCloneLibrary.Notebooks.Notes;
using NUnit.Framework;
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace EvernoteCloneLibraryTests.Notebooks.Notes
{
    [TestFixture, Order(4)]
    class NoteRepositoryTests
    {

        static List<Note> _insertedNotes = new List<Note>();

        [Order(1)]
        [TestCase("Some title", "<b>Message</b>", "", false, Description = "A note is not allowed to not have an author, and an author always has a name.")]
        [TestCase("Some title #2", "<b>Message</b>", null, false, Description = "A note is not allowed to not have an author.")]
        [TestCase(null, "<b>message</b>", "Lucas", true, Description = "If no title is given, a default one should be generated.")]
        [TestCase("", "<b>message</b>", "Lucas", true, Description = "If no title is given, a default one should be generated.")]
        [TestCase("Some stuff", "", "Lucas", true, Description = "Notes can be inserted without content")]
        [TestCase("", "", "text", true, Description = "If no title is given, a default one should be generated and notes dont have to have contents.")]
        public void Insert_ShouldReturn(string title, string content, string author, bool expected)
        {
            // Arrange
            Note note = new Note()
            {
                NotebookId = 2, // ID 2 is default test notebook
                Title = title,
                Content = content,
                CreationDate = DateTime.Now.Date,
                LastUpdated = DateTime.Now,
                Author = author,
            };

            NoteRepository noteRepository = new NoteRepository();

            // Act
            bool actual = noteRepository.Insert(note);

            // Should only add the Id if it got inserted.
            if (actual)
                _insertedNotes.Add(note);

            // Assert
            Assert.That(expected, Is.EqualTo(actual));
        }

        [Order(2)]
        [TestCase("", false, Description = "A note should always have an author.")]
        [TestCase(null, false, Description = "A note should always have an author.")]
        [TestCase("SomeText", true, Description = "A note should be able to change title")]
        public void Update_ShouldReturn(string author, bool expected)
        {
            // arrange
            NoteRepository repository = new NoteRepository();

            // act and assert
            foreach (Note note in _insertedNotes)
            {
                note.Author = author;
                Assert.That(repository.Update(note), Is.EqualTo(expected));
            }

        }

        [Order(3)]
        [Test]
        public void Read_ShouldReturn()
        {
            // Arrange
            NoteRepository noteRepository = new NoteRepository();

            // Act and Assert
            foreach (Note note in _insertedNotes)
            {
                Assert.That(
                    noteRepository.GetBy(
                        new string[] { "Id = @Id" },
                        new Dictionary<string, object> { { "@Id", note.Id } }).ToList().First().Id,
                    Is.EqualTo(note.Id));
            }
        }

        [Order(4)]
        [Test]
        public void Delete_ShouldReturn()
        {
            // Arrange
            NoteRepository noteRepository = new NoteRepository();

            // Act and Assert
            foreach (Note note in _insertedNotes)
            {
                Assert.That(noteRepository.Delete(note), Is.True);
            }
        }
    }
}
