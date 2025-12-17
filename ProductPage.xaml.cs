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

namespace Murzayanov41
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        private List<Product> selectedProducts = new List<Product>();
        private List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();

        private void UpdateProducts()
        {
            var currentServices = MurzayanovEntities.GetContext().Product.ToList();

            if (ComboType.SelectedIndex == 0)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 0 && Convert.ToInt32(p.ProductDiscountAmount) <= 100)).ToList();
            }
            if (ComboType.SelectedIndex == 1)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 0 && Convert.ToInt32(p.ProductDiscountAmount) < 10)).ToList();
            }
            if (ComboType.SelectedIndex == 2)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 10 && Convert.ToInt32(p.ProductDiscountAmount) < 15)).ToList();
            }
            if (ComboType.SelectedIndex == 3)
            {
                currentServices = currentServices.Where(p => (Convert.ToInt32(p.ProductDiscountAmount) >= 15 && Convert.ToInt32(p.ProductDiscountAmount) <= 100)).ToList();
            }

            currentServices = currentServices.Where(p => (p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower()))).ToList();

            ProductListView.ItemsSource = currentServices;

            if (RButtonUp.IsChecked.Value)
            {
                ProductListView.ItemsSource = currentServices.OrderBy(p => p.ProductCost).ToList();
            }
            if (RButtonDown.IsChecked.Value)
            {
                ProductListView.ItemsSource = currentServices.OrderByDescending(p => p.ProductCost).ToList();
            }
            
            TBlockProductsCount.Text = "Выведено элементов " + Convert.ToString(currentServices.Count) + " из " + Convert.ToString(MurzayanovEntities.GetContext().Product.Count());
        }

        public ProductPage(User user)
        {
            InitializeComponent();

            Manager.OrderButton = OrderButton;

            if (user != null)
            {
                FIOTBlock.Text = user.UserName + " " + user.UserSurname + " " + user.UserPatronymic;

                switch (user.UserRole)
                {
                    case 1:
                        RoleTBlock.Text = "Клиент";
                        break;
                    case 2:
                        RoleTBlock.Text = "Менеджер";
                        break;
                    case 3:
                        RoleTBlock.Text = "Администратор";
                        break;
                }
            }
            else
            {
                FIOTBlock.Text = "None";
                RoleTBlock.Text = "Клиент";
            }

            ComboType.SelectedIndex = 0;
            UpdateProducts();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProducts();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProducts();
        }

        private void ProductListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            if (ProductListView.SelectedIndex >= 0)
            {
                var prod = ProductListView.SelectedItem as Product;
                selectedProducts.Add(prod);

                var newOrderProd = new OrderProduct();
                newOrderProd.OrderID = 1;

                //Номер продукта в новую запись.
                newOrderProd.ProductArticleNumber = prod.ProductArticleNumber;
                newOrderProd.OrderProductCount = 1;

                //Проверим, есть ли уже такой заказ.
                var selOP = selectedOrderProducts.Where(p => Equals(p.ProductArticleNumber, prod.ProductArticleNumber));
                //MessageBox.Show(selOP.Count().ToString());
                if (selOP.Count() == 0)
                {
                    selectedOrderProducts.Add(newOrderProd);
                }
                else
                {
                    foreach (OrderProduct p in selectedOrderProducts)
                    {
                        if (p.ProductArticleNumber == prod.ProductArticleNumber)
                        {
                            p.OrderProductCount++;
                        }
                    }
                }
            }

            OrderButton.Visibility = Visibility.Visible;
            ProductListView.SelectedIndex = -1;
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            selectedProducts = selectedProducts.Distinct().ToList();
            OrderWindow orderWindow = new OrderWindow(selectedOrderProducts, selectedProducts, FIOTBlock.Text);
            orderWindow.ShowDialog();
        }
    }
}
