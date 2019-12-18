using System.Collections.Generic;
using System.Linq;
using EvernoteCloneLibrary.Constants;

namespace EvernoteCloneLibrary.Setting
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