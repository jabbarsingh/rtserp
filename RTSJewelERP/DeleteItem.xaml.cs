using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
    /// Interaction logic for DeleteItem.xaml
    /// </summary>
    public partial class DeleteItem : Window
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public DeleteItem()
        {
            InitializeComponent();
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
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                MessageBoxResult genResult = MessageBox.Show("Are you sure you want to  DELETE record ?", "Delete Item", MessageBoxButton.YesNo);
                if (genResult == MessageBoxResult.Yes)
                {
                    DataRowView row1 = (DataRowView)StockRegisterGrid.SelectedItems[0];
                    string itemanem = row1["Item"].ToString();
                    string pattern = row1["Pattern"].ToString();

                    SqlConnection connDelete = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSERPBasic;Trusted_Connection=Yes;");
                    connDelete.Open();
                    string sqlDelete = "";
                    SqlCommand cmdDelete;
                    //PurchaseInvoices
                    if (1 == 1)
                    {
                        SqlCommand myCommandDeleteDel = new SqlCommand("SPDeleteItem", connDelete);
                        myCommandDeleteDel.CommandType = CommandType.StoredProcedure;
                        myCommandDeleteDel.Parameters.Add(new SqlParameter("@ItemName", itemanem.Trim()));
                        myCommandDeleteDel.Parameters.Add(new SqlParameter("@Pattern", pattern.Trim()));
                        myCommandDeleteDel.Parameters.Add(new SqlParameter("@CompID", CompID));
                        //myCommandDeleteDel.Connection.Open();
                        int countRecDelDelDel = myCommandDeleteDel.ExecuteNonQuery();
                        if (countRecDelDelDel != 0)
                        {
                            //MessageBox.Show("Record Successfully Deleted....", "Delete Record");
                                //}

                                ////sqlDelete = "delete from RokadMilan  where LTRIM(RTRIM(CRAcct)) ='" + acctname.Trim() + "'  and TransactionDate='" + InvdateValue + "'";
                                //sqlDelete = "delete from StockItemsByPC  where LTRIM(RTRIM(ItemName)) ='" + itemanem.Trim() + "' and DesignNumberPattern='" + pattern.Trim() + "'";
                                //cmdDelete = new SqlCommand(sqlDelete, connDelete);
                                //int NumDelete = cmdDelete.ExecuteNonQuery();
                                //if (NumDelete != 0)
                                //{


                            using (SqlConnection con = new SqlConnection())
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

                                con.ConnectionString = ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString;
                                con.Open();

                                SqlCommand com = new SqlCommand("GetStockItems", con);
                                com.CommandType = CommandType.StoredProcedure;
                                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                                com.Parameters.Add(new SqlParameter("@ItemName", autocompleteItemName.autoTextBox1.Text.Trim()));

                                SqlDataAdapter sda = new SqlDataAdapter(com);
                                //SqlDataReader reader = com.ExecuteReader();        

                                System.Data.DataTable dt1 = new System.Data.DataTable("Items List");
                                sda.Fill(dt1);
                                StockRegisterGrid.ItemsSource = dt1.DefaultView;
                                StockRegisterGrid.AutoGenerateColumns = true;
                                StockRegisterGrid.CanUserAddRows = false;
                            }


                        }
                        myCommandDeleteDel.Connection.Close();
                        StockRegisterGrid.Items.Refresh();
                    }




                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Select Record");
            }

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

                SqlCommand com = new SqlCommand("GetStockItems", con);
                com.CommandType = CommandType.StoredProcedure;
                // com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                com.Parameters.Add(new SqlParameter("@ItemName", autocompleteItemName.autoTextBox1.Text.Trim()));

                SqlDataAdapter sda = new SqlDataAdapter(com);
                //SqlDataReader reader = com.ExecuteReader();        

                System.Data.DataTable dt1 = new System.Data.DataTable( "Items List");
                sda.Fill(dt1);
                StockRegisterGrid.ItemsSource = dt1.DefaultView;
                StockRegisterGrid.AutoGenerateColumns = true;
                StockRegisterGrid.CanUserAddRows = false;
            }
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

                SqlCommand com = new SqlCommand("GetStockItems", con);
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

                //double sumOp = 0;
                //double sumIn = 0;
                //double sumOut = 0;
                ////for (int s = 0; s < AccountLedgerGrid.Items.Count - 1; s++ )
                ////{
                ////    sumDr += (double.Parse((AccountLedgerGrid.Columns[5].GetCellContent(AccountLedgerGrid.Items[s]) as TextBlock).Text));
                ////}
                //foreach (DataRow row in dt1.Rows)
                //{
                //    //sumDr +=  Convert.ToDouble(row["DR"]);
                //    sumOp = sumOp + ((row["OpBal"] != DBNull.Value) ? (Convert.ToDouble(row["OpBal"])) : 0);
                //    sumIn = sumIn + ((row["InQty"] != DBNull.Value) ? (Convert.ToDouble(row["InQty"])) : 0);
                //    sumOut = sumOut + ((row["OutQty"] != DBNull.Value) ? (Convert.ToDouble(row["OutQty"])) : 0);
                //}
                //totalQtyOpBalStockRegister.Text = sumOp.ToString();
                //totalQtyInStockRegister.Text = sumIn.ToString();
                //totalQtyOutStockRegister.Text = sumOut.ToString();
                //totalQtyStockRegister.Text = (sumOp + sumIn - sumOut).ToString();


            }


        }
    }
}
