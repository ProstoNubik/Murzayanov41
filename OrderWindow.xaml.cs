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
using System.Windows.Shapes;

namespace Murzayanov41
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        List<OrderProduct> selectedOrderProducts = new List<OrderProduct>();
        List<Product> selectedProducts = new List<Product>();
        private Order currentOrder = new Order();
        private OrderProduct currentOrderProduct;

        private void UpdateCost()
        {
            decimal normal_sum = 0;
            double sum_with_amount = 0;

            foreach (Product p in selectedProducts)
            {
                normal_sum += (p.ProductCost * p.SelectedProductQuantity);
                sum_with_amount += Convert.ToDouble(p.ProductCost) * (Convert.ToDouble(100-p.ProductDiscountAmount)/100) * p.SelectedProductQuantity;
            }
            OrderCostTBlock.Text = normal_sum.ToString();
            OrderCostAmountTBlock.Text = sum_with_amount.ToString();
        }

        private void SetDeliveryDate()
        {
            if (selectedProducts.Where(p => p.ProductQuantityInStock > 3).Count() != selectedProducts.Count())
            {
                OrderDeliveryDatePicker.Text = DateTime.Now.AddDays(6).ToString();
            }
            else
            {
                OrderDeliveryDatePicker.Text = DateTime.Now.AddDays(3).ToString();
            }
        }

        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, string FIO)
        {
            InitializeComponent();

            var currentPickups = MurzayanovEntities.GetContext().PickUpPoint.ToList();
            PickupPointsComboBox.ItemsSource = currentPickups;

            FIOTBlock.Text = FIO;
            OrderIDTBlock.Text = (MurzayanovEntities.GetContext().Order.ToList().Count()+1).ToString();

            OrderListView.ItemsSource = selectedProducts;
            foreach (Product p in selectedProducts)
            {
                p.SelectedProductQuantity = 1;
                /*foreach (OrderProduct q in selectedOrderProducts) //Хрень какая-то
                {
                    if (p.ProductArticleNumber == q.ProductArticleNumber)
                    {
                        p.ProductQuantityInStock = q.OrderProductCount;
                    }
                }stet*/
            }

            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;
            OrderDatePicker.Text = DateTime.Now.ToString();
            SetDeliveryDate();
            UpdateCost();
        }

        private void ButtonMinus_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;

            if (prod.SelectedProductQuantity == 1)
            {
                selectedProducts.Remove(prod);

                selectedOrderProducts.Remove(selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber));

                if (selectedProducts.Count() == 0)
                {
                    Manager.OrderButton.Visibility = Visibility.Hidden;
                    MessageBox.Show("Окно закрыто, так как отсутствуют заказы.");
                    this.Close();
                }
            }
            else
            {
                prod.SelectedProductQuantity--;

                var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);

                int index = selectedOrderProducts.IndexOf(selectedOP);

                selectedOrderProducts[index].OrderProductCount--;
            }

            SetDeliveryDate();
            UpdateCost();

            OrderListView.Items.Refresh();
        }

        private void ButtonPlus_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.SelectedProductQuantity++;

            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);

            int index = selectedOrderProducts.IndexOf(selectedOP);

            selectedOrderProducts[index].OrderProductCount++;

            SetDeliveryDate();
            UpdateCost();
            OrderListView.Items.Refresh();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            currentOrder.OrderID = Convert.ToInt32(OrderIDTBlock.Text);
            currentOrder.OrderDate = Convert.ToDateTime(OrderDatePicker.Text);
            currentOrder.OrderDeliveryDate = Convert.ToDateTime(OrderDeliveryDatePicker.Text);
            currentOrder.PickUpPoint = MurzayanovEntities.GetContext().PickUpPoint.ToList().Where(p => p.PickUpPointID == PickupPointsComboBox.SelectedIndex).First();

            if (FIOTBlock.Text != "None")
            {
                currentOrder.OrderClientID = Manager.userID;
            }
            else
                currentOrder.OrderClientID = null;

            currentOrder.OrderReceiveCode = (MurzayanovEntities.GetContext().Order.ToList()[MurzayanovEntities.GetContext().Order.ToList().Count() - 1].OrderReceiveCode + 1);
            currentOrder.OrderStatus = "Новый";

            MurzayanovEntities.GetContext().Order.Add(currentOrder);

            foreach (Product p in selectedProducts)
            {
                currentOrderProduct = new OrderProduct();
                currentOrderProduct.OrderID = Convert.ToInt32(OrderIDTBlock.Text);
                currentOrderProduct.ProductArticleNumber = p.ProductArticleNumber;
                currentOrderProduct.OrderProductCount = p.SelectedProductQuantity;

                MurzayanovEntities.GetContext().OrderProduct.Add(currentOrderProduct);
            }

            MurzayanovEntities.GetContext().SaveChanges();

            MessageBox.Show("Заказ сохранён успешно!");
            Manager.OrderButton.Visibility = Visibility.Hidden;
            selectedProducts.Clear();
            selectedOrderProducts.Clear();
            this.Close();
        }
    }
}
