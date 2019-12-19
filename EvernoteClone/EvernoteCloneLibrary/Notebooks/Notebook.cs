using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Files.Parsers;
using EvernoteCloneLibrary.Notebooks.Location;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using EvernoteCloneLibrary.Extensions;
using System.IO;

// TODO add summary's
namespace EvernoteCloneLibrary.Notebooks
{
    public class Notebook : NotebookModel, IParseable
    {
        private string _title;

        public List<INote> Notes { get; set; }

        public NotebookLocation Path { get; set; } = new NotebookLocation();

        public string FsName { get; set; }

        public bool IsNotNoteOwner { get; set; }

        /// <summary>
        /// When an empty title is given, we give a default title.
        /// </summary>
        public override string Title
        {
            get
            {
                // If we do Title = null, then it will give the default title.
                if (_title == null)
                {
                    Title = null;
                }

                return _title;
            }
            set => _title = string.IsNullOrWhiteSpace(value) ? "Nameless notebook" : value;
        }

        public Notebook() =>
            Notes = new List<INote>();

        /// <summary>
        /// Load all the notebooks belonging to the specified user.
        /// </summary>
        /// <returns></returns>
        public static List<Notebook> Load()
        {
            int userId = Constant.User.Id;
            List<Notebook> notebooksToReturn = new List<Notebook>();

            // Load all the notebooks stored in the local storage
            List<Notebook> notebooksFromFileSystem = XmlImporter.TryImportNotebooks(StaticMethods.GetNotebookStoragePath());

            // Load all the notebooks stored in the database, if the user has a proper ID.
            // Note: Should also verify using password hash, but that is a TODO. This part will be rewritten later on.
            if (userId != -1)
            {
                NotebookRepository notebookRepository = new NotebookRepository();
                List<Notebook> notebooksFromDatabase = new List<Notebook>();

                foreach (NotebookModel model in notebookRepository.GetBy(
                    new[] { "UserID = @UserID" },
                    new Dictionary<string, object> { { "@UserID", userId } }))
                {
                    notebooksFromDatabase.Add((Notebook)model);
                }

                // If there are notebooks in the database, we want to compare which was updated more recently.
                if (notebooksFromDatabase.Count > 0)
                {
                    if (notebooksFromDatabase.Count > 0 && (notebooksFromFileSystem != null && notebooksFromFileSystem.Count > 0))
                    {
                        foreach (Notebook dbNotebook in notebooksFromDatabase)
                        {
                            foreach (Notebook fsNotebook in notebooksFromFileSystem)
                            {
                                // If they are both the same Id, we load the last updated one.
                                if (dbNotebook.Id == fsNotebook.Id)
                                {
                                    if (fsNotebook.LastUpdated > dbNotebook.LastUpdated)
                                    {
                                        notebooksToReturn.Add(fsNotebook);

                                        // Update the database with the newer version.
                                        fsNotebook.Save();
                                    }
                                    else
                                    {
                                        dbNotebook.FsName = fsNotebook.FsName;
                                        notebooksToReturn.Add(dbNotebook);

                                        // Update the local storage with the newer version
                                        UpdateLocalStorage(dbNotebook);
                                    }
                                }
                            }
                        }

                        foreach (Notebook dbNotebook in notebooksFromDatabase.Where(notebook => !notebooksToReturn.Contains(notebook)))
                        {
                            notebooksToReturn.Add(dbNotebook);
                            // Add notebook locally if user ever goes offline and still wants to use this application
                            UpdateLocalStorage(dbNotebook);
                        }

                        foreach (Notebook fsNotebook in notebooksFromFileSystem.Where(notebook => !notebooksToReturn.Contains(notebook)))
                        {
                            notebooksToReturn.Add(fsNotebook);

                            // forceInsert won't be used, because if you delete a notebook on a other client, you will upload it again since you force it to insert, even if the Id is not -1
                            // fsNotebook.Save(UserID, true);

                            // Check if the path-id of notebook is not -1 (is it is, update it, since it will cause an error with the server)
                            fsNotebook.UpdatePathId();

                            // Insert 'new' notebook, force insert
                            fsNotebook.Save();
                        }
                    }
                    else
                    {
                        if (notebooksFromFileSystem != null && (notebooksFromDatabase == null || notebooksFromDatabase.Count == 0))
                        {
                            notebooksToReturn.AddRange(notebooksFromFileSystem);
                            if (userId != -1)
                            {
                                // Update the records in the database 
                                foreach (Notebook notebook in notebooksFromFileSystem)
                                {
                                    notebook.Save();
                                }
                            }
                        }
                        else if (notebooksFromDatabase != null && (notebooksFromFileSystem == null || notebooksFromFileSystem.Count == 0))
                        {
                            notebooksToReturn.AddRange(notebooksFromDatabase);

                            // Store the notebooks locally in case the user is ever offline and wants to edit text
                            foreach (Notebook notebook in notebooksFromDatabase)
                            {
                                notebook.UserId = userId;
                                UpdateLocalStorage(notebook);
                            }
                        }
                    }

                }
                else if (notebooksFromFileSystem != null)
                {
                    // Update the records in the database 
                    foreach (Notebook notebook in notebooksFromFileSystem)
                    {
                        notebook.Save();
                    }

                    notebooksToReturn.AddRange(notebooksFromFileSystem);
                }
            }
            else if (notebooksFromFileSystem != null)
            {
                notebooksToReturn.AddRange(notebooksFromFileSystem);
            }

            return notebooksToReturn;
        }

