using EvernoteCloneLibrary.Notebooks;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Windows;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Notebooks.Location;
using EvernoteCloneLibrary.Utils;

// TODO check summaries
namespace EvernoteCloneLibrary.Files.Parsers
{
    /// <summary>
    /// The class responsible for importing 
    /// </summary>
    public static class XmlImporter
    {

        /// <summary>
        /// Method for importing notebooks, returns null if there is no directory found that matches the FilePath.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<Notebook> TryImportNotebooks(string filePath)
        {
            try
            {
                return ImportNotebooks(filePath);
            }
            catch (DirectoryNotFoundException) { }

            return null;
        }

        /// <summary>
        /// Method for importing notebooks
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<Notebook> ImportNotebooks(string filePath)
        {
            if (!(string.IsNullOrEmpty(filePath)))
            {
                List<Notebook> notebooks = new List<Notebook>();
                if (!(ValidateFolderExistsNotEmpty(filePath)))
                {
                    return null;
                }

                foreach (string file in Directory.GetFiles(filePath))
                {
                    // load the XML from the path and parse it for usage
                    XDocument xDocument = XDocument.Load(file);

                    // generate a notebook using the information from the file itself
                    Notebook notebook = GenerateNotebookFromFile(file, xDocument);

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

        /// <summary>
        /// Retrieve the notebook locations from a given filePath
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<NotebookLocation> ImportNotebookLocations(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                List<NotebookLocation> notebookLocations = new List<NotebookLocation>();
                if (!File.Exists(filePath))
                {
                    return null;
                }

                // load the XML from the path and parse it for usage
                XDocument xDocument = XDocument.Load(filePath);
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

        public static bool ImportSettings(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && ValidateFileExists(filePath))
            {
                SettingsConstant settingsConstant = new SettingsConstant();
                var settings = SettingsConstant.GetSettings();
                
                // load the XML from the path and parse it for usage
                XDocument xDocument = XDocument.Load(filePath);
                foreach (string name in settings.Keys)
                {
                    if (xDocument.Descendants("en-export").Descendants().Any(element => element.Name == name))
                    {
                        try
                        {
                            settingsConstant.GetType().GetField(name)
                                .SetValue(settingsConstant,
                                    Convert.ChangeType(
                                        xDocument.Descendants("en-export").Descendants()
                                            .First(element => element.Name == name).Value, settings[name].GetType()));
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("There went something wrong while importing the settings stored locally, don't change these values on your own!",
                                "NoteFever | Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                return true;
            }
            return false;
        }

        #region Methods for generating objects from XML

        /// <summary>
        /// Helper method for generating a notebook out of existing data of the file specified by the path.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        private static Notebook GenerateNotebookFromFile(string fullPath, XDocument xDocument)
        {
            if (fullPath != null && xDocument != null && xDocument.Descendants("en-export") != null)
            {
                foreach (XElement node in xDocument.Descendants("en-export").ToList())
                {
                    // Check if the required elements are not null
                    if (ValidationUtil.AreNotNull(node.Element("id")?.Value, node.Element("title")?.Value,
                        node.Element("path")?.Value, node.Element("path-id")?.Value,
                        node.Element("created")?.Value, node.Element("updated")?.Value,
                        node.Element("deleted")?.Value))
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
                            LocationId = int.Parse(node.Element("path-id").Value),
                            // File data which applies to the notebook.
                            CreationDate = DateTime.Parse(FormatDateTime(node.Element("created").Value)),
                            LastUpdated = DateTime.Parse(FormatDateTime(node.Element("updated").Value)),
                            IsDeleted = bool.Parse(node.Element("deleted").Value),
                            FsName = Path.GetFileNameWithoutExtension(fullPath)
                        };
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
        private static List<Note> GenerateNotesFromXml(XDocument xDocument, Notebook notebook)
        {

            if (ValidationUtil.AreNotNull(xDocument, xDocument.Descendants("en-export"),
                xDocument.Descendants("en-export").Descendants("note")))
            {
                List<Note> notes = new List<Note>();
                foreach (XElement node in xDocument.Descendants("en-export").Descendants("note"))
                {

                    // If all required data is existent, then we (eventually) add it to the list.
                    if (ValidationUtil.AreNotNull(node.Element("created")?.Value, node.Element("updated")?.Value, node.Element("note-attributes"),
                        node.Element("note-attributes").Element("author")?.Value, node.Element("id")?.Value, node.Element("title")?.Value,
                        node.Element("note-attributes").Element("deleted")?.Value, notebook))
                    {
                        Note note = new Note
                        {
                            // Fetch the Id for the import, if there is none, or it is -1: This note is not in the database.
                            Id = (node.Element("id") != null ? int.Parse(node.Element("id").Value) : -1),
                            // Fetch the title of note
                            Title = node.Element("title")?.Value
                        };

                        // fetch the content of the note
                        note.Content = note.NewContent = GetStrippedContent(node.Element("content")?.Value) ?? "";

                        // fetch the date the note was created, needed to change it from 'T00000000Z000000' where '0' is an arbitrary value
                        note.CreationDate = DateTime.Parse(FormatDateTime(node.Element("created").Value));

                        // fetch the date the note was last updated, needed to change it from 'T00000000Z000000' where '0' is an arbitrary value
                        note.LastUpdated = DateTime.Parse(FormatDateTime(node.Element("updated").Value));

                        // fetch the author of the note and a bool to check if it is soft-deleted. Both nodes are attributes of a note.
                        note.Author = node.Element("note-attributes").Element("author").Value;
                        note.IsDeleted = bool.Parse(node.Element("note-attributes").Element("deleted").Value);

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
                        note.NoteOwner = notebook;
                        note.NotebookId = notebook.Id;

                        if (ValidationUtil.IsNotNull(note.Tags))
                        {
                            notes.Add(note);
                        }

                    }

                }

                return notes;
            }

            return null;
        }

        #endregion

        #region Validation methods
        
        private static bool ValidateFolderExistsNotEmpty(string filePath)
        {
            return Directory.Exists(filePath) && Directory.GetFiles(filePath).Length > 0;
        }
        
        /// <summary>
        /// Validator method for if the file exists.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        private static bool ValidateFileExists(string fullPath) =>
            File.Exists(fullPath) && Path.HasExtension(fullPath);

        #endregion

        #region Helper methods
        
        private static string GetStrippedContent(string value)
            => value.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?><!DOCTYPE en-note SYSTEM \"http://xml.evernote.com/pub/enml2.dtd\">", "")
                            .Replace("<en-note>", "")
                            .Replace("</en-note>", "")
                            .Replace("'\r", "");


        /// <summary>
        /// An ISO-8601 '00000000T000000Z' formatter.
        /// Converts the above to an appropriate DateTime (ex: 2019-07-05 05:40:53)
        /// </summary>
        /// <param name="datetime"</param>
        /// <returns></returns>
        private static string FormatDateTime(string datetime)
        {
            if (datetime != null)
            {
                if (datetime.Length >= 16)
                {
                    string year = datetime.Substring(0, 4);
                    string month = datetime.Substring(4, 2);
                    string day = datetime.Substring(6, 2);
                    string hour = datetime.Substring(9, 2);
                    string minute = datetime.Substring(11, 2);
                    string second = datetime.Substring(13, 2);

                    return (year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second);
                }

                return DateTime.Now.ToString();
            }
            return null;
        }

        #endregion

    }
}