using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Povarenok.AppData
{
    internal class Nav
    {
        public static Frame MFrame;
        public static void Visible(Button btn)
        {
            btn.Visibility = Visibility.Visible;
        }

        public static void Collapsed(Button btn)
        {
            btn.Visibility = Visibility.Collapsed;
        }
    }

    public partial class Product
    {
        public bool CheckDisc
        {
            get
            {
                if (ProductDiscountAmount > 15)
                    return true;
                else
                    return false;
            }
        }

        public int CheckCount
        {
            get
            {
                if (ProductQuantityInStock > 3)
                    return 2;
                else
                    if (ProductQuantityInStock == 0)
                    return 0;
                return 1;
            }
        }

        public bool CheckUser
        {
            get
            {
                if (Rights.curUser != null)
                {
                    if(Rights.curUser.UserRole == 1)
                        return true;
                }
                return false;
            }
        }

        public decimal SumWithDiscount
        {
            get
            {
                return ProductCost - ProductCost * (decimal)ProductDiscountAmount / 100;
            }
        }
    }

    public partial class Order
    {
        public int CheckCount
        {
            get;
            set;
        }
    }
}

