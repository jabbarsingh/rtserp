using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    /// Interaction logic for ShowItemInfo.xaml
    /// </summary>
    public partial class ShowItemInfo : Window
    {
        string itemnames = "";
        string companyId = "";
        public ShowItemInfo()
        {
        }
        public ShowItemInfo(string itemName, string CompID)
        {
            InitializeComponent();
            itemnames = itemName;
            companyId = CompID;
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select * from StockItems where ItemName = '" + itemName + "' and CompID = '" + CompID + "'";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = con;
            SqlDataReader reader = cmd.ExecuteReader();

            Product tmpProductInfo = new Product();

            while (reader.Read())
            {
                string isSoldAlert = (reader["IsSoldFlag"] != DBNull.Value) ? (reader.GetBoolean(72).ToString()) : "False";
                //if (isSoldAlert == "True")
                //{
                //    //MessageBox.Show("Item is Sold Out !");
                //}
                //else
                //{

                //var CustID = reader.GetValue(0).ToString();

                tmpProductInfo.ItemName = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                tmpProductInfo.PrintName = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                tmpProductInfo.UnitID = (reader["UnitID"] != DBNull.Value) ? (reader.GetString(4)) : "";
                tmpProductInfo.ItemCode = (reader["ItemCode"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";

                //tmpProductInfo.HSN = "9503";  //HSN

                tmpProductInfo.ItemDesc = (reader["ItemDesc"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                tmpProductInfo.ItemBarCode = (reader["ItemBarCode"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
                tmpProductInfo.ItemPrice = (reader["ItemPrice"] != DBNull.Value) ? (reader.GetDouble(9)) : 0;
                tmpProductInfo.SetCriticalLevel = (reader["SetCriticalLevel"] != DBNull.Value) ? (reader.GetBoolean(12)) : false;
                tmpProductInfo.SetDefaultStorageID = (reader["SetDefaultStorageID"] != DBNull.Value) ? (reader.GetInt32(14)) : 0;
                tmpProductInfo.DecimalPlaces = (reader["DecimalPlaces"] != DBNull.Value) ? (reader.GetInt32(17)) : 0;
                tmpProductInfo.IsBarcodeCreated = (reader["IsBarcodeCreated"] != DBNull.Value) ? (reader.GetBoolean(18)) : false;
                tmpProductInfo.ItemPurchPrice = (reader["ItemPurchPrice"] != DBNull.Value) ? (reader.GetDouble(23)) : 0;
                tmpProductInfo.ItemAlias = (reader["ItemAlias"] != DBNull.Value) ? (reader.GetString(30).Trim()) : "";
                tmpProductInfo.UnderGroupID = (reader["UnderGroupID"] != DBNull.Value) ? (reader.GetInt64(32)) : 0;
                tmpProductInfo.UnderSubGroupID = (reader["UnderSubGroupID"] != DBNull.Value) ? (reader.GetInt64(34)) : 0;
                ActualQty.Text = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35).ToString()) : "";
                tmpProductInfo.HSN = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
                tmpProductInfo.GSTRate = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)) : 0;
                tmpProductInfo.StorageID = (reader["StorageID"] != DBNull.Value) ? (reader.GetInt32(38)) : 0;
                tmpProductInfo.TrayID = (reader["TrayID"] != DBNull.Value) ? (reader.GetInt32(39)) : 0;
                tmpProductInfo.CounterID = (reader["CounterID"] != DBNull.Value) ? (reader.GetInt32(40)) : 0;
                //tmpProductInfo.UpdateDate = reader.GetDateTime(44); //reader["UpdateDate"] != DBNull.Value) ? (reader.GetDateTime(44)) : "";  
                ActualWt.Text = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46).ToString()) : "";
                LastBuyDate.Text = (reader["LastBuyDate"] != DBNull.Value) ? (reader.GetDateTime(47).ToString()) : "";
                LastSaleDate.Text = (reader["LastSaleDate"] != DBNull.Value) ? (reader.GetDateTime(48).ToString()) : "";
                LastSalePrice.Text = (reader["LastSalePrice"] != DBNull.Value) ? (reader.GetDouble(50).ToString()) : "";
                LastBuyPrice.Text = (reader["LastBuyPrice"] != DBNull.Value) ? (reader.GetDouble(51).ToString()) : "";

                //HSN.Text = tmpProductInfo.HSN.ToString();
                //txtPrice.Text = tmpProductInfo.ItemPrice.ToString();
                //txtGSTRate.Text = tmpProductInfo.GSTRate.ToString();
                //txtWeight.Text = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)).ToString().Trim() : "";
                //HSN.Text = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
                //txtGSTRate.Text = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)).ToString().Trim() : "";
                //autocompleteItemName.autoTextBox1.Text = tmpProductInfo.ItemBarCode.ToString();
                //Get Counter , Tray and Storage Name by another call, get all count by sp or direct call for inventory 
                //cmbStorage.Text = (reader["StorageName"] != DBNull.Value) ? (reader.GetString(79).Trim()) : "";
                ////CounterName.Text = (reader["CounterName"] != DBNull.Value) ? (reader.GetString(80).Trim()) : "";
                //cmbTray.Text = (reader["TrayName"] != DBNull.Value) ? (reader.GetString(81).Trim()) : "";
                //cmbUnits.Text = tmpProductInfo.UnitID.ToString();
                ////txtMC.Text = (reader["MakingCharge"] != DBNull.Value) ? (reader.GetDouble(94)).ToString().Trim() : "";
                ////txtPrice.Text = (reader["RatePerGm"] != DBNull.Value) ? (reader.GetDouble(95)).ToString().Trim() : "";
                //txtPrice.Text = (reader["ItemPrice"] != DBNull.Value) ? (reader.GetDouble(9)).ToString().Trim() : "";

                //txtQtyStockEntry.Text = tmpProductInfo.ActualQty.ToString();
                ////txtWaste.Text = (reader["WastagePerc"] != DBNull.Value) ? (reader.GetDouble(96)).ToString().Trim() : "";
                //BindStorageComboBox(tmpProductInfo.ItemName);
            }
            //}
            reader.Close();



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
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[^0-9]+");
            //Regex regex = new Regex(@"^\d*\.?\d?$");
            //Regex regex = new Regex(@"[^0-9]\d{0,9}(\.\d{1,3})?%?$");
            //Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]+)?$");

            Regex regex = new Regex("[^0-9.-]+");   // Allow Decimal Only

            e.Handled = regex.IsMatch(e.Text);
        }

        private void TabItem_Selected(object sender, RoutedEventArgs e)
        {
            //SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //conn.Open();

            using (SqlConnection con = new SqlConnection())
               {
               con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
               con.Open();
               SqlCommand com = new SqlCommand("select InvoiceNumber As [Sale Invoice Number], BilledQty As Qty,SalePrice As [Sold Price],Amount,Discount,TaxablelAmount,GSTRate,GSTTax,TotalAmount from SalesVoucherInventory where LTRIM(RTRIM(ItemName))='" + itemnames + "' and CompID = '" + companyId + "'", con);
               SqlDataAdapter sda = new SqlDataAdapter(com);              
               System.Data.DataTable dt = new System.Data.DataTable("Sale History");
               sda.Fill(dt);
               ItemSaleGrid.ItemsSource = dt.DefaultView;
               ItemSaleGrid.AutoGenerateColumns = true;
               ItemSaleGrid.CanUserAddRows = false;
               }


          ////string sql = "select ItemName,HSN,BilledQty,BilledWt,WastePerc,TotalBilledWt,MakingCharge,SalePrice,TotalAmount,Discount,TaxablelAmount,TotalAmount,GSTRate,GSTTax,TotalAmount from SalesVoucherInventory where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text + "' and CompID = '" + CompID + "'";
          //  string sql = "select InvoiceNumber As [Sale Invoice Number], BilledQty As Qty,SalePrice As [Sold Price],TotalAmount,Discount,TaxablelAmount,GSTRate,GSTTax,Amount from SalesVoucherInventory where LTRIM(RTRIM(ItemName))='" + itemnames + "' and CompID = '" + companyId + "'";
          //  SqlCommand cmd = new SqlCommand(sql);
          //  cmd.Connection = conn;
          //  //SqlDataReader reader = cmd.ExecuteReader();
           
          //  using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
          //  {
          //      using (DataTable dt = new DataTable())
          //      {
          //          sda.Fill(dt);
          //          ItemSaleGrid.ItemsSource = dt;
          //      }
          //  }
           
        }

        private void TabItem_Selected_1(object sender, RoutedEventArgs e)
        {
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();
                SqlCommand com = new SqlCommand("select InvoiceNumber As [Buy Invoice Number], BilledQty As Qty,SalePrice As [Buy Price],Amount,Discount,TaxablelAmount,GSTRate,GSTTax,TotalAmount from PurchaseVoucherInventory where LTRIM(RTRIM(ItemName))='" + itemnames + "' and CompID = '" + companyId + "'", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt = new System.Data.DataTable("Buy History");
                sda.Fill(dt);
                ItemBuyGrid.ItemsSource = dt.DefaultView;
                ItemBuyGrid.AutoGenerateColumns = true;
                ItemBuyGrid.CanUserAddRows = false;
            }


        }


    }
}
