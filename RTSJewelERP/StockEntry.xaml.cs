using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
using RTSJewelERP.Controls;
using RTSJewelERP.GroupListTableAdapters;
using RTSJewelERP.StorageListTableAdapters;
using RTSJewelERP.TrayListTableAdapters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
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
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using RTSJewelERP.DebitCreditAccountsListTableAdapters;
namespace RTSJewelERP
{
    /// <summary>
    /// Interaction logic for StockEntry.xaml
    /// </summary>
    public partial class StockEntry : Window
    {


        public static PrinterSettings MyPrinterSettings = new PrinterSettings();
        string resetPrinter = MyPrinterSettings.PrinterName;
        //public static string Default_PrinterName
        //{
        //    get
        //    {
        //        return MyPrinterSettings.PrinterName;
        //    }
        //    set
        //    {
        //        MyPrinterSettings.DefaultPageSettings.PrinterSettings.PrinterName = "Bar Code Printer TT033-50";
        //        MyPrinterSettings.PrinterName = "Bar Code Printer TT033-50";
        //    }
        //}


        string autobarcodeNumber = "";
        string CompID = RTSJewelERP.ConfigClass.CompID;
        //string CompID = "1";
        public StockEntry()
        {
            InitializeComponent();
            
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window
            BindComboBoxStorage(StorageName);
            BindComboBoxGroupName(GroupName);
            BindComboPurchasePartyName(PurchasePartyName);
            autocompleteItemNameStockEntry.autoTextBoxStockEntry.Focus();
           

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select number from AutoIncrement where LTRIM(RTRIM(Name)) = 'BarCode'  and CompID = '" + CompID + "'";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = con;
            SqlDataReader reader = cmd.ExecuteReader();

            //tmpProduct = new Product();

            while (reader.Read())
            {
                autobarcodeNumber = reader.GetInt64(0).ToString();

            }
            AutoBarCodeNumber.Text = autobarcodeNumber;
            reader.Close();

            listAllPrinters();

        }

        private void listAllPrinters()
        {
            foreach (var item in PrinterSettings.InstalledPrinters)
            {
                this.listBox1.Items.Add(item.ToString());
            }
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string pname = this.listBox1.SelectedItem.ToString();
            myPrinters.SetDefaultPrinter(pname); 
        }

        public static class myPrinters
        {
            [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern bool SetDefaultPrinter(string Name);

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
                //MessageBoxResult result = MessageBox.Show("Are you sure want to Close?", "Close Page", MessageBoxButton.YesNo);
                //if (result == MessageBoxResult.Yes)
                //{
                    this.Close();
                    //this.NavigationService.GoBack();
                    //this.NavigationService.RemoveBackEntry();
                //}
            }

            
            if (e.Key == Key.PageUp )
            {
                AutoBarCodeNumber.Text = (AutoBarCodeNumber.Text.Trim() != "") ? AutoBarCodeNumber.Text.Trim() : autobarcodeNumber.Trim();
                if (Convert.ToInt64(AutoBarCodeNumber.Text.Trim()) < Convert.ToInt64(autobarcodeNumber))
                {
                    Int64 inpageup = (AutoBarCodeNumber.Text.Trim() != "") ? (Convert.ToInt64(AutoBarCodeNumber.Text.Trim()) + 1) : 0;
                    AutoBarCodeNumber.Text = inpageup.ToString();                   
                    MoveToBill(inpageup.ToString());

                }
                if (AutoBarCodeNumber.Text.Trim() == "")
                {
                   // AutoBarCodeNumber.Text.Trim() == ""
                    //autocompltCustName.autoTextBoxCustNameBarcode.Text = "Cash";
                    autocompleteItemNameStockEntry.autoTextBoxStockEntry.Focus();
                }
                e.Handled = true;
            }
            if (e.Key == Key.PageDown)
            {
                AutoBarCodeNumber.Text = (AutoBarCodeNumber.Text.Trim() != "") ? AutoBarCodeNumber.Text.Trim() : autobarcodeNumber.Trim();
                if (Convert.ToInt64(AutoBarCodeNumber.Text.Trim()) > 1)
                {
                    Int64 inpageup = (AutoBarCodeNumber.Text.Trim() != "") ? (Convert.ToInt64(AutoBarCodeNumber.Text.Trim()) - 1) : 0;
                    AutoBarCodeNumber.Text = inpageup.ToString();
                    MoveToBill(inpageup.ToString());
                    e.Handled = true;
                }


            }


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
        private void TextBoxHighlight_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            textBox.Background = Brushes.White;
            textBox.Foreground = Brushes.Black;
        }

