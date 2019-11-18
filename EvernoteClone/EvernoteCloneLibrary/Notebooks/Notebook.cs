using EvernoteCloneLibrary.Files.Parsers;
using EvernoteCloneLibrary.Notebooks.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Notebooks
{
    public class Notebook : NotebookModel, IParseable
    {
        public List<INote> Notes { get; set; }

        public Notebook()
        {
            Notes = new List<INote>();
        }

        /// <summary>
        /// When there's more than 0 notes: [TheTitle] (n)
        /// Otherwise: [TheTitle]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => Title + (Notes.Count > 0 ? $" ({Notes.Count})" : "");

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
