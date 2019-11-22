using EvernoteCloneLibrary.Files.Parsers;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvernoteCloneLibraryTests.TestHelpers
{
    public static class ObjectGenerator
    {
        /// <summary>
        /// A helper method for tests, used to generate notebooks with a specified amount of notes.
        /// '-1' means that null will be returned.
        /// </summary>
        /// <param name="notes"></param>
        /// <returns></returns>
        public static IParseable GenerateTestableNotebook(int notes)
        {
            if (notes == -1) return null;

            List<INote> innerNotes = new List<INote>();
            Notebook notebook = new Notebook()
            {
                Id = -1,
                LocationID = 1,
                Title = $"Notebook #{1}",
                CreationDate = DateTime.Now.Date,
                LastUpdated = DateTime.Now,
            };

            for (int j = 0; j < notes; j++)
            {
                Note note = new Note()
                {
                    Id = -1,
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
}
