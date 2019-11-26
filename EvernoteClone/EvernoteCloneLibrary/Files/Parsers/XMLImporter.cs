using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using EvernoteCloneLibrary.Notebooks.Location;

namespace EvernoteCloneLibrary.Files.Parsers
{
    /// <summary>
    /// The class responsible for importing 
    /// </summary>
    public static class XMLImporter
    {
        /// <summary>
        /// Method for importing notebooks
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static List<Notebook> ImportNotebooks(string FilePath)
        {
            if (!(string.IsNullOrEmpty(FilePath)))
            {
                List<Notebook> notebooks = new List<Notebook>();
                if (!(Directory.Exists(FilePath)) || Directory.GetFiles(FilePath).Length == 0)
                {
                    return null;
                }

                foreach (string File in Directory.GetFiles(FilePath))
                {
                    // load the XML from the path and parse it for usage
                    XDocument xDocument = XDocument.Load(File);

                    // generate a notebook using the information from the file itself
                    Notebook notebook = GenerateNotebookFromFile(File, xDocument);

                    if (notebook != null)
                    {
                        List<Note> data = GenerateNotesFromXML(xDocument);

                        // Add all the notes to the temporary notebook.
                        if (data != null)
                        {
                            notebook.Notes.AddRange(data);
                            notebooks.Add(notebook);
                        }

                    }
                }
                return notebooks;
            }

            return null;
        }

        /// <summary>
        /// Helper method for generating a notebook out of existing data of the file specified by the path.
        /// </summary>
        /// <param name="FullPath"></param>
        /// <returns></returns>
        private static Notebook GenerateNotebookFromFile(string FullPath, XDocument xDocument)
        {
            if (FullPath != null && xDocument != null)
            {
                var notebookData = (from nbook in xDocument.Descendants("en-export")
                                    select new
                                    {
                                        Id = int.Parse(nbook.Element("id").Value),
                                        Title = nbook.Element("title").Value,
                                        Path = nbook.Element("path").Value,
                                        LocationID = int.Parse(nbook.Element("path-id").Value),
                                    }).First();

                Notebook notebook = new Notebook
                {
                    // The Id of the notebook, might be -1 if it doesn't exist in a database
                    Id = notebookData.Id,
                    // The Title of the notebook
                    Title = notebookData.Title,
                    // location data
                    Path = new NotebookLocation() { Id = notebookData.LocationID, Path = notebookData.Path },
                    LocationID = notebookData.LocationID,
                    // File data which applies to the notebook.
                    CreationDate = File.GetCreationTime(FullPath),
                    LastUpdated = File.GetLastWriteTime(FullPath),
                    FSName = Path.GetFileNameWithoutExtension(FullPath)
                };

                return notebook;
            }
            return null;
        }

        /// <summary>
        /// Helper method which generates Note objects with the available information.
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        private static List<Note> GenerateNotesFromXML(XDocument xDocument)
        {
            if (xDocument != null)
            {
                return (from note in xDocument.Descendants("en-export").Descendants("note")
                        select new Note
                        {
                            // Fetch the Id for the import, if there is none, or it is -1: It is not in the database.
                            Id = (note.Element("id") != null ? (int)note.Element("id") : -1),
                            // Fetch the title of note
                            Title = note.Element("title").Value,
                            // fetch the content of the note
                            Content = note.Element("content").Value,
                            // fetch the date the note was created, needed to change it from 'T00000000Z000000' where '0' is an arbitrary value
                            CreationDate = DateTime.Parse(FormatDateTime(note.Element("created").Value)),
                            // fetch the date the note was last updated, needed to change it from 'T00000000Z000000' where '0' is an arbitrary value
                            LastUpdated = DateTime.Parse(FormatDateTime(note.Element("updated").Value)),
                            // fetch the author of the note
                            Author = note.Element("note-attributes").Element("author").Value,
                            // fetch all the tags of the note, there are multiple <tag></tag> elements which we want to retrieve.
                            Tags = note.Elements("tag").Select(tag => tag.Value).ToList()
                        }).ToList();
            }

            return null;
        }

        /// <summary>
        /// An ISO-8601 '00000000T000000Z' formatter. 
        /// </summary>
        /// <param name="Datetime"</param>
        /// <returns></returns>
        private static string FormatDateTime(string Datetime)
        {
            if (Datetime != null)
            {
                if (Datetime.Length >= 16)
                {
                    string year = Datetime.Substring(0, 4);
                    string month = Datetime.Substring(4, 2);
                    string day = Datetime.Substring(6, 2);
                    string hour = Datetime.Substring(9, 2);
                    string minute = Datetime.Substring(11, 2);
                    string second = Datetime.Substring(13, 2);

                    return (year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second);
                }

                return DateTime.Now.ToString();
            }
            return null;
        }

    }
}