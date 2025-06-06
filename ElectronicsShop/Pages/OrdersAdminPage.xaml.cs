using ElectronicsShop.AppData;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ElectronicsShop.Pages
{
    public partial class OrdersAdminPage : Page
    {
        private readonly ElectronicsShopEntities _context = new ElectronicsShopEntities();

        public OrdersAdminPage()
        {
            InitializeComponent();
            LoadOrders();
        }

        // Загрузка списка заказов
        private void LoadOrders()
        {
            OrdersGrid.ItemsSource = _context.Orders
                .Include("Users") // Подгружаем связанного пользователя 
                .OrderByDescending(o => o.Data)
                .ToList();
        }

        // Просмотр состава заказа
        private void ShowOrderDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int orderId)
            {
                var orderDetails = _context.OrdersPodr
                    .Where(op => op.ID_Orders == orderId)
                    .Select(op => new
                    {
                        Товар = op.Product.Name,
                        Количество = op.Quantity,
                        Цена = op.Product.Price
                    })
                    .ToList();

                string details = string.Join("\n", orderDetails.Select(d => $"{d.Товар} x{d.Количество} ({d.Цена} руб.)"));
                MessageBox.Show(details, $"Состав заказа #{orderId}", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}