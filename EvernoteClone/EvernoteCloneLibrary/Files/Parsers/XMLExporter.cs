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
    /// Class for exporting parseable objects to their XML representation.
    /// </summary>
    public static class XMLExporter
    {

        /// <summary>
        /// Method to export notebooks to XML form.
        /// </summary>
        /// <param name="PathName"></param>
        /// <param name="Filename"></param>
        /// <param name="ParseableObject"></param>
        /// <returns></returns>
        public static bool Export(string FilePath, string Filename, IParseable ParseableObject)
        {
            if (ValidateExportParameters(FilePath, Filename, ParseableObject))
            {
                string fullPath = $"{FilePath}/{Filename}";

                if (TryGeneratePath(FilePath))
                {
                    File.WriteAllLines(fullPath, ParseableObject.ToXmlRepresentation());
                }

                return ValidateFileExists(fullPath);
            }
            return false;
        }

        #region Validation
        /// <summary>
        /// Validate if the input parameters aren't null (or empty)
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="Filename"></param>
        /// <param name="ParseableObject"></param>
        /// <returns></returns>
        private static bool ValidateExportParameters(string FilePath, string Filename, IParseable ParseableObject)
        {
            return (!(string.IsNullOrEmpty(FilePath) || string.IsNullOrEmpty(Filename)) && ParseableObject != null);
        }

        /// <summary>
        /// Validator method for if the file exists.
        /// </summary>
        /// <param name="FullPath"></param>
        /// <returns></returns>
        private static bool ValidateFileExists(string FullPath)
        {
            return File.Exists(FullPath) && Path.HasExtension(FullPath);
        }

        #endregion

        #region Export path generation

        /// <summary>
        /// Try to generate the path. If it errors, return false.
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        private static bool TryGeneratePath(string FilePath)
        {
            try
            {
                return GeneratePath(FilePath);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Generate the path if it does not exist, return a bool indicating whether the path was generated.
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        private static bool GeneratePath(string FilePath)
        {
            if (!(Directory.Exists(FilePath)))
            {
                Directory.CreateDirectory(FilePath);
            }

            return Directory.Exists(FilePath);
        }

        #endregion

    }
}
