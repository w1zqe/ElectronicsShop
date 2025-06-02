using ElectronicsShop.AppData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ElectronicsShop.Pages
{
    public partial class UserPage : Page
    {
        private ElectronicsShopEntities _context = new ElectronicsShopEntities();
        private List<Product> _products;

        private Users _currentUser; // ← задаётся извне, можно через конструктор

        public UserPage(Users CurrentUser)
        {
            InitializeComponent();
            _currentUser = CurrentUser;
            LoadData();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is Product product)
            {
                var existingItem = _context.Korzina.FirstOrDefault(k => k.ID_User == _currentUser.ID_User && k.ID_Product == product.ID_Product);

                if (existingItem != null)
                {
                    existingItem.Quantity++;
                }
                else
                {
                    _context.Korzina.Add(new Korzina
                    {
                        ID_User = _currentUser.ID_User,
                        ID_Product = product.ID_Product,
                        Quantity = 1
                    });
                }

                _context.SaveChanges();

                MessageBox.Show($"Товар \"{product.Name}\" добавлен в корзину.");
            }
        }

        private void LoadData()
        {
            _products = _context.Product.ToList();

            var categories = _context.Category.ToList();
            categories.Insert(0, new Category { ID_Category = -1, Name = "Все категории" });

            CategoryBox.ItemsSource = categories;
            CategoryBox.SelectedIndex = 0;
            SortBox.SelectedIndex = 0;

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<Product> filtered = _products;

            string search = SearchBox.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(search))
                filtered = filtered.Where(p => p.Name.ToLower().Contains(search));

            if (CategoryBox.SelectedItem is Category category && category.ID_Category != -1)
                filtered = filtered.Where(p => p.ID_Category == category.ID_Category);

            switch ((SortBox.SelectedItem as ComboBoxItem)?.Content.ToString())
            {
                case "По названию":
                    filtered = filtered.OrderBy(p => p.Name);
                    break;
                case "По цене (возр)":
                    filtered = filtered.OrderBy(p => p.Price);
                    break;
                case "По цене (убыв)":
                    filtered = filtered.OrderByDescending(p => p.Price);
                    break;
            }

            ProductList.ItemsSource = filtered.ToList();
            ProductCountText.Text = $"Найдено товаров: {filtered.Count()}";
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();

        private void ProductList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ProductList.SelectedItem is Product selectedProduct)
            {
                NavigationService.Navigate(new ProductDetailsPage(selectedProduct));
            }
        }
        private void Cart_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CartPage(_currentUser));
        }
        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new UserProfilePage(_currentUser));
        }

        

        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new OrdersPage(_currentUser));
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
        private void ProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Можно оставить пустым, если не используешь одиночный выбор
        }
    }
}