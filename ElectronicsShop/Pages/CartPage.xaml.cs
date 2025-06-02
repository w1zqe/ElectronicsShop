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
    public partial class CartPage : Page
    {
        private ElectronicsShopEntities _context = new ElectronicsShopEntities();
        private Users _currentUser;
        private List<Korzina> _cartItems;

        public CartPage(Users currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            LoadCart();
        }

        private void LoadCart()
        {
            _cartItems = _context.Korzina
                .Where(k => k.ID_User == _currentUser.ID_User)
                .ToList();

            CartItemsList.ItemsSource = _cartItems;

            decimal total = _cartItems.Sum(i => i.Product.Price * i.Quantity);
            TotalPriceText.Text = $"Итого: {total:C}";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is Korzina item)
            {
                item.Quantity++;
                _context.SaveChanges();
                LoadCart();
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is Korzina item)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                }
                else
                {
                    _context.Korzina.Remove(item);
                }
                _context.SaveChanges();
                LoadCart();
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is Korzina item)
            {
                _context.Korzina.Remove(item);
                _context.SaveChanges();
                LoadCart();
            }
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (_cartItems.Count == 0)
            {
                MessageBox.Show("Ваша корзина пуста.");
                return;
            }

            try
            {
                // Создание нового заказа
                var newOrder = new Orders
                {
                    ID_User = _currentUser.ID_User,
                    Data = DateTime.Now
                };

                _context.Orders.Add(newOrder);
                _context.SaveChanges(); // Сохраняем заказ, чтобы получить ID

                foreach (var item in _cartItems)
                {
                    var product = item.Product;

                    if (product.StockQ < item.Quantity)
                    {
                        MessageBox.Show($"Недостаточно товара \"{product.Name}\" на складе. Доступно: {product.StockQ}");
                        return;
                    }

                    // Добавляем детали заказа
                    var orderDetail = new OrdersPodr
                    {
                        ID_Orders = newOrder.ID_Order,
                        ID_Product = product.ID_Product,
                        Quantity = item.Quantity
                    };
                    _context.OrdersPodr.Add(orderDetail);

                    // Уменьшаем количество на складе
                    product.StockQ -= item.Quantity;
                }

                // Очищаем корзину
                _context.Korzina.RemoveRange(_cartItems);

                // Сохраняем все изменения
                _context.SaveChanges();

                MessageBox.Show("Заказ успешно оформлен!");
                LoadCart();
                NavigationService.Navigate(new OrdersPage(_currentUser));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при оформлении заказа: " + ex.Message);
            }
        }
    }
}