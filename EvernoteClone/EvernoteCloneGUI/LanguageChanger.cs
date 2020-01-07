using System.Collections.Generic;
using EvernoteCloneLibrary;

namespace EvernoteCloneGUI
{
    public static class LanguageChanger
    {
        private static string _userLang;

        public static void UpdateResxFile(string language)
        {
            _userLang = language;

            SortedList<string, string> downloadedLanguages = LanguageLoader.DownloadLanguage(_userLang);
            
            foreach (KeyValuePair<string, string> pair in downloadedLanguages)
            {
                Properties.Settings.Default[pair.Key] = pair.Value;
            }
        }
    }
}