using iTextSharp.text;
using RTSJewelERP.TrayListTableAdapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Printing;
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
    public partial class RokadMonthlyReports : Window
    {

        public List<string> CountryList { get; set; }
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public RokadMonthlyReports()
        {
            InitializeComponent();
           // BindComboBoxTrayList(cmbTrayLists);
            //itemnames = itemName;
            //companyId = CompID;
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window

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



        

        private void TabTrialBalance_Selected(object sender, RoutedEventArgs e)
        {
            //goldInInvent.Clear();
            //// goldOutInvent.Clear();
            //oldGoldInInvent.Clear();
            ////oldGoldOutInvent.Clear();
            //silverInInvent.Clear();
            ////  silverOutInvent.Clear();

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


            sdt = years + "/" + 04 + "/" + 01;
            startDateTrialBalance.Text = sdt;

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

                SqlCommand com = new SqlCommand("GetRokadTrialBalance", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
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
                    sumDr = sumDr + ((row["DR(Namme)"] != DBNull.Value) ? (Convert.ToDouble(row["DR(Namme)"])) : 0);
                    sumCr = sumCr + ((row["CR(Jamma)"] != DBNull.Value) ? (Convert.ToDouble(row["CR(Jamma)"])) : 0);
                }
                totalDRTrialBal.Text = sumDr.ToString();
                totalCRTrialBal.Text = sumCr.ToString();

                lblRokadDiff.Content = string.Format("पोते बाकी नामे(ClosingBal): {0}", (Math.Round(sumCr - sumDr, 0)).ToString("C"));

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

                SqlCommand com = new SqlCommand("GetRokadTrialBalance", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
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
                    sumDr = sumDr + ((row["DR(Namme)"] != DBNull.Value) ? (Convert.ToDouble(row["DR(Namme)"])) : 0);
                    sumCr = sumCr + ((row["CR(Jamma)"] != DBNull.Value) ? (Convert.ToDouble(row["CR(Jamma)"])) : 0);
                }
                totalDRTrialBal.Text = sumDr.ToString();
                totalCRTrialBal.Text = sumCr.ToString();
                lblRokadDiff.Content = string.Format("पोते बाकी नामे(ClosingBal): {0}", (Math.Round(sumCr - sumDr, 0)).ToString("C"));

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

        private void TabRokad_Selected(object sender, RoutedEventArgs e)
        {
            //goldInInvent.Clear();
            //// goldOutInvent.Clear();
            //oldGoldInInvent.Clear();
            ////oldGoldOutInvent.Clear();
            //silverInInvent.Clear();
            ////  silverOutInvent.Clear();

            string sdt = startDateRokad.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateRokad.SelectedDate);
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

            string enddt = toDateRokad.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateRokad.SelectedDate);
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

                SqlCommand com = new SqlCommand("GetRokadSummary", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustNameRokad.autoTextBoxRokad.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                //com.Parameters.Add(new SqlParameter("@ItemName", autocompleteItemName.autoTextBox1.Text.Trim()));

                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        

                System.Data.DataTable dt1 = new System.Data.DataTable("Summary");
                sda.Fill(dt1);
                RokadGrid.ItemsSource = dt1.DefaultView;
                RokadGrid.AutoGenerateColumns = true;
                RokadGrid.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    sumDr = sumDr + ((row["DR(Namme)"] != DBNull.Value) ? (Convert.ToDouble(row["DR(Namme)"])) : 0);
                    sumCr = sumCr + ((row["CR(Jamma)"] != DBNull.Value) ? (Convert.ToDouble(row["CR(Jamma)"])) : 0);
                }
                totalDRRokad.Text = sumDr.ToString();
                totalCRRokad.Text = sumCr.ToString();
                if (sumDr >= sumCr)
                {
                    DRBalanceRokad.Text = Math.Round(sumDr - sumCr, 2).ToString();
                }

                if (sumCr >= sumDr)
                {
                    CRBalanceRokad.Text = Math.Round(sumCr - sumDr, 2).ToString();
                }

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

        private void Button_Click_Rokad(object sender, RoutedEventArgs e)
        {
            DRBalanceRokad.Text = "0";
            CRBalanceRokad.Text = "0";

            string sdt = startDateRokad.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateRokad.SelectedDate);
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

            string enddt = toDateRokad.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateRokad.SelectedDate);
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

                SqlCommand com = new SqlCommand("GetRokadSummary", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustNameRokad.autoTextBoxRokad.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                //com.Parameters.Add(new SqlParameter("@ItemName", autocompleteItemName.autoTextBox1.Text.Trim()));

                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        

                System.Data.DataTable dt1 = new System.Data.DataTable("Account Balance");
                sda.Fill(dt1);
                RokadGrid.ItemsSource = dt1.DefaultView;
                RokadGrid.AutoGenerateColumns = true;
                RokadGrid.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    sumDr = sumDr + ((row["DR(Namme)"] != DBNull.Value) ? (Convert.ToDouble(row["DR(Namme)"])) : 0);
                    sumCr = sumCr + ((row["CR(Jamma)"] != DBNull.Value) ? (Convert.ToDouble(row["CR(Jamma)"])) : 0);
                }
                totalDRRokad.Text = sumDr.ToString();
                totalCRRokad.Text = sumCr.ToString();

                if (sumDr >= sumCr)
                {
                    DRBalanceRokad.Text = Math.Round(sumDr - sumCr, 2).ToString();
                }

                if (sumCr >= sumDr)
                {
                    CRBalanceRokad.Text = Math.Round(sumCr - sumDr, 2).ToString();
                }
              
            }


        }

        private void autocompltCustNameRokad_LostFocus(object sender, RoutedEventArgs e)
        {
            DRBalanceRokad.Text = "0";
            CRBalanceRokad.Text = "0";

            string sdt = startDateRokad.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(startDateRokad.SelectedDate);
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

            string enddt = toDateRokad.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(toDateRokad.SelectedDate);
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

                SqlCommand com = new SqlCommand("GetRokadSummary", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustNameRokad.autoTextBoxRokad.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                //com.Parameters.Add(new SqlParameter("@ItemName", autocompleteItemName.autoTextBox1.Text.Trim()));

                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        

                System.Data.DataTable dt1 = new System.Data.DataTable("Balance");
                sda.Fill(dt1);
                RokadGrid.ItemsSource = dt1.DefaultView;
                RokadGrid.AutoGenerateColumns = true;
                RokadGrid.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    sumDr = sumDr + ((row["DR(Namme)"] != DBNull.Value) ? (Convert.ToDouble(row["DR(Namme)"])) : 0);
                    sumCr = sumCr + ((row["CR(Jamma)"] != DBNull.Value) ? (Convert.ToDouble(row["CR(Jamma)"])) : 0);
                }
                totalDRRokad.Text = sumDr.ToString();
                totalCRRokad.Text = sumCr.ToString();

                if (sumDr >= sumCr)
                {
                    DRBalanceRokad.Text = Math.Round(sumDr - sumCr, 2).ToString();
                }

                if (sumCr >= sumDr)
                {
                    CRBalanceRokad.Text = Math.Round(sumCr - sumDr, 2).ToString();
                }

               
            }

        }

        private void printRokadAcctLedger_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
