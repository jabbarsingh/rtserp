using RTSJewelERP.DebitCreditAccountsListTableAdapters;
using RTSJewelERP.SaleVoucherInvoiceNumberListTableAdapters;
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
    public partial class AddReceipt : Window
    {
        private long InvoiceNumber = 0;
        private long voucherNumber = 0;
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public AddReceipt()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window
            DEBIT_Account.Focus();
            autocompltCustName.autoTextBox.Clear();
            BindDebitAccountsComboBox(DEBIT_Account);
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();


            string sqlvoucher = "select number from AutoIncrement where Name = 'ReceiptVoucher' and CompID = '" + CompID + "'";
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

            if (e.Key == Key.PageUp)
            {
                if (Convert.ToInt64(VoucherNumber.Text.Trim()) < voucherNumber)
                {
                    Int64 inpageup = (VoucherNumber.Text.Trim() != "") ? (Convert.ToInt64(VoucherNumber.Text.Trim()) + 1) : 0;
                    VoucherNumber.Text = inpageup.ToString();
                    MoveToBill(inpageup.ToString());

                }
                if (Convert.ToInt64(VoucherNumber.Text.Trim()) == voucherNumber)
                {
                    //autocompltCustName.autoTextBox.Text = "Cash";
                    autocompltCustName.autoTextBox.Focus();
                }
                e.Handled = true;
            }
            if (e.Key == Key.PageDown)
            {
                if (Convert.ToInt64(VoucherNumber.Text.Trim()) > 1)
                {
                    Int64 inpageup = (VoucherNumber.Text.Trim() != "") ? (Convert.ToInt64(VoucherNumber.Text.Trim()) - 1) : 0;
                    VoucherNumber.Text = inpageup.ToString();
                    MoveToBill(inpageup.ToString());
                    e.Handled = true;
                }


            }

        }

        private void MoveToBill(string invnumbertxt)
        {

            //isShipping.IsChecked = false;
            autocompltCustName.autoTextBox.Clear();
            AgainstInv.Text = "";
            InvoiceNumberCmb.Text = "";
            //DEBIT_Account;
            //AgainstInv.Clear();
            //VoucherNumber.Clear();
            invDate.SelectedDate = DateTime.Now;
            txtAmount.Clear();
            //Mode.Clear();
            Narration.Clear();

            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //string sql = "select ItemName,HSN,BilledQty,BilledWt,WastePerc,TotalBilledWt,MakingCharge,SalePrice,TotalAmount,Discount,TaxablelAmount,TotalAmount,GSTRate,GSTTax,TotalAmount from SalesVoucherInventory where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text + "' and CompID = '" + CompID + "'";
            string sql = "select DebtorAccountName, CreditorAccountName,CR,DR,PayMode,InvoiceNumber,Against,Narration,TransactionDate from ReceiptVouchers where LTRIM(RTRIM(VoucherNumber))='" + invnumbertxt + "' and CompID = '" + CompID + "'";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = conn;
            cmd.Connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            double dCR = 0;
            double dDR = 0;
            string invfetchnumber = "";

            while (reader.Read())
            {
                DEBIT_Account.Text = (reader["DebtorAccountName"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "";
                autocompltCustName.autoTextBox.Text = (reader["CreditorAccountName"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "";
                dCR = (reader["CR"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                dDR = (reader["DR"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                Mode.Text = (reader["PayMode"] != DBNull.Value) ? (reader.GetString(4).Trim()) : "";

                AgainstInv.Text = (reader["Against"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                invfetchnumber = (reader["InvoiceNumber"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";
                InvoiceNumberCmb.Text = invfetchnumber;
                Narration.Text = (reader["Narration"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
                invDate.Text = reader.GetDateTime(8).ToString();
                txtAmount.Text = dCR.ToString();             

            }
            reader.Close();

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();
                //select SUM(CAST(DR AS float)) As DebtAmount  from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' --and  TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
                // select SUM(CAST(CR AS float)) As CreditAmount  from PaymentVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' --and TransactionDate  <= '" + enddt + "' and TransactionDate >= '" + sdt + "'


                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"

                SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(VoucherNumber)) As VoucherNumber ,LTRIM(RTRIM(VoucherType))  As VoucherType,LTRIM(RTRIM(DebtorAccountName)) As DebtorAccountName,LTRIM(RTRIM(CreditorAccountName)) As CreditorAccountName,CR As Amount,PayMode,Against,Narration,TransactionDate,CreationDate,UpdateDate,CreatedBy from ReceiptVouchersHistory where CompID = '" + CompID + "' and LTRIM(RTRIM(VoucherNumber)) = '" + invnumbertxt + "')", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt2 = new System.Data.DataTable("Cash Flow");
                sda.Fill(dt2);
                VoucherEntryHistory.ItemsSource = dt2.DefaultView;
                VoucherEntryHistory.AutoGenerateColumns = true;
                VoucherEntryHistory.CanUserAddRows = false;
            }


            autocompltCustName.autoTextBox.Focus();

        }

        private void AgainstInv_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            //InvoiceNumberCmb.Visibility = Visibility.Collapsed;

            ComboBox cbx = (ComboBox)sender;
            string val = String.Empty;
            if (cbx.SelectedValue == null)
                val = cbx.SelectionBoxItem.ToString();
            else
                val = cboParser(cbx.SelectedValue.ToString());

            if (val == "Invoice")
            {
                InvoiceNumberCmb.Visibility = Visibility.Visible;
                BindComboBoxAccountInvoiceList(autocompltCustName.autoTextBox.Text.Trim());

            }


            //InvoiceNumberCmb.Visibility = Visibility.Hidden;
        }

        private static string cboParser(string controlString)
        {
            if (controlString.Contains(':'))
            {
                controlString = controlString.Split(':')[1].TrimStart(' ');
            }
            return controlString;
        }

        public void BindComboBoxAccountInvoiceList(string custacctname)
        {
            var custAdpt = new SalesVouchersTableAdapter();
            var custInfoVal = custAdpt.GetData();
            //var LinqRes = (from UserRec in custInfoVal
            //               orderby UserRec.StorageName ascending
            //               select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
            //StorageName.ItemsSource = LinqRes;

            InvoiceNumberCmb.ItemsSource = custInfoVal.Where(c => ((c.AccountName.Trim() == custacctname.Trim()) && (c.CompID.ToString()==CompID.Trim() )))
                     .Select(x => x.InvoiceNumber.Trim()).Distinct().ToList();
            //TrayName.SelectedItem = "Cash";

            // comboBoxName.SelectedValueBinding = new Binding("Col6");
        }

        private void InvoiceNumberCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // open dialog box of complete payment or transaction history of the invoice number
        }


        public void BindDebitAccountsComboBox(ComboBox debitacct)
        {
            var custAdpt = new AccountsListTableAdapter();
            var custInfoVal = custAdpt.GetData();
            //var LinqRes = (from UserRec in custInfoVal
            //               where UserRec.PrimaryAcctName.Trim() == "Cash" || UserRec.PrimaryAcctName.Trim() == "Bank" || UserRec.AcctName.Trim() == "Cash"
            //               orderby UserRec.AcctName ascending
            //               select (UserRec.AcctName + "-" + UserRec.AcctID)).Distinct();


            DEBIT_Account.ItemsSource = custInfoVal.Where(c => (c.PrimaryAcctName.Trim() == "Cash" || c.PrimaryAcctName.Trim() == "Bank" || c.AcctName.Trim() == "Cash"))
                                      .Select(x => x.AcctName.Trim()).Distinct().ToList();
            DEBIT_Account.SelectedItem = "Cash";
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }





        private TargetType GetParent<TargetType>(DependencyObject o) where TargetType : DependencyObject
        {
            if (o == null || o is TargetType) return (TargetType)o;
            return GetParent<TargetType>(VisualTreeHelper.GetParent(o));
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





        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection myConnCustExistr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            myConnCustExistr.Open();
            string CountSVCustExts = "SELECT COUNT(*) From AccountsList  where AcctName = '" + autocompltCustName.autoTextBox.Text.Trim() + "' and CompID = '" + CompID + "'";
            // string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
            SqlCommand myCommandCustEx = new SqlCommand(CountSVCustExts, myConnCustExistr);
            myCommandCustEx.Connection = myConnCustExistr;

            //int countRec = myCommand.ExecuteNonQuery();
            int countRecCustEx = (int)myCommandCustEx.ExecuteScalar();
            myCommandCustEx.Connection.Close();
            if (countRecCustEx < 1)
            {

                MessageBox.Show("Wrong Account Name, please select correct account name ");
                //autocompltCustName.autoTextBoxCustNameBarcode.Focus();
            }
            else
            {


                if ((DEBIT_Account.Text.Trim() != "") && (autocompltCustName.autoTextBox.Text.Trim() != "") && (txtAmount.Text.Trim() != ""))
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

                    if (Regex.IsMatch(autocompltCustName.autoTextBox.Text.Trim(), @"^\d+$") || 1 == 1)
                    {
                        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                        //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                        con.Open();
                        //string sql = "SELECT COUNT(*) From AccountsList where AcctName='" + textBoxAcctName.Text.Trim() + "'";
                        SqlCommand cmd;//= new SqlCommand(sql, con);

                        long debitacctnumber = 0;
                        long creditacctnumber = 0;
                        string againstinvnumber = "";
                        cmd = new SqlCommand("SPUpdateAccountsForReceiptVoucher", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@DebtorAcctName", DEBIT_Account.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@CreditorAcctName", autocompltCustName.autoTextBox.Text.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@DebtorAcctNumber", debitacctnumber));
                        cmd.Parameters.Add(new SqlParameter("@CreditorAcctNumber", creditacctnumber));
                        cmd.Parameters.Add(new SqlParameter("@VoucherNumber", VoucherNumber.Text));
                        cmd.Parameters.Add(new SqlParameter("@VoucherType", "Receipt Voucher"));
                        cmd.Parameters.Add(new SqlParameter("@ReceiptDate", InvdateValue));
                        cmd.Parameters.Add(new SqlParameter("@PayMode", Mode.Text));
                        if (AgainstInv.Text == "Invoice")
                        {
                            againstinvnumber = InvoiceNumberCmb.Text;
                        }

                        cmd.Parameters.Add(new SqlParameter("@InvoiceNumber", againstinvnumber));
                        cmd.Parameters.Add(new SqlParameter("@Against", AgainstInv.Text));
                        cmd.Parameters.Add(new SqlParameter("@Narration", Narration.Text));
                        cmd.Parameters.Add(new SqlParameter("@amount", txtAmount.Text));
                        cmd.Parameters.Add(new SqlParameter("@CompID", CompID));

                        //con.Open();
                        //cmd.ExecuteNonQuery();
                        int countRecPay = cmd.ExecuteNonQuery();
                        if (countRecPay != 0)
                        {
                            MessageBox.Show("Success....", "Added Record");

                            if (voucherNumber == Convert.ToInt64(VoucherNumber.Text.Trim()))
                            {
                                string currentInvNumber = "";
                                SqlConnection conCurrentInv = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                                conCurrentInv.Open();
                                string sqlCurrentInv = "select number from AutoIncrement where Name = 'ReceiptVoucher' and CompID = '" + CompID + "'";
                                SqlCommand cmdCurrentInv = new SqlCommand(sqlCurrentInv);
                                cmdCurrentInv.Connection = conCurrentInv;
                                SqlDataReader readerCurrentInv = cmdCurrentInv.ExecuteReader();

                                //tmpProduct = new Product();

                                while (readerCurrentInv.Read())
                                {
                                currentInvNumber = readerCurrentInv.GetInt64(0).ToString().Trim();

                                }
                                readerCurrentInv.Close();

                                if (currentInvNumber == VoucherNumber.Text.Trim())
                                {


                                    SqlConnection consrauto = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                                    consrauto.Open();
                                    string updateVoucher = "";
                                    updateVoucher = "update AutoIncrement  set  Number='" + (Convert.ToInt64(VoucherNumber.Text) + 1) + "' where Name ='ReceiptVoucher' and Type='Receipt Voucher'  and CompID = '" + CompID + "' ";
                                    SqlCommand myCommandStkUpdateauto = new SqlCommand(updateVoucher, consrauto);
                                    myCommandStkUpdateauto.Connection = consrauto;
                                    int Numauto = myCommandStkUpdateauto.ExecuteNonQuery();
                                    if (Numauto != 0)
                                    {

                                        //autocompltCustName.autoTextBox.Clear();
                                        //txtAmount.Clear();
                                        //Narration.Clear();
                                        //VoucherNumber.Text = (voucherNumber + 1).ToString();
                                        //DEBIT_Account.Focus();
                                    }
                                }
                            }
                            SqlConnection conhist = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                            conhist.Open();
                            //string sql = "SELECT COUNT(*) From AccountsList where AcctName='" + textBoxAcctName.Text.Trim() + "'";
                            SqlCommand cmdhist;//= new SqlCommand(sql, con);

                            long debitacctnumberhist = 0;
                            long creditacctnumberhist = 0;
                            string againstinvnumberhist = "";
                            cmdhist = new SqlCommand("SPUpdateAccountsForReceiptVoucherHistory", con);
                            cmdhist.CommandType = CommandType.StoredProcedure;
                            cmdhist.Parameters.Add(new SqlParameter("@DebtorAcctName", DEBIT_Account.Text.Trim()));
                            cmdhist.Parameters.Add(new SqlParameter("@CreditorAcctName", autocompltCustName.autoTextBox.Text.Trim()));
                            cmdhist.Parameters.Add(new SqlParameter("@DebtorAcctNumber", debitacctnumberhist));
                            cmdhist.Parameters.Add(new SqlParameter("@CreditorAcctNumber", creditacctnumberhist));
                            cmdhist.Parameters.Add(new SqlParameter("@VoucherNumber", VoucherNumber.Text));
                            cmdhist.Parameters.Add(new SqlParameter("@VoucherType", "Receipt Voucher"));
                            cmdhist.Parameters.Add(new SqlParameter("@ReceiptDate", InvdateValue));
                            cmdhist.Parameters.Add(new SqlParameter("@PayMode", Mode.Text));

                            if (AgainstInv.Text == "Invoice")
                            {
                                againstinvnumberhist = InvoiceNumberCmb.Text;
                            }

                            cmdhist.Parameters.Add(new SqlParameter("@InvoiceNumber", againstinvnumberhist));
                            cmdhist.Parameters.Add(new SqlParameter("@Against", AgainstInv.Text));
                            cmdhist.Parameters.Add(new SqlParameter("@Narration", Narration.Text));
                            cmdhist.Parameters.Add(new SqlParameter("@amount", txtAmount.Text));
                            cmdhist.Parameters.Add(new SqlParameter("@CompID", CompID));

                            //con.Open();
                            //cmd.ExecuteNonQuery();
                            int countRecPayhist = cmdhist.ExecuteNonQuery();
                            if (countRecPayhist != 0)
                            {
                                DEBIT_Account.Focus();
                                AddReceipt sv = new AddReceipt();
                                //SaleVoucherBarcode sv = new SaleVoucherBarcode();
                                sv.ShowDialog();

                                //MessageBox.Show("Success....", "Added Record History");

                            }



                        }



                    }

                    //On Success clear all data and increment voucher number by 1

                }
                else
                    MessageBox.Show("Please Provide All Details");
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

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            AddSundryDebtor asd = new AddSundryDebtor();
            asd.ShowDialog();
            autocompltCustName.autoTextBox.Focus();
        }


        private void dateShortcut_Click(object sender, RoutedEventArgs e)
        {
            invDate.IsDropDownOpen = true;
        }

        private void DatePicker_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter)
            {
                DEBIT_Account.Focus();
                //TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                //UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                //if (keyboardFocus != null)
                //{
                //    keyboardFocus.MoveFocus(tRequest);
                //}

                //e.Handled = true;
            }

            //if (e.Key == Key.RightShift)
            //{

            //    TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Previous);
            //    UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

            //    if (keyboardFocus != null)
            //    {
            //        keyboardFocus.MoveFocus(tRequest);

            //    }

            //    e.Handled = true;
            //}

        }

    }
}
