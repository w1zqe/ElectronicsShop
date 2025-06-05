using ElectronicsShop.AppData;
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
using System.IO;

namespace ElectronicsShop.Pages
{
    public partial class AddEditPage : Page
    {
        private Product _currentProduct;
        private bool _isEdit;

        public AddEditPage(Product selectedProduct = null)
        {
            InitializeComponent();

            _isEdit = selectedProduct != null;
            _currentProduct = selectedProduct ?? new Product();

            this.DataContext = _currentProduct;

            PageTitleTextBlock.Text = _isEdit ? "Редактирование товара" : "Добавление товара";

            LoadComboBoxes();       // 1. загружаем списки
            FillFields();           // 2. заполняем значения
            UpdatePlaceholders();   // 3. обновляем визуально
        }

        private List<Brands> _allBrands;

        private void LoadComboBoxes()
        {
            try
            {
                using (var context = new ElectronicsShopEntities())
                {
                    CategoryComboBox.ItemsSource = context.Category.ToList();
                    _allBrands = context.Brands.ToList();
                    CountryComboBox.ItemsSource = context.Country.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CategoryComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoryComboBox.SelectedValue is int selectedCategoryId)
            {
                if (_categoryBrandMap.TryGetValue(selectedCategoryId, out var brandIds))
                {
                    var filteredBrands = _allBrands.Where(b => brandIds.Contains(b.ID_Brand)).ToList();
                    BrandComboBox.ItemsSource = filteredBrands;

                    // если текущий бренд больше не доступен — сбросить выбор
                    if (BrandComboBox.SelectedValue is int selectedBrandId &&
                        !brandIds.Contains(selectedBrandId))
                    {
                        BrandComboBox.SelectedItem = null;
                    }
                }
            }
        }

        private void FillFields()
        {
            if (_isEdit)
            {
                NameBox.Text = _currentProduct.Name;
                DescriptBox.Text = _currentProduct.Descript;
                PriceBox.Text = _currentProduct.Price.ToString();
                StockQBox.Text = _currentProduct.StockQ.ToString();
                ImageBox.Text = _currentProduct.Image;
            }

            // Сначала выбрать категорию
            CategoryComboBox.SelectedValue = _currentProduct.ID_Category;

            // Обновить список брендов под эту категорию
            CategoryComboBox_SelectionChanged(null, null);

            BrandComboBox.SelectedValue = _currentProduct.ID_Brand;
            CountryComboBox.SelectedValue = _currentProduct.ID_Country;
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
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Обновляем значения
            _currentProduct.Name = NameBox.Text;
            _currentProduct.Descript = DescriptBox.Text;
            // Проверка существования изображения
            string imagesFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
            string imageFilePath = System.IO.Path.Combine(imagesFolderPath, ImageBox.Text);

            if (!File.Exists(imageFilePath))
            {
                // Если файл не найден — подставляем заглушку
                _currentProduct.Image = "picture.jpg";
            }
            else
            {
                _currentProduct.Image = ImageBox.Text;
            }

            

            if (decimal.TryParse(PriceBox.Text, out decimal price))
                _currentProduct.Price = price;

            if (int.TryParse(StockQBox.Text, out int stockQ))
                _currentProduct.StockQ = stockQ;

            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentProduct.Name))
                errors.AppendLine("Укажите название товара");
            if (string.IsNullOrWhiteSpace(_currentProduct.Descript))
                errors.AppendLine("Укажите описание товара");
            if (_currentProduct.Price <= 0)
                errors.AppendLine("Укажите цену товара");
            if (_currentProduct.StockQ < 0)
                errors.AppendLine("Укажите корректное количество на складе");
            if (CategoryComboBox.SelectedItem == null)
                errors.AppendLine("Выберите категорию");
            if (BrandComboBox.SelectedItem == null)
                errors.AppendLine("Выберите бренд");
            if (CountryComboBox.SelectedItem == null)
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
                        var productInDb = context.Product.FirstOrDefault(p => p.ID_Product == _currentProduct.ID_Product);

                        if (productInDb == null)
                        {
                            MessageBox.Show("Продукт не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        // Обновление
                        productInDb.Name = _currentProduct.Name;
                        productInDb.Descript = _currentProduct.Descript;
                        productInDb.Price = _currentProduct.Price;
                        productInDb.StockQ = _currentProduct.StockQ;
                        productInDb.Image = _currentProduct.Image;
                        productInDb.ID_Category = (int)CategoryComboBox.SelectedValue;
                        productInDb.ID_Brand = (int)BrandComboBox.SelectedValue;
                        productInDb.ID_Country = (int)CountryComboBox.SelectedValue;
                    }
                    else
                    {
                        _currentProduct.ID_Category = (int)CategoryComboBox.SelectedValue;
                        _currentProduct.ID_Brand = (int)BrandComboBox.SelectedValue;
                        _currentProduct.ID_Country = (int)CountryComboBox.SelectedValue;

                        context.Product.Add(_currentProduct);
                    }

                    context.SaveChanges();
                    MessageBox.Show("Данные успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigationService.Navigate(new AdminPage(AppConnect.CurrentUser));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.InnerException?.Message ?? ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private Dictionary<int, List<int>> _categoryBrandMap = new Dictionary<int, List<int>>
        {
            { 1, new List<int> { 1, 2, 3, 4 } },      // Смартфоны: Apple, Samsung, Xiaomi, Huawei
            { 2, new List<int> { 1, 7, 8 } },         // Ноутбуки: Apple, Asus, HP
            { 3, new List<int> { 5, 6 } },            // Телевизоры: Sony, LG
            { 4, new List<int> { 1, 3, 4, 5 } },      // Наушники: Apple, Xiaomi, Huawei, Sony
            { 5, new List<int> { 1, 2, 3, 4 } },      // Планшеты: Apple, Samsung, Xiaomi, Huawei
            { 6, new List<int> { 5 } },              // Фотоаппараты: Sony
            { 7, new List<int> { 5, 6 } },            // Игровые консоли: Sony, LG (условно)
            { 8, new List<int> { 7, 8 } }             // Комплектующие: Asus, HP
        };


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите отменить изменения?", "Отмена",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                NavigationService.GoBack();
            }
        }
    }
}
