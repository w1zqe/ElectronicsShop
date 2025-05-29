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
    public partial class ProductPage : Page
    {
        private ElectronicsShopEntities _context = new ElectronicsShopEntities();
        private List<Product> _products;

        public ProductPage()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            _products = _context.Product.ToList();
            CategoryBox.ItemsSource = _context.Category.ToList();
            CategoryBox.SelectedIndex = -1;
            SortBox.SelectedIndex = 0;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            IEnumerable<Product> filtered = _products;

            string search = SearchBox.Text.ToLower();
            if (!string.IsNullOrWhiteSpace(search))
                filtered = filtered.Where(p => p.Name.ToLower().Contains(search));

            if (CategoryBox.SelectedItem is Category category)
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
    }
}