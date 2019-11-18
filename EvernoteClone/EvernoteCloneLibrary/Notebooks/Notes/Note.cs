using EvernoteCloneLibrary.Files.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebooks.Notes
{
    public class Note : NoteModel, INote
    {
        public List<string> Tags { get; internal set; }

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
