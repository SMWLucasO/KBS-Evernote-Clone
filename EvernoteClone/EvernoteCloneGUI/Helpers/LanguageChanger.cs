﻿using System.Collections.Generic;
using System.Globalization;
using EvernoteCloneLibrary.Utils;

 namespace EvernoteCloneGUI.Helpers
{
    public static class LanguageChanger
    {
        /// <summary>
        /// Load the language specified by the LastSelectedLanguage setting
        /// </summary>
        public static void ChangeLanguage()
        {
            SortedList<string, string> downloadedLanguages = 
                LanguageLoader.DownloadLanguage(Properties.Settings.Default.LastSelectedLanguage);
            
            foreach (KeyValuePair<string, string> pair in downloadedLanguages)
            {
                Properties.Settings.Default[pair.Key] = pair.Value;
            }
            
            TranslationSource.Instance.CurrentCulture = new CultureInfo(Properties.Settings.Default.LastSelectedLanguage);
        }
    }
}