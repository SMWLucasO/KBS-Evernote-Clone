using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using EvernoteCloneLibrary.Extensions;

namespace EvernoteCloneGUI.Helpers
{
    /// <summary>
    /// This class is used as a helper for ComboBoxes.
    /// It's general use is setting the width of the given ComboBox to the width of the biggest item
    /// </summary>
    public static class ComboBoxWidthFromItemsBehavior
    {
        /// <summary>
        /// 'Creates' a property for a given combobox
        /// </summary>
        public static readonly DependencyProperty ComboBoxWidthFromItemsProperty =
            DependencyProperty.RegisterAttached
            (
                "ComboBoxWidthFromItems",
                typeof(bool),
                typeof(ComboBoxWidthFromItemsBehavior),
                new UIPropertyMetadata(false, OnComboBoxWidthFromItemsPropertyChanged)
            );
        
        /// <summary>
        /// Returns whether the ComboBoxWidth is set according to its items
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetComboBoxWidthFromItems(DependencyObject obj)
        {
            return (bool)obj.GetValue(ComboBoxWidthFromItemsProperty);
        }
        
        /// <summary>
        /// Sets the ComboBoxWidth accordingly to its items
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetComboBoxWidthFromItems(DependencyObject obj, bool value)
        {
            obj.SetValue(ComboBoxWidthFromItemsProperty, value);
        }
        
        /// <summary>
        /// When the ComboBoxWidthFromItems property is changed, change the actual width of the ComboBox
        /// </summary>
        /// <param name="dpo"></param>
        /// <param name="e"></param>
        private static void OnComboBoxWidthFromItemsPropertyChanged(DependencyObject dpo,
            DependencyPropertyChangedEventArgs e)
        {
            if (dpo is ComboBox comboBox)
            {
                if ((bool)e.NewValue)
                {
                    comboBox.Loaded += OnComboBoxLoaded;
                }
                else
                {
                    comboBox.Loaded -= OnComboBoxLoaded;
                }
            }
        }
        
        /// <summary>
        /// When the ComboBox is loaded, call the SetWidthFromItems from the given ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnComboBoxLoaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            void Action()
            {
                comboBox.SetWidthFromItems();
            }

            comboBox?.Dispatcher?.BeginInvoke((Action) Action, DispatcherPriority.ContextIdle);
        }
    }
}