using RTSJewelERP.DebitCreditAccountsListTableAdapters;
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
    /// Interaction logic for Rokad.xaml
    /// </summary>
    public partial class Rokad : Window
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public Rokad()
        {
            InitializeComponent();

            //autocompltCustName.autoTextBox.Clear();
            //NammeTextbox.Clear();
            //NammeTextbox.Clear();
            //autocompltCustName.autoTextBox.Focus();
            CREDIT_Account.Focus();
            BindCreditAccountsComboBox(CREDIT_Account);
            BindDebitAccountsComboBox(DEBIT_Account);

            string BillDateInv = RokadDate.SelectedDate.ToString();

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

            double sumDrOpeningBalr = 0;
            double sumCrOpeningBal = 0;


            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                //SqlCommand com = new SqlCommand("SELECT  DRAcct , DRAmt,TransactionDate,Remarks  FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(CRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
                SqlCommand com = new SqlCommand("select SUM(CAST(CR AS float))   As [CR],SUM(CAST(DR AS float))  As [DR] from [RokadAccountsLedger] where  CompID = '" + CompID + "' and TransactionDate  < '" + InvdateValue + "' ", con);

                SqlDataReader reader = com.ExecuteReader();

                //tmpProduct = new Product();
                //double sumDr = 0;
                //double sumCr = 0;
                while (reader.Read())
                {

                    sumCrOpeningBal = (reader["CR"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    sumDrOpeningBalr = (reader["DR"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                }
                reader.Close();
                txtOpBal.Text = Math.Round((sumCrOpeningBal - sumDrOpeningBalr), 2).ToString();
                //lblOpBal.Content = string.Format("आज पोते बाकी जमा(TodayOpBal): {0}", Math.Round((sumCrOpeningBal - sumDrOpeningBalr), 2).ToString("C"));
                //txtTotalNammeAmount.Text = sumCr.ToString();
                //totalCRTrialBal.Text = sumCr.ToString();

            }



            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                //SqlCommand com = new SqlCommand("SELECT  DRAcct , DRAmt,TransactionDate,Remarks  FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(CRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
                SqlCommand com = new SqlCommand("SELECT AcctName, DR,TransactionDate,Remarks FROM RokadAccountsLedger WHERE  CompID = '" + CompID + "' and DR > 0  and TransactionDate = '" + InvdateValue + "'", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        
                System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
                sda.Fill(dt1);
                NammeGrid.ItemsSource = dt1.DefaultView;
                NammeGrid.AutoGenerateColumns = true;
                NammeGrid.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    sumDr = sumDr + ((row["DR"] != DBNull.Value) ? (Convert.ToDouble(row["DR"])) : 0);
                    //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                }
                txtTotalNammeAmount.Text = sumDr.ToString();
                //totalCRTrialBal.Text = sumCr.ToString();

            }


            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                //SqlCommand com = new SqlCommand("SELECT CRAcct, CRAmt,TransactionDate,Remarks FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(DRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
                SqlCommand com = new SqlCommand("SELECT AcctName, CR,TransactionDate,Remarks FROM RokadAccountsLedger WHERE  CompID = '" + CompID + "' and  CR > 0  and TransactionDate = '" + InvdateValue + "'", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        
                System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
                sda.Fill(dt1);
                JammaGrid.ItemsSource = dt1.DefaultView;
                JammaGrid.AutoGenerateColumns = true;
                JammaGrid.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    //sumDr = sumDr + ((row["CRAmt"] != DBNull.Value) ? (Convert.ToDouble(row["CRAmt"])) : 0);
                    sumCr = sumCr + ((row["CR"] != DBNull.Value) ? (Convert.ToDouble(row["CR"])) : 0);
                    //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                }
                txtTotalJammaAmount.Text = (sumCrOpeningBal-  sumDrOpeningBalr + sumCr).ToString();

                //totalCRTrialBal.Text = sumCr.ToString();

            }
            lblRokadDiff.Content = string.Format("आज पोते बाकी नामे(TodayClosingBal): {0}", (Math.Round((Convert.ToDouble(txtTotalJammaAmount.Text) - Convert.ToDouble(txtTotalNammeAmount.Text)), 0)).ToString("C"));

        }

        public void BindCreditAccountsComboBox(ComboBox creditacct)
        {
            var custAdpt = new AccountsListTableAdapter();
            var custInfoVal = custAdpt.GetData();
            //var LinqRes = (from UserRec in custInfoVal
            //               where (UserRec.AcctName.Trim().ToUpper().Contains("NAMME")) && UserRec.CompID.Equals(CompID)
            //               orderby UserRec.AcctName ascending
            //               select (UserRec.AcctName.Trim())).Distinct();
            //CREDIT_Account.ItemsSource = LinqRes;


            CREDIT_Account.ItemsSource = custInfoVal.Where(c => (!c.AcctName.Trim().ToUpper().Contains("NAMME"))).OrderBy(d => d.AcctName)
                     .Select(x => x.AcctName.Trim()).Distinct().ToList();
           
        }

        public void BindDebitAccountsComboBox(ComboBox debitacct)
        {
            var custAdpt = new AccountsListTableAdapter();
            var custInfoVal = custAdpt.GetData();
            //var LinqRes = (from UserRec in custInfoVal
            //               where (UserRec.AcctName.Trim().ToUpper().Contains("JAMMA")) && UserRec.CompID.Equals(CompID)
            //               orderby UserRec.AcctName ascending
            //               select (UserRec.AcctName.Trim())).Distinct();

            //DEBIT_Account.ItemsSource = LinqRes;

            DEBIT_Account.ItemsSource = custInfoVal.Where(c => (!c.AcctName.Trim().ToUpper().Contains("JAMMA"))).OrderBy(d => d.AcctName)
                                      .Select(x => x.AcctName.Trim()).Distinct().ToList();
          
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

            //if ((e.Key == Key.Left)  && (e.Key == Key.Tab))
            //((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) &&  Key.Tab))
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

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            AddSundryDebtor asd = new AddSundryDebtor();
            asd.ShowDialog();
            BindCreditAccountsComboBox(CREDIT_Account);
            BindDebitAccountsComboBox(DEBIT_Account);

            CREDIT_Account.Focus();
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

            //if ((e.Key == Key.Left)  && (e.Key == Key.Tab))
            //((Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)) &&  Key.Tab))
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
            string BillDateInv = RokadDate.SelectedDate.ToString();

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

            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            myConnSalesInvEntryStr.Open();
            //string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPc where ItemName ='" + txtItemNam.Text.Trim() + "' and CompID = '" + CompID + "'";
            //string CountStockItemsEntryStr = "SELECT COUNT(*) From RokadMilan where CRAcct ='" + CREDIT_Account.Text.Trim() + "'   and TransactionDate='" + InvdateValue + "'";

            string CountStockItemsEntryStr = "SELECT COUNT(*) From RokadAccountsLedger where CompID = '" + CompID + "' and LTRIM(RTRIM(AcctName)) ='" + CREDIT_Account.Text.Trim() + "'  and CR > 0  and  TransactionDate='" + InvdateValue + "'";

            SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
            myCommand.Connection = myConnSalesInvEntryStr;

            //int countRec = myCommand.ExecuteNonQuery();
            int countRec = (int)myCommand.ExecuteScalar();
            myCommand.Connection.Close();


            if (countRec != 0 && txtAmount.Text.Trim() != "")
            {

                string queryStrStockUpdate = "";
                //queryStrStockUpdate = "update RokadMilan  set Remarks ='" + Narration.Text.Trim() + "',  TransactionDate='" + InvdateValue + "',CRAcct='" + CREDIT_Account.Text.Trim() + "',CRAmt='" + txtAmount.Text.Trim() + "'where CRAcct ='" + CREDIT_Account.Text.Trim() + "'  and TransactionDate='" + InvdateValue + "'";
                queryStrStockUpdate = "update RokadAccountsLedger  set Remarks ='" + Narration.Text.Trim() + "',  TransactionDate='" + InvdateValue + "',AcctName='" + CREDIT_Account.Text.Trim() + "',CR='" + txtAmount.Text.Trim() + "'where CompID = '" + CompID + "' and AcctName ='" + CREDIT_Account.Text.Trim() + "' and  CR > 0  and TransactionDate='" + InvdateValue + "'";


                SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                myCommandStkUpdate.Connection.Open();
                myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                if (CREDIT_Account.Text.Trim() != "")
                {
                    // myCommandStk.Connection.Open();
                    int Num = myCommandStkUpdate.ExecuteNonQuery();
                    if (Num != 0)
                    {
                        txtAmount.Clear();
                        Narration.Clear();
                        CREDIT_Account.Focus();
                        MessageBox.Show("Record Successfully Updated....", "Update Record");
                    }
                    else
                    {
                        MessageBox.Show("Rokad is not Updated....", "Update Record Error");
                    }
                    // myCommandStk.Connection.Close();
                }
                else
                {
                    MessageBox.Show("Rokad can not be updated....", "Update Record Error");
                }
                myCommandStkUpdate.Connection.Close();
            }
            else
            {

                if (CREDIT_Account.Text.Trim() != "" && txtAmount.Text.Trim() != "")
                {
                    string querySalesInvEntry = "";
                    //querySalesInvEntry = "insert into RokadMilan(DRAcct, CRAcct, CRAmt,TransactionDate,Remarks) Values ('Cash', '" + CREDIT_Account.Text.Trim() + "','" + txtAmount.Text.Trim() + "','" + InvdateValue + "','" + Narration.Text.Trim() + "')";

                    querySalesInvEntry = "insert into RokadAccountsLedger(AcctName, CR,TransactionDate,Remarks,CompID) Values ('" + CREDIT_Account.Text.Trim() + "','" + txtAmount.Text.Trim() + "','" + InvdateValue + "','" + Narration.Text.Trim() + "','" + CompID + "')"; 

                    SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                    myCommandInvEntry.Connection.Open();
                    int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                    if (NumPInv != 0)
                    {
                        MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                        //CREDIT_Account.Clear();
                        txtAmount.Clear();
                        Narration.Clear();
                        CREDIT_Account.Focus();

                    }
                    else
                    {
                        MessageBox.Show("Stock is not Inserted....", "Insert Record Error");
                    }
                    myCommandInvEntry.Connection.Close();
                }

                // myConnStock.Close();}

            }

            //Grid Refresh

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                //SqlCommand com = new SqlCommand("SELECT CRAcct, CRAmt,TransactionDate,Remarks FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(DRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
                SqlCommand com = new SqlCommand("SELECT AcctName, CR,TransactionDate,Remarks FROM RokadAccountsLedger WHERE   CompID = '" + CompID + "' and  CR > 0  and TransactionDate = '" + InvdateValue + "'", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        
                System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
                sda.Fill(dt1);
                JammaGrid.ItemsSource = dt1.DefaultView;
                JammaGrid.AutoGenerateColumns = true;
                JammaGrid.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    //sumDr = sumDr + ((row["CRAmt"] != DBNull.Value) ? (Convert.ToDouble(row["CRAmt"])) : 0);
                    sumDr = sumDr + ((row["CR"] != DBNull.Value) ? (Convert.ToDouble(row["CR"])) : 0);
                    //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                }
                txtTotalJammaAmount.Text = (Math.Round((Convert.ToDouble(txtOpBal.Text))) +  sumDr).ToString();
                //totalCRTrialBal.Text = sumCr.ToString();

            }
            lblRokadDiff.Content = string.Format("आज पोते बाकी नामे(TodayClosingBal): {0}", (Math.Round((Convert.ToDouble(txtTotalJammaAmount.Text) - Convert.ToDouble(txtTotalNammeAmount.Text)), 0)).ToString("C"));

        }

        private void Button_ClickNamme(object sender, RoutedEventArgs e)
        {
            string BillDateInv = RokadDate.SelectedDate.ToString();

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


            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            myConnSalesInvEntryStr.Open();
            //string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPc where ItemName ='" + txtItemNam.Text.Trim() + "' and CompID = '" + CompID + "'";
            //string CountStockItemsEntryStr = "SELECT COUNT(*) From RokadMilan where DRAcct ='" + DEBIT_Account.Text.Trim() + "'  and TransactionDate='" + InvdateValue + "'";
            string CountStockItemsEntryStr = "SELECT COUNT(*) From RokadAccountsLedger where  CompID = '" + CompID + "' and  LTRIM(RTRIM(AcctName)) ='" + DEBIT_Account.Text.Trim() + "'  and DR > 0  and  TransactionDate='" + InvdateValue + "'";

            SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
            myCommand.Connection = myConnSalesInvEntryStr;

            //int countRec = myCommand.ExecuteNonQuery();
            int countRec = (int)myCommand.ExecuteScalar();
            myCommand.Connection.Close();


            if (countRec != 0 && txtAmountNamme.Text.Trim() != "")
            {

                string queryStrStockUpdate = "";
                //queryStrStockUpdate = "update RokadMilan  set Remarks ='" + NarrationNamme.Text.Trim() + "', TransactionDate='" + InvdateValue + "',DRAcct='" + DEBIT_Account.Text.Trim() + "',DRAmt='" + txtAmountNamme.Text.Trim() + "'where DRAcct ='" + DEBIT_Account.Text.Trim() + "'  and TransactionDate='" + InvdateValue + "'";
                queryStrStockUpdate = "update RokadAccountsLedger  set Remarks ='" + NarrationNamme.Text.Trim() + "',  TransactionDate='" + InvdateValue + "',AcctName='" + DEBIT_Account.Text.Trim() + "',DR='" + txtAmountNamme.Text.Trim() + "'where  CompID = '" + CompID + "' and AcctName ='" + DEBIT_Account.Text.Trim() + "' and  DR > 0  and TransactionDate='" + InvdateValue + "'";

                SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                myCommandStkUpdate.Connection.Open();
                myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                if (DEBIT_Account.Text.Trim() != "")
                {
                    // myCommandStk.Connection.Open();
                    int Num = myCommandStkUpdate.ExecuteNonQuery();
                    if (Num != 0)
                    {
                        //MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                        //NammeTextbox.Clear();
                        txtAmountNamme.Clear();
                        NarrationNamme.Clear();
                        DEBIT_Account.Focus();

                        MessageBox.Show("Record Successfully Updated....", "Update Record");
                    }
                    else
                    {
                        MessageBox.Show("Rokad is not Updated....", "Update Record Error");
                    }
                    // myCommandStk.Connection.Close();
                }
                else
                {
                    MessageBox.Show("Rokad can not be updated....", "Update Record Error");
                }
                myCommandStkUpdate.Connection.Close();
            }
            else
            {
                if (DEBIT_Account.Text.Trim() != "" && txtAmountNamme.Text.Trim() != "")
                {
                    string querySalesInvEntry = "";
                    //querySalesInvEntry = "insert into RokadMilan(CRAcct, DRAcct, DRAmt,TransactionDate,Remarks) Values ( 'Cash', '" + DEBIT_Account.Text.Trim() + "','" + txtAmountNamme.Text.Trim() + "','" + InvdateValue + "','" + NarrationNamme.Text.Trim() + "')";
                    querySalesInvEntry = "insert into RokadAccountsLedger(AcctName, DR,TransactionDate,Remarks, CompID) Values ('" + DEBIT_Account.Text.Trim() + "','" + txtAmountNamme.Text.Trim() + "','" + InvdateValue + "','" + NarrationNamme.Text.Trim() + "','" + CompID+ "')";
                    SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                    myCommandInvEntry.Connection.Open();
                    int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                    if (NumPInv != 0)
                    {
                        MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                        //NammeTextbox.Clear();
                        txtAmountNamme.Clear();
                        NarrationNamme.Clear();
                        DEBIT_Account.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Stock is not Inserted....", "Insert Record Error");
                    }
                    myCommandInvEntry.Connection.Close();

                    // myConnStock.Close();
                }

            }



            //Grid Refresh

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                //SqlCommand com = new SqlCommand("SELECT  DRAcct , DRAmt,TransactionDate,Remarks  FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(CRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
                SqlCommand com = new SqlCommand("SELECT AcctName, DR,TransactionDate,Remarks FROM RokadAccountsLedger WHERE   CompID = '" + CompID + "' and  DR > 0  and TransactionDate = '" + InvdateValue + "'", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        
                System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
                sda.Fill(dt1);
                NammeGrid.ItemsSource = dt1.DefaultView;
                NammeGrid.AutoGenerateColumns = true;
                NammeGrid.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    sumDr = sumDr + ((row["DR"] != DBNull.Value) ? (Convert.ToDouble(row["DR"])) : 0);
                    //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                }
                txtTotalNammeAmount.Text = sumDr.ToString();
                //totalCRTrialBal.Text = sumCr.ToString();

            }

            lblRokadDiff.Content = string.Format("आज पोते बाकी नामे(TodayClosingBal): {0}", (Math.Round((Convert.ToDouble(txtTotalJammaAmount.Text) - Convert.ToDouble(txtTotalNammeAmount.Text)), 0)).ToString("C"));
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            AddSundryDebtor asd = new AddSundryDebtor();
            asd.ShowDialog();
            BindCreditAccountsComboBox(CREDIT_Account);
            BindDebitAccountsComboBox(DEBIT_Account);
            DEBIT_Account.Focus();
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string BillDateInv = RokadDate.SelectedDate.ToString();

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
                MessageBoxResult genResult = MessageBox.Show("Are you sure you want to  DELETE record ?", "Sheet Account Ledger", MessageBoxButton.YesNo);
                if (genResult == MessageBoxResult.Yes)
                {
                    DataRowView row1 = (DataRowView)JammaGrid.SelectedItems[0];
                    string acctname = row1["AcctName"].ToString();
                    string transdate = row1["TransactionDate"].ToString();

                    SqlConnection connDelete = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSERPBasic;Trusted_Connection=Yes;");
                    connDelete.Open();
                    string sqlDelete = "";
                    SqlCommand cmdDelete;
                    //PurchaseInvoices
                    if (1 == 1)
                    {
                        //sqlDelete = "delete from RokadMilan  where LTRIM(RTRIM(CRAcct)) ='" + acctname.Trim() + "'  and TransactionDate='" + InvdateValue + "'";
                        sqlDelete = "delete from RokadAccountsLedger  where   CompID = '" + CompID + "' and  LTRIM(RTRIM(AcctName)) ='" + acctname.Trim() + "' and CR > 0 and TransactionDate='" + InvdateValue + "'";
                        cmdDelete = new SqlCommand(sqlDelete, connDelete);
                        int NumDelete = cmdDelete.ExecuteNonQuery();
                        if (NumDelete != 0)
                        {

                            //using (SqlConnection con = new SqlConnection())
                            //{
                            //    con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                            //    con.Open();

                            //    SqlCommand com = new SqlCommand("SELECT  CRAcct, CRAmt,TransactionDate,Remarks  FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(DRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
                            //    SqlDataAdapter sda = new SqlDataAdapter(com);
                            //    //SqlDataReader reader = com.ExecuteReader();        
                            //    System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
                            //    sda.Fill(dt1);
                            //    JammaGrid.ItemsSource = dt1.DefaultView;
                            //    JammaGrid.AutoGenerateColumns = true;
                            //    JammaGrid.CanUserAddRows = false;

                            //    double sumDr = 0;
                            //    double sumCr = 0;
                            //    foreach (DataRow row in dt1.Rows)
                            //    {
                            //        //sumDr +=  Convert.ToDouble(row["DR"]);
                            //        sumCr = sumCr + ((row["CRAmt"] != DBNull.Value) ? (Convert.ToDouble(row["CRAmt"])) : 0);
                            //        //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                            //    }
                            //    txtTotalJammaAmount.Text = sumCr.ToString();
                            //    //totalCRTrialBal.Text = sumCr.ToString();

                            //}

                            using (SqlConnection con = new SqlConnection())
                            {
                                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                                con.Open();

                                //SqlCommand com = new SqlCommand("SELECT CRAcct, CRAmt,TransactionDate,Remarks FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(DRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
                                SqlCommand com = new SqlCommand("SELECT AcctName, CR,TransactionDate,Remarks FROM RokadAccountsLedger WHERE   CompID = '" + CompID + "' and  CR > 0  and TransactionDate = '" + InvdateValue + "'", con);
                                SqlDataAdapter sda = new SqlDataAdapter(com);
                                //SqlDataReader reader = com.ExecuteReader();        
                                System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
                                sda.Fill(dt1);
                                JammaGrid.ItemsSource = dt1.DefaultView;
                                JammaGrid.AutoGenerateColumns = true;
                                JammaGrid.CanUserAddRows = false;

                                double sumDr = 0;
                                double sumCr = 0;
                                foreach (DataRow row in dt1.Rows)
                                {
                                    //sumDr +=  Convert.ToDouble(row["DR"]);
                                    //sumDr = sumDr + ((row["CRAmt"] != DBNull.Value) ? (Convert.ToDouble(row["CRAmt"])) : 0);
                                    sumDr = sumDr + ((row["CR"] != DBNull.Value) ? (Convert.ToDouble(row["CR"])) : 0);
                                    //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                                }
                                //txtTotalJammaAmount.Text = sumDr.ToString();
                                txtTotalJammaAmount.Text = (Math.Round((Convert.ToDouble(txtOpBal.Text))) + sumDr).ToString();
                                //totalCRTrialBal.Text = sumCr.ToString();

                            }

                            lblRokadDiff.Content = string.Format("आज पोते बाकी नामे(TodayClosingBal): {0}", (Math.Round((Convert.ToDouble(txtTotalJammaAmount.Text) - Convert.ToDouble(txtTotalNammeAmount.Text)), 0)).ToString("C"));



                        }
                    }



                    cmdDelete.Connection.Close();
                    JammaGrid.Items.Refresh();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Select Record");
            }

        }

        private void Delete_ClickNamme(object sender, RoutedEventArgs e)
        {
            try
            {
                string BillDateInv = RokadDate.SelectedDate.ToString();

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
                MessageBoxResult genResult = MessageBox.Show("Are you sure you want to  DELETE record ?", "Sheet Account Ledger", MessageBoxButton.YesNo);
                if (genResult == MessageBoxResult.Yes)
                {
                    DataRowView row1 = (DataRowView)NammeGrid.SelectedItems[0];
                    string acctname = row1["AcctName"].ToString();
                    string transdate = row1["TransactionDate"].ToString();

                    SqlConnection connDelete = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSERPBasic;Trusted_Connection=Yes;");
                    connDelete.Open();
                    string sqlDelete = "";
                    SqlCommand cmdDelete;
                    //PurchaseInvoices
                    if (1 == 1)
                    {
                        //sqlDelete = "delete from RokadMilan  where LTRIM(RTRIM(DRAcct)) ='" + acctname.Trim() + "'  and TransactionDate='" + InvdateValue + "'";
                        sqlDelete = "delete from RokadAccountsLedger  where   CompID = '" + CompID + "' and  LTRIM(RTRIM(AcctName)) ='" + acctname.Trim() + "' and DR > 0 and TransactionDate='" + InvdateValue + "'";
                        cmdDelete = new SqlCommand(sqlDelete, connDelete);
                        int NumDelete = cmdDelete.ExecuteNonQuery();
                        if (NumDelete != 0)
                        {

                            //using (SqlConnection con = new SqlConnection())
                            //{
                            //    con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                            //    con.Open();

                            //    SqlCommand com = new SqlCommand("SELECT  DRAcct, DRAmt,TransactionDate,Remarks  FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(CRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
                            //    SqlDataAdapter sda = new SqlDataAdapter(com);
                            //    //SqlDataReader reader = com.ExecuteReader();        
                            //    System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
                            //    sda.Fill(dt1);
                            //    NammeGrid.ItemsSource = dt1.DefaultView;
                            //    NammeGrid.AutoGenerateColumns = true;
                            //    NammeGrid.CanUserAddRows = false;

                            //    double sumDr = 0;
                            //    double sumCr = 0;
                            //    foreach (DataRow row in dt1.Rows)
                            //    {
                            //        //sumDr +=  Convert.ToDouble(row["DR"]);
                            //        sumDr = sumDr + ((row["DRAmt"] != DBNull.Value) ? (Convert.ToDouble(row["DRAmt"])) : 0);
                            //        //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                            //    }
                            //    txtTotalNammeAmount.Text = sumDr.ToString();
                            //    //totalCRTrialBal.Text = sumCr.ToString();

                            //}

                            using (SqlConnection con = new SqlConnection())
                            {
                                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                                con.Open();

                                //SqlCommand com = new SqlCommand("SELECT  DRAcct , DRAmt,TransactionDate,Remarks  FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(CRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
                                SqlCommand com = new SqlCommand("SELECT AcctName, DR,TransactionDate,Remarks FROM RokadAccountsLedger WHERE   CompID = '" + CompID + "' and  DR > 0  and TransactionDate = '" + InvdateValue + "'", con);
                                SqlDataAdapter sda = new SqlDataAdapter(com);
                                //SqlDataReader reader = com.ExecuteReader();        
                                System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
                                sda.Fill(dt1);
                                NammeGrid.ItemsSource = dt1.DefaultView;
                                NammeGrid.AutoGenerateColumns = true;
                                NammeGrid.CanUserAddRows = false;

                                double sumDr = 0;
                                double sumCr = 0;
                                foreach (DataRow row in dt1.Rows)
                                {
                                    //sumDr +=  Convert.ToDouble(row["DR"]);
                                    sumDr = sumDr + ((row["DR"] != DBNull.Value) ? (Convert.ToDouble(row["DR"])) : 0);
                                    //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                                }
                                txtTotalNammeAmount.Text = sumDr.ToString();
                                //totalCRTrialBal.Text = sumCr.ToString();

                            }


                            lblRokadDiff.Content = string.Format("आज पोते बाकी नामे(TodayClosingBal): {0}", (Math.Round((Convert.ToDouble(txtTotalJammaAmount.Text) - Convert.ToDouble(txtTotalNammeAmount.Text)), 0)).ToString("C"));



                        }
                    }



                    cmdDelete.Connection.Close();
                    NammeGrid.Items.Refresh();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Select Record");
            }

        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string BillDateInv = RokadDate.SelectedDate.ToString();

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


            double sumDrOpeningBalr = 0;
            double sumCrOpeningBal = 0;


            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                //SqlCommand com = new SqlCommand("SELECT  DRAcct , DRAmt,TransactionDate,Remarks  FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(CRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
                SqlCommand com = new SqlCommand("select SUM(CAST(CR AS float))   As [CR],SUM(CAST(DR AS float))  As [DR] from [RokadAccountsLedger] where    CompID = '" + CompID + "' and  TransactionDate  < '" + InvdateValue + "' ", con);

                SqlDataReader reader = com.ExecuteReader();

                //tmpProduct = new Product();
                //double sumDr = 0;
                //double sumCr = 0;
                while (reader.Read())
                {

                    sumCrOpeningBal = (reader["CR"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    sumDrOpeningBalr = (reader["DR"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                }
                reader.Close();
                txtOpBal.Text = Math.Round((sumCrOpeningBal - sumDrOpeningBalr), 2).ToString();
                //lblOpBal.Content = string.Format("आज पोते बाकी जमा(TodayOpBal): {0}", Math.Round((sumCrOpeningBal - sumDrOpeningBalr), 2).ToString("C"));
                //txtTotalNammeAmount.Text = sumCr.ToString();
                //totalCRTrialBal.Text = sumCr.ToString();

            }


            //using (SqlConnection con = new SqlConnection())
            //{
            //    con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
            //    con.Open();

            //    SqlCommand com = new SqlCommand("SELECT DRAcct,DRAmt,TransactionDate,Remarks FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(CRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
            //    SqlDataAdapter sda = new SqlDataAdapter(com);
            //    //SqlDataReader reader = com.ExecuteReader();        
            //    System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
            //    sda.Fill(dt1);
            //    NammeGrid.ItemsSource = dt1.DefaultView;
            //    NammeGrid.AutoGenerateColumns = true;
            //    NammeGrid.CanUserAddRows = false;

            //    double sumDr = 0;
            //    double sumCr = 0;
            //    foreach (DataRow row in dt1.Rows)
            //    {
            //        //sumDr +=  Convert.ToDouble(row["DR"]);
            //        sumDr = sumDr + ((row["DRAmt"] != DBNull.Value) ? (Convert.ToDouble(row["DRAmt"])) : 0);
            //        //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
            //    }
            //    txtTotalNammeAmount.Text = sumDr.ToString();
            //    //totalCRTrialBal.Text = sumCr.ToString();

            //}

            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                //SqlCommand com = new SqlCommand("SELECT  DRAcct , DRAmt,TransactionDate,Remarks  FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(CRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
                SqlCommand com = new SqlCommand("SELECT AcctName, DR,TransactionDate,Remarks FROM RokadAccountsLedger WHERE   CompID = '" + CompID + "' and  DR > 0  and TransactionDate = '" + InvdateValue + "'", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        
                System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
                sda.Fill(dt1);
                NammeGrid.ItemsSource = dt1.DefaultView;
                NammeGrid.AutoGenerateColumns = true;
                NammeGrid.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    sumDr = sumDr + ((row["DR"] != DBNull.Value) ? (Convert.ToDouble(row["DR"])) : 0);
                    //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                }
                txtTotalNammeAmount.Text = sumDr.ToString();
                //totalCRTrialBal.Text = sumCr.ToString();

            }



            //using (SqlConnection con = new SqlConnection())
            //{
            //    con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
            //    con.Open();

            //    SqlCommand com = new SqlCommand("SELECT CRAcct,CRAmt,TransactionDate,Remarks FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(DRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
            //    SqlDataAdapter sda = new SqlDataAdapter(com);
            //    //SqlDataReader reader = com.ExecuteReader();        
            //    System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
            //    sda.Fill(dt1);
            //    JammaGrid.ItemsSource = dt1.DefaultView;
            //    JammaGrid.AutoGenerateColumns = true;
            //    JammaGrid.CanUserAddRows = false;

            //    double sumDr = 0;
            //    double sumCr = 0;
            //    foreach (DataRow row in dt1.Rows)
            //    {
            //        //sumDr +=  Convert.ToDouble(row["DR"]);
            //        sumCr = sumCr + ((row["CRAmt"] != DBNull.Value) ? (Convert.ToDouble(row["CRAmt"])) : 0);
            //        //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
            //    }
            //    txtTotalJammaAmount.Text = sumCr.ToString();
            //    //totalCRTrialBal.Text = sumCr.ToString();

            //}


            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                //SqlCommand com = new SqlCommand("SELECT CRAcct, CRAmt,TransactionDate,Remarks FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(DRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
                SqlCommand com = new SqlCommand("SELECT AcctName, CR,TransactionDate,Remarks FROM RokadAccountsLedger WHERE   CompID = '" + CompID + "' and  CR > 0  and TransactionDate = '" + InvdateValue + "'", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        
                System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
                sda.Fill(dt1);
                JammaGrid.ItemsSource = dt1.DefaultView;
                JammaGrid.AutoGenerateColumns = true;
                JammaGrid.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    //sumDr = sumDr + ((row["CRAmt"] != DBNull.Value) ? (Convert.ToDouble(row["CRAmt"])) : 0);
                    sumDr = sumDr + ((row["CR"] != DBNull.Value) ? (Convert.ToDouble(row["CR"])) : 0);
                    //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                }
                //txtTotalJammaAmount.Text = sumDr.ToString();
                txtTotalJammaAmount.Text = (Math.Round((Convert.ToDouble(txtOpBal.Text))) + sumDr).ToString();
                //totalCRTrialBal.Text = sumCr.ToString();

            }


            //lblRokadDiff.Content = (Convert.ToDouble(txtTotalJammaAmount.Text) - Convert.ToDouble(txtTotalNammeAmount.Text)).ToString();
            lblRokadDiff.Content = string.Format("आज पोते बाकी नामे(TodayClosingBal): {0}", (Math.Round((Convert.ToDouble(txtTotalJammaAmount.Text) - Convert.ToDouble(txtTotalNammeAmount.Text)), 0)).ToString("C"));

            CREDIT_Account.Focus();
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

        private void rokadPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDlg = new PrintDialog();
            printDlg.PrintQueue = System.Printing.LocalPrintServer.GetDefaultPrintQueue();
            printDlg.PrintTicket = printDlg.PrintQueue.DefaultPrintTicket;
            printDlg.PrintTicket.PageOrientation = PageOrientation.Portrait;

            // Create a FlowDocument dynamically.
            //FlowDocument doc = CreateFlowDocumentJewellery();
            FlowDocument doc = CreateFlowDocumentRokad();
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

        private FlowDocument CreateFlowDocumentRokad()
        {
            //  Get Confirmation that data saved successfull, 

            string sdt = RokadDate.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(RokadDate.SelectedDate);
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

            string sdateIndia = days + "/" + months + "/" + years;

            //string enddt = toDate.SelectedDate.ToString();
            //DateTime edt = Convert.ToDateTime(toDate.SelectedDate);
            //int yeard = edt.Year;
            //string monthd = edt.Month.ToString();
            //if (edt.Month < 10)
            //{
            //    monthd = "0" + monthd;
            //}
            //string dayd = edt.Day.ToString();
            //if (edt.Day < 10)
            //{
            //    dayd = "0" + dayd;
            //}
            //enddt = yeard + "/" + monthd + "/" + dayd;


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

            //Font colorHighlight = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.RED);
            /* style for products table header, assigned via type + class selectors */

            System.Windows.Documents.Table completeTable = new System.Windows.Documents.Table();

            TableRow rowoncompleteTable = new TableRow();
            TableRow rowoncompleteTable1 = new TableRow();
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
            a3 = new Span(new Run("Rokad"));
            a3.FontWeight = FontWeights.Bold;
            a3.Inlines.Add(new LineBreak());//Line break is used for next line.  

            //Span a4 = new Span();
            //a4 = new Span(new Run("Invoice# " + invoiceNumber.Text));
            //a4.FontWeight = FontWeights.Bold;
            //a4.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a4acc = new Span();
            a4acc = new Span(new Run("Rokad"));
            a4acc.FontWeight = FontWeights.Bold;
            a4acc.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span a4date = new Span();
            a4date = new Span(new Run("Date: " + sdateIndia));
            a4date.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a5 = new Span();
            a5 = new Span(new Run("---------------------------------------------------------------------------------------------------------"));
            //a5.Inlines.Add(new LineBreak());//Line break is used for next line.  
            p.FontSize = 12;
            p.Inlines.Add(a3);// Add the span content into paragraph.  
            p.Inlines.Add(s);// Add the span content into paragraph.  

            //p.Inlines.Add(a2);// Add the span content into paragraph. 
            //p.Inlines.Add(a1);// Add the span content into paragraph. 
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
            for (int i = 0; i < JammaGrid.Items.Count; i++)
            {
                //TableColumn tc = new TableColumn();

                t5.Columns.Add(new TableColumn() { Width = GridLength.Auto });

            }

            ThicknessConverter tc1 = new ThicknessConverter();
            //// Create Table Borders
            t5.BorderThickness = (Thickness)tc1.ConvertFromString("0.02in");

            int count1 = JammaGrid.Items.Count;
            var rg1 = new TableRowGroup();

            TableRow rowheadertable1 = new TableRow();



            rowheadertable1.Background = Brushes.Silver;
            rowheadertable1.FontSize = 10;
            rowheadertable1.FontFamily = new FontFamily("Century Gothic");
            rowheadertable1.FontWeight = FontWeights.Bold;

            ThicknessConverter tc222 = new ThicknessConverter();



            TableCell tcell3 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Voucher#")));
            //tcell3.ColumnSpan = 3;
            tcell3.BorderBrush = Brushes.Black;
            tcell3.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell3);

            TableCell tcell4 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Account")));
            tcell4.ColumnSpan = 3;
            tcell4.BorderBrush = Brushes.Black;
            tcell4.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell4);


            TableCell tcell6 = new TableCell(new System.Windows.Documents.Paragraph(new Run("CR(जमा)")));
            //tcell6.ColumnSpan = 3;
            tcell6.BorderBrush = Brushes.Black;
            tcell6.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell6);


            TableCell tcell7 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Remarks")));
            //tcell7.ColumnSpan = 3;
            tcell7.BorderBrush = Brushes.Black;
            tcell7.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell7);



            SqlConnection conpdfj = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conpdfj.Open();

            SqlCommand cmdpdfj = new SqlCommand("SELECT LTRIM(RTRIM( [VoucherNumber])) As Voucher#   ,LTRIM(RTRIM([AcctName])) As Account  ,[CR] ,LTRIM(RTRIM([Remarks])) As Remarks FROM [RokadAccountsLedger] where   CompID = '" + CompID + "' and  TransactionDate = '" + sdt + "' and CR > 0", conpdfj);


            SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);

            //cmdpdfj.Connection = conpdfj;
            //SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);
            DataTable dttablej = new DataTable("Inv");
            sda.Fill(dttablej);

            rg1.Rows.Add(rowheadertable1);

            IEnumerable itemsSource1 = JammaGrid.ItemsSource as IEnumerable;
            if (itemsSource1 != null)
            {
                // foreach (var item in itemsSource)
                for (int k = 0; k < dttablej.Rows.Count; ++k)
                {
                    TableRow rowone = new TableRow();

                    // rowone.Background = Brushes.Silver;
                    rowone.FontSize = 10;
                    rowone.FontWeight = FontWeights.Regular;
                    rowone.FontFamily = new FontFamily("Century Gothic");

                    for (int i = 0; i < dttablej.Columns.Count; ++i)
                    {

                        TableCell firstcolproductcell = new TableCell(new System.Windows.Documents.Paragraph(new Run(dttablej.Rows[k][i].ToString())));
                        if (i == 1)
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



            System.Windows.Documents.Table t51 = new System.Windows.Documents.Table();

            t51.Padding = new Thickness(0);
            for (int i = 0; i < NammeGrid.Items.Count; i++)
            {
                //TableColumn tc = new TableColumn();

                t51.Columns.Add(new TableColumn() { Width = GridLength.Auto });

            }

            ThicknessConverter tc11 = new ThicknessConverter();
            //// Create Table Borders
            t51.BorderThickness = (Thickness)tc11.ConvertFromString("0.02in");

            int count11 = NammeGrid.Items.Count;
            var rg11 = new TableRowGroup();

            TableRow rowheadertable11 = new TableRow();



            rowheadertable11.Background = Brushes.Silver;
            rowheadertable11.FontSize = 10;
            rowheadertable11.FontFamily = new FontFamily("Century Gothic");
            rowheadertable11.FontWeight = FontWeights.Bold;

            ThicknessConverter tc2221 = new ThicknessConverter();



            TableCell tcell31 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Voucher#")));
            //tcell3.ColumnSpan = 3;
            tcell31.BorderBrush = Brushes.Black;
            tcell31.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable11.Cells.Add(tcell31);

            TableCell tcell41 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Account")));
            tcell41.ColumnSpan = 3;
            tcell41.BorderBrush = Brushes.Black;
            tcell41.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable11.Cells.Add(tcell41);


            TableCell tcell61 = new TableCell(new System.Windows.Documents.Paragraph(new Run("DR(नामे)")));
            //tcell6.ColumnSpan = 3;
            tcell61.BorderBrush = Brushes.Black;
            tcell61.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable11.Cells.Add(tcell61);


            TableCell tcell71 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Remarks")));
            //tcell7.ColumnSpan = 3;
            tcell71.BorderBrush = Brushes.Black;
            tcell71.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable11.Cells.Add(tcell71);



            SqlConnection conpdfj1 = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conpdfj1.Open();

            SqlCommand cmdpdfj1 = new SqlCommand("SELECT LTRIM(RTRIM( [VoucherNumber])) As Voucher#   ,LTRIM(RTRIM([AcctName])) As Account  ,[DR] ,LTRIM(RTRIM([Remarks])) As Remarks FROM [RokadAccountsLedger] where   CompID = '" + CompID + "' and  TransactionDate = '" + sdt + "' and DR > 0", conpdfj1);


            SqlDataAdapter sda1 = new SqlDataAdapter(cmdpdfj1);

            //cmdpdfj.Connection = conpdfj;
            //SqlDataAdapter sda = new SqlDataAdapter(cmdpdfj);
            DataTable dttablej1 = new DataTable("Inv1");
            sda1.Fill(dttablej1);

            rg11.Rows.Add(rowheadertable11);

            IEnumerable itemsSource11 = NammeGrid.ItemsSource as IEnumerable;
            if (itemsSource11 != null)
            {
                // foreach (var item in itemsSource)
                for (int k = 0; k < dttablej1.Rows.Count; ++k)
                {
                    TableRow rowone = new TableRow();

                    // rowone.Background = Brushes.Silver;
                    rowone.FontSize = 10;
                    rowone.FontWeight = FontWeights.Regular;
                    rowone.FontFamily = new FontFamily("Century Gothic");

                    for (int i = 0; i < dttablej1.Columns.Count; ++i)
                    {

                        TableCell firstcolproductcell = new TableCell(new System.Windows.Documents.Paragraph(new Run(dttablej1.Rows[k][i].ToString())));
                        if (i == 1)
                        {
                            firstcolproductcell.ColumnSpan = 3;
                        }
                        firstcolproductcell.BorderBrush = Brushes.Black;
                        firstcolproductcell.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
                        // rowone.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run((k + 1).ToString()))));
                        rowone.Cells.Add(firstcolproductcell);

                    }

                    rg11.Rows.Add(rowone);
                }
            }



            //----------------

            t51.CellSpacing = 0;


            t51.RowGroups.Add(rg11);





            System.Windows.Documents.Paragraph totalValParag = new System.Windows.Documents.Paragraph();


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
            ts11gTotaoBeforeDisc = new Span(new Run("\t Total CR(जमा): " + txtTotalJammaAmount.Text + "     "));
            ts11gTotaoBeforeDisc.Inlines.Add(new LineBreak());//Line break is used for next line.  
            //}

            Span ts11gDiscAmountItemTotal = new Span();

            ts11gDiscAmountItemTotal = new Span(new Run("\t Total DR(नामे):" + "₹ " + txtTotalNammeAmount.Text + "      "));
            ts11gDiscAmountItemTotal.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span tsTotalTaxableAmt = new Span();

            tsTotalTaxableAmt = new Span(new Run("\t  " + lblRokadDiff.Content));
            tsTotalTaxableAmt.Inlines.Add(new LineBreak());//Line break is used for next line.  



            totalVaGrand.FontSize = 14;
            totalVaGrand.FontFamily = new FontFamily("Century Gothic");
            totalVaGrand.Inlines.Add(ts11gTotaoBeforeDisc);// Add the span content into paragraph.  
            totalVaGrand.Inlines.Add(ts11gDiscAmountItemTotal);
            //totalVaGrand.Inlines.Add(tsMakingCharge);
            totalVaGrand.Inlines.Add(tsTotalTaxableAmt);


            //totalVal.Inlines.Add(ali5);// Add the span content into paragraph.  
            totalVaGrand.TextAlignment = TextAlignment.Center;

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

            TableCell tcellfirstTb = new TableCell(new System.Windows.Documents.Paragraph(new Run(" ")));

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

            TableCell txtcellcompleteTable1 = new TableCell(t51);
            txtcellcompleteTable1.BorderBrush = Brushes.Black;
            txtcellcompleteTable1.BorderThickness = (Thickness)tc22234completeTable.ConvertFromString("0.0001in");


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
            rowoncompleteTable1.Cells.Add(txtcellcompleteTable1);
            rowtwocompleteTable.Cells.Add(txtcell2completeTable);
            rowthreecompleteTable.Cells.Add(txtcell3completeTable);



            rowoncompleteTable.FontSize = 11;
            rowoncompleteTable.FontWeight = FontWeights.Regular;
            rowoncompleteTable.FontFamily = new FontFamily("Century Gothic");

            rowoncompleteTable1.FontSize = 11;
            rowoncompleteTable1.FontWeight = FontWeights.Regular;
            rowoncompleteTable1.FontFamily = new FontFamily("Century Gothic");


            rowtwocompleteTable.FontSize = 11;
            rowtwocompleteTable.FontWeight = FontWeights.Regular;
            rowtwocompleteTable.FontFamily = new FontFamily("Century Gothic");

            //rowoneHeadertbl.Cells.Add(new TableCell(p));
            rowgrpcompleteTable.Rows.Add(rowtwocompleteTable);
            rowgrpcompleteTable.Rows.Add(rowoncompleteTable);
            rowgrpcompleteTable.Rows.Add(rowoncompleteTable1);


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
            //doc.Blocks.Add(signpara);


            doc.Name = "FlowDoc";
            //doc.PageWidth = 900;
            doc.PagePadding = new Thickness(20, 10, 10, 20); //v3
            //doc.PagePadding = new Thickness(30, 20, 10, 5); //V2 
            // Create IDocumentPaginatorSource from FlowDocument
            // IDocumentPaginatorSource idpSource = doc;
            // Call PrintDocument method to send document to printer



            return doc;


        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            RokadMonthlyReports sv = new RokadMonthlyReports();
            sv.ShowDialog();


        }
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            RokadEntry sv = new RokadEntry();
            sv.ShowDialog();


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
                btnViewDayRokad.Focus();
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

        private void dateShortcut_Click(object sender, RoutedEventArgs e)
        {
            RokadDate.IsDropDownOpen = true;
        }

        private void groupAccounts_Click(object sender, RoutedEventArgs e)
        {
            string accountnametrialbal = CREDIT_Account.SelectedItem.ToString();
            string startdatev = RokadDate.SelectedDate.ToString();


            RokadAccountsBreakups sv = new RokadAccountsBreakups(accountnametrialbal, startdatev);
            sv.ShowDialog();

        }

        private void groupAccountsNamme_Click(object sender, RoutedEventArgs e)
        {
            string accountnametrialbal = DEBIT_Account.SelectedItem.ToString();
            string startdatev = RokadDate.SelectedDate.ToString();


            RokadAccountsBreakupsNamme sv = new RokadAccountsBreakupsNamme(accountnametrialbal, startdatev);
            sv.ShowDialog();
        }






    }
}
