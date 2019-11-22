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
        public List<string> Tags { get; set; }

        /// <summary>
        /// This string should be modified when content changes, not the Content property.
        /// The Content property is specifically for saved content.
        /// </summary>
        public string NewContent { get; set; }

        public Note()
        {
            // We want to remember the old content, saving should modify this part.
            NewContent = Content;
        }

        /// <summary>
        /// Method for saving specific notes.
        /// </summary>
        /// <returns></returns>
        public void Save()
        {
            // TODO update notebook last_updated component of the note.
            Content = NewContent;
        }


        public override string ToString() => $"{Title} by {Author}";

        /// <summary>
        /// Generate the XML representation of notes
        /// </summary>
        /// <returns></returns>
        public string[] ToXMLRepresentation()
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
