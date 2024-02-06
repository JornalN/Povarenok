using iTextSharp.text.pdf.parser;
using Microsoft.Win32;
using Povarenok.AppData;
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

namespace Povarenok.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditProduct.xaml
    /// </summary>
    public partial class AddEditProduct : Page
    {
        static Product prod;
        string pathImage = null;

        public AddEditProduct(Product product)
        {
            InitializeComponent();
            prod = product;
            DataContext = prod;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrEmpty(MaxDiscountTxt.Text)) MaxDiscountTxt.Text = prod.ProductMaxDiscountAmount.ToString();
            if (string.IsNullOrEmpty(prod.ProductArticleNumber)) errors.AppendLine("Артикул товара указан неверно");
            if (string.IsNullOrEmpty(prod.ProductName)) errors.AppendLine("Название товара указано неверно");
            if (string.IsNullOrEmpty(prod.ProductDescription)) errors.AppendLine("Описание товара указано неверно");
            if (string.IsNullOrEmpty(prod.ProductCategory)) errors.AppendLine("Категория указана неверно"); 
            try
            {
                if (pathImage != null && pathImage.Trim() != "")
                {
                    prod.ProductPhoto = File.ReadAllBytes(pathImage);
                }
            }
            catch
            {
                prod.ProductPhoto = null;
                errors.AppendLine("Ошибка загрузки изображения");
            }
            if (string.IsNullOrEmpty(prod.ProductManufacturer)) errors.AppendLine("Производитель указан неверно");
            if (prod.ProductCost < 0) errors.AppendLine("Цена не может быть отрицательной");
            if (!prod.ProductDiscountAmount.ToString().All(char.IsDigit)) errors.AppendLine("Скидка не может содержать ничего кроме чисел");
            else
            {
                if (prod.ProductDiscountAmount < 0) errors.AppendLine("Скидка не может быть отрицательной");
                if (string.IsNullOrEmpty(MaxDiscountTxt.Text.Trim())) errors.AppendLine("Максимальная скидка не может быть пустой");
                else
                    if(prod.ProductDiscountAmount > Convert.ToInt32(MaxDiscountTxt.Text)) errors.AppendLine("Скидка не может быть больше максимальной");
            }
            if (!prod.ProductMaxDiscountAmount.ToString().All(char.IsDigit)) errors.AppendLine("Максимальная скидка не может содержать ничего кроме чисел");
            else
                if (prod.ProductDiscountAmount < 0) errors.AppendLine("Максимальная скидка не может быть отрицательной");
            if (prod.ProductQuantityInStock < 0) errors.AppendLine("Количество не может быть отрицательным");

            if (string.IsNullOrEmpty(prod.ProductStatus)) errors.AppendLine("Единица измерения товара указана неверно");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (prod.ProductArticleNumber.Length<6)
                ConnectDB.GetCont().Product.Add(prod);
            try
            {
                ConnectDB.GetCont().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Nav.MFrame.GoBack();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Ошибка сохранения продукта " + ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImagePathBtn_Click(object sender, RoutedEventArgs e)
        {
            
            var dialog = new OpenFileDialog();
            dialog.Filter = "Файлы изображений (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png";
            if (dialog.ShowDialog().GetValueOrDefault(false))
            {
                pathImage = dialog.FileName; 
            }
            ImageBox.Source = new BitmapImage(new Uri(pathImage));
        }

        private void ClearImageBtn_Click(object sender, RoutedEventArgs e)
        {
            prod.ProductPhoto = null;
        }
    }
}
