using RTSJewelERP.MainAccountsTableAdapters;
using RTSJewelERP.StateTableAdapters;
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
    public partial class AccountMaster : Page
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public AccountMaster()
        {
            InitializeComponent();
            BindComboBox(State);
            BindUnderComboBox(MainAccounts);
            textBoxAcctName.Focus();




            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select number from AutoIncrement where Name = 'SaleInvoice and CompID = 0'   and CompID = '" + CompID + "'";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = con;
            SqlDataReader reader = cmd.ExecuteReader();

            //tmpProduct = new Product();

            while (reader.Read())
            {
                //InvoiceNumber = reader.GetInt64(0);


            }
            reader.Close();

            string sqlvoucher = "select number from AutoIncrement where Name = 'SaleVoucher'   and CompID = '" + CompID + "'";
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

        public void BindComboBox(ComboBox state)
        {
            var custAdpt = new StateTableAdapter();
            var custInfoVal = custAdpt.GetData();
            var LinqRes = (from UserRec in custInfoVal
                           orderby UserRec.StateName ascending
                           select (UserRec.StateName + "-"+ UserRec.StateCode)).Distinct();
            State.ItemsSource = LinqRes;
            // comboBoxName.SelectedValueBinding = new Binding("Col6");
        }

        public void BindUnderComboBox(ComboBox under)
        {
            var custAdpt = new MainAccountsTypeTableAdapter();
            var custInfoVal = custAdpt.GetData();
            var LinqRes = (from UserRec in custInfoVal
                           orderby UserRec.AcctName ascending
                           select (UserRec.AcctName.Trim() + "-" + UserRec.AcctID)).Distinct();
            MainAccounts.ItemsSource = LinqRes;
            // comboBoxName.SelectedValueBinding = new Binding("Col6");
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
            if (Regex.IsMatch(textBoxAcctName.Text.Trim(), @"^\d+$") || 1 == 1)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select AcctName from AccountsList where AcctName like '%" + textBoxAcctName.Text + "%'  and CompID = '" + CompID + "'";
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
                textBoxAcctName.Text = (sender as TextBlock).Text;
                textBoxAcctName.Focus();
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
            textBoxAcctName.Focus();
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



        private void textBoxAcctName_TextChanged(object sender, TextChangedEventArgs e)
        {

            //If a product code is not empty we search the database
            if (Regex.IsMatch(textBoxAcctName.Text.Trim(), @"^\d+$") || 1 == 1)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select * from AccountsList where AcctName = '" + textBoxAcctName.Text + "'  and CompID = '" + CompID + "'";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                //tmpProduct = new Product();

                while (reader.Read())
                {
                    var acctID = (reader["AcctID"] != DBNull.Value) ? (reader.GetInt64(0)).ToString().Trim() : "";
                    textBoxAcctName.Text = (reader["AcctName"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "";
                    var PrimaryAcctID = (reader["PrimaryAcctID"] != DBNull.Value) ? (reader.GetInt32(2)).ToString().Trim() : "";
                    MainAccounts.Text = ((reader["PrimaryAcctName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "") + "-" +((reader["PrimaryAcctID"] != DBNull.Value) ? (reader.GetInt32(2)).ToString().Trim() : "");
                    Alias.Text = (reader["Alias"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";
                    Address1.Text = (reader["Address1"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                    Address2.Text = (reader["Address2"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
                    City.Text = (reader["City"] != DBNull.Value) ? (reader.GetString(8).Trim()) : "";
                    State.Text = (reader["State"] != DBNull.Value) ? (reader.GetString(9).Trim()) : "";
                    PinCode.Text = (reader["PINCode"] != DBNull.Value) ? (reader.GetString(10).Trim()) : "";
                    Mob.Text = (reader["Mobile1"] != DBNull.Value) ? (reader.GetString(11).Trim()) : "";
                    Phone.Text = (reader["Phone"] != DBNull.Value) ? (reader.GetString(13).Trim()) : "";
                    GSTIN.Text = (reader["GSTIN"] != DBNull.Value) ? (reader.GetString(14).Trim()) : "";
                    Email.Text = (reader["Email"] != DBNull.Value) ? (reader.GetString(15).Trim()) : "";
                    Web.Text = (reader["Website"] != DBNull.Value) ? (reader.GetString(17).Trim()) : "";
                    OpeningBalanceDr.Text = (reader["OpBalanceDR"] != DBNull.Value) ? (reader.GetDouble(18)).ToString() : "";
                    OpeningBalanceCr.Text = (reader["OpBalanceCR"] != DBNull.Value) ? (reader.GetDouble(19)).ToString() : "";
                     //var ischeckinv = (reader["IsInventoryAffected"] != DBNull.Value) ? (reader.GetString(24).Trim()) : "";
                   //invAffectedYes.Checked
                    lblCR.Content = "CR :" +OpeningBalanceCr.Text;
                    lblDR.Content = "DR :" +OpeningBalanceDr.Text;
                    lblBal.Content = "Difference : " + (((reader["OpBalanceCR"] != DBNull.Value) ? (reader.GetDouble(19)) : 0) - ((reader["OpBalanceDR"] != DBNull.Value) ? (reader.GetDouble(18)) : 0)).ToString().Trim();
                    //OpeningStockWt.Text = (reader["OpeningStockWt"] != DBNull.Value) ? (reader.GetDouble(52)).ToString().Trim() : "";

                    //var CustID = reader.GetValue(0).ToString();




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


            string FinYrStartdate = FinYeraStartDate.SelectedDate.ToString();

            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dtin = Convert.ToDateTime(FinYrStartdate);
            //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            int yearsin = dtin.Year;
            string monthsin = dtin.Month.ToString();
            if (dtin.Month < 10)
            {
                monthsin = "0" + monthsin;
            }
            string daysin = dtin.Day.ToString();
            if (dtin.Day < 10)
            {
                daysin = "0" + daysin;
            }

            string FinYrStartdateVal = yearsin + "/" + monthsin + "/" + daysin;


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
            if (textBoxAcctName != null)
            {
                //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                myConnSalesInvEntryStr.Open();
                string CountStockItemsEntryStr = "SELECT COUNT(*) From AccountsList where AcctName ='" + textBoxAcctName.Text + "'  and CompID = '" + CompID + "'";
                SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
                myCommand.Connection = myConnSalesInvEntryStr;

                //int countRec = myCommand.ExecuteNonQuery();
                int countRec = (int)myCommand.ExecuteScalar();
                myCommand.Connection.Close();


                double dOpeningBalanceCr = (OpeningBalanceCr.Text.Trim() == "") ? 1 : Convert.ToDouble(OpeningBalanceCr.Text);
                double dOpeningBalanceDr = (OpeningBalanceDr.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningBalanceDr.Text);
                string dMainAccounts = (MainAccounts.Text.Trim() == "") ? "" : Convert.ToString(MainAccounts.Text).Trim();
                string dPrimaryAcctName = dMainAccounts.Split('-')[0].Trim();
                string dPrimaryAcctId = dMainAccounts.Split('-')[1].Trim();
                //double dItemMRP = (ItemMRP.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemMRP.Text);
                //double dItemMinSalePrice = (ItemMinSalePrice.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemMinSalePrice.Text);
                //double dSetDefaultStorageID = 1;
                //Int32 dDecimalPlaces = (DecimalPlaces.Text.Trim() == "") ? 0 : Convert.ToInt32(DecimalPlaces.Text);
                //double dSaleDiscount = (SaleDiscount.Text.Trim() == "") ? 0 : Convert.ToDouble(SaleDiscount.Text);
                //double dActualQty = (ActualQty.Text.Trim() == "") ? 0 : Convert.ToDouble(ActualQty.Text);
                //double dGSTRate = (GSTRate.Text.Trim() == "") ? 0 : Convert.ToDouble(GSTRate.Text);
                //Int32 dStorageID = 1;
                //Int32 dTrayID = 1;
                //Int32 dCounterID = 1;
                ////Int32 dStorageID = (StorageID.Text == "") ? 0 : Convert.ToInt32(StorageID.Text);
                ////Int32 dTrayID = (TrayID.Text == "") ? 0 : Convert.ToInt32(TrayID.Text);
                ////Int32 dCounterID = (CounterID.Text == "") ? 0 : Convert.ToInt32(CounterID.Text);
                //double dOpeningStock = (OpeningStock.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningStock.Text);
                //double dOpeningStockValue = (OpeningStockValue.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningStockValue.Text);
                //double dActualWt = (ActualWt.Text.Trim() == "") ? 0 : Convert.ToDouble(ActualWt.Text);
                //double dCurrentStockValue = (CurrentStockValue.Text.Trim() == "") ? 0 : Convert.ToDouble(CurrentStockValue.Text);
                //double dLastSalePrice = (LastSalePrice.Text.Trim() == "") ? 0 : Convert.ToDouble(LastSalePrice.Text);
                //double dLastBuyPrice = (LastBuyPrice.Text.Trim() == "") ? 0 : Convert.ToDouble(LastBuyPrice.Text);
                //double dOpeningStockWt = (OpeningStockWt.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningStockWt.Text);
                ////double ItemPrice = (ItemPurchPrice.Text == "") ? 0 : Convert.ToDouble(ItemPurchPrice.Text);
                ////double ItemPrice = (ItemPurchPrice.Text == "") ? 0 : Convert.ToDouble(ItemPurchPrice.Text);



                if (countRec != 0)
                {

                    string queryStrStockUpdate = "";
                    queryStrStockUpdate = "update AccountsList  set  AcctName='" + textBoxAcctName.Text + "',PrimaryAcctID='" + dPrimaryAcctId + "',PrimaryAcctName='" + dPrimaryAcctName + "',Alias='" + Alias.Text + "' ,Address1='" + Address1.Text + "' ,Address2='" + Address2.Text + "' ,City='" + City.Text + "' ,State='" + State.Text + "' ,Mobile1='" + Mob.Text + "' ,Phone='" + Phone.Text + "' ,GSTIN='" + GSTIN.Text + "' ,Email='" + Email.Text + "' ,Website='" + Web.Text + "' ,OpBalanceDR='" + dOpeningBalanceDr + "' ,OpBalanceCR='" + dOpeningBalanceCr + "' ,PINCode='" + PinCode.Text + "' where AcctName='" + textBoxAcctName.Text + "'  and CompID = '" + CompID + "' ";
                    SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                    myCommandStkUpdate.Connection.Open();
                    myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                    if (textBoxAcctName.Text.Trim() != "")
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
                    querySalesInvEntry = "insert into AccountsList(AcctName, PrimaryAcctID,PrimaryAcctName,Alias,Address1,Address2,City,State,Mobile1,Phone,GSTIN,Email,Website,OpBalanceDR,OpBalanceCR,PINCode,CompID)  Values ( '" + textBoxAcctName.Text + "','" + dPrimaryAcctId + "','" + dPrimaryAcctName + "','" + Alias.Text + "','" + Address1.Text + "','" + Address2.Text + "','" + City.Text + "','" + State.Text + "','" + Mob.Text + "','" + Phone.Text + "','" + GSTIN.Text + "','" + Email.Text + "','" + Web.Text + "','" + dOpeningBalanceDr + "','" + dOpeningBalanceCr + "','" + PinCode.Text + "', '" + CompID + "')";
                    SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                    myCommandInvEntry.Connection.Open();
                    int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                    if (NumPInv != 0)
                    {
                        MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                        textBoxAcctName.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Account is not Inserted....", "Insert Record Error");
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
