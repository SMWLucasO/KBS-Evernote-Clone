using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using EvernoteCloneLibrary.Files.Parsers;

namespace EvernoteCloneLibrary.Constants
{
    public class SettingsConstant : IParseable
    {
        public static string LANGUAGE = "en-US";

        public static string BUTTON_BACKGROUND = "#404040";

        public static string BUTTON_BACKGROUND_ACTIVE = "#0052cc";
        
        //THEME
        
        //STANDARD NAMES (Nameless note, Nameless notebook, etc.)
        
        //STANDARD TEXT COLOR, FONT (STYLE, COLOR), ETC.

        public static Dictionary<string, object> GetSettings()
        {
            Dictionary<string, object> settings = new Dictionary<string, object>();
            
            SettingsConstant settingsConstant = new SettingsConstant();
            foreach (FieldInfo fieldInfo in settingsConstant.GetType().GetFields())
            {
                settings.Add(fieldInfo.Name, fieldInfo.GetValue(settingsConstant));
            }

            return settings;
        }

        string[] IParseable.ToXmlRepresentation() =>
            ToXmlRepresentation();

        public static string[] ToXmlRepresentation()
        {
            List<string> xmlRepresentation =  new List<string>
            {
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?>",
                $"<en-export export-date=\"{DateTime.Now:yyyyMMdd}T{DateTime.Now:HHmmss}Z\"",
                " application=\"EvernoteClone/Windows\" version=\"6.x\">"
            };
            
            SettingsConstant settingsConstant = new SettingsConstant();
            foreach (FieldInfo fieldInfo in settingsConstant.GetType().GetFields())
            {
                xmlRepresentation.Add($"<{fieldInfo.Name}>" +
                                                $"{fieldInfo.GetValue(settingsConstant)}" +
                                            $"</{fieldInfo.Name}>");
            }

            xmlRepresentation.Add("</en-export>");

            return xmlRepresentation.ToArray();
        }
    }
}