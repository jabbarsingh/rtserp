using RTSJewelERP.MainAccountsTableAdapters;
using RTSJewelERP.StateTableAdapters;
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
    /// Interaction logic for AddInstantAccount.xaml
    /// </summary>
    public partial class AddAgent : Window
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public AddAgent()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window

            BindComboBox(State);
            BindUnderComboBox(MainAccounts);
            //PrintName.Text = custNameEntered;
            PrintName.Focus();
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
                //this.NavigationService.RemoveBackEntry();
                //}
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

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            //Regex regex = new Regex("[^0-9]+");
            //Regex regex = new Regex(@"^\d*\.?\d?$");
            //Regex regex = new Regex(@"[^0-9]\d{0,9}(\.\d{1,3})?%?$");
            //Regex regex = new Regex(@"^[0-9]*(?:\.[0-9]+)?$");

            Regex regex = new Regex("[^0-9.-]+");   // Allow Decimal Only

            e.Handled = regex.IsMatch(e.Text);
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
            CommissionPerc.Clear();
            PrintName.Clear();
            GSTIN.Clear();
            Address1.Clear();
            Address2.Clear();
            City.Clear();
            State.Text = "";
            Mob.Clear();


        }

        private void AddItemRow_GotFocus(object sender, RoutedEventArgs e)
        {
            var btn = e.OriginalSource as Button;
            btn.Background = Brushes.BlueViolet;
            btn.Foreground = Brushes.White;
        }

        private void AddItemRow_LostFocus(object sender, RoutedEventArgs e)
        {
            var btn = e.OriginalSource as Button;
            btn.Background = Brushes.White;
            btn.Foreground = Brushes.Black;
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


        private void autocompltCustName_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintName.Clear();

                //If a product code is not empty we search the database
                if (Regex.IsMatch(PrintName.Text.Trim(), @"^\d+$") || 1 == 1)
                {
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                    con.Open();
                    string sql = "select * from AgentsList where AcctName = '" + PrintName.Text.Trim() + "' and CompID = '" + CompID + "'";
                    SqlCommand cmd = new SqlCommand(sql);
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();

                    //tmpProduct = new Product();

                    while (reader.Read())
                    {
                        var acctID = (reader["AcctID"] != DBNull.Value) ? (reader.GetInt64(0)).ToString().Trim() : "";
                        PrintName.Text = (reader["AcctName"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "";
                        var PrimaryAcctID = (reader["PrimaryAcctID"] != DBNull.Value) ? (reader.GetInt32(2)).ToString().Trim() : "";
                        MainAccounts.Text = ((reader["PrimaryAcctName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "") + "-" + ((reader["PrimaryAcctID"] != DBNull.Value) ? (reader.GetInt32(2)).ToString().Trim() : "");

                        Address1.Text = (reader["Address1"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                        Address2.Text = (reader["Address2"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";

                        City.Text = (reader["City"] != DBNull.Value) ? (reader.GetString(8).Trim()) : "";
                        State.Text = (reader["State"] != DBNull.Value) ? (reader.GetString(9).Trim()) : "";

                        Mob.Text = (reader["Mobile1"] != DBNull.Value) ? (reader.GetString(11).Trim()) : "";

                        GSTIN.Text = (reader["GSTIN"] != DBNull.Value) ? (reader.GetString(14).Trim()) : "";

                        CommissionPerc.Text = (reader["CommissionPerc"] != DBNull.Value) ? (reader.GetString(30).Trim()) : "";

                        PrintName.Text = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(29)) : "";

                        if (PrintName.Text.Trim() == "")
                        {
                            PrintName.Text = PrintName.Text.Trim();
                        }





                    }
                    reader.Close();

                    if (PrintName.Text.Trim() == "")
                    {
                        PrintName.Text = PrintName.Text.Trim();
                    }


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





            //StockItems: CRUD Start
            if (PrintName != null)
            {

                //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                myConnSalesInvEntryStr.Open();
                string CountStockItemsEntryStr = "SELECT COUNT(*) From AgentsList where AcctName ='" + PrintName.Text.Trim() + "' and CompID = '" + CompID + "'";
                SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
                myCommand.Connection = myConnSalesInvEntryStr;

                //int countRec = myCommand.ExecuteNonQuery();
                int countRec = (int)myCommand.ExecuteScalar();
                myCommand.Connection.Close();


                //double dOpeningBalanceCr = (OpeningBalanceCr.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningBalanceCr.Text);
                //double dOpeningBalanceDr = (OpeningBalanceDr.Text.Trim() == "") ? 0 : Convert.ToDouble(OpeningBalanceDr.Text);
                string dMainAccounts = (MainAccounts.Text.Trim() == "") ? "" : Convert.ToString(MainAccounts.Text).Trim();
                string dPrimaryAcctName = dMainAccounts.Split('-')[0].Trim();
                string dPrimaryAcctId = dMainAccounts.Split('-')[1].Trim();




                if (countRec != 0)
                {

                    string queryStrStockUpdate = "";
                    queryStrStockUpdate = "update AgentsList  set  AcctName='" + PrintName.Text.Trim() + "',PrimaryAcctID='" + dPrimaryAcctId + "',PrimaryAcctName='" + dPrimaryAcctName + "',Alias='' ,Address1='" + Address1.Text + "' ,Address2='" + Address2.Text + "',ShippingAddr1='' ,ShippingAddr2='' ,City='" + City.Text + "' ,State='" + State.Text + "' ,Mobile1='" + Mob.Text + "' ,Phone='' ,GSTIN='" + GSTIN.Text + "' ,Email='' ,Website='' , PrintName='" + PrintName.Text + "',CommissionPerc='" + CommissionPerc.Text + "' where AcctName='" + PrintName.Text + "' and CompID = '" + CompID + "' ";
                    SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                    myCommandStkUpdate.Connection.Open();
                    myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                    if (PrintName.Text.Trim() != "")
                    {
                        // myCommandStk.Connection.Open();
                        int Num = myCommandStkUpdate.ExecuteNonQuery();
                        if (Num != 0)
                        {
                            MessageBox.Show("Record Successfully Updated....", "Update Record");
                            CleanUp();
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
                        PrintName.Focus();
                    }
                    myCommandStkUpdate.Connection.Close();
                }
                else
                {
                    if (PrintName.Text.Trim() != "")
                    {
                        string querySalesInvEntry = "";
                        querySalesInvEntry = "insert into AgentsList(AcctName, PrimaryAcctID,PrimaryAcctName,Alias,Address1,Address2,City,State,Mobile1,Phone,GSTIN,Email,Website,OpBalanceDR,OpBalanceCR,PINCode,CompID,ShippingAddr1,ShippingAddr2,Discount, PrintName, CommissionPerc)  Values ( '" + PrintName.Text.Trim() + "','" + dPrimaryAcctId + "','" + dPrimaryAcctName + "','','" + Address1.Text + "','" + Address2.Text + "','" + City.Text + "','" + State.Text + "','" + Mob.Text + "','','" + GSTIN.Text + "','','','0','0','','" + CompID + "','','','','" + PrintName.Text + "','" + CommissionPerc.Text + "')";
                        SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                        myCommandInvEntry.Connection.Open();
                        int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                        if (NumPInv != 0)
                        {
                            MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                            PrintName.Clear();
                            CleanUp();
                            this.Close();

                        }
                        else
                        {
                            MessageBox.Show("Account is not Inserted....", "Insert Record Error");
                        }
                        myCommandInvEntry.Connection.Close();
                    }
                    else
                    {
                        MessageBox.Show("Enter Name");
                        PrintName.Focus();
                    }
                    // myConnStock.Close();

                }



            }

        }


    }
}
