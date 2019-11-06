using RTSJewelERP.StorageTableAdapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
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
    /// Interaction logic for SaleVoucher.xaml
    /// </summary>
    public partial class CompanyMaster : Page
    {
        public CompanyMaster()
        {
            InitializeComponent();

            textBoxCompanyName.Focus();




            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select number from AutoIncrement where Name = 'SaleInvoice'";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = con;
            SqlDataReader reader = cmd.ExecuteReader();

            //tmpProduct = new Product();

            while (reader.Read())
            {
                //InvoiceNumber = reader.GetInt64(0);


            }
            reader.Close();

            string sqlvoucher = "select number from AutoIncrement where Name = 'SaleVoucher'";
            SqlCommand cmdvoucher = new SqlCommand(sqlvoucher);
            cmdvoucher.Connection = con;
            SqlDataReader readerVoucher = cmdvoucher.ExecuteReader();

            //tmpProduct = new Product();

            while (readerVoucher.Read())
            {
                //voucherNumber = readerVoucher.GetInt64(0);

            }
            readerVoucher.Close();

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
            if (Regex.IsMatch(textBoxCompanyName.Text.Trim(), @"^\d+$") || 1 == 1)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select CompanyName from Company where CompanyName like '%" + textBoxCompanyName.Text + "%'";
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
                textBoxCompanyName.Text = (sender as TextBlock).Text;
                textBoxCompanyName.Focus();
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
            textBoxCompanyName.Focus();
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
            ////shopping cart = a new empty list
            //ShoppingCart = new List<Product>();
            //OldCart = new List<Product>();
            ////Textboxes and labels are set to defaults
            //TxtProdCode.Text = string.Empty;
            //textBoxItemName.Text = string.Empty;
            //txtQty.Text = string.Empty;
            //lbTotal.Content = "Total: ₹ 0.00";
            //lbOldTotal.Content = "Total: ₹ 0.00";
            //lbGrandTotal.Content = "Total: ₹ 0.00";
            ////DataGrid items are set to null
            //CartGrid.ItemsSource = null;
            //CartGrid.Items.Refresh();
            ////Tmp variable is erased using null
            //tmpProduct = null;

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



        private void textBoxCompanyName_TextChanged(object sender, TextChangedEventArgs e)
        {
          
            //If a product code is not empty we search the database
            if (Regex.IsMatch(textBoxCompanyName.Text.Trim(), @"^\d+$") || 1 == 1)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select * from Company where CompanyName = '" + textBoxCompanyName.Text + "'";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                //tmpProduct = new Product();

                while (reader.Read())
                {

                    //var CustID = reader.GetValue(0).ToString();

                    GSTIN.Text = (reader["GSTIN"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                    Address.Text = (reader["Address1"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";
                    Address2.Text = (reader["Address2"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                    City.Text = (reader["City"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";

                    State.Text = (reader["State"] != DBNull.Value) ? (reader.GetString(8).Trim()) : "";

                    Mob.Text = (reader["Mobile1"] != DBNull.Value) ? (reader.GetString(10).Trim()) : "";
                    Phone.Text = (reader["Phone"] != DBNull.Value) ? (reader.GetString(12).Trim()) : "";

                    Email.Text = (reader["Email"] != DBNull.Value) ? (reader.GetString(13).Trim()) : "";
                    //FinYeraStartDate.Text = (reader["FinYearStartDate"] != DBNull.Value) ? (reader.GetString(17).Trim()) : "";
                    //BookStartDate.Text = (reader["BookStartDate"] != DBNull.Value) ? (reader.GetString(18).Trim()) : "";
                    Web.Text = (reader["Website"] != DBNull.Value) ? (reader.GetString(15).Trim()) : "";
                    Branches.Text = (reader["NumberOfBranches"] != DBNull.Value) ? (reader.GetInt32(16)).ToString() : "";
                    LogoUrl.Text = (reader["LogoPath"] != DBNull.Value) ? (reader.GetString(25).Trim()) : "";
                    SubTitle.Text = (reader["SubTitle"] != DBNull.Value) ? (reader.GetString(26).Trim()) : "";

                    BankName.Text = (reader["BankName"] != DBNull.Value) ? (reader.GetString(20).Trim()) : "";
                    BAddress.Text = (reader["BAddress"] != DBNull.Value) ? (reader.GetString(21).Trim()) : "";
                    IFSC.Text = (reader["IFSC"] != DBNull.Value) ? (reader.GetString(22).Trim()) : "";
                    AccNumber.Text = (reader["AccNumber"] != DBNull.Value) ? (reader.GetString(23).Trim()) : "";
                    Holder.Text = (reader["Holder"] != DBNull.Value) ? (reader.GetString(24).Trim()) : "";


                }
                reader.Close();
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


        private void NumberValidationInvoiceTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
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


            string BookStartdate = BookStartDate.SelectedDate.ToString();

            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dtinb = Convert.ToDateTime(BookStartdate);
            //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            int yearsinb = dtinb.Year;
            string monthsinb = dtinb.Month.ToString();
            if (dtinb.Month < 10)
            {
                monthsinb = "0" + monthsinb;
            }
            string daysinb = dtinb.Day.ToString();
            if (dtinb.Day < 10)
            {
                daysinb = "0" + daysinb;
            }

            string BookStartdateVal = yearsinb + "/" + monthsinb + "/" + daysinb;




            //StockItems: CRUD Start
            if (textBoxCompanyName != null)
            {
                //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                myConnSalesInvEntryStr.Open();
                string CountStockItemsEntryStr = "SELECT COUNT(*) From Company where CompanyName ='" + textBoxCompanyName.Text + "'";
                SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
                myCommand.Connection = myConnSalesInvEntryStr;

                //int countRec = myCommand.ExecuteNonQuery();
                int countRec = (int)myCommand.ExecuteScalar();
                myCommand.Connection.Close();


                if (countRec != 0)
                {

                    // string queryStrStockUpdate = "";
                    // queryStrStockUpdate = "update Company  set  CompanyName='" + balanceStk + "',GSTIN='" + balanceStkWt + "',Address1='" + priceText.Text + "'  where ItemName ='" + txtItemNam.Text + "'  ";
                    //SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                    //myCommandStkUpdate.Connection.Open();
                    //myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                    //if (textBoxCompanyName.Text.Trim() != "")
                    //{
                    //    // myCommandStk.Connection.Open();
                    //    int Num = myCommandStkUpdate.ExecuteNonQuery();
                    //    if (Num != 0)
                    //    {
                    //        // MessageBox.Show("Record Successfully Updated....", "Update Record");
                    //    }
                    //    else
                    //    {
                    //        MessageBox.Show("Stock is not Updated....", "Update Record Error");
                    //    }
                    //    // myCommandStk.Connection.Close();
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Stock can not be updated....", "Update Record Error");
                    //}
                    //myCommandStkUpdate.Connection.Close();
                }
                else
                {

                    string querySalesInvEntry = "";
                    querySalesInvEntry = "insert into Company(CompanyName, GSTIN,Address1,Address2,City,State,Mobile1,Phone,Email,Website,NumberOfBranches,FinYearStartDate,BookStartDate,BankName,BAddress,IFSC,AccNumber,Holder,SubTitle,LogoPath) Values ( '" + textBoxCompanyName.Text + "','" + GSTIN.Text + "','" + Address.Text + "','" + Address2.Text + "','" + City.Text + "','" + State.Text + "','" + Mob.Text + "','" + Phone.Text + "','" + Email.Text + "','" + Web.Text + "','" + Branches.Text + "','" + FinYrStartdateVal + "','" + BookStartdateVal + "','" + BankName.Text + "','" + BAddress.Text + "','" + IFSC.Text + "','" + AccNumber.Text + "','" + Holder.Text + "','" + SubTitle.Text + "','" + LogoUrl.Text + "')";
                    SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                    myCommandInvEntry.Connection.Open();
                    int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                    if (NumPInv != 0)
                    {
                        // MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                    }
                    else
                    {
                        MessageBox.Show("Stock is not Inserted....", "Insert Record Error");
                    }
                    myCommandInvEntry.Connection.Close();

                    // myConnStock.Close();

                }


            }

        }

        //private void resultStack_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    txtQty.Focus();
        //}




    }
}
