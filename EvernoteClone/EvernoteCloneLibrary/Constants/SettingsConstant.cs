using System;
using System.Collections.Generic;
using System.Reflection;
using EvernoteCloneLibrary.Files.Parsers;
using System.Drawing;
using System.Linq;
using EvernoteCloneLibrary.Settings.Locales;

namespace EvernoteCloneLibrary.Constants
{
    /// <summary>
    /// This class contains all settings (and some logic to retrieve these settings
    /// </summary>
    public class SettingsConstant : IParseable
    {
        #region Settings
        
        /// <value>
        /// This 'setting' is used to determine which setting is updated more recently.
        /// This way we can override the database or local settings accordingly
        /// </value>
        public static DateTime LASTUPDATED = DateTime.Parse("01/01/2000 00:00:00");
        
        /// <value>
        /// This sets the application language
        /// </value>
        public static string LANGUAGE = "en-US";

        /// <summary>
        /// The background color of buttons.
        /// Hexadecimal
        /// </summary>
        public static string BUTTON_BACKGROUND_COLOR = "#404040";

        /// <value>
        /// The accent color, used for active buttons and such.
        /// Hexadecimal
        /// </value>
        public static string ACCENT_COLOR = "#0052cc";

        /// <value>
        /// The background color of the settings view
        /// </value>
        public static string BACKGROUND_COLOR_SETTINGS = "#ffbfcd";
        
        /// <value>
        /// The default Note title
        /// </value>
        public static string DEFAULT_NOTE_TITLE = "Nameless note";

        /// <value>
        /// The default Notebook title
        /// </value>
        public static string DEFAULT_NOTEBOOK_TITLE = "Nameless notebook";

        /// <value>
        /// The default Label title
        /// </value>
        public static string DEFAULT_LABEL_TITLE = "Nameless label";
        
        /// <value>
        /// The default Font
        /// </value>
        public static string DEFAULT_FONT = FontFamily.Families.First(font => font.Name == "Arial").Name;

        /// <value>
        /// The default font size
        /// </value>
        public static int DEFAULT_FONT_SIZE = 11;
        
        #endregion

        #region Logic

        #region Constructor

        private SettingsConstant() { }

        #endregion
        
        #region Variables
        
        /// <value>
        /// The only instance of _settingsConstant.
        /// This is needed to get variable settings and names dynamically
        /// </value>
        private static readonly SettingsConstant _settingsConstant = new SettingsConstant();
        
        #endregion

        #region Methods
        
        /// <summary>
        /// Returns the value of the setting
        /// </summary>
        /// <param name="settingName">The name of the variable</param>
        /// <returns>An object (since these variables can be different types)</returns>
        public static object GetValue(string settingName)
        {
            return _settingsConstant.GetType().GetField(settingName).GetValue(_settingsConstant);
        }

        /// <summary>
        /// Sets the value of the setting
        /// </summary>
        /// <param name="settingName">The name of the variable</param>
        /// <param name="settingValue">The value of the variable</param>
        public static void SetValue(string settingName, object settingValue)
        {
            _settingsConstant.GetType().GetField(settingName)
                .SetValue(_settingsConstant, settingValue is Locale ? settingValue.ToString() : settingValue);
        }
        
        /// <summary>
        /// Get all settings variables
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// This is needed since we implement the IParseable interface
        /// </summary>
        /// <returns>Return static ToXmlRepresentation</returns>
        string[] IParseable.ToXmlRepresentation() =>
            ToXmlRepresentation();

        /// <summary>
        /// This creates an xml string array of all the settings
        /// </summary>
        /// <returns>A string array containing all xml that represents current settings</returns>
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
        
        #endregion
        
        #endregion
    }
}