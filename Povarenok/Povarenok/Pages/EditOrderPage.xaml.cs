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
    /// Логика взаимодействия для EditOrderPage.xaml
    /// </summary>
    public partial class EditOrderPage : Page
    {
        Order curOrder;
        public EditOrderPage(Order order)
        {
            InitializeComponent();
            DataContext = curOrder = order;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConnectDB.GetCont().SaveChanges();
                Nav.MFrame.GoBack();
            }
            catch
            {

            }
            
        }
    }
}
