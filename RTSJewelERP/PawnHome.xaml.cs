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

namespace RTSJewelERP
{
    /// <summary>
    /// Interaction logic for PawnHome.xaml
    /// </summary>
    public partial class PawnHome : Window
    {
        public PawnHome()
        {
            InitializeComponent();
            ImageBrush myBrush = new ImageBrush();

            myBrush.ImageSource = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), "C:\\ViewBill\\Logo\\Pawn.png"));
         // "D:\\Data\\IMG\\x.jpg"
            this.Background = myBrush;


        }
    }
}