        /// <summary>
        /// Update the path of the notebook
        /// </summary>
        /// <param name="userId"></param>
        public void UpdatePathId()
        {
            int userId = Constant.User.Id;
            
            if (userId != -1)
            {
                if (Path.Id == -1)
                {
                    Path = NotebookLocation.GetNotebookLocationByPath(Path.Path);
                }

                else
                {
                    NotebookLocation notebookLocation = NotebookLocation.GetNotebookLocationByPath(Path.Path);
                    if (Path.Id != notebookLocation.Id)
                    {
                        Path = notebookLocation;
                    }

                }
            }
        }

        public static List<Notebook> GetAllNotebooksFromDatabase()
        {
            NotebookRepository notebookRepository = new NotebookRepository();
            return notebookRepository.GetBy(
                new string[] { "UserID = @UserID" },
                new Dictionary<string, object>() { { "@UserID", Constant.User.Id } }
            ).Select((el) => ((Notebook)el)).ToList();
        }

        private static void LoadNotebook(Notebook fsNotebook, Notebook dbNotebook, List<Notebook> listToAddTo)
        {
            // If they both have the same Id, we load the last updated one.
            if (dbNotebook.Id == fsNotebook.Id)
            {
                listToAddTo.AddIfNotPresent(fsNotebook);
            }

            else if (dbNotebook.Path.Equals(fsNotebook.Path))
            {
                listToAddTo.AddIfNotPresent(fsNotebook.Update(dbNotebook.Id));
            }

            else if (dbNotebook.Path.Path == fsNotebook.Path.Path && dbNotebook.Title == fsNotebook.Title)
            {
                listToAddTo.AddIfNotPresent(fsNotebook.Update(dbNotebook.Id));
            }
        }

        public bool Update()
        {
            NotebookRepository notebookRepository = new NotebookRepository();
            
            bool updatedCloud = notebookRepository.Update(this);
            bool updatedLocally = XmlExporter.Export(StaticMethods.GetNotebookStoragePath(), $@"{FsName}.enex", this);

            return updatedCloud || updatedLocally;
        }

        private Notebook Update(int newId)
        {
            Id = newId;

            // If Path.Id equals -1 (and Id != -1, and thus the notebook is also stored in the database), update this to the database version
            if (Path.Id == -1 && Id != -1)
            {
                Path = NotebookLocation.GetNotebookLocationByPath(Path.Path);
            }

            return XmlExporter.Export(StaticMethods.GetNotebookStoragePath(), $@"{FsName}.enex", this) ? this : null;
        }

        public static int AddNewNotebookToDatabaseAndGetId(Notebook notebook)
        {
            int userId = Constant.User.Id;
            
            if (userId != -1)
            {
                new NotebookRepository().Insert(notebook);
            }

            return notebook.Id;
        }

