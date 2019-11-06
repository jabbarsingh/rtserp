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

namespace RTSJewelERP
{
    /// <summary>
    /// Interaction logic for SheetHome.xaml
    /// </summary>
    public partial class SheetHome : Page
    {
        public SheetHome()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window
        }

        /// <summary>
        /// Esc key close This window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to Close?", "Close Page", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    //this.Close();
                    this.NavigationService.GoBack();
                    //this.NavigationService.RemoveBackEntry();
                }
            }

            //if (e.Key == Key.PageUp)
            //{
            //    if (Convert.ToInt64(VoucherNumber.Text.Trim()) < voucherNumber)
            //    {
            //        Int64 inpageup = (VoucherNumber.Text.Trim() != "") ? (Convert.ToInt64(VoucherNumber.Text.Trim()) + 1) : 0;
            //        VoucherNumber.Text = inpageup.ToString();
            //        MoveToBill(inpageup.ToString());

            //    }
            //    if (Convert.ToInt64(VoucherNumber.Text.Trim()) == voucherNumber)
            //    {
            //        //autocompltCustName.autoTextBox.Text = "Cash";
            //        autocompltCustName.autoTextBox.Focus();
            //    }
            //    e.Handled = true;
            //}
            //if (e.Key == Key.PageDown)
            //{
            //    if (Convert.ToInt64(VoucherNumber.Text.Trim()) > 1)
            //    {
            //        Int64 inpageup = (VoucherNumber.Text.Trim() != "") ? (Convert.ToInt64(VoucherNumber.Text.Trim()) - 1) : 0;
            //        VoucherNumber.Text = inpageup.ToString();
            //        MoveToBill(inpageup.ToString());
            //        e.Handled = true;
            //    }


            //}

        }



        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            SheetNewTransactionEntry lp = new SheetNewTransactionEntry();
            lp.Show();
        }

        private void AddItemStock_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ViewAccount_Click(object sender, RoutedEventArgs e)
        {
            SheetViewAccounts lp = new SheetViewAccounts();
            lp.Show();
        }

        private void AddSheetAccount_Click(object sender, RoutedEventArgs e)
        {
            SheetAddAccount lp = new SheetAddAccount();
            lp.Show();

        } //if selected value dropdown close

    }
}
