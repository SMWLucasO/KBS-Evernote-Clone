using System.Linq;
using System.Windows.Controls;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Settings;

namespace EvernoteCloneGUI.ViewModels.Controls.Settings
{
    /// <summary>
    /// This class contains all logic for ComboBoxes that are children of SettingsView.xaml
    /// </summary>
    public static class ComboBoxHelper
    {
        #region Methods
        
        /// <summary>
        /// Select a item in the given ComboBox
        /// </summary>
        /// <param name="toSelectFrom">The ComboBox to select from</param>
        /// <param name="toSelect">The item that should be selected (comboBoxItem.Tag == toSelect)</param>
        public static void SelectComboBoxItemByTag(ref ComboBox toSelectFrom, object toSelect)
        {
            foreach (ComboBoxItem comboBoxItem in toSelectFrom.Items)
            {
                if (toSelect is SettingModel setting)
                {
                    if (((SettingModel)comboBoxItem.Tag).SettingValue.ToString() == setting.SettingValue.ToString())
                    {
                        toSelectFrom.SelectedItem = comboBoxItem;
                    }
                }
                else
                {
                    if (((SettingModel)comboBoxItem.Tag).SettingValue.ToString() == toSelect.ToString())
                    {
                        toSelectFrom.SelectedItem = comboBoxItem;
                    }
                }
            }
        }

        /// <summary>
        /// Add an item to the ComboBox.
        /// </summary>
        /// <param name="toAddTo">The ComboBox the item should be added to</param>
        /// <param name="toAdd">The item that should be added</param>
        public static void AddItemToComboBox(ref ComboBox toAddTo, string toAdd) =>
            AddItemToComboBox(ref toAddTo, SettingsConstant.GetValue(toAdd), toAdd);

        /// <summary>
        /// Add an item to the ComboBox.
        /// </summary>
        /// <param name="toAddTo">The ComboBox the item should be added to</param>
        /// <param name="toAdd">The item that should be added</param>
        /// <param name="settingRepresentation">The SettingConstant value that should be changed upon selection</param>
        public static void AddItemToComboBox(ref ComboBox toAddTo, object toAdd, string settingRepresentation) =>
            AddItemToComboBox(ref toAddTo, toAdd, settingRepresentation, toAdd.ToString());

        /// <summary>
        /// Add an item to the ComboBox.
        /// </summary>
        /// <param name="toAddTo">The ComboBox the item should be added to</param>
        /// <param name="toAdd">The item that should be added</param>
        /// <param name="settingRepresentation">The SettingConstant value that should be changed upon selection</param>
        /// <param name="content">The comboBox display item content</param>
        public static void AddItemToComboBox(ref ComboBox toAddTo, object toAdd, string settingRepresentation, string content)
        {
            SettingModel setting = new SettingModel
            {
                KeyWord = settingRepresentation,
                SettingValue = toAdd
            };
            
            if (toAddTo.Items.Cast<ComboBoxItem>().Any(comboBoxItem => comboBoxItem.Content.ToString() == toAdd.ToString()))
            {
                return;
            }
            
            toAddTo.Items.Add(new ComboBoxItem { Content = content, Tag = setting });
        }

        /// <summary>
        /// Check if a item already exists in ComboBox (content equals content)
        /// </summary>
        /// <param name="comboBox">The ComboBox that we should check on</param>
        /// <param name="toCheck">The Content of the ComboBoxItem that we should check for</param>
        /// <returns>A boolean indicating if the item already exists in comboBox</returns>
        public static bool ItemExistsInComboBox(ref ComboBox comboBox, string toCheck) =>
            comboBox.Items.Cast<ComboBoxItem>().Any(comboBoxItem => comboBoxItem.Content == SettingsConstant.GetValue(toCheck));
        
        #endregion
        
        #region Events
        
        /// <summary>
        /// When the selectedIndex is changed, change the language accordingly
        /// </summary>
        /// <param name="sender">ComboBoxItem</param>
        public static void ComboBoxSelectedIndexChanged(object sender)
        {
            if (sender is ComboBoxItem comboBoxItem)
            {
                SettingModel selectedSetting = (SettingModel) comboBoxItem.Tag;
                SettingsConstant.SetValue(selectedSetting.KeyWord, selectedSetting.SettingValue);
            }
        }
        
        #endregion
    }
}