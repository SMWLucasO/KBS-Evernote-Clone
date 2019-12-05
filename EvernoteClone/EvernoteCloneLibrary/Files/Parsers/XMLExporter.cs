using System;
using System.IO;

namespace EvernoteCloneLibrary.Files.Parsers
{
    /// <summary>
    /// Class for exporting parseable objects to their XML representation.
    /// </summary>
    public static class XmlExporter
    {
        /// <summary>
        /// Method to export notebooks to XML form.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="parseableObject"></param>
        /// <returns></returns>
        public static bool Export(string filePath, string fileName, IParseable parseableObject)
        {
            if (ValidateExportParameters(filePath, fileName, parseableObject))
            {
                string fullPath = $"{filePath}/{fileName}";

                if (TryGeneratePath(filePath))
                    File.WriteAllLines(fullPath, parseableObject.ToXmlRepresentation());
                return ValidateFileExists(fullPath);
            }
            return false;
        }

        #region Validation
        /// <summary>
        /// Validate if the input parameters aren't null (or empty)
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="filename"></param>
        /// <param name="parseableObject"></param>
        /// <returns></returns>
        private static bool ValidateExportParameters(string filePath, string filename, IParseable parseableObject) => 
            (!(string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(filename)) && parseableObject != null);

        /// <summary>
        /// Validator method for if the file exists.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        private static bool ValidateFileExists(string fullPath) =>
            File.Exists(fullPath) && Path.HasExtension(fullPath);
        #endregion
        #region Export path generation
        /// <summary>
        /// Try to generate the path. If it errors, return false.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool TryGeneratePath(string filePath)
        {
            try { return GeneratePath(filePath); }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// Generate the path if it does not exist, return a bool indicating whether the path was generated.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool GeneratePath(string filePath)
        {
            if (!(Directory.Exists(filePath)))
                Directory.CreateDirectory(filePath);
            return Directory.Exists(filePath);
        }
        #endregion

        public static bool Export(string filePath, string fileName, string[] content)
        {
            if (!(string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(fileName)))
            {
                string fullPath = $"{filePath}/{fileName}";
                if (Path.HasExtension(fullPath))
                {
                    if (!(Directory.Exists(filePath)))
                        Directory.CreateDirectory(filePath);
                    File.WriteAllLines(fullPath, content);

                    return File.Exists(fullPath);
                }
            }
            return false;
        }
    }
}
