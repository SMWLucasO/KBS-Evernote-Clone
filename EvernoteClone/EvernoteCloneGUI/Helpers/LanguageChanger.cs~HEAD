﻿using System.Collections.Generic;
using System.Globalization;
using EvernoteCloneGUI.Helpers;
using EvernoteCloneLibrary;
 using EvernoteCloneLibrary.Utils;

 namespace EvernoteCloneGUI
{
    public static class LanguageChanger
    {
        public static void UpdateResxFile()
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