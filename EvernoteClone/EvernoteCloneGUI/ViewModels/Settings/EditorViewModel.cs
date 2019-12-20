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
            
        }

        private void LoadDefaultFontComboBox()
        {
            // Load all the fonts into the databound font list.
            foreach (FontFamily font in FontFamily.Families)
                ComboBoxHelper.AddItemToComboBox(ref DefaultFont, font, nameof(SettingsConstant.DEFAULT_FONT));
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
