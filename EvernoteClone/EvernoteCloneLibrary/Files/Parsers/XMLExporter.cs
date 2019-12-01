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

        // Will cause issues with duplicate named notebooks, thus TODO: Impl. folder structure through path @Lucas is this fixed? Or no?

        /// <summary>
        /// Method to export notebooks to XML form.
        /// </summary>
        /// <param name="PathName"></param>
        /// <param name="Filename"></param>
        /// <param name="ParseableObject"></param>
        /// <returns></returns>
        public static bool Export(string FilePath, string FileName, IParseable ParseableObject)
        {
            if (!(string.IsNullOrEmpty(FilePath) || string.IsNullOrEmpty(FileName)) && ParseableObject != null)
            {

                string fullPath = $"{FilePath}/{FileName}";
                if (Path.HasExtension(fullPath))
                {
                    if (!(Directory.Exists(FilePath)))
                    {
                        Directory.CreateDirectory(FilePath);
                    }

                    File.WriteAllLines(fullPath, ParseableObject.ToXmlRepresentation());

                    return File.Exists(fullPath);
                }


            }

            return false;
        }

        public static bool Export(string FilePath, string FileName, string[] content)
        {
            if (!(string.IsNullOrEmpty(FilePath) || string.IsNullOrEmpty(FileName)))
            {

                string fullPath = $"{FilePath}/{FileName}";
                if (Path.HasExtension(fullPath))
                {
                    if (!(Directory.Exists(FilePath)))
                    {
                        Directory.CreateDirectory(FilePath);
                    }

                    File.WriteAllLines(fullPath, content);

                    return File.Exists(fullPath);
                }


            }

            return false;
        }
    }
}
