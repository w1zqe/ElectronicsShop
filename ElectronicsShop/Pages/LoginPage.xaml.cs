using ElectronicsShop.AppData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ElectronicsShop.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        // Контекст базы данных
        private ElectronicsShopEntities _context = new ElectronicsShopEntities();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void LoginBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Управление видимостью плейсхолдера для логина
            LoginPlaceholder.Visibility = string.IsNullOrEmpty(LoginBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;

            // Скрываем сообщение об ошибке при изменении текста
            ErrorMessage.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            // Управление видимостью плейсхолдера для пароля
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;

            // Скрываем сообщение об ошибке при изменении пароля
            ErrorMessage.Visibility = Visibility.Collapsed;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            // Проверка на пустые поля
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                ShowErrorMessage("Пожалуйста, введите логин и пароль");
                return;
            }

            try
            {
                // Поиск пользователя в базе данных
                var user = _context.Users
                        .Include("Roles") // Используем строку для указания навигационного свойства
                        .FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user != null)
                {
                    // Сохраняем текущего пользователя в статическом свойстве
                    App.Current.Properties["CurrentUser"] = user;

                    // Перенаправляем в зависимости от роли
                    if (user.Role_ID == 1) // Администратор
                    {
                        NavigationService.Navigate(new AdminPage());
                    }
                    else // Обычный пользователь
                    {
                        NavigationService.Navigate(new UserPage());
                    }
                }
                else
                {
                    ShowErrorMessage("Неверный логин или пароль");
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Ошибка при подключении к базе данных: " + ex.Message);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу регистрации
            NavigationService.Navigate(new RegisterPage());
        }

        private void ShowErrorMessage(string message)
        {
            // Отображение сообщения об ошибке
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }
    }
}
