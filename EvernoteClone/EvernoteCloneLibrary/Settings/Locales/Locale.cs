using System.Collections.Generic;
using System.Linq;
using EvernoteCloneLibrary.Constants;

namespace EvernoteCloneLibrary.Settings.Locales
{
    public class Locale : LocaleModel
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
            return AddNewLocale(this);
        }

        /// <summary>
        /// Delete this setting
        /// </summary>
        /// <returns>A boolean indicating whether Deleting this setting was successful or not</returns>
        public bool Delete() =>
            DeleteLocale(this);

        #endregion

        #region Static methods
        
        /// <summary>
        /// Returns all the Locales
        /// </summary>
        /// <returns>A List from Locales</returns>
        public static List<Locale> GetAllLocales() =>
             new LocaleRepository().GetAll().Select(el => (Locale)el).ToList();

        /// <summary>
        /// Insert a new Locale record
        /// </summary>
        /// <param name="locale">The to be inserted locale</param>
        /// <returns>A boolean indicating if the insert went successful</returns>
        public static bool AddNewLocale(LocaleModel locale) =>
            new LocaleRepository().Insert(locale);
        
        /// <summary>
        /// Delete a Locale record
        /// </summary>
        /// <param name="locale">The record that needs to be removed</param>
        /// <returns>A boolean indicating if it has been removed</returns>
        public static bool DeleteLocale(LocaleModel locale) =>
            new LocaleRepository().Delete(locale);

        /// <summary>
        /// Update a Locale
        /// </summary>
        /// <param name="locale">The locale record that should be updated</param>
        /// <returns>A boolean indicating if the update was successful or not</returns>
        public static bool Update(LocaleModel locale) =>
            new LocaleRepository().Update(locale);
        
        #endregion
    }
}