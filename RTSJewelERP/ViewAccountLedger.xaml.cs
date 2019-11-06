using iTextSharp.text;
using RTSJewelERP.MainAccountsTableAdapters;
using RTSJewelERP.StateTableAdapters;
using RTSJewelERP.TrayListTableAdapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Printing;
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
    /// Interaction logic for ShowItemInfo.xaml
    /// </summary>
    public partial class ViewAccountLedger : Window
    {

        private string estimateHomeIcon = "";
        private string saleHomeIcon = "";
        private string purchaseHomeIcon = "";
        private string ItemHomeIcon = "";
        private string stockEtryBarcodeHomeIcon = "";
        private string stockentryHomeIcon = "";
        private string stockentryWatchHomeIcon = "";
        private string DayReportHomeIcon = "";
        private string AccountHomeIcon = "";
        private string ReceiptHomeIcon = "";
        private string PaymentHomeIcon = "";
        private string JournalHomeIcon = "";
        private string BarcodeHomeIcon = "";
        private string CloseHomeIcon = "";
        private string BackupShortcutHome = "";
        private string AddAccountShortcutHome = "";
        private string AddItemShortcutHome = "";
        private string AddSaleShortcutHome = "";
        private string AddPurchaseShortcutHome = "";
        private string AddReceiptShortcutHome = "";
        private string AddPaymentShortcutHome = "";
        private string AddJournalShortcutHome = "";
        private string StockEntryShortcutHome = "";
        private string StockTransferShortcutHome = "";

        private string EstimationShortcutHome = "";
        private string EstimationA6ShortcutHome = "";
        private string WatchShortcutHome = "";
        private string DayBookShortcutHome = "";





        public List<string> CountryList { get; set; }
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public ViewAccountLedger()
        {
        }

        public ViewAccountLedger(string acctnameselected, string startdatev, string enddatev)
        {
            InitializeComponent();
            autocompltCustName.autoTextBox.Text = acctnameselected.Trim();
            startDate_Ledger.Text = startdatev.Trim();
            toDate_Ledger.Text = enddatev.Trim();
            autocompltCustName.autoTextBox.Focus();
            //BindComboBoxTrayList(cmbTrayLists);
            //BindComboBoxMainAccountType(cmbMainType);
            BindComboBox(cmbStates);
            //itemnames = itemName;
            //companyId = CompID;
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window


            SqlConnection conn2 = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conn2.Open();

            string sql = "select * from ConfigTable";

            SqlCommand cmdconfig = new SqlCommand(sql);
            cmdconfig.Connection = conn2;
            SqlDataReader readerConfig = cmdconfig.ExecuteReader();


            if (readerConfig.HasRows)
            {
                while (readerConfig.Read())
                {
                    string nameconf = (readerConfig["Name"] != DBNull.Value) ? (readerConfig.GetString(0).Trim()) : "";
                    //IDNo.Text = (readerConfig["ID"] != DBNull.Value) ? (readerConfig.GetString(1).Trim()) : "";
                    string parentconf = (readerConfig["Parent"] != DBNull.Value) ? (readerConfig.GetString(2).Trim()) : "";
                    string grandparentconf = (readerConfig["GrandParent"] != DBNull.Value) ? (readerConfig.GetString(3).Trim()) : "";
                    string pagenameconf = (readerConfig["MapTo"] != DBNull.Value) ? (readerConfig.GetString(4).Trim()) : "";

                    if (nameconf.Trim() == "Estimate" && parentconf.Trim() == "HomeIcon")
                    {
                        estimateHomeIcon = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "Sale" && parentconf.Trim() == "HomeIcon")
                    {
                        saleHomeIcon = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "Purchase" && parentconf.Trim() == "HomeIcon")
                    {
                        purchaseHomeIcon = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "Item" && parentconf.Trim() == "HomeIcon")
                    {
                        ItemHomeIcon = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "StockEntry-Barcode" && parentconf.Trim() == "HomeIcon")
                    {
                        stockEtryBarcodeHomeIcon = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "StockEntry" && parentconf.Trim() == "HomeIcon")
                    {
                        stockentryHomeIcon = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "StockEntry-Watch" && parentconf.Trim() == "HomeIcon")
                    {
                        stockentryWatchHomeIcon = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "DayReport" && parentconf.Trim() == "HomeIcon")
                    {
                        DayReportHomeIcon = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "Account" && parentconf.Trim() == "HomeIcon")
                    {
                        AccountHomeIcon = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "Receipt" && parentconf.Trim() == "HomeIcon")
                    {
                        ReceiptHomeIcon = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "Payment" && parentconf.Trim() == "HomeIcon")
                    {
                        PaymentHomeIcon = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "Journal" && parentconf.Trim() == "HomeIcon")
                    {
                        JournalHomeIcon = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "Barcode" && parentconf.Trim() == "HomeIcon")
                    {
                        BarcodeHomeIcon = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "Close" && parentconf.Trim() == "HomeIcon")
                    {
                        CloseHomeIcon = pagenameconf.Trim();
                    }


                    if (nameconf.Trim() == "AddAccountShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        AddAccountShortcutHome = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "AddItemShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        AddItemShortcutHome = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "AddSaleShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        AddSaleShortcutHome = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "AddPurchaseShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        AddPurchaseShortcutHome = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "ReceiptShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        AddReceiptShortcutHome = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "PaymentShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        AddPaymentShortcutHome = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "AddJournalShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        AddJournalShortcutHome = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "StockEntryShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        StockEntryShortcutHome = pagenameconf.Trim();
                    }



                    if (nameconf.Trim() == "StockTransferShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        StockTransferShortcutHome = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "BackupShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        BackupShortcutHome = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "EstimationShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        EstimationShortcutHome = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "Estimation-A6ShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        EstimationA6ShortcutHome = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "WatchShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        WatchShortcutHome = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "DayBookShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        DayBookShortcutHome = pagenameconf.Trim();

                    }




                }
            }

            readerConfig.Close();


        }


        public void BindComboBox(ComboBox cmbStates)
        {
            var custAdpt = new StateTableAdapter();
            var custInfoVal = custAdpt.GetData();
            var LinqRes = (from UserRec in custInfoVal
                           orderby UserRec.StateName ascending
                           select (UserRec.StateName)).Distinct();
            cmbStates.ItemsSource = LinqRes;
            // comboBoxName.SelectedValueBinding = new Binding("Col6");
        }

        //public void BindComboBoxMainAccountType(ComboBox mainTypeAct)
        //{
        //    var custAdpt = new MainAccountsTypeTableAdapter();
        //    var custInfoVal = custAdpt.GetData();
        //    var LinqRes = (from UserRec in custInfoVal
        //                   orderby UserRec.AcctName ascending
        //                   //select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
        //                   select (UserRec.AcctName.Trim())).Distinct();
        //    cmbMainType.ItemsSource = LinqRes;

        //    //var custAdpt = new TrayListInStorageByPcTableAdapter();
        //    //var custInfoVal = custAdpt.GetData();
        //    //if (custInfoVal != null)
        //    //{                
        //    //    cmbTrayLists.ItemsSource = custInfoVal.Where(c => (c.StorageName.Trim() == "Main")).Select(x => x.TrayName.Trim()).Distinct().ToList();
        //    //}
        //}


        //public void BindComboBoxTrayList(ComboBox trayname)
        //{
        //    //var custAdpt = new TrayListInStorageByPcTableAdapter();
        //    //var custInfoVal = custAdpt.GetData();
        //    //var LinqRes = (from UserRec in custInfoVal
        //    //               orderby UserRec.TrayName ascending
        //    //               //select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
        //    //               select (UserRec.TrayName.Trim())).Distinct();
        //    //cmbsTrayList.ItemsSource = LinqRes;

        //    var custAdpt = new TrayListInStorageByPcTableAdapter();
        //    var custInfoVal = custAdpt.GetData();
        //    if (custInfoVal != null)
        //    {
        //        cmbTrayLists.ItemsSource = custInfoVal.Where(c => (c.StorageName.Trim() == "Main")).Select(x => x.TrayName.Trim()).Distinct().ToList();
        //    }
        //}
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

        private void Button_LedgerClick(object sender, RoutedEventArgs e)
        {
            string sdt = startDate_Ledger.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDate_Ledger.SelectedDate);
            //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            int years = dt.Year;
            string months = dt.Month.ToString();
            if (dt.Month < 10)
            {
                months = "0" + months;
            }
            string days = dt.Day.ToString();
            if (dt.Day < 10)
            {
                days = "0" + days;
            }


            sdt = years + "/" + months + "/" + days;
            //sdt = years + "/" + 04 + "/" + 01;


            string enddt = toDate_Ledger.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDate_Ledger.SelectedDate);
            int yeard = edt.Year;
            string monthd = edt.Month.ToString();
            if (edt.Month < 10)
            {
                monthd = "0" + monthd;
            }
            string dayd = edt.Day.ToString();
            if (edt.Day < 10)
            {
                dayd = "0" + dayd;
            }
            enddt = yeard + "/" + monthd + "/" + dayd;



            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetAccountLedgerbyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@VchType", cmbVchType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        

                System.Data.DataTable dt1 = new System.Data.DataTable("Account Ledger");
                sda.Fill(dt1);
                AccountLedgerGrid.ItemsSource = dt1.DefaultView;
                AccountLedgerGrid.AutoGenerateColumns = true;
                AccountLedgerGrid.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                //for (int s = 0; s < AccountLedgerGrid.Items.Count - 1; s++ )
                //{
                //    sumDr += (double.Parse((AccountLedgerGrid.Columns[5].GetCellContent(AccountLedgerGrid.Items[s]) as TextBlock).Text));
                //}
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    sumDr = sumDr + ((row["DR"] != DBNull.Value) ? (Convert.ToDouble(row["DR"])) : 0);
                    sumCr = sumCr + ((row["CR"] != DBNull.Value) ? (Convert.ToDouble(row["CR"])) : 0);
                }
                TotalDebitAmt_Ledger.Text = sumDr.ToString();
                TotalCreditAmt_Ledger.Text = sumCr.ToString();
                Balance_Ledger.Text = (sumDr - sumCr).ToString();
                //while (reader.Read())
                //{
                //    double debitamt = (reader["DebtAmount"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                //    double creditamt = (reader["CreditAmount"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                //    double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                //    TotalDebitAmt_Ledger.Text = debitamt.ToString();
                //    TotalCreditAmt_Ledger.Text = creditamt.ToString();
                //    Balance_Ledger.Text = (creditamt - debitamt).ToString();
                //    openingBal_Ledger.Text = opBal.ToString();
                //}

                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(VoucherNumber)) As VoucherNumber ,LTRIM(RTRIM(VoucherType))  As VoucherType,LTRIM(RTRIM(DebtorAccountName)) As DebtorAccountName,LTRIM(RTRIM(CreditorAccountName)) As CreditorAccountName,CR As Amount,PayMode,Against,Narration,TransactionDate,CreationDate,UpdateDate,CreatedBy from ReceiptVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='" + autocompltCustName.autoTextBox.Text.Trim() + "' and CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') Union ( select  LTRIM(RTRIM(VoucherNumber)) As VoucherNumber ,LTRIM(RTRIM(VoucherType))  As VoucherType,LTRIM(RTRIM(DebtorAccountName)) As DebtorAccountName,LTRIM(RTRIM(CreditorAccountName)) As CreditorAccountName,CR As Amount,PayMode,Against,Narration,TransactionDate,CreationDate,UpdateDate,CreatedBy from PaymentVouchers  where UPPER(LTRIM(RTRIM(DebtorAccountName)))='" + autocompltCustName.autoTextBox.Text.Trim() + "' and CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "')", con);
                //SqlDataAdapter sda = new SqlDataAdapter(com);

            }

            using (SqlConnection con = new SqlConnection())
            {

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetAccountLedgerSummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double dDebtAcctLedgerAmt = (reader["DebtAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    double dPayVAmt = (reader["PayVAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double dCredAcctLedgerAmt = (reader["CredAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double dReceiptVAmt = (reader["ReceiptVAmt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    double opBalBookStartDr = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                    double opBalBookStartCr = (reader["OpeningBalBookStartCR"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;

                    //TotalDebitAmt_Ledger.Text = debitamt.ToString();
                    //TotalCreditAmt_Ledger.Text = creditamt.ToString();
                    //Balance_Ledger.Text = (creditamt - debitamt).ToString();
                    openingBal_Ledger.Text = opBal.ToString();
                    openingBalBookStartDR.Text = opBalBookStartDr.ToString();
                    openingBalBookStartCR.Text = opBalBookStartCr.ToString();
                }

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

        private void TabAccountsLedger_Selected(object sender, RoutedEventArgs e)
        {
            string sdt = startDate_Ledger.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDate_Ledger.SelectedDate);
            //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            int years = dt.Year;
            string months = dt.Month.ToString();
            if (dt.Month < 10)
            {
                months = "0" + months;
            }
            string days = dt.Day.ToString();
            if (dt.Day < 10)
            {
                days = "0" + days;
            }


            sdt = years + "/" + months + "/" + days;
            //sdt = years + "/" + 04 + "/" + 01;
            //startDate_Ledger.Text = sdt;

            string enddt = toDate_Ledger.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDate_Ledger.SelectedDate);
            int yeard = edt.Year;
            string monthd = edt.Month.ToString();
            if (edt.Month < 10)
            {
                monthd = "0" + monthd;
            }
            string dayd = edt.Day.ToString();
            if (edt.Day < 10)
            {
                dayd = "0" + dayd;
            }
            enddt = yeard + "/" + monthd + "/" + dayd;



            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetAccountLedgerbyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@VchType", cmbVchType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        

                System.Data.DataTable dt1 = new System.Data.DataTable("Account Ledger");
                sda.Fill(dt1);
                AccountLedgerGrid.ItemsSource = dt1.DefaultView;
                AccountLedgerGrid.AutoGenerateColumns = true;
                AccountLedgerGrid.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                //for (int s = 0; s < AccountLedgerGrid.Items.Count - 1; s++ )
                //{
                //    sumDr += (double.Parse((AccountLedgerGrid.Columns[5].GetCellContent(AccountLedgerGrid.Items[s]) as TextBlock).Text));
                //}
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    sumDr = sumDr + ((row["DR"] != DBNull.Value) ? (Convert.ToDouble(row["DR"])) : 0);
                    sumCr = sumCr + ((row["CR"] != DBNull.Value) ? (Convert.ToDouble(row["CR"])) : 0);
                }
                TotalDebitAmt_Ledger.Text = sumDr.ToString();
                TotalCreditAmt_Ledger.Text = sumCr.ToString();
                Balance_Ledger.Text = (sumDr - sumCr).ToString();
                //while (reader.Read())
                //{
                //    double debitamt = (reader["DebtAmount"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                //    double creditamt = (reader["CreditAmount"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                //    double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                //    TotalDebitAmt_Ledger.Text = debitamt.ToString();
                //    TotalCreditAmt_Ledger.Text = creditamt.ToString();
                //    Balance_Ledger.Text = (creditamt - debitamt).ToString();
                //    openingBal_Ledger.Text = opBal.ToString();
                //}

                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(VoucherNumber)) As VoucherNumber ,LTRIM(RTRIM(VoucherType))  As VoucherType,LTRIM(RTRIM(DebtorAccountName)) As DebtorAccountName,LTRIM(RTRIM(CreditorAccountName)) As CreditorAccountName,CR As Amount,PayMode,Against,Narration,TransactionDate,CreationDate,UpdateDate,CreatedBy from ReceiptVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='" + autocompltCustName.autoTextBox.Text.Trim() + "' and CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') Union ( select  LTRIM(RTRIM(VoucherNumber)) As VoucherNumber ,LTRIM(RTRIM(VoucherType))  As VoucherType,LTRIM(RTRIM(DebtorAccountName)) As DebtorAccountName,LTRIM(RTRIM(CreditorAccountName)) As CreditorAccountName,CR As Amount,PayMode,Against,Narration,TransactionDate,CreationDate,UpdateDate,CreatedBy from PaymentVouchers  where UPPER(LTRIM(RTRIM(DebtorAccountName)))='" + autocompltCustName.autoTextBox.Text.Trim() + "' and CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "')", con);
                //SqlDataAdapter sda = new SqlDataAdapter(com);

            }

            using (SqlConnection con = new SqlConnection())
            {

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetAccountLedgerSummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double dDebtAcctLedgerAmt = (reader["DebtAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    double dPayVAmt = (reader["PayVAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double dCredAcctLedgerAmt = (reader["CredAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double dReceiptVAmt = (reader["ReceiptVAmt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    double opBalBookStartDr = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                    double opBalBookStartCr = (reader["OpeningBalBookStartCR"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;

                    //TotalDebitAmt_Ledger.Text = debitamt.ToString();
                    //TotalCreditAmt_Ledger.Text = creditamt.ToString();
                    //Balance_Ledger.Text = (creditamt - debitamt).ToString();
                    openingBal_Ledger.Text = opBal.ToString();
                    openingBalBookStartDR.Text = opBalBookStartDr.ToString();
                    openingBalBookStartCR.Text = opBalBookStartCr.ToString();
                }

            }

        }

        private void autocompltCustName_LostFocus(object sender, RoutedEventArgs e)
        {
            string sdt = startDate_Ledger.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDate_Ledger.SelectedDate);
            //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            int years = dt.Year;
            string months = dt.Month.ToString();
            if (dt.Month < 10)
            {
                months = "0" + months;
            }
            string days = dt.Day.ToString();
            if (dt.Day < 10)
            {
                days = "0" + days;
            }


            sdt = years + "/" + months + "/" + days;

            string enddt = toDate_Ledger.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDate_Ledger.SelectedDate);
            int yeard = edt.Year;
            string monthd = edt.Month.ToString();
            if (edt.Month < 10)
            {
                monthd = "0" + monthd;
            }
            string dayd = edt.Day.ToString();
            if (edt.Day < 10)
            {
                dayd = "0" + dayd;
            }
            enddt = yeard + "/" + monthd + "/" + dayd;



            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetAccountLedgerbyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@VchType", cmbVchType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        

                System.Data.DataTable dt1 = new System.Data.DataTable("Account Ledger");
                sda.Fill(dt1);
                AccountLedgerGrid.ItemsSource = dt1.DefaultView;
                AccountLedgerGrid.AutoGenerateColumns = true;
                AccountLedgerGrid.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                //for (int s = 0; s < AccountLedgerGrid.Items.Count - 1; s++ )
                //{
                //    sumDr += (double.Parse((AccountLedgerGrid.Columns[5].GetCellContent(AccountLedgerGrid.Items[s]) as TextBlock).Text));
                //}
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    sumDr = sumDr + ((row["DR"] != DBNull.Value) ? (Convert.ToDouble(row["DR"])) : 0);
                    sumCr = sumCr + ((row["CR"] != DBNull.Value) ? (Convert.ToDouble(row["CR"])) : 0);
                }
                TotalDebitAmt_Ledger.Text = sumDr.ToString();
                TotalCreditAmt_Ledger.Text = sumCr.ToString();
                Balance_Ledger.Text = (sumDr - sumCr).ToString();
                //while (reader.Read())
                //{
                //    double debitamt = (reader["DebtAmount"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                //    double creditamt = (reader["CreditAmount"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                //    double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                //    TotalDebitAmt_Ledger.Text = debitamt.ToString();
                //    TotalCreditAmt_Ledger.Text = creditamt.ToString();
                //    Balance_Ledger.Text = (creditamt - debitamt).ToString();
                //    openingBal_Ledger.Text = opBal.ToString();
                //}

                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(VoucherNumber)) As VoucherNumber ,LTRIM(RTRIM(VoucherType))  As VoucherType,LTRIM(RTRIM(DebtorAccountName)) As DebtorAccountName,LTRIM(RTRIM(CreditorAccountName)) As CreditorAccountName,CR As Amount,PayMode,Against,Narration,TransactionDate,CreationDate,UpdateDate,CreatedBy from ReceiptVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='" + autocompltCustName.autoTextBox.Text.Trim() + "' and CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') Union ( select  LTRIM(RTRIM(VoucherNumber)) As VoucherNumber ,LTRIM(RTRIM(VoucherType))  As VoucherType,LTRIM(RTRIM(DebtorAccountName)) As DebtorAccountName,LTRIM(RTRIM(CreditorAccountName)) As CreditorAccountName,CR As Amount,PayMode,Against,Narration,TransactionDate,CreationDate,UpdateDate,CreatedBy from PaymentVouchers  where UPPER(LTRIM(RTRIM(DebtorAccountName)))='" + autocompltCustName.autoTextBox.Text.Trim() + "' and CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "')", con);
                //SqlDataAdapter sda = new SqlDataAdapter(com);

            }

            using (SqlConnection con = new SqlConnection())
            {

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetAccountLedgerSummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double dDebtAcctLedgerAmt = (reader["DebtAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    double dPayVAmt = (reader["PayVAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double dCredAcctLedgerAmt = (reader["CredAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double dReceiptVAmt = (reader["ReceiptVAmt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    double opBalBookStartDr = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                    double opBalBookStartCr = (reader["OpeningBalBookStartCR"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;

                    //TotalDebitAmt_Ledger.Text = debitamt.ToString();
                    //TotalCreditAmt_Ledger.Text = creditamt.ToString();
                    //Balance_Ledger.Text = (creditamt - debitamt).ToString();
                    openingBal_Ledger.Text = opBal.ToString();
                    openingBalBookStartDR.Text = opBalBookStartDr.ToString();
                    openingBalBookStartCR.Text = opBalBookStartCr.ToString();
                }

            }
        }

        private void Receipt_Click(object sender, RoutedEventArgs e)
        {

            AddReceipt sv = new AddReceipt();
            sv.ShowDialog();
        }

        private void Payment_Click(object sender, RoutedEventArgs e)
        {

            AddPayment sv = new AddPayment();
            sv.ShowDialog();

        }


        private void printAcctLedger_Click(object sender, RoutedEventArgs e)
        {
            string sdt = startDate_Ledger.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDate_Ledger.SelectedDate);
            //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            int years = dt.Year;
            string months = dt.Month.ToString();
            if (dt.Month < 10)
            {
                months = "0" + months;
            }
            string days = dt.Day.ToString();
            if (dt.Day < 10)
            {
                days = "0" + days;
            }


            sdt = years + "/" + months + "/" + days;

            string enddt = toDate_Ledger.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDate_Ledger.SelectedDate);
            int yeard = edt.Year;
            string monthd = edt.Month.ToString();
            if (edt.Month < 10)
            {
                monthd = "0" + monthd;
            }
            string dayd = edt.Day.ToString();
            if (edt.Day < 10)
            {
                dayd = "0" + dayd;
            }
            enddt = yeard + "/" + monthd + "/" + dayd;



            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetAccountLedgerbyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@VchType", cmbVchType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        

                System.Data.DataTable dt1 = new System.Data.DataTable("Account Ledger");
                sda.Fill(dt1);
                AccountLedgerGrid.ItemsSource = dt1.DefaultView;
                AccountLedgerGrid.AutoGenerateColumns = true;
                AccountLedgerGrid.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    sumDr = sumDr + ((row["DR"] != DBNull.Value) ? (Convert.ToDouble(row["DR"])) : 0);
                    sumCr = sumCr + ((row["CR"] != DBNull.Value) ? (Convert.ToDouble(row["CR"])) : 0);
                }
                TotalDebitAmt_Ledger.Text = sumDr.ToString();
                TotalCreditAmt_Ledger.Text = sumCr.ToString();
                Balance_Ledger.Text = (sumDr - sumCr).ToString();

            }

            using (SqlConnection con = new SqlConnection())
            {

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetAccountLedgerSummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double dDebtAcctLedgerAmt = (reader["DebtAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    double dPayVAmt = (reader["PayVAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double dCredAcctLedgerAmt = (reader["CredAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double dReceiptVAmt = (reader["ReceiptVAmt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    double opBalBookStartDr = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                    double opBalBookStartCr = (reader["OpeningBalBookStartCR"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;

                    //TotalDebitAmt_Ledger.Text = debitamt.ToString();
                    //TotalCreditAmt_Ledger.Text = creditamt.ToString();
                    //Balance_Ledger.Text = (creditamt - debitamt).ToString();
                    openingBal_Ledger.Text = opBal.ToString();
                    openingBalBookStartDR.Text = opBalBookStartDr.ToString();
                    openingBalBookStartCR.Text = opBalBookStartCr.ToString();
                }

            }


            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintQueue = System.Printing.LocalPrintServer.GetDefaultPrintQueue();
            printDlg.PrintTicket = printDlg.PrintQueue.DefaultPrintTicket;
            printDlg.PrintTicket.PageOrientation = PageOrientation.Portrait;

            // Create a FlowDocument dynamically.
            //FlowDocument doc = CreateFlowDocumentJewellery();
            FlowDocument doc = CreateFlowDocumentAccount();
            doc.ColumnWidth = 600;
            doc.Name = "FlowDoc";
            doc.PageHeight = 900;
            doc.PageWidth = 800;
            doc.MinPageWidth = 800;


            // Create IDocumentPaginatorSource from FlowDocument
            IDocumentPaginatorSource idpSource = doc;

            // Call PrintDocument method to send document to printer
            //Uncomment for Print
            printDlg.PrintDocument(idpSource.DocumentPaginator, "Receipt Printing.");
        }

        private FlowDocument CreateFlowDocumentAccount()
        {
            //  Get Confirmation that data saved successfull, 

            string sdt = startDate_Ledger.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDate_Ledger.SelectedDate);
            //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            int years = dt.Year;
            string months = dt.Month.ToString();
            if (dt.Month < 10)
            {
                months = "0" + months;
            }
            string days = dt.Day.ToString();
            if (dt.Day < 10)
            {
                days = "0" + days;
            }


            sdt = years + "/" + months + "/" + days;

            string enddt = toDate_Ledger.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDate_Ledger.SelectedDate);
            int yeard = edt.Year;
            string monthd = edt.Month.ToString();
            if (edt.Month < 10)
            {
                monthd = "0" + monthd;
            }
            string dayd = edt.Day.ToString();
            if (edt.Day < 10)
            {
                dayd = "0" + dayd;
            }
            enddt = yeard + "/" + monthd + "/" + dayd;


            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select  * from Company where CompanyID =  " + CompID + "";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = con;
            SqlDataReader reader = cmd.ExecuteReader();

            //tmpProduct = new Product();
            string CompanyName = "";
            string GSTIN = "";
            string Address = "";
            string Address2 = "";
            string City = "";
            string State = "";
            string Mob = "";
            string Phone = "";
            string Email = "";
            string Web = "";
            string Branches = "";
            string LogoUrl = "";
            string SubTitle = "";
            string BankName = "";
            string BAddress = "";
            string IFSC = "";
            string AccNumber = "";
            string Holder = "";
            string PinCode = "";
            while (reader.Read())
            {

                //var CustID = reader.GetValue(0).ToString();
                CompanyName = (reader["CompanyName"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "";
                GSTIN = (reader["GSTIN"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                Address = (reader["Address1"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";
                Address2 = (reader["Address2"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                City = (reader["City"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";

                State = (reader["State"] != DBNull.Value) ? (reader.GetString(8).Trim()) : "";
                PinCode = (reader["PINCode"] != DBNull.Value) ? (reader.GetString(9).Trim()) : "";
                Mob = (reader["Mobile1"] != DBNull.Value) ? (reader.GetString(10).Trim()) : "";
                Phone = (reader["Phone"] != DBNull.Value) ? (reader.GetString(12).Trim()) : "";

                Email = (reader["Email"] != DBNull.Value) ? (reader.GetString(13).Trim()) : "";
                //FinYeraStartDate  = (reader["FinYearStartDate"] != DBNull.Value) ? (reader.GetString(17).Trim()) : "";
                //BookStartDate  = (reader["BookStartDate"] != DBNull.Value) ? (reader.GetString(18).Trim()) : "";
                Web = (reader["Website"] != DBNull.Value) ? (reader.GetString(15).Trim()) : "";
                Branches = (reader["NumberOfBranches"] != DBNull.Value) ? (reader.GetInt32(16)).ToString() : "";
                LogoUrl = (reader["LogoPath"] != DBNull.Value) ? (reader.GetString(26).Trim()) : "";
                SubTitle = (reader["SubTitle"] != DBNull.Value) ? (reader.GetString(25).Trim()) : "";

                BankName = (reader["BankName"] != DBNull.Value) ? (reader.GetString(20).Trim()) : "";
                BAddress = (reader["BAddress"] != DBNull.Value) ? (reader.GetString(21).Trim()) : "";
                IFSC = (reader["IFSC"] != DBNull.Value) ? (reader.GetString(22).Trim()) : "";
                AccNumber = (reader["AccNumber"] != DBNull.Value) ? (reader.GetString(23).Trim()) : "";
                Holder = (reader["Holder"] != DBNull.Value) ? (reader.GetString(24).Trim()) : "";


            }
            reader.Close();


            // create document and register styles
            FlowDocument doc = new FlowDocument();
            doc.ColumnWidth = 1024;
            doc.Name = "FlowDoc";
            doc.PageHeight = 900;
            doc.PageWidth = 800;
            doc.MinPageWidth = 800;

            Font colorHighlight = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.RED);
            /* style for products table header, assigned via type + class selectors */

            System.Windows.Documents.Table completeTable = new System.Windows.Documents.Table();

            TableRow rowoncompleteTable = new TableRow();
            ThicknessConverter tc1completeTable = new ThicknessConverter();
            //// Create Table Borders
            completeTable.BorderThickness = (Thickness)tc1completeTable.ConvertFromString("0.0001in");





            System.Windows.Documents.Table headertbl = new System.Windows.Documents.Table();

            System.Windows.Documents.Paragraph p = new System.Windows.Documents.Paragraph();

            Span s = new Span();

            s = new Span(new Run(CompanyName));
            s.FontWeight = FontWeights.ExtraBold;
            s.FontSize = 20;


            s.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a1 = new Span();
            a1 = new Span(new Run("GSTIN: " + GSTIN));
            a1.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a2 = new Span();
            a2 = new Span(new Run(Address + "," + Address2 + "," + City + "-" + PinCode + "," + State));
            a2.FontSize = 11;
            a2.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a3 = new Span();
            a3 = new Span(new Run("Account Statement"));
            a3.Inlines.Add(new LineBreak());//Line break is used for next line.  

            //Span a4 = new Span();
            //a4 = new Span(new Run("Invoice# " + invoiceNumber.Text));
            //a4.FontWeight = FontWeights.Bold;
            //a4.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a4acc = new Span();
            a4acc = new Span(new Run("M/S. " + autocompltCustName.autoTextBox.Text));
            a4acc.FontWeight = FontWeights.Bold;
            a4acc.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span a4date = new Span();
            a4date = new Span(new Run("Period: " + sdt + "-To- " + enddt));
            a4date.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a5 = new Span();
            a5 = new Span(new Run("---------------------------------------------------------------------------------------------------------"));
            //a5.Inlines.Add(new LineBreak());//Line break is used for next line.  
            p.FontSize = 12;
            p.Inlines.Add(a3);// Add the span content into paragraph.  
            p.Inlines.Add(s);// Add the span content into paragraph.  

            p.Inlines.Add(a2);// Add the span content into paragraph. 
            p.Inlines.Add(a1);// Add the span content into paragraph. 
            //p.Inlines.Add(a3);// Add the span content into paragraph.  

            //p.Inlines.Add(a4);// Add the span content into paragraph.  
            p.Inlines.Add(a4acc);// Add the span content into paragraph.  
            p.Inlines.Add(a4date);// Add the span content into paragraph.  
            //p.Inlines.Add(a5);// Add the span content into paragraph. 

            //If we have some dynamic text the span in flow document does not under "    " as space and we need to use "\t"  for space.  
            // s = new Span(new Run(s1 + "\t" + s2));//we need to use \t for space between s1 and s2 content.  
            //s.Inlines.Add(new LineBreak());
            //p.Inlines.Add(s);
            //Give style and formatting to paragraph content.  
            p.FontSize = 13;
            p.FontStyle = FontStyles.Normal;
            p.TextAlignment = TextAlignment.Center;
            p.FontFamily = new FontFamily("Century Gothic");
            p.BorderBrush = Brushes.Black;

            TableRow rowoneHeadertbl = new TableRow();
            ThicknessConverter tc1head = new ThicknessConverter();
            //// Create Table Borders
            headertbl.BorderThickness = (Thickness)tc1head.ConvertFromString("0.0000in");

            var rowgrpHeadertable = new TableRowGroup();

            ThicknessConverter tc22234 = new ThicknessConverter();
            // rowone.Background = Brushes.Silver;
            TableCell txtcellHeadtble12 = new TableCell(p);
            txtcellHeadtble12.BorderBrush = Brushes.Black;
            txtcellHeadtble12.BorderThickness = (Thickness)tc22234.ConvertFromString("0.0000in");
            rowoneHeadertbl.Cells.Add(txtcellHeadtble12);

            rowoneHeadertbl.FontSize = 11;
            rowoneHeadertbl.FontWeight = FontWeights.Regular;
            rowoneHeadertbl.FontFamily = new FontFamily("Century Gothic");
            //rowoneHeadertbl.Cells.Add(new TableCell(p));
            rowgrpHeadertable.Rows.Add(rowoneHeadertbl);
            headertbl.RowGroups.Add(rowgrpHeadertable);

            headertbl.Padding = new Thickness(0);

            //doc.Blocks.Add(p);

            System.Windows.Documents.Table t5 = new System.Windows.Documents.Table();

            t5.Padding = new Thickness(0);
            for (int i = 0; i < AccountLedgerGrid.Items.Count; i++)
            {
                //TableColumn tc = new TableColumn();

                t5.Columns.Add(new TableColumn() { Width = GridLength.Auto });

            }

            ThicknessConverter tc1 = new ThicknessConverter();
            //// Create Table Borders
            t5.BorderThickness = (Thickness)tc1.ConvertFromString("0.02in");

            int count1 = AccountLedgerGrid.Items.Count;
            var rg1 = new TableRowGroup();

            TableRow rowheadertable1 = new TableRow();



            rowheadertable1.Background = Brushes.Silver;
            rowheadertable1.FontSize = 8;
            rowheadertable1.FontFamily = new FontFamily("Century Gothic");
            rowheadertable1.FontWeight = FontWeights.Bold;

            ThicknessConverter tc222 = new ThicknessConverter();


            //TableCell tcellfirst = new TableCell(new System.Windows.Documents.Paragraph(new Run("VN")));
            ////tcellfirst.ColumnSpan = 3;
            //tcellfirst.BorderBrush = Brushes.Black;
            //tcellfirst.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcellfirst);

            //TableCell tcell2 = new TableCell(new System.Windows.Documents.Paragraph(new Run("HSN")));
            ////tcell2.ColumnSpan = 3;
            //tcell2.BorderBrush = Brushes.Black;
            //tcell2.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell2);
            TableCell tcell11 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Inv#")));
            //tcell11.ColumnSpan = 3;
            tcell11.BorderBrush = Brushes.Black;
            tcell11.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell11);

            TableCell tcell11122 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Dt")));
            //tcell11.ColumnSpan = 3;
            tcell11122.BorderBrush = Brushes.Black;
            tcell11122.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell11122);


            TableCell tcell3 = new TableCell(new System.Windows.Documents.Paragraph(new Run("V#")));
            //tcell3.ColumnSpan = 3;
            tcell3.BorderBrush = Brushes.Black;
            tcell3.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell3);

            TableCell tcell3212 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Type")));
            //tcell3.ColumnSpan = 3;
            tcell3212.BorderBrush = Brushes.Black;
            tcell3212.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell3212);

            TableCell tcell4 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Debtor")));
            tcell4.ColumnSpan = 3;
            tcell4.BorderBrush = Brushes.Black;
            tcell4.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell4);

            TableCell tcell5 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Creditor")));
            tcell5.ColumnSpan = 3;
            tcell5.BorderBrush = Brushes.Black;
            tcell5.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell5);

            TableCell tcell6 = new TableCell(new System.Windows.Documents.Paragraph(new Run("CR")));
            //tcell6.ColumnSpan = 3;
            tcell6.BorderBrush = Brushes.Black;
            tcell6.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell6);

            TableCell tcell7 = new TableCell(new System.Windows.Documents.Paragraph(new Run("DR")));
            //tcell7.ColumnSpan = 3;
            tcell7.BorderBrush = Brushes.Black;
            tcell7.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell7);

            TableCell tcell8 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Status")));
            //tcell8.ColumnSpan = 3;
            tcell8.BorderBrush = Brushes.Black;
            tcell8.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell8);

            TableCell tcell9 = new TableCell(new System.Windows.Documents.Paragraph(new Run("DueAmt")));
            //tcell9.ColumnSpan = 3;
            tcell9.BorderBrush = Brushes.Black;
            tcell9.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell9);

            TableCell tcell10cr = new TableCell(new System.Windows.Documents.Paragraph(new Run("DueDt")));
            //tcell10.ColumnSpan = 3;
            tcell10cr.BorderBrush = Brushes.Black;
            tcell10cr.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell10cr);

            TableCell tcell10duedt = new TableCell(new System.Windows.Documents.Paragraph(new Run("CrDays")));
            //tcell10.ColumnSpan = 3;
            tcell10duedt.BorderBrush = Brushes.Black;
            tcell10duedt.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell10duedt);



            TableCell tcell10 = new TableCell(new System.Windows.Documents.Paragraph(new Run("DueDays")));
            //tcell10.ColumnSpan = 3;
            tcell10.BorderBrush = Brushes.Black;
            tcell10.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell10);

            TableCell tcell10mod = new TableCell(new System.Windows.Documents.Paragraph(new Run("Mode")));
            //tcell10.ColumnSpan = 3;
            tcell10mod.BorderBrush = Brushes.Black;
            tcell10mod.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell10mod);

            TableCell tcell10Againt = new TableCell(new System.Windows.Documents.Paragraph(new Run("Against")));
            //tcell10.ColumnSpan = 3;
            tcell10Againt.BorderBrush = Brushes.Black;
            tcell10Againt.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell10Againt);

            TableCell tcell10Nar = new TableCell(new System.Windows.Documents.Paragraph(new Run("Narration")));
            //tcell10.ColumnSpan = 3;
            tcell10Nar.BorderBrush = Brushes.Black;
            tcell10Nar.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell10Nar);

            TableCell tcell10p = new TableCell(new System.Windows.Documents.Paragraph(new Run("LastPay")));
            //tcell10.ColumnSpan = 3;
            tcell10p.BorderBrush = Brushes.Black;
            tcell10p.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell10p);


            TableCell tcell10rmrks = new TableCell(new System.Windows.Documents.Paragraph(new Run("Remarks")));
            //tcell10.ColumnSpan = 3;
            tcell10rmrks.BorderBrush = Brushes.Black;
            tcell10rmrks.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell10rmrks);








            SqlConnection conpdfj = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conpdfj.Open();
            //string sqlpdf = "SELECT row_number() OVER (order by srnumber ) Sr ,DesignNumberPattern AS Style,[ItemName] As [Item Name]  ,[HSN],Small As S, Mediium As M, Large As L, XL, XL2, XL3,XL4,XL5,XL6 ,[BilledQty] As [Qty] ,[UnitID] As [UOM],[SalePrice] As [Price],Amount ,[Discount] As [Disc(%)] ,[TaxablelAmount] As [Taxable] ,[GSTRate] As [GST%] ,[TotalAmount] As [Total]   FROM [SalesVoucherInventorycloths] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "'";
            // string sqlpdfj = "SELECT [ItemName] As [ITEM NAME],[BilledQty] As [Qty] ,[BilledWt] As [Wt],WastePerc,[TotalBilledWt],MakingCharge,[SalePrice] As [Price],Amount,[Discount] As [Disc(%)],TaxablelAmount ,[GSTRate] As [GST%] ,[TotalAmount] As [TOTAL]   FROM [SalesVoucherInventoryByPc] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "' and ItemName not in ( 'Old Gold','Old Silver')";
            //SqlCommand cmdpdfj = new SqlCommand(sqlpdfj);
            SqlCommand cmdpdfj = new SqlCommand("GetAccountLedgerbyPeriod", conpdfj);
            cmdpdfj.CommandType = CommandType.StoredProcedure;
            cmdpdfj.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
            cmdpdfj.Parameters.Add(new SqlParameter("@StartDate", sdt));
            cmdpdfj.Parameters.Add(new SqlParameter("@EndDate", enddt));
            cmdpdfj.Parameters.Add(new SqlParameter("@CompID", CompID));
            cmdpdfj.Parameters.Add(new SqlParameter("@VchType", cmbVchType.Text.Trim()));
            SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);

            //cmdpdfj.Connection = conpdfj;
            //SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);
            DataTable dttablej = new DataTable("Inv");
            sda.Fill(dttablej);

            rg1.Rows.Add(rowheadertable1);

            IEnumerable itemsSource1 = AccountLedgerGrid.ItemsSource as IEnumerable;
            if (itemsSource1 != null)
            {
                // foreach (var item in itemsSource)
                for (int k = 0; k < dttablej.Rows.Count; ++k)
                {
                    TableRow rowone = new TableRow();

                    // rowone.Background = Brushes.Silver;
                    rowone.FontSize = 8;
                    rowone.FontWeight = FontWeights.Regular;
                    rowone.FontFamily = new FontFamily("Century Gothic");

                    for (int i = 0; i < dttablej.Columns.Count; ++i)
                    {

                        TableCell firstcolproductcell = new TableCell(new System.Windows.Documents.Paragraph(new Run(dttablej.Rows[k][i].ToString())));
                        if (i == 4 || i == 5)
                        {
                            firstcolproductcell.ColumnSpan = 3;
                        }
                        firstcolproductcell.BorderBrush = Brushes.Black;
                        firstcolproductcell.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
                        // rowone.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run((k + 1).ToString()))));
                        rowone.Cells.Add(firstcolproductcell);

                    }

                    rg1.Rows.Add(rowone);
                }
            }



            //----------------

            t5.CellSpacing = 0;


            t5.RowGroups.Add(rg1);
            //doc.Blocks.Add(t5);



            System.Windows.Documents.Paragraph totalValParag = new System.Windows.Documents.Paragraph();

            //Span ts = new Span();
            ////ts = new Span(new Run("\t" + " "+  lbTotalTax.Content+"    " + lbTotal.Content));

            //ts = new Span(new Run("\t" + lbTotal.Content));

            //ts.Inlines.Add(new LineBreak());//Line break is used for next line.  

            //Span cgsttax = new Span();
            //cgsttax = new Span(new Run("\t" + "                          " + lbTotalTax.Content));
            //cgsttax.Inlines.Add(new LineBreak());//Line break is used for next line.  

            totalValParag.TextAlignment = TextAlignment.Right;
            totalValParag.FontFamily = new FontFamily("Century Gothic");
            totalValParag.FontSize = 12;
            //totalValParag.Inlines.Add(ts);// Add the span content into paragraph.  
            //totalVal.Inlines.Add(cgsttax);// Add the span content into paragraph. 
            //totalVal.Inlines.Add(sgsttax);// Add the span content into paragraph. 

            //totalVal.Inlines.Add(ali5);// Add the span content into paragraph.  

            //doc.Blocks.Add(totalValParag);


            System.Windows.Documents.Table t4 = new System.Windows.Documents.Table();


            System.Windows.Documents.Paragraph totalVaGrand = new System.Windows.Documents.Paragraph();
            //totalValold.FontFamily 

            Span ts11gTotaoBeforeDisc = new Span();
            //if (totalValBeforeItemDis > 0)
            //{
            ts11gTotaoBeforeDisc = new Span(new Run("\t Total DR:" + "₹" + TotalDebitAmt_Ledger.Text + "       "));
            ts11gTotaoBeforeDisc.Inlines.Add(new LineBreak());//Line break is used for next line.  
            //}

            Span ts11gDiscAmountItemTotal = new Span();

            ts11gDiscAmountItemTotal = new Span(new Run("\t Total CR:" + "₹ " + TotalCreditAmt_Ledger.Text + "       "));
            ts11gDiscAmountItemTotal.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span tsTotalTaxableAmt = new Span();

            tsTotalTaxableAmt = new Span(new Run("\t Balance :" + "₹ " + Balance_Ledger.Text + "       "));
            tsTotalTaxableAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  



            totalVaGrand.FontSize = 14;
            totalVaGrand.FontFamily = new FontFamily("Century Gothic");
            totalVaGrand.Inlines.Add(ts11gTotaoBeforeDisc);// Add the span content into paragraph.  
            totalVaGrand.Inlines.Add(ts11gDiscAmountItemTotal);
            //totalVaGrand.Inlines.Add(tsMakingCharge);
            totalVaGrand.Inlines.Add(tsTotalTaxableAmt);


            //totalVal.Inlines.Add(ali5);// Add the span content into paragraph.  
            totalVaGrand.TextAlignment = TextAlignment.Right;

            totalVaGrand.FontWeight = FontWeights.Bold;
            //doc.Blocks.Add(totalVaGrand);

            TableRow rowtwocompleteTable = new TableRow();

            TableRow rowthreecompleteTable = new TableRow();

            //-------------
            System.Windows.Documents.Table colTableAdd = new System.Windows.Documents.Table();
            var rg1tb = new TableRowGroup();
            TableRow rowColCellheadertable = new TableRow();
            //rowColCellheadertable.Background = Brushes.Silver;
            rowColCellheadertable.FontSize = 12;
            rowColCellheadertable.FontFamily = new FontFamily("Century Gothic");
            rowColCellheadertable.FontWeight = FontWeights.Bold;

            ThicknessConverter tc222tbc = new ThicknessConverter();

            TableCell tcellfirstTb = new TableCell(new System.Windows.Documents.Paragraph(new Run("E&OE. \n *Subject to Chennai Jurisdiction \n * ")));

            tcellfirstTb.BorderBrush = Brushes.Black;
            tcellfirstTb.BorderThickness = (Thickness)tc222tbc.ConvertFromString("0.0000in");
            rowColCellheadertable.Cells.Add(tcellfirstTb);

            TableCell tcell2tb = new TableCell(totalVaGrand);
            //tcell2.ColumnSpan = 3;
            tcell2tb.BorderBrush = Brushes.Black;
            tcell2tb.BorderThickness = (Thickness)tc222tbc.ConvertFromString("0.0000in");
            rowColCellheadertable.Cells.Add(tcell2tb);

            rg1tb.Rows.Add(rowColCellheadertable);
            colTableAdd.RowGroups.Add(rg1tb);

            var rowgrpcompleteTable = new TableRowGroup();

            ThicknessConverter tc22234completeTable = new ThicknessConverter();
            // rowone.Background = Brushes.Silver;
            TableCell txtcellcompleteTable = new TableCell(t5);
            txtcellcompleteTable.BorderBrush = Brushes.Black;
            txtcellcompleteTable.BorderThickness = (Thickness)tc22234completeTable.ConvertFromString("0.0001in");

            TableCell txtcell2completeTable = new TableCell(headertbl);
            txtcell2completeTable.BorderBrush = Brushes.Black;
            txtcell2completeTable.BorderThickness = (Thickness)tc22234completeTable.ConvertFromString("0.0001in");

            TableCell txtcell3completeTable = new TableCell(colTableAdd);
            txtcell3completeTable.BorderBrush = Brushes.Black;
            txtcell3completeTable.BorderThickness = (Thickness)tc22234completeTable.ConvertFromString("0.0001in");



            //TableCell txtcell31completeTable = new TableCell(totalVaGrand);
            //txtcell31completeTable.BorderBrush = Brushes.Black;
            //txtcell31completeTable.BorderThickness = (Thickness)tc22234completeTable.ConvertFromString("0.0001in");

            TableCell txtcellOldcompleteTable = new TableCell(t4);
            txtcellOldcompleteTable.BorderBrush = Brushes.Black;
            txtcellOldcompleteTable.BorderThickness = (Thickness)tc22234completeTable.ConvertFromString("0.0001in");

            rowoncompleteTable.Cells.Add(txtcellcompleteTable);
            rowtwocompleteTable.Cells.Add(txtcell2completeTable);
            rowthreecompleteTable.Cells.Add(txtcell3completeTable);



            rowoncompleteTable.FontSize = 11;
            rowoncompleteTable.FontWeight = FontWeights.Regular;
            rowoncompleteTable.FontFamily = new FontFamily("Century Gothic");


            rowtwocompleteTable.FontSize = 11;
            rowtwocompleteTable.FontWeight = FontWeights.Regular;
            rowtwocompleteTable.FontFamily = new FontFamily("Century Gothic");

            //rowoneHeadertbl.Cells.Add(new TableCell(p));
            rowgrpcompleteTable.Rows.Add(rowtwocompleteTable);
            rowgrpcompleteTable.Rows.Add(rowoncompleteTable);



            rowgrpcompleteTable.Rows.Add(rowthreecompleteTable);

            completeTable.RowGroups.Add(rowgrpcompleteTable);

            completeTable.Padding = new Thickness(10);
            doc.Blocks.Add(completeTable);

            //doc.Blocks.Add(linedot);

            System.Windows.Documents.Paragraph signpara = new System.Windows.Documents.Paragraph();

            Span linebrktble1 = new Span();
            linebrktble1 = new Span(new Run("Signed By "));
            // linebrktble.Inlines.Add(new LineBreak());//Line break is used for next line.  

            signpara.FontSize = 13;

            signpara.Inlines.Add(linebrktble1);// Add the span content into paragraph.  
            signpara.TextAlignment = TextAlignment.Right;
            //linedot.Inlines.Add(linebrktble1);// Add the span content into paragraph.  
            //doc.Blocks.Add(linedot);
            doc.Blocks.Add(signpara);


            doc.Name = "FlowDoc";
            //doc.PageWidth = 900;
            doc.PagePadding = new Thickness(50, 30, 10, 5); //v3
            //doc.PagePadding = new Thickness(30, 20, 10, 5); //V2 
            // Create IDocumentPaginatorSource from FlowDocument
            // IDocumentPaginatorSource idpSource = doc;
            // Call PrintDocument method to send document to printer



            return doc;


        }

        private void AccountLedgerGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
            var uiElement = e.OriginalSource as UIElement;
            if (e.Key == Key.Enter && uiElement != null)
            {
                DataRowView row = (DataRowView)AccountLedgerGrid.SelectedItems[0];
                string invoiceV = row["Invoice"].ToString();
                string vouchertypeV = row["VchType"].ToString();
                string vouchernumbVa = row["VchNumber"].ToString();

                if (saleHomeIcon == "SaleVoucherJewellLatha" && vouchertypeV=="Sale Voucher")
                {
                    NavigationWindow navWIN = new NavigationWindow();
                    navWIN.Content = new SaleVoucherJewellLatha(invoiceV);
                    navWIN.Show();
                }

                if (saleHomeIcon == "SaleVoucherQtyGhansyam" && vouchertypeV == "Sale Voucher")
                {
                    NavigationWindow navWIN = new NavigationWindow();
                    navWIN.Content = new SaleVoucherQtyGhansyam(invoiceV);
                    navWIN.Show();
                }

                if (saleHomeIcon == "SaleVoucherAllInOneQtyGSTSteel" && vouchertypeV == "Sale Voucher")
                {
                    NavigationWindow navWIN = new NavigationWindow();
                    navWIN.Content = new SaleVoucherAllInOneQtyGSTSteel(invoiceV);
                    navWIN.Show();
                }
                if (purchaseHomeIcon == "PurchaseQtyGSTVoucherxaml" && vouchertypeV == "Purchase Voucher")
                {
                    NavigationWindow navWIN = new NavigationWindow();
                    navWIN.Content = new PurchaseQtyGSTVoucherxaml(vouchernumbVa);
                    navWIN.Show();
                }

                if (vouchertypeV == "Receipt Voucher")
                {
                    NavigationWindow navWIN = new NavigationWindow();
                    navWIN.Content = new Receipt(vouchernumbVa);
                    navWIN.Show();
                }

                if (vouchertypeV == "Payment Voucher")
                {
                    NavigationWindow navWIN = new NavigationWindow();
                    navWIN.Content = new Payment(vouchernumbVa);
                    navWIN.Show();
                }



                e.Handled = true;
                //uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            }


        }


    }
}
