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
    public partial class UserProfilePage : Page
    {
        private ElectronicsShopEntities _context = new ElectronicsShopEntities();
        private Users _currentUser;

        public UserProfilePage(Users currentUser)
        {
            InitializeComponent();
            _currentUser = _context.Users.FirstOrDefault(u => u.ID_User == currentUser.ID_User); // Загружаем пользователя из контекста
            LoadUserData();
        }

        private void LoadUserData()
        {
            if (_currentUser != null)
            {
                UserNameBox.Text = _currentUser.UserName;
                LoginBox.Text = _currentUser.Login;
                PasswordBox.Password = _currentUser.Password;
                PhoneBox.Text = _currentUser.Phone;
            }
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UserNameBox.Text) ||
                string.IsNullOrWhiteSpace(LoginBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password) ||
                string.IsNullOrWhiteSpace(PhoneBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            try
            {
                var userToUpdate = _context.Users.FirstOrDefault(u => u.ID_User == _currentUser.ID_User);

                if (userToUpdate != null)
                {
                    userToUpdate.UserName = UserNameBox.Text.Trim();
                    userToUpdate.Login = LoginBox.Text.Trim();
                    userToUpdate.Password = PasswordBox.Password.Trim();
                    userToUpdate.Phone = PhoneBox.Text.Trim();

                    _context.SaveChanges();
                    MessageBox.Show("Данные успешно сохранены!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении данных: " + ex.Message);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Properties["CurrentUser"] = null;
            NavigationService.Navigate(new LoginPage());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserPage(_currentUser));
        }
    }
}
