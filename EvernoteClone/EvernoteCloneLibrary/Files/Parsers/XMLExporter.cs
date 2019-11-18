using EvernoteCloneLibrary.Notebooks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvernoteCloneLibrary.Files.Parsers
{
    /// <summary>
    /// 
    /// </summary>
    public static class XMLExporter
    {
        // NOTE: This class can be further abstracted by changing Notebook notebook to IParseable item
        // or something alike.

        /// <summary>
        /// Method to export notebooks to XML form.
        /// </summary>
        /// <param name="PathName"></param>
        /// <param name="Filename"></param>
        /// <param name="Notebook"></param>
        /// <returns></returns>
        public static bool Export(string PathName, string Filename, Notebook Notebook)
        {
            string fullPath = $"{PathName}/{Filename}";
            File.WriteAllLines(fullPath, Notebook.ToXMLRepresentation());

            return File.Exists(fullPath);
        }
    }
}
