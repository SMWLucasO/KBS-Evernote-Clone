using EvernoteCloneLibrary.Files.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebooks.Notes
{
    /// <summary>
    /// The concrete class for notes.
    /// </summary>
    public class Note : NoteModel, INote
    {

        private string _title = "Nameless note";
        private string _newContent = "";

        public List<string> Tags { get; set; }

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
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _title = "Nameless note";
                }
                else
                {
                    _title = value;
                }
            }
        }

        /// <summary>
        /// This string should be modified when content changes, not the Content property.
        /// The Content property is specifically for saved content.
        /// </summary>
        public string NewContent {
            get
            {
                if (Content == null) Content = _newContent;
                return _newContent;
            }
            set
            {
                _newContent = value;
            }
        }

        public Note()
        {
           
        }

        /// <summary>
        /// Method for saving specific notes.
        /// </summary>
        /// <returns></returns>
        public void Save()
        {
            Content = NewContent;
            LastUpdated = DateTime.Now;
            if(Id == -1 || CreationDate == null)
            {
                CreationDate = DateTime.Now.Date;
            }
        }


        /// <summary>
        /// If the author is null, we will throw an exception.
        /// We don't have to care about the Title, because it will automatically be renamed when null.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Author))
                throw new InvalidOperationException("Author must exist and cannot be empty.");
            
            return $"{Title} by {Author}";
        }

        /// <summary>
        /// Generate the XML representation of notes
        /// </summary>
        /// <returns></returns>
        public string[] ToXmlRepresentation()
        {
            // TODO: add tag nodes

            return new string[] {
                   "<note>",
                       $"<title>{Title}</title>",
                       $"<id>{Id}</id>",
                       $"<content>",
                            $"<![CDATA[<?xml version=\"1.0\" encoding=\"UTF-8\"?><!DOCTYPE en-note SYSTEM \"http://xml.evernote.com/pub/enml2.dtd\">",
                            $"<en-note>{Content}</en-note>]]></content>",
                       $"<created>{CreationDate.ToString("yyyyMMdd")}T{CreationDate.ToString("HHmmss")}Z</created>",
                       $"<updated>{LastUpdated.ToString("yyyyMMdd")}T{LastUpdated.ToString("HHmmss")}Z</updated>",
                       $"<note-attributes>",
                           $"<author>{Author}</author>",
                       "</note-attributes>",
                   $"</note>"
            };
        }
    }
}
