using RTSJewelERP.StorageTableAdapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for SaleVoucher.xaml
    /// </summary>
    public partial class AddItem : Window
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;

        public AddItem()
        {
            InitializeComponent();

            textBoxItemname.Focus();

          


            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select number from AutoIncrement where Name = 'SaleInvoice' and CompID = "+CompID+"";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = con;
            SqlDataReader reader = cmd.ExecuteReader();

            //tmpProduct = new Product();

            while (reader.Read())
            {
                //InvoiceNumber = reader.GetInt64(0);


            }
            reader.Close();

            string sqlvoucher = "select number from AutoIncrement where Name = 'SaleVoucher'  and CompID = '" + CompID + "'";
            SqlCommand cmdvoucher = new SqlCommand(sqlvoucher);
            cmdvoucher.Connection = con;
            SqlDataReader readerVoucher = cmdvoucher.ExecuteReader();

            //tmpProduct = new Product();

            while (readerVoucher.Read())
            {
                //voucherNumber = readerVoucher.GetInt64(0);

            }
            readerVoucher.Close();

        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            //var data ;
            //= Model.GetData();

            //If a product code is not empty we search the database
            if (Regex.IsMatch(textBoxItemname.Text.Trim(), @"^\d+$") || 1 == 1)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select ItemName from StockItems where ItemName like '%" + textBoxItemname.Text + "%' and CompID = '" + CompID + "'";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                //tmpProduct = new Product();

                string query = (sender as TextBox).Text;

                if (query.Length == 0)
                {
                    // Clear    
                    resultStack.Children.Clear();
                    border.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    border.Visibility = System.Windows.Visibility.Visible;
                }

                // Clear the list    
                resultStack.Children.Clear();

                while (reader.Read())
                {
                    //var CustID = reader.GetValue(0).ToString();

                    //tmpProduct.ItemName = reader.GetString(0).Trim();
                    //if (tmpProduct.ItemName.ToLower().Contains(query.ToLower()))
                    //{
                    //    // The word starts with this... Autocomplete must work    
                    addItem(reader.GetString(0).Trim().ToString());



                    //    found = true;
                    //}
                    //tmpProduct.PrintName = reader.GetString(3).Trim();
                    //tmpProduct.ItemCode = reader.GetString(5).Trim();
                    //tmpProduct.ItemBarCode = reader.GetString(7).Trim();

                    //tmpProduct.ItemPrice = reader.GetDouble(9);
                    //tmpProduct.ActualQty = reader.GetDouble(35);
                    //tmpProduct.ActualWt = reader.GetDouble(46);

                }
                reader.Close();
            }



            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }

        private void addItem(string text)
        {
            TextBlock block = new TextBlock();

            // Add the text
            block.Text = text;

            // A little style...
            block.Margin = new Thickness(2, 3, 2, 3);
            block.Cursor = Cursors.Hand;

            // Mouse events
            block.MouseLeftButtonUp += (sender, e) =>
            {
                textBoxItemname.Text = (sender as TextBlock).Text;
                textBoxItemname.Focus();
            };

            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.PeachPuff;
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };

            // Add to the panel
            resultStack.Children.Add(block);
            textBoxItemname.Focus();
        }

        void CartGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()).ToString();
            //CartGrid.Items.Refresh();
        }
        private int i = 1;



        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CleanUp();
        }

        //this method will clear/reset form values
        private void CleanUp()
        {
            ////shopping cart = a new empty list
            //ShoppingCart = new List<Product>();
            //OldCart = new List<Product>();
            ////Textboxes and labels are set to defaults
            //TxtProdCode.Text = string.Empty;
            //textBoxItemName.Text = string.Empty;
            //txtQty.Text = string.Empty;
            //lbTotal.Content = "Total: ₹ 0.00";
            //lbOldTotal.Content = "Total: ₹ 0.00";
            //lbGrandTotal.Content = "Total: ₹ 0.00";
            ////DataGrid items are set to null
            //CartGrid.ItemsSource = null;
            //CartGrid.Items.Refresh();
            ////Tmp variable is erased using null
            //tmpProduct = null;

        }

        private TargetType GetParent<TargetType>(DependencyObject o) where TargetType : DependencyObject
        {
            if (o == null || o is TargetType) return (TargetType)o;
            return GetParent<TargetType>(VisualTreeHelper.GetParent(o));
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
        }



        private void textBoxItemname_TextChanged(object sender, TextChangedEventArgs e)
        {

            //If a product code is not empty we search the database
            if (Regex.IsMatch(textBoxItemname.Text.Trim(), @"^\d+$") || 1 == 1)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select * from StockItems where ItemName = '" + textBoxItemname.Text + "'   and CompID = '" + CompID + "'";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                //tmpProduct = new Product();

                while (reader.Read())
                {



                    //HSN.Text = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                    PrintName.Text = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                    UnitID.Text = (reader["UnitID"] != DBNull.Value) ? (reader.GetInt32(4)).ToString().Trim() : "";
                    ItemCode.Text = (reader["ItemCode"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";


                    ItemDesc.Text = (reader["ItemDesc"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                    ItemBarCode.Text = (reader["ItemBarCode"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
                    ItemPrice.Text = (reader["ItemPrice"] != DBNull.Value) ? (reader.GetDouble(9)).ToString().Trim() : "";
                    SetCriticalLevel.Text = (reader["SetCriticalLevel"] != DBNull.Value) ? (reader.GetBoolean(12)).ToString().Trim() : "false";
                    SetDefaultStorageID.Text = (reader["SetDefaultStorageID"] != DBNull.Value) ? (reader.GetInt32(14)).ToString().Trim() : "";
                    DecimalPlaces.Text = (reader["DecimalPlaces"] != DBNull.Value) ? (reader.GetInt32(17)).ToString().Trim() : "";
                    //HSN.Text = (reader["IsBarcodeCreated"] != DBNull.Value) ? (reader.GetBoolean(18)).ToString().Trim() : "false";
                    ItemPurchPrice.Text = (reader["ItemPurchPrice"] != DBNull.Value) ? (reader.GetDouble(23)).ToString().Trim() : "";
                    ItemAlias.Text = (reader["ItemAlias"] != DBNull.Value) ? (reader.GetString(30).Trim()) : "";
                    //get Group Name 
                    UnderGroupName.Text = (reader["UnderGroupID"] != DBNull.Value) ? (reader.GetInt64(32)).ToString().Trim() : "";
                    UnderSubGroupName.Text = (reader["UnderSubGroupID"] != DBNull.Value) ? (reader.GetInt64(34)).ToString().Trim() : "";
                    ActualQty.Text = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)).ToString().Trim() : "";
                    HSN.Text = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
                    GSTRate.Text = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)).ToString().Trim() : "";
                    //Get Name instead ID
                    StorageName.Text = (reader["StorageID"] != DBNull.Value) ? (reader.GetInt32(38)).ToString().Trim() : "";
                    TrayName.Text = (reader["TrayID"] != DBNull.Value) ? (reader.GetInt32(39)).ToString().Trim() : "";
                    CounterName.Text = (reader["CounterID"] != DBNull.Value) ? (reader.GetInt32(40)).ToString().Trim() : "";
                    OpeningStock.Text = (reader["OpeningStock"] != DBNull.Value) ? (reader.GetDouble(41)).ToString().Trim() : "";
                    OpeningStockValue.Text = (reader["OpeningStockValue"] != DBNull.Value) ? (reader.GetDouble(42)).ToString().Trim() : "";
                    //tmpProduct.UpdateDate = reader.GetDateTime(44); //reader["UpdateDate"] != DBNull.Value) ? (reader.GetDateTime(44)) : "";  
                    ActualWt.Text = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)).ToString().Trim() : "";
                    //tmpProduct.LastBuyDate = reader.GetDateTime(47); //(reader["LastBuyDate"] != DBNull.Value) ? (reader.GetDateTime(47) : "";
                    //tmpProduct.LastSaleDate = reader.GetDateTime(48);//(reader["LastSaleDate"] != DBNull.Value) ? (reader.GetDateTime(48) : "";
                    CurrentStockValue.Text = (reader["CurrentStockValue"] != DBNull.Value) ? (reader.GetDouble(49)).ToString().Trim() : "";
                    LastSalePrice.Text = (reader["LastSalePrice"] != DBNull.Value) ? (reader.GetDouble(50)).ToString().Trim() : "";
                    LastBuyPrice.Text = (reader["LastBuyPrice"] != DBNull.Value) ? (reader.GetDouble(51)).ToString().Trim() : "";

                    OpeningStockWt.Text = (reader["OpeningStockWt"] != DBNull.Value) ? (reader.GetDouble(52)).ToString().Trim() : "";

                    //HSN.Text = tmpProduct.HSN.ToString();
                    //txtPrice.Text = tmpProduct.ItemPrice.ToString();
                    //txtGSTRate.Text = tmpProduct.GSTRate.ToString();
                    //TxtProdCode.Text = tmpProduct.ItemBarCode.ToString();
                    //Get Counter , Tray and Storage Name by another call, get all count by sp or direct call for inventory 

                }
                reader.Close();
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {




        }



        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj)
                where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static childItem FindVisualChild<childItem>(DependencyObject obj)
                where childItem : DependencyObject
        {
            foreach (childItem child in FindVisualChildren<childItem>(obj))
            {
                return child;
            }

            return null;
        }


        private void NumberValidationInvoiceTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
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
            if (textBoxItemname != null)
            {
                //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                myConnSalesInvEntryStr.Open();
                string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItems where ItemName ='" + textBoxItemname.Text + "'   and CompID = '" + CompID + "'";
                SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
                myCommand.Connection = myConnSalesInvEntryStr;

                //int countRec = myCommand.ExecuteNonQuery();
                int countRec = (int)myCommand.ExecuteScalar();
                myCommand.Connection.Close();

                double dUnitID = (UnitID.Text.Trim() == "") ? 1 : Convert.ToInt32(UnitID.Text);
                double dItemPrice = (ItemPrice.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemPrice.Text);
                double dItemPurchPrice = (ItemPurchPrice.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemPurchPrice.Text);
                double dItemMRP = (ItemMRP.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemMRP.Text);
                double dItemMinSalePrice = (ItemMinSalePrice.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemMinSalePrice.Text);
                double dSetDefaultStorageID = 1;
                Int32 dDecimalPlaces = (DecimalPlaces.Text.Trim() == "") ? 0 : Convert.ToInt32(DecimalPlaces.Text);
                double dSaleDiscount = (SaleDiscount.Text.Trim() == "") ? 0 : Convert.ToDouble(SaleDiscount.Text);
                double dActualQty = (ActualQty.Text.Trim() == "") ? 0 : Convert.ToDouble(ActualQty.Text);
                double dGSTRate = (GSTRate.Text.Trim() == "") ? 0 : Convert.ToDouble(GSTRate.Text);
                Int32 dStorageID = 1;
                Int32 dTrayID = 1;
                Int32 dCounterID = 1;
                //Int32 dStorageID = (StorageID.Text == "") ? 0 : Convert.ToInt32(StorageID.Text);
                //Int32 dTrayID = (TrayID.Text == "") ? 0 : Convert.ToInt32(TrayID.Text);
                //Int32 dCounterID = (CounterID.Text == "") ? 0 : Convert.ToInt32(CounterID.Text);
                double dOpeningStock = (OpeningStock.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningStock.Text);
                double dOpeningStockValue = (OpeningStockValue.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningStockValue.Text);
                double dActualWt = (ActualWt.Text.Trim() == "") ? 0 : Convert.ToDouble(ActualWt.Text);
                double dCurrentStockValue = (CurrentStockValue.Text.Trim() == "") ? 0 : Convert.ToDouble(CurrentStockValue.Text);
                double dLastSalePrice = (LastSalePrice.Text.Trim() == "") ? 0 : Convert.ToDouble(LastSalePrice.Text);
                double dLastBuyPrice = (LastBuyPrice.Text.Trim() == "") ? 0 : Convert.ToDouble(LastBuyPrice.Text);
                double dOpeningStockWt = (OpeningStockWt.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningStockWt.Text);
                //double ItemPrice = (ItemPurchPrice.Text == "") ? 0 : Convert.ToDouble(ItemPurchPrice.Text);
                //double ItemPrice = (ItemPurchPrice.Text == "") ? 0 : Convert.ToDouble(ItemPurchPrice.Text);

                //SetCriticalLevel,SetImageUrl,SetDefaultStorageID,SetDefaultSundryCreditor,SetDefaultSundryDebtor,DecimalPlaces,SaleDiscount,ItemPurchPrice,ItemAlias,UnderGroupName,UnderSubGroupName,
                //ActualQty,HSN,GSTRate,StorageID,TrayID,CounterID,OpeningStock,OpeningStockValue,ActualWt,CurrentStockValue,LastSalePrice,LastBuyPrice,OpeningStockWt) Values ( '" + textBoxItemname.Text + "','" + PrintName.Text + "','" + Convert.ToInt32(UnitID.Text) + "','" + ItemCode.Text + "','" + ItemDesc.Text + "','" + ItemBarCode.Text + "','" + ItemMRP.Text + "','" + ItemPrice.Text + "','" + ItemMinSalePrice.Text + "','" + Convert.ToBoolean(SetCriticalLevel.Text) + "','" + SetImageUrl.Text + "','" + Convert.ToInt32(SetDefaultStorageID.Text) + "','" + Convert.ToInt32(SetDefaultSundryCreditor.Text) + "','" + Convert.ToInt32(SetDefaultSundryDebtor.Text) + "','" + Convert.ToInt32(DecimalPlaces.Text) + "','" + Convert.ToDouble(SaleDiscount.Text) + "','" + Convert.ToDouble(ItemPurchPrice.Text) + "','" + ItemAlias.Text + "','" + UnderGroupName.Text + "','" + UnderSubGroupName.Text + "','" + Convert.ToDouble(ActualQty.Text) + "','" + HSN.Text + "','" + Convert.ToDouble(GSTRate.Text) + "','" + Convert.ToInt32(StorageName.Text) + "','" + Convert.ToInt32(TrayName.Text) + "','" + Convert.ToInt32(CounterName.Text) + "','" + Convert.ToDouble(OpeningStock.Text) + "','" + Convert.ToDouble(OpeningStockValue.Text) + "','" + Convert.ToDouble(ActualWt.Text) + "','" + Convert.ToDouble(CurrentStockValue.Text) + "','" + Convert.ToDouble(LastSalePrice.Text) + "','" + Convert.ToDouble(LastBuyPrice.Text) + "','" + Convert.ToDouble(OpeningStockWt.Text) + "')";
                if (countRec != 0)
                {

                    string queryStrStockUpdate = "";
                    queryStrStockUpdate = "update StockItems  set  ItemName='" + textBoxItemname.Text + "',PrintName='" + PrintName.Text + "',UnitID='" + dUnitID + "' ,ItemDesc='" + ItemDesc.Text + "' ,ItemBarCode='" + ItemBarCode.Text + "' ,ItemMRP='" + dItemMRP + "' ,ItemPrice='" + dItemPrice + "' ,ItemMinSalePrice='" + dItemMinSalePrice + "' ,SetCriticalLevel='" + SetCriticalLevel.Text + "' ,SetImageUrl='" + SetImageUrl.Text + "' ,SetDefaultStorageID='" + dSetDefaultStorageID + "' ,SetDefaultSundryCreditor='" + dSetDefaultStorageID + "' ,DecimalPlaces='" + dDecimalPlaces + "' ,SaleDiscount='" + dSaleDiscount + "' ,ItemPurchPrice='" + dItemPurchPrice + "' ,ItemAlias='" + ItemAlias.Text + "' ,UnderGroupName='" + UnderGroupName.Text + "' ,UnderSubGroupName='" + UnderSubGroupName.Text + "' ,ActualQty='" + dActualQty + "' ,HSN='" + HSN.Text + "' ,GSTRate='" + dGSTRate + "' ,OpeningStock='" + dOpeningStock + "' ,OpeningStockValue='" + dOpeningStockValue + "' ,ActualWt='" + dActualWt + "' ,CurrentStockValue='" + dCurrentStockValue + "' ,LastSalePrice='" + dLastSalePrice + "' ,LastBuyPrice='" + dLastBuyPrice + "' ,OpeningStockWt='" + dOpeningStockWt + "' where ItemName ='" + textBoxItemname.Text + "'   and CompID = '" + CompID + "' ";

                    SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                    myCommandStkUpdate.Connection.Open();
                    myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                    if (textBoxItemname.Text.Trim() != "")
                    {
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
                    }
                    else
                    {
                        MessageBox.Show("Stock can not be updated....", "Update Record Error");
                    }
                    myCommandStkUpdate.Connection.Close();
                }
                else
                {


                    string querySalesInvEntry = "";
                    //querySalesInvEntry = "insert into StockItems(ItemName, PrintName,ItemDesc,ItemPrice,ItemPurchPrice,UnderGroupName,UnderSubGroupName,ActualQty,HSN,GSTRate,OpeningStock,OpeningStockValue,ActualWt,CurrentStockValue,OpeningStockWt)   Values ( '" + textBoxItemname.Text + "','" + ItemDesc.Text + "','" + ItemBarCode.Text + "','" + ItemPrice.Text + "','" + Convert.ToDouble(ItemPurchPrice.Text) + "','" + UnderGroupName.Text + "','" + UnderSubGroupName.Text + "','" + Convert.ToDouble(ActualQty.Text) + "','" + HSN.Text + "','" + Convert.ToDouble(GSTRate.Text) + "','" + Convert.ToDouble(OpeningStock.Text) + "','" + Convert.ToDouble(OpeningStockValue.Text) + "','" + Convert.ToDouble(ActualWt.Text) + "','" + Convert.ToDouble(CurrentStockValue.Text) + "','" + Convert.ToDouble(OpeningStockWt.Text) + "')";
                    querySalesInvEntry = "insert into StockItems(ItemName, PrintName,UnitID,ItemCode,ItemDesc,ItemBarCode,ItemMRP,ItemPrice,ItemMinSalePrice,SetCriticalLevel,SetImageUrl,SetDefaultStorageID,SetDefaultSundryCreditor,SetDefaultSundryDebtor,DecimalPlaces,SaleDiscount,ItemPurchPrice,ItemAlias,UnderGroupName,UnderSubGroupName,ActualQty,HSN,GSTRate,StorageID,TrayID,CounterID,OpeningStock,OpeningStockValue,ActualWt,CurrentStockValue,LastSalePrice,LastBuyPrice,OpeningStockWt,CompID)  Values ( '" + textBoxItemname.Text + "','" + PrintName.Text + "','" + dUnitID + "','" + ItemCode.Text + "','" + ItemDesc.Text + "','" + ItemBarCode.Text + "','" + dItemMRP + "','" + dItemPrice + "','" + dItemMinSalePrice + "','" + Convert.ToBoolean(SetCriticalLevel.Text) + "','" + SetImageUrl.Text + "','" + '1' + "','" + SetDefaultSundryCreditor.Text + "','" + SetDefaultSundryDebtor.Text + "','" + dDecimalPlaces + "','" + dSaleDiscount + "','" + dItemPurchPrice + "','" + ItemAlias.Text + "','" + UnderGroupName.Text + "','" + UnderSubGroupName.Text + "','" + dActualQty + "','" + HSN.Text + "','" + dGSTRate + "','" + StorageName.Text + "','" + TrayName.Text + "','" + CounterName.Text + "','" + dOpeningStock + "','" + dOpeningStockValue + "','" + dActualWt + "','" + dCurrentStockValue + "','" + dLastSalePrice + "','" + dLastBuyPrice + "','" + dOpeningStockWt + "','"+CompID+"')";
                    SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                    myCommandInvEntry.Connection.Open();
                    int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                    if (NumPInv != 0)
                    {
                        // MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                    }
                    else
                    {
                        MessageBox.Show("Stock is not Inserted....", "Insert Record Error");
                    }
                    myCommandInvEntry.Connection.Close();

                    // myConnStock.Close();

                }


            }

        }

        //private void resultStack_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    txtQty.Focus();
        //}




    }
}
