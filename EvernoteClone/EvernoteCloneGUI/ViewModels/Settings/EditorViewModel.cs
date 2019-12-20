using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using EvernoteCloneGUI.Views.Settings;
using System.Drawing;
using EvernoteCloneGUI.ViewModels.Controls.Settings;
using EvernoteCloneLibrary.Constants;

namespace EvernoteCloneGUI.ViewModels.Settings
{
    public class EditorViewModel : SettingViewModel
    {
        #region Variables

        /// <value>
        /// The ComboBox that contains all fonts
        /// </value>
        public ComboBox DefaultFont;
        
        /// <value>
        /// The ComboBox that contains all font sizes
        /// </value>
        public ComboBox DefaultFontSize;

        #endregion
        
        #region ComboBoxes

        private void LoadComboBoxes()
        {
            LoadDefaultFontComboBox();
            LoadDefaultFontSizeComboBox();
        }

        private void LoadDefaultFontComboBox()
        {
            // Load all the fonts into the databound font list.
            foreach (FontFamily font in FontFamily.Families)
                ComboBoxHelper.AddItemToComboBox(ref DefaultFont, font.Name, nameof(SettingsConstant.DEFAULT_FONT));
            
            // If used offline (or if something else happens) and no font is added, add standard font
            ComboBoxHelper.AddItemToComboBox(ref DefaultFont, nameof(SettingsConstant.DEFAULT_FONT));
            
            // Select standard font
            ComboBoxHelper.SelectComboBoxItemByTag(ref DefaultFont, SettingsConstant.DEFAULT_FONT);
        }
        
        private void LoadDefaultFontSizeComboBox()
        {
            // Load all the fonts into the databound font size list.
            for (int i = 1; i < 200; i++)
                ComboBoxHelper.AddItemToComboBox(ref DefaultFontSize, i, nameof(SettingsConstant.DEFAULT_FONT_SIZE));
            
            // If used offline (or if something else happens) and no font is added, add standard font
            ComboBoxHelper.AddItemToComboBox(ref DefaultFontSize, nameof(SettingsConstant.DEFAULT_FONT_SIZE));
            
            // Select standard font
            ComboBoxHelper.SelectComboBoxItemByTag(ref DefaultFontSize, SettingsConstant.DEFAULT_FONT_SIZE);
        }
        
        #endregion

        #region Events
        
        /// <summary>
        /// When the view is attached, prepare the editor view for usage
        /// </summary>
        /// <param name="view"></param>
        /// <param name="context"></param>
        protected override void OnViewAttached(object view, object context)
        {
            if (view is EditorView editorView)
            {
                DefaultFont = editorView.DefaultFont;
                DefaultFontSize = editorView.DefaultFontSize;
            }

            if (!Loaded)
            {
                LoadComboBoxes();
                Loaded = true;
            }
        }
        
        #endregion
    }
}
