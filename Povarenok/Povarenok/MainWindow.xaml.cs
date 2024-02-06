using Povarenok.AppData;
using Povarenok.Pages;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Povarenok
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Nav.MFrame = MainFrame;
            Nav.MFrame.Navigate(new Avtoris());
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Nav.MFrame.CanGoBack) 
            { 
                Nav.MFrame.GoBack();
            }
        }

        private void MainFrame_ContentRendered(object sender, EventArgs e)
        {
            if (Nav.MFrame.CanGoBack)
            {
                BackBtn.Visibility = Visibility.Visible;
            }
            else
            {
                BackBtn.Visibility = Visibility.Collapsed;
                Rights.curUser = null;
            }
        }

        public static void ImgImport(string st)
        {
            var images = Directory.GetFiles(st);
            var products = ConnectDB.GetCont().Product.ToList();
            foreach (var prod in products)
            {
                try
                {
                    prod.ProductPhoto = File.ReadAllBytes(images.FirstOrDefault(x => x.Contains(prod.ProductArticleNumber)));
                }
                catch
                {

                }
                ConnectDB.GetCont().SaveChanges();
            }
        }
    }
}
