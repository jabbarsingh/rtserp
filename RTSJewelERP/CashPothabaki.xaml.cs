using iTextSharp.text;
using RTSJewelERP.GroupListTableAdapters;
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
    public partial class CashPothabaki : Window
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


        private string startDateFinCurrentYr = "";
        private string endDateFinCurrentYr = "";



        public List<string> CountryList { get; set; }
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public CashPothabaki()
        {
            InitializeComponent();
            BindComboBoxTrayList(cmbTrayLists);
            BindComboBoxMainAccountType(cmbMainType);
            BindComboBox(cmbStates);
            BindComboBoxGroupName(GroupName);
            //itemnames = itemName;
            //companyId = CompID;
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window

            SqlConnection conFinyear = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conFinyear.Open();
            string sqlFinyear = "select * from FinancialYear where GETDATE() >= StartDate and GETDATE() <= EndDate ";
            SqlCommand cmdFinyear = new SqlCommand(sqlFinyear);
            cmdFinyear.Connection = conFinyear;
            SqlDataReader readerFinyear = cmdFinyear.ExecuteReader();

            //tmpProduct = new Product();

            while (readerFinyear.Read())
            {
                startDateFinCurrentYr = readerFinyear.GetDateTime(1).ToString();
                endDateFinCurrentYr = readerFinyear.GetDateTime(2).ToString();

            }
            readerFinyear.Close();



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

        public void BindComboBoxMainAccountType(ComboBox mainTypeAct)
        {
            var custAdpt = new MainAccountsTypeTableAdapter();
            var custInfoVal = custAdpt.GetData();
            var LinqRes = (from UserRec in custInfoVal
                           orderby UserRec.AcctName ascending
                           //select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
                           select (UserRec.AcctName.Trim())).Distinct();
            cmbMainType.ItemsSource = LinqRes;

            //var custAdpt = new TrayListInStorageByPcTableAdapter();
            //var custInfoVal = custAdpt.GetData();
            //if (custInfoVal != null)
            //{                
            //    cmbTrayLists.ItemsSource = custInfoVal.Where(c => (c.StorageName.Trim() == "Main")).Select(x => x.TrayName.Trim()).Distinct().ToList();
            //}
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
                cmbTrayLists.ItemsSource = custInfoVal.Where(c => (c.StorageName.Trim() == "Main")).Select(x => x.TrayName.Trim()).Distinct().ToList();
            }
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

            string sdt = startDate.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDate.SelectedDate);
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

            string enddt = toDate.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDate.SelectedDate);
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
                //select SUM(CAST(DR AS float)) As DebtAmount  from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' --and  TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
               // select SUM(CAST(CR AS float)) As CreditAmount  from PaymentVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' --and TransactionDate  <= '" + enddt + "' and TransactionDate >= '" + sdt + "'


                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"
                SqlCommand com = new SqlCommand("(SELECT  CONVERT(varchar, TransactionDate, 103) AS [Date] , LTRIM(RTRIM([VoucherNumber])) As VoucherNumber ,LTRIM(RTRIM([VoucherType])) As VoucherType,LTRIM(RTRIM([AcctName])) As AccountName,LTRIM(RTRIM([PayMode]))  As Mode,LTRIM(RTRIM([Remarks]))  As Remarks ,[CR] ,[DR] FROM [CashAccountsLedgers] where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "')", con);
                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(VoucherNumber)) As VoucherNumber ,LTRIM(RTRIM(VoucherType))  As VoucherType,LTRIM(RTRIM(DebtorAccountName)) As DebtorAccountName,LTRIM(RTRIM(CreditorAccountName)) As CreditorAccountName,CR As Amount,PayMode,Against,Narration,TransactionDate,CreationDate,UpdateDate,CreatedBy from ReceiptVouchers where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') Union ( select  LTRIM(RTRIM(VoucherNumber)) As VoucherNumber ,LTRIM(RTRIM(VoucherType))  As VoucherType,LTRIM(RTRIM(DebtorAccountName)) As DebtorAccountName,LTRIM(RTRIM(CreditorAccountName)) As CreditorAccountName,CR As Amount,PayMode,Against,Narration,TransactionDate,CreationDate,UpdateDate,CreatedBy from PaymentVouchers  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "')", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt2 = new System.Data.DataTable("Cash Flow");
                sda.Fill(dt2);
                ItemSaleGrid.ItemsSource = dt2.DefaultView;
                ItemSaleGrid.AutoGenerateColumns = true;
                ItemSaleGrid.CanUserAddRows = false;
            }


            using (SqlConnection con = new SqlConnection())
            {

               

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetCashAccountLedgerSummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                 com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                    com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                    com.Parameters.Add(new SqlParameter("@CompID", CompID));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {

                    double dDebtAcctLedgerAmt = (reader["DebtAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    double dCredAcctLedgerAmt = (reader["CredAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double opBalBookStart = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;

                    TotalDebitAmt.Text = dDebtAcctLedgerAmt.ToString();
                    TotalCreditAmt.Text = dCredAcctLedgerAmt.ToString();
                    Balance.Text = (dDebtAcctLedgerAmt - dCredAcctLedgerAmt).ToString();
                    openingBal.Text = opBal.ToString();
                    openingBalBookStart.Text = opBalBookStart.ToString();

                    closingBalEndDate.Text = (opBal + Convert.ToDouble(Balance.Text)).ToString();

                    //double dDebtAcctLedgerAmt = (reader["DebtAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    //double dPayVAmt = (reader["PayVAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    //double dCredAcctLedgerAmt = (reader["CredAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    //double dReceiptVAmt = (reader["ReceiptVAmt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    //double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    //TotalDebitAmt_Ledger.Text = debitamt.ToString();
                    //TotalCreditAmt_Ledger.Text = creditamt.ToString();
                    //Balance_Ledger.Text = (creditamt - debitamt).ToString();
                    //openingBal.Text = opBal.ToString();

                    //double debitamt = (reader["DebtAmount"] != DBNull.Value) ? (reader.GetDouble(0)): 0;
                    // double creditamt = (reader["CreditAmount"] != DBNull.Value) ? (reader.GetDouble(1)): 0;
                    // double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    //TotalDebitAmt.Text= debitamt.ToString();
                    //TotalCreditAmt.Text = creditamt.ToString();
                    //Balance.Text = (creditamt - debitamt).ToString();
                    //openingBal.Text = opBal.ToString();
                }

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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            openingBal.Clear();
            TotalCreditAmt.Clear();
            TotalDebitAmt.Clear();
            Balance.Clear();

            string sdt = startDate.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDate.SelectedDate);
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

            string enddt = toDate.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDate.SelectedDate);
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
                //select SUM(CAST(DR AS float)) As DebtAmount  from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' --and  TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
                // select SUM(CAST(CR AS float)) As CreditAmount  from PaymentVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' --and TransactionDate  <= '" + enddt + "' and TransactionDate >= '" + sdt + "'


                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"
                SqlCommand com = new SqlCommand("(SELECT CONVERT(varchar, TransactionDate, 103) AS [Date] , LTRIM(RTRIM([VoucherNumber])) As VoucherNumber ,LTRIM(RTRIM([VoucherType])) As VoucherType,LTRIM(RTRIM([AcctName])) As AccountName,LTRIM(RTRIM([PayMode]))  As Mode,LTRIM(RTRIM([Remarks]))  As Remarks ,[CR] ,[DR] FROM [CashAccountsLedgers] where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "')", con);
                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(VoucherNumber)) As VoucherNumber ,LTRIM(RTRIM(VoucherType))  As VoucherType,LTRIM(RTRIM(DebtorAccountName)) As DebtorAccountName,LTRIM(RTRIM(CreditorAccountName)) As CreditorAccountName,CR As Amount,PayMode,Against,Narration,TransactionDate,CreationDate,UpdateDate,CreatedBy from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' and CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') Union ( select  LTRIM(RTRIM(VoucherNumber)) As VoucherNumber ,LTRIM(RTRIM(VoucherType))  As VoucherType,LTRIM(RTRIM(DebtorAccountName)) As DebtorAccountName,LTRIM(RTRIM(CreditorAccountName)) As CreditorAccountName,CR As Amount,PayMode,Against,Narration,TransactionDate,CreationDate,UpdateDate,CreatedBy from PaymentVouchers  where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' and CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "')", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt1 = new System.Data.DataTable("Cash Flow");
                sda.Fill(dt1);
                ItemSaleGrid.ItemsSource = dt1.DefaultView;
                ItemSaleGrid.AutoGenerateColumns = true;
                ItemSaleGrid.CanUserAddRows = false;
            }

            using (SqlConnection con = new SqlConnection())
            {



                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetCashAccountLedgerSummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double dDebtAcctLedgerAmt = (reader["DebtAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    double dCredAcctLedgerAmt = (reader["CredAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double opBalBookStart = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;

                    TotalDebitAmt.Text = dDebtAcctLedgerAmt.ToString();
                    TotalCreditAmt.Text = dCredAcctLedgerAmt.ToString();
                    Balance.Text = (dDebtAcctLedgerAmt - dCredAcctLedgerAmt).ToString();
                    openingBal.Text = opBal.ToString();
                    openingBalBookStart.Text = opBalBookStart.ToString();


                    closingBalEndDate.Text = (opBal + Convert.ToDouble(Balance.Text)).ToString();

                }

            }

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
                    sumDr =  sumDr + ((row["DR"] != DBNull.Value) ? (Convert.ToDouble(row["DR"])) : 0);
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
            startDate_Ledger.Text = startDateFinCurrentYr; // set current fin startdate

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

        private void AccountLedgerGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            var uiElement = e.OriginalSource as UIElement;
            if (e.Key == Key.Enter && uiElement != null)
            {
                DataRowView row = (DataRowView)AccountLedgerGrid.SelectedItems[0];
                string invoiceV = row["Invoice"].ToString();
                string vouchertypeV = row["VchType"].ToString();
                string vouchernumbVa = row["VchNumber"].ToString();

                if (saleHomeIcon == "SaleVoucherJewellLatha" && vouchertypeV == "Sale Voucher")
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


        private void TabSale_Selected(object sender, RoutedEventArgs e)
        {
            autocompltCustNameSaleTab.autoTextBox.Text = "";
            string sdt = startDateSale.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateSale.SelectedDate);
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

            string enddt = toDateSale.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateSale.SelectedDate);
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

                SqlCommand com = new SqlCommand("GetSaleInvoiceList", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustNameSaleTab.autoTextBox.Text.Trim()));

                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt2 = new System.Data.DataTable("Sale Summary");
                sda.Fill(dt2);
                SaleSummaryGrid.ItemsSource = dt2.DefaultView;
                SaleSummaryGrid.AutoGenerateColumns = true;
                SaleSummaryGrid.CanUserAddRows = false;


                ////select SUM(CAST(DR AS float)) As DebtAmount  from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' --and  TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
                //// select SUM(CAST(CR AS float)) As CreditAmount  from PaymentVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' --and TransactionDate  <= '" + enddt + "' and TransactionDate >= '" + sdt + "'


                ////select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"

                //SqlCommand com = new SqlCommand("(select CONVERT(varchar, TransactionDate, 103) AS [Date] , LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number], LTRIM(RTRIM(InvoiceAmt)) As Amount, DueAmount As [Due Amount]  from SalesVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') order by  CAST(InvoiceNumber AS float) desc", con);
                //SqlDataAdapter sda = new SqlDataAdapter(com);
                //System.Data.DataTable dt2 = new System.Data.DataTable("Sale Summary");
                //sda.Fill(dt2);
                //SaleSummaryGrid.ItemsSource = dt2.DefaultView;
                //SaleSummaryGrid.AutoGenerateColumns = true;
                //SaleSummaryGrid.CanUserAddRows = false;
            }


            using (SqlConnection con = new SqlConnection())
            {



                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetSaleAccountummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@AcctName", (autocompltCustNameSaleTab.autoTextBox.Text.Trim())));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double totalInvAmount = (reader["TotalSaleAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    double totalInvDueAmount = (reader["TotalSaleDueAmt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    //double dPayVAmt = (reader["PayVAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    //double dCredAcctLedgerAmt = (reader["CredAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    //double dReceiptVAmt = (reader["ReceiptVAmt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    //double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    totalSale.Text = Math.Round(totalInvAmount,0).ToString();
                    totalSaleDueAMount.Text = Math.Round(totalInvDueAmount,0).ToString();


                }

            }


        }

        private void SButton_Click_SaleSummary(object sender, RoutedEventArgs e)
        {
            totalSale.Clear();
           
            string sdt = startDateSale.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateSale.SelectedDate);
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

            string enddt = toDateSale.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateSale.SelectedDate);
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


                SqlCommand com = new SqlCommand("GetSaleInvoiceList", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustNameSaleTab.autoTextBox.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt2 = new System.Data.DataTable("Sale Summary");
                sda.Fill(dt2);
                SaleSummaryGrid.ItemsSource = dt2.DefaultView;
                SaleSummaryGrid.AutoGenerateColumns = true;
                SaleSummaryGrid.CanUserAddRows = false;

                //string sqlselectQuerySale = "";
                //if (autocompltCustNameSaleTab.autoTextBox.Text.Trim() != "")
                //{
                //    sqlselectQuerySale = "(select CONVERT(varchar, TransactionDate, 103) AS [Date] , LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number], LTRIM(RTRIM(InvoiceAmt)) As Amount,  DueAmount As [Due Amount]  from SalesVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "' and AccountName = '" + autocompltCustNameSaleTab.autoTextBox.Text.Trim() + "') order by  CAST(InvoiceNumber AS float) desc";
                //    //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"
                //}
                //else
                //{
                //    sqlselectQuerySale = "(select CONVERT(varchar, TransactionDate, 103) AS [Date] ,  LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number], LTRIM(RTRIM(InvoiceAmt)) As Amount,  DueAmount As [Due Amount]  from SalesVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') order by  CAST(InvoiceNumber AS float) desc";
                //}

                //SqlCommand com = new SqlCommand(sqlselectQuerySale, con);
                //SqlDataAdapter sda = new SqlDataAdapter(com);
                //System.Data.DataTable dt2 = new System.Data.DataTable("Sale Summary");
                //sda.Fill(dt2);
                //SaleSummaryGrid.ItemsSource = dt2.DefaultView;
                //SaleSummaryGrid.AutoGenerateColumns = true;
                //SaleSummaryGrid.CanUserAddRows = false;
            }

            using (SqlConnection con = new SqlConnection())
            {



                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetSaleAccountummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@AcctName", (autocompltCustNameSaleTab.autoTextBox.Text.Trim())));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double totalInvAmount = (reader["TotalSaleAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    double totalInvDueAmount = (reader["TotalSaleDueAmt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    //double dPayVAmt = (reader["PayVAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    //double dCredAcctLedgerAmt = (reader["CredAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    //double dReceiptVAmt = (reader["ReceiptVAmt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    //double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    totalSale.Text = Math.Round(totalInvAmount,0).ToString();
                    totalSaleDueAMount.Text =  Math.Round(totalInvDueAmount,0).ToString();


                }

            }
        }

        private void TabPurchase_Selected(object sender, RoutedEventArgs e)
        {
            autocompltCustNamePurTab.autoTextBox.Text = "";

            string sdt = startDatePur.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDatePur.SelectedDate);
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

            string enddt = toDatePur.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDatePur.SelectedDate);
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
                //select SUM(CAST(DR AS float)) As DebtAmount  from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' --and  TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
                // select SUM(CAST(CR AS float)) As CreditAmount  from PaymentVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' --and TransactionDate  <= '" + enddt + "' and TransactionDate >= '" + sdt + "'


                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"

                SqlCommand com = new SqlCommand("(select CONVERT(varchar, TransactionDate, 103) AS [Date] , LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number],LTRIM(RTRIM(VoucherNumber)) As [Voucher Number], LTRIM(RTRIM(InvoiceAmt)) As Amount,  DueAmount As [Due Amount]  from PurchaseVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "')", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt2 = new System.Data.DataTable("Purchase Summary");
                sda.Fill(dt2);
                PurchaseSummaryGrid.ItemsSource = dt2.DefaultView;
                PurchaseSummaryGrid.AutoGenerateColumns = true;
                PurchaseSummaryGrid.CanUserAddRows = false;
            }


            using (SqlConnection con = new SqlConnection())
            {



                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetSaleAccountummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@AcctName", (autocompltCustNamePurTab.autoTextBox.Text.Trim())));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double totalInvAmount = (reader["TotalSaleAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    double totalInvAmountPur = (reader["TotalPurAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double totalInvDueAmountPur = (reader["TotalPurDueAmt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    //double dPayVAmt = (reader["PayVAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    //double dCredAcctLedgerAmt = (reader["CredAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    //double dReceiptVAmt = (reader["ReceiptVAmt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    //double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    totalPur.Text =  Math.Round(totalInvAmountPur,0).ToString();
                    totalPurDueAMount.Text =  Math.Round(totalInvDueAmountPur,0).ToString(); 

                }

            }


        }

        private void Button_Click_Purchummary(object sender, RoutedEventArgs e)
        {
            totalPur.Clear();


            string sdt = startDatePur.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDatePur.SelectedDate);
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

            string enddt = toDatePur.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDatePur.SelectedDate);
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
                //select SUM(CAST(DR AS float)) As DebtAmount  from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' --and  TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
                // select SUM(CAST(CR AS float)) As CreditAmount  from PaymentVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' --and TransactionDate  <= '" + enddt + "' and TransactionDate >= '" + sdt + "'


                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"
                //autocompltCustNamePurTab
                string sqlselectQuerySale = "";
                if (autocompltCustNamePurTab.autoTextBox.Text.Trim() != "")
                {
                    //sqlselectQuerySale = "(select LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number], LTRIM(RTRIM(InvoiceAmt)) As Amount, TransactionDate, DueAmount As [Due Amount]  from SalesVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "' and AccountName = '" + autocompltCustNameSaleTab.autoTextBox.Text.Trim() + "') order by  CAST(InvoiceNumber AS float) desc";
                    sqlselectQuerySale = "(select CONVERT(varchar, TransactionDate, 103) AS [Date] , LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number],LTRIM(RTRIM(VoucherNumber)) As [Voucher Number], LTRIM(RTRIM(InvoiceAmt)) As Amount,  DueAmount As [Due Amount]  from PurchaseVouchersOtherDetails   where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "' and AccountName = '" + autocompltCustNamePurTab.autoTextBox.Text.Trim() + "') order by  CAST(VoucherNumber AS float) desc";
                    //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"
                }
                else
                {
                    sqlselectQuerySale = "(select CONVERT(varchar, TransactionDate, 103) AS [Date] , LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number],LTRIM(RTRIM(VoucherNumber)) As [Voucher Number], LTRIM(RTRIM(InvoiceAmt)) As Amount, DueAmount As [Due Amount]  from PurchaseVouchersOtherDetails   where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "')";
                }

                SqlCommand com = new SqlCommand(sqlselectQuerySale, con);
                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number],LTRIM(RTRIM(VoucherNumber)) As [Voucher Number], LTRIM(RTRIM(InvoiceAmt)) As Amount, TransactionDate  , DueAmount As [Due Amount]  from PurchaseVouchersOtherDetails   where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "')", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt2 = new System.Data.DataTable("Purchase Summary");
                sda.Fill(dt2);
                PurchaseSummaryGrid.ItemsSource = dt2.DefaultView;
                PurchaseSummaryGrid.AutoGenerateColumns = true;
                PurchaseSummaryGrid.CanUserAddRows = false;
            }

            using (SqlConnection con = new SqlConnection())
            {



                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetSaleAccountummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@AcctName", (autocompltCustNamePurTab.autoTextBox.Text.Trim())));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double totalInvAmount = (reader["TotalSaleAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    double totalInvAmountPur = (reader["TotalPurAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double totalInvDueAmountPur = (reader["TotalPurDueAmt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    //double dCredAcctLedgerAmt = (reader["CredAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    //double dReceiptVAmt = (reader["ReceiptVAmt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    //double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    totalPur.Text =  Math.Round(totalInvAmountPur,0).ToString();
                    totalPurDueAMount.Text =  Math.Round(totalInvDueAmountPur,0).ToString();

                }

            }

        }

        private void TabStock_Selected(object sender, RoutedEventArgs e)
        {
            lblTotalSaleOuta.Content = "Total Sale ₹: ";
            lblTotalOldIns.Content = "Total Old ₹: ";

            lblTotalExchangeIns.Content = "Exchange ₹: ";

            lblGoldOutAmt.Content = "₹: ";
            lblSilverOutAmt.Content = "₹: ";
            lblOldGoldInAmt.Content = "₹: ";
            lblOldSilverInAmt.Content = "₹: ";

            goldIn.Clear();
            goldOut.Clear();
            oldGoldIn.Clear();
            oldSilverIn.Clear();
            oldSilverOut.Clear();
            oldGoldOut.Clear();
            silverIn.Clear();
            silverOut.Clear();
            goldInsada.Clear();
            silverInsada.Clear();
            goldOutsada.Clear();
            silverOutsada.Clear();


            string sdt = startDateStock.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateStock.SelectedDate);
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

            string enddt = toDateStock.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateStock.SelectedDate);
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
                //select SUM(CAST(DR AS float)) As DebtAmount  from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' --and  TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
                // select SUM(CAST(CR AS float)) As CreditAmount  from PaymentVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' --and TransactionDate  <= '" + enddt + "' and TransactionDate >= '" + sdt + "'


                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"

                SqlCommand com = new SqlCommand("SELECT CONVERT(varchar, TransactionDate, 103) AS [Date] ,Ltrim(rtrim(SVP.[InvoiceNumber])) As [Invoice Number] ,Ltrim(rtrim(SVP.[ItemName]))  As [Item Name],Ltrim(rtrim(SP.[ItemBarCode]))  As [Barcode],Ltrim(rtrim(SP.UnderGroupName)) As [Group] ,Ltrim(rtrim(SP.UnderSubGroupName))  As [Sub Group],[BilledQty] ,[BilledWt] ,[SalePrice],[BuyPrice] ,SVP.[GSTRate] ,[GSTTax] ,[Discount],[TaxablelAmount] ,[TotalAmount],[Labour],SVP.[MakingCharge],[WastePerc] ,[Wastage] ,[TotalBilledWt] ,[TransactionDate] FROM [SalesVoucherInventoryByPc] SVP inner join StockItemsByPc SP on  Ltrim(rtrim(SVP.ItemName)) =Ltrim(rtrim(SP.ItemName)) and  Ltrim(rtrim(SVP.ItemBarCode)) =Ltrim(rtrim(SP.ItemBarCode))  where SVP.CompID = '" + CompID + "' and SP.CompID = '" + CompID + "' and SVP.TransactionDate <= '" + enddt + "' and SVP.TransactionDate >= '" + sdt + "'  ORDER BY SP.UnderGroupName", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt2 = new System.Data.DataTable("Sale Summary");
                sda.Fill(dt2);
                StockSummaryGrid.ItemsSource = dt2.DefaultView;
                StockSummaryGrid.AutoGenerateColumns = true;
                StockSummaryGrid.CanUserAddRows = false;
            }

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();
                //select SUM(CAST(DR AS float)) As DebtAmount  from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' --and  TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
                // select SUM(CAST(CR AS float)) As CreditAmount  from PaymentVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' --and TransactionDate  <= '" + enddt + "' and TransactionDate >= '" + sdt + "'


                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"

                SqlCommand com = new SqlCommand("SELECT CONVERT(varchar, TransactionDate, 103) AS [Date] ,Ltrim(rtrim(SVP.[InvoiceNumber])) As [Invoice Number] ,Ltrim(rtrim(SVP.[ItemName]))  As [Item Name],Ltrim(rtrim(SP.[ItemBarCode]))  As [Barcode],Ltrim(rtrim(SP.UnderGroupName)) As [Group] ,Ltrim(rtrim(SP.UnderSubGroupName))  As [Sub Group],[BilledQty] ,[BilledWt] ,[SalePrice],[BuyPrice] ,SVP.[GSTRate] ,[GSTTax] ,[Discount],[TaxablelAmount] ,[TotalAmount],[Labour],SVP.[MakingCharge],[WastePerc] ,[Wastage] ,[TotalBilledWt] ,[TransactionDate] FROM [PurchaseVoucherInventory] SVP inner join StockItemsByPc SP on  Ltrim(rtrim(SVP.ItemName)) =Ltrim(rtrim(SP.ItemName))  where SVP.CompID = '" + CompID + "'  and SP.CompID = '" + CompID + "' and SVP.TransactionDate <= '" + enddt + "' and SVP.TransactionDate >= '" + sdt + "'  ORDER BY SP.UnderGroupName", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt2 = new System.Data.DataTable("Buy Summary");
                sda.Fill(dt2);
                StockSummaryPurGrid.ItemsSource = dt2.DefaultView;
                StockSummaryPurGrid.AutoGenerateColumns = true;
                StockSummaryPurGrid.CanUserAddRows = false;
            }


            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();
                //select SUM(CAST(DR AS float)) As DebtAmount  from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' --and  TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
                // select SUM(CAST(CR AS float)) As CreditAmount  from PaymentVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' --and TransactionDate  <= '" + enddt + "' and TransactionDate >= '" + sdt + "'


                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"

                SqlCommand com = new SqlCommand("SELECT CONVERT(varchar, TransactionDate, 103) AS [Date] ,Ltrim(rtrim(SVP.[InvoiceNumber])) As [Voucher Number] ,Ltrim(rtrim(SVP.[ItemName]))  As [Item Name],Ltrim(rtrim(SP.[ItemBarCode]))  As [Barcode],Ltrim(rtrim(SP.UnderGroupName)) As [Group] ,Ltrim(rtrim(SP.UnderSubGroupName))  As [Sub Group],[BilledQty] ,[BilledWt] ,[SalePrice],[BuyPrice] ,SVP.[GSTRate] ,[GSTTax] ,[Discount],[TaxablelAmount] ,[TotalAmount],[Labour],SVP.[MakingCharge],[WastePerc] ,[Wastage] ,[TotalBilledWt] ,[TransactionDate] FROM [OldPurchaseVoucherInventoryByPc] SVP inner join StockItemsByPc SP on  Ltrim(rtrim(SVP.ItemName)) =Ltrim(rtrim(SP.ItemName))  where SVP.CompID = '" + CompID + "'  and SP.CompID = '" + CompID + "' and SVP.TransactionDate <= '" + enddt + "' and SVP.TransactionDate >= '" + sdt + "'  ORDER BY SP.UnderGroupName", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt2 = new System.Data.DataTable("Buy Summary");
                sda.Fill(dt2);
                StockSummaryPurGridOld.ItemsSource = dt2.DefaultView;
                StockSummaryPurGridOld.AutoGenerateColumns = true;
                StockSummaryPurGridOld.CanUserAddRows = false;
            }

            double totalsaleamtpure = 0.0;
            double totalOldbuyamtpure = 0.0;
            double totalExchangebuyamtpure = 0.0;

            using (SqlConnection con = new SqlConnection())
            {


                //bool isOldGoldSeparateVoucher = false;

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetStockAccountummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    string voucherTypeName = (reader["VoucherType"] != DBNull.Value) ? (reader.GetString(0)) : "";
                    string grpName = (reader["GroupName"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "General";
                    double tptalWt = (reader["Wt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double tptalamt = (reader["Amt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    if (voucherTypeName.Trim() == "Sale Voucher")
                    {
                        if (grpName.Trim() == "Gold")
                        {
                            goldOut.Text = tptalWt.ToString();
                            lblGoldOutAmt.Content = "₹: " + tptalamt.ToString();
                            totalsaleamtpure = totalsaleamtpure + tptalamt;
                        }
                        if (grpName.Trim() == "Gold Sada")
                        {
                            goldOutsada.Text = tptalWt.ToString();
                            totalsaleamtpure = totalsaleamtpure + tptalamt;
                        }
                        if (grpName.Trim() == "Silver")
                        {
                            silverOut.Text = tptalWt.ToString();
                            lblSilverOutAmt.Content = "₹: " + tptalamt.ToString();
                            totalsaleamtpure = totalsaleamtpure + tptalamt;
                        }
                        if (grpName.Trim() == "Silver Sada")
                        {
                            silverOutsada.Text = tptalWt.ToString();
                            totalsaleamtpure = totalsaleamtpure + tptalamt;
                        }
                        if (grpName.Trim() == "Old Gold")
                        {
                            oldGoldIn.Text = tptalWt.ToString();
                            lblOldGoldInAmt.Content = "₹: " + tptalamt.ToString();
                            totalOldbuyamtpure = totalOldbuyamtpure + tptalamt;

                        }
                        if (grpName.Trim() == "Old Silver")
                        {
                            oldSilverIn.Text = tptalWt.ToString();
                            lblOldSilverInAmt.Content = "₹: " + tptalamt.ToString();
                            totalOldbuyamtpure = totalOldbuyamtpure + tptalamt;
                        }

                        if (grpName.Trim() == "Exchange Gold")
                        {
                            exchangeGoldIn.Text = tptalWt.ToString();
                            lblexchangeGoldInAmt.Content = "₹: " + tptalamt.ToString();
                            totalExchangebuyamtpure = totalExchangebuyamtpure + tptalamt;

                        }
                        if (grpName.Trim() == "Exchange Silver")
                        {
                            exchangeSilverIn.Text = tptalWt.ToString();
                            lblexchangeSilverInAmt.Content = "₹: " + tptalamt.ToString();
                            totalExchangebuyamtpure = totalExchangebuyamtpure + tptalamt;
                        }

                        if (grpName.Trim() == "General")
                        {
                           // oldSilverIn.Text = tptalWt.ToString();
                            //lblOldSilverInAmt.Content = "₹: " + tptalamt.ToString();
                            totalsaleamtpure = totalsaleamtpure + tptalamt;
                        }


                        if (grpName.Trim().ToUpper() == "WATCH")
                        {
                            // oldSilverIn.Text = tptalWt.ToString();
                            //lblOldSilverInAmt.Content = "₹: " + tptalamt.ToString();
                            totalsaleamtpure = totalsaleamtpure + tptalamt;
                        }

                    }



                    if (voucherTypeName.Trim() == "Purchase Voucher")
                    {
                        if (grpName.Trim() == "Gold")
                        {
                            goldIn.Text = tptalWt.ToString();
                        }
                        if (grpName.Trim() == "Gold Sada")
                        {
                            goldInsada.Text = tptalWt.ToString();
                        }
                        if (grpName.Trim() == "Silver")
                        {
                            silverIn.Text = tptalWt.ToString();
                        }
                        if (grpName.Trim() == "Silver Sada")
                        {
                            silverInsada.Text = tptalWt.ToString();
                        }
                        if (grpName.Trim() == "Old Gold")
                        {
                            oldGoldOut.Text = tptalWt.ToString();
                        }
                        if (grpName.Trim() == "Old Silver")
                        {
                            oldSilverOut.Text = tptalWt.ToString();
                        }
                    }
                }


                lblTotalSaleOuta.Content = "Total Sale ₹: " +  Math.Round(totalsaleamtpure,0).ToString();
                lblTotalOldIns.Content = "Total Old ₹: " +  Math.Round(totalOldbuyamtpure,0).ToString();
                lblTotalExchangeIns.Content = "Exchange ₹: " + Math.Round(totalExchangebuyamtpure, 0).ToString();
                lblTotalSaleActually.Content = "Actual Sale ₹: " + Math.Round(totalsaleamtpure - totalExchangebuyamtpure, 0).ToString();
            }



            //using (SqlConnection con = new SqlConnection())
            //{



            //    con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
            //    con.Open();

            //    SqlCommand com = new SqlCommand("GetStockAccountummarybyPeriodOldJewell", con);
            //    com.CommandType = CommandType.StoredProcedure;
            //    com.Parameters.Add(new SqlParameter("@StartDate", sdt));
            //    com.Parameters.Add(new SqlParameter("@EndDate", enddt));
            //    com.Parameters.Add(new SqlParameter("@CompID", CompID));
            //    SqlDataAdapter sda = new SqlDataAdapter(com);
            //    SqlDataReader reader = com.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        string voucherTypeName = (reader["VoucherType"] != DBNull.Value) ? (reader.GetString(0)) : "";
            //        string grpName = (reader["GroupName"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "General";
            //        double tptalWt = (reader["Wt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
            //        double tptalamt = (reader["Amt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;

            //        if (voucherTypeName.Trim() == "Old Purchase Voucher")
            //        {
            //            if (grpName.Trim() == "Old Gold")
            //            {
            //                oldGoldIn.Text = tptalWt.ToString();
            //                lblOldGoldInAmt.Content = "₹: " + tptalamt.ToString();
            //            }
            //            if (grpName.Trim() == "Old Silver")
            //            {
            //                oldSilverIn.Text = tptalWt.ToString();
            //                lblOldSilverInAmt.Content = "₹: " + tptalamt.ToString();
            //            }
            //        }
            //    }

            //}

//enable below code when GST OlD Entry done from separate voucher else keep it commented
            if (totalOldbuyamtpure.Equals(0.0))
            {
                using (SqlConnection con = new SqlConnection())
                {
                    double totaloldinvalueOldVoucher = 0.0;
                    con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                    con.Open();

                    SqlCommand com = new SqlCommand("GetStockAccountummarybyPeriodOldJewell", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                    com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                    com.Parameters.Add(new SqlParameter("@CompID", CompID));
                    SqlDataAdapter sda = new SqlDataAdapter(com);
                    SqlDataReader reader = com.ExecuteReader();
                    while (reader.Read())
                    {
                        string voucherTypeName = (reader["VoucherType"] != DBNull.Value) ? (reader.GetString(0)) : "";
                        string grpName = (reader["GroupName"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "General";
                        double tptalWt = (reader["Wt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                        double tptalamt = (reader["Amt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;

                        if (voucherTypeName.Trim() == "Old Purchase Voucher")
                        {
                            if (grpName.Trim() == "Old Gold")
                            {
                                oldGoldIn.Text = tptalWt.ToString();
                                lblOldGoldInAmt.Content = "₹: " + tptalamt.ToString();
                                totaloldinvalueOldVoucher = totaloldinvalueOldVoucher + tptalamt;
                            }
                            if (grpName.Trim() == "Old Silver")
                            {
                                oldSilverIn.Text = tptalWt.ToString();
                                lblOldSilverInAmt.Content = "₹: " + tptalamt.ToString();
                                totaloldinvalueOldVoucher = totaloldinvalueOldVoucher + tptalamt;
                            }
                        }
                    }


                    lblTotalOldIns.Content = "Total Old ₹: " + totaloldinvalueOldVoucher.ToString();
                }

            }

        }

        private void Button_Click_StockSummary(object sender, RoutedEventArgs e)
        {
            lblTotalSaleOuta.Content = "Total Sale ₹: ";
            lblTotalOldIns.Content = "Total Old ₹: "; 
            lblTotalExchangeIns.Content = "Exchange ₹: ";
            lblGoldOutAmt.Content = "₹: ";
            lblSilverOutAmt.Content = "₹: ";
            lblOldGoldInAmt.Content = "₹: ";
            lblOldSilverInAmt.Content = "₹: ";

            goldIn.Clear();
            goldOut.Clear();
            oldGoldIn.Clear();
            oldSilverIn.Clear();
            oldSilverOut.Clear();
            oldGoldOut.Clear();
            silverIn.Clear();
            silverOut.Clear();
            goldInsada.Clear();
            silverInsada.Clear();
            goldOutsada.Clear();
            silverOutsada.Clear();


            string sdt = startDateStock.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateStock.SelectedDate);
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

            string enddt = toDateStock.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateStock.SelectedDate);
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
                //select SUM(CAST(DR AS float)) As DebtAmount  from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' --and  TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
                // select SUM(CAST(CR AS float)) As CreditAmount  from PaymentVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' --and TransactionDate  <= '" + enddt + "' and TransactionDate >= '" + sdt + "'


                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"

                SqlCommand com = new SqlCommand("SELECT CONVERT(varchar, TransactionDate, 103) AS [Date] , Ltrim(rtrim(SVP.[InvoiceNumber])) As [Invoice Number] ,Ltrim(rtrim(SVP.[ItemName]))  As [Item Name],Ltrim(rtrim(SP.[ItemBarCode]))  As [Barcode],Ltrim(rtrim(SP.UnderGroupName)) As [Group] ,Ltrim(rtrim(SP.UnderSubGroupName))  As [Sub Group],[BilledQty] ,[BilledWt] ,[SalePrice],[BuyPrice] ,SVP.[GSTRate] ,[GSTTax] ,[Discount],[TaxablelAmount] ,[TotalAmount],[Labour],SVP.[MakingCharge],[WastePerc] ,[Wastage] ,[TotalBilledWt] ,[TransactionDate] FROM [SalesVoucherInventoryByPc] SVP inner join StockItemsByPc SP on  Ltrim(rtrim(SVP.ItemName)) =Ltrim(rtrim(SP.ItemName)) and  Ltrim(rtrim(SVP.ItemBarCode)) =Ltrim(rtrim(SP.ItemBarCode))  where SVP.CompID = '" + CompID + "' and SP.CompID = '" + CompID + "' and SVP.TransactionDate <= '" + enddt + "' and SVP.TransactionDate >= '" + sdt + "'  ORDER BY SP.UnderGroupName", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt2 = new System.Data.DataTable("Sale Summary");
                sda.Fill(dt2);
                StockSummaryGrid.ItemsSource = dt2.DefaultView;
                StockSummaryGrid.AutoGenerateColumns = true;
                StockSummaryGrid.CanUserAddRows = false;
            }

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();
                //select SUM(CAST(DR AS float)) As DebtAmount  from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' --and  TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
                // select SUM(CAST(CR AS float)) As CreditAmount  from PaymentVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' --and TransactionDate  <= '" + enddt + "' and TransactionDate >= '" + sdt + "'


                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"

                SqlCommand com = new SqlCommand("SELECT CONVERT(varchar, TransactionDate, 103) AS [Date] , Ltrim(rtrim(SVP.[InvoiceNumber])) As [Invoice Number] ,Ltrim(rtrim(SVP.[ItemName]))  As [Item Name],Ltrim(rtrim(SP.[ItemBarCode]))  As [Barcode],Ltrim(rtrim(SP.UnderGroupName)) As [Group] ,Ltrim(rtrim(SP.UnderSubGroupName))  As [Sub Group],[BilledQty] ,[BilledWt] ,[SalePrice],[BuyPrice] ,SVP.[GSTRate] ,[GSTTax] ,[Discount],[TaxablelAmount] ,[TotalAmount],[Labour],SVP.[MakingCharge],[WastePerc] ,[Wastage] ,[TotalBilledWt] ,[TransactionDate] FROM [PurchaseVoucherInventory] SVP inner join StockItemsByPc SP on  Ltrim(rtrim(SVP.ItemName)) =Ltrim(rtrim(SP.ItemName))  where SVP.CompID = '" + CompID + "'  and SP.CompID = '" + CompID + "' and SVP.TransactionDate <= '" + enddt + "' and SVP.TransactionDate >= '" + sdt + "'  ORDER BY SP.UnderGroupName", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt2 = new System.Data.DataTable("Buy Summary");
                sda.Fill(dt2);
                StockSummaryPurGrid.ItemsSource = dt2.DefaultView;
                StockSummaryPurGrid.AutoGenerateColumns = true;
                StockSummaryPurGrid.CanUserAddRows = false;
            }


            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();
                //select SUM(CAST(DR AS float)) As DebtAmount  from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' --and  TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
                // select SUM(CAST(CR AS float)) As CreditAmount  from PaymentVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' --and TransactionDate  <= '" + enddt + "' and TransactionDate >= '" + sdt + "'


                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"

                SqlCommand com = new SqlCommand("SELECT CONVERT(varchar, TransactionDate, 103) AS [Date] ,Ltrim(rtrim(SVP.[InvoiceNumber])) As [Voucher Number] ,Ltrim(rtrim(SVP.[ItemName]))  As [Item Name],Ltrim(rtrim(SP.[ItemBarCode]))  As [Barcode],Ltrim(rtrim(SP.UnderGroupName)) As [Group] ,Ltrim(rtrim(SP.UnderSubGroupName))  As [Sub Group],[BilledQty] ,[BilledWt] ,[SalePrice],[BuyPrice] ,SVP.[GSTRate] ,[GSTTax] ,[Discount],[TaxablelAmount] ,[TotalAmount],[Labour],SVP.[MakingCharge],[WastePerc] ,[Wastage] ,[TotalBilledWt] ,[TransactionDate] FROM [OldPurchaseVoucherInventoryByPc] SVP inner join StockItemsByPc SP on  Ltrim(rtrim(SVP.ItemName)) =Ltrim(rtrim(SP.ItemName))  where SVP.CompID = '" + CompID + "'  and SP.CompID = '" + CompID + "' and SVP.TransactionDate <= '" + enddt + "' and SVP.TransactionDate >= '" + sdt + "'  ORDER BY SP.UnderGroupName", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt2 = new System.Data.DataTable("Buy Summary");
                sda.Fill(dt2);
                StockSummaryPurGridOld.ItemsSource = dt2.DefaultView;
                StockSummaryPurGridOld.AutoGenerateColumns = true;
                StockSummaryPurGridOld.CanUserAddRows = false;
            }





            double totalsaleamtpure = 0.0;
            double totalOldbuyamtpure = 0.0;
            double totalExchangebuyamtpure = 0.0;

            using (SqlConnection con = new SqlConnection())
            {



                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetStockAccountummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    string voucherTypeName = (reader["VoucherType"] != DBNull.Value) ? (reader.GetString(0)) : "";
                    string grpName = (reader["GroupName"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "General";
                    double tptalWt = (reader["Wt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double tptalamt = (reader["Amt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    //if (voucherTypeName.Trim() == "Sale Voucher")
                    //{
                    //    if (grpName.Trim() == "Gold")
                    //    {
                    //        goldOut.Text = tptalWt.ToString();
                    //        lblGoldOutAmt.Content = "₹: " + tptalamt.ToString();
                    //    }
                    //    if (grpName.Trim() == "Gold Sada")
                    //    {
                    //        goldOutsada.Text = tptalWt.ToString();
                    //    }
                    //    if (grpName.Trim() == "Silver")
                    //    {
                    //        silverOut.Text = tptalWt.ToString();
                    //        lblSilverOutAmt.Content = "₹: " + tptalamt.ToString();
                    //    }
                    //    if (grpName.Trim() == "Silver Sada")
                    //    {
                    //        silverOutsada.Text = tptalWt.ToString();
                    //    }
                    //    if (grpName.Trim() == "Old Gold")
                    //    {
                    //        oldGoldIn.Text = tptalWt.ToString();
                    //        lblOldGoldInAmt.Content = "₹: " + tptalamt.ToString();
                    //    }
                    //    if (grpName.Trim() == "Old Silver")
                    //    {
                    //        oldSilverIn.Text = tptalWt.ToString();
                    //        lblOldSilverInAmt.Content = "₹: " + tptalamt.ToString();
                    //    }
                    //}

                    if (voucherTypeName.Trim() == "Sale Voucher")
                    {
                        if (grpName.Trim() == "Gold")
                        {
                            goldOut.Text = tptalWt.ToString();
                            lblGoldOutAmt.Content = "₹: " + tptalamt.ToString();
                            totalsaleamtpure = totalsaleamtpure + tptalamt;
                        }
                        if (grpName.Trim() == "Gold Sada")
                        {
                            goldOutsada.Text = tptalWt.ToString();
                            totalsaleamtpure = totalsaleamtpure + tptalamt;
                        }
                        if (grpName.Trim() == "Silver")
                        {
                            silverOut.Text = tptalWt.ToString();
                            lblSilverOutAmt.Content = "₹: " + tptalamt.ToString();
                            totalsaleamtpure = totalsaleamtpure + tptalamt;
                        }
                        if (grpName.Trim() == "Silver Sada")
                        {
                            silverOutsada.Text = tptalWt.ToString();
                            totalsaleamtpure = totalsaleamtpure + tptalamt;
                        }
                        if (grpName.Trim() == "Old Gold")
                        {
                            oldGoldIn.Text = tptalWt.ToString();
                            lblOldGoldInAmt.Content = "₹: " + tptalamt.ToString();
                            totalOldbuyamtpure = totalOldbuyamtpure + tptalamt;

                        }
                        if (grpName.Trim() == "Old Silver")
                        {
                            oldSilverIn.Text = tptalWt.ToString();
                            lblOldSilverInAmt.Content = "₹: " + tptalamt.ToString();
                            totalOldbuyamtpure = totalOldbuyamtpure + tptalamt;
                        }

                        if (grpName.Trim() == "Exchange Gold")
                        {
                            exchangeGoldIn.Text = tptalWt.ToString();
                            lblexchangeGoldInAmt.Content = "₹: " + tptalamt.ToString();
                            totalExchangebuyamtpure = totalExchangebuyamtpure + tptalamt;

                        }
                        if (grpName.Trim() == "Exchange Silver")
                        {
                            exchangeSilverIn.Text = tptalWt.ToString();
                            lblexchangeSilverInAmt.Content = "₹: " + tptalamt.ToString();
                            totalExchangebuyamtpure = totalExchangebuyamtpure + tptalamt;
                        }


                        if (grpName.Trim() == "General")
                        {
                            // oldSilverIn.Text = tptalWt.ToString();
                            //lblOldSilverInAmt.Content = "₹: " + tptalamt.ToString();
                            totalsaleamtpure = totalsaleamtpure + tptalamt;
                        }


                        if (grpName.Trim().ToUpper() == "WATCH")
                        {
                            // oldSilverIn.Text = tptalWt.ToString();
                            //lblOldSilverInAmt.Content = "₹: " + tptalamt.ToString();
                            totalsaleamtpure = totalsaleamtpure + tptalamt;
                        }

                    }




                    if (voucherTypeName.Trim() == "Purchase Voucher")
                    {
                        if (grpName.Trim() == "Gold")
                        {
                            goldIn.Text = tptalWt.ToString();
                        }
                        if (grpName.Trim() == "Gold Sada")
                        {
                            goldInsada.Text = tptalWt.ToString();
                        }
                        if (grpName.Trim() == "Silver")
                        {
                            silverIn.Text = tptalWt.ToString();
                        }
                        if (grpName.Trim() == "Silver Sada")
                        {
                            silverInsada.Text = tptalWt.ToString();
                        }
                        if (grpName.Trim() == "Old Gold")
                        {
                            oldGoldOut.Text = tptalWt.ToString();
                        }
                        if (grpName.Trim() == "Old Silver")
                        {
                            oldSilverOut.Text = tptalWt.ToString();
                        }
                    }                    
                                       
                }

                lblTotalSaleOuta.Content = "Total Sale ₹: " +  Math.Round(totalsaleamtpure,0).ToString();
                lblTotalOldIns.Content = "Total Old ₹: " +  Math.Round(totalOldbuyamtpure,0).ToString();
                lblTotalExchangeIns.Content = "Exchange ₹: " + Math.Round(totalExchangebuyamtpure, 0).ToString();
                lblTotalSaleActually.Content = "Actual Sale ₹: " + Math.Round(totalsaleamtpure - totalExchangebuyamtpure, 0).ToString();
                


            }

            //enable below code when GST OlD Entry done from separate voucher else keep it commented
            if (totalOldbuyamtpure.Equals(0.0))
            {
                using (SqlConnection con = new SqlConnection())
                {
                    double totaloldinvalueOldVoucher = 0.0;
                    con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                    con.Open();

                    SqlCommand com = new SqlCommand("GetStockAccountummarybyPeriodOldJewell", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                    com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                    com.Parameters.Add(new SqlParameter("@CompID", CompID));
                    SqlDataAdapter sda = new SqlDataAdapter(com);
                    SqlDataReader reader = com.ExecuteReader();
                    while (reader.Read())
                    {
                        string voucherTypeName = (reader["VoucherType"] != DBNull.Value) ? (reader.GetString(0)) : "";
                        string grpName = (reader["GroupName"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "General";
                        double tptalWt = (reader["Wt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                        double tptalamt = (reader["Amt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;

                        if (voucherTypeName.Trim() == "Old Purchase Voucher")
                        {
                            if (grpName.Trim() == "Old Gold")
                            {
                                oldGoldIn.Text = tptalWt.ToString();
                                lblOldGoldInAmt.Content = "₹: " + tptalamt.ToString();
                                totaloldinvalueOldVoucher = totaloldinvalueOldVoucher + tptalamt;
                            }
                            if (grpName.Trim() == "Old Silver")
                            {
                                oldSilverIn.Text = tptalWt.ToString();
                                lblOldSilverInAmt.Content = "₹: " + tptalamt.ToString();
                                totaloldinvalueOldVoucher = totaloldinvalueOldVoucher + tptalamt;
                            }
                        }
                    }


                    lblTotalOldIns.Content = "Total Old ₹: " + totaloldinvalueOldVoucher.ToString();
                }

            }



            //using (SqlConnection con = new SqlConnection())
            //{


            //    double totaloldinvalueOldVoucher = 0.0;
            //    con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
            //    con.Open();

            //    SqlCommand com = new SqlCommand("GetStockAccountummarybyPeriodOldJewell", con);
            //    com.CommandType = CommandType.StoredProcedure;
            //    com.Parameters.Add(new SqlParameter("@StartDate", sdt));
            //    com.Parameters.Add(new SqlParameter("@EndDate", enddt));
            //    com.Parameters.Add(new SqlParameter("@CompID", CompID));
            //    SqlDataAdapter sda = new SqlDataAdapter(com);
            //    SqlDataReader reader = com.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        string voucherTypeName = (reader["VoucherType"] != DBNull.Value) ? (reader.GetString(0)) : "";
            //        string grpName = (reader["GroupName"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "General";
            //        double tptalWt = (reader["Wt"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
            //        double tptalamt = (reader["Amt"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;

            //        if (voucherTypeName.Trim() == "Old Purchase Voucher")
            //        {
            //            if (grpName.Trim() == "Old Gold")
            //            {
            //                oldGoldIn.Text = tptalWt.ToString();
            //                lblOldGoldInAmt.Content = "₹: " + tptalamt.ToString();
            //                totaloldinvalueOldVoucher = totaloldinvalueOldVoucher + tptalamt;
            //            }
            //            if (grpName.Trim() == "Old Silver")
            //            {
            //                oldSilverIn.Text = tptalWt.ToString();
            //                lblOldSilverInAmt.Content = "₹: " + tptalamt.ToString();
            //                totaloldinvalueOldVoucher = totaloldinvalueOldVoucher + tptalamt;
            //            }
            //        }
            //    }


            //    lblTotalOldIns.Content = "Total Old ₹: " + totaloldinvalueOldVoucher.ToString();
            //}


        }

        private void printSaleStockSummaryLedger_Click(object sender, RoutedEventArgs e)
        {
            string sdt = startDateStock.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateStock.SelectedDate);
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

            string enddt = toDateStock.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateStock.SelectedDate);
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
           


          


            PrintDialog printDlg = new PrintDialog();

            printDlg.PrintQueue = System.Printing.LocalPrintServer.GetDefaultPrintQueue();
            printDlg.PrintTicket = printDlg.PrintQueue.DefaultPrintTicket;
            printDlg.PrintTicket.PageOrientation = PageOrientation.Portrait;

            // Create a FlowDocument dynamically.
            //FlowDocument doc = CreateFlowDocumentJewellery();
            FlowDocument doc = CreateFlowDocumentSaleStockSummary();
            doc.ColumnWidth = 600;
            doc.Name = "FlowDoc";
            doc.PageHeight = 1000;
            doc.PageWidth = 800;
            doc.MinPageWidth = 800;


            // Create IDocumentPaginatorSource from FlowDocument
            IDocumentPaginatorSource idpSource = doc;

            // Call PrintDocument method to send document to printer
            //Uncomment for Print
            printDlg.PrintDocument(idpSource.DocumentPaginator, "Receipt Printing.");
        }

        private FlowDocument CreateFlowDocumentSaleStockSummary()
        {
            //  Get Confirmation that data saved successfull, 

            string sdt = startDateStock.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateStock.SelectedDate);
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

            string enddt = toDateStock.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateStock.SelectedDate);
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

            string sdateIndFormat = days + "/" + months + "/" + years;
            string enddateIndFormat = dayd + "/" + monthd + "/" + yeard;

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
            doc.PageHeight = 1000;
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
            a3 = new Span(new Run("Sale Out Report"));
            a3.FontWeight = FontWeights.Bold;
            a3.Inlines.Add(new LineBreak());//Line break is used for next line.  

            //Span a4 = new Span();
            //a4 = new Span(new Run("Invoice# " + invoiceNumber.Text));
            //a4.FontWeight = FontWeights.Bold;
            //a4.Inlines.Add(new LineBreak());//Line break is used for next line.  

            //Span a4acc = new Span();
            //a4acc = new Span(new Run("M/S. " + autocompltCustName.autoTextBox.Text));
            ////a4acc.FontWeight = FontWeights.Bold;
            //a4acc.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span a4date = new Span();
            a4date = new Span(new Run("Period: " + sdateIndFormat + "-To- " + enddateIndFormat));
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
            //p.Inlines.Add(a4acc);// Add the span content into paragraph.  
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
            for (int i = 0; i < StockSummaryGrid.Items.Count; i++)
            {
                //TableColumn tc = new TableColumn();

                t5.Columns.Add(new TableColumn() { Width = GridLength.Auto });

            }

            ThicknessConverter tc1 = new ThicknessConverter();
            //// Create Table Borders
            t5.BorderThickness = (Thickness)tc1.ConvertFromString("0.02in");

            int count1 = StockSummaryGrid.Items.Count;
            var rg1 = new TableRowGroup();

            TableRow rowheadertable1 = new TableRow();



            rowheadertable1.Background = Brushes.Silver;
            rowheadertable1.FontSize = 9;
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

            TableCell tcell3 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Invoice")));
            //tcell3.ColumnSpan = 3;
            tcell3.BorderBrush = Brushes.Black;
            tcell3.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell3);

            TableCell tcell4 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Item")));
            tcell4.ColumnSpan = 2;
            tcell4.BorderBrush = Brushes.Black;
            tcell4.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell4);

            TableCell tcell5 = new TableCell(new System.Windows.Documents.Paragraph(new Run("BarCode")));
            //tcell5.ColumnSpan = 3;
            tcell5.BorderBrush = Brushes.Black;
            tcell5.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell5);

            TableCell tcell6 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Group")));
            //tcell6.ColumnSpan = 3;
            tcell6.BorderBrush = Brushes.Black;
            tcell6.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell6);

            TableCell tcell7 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Qty")));
            //tcell7.ColumnSpan = 3;
            tcell7.BorderBrush = Brushes.Black;
            tcell7.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell7);

            TableCell tcell8 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Wt")));
            //tcell8.ColumnSpan = 3;
            tcell8.BorderBrush = Brushes.Black;
            tcell8.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell8);

            TableCell tcell9 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Waste%")));
            //tcell9.ColumnSpan = 3;
            tcell9.BorderBrush = Brushes.Black;
            tcell9.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell9);

            TableCell tcell10 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Waste")));
            //tcell10.ColumnSpan = 3;
            tcell10.BorderBrush = Brushes.Black;
            tcell10.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell10);

            TableCell tcell11 = new TableCell(new System.Windows.Documents.Paragraph(new Run("GrossWt")));
            //tcell11.ColumnSpan = 3;
            tcell11.BorderBrush = Brushes.Black;
            tcell11.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell11);

            TableCell tcell12 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Price")));
            //tcell11.ColumnSpan = 3;
            tcell12.BorderBrush = Brushes.Black;
            tcell12.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell12);

            TableCell tcell13 = new TableCell(new System.Windows.Documents.Paragraph(new Run("GST%")));
            //tcell11.ColumnSpan = 3;
            tcell13.BorderBrush = Brushes.Black;
            tcell13.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell13);

            TableCell tcell14 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Tax")));
            //tcell11.ColumnSpan = 3;
            tcell14.BorderBrush = Brushes.Black;
            tcell14.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell14);

            TableCell tcell15 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Dis")));
            //tcell11.ColumnSpan = 3;
            tcell15.BorderBrush = Brushes.Black;
            tcell15.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell15);

            TableCell tcell16 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Taxable")));
            //tcell11.ColumnSpan = 3;
            tcell16.BorderBrush = Brushes.Black;
            tcell16.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell16);

            TableCell tcell17 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Total")));
            //tcell11.ColumnSpan = 3;
            tcell17.BorderBrush = Brushes.Black;
            tcell17.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell17);


            //TableCell tcell18 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Labour")));
            ////tcell11.ColumnSpan = 3;
            //tcell18.BorderBrush = Brushes.Black;
            //tcell18.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell18);

            TableCell tcell19 = new TableCell(new System.Windows.Documents.Paragraph(new Run("MC")));
            //tcell11.ColumnSpan = 3;
            tcell19.BorderBrush = Brushes.Black;
            tcell19.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell19);

            TableCell tcell20 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Date")));
            //tcell11.ColumnSpan = 3;
            tcell20.BorderBrush = Brushes.Black;
            tcell20.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell20);



            SqlConnection conpdfj = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conpdfj.Open();
            //string sqlpdf = "SELECT row_number() OVER (order by srnumber ) Sr ,DesignNumberPattern AS Style,[ItemName] As [Item Name]  ,[HSN],Small As S, Mediium As M, Large As L, XL, XL2, XL3,XL4,XL5,XL6 ,[BilledQty] As [Qty] ,[UnitID] As [UOM],[SalePrice] As [Price],Amount ,[Discount] As [Disc(%)] ,[TaxablelAmount] As [Taxable] ,[GSTRate] As [GST%] ,[TotalAmount] As [Total]   FROM [SalesVoucherInventorycloths] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "'";
            // string sqlpdfj = "SELECT [ItemName] As [ITEM NAME],[BilledQty] As [Qty] ,[BilledWt] As [Wt],WastePerc,[TotalBilledWt],MakingCharge,[SalePrice] As [Price],Amount,[Discount] As [Disc(%)],TaxablelAmount ,[GSTRate] As [GST%] ,[TotalAmount] As [TOTAL]   FROM [SalesVoucherInventoryByPc] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "' and ItemName not in ( 'Old Gold','Old Silver')";
            //SqlCommand cmdpdfj = new SqlCommand(sqlpdfj);

            SqlCommand cmdpdfj = new SqlCommand("SELECT Ltrim(rtrim(SVP.[InvoiceNumber])) As [Invoice Number] ,Ltrim(rtrim(SVP.[ItemName]))  As [Item Name],Ltrim(rtrim(SP.[ItemBarCode]))  As [Barcode],Ltrim(rtrim(SP.UnderGroupName)) As [Group] ,[BilledQty] ,[BilledWt],[WastePerc] ,[Wastage] ,[TotalBilledWt] ,[SalePrice],SVP.[GSTRate] ,[GSTTax] ,[Discount],[TaxablelAmount] ,[TotalAmount],SVP.[MakingCharge],LTRIM(RTRIM(Convert(varchar(10), SVP.TransactionDate,120))) As InvoiceDate FROM [SalesVoucherInventoryByPc] SVP inner join StockItemsByPc SP on  Ltrim(rtrim(SVP.ItemName)) =Ltrim(rtrim(SP.ItemName)) and  Ltrim(rtrim(SVP.ItemBarCode)) =Ltrim(rtrim(SP.ItemBarCode))  where SVP.CompID = '" + CompID + "' and SP.CompID = '" + CompID + "' and SVP.TransactionDate <= '" + enddt + "' and SVP.TransactionDate >= '" + sdt + "'  ORDER BY SP.UnderGroupName", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);
            System.Data.DataTable dttablej = new System.Data.DataTable("Sale Summary");
            sda.Fill(dttablej);

            //SqlCommand cmdpdfj = new SqlCommand("GetAccountLedgerbyPeriod", conpdfj);
            //cmdpdfj.CommandType = CommandType.StoredProcedure;
            //cmdpdfj.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
            //cmdpdfj.Parameters.Add(new SqlParameter("@StartDate", sdt));
            //cmdpdfj.Parameters.Add(new SqlParameter("@EndDate", enddt));
            //cmdpdfj.Parameters.Add(new SqlParameter("@CompID", CompID));
            //SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);

            //cmdpdfj.Connection = conpdfj;
            //SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);
            //DataTable dttablej = new DataTable("Inv");
            //sda.Fill(dttablej);

            rg1.Rows.Add(rowheadertable1);

            IEnumerable itemsSource1 = StockSummaryGrid.ItemsSource as IEnumerable;
            if (itemsSource1 != null)
            {
                // foreach (var item in itemsSource)
                for (int k = 0; k < dttablej.Rows.Count; ++k)
                {
                    TableRow rowone = new TableRow();

                    // rowone.Background = Brushes.Silver;
                    rowone.FontSize = 9;
                    rowone.FontWeight = FontWeights.Regular;
                    rowone.FontFamily = new FontFamily("Century Gothic");

                    for (int i = 0; i < dttablej.Columns.Count; ++i)
                    {

                        TableCell firstcolproductcell = new TableCell(new System.Windows.Documents.Paragraph(new Run(dttablej.Rows[k][i].ToString())));
                        if (i == 1)
                        {
                            firstcolproductcell.ColumnSpan = 2;
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
            totalValParag.FontSize = 11;
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
            ts11gTotaoBeforeDisc = new Span(new Run("Gold Sold: " + goldOut.Text + "gms    Amount: " + lblGoldOutAmt.Content.ToString() + "    "));
            ts11gTotaoBeforeDisc.Inlines.Add(new LineBreak());//Line break is used for next line.  
            //}

            Span ts11gDiscAmountItemTotal = new Span();

            ts11gDiscAmountItemTotal = new Span(new Run("Silver Sold: " + silverOut.Text + "gms    Amount: " + lblSilverOutAmt.Content.ToString() + "    "));
            ts11gDiscAmountItemTotal.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span tsTotalTaxableAmt = new Span();
            tsTotalTaxableAmt = new Span(new Run("Old Gold Buy: " + oldGoldIn.Text + "gms        "));
            tsTotalTaxableAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span tsTotalOldSilver = new Span();
            tsTotalOldSilver = new Span(new Run("Old Silver Buy: " + oldSilverIn.Text + "gms          "));
            tsTotalOldSilver.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span tsTotalSaleValamOld = new Span();
            tsTotalSaleValamOld = new Span(new Run(lblTotalOldIns.Content.ToString() + "     "));
            tsTotalSaleValamOld.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span tsTotalSaleexchange = new Span();
            tsTotalSaleexchange = new Span(new Run(lblTotalExchangeIns.Content.ToString() + "     "));
            tsTotalSaleexchange.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span tsTotalSaleValam = new Span();
            tsTotalSaleValam = new Span(new Run(lblTotalSaleOuta.Content.ToString() + "     "));
            tsTotalSaleValam.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span linebreakUnderline = new Span();
            linebreakUnderline = new Span(new Run("____________________________________________________________"));
            linebreakUnderline.Inlines.Add(new LineBreak());//Line break is used for next line.  

            //Span tsTotalTaxableAmt = new Span();
            //tsTotalTaxableAmt = new Span(new Run("\t Total Old Gold Buy :" + "₹ " + oldGoldIn.Text));
            //tsTotalTaxableAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  



            totalVaGrand.FontSize = 12;
            totalVaGrand.TextAlignment = TextAlignment.Left;
            totalVaGrand.FontFamily = new FontFamily("Century Gothic");
            totalVaGrand.Inlines.Add(ts11gTotaoBeforeDisc);// Add the span content into paragraph.  
            totalVaGrand.Inlines.Add(ts11gDiscAmountItemTotal);
       
            totalVaGrand.Inlines.Add(tsTotalSaleValam);
            //totalVaGrand.Inlines.Add(tsMakingCharge);
            totalVaGrand.Inlines.Add(linebreakUnderline);

            totalVaGrand.Inlines.Add(tsTotalTaxableAmt);
            totalVaGrand.Inlines.Add(tsTotalOldSilver);
            //totalVaGrand.Inlines.Add(linebreakUnderline);
            totalVaGrand.Inlines.Add(tsTotalSaleValamOld);
            totalVaGrand.Inlines.Add(tsTotalSaleexchange);
            //totalVaGrand.Inlines.Add(linebreakUnderline);
            //totalVal.Inlines.Add(ali5);// Add the span content into paragraph.  
            totalVaGrand.TextAlignment = TextAlignment.Left;

            totalVaGrand.FontWeight = FontWeights.Bold;
            //doc.Blocks.Add(totalVaGrand);


            System.Windows.Documents.Paragraph totalVaGrand1 = new System.Windows.Documents.Paragraph();
            //totalValold.FontFamily 

            Span ts11gTotaoBeforeDisc1 = new Span();
            //if (totalValBeforeItemDis > 0)
            //{
            ts11gTotaoBeforeDisc1 = new Span(new Run("\t Total Gold Buy(gms):" + goldIn.Text + "    "));
            ts11gTotaoBeforeDisc1.Inlines.Add(new LineBreak());//Line break is used for next line.  
            //}

            Span ts11gDiscAmountItemTotal1 = new Span();

            ts11gDiscAmountItemTotal1 = new Span(new Run("\t Total Silver Buy(gms):" + silverIn.Text + "    "));
            ts11gDiscAmountItemTotal1.Inlines.Add(new LineBreak());//Line break is used for next line.  


            //Span tsTotalTaxableAmt1 = new Span();
            //tsTotalTaxableAmt1 = new Span(new Run("\t Total Old Gold Buy(gms):" + oldGoldIn.Text));
            //tsTotalTaxableAmt1.Inlines.Add(new LineBreak());//Line break is used for next line.  


            //Span tsTotalOldSilver11 = new Span();
            //tsTotalOldSilver11 = new Span(new Run("\t Total Old Silver Buy(gms):" + oldSilverIn.Text));
            //tsTotalOldSilver11.Inlines.Add(new LineBreak());//Line break is used for next line.  



            //Span tsTotalTaxableAmt = new Span();
            //tsTotalTaxableAmt = new Span(new Run("\t Total Old Gold Buy :" + "₹ " + oldGoldIn.Text));
            //tsTotalTaxableAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  



            totalVaGrand1.FontSize = 11;
            totalVaGrand1.FontFamily = new FontFamily("Century Gothic");
            totalVaGrand1.Inlines.Add(ts11gTotaoBeforeDisc1);// Add the span content into paragraph.  
            totalVaGrand1.Inlines.Add(ts11gDiscAmountItemTotal1);
            //totalVaGrand.Inlines.Add(tsMakingCharge);
            //totalVaGrand1.Inlines.Add(tsTotalTaxableAmt);
            //totalVaGrand1.Inlines.Add(tsTotalOldSilver);

            //totalVal.Inlines.Add(ali5);// Add the span content into paragraph.  
            totalVaGrand1.TextAlignment = TextAlignment.Left;

            totalVaGrand1.FontWeight = FontWeights.Bold;


            TableRow rowtwocompleteTable = new TableRow();

            TableRow rowthreecompleteTable = new TableRow();

            //-------------
            System.Windows.Documents.Table colTableAdd = new System.Windows.Documents.Table();
            var rg1tb = new TableRowGroup();
            TableRow rowColCellheadertable = new TableRow();
            //rowColCellheadertable.Background = Brushes.Silver;
            rowColCellheadertable.FontSize = 11;
            rowColCellheadertable.FontFamily = new FontFamily("Century Gothic");
            rowColCellheadertable.FontWeight = FontWeights.Bold;

            ThicknessConverter tc222tbc = new ThicknessConverter();

            TableCell tcellfirstTb = new TableCell(totalVaGrand1);

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
            doc.PagePadding = new Thickness(20, 20, 20, 5); //v3
            //doc.PagePadding = new Thickness(30, 20, 10, 5); //V2 
            // Create IDocumentPaginatorSource from FlowDocument
            // IDocumentPaginatorSource idpSource = doc;
            // Call PrintDocument method to send document to printer



            return doc;


        }

        private void TabInventory_Selected(object sender, RoutedEventArgs e)
        {

            goldInInvent.Clear();
            goldInInventQty.Clear();
            silverInInventQty.Clear();
            // goldOutInvent.Clear();
            oldGoldInInvent.Clear();
            goldInsadaInvent.Clear();
            goldInsadaInventQty.Clear();
            silverInInventQty.Clear();
            silverInsadaInvent.Clear();
            silverInsadaInventQty.Clear();

            goldInInvent.Clear();
           // goldOutInvent.Clear();
            oldGoldInInvent.Clear();
            //oldGoldOutInvent.Clear();
            silverInInvent.Clear();
          //  silverOutInvent.Clear();

            string sdt = startDateStockInvent.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateStockInvent.SelectedDate);
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

            string enddt = toDateStockInvent.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateStockInvent.SelectedDate);
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

            string sqlFilterQuery = "";
            if (itemBarcodeFilter.Text.Trim() !="")
            {
                sqlFilterQuery = "And ItemBarCode='" + itemBarcodeFilter.Text.Trim() + "' ";
            }
            bool isSoldChecked= false;
            if (isSoldOutChkbFilter.IsChecked == true)
            {
                isSoldChecked = true;
                sqlFilterQuery = sqlFilterQuery + " And IsSoldFlag= 1";
            }

            //if (isSoldOutChkbFilter.IsChecked == true)
            //{
            //    sqlFilterQuery = sqlFilterQuery + " And IsSoldFlag= 1 ";
            //}

            if (GroupName.Text != "")
            {
                sqlFilterQuery = sqlFilterQuery + " And Ltrim(rtrim(UnderGroupName))='" + GroupName.Text.Trim() + "'";
            }
            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();
                //select SUM(CAST(DR AS float)) As DebtAmount  from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' --and  TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
                // select SUM(CAST(CR AS float)) As CreditAmount  from PaymentVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' --and TransactionDate  <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
                string sqlQueryS = "";
                
                sqlQueryS = "SELECT  SrNumber As [SrNo], Ltrim(rtrim([ItemName]))  As [Item Name],Ltrim(rtrim([ItemBarCode]))  As [Barcode],Ltrim(rtrim(UnderGroupName)) As [Group] ,Ltrim(rtrim(UnderSubGroupName))  As [Sub Group], ActualQty, ActualWt,[GSTRate], [IsSoldFlag] As [SoldOut],[UnitID],[ItemPrice],[CompID] FROM StockItemsByPc WHERE  CompID = '" + CompID + "' and ItemName not like  '%Purchase%' ORDER BY UnderGroupName";

                if (sqlFilterQuery != "")
                {
                    sqlQueryS = "SELECT  SrNumber As [SrNo], Ltrim(rtrim([ItemName]))  As [Item Name],Ltrim(rtrim([ItemBarCode]))  As [Barcode],Ltrim(rtrim(UnderGroupName)) As [Group] ,Ltrim(rtrim(UnderSubGroupName))  As [Sub Group], ActualQty, ActualWt,[GSTRate], [IsSoldFlag] As [SoldOut],[UnitID],[ItemPrice],[CompID] FROM StockItemsByPc WHERE  CompID = '" + CompID + "' and ItemName not like  '%Purchase%' " + sqlFilterQuery + " ORDER BY UnderGroupName ";
                }
                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"

                SqlCommand com = new SqlCommand(sqlQueryS, con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt2 = new System.Data.DataTable("Stock Summary");
                sda.Fill(dt2);
                StockInventSummaryGrid.ItemsSource = dt2.DefaultView;
                StockInventSummaryGrid.AutoGenerateColumns = true;
                StockInventSummaryGrid.CanUserAddRows = false;




                //string grpName = "General";
                //double tptalWt = 0;
                //double tptalQty = 0;

                //double sumDr = 0;
                //double sumCr = 0;
                //foreach (DataRow row in dt2.Rows)
                //{
                //    //sumDr +=  Convert.ToDouble(row["DR"]);
                //    tptalWt = tptalWt + ((row["ActualWt"] != DBNull.Value) ? (Convert.ToDouble(row["ActualWt"])) : 0);
                //    tptalQty = tptalQty + ((row["ActualQty"] != DBNull.Value) ? (Convert.ToDouble(row["ActualQty"])) : 0);

                //    if (grpName.Trim() == "Gold")
                //    {
                //        goldInInvent.Text = tptalWt.ToString();
                //        goldInInventQty.Text = tptalQty.ToString();
                //    }
                //    if (grpName.Trim() == "Gold Sada")
                //    {
                //        goldInsadaInvent.Text = tptalWt.ToString();
                //        goldInsadaInventQty.Text = tptalQty.ToString();
                //    }
                //    if (grpName.Trim() == "Silver")
                //    {
                //        silverInInvent.Text = tptalWt.ToString();
                //        silverInInventQty.Text = tptalQty.ToString();
                //    }
                //    if (grpName.Trim() == "Silver Sada")
                //    {
                //        silverInsadaInvent.Text = tptalWt.ToString();
                //        silverInsadaInventQty.Text = tptalQty.ToString();
                //    }
                //    if (grpName.Trim() == "Old Gold")
                //    {
                //        oldGoldInInvent.Text = tptalWt.ToString();

                //    }
                //    if (grpName.Trim() == "Old Silver")
                //    {
                //        oldSilverInInvent.Text = tptalWt.ToString();

                //    }


                //}









            }


            using (SqlConnection con = new SqlConnection())
            {



                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetStockInventorySummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@ItemBarCode", itemBarcodeFilter.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@IsSoldFlag", isSoldChecked));
                com.Parameters.Add(new SqlParameter("@GroupName", GroupName.Text.Trim()));

                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    
                    string grpName = (reader["GroupName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "General";
                    double tptalWt = (reader["Wt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double tptalQty = (reader["Qty"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                        if (grpName.Trim() == "Gold")
                        {
                            goldInInvent.Text = tptalWt.ToString();
                            goldInInventQty.Text = tptalQty.ToString();
                        }
                        if (grpName.Trim() == "Gold Sada")
                        {
                            goldInsadaInvent.Text = tptalWt.ToString();
                            goldInsadaInventQty.Text = tptalQty.ToString();
                        }
                        if (grpName.Trim() == "Silver")
                        {
                            silverInInvent.Text = tptalWt.ToString();
                            silverInInventQty.Text = tptalQty.ToString();
                        }
                        if (grpName.Trim() == "Silver Sada")
                        {
                            silverInsadaInvent.Text = tptalWt.ToString();
                            silverInsadaInventQty.Text = tptalQty.ToString();
                        }
                        if (grpName.Trim() == "Old Gold")
                        {
                            oldGoldInInvent.Text = tptalWt.ToString();
                          
                        }
                        if (grpName.Trim() == "Old Silver")
                        {
                            oldSilverInInvent.Text = tptalWt.ToString();
                           
                        }
           

                }

            }


        }


        private void Button_Click_StockSummaryInventory(object sender, RoutedEventArgs e)
        {

            goldInInvent.Clear();
            goldInInventQty.Clear();
            silverInInventQty.Clear();
            // goldOutInvent.Clear();
            oldGoldInInvent.Clear();
            goldInsadaInvent.Clear();
            goldInsadaInventQty.Clear();
            silverInInventQty.Clear();
            silverInsadaInvent.Clear();
            silverInsadaInventQty.Clear();

              
            //oldGoldOutInvent.Clear();
            silverInInvent.Clear();
            //  silverOutInvent.Clear();

            string sdt = startDateStockInvent.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateStockInvent.SelectedDate);
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

            string enddt = toDateStockInvent.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateStockInvent.SelectedDate);
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

            string sqlFilterQuery = "";
            if (itemBarcodeFilter.Text.Trim() != "")
            {
                sqlFilterQuery = "And ItemBarCode='" + itemBarcodeFilter.Text.Trim() + "' ";
            }
            bool isSoldChecked = false;
            if (isSoldOutChkbFilter.IsChecked == true)
            {
                isSoldChecked = true;
                sqlFilterQuery = sqlFilterQuery + " And IsSoldFlag= 1";
            }

            //if (isSoldOutChkbFilter.IsChecked == true)
            //{
            //    sqlFilterQuery = sqlFilterQuery + " And IsSoldFlag= 1 ";
            //}
            if (GroupName.Text != "")
            {
                sqlFilterQuery = sqlFilterQuery + " And Ltrim(rtrim(UnderGroupName))='" + GroupName.Text.Trim() + "'";
            }

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();
                //select SUM(CAST(DR AS float)) As DebtAmount  from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' --and  TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
                // select SUM(CAST(CR AS float)) As CreditAmount  from PaymentVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' --and TransactionDate  <= '" + enddt + "' and TransactionDate >= '" + sdt + "'

                string sqlQueryS = "";

                sqlQueryS = "SELECT  SrNumber As [SrNo], Ltrim(rtrim([ItemName]))  As [Item Name],Ltrim(rtrim([ItemBarCode]))  As [Barcode],Ltrim(rtrim(UnderGroupName)) As [Group] ,Ltrim(rtrim(UnderSubGroupName))  As [Sub Group], ActualQty, ActualWt,[GSTRate], [IsSoldFlag] As [SoldOut],[UnitID],[ItemPrice],[CompID] FROM StockItemsByPc WHERE  CompID = '" + CompID + "' and ItemName not like  '%Purchase%' ORDER BY UnderGroupName";

                if (sqlFilterQuery != "")
                {
                    sqlQueryS = "SELECT  SrNumber As [SrNo], Ltrim(rtrim([ItemName]))  As [Item Name],Ltrim(rtrim([ItemBarCode]))  As [Barcode],Ltrim(rtrim(UnderGroupName)) As [Group] ,Ltrim(rtrim(UnderSubGroupName))  As [Sub Group], ActualQty, ActualWt,[GSTRate], [IsSoldFlag] As [SoldOut],[UnitID],[ItemPrice],[CompID] FROM StockItemsByPc WHERE  CompID = '" + CompID + "' and ItemName not like  '%Purchase%' " + sqlFilterQuery + " ORDER BY UnderGroupName ";
                }
                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"

                SqlCommand com = new SqlCommand(sqlQueryS, con);


                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"

                //SqlCommand com = new SqlCommand("SELECT SrNumber As [SrNo], Ltrim(rtrim([ItemName]))  As [Item Name],Ltrim(rtrim([ItemBarCode]))  As [Barcode],Ltrim(rtrim(UnderGroupName)) As [Group] ,Ltrim(rtrim(UnderSubGroupName))  As [Sub Group], ActualQty, ActualWt,[GSTRate] , [IsSoldFlag]  As [SoldOut],[UnitID],[ItemPrice],[CompID] FROM StockItemsByPc WHERE  CompID = '" + CompID + "' and ItemName not like  '%Purchase%' ORDER BY UnderGroupName", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt2 = new System.Data.DataTable("Stock Summary");
                sda.Fill(dt2);
                StockInventSummaryGrid.ItemsSource = dt2.DefaultView;
                StockInventSummaryGrid.AutoGenerateColumns = true;
                StockInventSummaryGrid.CanUserAddRows = false;
            }


            using (SqlConnection con = new SqlConnection())
            {



                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetStockInventorySummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@ItemBarCode", itemBarcodeFilter.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@IsSoldFlag", isSoldChecked));
                com.Parameters.Add(new SqlParameter("@GroupName", GroupName.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {

                    string grpName = (reader["GroupName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "General";
                    double tptalWt = (reader["Wt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double tptalQty = (reader["Qty"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    if (grpName.Trim() == "Gold")
                    {
                        goldInInvent.Text = tptalWt.ToString();
                        goldInInventQty.Text = tptalQty.ToString();
                    }
                    if (grpName.Trim() == "Gold Sada")
                    {
                        goldInsadaInvent.Text = tptalWt.ToString();
                        goldInsadaInventQty.Text = tptalQty.ToString();
                    }
                    if (grpName.Trim() == "Silver")
                    {
                        silverInInvent.Text = tptalWt.ToString();
                        silverInInventQty.Text = tptalQty.ToString();
                    }
                    if (grpName.Trim() == "Silver Sada")
                    {
                        silverInsadaInvent.Text = tptalWt.ToString();
                        silverInsadaInventQty.Text = tptalQty.ToString();
                    }
                    if (grpName.Trim() == "Old Gold")
                    {
                        oldGoldInInvent.Text = tptalWt.ToString();

                    }
                    if (grpName.Trim() == "Old Silver")
                    {
                        oldSilverInInvent.Text = tptalWt.ToString();

                    }


                }

            }

        }

        private void GroupName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (GroupName.SelectedItem != null)
            //{
            //    string storagenameselected = GroupName.SelectedItem.ToString();
            //    BindComboBoxSubGroup(storagenameselected);
            //}

        }

        private void TabDayBook_Selected(object sender, RoutedEventArgs e)
        {
            //goldInInvent.Clear();
            //// goldOutInvent.Clear();
            //oldGoldInInvent.Clear();
            ////oldGoldOutInvent.Clear();
            //silverInInvent.Clear();
            ////  silverOutInvent.Clear();

            string sdt = startDtAllLedger.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDtAllLedger.SelectedDate);
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

            string enddt = toDtAllLedger.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDtAllLedger.SelectedDate);
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

                SqlCommand com = new SqlCommand("GetDaybookDualEntrySummary", con);
                com.CommandType = CommandType.StoredProcedure;
               // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        

                System.Data.DataTable dt1 = new System.Data.DataTable("Account Ledger");
                sda.Fill(dt1);
                AllAccointsSummaryGrid.ItemsSource = dt1.DefaultView;
                AllAccointsSummaryGrid.AutoGenerateColumns = true;
                AllAccointsSummaryGrid.CanUserAddRows = false;
            }

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetDaybookDualEntrySummaryTotalCRDR", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double dCrAcctLedgerAmt = (reader["TotalCredit"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    double dDrAcctLedgerAmt = (reader["TotalDebit"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    //double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    //double opBalBookStart = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;

                    totalDaybookCR.Text = dCrAcctLedgerAmt.ToString();
                    totalDaybookDR.Text = dDrAcctLedgerAmt.ToString();
                }
            }


        }

        private void Button_Click_DayBookSummary(object sender, RoutedEventArgs e)
        {
            string sdt = startDtAllLedger.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDtAllLedger.SelectedDate);
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

            string enddt = toDtAllLedger.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDtAllLedger.SelectedDate);
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

                SqlCommand com = new SqlCommand("GetDaybookDualEntrySummary", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        

                System.Data.DataTable dt1 = new System.Data.DataTable("Account Ledger");
                sda.Fill(dt1);
                AllAccointsSummaryGrid.ItemsSource = dt1.DefaultView;
                AllAccointsSummaryGrid.AutoGenerateColumns = true;
                AllAccointsSummaryGrid.CanUserAddRows = false;
            }

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetDaybookDualEntrySummaryTotalCRDR", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double dCrAcctLedgerAmt = (reader["TotalCredit"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    double dDrAcctLedgerAmt = (reader["TotalDebit"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    //double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    //double opBalBookStart = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;

                    totalDaybookCR.Text = dCrAcctLedgerAmt.ToString();
                    totalDaybookDR.Text = dDrAcctLedgerAmt.ToString();
                }
            }



        }

        private void TabBank_Selected(object sender, RoutedEventArgs e)
        {
            //SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //conn.Open();

            string sdt = startDateBank.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateBank.SelectedDate);
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

            string enddt = toDateBank.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateBank.SelectedDate);
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
                SqlCommand com = new SqlCommand("(SELECT  CONVERT(varchar, TransactionDate, 103) AS [Date], LTRIM(RTRIM([VoucherNumber])) As VoucherNumber ,LTRIM(RTRIM([VoucherType])) As VoucherType,LTRIM(RTRIM([AcctName])) As AccountName,LTRIM(RTRIM([PayMode]))  As Mode,LTRIM(RTRIM([Remarks]))  As Remarks ,[CR] ,[DR] FROM [BankAccountsLedgers] where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "')", con);
               // SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(VoucherNumber)) As VoucherNumber ,LTRIM(RTRIM(VoucherType))  As VoucherType,LTRIM(RTRIM(DebtorAccountName)) As DebtorAccountName,LTRIM(RTRIM(CreditorAccountName)) As CreditorAccountName,CR As Amount,PayMode,Against,Narration,TransactionDate,CreationDate,UpdateDate,CreatedBy from ReceiptVouchers where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') Union ( select  LTRIM(RTRIM(VoucherNumber)) As VoucherNumber ,LTRIM(RTRIM(VoucherType))  As VoucherType,LTRIM(RTRIM(DebtorAccountName)) As DebtorAccountName,LTRIM(RTRIM(CreditorAccountName)) As CreditorAccountName,CR As Amount,PayMode,Against,Narration,TransactionDate,CreationDate,UpdateDate,CreatedBy from PaymentVouchers  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "')", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt2 = new System.Data.DataTable("Bank Flow");
                sda.Fill(dt2);
                ItemBankGrid.ItemsSource = dt2.DefaultView;
                ItemBankGrid.AutoGenerateColumns = true;
                ItemBankGrid.CanUserAddRows = false;
            }


            using (SqlConnection con = new SqlConnection())
            {



                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetBankAccountLedgerSummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double dDebtAcctLedgerAmt = (reader["DebtAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    double dCredAcctLedgerAmt = (reader["CredAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double opBalBookStart = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;

                    TotalDebitAmtBank.Text = dDebtAcctLedgerAmt.ToString();
                    TotalCreditAmtBank.Text = dCredAcctLedgerAmt.ToString();
                    BalanceBank.Text = (dDebtAcctLedgerAmt - dCredAcctLedgerAmt).ToString();
                    openingBalBank.Text = opBal.ToString();
                    openingBalBookStartBank.Text = opBalBookStart.ToString();

                    closingBalBankEndDate.Text = (opBal + Convert.ToDouble(BalanceBank.Text)).ToString();
                    //double debitamt = (reader["DebtAmount"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    //double creditamt = (reader["CreditAmount"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    //double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    //TotalDebitAmt.Text = debitamt.ToString();
                    //TotalCreditAmt.Text = creditamt.ToString();
                    //Balance.Text = (creditamt - debitamt).ToString();
                    //openingBal.Text = opBal.ToString();
                }

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

        private void ButtonBank_Click(object sender, RoutedEventArgs e)
        {
            
            openingBalBank.Clear();
            TotalCreditAmtBank.Clear();
            TotalDebitAmtBank.Clear();
            BalanceBank.Clear();

            string sdt = startDateBank.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateBank.SelectedDate);
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

            string enddt = toDateBank.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateBank.SelectedDate);
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
                //select SUM(CAST(DR AS float)) As DebtAmount  from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' --and  TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "'
                // select SUM(CAST(CR AS float)) As CreditAmount  from PaymentVouchers where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' --and TransactionDate  <= '" + enddt + "' and TransactionDate >= '" + sdt + "'


                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"
                SqlCommand com = new SqlCommand("(SELECT  CONVERT(varchar, TransactionDate, 103) AS [Date], LTRIM(RTRIM([VoucherNumber])) As VoucherNumber ,LTRIM(RTRIM([VoucherType])) As VoucherType,LTRIM(RTRIM([AcctName])) As AccountName,LTRIM(RTRIM([PayMode]))  As Mode,LTRIM(RTRIM([Remarks]))  As Remarks ,[CR] ,[DR] FROM [BankAccountsLedgers] where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "')", con);
                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(VoucherNumber)) As VoucherNumber ,LTRIM(RTRIM(VoucherType))  As VoucherType,LTRIM(RTRIM(DebtorAccountName)) As DebtorAccountName,LTRIM(RTRIM(CreditorAccountName)) As CreditorAccountName,CR As Amount,PayMode,Against,Narration,TransactionDate,CreationDate,UpdateDate,CreatedBy from ReceiptVouchers where UPPER(LTRIM(RTRIM(DebtorAccountName)))='CASH' and CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') Union ( select  LTRIM(RTRIM(VoucherNumber)) As VoucherNumber ,LTRIM(RTRIM(VoucherType))  As VoucherType,LTRIM(RTRIM(DebtorAccountName)) As DebtorAccountName,LTRIM(RTRIM(CreditorAccountName)) As CreditorAccountName,CR As Amount,PayMode,Against,Narration,TransactionDate,CreationDate,UpdateDate,CreatedBy from PaymentVouchers  where UPPER(LTRIM(RTRIM(CreditorAccountName)))='CASH' and CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "')", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                System.Data.DataTable dt1 = new System.Data.DataTable("Bank Flow");
                sda.Fill(dt1);
                ItemBankGrid.ItemsSource = dt1.DefaultView;
                ItemBankGrid.AutoGenerateColumns = true;
                ItemBankGrid.CanUserAddRows = false;
            }

            using (SqlConnection con = new SqlConnection())
            {



                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetBankAccountLedgerSummarybyPeriod", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double dDebtAcctLedgerAmt = (reader["DebtAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;                 
                    double dCredAcctLedgerAmt = (reader["CredAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double opBalBookStart = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;

                    TotalDebitAmtBank.Text = dDebtAcctLedgerAmt.ToString();
                    TotalCreditAmtBank.Text = dCredAcctLedgerAmt.ToString();
                    BalanceBank.Text = (dDebtAcctLedgerAmt - dCredAcctLedgerAmt).ToString();
                    openingBalBank.Text= opBal.ToString();
                    openingBalBookStartBank.Text = opBalBookStart.ToString();

                    closingBalBankEndDate.Text = (opBal + Convert.ToDouble(BalanceBank.Text)).ToString();
                    //double debitamt = (reader["DebtAmount"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    //double creditamt = (reader["CreditAmount"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    //double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    //TotalDebitAmt.Text = debitamt.ToString();
                    //TotalCreditAmt.Text = creditamt.ToString();
                    //Balance.Text = (creditamt - debitamt).ToString();
                    //openingBal.Text = opBal.ToString();
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

        private void printCashAcctLedger_Click(object sender, RoutedEventArgs e)
        {
               PrintDialog printDlg = new PrintDialog();
               printDlg.PrintQueue = System.Printing.LocalPrintServer.GetDefaultPrintQueue();
               printDlg.PrintTicket = printDlg.PrintQueue.DefaultPrintTicket;
               printDlg.PrintTicket.PageOrientation = PageOrientation.Portrait;

            // Create a FlowDocument dynamically.
            //FlowDocument doc = CreateFlowDocumentJewellery();
               FlowDocument doc = CreateFlowDocumentCashLedger();
            doc.ColumnWidth = 600;
            doc.Name = "FlowDoc";
            doc.PageHeight = 1000;
            doc.PageWidth = 800;
            doc.MinPageWidth = 800;


            // Create IDocumentPaginatorSource from FlowDocument
            IDocumentPaginatorSource idpSource = doc;

            // Call PrintDocument method to send document to printer
            //Uncomment for Print
            printDlg.PrintDocument(idpSource.DocumentPaginator, "Receipt Printing.");
        }

        private FlowDocument CreateFlowDocumentCashLedger()
        {
            //  Get Confirmation that data saved successfull, 

            string sdt = startDate.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDate.SelectedDate);
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

            string enddt = toDate.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDate.SelectedDate);
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
            doc.PageHeight = 1000;
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
            a3 = new Span(new Run("Cash Statement"));
            a3.FontWeight = FontWeights.Bold;
            a3.Inlines.Add(new LineBreak());//Line break is used for next line.  

            //Span a4 = new Span();
            //a4 = new Span(new Run("Invoice# " + invoiceNumber.Text));
            //a4.FontWeight = FontWeights.Bold;
            //a4.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a4acc = new Span();            
            a4acc = new Span(new Run("M/S.-Cash"));
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
            for (int i = 0; i < ItemSaleGrid.Items.Count; i++)
            {
                //TableColumn tc = new TableColumn();

                t5.Columns.Add(new TableColumn() { Width = GridLength.Auto });

            }

            ThicknessConverter tc1 = new ThicknessConverter();
            //// Create Table Borders
            t5.BorderThickness = (Thickness)tc1.ConvertFromString("0.02in");

            int count1 = ItemSaleGrid.Items.Count;
            var rg1 = new TableRowGroup();

            TableRow rowheadertable1 = new TableRow();



            rowheadertable1.Background = Brushes.Silver;
            rowheadertable1.FontSize = 9;
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

            TableCell tcell3 = new TableCell(new System.Windows.Documents.Paragraph(new Run("VType")));
            //tcell3.ColumnSpan = 3;
            tcell3.BorderBrush = Brushes.Black;
            tcell3.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell3);

            TableCell tcell4 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Account")));
            tcell4.ColumnSpan = 3;
            tcell4.BorderBrush = Brushes.Black;
            tcell4.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell4);


            TableCell tcell6 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Mode")));
            //tcell6.ColumnSpan = 3;
            tcell6.BorderBrush = Brushes.Black;
            tcell6.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell6);

            TableCell tcell7 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Remarks")));
            //tcell7.ColumnSpan = 3;
            tcell7.BorderBrush = Brushes.Black;
            tcell7.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell7);

            TableCell tcell8 = new TableCell(new System.Windows.Documents.Paragraph(new Run("CR")));
            //tcell8.ColumnSpan = 3;
            tcell8.BorderBrush = Brushes.Black;
            tcell8.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell8);

            TableCell tcell9 = new TableCell(new System.Windows.Documents.Paragraph(new Run("DR")));
            //tcell9.ColumnSpan = 3;
            tcell9.BorderBrush = Brushes.Black;
            tcell9.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell9);

            TableCell tcell11 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Date")));
            //tcell11.ColumnSpan = 3;
            tcell11.BorderBrush = Brushes.Black;
            tcell11.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell11);


           
            SqlConnection conpdfj = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conpdfj.Open();
            //string sqlpdf = "SELECT row_number() OVER (order by srnumber ) Sr ,DesignNumberPattern AS Style,[ItemName] As [Item Name]  ,[HSN],Small As S, Mediium As M, Large As L, XL, XL2, XL3,XL4,XL5,XL6 ,[BilledQty] As [Qty] ,[UnitID] As [UOM],[SalePrice] As [Price],Amount ,[Discount] As [Disc(%)] ,[TaxablelAmount] As [Taxable] ,[GSTRate] As [GST%] ,[TotalAmount] As [Total]   FROM [SalesVoucherInventorycloths] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "'";
            // string sqlpdfj = "SELECT [ItemName] As [ITEM NAME],[BilledQty] As [Qty] ,[BilledWt] As [Wt],WastePerc,[TotalBilledWt],MakingCharge,[SalePrice] As [Price],Amount,[Discount] As [Disc(%)],TaxablelAmount ,[GSTRate] As [GST%] ,[TotalAmount] As [TOTAL]   FROM [SalesVoucherInventoryByPc] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "' and ItemName not in ( 'Old Gold','Old Silver')";
            //SqlCommand cmdpdfj = new SqlCommand(sqlpdfj);
            SqlCommand cmdpdfj = new SqlCommand("(SELECT LTRIM(RTRIM([VoucherNumber])) As VoucherNumber ,LTRIM(RTRIM([VoucherType])) As VoucherType,LTRIM(RTRIM([AcctName])) As AccountName,LTRIM(RTRIM([PayMode]))  As Mode,LTRIM(RTRIM([Remarks]))  As Remarks ,[CR] ,[DR],CONVERT(varchar, TransactionDate, 103) AS [Date] FROM [CashAccountsLedgers] where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "')", conpdfj);
            
           
            SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);

            //cmdpdfj.Connection = conpdfj;
            //SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);
            DataTable dttablej = new DataTable("Inv");
            sda.Fill(dttablej);

            rg1.Rows.Add(rowheadertable1);

            IEnumerable itemsSource1 = ItemSaleGrid.ItemsSource as IEnumerable;
            if (itemsSource1 != null)
            {
                // foreach (var item in itemsSource)
                for (int k = 0; k < dttablej.Rows.Count; ++k)
                {
                    TableRow rowone = new TableRow();

                    // rowone.Background = Brushes.Silver;
                    rowone.FontSize = 9;
                    rowone.FontWeight = FontWeights.Regular;
                    rowone.FontFamily = new FontFamily("Century Gothic");

                    for (int i = 1; i < dttablej.Columns.Count; ++i)
                    {

                        TableCell firstcolproductcell = new TableCell(new System.Windows.Documents.Paragraph(new Run(dttablej.Rows[k][i].ToString())));
                        if (i == 2)
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
            ts11gTotaoBeforeDisc = new Span(new Run("\t Opening Balance: " +openingBal.Text +"      "+ "\n Total DR:" + "₹" + TotalDebitAmt.Text+"      "));
            ts11gTotaoBeforeDisc.Inlines.Add(new LineBreak());//Line break is used for next line.  
            //}

            Span ts11gDiscAmountItemTotal = new Span();

            ts11gDiscAmountItemTotal = new Span(new Run("\t Total CR:" + "₹ " + TotalCreditAmt.Text + "      "));
            ts11gDiscAmountItemTotal.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span tsTotalTaxableAmt = new Span();

            tsTotalTaxableAmt = new Span(new Run("\t Balance :" + "₹ " + Balance.Text + "      "+"\n  Closing Bal:" + "₹ " + closingBalEndDate.Text + "      "));
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

            TableCell tcellfirstTb = new TableCell(new System.Windows.Documents.Paragraph(new Run("E&OE. \n *Subject to Jurisdiction \n * ")));

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

            completeTable.Padding = new Thickness(20);
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
            doc.PagePadding = new Thickness(20, 10, 10, 20); //v3
            //doc.PagePadding = new Thickness(30, 20, 10, 5); //V2 
            // Create IDocumentPaginatorSource from FlowDocument
            // IDocumentPaginatorSource idpSource = doc;
            // Call PrintDocument method to send document to printer



            return doc;


        }


        private void printSaleAcctLedger_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintQueue = System.Printing.LocalPrintServer.GetDefaultPrintQueue();
            printDlg.PrintTicket = printDlg.PrintQueue.DefaultPrintTicket;
            printDlg.PrintTicket.PageOrientation = PageOrientation.Portrait;

            // Create a FlowDocument dynamically.
            //FlowDocument doc = CreateFlowDocumentJewellery();
            FlowDocument doc = CreateFlowDocumentSaleLedger();
            doc.ColumnWidth = 600;
            doc.Name = "FlowDoc";
            doc.PageHeight = 1000;
            doc.PageWidth = 800;
            doc.MinPageWidth = 800;


            // Create IDocumentPaginatorSource from FlowDocument
            IDocumentPaginatorSource idpSource = doc;

            // Call PrintDocument method to send document to printer
            //Uncomment for Print
            printDlg.PrintDocument(idpSource.DocumentPaginator, "Sale Printing.");
        }

        private FlowDocument CreateFlowDocumentSaleLedger()
        {
            //  Get Confirmation that data saved successfull, 

            string sdt = startDateSale.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateSale.SelectedDate);
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

            string enddt = toDateSale.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateSale.SelectedDate);
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

            string sdateIndFormat = days + "/" + months + "/" + years;
            string enddateIndFormat = dayd + "/" + monthd + "/" + yeard;

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
            doc.PageHeight = 1000;
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
            a3 = new Span(new Run("Sale Statement"));
            a3.FontWeight = FontWeights.Bold;
            a3.Inlines.Add(new LineBreak());//Line break is used for next line.  

            //Span a4 = new Span();
            //a4 = new Span(new Run("Invoice# " + invoiceNumber.Text));
            //a4.FontWeight = FontWeights.Bold;
            //a4.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a4acc = new Span();
            a4acc = new Span(new Run("M/S.-Cash"));
            a4acc.FontWeight = FontWeights.Bold;
            a4acc.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span a4date = new Span();
            a4date = new Span(new Run("Period: " + sdateIndFormat + "-To- " + enddateIndFormat));
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
            //p.Inlines.Add(a4acc);// Add the span content into paragraph.  
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
            for (int i = 0; i < ItemSaleGrid.Items.Count; i++)
            {
                //TableColumn tc = new TableColumn();

                t5.Columns.Add(new TableColumn() { Width = GridLength.Auto });

            }

            ThicknessConverter tc1 = new ThicknessConverter();
            //// Create Table Borders
            t5.BorderThickness = (Thickness)tc1.ConvertFromString("0.02in");

            int count1 = ItemSaleGrid.Items.Count;
            var rg1 = new TableRowGroup();

            TableRow rowheadertable1 = new TableRow();



            rowheadertable1.Background = Brushes.Silver;
            rowheadertable1.FontSize = 9;
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

            //TableCell tcell3 = new TableCell(new System.Windows.Documents.Paragraph(new Run("VType")));
            ////tcell3.ColumnSpan = 3;
            //tcell3.BorderBrush = Brushes.Black;
            //tcell3.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell3);

            TableCell tcell4 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Account")));
            tcell4.ColumnSpan = 3;
            tcell4.BorderBrush = Brushes.Black;
            tcell4.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell4);


            TableCell tcell6 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Invoice")));
            //tcell6.ColumnSpan = 3;
            tcell6.BorderBrush = Brushes.Black;
            tcell6.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell6);

            TableCell tcell7 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Amount")));
            //tcell7.ColumnSpan = 3;
            tcell7.BorderBrush = Brushes.Black;
            tcell7.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell7);

            TableCell tcell9 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Balance")));
            //tcell9.ColumnSpan = 3;
            tcell9.BorderBrush = Brushes.Black;
            tcell9.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell9);

            TableCell tcell8 = new TableCell(new System.Windows.Documents.Paragraph(new Run("InvoiceDate")));
            //tcell8.ColumnSpan = 3;
            tcell8.BorderBrush = Brushes.Black;
            tcell8.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell8);



            //TableCell tcell11 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Date")));
            ////tcell11.ColumnSpan = 3;
            //tcell11.BorderBrush = Brushes.Black;
            //tcell11.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell11);




            SqlConnection conpdfj = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conpdfj.Open();

            string sqlselectQuerySale = "";
            if (autocompltCustNameSaleTab.autoTextBox.Text.Trim() != "")
            {
                sqlselectQuerySale = "(select LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number], LTRIM(RTRIM(InvoiceAmt)) As Amount, DueAmount As [Balance], CONVERT(varchar, TransactionDate, 103) AS [Date] from SalesVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "' and AccountName = '" + autocompltCustNameSaleTab.autoTextBox.Text.Trim() + "') order by  CAST(InvoiceNumber AS float) desc";
                //select * from ReceiptVouchers where  CompID = '" + companyId + "'"  Union  select * from PaymentVouchers where  CompID = '" + companyId + "'"
            }
            else
            {
                sqlselectQuerySale = "(select LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number], LTRIM(RTRIM(InvoiceAmt)) As Amount, DueAmount As [Balance], CONVERT(varchar, TransactionDate, 103) AS [Date] from SalesVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') order by  CAST(InvoiceNumber AS float) desc";
            }

            SqlCommand cmdpdfj = new SqlCommand(sqlselectQuerySale, conpdfj);


            SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);

            //cmdpdfj.Connection = conpdfj;
            //SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);
            DataTable dttablej = new DataTable("Inv");
            sda.Fill(dttablej);

            rg1.Rows.Add(rowheadertable1);

            IEnumerable itemsSource1 = SaleSummaryGrid.ItemsSource as IEnumerable;
            if (itemsSource1 != null)
            {
                // foreach (var item in itemsSource)
                for (int k = 0; k < dttablej.Rows.Count; ++k)
                {
                    TableRow rowone = new TableRow();

                    // rowone.Background = Brushes.Silver;
                    rowone.FontSize = 9;
                    rowone.FontWeight = FontWeights.Regular;
                    rowone.FontFamily = new FontFamily("Century Gothic");

                    for (int i = 0; i < dttablej.Columns.Count; ++i)
                    {

                        TableCell firstcolproductcell = new TableCell(new System.Windows.Documents.Paragraph(new Run(dttablej.Rows[k][i].ToString())));
                        if (i == 0)
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
            ts11gTotaoBeforeDisc = new Span(new Run("\t Total Sale: " + totalSale.Text.Trim() + "           "));
            ts11gTotaoBeforeDisc.Inlines.Add(new LineBreak());//Line break is used for next line.  
            //}

            Span ts11gDiscAmountItemTotal = new Span();

            ts11gDiscAmountItemTotal = new Span(new Run("\t Total CR:" + "₹ " + TotalCreditAmt.Text + "      "));
            ts11gDiscAmountItemTotal.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span tsTotalTaxableAmt = new Span();

            tsTotalTaxableAmt = new Span(new Run("\t Balance :" + "₹ " + Balance.Text + "      " + "\n  Closing Bal:" + "₹ " + closingBalEndDate.Text + "      "));
            tsTotalTaxableAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  



            totalVaGrand.FontSize = 14;
            totalVaGrand.FontFamily = new FontFamily("Century Gothic");
            totalVaGrand.Inlines.Add(ts11gTotaoBeforeDisc);// Add the span content into paragraph.  
            //totalVaGrand.Inlines.Add(ts11gDiscAmountItemTotal);
            //totalVaGrand.Inlines.Add(tsMakingCharge);
            //totalVaGrand.Inlines.Add(tsTotalTaxableAmt);


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

            TableCell tcellfirstTb = new TableCell(new System.Windows.Documents.Paragraph(new Run("E&OE. \n *Subject to Jurisdiction \n * ")));

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

            completeTable.Padding = new Thickness(20);
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
            doc.PagePadding = new Thickness(20, 10, 10, 20); //v3
            //doc.PagePadding = new Thickness(30, 20, 10, 5); //V2 
            // Create IDocumentPaginatorSource from FlowDocument
            // IDocumentPaginatorSource idpSource = doc;
            // Call PrintDocument method to send document to printer



            return doc;


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

            TableCell tcell10Againt= new TableCell(new System.Windows.Documents.Paragraph(new Run("Against")));
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
                        if (i == 4|| i == 5)
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

        private void printSaleStockBookLedger_Click(object sender, RoutedEventArgs e)
        {
            string sdt = startDateStockInvent.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateStockInvent.SelectedDate);
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

            string enddt = toDateStockInvent.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateStockInvent.SelectedDate);
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






            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintQueue = System.Printing.LocalPrintServer.GetDefaultPrintQueue();
            printDlg.PrintTicket = printDlg.PrintQueue.DefaultPrintTicket;
            printDlg.PrintTicket.PageOrientation = PageOrientation.Portrait;
            // Create a FlowDocument dynamically.
            //FlowDocument doc = CreateFlowDocumentJewellery();
            FlowDocument doc = CreateFlowDocumentSaleStockBook();
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

        private FlowDocument CreateFlowDocumentSaleStockBook()
        {
            //  Get Confirmation that data saved successfull, 

            string sdt = startDateStockInvent.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateStockInvent.SelectedDate);
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

            string enddt = toDateStockInvent.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateStockInvent.SelectedDate);
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
            a3 = new Span(new Run("Available Stock-Book"));
            a3.FontWeight = FontWeights.Bold;
            a3.Inlines.Add(new LineBreak());//Line break is used for next line.  

            //Span a4 = new Span();
            //a4 = new Span(new Run("Invoice# " + invoiceNumber.Text));
            //a4.FontWeight = FontWeights.Bold;
            //a4.Inlines.Add(new LineBreak());//Line break is used for next line.  

            //Span a4acc = new Span();
            //a4acc = new Span(new Run("M/S. " + autocompltCustName.autoTextBox.Text));
            ////a4acc.FontWeight = FontWeights.Bold;
            //a4acc.Inlines.Add(new LineBreak());//Line break is used for next line.  


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
            //p.Inlines.Add(a4acc);// Add the span content into paragraph.  
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
            for (int i = 0; i < StockSummaryGrid.Items.Count; i++)
            {
                //TableColumn tc = new TableColumn();

                t5.Columns.Add(new TableColumn() { Width = GridLength.Auto });

            }

            ThicknessConverter tc1 = new ThicknessConverter();
            //// Create Table Borders
            t5.BorderThickness = (Thickness)tc1.ConvertFromString("0.02in");

            int count1 = StockSummaryGrid.Items.Count;
            var rg1 = new TableRowGroup();

            TableRow rowheadertable1 = new TableRow();



            rowheadertable1.Background = Brushes.Silver;
            rowheadertable1.FontSize = 9;
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

            //TableCell tcell3 = new TableCell(new System.Windows.Documents.Paragraph(new Run("ItemName")));
            ////tcell3.ColumnSpan = 3;
            //tcell3.BorderBrush = Brushes.Black;
            //tcell3.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell3);

            TableCell tcell4 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Item")));
            tcell4.ColumnSpan = 2;
            tcell4.BorderBrush = Brushes.Black;
            tcell4.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell4);

            TableCell tcell5 = new TableCell(new System.Windows.Documents.Paragraph(new Run("BarCode")));
            //tcell5.ColumnSpan = 3;
            tcell5.BorderBrush = Brushes.Black;
            tcell5.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell5);

            TableCell tcell6 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Group")));
            //tcell6.ColumnSpan = 3;
            tcell6.BorderBrush = Brushes.Black;
            tcell6.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell6);

            TableCell tcell7 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Live-Qty")));
            //tcell7.ColumnSpan = 3;
            tcell7.BorderBrush = Brushes.Black;
            tcell7.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell7);

            TableCell tcell8 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Wt(gms)")));
            //tcell8.ColumnSpan = 3;
            tcell8.BorderBrush = Brushes.Black;
            tcell8.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell8);

            TableCell tcell9 = new TableCell(new System.Windows.Documents.Paragraph(new Run("GST(%)")));
            //tcell9.ColumnSpan = 3;
            tcell9.BorderBrush = Brushes.Black;
            tcell9.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell9);

            //TableCell tcell10 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Waste")));
            ////tcell10.ColumnSpan = 3;
            //tcell10.BorderBrush = Brushes.Black;
            //tcell10.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell10);

            //TableCell tcell11 = new TableCell(new System.Windows.Documents.Paragraph(new Run("GrossWt")));
            ////tcell11.ColumnSpan = 3;
            //tcell11.BorderBrush = Brushes.Black;
            //tcell11.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell11);

            //TableCell tcell12 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Price")));
            ////tcell11.ColumnSpan = 3;
            //tcell12.BorderBrush = Brushes.Black;
            //tcell12.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell12);

            //TableCell tcell13 = new TableCell(new System.Windows.Documents.Paragraph(new Run("GST%")));
            ////tcell11.ColumnSpan = 3;
            //tcell13.BorderBrush = Brushes.Black;
            //tcell13.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell13);

            //TableCell tcell14 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Tax")));
            ////tcell11.ColumnSpan = 3;
            //tcell14.BorderBrush = Brushes.Black;
            //tcell14.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell14);

            //TableCell tcell15 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Dis")));
            ////tcell11.ColumnSpan = 3;
            //tcell15.BorderBrush = Brushes.Black;
            //tcell15.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell15);

            //TableCell tcell16 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Taxable")));
            ////tcell11.ColumnSpan = 3;
            //tcell16.BorderBrush = Brushes.Black;
            //tcell16.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell16);

            //TableCell tcell17 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Total")));
            ////tcell11.ColumnSpan = 3;
            //tcell17.BorderBrush = Brushes.Black;
            //tcell17.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell17);


            ////TableCell tcell18 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Labour")));
            //////tcell11.ColumnSpan = 3;
            ////tcell18.BorderBrush = Brushes.Black;
            ////tcell18.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            ////rowheadertable1.Cells.Add(tcell18);

            //TableCell tcell19 = new TableCell(new System.Windows.Documents.Paragraph(new Run("MC")));
            ////tcell11.ColumnSpan = 3;
            //tcell19.BorderBrush = Brushes.Black;
            //tcell19.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell19);

            //TableCell tcell20 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Date")));
            ////tcell11.ColumnSpan = 3;
            //tcell20.BorderBrush = Brushes.Black;
            //tcell20.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell20);




            SqlConnection conpdfj = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conpdfj.Open();
            //string sqlpdf = "SELECT row_number() OVER (order by srnumber ) Sr ,DesignNumberPattern AS Style,[ItemName] As [Item Name]  ,[HSN],Small As S, Mediium As M, Large As L, XL, XL2, XL3,XL4,XL5,XL6 ,[BilledQty] As [Qty] ,[UnitID] As [UOM],[SalePrice] As [Price],Amount ,[Discount] As [Disc(%)] ,[TaxablelAmount] As [Taxable] ,[GSTRate] As [GST%] ,[TotalAmount] As [Total]   FROM [SalesVoucherInventorycloths] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "'";
            // string sqlpdfj = "SELECT [ItemName] As [ITEM NAME],[BilledQty] As [Qty] ,[BilledWt] As [Wt],WastePerc,[TotalBilledWt],MakingCharge,[SalePrice] As [Price],Amount,[Discount] As [Disc(%)],TaxablelAmount ,[GSTRate] As [GST%] ,[TotalAmount] As [TOTAL]   FROM [SalesVoucherInventoryByPc] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "' and ItemName not in ( 'Old Gold','Old Silver')";
            //SqlCommand cmdpdfj = new SqlCommand(sqlpdfj);

            SqlCommand cmdpdfj = new SqlCommand("SELECT Ltrim(rtrim([ItemName]))  As [Item Name],Ltrim(rtrim([ItemBarCode]))  As [Barcode],Ltrim(rtrim(UnderGroupName)) As [Group] , ActualQty, ActualWt,[GSTRate] FROM StockItemsByPc WHERE  CompID = '" + CompID + "' and ItemName not like  '%Purchase%' ORDER BY UnderGroupName", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);
            System.Data.DataTable dttablej = new System.Data.DataTable("Stock Book");
            sda.Fill(dttablej);

            //SqlCommand cmdpdfj = new SqlCommand("GetAccountLedgerbyPeriod", conpdfj);
            //cmdpdfj.CommandType = CommandType.StoredProcedure;
            //cmdpdfj.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
            //cmdpdfj.Parameters.Add(new SqlParameter("@StartDate", sdt));
            //cmdpdfj.Parameters.Add(new SqlParameter("@EndDate", enddt));
            //cmdpdfj.Parameters.Add(new SqlParameter("@CompID", CompID));
            //SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);

            //cmdpdfj.Connection = conpdfj;
            //SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);
            //DataTable dttablej = new DataTable("Inv");
            //sda.Fill(dttablej);

            rg1.Rows.Add(rowheadertable1);

            IEnumerable itemsSource1 = StockInventSummaryGrid.ItemsSource as IEnumerable;
            if (itemsSource1 != null)
            {
                // foreach (var item in itemsSource)
                for (int k = 0; k < dttablej.Rows.Count; ++k)
                {
                    TableRow rowone = new TableRow();

                    // rowone.Background = Brushes.Silver;
                    rowone.FontSize = 9;
                    rowone.FontWeight = FontWeights.Regular;
                    rowone.FontFamily = new FontFamily("Century Gothic");

                    for (int i = 0; i < dttablej.Columns.Count; ++i)
                    {

                        TableCell firstcolproductcell = new TableCell(new System.Windows.Documents.Paragraph(new Run(dttablej.Rows[k][i].ToString())));
                        if (i == 0)
                        {
                            firstcolproductcell.ColumnSpan = 2;
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
            totalValParag.FontSize = 11;
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
            ts11gTotaoBeforeDisc = new Span(new Run("\t Total Gold(gms):" + goldInInvent.Text + " Qty: " + goldInInventQty.Text +"     "));
            ts11gTotaoBeforeDisc.Inlines.Add(new LineBreak());//Line break is used for next line.  
            //}

            Span ts11gDiscAmountItemTotal = new Span();

            ts11gDiscAmountItemTotal = new Span(new Run("\t Total Silver(gms):" + silverInInvent.Text + " Qty: " + silverInInventQty.Text + "     "));
            ts11gDiscAmountItemTotal.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span tsTotalTaxableAmt = new Span();
            tsTotalTaxableAmt = new Span(new Run("\t Total Old Gold(gms):" + oldGoldInInvent.Text + "     "));
            tsTotalTaxableAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span tsTotalOldSilver = new Span();
            tsTotalOldSilver = new Span(new Run("\t Total Old Silver(gms):" + oldSilverInInvent.Text + "     "));
            tsTotalOldSilver.Inlines.Add(new LineBreak());//Line break is used for next line.  



            //Span tsTotalTaxableAmt = new Span();
            //tsTotalTaxableAmt = new Span(new Run("\t Total Old Gold Buy :" + "₹ " + oldGoldIn.Text));
            //tsTotalTaxableAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  



            totalVaGrand.FontSize = 11;
            totalVaGrand.FontFamily = new FontFamily("Century Gothic");
            totalVaGrand.Inlines.Add(ts11gTotaoBeforeDisc);// Add the span content into paragraph.  
            totalVaGrand.Inlines.Add(ts11gDiscAmountItemTotal);
            //totalVaGrand.Inlines.Add(tsMakingCharge);
            totalVaGrand.Inlines.Add(tsTotalTaxableAmt);
            totalVaGrand.Inlines.Add(tsTotalOldSilver);

            //totalVal.Inlines.Add(ali5);// Add the span content into paragraph.  
            totalVaGrand.TextAlignment = TextAlignment.Right;

            totalVaGrand.FontWeight = FontWeights.Bold;
            //doc.Blocks.Add(totalVaGrand);


            System.Windows.Documents.Paragraph totalVaGrand1 = new System.Windows.Documents.Paragraph();
            //totalValold.FontFamily 

            Span ts11gTotaoBeforeDisc1 = new Span();
            //if (totalValBeforeItemDis > 0)
            //{
            ts11gTotaoBeforeDisc1 = new Span(new Run("\t"));
            ts11gTotaoBeforeDisc1.Inlines.Add(new LineBreak());//Line break is used for next line.  
            //}

            Span ts11gDiscAmountItemTotal1 = new Span();

            ts11gDiscAmountItemTotal1 = new Span(new Run("\t "));
            ts11gDiscAmountItemTotal1.Inlines.Add(new LineBreak());//Line break is used for next line.  


            //Span tsTotalTaxableAmt1 = new Span();
            //tsTotalTaxableAmt1 = new Span(new Run("\t Total Old Gold Buy(gms):" + oldGoldIn.Text));
            //tsTotalTaxableAmt1.Inlines.Add(new LineBreak());//Line break is used for next line.  


            //Span tsTotalOldSilver11 = new Span();
            //tsTotalOldSilver11 = new Span(new Run("\t Total Old Silver Buy(gms):" + oldSilverIn.Text));
            //tsTotalOldSilver11.Inlines.Add(new LineBreak());//Line break is used for next line.  



            //Span tsTotalTaxableAmt = new Span();
            //tsTotalTaxableAmt = new Span(new Run("\t Total Old Gold Buy :" + "₹ " + oldGoldIn.Text));
            //tsTotalTaxableAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  



            totalVaGrand1.FontSize = 11;
            totalVaGrand1.FontFamily = new FontFamily("Century Gothic");
            totalVaGrand1.Inlines.Add(ts11gTotaoBeforeDisc1);// Add the span content into paragraph.  
            totalVaGrand1.Inlines.Add(ts11gDiscAmountItemTotal1);
            //totalVaGrand.Inlines.Add(tsMakingCharge);
            //totalVaGrand1.Inlines.Add(tsTotalTaxableAmt);
            //totalVaGrand1.Inlines.Add(tsTotalOldSilver);

            //totalVal.Inlines.Add(ali5);// Add the span content into paragraph.  
            totalVaGrand1.TextAlignment = TextAlignment.Left;

            totalVaGrand1.FontWeight = FontWeights.Bold;


            TableRow rowtwocompleteTable = new TableRow();

            TableRow rowthreecompleteTable = new TableRow();

            //-------------
            System.Windows.Documents.Table colTableAdd = new System.Windows.Documents.Table();
            var rg1tb = new TableRowGroup();
            TableRow rowColCellheadertable = new TableRow();
            //rowColCellheadertable.Background = Brushes.Silver;
            rowColCellheadertable.FontSize = 11;
            rowColCellheadertable.FontFamily = new FontFamily("Century Gothic");
            rowColCellheadertable.FontWeight = FontWeights.Bold;

            ThicknessConverter tc222tbc = new ThicknessConverter();

            TableCell tcellfirstTb = new TableCell(totalVaGrand1);

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
            doc.PagePadding = new Thickness(20, 20, 20, 5); //v3
            //doc.PagePadding = new Thickness(30, 20, 10, 5); //V2 
            // Create IDocumentPaginatorSource from FlowDocument
            // IDocumentPaginatorSource idpSource = doc;
            // Call PrintDocument method to send document to printer



            return doc;


        }


        private void printAllAccountsLedgerDaybook_Click(object sender, RoutedEventArgs e)
        {
            string sdt = startDtAllLedger.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDtAllLedger.SelectedDate);
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

            string enddt = toDtAllLedger.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDtAllLedger.SelectedDate);
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






            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintQueue = System.Printing.LocalPrintServer.GetDefaultPrintQueue();
            printDlg.PrintTicket = printDlg.PrintQueue.DefaultPrintTicket;
            printDlg.PrintTicket.PageOrientation = PageOrientation.Portrait;

            // Create a FlowDocument dynamically.
            //FlowDocument doc = CreateFlowDocumentJewellery();
            FlowDocument doc = CreateFlowDocumentAllAccountsLedgerDaybook();
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

        private FlowDocument CreateFlowDocumentAllAccountsLedgerDaybook()
        {
            //  Get Confirmation that data saved successfull, 

            string sdt = startDtAllLedger.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDtAllLedger.SelectedDate);
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

            string enddt = toDtAllLedger.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDtAllLedger.SelectedDate);
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
            a3 = new Span(new Run("Day Transaction Summary"));
            a3.FontWeight = FontWeights.Bold;
            a3.Inlines.Add(new LineBreak());//Line break is used for next line.  

            //Span a4 = new Span();
            //a4 = new Span(new Run("Invoice# " + invoiceNumber.Text));
            //a4.FontWeight = FontWeights.Bold;
            //a4.Inlines.Add(new LineBreak());//Line break is used for next line.  

            //Span a4acc = new Span();
            //a4acc = new Span(new Run("M/S. " + autocompltCustName.autoTextBox.Text));
            ////a4acc.FontWeight = FontWeights.Bold;
            //a4acc.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span a4date = new Span();
            a4date = new Span(new Run("Period: From- " + sdt + " To- " + enddt));
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
            //p.Inlines.Add(a4acc);// Add the span content into paragraph.  
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
            for (int i = 0; i < StockSummaryGrid.Items.Count; i++)
            {
                //TableColumn tc = new TableColumn();

                t5.Columns.Add(new TableColumn() { Width = GridLength.Auto });

            }

            ThicknessConverter tc1 = new ThicknessConverter();
            //// Create Table Borders
            t5.BorderThickness = (Thickness)tc1.ConvertFromString("0.02in");

            int count1 = StockSummaryGrid.Items.Count;
            var rg1 = new TableRowGroup();

            TableRow rowheadertable1 = new TableRow();



            rowheadertable1.Background = Brushes.Silver;
            rowheadertable1.FontSize = 9;
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

            TableCell tcell3 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Date")));
            //tcell3.ColumnSpan = 3;
            tcell3.BorderBrush = Brushes.Black;
            tcell3.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell3);

            TableCell tcell4 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Particulars")));
            tcell4.ColumnSpan = 2;
            tcell4.BorderBrush = Brushes.Black;
            tcell4.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell4);

            TableCell tcell5 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Vch Type")));
            //tcell5.ColumnSpan = 3;
            tcell5.BorderBrush = Brushes.Black;
            tcell5.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell5);

            TableCell tcell6 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Vch No.")));
            //tcell6.ColumnSpan = 3;
            tcell6.BorderBrush = Brushes.Black;
            tcell6.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell6);

            TableCell tcell7 = new TableCell(new System.Windows.Documents.Paragraph(new Run("CR Amt/Outwards Qty"))); 
            //tcell7.ColumnSpan = 3;
            tcell7.BorderBrush = Brushes.Black;
            tcell7.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell7);

            TableCell tcell8 = new TableCell(new System.Windows.Documents.Paragraph(new Run("DR Amt/Inwards Qty")));
            //tcell8.ColumnSpan = 3;
            tcell8.BorderBrush = Brushes.Black;
            tcell8.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell8);

            //TableCell tcell9 = new TableCell(new System.Windows.Documents.Paragraph(new Run("GST(%)")));
            ////tcell9.ColumnSpan = 3;
            //tcell9.BorderBrush = Brushes.Black;
            //tcell9.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell9);

            //TableCell tcell10 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Waste")));
            ////tcell10.ColumnSpan = 3;
            //tcell10.BorderBrush = Brushes.Black;
            //tcell10.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell10);

            //TableCell tcell11 = new TableCell(new System.Windows.Documents.Paragraph(new Run("GrossWt")));
            ////tcell11.ColumnSpan = 3;
            //tcell11.BorderBrush = Brushes.Black;
            //tcell11.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell11);

            //TableCell tcell12 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Price")));
            ////tcell11.ColumnSpan = 3;
            //tcell12.BorderBrush = Brushes.Black;
            //tcell12.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell12);

            //TableCell tcell13 = new TableCell(new System.Windows.Documents.Paragraph(new Run("GST%")));
            ////tcell11.ColumnSpan = 3;
            //tcell13.BorderBrush = Brushes.Black;
            //tcell13.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell13);

            //TableCell tcell14 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Tax")));
            ////tcell11.ColumnSpan = 3;
            //tcell14.BorderBrush = Brushes.Black;
            //tcell14.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell14);

            //TableCell tcell15 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Dis")));
            ////tcell11.ColumnSpan = 3;
            //tcell15.BorderBrush = Brushes.Black;
            //tcell15.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell15);

            //TableCell tcell16 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Taxable")));
            ////tcell11.ColumnSpan = 3;
            //tcell16.BorderBrush = Brushes.Black;
            //tcell16.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell16);

            //TableCell tcell17 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Total")));
            ////tcell11.ColumnSpan = 3;
            //tcell17.BorderBrush = Brushes.Black;
            //tcell17.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell17);


            ////TableCell tcell18 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Labour")));
            //////tcell11.ColumnSpan = 3;
            ////tcell18.BorderBrush = Brushes.Black;
            ////tcell18.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            ////rowheadertable1.Cells.Add(tcell18);

            //TableCell tcell19 = new TableCell(new System.Windows.Documents.Paragraph(new Run("MC")));
            ////tcell11.ColumnSpan = 3;
            //tcell19.BorderBrush = Brushes.Black;
            //tcell19.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell19);

            //TableCell tcell20 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Date")));
            ////tcell11.ColumnSpan = 3;
            //tcell20.BorderBrush = Brushes.Black;
            //tcell20.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell20);

            using (SqlConnection conDRCR = new SqlConnection())
            {
                conDRCR.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                conDRCR.Open();

                SqlCommand comDRCR = new SqlCommand("GetDaybookDualEntrySummaryTotalCRDR", conDRCR);
                comDRCR.CommandType = CommandType.StoredProcedure;
                comDRCR.Parameters.Add(new SqlParameter("@StartDate", sdt));
                comDRCR.Parameters.Add(new SqlParameter("@EndDate", enddt));
                comDRCR.Parameters.Add(new SqlParameter("@CompID", CompID));
                //SqlDataAdapter sdaDRCR = new SqlDataAdapter(com);
                SqlDataReader readerDRCR = comDRCR.ExecuteReader();
                while (readerDRCR.Read())
                {
                    double dCrAcctLedgerAmt = (readerDRCR["TotalCredit"] != DBNull.Value) ? (readerDRCR.GetDouble(0)) : 0;
                    double dDrAcctLedgerAmt = (readerDRCR["TotalDebit"] != DBNull.Value) ? (readerDRCR.GetDouble(1)) : 0;
                    //double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    //double opBalBookStart = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;

                    totalDaybookCR.Text = dCrAcctLedgerAmt.ToString();
                    totalDaybookDR.Text = dDrAcctLedgerAmt.ToString();
                }
            }



            SqlConnection conpdfj = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            SqlCommand cmdpdfj = new SqlCommand("GetDaybookDualEntrySummary", conpdfj);
            cmdpdfj.CommandType = CommandType.StoredProcedure;
            //cmdpdfj.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
            cmdpdfj.Parameters.Add(new SqlParameter("@StartDate", sdt));
            cmdpdfj.Parameters.Add(new SqlParameter("@EndDate", enddt));
            cmdpdfj.Parameters.Add(new SqlParameter("@CompID", CompID));
            SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);

            cmdpdfj.Connection = conpdfj;
            //SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);
            DataTable dttablej = new DataTable("Inv");
            sda.Fill(dttablej);

            rg1.Rows.Add(rowheadertable1);

            IEnumerable itemsSource1 = AllAccointsSummaryGrid.ItemsSource as IEnumerable;
            if (itemsSource1 != null)
            {
                // foreach (var item in itemsSource)
                for (int k = 0; k < dttablej.Rows.Count; ++k)
                {
                    TableRow rowone = new TableRow();

                    // rowone.Background = Brushes.Silver;
                    rowone.FontSize = 9;
                    rowone.FontWeight = FontWeights.Regular;
                    rowone.FontFamily = new FontFamily("Century Gothic");

                    for (int i = 0; i < dttablej.Columns.Count; ++i)
                    {

                        TableCell firstcolproductcell = new TableCell(new System.Windows.Documents.Paragraph(new Run(dttablej.Rows[k][i].ToString())));
                        if (i == 1)
                        {
                            firstcolproductcell.ColumnSpan = 2;
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
            totalValParag.FontSize = 11;
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
            ts11gTotaoBeforeDisc = new Span(new Run("\t Total Credit:₹ " + totalDaybookCR.Text+"                                         "));
            ts11gTotaoBeforeDisc.Inlines.Add(new LineBreak());//Line break is used for next line.  
            //}

            Span ts11gDiscAmountItemTotal = new Span();

            ts11gDiscAmountItemTotal = new Span(new Run("\t Total Debit:₹ " + totalDaybookDR.Text + "          "));
            ts11gDiscAmountItemTotal.Inlines.Add(new LineBreak());//Line break is used for next line.  


            //Span tsTotalTaxableAmt = new Span();
            //tsTotalTaxableAmt = new Span(new Run("\t Total Old Gold(gms):" + oldGoldInInvent.Text + "     "));
            //tsTotalTaxableAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  


            //Span tsTotalOldSilver = new Span();
            //tsTotalOldSilver = new Span(new Run("\t Total Old Silver(gms):" + oldSilverInInvent.Text + "     "));
            //tsTotalOldSilver.Inlines.Add(new LineBreak());//Line break is used for next line.  



            //Span tsTotalTaxableAmt = new Span();
            //tsTotalTaxableAmt = new Span(new Run("\t Total Old Gold Buy :" + "₹ " + oldGoldIn.Text));
            //tsTotalTaxableAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  



            totalVaGrand.FontSize = 11;
            totalVaGrand.FontFamily = new FontFamily("Century Gothic");
            totalVaGrand.Inlines.Add(ts11gTotaoBeforeDisc);// Add the span content into paragraph.  
            totalVaGrand.Inlines.Add(ts11gDiscAmountItemTotal);
            //totalVaGrand.Inlines.Add(tsMakingCharge);
            //totalVaGrand.Inlines.Add(tsTotalTaxableAmt);
            //totalVaGrand.Inlines.Add(tsTotalOldSilver);

            //totalVal.Inlines.Add(ali5);// Add the span content into paragraph.  
            totalVaGrand.TextAlignment = TextAlignment.Right;

            totalVaGrand.FontWeight = FontWeights.Bold;
            //doc.Blocks.Add(totalVaGrand);


            System.Windows.Documents.Paragraph totalVaGrand1 = new System.Windows.Documents.Paragraph();
            //totalValold.FontFamily 

            Span ts11gTotaoBeforeDisc1 = new Span();
            //if (totalValBeforeItemDis > 0)
            //{
            ts11gTotaoBeforeDisc1 = new Span(new Run("\t"));
            ts11gTotaoBeforeDisc1.Inlines.Add(new LineBreak());//Line break is used for next line.  
            //}

            Span ts11gDiscAmountItemTotal1 = new Span();

            ts11gDiscAmountItemTotal1 = new Span(new Run("\t "));
            ts11gDiscAmountItemTotal1.Inlines.Add(new LineBreak());//Line break is used for next line.  


            //Span tsTotalTaxableAmt1 = new Span();
            //tsTotalTaxableAmt1 = new Span(new Run("\t Total Old Gold Buy(gms):" + oldGoldIn.Text));
            //tsTotalTaxableAmt1.Inlines.Add(new LineBreak());//Line break is used for next line.  


            //Span tsTotalOldSilver11 = new Span();
            //tsTotalOldSilver11 = new Span(new Run("\t Total Old Silver Buy(gms):" + oldSilverIn.Text));
            //tsTotalOldSilver11.Inlines.Add(new LineBreak());//Line break is used for next line.  



            //Span tsTotalTaxableAmt = new Span();
            //tsTotalTaxableAmt = new Span(new Run("\t Total Old Gold Buy :" + "₹ " + oldGoldIn.Text));
            //tsTotalTaxableAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  



            totalVaGrand1.FontSize = 11;
            totalVaGrand1.FontFamily = new FontFamily("Century Gothic");
            totalVaGrand1.Inlines.Add(ts11gTotaoBeforeDisc1);// Add the span content into paragraph.  
            totalVaGrand1.Inlines.Add(ts11gDiscAmountItemTotal1);
            //totalVaGrand.Inlines.Add(tsMakingCharge);
            //totalVaGrand1.Inlines.Add(tsTotalTaxableAmt);
            //totalVaGrand1.Inlines.Add(tsTotalOldSilver);

            //totalVal.Inlines.Add(ali5);// Add the span content into paragraph.  
            totalVaGrand1.TextAlignment = TextAlignment.Left;

            totalVaGrand1.FontWeight = FontWeights.Bold;


            TableRow rowtwocompleteTable = new TableRow();

            TableRow rowthreecompleteTable = new TableRow();
            TableRow rowVoucherWisecompleteTable = new TableRow();
            TableRow rowAccountWisecompleteTable = new TableRow(); 


            //-------------
            System.Windows.Documents.Table colTableAdd = new System.Windows.Documents.Table();
            System.Windows.Documents.Table colTableAddVoucherWise = new System.Windows.Documents.Table();
            System.Windows.Documents.Table colTableAddAccountWise = new System.Windows.Documents.Table();
            var rg1tb = new TableRowGroup();
            var rg1tbVoucherWise = new TableRowGroup();
            var rg1tbAccountWise = new TableRowGroup();
            TableRow rowColCellheadertable = new TableRow();
            //rowColCellheadertable.Background = Brushes.Silver;
            rowColCellheadertable.FontSize = 11;
            rowColCellheadertable.FontFamily = new FontFamily("Century Gothic");
            rowColCellheadertable.FontWeight = FontWeights.Bold;

            ThicknessConverter tc222tbc = new ThicknessConverter();

            TableCell tcellfirstTb = new TableCell(totalVaGrand1);

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

            ////////////////
            TableRow rowheadertableAccountWise = new TableRow();

            rowheadertableAccountWise.Background = Brushes.Silver;
            rowheadertableAccountWise.FontSize = 9;
            rowheadertableAccountWise.FontFamily = new FontFamily("Century Gothic");
            rowheadertableAccountWise.FontWeight = FontWeights.Bold;

            //ThicknessConverter tc222 = new ThicknessConverter();

            TableCell tcell3AcctWise = new TableCell(new System.Windows.Documents.Paragraph(new Run("Particular")));
            tcell3AcctWise.ColumnSpan = 4;
            tcell3AcctWise.BorderBrush = Brushes.Black;
            tcell3AcctWise.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertableAccountWise.Cells.Add(tcell3AcctWise);

            TableCell tcell4AcctWise = new TableCell(new System.Windows.Documents.Paragraph(new Run("DR-Balance")));
            //tcell4AcctWise.ColumnSpan = 2;
            tcell4AcctWise.BorderBrush = Brushes.Black;
            tcell4AcctWise.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertableAccountWise.Cells.Add(tcell4AcctWise);

            TableCell tcell5AcctWise = new TableCell(new System.Windows.Documents.Paragraph(new Run("CR")));
            //tcell5.ColumnSpan = 3;
            tcell5AcctWise.BorderBrush = Brushes.Black;
            tcell5AcctWise.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertableAccountWise.Cells.Add(tcell5AcctWise);

            TableCell tcell6AcctWise = new TableCell(new System.Windows.Documents.Paragraph(new Run("DR")));
            //tcell5.ColumnSpan = 3;
            tcell6AcctWise.BorderBrush = Brushes.Black;
            tcell6AcctWise.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertableAccountWise.Cells.Add(tcell6AcctWise);


            ////////////////
            SqlConnection conpdfjVch = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            SqlCommand cmdpdfjVch = new SqlCommand("GetDaybookDualEntrySummaryTotalCRDBreakups", conpdfjVch);
            cmdpdfjVch.CommandType = CommandType.StoredProcedure;
            //cmdpdfj.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
            cmdpdfjVch.Parameters.Add(new SqlParameter("@StartDate", sdt));
            cmdpdfjVch.Parameters.Add(new SqlParameter("@EndDate", enddt));
            cmdpdfjVch.Parameters.Add(new SqlParameter("@CompID", CompID));
            SqlDataAdapter sdaVch = new SqlDataAdapter(cmdpdfjVch);

            cmdpdfjVch.Connection = conpdfjVch;
            //SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);
            DataTable dttablejVch = new DataTable("Inv");
            sdaVch.Fill(dttablejVch);

            //rg1.Rows.Add(rowheadertable1);

            //IEnumerable itemsSource1 = AllAccointsSummaryGrid.ItemsSource as IEnumerable;
            //if (dttable != null)
            //{
                // foreach (var item in itemsSource)
            for (int k = 0; k < dttablejVch.Rows.Count; ++k)
                {
                    TableRow rowColCellheadertableVoucherWise = new TableRow();

                    // rowone.Background = Brushes.Silver;
                    rowColCellheadertableVoucherWise.FontSize = 9;
                    rowColCellheadertableVoucherWise.FontWeight = FontWeights.Regular;
                    rowColCellheadertableVoucherWise.FontFamily = new FontFamily("Century Gothic");

                    for (int i = 0; i < dttablejVch.Columns.Count; ++i)
                    {

                        TableCell tcellfirstTbVoucherWise = new TableCell(new System.Windows.Documents.Paragraph(new Run(dttablejVch.Rows[k][i].ToString())));
                        if (i == 0)
                        {
                            tcellfirstTbVoucherWise.ColumnSpan = 5;
                        }
                        tcellfirstTbVoucherWise.BorderBrush = Brushes.Black;
                        tcellfirstTbVoucherWise.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
                        // rowone.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run((k + 1).ToString()))));
                        rowColCellheadertableVoucherWise.Cells.Add(tcellfirstTbVoucherWise);

                    }

                    rg1tbVoucherWise.Rows.Add(rowColCellheadertableVoucherWise);
                }

            SqlConnection conpdfjAcct = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            SqlCommand cmdpdfjAcct = new SqlCommand("GetDaybookDualEntrySummaryTotalCRDBreakupsAccountWise", conpdfjAcct);
            cmdpdfjAcct.CommandType = CommandType.StoredProcedure;
            //cmdpdfj.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
            cmdpdfjAcct.Parameters.Add(new SqlParameter("@StartDate", sdt));
            cmdpdfjAcct.Parameters.Add(new SqlParameter("@EndDate", enddt));
            cmdpdfjAcct.Parameters.Add(new SqlParameter("@CompID", CompID));
            SqlDataAdapter sdaAcct = new SqlDataAdapter(cmdpdfjAcct);

            cmdpdfjAcct.Connection = conpdfjAcct;
            //SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);
            DataTable dttablejAcct = new DataTable("Inv");
            sdaAcct.Fill(dttablejAcct);

            //rg1.Rows.Add(rowheadertable1);

            //IEnumerable itemsSource1 = AllAccointsSummaryGrid.ItemsSource as IEnumerable;
            //if (dttable != null)
            //{
            // foreach (var item in itemsSource)

            rg1tbAccountWise.Rows.Add(rowheadertableAccountWise);

            for (int k = 0; k < dttablejAcct.Rows.Count; ++k)
            {
                TableRow rowColCellheadertableAccountWise = new TableRow();

                // rowone.Background = Brushes.Silver;
                rowColCellheadertableAccountWise.FontSize = 9;
                rowColCellheadertableAccountWise.FontWeight = FontWeights.Regular;
                rowColCellheadertableAccountWise.FontFamily = new FontFamily("Century Gothic");

                for (int i = 0; i < dttablejAcct.Columns.Count; ++i)
                {

                    TableCell tcellfirstTbAccountWise = new TableCell(new System.Windows.Documents.Paragraph(new Run(dttablejAcct.Rows[k][i].ToString())));
                    tcellfirstTbAccountWise.BorderBrush = Brushes.Black;
                    if (i == 0)
                    {
                        tcellfirstTbAccountWise.ColumnSpan = 4;
                        
                    }
                    
                    tcellfirstTbAccountWise.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
                    // rowone.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run((k + 1).ToString()))));
                    rowColCellheadertableAccountWise.Cells.Add(tcellfirstTbAccountWise);

                }

                rg1tbAccountWise.Rows.Add(rowColCellheadertableAccountWise);
            }


            //}

            ///////////////

            //rg1tbVoucherWise.Rows.Add(rowColCellheadertableVoucherWise);
            colTableAddVoucherWise.RowGroups.Add(rg1tbVoucherWise);

            //rg1tbAccountWise.Rows.Add(rowColCellheadertableAccountWise);
            colTableAddAccountWise.RowGroups.Add(rg1tbAccountWise);

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


            TableCell txtcellVoucherWisecompleteTable = new TableCell(colTableAddVoucherWise);
            txtcellVoucherWisecompleteTable.BorderBrush = Brushes.Black;
            txtcellVoucherWisecompleteTable.BorderThickness = (Thickness)tc22234completeTable.ConvertFromString("0.0001in");


            TableCell txtcellAccountWisecompleteTable = new TableCell(colTableAddAccountWise);
            txtcellAccountWisecompleteTable.BorderBrush = Brushes.Black;
            txtcellAccountWisecompleteTable.BorderThickness = (Thickness)tc22234completeTable.ConvertFromString("0.0001in");



            //TableCell txtcell31completeTable = new TableCell(totalVaGrand);
            //txtcell31completeTable.BorderBrush = Brushes.Black;
            //txtcell31completeTable.BorderThickness = (Thickness)tc22234completeTable.ConvertFromString("0.0001in");

            TableCell txtcellOldcompleteTable = new TableCell(t4);
            txtcellOldcompleteTable.BorderBrush = Brushes.Black;
            txtcellOldcompleteTable.BorderThickness = (Thickness)tc22234completeTable.ConvertFromString("0.0001in");

            rowoncompleteTable.Cells.Add(txtcellcompleteTable);
            rowtwocompleteTable.Cells.Add(txtcell2completeTable);
            rowthreecompleteTable.Cells.Add(txtcell3completeTable);

            rowVoucherWisecompleteTable.Cells.Add(txtcellVoucherWisecompleteTable);
            rowAccountWisecompleteTable.Cells.Add(txtcellAccountWisecompleteTable);


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

            rowgrpcompleteTable.Rows.Add(rowVoucherWisecompleteTable);

            rowgrpcompleteTable.Rows.Add(rowAccountWisecompleteTable);

            completeTable.RowGroups.Add(rowgrpcompleteTable);

            completeTable.Padding = new Thickness(10);
            doc.Blocks.Add(completeTable);

            //doc.Blocks.Add(linedot);

            System.Windows.Documents.Paragraph signpara = new System.Windows.Documents.Paragraph();

            Span linebrktble1 = new Span();
            linebrktble1 = new Span(new Run("Signed By         "));
            // linebrktble.Inlines.Add(new LineBreak());//Line break is used for next line.  

            signpara.FontSize = 13;

            signpara.Inlines.Add(linebrktble1);// Add the span content into paragraph.  
            signpara.TextAlignment = TextAlignment.Right;
            //linedot.Inlines.Add(linebrktble1);// Add the span content into paragraph.  
            //doc.Blocks.Add(linedot);
            doc.Blocks.Add(signpara);


            doc.Name = "FlowDoc";
            //doc.PageWidth = 900;
            doc.PagePadding = new Thickness(20, 20, 20, 5); //v3
            //doc.PagePadding = new Thickness(30, 20, 10, 5); //V2 
            // Create IDocumentPaginatorSource from FlowDocument
            // IDocumentPaginatorSource idpSource = doc;
            // Call PrintDocument method to send document to printer



            return doc;


        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


  
        /// <summary>
        /// Handles the TextChanged event of the autoTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>

         private void TabListAccts_Selected(object sender, RoutedEventArgs e)
        {
           CountryList = new List<string>();

            //If a product code is not empty we search the database

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            //string sql = "select AcctName from AccountsList where CompID = '" + CompID + "'";
            string sql = "select Distinct  AcctName from AccountsList where CompID = '" + CompID + "'";
            SqlCommand cmd = new SqlCommand(sql);

            cmd.Connection = con;
            SqlDataReader reader = cmd.ExecuteReader();




            while (reader.Read())
            {

                CountryList.Add(reader.GetValue(0).ToString().Trim());

            }
            reader.Close();
            listbox.ItemsSource = CountryList;

         }

         private void TabSale_SelectedTrayBook(object sender, RoutedEventArgs e)
         {
             if (cmbTrayLists.Text != "")
             {

                 string trayenameselected = cmbTrayLists.SelectedItem.ToString();
                 string sdt = startDateSaleTrayBook.SelectedDate.ToString();
                 // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
                 DateTime dt = Convert.ToDateTime(startDateSaleTrayBook.SelectedDate);
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

                 string enddt = toDateSaleTrayBook.SelectedDate.ToString();
                 DateTime edt = Convert.ToDateTime(toDateSaleTrayBook.SelectedDate);
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

                     SqlCommand com = new SqlCommand("GetTrayBookbyPeriod", con);
                     com.CommandType = CommandType.StoredProcedure;
                     com.Parameters.Add(new SqlParameter("@TrayName", trayenameselected.Trim()));
                     com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                     com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                     com.Parameters.Add(new SqlParameter("@CompID", CompID));
                     SqlDataAdapter sda = new SqlDataAdapter(com);
                     //SqlDataReader reader = com.ExecuteReader();        

                     System.Data.DataTable dt1 = new System.Data.DataTable("Tray Book");
                     sda.Fill(dt1);
                     SaleSummaryGridTrayBook.ItemsSource = dt1.DefaultView;
                     SaleSummaryGridTrayBook.AutoGenerateColumns = true;
                     SaleSummaryGridTrayBook.CanUserAddRows = false;

                     double sumDr = 0;
                     double sumCr = 0;
                     foreach (DataRow row in dt1.Rows)
                     {
                         //sumDr +=  Convert.ToDouble(row["DR"]);
                         sumDr = sumDr + ((row["OutQty"] != DBNull.Value) ? (Convert.ToDouble(row["OutQty"])) : 0);
                         sumCr = sumCr + ((row["OutWeight"] != DBNull.Value) ? (Convert.ToDouble(row["OutWeight"])) : 0);
                     }
                     totalQtyTayBook.Text = sumDr.ToString();
                     totalWeightTrayBook.Text = sumCr.ToString();
                     //Balance_Ledger.Text = (sumCr - sumDr).ToString();

                 }
             }
         }

        private void OnSelectedAllItems(object sender, RoutedEventArgs e)
        {
            listbox.SelectAll();
        }
        private void OnUnSelectedAllItems(object sender, RoutedEventArgs e)
        {
            listbox.UnselectAll();
        }

        private void cmbTrayLists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTrayLists.Text != "")
            {

                string trayenameselected = cmbTrayLists.SelectedItem.ToString();

                string sdt = startDateSaleTrayBook.SelectedDate.ToString();
                // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
                DateTime dt = Convert.ToDateTime(startDateSaleTrayBook.SelectedDate);
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

                string enddt = toDateSaleTrayBook.SelectedDate.ToString();
                DateTime edt = Convert.ToDateTime(toDateSaleTrayBook.SelectedDate);
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

                    SqlCommand com = new SqlCommand("GetTrayBookbyPeriod", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add(new SqlParameter("@TrayName", trayenameselected.Trim()));
                    com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                    com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                    com.Parameters.Add(new SqlParameter("@CompID", CompID));
                    SqlDataAdapter sda = new SqlDataAdapter(com);
                    //SqlDataReader reader = com.ExecuteReader();        

                    System.Data.DataTable dt1 = new System.Data.DataTable("Tray Book");
                    sda.Fill(dt1);
                    SaleSummaryGridTrayBook.ItemsSource = dt1.DefaultView;
                    SaleSummaryGridTrayBook.AutoGenerateColumns = true;
                    SaleSummaryGridTrayBook.CanUserAddRows = false;

                    double sumDr = 0;
                    double sumCr = 0;
                    foreach (DataRow row in dt1.Rows)
                    {
                        //sumDr +=  Convert.ToDouble(row["DR"]);
                        sumDr = sumDr + ((row["OutQty"] != DBNull.Value) ? (Convert.ToDouble(row["OutQty"])) : 0);
                        sumCr = sumCr + ((row["OutWeight"] != DBNull.Value) ? (Convert.ToDouble(row["OutWeight"])) : 0);
                    }
                    totalQtyTayBook.Text = sumDr.ToString();
                    totalWeightTrayBook.Text = sumCr.ToString();
                    //Balance_Ledger.Text = (sumCr - sumDr).ToString();


                }
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                    con.Open();

                    SqlCommand com = new SqlCommand("GetTrayBookbyPeriodStock", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.Add(new SqlParameter("@TrayName", trayenameselected.Trim()));
                    com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                    com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                    com.Parameters.Add(new SqlParameter("@CompID", CompID));
                    SqlDataAdapter sda = new SqlDataAdapter(com);
                    //SqlDataReader reader = com.ExecuteReader();        

                    System.Data.DataTable dt1 = new System.Data.DataTable("Tray Book Stock");
                    sda.Fill(dt1);
                    SaleSummaryGridTrayBookStk.ItemsSource = dt1.DefaultView;
                    SaleSummaryGridTrayBookStk.AutoGenerateColumns = true;
                    SaleSummaryGridTrayBookStk.CanUserAddRows = false;

                    double sumDr = 0;
                    double sumCr = 0;
                    foreach (DataRow row in dt1.Rows)
                    {
                        //sumDr +=  Convert.ToDouble(row["DR"]);
                        sumDr = sumDr + ((row["Qty"] != DBNull.Value) ? (Convert.ToDouble(row["Qty"])) : 0);
                        sumCr = sumCr + ((row["Weight"] != DBNull.Value) ? (Convert.ToDouble(row["Weight"])) : 0);
                    }
                    totalQtyTayBookStk.Text = sumDr.ToString();
                    totalWeightTrayBookStk.Text = sumCr.ToString();
                    //Balance_Ledger.Text = (sumCr - sumDr).ToString();


                }
            }
        }

        private void TabStockRegister_Selected(object sender, RoutedEventArgs e)
        {
            //goldInInvent.Clear();
            //// goldOutInvent.Clear();
            //oldGoldInInvent.Clear();
            ////oldGoldOutInvent.Clear();
            //silverInInvent.Clear();
            ////  silverOutInvent.Clear();

            string sdt = startDateStockRegister.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateStockRegister.SelectedDate);
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
            //startDateStockRegister.Text = sdt;

            string enddt = toDateStockRegister.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateStockRegister.SelectedDate);
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

                SqlCommand com = new SqlCommand("GetStockRegister", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@ItemName", autocompleteItemName.autoTextBox1.Text.Trim())); 
                
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        

                System.Data.DataTable dt1 = new System.Data.DataTable("Account Ledger");
                sda.Fill(dt1);
                StockRegisterGrid.ItemsSource = dt1.DefaultView;
                StockRegisterGrid.AutoGenerateColumns = true;
                StockRegisterGrid.CanUserAddRows = false;
            }

            //using (SqlConnection con = new SqlConnection())
            //{
            //    con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
            //    con.Open();

            //    SqlCommand com = new SqlCommand("GetDaybookDualEntrySummaryTotalCRDR", con);
            //    com.CommandType = CommandType.StoredProcedure;
            //    com.Parameters.Add(new SqlParameter("@StartDate", sdt));
            //    com.Parameters.Add(new SqlParameter("@EndDate", enddt));
            //    com.Parameters.Add(new SqlParameter("@CompID", CompID));
            //    SqlDataAdapter sda = new SqlDataAdapter(com);
            //    SqlDataReader reader = com.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        double dCrAcctLedgerAmt = (reader["TotalCredit"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
            //        double dDrAcctLedgerAmt = (reader["TotalDebit"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
            //        //double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
            //        //double opBalBookStart = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;

            //        totalDaybookCR.Text = dCrAcctLedgerAmt.ToString();
            //        totalDaybookDR.Text = dDrAcctLedgerAmt.ToString();
            //    }
            //}


        }

        private void autocompleteItemName_LostFocus(object sender, RoutedEventArgs e)
        {
            //Window waitWindow = new Window { Height = 100, Width = 200, WindowStartupLocation = WindowStartupLocation.CenterScreen, WindowStyle = WindowStyle.None };
            //waitWindow.Content = new TextBlock { Text = "Please Wait", FontSize = 30, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };

            string sdt = startDateStockRegister.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateStockRegister.SelectedDate);
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

            string enddt = toDateStockRegister.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateStockRegister.SelectedDate);
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

                SqlCommand com = new SqlCommand("GetStockRegister", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@ItemName", autocompleteItemName.autoTextBox1.Text.Trim()));

                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        

                System.Data.DataTable dt1 = new System.Data.DataTable("Account Ledger");
                sda.Fill(dt1);
                StockRegisterGrid.ItemsSource = dt1.DefaultView;
                StockRegisterGrid.AutoGenerateColumns = true;
                StockRegisterGrid.CanUserAddRows = false;
            }
        }

        private void TabTrialBalance_Selected(object sender, RoutedEventArgs e)
        {
            //goldInInvent.Clear();
            //// goldOutInvent.Clear();
            //oldGoldInInvent.Clear();
            ////oldGoldOutInvent.Clear();
            //silverInInvent.Clear();
            ////  silverOutInvent.Clear();
            startDateTrialBalance.Text = startDateFinCurrentYr; // set current fin startdate

            string sdt = startDateTrialBalance.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateTrialBalance.SelectedDate);
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
            //startDateTrialBalance.Text = sdt;

            string enddt = toDateTrialBalance.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateTrialBalance.SelectedDate);
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

                SqlCommand com = new SqlCommand("GetTrialBalanceSummary", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@Type", cmbMainType.Text.Trim()));
                //com.Parameters.Add(new SqlParameter("@ItemName", autocompleteItemName.autoTextBox1.Text.Trim()));

                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        

                System.Data.DataTable dt1 = new System.Data.DataTable("Trial Balance");
                sda.Fill(dt1);
                TrialBalanceGrid.ItemsSource = dt1.DefaultView;
                TrialBalanceGrid.AutoGenerateColumns = true;
                TrialBalanceGrid.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    sumDr = sumDr + ((row["Debit"] != DBNull.Value) ? (Convert.ToDouble(row["Debit"])) : 0);
                    sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                }
                totalDRTrialBal.Text = sumDr.ToString();
                totalCRTrialBal.Text = sumCr.ToString();

            }

            //using (SqlConnection con = new SqlConnection())
            //{
            //    con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
            //    con.Open();

            //    SqlCommand com = new SqlCommand("GetDaybookDualEntrySummaryTotalCRDR", con);
            //    com.CommandType = CommandType.StoredProcedure;
            //    com.Parameters.Add(new SqlParameter("@StartDate", sdt));
            //    com.Parameters.Add(new SqlParameter("@EndDate", enddt));
            //    com.Parameters.Add(new SqlParameter("@CompID", CompID));
            //    SqlDataAdapter sda = new SqlDataAdapter(com);
            //    SqlDataReader reader = com.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        double dCrAcctLedgerAmt = (reader["TotalCredit"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
            //        double dDrAcctLedgerAmt = (reader["TotalDebit"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
            //        //double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
            //        //double opBalBookStart = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;

            //        totalDaybookCR.Text = dCrAcctLedgerAmt.ToString();
            //        totalDaybookDR.Text = dDrAcctLedgerAmt.ToString();
            //    }
            //}


        }



        private void Button_Click_StockRegister(object sender, RoutedEventArgs e)
        {
            string sdt = startDateStockRegister.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateStockRegister.SelectedDate);
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

            //sdt = (years-1) + "/" + 04 + "/" + 01;


            string enddt = toDateStockRegister.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateStockRegister.SelectedDate);
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

                SqlCommand com = new SqlCommand("GetStockRegister", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@ItemName", autocompleteItemName.autoTextBox1.Text.Trim()));

                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        

                System.Data.DataTable dt1 = new System.Data.DataTable("Account Ledger");
                sda.Fill(dt1);
                StockRegisterGrid.ItemsSource = dt1.DefaultView;
                StockRegisterGrid.AutoGenerateColumns = true;
                StockRegisterGrid.CanUserAddRows = false;

                double sumOp = 0;
                double sumIn = 0;
                double sumOut = 0;
                //for (int s = 0; s < AccountLedgerGrid.Items.Count - 1; s++ )
                //{
                //    sumDr += (double.Parse((AccountLedgerGrid.Columns[5].GetCellContent(AccountLedgerGrid.Items[s]) as TextBlock).Text));
                //}
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    sumOp = sumOp + ((row["OpBal"] != DBNull.Value) ? (Convert.ToDouble(row["OpBal"])) : 0);
                    sumIn = sumIn + ((row["InQty"] != DBNull.Value) ? (Convert.ToDouble(row["InQty"])) : 0);
                    sumOut = sumOut + ((row["OutQty"] != DBNull.Value) ? (Convert.ToDouble(row["OutQty"])) : 0);
                }
                totalQtyOpBalStockRegister.Text = sumOp.ToString();
                totalQtyInStockRegister.Text = sumIn.ToString();
                totalQtyOutStockRegister.Text = sumOut.ToString();
                totalQtyStockRegister.Text = (sumOp + sumIn - sumOut).ToString();


            }


            //using (SqlConnection con = new SqlConnection())
            //{



            //    con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
            //    con.Open();

            //    SqlCommand com = new SqlCommand("GetCashAccountLedgerSummarybyPeriod", con);
            //    com.CommandType = CommandType.StoredProcedure;
            //    com.Parameters.Add(new SqlParameter("@StartDate", sdt));
            //    com.Parameters.Add(new SqlParameter("@EndDate", enddt));
            //    com.Parameters.Add(new SqlParameter("@CompID", CompID));
            //    SqlDataAdapter sda = new SqlDataAdapter(com);
            //    SqlDataReader reader = com.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        double dDebtAcctLedgerAmt = (reader["DebtAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
            //        double dCredAcctLedgerAmt = (reader["CredAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
            //        double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
            //        double opBalBookStart = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;

            //        TotalDebitAmt.Text = dDebtAcctLedgerAmt.ToString();
            //        TotalCreditAmt.Text = dCredAcctLedgerAmt.ToString();
            //        Balance.Text = (dDebtAcctLedgerAmt - dCredAcctLedgerAmt).ToString();
            //        openingBal.Text = opBal.ToString();
            //        openingBalBookStart.Text = opBalBookStart.ToString();


            //        closingBalEndDate.Text = (opBal + Convert.ToDouble(Balance.Text)).ToString();

            //    }

            //}

        }

        private void Button_Click_TrialBalance(object sender, RoutedEventArgs e)
        {
            string sdt = startDateTrialBalance.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateTrialBalance.SelectedDate);
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

            string enddt = toDateTrialBalance.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateTrialBalance.SelectedDate);
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

                SqlCommand com = new SqlCommand("GetTrialBalanceSummary", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@Type", cmbMainType.Text.Trim()));

                //com.Parameters.Add(new SqlParameter("@ItemName", autocompleteItemName.autoTextBox1.Text.Trim()));

                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        

                System.Data.DataTable dt1 = new System.Data.DataTable("Trial Balance");
                sda.Fill(dt1);
                TrialBalanceGrid.ItemsSource = dt1.DefaultView;
                TrialBalanceGrid.AutoGenerateColumns = true;
                TrialBalanceGrid.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    sumDr = sumDr + ((row["Debit"] != DBNull.Value) ? (Convert.ToDouble(row["Debit"])) : 0);
                    sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                }
                totalDRTrialBal.Text = sumDr.ToString();
                totalCRTrialBal.Text = sumCr.ToString();

            }


            //using (SqlConnection con = new SqlConnection())
            //{



            //    con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
            //    con.Open();

            //    SqlCommand com = new SqlCommand("GetCashAccountLedgerSummarybyPeriod", con);
            //    com.CommandType = CommandType.StoredProcedure;
            //    com.Parameters.Add(new SqlParameter("@StartDate", sdt));
            //    com.Parameters.Add(new SqlParameter("@EndDate", enddt));
            //    com.Parameters.Add(new SqlParameter("@CompID", CompID));
            //    SqlDataAdapter sda = new SqlDataAdapter(com);
            //    SqlDataReader reader = com.ExecuteReader();
            //    while (reader.Read())
            //    {
            //        double dDebtAcctLedgerAmt = (reader["DebtAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
            //        double dCredAcctLedgerAmt = (reader["CredAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
            //        double opBal = (reader["OpeningBal"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
            //        double opBalBookStart = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;

            //        TotalDebitAmt.Text = dDebtAcctLedgerAmt.ToString();
            //        TotalCreditAmt.Text = dCredAcctLedgerAmt.ToString();
            //        Balance.Text = (dDebtAcctLedgerAmt - dCredAcctLedgerAmt).ToString();
            //        openingBal.Text = opBal.ToString();
            //        openingBalBookStart.Text = opBalBookStart.ToString();


            //        closingBalEndDate.Text = (opBal + Convert.ToDouble(Balance.Text)).ToString();

            //    }

            //}

        }
              



        private void SaleSummaryGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {



                var uiElement = e.OriginalSource as UIElement;
                if (e.Key == Key.Enter && uiElement != null)
                {
                        DataRowView row = (DataRowView)SaleSummaryGrid.SelectedItems[0];
                        string invoiceNumber = row["Invoice Number"].ToString();
                        //string voucherNumbVa = row["Voucher Number"].ToString();
                        //string otherCharge = row["AnyotherCharges"].ToString();
                        //string statecodeCust = row["GSTIN"].ToString();
                        //statecodeCust = statecodeCust.Trim().Substring(0, 2);
                  
                        //SaleVoucherJewellLatha viewBillObj = new SaleVoucherJewellLatha();

                        if (saleHomeIcon == "SaleVoucherJewellLatha")
                        {
                            NavigationWindow navWIN = new NavigationWindow();
                            navWIN.Content = new SaleVoucherJewellLatha(invoiceNumber);
                            navWIN.Show(); 
                        }

                        if (saleHomeIcon == "SaleVoucherQtyGhansyam")
                        {
                            NavigationWindow navWIN = new NavigationWindow();
                            navWIN.Content = new SaleVoucherQtyGhansyam(invoiceNumber);
                            navWIN.Show(); 
                        }

                        if (saleHomeIcon == "SaleVoucherAllInOneQtyGSTSteel")
                        {
                            NavigationWindow navWIN = new NavigationWindow();
                            navWIN.Content = new SaleVoucherAllInOneQtyGSTSteel(invoiceNumber);
                            navWIN.Show();
                        }
                        //if (saleHomeIcon == "PurchaseQtyGSTVoucherxaml")
                        //{
                        //    NavigationWindow navWIN = new NavigationWindow();
                        //    navWIN.Content = new PurchaseQtyGSTVoucherxaml(invoiceNumber);
                        //    navWIN.Show();
                        //}







                    e.Handled = true;
                    //uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

                }

                
            }


        private void PurchaseSummaryGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {



            var uiElement = e.OriginalSource as UIElement;
            if (e.Key == Key.Enter && uiElement != null)
            {
                DataRowView row = (DataRowView)PurchaseSummaryGrid.SelectedItems[0];
                string invoiceNumber = row["Voucher Number"].ToString();
                //string customerName = row["CustomerName"].ToString();
                //string otherCharge = row["AnyotherCharges"].ToString();
                //string statecodeCust = row["GSTIN"].ToString();
                //statecodeCust = statecodeCust.Trim().Substring(0, 2);

                //SaleVoucherJewellLatha viewBillObj = new SaleVoucherJewellLatha();

                if (purchaseHomeIcon == "PurchaseVoucher")
                {
                    NavigationWindow navWIN = new NavigationWindow();
                    navWIN.Content = new PurchaseVoucher(invoiceNumber);
                    navWIN.Show();
                }

                //if (purchaseHomeIcon == "SaleVoucherQtyGhansyam")
                //{
                //    NavigationWindow navWIN = new NavigationWindow();
                //    navWIN.Content = new SaleVoucherQtyGhansyam(invoiceNumber);
                //    navWIN.Show();
                //}

                if (purchaseHomeIcon == "PurchaseQtyGSTVoucherxaml")
                {
                    NavigationWindow navWIN = new NavigationWindow();
                    navWIN.Content = new PurchaseQtyGSTVoucherxaml(invoiceNumber);
                    navWIN.Show();
                }







                e.Handled = true;
                //uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            }


        }


        private void exportTo_ClickCategory(object sender, RoutedEventArgs e)
        {

            try
            {
                string sdt = toDateSale.SelectedDate.ToString();
                // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
                DateTime dt = Convert.ToDateTime(toDateSale.SelectedDate);
                //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                int years = dt.Year;
                string months = dt.Month.ToString();



                SaleSummaryGrid.SelectAllCells();

                //GSTR1FullSummaryGrid.Columns.RemoveAt(0); // because of this it throw one column less when creating invoice as it deleted column 

                //DataTable tempDt = new DataTable();
                //tempDt = DataGridtoDataTable(GSTR1FullSummaryGrid);

                SaleSummaryGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                ApplicationCommands.Copy.Execute(null, SaleSummaryGrid);

                SaleSummaryGrid.UnselectAllCells();

                String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
                //int billno = Convert.ToInt16(invoiceNumber.Text.Trim());
                try
                {
                    StreamWriter sw = new StreamWriter(@"C:\ViewBill\\GST\Sale\Sale-Report-"+ months + "-" + years + ".csv");
                    sw.WriteLine(result);
                    sw.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("In Excel Export ");

            }

        }

        private void StockInventSummaryGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {



            var uiElement = e.OriginalSource as UIElement;
            if (e.Key == Key.Enter && uiElement != null)
            {
                DataRowView row = (DataRowView)StockInventSummaryGrid.SelectedItems[0];
                string srnumber = row["SrNo"].ToString();
                string itemname = row["Item Name"].ToString();
                string itembarcode = row["Barcode"].ToString();
                string group = row["Group"].ToString();
                string issold = row["SoldOut"].ToString();
                string qty = row["ActualQty"].ToString();
                string wt = row["ActualWt"].ToString();
                string price = row["ItemPrice"].ToString();
                string gstrate = row["GSTRate"].ToString();
                string compid = row["CompID"].ToString();
                //string accountnametrialbal = row["Particular"].ToString();
                //string accountnametrialbal = row["Particular"].ToString();

                //string startdatev = startDateTrialBalance.SelectedDate.ToString();
                //string enddatev = toDateTrialBalance.SelectedDate.ToString();

                //string customerName = row["CustomerName"].ToString();
                //string otherCharge = row["AnyotherCharges"].ToString();
                //string statecodeCust = row["GSTIN"].ToString();
                //statecodeCust = statecodeCust.Trim().Substring(0, 2);

                //SaleVoucherJewellLatha viewBillObj = new SaleVoucherJewellLatha();

                //if (saleHomeIcon == "SaleVoucherJewellLatha")
                //{
                UpdateItemInstantly sv = new UpdateItemInstantly(srnumber, itemname, itembarcode, group, issold, qty, wt, price,gstrate,compid);
                sv.ShowDialog();
                //}


                e.Handled = true;
                //uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            }


        }


        private void TrialBalanceGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {



            var uiElement = e.OriginalSource as UIElement;
            if (e.Key == Key.Enter && uiElement != null)
            {
                DataRowView row = (DataRowView)TrialBalanceGrid.SelectedItems[0];
                string accountnametrialbal = row["Particular"].ToString();
                string startdatev = startDateTrialBalance.SelectedDate.ToString();
                string enddatev = toDateTrialBalance.SelectedDate.ToString();

                //string customerName = row["CustomerName"].ToString();
                //string otherCharge = row["AnyotherCharges"].ToString();
                //string statecodeCust = row["GSTIN"].ToString();
                //statecodeCust = statecodeCust.Trim().Substring(0, 2);

                //SaleVoucherJewellLatha viewBillObj = new SaleVoucherJewellLatha();

                //if (saleHomeIcon == "SaleVoucherJewellLatha")
                //{
                ViewAccountLedger sv = new ViewAccountLedger(accountnametrialbal.Trim(), startdatev, enddatev);
                sv.ShowDialog();
                //}


                e.Handled = true;
                //uiElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            }


        }


        private void ListBoxSelectedItems_Click(object sender, RoutedEventArgs e)
        {
        
          
  

        } 

 

    }
}
