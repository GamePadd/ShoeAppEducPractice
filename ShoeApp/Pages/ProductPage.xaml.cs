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
            listProduct.ItemsSource = AppConnect.FruktimDB.StockItems.ToList();
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
            var categories = AppConnect.FruktimDB.Categories.ToList();

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

        private StockItems[] ProductFind()
        {
            var items = AppConnect.FruktimDB.StockItems.ToList();

            if (txtSearch.Text.Length > 0)
            {
                string text = txtSearch.Text.ToLower();
                items = items.Where(p => p.Products.name.ToLower().Contains(text) || p.Products.description.ToLower().Contains(text)).ToList();
            }
            if (cmbFilter.SelectedIndex > 0)
            {
                items = items.Where(p => p.Products.categoryId == cmbFilter.SelectedIndex).ToList();
            }
            if(cmbSort.SelectedIndex == 1)
            {
                items = items.OrderBy(p => p.FinalPrice).ToList();
            }
            if (cmbSort.SelectedIndex == 2)
            {
                items = items.OrderByDescending(p => p.FinalPrice).ToList();
            }

            return items.ToArray();
        }

        private void ListProductMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            var curUser = AppConnect.CurrentUser;
            if ((curUser.roleId != 1 && curUser.roleId != 2) || curUser == null) return;

            var selectedItem = listProduct.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Не выбран товар для удаления!");
                return;
            }

            if (MessageBox.Show("Удалить выбранный товар?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes) return;

            try
            {
                var stockItem = selectedItem as StockItems;

                if (stockItem == null)
                {
                    MessageBox.Show("Не удалось определить продукт!");
                    return;
                }

                AppConnect.FruktimDB.StockItems.Remove(stockItem);
                AppConnect.FruktimDB.SaveChanges();

                listProduct.ItemsSource = AppConnect.FruktimDB.StockItems.ToList();

                MessageBox.Show("Товар удалён!");
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Критическая ошибка: {ex}");
            }
        }

        private void ExitButton(object sender, RoutedEventArgs e)
        {

        }
    }
}
