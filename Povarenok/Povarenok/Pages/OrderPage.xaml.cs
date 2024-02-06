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
    /// Логика взаимодействия для OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        public OrderPage()
        {
            InitializeComponent();
            FilterCmb.ItemsSource = new string[]
            {
                "Все диапазоны",
                "0-10%",
                "11-14%",
                "15% и более"
            };
            SortCmb.ItemsSource = new string[]
            {
                "по возрастанию стоимости",
                "по убыванию стоимости"
            };
            if (Rights.curUser != null)
                CurrentUserTxt.Text = Rights.curUser.UserSurname + " " + Rights.curUser.UserName + " " + Rights.curUser.UserPatronymic;
            UpdateOrderLV();
        }
            
        private void UpdateOrderLV()
        {
            var orders = ConnectDB.GetCont().Order.ToList();
            switch (FilterCmb.SelectedIndex)
            {
                case 1:
                    orders = orders.Where(x => x.DiscountAmount <= 10).ToList();
                    break;
                case 2:
                    orders = orders.Where(x => x.DiscountAmount < 15 && x.DiscountAmount > 10).ToList();
                    break;
                case 3:
                    orders = orders.Where(x => x.DiscountAmount >= 15).ToList();
                    break;
            }
            if (SortCmb.SelectedIndex == 0) orders = orders.OrderBy(x => x.SummOrder).ToList();
            else orders = orders.OrderByDescending(x => x.SummOrder).ToList();
            foreach (var order in orders)
            {
                if(order.OrderProduct.All(x=>x.Product.CheckCount == 2)) 
                    order.CheckCount = 2;
                else if(order.OrderProduct.Any(x=>x.Product.CheckCount == 0)) 
                    order.CheckCount = 0;
                else order.CheckCount = 1;
            }
            OrderLV.ItemsSource = orders;
        }

        private void FilterCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateOrderLV();
        }

        private void SortCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateOrderLV();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Nav.MFrame.Navigate(new EditOrderPage((sender as Button).DataContext as Order));
        }
    }
}
