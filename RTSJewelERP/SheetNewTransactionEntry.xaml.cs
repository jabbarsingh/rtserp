using RTSJewelERP.SheetAccountListTableAdapters;
using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Interaction logic for SheetNewTransactionEntry.xaml
    /// </summary>
    public partial class SheetNewTransactionEntry : Window
    {
        //private long InvoiceNumber = 0;
        private long voucherNumber = 0;
        public SheetNewTransactionEntry()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window
            BindComboBox(PartyNameDropBox);
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();


            string sqlvoucher = "select number from AutoIncrement where Name = 'SheetReceiptVoucher'";
            SqlCommand cmdvoucher = new SqlCommand(sqlvoucher);
            cmdvoucher.Connection = con;
            SqlDataReader readerVoucher = cmdvoucher.ExecuteReader();

            //tmpProduct = new Product();

            while (readerVoucher.Read())
            {
                voucherNumber = readerVoucher.GetInt64(0);

            }
            readerVoucher.Close();
            SheetReceiptNumber.Text = voucherNumber.ToString();

            //billQuoteNumber = File.ReadAllText(@"c:\RTSERPBasic\Database\BillNumber.txt", Encoding.UTF8);
            //SheetReceiptNumber.Text = billQuoteNumber;
            textBoxAcNumber.Focus();

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
                           select UserRec.CustomerName).Distinct();
            comboBoxName.ItemsSource = LinqRes;
            // comboBoxName.SelectedValueBinding = new Binding("Col6");


        }

        private void PartyNameDropBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {



        }




        private void AddLedger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedValueItem = (PartyNameDropBox.SelectedItem);
                var modeDropBox = ((ComboBoxItem)ModeDropBox.SelectedItem).Content.ToString();
                string crValue = "0";
                string drValue = "0";
                if (CR.Text != "")
                {
                    crValue = CR.Text;
                }
                if (DR.Text != "")
                {
                    drValue = DR.Text;
                }


                string transDate = TransDate.SelectedDate.ToString();

                // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
                DateTime dt = Convert.ToDateTime(TransDate.SelectedDate);
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


                transDate = years + "/" + months + "/" + days;

                MessageBox.Show("Are you sure you want to proceed,Please cross verify Party Name and Receipt Number", "Confirmation");

                //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                myConnSalesInvEntryStr.Open();

                string CountStockItemsEntryStr = "SELECT COUNT(*) From AccountLedgersSheet where LTRIM(RTRIM(PartyName)) ='" + selectedValueItem + "' and LTRIM(RTRIM(ReceiptNumber))='" + SheetReceiptNumber.Text.Trim() + "' and LTRIM(RTRIM(accountnumber)) = '" + SheetAccountNumber.Text.Trim() + "'";
                SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
                myCommand.Connection = myConnSalesInvEntryStr;

                //int countRec = myCommand.ExecuteNonQuery();
                int countRec = (int)myCommand.ExecuteScalar();
                myCommand.Connection.Close();


                if (countRec != 0)
                {

                    string queryStrStockUpdate = "";
                    queryStrStockUpdate = "update AccountLedgersSheet  set UpdateDate='" + UpdateDate.Text + "',  CR='" + crValue + "',   DR='" + drValue + "'  where LTRIM(RTRIM(PartyName)) ='" + selectedValueItem + "' and LTRIM(RTRIM(ReceiptNumber))='" + SheetReceiptNumber.Text.Trim() + "' and LTRIM(RTRIM(accountnumber)) = '" + SheetAccountNumber.Text.Trim() + "'";

                    SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                    myCommandStkUpdate.Connection.Open();
                    myCommandStkUpdate.Connection = myConnSalesInvEntryStr;

                        // myCommandStk.Connection.Open();
                        int Num = myCommandStkUpdate.ExecuteNonQuery();
                        if (Num != 0)
                        {
                            // MessageBox.Show("Record Successfully Updated....", "Update Record");
                        }
                        else
                        {
                            MessageBox.Show("Record is not Updated....", "Update Record Error");
                        }
                        // myCommandStk.Connection.Close();

                    myCommandStkUpdate.Connection.Close();
                }
                else
                {

                    if (selectedValueItem == null)
                    {
                        MessageBox.Show("Please Select Party", "Add Record");
                    }
                    else
                    {




                        string queryStr = "";

                        queryStr = "insert into AccountLedgersSheet(PartyName,ReceiptNumber, TransactionDate,Mode,Against,CR,DR,UpdateDate, accountnumber) Values ( '" + selectedValueItem + "','" + SheetReceiptNumber.Text + "','" + transDate + "','" + modeDropBox + "','" + Against.Text + "','" + crValue + "' ,'" + drValue + "','" + UpdateDate.Text + "','" + SheetAccountNumber.Text + "')";

                        myCommand = new SqlCommand(queryStr, myConnSalesInvEntryStr);
                        myCommand.Connection.Open();
                        if (SheetReceiptNumber.Text.Trim() != "" && TransDate.Text.Trim() != "")
                        {
                            //myCommand.Connection.Open();
                            int Num = myCommand.ExecuteNonQuery();

                            MessageBox.Show("Transaction Successfully Added....", "Add Record");
                            if (voucherNumber == Convert.ToInt64(SheetReceiptNumber.Text.Trim()))
                            {
                                SqlConnection consrauto = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                                consrauto.Open();
                                string updateVoucher = "";
                                updateVoucher = "update AutoIncrement  set  Number='" + (Convert.ToInt64(SheetReceiptNumber.Text.Trim()) + 1) + "' where Name ='SheetReceiptVoucher' and Type='SheetReceiptVoucher'";
                                SqlCommand myCommandStkUpdateauto = new SqlCommand(updateVoucher, consrauto);
                                myCommandStkUpdateauto.Connection = consrauto;
                                int Numauto = myCommandStkUpdateauto.ExecuteNonQuery();
                                if (Numauto != 0)
                                {

                                }
                            }



                            Against.Clear();
                            //CR.Clear();
                            DR.Clear();
                            //SheetReceiptNumber.Text = billquoteNo.ToString(); 

                            SheetReceiptNumber.IsEnabled = false;
                            Submitreceipt.IsEnabled = false;

                        }
                        else
                        {
                            MessageBox.Show("Record is not Added....", "Add Record Error");
                        }



                        myCommand.Connection.Close();
                    }

                }

               
                this.Close();
                SheetNewTransactionEntry sv = new SheetNewTransactionEntry();
                sv.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



        }

        private void AddSheetAccount_Click(object sender, RoutedEventArgs e)
        {
            SheetAddAccount lp = new SheetAddAccount();
            lp.Show();
            this.Close();
        } //if selected value dropdown close

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            //var data ;
                //= Model.GetData();

            var custAdpt = new AccountsMasterSheetTableAdapter();
            var custInfoVal = custAdpt.GetData();
            var LinqRes = (from UserRec in custInfoVal
                           orderby UserRec.CustomerName ascending
                           select UserRec.CustomerName).Distinct();
            //data.ItemsSource = LinqRes;

            var data = custInfoVal;




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

            // Add the result    
            foreach (var obj in data)
            {
                if (obj.GSTIN.ToLower().Contains(query.ToLower()))  /// StartsWith(query.ToLower()))
                //if (obj.GSTIN.ToLower().StartsWith(query.ToLower()))
                {
                    // The word starts with this... Autocomplete must work    
                    addItem(obj.GSTIN);
                  
                    found = true;
                }
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
                textBoxAcNumber.Text = (sender as TextBlock).Text;
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
        }

        private void textBoxAcNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            string custnme = textBoxAcNumber.Text;
            SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSERPBasic;Trusted_Connection=Yes;");
            conn.Open();
            string sql = "select * from AccountsMasterSheet where GSTIN = '" + custnme + "'";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = conn;
            SqlDataReader reader = cmd.ExecuteReader();

            //string constr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\RTSERPBasic\Database\InvWpf-Enhanced.accdb;";
            //OleDbConnection con = new OleDbConnection(constr);
            //string queryStr = @"select * from PurchaseInvoices where PartyName = '" + custnme + "'";
            //OleDbCommand command = new OleDbCommand(queryStr, con);
            //con.Open();
            //OleDbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {

                //var CustID = reader.GetValue(0).ToString();
                PartyNameDropBox.SelectedValue = reader.GetString(0);
                SheetAccountNumber.Text = reader.GetString(4);
                AliasCust.Text = reader.GetString(1);
                //SheetReceiptNumber.Text = reader.GetString(4);


            }
            reader.Close();
        }

        private void Barcode_TextChanged(object sender, TextChangedEventArgs e)
        {
            string custnme = Barcode.Text;
            SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSERPBasic;Trusted_Connection=Yes;");
            conn.Open();
            string sql = "select * from AccountsMasterSheet where Barcode = '" + custnme + "'";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = conn;
            SqlDataReader reader = cmd.ExecuteReader();

            //string constr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\RTSERPBasic\Database\InvWpf-Enhanced.accdb;";
            //OleDbConnection con = new OleDbConnection(constr);
            //string queryStr = @"select * from PurchaseInvoices where PartyName = '" + custnme + "'";
            //OleDbCommand command = new OleDbCommand(queryStr, con);
            //con.Open();
            //OleDbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {

                //var CustID = reader.GetValue(0).ToString();
                PartyNameDropBox.SelectedValue = reader.GetString(0);
                SheetAccountNumber.Text = reader.GetString(4);
                AliasCust.Text = reader.GetString(1);
                //SheetReceiptNumber.Text = reader.GetString(4);


            }
            reader.Close();
        }

        private void PrintSimpleTextButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a PrintDialog
            PrintDialog printDlg = new PrintDialog();

            // Create a FlowDocument dynamically.
            FlowDocument doc = CreateFlowDocument();
            doc.Name = "FlowDoc";

            // Create IDocumentPaginatorSource from FlowDocument
            IDocumentPaginatorSource idpSource = doc;

            // Call PrintDocument method to send document to printer
            //Uncomment for Print
            printDlg.PrintDocument(idpSource.DocumentPaginator, "Receipt Printing.");

            this.Close();
        }

        /// <summary>
        /// This method creates a dynamic FlowDocument. You can add anything to this
        /// FlowDocument that you would like to send to the printer
        /// </summary>
        /// <returns></returns>
        private FlowDocument CreateFlowDocument()
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select top 1 * from Company where LTRIM(RTRIM(Alias))='GST'";
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
            /* style for products table header, assigned via type + class selectors */

            System.Windows.Documents.Paragraph p = new System.Windows.Documents.Paragraph();
            Span s = new Span();
            s = new Span(new Run(CompanyName));
            s.FontWeight = FontWeights.Bold;
            s.FontSize = 16;
            s.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a1 = new Span();
            a1 = new Span(new Run(GSTIN));
            a1.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a2 = new Span();
            a2 = new Span(new Run(Address + " " + Address2));
            a2.FontSize = 10;
            a2.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a3 = new Span();
            a3 = new Span(new Run("Cash Receipt"));
            a3.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a4 = new Span();
            a4 = new Span(new Run("Receipt# " + voucherNumber));
            a4.FontWeight = FontWeights.Bold;
            a4.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a4acc = new Span();
            a4acc = new Span(new Run("Account# " + SheetAccountNumber.Text));
            a4acc.FontWeight = FontWeights.Bold;
            a4acc.Inlines.Add(new LineBreak());//Line break is used for next line.  

     
            Span a4date = new Span();
            a4date = new Span(new Run("Date: " + UpdateDate.Text));
            a4date.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a5 = new Span();
            a5 = new Span(new Run("------------------------------------"));
            a5.Inlines.Add(new LineBreak());//Line break is used for next line.  
            p.FontSize = 12;
            p.Inlines.Add(s);// Add the span content into paragraph.  
            // p.Inlines.Add(a1);// Add the span content into paragraph.  
            p.Inlines.Add(a2);// Add the span content into paragraph.  
            p.Inlines.Add(a3);// Add the span content into paragraph.  
            p.Inlines.Add(a3);// Add the span content into paragraph.  
            p.Inlines.Add(a4);// Add the span content into paragraph.  
            p.Inlines.Add(a4acc);// Add the span content into paragraph.  
            p.Inlines.Add(a4date);// Add the span content into paragraph.  
            p.Inlines.Add(a5);// Add the span content into paragraph. 

            //If we have some dynamic text the span in flow document does not under "    " as space and we need to use "\t"  for space.  
            // s = new Span(new Run(s1 + "\t" + s2));//we need to use \t for space between s1 and s2 content.  
            //s.Inlines.Add(new LineBreak());
            //p.Inlines.Add(s);
            //Give style and formatting to paragraph content.  
            p.FontSize = 11;
            p.FontStyle = FontStyles.Normal;
            p.TextAlignment = TextAlignment.Center;
            doc.Blocks.Add(p);

            System.Windows.Documents.Table t = new System.Windows.Documents.Table();

            //GridLengthConverter glc = new GridLengthConverter();
            //t.Columns[0].Width = (GridLength)glc.ConvertFromString("30");
            //t.Columns[1].Width = (GridLength)glc.ConvertFromString("100");
            //t.Columns[2].Width = (GridLength)glc.ConvertFromString("70");
            //t.Columns[3].Width = (GridLength)glc.ConvertFromString("70");
            //t.Columns[4].Width = (GridLength)glc.ConvertFromString("70");
            //t.Columns[5].Width = (GridLength)glc.ConvertFromString("70");
            //t.Columns[6].Width = (GridLength)glc.ConvertFromString("70");



            System.Windows.Documents.Paragraph linedot = new System.Windows.Documents.Paragraph();

            System.Windows.Documents.Paragraph p1 = new System.Windows.Documents.Paragraph();
            Span s1 = new Span();
            s1 = new Span(new Run(" Thanks , Cash Received From        " + PartyNameDropBox.Text  ));
            //s1.FontWeight = FontWeights.Bold;
            s1.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a111 = new Span();
            a111 = new Span(new Run("                  Of        ₹  " + CR .Text+ "         For Monthly Sheet. "));
            a111.Inlines.Add(new LineBreak());//Line break is used for next line.  

            p1.FontSize = 12;
            p1.Inlines.Add(s1);// Add the span content into paragraph.  
            // p.Inlines.Add(a1);// Add the span content into paragraph.  
            p1.Inlines.Add(a111);// Add the span content into paragraph.  
            //p1.Inlines.Add(a3);// Add the span content into paragraph.  
            //p1.Inlines.Add(a3);// Add the span content into paragraph.  
            //p1.Inlines.Add(a4);// Add the span content into paragraph.  
            //p1.Inlines.Add(a4date);// Add the span content into paragraph.  
            //p1.Inlines.Add(a5);// Add the span content into paragraph. 

            //If we have some dynamic text the span in flow document does not under "    " as space and we need to use "\t"  for space.  
            // s = new Span(new Run(s1 + "\t" + s2));//we need to use \t for space between s1 and s2 content.  
            //s.Inlines.Add(new LineBreak());
            //p.Inlines.Add(s);
            //Give style and formatting to paragraph content.  
            p1.FontSize = 11;
            p1.FontStyle = FontStyles.Normal;
            p1.TextAlignment = TextAlignment.Center;
            doc.Blocks.Add(p1);

            Span linebrktble = new Span();
            linebrktble = new Span(new Run("-------------------------------------------- "));
            // linebrktble.Inlines.Add(new LineBreak());//Line break is used for next line.  

            linedot.Inlines.Add(linebrktble);// Add the span content into paragraph. 
            linedot.TextAlignment = TextAlignment.Center;
            doc.Blocks.Add(linedot);

            
            System.Windows.Documents.Paragraph signpara = new System.Windows.Documents.Paragraph();

            Span linebrktble1 = new Span();
            linebrktble1 = new Span(new Run("                                                             Signed By "));
            // linebrktble.Inlines.Add(new LineBreak());//Line break is used for next line.  

            signpara.FontSize = 12;
            signpara.Inlines.Add(linebrktble1);// Add the span content into paragraph.  
            signpara.TextAlignment = TextAlignment.Center;
            //linedot.Inlines.Add(linebrktble1);// Add the span content into paragraph.  
            //doc.Blocks.Add(linedot);
            doc.Blocks.Add(signpara);



            doc.Name = "FlowDoc";
            doc.PageWidth = 700;
            doc.PagePadding = new Thickness(55, 25, 5, 5);
            // Create IDocumentPaginatorSource from FlowDocument
            // IDocumentPaginatorSource idpSource = doc;
            // Call PrintDocument method to send document to printer


            return doc;

        
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
                combobox.Background = Brushes.White;
                combobox.Foreground = Brushes.Black;
        }


        private void CombopboxHighlight_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as ComboBox;
                textBox.Background = Brushes.BlueViolet;
                textBox.Foreground = Brushes.Black;

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
    }

}
