using Povarenok.AppData;
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

namespace Povarenok.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        User curUser = Rights.curUser;
        public ProductPage()
        {
            InitializeComponent();
            FilterCmb.ItemsSource = new string[]
            {
                "Все диапазоны",
                "0-9,99%",
                "10-14,99%",
                "15% и более"
            };
            SortCmb.ItemsSource = new string[]
            {
                "по возрастанию цены",
                "по убыванию цены"
            };
            if (curUser != null)
                CurrentUserTxt.Text = curUser.UserSurname + " " + curUser.UserName + " " + curUser.UserPatronymic;
            UpdateLV();
        }

        public void UpdateLV()
        {
            var prod = ConnectDB.GetCont().Product.ToList();
            string countDb = prod.Count.ToString();
            prod = prod.Where(x=> x.ProductName.Contains(SearchTxt.Text)).ToList();
            switch (FilterCmb.SelectedIndex)
            {
                case 1: prod = prod.Where(x => x.ProductDiscountAmount < 10).ToList();
                    break;
                case 2:
                    prod = prod.Where(x => x.ProductDiscountAmount < 15 && x.ProductDiscountAmount >= 10).ToList();
                    break;
                case 3:
                    prod = prod.Where(x => x.ProductDiscountAmount >= 15).ToList();
                    break;
            }
            CountRowTxt.Text = "Строк в БД: "+ prod.Count.ToString() + " из " + countDb;
            if (SortCmb.SelectedIndex == 0) prod = prod.OrderBy(x => x.ProductCost).ToList();
            else prod = prod.OrderByDescending(x => x.ProductCost).ToList();
            if (prod.Count==0)
            {
                MessageBox.Show("Данные не найдены");
            }
            ProdLV.ItemsSource = prod;
            if(NewOrder.order!=null)
                if (NewOrder.order.OrderProduct.ToList().Count > 0) Nav.Visible(AddOrderBtn);
                else Nav.Collapsed(AddOrderBtn);
            else Nav.Collapsed(AddOrderBtn);
            if (curUser != null)
            {
                if (curUser.UserRole == 1)
                {
                    Nav.Visible(AddProdBtn);
                    Nav.Visible(DelProd);
                    Nav.Visible(OrderBtn);
                }else
                if (curUser.UserRole == 3)
                {
                    Nav.Collapsed(AddProdBtn);
                    Nav.Collapsed(DelProd);
                    Nav.Visible(OrderBtn);
                }
                else
                {
                    Nav.Collapsed(AddProdBtn);
                    Nav.Collapsed(DelProd);
                    Nav.Collapsed(OrderBtn);
                }
            }
            else
            {
                Nav.Collapsed(AddProdBtn);
                Nav.Collapsed(DelProd);
                Nav.Collapsed(OrderBtn);
            }
        }

        private void AddProdBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MFrame.Navigate(new AddEditProduct(new Product()));
        }

        private void AddToOrder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var products = ProdLV.SelectedItems;
                if (NewOrder.order == null)
                {
                    NewOrder.order = new Order()
                    {
                        OrderStatus = "Новый"
                    };
                }
                foreach (var item in products)
                {
                    if(NewOrder.order.OrderProduct.Any(x=>x.Product == item))
                    {
                        NewOrder.order.OrderProduct.FirstOrDefault(x => x.Product.ProductArticleNumber == (item as Product).ProductArticleNumber).Quantity++;
                    }
                    else
                        NewOrder.order.OrderProduct.Add(new OrderProduct 
                        { 
                            Product = item as Product, 
                            Quantity = 1
                        });
                    NewOrder.discount += (item as Product).ProductCost * (decimal)(item as Product).ProductDiscountAmount / 100;
                    NewOrder.sum += (item as Product).ProductCost - (item as Product).ProductCost * (decimal)(item as Product).ProductDiscountAmount / 100;
                }
                UpdateLV();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void AddOrderBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MFrame.Navigate(new AddEditOrderClientPage());
        }

        private void OrderBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MFrame.Navigate(new OrderPage());
        }

        private void SortCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateLV();
        }

        private void SearchTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateLV();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MFrame.Navigate(new AddEditProduct((sender as Button).DataContext as Product));
        }

        private void DelProd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var delProd = ProdLV.SelectedItems.Cast<Product>().ToList();
                foreach(var item in delProd)
                {
                    if (ConnectDB.GetCont().OrderProduct.Any(x => x.Product.ProductArticleNumber == item.ProductArticleNumber))
                    {
                        MessageBox.Show("Данные используются в другой таблице");
                        return;
                    }
                }
                if (MessageBox.Show("Удалить "+ delProd.Count + " записей", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    ConnectDB.GetCont().Product.RemoveRange(delProd);
                    ConnectDB.GetCont().SaveChanges();
                    MessageBox.Show("Данные удалены");
                }
            }
            catch
            {
                
            }
            UpdateLV();
        }

        private void ProductPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateLV();
        }
    }
}