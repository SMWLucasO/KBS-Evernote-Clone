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
        /// <param name="ParseableObject"></param>
        /// <returns></returns>
        public static bool Export(string FilePath, string Filename, IParseable ParseableObject)
        {
            string fullPath = $"{FilePath}/{Filename}";
            File.WriteAllLines(fullPath, ParseableObject.ToXMLRepresentation());

            return File.Exists(fullPath);
        }
    }
}
