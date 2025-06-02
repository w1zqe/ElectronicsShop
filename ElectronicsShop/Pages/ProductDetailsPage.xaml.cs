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
    public partial class ProductDetailsPage : Page
    {
        private Product _product;

        public ProductDetailsPage(Product product)
        {
            InitializeComponent();
            _product = product;

            LoadProductData();
        }

        private void LoadProductData()
        {
            NameText.Text = _product.Name;
            DescriptionText.Text = _product.Descript;
            PriceText.Text = $"Цена: {_product.Price:C}";
            StockText.Text = $"В наличии: {_product.StockQ} шт.";
            BrandText.Text = $"Бренд: {_product.Brands?.Name}";
            CategoryText.Text = $"Категория: {_product.Category?.Name}";
            CountryText.Text = $"Страна: {_product.Country?.NameC}";

            try
            {
                ProductImage.Source = new BitmapImage(new Uri(_product.CurrentPhoto, UriKind.RelativeOrAbsolute));
            }
            catch
            {
                ProductImage.Source = new BitmapImage(new Uri("/Images/picture.jpg", UriKind.Relative));
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
