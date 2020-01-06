using System;
using System.Windows;
using System.Windows.Controls;
using EvernoteCloneLibrary.Constants;
using EvernoteCloneLibrary.Settings;

namespace EvernoteCloneGUI.ViewModels.Controls.Settings
{
    /// <summary>
    /// This class contains all logic for TextBoxes that are children of SettingsView.xaml
    /// </summary>
    public static class TextBoxHelper
    {
        #region Methods
        
        /// <summary>
        /// Set the Text of a TextBox, also set it's tag
        /// </summary>
        /// <param name="setTextFrom">The TextBox the Text should be set from</param>
        /// <param name="toSet">The SettingsConstant variable name that should be set</param>
        public static void SetTextBox(ref TextBox setTextFrom, string toSet)
        {
            object value = SettingsConstant.GetValue(toSet);

            SettingModel setting = new SettingModel
            {
                KeyWord = toSet,
                SettingValue = value
            };

            setTextFrom.Text = value.ToString();
            setTextFrom.Tag = setting;
        }
        
        #endregion
        
        #region Events
        
        /// <summary>
        /// Fired when text from TextBox is changed
        /// </summary>
        /// <param name="sender">The TextBox</param>
        public static void TextBoxTextChanged(object sender)
        {
            if (sender is TextBox textBox)
            {
                SettingModel selectedSetting = (SettingModel)textBox.Tag;

                try
                {
                    selectedSetting.SettingValue = Convert.ChangeType(textBox.Text, selectedSetting.SettingValue.GetType());
                }
                catch (Exception)
                {
                    MessageBox.Show("The given input can only be of type "+selectedSetting.SettingValue.GetType(), "NoteFever | Settings", MessageBoxButton.OK, MessageBoxImage.Error);
                    textBox.Text = selectedSetting.SettingValue.ToString();
                    return;
                }

                SettingsConstant.SetValue(selectedSetting.KeyWord, selectedSetting.SettingValue);
            }
        }
        
        #endregion
    }
}