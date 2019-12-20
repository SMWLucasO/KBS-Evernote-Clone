using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EvernoteCloneLibrary.Database;

namespace EvernoteCloneLibrary
{
    public class Language_Loader
    {

        public Language_Loader() { }

        public SortedList<string, string> DownloadLanguage(string _string)
        {
            SortedList<string, string> _result = new SortedList<string, string>();
            DataTable _download = DataAccess.Instance.GetLanguageTable(_string);

            foreach (DataRow row in _download.AsEnumerable())
                _result.Add(row["Keyword"].ToString(), row["Translation"].ToString());

            return _result;
        }
    }
}