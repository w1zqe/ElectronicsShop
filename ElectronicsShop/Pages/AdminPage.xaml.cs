using ElectronicsShop.AppData;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ElectronicsShop.Pages
{
    public partial class AdminPage : Page
    {
        private ElectronicsShopEntities _context = new ElectronicsShopEntities();
        private List<Product> _products;
        private Product _selectedProduct;
        private Users _currentUser;


        public AdminPage(object currentUser)
        {
            InitializeComponent();
            
            LoadData();
            ProductList.SelectionChanged += ProductList_SelectionChanged;
            
        }

        private void LoadData()
        {
            _products = _context.Product.ToList();

            var allCategories = _context.Category.ToList();
            allCategories.Insert(0, new Category { ID_Category = 0, Name = "Все категории" });
            CategoryBox.ItemsSource = allCategories;
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

            if (CategoryBox.SelectedItem is Category category && category.ID_Category != 0)
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

            // Обновляем состояние кнопок
            UpdateButtonsState();
        }

        private void ProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedProduct = ProductList.SelectedItem as Product;
            UpdateButtonsState();
        }

        private void UpdateButtonsState()
        {
            bool isProductSelected = _selectedProduct != null;
            EditProduct.IsEnabled = isProductSelected;
            DeleteProduct.IsEnabled = isProductSelected;
        }

        private void Filter_Changed(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditPage());
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct != null)
            {
                NavigationService.Navigate(new AddEditPage(_selectedProduct));
            }
        }
        private void ProductList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var currentUser = AppConnect.CurrentUser as Users;
            if (ProductList.SelectedItem is Product selectedProduct)
            {
                NavigationService.Navigate(new ProductDetailsAdminPage(selectedProduct, currentUser));
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить товар '{_selectedProduct.Name}'?",
                                           "Подтверждение удаления",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Удаляем товар из контекста
                        _context.Product.Remove(_selectedProduct);

                        // Сохраняем изменения в БД
                        _context.SaveChanges();

                        // Удаляем товар из локального списка
                        _products.Remove(_selectedProduct);

                        // Обновляем отображение
                        ApplyFilters();

                        MessageBox.Show("Товар успешно удален!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении товара: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ExportToCSV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var productsToExport = ProductList.ItemsSource as IEnumerable<Product> ?? _products;

                SaveFileDialog saveDialog = new SaveFileDialog
                {
                    Filter = "CSV files (*.csv)|*.csv",
                    FileName = $"Товары_{DateTime.Now:yyyyMMddHHmmss}.csv"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    using (var writer = new System.IO.StreamWriter(saveDialog.FileName, false, Encoding.UTF8))
                    {
                        // Заголовки
                        writer.WriteLine("ID;Название;Описание;Цена;Количество;Бренд;Категория;Страна");

                        // Данные
                        foreach (var product in productsToExport)
                        {
                            writer.WriteLine(
                                $"{product.ID_Product};" +
                                $"\"{product.Name}\";" +
                                $"\"{product.Descript}\";" +
                                $"{product.Price};" +
                                $"{product.StockQ};" +
                                $"\"{product.Brands?.Name}\";" +
                                $"\"{product.Category?.Name}\";" +
                                $"\"{product.Country?.NameC}\"");
                        }
                    }

                    MessageBox.Show("Данные успешно экспортированы в CSV!", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте в CSV: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
       
    }
}