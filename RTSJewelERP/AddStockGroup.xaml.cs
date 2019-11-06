using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AddStockGroup.xaml
    /// </summary>
    public partial class AddStockGroup : Window
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public AddStockGroup()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window
            autocompleteItemNameStockEntry.autoTextBoxStockGroup.Focus();
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
                    this.Close();
                    //this.Close();
                    //this.NavigationService.GoBack();
                    //this.NavigationService.RemoveBackEntry();
                }
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[^0-9]+");
            //Regex regex = new Regex(@"^\d*\.?\d?$");
            //Regex regex = new Regex(@"[^0-9]\d{0,9}(\.\d{1,3})?%?$");
            //Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]+)?$");

            Regex regex = new Regex("[^0-9.-]+");   // Allow Decimal Only

            e.Handled = regex.IsMatch(e.Text);
        }
        private void autocompleteItemName_LostFocus(object sender, RoutedEventArgs e)
        {
            //autocompleteItemNameStockEntry.autoTextBoxStockEntry.Background = Brushes.White;
            //autocompleteItemNameStockEntry.autoTextBoxStockEntry.Foreground = Brushes.Black;

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Tab)
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter)
            {
                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                if (keyboardFocus != null)
                {
                    keyboardFocus.MoveFocus(tRequest);
                }

                e.Handled = true;
            }

            if (e.Key == Key.RightShift)
            {

                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Previous);
                UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                if (keyboardFocus != null)
                {
                    keyboardFocus.MoveFocus(tRequest);

                }

                e.Handled = true;
            }

        }

        private void TextBoxHighlight_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            textBox.Background = Brushes.BlueViolet;
            textBox.Foreground = Brushes.White;

        }

        private void SaveDesign_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection myConnSVEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            myConnSVEntryStr.Open();
            string CountSVEntryStr = "SELECT COUNT(*) From StockGroups where GroupName= '" + autocompleteItemNameStockEntry.autoTextBoxStockGroup.Text + "' and CompID = '" + CompID + "'";
            // string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
            SqlCommand myCommandDel = new SqlCommand(CountSVEntryStr, myConnSVEntryStr);
            myCommandDel.Connection = myConnSVEntryStr;

            //int countRec = myCommand.ExecuteNonQuery();
            int countRecDelDel = (int)myCommandDel.ExecuteScalar();
            myCommandDel.Connection.Close();
            if (countRecDelDel != 0)
            {
                MessageBox.Show("Item is already available");
            }
            else
            {
                SqlConnection myConSVInventoryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                myConSVInventoryStr.Open();


                string querySalesInventory = "";
                querySalesInventory = "insert into StockGroups(GroupName,HSN,GSTRate, CompID) Values('" + autocompleteItemNameStockEntry.autoTextBoxStockGroup.Text.Trim() + "','" + HSN.Text.Trim() + "','" + GSTRate.Text.Trim() + "','" + CompID + "')";



                SqlCommand myCommandSVInventory = new SqlCommand(querySalesInventory, myConSVInventoryStr);
                myCommandSVInventory.Connection = myConSVInventoryStr;
                //myCommandInvEntry.Connection.Open();
                int NumPI = myCommandSVInventory.ExecuteNonQuery();
                if (NumPI != 0)
                {
                    //DesignPattern.Clear();
                    //GroupName.Clear();
                    //Size.Clear();
                    //Color.Clear();
                    this.Close();
                }
                myCommandSVInventory.Connection.Close();
            }

        }

    }
}
