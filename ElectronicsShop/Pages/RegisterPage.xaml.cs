using ElectronicsShop.AppData;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ElectronicsShop.Pages
{
    public partial class RegisterPage : Page
    {
        private ElectronicsShopEntities _context = new ElectronicsShopEntities();

        public RegisterPage()
        {
            InitializeComponent();
        }

        // Общие обработчики для TextBox
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var placeholder = FindName(textBox.Name + "Placeholder") as TextBlock;
                placeholder?.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var placeholder = FindName(textBox.Name + "Placeholder") as TextBlock;
                if (placeholder != null)
                {
                    placeholder.Visibility = string.IsNullOrEmpty(textBox.Text)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var placeholder = FindName(textBox.Name + "Placeholder") as TextBlock;
                placeholder?.SetCurrentValue(VisibilityProperty,
                    string.IsNullOrEmpty(textBox.Text) ? Visibility.Visible : Visibility.Collapsed);
            }
            ErrorMessage.Visibility = Visibility.Collapsed;
        }

        // Обработчики для PasswordBox
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                var placeholder = FindName(passwordBox.Name + "Placeholder") as TextBlock;
                placeholder?.SetCurrentValue(VisibilityProperty, Visibility.Collapsed);
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                var placeholder = FindName(passwordBox.Name + "Placeholder") as TextBlock;
                if (placeholder != null)
                {
                    placeholder.Visibility = string.IsNullOrEmpty(passwordBox.Password)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                var placeholder = FindName(passwordBox.Name + "Placeholder") as TextBlock;
                placeholder?.SetCurrentValue(VisibilityProperty,
                    string.IsNullOrEmpty(passwordBox.Password) ? Visibility.Visible : Visibility.Collapsed);
            }
            ErrorMessage.Visibility = Visibility.Collapsed;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Ваша логика регистрации...
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void ShowErrorMessage(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }
    }
}