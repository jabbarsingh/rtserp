using RTSJewelERP.DebitCreditAccountsListTableAdapters;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RTSJewelERP
{
    /// <summary>
    /// Interaction logic for Receipt.xaml
    /// </summary>
    public partial class Payment : Page
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;
        private long InvoiceNumber = 0;
        private long voucherNumber = 0;

        public Payment()
        {
            InitializeComponent();
            CREDIT_Account.Focus();
            BindCreditAccountsComboBox(CREDIT_Account);
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();


            string sqlvoucher = "select number from AutoIncrement where Name = 'PaymentVoucher'  and CompID = '" + CompID + "'";
            SqlCommand cmdvoucher = new SqlCommand(sqlvoucher);
            cmdvoucher.Connection = con;
            SqlDataReader readerVoucher = cmdvoucher.ExecuteReader();

            //tmpProduct = new Product();

            while (readerVoucher.Read())
            {
                voucherNumber = readerVoucher.GetInt64(0);

            }
            readerVoucher.Close();
            VoucherNumber.Text = voucherNumber.ToString();
        }

        public void BindCreditAccountsComboBox(ComboBox creditacct)
        {
            var custAdpt = new AccountsListTableAdapter();
            var custInfoVal = custAdpt.GetData();
            //var LinqRes = (from UserRec in custInfoVal
            //               where UserRec.PrimaryAcctName.Trim() == "Cash" || UserRec.PrimaryAcctName.Trim() == "Bank" || UserRec.AcctName.Trim() == "Cash"
            //               orderby UserRec.AcctName ascending
            //               select (UserRec.AcctName + "-" + UserRec.AcctID)).Distinct();


            CREDIT_Account.ItemsSource = custInfoVal.Where(c => (c.PrimaryAcctName.Trim() == "Cash" || c.PrimaryAcctName.Trim() == "Bank" || c.AcctName.Trim() == "Cash"))
                     .Select(x => x.AcctName.Trim()).Distinct().ToList();
            CREDIT_Account.SelectedItem = "Cash";
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
                    //var PrimaryAcctID = (reader["PrimaryAcctID"] != DBNull.Value) ? (reader.GetInt32(2)).ToString().Trim() : "";
                    //MainAccounts.Text = ((reader["PrimaryAcctName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "") + "-" + ((reader["PrimaryAcctID"] != DBNull.Value) ? (reader.GetInt32(2)).ToString().Trim() : "");
                    //Alias.Text = (reader["Alias"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";
                    //Address1.Text = (reader["Address1"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                    //Address2.Text = (reader["Address2"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
                    //City.Text = (reader["City"] != DBNull.Value) ? (reader.GetString(8).Trim()) : "";
                    //State.Text = (reader["State"] != DBNull.Value) ? (reader.GetString(9).Trim()) : "";
                    //PinCode.Text = (reader["PINCode"] != DBNull.Value) ? (reader.GetString(10).Trim()) : "";
                    //Mob.Text = (reader["Mobile1"] != DBNull.Value) ? (reader.GetString(11).Trim()) : "";
                    //Phone.Text = (reader["Phone"] != DBNull.Value) ? (reader.GetString(13).Trim()) : "";
                    //GSTIN.Text = (reader["GSTIN"] != DBNull.Value) ? (reader.GetString(14).Trim()) : "";
                    //Email.Text = (reader["Email"] != DBNull.Value) ? (reader.GetString(15).Trim()) : "";
                    //Web.Text = (reader["Website"] != DBNull.Value) ? (reader.GetString(17).Trim()) : "";
                    //OpeningBalanceDr.Text = (reader["OpBalanceDR"] != DBNull.Value) ? (reader.GetDouble(18)).ToString() : "";
                    //OpeningBalanceCr.Text = (reader["OpBalanceCR"] != DBNull.Value) ? (reader.GetDouble(19)).ToString() : "";
                    ////var ischeckinv = (reader["IsInventoryAffected"] != DBNull.Value) ? (reader.GetString(24).Trim()) : "";
                    ////invAffectedYes.Checked
                    //lblCR.Content = "CR :" + OpeningBalanceCr.Text;
                    //lblDR.Content = "DR :" + OpeningBalanceDr.Text;
                    //lblBal.Content = "Difference : " + (((reader["OpBalanceCR"] != DBNull.Value) ? (reader.GetDouble(19)) : 0) - ((reader["OpBalanceDR"] != DBNull.Value) ? (reader.GetDouble(18)) : 0)).ToString().Trim();
                    ////OpeningStockWt.Text = (reader["OpeningStockWt"] != DBNull.Value) ? (reader.GetDouble(52)).ToString().Trim() : "";

                    ////var CustID = reader.GetValue(0).ToString();




                }
                reader.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string BillDateInv = invDate.SelectedDate.ToString();

            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dtin = Convert.ToDateTime(BillDateInv);
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

            string InvdateValue = yearsin + "/" + monthsin + "/" + daysin;

            if (Regex.IsMatch(textBoxAcctName.Text.Trim(), @"^\d+$") || 1 == 1)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                //string sql = "SELECT COUNT(*) From AccountsList where AcctName='" + textBoxAcctName.Text.Trim() + "'";
                SqlCommand cmd ; // = new SqlCommand(sql, con);
                //cmd.Connection = con;
                //cmd.Connection = con;
                //int countRecDelDel = (int)cmd.ExecuteScalar();
                //cmd.Connection.Close();
                //if (countRecDelDel == 0)
                //{
                //    MessageBoxResult result = MessageBox.Show("Debit Account Does Not Exist?", "Add Record", MessageBoxButton.YesNo);
                //    if (result == MessageBoxResult.Yes)
                //        MessageBox.Show("Show Popup");
                //}
                long debitacctnumber = 0;
                long creditacctnumber = 0;
                string againstinvnumber = "";
                cmd = new SqlCommand("SPUpdateAccountsForPaymentVoucher", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@DebtorAcctName", CREDIT_Account.Text));
                cmd.Parameters.Add(new SqlParameter("@CreditorAcctName", textBoxAcctName.Text));
                cmd.Parameters.Add(new SqlParameter("@DebtorAcctNumber", debitacctnumber));
                cmd.Parameters.Add(new SqlParameter("@CreditorAcctNumber", creditacctnumber));
                cmd.Parameters.Add(new SqlParameter("@VoucherNumber", VoucherNumber.Text));
                cmd.Parameters.Add(new SqlParameter("@VoucherType", "Receipt Voucher"));
                cmd.Parameters.Add(new SqlParameter("@ReceiptDate", InvdateValue));
                cmd.Parameters.Add(new SqlParameter("@PayMode", Mode.Text));
                cmd.Parameters.Add(new SqlParameter("@InvoiceNumber", againstinvnumber));
                cmd.Parameters.Add(new SqlParameter("@Against", AgainstInv.Text));
                cmd.Parameters.Add(new SqlParameter("@Narration", Narration.Text));
                cmd.Parameters.Add(new SqlParameter("@amount", txtAmount.Text));
                cmd.Parameters.Add(new SqlParameter("@CompID", CompID));
                //con.Open();
                cmd.ExecuteNonQuery();
            }


            //On Success clear all data and increment voucher number by 1

            textBoxAcctName.Clear();
            txtAmount.Clear();
            Narration.Clear();
            VoucherNumber.Text = (Convert.ToInt32(VoucherNumber.Text) + 1).ToString();
            CREDIT_Account.Focus();
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

    }
}
