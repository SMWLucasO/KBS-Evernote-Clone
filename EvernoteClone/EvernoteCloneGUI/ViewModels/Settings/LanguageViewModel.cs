using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using EvernoteCloneGUI.Views.Settings;
using EvernoteCloneLibrary.Constants;

namespace EvernoteCloneGUI.ViewModels.Settings
{
    public class LanguageViewModel : Screen
    {
        // TODO change this class when merged with actual language
        public ObservableCollection<string> Languages;

        public ComboBox LanguageComboBox;

        /// <summary>
        /// When the selectedIndex is changed, change the language accordingly
        /// </summary>
        /// <param name="sender">ComboBoxItem</param>
        /// <param name="args"></param>
        public void ComboBoxSelectedIndexChanged(object sender, RoutedEventArgs args)
        {
            if (sender is ComboBoxItem comboBoxItem)
            {
                object selectedLanguage = comboBoxItem.Tag;
                // TODO (make this work) Do something with language
                //Constant.DefaultLanguage = comboBoxItem.Tag;
            }
        }

        /// <summary>
        /// Select a item in the given ComboBox
        /// </summary>
        /// <param name="toSelectFrom">The ComboBox to select from</param>
        /// <param name="toSelect">The item that should be selected (comboBoxItem.Tag == toSelect)</param>
        public void SelectComboBoxItemByTag(ref ComboBox toSelectFrom, object toSelect)
        {
            foreach (ComboBoxItem comboBoxItem in toSelectFrom.Items)
            {
                if (comboBoxItem.Tag == toSelect)
                {
                    toSelectFrom.SelectedItem = comboBoxItem;
                }
            }
        }

        /// <summary>
        /// When this view is activated, load all languages
        /// </summary>
        protected override void OnActivate()
        {
            base.OnActivate();
            
            // TODO load Languages
        }

        /// <summary>
        /// When the view is attached, prepare the settings view for usage
        /// </summary>
        /// <param name="view"></param>
        /// <param name="context"></param>
        protected override void OnViewAttached(object view, object context)
        {
            if (view is LanguageView languageView)
            {
                LanguageComboBox = languageView.LanguageComboBox;
            }
            
            // TODO make this work
            //SelectComboBoxItemByTag(ref LanguageComboBox, Constant.DefaultLanguage);
        }
    }
}
