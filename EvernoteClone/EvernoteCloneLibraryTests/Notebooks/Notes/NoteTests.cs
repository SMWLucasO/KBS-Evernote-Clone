using EvernoteCloneLibrary.Notebooks.Notes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvernoteCloneLibraryTests.Notebooks.Notes
{
    [TestFixture]
    class NoteTests
    {

        [TestCase("A title", "Author", "A title by Author")]
        [TestCase("", "Author", "Nameless note by Author")]
        [TestCase(null, "Author", "Nameless note by Author")]
        public void ToString_ShouldReturn(string title, string author, string expected)
        {
            // Arrange
            Note note = new Note() { 
                Title = title,
                Author = author
            };
            // Act and Assert
            Assert.That(note.ToString(), Is.EqualTo(expected));

        }

        [TestCase("A title", "", typeof(InvalidOperationException))]
        [TestCase("A title", null, typeof(InvalidOperationException))]
        public void ToString_ShouldError(string title, string author, Type exception)
        {
            // Arrange
            Note note = new Note()
            {
                Title = title,
                Author = author
            };

            // Act and Assert
            Assert.Throws(exception, () => note.ToString());

        }

    }
}
