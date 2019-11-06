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
    /// Interaction logic for AddSundryCreditor.xaml
    /// </summary>
    public partial class AddSundryCreditor : Window
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public AddSundryCreditor()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window

            autocompltCustName.autoTextBoxCustNameBarcode.Text = "";
            BindComboBox(State);
            BindUnderComboBox(MainAccounts);
            autocompltCustName.autoTextBoxCustNameBarcode.Focus();

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select number from AutoIncrement where Name = 'SaleInvoice' and CompID = '" + CompID + "'";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = con;
            SqlDataReader reader = cmd.ExecuteReader();

            //tmpProduct = new Product();

            while (reader.Read())
            {
                //InvoiceNumber = reader.GetInt64(0);


            }
            reader.Close();

            string sqlvoucher = "select number from AutoIncrement where Name = 'SaleVoucher' and CompID = '" + CompID + "'";
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
        public void BindComboBox(ComboBox state)
        {
            var custAdpt = new StateTableAdapter();
            var custInfoVal = custAdpt.GetData();
            var LinqRes = (from UserRec in custInfoVal
                           orderby UserRec.StateName ascending
                           select (UserRec.StateName + "-" + UserRec.StateCode)).Distinct();
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
            PrintName.Clear();
            TransportName.Clear();
            TransportOtherDetails.Clear();
            autocompltCustName.autoTextBoxCustNameBarcode.Text = "";
            Alias.Clear();
            GSTIN.Clear();
            Address1.Clear();
            ShipAddress1.Clear();
            ShipAddress1.Clear();
            //LastBuyDate.SelectedDate = DateTime.Now;
            Address2.Clear();
            PinCode.Clear();
            City.Clear();
            State.Text = "";
            Phone.Clear();
            // OpeningStock.Clear();
            //ActualQty.Clear();
            Mob.Clear();
            //  OpeningStockValue.Clear();
            // ActualWt.Clear();
            Email.Clear();
            //  OpeningStockWt.Clear();
            Web.Clear();
            discountRate.Clear();
            OpeningBalanceCr.Clear();
            OpeningBalanceDr.Clear();
            autocompltCustName.autoTextBoxCustNameBarcode.Focus();

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
            //Regex regex = new Regex("[^0-9]+");
            //Regex regex = new Regex(@"^\d*\.?\d?$");
            //Regex regex = new Regex(@"[^0-9]\d{0,9}(\.\d{1,3})?%?$");
            //Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]+)?$");

            Regex regex = new Regex("[^0-9.-]+");   // Allow Decimal Only

            e.Handled = regex.IsMatch(e.Text);
        }


        private void autocompltCustName_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintName.Clear();
                TransportName.Clear();
                TransportOtherDetails.Clear();

                //If a product code is not empty we search the database
                if (Regex.IsMatch(autocompltCustName.autoTextBoxCustNameBarcode.Text.Trim(), @"^\d+$") || 1 == 1)
                {
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                    con.Open();
                    string sql = "select * from AccountsList where AcctName = '" + autocompltCustName.autoTextBoxCustNameBarcode.Text.Trim() + "' and CompID = '" + CompID + "'";
                    SqlCommand cmd = new SqlCommand(sql);
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();

                    //tmpProduct = new Product();

                    while (reader.Read())
                    {
                        var acctID = (reader["AcctID"] != DBNull.Value) ? (reader.GetInt64(0)).ToString().Trim() : "";
                        autocompltCustName.autoTextBoxCustNameBarcode.Text = (reader["AcctName"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "";
                        var PrimaryAcctID = (reader["PrimaryAcctID"] != DBNull.Value) ? (reader.GetInt32(2)).ToString().Trim() : "";
                        MainAccounts.Text = ((reader["PrimaryAcctName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "") + "-" + ((reader["PrimaryAcctID"] != DBNull.Value) ? (reader.GetInt32(2)).ToString().Trim() : "");
                        Alias.Text = (reader["Alias"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";
                        Address1.Text = (reader["Address1"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                        Address2.Text = (reader["Address2"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
                        ShipAddress1.Text = (reader["ShippingAddr1"] != DBNull.Value) ? (reader.GetString(26).Trim()) : "";
                        ShipAddress2.Text = (reader["ShippingAddr2"] != DBNull.Value) ? (reader.GetString(27).Trim()) : "";
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
                        discountRate.Text = (reader["Discount"] != DBNull.Value) ? (reader.GetString(28)) : "";
                        PrintName.Text = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(29)) : "";

                        if (PrintName.Text.Trim() == "")
                        {
                            PrintName.Text = autocompltCustName.autoTextBoxCustNameBarcode.Text.Trim();
                        }

                        //invAffectedYes.Checked
                        lblCR.Content = "CR :" + OpeningBalanceCr.Text;
                        lblDR.Content = "DR :" + OpeningBalanceDr.Text;
                        lblBal.Content = "Difference : " + (((reader["OpBalanceCR"] != DBNull.Value) ? (reader.GetDouble(19)) : 0) - ((reader["OpBalanceDR"] != DBNull.Value) ? (reader.GetDouble(18)) : 0)).ToString().Trim();
                        //OpeningStockWt.Text = (reader["OpeningStockWt"] != DBNull.Value) ? (reader.GetDouble(52)).ToString().Trim() : "";

                        //var CustID = reader.GetValue(0).ToString();




                    }
                    reader.Close();

                    if (PrintName.Text.Trim() == "")
                    {
                        PrintName.Text = autocompltCustName.autoTextBoxCustNameBarcode.Text.Trim();
                    }

                    SqlConnection contransport = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                    contransport.Open();
                    string sqltransport = "select TransportName, Address from TransportDetails where AcctName = '" + autocompltCustName.autoTextBoxCustNameBarcode.Text.Trim() + "'";
                    SqlCommand cmdtransport = new SqlCommand(sqltransport);
                    cmdtransport.Connection = contransport;
                    SqlDataReader readertransport = cmdtransport.ExecuteReader();

                    //tmpProduct = new Product();

                    while (readertransport.Read())
                    {
                        TransportName.Text = (readertransport["TransportName"] != DBNull.Value) ? (readertransport.GetString(0).Trim()) : "";
                        TransportOtherDetails.Text = (readertransport["Address"] != DBNull.Value) ? (readertransport.GetString(1).Trim()) : "";


                    }
                    readertransport.Close();


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please Do not use Special Symbols, Please Validate Data");
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

        private void CombopboxHighlight_LostFocus(object sender, RoutedEventArgs e)
        {
            var combobox = e.OriginalSource as ComboBox;
            if (combobox != null)
            {
                combobox.Background = Brushes.White;
                combobox.Foreground = Brushes.Black;
            }
        }

        private void CombopboxHighlight_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as ComboBox;
            if (textBox != null)
            {
                //textBox.Background = Brushes.Blue;
                //textBox.Foreground = Brushes.Black;
            }
        }

        private void NumberValidationInvoiceTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
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
            if (autocompltCustName.autoTextBoxCustNameBarcode != null)
            {
                //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                myConnSalesInvEntryStr.Open();
                string CountStockItemsEntryStr = "SELECT COUNT(*) From AccountsList where AcctName ='" + autocompltCustName.autoTextBoxCustNameBarcode.Text.Trim() + "' and CompID = '" + CompID + "'";
                SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
                myCommand.Connection = myConnSalesInvEntryStr;

                //int countRec = myCommand.ExecuteNonQuery();
                int countRec = (int)myCommand.ExecuteScalar();
                myCommand.Connection.Close();


                double dOpeningBalanceCr = (OpeningBalanceCr.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningBalanceCr.Text);
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

                SqlConnection conTrans = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                conTrans.Open();
                //string sql = "SELECT COUNT(*) From AccountsList where AcctName='" + textBoxAcctName.Text.Trim() + "'";
                SqlCommand cmdTrans;//= new SqlCommand(sql, con);

                cmdTrans = new SqlCommand("SPUpdateAccountsForTransportDetails", conTrans);
                cmdTrans.CommandType = CommandType.StoredProcedure;
                cmdTrans.Parameters.Add(new SqlParameter("@TransportName", TransportName.Text.Trim()));
                cmdTrans.Parameters.Add(new SqlParameter("@OtherDetails", TransportOtherDetails.Text.Trim()));
                cmdTrans.Parameters.Add(new SqlParameter("@AccountName", autocompltCustName.autoTextBoxCustNameBarcode.Text.Trim()));
                int countRecPayTrans = cmdTrans.ExecuteNonQuery();



                if (countRec != 0)
                {

                    string queryStrStockUpdate = "";
                    queryStrStockUpdate = "update AccountsList  set  AcctName='" + autocompltCustName.autoTextBoxCustNameBarcode.Text.Trim() + "',PrimaryAcctID='" + dPrimaryAcctId + "',PrimaryAcctName='" + dPrimaryAcctName + "',Alias='" + Alias.Text + "' ,Address1='" + Address1.Text + "' ,Address2='" + Address2.Text + "',ShippingAddr1='" + ShipAddress1.Text + "' ,ShippingAddr2='" + ShipAddress2.Text + "' ,City='" + City.Text + "' ,State='" + State.Text + "' ,Mobile1='" + Mob.Text + "' ,Phone='" + Phone.Text + "' ,GSTIN='" + GSTIN.Text + "' ,Email='" + Email.Text + "' ,Website='" + Web.Text + "' ,OpBalanceDR='" + dOpeningBalanceDr + "' ,OpBalanceCR='" + dOpeningBalanceCr + "' ,PINCode='" + PinCode.Text + "' , Discount='" + discountRate.Text + "', PrintName='" + PrintName.Text + "'  where AcctName='" + autocompltCustName.autoTextBoxCustNameBarcode.Text + "' and CompID = '" + CompID + "' ";
                    SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                    myCommandStkUpdate.Connection.Open();
                    myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                    if (autocompltCustName.autoTextBoxCustNameBarcode.Text.Trim() != "")
                    {
                        // myCommandStk.Connection.Open();
                        int Num = myCommandStkUpdate.ExecuteNonQuery();
                        if (Num != 0)
                        {
                            MessageBox.Show("Record Successfully Updated....", "Update Record");
                            CleanUp();
                            TransportName.Clear();
                            TransportOtherDetails.Clear();
                            this.Close();

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
                    querySalesInvEntry = "insert into AccountsList(AcctName, PrimaryAcctID,PrimaryAcctName,Alias,Address1,Address2,City,State,Mobile1,Phone,GSTIN,Email,Website,OpBalanceDR,OpBalanceCR,PINCode,CompID,ShippingAddr1,ShippingAddr2,Discount,PrintName)  Values ( '" + autocompltCustName.autoTextBoxCustNameBarcode.Text.Trim() + "','" + dPrimaryAcctId + "','" + dPrimaryAcctName + "','" + Alias.Text + "','" + Address1.Text + "','" + Address2.Text + "','" + City.Text + "','" + State.Text + "','" + Mob.Text + "','" + Phone.Text + "','" + GSTIN.Text + "','" + Email.Text + "','" + Web.Text + "','" + dOpeningBalanceDr + "','" + dOpeningBalanceCr + "','" + PinCode.Text + "','" + CompID + "','" + ShipAddress1.Text + "','" + ShipAddress2.Text + "','" + discountRate.Text + "','" + PrintName.Text + "')";
                    SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                    myCommandInvEntry.Connection.Open();
                    int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                    if (NumPInv != 0)
                    {
                        MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                        autocompltCustName.autoTextBoxCustNameBarcode.Clear();
                        CleanUp();
                        TransportName.Clear();
                        TransportOtherDetails.Clear();
                        this.Close();
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