        private void MoveToBill(string invnumbertxt)
        {
            CleanUp();
            //if (txtBarcode.Text.Trim() != "")
            //{
            //    AutoBarCodeNumber.Clear();
            //}
            //else
            //{
            //    AutoBarCodeNumber.Text = autobarcodeNumber;
            //}
            //string custnme = txtBarcode.Text.Trim();
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conn.Open();
            string sql = "select * from StockItemsByPc where LTRIM(RTRIM(ItemBarCode)) = '" + invnumbertxt.Trim() + "'  and CompID = '" + CompID + "'";
            //string sql = "select * from AccountsMaster where Barcode = '" + txtBarcode.Text.Trim() + "'";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = conn;
            SqlDataReader reader = cmd.ExecuteReader();

            //string constr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\RTSProSoft\Database\InvWpf-Enhanced.accdb;";
            //OleDbConnection con = new OleDbConnection(constr);
            //string queryStr = @"select * from PurchaseInvoices where PartyName = '" + custnme + "'";
            //OleDbCommand command = new OleDbCommand(queryStr, con);
            //con.Open();
            //OleDbDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                    //HSN.Text = (reader["HSN"] != DBNull.Value) ? (reader.GetString(18).Trim()) : "";
                    PrintName.Text = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                    //UnitID.Text = (reader["UnitID"] != DBNull.Value) ? (reader.GetInt32(4)).ToString().Trim() : "";
                    //ItemCode.Text = (reader["ItemCode"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";


                    ItemDesc.Text = (reader["ItemDesc"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                    //ItemBarCode.Text = (reader["ItemBarCode"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
                    ItemPrice.Text = (reader["ItemPrice"] != DBNull.Value) ? (reader.GetDouble(9)).ToString().Trim() : "";
                    //SetCriticalLevel.Text = (reader["SetCriticalLevel"] != DBNull.Value) ? (reader.GetBoolean(12)).ToString().Trim() : "false";
                    //SetDefaultStorageID.Text = (reader["SetDefaultStorageID"] != DBNull.Value) ? (reader.GetInt32(14)).ToString().Trim() : "";
                    //DecimalPlaces.Text = (reader["DecimalPlaces"] != DBNull.Value) ? (reader.GetInt32(17)).ToString().Trim() : "";
                    //HSN.Text = (reader["IsBarcodeCreated"] != DBNull.Value) ? (reader.GetBoolean(18)).ToString().Trim() : "false";
                    //ItemPurchPrice.Text = (reader["ItemPurchPrice"] != DBNull.Value) ? (reader.GetDouble(23)).ToString().Trim() : "";
                    ItemAlias.Text = (reader["ItemAlias"] != DBNull.Value) ? (reader.GetString(30).Trim()) : "";
                    //get Group Name 
                    GroupName.Text = (reader["UnderGroupName"] != DBNull.Value) ? (reader.GetString(31)).ToString().Trim() : "";
                    SubGroupName.Text = (reader["UnderSubGroupName"] != DBNull.Value) ? (reader.GetString(33)).ToString().Trim() : "";
                    ActualQty.Text = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)).ToString().Trim() : "";
                    HSN.Text = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
                    GSTRate.Text = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)).ToString().Trim() : "";
                    //Get Name instead ID
                    //StorageName.Text = (reader["StorageID"] != DBNull.Value) ? (reader.GetInt32(38)).ToString().Trim() : "";
                    //TrayName.Text = (reader["TrayID"] != DBNull.Value) ? (reader.GetInt32(39)).ToString().Trim() : "";
                    //CounterName.Text = (reader["CounterID"] != DBNull.Value) ? (reader.GetInt32(40)).ToString().Trim() : "";
                    OpeningStock.Text = (reader["OpeningStock"] != DBNull.Value) ? (reader.GetDouble(41)).ToString().Trim() : "";
                    OpeningStockValue.Text = (reader["OpeningStockValue"] != DBNull.Value) ? (reader.GetDouble(42)).ToString().Trim() : "";
                    //tmpProduct.UpdateDate = reader.GetDateTime(44); //reader["UpdateDate"] != DBNull.Value) ? (reader.GetDateTime(44)) : "";  
                    actualWt.Text = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)).ToString().Trim() : "";
                    //tmpProduct.LastBuyDate = reader.GetDateTime(47); //(reader["LastBuyDate"] != DBNull.Value) ? (reader.GetDateTime(47) : "";
                    //tmpProduct.LastSaleDate = reader.GetDateTime(48);//(reader["LastSaleDate"] != DBNull.Value) ? (reader.GetDateTime(48) : "";
                    CurrentStockValue.Text = (reader["CurrentStockValue"] != DBNull.Value) ? (reader.GetDouble(49)).ToString().Trim() : "";
                    LastSalePrice.Text = (reader["LastSalePrice"] != DBNull.Value) ? (reader.GetDouble(50)).ToString().Trim() : "";
                    LastBuyPrice.Text = (reader["LastBuyPrice"] != DBNull.Value) ? (reader.GetDouble(51)).ToString().Trim() : "";

                    OpeningStockWt.Text = (reader["OpeningStockWt"] != DBNull.Value) ? (reader.GetDouble(52)).ToString().Trim() : "";

                    StorageName.Text = (reader["StorageName"] != DBNull.Value) ? (reader.GetString(79)).ToString().Trim() : "";
                    TrayName.Text = (reader["TrayName"] != DBNull.Value) ? (reader.GetString(81)).ToString().Trim() : "";

                    quality.Text = (reader["Quality"] != DBNull.Value) ? (reader.GetString(82)).ToString().Trim() : "";
                    size.Text = (reader["Size"] != DBNull.Value) ? (reader.GetString(83)).ToString().Trim() : "";


                    makingcharge.Text = (reader["MakingCharge"] != DBNull.Value) ? (reader.GetDouble(94)).ToString().Trim() : "";
                    ratepergm.Text = (reader["RatePerGm"] != DBNull.Value) ? (reader.GetDouble(95)).ToString().Trim() : "";
                    wasteperc.Text = (reader["WastagePerc"] != DBNull.Value) ? (reader.GetDouble(96)).ToString().Trim() : "";
                    //cmbUnits.Text = tmpProduct.UnitID.ToString();
                    //CounterName.Text = (reader["CounterID"] != DBNull.Value) ? (reader.GetInt32(40)).ToString().Trim() : "";
                    //HSN.Text = tmpProduct.HSN.ToString();
                    //txtPrice.Text = tmpProduct.ItemPrice.ToString();
                    //txtGSTRate.Text = tmpProduct.GSTRate.ToString();
                    //TxtProdCode.Text = tmpProduct.ItemBarCode.ToString();
                    //Get Counter , Tray and Storage Name by another call, get all count by sp or direct call for inventory 


                }
            }
            else
            {
                MessageBox.Show("Item does not Found", "Not Found Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                txtBarcode.Clear();
                autocompleteItemNameStockEntry.autoTextBoxStockEntry.Clear();
                txtBarcode.Focus();

            }


            reader.Close();

        }


        public void BindComboBoxStorage(ComboBox storage)
        {
            var custAdpt = new StorageLocationsByPcTableAdapter();
            var custInfoVal = custAdpt.GetData();
            var LinqRes = (from UserRec in custInfoVal
                           orderby UserRec.StorageName ascending
                           //select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
                           select (UserRec.StorageName.Trim())).Distinct();
            StorageName.ItemsSource = LinqRes;
            // comboBoxName.SelectedValueBinding = new Binding("Col6");
        }

        public void BindComboBoxGroupName(ComboBox groupname)
        {
            var custAdpt = new StockGroupsTableAdapter();
            var custInfoVal = custAdpt.GetData();
            //var LinqRes = (from UserRec in custInfoVal
            //               orderby UserRec.GroupName ascending
            //               //select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
            //               select (UserRec.GroupName.Trim())).Distinct();
            //GroupName.ItemsSource = LinqRes;

            GroupName.ItemsSource = custInfoVal.Where(c => (c.ParentGroupName.Trim() == "Main"))
         .Select(x => x.GroupName.Trim()).Distinct().ToList();


            // comboBoxName.SelectedValueBinding = new Binding("Col6");

        }

        public void BindComboPurchasePartyName(ComboBox PurchasePartyName)
        {
            var custAdpt = new AccountsListTableAdapter();
            var custInfoVal = custAdpt.GetData();
            //var LinqRes = (from UserRec in custInfoVal
            //               orderby UserRec.GroupName ascending
            //               //select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
            //               select (UserRec.GroupName.Trim())).Distinct();
            //GroupName.ItemsSource = LinqRes;

            PurchasePartyName.ItemsSource = custInfoVal.Where(c => (c.PrimaryAcctName.Trim() == "Sundry Creditors"))
         .Select(x => x.AcctName.Trim()).Distinct().ToList();


            // comboBoxName.SelectedValueBinding = new Binding("Col6");

        }
        private void autocompleteItemName_LostFocus(object sender, RoutedEventArgs e)
        {
            //CleanUp();
           
            autocompleteItemNameStockEntry.autoTextBoxStockEntry.Background = Brushes.White;
            autocompleteItemNameStockEntry.autoTextBoxStockEntry.Foreground = Brushes.Black;


            //if (Regex.IsMatch(autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim(), @"^\d+$") || 1 == 1)
            //{
                
            //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            //    con.Open();
            //    string sql = "select * from StockItemsByPc where ItemName = '" + autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text + "' and CompID = '" + CompID + "'";
            //    SqlCommand cmd = new SqlCommand(sql);
            //    cmd.Connection = con;
            //    SqlDataReader reader = cmd.ExecuteReader();

            //    //string constr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\RTSProSoft\Database\InvWpf-Enhanced.accdb;";
            //    //OleDbConnection con = new OleDbConnection(constr);
            //    //string queryStr = @"select * from PurchaseInvoices where PartyName = '" + custnme + "'";
            //    //OleDbCommand command = new OleDbCommand(queryStr, con);
            //    //con.Open();
            //    //OleDbDataReader reader = command.ExecuteReader();

            //    while (reader.Read())
            //    {

            //        HSN.Text = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
            //        PrintName.Text = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
            //        //UnitID.Text = (reader["UnitID"] != DBNull.Value) ? (reader.GetInt32(4)).ToString().Trim() : "";
            //        //ItemCode.Text = (reader["ItemCode"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";


            //        ItemDesc.Text = (reader["ItemDesc"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
            //        txtBarcode.Text.Trim() = (reader["ItemBarCode"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
            //        ItemPrice.Text = (reader["ItemPrice"] != DBNull.Value) ? (reader.GetDouble(9)).ToString().Trim() : "";
            //        //SetCriticalLevel.Text = (reader["SetCriticalLevel"] != DBNull.Value) ? (reader.GetBoolean(12)).ToString().Trim() : "false";
            //        //SetDefaultStorageID.Text = (reader["SetDefaultStorageID"] != DBNull.Value) ? (reader.GetInt32(14)).ToString().Trim() : "";
            //        //DecimalPlaces.Text = (reader["DecimalPlaces"] != DBNull.Value) ? (reader.GetInt32(17)).ToString().Trim() : "";
            //        //HSN.Text = (reader["IsBarcodeCreated"] != DBNull.Value) ? (reader.GetBoolean(18)).ToString().Trim() : "false";
            //        //ItemPurchPrice.Text = (reader["ItemPurchPrice"] != DBNull.Value) ? (reader.GetDouble(23)).ToString().Trim() : "";
            //        ItemAlias.Text = (reader["ItemAlias"] != DBNull.Value) ? (reader.GetString(30).Trim()) : "";
            //        //get Group Name 
            //        autocompleteItemNameStockGroup.autoTextBoxStockGroup.Text = (reader["UnderGroupID"] != DBNull.Value) ? (reader.GetInt64(32)).ToString().Trim() : "";
            //        autocompleteItemNameStockSubGroup.autoTextBoxStockSubGroup.Text = (reader["UnderSubGroupID"] != DBNull.Value) ? (reader.GetInt64(34)).ToString().Trim() : "";
            //        ActualQty.Text = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)).ToString().Trim() : "";
            //        HSN.Text = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
            //        GSTRate.Text = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)).ToString().Trim() : "";
            //        //Get Name instead ID
            //        //StorageName.Text = (reader["StorageID"] != DBNull.Value) ? (reader.GetInt32(38)).ToString().Trim() : "";
            //        //TrayName.Text = (reader["TrayID"] != DBNull.Value) ? (reader.GetInt32(39)).ToString().Trim() : "";
            //        //CounterName.Text = (reader["CounterID"] != DBNull.Value) ? (reader.GetInt32(40)).ToString().Trim() : "";
            //        OpeningStock.Text = (reader["OpeningStock"] != DBNull.Value) ? (reader.GetDouble(41)).ToString().Trim() : "";
            //        OpeningStockValue.Text = (reader["OpeningStockValue"] != DBNull.Value) ? (reader.GetDouble(42)).ToString().Trim() : "";
            //        //tmpProduct.UpdateDate = reader.GetDateTime(44); //reader["UpdateDate"] != DBNull.Value) ? (reader.GetDateTime(44)) : "";  
            //        actualWt.Text = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)).ToString().Trim() : "";
            //        //tmpProduct.LastBuyDate = reader.GetDateTime(47); //(reader["LastBuyDate"] != DBNull.Value) ? (reader.GetDateTime(47) : "";
            //        //tmpProduct.LastSaleDate = reader.GetDateTime(48);//(reader["LastSaleDate"] != DBNull.Value) ? (reader.GetDateTime(48) : "";
            //        CurrentStockValue.Text = (reader["CurrentStockValue"] != DBNull.Value) ? (reader.GetDouble(49)).ToString().Trim() : "";
            //        LastSalePrice.Text = (reader["LastSalePrice"] != DBNull.Value) ? (reader.GetDouble(50)).ToString().Trim() : "";
            //        LastBuyPrice.Text = (reader["LastBuyPrice"] != DBNull.Value) ? (reader.GetDouble(51)).ToString().Trim() : "";

            //        OpeningStockWt.Text = (reader["OpeningStockWt"] != DBNull.Value) ? (reader.GetDouble(52)).ToString().Trim() : "";

            //        StorageName.Text = (reader["StorageName"] != DBNull.Value) ? (reader.GetString(79)).ToString().Trim() : "";
            //        TrayName.Text = (reader["TrayName"] != DBNull.Value) ? (reader.GetString(81)).ToString().Trim() : "";

            //        quality.Text = (reader["Quality"] != DBNull.Value) ? (reader.GetString(82)).ToString().Trim() : "";
            //        size.Text = (reader["Size"] != DBNull.Value) ? (reader.GetString(83)).ToString().Trim() : "";


            //        makingcharge.Text = (reader["MakingCharge"] != DBNull.Value) ? (reader.GetDouble(94)).ToString().Trim() : "";
            //        ratepergm.Text = (reader["RatePerGm"] != DBNull.Value) ? (reader.GetDouble(95)).ToString().Trim() : "";
            //        wasteperc.Text = (reader["WastagePerc"] != DBNull.Value) ? (reader.GetDouble(96)).ToString().Trim() : "";

            //        //CounterName.Text = (reader["CounterID"] != DBNull.Value) ? (reader.GetInt32(40)).ToString().Trim() : "";
            //        //HSN.Text = tmpProduct.HSN.ToString();
            //        //txtPrice.Text = tmpProduct.ItemPrice.ToString();
            //        //txtGSTRate.Text = tmpProduct.GSTRate.ToString();
            //        //TxtProdCode.Text = tmpProduct.ItemBarCode.ToString();
            //        //Get Counter , Tray and Storage Name by another call, get all count by sp or direct call for inventory 


            //    }
            //    reader.Close();
            //}
           


           
        }




        private void WindowAutoBar_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Tab)
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter)
            {
                autocompleteItemNameStockEntry.autoTextBoxStockEntry.Focus();
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


        private void Barcode_TextChanged(object sender, TextChangedEventArgs e)
        {
            CleanUp();
            if (txtBarcode.Text.Trim() != "")
            {
                AutoBarCodeNumber.Clear();
            }
            else
            {
                AutoBarCodeNumber.Text = autobarcodeNumber;
            }
            //string custnme = txtBarcode.Text.Trim();
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conn.Open();
            string sql = "select * from StockItemsByPc where LTRIM(RTRIM(ItemBarCode)) = '" + txtBarcode.Text.Trim() + "'  and CompID = '" + CompID + "'";
            //string sql = "select * from AccountsMaster where Barcode = '" + txtBarcode.Text.Trim() + "'";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = conn;
            SqlDataReader reader = cmd.ExecuteReader();

            //string constr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\RTSProSoft\Database\InvWpf-Enhanced.accdb;";
            //OleDbConnection con = new OleDbConnection(constr);
            //string queryStr = @"select * from PurchaseInvoices where PartyName = '" + custnme + "'";
            //OleDbCommand command = new OleDbCommand(queryStr, con);
            //con.Open();
            //OleDbDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                    //HSN.Text = (reader["HSN"] != DBNull.Value) ? (reader.GetString(18).Trim()) : "";
                    PrintName.Text = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                    //UnitID.Text = (reader["UnitID"] != DBNull.Value) ? (reader.GetInt32(4)).ToString().Trim() : "";
                    //ItemCode.Text = (reader["ItemCode"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";


                    ItemDesc.Text = (reader["ItemDesc"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                    //ItemBarCode.Text = (reader["ItemBarCode"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
                    ItemPrice.Text = (reader["ItemPrice"] != DBNull.Value) ? (reader.GetDouble(9)).ToString().Trim() : "";
                    //SetCriticalLevel.Text = (reader["SetCriticalLevel"] != DBNull.Value) ? (reader.GetBoolean(12)).ToString().Trim() : "false";
                    //SetDefaultStorageID.Text = (reader["SetDefaultStorageID"] != DBNull.Value) ? (reader.GetInt32(14)).ToString().Trim() : "";
                    //DecimalPlaces.Text = (reader["DecimalPlaces"] != DBNull.Value) ? (reader.GetInt32(17)).ToString().Trim() : "";
                    //HSN.Text = (reader["IsBarcodeCreated"] != DBNull.Value) ? (reader.GetBoolean(18)).ToString().Trim() : "false";
                    //ItemPurchPrice.Text = (reader["ItemPurchPrice"] != DBNull.Value) ? (reader.GetDouble(23)).ToString().Trim() : "";
                    ItemAlias.Text = (reader["ItemAlias"] != DBNull.Value) ? (reader.GetString(30).Trim()) : "";
                    //get Group Name 
                    GroupName.Text = (reader["UnderGroupName"] != DBNull.Value) ? (reader.GetString(31)).ToString().Trim() : "";
                    SubGroupName.Text = (reader["UnderSubGroupName"] != DBNull.Value) ? (reader.GetString(33)).ToString().Trim() : "";
                    ActualQty.Text = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)).ToString().Trim() : "";
                    HSN.Text = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
                    GSTRate.Text = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)).ToString().Trim() : "";
                    //Get Name instead ID
                    //StorageName.Text = (reader["StorageID"] != DBNull.Value) ? (reader.GetInt32(38)).ToString().Trim() : "";
                    //TrayName.Text = (reader["TrayID"] != DBNull.Value) ? (reader.GetInt32(39)).ToString().Trim() : "";
                    //CounterName.Text = (reader["CounterID"] != DBNull.Value) ? (reader.GetInt32(40)).ToString().Trim() : "";
                    OpeningStock.Text = (reader["OpeningStock"] != DBNull.Value) ? (reader.GetDouble(41)).ToString().Trim() : "";
                    OpeningStockValue.Text = (reader["OpeningStockValue"] != DBNull.Value) ? (reader.GetDouble(42)).ToString().Trim() : "";
                    //tmpProduct.UpdateDate = reader.GetDateTime(44); //reader["UpdateDate"] != DBNull.Value) ? (reader.GetDateTime(44)) : "";  
                    actualWt.Text = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)).ToString().Trim() : "";
                    //tmpProduct.LastBuyDate = reader.GetDateTime(47); //(reader["LastBuyDate"] != DBNull.Value) ? (reader.GetDateTime(47) : "";
                    //tmpProduct.LastSaleDate = reader.GetDateTime(48);//(reader["LastSaleDate"] != DBNull.Value) ? (reader.GetDateTime(48) : "";
                    CurrentStockValue.Text = (reader["CurrentStockValue"] != DBNull.Value) ? (reader.GetDouble(49)).ToString().Trim() : "";
                    LastSalePrice.Text = (reader["LastSalePrice"] != DBNull.Value) ? (reader.GetDouble(50)).ToString().Trim() : "";
                    LastBuyPrice.Text = (reader["LastBuyPrice"] != DBNull.Value) ? (reader.GetDouble(51)).ToString().Trim() : "";

                    OpeningStockWt.Text = (reader["OpeningStockWt"] != DBNull.Value) ? (reader.GetDouble(52)).ToString().Trim() : "";

                    StorageName.Text = (reader["StorageName"] != DBNull.Value) ? (reader.GetString(79)).ToString().Trim() : "";
                    TrayName.Text = (reader["TrayName"] != DBNull.Value) ? (reader.GetString(81)).ToString().Trim() : "";

                    quality.Text = (reader["Quality"] != DBNull.Value) ? (reader.GetString(82)).ToString().Trim() : "";
                    size.Text = (reader["Size"] != DBNull.Value) ? (reader.GetString(83)).ToString().Trim() : "";


                    makingcharge.Text = (reader["MakingCharge"] != DBNull.Value) ? (reader.GetDouble(94)).ToString().Trim() : "";
                    ratepergm.Text = (reader["RatePerGm"] != DBNull.Value) ? (reader.GetDouble(95)).ToString().Trim() : "";
                    wasteperc.Text = (reader["WastagePerc"] != DBNull.Value) ? (reader.GetDouble(96)).ToString().Trim() : "";

                    PurchaseInvoice.Text = (reader["PurchaseInvoice"] != DBNull.Value) ? (reader.GetString(97)).Trim() : "";
                    PurchasePartyName.Text = (reader["SundryCreditorName"] != DBNull.Value) ? (reader.GetString(70)).Trim() : "";
                    //cmbUnits.Text = tmpProduct.UnitID.ToString();
                    //CounterName.Text = (reader["CounterID"] != DBNull.Value) ? (reader.GetInt32(40)).ToString().Trim() : "";
                    //HSN.Text = tmpProduct.HSN.ToString();
                    //txtPrice.Text = tmpProduct.ItemPrice.ToString();
                    //txtGSTRate.Text = tmpProduct.GSTRate.ToString();
                    //TxtProdCode.Text = tmpProduct.ItemBarCode.ToString();
                    //Get Counter , Tray and Storage Name by another call, get all count by sp or direct call for inventory 




                }
            }
            else
            {
               // MessageBox.Show("Item does not Found", "Not Found Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
               // txtBarcode.Clear();
                autocompleteItemNameStockEntry.autoTextBoxStockEntry.Clear();
                txtBarcode.Focus();
              
            }
            

            reader.Close();
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

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        //private void PrintQuick()
        //{
        //    var retangulo = new iTextSharp.text.Rectangle(250, 40);
        //    //string barcodenumber = "";
        //    //if (ScannedCode.Text.Trim() == "")
        //    //{
        //    //    barcodenumber = AutoBarCodeNumber.Text;
        //    //}
        //    //else
        //    //    barcodenumber = ScannedCode.Text;

        //    string barcodenumber = "";
        //    if (txtBarcode.Text.Trim().Trim() == "")
        //    {
        //        barcodenumber = AutoBarCodeNumber.Text.Trim();
        //    }
        //    else
        //        barcodenumber = txtBarcode.Text.Trim();


        //    FileStream fs = File.Open(@"C:\ViewBill\Barcode\Barcode-" + barcodenumber + ".pdf", FileMode.Create);


        //    Document document = new Document(retangulo);
        //    //commented below for memort=y stream
        //    PdfWriter writer = PdfWriter.GetInstance(document, fs);
        //    document.Open();


        //    PdfContentByte cb = writer.DirectContent;

        //    BaseFont outraFonte = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, false, false);


        //    Barcode128 codeEAN13 = null;
        //    codeEAN13 = new Barcode128();
        //    codeEAN13.CodeType = Barcode.CODE128;

        //    codeEAN13.BarHeight = 8;  //Set this Barcode height
        //    //codeEAN13.AltText = "";

        //    //codeEAN13.TextAlignment = Element.ALIGN_RIGHT;

        //    //////////////

        //    //PDf , Regular Screen, Portrait
        //    // Go to Printer Settings 
        //    //SetBinding Width  = 98, height 15




        //    ///////////

        //    if (txtBarcode.Text.Trim().Trim() != "")
        //    {
        //        codeEAN13.Code = txtBarcode.Text.Trim().Trim();
        //    }
        //    else
        //    {
        //        codeEAN13.Code = AutoBarCodeNumber.Text.Trim();
        //    }
        //    //check standrad format of barcode

        //    iTextSharp.text.Image imgBarCode1 = codeEAN13.CreateImageWithBarcode(cb, null, null);
        //    //imgBarCode1.PaddingTop = 15;
        //    //imgBarCode1.SetAbsolutePosition(5, 10);
        //    //imgBarCode1.Alignment = iTextSharp.text.Image.ALIGN_RIGHT;

        //    //Maximum height is 800 pixels.
        //    //float percentage = 0.0f;
        //    //percentage = 300 / imgBarCode1.Height;
        //    imgBarCode1.ScalePercent(75);



        //    PdfPTable barcodeTable = new iTextSharp.text.pdf.PdfPTable(2) { TotalWidth = 250 };
        //    float[] widthsTotalTableHzl = new float[] { 100, 150 };
        //    barcodeTable.DefaultCell.Border = 0;


        //    barcodeTable.SetWidths(widthsTotalTableHzl);
        //    //barcodeTable.WidthPercentage = 100;
        //    //barcodeTable.TotalWidth = 100;

        //    //PdfPCell barcodeTableCell = new PdfPCell();
        //    //ourbankdetails1Cell.Border = 0;
        //    //barcodeTable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

        //    ////barcodeTableCell.AddElement(imgBarCode1);
        //    ////barcodeTableCell.AddElement(new Phrase("A/C#:"));
        //    ////barcodeTableCell.AddElement(new Phrase("2"));
        //    ////barcodeTableCell.AddElement(new Phrase("IFSC:"));
        //    ////barcodeTableCell.AddElement(new Phrase(firmBankAddress.Trim(), taxslabAmtFont));
        //    //barcodeTableCell.HorizontalAlignment = Element.ALIGN_LEFT;
        //    //barcodeTable.AddCell("");
        //    //barcodeTable.AddCell("wt");
        //    //barcodeTable.AddCell("quality");
        //    //ourbankdetails1.DefaultCell.Rowspan = 2;
        //    //barcodeTable.DefaultCell.BorderWidthRight = 0;
        //    //barcodeTable.DefaultCell.BorderWidthBottom = 0;
        //    //barcodeTable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
        //    //PdfPCell ourbankdetails1cell = new PdfPCell();
        //    //document.Add(barcodeTable);
        //    Font barfont = new Font(Font.FontFamily.HELVETICA, 5f, Font.BOLDITALIC, BaseColor.BLACK);
        //    barcodeTable.SplitLate = false;
        //    barcodeTable.SplitRows = true;
        //    PdfPCell itemdetails = new PdfPCell();
        //    itemdetails.Border = 0;
        //    itemdetails.AddElement(new Phrase(autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() + "," + actualWt.Text.Trim() + "gms" + "\n" + "Waste:" + wasteperc.Text.Trim() + "%," + quality.Text.Trim() + "\n", barfont));
        //    //itemdetails.AddElement(new Phrase(autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() + "," + actualWt.Text.Trim() +"gms"+ "\n"  +wasteperc.Text.Trim() +"%,"+ size.Text.Trim() + "," + quality.Text.Trim() + "\n", barfont));
        //    //barcodeTable.AddCell(new Phrase("ring , 9.26gm , 916 :", barfont));
        //    itemdetails.HorizontalAlignment = Element.ALIGN_LEFT;

        //    PdfPCell ForFirmCell = new PdfPCell();
        //    ForFirmCell.Border = 0;
        //    ForFirmCell.HorizontalAlignment = Element.ALIGN_CENTER;
        //    barcodeTable.AddCell(itemdetails);
        //    ForFirmCell.AddElement(imgBarCode1); //imgBarCode1ForFirmCell.
        //    ForFirmCell.PaddingTop = 7;
        //    barcodeTable.AddCell(ForFirmCell);

        //    //ForFirmCell.MinimumHeight = 40f;
        //    document.Add(barcodeTable);

        //    //Font barfont = new Font(Font.FontFamily.TIMES_ROMAN, 4f, Font.NORMAL, BaseColor.BLACK);

        //    //Phrase size = new Phrase("12*45", barfont);

        //    //document.Add(imgBarCode1);
        //    //iTextSharp.text.Paragraph wt = new iTextSharp.text.Paragraph("4.7gm") { Height:12,Width:10};


        //    //document.Add(size);
        //    //Phrase quality2 = new Phrase("916");
        //    //document.Add(quality2);             

        //    document.Close();



        //    try
        //    {
        //        //Open RTSProSoft Folder On PDf button Click
        //        Process process = new Process();
        //        process.StartInfo.UseShellExecute = true;
        //        if (txtBarcode.Text.Trim() == "")
        //        {
        //            process.StartInfo.FileName = @"C:\ViewBill\Barcode\barcode-" + AutoBarCodeNumber.Text.Trim() + ".pdf";
        //            //process.StartInfo.FileName = @"C:\ViewBill\" + "Bill-" + (invoiceNumber.Text).Trim() + "-" + custName.Text + ".pdf";
        //            //process.StartInfo.FileName = @"C:\RTSProSoft\";
        //        }
        //        else
        //            process.StartInfo.FileName = @"C:\ViewBill\Barcode\barcode-" + txtBarcode.Text.Trim() + ".pdf";

        //        process.Start();
        //        process.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("In Procees. Start");
        //    }


        //}

        private void PrintQuick()
        {

            var retangulo = new iTextSharp.text.Rectangle(250, 30);

            string barcodenumber = "";
            if (txtBarcode.Text.Trim().Trim() == "")
            {
                barcodenumber = AutoBarCodeNumber.Text.Trim();
            }
            else
                barcodenumber = txtBarcode.Text.Trim();

            FileStream fs = File.Open(@"C:\ViewBill\Barcode\Barcode-" + barcodenumber + ".pdf", FileMode.Create);
            Document document = new Document(retangulo);
            //commented below for memort=y stream
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();
            PdfContentByte cb = writer.DirectContent;
            BaseFont outraFonte = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, false, false);
            Barcode128 codeEAN13 = null;
            codeEAN13 = new Barcode128();
            codeEAN13.CodeType = Barcode.CODE128;
            codeEAN13.BarHeight = 9;  //Set this Barcode height

            if (txtBarcode.Text.Trim().Trim() != "")
            {
                codeEAN13.Code = txtBarcode.Text.Trim().Trim();
            }
            else
            {
                codeEAN13.Code = AutoBarCodeNumber.Text.Trim();
            }
            //check standrad format of barcode

            iTextSharp.text.Image imgBarCode1 = codeEAN13.CreateImageWithBarcode(cb, null, null);
            imgBarCode1.ScalePercent(90);

            //PdfPTable barcodeTable = new iTextSharp.text.pdf.PdfPTable(2) { TotalWidth = 400 }; //on 5 March
            ////float[] widthsTotalTableHzl = new float[] { 200, 200 }; revert if not fit
            //float[] widthsTotalTableHzl = new float[] { 170, 230 };

            PdfPTable barcodeTable = new iTextSharp.text.pdf.PdfPTable(2) { TotalWidth = 525 };
            //float[] widthsTotalTableHzl = new float[] { 200, 200 }; revert if not fit
            float[] widthsTotalTableHzl = new float[] { 250, 275 };

            barcodeTable.HorizontalAlignment = Element.ALIGN_LEFT;
            barcodeTable.DefaultCell.Border = 0;
            barcodeTable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;

            barcodeTable.SetWidths(widthsTotalTableHzl);
            //document.Add(barcodeTable);
            Font barfont = new Font(Font.FontFamily.HELVETICA, 6f, Font.BOLDITALIC, BaseColor.BLACK); // changed from 6.5 on 5 March
            barcodeTable.SplitLate = false;
            barcodeTable.SplitRows = true;
            PdfPCell itemdetails = new PdfPCell();
            itemdetails.Border = 0;

            if (GroupName.Text.Trim() == "Watch")
            {
                itemdetails.AddElement(new Phrase(autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() + "\n Price: ₹ " + ItemPrice.Text.Trim() , barfont));

            }
            else
                itemdetails.AddElement(new Phrase(autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() + "\n Wt: " + actualWt.Text.Trim() + "\n Waste: " + wasteperc.Text.Trim() + "%, " + quality.Text.Trim() + "\n", barfont));

            itemdetails.HorizontalAlignment = Element.ALIGN_LEFT;

     
            
            PdfPCell ForFirmCell = new PdfPCell();
            ForFirmCell.Border = 0;            
            ForFirmCell.HorizontalAlignment = Element.ALIGN_CENTER;
            ForFirmCell.VerticalAlignment = Element.ALIGN_CENTER;
            barcodeTable.AddCell(itemdetails);
            ForFirmCell.AddElement(imgBarCode1); //imgBarCode1ForFirmCell.
            barcodeTable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
            ForFirmCell.PaddingTop = 12;
            barcodeTable.AddCell(ForFirmCell);
            document.Add(barcodeTable);
       
            document.Close();

            try
            {
                //Open RTSProSoft Folder On PDf button Click
                //Process process = new Process();
                //process.StartInfo.UseShellExecute = true;
                //if (txtBarcode.Text.Trim() == "")
                //{
                //    process.StartInfo.FileName = @"C:\ViewBill\Barcode\barcode-" + AutoBarCodeNumber.Text.Trim() + ".pdf";
                //}
                //else
                //    process.StartInfo.FileName = @"C:\ViewBill\Barcode\barcode-" + txtBarcode.Text.Trim() + ".pdf";

                //process.Start();
                //process.Close();

                //Direct send pdf to Printer from the saved pdf location.
                ProcessStartInfo info = new ProcessStartInfo();
                info.Verb = "print";

                if (txtBarcode.Text.Trim() == "")
                {
                    info.FileName = @"C:\ViewBill\Barcode\barcode-" + AutoBarCodeNumber.Text.Trim() + ".pdf";
                }
                else
                    info.FileName = @"C:\ViewBill\Barcode\barcode-" + txtBarcode.Text.Trim() + ".pdf";

               
                info.CreateNoWindow = true;
                info.WindowStyle = ProcessWindowStyle.Hidden;

                Process p = new Process();
                p.StartInfo = info;
                p.Start();
                p.WaitForInputIdle();
                System.Threading.Thread.Sleep(5000);
                if (false == p.CloseMainWindow())
                {
                    p.Kill();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("In Procees. Start");
            }


        }
        private void Print_Click(object sender, RoutedEventArgs e)
        {
            if (autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() == "" || GroupName.SelectedItem == null)
            {
                MessageBox.Show("Item Name or Item Group is Empty", "Add Item Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                autocompleteItemNameStockEntry.autoTextBoxStockEntry.Focus();
            }
            else
            {


      

                //StockItems: CRUD Start
                //if (autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text != null)
                //{
                //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //if (txtBarcode.Text.Trim() != "")
                //{
                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                myConnSalesInvEntryStr.Open();
                string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPc where  ItemBarCode ='" + (txtBarcode.Text.Trim() != "" ? txtBarcode.Text.Trim() : AutoBarCodeNumber.Text.Trim()) + "' and CompID = '" + CompID + "'";
                //string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPc where ( ItemName='" + txtBarcode.Text.Trim() + "'  OR  ItemBarCode ='" + txtBarcode.Text.Trim() + "') and CompID = '" + CompID + "'";
                SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
                myCommand.Connection = myConnSalesInvEntryStr;

                //int countRec = myCommand.ExecuteNonQuery();
                int countRec = (int)myCommand.ExecuteScalar();
                myCommand.Connection.Close();

                //double dUnitID = (UnitID.Text.Trim() == "") ? 1 : Convert.ToInt32(UnitID.Text);
                double dItemPrice = (ItemPrice.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemPrice.Text.Trim());
                //double dItemPurchPrice = (ItemPurchPrice.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemPurchPrice.Text);
                double dItemMRP = (ItemMRP.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemMRP.Text.Trim());
                //double dItemMinSalePrice = (ItemMinSalePrice.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemMinSalePrice.Text);
                double dSetDefaultStorageID = 1;
                //Int32 dDecimalPlaces = (DecimalPlaces.Text.Trim() == "") ? 0 : Convert.ToInt32(DecimalPlaces.Text);
                double dSaleDiscount = (SaleDiscount.Text.Trim() == "") ? 0 : Convert.ToDouble(SaleDiscount.Text.Trim());
                double dActualQty = (ActualQty.Text.Trim() == "") ? 0 : Convert.ToDouble(ActualQty.Text.Trim());
                double dGSTRate = (GSTRate.Text.Trim() == "") ? 0 : Convert.ToDouble(GSTRate.Text.Trim());
                Int32 dStorageID = 1;
                Int32 dTrayID = 1;
                Int32 dCounterID = 1;
                //Int32 dStorageID = (StorageID.Text == "") ? 0 : Convert.ToInt32(StorageID.Text);
                //Int32 dTrayID = (TrayID.Text == "") ? 0 : Convert.ToInt32(TrayID.Text);
                //Int32 dCounterID = (CounterID.Text == "") ? 0 : Convert.ToInt32(CounterID.Text);
                double dOpeningStock = (OpeningStock.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningStock.Text.Trim());
                double dOpeningStockValue = (OpeningStockValue.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningStockValue.Text.Trim());
                double dActualWt = (actualWt.Text.Trim() == "") ? 0 : Convert.ToDouble(actualWt.Text.Trim());
                double dCurrentStockValue = (CurrentStockValue.Text.Trim() == "") ? 0 : Convert.ToDouble(CurrentStockValue.Text.Trim());
                double dLastSalePrice = (LastSalePrice.Text.Trim() == "") ? 0 : Convert.ToDouble(LastSalePrice.Text.Trim());
                double dLastBuyPrice = (LastBuyPrice.Text.Trim() == "") ? 0 : Convert.ToDouble(LastBuyPrice.Text.Trim());
                double dOpeningStockWt = (OpeningStockWt.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningStockWt.Text.Trim());
                double dRatepergm = (ratepergm.Text.Trim() == "") ? 0 : Convert.ToDouble(ratepergm.Text.Trim());
                double dWasteperc = (wasteperc.Text.Trim() == "") ? 0 : Convert.ToDouble(wasteperc.Text.Trim());
                double dmakingcharge = (makingcharge.Text.Trim() == "") ? 0 : Convert.ToDouble(makingcharge.Text.Trim());

                //SetCriticalLevel,SetImageUrl,SetDefaultStorageID,SetDefaultSundryCreditor,SetDefaultSundryDebtor,DecimalPlaces,SaleDiscount,ItemPurchPrice,ItemAlias,UnderGroupName,UnderSubGroupName,
                //ActualQty,HSN,GSTRate,StorageID,TrayID,CounterID,OpeningStock,OpeningStockValue,ActualWt,CurrentStockValue,LastSalePrice,LastBuyPrice,OpeningStockWt) Values ( '" + textBoxItemname.Text + "','" + PrintName.Text + "','" + Convert.ToInt32(UnitID.Text) + "','" + ItemCode.Text + "','" + ItemDesc.Text + "','" + ItemBarCode.Text + "','" + ItemMRP.Text + "','" + ItemPrice.Text + "','" + ItemMinSalePrice.Text + "','" + Convert.ToBoolean(SetCriticalLevel.Text) + "','" + SetImageUrl.Text + "','" + Convert.ToInt32(SetDefaultStorageID.Text) + "','" + Convert.ToInt32(SetDefaultSundryCreditor.Text) + "','" + Convert.ToInt32(SetDefaultSundryDebtor.Text) + "','" + Convert.ToInt32(DecimalPlaces.Text) + "','" + Convert.ToDouble(SaleDiscount.Text) + "','" + Convert.ToDouble(ItemPurchPrice.Text) + "','" + ItemAlias.Text + "','" + UnderGroupName.Text + "','" + UnderSubGroupName.Text + "','" + Convert.ToDouble(ActualQty.Text) + "','" + HSN.Text + "','" + Convert.ToDouble(GSTRate.Text) + "','" + Convert.ToInt32(StorageName.Text) + "','" + Convert.ToInt32(TrayName.Text) + "','" + Convert.ToInt32(CounterName.Text) + "','" + Convert.ToDouble(OpeningStock.Text) + "','" + Convert.ToDouble(OpeningStockValue.Text) + "','" + Convert.ToDouble(ActualWt.Text) + "','" + Convert.ToDouble(CurrentStockValue.Text) + "','" + Convert.ToDouble(LastSalePrice.Text) + "','" + Convert.ToDouble(LastBuyPrice.Text) + "','" + Convert.ToDouble(OpeningStockWt.Text) + "')";
                if (countRec != 0)
                {

                    string queryStrStockUpdate = "";
                    queryStrStockUpdate = "update StockItemsByPc  set  ItemName='" + autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() + "',PrintName='" + autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text + "',ItemDesc='" + ItemDesc.Text + "' , ItemBarCode ='" + (txtBarcode.Text.Trim() != "" ? txtBarcode.Text.Trim() : AutoBarCodeNumber.Text.Trim()) + "' ,ItemMRP='" + dItemMRP + "' ,ItemPrice='" + dItemPrice + "' ,SetImageUrl='" + ItemImageUrl.Text + "',SaleDiscount='" + dSaleDiscount + "' ,ItemPurchPrice='" + dLastBuyPrice + "' ,ItemAlias='" + ItemAlias.Text + "' ,UnderGroupName='" + GroupName.Text + "' ,UnderSubGroupName='" + SubGroupName.Text + "' ,ActualQty='" + dActualQty + "' ,HSN='" + HSN.Text + "' ,GSTRate='" + dGSTRate + "' ,OpeningStock='" + dOpeningStock + "' ,OpeningStockValue='" + dOpeningStockValue + "' ,ActualWt='" + dActualWt + "' ,CurrentStockValue='" + dCurrentStockValue + "' ,LastSalePrice='" + dLastSalePrice + "' ,LastBuyPrice='" + dLastBuyPrice + "' ,OpeningStockWt='" + dOpeningStockWt + "' ,StorageName='" + StorageName.Text + "',TrayName='" + TrayName.Text + "',Quality='" + quality.Text + "',[Size]='" + size.Text + "',MakingCharge='" + dmakingcharge + "',RatePerGm='" + dRatepergm + "',WastagePerc='" + dWasteperc + "' ,SundryCreditorName='" + PurchasePartyName.Text + "',PurchaseInvoice='" + PurchaseInvoice.Text.Trim() + "'  where ItemBarCode ='" + (txtBarcode.Text.Trim() != "" ? txtBarcode.Text.Trim() : AutoBarCodeNumber.Text.Trim()) + "' and CompID = '" + CompID + "' ";


                    SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                    myCommandStkUpdate.Connection.Open();
                    myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                    //if (autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() != "")
                    //{
                    // myCommandStk.Connection.Open();
                    int Num = myCommandStkUpdate.ExecuteNonQuery();
                    if (Num != 0)
                    {
                        MessageBox.Show("Record Successfully Updated....", "Update Record");
                    }
                    else
                    {
                        MessageBox.Show("Stock is not Updated....", "Update Record Error");
                    }
                    // myCommandStk.Connection.Close();
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Stock can not be updated....", "Update Record Error");
                    //}
                    myCommandStkUpdate.Connection.Close();

                    //PrintQuick();
                    //this.Close();
                    //StockEntry se = new StockEntry();
                    //se.ShowDialog();
                }
                else
                {

                    string CountAutoBarCodeItems = "SELECT COUNT(*) From StockItemsByPc where  ItemBarCode ='" + (txtBarcode.Text.Trim() != "" ? txtBarcode.Text.Trim() : AutoBarCodeNumber.Text.Trim()) + "' and CompID = '" + CompID + "'";
                    //string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPc where ( ItemName='" + txtBarcode.Text.Trim() + "'  OR  ItemBarCode ='" + txtBarcode.Text.Trim() + "') and CompID = '" + CompID + "'";
                    SqlCommand myCommandauto = new SqlCommand(CountAutoBarCodeItems, myConnSalesInvEntryStr);
                    myCommandauto.Connection.Open();
                    myCommandauto.Connection = myConnSalesInvEntryStr;

                    //int countRec = myCommand.ExecuteNonQuery();
                    int countRecauto = (int)myCommandauto.ExecuteScalar();
                    myCommandauto.Connection.Close();
                    if (countRecauto < 1)
                    {
                        string querySalesInvEntry = "";
                        //querySalesInvEntry = "insert into StockItems(ItemName, PrintName,ItemDesc,ItemPrice,ItemPurchPrice,UnderGroupName,UnderSubGroupName,ActualQty,HSN,GSTRate,OpeningStock,OpeningStockValue,ActualWt,CurrentStockValue,OpeningStockWt)   Values ( '" + textBoxItemname.Text + "','" + ItemDesc.Text + "','" + ItemBarCode.Text + "','" + ItemPrice.Text + "','" + Convert.ToDouble(ItemPurchPrice.Text) + "','" + UnderGroupName.Text + "','" + UnderSubGroupName.Text + "','" + Convert.ToDouble(ActualQty.Text) + "','" + HSN.Text + "','" + Convert.ToDouble(GSTRate.Text) + "','" + Convert.ToDouble(OpeningStock.Text) + "','" + Convert.ToDouble(OpeningStockValue.Text) + "','" + Convert.ToDouble(ActualWt.Text) + "','" + Convert.ToDouble(CurrentStockValue.Text) + "','" + Convert.ToDouble(OpeningStockWt.Text) + "')";
                        querySalesInvEntry = "insert into StockItemsByPc(ItemName, PrintName,ItemDesc,ItemBarCode,ItemMRP,ItemPrice,SetImageUrl,SaleDiscount,ItemPurchPrice,ItemAlias,UnderGroupName,UnderSubGroupName,ActualQty,ActualWt,HSN,GSTRate,StorageID,TrayID,CounterID,OpeningStock,OpeningStockValue,CurrentStockValue,LastSalePrice,LastBuyPrice,OpeningStockWt,CompID,StorageName,TrayName,Quality,Size,MakingCharge,RatePerGm,WastagePerc,PurchaseInvoice,SundryCreditorName)  Values ( '" + autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text + "','" + PrintName.Text + "','" + ItemDesc.Text + "','" + AutoBarCodeNumber.Text + "','" + dItemMRP + "','" + dItemPrice + "','" + ItemImageUrl.Text + "','" + '1' + "','" + dSaleDiscount + "','" + ItemAlias.Text + "','" + GroupName.Text + "','" + SubGroupName.Text + "','" + dActualQty + "','" + dActualWt + "','" + HSN.Text + "','" + dGSTRate + "',0,0,0,'" + dOpeningStock + "','" + dOpeningStockValue + "','" + dCurrentStockValue + "','" + dLastSalePrice + "','" + dLastBuyPrice + "','" + dOpeningStockWt + "', '" + CompID + "','" + StorageName.Text + "','" + TrayName.Text + "','" + quality.Text + "','" + size.Text + "','" + dmakingcharge + "','" + dRatepergm + "','" + dWasteperc + "','" + PurchaseInvoice.Text.Trim() + "','" + PurchasePartyName.Text + "')";
                        SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                        myCommandInvEntry.Connection.Open();
                        int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                        if (NumPInv != 0)
                        {
                            MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                            if (txtBarcode.Text.Trim().Trim() == "" && AutoBarCodeNumber.Text.Trim().Trim() != "")
                            {
                                if (AutoBarCodeNumber.Text.Trim().Trim() == autobarcodeNumber.Trim())
                                {
                                    SqlConnection consr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                                    consr.Open();
                                    string update = "";
                                    update = "update AutoIncrement  set  Number='" + (Convert.ToInt64(autobarcodeNumber.Trim()) + 1) + "' where Name ='BarCode' and Type='BarCode'   and CompID = '" + CompID + "' ";
                                    SqlCommand myCommandStkUpdate = new SqlCommand(update, consr);
                                    //myCommandStkUpdate.Connection.Open();
                                    myCommandStkUpdate.Connection = consr;
                                    // myCommandStk.Connection.Open();
                                    int Num = myCommandStkUpdate.ExecuteNonQuery();

                                    myCommandStkUpdate.Connection.Close();
                                }
                                //PrintQuick();
                                //this.Close();
                                //StockEntry se = new StockEntry();
                                //se.ShowDialog();
                            }


                            //string querySGEntry = "";
                            ////querySalesInvEntry = "insert into StockItems(ItemName, PrintName,ItemDesc,ItemPrice,ItemPurchPrice,UnderGroupName,UnderSubGroupName,ActualQty,HSN,GSTRate,OpeningStock,OpeningStockValue,ActualWt,CurrentStockValue,OpeningStockWt)   Values ( '" + textBoxItemname.Text + "','" + ItemDesc.Text + "','" + ItemBarCode.Text + "','" + ItemPrice.Text + "','" + Convert.ToDouble(ItemPurchPrice.Text) + "','" + UnderGroupName.Text + "','" + UnderSubGroupName.Text + "','" + Convert.ToDouble(ActualQty.Text) + "','" + HSN.Text + "','" + Convert.ToDouble(GSTRate.Text) + "','" + Convert.ToDouble(OpeningStock.Text) + "','" + Convert.ToDouble(OpeningStockValue.Text) + "','" + Convert.ToDouble(ActualWt.Text) + "','" + Convert.ToDouble(CurrentStockValue.Text) + "','" + Convert.ToDouble(OpeningStockWt.Text) + "')";
                            //querySalesInvEntry = "insert into StockGroups(GroupName, ,CompID)  Values ('" + autocompleteItemNameStockGroup.autoTextBoxStockGroup.Text + "','" + autocompleteItemNameStockSubGroup.autoTextBoxStockSubGroup.Text + "','" + dActualQty + "','" + HSN.Text + "','" + dGSTRate + "','" + StorageName.Text + "','" + TrayName.Text + "','" + CounterName.Text + "','" + dOpeningStock + "','" + dOpeningStockValue + "','" + dActualWt + "','" + dCurrentStockValue + "','" + dLastSalePrice + "','" + dLastBuyPrice + "','" + dOpeningStockWt + "', '" + CompID + "')";
                            //SqlCommand myCommandSGEntry = new SqlCommand(querySGEntry, myConnSalesInvEntryStr);

                            //myCommandSGEntry.Connection.Open();
                            //int SGEntr = myCommandSGEntry.ExecuteNonQuery();

                            // MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                        }
                        else
                        {
                            MessageBox.Show("Stock is not Inserted....", "Insert Record Error");
                        }
                        myCommandInvEntry.Connection.Close();

                        // myConnStock.Close();
                    }
                    else
                    {
                        MessageBox.Show("Auto Barccode Item Already There , Please select another barcode number....", "Auto Barcode Insert Error");
                    }

                }
                //}

                PrintQuick();
                this.Close();
                StockEntry se = new StockEntry();
                se.ShowDialog();



            }
        }

        //COde ForWindow Closing Confirmation
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //if (MessageBox.Show("ARE YOU WANT TO CLOSE?", "CLOSING", MessageBoxButton.YesNo) == MessageBoxResult.No)
            //{
            //    e.Cancel = true;
            //}
            //else
            //{
                myPrinters.SetDefaultPrinter(resetPrinter); 
            //}
        }

        //private FlowDocument CreateFlowDocumentJewellery()
        //{
        //    //  Get Confirmation that data saved successfull, 




        //    // create document and register styles
        //    FlowDocument doc = new FlowDocument();
        //    doc.ColumnWidth = 1024;
        //    doc.Name = "FlowDoc";
        //    doc.PageHeight = 600;
        //    doc.PageWidth = 800;
        //    doc.MinPageWidth = 800;


        //    /* style for products table header, assigned via type + class selectors */

        //    System.Windows.Documents.Paragraph p = new System.Windows.Documents.Paragraph();

        //    Span s = new Span();

        //    s = new Span(new Run(CompanyName));
        //    s.FontWeight = FontWeights.Bold;

        //    s.FontSize = 20;
        //    s.Inlines.Add(new LineBreak());//Line break is used for next line.  

        //    Span a1 = new Span();
        //    a1 = new Span(new Run("GSTIN: " + GSTIN));
        //    a1.Inlines.Add(new LineBreak());//Line break is used for next line.  

        //    Span a2 = new Span();
        //    a2 = new Span(new Run(Address + "," + Address2 + "," + City + "-" + PinCode + "," + State));
        //    a2.FontSize = 11;
        //    a2.Inlines.Add(new LineBreak());//Line break is used for next line.  

        //    Span a3 = new Span();
        //    a3 = new Span(new Run("Tax Invoice"));
        //    a3.Inlines.Add(new LineBreak());//Line break is used for next line.  

        //    Span a4 = new Span();
        //    a4 = new Span(new Run("Invoice# " + invoiceNumber.Text));
        //    a4.FontWeight = FontWeights.Bold;
        //    a4.Inlines.Add(new LineBreak());//Line break is used for next line.  

        //    Span a4acc = new Span();
        //    a4acc = new Span(new Run(autocompltCustName.autoTextBoxCustNameBarcode.Text + " : " + CashCustName.Text));
        //    a4acc.FontWeight = FontWeights.Bold;
        //    a4acc.Inlines.Add(new LineBreak());//Line break is used for next line.  


        //    Span a4date = new Span();
        //    a4date = new Span(new Run("Date: " + invDate.Text));
        //    a4date.Inlines.Add(new LineBreak());//Line break is used for next line.  

        //    Span a5 = new Span();
        //    a5 = new Span(new Run("---------------------------------------------------------------------------------------------------------"));
        //    //a5.Inlines.Add(new LineBreak());//Line break is used for next line.  
        //    p.FontSize = 12;
        //    p.Inlines.Add(s);// Add the span content into paragraph.  
        //    // p.Inlines.Add(a1);// Add the span content into paragraph.  
        //    p.Inlines.Add(a2);// Add the span content into paragraph.  
        //    p.Inlines.Add(a3);// Add the span content into paragraph.  
        //    p.Inlines.Add(a3);// Add the span content into paragraph.  
        //    p.Inlines.Add(a4);// Add the span content into paragraph.  
        //    p.Inlines.Add(a4acc);// Add the span content into paragraph.  
        //    p.Inlines.Add(a4date);// Add the span content into paragraph.  
        //    p.Inlines.Add(a5);// Add the span content into paragraph. 

        //    //If we have some dynamic text the span in flow document does not under "    " as space and we need to use "\t"  for space.  
        //    // s = new Span(new Run(s1 + "\t" + s2));//we need to use \t for space between s1 and s2 content.  
        //    //s.Inlines.Add(new LineBreak());
        //    //p.Inlines.Add(s);
        //    //Give style and formatting to paragraph content.  
        //    p.FontSize = 13;
        //    p.FontStyle = FontStyles.Normal;
        //    p.TextAlignment = TextAlignment.Center;
        //    p.FontFamily = new FontFamily("Century Gothic");
        //    doc.Blocks.Add(p);

        //    doc.Name = "FlowDoc";
        //    //doc.PageWidth = 900;
        //    doc.PagePadding = new Thickness(50, 30, 10, 5); //v3
        //    //doc.PagePadding = new Thickness(30, 20, 10, 5); //V2 
        //    // Create IDocumentPaginatorSource from FlowDocument
        //    // IDocumentPaginatorSource idpSource = doc;
        //    // Call PrintDocument method to send document to printer



        //    return doc;


        //}


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

                if (autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() == "" || GroupName.SelectedItem==null )
                    {
                        MessageBox.Show("Item Name or Item Group is Empty", "Add Item Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        autocompleteItemNameStockEntry.autoTextBoxStockEntry.Focus();
                    }
                    else
                    {


            //string FinYrStartdate = FinYeraStartDate.SelectedDate.ToString();

            //// DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            //DateTime dtin = Convert.ToDateTime(FinYrStartdate);
            ////DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //int yearsin = dtin.Year;
            //string monthsin = dtin.Month.ToString();
            //if (dtin.Month < 10)
            //{
            //    monthsin = "0" + monthsin;
            //}
            //string daysin = dtin.Day.ToString();
            //if (dtin.Day < 10)
            //{
            //    daysin = "0" + daysin;
            //}

            //string FinYrStartdateVal = yearsin + "/" + monthsin + "/" + daysin;


            //string BookStartdate = BookStartDate.SelectedDate.ToString();

            //// DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            //DateTime dtinb = Convert.ToDateTime(BookStartdate);
            ////DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //int yearsinb = dtinb.Year;
            //string monthsinb = dtinb.Month.ToString();
            //if (dtinb.Month < 10)
            //{
            //    monthsinb = "0" + monthsinb;
            //}
            //string daysinb = dtinb.Day.ToString();
            //if (dtinb.Day < 10)
            //{
            //    daysinb = "0" + daysinb;
            //}

            //string BookStartdateVal = yearsinb + "/" + monthsinb + "/" + daysinb;




            //StockItems: CRUD Start
            //if (autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text != null)
            //{
                //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //if (txtBarcode.Text.Trim() != "")
            //{
                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                myConnSalesInvEntryStr.Open();
                string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPc where  ItemBarCode ='" + (txtBarcode.Text.Trim() != "" ? txtBarcode.Text.Trim() : AutoBarCodeNumber.Text.Trim()) + "' and CompID = '" + CompID + "'";    
            //string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPc where ( ItemName='" + txtBarcode.Text.Trim() + "'  OR  ItemBarCode ='" + txtBarcode.Text.Trim() + "') and CompID = '" + CompID + "'";
                SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
                myCommand.Connection = myConnSalesInvEntryStr;

                //int countRec = myCommand.ExecuteNonQuery();
                int countRec = (int)myCommand.ExecuteScalar();
                myCommand.Connection.Close();

                //double dUnitID = (UnitID.Text.Trim() == "") ? 1 : Convert.ToInt32(UnitID.Text);
                double dItemPrice = (ItemPrice.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemPrice.Text.Trim());
                //double dItemPurchPrice = (ItemPurchPrice.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemPurchPrice.Text);
                double dItemMRP = (ItemMRP.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemMRP.Text.Trim());
                //double dItemMinSalePrice = (ItemMinSalePrice.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemMinSalePrice.Text);
                double dSetDefaultStorageID = 1;
                //Int32 dDecimalPlaces = (DecimalPlaces.Text.Trim() == "") ? 0 : Convert.ToInt32(DecimalPlaces.Text);
                double dSaleDiscount = (SaleDiscount.Text.Trim() == "") ? 0 : Convert.ToDouble(SaleDiscount.Text.Trim());
                double dActualQty = (ActualQty.Text.Trim() == "") ? 0 : Convert.ToDouble(ActualQty.Text.Trim());
                double dGSTRate = (GSTRate.Text.Trim() == "") ? 0 : Convert.ToDouble(GSTRate.Text.Trim());
                Int32 dStorageID = 1;
                Int32 dTrayID = 1;
                Int32 dCounterID = 1;
                //Int32 dStorageID = (StorageID.Text == "") ? 0 : Convert.ToInt32(StorageID.Text);
                //Int32 dTrayID = (TrayID.Text == "") ? 0 : Convert.ToInt32(TrayID.Text);
                //Int32 dCounterID = (CounterID.Text == "") ? 0 : Convert.ToInt32(CounterID.Text);
                double dOpeningStock = (OpeningStock.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningStock.Text.Trim());
                double dOpeningStockValue = (OpeningStockValue.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningStockValue.Text.Trim());
                double dActualWt = (actualWt.Text.Trim() == "") ? 0 : Convert.ToDouble(actualWt.Text.Trim());
                double dCurrentStockValue = (CurrentStockValue.Text.Trim() == "") ? 0 : Convert.ToDouble(CurrentStockValue.Text.Trim());
                double dLastSalePrice = (LastSalePrice.Text.Trim() == "") ? 0 : Convert.ToDouble(LastSalePrice.Text.Trim());
                double dLastBuyPrice = (LastBuyPrice.Text.Trim() == "") ? 0 : Convert.ToDouble(LastBuyPrice.Text.Trim());
                double dOpeningStockWt = (OpeningStockWt.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningStockWt.Text.Trim());
                double dRatepergm = (ratepergm.Text.Trim() == "") ? 0 : Convert.ToDouble(ratepergm.Text.Trim());
                double dWasteperc = (wasteperc.Text.Trim() == "") ? 0 : Convert.ToDouble(wasteperc.Text.Trim());
                double dmakingcharge = (makingcharge.Text.Trim() == "") ? 0 : Convert.ToDouble(makingcharge.Text.Trim());

                //SetCriticalLevel,SetImageUrl,SetDefaultStorageID,SetDefaultSundryCreditor,SetDefaultSundryDebtor,DecimalPlaces,SaleDiscount,ItemPurchPrice,ItemAlias,UnderGroupName,UnderSubGroupName,
                //ActualQty,HSN,GSTRate,StorageID,TrayID,CounterID,OpeningStock,OpeningStockValue,ActualWt,CurrentStockValue,LastSalePrice,LastBuyPrice,OpeningStockWt) Values ( '" + textBoxItemname.Text + "','" + PrintName.Text + "','" + Convert.ToInt32(UnitID.Text) + "','" + ItemCode.Text + "','" + ItemDesc.Text + "','" + ItemBarCode.Text + "','" + ItemMRP.Text + "','" + ItemPrice.Text + "','" + ItemMinSalePrice.Text + "','" + Convert.ToBoolean(SetCriticalLevel.Text) + "','" + SetImageUrl.Text + "','" + Convert.ToInt32(SetDefaultStorageID.Text) + "','" + Convert.ToInt32(SetDefaultSundryCreditor.Text) + "','" + Convert.ToInt32(SetDefaultSundryDebtor.Text) + "','" + Convert.ToInt32(DecimalPlaces.Text) + "','" + Convert.ToDouble(SaleDiscount.Text) + "','" + Convert.ToDouble(ItemPurchPrice.Text) + "','" + ItemAlias.Text + "','" + UnderGroupName.Text + "','" + UnderSubGroupName.Text + "','" + Convert.ToDouble(ActualQty.Text) + "','" + HSN.Text + "','" + Convert.ToDouble(GSTRate.Text) + "','" + Convert.ToInt32(StorageName.Text) + "','" + Convert.ToInt32(TrayName.Text) + "','" + Convert.ToInt32(CounterName.Text) + "','" + Convert.ToDouble(OpeningStock.Text) + "','" + Convert.ToDouble(OpeningStockValue.Text) + "','" + Convert.ToDouble(ActualWt.Text) + "','" + Convert.ToDouble(CurrentStockValue.Text) + "','" + Convert.ToDouble(LastSalePrice.Text) + "','" + Convert.ToDouble(LastBuyPrice.Text) + "','" + Convert.ToDouble(OpeningStockWt.Text) + "')";
                if (countRec != 0)
                {

                    string queryStrStockUpdate = "";
                    queryStrStockUpdate = "update StockItemsByPc  set  ItemName='" + autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() + "',PrintName='" + autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text + "',ItemDesc='" + ItemDesc.Text + "' , ItemBarCode ='" + (txtBarcode.Text.Trim() != "" ? txtBarcode.Text.Trim() : AutoBarCodeNumber.Text.Trim()) + "' ,ItemMRP='" + dItemMRP + "' ,ItemPrice='" + dItemPrice + "' ,SetImageUrl='" + ItemImageUrl.Text + "',SaleDiscount='" + dSaleDiscount + "' ,ItemPurchPrice='" + dLastBuyPrice + "' ,ItemAlias='" + ItemAlias.Text + "' ,UnderGroupName='" + GroupName.Text + "' ,UnderSubGroupName='" + SubGroupName.Text + "' ,ActualQty='" + dActualQty + "' ,HSN='" + HSN.Text + "' ,GSTRate='" + dGSTRate + "' ,OpeningStock='" + dOpeningStock + "' ,OpeningStockValue='" + dOpeningStockValue + "' ,ActualWt='" + dActualWt + "' ,CurrentStockValue='" + dCurrentStockValue + "' ,LastSalePrice='" + dLastSalePrice + "' ,LastBuyPrice='" + dLastBuyPrice + "' ,OpeningStockWt='" + dOpeningStockWt + "' ,StorageName='" + StorageName.Text + "',TrayName='" + TrayName.Text + "',Quality='" + quality.Text + "',[Size]='" + size.Text + "',MakingCharge='" + dmakingcharge + "',RatePerGm='" + dRatepergm + "',WastagePerc='" + dWasteperc + "' ,SundryCreditorName='" + PurchasePartyName.Text + "',PurchaseInvoice='" + PurchaseInvoice.Text.Trim() + "'  where ItemBarCode ='" + (txtBarcode.Text.Trim() != "" ? txtBarcode.Text.Trim() : AutoBarCodeNumber.Text.Trim()) + "' and CompID = '" + CompID + "' ";

                    
                    SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                    myCommandStkUpdate.Connection.Open();
                    myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                    //if (autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text.Trim() != "")
                    //{
                        // myCommandStk.Connection.Open();
                        int Num = myCommandStkUpdate.ExecuteNonQuery();
                        if (Num != 0)
                        {
                            MessageBox.Show("Record Successfully Updated....", "Update Record");
                        }
                        else
                        {
                            MessageBox.Show("Stock is not Updated....", "Update Record Error");
                        }
                        // myCommandStk.Connection.Close();
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Stock can not be updated....", "Update Record Error");
                    //}
                    myCommandStkUpdate.Connection.Close();

                    //PrintQuick();
                    //this.Close();
                    //StockEntry se = new StockEntry();
                    //se.ShowDialog();
                }
                else
                {

                    string CountAutoBarCodeItems = "SELECT COUNT(*) From StockItemsByPc where  ItemBarCode ='" + AutoBarCodeNumber.Text.Trim() + "' and CompID = '" + CompID + "'";
                    //string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPc where ( ItemName='" + txtBarcode.Text.Trim() + "'  OR  ItemBarCode ='" + txtBarcode.Text.Trim() + "') and CompID = '" + CompID + "'";
                    SqlCommand myCommandauto = new SqlCommand(CountAutoBarCodeItems, myConnSalesInvEntryStr);
                    myCommandauto.Connection.Open();
                    myCommandauto.Connection = myConnSalesInvEntryStr;

                    //int countRec = myCommand.ExecuteNonQuery();
                    int countRecauto = (int)myCommandauto.ExecuteScalar();
                    myCommandauto.Connection.Close();
                    if (countRecauto < 1)
                    {
                        string querySalesInvEntry = "";
                        //querySalesInvEntry = "insert into StockItems(ItemName, PrintName,ItemDesc,ItemPrice,ItemPurchPrice,UnderGroupName,UnderSubGroupName,ActualQty,HSN,GSTRate,OpeningStock,OpeningStockValue,ActualWt,CurrentStockValue,OpeningStockWt)   Values ( '" + textBoxItemname.Text + "','" + ItemDesc.Text + "','" + ItemBarCode.Text + "','" + ItemPrice.Text + "','" + Convert.ToDouble(ItemPurchPrice.Text) + "','" + UnderGroupName.Text + "','" + UnderSubGroupName.Text + "','" + Convert.ToDouble(ActualQty.Text) + "','" + HSN.Text + "','" + Convert.ToDouble(GSTRate.Text) + "','" + Convert.ToDouble(OpeningStock.Text) + "','" + Convert.ToDouble(OpeningStockValue.Text) + "','" + Convert.ToDouble(ActualWt.Text) + "','" + Convert.ToDouble(CurrentStockValue.Text) + "','" + Convert.ToDouble(OpeningStockWt.Text) + "')";
                        querySalesInvEntry = "insert into StockItemsByPc(ItemName, PrintName,ItemDesc,ItemBarCode,ItemMRP,ItemPrice,SetImageUrl,SaleDiscount,ItemPurchPrice,ItemAlias,UnderGroupName,UnderSubGroupName,ActualQty,ActualWt,HSN,GSTRate,StorageID,TrayID,CounterID,OpeningStock,OpeningStockValue,CurrentStockValue,LastSalePrice,LastBuyPrice,OpeningStockWt,CompID,StorageName,TrayName,Quality,Size,MakingCharge,RatePerGm,WastagePerc,PurchaseInvoice,SundryCreditorName)  Values ( '" + autocompleteItemNameStockEntry.autoTextBoxStockEntry.Text + "','" + PrintName.Text + "','" + ItemDesc.Text + "','" + AutoBarCodeNumber.Text + "','" + dItemMRP + "','" + dItemPrice + "','" + ItemImageUrl.Text + "','" + '1' + "','" + dSaleDiscount + "','" + ItemAlias.Text + "','" + GroupName.Text + "','" + SubGroupName.Text + "','" + dActualQty + "','" + dActualWt + "','" + HSN.Text + "','" + dGSTRate + "',0,0,0,'" + dOpeningStock + "','" + dOpeningStockValue + "','" + dCurrentStockValue + "','" + dLastSalePrice + "','" + dLastBuyPrice + "','" + dOpeningStockWt + "', '" + CompID + "','" + StorageName.Text + "','" + TrayName.Text + "','" + quality.Text + "','" + size.Text + "','" + dmakingcharge + "','" + dRatepergm + "','" + dWasteperc + "','" + PurchaseInvoice.Text.Trim() + "','" + PurchasePartyName.Text + "')";
                        SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                        myCommandInvEntry.Connection.Open();
                        int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                        if (NumPInv != 0)
                        {
                            MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                            if (txtBarcode.Text.Trim().Trim() == "" && AutoBarCodeNumber.Text.Trim().Trim() != "")
                            {
                                if (AutoBarCodeNumber.Text.Trim().Trim() == autobarcodeNumber.Trim())
                                {
                                    SqlConnection consr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                                    consr.Open();
                                    string update = "";
                                    update = "update AutoIncrement  set  Number='" + (Convert.ToInt64(autobarcodeNumber.Trim()) + 1) + "' where Name ='BarCode' and Type='BarCode'   and CompID = '" + CompID + "' ";
                                    SqlCommand myCommandStkUpdate = new SqlCommand(update, consr);
                                    //myCommandStkUpdate.Connection.Open();
                                    myCommandStkUpdate.Connection = consr;
                                    // myCommandStk.Connection.Open();
                                    int Num = myCommandStkUpdate.ExecuteNonQuery();

                                    myCommandStkUpdate.Connection.Close();
                                }
                                //PrintQuick();
                                //this.Close();
                                //StockEntry se = new StockEntry();
                                //se.ShowDialog();
                            }
                            

                            //string querySGEntry = "";
                            ////querySalesInvEntry = "insert into StockItems(ItemName, PrintName,ItemDesc,ItemPrice,ItemPurchPrice,UnderGroupName,UnderSubGroupName,ActualQty,HSN,GSTRate,OpeningStock,OpeningStockValue,ActualWt,CurrentStockValue,OpeningStockWt)   Values ( '" + textBoxItemname.Text + "','" + ItemDesc.Text + "','" + ItemBarCode.Text + "','" + ItemPrice.Text + "','" + Convert.ToDouble(ItemPurchPrice.Text) + "','" + UnderGroupName.Text + "','" + UnderSubGroupName.Text + "','" + Convert.ToDouble(ActualQty.Text) + "','" + HSN.Text + "','" + Convert.ToDouble(GSTRate.Text) + "','" + Convert.ToDouble(OpeningStock.Text) + "','" + Convert.ToDouble(OpeningStockValue.Text) + "','" + Convert.ToDouble(ActualWt.Text) + "','" + Convert.ToDouble(CurrentStockValue.Text) + "','" + Convert.ToDouble(OpeningStockWt.Text) + "')";
                            //querySalesInvEntry = "insert into StockGroups(GroupName, ,CompID)  Values ('" + autocompleteItemNameStockGroup.autoTextBoxStockGroup.Text + "','" + autocompleteItemNameStockSubGroup.autoTextBoxStockSubGroup.Text + "','" + dActualQty + "','" + HSN.Text + "','" + dGSTRate + "','" + StorageName.Text + "','" + TrayName.Text + "','" + CounterName.Text + "','" + dOpeningStock + "','" + dOpeningStockValue + "','" + dActualWt + "','" + dCurrentStockValue + "','" + dLastSalePrice + "','" + dLastBuyPrice + "','" + dOpeningStockWt + "', '" + CompID + "')";
                            //SqlCommand myCommandSGEntry = new SqlCommand(querySGEntry, myConnSalesInvEntryStr);

                            //myCommandSGEntry.Connection.Open();
                            //int SGEntr = myCommandSGEntry.ExecuteNonQuery();

                            // MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                        }
                        else
                        {
                            MessageBox.Show("Stock is not Inserted....", "Insert Record Error");
                        }
                        myCommandInvEntry.Connection.Close();

                        // myConnStock.Close();
                    }
                    else
                    {
                        MessageBox.Show("Auto Barccode Item Already There , Please select another barcode number....", "Auto Barcode Insert Error");
                    }

                }
        //}

                   // PrintQuick();
                    this.Close();
                    StockEntry se = new StockEntry();
                    se.ShowDialog();



            }
        }

        //this method will clear/reset form values
        private void CleanUp()
        {
            PurchasePartyName.Text = "";
            PurchaseInvoice.Clear();
            size.Clear();
            //VoucherNumber.Clear();
            LastBuyDate.SelectedDate = DateTime.Now;
            quality.Clear();
            color.Clear();
            designnumber.Clear();
            ItemPrice.Clear();
            HSN.Clear();
            OpeningStock.Clear();
            //ActualQty.Clear();
            ActualQty.Text="1";
            HSN.Text = "7113";
            //GSTRate.Clear();
            GSTRate.Text = "3";
            OpeningStockValue.Clear();
            ActualWt.Clear();
            CurrentStockValue.Clear();
            OpeningStockWt.Clear();
            LastSalePrice.Clear();
            LastBuyPrice.Clear();
            SaleDiscount.Clear();
            ActualCurrentStkQty.Clear();
            actualWt.Clear();
            ratepergm.Clear();
            wasteperc.Clear();
            makingcharge.Clear();
            ItemAlias.Text = "";
            ItemMRP.Text = "";
            ItemImageUrl.Text = "";
            ItemDesc.Text = "";
            PrintName.Text = "";
            GroupName.Text = "";
            SubGroupName.Text = "";

            //StorageName.ClearValue();
            //TrayName.ClearValue();
          

        }

        private void AddGroup_Click(object sender, RoutedEventArgs e)
        {
            //AddStockGroup asg = new AddStockGroup();
            //asg.ShowDialog();
        }

        private void AddSubGroup_Click(object sender, RoutedEventArgs e)
        {
            //AddStockSubGroup asg = new AddStockSubGroup();
            //asg.ShowDialog();
        }

        private void StorageName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StorageName.SelectedItem != null)
            {
                string storagenameselected = StorageName.SelectedItem.ToString();
                BindComboBoxTray(storagenameselected);
            }

        }

        private void GroupName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupName.SelectedItem != null)
            {
                string storagenameselected = GroupName.SelectedItem.ToString();
                BindComboBoxSubGroup(storagenameselected);
            }

        }

        public void BindComboBoxSubGroup(string groupname)
        {
            var custAdpt = new StockGroupsTableAdapter();
            var custInfoVal = custAdpt.GetData();
            //var LinqRes = (from UserRec in custInfoVal
            //               orderby UserRec.StorageName ascending
            //               select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
            //StorageName.ItemsSource = LinqRes;


            SubGroupName.ItemsSource = custInfoVal.Where(c => (c.ParentGroupName.Trim() == groupname.Trim()))
                     .Select(x => x.GroupName.Trim()).Distinct().ToList();
            //TrayName.SelectedItem = "Cash";

            // comboBoxName.SelectedValueBinding = new Binding("Col6");
        }


        public void BindComboBoxTray(string storagenameselected)
        {
            var custAdpt = new TrayListInStorageByPcTableAdapter();
            var custInfoVal = custAdpt.GetData();
            //var LinqRes = (from UserRec in custInfoVal
            //               orderby UserRec.StorageName ascending
            //               select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
            //StorageName.ItemsSource = LinqRes;


            TrayName.ItemsSource = custInfoVal.Where(c => (c.StorageName.Trim() == storagenameselected.Trim()))
                     .Select(x => x.TrayName.Trim()).Distinct().ToList();
            //TrayName.SelectedItem = "Cash";

            // comboBoxName.SelectedValueBinding = new Binding("Col6");
        }

        private void AutoBarCodeNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBarcode.Text.Trim().Trim() != "")
            {
              txtBarcode.Clear();
            }
          
        }

        private void txtBarcode_GotFocus(object sender, RoutedEventArgs e)
        {
            AutoBarCodeNumber.Clear();
            //autocompleteItemNameStockEntry.autoTextBoxStockEntry.Clear();
        }

        private void txtBarcode_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtBarcode.Text.Trim().Trim() == "")
            {
                AutoBarCodeNumber.Text = autobarcodeNumber;
            }
           
        }

        private void AutoBarCodeNumber_GotFocus(object sender, RoutedEventArgs e)
        {
            txtBarcode.Clear();
            autocompleteItemNameStockEntry.autoTextBoxStockEntry.Clear();
            CleanUp();                    
            AutoBarCodeNumber.Text = autobarcodeNumber;
        }


    }
}
