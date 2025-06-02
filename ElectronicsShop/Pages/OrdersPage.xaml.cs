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
    public partial class OrdersPage : Page
    {
        private ElectronicsShopEntities _context = new ElectronicsShopEntities();
        private Users _currentUser;

        public OrdersPage(Users currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            LoadOrders();
        }

        private void LoadOrders()
        {
            var userOrders = _context.Orders
                .Where(o => o.ID_User == _currentUser.ID_User)
                .OrderByDescending(o => o.Data)
                .ToList();

            var displayOrders = userOrders.Select(order => new OrderDisplay
            {
                OrderHeader = $"Заказ от {order.Data.ToShortDateString()}",
                Items = order.OrdersPodr.Select(podr => new OrderItemDisplay
                {
                    ProductName = podr.Product.Name,
                    Quantity = podr.Quantity,
                    TotalPrice = podr.Product.Price * podr.Quantity
                }).ToList(),
                TotalOrderPrice = order.OrdersPodr.Sum(p => p.Product.Price * p.Quantity)
            }).ToList();

            OrdersList.ItemsSource = displayOrders;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserPage(_currentUser));
        }

        // Классы для отображения
        private class OrderDisplay
        {
            public string OrderHeader { get; set; }
            public List<OrderItemDisplay> Items { get; set; }
            public decimal TotalOrderPrice { get; set; }
        }

        private class OrderItemDisplay
        {
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public decimal TotalPrice { get; set; }
        }
    }
}