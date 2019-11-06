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
    public partial class RokadAccountsBreakupsNamme : Window
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;
        string groupactnameGlobal = "";
        public RokadAccountsBreakupsNamme(string groupacctname, string transactionDatVal)
        {
            InitializeComponent();

            acctGroupName.Content = groupacctname;
            RokadDate.Text = transactionDatVal;
            DEBIT_Account.Focus();
            BindCreditAccountsComboBox(DEBIT_Account);
            //BindDebitAccountsComboBox(DEBIT_Account);
            groupactnameGlobal = groupacctname.Trim();
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

                //SqlCommand com = new SqlCommand("SELECT CRAcct, CRAmt,TransactionDate,Remarks FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(DRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
                SqlCommand com = new SqlCommand("SELECT AcctName, DR,TransactionDate,Remarks FROM RokadGroupAccountsLedger WHERE RokadGroupAccountName = '" + groupacctname.Trim() + "' and CompID = '" + CompID + "' and  DR > 0  and TransactionDate = '" + InvdateValue + "'", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        
                System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
                sda.Fill(dt1);
                NammeGridBreakups.ItemsSource = dt1.DefaultView;
                NammeGridBreakups.AutoGenerateColumns = true;
                NammeGridBreakups.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    //sumDr = sumDr + ((row["CRAmt"] != DBNull.Value) ? (Convert.ToDouble(row["CRAmt"])) : 0);
                    sumCr = sumCr + ((row["DR"] != DBNull.Value) ? (Convert.ToDouble(row["DR"])) : 0);
                    //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                }
                txtTotalNammeAmount.Text = (sumCr).ToString();

                //totalCRTrialBal.Text = sumCr.ToString();

            }
            //lblRokadDiff.Content = string.Format("आज पोते बाकी नामे(TodayClosingBal): {0}", (Math.Round((Convert.ToDouble(txtTotalNammeAmount.Text) - Convert.ToDouble(txtTotalNammeAmount.Text)), 0)).ToString("C"));








            using (SqlConnection con = new SqlConnection())
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                con.Open();

                //SqlCommand com = new SqlCommand("SELECT CRAcct, CRAmt,TransactionDate,Remarks FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(DRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
                SqlCommand com = new SqlCommand("SELECT AcctName, DR,TransactionDate,Remarks FROM RokadGroupAccountsLedger WHERE   RokadGroupAccountName = '" + groupacctname.Trim() + "' and CompID = '" + CompID + "' and   DR > 0  and TransactionDate = '" + InvdateValue + "'", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        
                System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
                sda.Fill(dt1);
                NammeGridBreakups.ItemsSource = dt1.DefaultView;
                NammeGridBreakups.AutoGenerateColumns = true;
                NammeGridBreakups.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    //sumDr = sumDr + ((row["CRAmt"] != DBNull.Value) ? (Convert.ToDouble(row["CRAmt"])) : 0);
                    sumDr = sumDr + ((row["DR"] != DBNull.Value) ? (Convert.ToDouble(row["DR"])) : 0);
                    //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                }
                //txtTotalNammeAmount.Text = sumDr.ToString();
                txtTotalNammeAmount.Text = (Math.Round(sumDr)).ToString();
                //totalCRTrialBal.Text = sumCr.ToString();

            }


        }

        public void BindCreditAccountsComboBox(ComboBox creditacct)
        {
            var custAdpt = new AccountsListTableAdapter();
            var custInfoVal = custAdpt.GetData();
            //var LinqRes = (from UserRec in custInfoVal
            //               where (UserRec.AcctName.Trim().ToUpper().Contains("NAMME")) && UserRec.CompID.Equals(CompID)
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
            BindCreditAccountsComboBox(DEBIT_Account);
            //BindDebitAccountsComboBox(DEBIT_Account);

            DEBIT_Account.Focus();
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
            //string CountStockItemsEntryStr = "SELECT COUNT(*) From RokadMilan where CRAcct ='" + DEBIT_Account.Text.Trim() + "'   and TransactionDate='" + InvdateValue + "'";

            string CountStockItemsEntryStr = "SELECT COUNT(*) From RokadGroupAccountsLedger where  RokadGroupAccountName = '" + groupactnameGlobal.Trim() + "' and CompID = '" + CompID + "' and LTRIM(RTRIM(AcctName)) ='" + DEBIT_Account.Text.Trim() + "'  and DR > 0  and  TransactionDate='" + InvdateValue + "'";

            SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
            myCommand.Connection = myConnSalesInvEntryStr;

            //int countRec = myCommand.ExecuteNonQuery();
            int countRec = (int)myCommand.ExecuteScalar();
            myCommand.Connection.Close();


            if (countRec != 0 && txtAmountNamme.Text.Trim() != "")
            {

                string queryStrStockUpdate = "";
                //queryStrStockUpdate = "update RokadMilan  set Remarks ='" + NarrationNamme.Text.Trim() + "',  TransactionDate='" + InvdateValue + "',CRAcct='" + DEBIT_Account.Text.Trim() + "',CRAmt='" + txtAmountNamme.Text.Trim() + "'where CRAcct ='" + DEBIT_Account.Text.Trim() + "'  and TransactionDate='" + InvdateValue + "'";
                queryStrStockUpdate = "update RokadGroupAccountsLedger  set Remarks ='" + NarrationNamme.Text.Trim() + "',  TransactionDate='" + InvdateValue + "',AcctName='" + DEBIT_Account.Text.Trim() + "',DR='" + txtAmountNamme.Text.Trim() + "'where  RokadGroupAccountName = '" + groupactnameGlobal.Trim() + "' and  CompID = '" + CompID + "' and AcctName ='" + DEBIT_Account.Text.Trim() + "' and  DR > 0  and TransactionDate='" + InvdateValue + "'";


                SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                myCommandStkUpdate.Connection.Open();
                myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                if (DEBIT_Account.Text.Trim() != "")
                {
                    // myCommandStk.Connection.Open();
                    int Num = myCommandStkUpdate.ExecuteNonQuery();
                    if (Num != 0)
                    {
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
                    //querySalesInvEntry = "insert into RokadMilan(DRAcct, CRAcct, CRAmt,TransactionDate,Remarks) Values ('Cash', '" + DEBIT_Account.Text.Trim() + "','" + txtAmountNamme.Text.Trim() + "','" + InvdateValue + "','" + NarrationNamme.Text.Trim() + "')";

                    querySalesInvEntry = "insert into RokadGroupAccountsLedger(AcctName, DR,TransactionDate,Remarks,CompID,RokadGroupAccountName) Values ('" + DEBIT_Account.Text.Trim() + "','" + txtAmountNamme.Text.Trim() + "','" + InvdateValue + "','" + NarrationNamme.Text.Trim() + "','" + CompID + "','" + groupactnameGlobal + "')";

                    SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                    myCommandInvEntry.Connection.Open();
                    int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                    if (NumPInv != 0)
                    {
                        MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                        //DEBIT_Account.Clear();
                        txtAmountNamme.Clear();
                        NarrationNamme.Clear();
                        DEBIT_Account.Focus();

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
                SqlCommand com = new SqlCommand("SELECT AcctName, DR,TransactionDate,Remarks FROM RokadGroupAccountsLedger WHERE RokadGroupAccountName = '" + groupactnameGlobal.Trim() + "' and    CompID = '" + CompID + "' and  DR > 0  and TransactionDate = '" + InvdateValue + "'", con);
                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        
                System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
                sda.Fill(dt1);
                NammeGridBreakups.ItemsSource = dt1.DefaultView;
                NammeGridBreakups.AutoGenerateColumns = true;
                NammeGridBreakups.CanUserAddRows = false;

                double sumDr = 0;
                double sumCr = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    //sumDr +=  Convert.ToDouble(row["DR"]);
                    //sumDr = sumDr + ((row["CRAmt"] != DBNull.Value) ? (Convert.ToDouble(row["CRAmt"])) : 0);
                    sumDr = sumDr + ((row["DR"] != DBNull.Value) ? (Convert.ToDouble(row["DR"])) : 0);
                    //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                }
                txtTotalNammeAmount.Text = (Math.Round(sumDr)).ToString();
                //totalCRTrialBal.Text = sumCr.ToString();

            }
            //  lblRokadDiff.Content = string.Format("आज पोते बाकी नामे(TodayClosingBal): {0}", (Math.Round((Convert.ToDouble(txtTotalNammeAmount.Text) - Convert.ToDouble(txtTotalNammeAmount.Text)), 0)).ToString("C"));

        }


        //private void Button_Click_5(object sender, RoutedEventArgs e)
        //{
        //    AddSundryDebtor asd = new AddSundryDebtor();
        //    asd.ShowDialog();
        //    BindCreditAccountsComboBox(DEBIT_Account);
        //    //BindDebitAccountsComboBox(DEBIT_Account);
        //    DEBIT_Account.Focus();
        //}


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
                    DataRowView row1 = (DataRowView)NammeGridBreakups.SelectedItems[0];
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
                        sqlDelete = "delete from RokadGroupAccountsLedger  where RokadGroupAccountName = '" + groupactnameGlobal.Trim() + "' and    CompID = '" + CompID + "' and  LTRIM(RTRIM(AcctName)) ='" + acctname.Trim() + "' and DR > 0 and TransactionDate='" + InvdateValue + "'";
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
                            //    txtTotalNammeAmount.Text = sumCr.ToString();
                            //    //totalCRTrialBal.Text = sumCr.ToString();

                            //}

                            using (SqlConnection con = new SqlConnection())
                            {
                                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                                con.Open();

                                //SqlCommand com = new SqlCommand("SELECT CRAcct, CRAmt,TransactionDate,Remarks FROM ROKADMILAN WHERE UPPER(LTRIM(RTRIM(DRAcct)))='CASH' and TransactionDate = '" + InvdateValue + "'", con);
                                SqlCommand com = new SqlCommand("SELECT AcctName, DR,TransactionDate,Remarks FROM RokadGroupAccountsLedger WHERE  RokadGroupAccountName = '" + groupactnameGlobal.Trim() + "' and   CompID = '" + CompID + "' and  DR > 0  and TransactionDate = '" + InvdateValue + "'", con);
                                SqlDataAdapter sda = new SqlDataAdapter(com);
                                //SqlDataReader reader = com.ExecuteReader();        
                                System.Data.DataTable dt1 = new System.Data.DataTable("Rokad");
                                sda.Fill(dt1);
                                NammeGridBreakups.ItemsSource = dt1.DefaultView;
                                NammeGridBreakups.AutoGenerateColumns = true;
                                NammeGridBreakups.CanUserAddRows = false;

                                double sumDr = 0;
                                double sumCr = 0;
                                foreach (DataRow row in dt1.Rows)
                                {
                                    //sumDr +=  Convert.ToDouble(row["DR"]);
                                    //sumDr = sumDr + ((row["CRAmt"] != DBNull.Value) ? (Convert.ToDouble(row["CRAmt"])) : 0);
                                    sumDr = sumDr + ((row["DR"] != DBNull.Value) ? (Convert.ToDouble(row["DR"])) : 0);
                                    //sumCr = sumCr + ((row["Credit"] != DBNull.Value) ? (Convert.ToDouble(row["Credit"])) : 0);
                                }
                                //txtTotalNammeAmount.Text = sumDr.ToString();
                                txtTotalNammeAmount.Text = (Math.Round(sumDr)).ToString();
                                //totalCRTrialBal.Text = sumCr.ToString();

                            }

                            // lblRokadDiff.Content = string.Format("आज पोते बाकी नामे(TodayClosingBal): {0}", (Math.Round((Convert.ToDouble(txtTotalNammeAmount.Text) - Convert.ToDouble(txtTotalNammeAmount.Text)), 0)).ToString("C"));



                        }
                    }



                    cmdDelete.Connection.Close();
                    NammeGridBreakups.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Select Record");
            }

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
                //btnViewDayRokad.Focus();
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

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            AddSundryDebtor asd = new AddSundryDebtor();
            asd.ShowDialog();
            DEBIT_Account.Focus();
        }

        private void dateShortcut_Click(object sender, RoutedEventArgs e)
        {
            RokadDate.IsDropDownOpen = true;
        }






    }
}
