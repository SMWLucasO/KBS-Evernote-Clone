using EvernoteCloneLibrary.Files.Parsers;
using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Location;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if (notes < 0) return null;

            List<INote> innerNotes = new List<INote>();
            Notebook notebook = new Notebook()
            {
                Id = -1,
                LocationId = 1,
                Path = ((new NotebookLocationRepository()).GetBy(
                        new[] { "Id = @Id" },
                        new Dictionary<string, object> { { "@Id", 1 } }
                        ).Select(
                        el => new NotebookLocation { Id = el.Id, Path = el.Path }
                        )
                    ).First(),
                Title = "Notebook #1",
                CreationDate = DateTime.Now.Date,
                LastUpdated = DateTime.Now,
            };

            for (int j = 0; j < notes; j++)
            {
                Note note = new Note()
                {
                    Id = -1,
                    NotebookId = 1,
                    Title = $"Test #{j}",
                    Author = $"Some #{j}",
                    Content = $"Notebooks #{j}",
                    CreationDate = DateTime.Now.Date,
                    LastUpdated = DateTime.Now
                };

                innerNotes.Add(note);
            }

            notebook.Notes = innerNotes;

            return notebook;
        }

        /// <summary>
        /// Creates a new NotebookLocation with Path path and Id -1
        /// </summary>
        /// <param name="path">The path of the to be generated NotebookLocation</param>
        /// <returns>Returns a NotebookLocation</returns>
        public static NotebookLocation GenerateNotebookLocation(string path) => 
            new NotebookLocation { Id=-1, Path = path};
    }
}
