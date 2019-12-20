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
        
        public static readonly DependencyProperty ComboBoxWidthFromItemsProperty =
            DependencyProperty.RegisterAttached
            (
                "ComboBoxWidthFromItems",
                typeof(bool),
                typeof(ComboBoxWidthFromItemsBehavior),
                new UIPropertyMetadata(false, OnComboBoxWidthFromItemsPropertyChanged)
            );
        public static bool GetComboBoxWidthFromItems(DependencyObject obj)
        {
            return (bool)obj.GetValue(ComboBoxWidthFromItemsProperty);
        }
        public static void SetComboBoxWidthFromItems(DependencyObject obj, bool value)
        {
            obj.SetValue(ComboBoxWidthFromItemsProperty, value);
        }
        private static void OnComboBoxWidthFromItemsPropertyChanged(DependencyObject dpo,
            DependencyPropertyChangedEventArgs e)
        {
            if (dpo is ComboBox comboBox)
            {
                if ((bool)e.NewValue == true)
                {
                    comboBox.Loaded += OnComboBoxLoaded;
                }
                else
                {
                    comboBox.Loaded -= OnComboBoxLoaded;
                }
            }
        }
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