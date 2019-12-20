using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using EvernoteCloneLibrary;

namespace EvernoteCloneGUI
{
    public class LanguageChanger
    {
        private string UserLang;
        private LanguageLoader LangLoad;

        public LanguageChanger(string _lang)
        {
            LangLoad = new LanguageLoader();
            UserLang = _lang;
            UpdateResxFile();
        }

        private void UpdateResxFile()
        {
            SortedList<string, string> LangDown = LangLoad.DownloadLanguage(UserLang);

            string path = "C:/git/EvernoteClone/EvernoteClone/EvernoteCloneGUI/Language.resx";
            ResXResourceReader reader = new ResXResourceReader(path);
            var writer = new ResXResourceWriter(path);

            //set resources in LangExisting from resx file
            if (reader != null)
            {
                foreach (KeyValuePair<string, string> pair in LangDown)
                {
                    writer.AddResource(pair.Key, pair.Value);
                }
                reader.Close();
            }
            writer.Generate();

            /*
            //set resources in LangExisting from resx file
            if (reader != null)
            {
                foreach (DictionaryEntry dic in reader)
                {
                    string _dicKey = dic.Key.ToString();
                    if(LangDown.ContainsKey(_dicKey))
                        writer.AddResource(_dicKey, LangDown[_dicKey]);
                }
                reader.Close();
            }
            writer.Generate();
            */
        }

    }
}