        /// <summary>
        /// Save all the notebooks belonging to the specified user.
        /// </summary>
        /// <param name="forceInsert"></param>
        /// <returns></returns>
        public bool Save(bool forceInsert = false)
        {
            int userId = Constant.User.Id;
            
            // TODO revisit this method for check
            LastUpdated = DateTime.Now;

            List<int> savedNotebookIDs = new List<int>();
            bool storedLocally;

            if (IsNotNoteOwner)
            {
                foreach (Note note in Notes)
                {
                    if (note.NoteOwner != null && !(savedNotebookIDs.Contains(note.NoteOwner.Id)))
                    {
                        // should never happen, but just in case (would cause StackOverFlow)
                        if (!(note.NoteOwner.Equals(this)))
                        {
                            storedLocally = note.NoteOwner.Save();
                            savedNotebookIDs.Add(note.NoteOwner.Id);
                        }
                    }
                }
            }
            else
            {
                storedLocally = UpdateLocalStorage(this);
            }

            bool storedInTheCloud = false;

            if (userId != -1)
            {
                // TODO check if this try catch can be removed
                try
                {
                    this.UserId = userId;
                    NotebookRepository notebookRepository = new NotebookRepository();
                    NoteRepository noteRepository = new NoteRepository();

                    // If the Id is '-1', that means it is a new notebook. Thus it should be inserted instead of updated.
                    if (IsNotNoteOwner)
                    {
                        savedNotebookIDs.Clear();
                        foreach (Note note in Notes.Cast<Note>())
                        {
                            if (note.NoteOwner != null && !(savedNotebookIDs.Contains(note.NoteOwner.Id)))
                            {
                                // You should only be allowed to edit (not create new) notes if the notebook isn't the owner of the given notes.
                                if (note.NoteOwner.Id != -1)
                                {
                                    // Set the NotebookID just in case it was not set before.
                                    note.NotebookId = note.NoteOwner.Id;
                                    note.NoteOwner.Save();
                                    savedNotebookIDs.Add(note.NoteOwner.Id);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Id != -1 && !forceInsert)
                        {
                            storedInTheCloud = notebookRepository.Update(this);
                            if (!storedInTheCloud) // the note is (probably) removed on another client TODO: do something with the bin here!
                            {
                                if (!Title.EndsWith(" (notebook is removed)"))
                                {
                                    Title += " (notebook is removed)";
                                }
                            }
                        }
                        else
                        {
                            storedInTheCloud = notebookRepository.Insert(this);
                        }

                        foreach (Note note in Notes.Cast<Note>())
                        {
                            // Set the note's notebookID to the id of this notebook, in case it was -1 before.
                            note.NotebookId = Id;
                            if (note.Id == -1)
                            {
                                noteRepository.Insert(note);
                            }
                            else
                            {
                                noteRepository.Update(note);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            storedLocally = UpdateLocalStorage(this);

            return storedInTheCloud || storedLocally;
        }

        /// <summary>
        /// Delete the notebook, including its bindings, completely.
        /// TODO: @Jorisvos add deletion logic for folders here.
        /// </summary>
        public void DeletePermanently()
        {
            NotebookRepository notebookRepository = new NotebookRepository();
            if (Id != -1)
            {
                notebookRepository.Delete(this);
            }

            // Delete the notebook file from local storage.
            string path = StaticMethods.GetNotebookStoragePath() + $"/{this.FsName}.enex";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        /// <summary>
        /// A method for updating the local storage of notebooks.
        /// Generates the filenames & exports the contents of the notebook.
        /// </summary>
        /// <param name="notebook"></param>
        /// <returns></returns>
        private static bool UpdateLocalStorage(Notebook notebook)
        {
            if (notebook != null)
            {
                if (notebook.FsName == null)
                {
                    notebook.FsName = $"{Guid.NewGuid()}";
                }

                return XmlExporter.Export(StaticMethods.GetNotebookStoragePath(), $@"{notebook.FsName}.enex", notebook);
            }
            return false;
        }

        /// <summary>
        /// Generates a list of notes, bounded by a condition.
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<INote> RetrieveNoteList(Func<INote, bool> condition)
        {
            List<INote> notes = new List<INote>();

            foreach (Note note in Notes)
            {
                if (condition(note))
                {
                    notes.Add(note);
                }
            }

            return notes;
        }

        /// <summary>
        /// When there's more than 0 notes: [TheTitle] (n)
        /// Otherwise: [TheTitle]
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            Title + (Notes.Count > 0 ? $" ({Notes.Count})" : "");

        #region Comparison methods
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Notebook notebook)
            {
                if (notebook.Id != -1)
                    return notebook.Id == Id;
                return notebook.Path.Path == Path.Path && notebook.Title == Title;
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (Id != -1)
            {
                return Id;
            }

            return base.GetHashCode();
        }
        #endregion

        /// <summary>
        /// Method which returns the XML representation of this class.
        /// </summary>
        /// <returns></returns>
        public string[] ToXmlRepresentation()
        {
            List<string> xmlRepresentation = new List<string>()
            {
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
                "<!DOCTYPE en-export SYSTEM \"http://xml.evernote.com/pub/evernote-export2.dtd\">",
                $"<en-export export-date=\"{DateTime.Now.ToString("yyyyMMdd")}T{DateTime.Now.ToString("HHmmss")}Z\"",
                $" application=\"EvernoteClone/Windows\" version=\"6.x\">",
                $"<title>{Title}</title>",
                $"<deleted>{IsDeleted}</deleted>",
                $"<created>{CreationDate.ToString("yyyyMMdd")}T{CreationDate.ToString("HHmmss")}Z</created>",
                $"<updated>{LastUpdated.ToString("yyyyMMdd")}T{LastUpdated.ToString("HHmmss")}Z</updated>",
                $"<id>{Id}</id>",
                $"<path-id>{Path.Id}</path-id>",
                $"<path>{Path.Path}</path>",
            };

            foreach (var note in Notes)
            {
                xmlRepresentation.AddRange(note.ToXmlRepresentation());
            }

            xmlRepresentation.Add("</en-export>");

            return xmlRepresentation.ToArray();
        }
    }
}