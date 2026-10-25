using ShoeApp.Models;
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

namespace ShoeApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        MainWindow mainWindow;
        public ProductPage()
        {
            InitializeComponent();

            mainWindow = Application.Current.MainWindow as MainWindow;
            Users currentUser = AppConnect.CurrentUser;

            if (currentUser != null)
            {
                filterPanel.Visibility = Visibility.Visible;
            }

            mainWindow.fullName.Text = currentUser?.FullName ?? "Гость";
            listProduct.ItemsSource = AppConnect.ShoeDB.Products.ToList();
            Load();
        }

        private void Load()
        {
            cmbSort.Items.Clear();
            cmbSort.Items.Add("Без сортировки");
            cmbSort.Items.Add("Цена ^");
            cmbSort.Items.Add("Цена v");
            cmbSort.SelectedIndex = 0;

            cmbFilter.Items.Clear();
            cmbFilter.Items.Add("Все категории");
            var categories = AppConnect.ShoeDB.Categories.ToList();

            foreach(var category in categories)
            {
                cmbFilter.Items.Add(category.name);
            }
            cmbSort.SelectedIndex = 0;
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            listProduct.ItemsSource = ProductFind();
        }

        private Products[] ProductFind()
        {
            var products = AppConnect.ShoeDB.Products.ToList();

            if (txtSearch.Text.Length > 0)
            {
                string text = txtSearch.Text.ToLower();
                products = products.Where(p => p.name.ToLower().Contains(text) || p.description.ToLower().Contains(text)).ToList();
            }
            if (cmbFilter.SelectedIndex > 0)
            {
                products = products.Where(p => p.categoryId == cmbFilter.SelectedIndex).ToList();
            }
            if(cmbSort.SelectedIndex == 1)
            {
                products = products.OrderBy(p => p.FinalPrice).ToList();
            }
            if (cmbSort.SelectedIndex == 2)
            {
                products = products.OrderByDescending(p => p.FinalPrice).ToList();
            }

            return products.ToArray();
        }

        private void ListProductMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {

        }

        private void ExitButton(object sender, RoutedEventArgs e)
        {

        }
    }
}
