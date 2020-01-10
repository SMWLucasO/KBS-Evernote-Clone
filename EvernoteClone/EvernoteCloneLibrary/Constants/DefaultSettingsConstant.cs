using System;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace EvernoteCloneLibrary.Constants
{
    /// <summary>
    /// This class is used to save all settings to database on creation of a user (this way we can't duplicate settings)
    /// </summary>
    public class DefaultSettingsConstant
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
        
        #region Method

        /// <summary>
        /// Copy the default settings (this is used when changing from user)
        /// </summary>
        public static void CopyDefaults()
        {
            DefaultSettingsConstant settingsConstant = new DefaultSettingsConstant();
            foreach (FieldInfo fieldInfo in settingsConstant.GetType().GetFields())
            {
                SettingsConstant.SetValue(fieldInfo.Name, fieldInfo.GetValue(settingsConstant));
            }
        }
        
        #endregion
    }
}