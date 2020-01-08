using System;
using System.Collections.Generic;
using EvernoteCloneLibrary.Constants;
using System.Linq;

namespace EvernoteCloneLibrary.Notebooks.Notes
{
    /// <summary>
    /// The concrete class for notes.
    /// </summary>
    public class Note : NoteModel, INote
    {
        #region Variables
        
        /// <value>
        /// This contains the title of the note
        /// </value>
        private string _title = SettingsConstant.DEFAULT_NOTE_TITLE;
        
        /// <value>
        /// This contains the new content for this note
        /// </value>
        private string _newContent = "";

        #endregion
        
        #region Properties
        
        /// <value>
        /// Contains all the labels
        /// </value>
        public List<string> Labels { get; set; }

        /// <value>
        /// The notebook this note is in
        /// </value>
        public Notebook NoteOwner { get; set; }

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
            set => _title = string.IsNullOrWhiteSpace(value) ? SettingsConstant.DEFAULT_NOTE_TITLE : value;
        }

        /// <summary>
        /// This string should be modified when content changes, not the Content property.
        /// The Content property is specifically for saved content.
        /// </summary>
        public string NewContent
        {
            get
            {
                if (Content == null)
                {
                    Content = _newContent;
                }
                    
                return _newContent;
            }
            set => _newContent = value;
        }
        
        #endregion

        /// <summary>
        /// Method for saving this note.
        /// This updates the content and also the last updated 
        /// </summary>
        /// <returns></returns>
        public void Save()
        {
            Content = NewContent;
            LastUpdated = DateTime.Now;

            if (Id == -1)
            {
                CreationDate = DateTime.Now.Date;
            }
        }

        /// <summary>
        /// Delete the note permanently
        /// </summary>
        public void DeletePermanently()
        {
            NoteRepository repository = new NoteRepository();
            if(Id != -1)
            {
                repository.Delete(this);
            }
           
            NoteOwner.Save();
        }

        /// <summary>
        /// Returns a note by an specified Id (from the database)
        /// </summary>
        /// <param name="noteId">The Id of the note it should retrieve</param>
        /// <returns>A Note</returns>
        public static Note GetNoteFromDatabaseById(int noteId)
        {
            NoteRepository noteRepository = new NoteRepository();
            List<Note> notes = noteRepository.GetBy(
                new[] { "Id = @Id" },
                new Dictionary<string, object> { { "@Id", noteId } }
                ).Cast<Note>().ToList();

            if (notes.Count > 0)
            {
                return notes[0];
            }

            return null;
        }

        /// <summary>
        /// If the author is null, we will throw an exception.
        /// We don't have to care about the Title, because it will automatically be renamed when null.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(Author))
            {
                throw new InvalidOperationException("Author must exist and cannot be empty.");
            }
            
            return $"{Title} by {Author}";
        }

        /// <summary>
        /// Generate the XML representation of notes
        /// </summary>
        /// <returns></returns>
        public string[] ToXmlRepresentation()
        {
            return new[] {
                   "<note>",
                       $"<title>{Title}</title>",
                       $"<id>{Id}</id>",
                       $"<content>",
                            $"<![CDATA[<?xml version=\"1.0\" encoding=\"UTF-8\"?><!DOCTYPE en-note SYSTEM \"http://xml.evernote.com/pub/enml2.dtd\"><en-note>{Content}</en-note>]]></content>",
                       $"<created>{CreationDate:yyyyMMdd}T{CreationDate:HHmmss}Z</created>",
                       $"<updated>{LastUpdated:yyyyMMdd}T{LastUpdated:HHmmss}Z</updated>",
                       $"<note-attributes>",
                           $"<author>{Author}</author>",
                           $"<deleted>{IsDeleted}</deleted>",
                       "</note-attributes>",
                   $"</note>"
            };
        }
    }
}