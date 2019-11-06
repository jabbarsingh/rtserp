using RTSJewelERP.SheetAccountListTableAdapters;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for SheetAddAccount.xaml
    /// </summary>
    public partial class SheetAddAccount : Window
    {
        public SheetAddAccount()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window
            BindComboBox(CustNameDropBox);
            CustName.Focus();

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

            //if (e.Key == Key.PageUp)
            //{
            //    if (Convert.ToInt64(VoucherNumber.Text.Trim()) < voucherNumber)
            //    {
            //        Int64 inpageup = (VoucherNumber.Text.Trim() != "") ? (Convert.ToInt64(VoucherNumber.Text.Trim()) + 1) : 0;
            //        VoucherNumber.Text = inpageup.ToString();
            //        MoveToBill(inpageup.ToString());

            //    }
            //    if (Convert.ToInt64(VoucherNumber.Text.Trim()) == voucherNumber)
            //    {
            //        //autocompltCustName.autoTextBox.Text = "Cash";
            //        autocompltCustName.autoTextBox.Focus();
            //    }
            //    e.Handled = true;
            //}
            //if (e.Key == Key.PageDown)
            //{
            //    if (Convert.ToInt64(VoucherNumber.Text.Trim()) > 1)
            //    {
            //        Int64 inpageup = (VoucherNumber.Text.Trim() != "") ? (Convert.ToInt64(VoucherNumber.Text.Trim()) - 1) : 0;
            //        VoucherNumber.Text = inpageup.ToString();
            //        MoveToBill(inpageup.ToString());
            //        e.Handled = true;
            //    }


            //}

        }
        public void BindComboBox(ComboBox comboBoxName)
        {
            var custAdpt = new AccountsMasterSheetTableAdapter();
            var custInfoVal = custAdpt.GetData();
            var LinqRes = (from UserRec in custInfoVal
                           orderby UserRec.CustomerName ascending
                           select UserRec.CustomerName + ":" + UserRec.GSTIN);
                          //select UserRec.CustomerName).Distinct();
            comboBoxName.ItemsSource = LinqRes;
            // comboBoxName.SelectedValueBinding = new Binding("Col6");
        }
        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
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


            string SelectedValueState = ((ComboBoxItem)State.SelectedItem).Content.ToString();

            //string constr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\RTSERPBasic\Database\InvWpf-Enhanced.accdb;";
            //OleDbConnection myConn = new OleDbConnection(constr);                                 
            //string queryStr ="";
            //OleDbCommand myCommand  ;

            SqlConnection myConn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSERPBasic;Trusted_Connection=Yes;");
            myConn.Open();
            string queryStr = "";
            SqlCommand myCommand = new SqlCommand(queryStr);
            myCommand.Connection = myConn;

            //OleDbCommand myCommand = new OleDbCommand(queryStr, myConn);
            var selectedValueCustomer = (CustNameDropBox.SelectedItem);

            if (selectedValueCustomer == null)
            {
                queryStr = "SELECT COUNT(*) From AccountsMasterSheet where LTRIM(RTRIM(CustomerName)) ='" + CustName.Text.Trim() + "'  OR LTRIM(RTRIM(GSTIN))='"+GstTIN.Text.Trim()+"' ";
                //myCommand = new OleDbCommand(queryStr, myConn);
                myCommand = new SqlCommand(queryStr, myConn);
                //myCommand.Connection.Open();
                //int countRec = myCommand.ExecuteNonQuery();
                int countRec = (int)myCommand.ExecuteScalar();
                if (countRec != 0)
                {
                    MessageBox.Show("Customer is exist with this account, please change account number", "Add Record");
                    myCommand.Connection.Close();
                }
                else
                {
                    queryStr = "insert into AccountsMasterSheet(CustomerName,AliasName,Address, Contact,GSTIN, State, StateCode,AddressShipped,ContactShipped,TransportName,DateOfUpdate,DateOfCreation,EmailID) Values ( '" + CustName.Text.Replace("/", " ").Replace("'", " ") + "','" + AliasName.Text.Replace("/", " ").Replace("'", " ") + "','" + CustAddress.Text + "','" + CustContact.Text + "','" + GstTIN.Text + "','" + SelectedValueState + "','" + StateCode.Text + "','','','','','" + sdt + "','" + CustEmail.Text + "')";

                    // myCommand = new OleDbCommand(queryStr, myConn);
                    myCommand = new SqlCommand(queryStr, myConn);
                    if (CustName.Text.Trim() != "")
                    {
                        // myCommand.Connection.Open();
                        int Num = myCommand.ExecuteNonQuery();
                        if (Num != 0)
                        {
                            MessageBox.Show("Record Successfully Added....", "Add Record");
                            //this.Close();
                            if (selectedValueCustomer == null)
                            {
                                CustName.Clear();
                                CustAddress.Clear();
                                CustContact.Clear();
                                GstTIN.Clear();
                                State.SelectedItem = "Tamil Nadu";
                                StateCode.Clear();
                              
                               
                                CustEmail.Clear();
                                startDate.SelectedDate = null;
                              

                            }
                        }
                        else
                        {
                            MessageBox.Show("Record is not Added....", "Add Record Error");
                        }
                        myCommand.Connection.Close();
                    }
                    else
                    {
                        MessageBox.Show("Item is blank, Please Enter....", "Add Record Error");
                    }
                }
            } //if selected value dropdown close
            else
            {
                string custnme = CustNameDropBox.Text;

               // string custnmeAccount = (custnme.Split(':')[1]).Trim();
                custnme = (custnme.Split(':')[0]).Trim();

                queryStr = "update AccountsMasterSheet set CustomerName ='" + CustName.Text.Replace("/", " ").Replace("'", " ") + "' , AliasName ='" + AliasName.Text.Replace("/", " ").Replace("'", " ") + "'  ,Address ='" + CustAddress.Text + "' , Contact = '" + CustContact.Text + "',GSTIN='" + GstTIN.Text + "', State='" + SelectedValueState + "', StateCode='" + StateCode.Text + "', AddressShipped ='' ,ContactShipped ='' , TransportName = '',DateOfUpdate='" + UpdateDate.Text + "', DateOfCreation='" + sdt + "', EmailID='" + CustEmail.Text + "' where  LTRIM(RTRIM(CustomerName)) ='" + custnme + "'  and LTRIM(RTRIM(GSTIN))='" + GstTIN.Text.Trim() + "'  ";
                //myCommand = new OleDbCommand(queryStr, myConn);
                myCommand = new SqlCommand(queryStr, myConn);
                //SqlCommand cmd  = new SqlCommand(sql, conn);   
                if (CustName.Text.Trim() != "")
                {
                    // myCommand.Connection.Open();
                    int Num = myCommand.ExecuteNonQuery();
                    if (Num != 0)
                    {
                        MessageBox.Show("Record Successfully Updated....", "Update Record");
                    }
                    else
                    {
                        MessageBox.Show("Record is not Added....", "Update Record Error");
                    }
                    myCommand.Connection.Close();
                }
                else
                {
                    MessageBox.Show("Records can not be updated....", "Add Record Error");
                }
            }

            //Reload Combobox with server data           
            BindComboBox(CustNameDropBox);
            this.Close();

            SheetNewTransactionEntry lp = new SheetNewTransactionEntry();
            lp.Show();

        }
        private void CustNameDropBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string custnme = ((object[])(e.AddedItems))[0].ToString();
             
            string custnmeAccount = (custnme.Split(':')[1]).Trim();
            custnme = (custnme.Split(':')[0]).Trim();
            SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSERPBasic;Trusted_Connection=Yes;");
            conn.Open();
            string sql = "SELECT * FROM AccountsMasterSheet  where  LTRIM(RTRIM(CustomerName)) ='" + custnme.Trim() + "'  and LTRIM(RTRIM(GSTIN))='" + custnmeAccount + "'";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = conn;
            SqlDataReader reader = cmd.ExecuteReader();


            // string constr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\RTSERPBasic\Database\InvWpf-Enhanced.accdb;";
            //OleDbConnection con = new OleDbConnection(constr);
            //string queryStr = @"select * from CustomersDetails where CustomerName = '" + custnme + "'";
            //OleDbCommand command = new OleDbCommand(queryStr, con);
            //con.Open();
            //OleDbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                // var CustID = reader.GetValue(0).ToString();
                CustName.Text = reader.GetString(0);
                AliasName.Text = reader.GetString(1);
                CustAddress.Text = reader.GetString(2);
                CustContact.Text = reader.GetString(3);
                GstTIN.Text = reader.GetString(4);
                State.Text = reader.GetString(5);
                StateCode.Text = reader.GetString(6);
               
                //UpdateDate.Text = reader.GetString(10);
                //AddDate.Text = reader.GetString(11);
                startDate.SelectedDate = Convert.ToDateTime(reader.GetString(11));
                CustEmail.Text = reader.GetString(12);

            }
            reader.Close();
        }

        private void TextBoxHighlight_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                textBox.Background = Brushes.BlueViolet;
                textBox.Foreground = Brushes.White;
            }
            //SolidColorBrush brush = (sender as TextBox).Foreground as SolidColorBrush;
            //if (null != brush)
            //{
            //Brush brush = Brushes.Black;
            //if (brush.IsFrozen)
            //{
            //    brush = brush.Clone();
            //}
            //brush.Opacity = 0.2;

            //Background = Brushes.PaleGoldenrod;

            //}
        }
        private void TextBoxHighlight_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            textBox.Background = Brushes.White;
            textBox.Foreground = Brushes.Black;
        }

        private void CombopboxHighlight_LostFocus(object sender, RoutedEventArgs e)
        {
            var combobox = e.OriginalSource as ComboBox;
            if (combobox != null)
            {
                combobox.Background = Brushes.White;
                combobox.Foreground = Brushes.Black;
            }
        }


        private void CombopboxHighlight_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as ComboBox;
            if (textBox != null)
            {
                //textBox.Background = Brushes.Blue;
                //textBox.Foreground = Brushes.Black;
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

        private void DatePicker_PreviewKeyUp(object sender, KeyEventArgs e)
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
    }
}
