using System.Collections.Generic;
using System.Data;
using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary
{
    public static class LanguageLoader
    {
        public static SortedList<string, string> DownloadLanguage(string langCode)
        {
            SortedList<string, string> result = new SortedList<string, string>();
            DataTable download = DataAccess.Instance.GetLanguageTable(langCode);

            foreach (DataRow row in download.AsEnumerable())
                result.Add(row["Keyword"].ToString(), row["Translation"].ToString());

            return result;
        }
    }
}