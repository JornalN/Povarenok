using Povarenok.AppData;
using System;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Povarenok.Pages
{
    /// <summary>
    /// Логика взаимодействия для Avtoris.xaml
    /// </summary>
    public partial class Avtoris : Page
    {
        public Avtoris()
        {
            InitializeComponent();
            timeBlock = new DispatcherTimer();
            timeBlock.Tick += new EventHandler(timer_Tick);
            timeBlock.Interval = new TimeSpan(0,0,0,10);
        }

        DispatcherTimer timeBlock;
        int countEnter = 0;
        string cap ="";
        private void timer_Tick(object sender, EventArgs e)
        {
            Captcha();
            EnterBtn.IsEnabled = true;
            timeBlock.IsEnabled = false;
        }
        private void Captcha()
        {
            CaptchaStack.Visibility = Visibility.Visible;
            string allowchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";
            allowchar += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,y,z,";
            allowchar += "1,2,3,4,5,6,7,8,9,0";
            string[] ar = allowchar.Split(',');
            Random r = new Random();
            cap = "";
            cap += CaptchaTxt1.Text = ar[r.Next(0, ar.Length)];
            cap += CaptchaTxt2.Text = ar[r.Next(0, ar.Length)];
            cap += CaptchaTxt3.Text = ar[r.Next(0, ar.Length)];
            cap += CaptchaTxt4.Text = ar[r.Next(0, ar.Length)];
            CaptchaTxt1.Margin = new Thickness(0, r.Next(0, 20), 0, 0);
            CaptchaTxt2.Margin = new Thickness(0, r.Next(0, 20), 0, 0);
            CaptchaTxt3.Margin = new Thickness(0, r.Next(0, 20), 0, 0);
            CaptchaTxt4.Margin = new Thickness(0, r.Next(0, 20), 0, 0);
        }

        private void EnterBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ConnectDB.GetCont().User.Any(x => x.UserLogin == LoginTxt.Text && x.UserPassword == PasswordTxt.Password) || (cap != "" && cap != EnterCapTxt.Text))
            {
                countEnter++;
                Captcha();
                if (countEnter == 3)
                {
                    countEnter = 0;
                    EnterBtn.IsEnabled = false;
                    timeBlock.IsEnabled = true;
                    MessageBox.Show("Были произведены 3 неверных попытки, авторизация заблокирована на 10 секунд");
                }
                TxtBlockMessage.Visibility = Visibility.Visible;
                return;
            }
            cap = "";
            Rights.curUser = ConnectDB.GetCont().User.FirstOrDefault(x => x.UserLogin == LoginTxt.Text && x.UserPassword == PasswordTxt.Password);
            CaptchaStack.Visibility = Visibility.Collapsed;
            TxtBlockMessage.Visibility = Visibility.Collapsed;
            Nav.MFrame.Navigate(new ProductPage());
        }

        private void EnterGBtn_Click(object sender, RoutedEventArgs e)
        {
            cap = "";
            CaptchaStack.Visibility = Visibility.Collapsed;
            TxtBlockMessage.Visibility = Visibility.Collapsed;
            Nav.MFrame.Navigate(new ProductPage());
        }


        private void CaptchaTxtClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Captcha();
        }
    }
}
