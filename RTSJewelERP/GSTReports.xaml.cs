using iTextSharp.text;
using iTextSharp.text.pdf;
using RTSJewelERP.GroupListTableAdapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.InteropServices;
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
    /// Interaction logic for GSTReports.xaml
    /// </summary>
    public partial class GSTReports : Window
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public GSTReports()
        {
            InitializeComponent();
            BindComboBoxGroupName(GroupName);
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window

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

        private void TabGSTR1_Selected(object sender, RoutedEventArgs e)
        {
            TotalWtgms.Clear();
            TotalTaxableAmount.Clear();
            TotalTaxGST.Clear();
            TotalSum.Clear();

            // autocompltCustNameSaleTab.autoTextBox.Text = "";
            string sdt = startDateGstr1Sale.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr1Sale.SelectedDate);
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

            string enddt = toDateGStr1Sale.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr1Sale.SelectedDate);
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

                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number], LTRIM(RTRIM(InvoiceAmt)) As Amount, TransactionDate , DueAmount As [Due Amount]  from SalesVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') order by  CAST(InvoiceNumber AS float) desc", con);
                //SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlCommand com = new SqlCommand("GetSaleGSTReportForJewellery", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@GroupName", GroupName.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);

                System.Data.DataTable dt2 = new System.Data.DataTable("Sale GST Summary");
                sda.Fill(dt2);
                GSTR1SummaryGrid.ItemsSource = dt2.DefaultView;
                GSTR1SummaryGrid.AutoGenerateColumns = true;
                GSTR1SummaryGrid.CanUserAddRows = false;
            }


            using (SqlConnection con = new SqlConnection())
            {

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetSaleGSTReportSummaryForJewellery", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@GroupName", GroupName.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double dtotalweightgms = (reader["TotalWt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double dtotaltaxableamts = (reader["TotalTaxableAmount"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double dtotalgsttaxams = (reader["TotalGSTTax"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    double dtotalamtgrand = (reader["TotalAmount"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                    //double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    //double opBalBookStartDr = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                    //double opBalBookStartCr = (reader["OpeningBalBookStartCR"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;

                    TotalWtgms.Text = dtotalweightgms.ToString();
                    TotalTaxableAmount.Text = dtotaltaxableamts.ToString();
                    TotalTaxGST.Text = dtotalgsttaxams.ToString();
                    TotalSum.Text = dtotalamtgrand.ToString();

                    //openingBalBookStartCR.Text = opBalBookStartCr.ToString();
                }

            }
        }

        private void Button_Click_GSTR1Summary(object sender, RoutedEventArgs e)
        {
            TotalWtgms.Clear();
            TotalTaxableAmount.Clear();
            TotalTaxGST.Clear();
            TotalSum.Clear();

            string sdt = startDateGstr1Sale.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr1Sale.SelectedDate);
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

            string enddt = toDateGStr1Sale.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr1Sale.SelectedDate);
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

                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number], LTRIM(RTRIM(InvoiceAmt)) As Amount, TransactionDate , DueAmount As [Due Amount]  from SalesVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') order by  CAST(InvoiceNumber AS float) desc", con);
                //SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlCommand com = new SqlCommand("GetSaleGSTReportForJewellery", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@GroupName", GroupName.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);

                System.Data.DataTable dt2 = new System.Data.DataTable("Sale GST Summary");
                sda.Fill(dt2);
                GSTR1SummaryGrid.ItemsSource = dt2.DefaultView;
                GSTR1SummaryGrid.AutoGenerateColumns = true;
                GSTR1SummaryGrid.CanUserAddRows = false;
            }

            using (SqlConnection con = new SqlConnection())
            {

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetSaleGSTReportSummaryForJewellery", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@GroupName", GroupName.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double dtotalweightgms = (reader["TotalWt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double dtotaltaxableamts = (reader["TotalTaxableAmount"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double dtotalgsttaxams = (reader["TotalGSTTax"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    double dtotalamtgrand = (reader["TotalAmount"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                    //double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    //double opBalBookStartDr = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                    //double opBalBookStartCr = (reader["OpeningBalBookStartCR"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;

                    TotalWtgms.Text = dtotalweightgms.ToString();
                    TotalTaxableAmount.Text = dtotaltaxableamts.ToString();
                    TotalTaxGST.Text = dtotalgsttaxams.ToString();
                    TotalSum.Text = dtotalamtgrand.ToString();

                    //openingBalBookStartCR.Text = opBalBookStartCr.ToString();
                }

            }
        }

        private void printGSTR1AcctLedger_Click(object sender, RoutedEventArgs e)
        {
            string sdt = startDateGstr1Sale.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr1Sale.SelectedDate);
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

            string enddt = toDateGStr1Sale.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr1Sale.SelectedDate);
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

                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number], LTRIM(RTRIM(InvoiceAmt)) As Amount, TransactionDate , DueAmount As [Due Amount]  from SalesVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') order by  CAST(InvoiceNumber AS float) desc", con);
                //SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlCommand com = new SqlCommand("GetSaleGSTReportForJewellery", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@GroupName", GroupName.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);

                System.Data.DataTable dt2 = new System.Data.DataTable("Sale GST Summary");
                sda.Fill(dt2);
                GSTR1SummaryGrid.ItemsSource = dt2.DefaultView;
                GSTR1SummaryGrid.AutoGenerateColumns = true;
                GSTR1SummaryGrid.CanUserAddRows = false;
            }

            using (SqlConnection con = new SqlConnection())
            {

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetSaleGSTReportSummaryForJewellery", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@GroupName", GroupName.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double dtotalweightgms = (reader["TotalWt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double dtotaltaxableamts = (reader["TotalTaxableAmount"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double dtotalgsttaxams = (reader["TotalGSTTax"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    double dtotalamtgrand = (reader["TotalAmount"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                    //double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    //double opBalBookStartDr = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                    //double opBalBookStartCr = (reader["OpeningBalBookStartCR"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;

                    TotalWtgms.Text = dtotalweightgms.ToString();
                    TotalTaxableAmount.Text = dtotaltaxableamts.ToString();
                    TotalTaxGST.Text = dtotalgsttaxams.ToString();
                    TotalSum.Text = dtotalamtgrand.ToString();

                    //openingBalBookStartCR.Text = opBalBookStartCr.ToString();
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

            string sdt = startDateGstr1Sale.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr1Sale.SelectedDate);
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

            string enddt = toDateGStr1Sale.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr1Sale.SelectedDate);
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
            a3 = new Span(new Run("GSTR1 Monthly Sale Statement"));
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
            a5 = new Span(new Run(GroupName.Text));
            a5.FontWeight = FontWeights.Bold;
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
            p.Inlines.Add(a5);// Add the span content into paragraph. 

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
            for (int i = 0; i < GSTR1SummaryGrid.Items.Count; i++)
            {
                //TableColumn tc = new TableColumn();

                t5.Columns.Add(new TableColumn() { Width = GridLength.Auto });

            }

            ThicknessConverter tc1 = new ThicknessConverter();
            //// Create Table Borders
            t5.BorderThickness = (Thickness)tc1.ConvertFromString("0.02in");

            int count1 = GSTR1SummaryGrid.Items.Count;
            var rg1 = new TableRowGroup();

            TableRow rowheadertable1 = new TableRow();



            rowheadertable1.Background = Brushes.Silver;
            rowheadertable1.FontSize = 10;
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

            TableCell tcell3 = new TableCell(new System.Windows.Documents.Paragraph(new Run("InvoiceNo")));
            //tcell3.ColumnSpan = 3;
            tcell3.BorderBrush = Brushes.Black;
            tcell3.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell3);

            TableCell tcell4 = new TableCell(new System.Windows.Documents.Paragraph(new Run("InvoiceDate")));
            //tcell4.ColumnSpan = 3;
            tcell4.BorderBrush = Brushes.Black;
            tcell4.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell4);

            TableCell tcell5 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Category")));
            //tcell5.ColumnSpan = 3;
            tcell5.BorderBrush = Brushes.Black;
            tcell5.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell5);

            TableCell tcell6 = new TableCell(new System.Windows.Documents.Paragraph(new Run("GStRate(%)")));
            //tcell6.ColumnSpan = 3;
            tcell6.BorderBrush = Brushes.Black;
            tcell6.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell6);

            TableCell tcell7 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Weight(gm)")));
            //tcell7.ColumnSpan = 3;
            tcell7.BorderBrush = Brushes.Black;
            tcell7.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell7);

            TableCell tcell8 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Taxable Amount")));
            //tcell8.ColumnSpan = 3;
            tcell8.BorderBrush = Brushes.Black;
            tcell8.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell8);

            TableCell tcell9 = new TableCell(new System.Windows.Documents.Paragraph(new Run("GST Tax")));
            //tcell9.ColumnSpan = 3;
            tcell9.BorderBrush = Brushes.Black;
            tcell9.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell9);

            TableCell tcell10 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Total Amount")));
            //tcell10.ColumnSpan = 3;
            tcell10.BorderBrush = Brushes.Black;
            tcell10.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell10);

            //TableCell tcell11 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Date")));
            ////tcell11.ColumnSpan = 3;
            //tcell11.BorderBrush = Brushes.Black;
            //tcell11.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell11);




            SqlConnection conpdfj = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conpdfj.Open();
            //string sqlpdf = "SELECT row_number() OVER (order by srnumber ) Sr ,DesignNumberPattern AS Style,[ItemName] As [Item Name]  ,[HSN],Small As S, Mediium As M, Large As L, XL, XL2, XL3,XL4,XL5,XL6 ,[BilledQty] As [Qty] ,[UnitID] As [UOM],[SalePrice] As [Price],Amount ,[Discount] As [Disc(%)] ,[TaxablelAmount] As [Taxable] ,[GSTRate] As [GST%] ,[TotalAmount] As [Total]   FROM [SalesVoucherInventorycloths] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "'";
            // string sqlpdfj = "SELECT [ItemName] As [ITEM NAME],[BilledQty] As [Qty] ,[BilledWt] As [Wt],WastePerc,[TotalBilledWt],MakingCharge,[SalePrice] As [Price],Amount,[Discount] As [Disc(%)],TaxablelAmount ,[GSTRate] As [GST%] ,[TotalAmount] As [TOTAL]   FROM [SalesVoucherInventoryByPc] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "' and ItemName not in ( 'Old Gold','Old Silver')";
            //SqlCommand cmdpdfj = new SqlCommand(sqlpdfj);
            SqlCommand cmdpdfj = new SqlCommand("GetSaleGSTReportForJewellery", conpdfj);
            cmdpdfj.CommandType = CommandType.StoredProcedure;
            cmdpdfj.Parameters.Add(new SqlParameter("@StartDate", sdt));
            cmdpdfj.Parameters.Add(new SqlParameter("@EndDate", enddt));
            cmdpdfj.Parameters.Add(new SqlParameter("@CompID", CompID));
            cmdpdfj.Parameters.Add(new SqlParameter("@GroupName", GroupName.Text.Trim()));
            SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);


            //cmdpdfj.Connection = conpdfj;
            //SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);
            DataTable dttablej = new DataTable("Inv");
            sda.Fill(dttablej);

            rg1.Rows.Add(rowheadertable1);

            IEnumerable itemsSource1 = GSTR1SummaryGrid.ItemsSource as IEnumerable;
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
                        //if (i == 2 || i == 3)
                        //{
                        //    firstcolproductcell.ColumnSpan = 3;
                        //}
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
            ts11gTotaoBeforeDisc = new Span(new Run("\t Total WT(GMS):" + TotalWtgms.Text));
            ts11gTotaoBeforeDisc.Inlines.Add(new LineBreak());//Line break is used for next line.  
            //}

            Span ts11gDiscAmountItemTotal = new Span();

            ts11gDiscAmountItemTotal = new Span(new Run("\t Taxable:" + "₹ " + TotalTaxableAmount.Text));
            ts11gDiscAmountItemTotal.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span tsTotalTaxableAmt = new Span();

            tsTotalTaxableAmt = new Span(new Run("\t Total Tax :" + "₹ " + TotalTaxGST.Text));
            tsTotalTaxableAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span tsTotalGrandSumeAmt = new Span();
            tsTotalGrandSumeAmt = new Span(new Run("\t Total Amount :" + "₹ " + TotalSum.Text));
            tsTotalGrandSumeAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  



            totalVaGrand.FontSize = 11;
            totalVaGrand.FontFamily = new FontFamily("Century Gothic");
            totalVaGrand.Inlines.Add(ts11gTotaoBeforeDisc);// Add the span content into paragraph.  
            totalVaGrand.Inlines.Add(ts11gDiscAmountItemTotal);

            totalVaGrand.Inlines.Add(tsTotalTaxableAmt);
            totalVaGrand.Inlines.Add(tsTotalGrandSumeAmt);


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
            rowColCellheadertable.FontSize = 11;
            rowColCellheadertable.FontFamily = new FontFamily("Century Gothic");
            rowColCellheadertable.FontWeight = FontWeights.Bold;

            ThicknessConverter tc222tbc = new ThicknessConverter();

            TableCell tcellfirstTb = new TableCell(new System.Windows.Documents.Paragraph(new Run("")));

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

            completeTable.Padding = new Thickness(12);
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
            //doc.Blocks.Add(signpara);


            doc.Name = "FlowDoc";
            //doc.PageWidth = 900;
            doc.PagePadding = new Thickness(20, 20, 20, 5); //v3
            //doc.PagePadding = new Thickness(30, 20, 10, 5); //V2 
            // Create IDocumentPaginatorSource from FlowDocument
            // IDocumentPaginatorSource idpSource = doc;
            // Call PrintDocument method to send document to printer



            return doc;


        }



        ///GSTR1 Report
        ///
        private void TabGSTR1Full_Selected(object sender, RoutedEventArgs e)
        {
            TotalWtgms.Clear();
            TotalTaxableAmount.Clear();
            TotalTaxGST.Clear();
            TotalSum.Clear();

            // autocompltCustNameSaleTab.autoTextBox.Text = "";
            string sdt = startDateGstr1SaleFull.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr1SaleFull.SelectedDate);
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

            string enddt = toDateGStr1SaleFull.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr1SaleFull.SelectedDate);
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

                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number], LTRIM(RTRIM(InvoiceAmt)) As Amount, TransactionDate , DueAmount As [Due Amount]  from SalesVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') order by  CAST(InvoiceNumber AS float) desc", con);
                //SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlCommand com = new SqlCommand("GetGSTR1ByType", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);

                System.Data.DataTable dt2 = new System.Data.DataTable("Sale GST Summary");
                sda.Fill(dt2);
                GSTR1FullSummaryGrid.ItemsSource = dt2.DefaultView;
                GSTR1FullSummaryGrid.AutoGenerateColumns = true;
                GSTR1FullSummaryGrid.CanUserAddRows = false;
            }


            using (SqlConnection con = new SqlConnection())
            {

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetGSTR1ByTypeSummary", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    //double dtotalweightgms = (reader["TotalWt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double dtotaltaxableamts = (reader["TaxableValue"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    //double dtotalgsttaxams = (reader["TotalGSTTax"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    double dtotalamtgrand = (reader["InvoiceValue"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    //double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    //double opBalBookStartDr = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                    //double opBalBookStartCr = (reader["OpeningBalBookStartCR"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;

                    //TotalWtgms.Text = dtotalweightgms.ToString();
                    TotalTaxableAmountFull.Text = dtotaltaxableamts.ToString();
                    //TotalTaxGST.Text = dtotalgsttaxams.ToString();
                    TotalSumFull.Text = dtotalamtgrand.ToString();

                    //openingBalBookStartCR.Text = opBalBookStartCr.ToString();
                }

            }
        }

        private void Button_Click_GSTR1SummaryFull(object sender, RoutedEventArgs e)
        {
            TotalWtgms.Clear();
            TotalTaxableAmount.Clear();
            TotalTaxGST.Clear();
            TotalSum.Clear();

            string sdt = startDateGstr1SaleFull.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr1SaleFull.SelectedDate);
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

            string enddt = toDateGStr1SaleFull.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr1SaleFull.SelectedDate);
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

                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number], LTRIM(RTRIM(InvoiceAmt)) As Amount, TransactionDate , DueAmount As [Due Amount]  from SalesVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') order by  CAST(InvoiceNumber AS float) desc", con);
                //SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlCommand com = new SqlCommand("GetGSTR1ByType", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);

                System.Data.DataTable dt2 = new System.Data.DataTable("Sale GST Summary");
                sda.Fill(dt2);
                GSTR1FullSummaryGrid.ItemsSource = dt2.DefaultView;
                GSTR1FullSummaryGrid.AutoGenerateColumns = true;
                GSTR1FullSummaryGrid.CanUserAddRows = false;
            }

            using (SqlConnection con = new SqlConnection())
            {

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetGSTR1ByTypeSummary", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    //double dtotalweightgms = (reader["TotalWt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double dtotaltaxableamts = (reader["TaxableValue"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    //double dtotalgsttaxams = (reader["TotalGSTTax"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    double dtotalamtgrand = (reader["InvoiceValue"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    //double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    //double opBalBookStartDr = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                    //double opBalBookStartCr = (reader["OpeningBalBookStartCR"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;

                    //TotalWtgms.Text = dtotalweightgms.ToString();
                    TotalTaxableAmountFull.Text = dtotaltaxableamts.ToString();
                    //TotalTaxGST.Text = dtotalgsttaxams.ToString();
                    TotalSumFull.Text = dtotalamtgrand.ToString();

                    //openingBalBookStartCR.Text = opBalBookStartCr.ToString();
                }

            }
        }

        private void printGSTR1AcctLedgerFull_Click(object sender, RoutedEventArgs e)
        {
            string sdt = startDateGstr1SaleFull.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr1SaleFull.SelectedDate);
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

            string enddt = toDateGStr1SaleFull.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr1SaleFull.SelectedDate);
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

                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number], LTRIM(RTRIM(InvoiceAmt)) As Amount, TransactionDate , DueAmount As [Due Amount]  from SalesVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') order by  CAST(InvoiceNumber AS float) desc", con);
                //SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlCommand com = new SqlCommand("GetGSTR1ByType", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);

                System.Data.DataTable dt2 = new System.Data.DataTable("Sale GST Summary");
                sda.Fill(dt2);
                GSTR1FullSummaryGrid.ItemsSource = dt2.DefaultView;
                GSTR1FullSummaryGrid.AutoGenerateColumns = true;
                GSTR1FullSummaryGrid.CanUserAddRows = false;
            }



            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintQueue = System.Printing.LocalPrintServer.GetDefaultPrintQueue();
            printDlg.PrintTicket = printDlg.PrintQueue.DefaultPrintTicket;
            printDlg.PrintTicket.PageOrientation = PageOrientation.Portrait;

            // Create a FlowDocument dynamically.
            //FlowDocument doc = CreateFlowDocumentJewellery();
            FlowDocument doc = CreateFlowDocumentGSTR1FullAccount();
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

        private FlowDocument CreateFlowDocumentGSTR1FullAccount()
        {
            //  Get Confirmation that data saved successfull, 

            string sdt = startDateGstr1SaleFull.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr1SaleFull.SelectedDate);
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

            string enddt = toDateGStr1SaleFull.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr1SaleFull.SelectedDate);
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
            a3 = new Span(new Run("GSTR1 Monthly Sale Statement"));
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
            a5 = new Span(new Run(GSTRType.Text));
            a5.FontWeight = FontWeights.Bold;
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
            for (int i = 0; i < GSTR1SummaryGrid.Items.Count; i++)
            {
                //TableColumn tc = new TableColumn();

                t5.Columns.Add(new TableColumn() { Width = GridLength.Auto });

            }

            ThicknessConverter tc1 = new ThicknessConverter();
            //// Create Table Borders
            t5.BorderThickness = (Thickness)tc1.ConvertFromString("0.02in");

            int count1 = GSTR1SummaryGrid.Items.Count;
            var rg1 = new TableRowGroup();

            TableRow rowheadertable1 = new TableRow();



            rowheadertable1.Background = Brushes.Silver;
            rowheadertable1.FontSize = 10;
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

            //TableCell tcell3 = new TableCell(new System.Windows.Documents.Paragraph(new Run("InvoiceNo")));
            ////tcell3.ColumnSpan = 3;
            //tcell3.BorderBrush = Brushes.Black;
            //tcell3.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell3);

            //TableCell tcell4 = new TableCell(new System.Windows.Documents.Paragraph(new Run("InvoiceDate")));
            ////tcell4.ColumnSpan = 3;
            //tcell4.BorderBrush = Brushes.Black;
            //tcell4.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell4);

            //TableCell tcell5 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Category")));
            ////tcell5.ColumnSpan = 3;
            //tcell5.BorderBrush = Brushes.Black;
            //tcell5.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell5);

            //TableCell tcell6 = new TableCell(new System.Windows.Documents.Paragraph(new Run("GStRate(%)")));
            ////tcell6.ColumnSpan = 3;
            //tcell6.BorderBrush = Brushes.Black;
            //tcell6.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell6);

            //TableCell tcell7 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Weight(gm)")));
            ////tcell7.ColumnSpan = 3;
            //tcell7.BorderBrush = Brushes.Black;
            //tcell7.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell7);

            //TableCell tcell8 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Taxable Amount")));
            ////tcell8.ColumnSpan = 3;
            //tcell8.BorderBrush = Brushes.Black;
            //tcell8.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell8);

            //TableCell tcell9 = new TableCell(new System.Windows.Documents.Paragraph(new Run("GST Tax")));
            ////tcell9.ColumnSpan = 3;
            //tcell9.BorderBrush = Brushes.Black;
            //tcell9.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell9);

            //TableCell tcell10 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Total Amount")));
            ////tcell10.ColumnSpan = 3;
            //tcell10.BorderBrush = Brushes.Black;
            //tcell10.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell10);

            //TableCell tcell11 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Date")));
            ////tcell11.ColumnSpan = 3;
            //tcell11.BorderBrush = Brushes.Black;
            //tcell11.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell11);




            SqlConnection conpdfj = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conpdfj.Open();
            //string sqlpdf = "SELECT row_number() OVER (order by srnumber ) Sr ,DesignNumberPattern AS Style,[ItemName] As [Item Name]  ,[HSN],Small As S, Mediium As M, Large As L, XL, XL2, XL3,XL4,XL5,XL6 ,[BilledQty] As [Qty] ,[UnitID] As [UOM],[SalePrice] As [Price],Amount ,[Discount] As [Disc(%)] ,[TaxablelAmount] As [Taxable] ,[GSTRate] As [GST%] ,[TotalAmount] As [Total]   FROM [SalesVoucherInventorycloths] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "'";
            // string sqlpdfj = "SELECT [ItemName] As [ITEM NAME],[BilledQty] As [Qty] ,[BilledWt] As [Wt],WastePerc,[TotalBilledWt],MakingCharge,[SalePrice] As [Price],Amount,[Discount] As [Disc(%)],TaxablelAmount ,[GSTRate] As [GST%] ,[TotalAmount] As [TOTAL]   FROM [SalesVoucherInventoryByPc] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "' and ItemName not in ( 'Old Gold','Old Silver')";
            //SqlCommand cmdpdfj = new SqlCommand(sqlpdfj);
            SqlCommand cmdpdfj = new SqlCommand("GetGSTR1ByType", conpdfj);
            cmdpdfj.CommandType = CommandType.StoredProcedure;
            cmdpdfj.Parameters.Add(new SqlParameter("@StartDate", sdt));
            cmdpdfj.Parameters.Add(new SqlParameter("@EndDate", enddt));
            cmdpdfj.Parameters.Add(new SqlParameter("@CompID", CompID));
            cmdpdfj.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
            SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);


            //cmdpdfj.Connection = conpdfj;
            //SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);
            DataTable dttablej = new DataTable("Inv");
            sda.Fill(dttablej);

            rg1.Rows.Add(rowheadertable1);

            IEnumerable itemsSource1 = GSTR1SummaryGrid.ItemsSource as IEnumerable;
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
                        //if (i == 2 || i == 3)
                        //{
                        //    firstcolproductcell.ColumnSpan = 3;
                        //}
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
            ts11gTotaoBeforeDisc = new Span(new Run("\t Total WT(GMS):" + TotalWtgms.Text));
            ts11gTotaoBeforeDisc.Inlines.Add(new LineBreak());//Line break is used for next line.  
            //}

            Span ts11gDiscAmountItemTotal = new Span();

            ts11gDiscAmountItemTotal = new Span(new Run("\t Taxable:" + "₹ " + TotalTaxableAmount.Text));
            ts11gDiscAmountItemTotal.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span tsTotalTaxableAmt = new Span();

            tsTotalTaxableAmt = new Span(new Run("\t Total Tax :" + "₹ " + TotalTaxGST.Text));
            tsTotalTaxableAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span tsTotalGrandSumeAmt = new Span();
            tsTotalGrandSumeAmt = new Span(new Run("\t Total Amount :" + "₹ " + TotalSum.Text));
            tsTotalGrandSumeAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  



            totalVaGrand.FontSize = 11;
            totalVaGrand.FontFamily = new FontFamily("Century Gothic");
            totalVaGrand.Inlines.Add(ts11gTotaoBeforeDisc);// Add the span content into paragraph.  
            totalVaGrand.Inlines.Add(ts11gDiscAmountItemTotal);

            totalVaGrand.Inlines.Add(tsTotalTaxableAmt);
            totalVaGrand.Inlines.Add(tsTotalGrandSumeAmt);


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
            rowColCellheadertable.FontSize = 11;
            rowColCellheadertable.FontFamily = new FontFamily("Century Gothic");
            rowColCellheadertable.FontWeight = FontWeights.Bold;

            ThicknessConverter tc222tbc = new ThicknessConverter();

            TableCell tcellfirstTb = new TableCell(new System.Windows.Documents.Paragraph(new Run("")));

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

            completeTable.Padding = new Thickness(12);
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
            //doc.Blocks.Add(signpara);


            doc.Name = "FlowDoc";
            //doc.PageWidth = 900;
            doc.PagePadding = new Thickness(20, 20, 20, 5); //v3
            //doc.PagePadding = new Thickness(30, 20, 10, 5); //V2 
            // Create IDocumentPaginatorSource from FlowDocument
            // IDocumentPaginatorSource idpSource = doc;
            // Call PrintDocument method to send document to printer



            return doc;


        }


        private void exportTo_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string sdt = startDateGstr1SaleFull.SelectedDate.ToString();
                // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
                DateTime dt = Convert.ToDateTime(startDateGstr1SaleFull.SelectedDate);
                //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                int years = dt.Year;
                string months = dt.Month.ToString();



                GSTR1FullSummaryGrid.SelectAllCells();

                //GSTR1FullSummaryGrid.Columns.RemoveAt(0); // because of this it throw one column less when creating invoice as it deleted column 

                //DataTable tempDt = new DataTable();
                //tempDt = DataGridtoDataTable(GSTR1FullSummaryGrid);

                GSTR1FullSummaryGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                ApplicationCommands.Copy.Execute(null, GSTR1FullSummaryGrid);

                GSTR1FullSummaryGrid.UnselectAllCells();

                String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
                //int billno = Convert.ToInt16(invoiceNumber.Text.Trim());
                try
                {
                    StreamWriter sw = new StreamWriter(@"C:\ViewBill\\GST\Sale\GSTR1-" + months + "-" + years + ".csv");
                    sw.WriteLine(result);
                    sw.Close();

                    Process process = new Process();
                    process.StartInfo.UseShellExecute = true;

                    //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    process.StartInfo.FileName = @"C:\ViewBill\\GST\Sale\GSTR1-" + months + "-" + years + ".csv";

                    process.Start();
                    process.Close();


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


        private void exportTo_ClickCategory(object sender, RoutedEventArgs e)
        {

            try
            {
                string sdt = startDateGstr1Sale.SelectedDate.ToString();
                // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
                DateTime dt = Convert.ToDateTime(startDateGstr1Sale.SelectedDate);
                //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                int years = dt.Year;
                string months = dt.Month.ToString();



                GSTR1SummaryGrid.SelectAllCells();

                //GSTR1FullSummaryGrid.Columns.RemoveAt(0); // because of this it throw one column less when creating invoice as it deleted column 

                //DataTable tempDt = new DataTable();
                //tempDt = DataGridtoDataTable(GSTR1FullSummaryGrid);

                GSTR1SummaryGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                ApplicationCommands.Copy.Execute(null, GSTR1SummaryGrid);

                GSTR1SummaryGrid.UnselectAllCells();

                String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
                //int billno = Convert.ToInt16(invoiceNumber.Text.Trim());
                try
                {
                    StreamWriter sw = new StreamWriter(@"C:\ViewBill\\GST\Sale\GSTR1-" + GroupName.Text.Trim() + "-" + months + "-" + years + ".csv");
                    sw.WriteLine(result);
                    sw.Close();

                    Process process = new Process();
                    process.StartInfo.UseShellExecute = true;

                    //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    process.StartInfo.FileName = @"C:\ViewBill\\GST\Sale\GSTR1-" + GroupName.Text.Trim() + "-" + months + "-" + years + ".csv";

                    process.Start();
                    process.Close();


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



        ///GSTR2 Report
        ///
        private void TabGSTR2Full_Selected(object sender, RoutedEventArgs e)
        {
            TotalWtgms.Clear();
            TotalTaxableAmount.Clear();
            TotalTaxGST.Clear();
            TotalSum.Clear();

            // autocompltCustNameSaleTab.autoTextBox.Text = "";
            string sdt = startDateGstr2PurFull.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr2PurFull.SelectedDate);
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

            string enddt = toDateGStr2PurFull.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr2PurFull.SelectedDate);
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

                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number], LTRIM(RTRIM(InvoiceAmt)) As Amount, TransactionDate , DueAmount As [Due Amount]  from SalesVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') order by  CAST(InvoiceNumber AS float) desc", con);
                //SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlCommand com = new SqlCommand("GetGSTR2ByType", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);

                System.Data.DataTable dt2 = new System.Data.DataTable("Sale GST Summary");
                sda.Fill(dt2);
                GSTR2FullSummaryGrid.ItemsSource = dt2.DefaultView;
                GSTR2FullSummaryGrid.AutoGenerateColumns = true;
                GSTR2FullSummaryGrid.CanUserAddRows = false;
            }



            using (SqlConnection con = new SqlConnection())
            {

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetGSTR2ByTypeSummary", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    //double dtotalweightgms = (reader["TotalWt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double dtotaltaxableamts = (reader["TaxableValue"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    //double dtotalgsttaxams = (reader["TotalGSTTax"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    double dtotalamtgrand = (reader["InvoiceValue"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    //double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    //double opBalBookStartDr = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                    //double opBalBookStartCr = (reader["OpeningBalBookStartCR"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;

                    //TotalWtgms.Text = dtotalweightgms.ToString();
                    TotalTaxableAmountFullGSTR2.Text = dtotaltaxableamts.ToString();
                    //TotalTaxGST.Text = dtotalgsttaxams.ToString();
                    TotalSumFullGSTR2.Text = dtotalamtgrand.ToString();

                    //openingBalBookStartCR.Text = opBalBookStartCr.ToString();
                }

            }
        }

        private void Button_Click_GSTR2SummaryFull(object sender, RoutedEventArgs e)
        {
            TotalWtgms.Clear();
            TotalTaxableAmountFullGSTR2.Clear();
            TotalSumFullGSTR2.Clear();
            TotalSum.Clear();

            string sdt = startDateGstr2PurFull.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr2PurFull.SelectedDate);
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

            string enddt = toDateGStr2PurFull.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr2PurFull.SelectedDate);
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

                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number], LTRIM(RTRIM(InvoiceAmt)) As Amount, TransactionDate , DueAmount As [Due Amount]  from SalesVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') order by  CAST(InvoiceNumber AS float) desc", con);
                //SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlCommand com = new SqlCommand("GetGSTR2ByType", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);

                System.Data.DataTable dt2 = new System.Data.DataTable("Sale GST Summary");
                sda.Fill(dt2);
                GSTR2FullSummaryGrid.ItemsSource = dt2.DefaultView;
                GSTR2FullSummaryGrid.AutoGenerateColumns = true;
                GSTR2FullSummaryGrid.CanUserAddRows = false;
            }

            using (SqlConnection con = new SqlConnection())
            {

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetGSTR2ByTypeSummary", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    //double dtotalweightgms = (reader["TotalWt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double dtotaltaxableamts = (reader["TaxableValue"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    //double dtotalgsttaxams = (reader["TotalGSTTax"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    double dtotalamtgrand = (reader["InvoiceValue"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    //double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    //double opBalBookStartDr = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                    //double opBalBookStartCr = (reader["OpeningBalBookStartCR"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;

                    //TotalWtgms.Text = dtotalweightgms.ToString();
                    TotalTaxableAmountFullGSTR2.Text = dtotaltaxableamts.ToString();
                    //TotalTaxGST.Text = dtotalgsttaxams.ToString();
                    TotalSumFullGSTR2.Text = dtotalamtgrand.ToString();

                    //openingBalBookStartCR.Text = opBalBookStartCr.ToString();
                }

            }
        }

        private void printGSTR2AcctLedger_Click(object sender, RoutedEventArgs e)
        {
            string sdt = startDateGstr2PurFull.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr2PurFull.SelectedDate);
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

            string enddt = toDateGStr2PurFull.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr2PurFull.SelectedDate);
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

                //SqlCommand com = new SqlCommand("(select LTRIM(RTRIM(AccountName)) As [Account Name] ,LTRIM(RTRIM(InvoiceNumber)) As [Invoice Number], LTRIM(RTRIM(InvoiceAmt)) As Amount, TransactionDate , DueAmount As [Due Amount]  from SalesVouchersOtherDetails  where CompID = '" + CompID + "' and TransactionDate <= '" + enddt + "' and TransactionDate >= '" + sdt + "') order by  CAST(InvoiceNumber AS float) desc", con);
                //SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlCommand com = new SqlCommand("GetGSTR2ByType", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);

                System.Data.DataTable dt2 = new System.Data.DataTable("Sale GST Summary");
                sda.Fill(dt2);
                GSTR2FullSummaryGrid.ItemsSource = dt2.DefaultView;
                GSTR2FullSummaryGrid.AutoGenerateColumns = true;
                GSTR2FullSummaryGrid.CanUserAddRows = false;
            }


            using (SqlConnection con = new SqlConnection())
            {

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetGSTR2ByTypeSummary", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    //double dtotalweightgms = (reader["TotalWt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double dtotaltaxableamts = (reader["TaxableValue"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    //double dtotalgsttaxams = (reader["TotalGSTTax"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    double dtotalamtgrand = (reader["InvoiceValue"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    //double opBal = dCredAcctLedgerAmt + dReceiptVAmt - dDebtAcctLedgerAmt - dPayVAmt;
                    //double opBalBookStartDr = (reader["OpeningBalBookStart"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                    //double opBalBookStartCr = (reader["OpeningBalBookStartCR"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;

                    //TotalWtgms.Text = dtotalweightgms.ToString();
                    TotalTaxableAmountFullGSTR2.Text = dtotaltaxableamts.ToString();
                    //TotalTaxGST.Text = dtotalgsttaxams.ToString();
                    TotalSumFullGSTR2.Text = dtotalamtgrand.ToString();

                    //openingBalBookStartCR.Text = opBalBookStartCr.ToString();
                }

            }


            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintQueue = System.Printing.LocalPrintServer.GetDefaultPrintQueue();
            printDlg.PrintTicket = printDlg.PrintQueue.DefaultPrintTicket;
            printDlg.PrintTicket.PageOrientation = PageOrientation.Portrait;

            // Create a FlowDocument dynamically.
            //FlowDocument doc = CreateFlowDocumentJewellery();
            FlowDocument doc = CreateFlowDocumentGSTR2FullAccount();
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

        private FlowDocument CreateFlowDocumentGSTR2FullAccount()
        {
            //  Get Confirmation that data saved successfull, 

            string sdt = startDateGstr2PurFull.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr2PurFull.SelectedDate);
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

            string enddt = toDateGStr2PurFull.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr2PurFull.SelectedDate);
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
            a3 = new Span(new Run("GSTR1 Monthly Sale Statement"));
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
            a5 = new Span(new Run(GSTRType.Text));
            a5.FontWeight = FontWeights.Bold;
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
            p.Inlines.Add(a5);// Add the span content into paragraph. 

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
            for (int i = 0; i < GSTR1SummaryGrid.Items.Count; i++)
            {
                //TableColumn tc = new TableColumn();

                t5.Columns.Add(new TableColumn() { Width = GridLength.Auto });

            }

            ThicknessConverter tc1 = new ThicknessConverter();
            //// Create Table Borders
            t5.BorderThickness = (Thickness)tc1.ConvertFromString("0.02in");

            int count1 = GSTR1SummaryGrid.Items.Count;
            var rg1 = new TableRowGroup();

            TableRow rowheadertable1 = new TableRow();



            rowheadertable1.Background = Brushes.Silver;
            rowheadertable1.FontSize = 10;
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

            TableCell tcell3 = new TableCell(new System.Windows.Documents.Paragraph(new Run("InvoiceNo")));
            //tcell3.ColumnSpan = 3;
            tcell3.BorderBrush = Brushes.Black;
            tcell3.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell3);

            TableCell tcell4 = new TableCell(new System.Windows.Documents.Paragraph(new Run("InvoiceDate")));
            //tcell4.ColumnSpan = 3;
            tcell4.BorderBrush = Brushes.Black;
            tcell4.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell4);

            TableCell tcell5 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Category")));
            //tcell5.ColumnSpan = 3;
            tcell5.BorderBrush = Brushes.Black;
            tcell5.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell5);

            TableCell tcell6 = new TableCell(new System.Windows.Documents.Paragraph(new Run("GStRate(%)")));
            //tcell6.ColumnSpan = 3;
            tcell6.BorderBrush = Brushes.Black;
            tcell6.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell6);

            TableCell tcell7 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Weight(gm)")));
            //tcell7.ColumnSpan = 3;
            tcell7.BorderBrush = Brushes.Black;
            tcell7.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell7);

            TableCell tcell8 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Taxable Amount")));
            //tcell8.ColumnSpan = 3;
            tcell8.BorderBrush = Brushes.Black;
            tcell8.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell8);

            TableCell tcell9 = new TableCell(new System.Windows.Documents.Paragraph(new Run("GST Tax")));
            //tcell9.ColumnSpan = 3;
            tcell9.BorderBrush = Brushes.Black;
            tcell9.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell9);

            TableCell tcell10 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Total Amount")));
            //tcell10.ColumnSpan = 3;
            tcell10.BorderBrush = Brushes.Black;
            tcell10.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell10);

            //TableCell tcell11 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Date")));
            ////tcell11.ColumnSpan = 3;
            //tcell11.BorderBrush = Brushes.Black;
            //tcell11.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell11);




            SqlConnection conpdfj = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conpdfj.Open();
            //string sqlpdf = "SELECT row_number() OVER (order by srnumber ) Sr ,DesignNumberPattern AS Style,[ItemName] As [Item Name]  ,[HSN],Small As S, Mediium As M, Large As L, XL, XL2, XL3,XL4,XL5,XL6 ,[BilledQty] As [Qty] ,[UnitID] As [UOM],[SalePrice] As [Price],Amount ,[Discount] As [Disc(%)] ,[TaxablelAmount] As [Taxable] ,[GSTRate] As [GST%] ,[TotalAmount] As [Total]   FROM [SalesVoucherInventorycloths] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "'";
            // string sqlpdfj = "SELECT [ItemName] As [ITEM NAME],[BilledQty] As [Qty] ,[BilledWt] As [Wt],WastePerc,[TotalBilledWt],MakingCharge,[SalePrice] As [Price],Amount,[Discount] As [Disc(%)],TaxablelAmount ,[GSTRate] As [GST%] ,[TotalAmount] As [TOTAL]   FROM [SalesVoucherInventoryByPc] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "' and ItemName not in ( 'Old Gold','Old Silver')";
            //SqlCommand cmdpdfj = new SqlCommand(sqlpdfj);
            SqlCommand cmdpdfj = new SqlCommand("GetGSTR1ByType", conpdfj);
            cmdpdfj.CommandType = CommandType.StoredProcedure;
            cmdpdfj.Parameters.Add(new SqlParameter("@StartDate", sdt));
            cmdpdfj.Parameters.Add(new SqlParameter("@EndDate", enddt));
            cmdpdfj.Parameters.Add(new SqlParameter("@CompID", CompID));
            cmdpdfj.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
            SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);


            //cmdpdfj.Connection = conpdfj;
            //SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);
            DataTable dttablej = new DataTable("Inv");
            sda.Fill(dttablej);

            rg1.Rows.Add(rowheadertable1);

            IEnumerable itemsSource1 = GSTR1SummaryGrid.ItemsSource as IEnumerable;
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
                        //if (i == 2 || i == 3)
                        //{
                        //    firstcolproductcell.ColumnSpan = 3;
                        //}
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
            ts11gTotaoBeforeDisc = new Span(new Run("\t Total WT(GMS):" + TotalWtgms.Text));
            ts11gTotaoBeforeDisc.Inlines.Add(new LineBreak());//Line break is used for next line.  
            //}

            Span ts11gDiscAmountItemTotal = new Span();

            ts11gDiscAmountItemTotal = new Span(new Run("\t Taxable:" + "₹ " + TotalTaxableAmount.Text));
            ts11gDiscAmountItemTotal.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span tsTotalTaxableAmt = new Span();

            tsTotalTaxableAmt = new Span(new Run("\t Total Tax :" + "₹ " + TotalTaxGST.Text));
            tsTotalTaxableAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span tsTotalGrandSumeAmt = new Span();
            tsTotalGrandSumeAmt = new Span(new Run("\t Total Amount :" + "₹ " + TotalSum.Text));
            tsTotalGrandSumeAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  



            totalVaGrand.FontSize = 11;
            totalVaGrand.FontFamily = new FontFamily("Century Gothic");
            totalVaGrand.Inlines.Add(ts11gTotaoBeforeDisc);// Add the span content into paragraph.  
            totalVaGrand.Inlines.Add(ts11gDiscAmountItemTotal);

            totalVaGrand.Inlines.Add(tsTotalTaxableAmt);
            totalVaGrand.Inlines.Add(tsTotalGrandSumeAmt);


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
            rowColCellheadertable.FontSize = 11;
            rowColCellheadertable.FontFamily = new FontFamily("Century Gothic");
            rowColCellheadertable.FontWeight = FontWeights.Bold;

            ThicknessConverter tc222tbc = new ThicknessConverter();

            TableCell tcellfirstTb = new TableCell(new System.Windows.Documents.Paragraph(new Run("")));

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

            completeTable.Padding = new Thickness(12);
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
            //doc.Blocks.Add(signpara);


            doc.Name = "FlowDoc";
            //doc.PageWidth = 900;
            doc.PagePadding = new Thickness(20, 20, 20, 5); //v3
            //doc.PagePadding = new Thickness(30, 20, 10, 5); //V2 
            // Create IDocumentPaginatorSource from FlowDocument
            // IDocumentPaginatorSource idpSource = doc;
            // Call PrintDocument method to send document to printer



            return doc;


        }


        private void exportGSTR2To_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string sdt = startDateGstr2PurFull.SelectedDate.ToString();
                // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
                DateTime dt = Convert.ToDateTime(startDateGstr2PurFull.SelectedDate);
                //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                int years = dt.Year;
                string months = dt.Month.ToString();



                GSTR2FullSummaryGrid.SelectAllCells();

                //GSTR1FullSummaryGrid.Columns.RemoveAt(0); // because of this it throw one column less when creating invoice as it deleted column 

                //DataTable tempDt = new DataTable();
                //tempDt = DataGridtoDataTable(GSTR1FullSummaryGrid);

                GSTR2FullSummaryGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                ApplicationCommands.Copy.Execute(null, GSTR2FullSummaryGrid);

                GSTR2FullSummaryGrid.UnselectAllCells();

                String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
                //int billno = Convert.ToInt16(invoiceNumber.Text.Trim());
                try
                {
                    StreamWriter sw = new StreamWriter(@"C:\ViewBill\\GST\Purchase\GSTR2-" + months + "-" + years + ".csv");
                    sw.WriteLine(result);
                    sw.Close();

                    Process process = new Process();
                    process.StartInfo.UseShellExecute = true;

                    //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                    process.StartInfo.FileName = @"C:\ViewBill\\GST\Purchase\GSTR2-" + months + "-" + years + ".csv";

                    process.Start();
                    process.Close();


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



        //public static DataTable DataGridtoDataTable(DataGrid dg)
        //{
        //    dg.SelectAllCells();
        //    dg.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
        //    ApplicationCommands.Copy.Execute(null, dg);
        //    dg.UnselectAllCells();
        //    String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
        //    string[] Lines = result.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
        //    string[] Fields;
        //    Fields = Lines[0].Split(new char[] { ',' });
        //    int Cols = Fields.GetLength(0);
        //    DataTable dt = new DataTable();
        //    //1st row must be column names; force lower case to ensure matching later on.
        //    for (int i = 0; i < Cols; i++)
        //        dt.Columns.Add(Fields[i].ToUpper(), typeof(string));
        //    DataRow Row;
        //    for (int i = 1; i < Lines.GetLength(0) - 1; i++)
        //    {
        //        Fields = Lines[i].Split(new char[] { ',' });
        //        Row = dt.NewRow();
        //        for (int f = 0; f < Cols; f++)
        //        {
        //            Row[f] = Fields[f];
        //        }
        //        dt.Rows.Add(Row);
        //    }
        //    return dt;

        //}











        ////

        private void TabGSTR3BFull_Selected(object sender, RoutedEventArgs e)
        {
            TotalTaxableAmountOutward.Clear();
            TotalIGSTOutward.Clear();
            TotalTaxCGSTOutward.Clear();
            TotalTaxSGSTOutward.Clear();

            // autocompltCustNameSaleTab.autoTextBox.Text = "";
            string sdt = startDateGstr3bFull.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr3bFull.SelectedDate);
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

            string enddt = toDateGStr3bFull.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr3bFull.SelectedDate);
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
                SqlCommand com = new SqlCommand("GetTaxTypeDeatils", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                //com.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);

                System.Data.DataTable dt2 = new System.Data.DataTable("Sales Tax");
                sda.Fill(dt2);
                GSTR3BFullSummaryGrid.ItemsSource = dt2.DefaultView;
                GSTR3BFullSummaryGrid.AutoGenerateColumns = true;
                GSTR3BFullSummaryGrid.CanUserAddRows = false;
            }



            using (SqlConnection con = new SqlConnection())
            {

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetTaxTypeDeatilsSummary", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                //com.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double dTotalTaxableAmt = (reader["TotalTaxableAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    double dOutputCGST = (reader["OutputCGST"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;

                    double dOutputSGST = (reader["OutputSGST"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double dOutputIGST = (reader["OutputIGST"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;


                    //TotalWtgms.Text = dtotalweightgms.ToString();
                    TotalTaxableAmountOutward.Text = dTotalTaxableAmt.ToString();
                    //TotalTaxGST.Text = dtotalgsttaxams.ToString();
                    TotalTaxCGSTOutward.Text = dOutputCGST.ToString();
                    TotalTaxSGSTOutward.Text = dOutputSGST.ToString();
                    TotalIGSTOutward.Text = dOutputIGST.ToString();

                    //openingBalBookStartCR.Text = opBalBookStartCr.ToString();
                }

            }
        }

        private void Button_Click_GSTR3bFull(object sender, RoutedEventArgs e)
        {
            TotalTaxableAmountOutward.Clear();
            TotalIGSTOutward.Clear();
            TotalTaxCGSTOutward.Clear();
            TotalTaxSGSTOutward.Clear();

            // autocompltCustNameSaleTab.autoTextBox.Text = "";
            string sdt = startDateGstr3bFull.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr3bFull.SelectedDate);
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

            string enddt = toDateGStr3bFull.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr3bFull.SelectedDate);
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
                SqlCommand com = new SqlCommand("GetTaxTypeDeatils", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                //com.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);

                System.Data.DataTable dt2 = new System.Data.DataTable("Sales Tax");
                sda.Fill(dt2);
                GSTR3BFullSummaryGrid.ItemsSource = dt2.DefaultView;
                GSTR3BFullSummaryGrid.AutoGenerateColumns = true;
                GSTR3BFullSummaryGrid.CanUserAddRows = false;
            }



            using (SqlConnection con = new SqlConnection())
            {

                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                SqlCommand com = new SqlCommand("GetTaxTypeDeatilsSummary", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                //com.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    double dTotalTaxableAmt = (reader["TotalTaxableAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    double dOutputCGST = (reader["OutputCGST"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;

                    double dOutputSGST = (reader["OutputSGST"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double dOutputIGST = (reader["OutputIGST"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;


                    //TotalWtgms.Text = dtotalweightgms.ToString();
                    TotalTaxableAmountOutward.Text = dTotalTaxableAmt.ToString();
                    //TotalTaxGST.Text = dtotalgsttaxams.ToString();
                    TotalTaxCGSTOutward.Text = dOutputCGST.ToString();
                    TotalTaxSGSTOutward.Text = dOutputSGST.ToString();
                    TotalIGSTOutward.Text = dOutputIGST.ToString();

                    //openingBalBookStartCR.Text = opBalBookStartCr.ToString();
                }

            }
        }

        private void exportGSTR3bTo_Click(object sender, RoutedEventArgs e)
        {

            try
            {

                string sdt = startDateGstr2PurFull.SelectedDate.ToString();
                // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
                DateTime dt = Convert.ToDateTime(startDateGstr2PurFull.SelectedDate);
                //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                int years = dt.Year;
                string months = dt.Month.ToString();



                /////

                //if (FileUpload.HasFile)
                //       try
                //       {
                //           var excelApp = new Application();
                //           excelApp.Workbooks.Open("C:\\myFile.xlsx", Type.Missing, Type.Missing,
                //                                                  Type.Missing, Type.Missing,
                //                                                  Type.Missing, Type.Missing,
                //                                                  Type.Missing, Type.Missing,
                //                                                  Type.Missing, Type.Missing,
                //                                                  Type.Missing, Type.Missing,
                //                                                  Type.Missing, Type.Missing);
                //           var ws = excelApp.Worksheets;
                //           var worksheet = (Worksheet)ws.get_Item("Sheet1");
                //           Range range = worksheet.UsedRange;
                //           object[,] values = (object[,])range.Value2;

                //           for (int row = 1; row <= values.GetUpperBound(0); row++)
                //           {
                //               string phone = Convert.ToString(values[row, 2]);
                //               if (!phone.StartsWith("0"))
                //               {
                //                   phone = "0" + phone;
                //               }
                //               range.Cells.set_Item(row, 2, phone);
                //           }
                //           excelApp.Save("C:\\Leads.xls");
                //           excelApp.Quit();
                //      }
                //       catch (Exception ex)
                //       {
                //       }
                //   else
                //   {}






                ////////////
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;

                //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                process.StartInfo.FileName = @"C:\ViewBill\\GST\GSTR3B\GSTR3B_Excel_Utility_V4.1.xlsm";

                process.Start();
                process.Close();



                ///////////////

                //GSTR2FullSummaryGrid.SelectAllCells();

                ////GSTR1FullSummaryGrid.Columns.RemoveAt(0); // because of this it throw one column less when creating invoice as it deleted column 

                ////DataTable tempDt = new DataTable();
                ////tempDt = DataGridtoDataTable(GSTR1FullSummaryGrid);

                //GSTR2FullSummaryGrid.ClipboardCopyMode = DataGridClipboardCopyMode.IncludeHeader;
                //ApplicationCommands.Copy.Execute(null, GSTR2FullSummaryGrid);

                //GSTR2FullSummaryGrid.UnselectAllCells();

                //String result = (string)Clipboard.GetData(DataFormats.CommaSeparatedValue);
                ////int billno = Convert.ToInt16(invoiceNumber.Text.Trim());
                //try
                //{
                //    StreamWriter sw = new StreamWriter(@"C:\ViewBill\\GST\Purchase\GSTR2-" + months + "-" + years + ".csv");
                //    sw.WriteLine(result);
                //    sw.Close();

                //    Process process = new Process();
                //    process.StartInfo.UseShellExecute = true;

                //    //string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                //    process.StartInfo.FileName = @"C:\ViewBill\\GST\Purchase\GSTR2-" + months + "-" + years + ".csv";

                //    process.Start();
                //    process.Close();


                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}


            }
            catch (Exception ex)
            {
                MessageBox.Show("In Excel Export ");

            }

        }

        private void exportToPdfgstr1_Click(object sender, RoutedEventArgs e)
        {
            //  try
            //{
            // autocompltCustNameSaleTab.autoTextBox.Text = "";
            string sdt = startDateGstr1SaleFull.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr1SaleFull.SelectedDate);
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

            string enddt = toDateGStr1SaleFull.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr1SaleFull.SelectedDate);
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




            string BillDateInv = startDateGstr1SaleFull.SelectedDate.ToString();

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

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select * from Company where CompanyID = '" + CompID + "'";
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
            con.Close();



            ///
            // Font headerFONT = new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.BOLD, BaseColor.BLACK);
            Font allFONTsize = new Font(Font.FontFamily.TIMES_ROMAN, 8.5f, Font.NORMAL, BaseColor.BLACK);
            Font forFontSize = new Font(Font.FontFamily.COURIER, 8.5f, Font.UNDERLINE, BaseColor.BLACK);
            Font allFONTsizetotal = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.BLACK);
            // Font tinfont = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.NORMAL, BaseColor.BLACK);
            // Font dateInv = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.BLACK);
            //for table font 
            Font tablefontsize = new Font(Font.FontFamily.TIMES_ROMAN, 5f, Font.NORMAL, BaseColor.BLACK);
            Font tablefontsizeHeader = new Font(Font.FontFamily.TIMES_ROMAN, 5f, Font.BOLD, BaseColor.BLACK);

            Font taxslabAmtFont = new Font(Font.FontFamily.TIMES_ROMAN, 6.5f, Font.NORMAL, BaseColor.BLACK);
            Font termsFont = new Font(Font.FontFamily.TIMES_ROMAN, 4f, Font.BOLD, BaseColor.BLACK);
            Font BankDetailFont = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.NORMAL, BaseColor.BLACK);

            //PdfPTable table = new iTextSharp.text.pdf.PdfPTable(CartGrid.Columns.Count) { TotalWidth = 390, LockedWidth = true };




            Font smallfont = new Font(Font.FontFamily.TIMES_ROMAN, 6f, Font.NORMAL, BaseColor.BLACK);



            // long rupeesFig = Convert.ToInt64(Math.Round((Convert.ToDouble(totalInvValues)), 0));

            // string reupeesWords = ConvertNumbertoWords(rupeesFig);

            Font WwordsFormat = new Font(Font.FontFamily.TIMES_ROMAN, 7.5f, Font.NORMAL, BaseColor.BLACK);


            //Remove all special character from textBoxCustName
            FileStream fs = File.Open(@"C:\ViewBill\" + "GSTR1-Sale-" + months+ "-"+years+".pdf", FileMode.Create);


            using (MemoryStream output = new MemoryStream())
            {

                Document document = new Document(iTextSharp.text.PageSize.A5, 2f, 2f, 15f, 2f);// from 159
                //commented below for memort=y stream
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                //PdfWriter writer = PdfWriter.GetInstance(document, output);

                ///



                document.Open();


                IEnumerable itemsSource = GSTR1FullSummaryGrid.ItemsSource as IEnumerable;
                if (itemsSource != null)
                {


                    SqlConnection conpdf = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                    conpdf.Open();
                    //string sqlpdf = "SELECT row_number() OVER (order by srnumber ) Sr ,[ItemName] As [Item Name]  ,[HSN],[Size] ,[BilledQty] As [Qty] ,[UnitID] As [Uom],[SalePrice] As [Price],Amount ,[Discount] As [Disc%] ,[TaxablelAmount] As [Taxable] ,[GSTRate] As [GST%] ,[TotalAmount] As [Total]   FROM [SalesVoucherInventorycloths] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "'";
                    //SqlCommand cmdpdf = new SqlCommand(sqlpdf);
                    //cmdpdf.Connection = conpdf;
                    //SqlDataAdapter sda = new SqlDataAdapter(cmdpdf);

                    SqlCommand cmdpdf = new SqlCommand("GetGSTR1ByType", conpdf);
                    cmdpdf.CommandType = CommandType.StoredProcedure;
                    cmdpdf.Parameters.Add(new SqlParameter("@StartDate", sdt));
                    cmdpdf.Parameters.Add(new SqlParameter("@EndDate", enddt));
                    cmdpdf.Parameters.Add(new SqlParameter("@CompID", CompID));
                    cmdpdf.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                    SqlDataAdapter sda = new SqlDataAdapter(cmdpdf);

                    DataTable dttable = new DataTable("Inv");
                    sda.Fill(dttable);




                    System.Data.DataTable dt2 = new System.Data.DataTable("Sale GST Summary");
                    sda.Fill(dt2);
                    GSTR1FullSummaryGrid.ItemsSource = dt2.DefaultView;
                    GSTR1FullSummaryGrid.AutoGenerateColumns = true;
                    GSTR1FullSummaryGrid.CanUserAddRows = false;







                    PdfPTable table = new iTextSharp.text.pdf.PdfPTable(dttable.Columns.Count) { TotalWidth = 390, LockedWidth = true };
                    //float[] widths = new float[] { 30, 50, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 }; //remove disc and taxable
                    //table.SetWidths(widths);
                    table.HeaderRows = 1;

                    foreach (DataColumn c in dttable.Columns)
                    {

                        table.AddCell(new Phrase(c.ColumnName, tablefontsizeHeader));
                    }




                    PdfPCell PdfPCellhsn = null;

                    for (int rows = 0; rows < dttable.Rows.Count; rows++)
                    {
                        for (int column = 0; column < dttable.Columns.Count; column++)
                        {
                            //if (dttable.Rows[rows][column].ToString() != "0")
                            //{
                            PdfPCellhsn = new PdfPCell(new Phrase(new Chunk(dttable.Rows[rows][column].ToString(), tablefontsize)));

                            //}

                            //if ((rows == dttable.Rows.Count - 1) && (column == dttable.Columns.Count - 1))
                            //{


                            //    float totaltblHorizntal = totalTableHorizontal.TotalHeight;
                            //    float totalTableHight = totalTable.TotalHeight;
                            //    float ttlhght = table.TotalHeight;
                            //    // float footerTblehght = footerTable.TotalHeight;
                            //    //float bankseparateTaxheght = bankseparateTax.TotalHeight; 
                            //    float bankseparateTaxheght = 60;//60;
                            //    float footertablehght = 189;//189;
                            //    float maxhght = document.PageSize.Height;
                            //    float balancehght = maxhght - (ttlhght + footertablehght + bankseparateTaxheght + totalTableHight + totaltblHorizntal);

                            //    Phrase newPhrase = new Phrase("");
                            //    iTextSharp.text.pdf.PdfPCell newCell = new iTextSharp.text.pdf.PdfPCell(newPhrase);
                            //    newCell.FixedHeight = balancehght;
                            //    //table.AddCell(newCell);

                            //    PdfPCellhsn.FixedHeight = balancehght;
                            //    table.AddCell(PdfPCellhsn);


                            //}
                            //else
                            table.AddCell(PdfPCellhsn);
                        }

                    }



                    double sumGST = 0;
                    double sumTaxable = 0;
                    //for (int s = 0; s < AccountLedgerGrid.Items.Count - 1; s++ )
                    //{
                    //    sumDr += (double.Parse((AccountLedgerGrid.Columns[5].GetCellContent(AccountLedgerGrid.Items[s]) as TextBlock).Text));
                    //}
                    foreach (DataRow row in dttable.Rows)
                    {
                        //sumDr +=  Convert.ToDouble(row["DR"]);
                        sumGST = sumGST + ((row["GST"] != DBNull.Value) ? (Convert.ToDouble(row["GST"])) : 0);
                        sumTaxable = sumTaxable + ((row["Taxable Value"] != DBNull.Value) ? (Convert.ToDouble(row["Taxable Value"])) : 0);
                    }
                    //TotalDebitAmt_Ledger.Text = sumDr.ToString();
                    //TotalCreditAmt_Ledger.Text = sumCr.ToString();
                    //Balance_Ledger.Text = (sumDr - sumCr).ToString();

                    // TotalTable Start Here 
                    PdfPTable totalTable = new iTextSharp.text.pdf.PdfPTable(2) { TotalWidth = 390, LockedWidth = true };


                    float[] widthsTotalTable = new float[] { 200, 150};
                    totalTable.SetWidths(widthsTotalTable);



                    PdfPTable totaltableVerticalalign = new iTextSharp.text.pdf.PdfPTable(1);
                    totaltableVerticalalign.DefaultCell.Border = 0;

                    totaltableVerticalalign.AddCell(new Phrase("Total Taxable Value:", allFONTsizetotal));

                    totaltableVerticalalign.AddCell(new Phrase("Total Output GST:", allFONTsizetotal));


                    //if (disc.Text != "")
                    //{
                    //    totaltableVerticalalign.AddCell(new Phrase("Discount:@" + discountperc.Text + "%", allFONTsizetotal));
                    //}
                    Font colorHighlight = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.RED);

                    totaltableVerticalalign.DefaultCell.Rowspan = 2;
                    totaltableVerticalalign.DefaultCell.BorderWidthRight = 0;
                    totaltableVerticalalign.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    PdfPCell totaltableVerticalalignCell = new PdfPCell();
                    totaltableVerticalalignCell.BorderWidthRight = 0;
                    totaltableVerticalalignCell.AddElement(totaltableVerticalalign);


              


                    PdfPTable totaltableVerticalalign1 = new iTextSharp.text.pdf.PdfPTable(1);
                    totaltableVerticalalign1.DefaultCell.Border = 0;
                    // Chunk chunkRupee = new Chunk(" \u20B9 5410", allFONTsize); ₹
                    BaseFont bf = BaseFont.CreateFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font font3 = new Font(bf, 7);
                    //Chunk chunkRupee = new Chunk(" \u20B9", font3);

                    Phrase totalsumrupee = new Phrase(" \u20B9 ", font3);
                    totalsumrupee.Add(new Phrase(Math.Round(sumTaxable, 2).ToString(), allFONTsizetotal));

                    Phrase gstamtttl = new Phrase(" \u20B9 ", font3);
                    gstamtttl.Add(new Phrase(Math.Round(sumGST, 2).ToString(), allFONTsizetotal));

                    totaltableVerticalalign1.AddCell(totalsumrupee);
                    totaltableVerticalalign1.AddCell(gstamtttl);

                    PdfPCell totaltableVerticalalignCell1 = new PdfPCell();
                    totaltableVerticalalignCell1.BorderWidthRight = 1;
                    totaltableVerticalalignCell1.AddElement(totaltableVerticalalign1);

                    totalTable.AddCell(totaltableVerticalalignCell);
                    totalTable.AddCell(totaltableVerticalalignCell1);



                    iTextSharp.text.Paragraph p4 = new iTextSharp.text.Paragraph();
                    Phrase pht1 = new Phrase(CompanyName + "\n", forFontSize);
                    Phrase pht2 = new Phrase("GSTR1-Sale- Report -" + months + "-" + years + "\n", allFONTsizetotal);
                    //Phrase pht3 = new Phrase("GSTR1- Report" + CompanyName + "\n", allFONTsize);
                    Font chunkguru = new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.BOLD, BaseColor.BLACK);

                    Font chunkInvDateInv = new Font(Font.FontFamily.TIMES_ROMAN, 8.6f, Font.BOLD, BaseColor.BLACK);

                    //Phrase pht2 = new Phrase("          --om--  " + "\n" + "       INVOICE", chunkguru);
                    //p4.Add(pht2);

                    p4.Add(pht1);
                    Phrase pht3 = new Phrase("               ", allFONTsize);
                    p4.Add(pht2);
                    p4.Add(pht3);
                    p4.Alignment = 1;

                    iTextSharp.text.Paragraph p5 = new iTextSharp.text.Paragraph();
                    Phrase pht10 = new Phrase(" \n", allFONTsizetotal);
                    p5.Add(pht10);


                    document.Add(p4);
                    //document.Add(jpg2);

                    document.Add(table);
                    document.Add(p5);
                    document.Add(totalTable);
                    //document.Add(totaltableVerticalalign1);

                    document.Close();

                    //commented for memory stream
                    writer.Close();

                    fs.Close();



                }
            }
            //  }
            //catch(Exception ex)
            //  {

            //}


        }

        private void exportToPdfgstr2_Click(object sender, RoutedEventArgs e)
        {

            //  try
            //{
            // autocompltCustNameSaleTab.autoTextBox.Text = "";
            string sdt = startDateGstr2PurFull.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr2PurFull.SelectedDate);
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

            string enddt = toDateGStr2PurFull.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr2PurFull.SelectedDate);
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




            string BillDateInv = startDateGstr2PurFull.SelectedDate.ToString();

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

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select * from Company where CompanyID = '" + CompID + "'";
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
            con.Close();



            ///
            // Font headerFONT = new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.BOLD, BaseColor.BLACK);
            Font allFONTsize = new Font(Font.FontFamily.TIMES_ROMAN, 8.5f, Font.NORMAL, BaseColor.BLACK);
            Font forFontSize = new Font(Font.FontFamily.COURIER, 8.5f, Font.UNDERLINE, BaseColor.BLACK);
            Font allFONTsizetotal = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.BLACK);
            // Font tinfont = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.NORMAL, BaseColor.BLACK);
            // Font dateInv = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.BLACK);
            //for table font 
            Font tablefontsize = new Font(Font.FontFamily.TIMES_ROMAN, 5f, Font.NORMAL, BaseColor.BLACK);
            Font tablefontsizeHeader = new Font(Font.FontFamily.TIMES_ROMAN, 5f, Font.BOLD, BaseColor.BLACK);

            Font taxslabAmtFont = new Font(Font.FontFamily.TIMES_ROMAN, 6.5f, Font.NORMAL, BaseColor.BLACK);
            Font termsFont = new Font(Font.FontFamily.TIMES_ROMAN, 4f, Font.BOLD, BaseColor.BLACK);
            Font BankDetailFont = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.NORMAL, BaseColor.BLACK);

            //PdfPTable table = new iTextSharp.text.pdf.PdfPTable(CartGrid.Columns.Count) { TotalWidth = 390, LockedWidth = true };




            Font smallfont = new Font(Font.FontFamily.TIMES_ROMAN, 6f, Font.NORMAL, BaseColor.BLACK);



            // long rupeesFig = Convert.ToInt64(Math.Round((Convert.ToDouble(totalInvValues)), 0));

            // string reupeesWords = ConvertNumbertoWords(rupeesFig);

            Font WwordsFormat = new Font(Font.FontFamily.TIMES_ROMAN, 7.5f, Font.NORMAL, BaseColor.BLACK);


            //Remove all special character from textBoxCustName
            FileStream fs = File.Open(@"C:\ViewBill\" + "GSTR2-Purchase- " + months + "-" + years + ".pdf", FileMode.Create);


            using (MemoryStream output = new MemoryStream())
            {

                Document document = new Document(iTextSharp.text.PageSize.A5, 2f, 2f, 15f, 2f);// from 159
                //commented below for memort=y stream
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                //PdfWriter writer = PdfWriter.GetInstance(document, output);

                ///



                document.Open();


                IEnumerable itemsSource = GSTR2FullSummaryGrid.ItemsSource as IEnumerable;
                if (itemsSource != null)
                {


                    SqlConnection conpdf = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                    conpdf.Open();
                    //string sqlpdf = "SELECT row_number() OVER (order by srnumber ) Sr ,[ItemName] As [Item Name]  ,[HSN],[Size] ,[BilledQty] As [Qty] ,[UnitID] As [Uom],[SalePrice] As [Price],Amount ,[Discount] As [Disc%] ,[TaxablelAmount] As [Taxable] ,[GSTRate] As [GST%] ,[TotalAmount] As [Total]   FROM [SalesVoucherInventorycloths] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "'";
                    //SqlCommand cmdpdf = new SqlCommand(sqlpdf);
                    //cmdpdf.Connection = conpdf;
                    //SqlDataAdapter sda = new SqlDataAdapter(cmdpdf);

                    SqlCommand cmdpdf = new SqlCommand("GetGSTR2ByType", conpdf);
                    cmdpdf.CommandType = CommandType.StoredProcedure;
                    cmdpdf.Parameters.Add(new SqlParameter("@StartDate", sdt));
                    cmdpdf.Parameters.Add(new SqlParameter("@EndDate", enddt));
                    cmdpdf.Parameters.Add(new SqlParameter("@CompID", CompID));
                    cmdpdf.Parameters.Add(new SqlParameter("@Type", GSTRType.Text.Trim()));
                    SqlDataAdapter sda = new SqlDataAdapter(cmdpdf);

                    DataTable dttable = new DataTable("Inv");
                    sda.Fill(dttable);




                    System.Data.DataTable dt2 = new System.Data.DataTable("Purchase GST Summary");
                    sda.Fill(dt2);
                    GSTR2FullSummaryGrid.ItemsSource = dt2.DefaultView;
                    GSTR2FullSummaryGrid.AutoGenerateColumns = true;
                    GSTR2FullSummaryGrid.CanUserAddRows = false;







                    PdfPTable table = new iTextSharp.text.pdf.PdfPTable(dttable.Columns.Count) { TotalWidth = 390, LockedWidth = true };
                    //float[] widths = new float[] { 30, 50, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 }; //remove disc and taxable
                    //table.SetWidths(widths);
                    table.HeaderRows = 1;

                    foreach (DataColumn c in dttable.Columns)
                    {

                        table.AddCell(new Phrase(c.ColumnName, tablefontsizeHeader));
                    }




                    PdfPCell PdfPCellhsn = null;

                    for (int rows = 0; rows < dttable.Rows.Count; rows++)
                    {
                        for (int column = 0; column < dttable.Columns.Count; column++)
                        {
                            //if (dttable.Rows[rows][column].ToString() != "0")
                            //{
                            PdfPCellhsn = new PdfPCell(new Phrase(new Chunk(dttable.Rows[rows][column].ToString(), tablefontsize)));

                            //}

                            //if ((rows == dttable.Rows.Count - 1) && (column == dttable.Columns.Count - 1))
                            //{


                            //    float totaltblHorizntal = totalTableHorizontal.TotalHeight;
                            //    float totalTableHight = totalTable.TotalHeight;
                            //    float ttlhght = table.TotalHeight;
                            //    // float footerTblehght = footerTable.TotalHeight;
                            //    //float bankseparateTaxheght = bankseparateTax.TotalHeight; 
                            //    float bankseparateTaxheght = 60;//60;
                            //    float footertablehght = 189;//189;
                            //    float maxhght = document.PageSize.Height;
                            //    float balancehght = maxhght - (ttlhght + footertablehght + bankseparateTaxheght + totalTableHight + totaltblHorizntal);

                            //    Phrase newPhrase = new Phrase("");
                            //    iTextSharp.text.pdf.PdfPCell newCell = new iTextSharp.text.pdf.PdfPCell(newPhrase);
                            //    newCell.FixedHeight = balancehght;
                            //    //table.AddCell(newCell);

                            //    PdfPCellhsn.FixedHeight = balancehght;
                            //    table.AddCell(PdfPCellhsn);


                            //}
                            //else
                            table.AddCell(PdfPCellhsn);
                        }

                    }



                    double sumGST = 0;
                    double sumTaxable = 0;
                    //for (int s = 0; s < AccountLedgerGrid.Items.Count - 1; s++ )
                    //{
                    //    sumDr += (double.Parse((AccountLedgerGrid.Columns[5].GetCellContent(AccountLedgerGrid.Items[s]) as TextBlock).Text));
                    //}
                    foreach (DataRow row in dttable.Rows)
                    {
                        //sumDr +=  Convert.ToDouble(row["DR"]);
                        sumGST = sumGST + ((row["GST"] != DBNull.Value) ? (Convert.ToDouble(row["GST"])) : 0);
                        sumTaxable = sumTaxable + ((row["Taxable Value"] != DBNull.Value) ? (Convert.ToDouble(row["Taxable Value"])) : 0);
                    }
                    //TotalDebitAmt_Ledger.Text = sumDr.ToString();
                    //TotalCreditAmt_Ledger.Text = sumCr.ToString();
                    //Balance_Ledger.Text = (sumDr - sumCr).ToString();

                    // TotalTable Start Here 
                    PdfPTable totalTable = new iTextSharp.text.pdf.PdfPTable(2) { TotalWidth = 390, LockedWidth = true };


                    float[] widthsTotalTable = new float[] { 200, 150 };
                    totalTable.SetWidths(widthsTotalTable);



                    PdfPTable totaltableVerticalalign = new iTextSharp.text.pdf.PdfPTable(1);
                    totaltableVerticalalign.DefaultCell.Border = 0;

                    totaltableVerticalalign.AddCell(new Phrase("Total Taxable Value:", allFONTsizetotal));

                    totaltableVerticalalign.AddCell(new Phrase("Total Input GST:", allFONTsizetotal));


                    //if (disc.Text != "")
                    //{
                    //    totaltableVerticalalign.AddCell(new Phrase("Discount:@" + discountperc.Text + "%", allFONTsizetotal));
                    //}
                    Font colorHighlight = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.RED);

                    totaltableVerticalalign.DefaultCell.Rowspan = 2;
                    totaltableVerticalalign.DefaultCell.BorderWidthRight = 0;
                    totaltableVerticalalign.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    PdfPCell totaltableVerticalalignCell = new PdfPCell();
                    totaltableVerticalalignCell.BorderWidthRight = 0;
                    totaltableVerticalalignCell.AddElement(totaltableVerticalalign);





                    PdfPTable totaltableVerticalalign1 = new iTextSharp.text.pdf.PdfPTable(1);
                    totaltableVerticalalign1.DefaultCell.Border = 0;
                    // Chunk chunkRupee = new Chunk(" \u20B9 5410", allFONTsize); ₹
                    BaseFont bf = BaseFont.CreateFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font font3 = new Font(bf, 7);
                    //Chunk chunkRupee = new Chunk(" \u20B9", font3);

                    Phrase totalsumrupee = new Phrase(" \u20B9 ", font3);
                    totalsumrupee.Add(new Phrase(Math.Round(sumTaxable, 2).ToString(), allFONTsizetotal));

                    Phrase gstamtttl = new Phrase(" \u20B9 ", font3);
                    gstamtttl.Add(new Phrase(Math.Round(sumGST, 2).ToString(), allFONTsizetotal));

                    totaltableVerticalalign1.AddCell(totalsumrupee);
                    totaltableVerticalalign1.AddCell(gstamtttl);

                    PdfPCell totaltableVerticalalignCell1 = new PdfPCell();
                    totaltableVerticalalignCell1.BorderWidthRight = 1;
                    totaltableVerticalalignCell1.AddElement(totaltableVerticalalign1);

                    totalTable.AddCell(totaltableVerticalalignCell);
                    totalTable.AddCell(totaltableVerticalalignCell1);



                    iTextSharp.text.Paragraph p4 = new iTextSharp.text.Paragraph();
                    Phrase pht1 = new Phrase(CompanyName + "\n", forFontSize);
                    Phrase pht2 = new Phrase("GSTR2-Purchase- Report -" + months + "-" + years + "\n", allFONTsizetotal);
                    //Phrase pht3 = new Phrase("GSTR1- Report" + CompanyName + "\n", allFONTsize);
                    Font chunkguru = new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.BOLD, BaseColor.BLACK);

                    Font chunkInvDateInv = new Font(Font.FontFamily.TIMES_ROMAN, 8.6f, Font.BOLD, BaseColor.BLACK);

                    //Phrase pht2 = new Phrase("          --om--  " + "\n" + "       INVOICE", chunkguru);
                    //p4.Add(pht2);

                    p4.Add(pht1);
                    Phrase pht3 = new Phrase("               ", allFONTsize);
                    p4.Add(pht2);
                    p4.Add(pht3);
                    p4.Alignment = 1;

                    iTextSharp.text.Paragraph p5 = new iTextSharp.text.Paragraph();
                    Phrase pht10 = new Phrase(" \n", allFONTsizetotal);
                    p5.Add(pht10);


                    document.Add(p4);
                    //document.Add(jpg2);

                    document.Add(table);
                    document.Add(p5);
                    document.Add(totalTable);
                    //document.Add(totaltableVerticalalign1);

                    document.Close();

                    //commented for memory stream
                    writer.Close();

                    fs.Close();



                }
            }
            //  }
            //catch(Exception ex)
            //  {

            //}

        }

        private void exportToPdfCategory_Click(object sender, RoutedEventArgs e)
        {
            //  try
            //{
            // autocompltCustNameSaleTab.autoTextBox.Text = "";
            string sdt = startDateGstr1Sale.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateGstr1Sale.SelectedDate);
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

            string enddt = toDateGStr1Sale.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateGStr1Sale.SelectedDate);
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




            string BillDateInv = startDateGstr1Sale.SelectedDate.ToString();

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

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select * from Company where CompanyID = '" + CompID + "'";
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
            con.Close();



            ///
            // Font headerFONT = new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.BOLD, BaseColor.BLACK);
            Font allFONTsize = new Font(Font.FontFamily.TIMES_ROMAN, 8.5f, Font.NORMAL, BaseColor.BLACK);
            Font forFontSize = new Font(Font.FontFamily.COURIER, 8.5f, Font.UNDERLINE, BaseColor.BLACK);
            Font allFONTsizetotal = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.BLACK);
            // Font tinfont = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.NORMAL, BaseColor.BLACK);
            // Font dateInv = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.BLACK);
            //for table font 
            Font tablefontsize = new Font(Font.FontFamily.TIMES_ROMAN, 5f, Font.NORMAL, BaseColor.BLACK);
            Font tablefontsizeHeader = new Font(Font.FontFamily.TIMES_ROMAN, 5f, Font.BOLD, BaseColor.BLACK);

            Font taxslabAmtFont = new Font(Font.FontFamily.TIMES_ROMAN, 6.5f, Font.NORMAL, BaseColor.BLACK);
            Font termsFont = new Font(Font.FontFamily.TIMES_ROMAN, 4f, Font.BOLD, BaseColor.BLACK);
            Font BankDetailFont = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.NORMAL, BaseColor.BLACK);

            //PdfPTable table = new iTextSharp.text.pdf.PdfPTable(CartGrid.Columns.Count) { TotalWidth = 390, LockedWidth = true };




            Font smallfont = new Font(Font.FontFamily.TIMES_ROMAN, 6f, Font.NORMAL, BaseColor.BLACK);



            // long rupeesFig = Convert.ToInt64(Math.Round((Convert.ToDouble(totalInvValues)), 0));

            // string reupeesWords = ConvertNumbertoWords(rupeesFig);

            Font WwordsFormat = new Font(Font.FontFamily.TIMES_ROMAN, 7.5f, Font.NORMAL, BaseColor.BLACK);


            //Remove all special character from textBoxCustName
            FileStream fs = File.Open(@"C:\ViewBill\" + "Sale- " + GroupName.Text.Trim() +" -" + months + "-" + years + ".pdf", FileMode.Create);


            using (MemoryStream output = new MemoryStream())
            {

                Document document = new Document(iTextSharp.text.PageSize.A5, 2f, 2f, 15f, 2f);// from 159
                //commented below for memort=y stream
                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                //PdfWriter writer = PdfWriter.GetInstance(document, output);

                ///



                document.Open();


                IEnumerable itemsSource = GSTR1SummaryGrid.ItemsSource as IEnumerable;
                if (itemsSource != null)
                {


                    SqlConnection conpdf = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                    conpdf.Open();
                    //string sqlpdf = "SELECT row_number() OVER (order by srnumber ) Sr ,[ItemName] As [Item Name]  ,[HSN],[Size] ,[BilledQty] As [Qty] ,[UnitID] As [Uom],[SalePrice] As [Price],Amount ,[Discount] As [Disc%] ,[TaxablelAmount] As [Taxable] ,[GSTRate] As [GST%] ,[TotalAmount] As [Total]   FROM [SalesVoucherInventorycloths] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "'";
                    //SqlCommand cmdpdf = new SqlCommand(sqlpdf);
                    //cmdpdf.Connection = conpdf;
                    //SqlDataAdapter sda = new SqlDataAdapter(cmdpdf);

                    SqlCommand cmdpdf = new SqlCommand("GetSaleGSTReportForJewellery", conpdf);
                    cmdpdf.CommandType = CommandType.StoredProcedure;
                    cmdpdf.Parameters.Add(new SqlParameter("@StartDate", sdt));
                    cmdpdf.Parameters.Add(new SqlParameter("@EndDate", enddt));
                    cmdpdf.Parameters.Add(new SqlParameter("@CompID", CompID));
                    cmdpdf.Parameters.Add(new SqlParameter("@GroupName", GroupName.Text.Trim()));
                    SqlDataAdapter sda = new SqlDataAdapter(cmdpdf);

                    DataTable dttable = new DataTable("Inv");
                    sda.Fill(dttable);




                    System.Data.DataTable dt2 = new System.Data.DataTable("Sale Category");
                    sda.Fill(dt2);
                    GSTR1SummaryGrid.ItemsSource = dt2.DefaultView;
                    GSTR1SummaryGrid.AutoGenerateColumns = true;
                    GSTR1SummaryGrid.CanUserAddRows = false;







                    PdfPTable table = new iTextSharp.text.pdf.PdfPTable(dttable.Columns.Count) { TotalWidth = 390, LockedWidth = true };
                    //float[] widths = new float[] { 30, 50, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30 }; //remove disc and taxable
                    //table.SetWidths(widths);
                    table.HeaderRows = 1;

                    foreach (DataColumn c in dttable.Columns)
                    {

                        table.AddCell(new Phrase(c.ColumnName, tablefontsizeHeader));
                    }




                    PdfPCell PdfPCellhsn = null;

                    for (int rows = 0; rows < dttable.Rows.Count; rows++)
                    {
                        for (int column = 0; column < dttable.Columns.Count; column++)
                        {
                            //if (dttable.Rows[rows][column].ToString() != "0")
                            //{
                            PdfPCellhsn = new PdfPCell(new Phrase(new Chunk(dttable.Rows[rows][column].ToString(), tablefontsize)));

                            //}

                            //if ((rows == dttable.Rows.Count - 1) && (column == dttable.Columns.Count - 1))
                            //{


                            //    float totaltblHorizntal = totalTableHorizontal.TotalHeight;
                            //    float totalTableHight = totalTable.TotalHeight;
                            //    float ttlhght = table.TotalHeight;
                            //    // float footerTblehght = footerTable.TotalHeight;
                            //    //float bankseparateTaxheght = bankseparateTax.TotalHeight; 
                            //    float bankseparateTaxheght = 60;//60;
                            //    float footertablehght = 189;//189;
                            //    float maxhght = document.PageSize.Height;
                            //    float balancehght = maxhght - (ttlhght + footertablehght + bankseparateTaxheght + totalTableHight + totaltblHorizntal);

                            //    Phrase newPhrase = new Phrase("");
                            //    iTextSharp.text.pdf.PdfPCell newCell = new iTextSharp.text.pdf.PdfPCell(newPhrase);
                            //    newCell.FixedHeight = balancehght;
                            //    //table.AddCell(newCell);

                            //    PdfPCellhsn.FixedHeight = balancehght;
                            //    table.AddCell(PdfPCellhsn);


                            //}
                            //else
                            table.AddCell(PdfPCellhsn);
                        }

                    }



                    double sumGST = 0;
                    double sumTaxable = 0;
                    double sumWtGms = 0;
                    //for (int s = 0; s < AccountLedgerGrid.Items.Count - 1; s++ )
                    //{
                    //    sumDr += (double.Parse((AccountLedgerGrid.Columns[5].GetCellContent(AccountLedgerGrid.Items[s]) as TextBlock).Text));
                    //}
                    foreach (DataRow row in dttable.Rows)
                    {
                        //sumDr +=  Convert.ToDouble(row["DR"]);
                        sumGST = sumGST + ((row["GSTTax"] != DBNull.Value) ? (Convert.ToDouble(row["GSTTax"])) : 0);
                        sumTaxable = sumTaxable + ((row["TaxableAmount"] != DBNull.Value) ? (Convert.ToDouble(row["TaxableAmount"])) : 0);
                        sumWtGms = sumWtGms + ((row["Wt(gms)"] != DBNull.Value) ? (Convert.ToDouble(row["Wt(gms)"])) : 0);
                    }
                    //TotalDebitAmt_Ledger.Text = sumDr.ToString();
                    //TotalCreditAmt_Ledger.Text = sumCr.ToString();
                    //Balance_Ledger.Text = (sumDr - sumCr).ToString();

                    // TotalTable Start Here 
                    PdfPTable totalTable = new iTextSharp.text.pdf.PdfPTable(2) { TotalWidth = 390, LockedWidth = true };


                    float[] widthsTotalTable = new float[] { 200, 150 };
                    totalTable.SetWidths(widthsTotalTable);



                    PdfPTable totaltableVerticalalign = new iTextSharp.text.pdf.PdfPTable(1);
                    totaltableVerticalalign.DefaultCell.Border = 0;

                    totaltableVerticalalign.AddCell(new Phrase("Total Taxable Value:", allFONTsizetotal));

                    totaltableVerticalalign.AddCell(new Phrase("Total Output GST:", allFONTsizetotal));

                    totaltableVerticalalign.AddCell(new Phrase("Total Wt(gms):", allFONTsizetotal));


                    //if (disc.Text != "")
                    //{
                    //    totaltableVerticalalign.AddCell(new Phrase("Discount:@" + discountperc.Text + "%", allFONTsizetotal));
                    //}
                    Font colorHighlight = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.RED);

                    totaltableVerticalalign.DefaultCell.Rowspan = 2;
                    totaltableVerticalalign.DefaultCell.BorderWidthRight = 0;
                    totaltableVerticalalign.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    PdfPCell totaltableVerticalalignCell = new PdfPCell();
                    totaltableVerticalalignCell.BorderWidthRight = 0;
                    totaltableVerticalalignCell.AddElement(totaltableVerticalalign);





                    PdfPTable totaltableVerticalalign1 = new iTextSharp.text.pdf.PdfPTable(1);
                    totaltableVerticalalign1.DefaultCell.Border = 0;
                    // Chunk chunkRupee = new Chunk(" \u20B9 5410", allFONTsize); ₹
                    BaseFont bf = BaseFont.CreateFont("c:/windows/fonts/arial.ttf", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    Font font3 = new Font(bf, 7);
                    //Chunk chunkRupee = new Chunk(" \u20B9", font3);

                    Phrase totalsumrupee = new Phrase(" \u20B9 ", font3);
                    totalsumrupee.Add(new Phrase(Math.Round(sumTaxable, 2).ToString(), allFONTsizetotal));

                    Phrase gstamtttl = new Phrase(" \u20B9 ", font3);
                    gstamtttl.Add(new Phrase(Math.Round(sumGST, 2).ToString(), allFONTsizetotal));

                    Phrase ttlWtGms = new Phrase("  ", font3);
                    ttlWtGms.Add(new Phrase(Math.Round(sumWtGms, 2).ToString(), allFONTsizetotal));

                    totaltableVerticalalign1.AddCell(totalsumrupee);
                    totaltableVerticalalign1.AddCell(gstamtttl);
                    totaltableVerticalalign1.AddCell(ttlWtGms);

                    PdfPCell totaltableVerticalalignCell1 = new PdfPCell();
                    totaltableVerticalalignCell1.BorderWidthRight = 1;
                    totaltableVerticalalignCell1.AddElement(totaltableVerticalalign1);

                    totalTable.AddCell(totaltableVerticalalignCell);
                    totalTable.AddCell(totaltableVerticalalignCell1);



                    iTextSharp.text.Paragraph p4 = new iTextSharp.text.Paragraph();
                    Phrase pht1 = new Phrase(CompanyName + "\n", forFontSize);
                    Phrase pht2 = new Phrase("Sale- "+ GroupName.Text.Trim() +"- " + months + "-" + years + "\n", allFONTsizetotal);
                    //Phrase pht3 = new Phrase("GSTR1- Report" + CompanyName + "\n", allFONTsize);
                    Font chunkguru = new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.BOLD, BaseColor.BLACK);

                    Font chunkInvDateInv = new Font(Font.FontFamily.TIMES_ROMAN, 8.6f, Font.BOLD, BaseColor.BLACK);

                    //Phrase pht2 = new Phrase("          --om--  " + "\n" + "       INVOICE", chunkguru);
                    //p4.Add(pht2);

                    p4.Add(pht1);
                    Phrase pht3 = new Phrase("               ", allFONTsize);
                    p4.Add(pht2);
                    p4.Add(pht3);
                    p4.Alignment = 1;

                    iTextSharp.text.Paragraph p5 = new iTextSharp.text.Paragraph();
                    Phrase pht10 = new Phrase(" \n", allFONTsizetotal);
                    p5.Add(pht10);


                    document.Add(p4);
                    //document.Add(jpg2);

                    document.Add(table);
                    document.Add(p5);
                    document.Add(totalTable);
                    //document.Add(totaltableVerticalalign1);

                    document.Close();

                    //commented for memory stream
                    writer.Close();

                    fs.Close();



                }
            }
            //  }
            //catch(Exception ex)
            //  {

            //}
        }


    }
}