using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Files.Parsers;
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
            List<Notebook> notebooksFromFileSystem = new List<Notebook>();
            foreach (string file in Directory.GetFiles(GetStoragePath()))
            {
                Notebook fsNotebook = (Notebook)XMLImporter.Import(GetStoragePath(), file);
                if (fsNotebook != null)
                {
                    notebooksFromFileSystem.Add(fsNotebook);
                }
            }

            // Load all the notebooks stored in the database, if the user has a proper ID.
            // Note: Should also verify using password hash, but that is a TODO. This part will be rewritten later on.
            if (UserID != -1)
            {
                NotebookRepository repository = new NotebookRepository();
                List<Notebook> notebooksFromDatabase = repository.GetBy(
                    new string[] { "UserID = @UserID" },
                    new Dictionary<string, object>() { { "@UserID", UserID } }
                   ).Select((el) => ((Notebook)el)).ToList();

                // If there are notebooks in the database, we want to compare which was updated more recently.
                if (notebooksFromDatabase != null)
                {
                    if (notebooksFromDatabase.Count > 0 && notebooksFromFileSystem.Count > 0)
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
                                        repository.Update(fsNotebook);
                                    }
                                    else
                                    {
                                        notebooksToReturn.Add(dbNotebook);

                                        // Update the local storage with the newer version
                                        XMLExporter.Export(GetStoragePath(), $"{dbNotebook.Title}.enex", dbNotebook);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (notebooksFromDatabase.Count == 0)
                        {
                            notebooksToReturn.AddRange(notebooksFromFileSystem);
                        }
                        else if (notebooksFromFileSystem.Count == 0)
                        {
                            notebooksToReturn.AddRange(notebooksFromDatabase);
                        }

                    }

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
            bool stored = XMLExporter.Export(GetStoragePath(), $"{Title}.enex", this);

            if (UserID != -1)
            {
                this.UserID = UserID;
                NotebookRepository notebookRepository = new NotebookRepository();
                NoteRepository noteRepository = new NoteRepository();

                // If the Id is '-1', that means it is a new notebook. Thus it should be inserted instead of updated.
                if (this.Id != -1)
                {
                    stored = notebookRepository.Update(this);
                }
                else
                {
                    stored = notebookRepository.Insert(this);
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

            return stored;
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

        /// <summary>
        /// Method which returns the XML representation of this class.
        /// </summary>
        /// <returns></returns>
        public string[] ToXMLRepresentation()
        {
            List<string> XMLRepresentation = new List<string>()
            {
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
                "<!DOCTYPE en-export SYSTEM \"http://xml.evernote.com/pub/evernote-export2.dtd\">",
                $"<en-export export-date=\"{DateTime.Now.ToString("yyyyMMdd")}T{DateTime.Now.ToString("HHmmss")}Z\"",
                " application=\"EvernoteClone/Windows\" version=\"6.x\">",
            };

            foreach (var Note in Notes)
            {
                XMLRepresentation.AddRange(Note.ToXMLRepresentation());
            }

            XMLRepresentation.Add("</en-export>");

            return XMLRepresentation.ToArray();
        }
    }
}
