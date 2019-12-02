using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Files.Parsers;
using EvernoteCloneLibrary.Notebooks.Location;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebooks
{
    public class Notebook : NotebookModel, IParseable
    {
        private string _title = null;

        public List<INote> Notes { get; set; }

        public NotebookLocation Path { get; set; }
            = new NotebookLocation();

        public string FSName { get; set; }

        public bool IsNotNoteOwner { get; set; }

        /// <summary>
        /// When an empty title is given, we give a default title.
        /// </summary>
        public override string Title
        {
            get
            {
                // If we do Title = null, then it will give the default title.
                if (_title == null) Title = null;
                return _title;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _title = "Nameless notebook";
                }
                else
                {
                    _title = value;
                }
            }
        }

        public Notebook()
        {
            Notes = new List<INote>();
        }

        /// <summary>
        /// Load all the notebooks belonging to the specified user.
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public static List<Notebook> Load(int UserID = -1)
        {

            List<Notebook> notebooksToReturn = new List<Notebook>();

            // Load all the notebooks stored in the local storage
            List<Notebook> notebooksFromFileSystem = XMLImporter.TryImportNotebooks(GetStoragePath());

            // Load all the notebooks stored in the database, if the user has a proper ID.
            // Note: Should also verify using password hash, but that is a TODO. This part will be rewritten later on.
            if (UserID != -1)
            {
                NotebookRepository notebookRepository = new NotebookRepository();
                List<Notebook> notebooksFromDatabase = new List<Notebook>();

                foreach (NotebookModel model in notebookRepository.GetBy(
                    new string[] { "UserID = @UserID" },
                    new Dictionary<string, object>() { { "@UserID", UserID } }
                   ))
                {
                    notebooksFromDatabase.Add((Notebook)model);
                }

                // If there are notebooks in the database, we want to compare which was updated more recently.
                if (notebooksFromDatabase != null)
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
                                        fsNotebook.Save(UserID);
                                    }
                                    else
                                    {
                                        dbNotebook.FSName = fsNotebook.FSName;
                                        notebooksToReturn.Add(dbNotebook);

                                        // Update the local storage with the newer version
                                        UpdateLocalStorage(dbNotebook);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {

                        if (notebooksFromFileSystem != null && (notebooksFromDatabase == null || notebooksFromDatabase.Count == 0))
                        {
                            notebooksToReturn.AddRange(notebooksFromFileSystem);
                            if (UserID != -1)
                            {
                                // Update the records in the database 
                                foreach (Notebook notebook in notebooksFromFileSystem)
                                {
                                    notebook.Save(UserID);
                                }
                            }
                        }
                        else if (notebooksFromDatabase != null && (notebooksFromFileSystem == null || notebooksFromFileSystem.Count == 0))
                        {
                            notebooksToReturn.AddRange(notebooksFromDatabase);

                            // Store the notebooks locally in case the user is ever offline and wants to edit text
                            foreach (Notebook notebook in notebooksFromDatabase)
                            {
                                notebook.UserID = UserID;
                                UpdateLocalStorage(notebook);
                            }
                        }
                    }

                }
                else
                {
                    notebooksToReturn.AddRange(notebooksFromFileSystem);
                }

            }
            else
            {
                notebooksToReturn.AddRange(notebooksFromFileSystem);
            }

            return notebooksToReturn;
        }

        /// <summary>
        /// Save all the notebooks belonging to the specified user.
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public bool Save(int UserID = -1)
        {
            LastUpdated = DateTime.Now;

            List<int> savedNotebookIDs = new List<int>();
            bool storedLocally = false;

            if (IsNotNoteOwner)
            {
                foreach (Note note in this.Notes)
                {
                    if (note.NoteOwner != null && !(savedNotebookIDs.Contains(note.NoteOwner.Id)))
                    {
                        // should never happen, but just in case (would cause stackoverflows)
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

            if (UserID != -1)
            {
                try
                {

                    this.UserID = UserID;
                    NotebookRepository notebookRepository = new NotebookRepository();
                    NoteRepository noteRepository = new NoteRepository();

                    // If the Id is '-1', that means it is a new notebook. Thus it should be inserted instead of updated.
                    if (IsNotNoteOwner)
                    {
                        savedNotebookIDs.Clear();
                        foreach (Note note in this.Notes)
                        {
                            if (note.NoteOwner != null && !(savedNotebookIDs.Contains(note.NoteOwner.Id)))
                            {
                                // You should only be allowed to edit (not create new) notes if the notebook isn't the owner of the given notes.
                                if (note.NoteOwner.Id != -1)
                                {
                                    // Set the NotebookID just in case it was not set before.
                                    note.NotebookID = note.NoteOwner.Id;
                                    note.NoteOwner.Save();
                                    savedNotebookIDs.Add(note.NoteOwner.Id);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.Id != -1)
                        {
                            storedInTheCloud = notebookRepository.Update(this);
                        }
                        else
                        {
                            storedInTheCloud = notebookRepository.Insert(this);
                        }

                        foreach (Note note in this.Notes)
                        {
                            // Set the note's notebookID to the id of this notebook, in case it was -1 before.
                            note.NotebookID = this.Id;
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
                catch (Exception) { }
            }

            return storedInTheCloud || storedLocally;
        }


        /// <summary>
        /// A method for updating the local storage of notebooks.
        /// Generates the filenames & exports the contents of the notebook.
        /// </summary>
        /// <param name="Notebook"></param>
        /// <returns></returns>
        private static bool UpdateLocalStorage(Notebook Notebook)
        {
            if (Notebook != null)
            {
                if (Notebook.FSName == null)
                {
                    Notebook.FSName = $"{Guid.NewGuid()}";
                }

                return XMLExporter.Export(GetStoragePath(), $@"{Notebook.FSName}.enex", Notebook);

            }

            return false;
        }

        /// <summary>
        /// Get the storage path for saving notes and notebooks locally.
        /// </summary>
        /// <returns></returns>
        private static string GetStoragePath()
        {
            return Constant.TEST_MODE ? Constant.TEST_STORAGE_PATH : Constant.PRODUCTION_STORAGE_PATH;
        }

        /// <summary>
        /// When there's more than 0 notes: [TheTitle] (n)
        /// Otherwise: [TheTitle]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Title + (Notes.Count > 0 ? $" ({Notes.Count})" : "");
        }

        // Comparison methods

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (obj is Notebook notebook)
                {
                    if (notebook.Id != -1)
                    {
                        return notebook.Id == this.Id;
                    }
                    else
                    {
                        return base.Equals(obj);
                    }
                }
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

        // END comparison methods

        /// <summary>
        /// Method which returns the XML representation of this class.
        /// </summary>
        /// <returns></returns>
        public string[] ToXmlRepresentation()
        {
            List<string> XMLRepresentation = new List<string>()
            {
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
                "<!DOCTYPE en-export SYSTEM \"http://xml.evernote.com/pub/evernote-export2.dtd\">",
                $"<en-export export-date=\"{DateTime.Now.ToString("yyyyMMdd")}T{DateTime.Now.ToString("HHmmss")}Z\"",
                $" application=\"EvernoteClone/Windows\" version=\"6.x\">",
                $"<title>{Title}</title>",
                $"<id>{Id}</id>",
                $"<path-id>{Path.Id}</path-id>",
                $"<path>{Path.Path}</path>",
            };

            foreach (var Note in Notes)
            {
                XMLRepresentation.AddRange(Note.ToXmlRepresentation());
            }

            XMLRepresentation.Add("</en-export>");

            return XMLRepresentation.ToArray();
        }
    }
}
