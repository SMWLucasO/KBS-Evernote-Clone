using System;
using System.Windows;
using System.Windows.Controls;
using EvernoteCloneLibrary.Constants;

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

            Setting setting = new Setting
            {
                setting = toSet,
                value = value
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
                Setting selectedSetting = (Setting)textBox.Tag;

                try
                {
                    selectedSetting.value = Convert.ChangeType(textBox.Text, selectedSetting.value.GetType());
                }
                catch (Exception)
                {
                    MessageBox.Show("The given input can only be of type "+selectedSetting.value.GetType(), "NoteFever | Settings", MessageBoxButton.OK, MessageBoxImage.Error);
                    textBox.Text = selectedSetting.value.ToString();
                    return;
                }

                SettingsConstant.SetValue(selectedSetting.setting, selectedSetting.value);
            }
        }
        
        #endregion
    }
}