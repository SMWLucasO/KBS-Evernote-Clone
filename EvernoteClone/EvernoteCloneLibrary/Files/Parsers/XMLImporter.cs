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
                        List<Note> notes = GenerateNotesFromXml(xDocument, notebook);

                        // Add all the notes to the temporary notebook.
                        if (notes != null)
                        {
                            notebook.Notes.AddRange(notes);
                            notebooks.Add(notebook);
                        }

                    }
                }
                return notebooks;
            }

            return null;
        }

        // TODO: add summary
        public static List<NotebookLocation> ImportNotebookLocations(string FilePath)
        {
            if (!(string.IsNullOrEmpty(FilePath)))
            {
                List<NotebookLocation> notebookLocations = new List<NotebookLocation>();
                if (!File.Exists(FilePath))
                {
                    return null;
                }

                // load the XML from the path and parse it for usage
                XDocument xDocument = XDocument.Load(FilePath);
                foreach (XElement xElement in xDocument.Descendants("location"))
                {
                    notebookLocations.Add(new NotebookLocation
                    {
                        Id = Convert.ToInt32(xElement.Descendants().First(element => element.Name == "id").Value), 
                        Path = xElement.Descendants().First(element => element.Name == "path").Value
                    });
                }
                return notebookLocations;
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

                if (xDocument.Descendants("en-export") != null)
                {
                    foreach (XElement node in xDocument.Descendants("en-export").ToList())
                    {
                        if (node.Element("id")?.Value != null && node.Element("title")?.Value != null
                            && node.Element("path")?.Value != null && node.Element("path-id")?.Value != null)
                        {

                            return new Notebook
                            {
                                // The Id of the notebook, might be -1 if the notebook doesn't exist in the database
                                Id = int.Parse(node.Element("id").Value),
                                Title = node.Element("title").Value,
                                // location data
                                Path = new NotebookLocation()
                                {
                                    Id = int.Parse(node.Element("path-id").Value),
                                    Path = node.Element("path").Value
                                },
                                LocationID = int.Parse(node.Element("path-id").Value),
                                // File data which applies to the notebook.
                                CreationDate = File.GetCreationTime(FullPath),
                                LastUpdated = File.GetLastWriteTime(FullPath),
                                FSName = Path.GetFileNameWithoutExtension(FullPath)
                            };
                        }

                    }

                }
            }
            return null;
        }

        /// <summary>
        /// Helper method which generates Note objects with the available information.
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        private static List<Note> GenerateNotesFromXml(XDocument xDocument, Notebook Notebook)
        {
            if (xDocument != null)
            {
                if (xDocument.Descendants("en-export") != null && xDocument.Descendants("en-export").Descendants("note") != null)
                {
                    List<Note> notes = new List<Note>();
                    foreach (XElement node in xDocument.Descendants("en-export").Descendants("note"))
                    {
                        Note note = new Note
                        {
                            // Fetch the Id for the import, if there is none, or it is -1: This note is not in the database.
                            Id = (node.Element("id") != null ? (int)node.Element("id") : -1),
                            // Fetch the title of note
                            Title = node.Element("title")?.Value
                        };

                        // fetch the content of the note
                        note.Content = note.NewContent = GetStrippedContent(node.Element("content")?.Value) ?? "";

                        // fetch the date the note was created, needed to change it from 'T00000000Z000000' where '0' is an arbitrary value
                        if (node.Element("created")?.Value != null)
                        {
                            note.CreationDate = DateTime.Parse(FormatDateTime(node.Element("created").Value));
                        }

                        // fetch the date the note was last updated, needed to change it from 'T00000000Z000000' where '0' is an arbitrary value
                        if (node.Element("updated")?.Value != null)
                        {
                            note.LastUpdated = DateTime.Parse(FormatDateTime(node.Element("updated").Value));
                        }

                        // fetch the author of the note, the author lives in a subnode.
                        if (node.Element("note-attributes") != null && node.Element("note-attributes").Element("author")?.Value != null)
                        {
                            note.Author = node.Element("note-attributes").Element("author").Value;
                        }

                        // fetch all the tags of the note.
                        // There can be zero or more tags, therefore make sure it exists 
                        // & if so add them all the the tags list.
                        // If there is no tags, we will still load in an empty list to avoid nulls 
                        List<string> tags = new List<string>();
                        if (node.Elements("tag") != null)
                        {
                            foreach (string tag in node.Elements("tag").ToList())
                            {
                                tags.Add(tag);
                            }
                        }

                        note.Tags = tags;

                        // Set all the notebook data for the note
                        note.NoteOwner = Notebook;
                        note.NotebookID = Notebook.Id;

                        // If all required data is existent, then we add it to the list.
                        if (note.Author != null && note.CreationDate != null && note.LastUpdated != null
                            && note.Tags != null && note.NoteOwner != null)
                        {
                            notes.Add(note);
                        }
                    }

                    return notes;
                }
            }

            return null;
        }

        private static string GetStrippedContent(string Value)
            => Value.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?><!DOCTYPE en-note SYSTEM \"http://xml.evernote.com/pub/enml2.dtd\">", "")
                            .Replace("<en-note>", "")
                            .Replace("</en-note>", "");


        /// <summary>
        /// An ISO-8601 '00000000T000000Z' formatter.
        /// Converts the above to an appropriate DateTime (ex: 2019-07-05 05:40:53)
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