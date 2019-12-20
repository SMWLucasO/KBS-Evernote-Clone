using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Files.Parsers;

namespace EvernoteCloneLibrary.Settings
{
    /// <summary>
    /// A class that contains a few methods for managing settings
    /// </summary>
    public class Setting : SettingModel
    {
        #region Setting methods

        /// <summary>
        /// Try to update the setting, if update was not possible, it will be added.
        /// </summary>
        /// <returns>A boolean indicating whether Saving this setting was successful or not</returns>
        public bool Save()
        {
            if (Update(this))
                return true;
            return AddNewSetting(this);
        }

        /// <summary>
        /// Delete this setting
        /// </summary>
        /// <returns>A boolean indicating whether Deleting this setting was successful or not</returns>
        public bool Delete() =>
            DeleteSetting(this);

        #endregion

        #region Static methods

        /// <summary>
        /// Save the settings (locally and in database (if logged in))
        /// </summary>
        /// <returns>A boolean indicating whether saving the settings was a success or not</returns>
        public static bool SaveSettings(bool onlySaveLocally = false)
        {
            SettingsConstant.LASTUPDATED = DateTime.Now;

            bool savedLocally = XmlExporter.Export(StaticMethods.GetUserDataStoragePath(), "Settings.enex",
                SettingsConstant.ToXmlRepresentation());
            bool savedInCloud = false;

            if (Constant.User.Id != -1 && !onlySaveLocally)
            {
                List<Setting> settings = new List<Setting>();
                foreach (KeyValuePair<string, object> setting in SettingsConstant.GetSettings())
                {
                    settings.Add(
                        new Setting
                        {
                            UserId = Constant.User.Id,
                            KeyWord = setting.Key,
                            SettingValue = setting.Value
                        });
                }

                bool errorWhileSavingInCloud = false;
                foreach (Setting setting in settings)
                {
                    errorWhileSavingInCloud = !setting.Save();
                }

                savedInCloud = !errorWhileSavingInCloud;
            }

            return savedLocally || savedInCloud;
        }

        /// <summary>
        /// Loads the settings locally, and from database
        /// This is done based on last updated setting.
        /// If last updated is not in database, settings are not loaded from the database
        /// </summary>
        /// <returns>boolean indicating whether loading the settings was a success or not</returns>
        public static bool Load(bool onlyLoadLocally = false)
        {
            bool importedLocally = XmlImporter.ImportSettings(StaticMethods.GetUserDataStoragePath() + @"/Settings.enex");
            bool importedFromDatabase = false;

            if (Constant.User.Id != -1 && !onlyLoadLocally)
            {
                List<Setting> allSettingsFromDatabase = GetAllSettingsFromUser();
                DateTime? lastUpdatedFromDatabase = null;
                
                foreach (Setting setting in allSettingsFromDatabase)
                {
                    if (setting.KeyWord == "LASTUPDATED")
                    {
                        lastUpdatedFromDatabase = Convert.ToDateTime(setting.SettingValue);
                    }
                }

                if (lastUpdatedFromDatabase != null)
                {
                    if (lastUpdatedFromDatabase > SettingsConstant.LASTUPDATED)
                    {
                        Dictionary<string, object> settings = SettingsConstant.GetSettings();
                        importedFromDatabase = true;

                        foreach (Setting setting in allSettingsFromDatabase)
                        {
                            try
                            {
                                SettingsConstant.SetValue(setting.KeyWord,
                                    Convert.ChangeType(
                                        setting.SettingValue,
                                        settings[setting.KeyWord].GetType()));
                            }
                            catch (Exception)
                            {
                                MessageBox.Show(
                                    "There went something wrong while importing the settings stored in the database. Contact product owner.",
                                    "NoteFever | Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                importedFromDatabase = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        SaveSettings();
                    }
                }
            }

            if (importedFromDatabase)
            {
                SaveSettings();
            }

            return importedLocally || importedFromDatabase;
        }
        
        /// <summary>
        /// Returns all the Settings linked to the current User
        /// </summary>
        /// <returns>A List from Settings</returns>
        public static List<Setting> GetAllSettingsFromUser()
        {
            return new SettingRepository().GetBy(
                new[] { "UserID = @UserID" },
                new Dictionary<string, object>() { { "@UserID", Constant.User.Id } }
            ).Select(el => (Setting)el).ToList();
        }

        /// <summary>
        /// Insert a new Setting record
        /// </summary>
        /// <param name="setting">The to be inserted setting</param>
        /// <returns>A boolean indicating if the insert went successful</returns>
        public static bool AddNewSetting(SettingModel setting) =>
            new SettingRepository().Insert(setting);
        
        /// <summary>
        /// Delete a Setting record
        /// </summary>
        /// <param name="setting">The record that needs to be removed</param>
        /// <returns>A boolean indicating if it has been removed</returns>
        public static bool DeleteSetting(SettingModel setting) =>
            new SettingRepository().Delete(setting);

        /// <summary>
        /// Returns a Setting based on a Keyword and UserId
        /// </summary>
        /// <param name="keyWord">The KeyWord used to retrieve a specific Setting</param>
        /// <returns>The requested Setting</returns>
        public static Setting GetLocationUserByNotebookLocation(string keyWord)
        {
            List<Setting> settings = new SettingRepository().GetBy(
                new[] { "UserID = @UserID", "Keyword = @Keyword" },
                new Dictionary<string, object> { { "@UserID", Constant.User.Id }, { "@Keyword", keyWord } }
            ).Select(el => (Setting)el).ToList();

            if (settings.Count == 1)
                return settings[0];
            return null;
        }

        /// <summary>
        /// Update a Setting
        /// </summary>
        /// <param name="setting">The setting record that should be updated</param>
        /// <returns>A boolean indicating if the update was successful or not</returns>
        public static bool Update(SettingModel setting) =>
            new SettingRepository().Update(setting);
        
        #endregion
    }
}