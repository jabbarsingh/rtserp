using RTSJewelERP.StorageListTableAdapters;
using RTSJewelERP.TrayListTableAdapters;
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
    /// Interaction logic for InstantStockTransfer.xaml
    /// </summary>
    public partial class InstantStockTransfer : Window
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public InstantStockTransfer()
        {
            InitializeComponent();
            BindComboBoxStorageList(cmbStorageList);
            BindComboBoxTrayList(TrayComboBoxList);
            autocompleteItemNameStockEntry.autoTextBoxStockEntry.Focus();
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
                    this.Close();
                    //this.NavigationService.GoBack();
                    //this.NavigationService.RemoveBackEntry();
                }
            }
        }

        public void BindComboBoxStorageList(ComboBox storageName)
        {
            var custAdpt = new StorageLocationsByPcTableAdapter();
            var custInfoVal = custAdpt.GetData();
            if (custInfoVal != null)
            {
                cmbStorageList.ItemsSource = custInfoVal.Select(x => x.StorageName.Trim()).Distinct().ToList();
                cmbStorageList1.ItemsSource = custInfoVal.Select(x => x.StorageName.Trim()).Distinct().ToList();
            }
        }


        public void BindComboBoxTrayList(ComboBox trayname)
        {
            //var custAdpt = new TrayListInStorageByPcTableAdapter();
            //var custInfoVal = custAdpt.GetData();
            //var LinqRes = (from UserRec in custInfoVal
            //               orderby UserRec.TrayName ascending
            //               //select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
            //               select (UserRec.TrayName.Trim())).Distinct();
            //cmbsTrayList.ItemsSource = LinqRes;

            var custAdpt = new TrayListInStorageByPcTableAdapter();
            var custInfoVal = custAdpt.GetData();
            if (custInfoVal != null)
            {
                TrayComboBoxList.ItemsSource = custInfoVal.Where(c => (c.StorageName.Trim() == "Main")).Select(x => x.TrayName.Trim()).Distinct().ToList();
                cmbTrayList1.ItemsSource = custInfoVal.Where(c => (c.StorageName.Trim() == "Main")).Select(x => x.TrayName.Trim()).Distinct().ToList();
            }
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

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {

            Regex regex = new Regex("[^0-9.-]+");   // Allow Decimal Only

            e.Handled = regex.IsMatch(e.Text);
        }

        private void autocompleteItemName_LostFocus(object sender, RoutedEventArgs e)
        {
            //CleanUp();

            autocompleteItemNameStockEntry.autoTextBoxStockEntry.Background = Brushes.White;
            autocompleteItemNameStockEntry.autoTextBoxStockEntry.Foreground = Brushes.Black;


            if (Regex.IsMatch(autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim(), @"^\d+$") || 1 == 1)
            {

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select ItemName,ActualQty,ActualWt,ItemBarCode from StockItemsByPc where LTRIM(RTRIM(ItemName)) = '" + autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() + "' and CompID = '" + CompID + "'";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "";
                        QtyStock.Text = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(1)).ToString().Trim() : "";
                        WeightStock.Text = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(2)).ToString().Trim() : "";
                        //ItemBarCode.Text = (reader["ItemBarCode"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
                       
                    }
                }
                else
                {

                }

                reader.Close();
            }
        }

        private void Window_CustKeyDown(object sender, KeyEventArgs e)
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

        private void TrayComboBoxList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QtyInTray.Clear();
            WeightInTray.Clear();
            if (TrayComboBoxList.SelectedItem != null)
            {
                string trayenameselected = TrayComboBoxList.SelectedItem.ToString();

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select * from TrayItemAllocation where LTRIM(RTRIM(ItemName)) = '" + autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() + "' and  LTRIM(RTRIM(TrayName)) = '" + trayenameselected + "' and LTRIM(RTRIM(ItemBarCode)) = '" + itemBarCode.Text.Trim() + "' and CompID = '" + CompID + "'";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "";
                        QtyInTray.Text = (reader["Qty"] != DBNull.Value) ? (reader.GetDouble(4)).ToString().Trim() : "";
                        WeightInTray.Text = (reader["Weight"] != DBNull.Value) ? (reader.GetDouble(5)).ToString().Trim() : "";
                        //ItemBarCode.Text = (reader["ItemBarCode"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";

                    }
                }
                else
                {

                }

                reader.Close();


            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection myConnCustExistr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            myConnCustExistr.Open();
            string CountSVEntryStr = "SELECT COUNT(*) From TrayItemAllocation  where LTRIM(RTRIM(ItemName)) = '" + autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() + "' and  LTRIM(RTRIM(TrayName)) = '" + TrayComboBoxList.Text + "' and LTRIM(RTRIM(ItemBarCode)) = '" + itemBarCode.Text.Trim() + "' and CompID = '" + CompID + "'";
            // string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
            SqlCommand myCommandCustEx = new SqlCommand(CountSVEntryStr, myConnCustExistr);
            myCommandCustEx.Connection = myConnCustExistr;

            //int countRec = myCommand.ExecuteNonQuery();
            int countRecCustEx = (int)myCommandCustEx.ExecuteScalar();
            myCommandCustEx.Connection.Close();
            if (countRecCustEx < 1)
            {
                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                myConnSalesInvEntryStr.Open();
                string querySalesInvEntry = "";
                querySalesInvEntry = "insert into TrayItemAllocation(TrayName, ItemName,ItemBarCode,Qty,Weight,StorageName, CompID)  Values ( '" + TrayComboBoxList.Text + "','" + autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() + "','" + itemBarCode.Text.Trim() + "','" + Qty.Text + "','" + Weight.Text + "','" + cmbStorageList.Text + "','" + CompID + "')";
                SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                //myCommandInvEntry.Connection.Open();
                int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                if (NumPInv != 0)
                {
                    MessageBox.Show("Successfully", "Insert Record");
                    Qty.Clear();
                    Weight.Clear();
                }
                else
                {
                    MessageBox.Show("Stock is not Transferred....", "Insert Record Error");
                }
                myCommandInvEntry.Connection.Close();
            }
            else
            {
                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                myConnSalesInvEntryStr.Open();
                string querySalesInvEntry = "";

                double inStorageQty = (Qty.Text != "") ? (Convert.ToDouble(Qty.Text)) : 0;
                double inStorageWt = (Weight.Text != "") ? (Convert.ToDouble(Weight.Text)) : 0;

                double inTrayQty = (QtyInTray.Text != "") ? (Convert.ToDouble(QtyInTray.Text)) : 0;
                double inTrayWt = (WeightInTray.Text != "") ? (Convert.ToDouble(WeightInTray.Text)) : 0;
                double balanceStk = Math.Round((inTrayQty + inStorageQty), 2);
                double balanceStkWt = Math.Round((inTrayWt + inStorageWt), 2);

                querySalesInvEntry = "update TrayItemAllocation  set Qty='" + balanceStk + "',Weight='" + balanceStkWt + "' where LTRIM(RTRIM(ItemName)) = '" + autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() + "' and  LTRIM(RTRIM(TrayName)) = '" + TrayComboBoxList.Text + "' and LTRIM(RTRIM(ItemBarCode)) = '" + itemBarCode.Text.Trim() + "' and CompID = '" + CompID + "'";
                SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                //myCommandInvEntry.Connection.Open();
                int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                if (NumPInv != 0)
                {
                    MessageBox.Show("Successfully", "Insert Record");
                    Qty.Clear();
                    Weight.Clear();
                }
                else
                {
                    MessageBox.Show("Stock is not Transferred....", "Insert Record Error");
                }
                myCommandInvEntry.Connection.Close();
                

            }
            autocompleteItemNameStockEntry.autoTextBoxStockEntry.Focus();
        }

        private void TextBoxHighlight_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                textBox.Background = Brushes.BlueViolet;
                textBox.Foreground = Brushes.White;
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SqlConnection myConnCustExistr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            myConnCustExistr.Open();
            string CountSVEntryStr = "SELECT COUNT(*) From TrayItemAllocation  where LTRIM(RTRIM(ItemName)) = '" + autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() + "' and  LTRIM(RTRIM(TrayName)) = '" + TrayComboBoxList.Text + "' and LTRIM(RTRIM(ItemBarCode)) = '" + itemBarCode.Text.Trim() + "' and CompID = '" + CompID + "'";
            // string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
            SqlCommand myCommandCustEx = new SqlCommand(CountSVEntryStr, myConnCustExistr);
            myCommandCustEx.Connection = myConnCustExistr;

            //int countRec = myCommand.ExecuteNonQuery();
            int countRecCustEx = (int)myCommandCustEx.ExecuteScalar();
            myCommandCustEx.Connection.Close();
            if (countRecCustEx < 1)
            {
                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                myConnSalesInvEntryStr.Open();
                string querySalesInvEntry = "";
                querySalesInvEntry = "insert into TrayItemAllocation(TrayName, ItemName,ItemBarCode,Qty,Weight,StorageName, CompID)  Values ( '" + TrayComboBoxList.Text + "','" + autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() + "','" + itemBarCode.Text.Trim() + "','" + Qty.Text + "','" + Weight.Text + "','" + cmbStorageList.Text + "','" + CompID + "')";
                SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                //myCommandInvEntry.Connection.Open();
                int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                if (NumPInv != 0)
                {
                    MessageBox.Show("Successfully", "Insert Record");
                    Qty.Clear();
                    Weight.Clear();
                }
                else
                {
                    MessageBox.Show("Stock is not Transferred....", "Insert Record Error");
                }
                myCommandInvEntry.Connection.Close();
            }
            else
            {
                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                myConnSalesInvEntryStr.Open();
                string querySalesInvEntry = "";

                double inStorageQty = (Qty.Text != "") ? (Convert.ToDouble(Qty.Text)) : 0;
                double inStorageWt = (Weight.Text != "") ? (Convert.ToDouble(Weight.Text)) : 0;

                double inTrayQty = (QtyInTray.Text != "") ? (Convert.ToDouble(QtyInTray.Text)) : 0;
                double inTrayWt = (WeightInTray.Text != "") ? (Convert.ToDouble(WeightInTray.Text)) : 0;
                double balanceStk = Math.Round((inTrayQty + inStorageQty), 2);
                double balanceStkWt = Math.Round((inTrayWt + inStorageWt), 2);

                querySalesInvEntry = "update TrayItemAllocation  set Qty='" + balanceStk + "',Weight='" + balanceStkWt + "' where LTRIM(RTRIM(ItemName)) = '" + autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() + "' and  LTRIM(RTRIM(TrayName)) = '" + TrayComboBoxList.Text + "' and LTRIM(RTRIM(ItemBarCode)) = '" + itemBarCode.Text.Trim() + "' and CompID = '" + CompID + "'";
                SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                //myCommandInvEntry.Connection.Open();
                int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                if (NumPInv != 0)
                {
                    MessageBox.Show("Successfully", "Insert Record");
                    Qty.Clear();
                    Weight.Clear();
                }
                else
                {
                    MessageBox.Show("Stock is not Transferred....", "Insert Record Error");
                }
                myCommandInvEntry.Connection.Close();


            }
        }
    }
}
