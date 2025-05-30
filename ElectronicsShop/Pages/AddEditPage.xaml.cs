﻿using ElectronicsShop.AppData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Product _currentProduct = new Product();
        private bool _isEdit = false;

        public AddEditPage(Product selectedProduct = null)
        {
            InitializeComponent();

            if (selectedProduct != null)
            {
                _currentProduct = selectedProduct;
                _isEdit = true;
                Title = "Редактирование товара";
            }
            else
            {
                Title = "Добавление товара";
            }

            DataContext = _currentProduct;

            LoadComboBoxes();
            UpdatePlaceholders();
        }

        private void LoadComboBoxes()
        {
            try
            {
                using (var context = new ElectronicsShopEntities())
                {
                    CategoryComboBox.ItemsSource = context.Category.ToList();
                    BrandComboBox.ItemsSource = context.Brands.ToList();
                    CountryComboBox.ItemsSource = context.Country.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePlaceholders()
        {
            NamePlaceholder.Visibility = string.IsNullOrEmpty(NameBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            DescriptPlaceholder.Visibility = string.IsNullOrEmpty(DescriptBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            PricePlaceholder.Visibility = string.IsNullOrEmpty(PriceBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            StockQPlaceholder.Visibility = string.IsNullOrEmpty(StockQBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            ImagePlaceholder.Visibility = string.IsNullOrEmpty(ImageBox.Text) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePlaceholders();
        }

        private void ImageBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdatePlaceholders();
            _currentProduct.Image = ImageBox.Text;
        }

        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentProduct.Name))
                errors.AppendLine("Укажите название товара");
            if (string.IsNullOrWhiteSpace(_currentProduct.Descript))
                errors.AppendLine("Укажите описание товара");
            if (_currentProduct.Price <= 0)
                errors.AppendLine("Укажите цену товара");
            if (_currentProduct.StockQ < 0)
                errors.AppendLine("Укажите корректное количество на складе");
            if (_currentProduct.ID_Category == 0)
                errors.AppendLine("Выберите категорию");
            if (_currentProduct.ID_Brand == 0)
                errors.AppendLine("Выберите бренд");
            if (_currentProduct.ID_Country == 0)
                errors.AppendLine("Выберите страну");

            if (errors.Length > 0)
            {
                ErrorMessage.Text = errors.ToString();
                ErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                using (var context = new ElectronicsShopEntities())
                {
                    if (_isEdit)
                    {
                        var productToUpdate = context.Product.Find(_currentProduct.ID_Product);
                        if (productToUpdate != null)
                        {
                            productToUpdate.Name = _currentProduct.Name;
                            productToUpdate.Descript = _currentProduct.Descript;
                            productToUpdate.Price = _currentProduct.Price;
                            productToUpdate.StockQ = _currentProduct.StockQ;
                            productToUpdate.ID_Category = _currentProduct.ID_Category;
                            productToUpdate.ID_Brand = _currentProduct.ID_Brand;
                            productToUpdate.ID_Country = _currentProduct.ID_Country;
                            productToUpdate.Image = _currentProduct.Image;
                        }
                    }
                    else
                    {
                        context.Product.Add(_currentProduct);
                    }

                    context.SaveChanges();
                    MessageBox.Show("Данные сохранены успешно!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите отменить изменения?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                NavigationService.GoBack();
            }
        }
    }
}
