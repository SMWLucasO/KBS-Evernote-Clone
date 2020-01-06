using System.Windows;
using System.Windows.Controls;

namespace EvernoteCloneGUI.Helpers
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Makes the password box a dependency object
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password",
            typeof(string), typeof(PasswordHelper),
            new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach",
            typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
            typeof(PasswordHelper));

        /// <summary>
        /// Setters and getters to ensure that the password boxes are bindable and the passwords can be retrieved.
        /// </summary>
        public static void SetAttach(DependencyObject dp, bool value) =>
            dp.SetValue(AttachProperty, value);

        public static bool GetAttach(DependencyObject dp) =>
            (bool)dp.GetValue(AttachProperty);

        public static string GetPassword(DependencyObject dp) =>
            (string)dp.GetValue(PasswordProperty);

        public static void SetPassword(DependencyObject dp, string value) =>
            dp.SetValue(PasswordProperty, value);

        private static bool GetIsUpdating(DependencyObject dp) =>
            (bool)dp.GetValue(IsUpdatingProperty);

        private static void SetIsUpdating(DependencyObject dp, bool value) =>
            dp.SetValue(IsUpdatingProperty, value);

        /// <summary>
        /// Checks if the first password box had any changes. If there were any changes
        /// The password on the memory will be replaced.
        /// </summary>
        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordChanged;

                if (!GetIsUpdating(passwordBox))
                    passwordBox.Password = (string) e.NewValue;

                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        /// <summary>
        /// Attach makes the password box bindable. This way passwords that are typed can be safed on the memory
        /// for until the dialog with the password box is closed.
        /// </summary>
        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                if ((bool) e.OldValue)
                    passwordBox.PasswordChanged -= PasswordChanged;
                if ((bool) e.NewValue)
                    passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        /// <summary>
        /// Helper method for OnPasswordPropertyChanged to check if password had been changed.
        /// </summary>
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetIsUpdating(passwordBox, true);
                SetPassword(passwordBox, passwordBox.Password);
                SetIsUpdating(passwordBox, false);
            }
        }
    }
}
