using iTextSharp.text;
using iTextSharp.text.pdf;
using Povarenok.AppData;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Povarenok.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditOrder.xaml
    /// </summary>
    public partial class AddEditOrderClientPage : Page
    {
        public AddEditOrderClientPage()
        {
            InitializeComponent();
            UpdateLV();
            if (Rights.curUser != null)
                CurrentUserTxt.Text = Rights.curUser.UserSurname + " " + Rights.curUser.UserName + " " + Rights.curUser.UserPatronymic;
        }

        int disc;
        decimal cost;

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool checkQuantity = true;
                foreach(var item in NewOrder.order.OrderProduct.ToList())
                {
                    if(item.Product.ProductQuantityInStock<3) checkQuantity = false;
                }
                int days = 6;
                NewOrder.order.OrderPickupPointID = (PointCmb.SelectedItem as OrderPickupPoint).OrderPickupPointID;
                if (NewOrder.order.OrderProduct.Count > 3 && checkQuantity) days = 3;
                NewOrder.order.OrderDate = DateTime.Now;
                NewOrder.order.DiscountAmount = Math.Round(NewOrder.discount * 100 / (NewOrder.sum+ NewOrder.discount), 2);
                NewOrder.order.SummOrder = NewOrder.sum;
                NewOrder.order.OrderDeliveryDate = DateTime.Now + new TimeSpan(days, 0, 0, 0);
                if (Rights.curUser != null) NewOrder.order.UserID = Rights.curUser.UserID;
                ConnectDB.GetCont().Order.Add(NewOrder.order);
                Random random = new Random();
                string kodP = random.Next(9).ToString() + random.Next(9).ToString() + random.Next(9).ToString();
                NewOrder.order.KodPickup = Convert.ToInt32(kodP);
                ConnectDB.GetCont().SaveChanges();
                Document doc = new Document();
                PdfWriter.GetInstance(doc, new FileStream("Талон.pdf", FileMode.Create));
                doc.Open();
                BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                Font font = new Font(baseFont, Font.DEFAULTSIZE, Font.NORMAL);
                Font font1 = new Font(baseFont, 32, Font.NORMAL);
                PdfPTable table = new PdfPTable(1);
                PdfPCell cell = new PdfPCell(new Phrase("Талон заказа", font));
                cell.HorizontalAlignment = 1;
                cell.Border = 0;
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(NewOrder.order.OrderDate.ToString(), font))
                {
                    Border = 0
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Номер заказа: " + NewOrder.order.OrderID.ToString(), font))
                {
                    Border = 0
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Состав заказа:", font))
                {
                    Border = 0
                };
                table.AddCell(cell);
                foreach (var order in NewOrder.order.OrderProduct.ToList())
                {
                    cell = new PdfPCell(new Phrase(order.Product.ProductName, font))
                    {
                        Border = 0
                    };
                    table.AddCell(cell);
                }
                cell = new PdfPCell(new Phrase("Размер скидки: " + NewOrder.discount.ToString(), font))
                {
                    Border = 0
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Сумма заказа: " + NewOrder.sum.ToString(), font))
                {
                    Border = 0
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Пункт выдачи: " + NewOrder.order.OrderPickupPoint.OrderPickupPointName.ToString(), font))
                {
                    Border = 0
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase("Код получения:",font))
                {
                    Border = 0
                };
                table.AddCell(cell);
                cell = new PdfPCell(new Phrase(kodP, font1))
                {
                    Border = 0
                };
                table.AddCell(cell);
                doc.Add(table);
                doc.Close();
                MessageBox.Show("Pdf-документ сохранен");
                NewOrder.order = null;
                Nav.MFrame.GoBack();
                NewOrder.sum = NewOrder.discount = 0;
            }
            catch
            {
            }
        }

        void UpdateLV()
        {
            if (NewOrder.order != null) ProdLV.ItemsSource = NewOrder.order.OrderProduct.ToList();
            PointCmb.ItemsSource = ConnectDB.GetCont().OrderPickupPoint.ToList();
            DiscountTxt.Text = "Сумма скидки\t" + NewOrder.discount.ToString();
            CostTxt.Text = "Сумма заказа\t" + NewOrder.sum.ToString();
        }

        private void IncrBtn_Click(object sender, RoutedEventArgs e)
        {
            NewOrder.order.OrderProduct.FirstOrDefault(x => x.Product.ProductArticleNumber == ((sender as Button).DataContext as OrderProduct).Product.ProductArticleNumber).Quantity++;
            disc = (int)((sender as Button).DataContext as OrderProduct).Product.ProductDiscountAmount;
            cost = ((sender as Button).DataContext as OrderProduct).Product.ProductCost;
            NewOrder.discount += cost * disc / 100;
            NewOrder.sum += cost - cost * disc / 100;
            UpdateLV();
        }

        private void DecrBtn_Click(object sender, RoutedEventArgs e)
        {
            if (--NewOrder.order.OrderProduct.FirstOrDefault(x => x.Product.ProductArticleNumber == ((sender as Button).DataContext as OrderProduct).Product.ProductArticleNumber).Quantity < 1)
                NewOrder.order.OrderProduct.Remove((sender as Button).DataContext as OrderProduct);
            disc = (int)((sender as Button).DataContext as OrderProduct).Product.ProductDiscountAmount;
            cost = ((sender as Button).DataContext as OrderProduct).Product.ProductCost;
            NewOrder.discount -= cost * disc / 100;
            NewOrder.sum -= cost - cost * disc / 100;
            UpdateLV();
        }

        private void DelProdBtn_Click(object sender, RoutedEventArgs e)
        {
            disc = (int)((sender as Button).DataContext as OrderProduct).Product.ProductDiscountAmount;
            cost = ((sender as Button).DataContext as OrderProduct).Product.ProductCost;
            for(int i = 0; i < NewOrder.order.OrderProduct.FirstOrDefault(x => x.Product.ProductArticleNumber == ((sender as Button).DataContext as OrderProduct).Product.ProductArticleNumber).Quantity; i++)
            {
                NewOrder.discount -= cost * disc / 100;
                NewOrder.sum -= cost - cost * disc / 100;
            }
            NewOrder.order.OrderProduct.Remove((sender as Button).DataContext as OrderProduct);
            UpdateLV();
        }
    }
}