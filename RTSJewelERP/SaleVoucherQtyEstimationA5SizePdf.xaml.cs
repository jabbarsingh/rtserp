using iTextSharp.text;
using iTextSharp.text.pdf;
using RTSJewelERP.StorageTableAdapters;
using RTSJewelERP.UnitsTableAdapters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
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
    public partial class SaleVoucherQtyEstimationA5SizePdf : Page
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;
        string Gold916Rate = RTSJewelERP.ConfigClass.Gold916Rate;
        string GoldSadaRate = RTSJewelERP.ConfigClass.GoldSadaRate;
        string SilverPureRate = RTSJewelERP.ConfigClass.SilverPureRate;
        string SilverSadaRate = RTSJewelERP.ConfigClass.SilverSadaRate;
        string OldGoldRate = RTSJewelERP.ConfigClass.OldGoldRate;
        string OldGoldSadaRate = RTSJewelERP.ConfigClass.OldGoldSadaRate;
        string OldSilverRate = RTSJewelERP.ConfigClass.OldSilverRate;

        //public List<BindingData> dataBindingList = new List<BindingData>();
        private long InvoiceNumber = 0;
        private long voucherNumber = 0;
        //Temp varible to hold the last found item
        private Boolean IState = true;
        private string stateCodeVal = "";
        private string SaleAcctName = "";
        private Double discounttotalCommon = 0.0;
        private Double discounttotalByItem = 0.0;
        private Double labourTotal = 0.0;
        private Double makingTotalCharge = 0.0;
        private Double totalInvValues = 0.0;
        private Double totalGrandInvValues = 0.0;
        private Double totalTaxableValues = 0.0;
        private Double totalSGSTTax = 0.0;
        private Double totalCGSTTax = 0.0;
        private Double totalIGSTTax = 0.0;
        private Double totalQuanty = 0.0;
        private Double totalPaid = 0.0;
        private Double BalanceCRorDR = 0.0;
        private Double PackingAndForwarding = 0.0;
        private Double Freight = 0.0;
        private Boolean IsShipBillBothAdreess = false;
        private Double totalVal = 0.0;
        private Double totalBeforeItemDiscount = 0.0;
        private Double oldtotalVal = 0.0;
        private Double totalTaxAmount = 0.0;
        private Double totalCTN = 0.0;
        private Double totalQty = 0.0;
        private Product tmpProduct = null;



        //Array of Cart items 
        private List<Product> ShoppingCart;
        private List<Product> OldCart;
        public SaleVoucherQtyEstimationA5SizePdf()
        {
            //this.PreviewKeyDown += new KeyEventHandler(commonKeyPressed); // Esc Key Close Window



            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window
            dueBal.Content = string.Format("Balance: {0}", (BalanceCRorDR).ToString("C"));
            BindComboBoxUnits(cmbUnits);

            //on the constructor of the class we create a new instance of the shooping cart
            ShoppingCart = new List<Product>();
            OldCart = new List<Product>();
            //autocompleteItemName.autoTextBox1.Focus();
            autocompltCustName.autoTextBox.Focus();

            //txtBarCode.Focus();


            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select number from AutoIncrement where Name = 'SaleInvoice' and CompID = '" + CompID + "'";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = con;
            SqlDataReader reader = cmd.ExecuteReader();

            //tmpProduct = new Product();

            while (reader.Read())
            {
                InvoiceNumber = reader.GetInt64(0);
                invoiceNumber.Text = InvoiceNumber.ToString();

            }
            reader.Close();

            string sqlvoucher = "select number from AutoIncrement where Name = 'SaleVoucher' and CompID = '" + CompID + "'";
            SqlCommand cmdvoucher = new SqlCommand(sqlvoucher);
            cmdvoucher.Connection = con;
            SqlDataReader readerVoucher = cmdvoucher.ExecuteReader();

            //tmpProduct = new Product();

            while (readerVoucher.Read())
            {
                voucherNumber = readerVoucher.GetInt64(0);
                VoucherNumber.Text = voucherNumber.ToString();
            }
            readerVoucher.Close();

        }

        public void BindComboBoxUnits(ComboBox cmbUnitsList)
        {
            var custAdpt = new UnitsTableAdapter();
            var custInfoVal = custAdpt.GetData();
            var LinqRes = (from UserRec in custInfoVal
                           orderby UserRec.Name ascending
                           //select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
                           select (UserRec.Name.Trim())).Distinct();
            cmbUnits.ItemsSource = LinqRes;
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
                    //this.Close();
                    this.NavigationService.GoBack();
                    this.NavigationService.RemoveBackEntry();
                }
            }

            if (e.Key == Key.PageUp)
            {
                if (Convert.ToInt64(invoiceNumber.Text.Trim()) < InvoiceNumber)
                {
                    Int64 inpageup = (invoiceNumber.Text.Trim() != "") ? (Convert.ToInt64(invoiceNumber.Text.Trim()) + 1) : 0;
                    invoiceNumber.Text = inpageup.ToString();
                    VoucherNumber.Text = voucherNumber.ToString();
                    MoveToBill(inpageup.ToString());

                }
                if (Convert.ToInt64(invoiceNumber.Text.Trim()) == InvoiceNumber)
                {
                    autocompltCustName.autoTextBox.Text = "Cash";
                    autocompltCustName.autoTextBox.Focus();
                }
                e.Handled = true;
            }
            if (e.Key == Key.PageDown)
            {
                if (Convert.ToInt64(invoiceNumber.Text.Trim()) > 1)
                {
                    Int64 inpageup = (invoiceNumber.Text.Trim() != "") ? (Convert.ToInt64(invoiceNumber.Text.Trim()) - 1) : 0;
                    invoiceNumber.Text = inpageup.ToString();
                    MoveToBill(inpageup.ToString());
                    e.Handled = true;
                }
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        private void TextBoxCust_KeyUp(object sender, KeyEventArgs e)
        {
            if (autocompltCustName.autoTextBox.Text.Trim() != "Cash")
            {
                CashCustName.Visibility = Visibility.Collapsed;
                //CashName.Visibility = Visibility.Collapsed;
            }
            else
            {
                //CashName.Visibility = Visibility.Visible;
                CashCustName.Visibility = Visibility.Visible;
            }

            bool found = false;
            var border = (resultStackCust.Parent as ScrollViewer).Parent as Border;
            //var data ;
            //= Model.GetData();

            //If a product code is not empty we search the database
            if (Regex.IsMatch(autocompltCustName.autoTextBox.Text.Trim(), @"^\d+$") || 1 == 1)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select AcctName from AccountsList where AcctName like '%" + autocompltCustName.autoTextBox + "%' and CompID = '" + CompID + "'";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                tmpProduct = new Product();

                string query = (sender as TextBox).Text;

                if (query.Length == 0)
                {
                    // Clear    
                    resultStackCust.Children.Clear();
                    border.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    border.Visibility = System.Windows.Visibility.Visible;
                }

                // Clear the list    
                resultStackCust.Children.Clear();

                while (reader.Read())
                {
                    //var CustID = reader.GetValue(0).ToString();

                    tmpProduct.ItemName = reader.GetString(0).Trim();
                    if (tmpProduct.ItemName.ToLower().Contains(query.ToLower()))
                    {
                        // The word starts with this... Autocomplete must work    
                        addCust(tmpProduct.ItemName);



                        found = true;
                    }
                    //tmpProduct.PrintName = reader.GetString(3).Trim();
                    //tmpProduct.ItemCode = reader.GetString(5).Trim();
                    //tmpProduct.ItemBarCode = reader.GetString(7).Trim();

                    //tmpProduct.ItemPrice = reader.GetDouble(9);
                    //tmpProduct.ActualQty = reader.GetDouble(35);
                    //tmpProduct.ActualWt = reader.GetDouble(46);

                }
                reader.Close();
            }









            // Add the result    
            //foreach (var obj in data)
            //{

            //}

            if (!found)
            {
                resultStackCust.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;
            //var data ;
            //= Model.GetData();

            //If a product code is not empty we search the database
            if (Regex.IsMatch(autocompleteItemName.autoTextBox1.Text.Trim(), @"^\d+$") || 1 == 1)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select ItemName from StockItemsByPC where ItemName like '%" + autocompleteItemName.autoTextBox1.Text + "%' and CompID = '" + CompID + "'";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                tmpProduct = new Product();

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

                    tmpProduct.ItemName = reader.GetString(0).Trim();
                    if (tmpProduct.ItemName.ToLower().Contains(query.ToLower()))
                    {
                        // The word starts with this... Autocomplete must work    
                        addItem(tmpProduct.ItemName);



                        found = true;

                    }
                    //tmpProduct.PrintName = reader.GetString(3).Trim();
                    //tmpProduct.ItemCode = reader.GetString(5).Trim();
                    //tmpProduct.ItemBarCode = reader.GetString(7).Trim();

                    //tmpProduct.ItemPrice = reader.GetDouble(9);
                    //tmpProduct.ActualQty = reader.GetDouble(35);
                    //tmpProduct.ActualWt = reader.GetDouble(46);

                }
                reader.Close();
            }









            // Add the result    
            //foreach (var obj in data)
            //{

            //}

            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }

        private void TextBoxAuto_KeyUp(object sender, KeyEventArgs e)
        {
            if (autocompltCustName.autoTextBox.Text.Trim() != "Cash")
            {
                CashCustName.Clear();
                CashCustName.Visibility = Visibility.Collapsed;
                //CashName.Visibility = Visibility.Collapsed;

            }
            else
            {
                //string namecash = CashCustName.Text;
                //CashCustName.Clear();
                //CashCustName.Text = "-" + namecash;

                //CashName.Visibility = Visibility.Visible;
                CashCustName.Visibility = Visibility.Visible;
            }

            string sdt = invDate.SelectedDate.ToString();
            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dt = Convert.ToDateTime(invDate.SelectedDate);
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


            //sdt = years + "/" + months + "/" + days;
            sdt = 2000 + "/" + 04 + "/" + 01;


            string enddt = invDate.SelectedDate.ToString();
            DateTime edt = Convert.ToDateTime(invDate.SelectedDate);
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

                SqlCommand com = new SqlCommand("GetAccountBalanceFinal", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.Add(new SqlParameter("@AcctName", autocompltCustName.autoTextBox.Text.Trim()));
                com.Parameters.Add(new SqlParameter("@StartDate", sdt));
                com.Parameters.Add(new SqlParameter("@EndDate", enddt));
                com.Parameters.Add(new SqlParameter("@CompID", CompID));
                SqlDataAdapter sda = new SqlDataAdapter(com);
                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    // double dDebtAcctLedgerAmt = (reader["DebtAcctLedgerAmt"] != DBNull.Value) ? (reader.GetDouble(0)) : 0;
                    //double dPayVAmt = (reader["PayVAmt"] != DBNull.Value) ? (reader.GetDouble(1)) : 0;
                    double dDRAmt = (reader["Debit"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                    double dCRAmt = (reader["Credit"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                    double actBalAmt = dDRAmt - dCRAmt;




                    lblCustBalance.Content = string.Format("Balance: {0}", actBalAmt.ToString());
                }

            }



        }

        private void addCust(string text)
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
                autocompltCustName.autoTextBox.Text = (sender as TextBlock).Text;

                autocompltCustName.autoTextBox.Focus();
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
            resultStackCust.Children.Add(block);
            //textBoxCustName.Focus();
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
                autocompleteItemName.autoTextBox1.Text = (sender as TextBlock).Text;
                autocompleteItemName.autoTextBox1.Focus();
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
            autocompleteItemName.autoTextBox1.Focus();
        }

        void CartGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()).ToString();
            //CartGrid.Items.Refresh();
        }
        private int i = 1;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (autocompltCustName.autoTextBox.Text == "Card")
                {
                    receivedCash.Clear();
                    receivedCard.Text = Math.Round((totalTaxableValues - oldtotalVal), 0).ToString();
                }
                if (autocompltCustName.autoTextBox.Text == "Cash")
                {
                    receivedCard.Clear();
                    receivedCash.Text = Math.Round((totalTaxableValues - oldtotalVal), 0).ToString();
                }

                if (autocompleteItemName.autoTextBox1.Text != "")
                {
                    //Customer sv = new Customer();
                    //this.NavigationService.Navigate(sv);
                    //we first check if a product has been selected
                    //if (tmpProduct == null)
                    //{
                    //    //if not we call the search button method
                    //    //Button_Click_1(null, null);
                    //    //we check again if the product was found
                    //    if (tmpProduct == null)
                    //    {
                    //        //if tmpProduct is empty (Product not found) we exit the procedure
                    //        MessageBox.Show("No product was selected", "No product", MessageBoxButton.OK,
                    //            MessageBoxImage.Exclamation);
                    //        //exit procedure
                    //        return;
                    //    }
                    //}



                    //product quantity
                    double qty;
                    double wtqty;

                    // we try to parse the number of the textbox if the number is invalid 
                    double.TryParse(txtQty.Text, out qty);
                    //double.TryParse(txtWeight.Text, out wtqty);
                    //if qty is 0 we assign 0 otherwise we assign the actual parsed value
                    qty = qty == 0 ? 1 : qty;
                    //really basic validation that checks inventory
                    //if (tmpProduct.ItemName == "Old Gold" || tmpProduct.ItemName == "Old Silver" || autocompleteItemName.autoTextBox1.Text == "Old Gold" || autocompleteItemName.autoTextBox1.Text == "Old Silver")
                    //{

                    //    //we check if product is not already in the cart if it is we remove the old one
                    //    var isexistItem = OldCart.Where(s => s.ItemName == tmpProduct.ItemName);
                    //    if (isexistItem.Count() == 1)
                    //    {

                    //    }
                    //    //OldCart.RemoveAll(s => s.ItemName == tmpProduct.ItemName);
                    //    //we add the product to the Cart
                    //    OldCart.Add(new Product()
                    //    {
                    //        //Sr = i,
                    //        BilledWt = (txtWeight.Text == "") ? 0.0 : Convert.ToDouble(txtWeight.Text),
                    //        ItemName = tmpProduct.ItemName != null ? tmpProduct.ItemName : autocompleteItemName.autoTextBox1.Text,
                    //        ItemPrice = (txtPrice.Text == "") ? 0.0 : Convert.ToDouble(txtPrice.Text),//tmpProduct.ItemPrice, //Get from textbox if changed
                    //        BilledQty = (txtQty.Text == "") ? 0.0 : Convert.ToDouble(txtQty.Text),
                    //        WastagePerc = (txtWaste.Text == "") ? 0.0 : Convert.ToDouble(txtWaste.Text),
                    //        MC = (txtMC.Text == "") ? 0.0 : Convert.ToDouble(txtMC.Text),
                    //        SaleDiscountPerc = (txtDiscPerc.Text == "") ? 0.0 : Convert.ToDouble(txtDiscPerc.Text),
                    //        GSTRate = (txtGSTRate.Text == "") ? 0 : Convert.ToInt16(txtGSTRate.Text)
                    //    });

                    //    //perform  query on Shopping Cart to select certain fields and perform subtotal operation 
                    //    BindDataOldGridGrid();
                    //    i++;
                    //    //<----------------------
                    //    //cleanup variables
                    //    tmpProduct = null;
                    //    //once the products had been added we clear the textbox of code and quantity.
                    //    autocompleteItemName.autoTextBox1.Text = string.Empty;
                    //    //autocompleteItemName.autoTextBox1.Text = string.Empty;
                    //    //txtQty.Text = string.Empty;
                    //    txtQty.Text = "1";
                    //    txtDiscPerc.Text = string.Empty;
                    //    txtGSTRate.Text = string.Empty;
                    //    txtMC.Text = string.Empty;
                    //    txtWeight.Text = string.Empty;
                    //    txtWaste.Text = string.Empty;
                    //    txtPrice.Text = string.Empty;
                    //    autocompleteItemName.autoTextBox1.Focus(); // Uncomment for without Barcode
                    //    //txtBarCode.Focus();//Only when Barcode customer
                    //    //clean up current product label
                    //    //cprod.Content = "Current product N/A";



                    //}
                    //else
                    //{
                    if (qty <= tmpProduct.ActualQty)
                    {

                        //we check if product is not already in the cart if it is we remove the old one
                        var isexistItem = ShoppingCart.Where(s => s.ItemName == tmpProduct.ItemName);
                        if (isexistItem.Count() == 1)
                        {

                        }
                        //ShoppingCart.RemoveAll(s => s.ItemName == tmpProduct.ItemName); // Remove Existing item if same name
                        //ShoppingCart.RemoveAll(s => s.ItemName == tmpProduct.ItemName); // Remove Existing item if same barcode
                        //we add the product to the Cart
                        ShoppingCart.Add(new Product()
                        {
                            //Sr = i,
                            //ItemName = tmpProduct.ItemName,
                            //ItemPrice = tmpProduct.ItemPrice,
                            //BilledQty = qty,
                            //BilledWt = (txtWeight.Text == "") ? 0.0 : Convert.ToDouble(txtWeight.Text),
                            ItemDesc = (txtCTN.Text.Trim() == "") ? "" : txtCTN.Text.Trim(),
                            UnitID = tmpProduct.UnitID,
                            ItemName = tmpProduct.ItemName,
                            ItemPrice = (txtPrice.Text.Trim() == "") ? 0.0 : Convert.ToDouble(txtPrice.Text),//tmpProduct.ItemPrice, //Get from textbox if changed
                            BilledQty = (txtQty.Text.Trim() == "") ? 0.0 : Convert.ToDouble(txtQty.Text),
                            //WastagePerc = (txtWaste.Text == "") ? 0.0 : Convert.ToDouble(txtWaste.Text),
                            //MC = (txtMC.Text == "") ? 0.0 : Convert.ToDouble(txtMC.Text),
                            SaleDiscountPerc = (txtDiscPerc.Text.Trim() == "") ? 0.0 : Convert.ToDouble(txtDiscPerc.Text)
                            //GSTRate = (txtGSTRate.Text == "") ? 0 : Convert.ToInt16(txtGSTRate.Text)
                        });

                        //perform  query on Shopping Cart to select certain fields and perform subtotal operation 
                        BindDataGrid();
                        i++;
                        //<----------------------
                        //cleanup variables
                        tmpProduct = null;
                        //once the products had been added we clear the textbox of code and quantity.
                        autocompleteItemName.autoTextBox1.Text = string.Empty;
                        //autocompleteItemName.autoTextBox1.Text = string.Empty;
                        txtQty.Text = "1";
                        txtDiscPerc.Text = string.Empty;
                        //txtGSTRate.Text = string.Empty;
                        //txtMC.Text = string.Empty;
                        //txtWeight.Text = string.Empty;
                        //txtWaste.Text = string.Empty;
                        txtPrice.Text = string.Empty;
                        autocompleteItemName.autoTextBox1.Focus();
                        txtCTN.Text = string.Empty;
                        //clean up current product label
                        //cprod.Content = "Current product N/A";

                    }
                    else
                    {
                        MessageBox.Show("Not enough Inventory", "Inventory Error", MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);


                        //-------------Add Product even though not in inventory
                        //we check if product is not already in the cart if it is we remove the old one
                        var isexistItem = ShoppingCart.Where(s => s.ItemName == tmpProduct.ItemName);
                        if (isexistItem.Count() == 1)
                        {

                        }
                        //ShoppingCart.RemoveAll(s => s.ItemName == tmpProduct.ItemName);
                        //we add the product to the Cart
                        ShoppingCart.Add(new Product()
                        {
                            //Sr = i,
                            //ItemName = tmpProduct.ItemName,
                            //ItemPrice = tmpProduct.ItemPrice,
                            //BilledQty = qty,
                            //BilledWt = (txtWeight.Text == "") ? 0.0 : Convert.ToDouble(txtWeight.Text),
                            ItemDesc = (txtCTN.Text.Trim() == "") ? "" : (txtCTN.Text.Trim()),
                            UnitID = (cmbUnits.Text.Trim() != "") ? cmbUnits.Text : "Pc",
                            ItemName = autocompleteItemName.autoTextBox1.Text.Trim(),// tmpProduct.ItemName,                                              
                            ItemPrice = (txtPrice.Text.Trim() == "") ? 0.0 : Convert.ToDouble(txtPrice.Text),//tmpProduct.ItemPrice, //Get from textbox if changed
                            BilledQty = (txtQty.Text.Trim() == "") ? 0.0 : Convert.ToDouble(txtQty.Text),
                            //WastagePerc = (txtWaste.Text == "") ? 0.0 : Convert.ToDouble(txtWaste.Text),
                            //MC = (txtMC.Text == "") ? 0.0 : Convert.ToDouble(txtMC.Text),
                            SaleDiscountPerc = (txtDiscPerc.Text.Trim() == "") ? 0.0 : Convert.ToDouble(txtDiscPerc.Text)
                            //GSTRate = (txtGSTRate.Text == "") ? 0 : Convert.ToInt16(txtGSTRate.Text)
                        });

                        //perform  query on Shopping Cart to select certain fields and perform subtotal operation 
                        BindDataGrid();
                        i++;
                        //<----------------------
                        //cleanup variables
                        tmpProduct = null;
                        //once the products had been added we clear the textbox of code and quantity.
                        autocompleteItemName.autoTextBox1.Text = string.Empty;
                        //autocompleteItemName.autoTextBox1.Text = string.Empty;
                        txtQty.Text = "1";
                        txtDiscPerc.Text = string.Empty;
                        //txtGSTRate.Text = string.Empty;
                        //txtMC.Text = string.Empty;
                        //txtWeight.Text = string.Empty;
                        //txtWaste.Text = string.Empty;
                        txtPrice.Text = string.Empty;
                        autocompleteItemName.autoTextBox1.Focus();
                        txtCTN.Text = string.Empty;
                        //---------------Write Code Below to Add Item in StockItems Dynamically with minimum data, if some data not provided then send the item to Pending tasks




                    }
                }
                else
                {
                    MessageBox.Show("Item Empty", "Add Item Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    //autocompleteItemName.autoTextBox1.Focus();
                    receivedCash.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please Enter Valid Data");
            }
        }

        //private void BindDataOldGridGrid()
        //{
        //    //we query the array cart and add a new calculated field Subtotal
        //    var cartItems = from s in OldCart
        //                    select new
        //                    {
        //                        //s.Sr,
        //                        Product = s.ItemName,
        //                        //HSN = s.HSN,
        //                        Qty = s.BilledQty,
        //                        Wt = s.BilledWt,
        //                        Wast = s.WastagePerc,
        //                        TotalWt = Math.Round(s.BilledWt - (s.BilledWt * s.WastagePerc / 100), 2),
        //                        s.MC,
        //                        Price = s.ItemPrice,
        //                        Amount = Math.Round(  (s.BilledQty * (s.BilledWt - (s.BilledWt * s.WastagePerc / 100)) * s.ItemPrice), 2),
        //                        Disc = s.SaleDiscountPerc,
        //                        TaxableAmount = Math.Round((  (s.BilledQty * (s.BilledWt - (s.BilledWt * s.WastagePerc / 100)) * s.ItemPrice)) - ((  (s.BilledQty * (s.BilledWt - (s.BilledWt * s.WastagePerc / 100)) * s.ItemPrice)) * s.SaleDiscountPerc / 100), 2),
        //                        GST = s.GSTRate,
        //                        Total = Math.Round(((  (s.BilledQty * (s.BilledWt - (s.BilledWt * s.WastagePerc / 100)) * s.ItemPrice)) - ((  (s.BilledQty * (s.BilledWt - (s.BilledWt * s.WastagePerc / 100)) * s.ItemPrice)) * s.SaleDiscountPerc / 100)) + (((  (s.BilledQty * (s.BilledWt - (s.BilledWt * s.WastagePerc / 100)) * s.ItemPrice)) - ((  (s.BilledQty * (s.BilledWt - (s.BilledWt * s.WastagePerc / 100)) * s.ItemPrice)) * s.SaleDiscountPerc / 100)) * s.GSTRate / 100), 2)

        //                    };

        //    //refresh dataGridview-----------
        //    OldGoldGrid.ItemsSource = null;
        //    OldGoldGrid.ItemsSource = cartItems;
        //    //we add the total with sum(price) and apply a currency formating.
        //    lbOldTotal.Content = string.Format("Total: {0}", OldCart.Sum(x => ((  (x.BilledQty * (x.BilledWt - (x.BilledWt * x.WastagePerc / 100)) * x.ItemPrice)) - ((  (x.BilledQty * (x.BilledWt - (x.BilledWt * x.WastagePerc / 100)) * x.ItemPrice)) * x.SaleDiscountPerc / 100)) + (((  (x.BilledQty * (x.BilledWt - (x.BilledWt * x.WastagePerc / 100)) * x.ItemPrice)) - ((  (x.BilledQty * (x.BilledWt - (x.BilledWt * x.WastagePerc / 100)) * x.ItemPrice)) * x.SaleDiscountPerc / 100)) * x.GSTRate / 100)).ToString("C"));
        //    oldtotalVal = cartItems.Sum(x => x.Total);

        //    lbGrandTotal.Content = string.Format("Grand Total: {0}", (Math.Round((totalVal - oldtotalVal), 0)).ToString("C"));

        //    if (autocompltCustName.autoTextBox.Text == "Card")
        //    {
        //        receivedCash.Clear();
        //        receivedCard.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
        //    }
        //    if (autocompltCustName.autoTextBox.Text == "Cash")
        //    {
        //        receivedCard.Clear();
        //        receivedCash.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
        //    }

        //    double cashreceived = (receivedCash.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCash.Text.Trim());
        //    double cardreceived = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());
        //    double paytmreceived = (receivedPaytm.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedPaytm.Text.Trim());
        //    double flatoff = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());

        //    double offerzone = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
        //    double loyaltycard = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());

        //    dueBal.Content = string.Format("Balance:  {0}", Math.Round((totalVal - oldtotalVal - (cashreceived + cardreceived + paytmreceived + flatoff + offerzone + loyaltycard)), 2)).ToString();



        //}

        //Adds the Shopping cart data to the grid
        private void BindDataGrid()
        {
            //receivedCash.Text = "";
            //receivedCard.Text = "";
            //receivedOffer.Text = "";
            //var Amount = 0.0;
            //var TaxableAmount = 0.0;
            //var TotalWt = 0.0;
            //var Qty = 0.0;
            //var Wt = 0.0;
            //var Disc = 0.0;
            //var Price = 0.0;
            //var Wast = 0.0;
            //var GST = 0.0;
            //we query the array cart and add a new calculated field Subtotal
            var cartItems = from s in ShoppingCart
                            select new
                            {
                                Product = s.ItemName,
                                //HSN = s.HSN,
                                Desc = s.ItemDesc,
                                Qty = s.BilledQty,
                                UOM = s.UnitID,
                                //Wt = s.BilledWt,
                                //Wast = s.WastagePerc,
                                //TotalWt = Math.Round((s.BilledWt + (s.BilledWt * s.WastagePerc / 100)), 2),
                                //s.MC,
                                Price = s.ItemPrice,
                                Amount = Math.Round((s.BilledQty * s.ItemPrice), 2),
                                Disc = s.SaleDiscountPerc,
                                Total = Math.Round(((s.BilledQty * s.ItemPrice)) - (((s.BilledQty * s.ItemPrice)) * s.SaleDiscountPerc / 100), 0)
                                //GST = s.GSTRate,
                                //Tax = Math.Round((((s.BilledQty * s.ItemPrice)) - (((s.BilledQty * s.ItemPrice)) * s.SaleDiscountPerc / 100)) * (s.GSTRate) / 100, 2),
                                //Total = Math.Round((((s.BilledQty * s.ItemPrice)) - (((s.BilledQty * s.ItemPrice)) * s.SaleDiscountPerc / 100)) + ((((s.BilledQty * s.ItemPrice)) - (((s.BilledQty * s.ItemPrice)) * s.SaleDiscountPerc / 100)) * s.GSTRate / 100), 2)


                                //Product = s.ItemName,
                                //Qty = Convert.ToDouble(txtQty.Text), // s.BilledQty,
                                //Wt = Convert.ToDouble(txtWeight.Text), // s.BilledWt,
                                //Wast = Convert.ToDouble(txtWaste.Text),// s.WastagePerc,
                                //TotalWt = Convert.ToDouble(txtWeight.Text) + (Convert.ToDouble(txtWeight.Text) * Convert.ToDouble(txtWaste.Text) / 100),
                                //MC = Convert.ToDouble(txtMC.Text),//s.MC,
                                //Price = Convert.ToDouble(txtPrice.Text),//s.ItemPrice,
                                //Amount = Convert.ToDouble(txtMC.Text) + Convert.ToDouble(txtQty.Text) * (Convert.ToDouble(txtWeight.Text) + (Convert.ToDouble(txtWeight.Text) * Convert.ToDouble(txtWaste.Text) / 100)) * Convert.ToDouble(txtPrice.Text),
                                //Disc = Convert.ToDouble(txtDiscPerc.Text),//s.SaleDiscountPerc,
                                //TaxableAmount = ((Convert.ToDouble(txtMC.Text) + Convert.ToDouble(txtQty.Text) * (Convert.ToDouble(txtWeight.Text) + (Convert.ToDouble(txtWeight.Text) * Convert.ToDouble(txtWaste.Text) / 100)) * Convert.ToDouble(txtPrice.Text)) - ((Convert.ToDouble(txtMC.Text) + Convert.ToDouble(txtQty.Text) * (Convert.ToDouble(txtWeight.Text) + (Convert.ToDouble(txtWeight.Text) * Convert.ToDouble(txtWaste.Text) / 100)) * Convert.ToDouble(txtPrice.Text)) * Convert.ToDouble(txtDiscPerc.Text) / 100)),
                                //GST = Convert.ToDouble(txtGSTRate.Text),// s.GSTRate,
                                //Total = (((Convert.ToDouble(txtMC.Text) + Convert.ToDouble(txtQty.Text) * (Convert.ToDouble(txtWeight.Text) + (Convert.ToDouble(txtWeight.Text) * Convert.ToDouble(txtWaste.Text) / 100)) * Convert.ToDouble(txtPrice.Text)) - ((Convert.ToDouble(txtMC.Text) + Convert.ToDouble(txtQty.Text) * (Convert.ToDouble(txtWeight.Text) + (Convert.ToDouble(txtWeight.Text) * Convert.ToDouble(txtWaste.Text) / 100)) * Convert.ToDouble(txtPrice.Text)) * Convert.ToDouble(txtDiscPerc.Text) / 100))) + ((((Convert.ToDouble(txtMC.Text) + Convert.ToDouble(txtQty.Text) * (Convert.ToDouble(txtWeight.Text) + (Convert.ToDouble(txtWeight.Text) * Convert.ToDouble(txtWaste.Text) / 100)) * Convert.ToDouble(txtPrice.Text)) - ((Convert.ToDouble(txtMC.Text) + Convert.ToDouble(txtQty.Text) * (Convert.ToDouble(txtWeight.Text) + (Convert.ToDouble(txtWeight.Text) * Convert.ToDouble(txtWaste.Text) / 100)) * Convert.ToDouble(txtPrice.Text)) * Convert.ToDouble(txtDiscPerc.Text) / 100))) * Convert.ToDouble(txtGSTRate.Text) / 100)
                            };

            //refresh dataGridview-----------
            CartGrid.ItemsSource = null;
            CartGrid.ItemsSource = cartItems;

            //we add the total with sum(price) and apply a currency formating.
            //lbTotal.Content = string.Format("Total: {0}", ShoppingCart.Sum(x => (((x.BilledQty * x.ItemPrice)) - (((x.BilledQty * x.ItemPrice)) * x.SaleDiscountPerc / 100)) + ((((x.BilledQty * x.ItemPrice)) - (((x.BilledQty * x.ItemPrice)) * x.SaleDiscountPerc / 100)) * x.GSTRate / 100)).ToString("C"));

            // totalCTN = cartItems.Sum(x => x.CTN);

            totalVal = cartItems.Sum(x => x.Total);
            lbTotal.Content = string.Format("Total: {0}", totalVal.ToString("C"));

            totalBeforeItemDiscount = cartItems.Sum(x => x.Amount);
            totalInvValues = cartItems.Sum(x => x.Total);
            //totalTaxAmount = cartItems.Sum(x => x.Amount);
            totalQuanty = cartItems.Sum(x => x.Qty);
            totalTaxableValues = cartItems.Sum(x => x.Total);
            discounttotalByItem = cartItems.Sum(x => (x.Disc * x.Amount / 100));
            //makingTotalCharge = cartItems.Sum(x => x.MC);



            //discounttotalval = cartItems.Sum(x => x.Disc);
            //lbTotalTax.Content = string.Format("Tax: {0}", cartItems.Sum(x => x.Tax).ToString("C"));
            lbGrandTotal.Content = string.Format("Grand Total: {0}", (Math.Round((totalTaxableValues - oldtotalVal), 0)).ToString("C"));

            double packCharge = (PackCharge.Text.Trim() == "") ? 0 : Convert.ToDouble(PackCharge.Text.Trim());
            double gsttaxVCharge = (GSTTaxVa.Text.Trim() == "") ? 0 : Convert.ToDouble(GSTTaxVa.Text.Trim());

            totalGrandInvValues = Math.Round((totalTaxableValues - oldtotalVal + packCharge + gsttaxVCharge), 0);
            lbGrandSum.Content = string.Format("Grand Sum: {0}", totalGrandInvValues.ToString("C"));


            lblTotalDiscByItem.Content = string.Format("Discount: -{0}", (discounttotalByItem).ToString("C"));
            //if (autocompltCustName.autoTextBox.Text == "Card")
            //{
            //    receivedCash.Clear();
            //    receivedCard.Text = Math.Round((totalTaxableValues - oldtotalVal), 0).ToString();
            //}
            //if (autocompltCustName.autoTextBox.Text == "Cash")
            //{
            //    receivedCard.Clear();
            //    receivedCash.Text = Math.Round((totalTaxableValues - oldtotalVal), 0).ToString();
            //}

            double cashreceived = (receivedCash.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCash.Text.Trim());
            double cardreceived = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());
            double paytmreceived = (receivedPaytm.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedPaytm.Text.Trim());
            double flatoff = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());

            double offerzone = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
            double loyaltycard = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());



            if (autocompltCustName.autoTextBox.Text == "Card")
            {
                if (cashreceived < 1)
                {
                    //receivedCash.Clear();
                    //receivedCard.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
                    receivedCard.Text = Math.Round((totalVal - oldtotalVal - flatoff), 0).ToString();// Changed and applied Flatoff after - Entry of flat off amount in Balance
                }
            }
            if (autocompltCustName.autoTextBox.Text == "Cash")
            {
                if (cardreceived < 1)
                {
                    //receivedCard.Clear();
                    //receivedCash.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
                    receivedCash.Text = Math.Round((totalVal - oldtotalVal - flatoff), 0).ToString(); // Changed and applied Flatoff after - Entry of flat off amount in Balance
                }
            }

            cashreceived = (receivedCash.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCash.Text.Trim());
            cardreceived = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());

            dueBal.Content = string.Format("Balance:  {0}", Math.Round((totalVal - oldtotalVal - (cashreceived + cardreceived + paytmreceived + flatoff + offerzone + loyaltycard)), 0)).ToString();


        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            CleanUp();
            totalVal = 0;
            totalTaxableValues = 0;
            totalTaxAmount = 0;
            oldtotalVal = 0;
            discounttotalByItem = 0;
            discounttotalCommon = 0;
            invoiceNumber.Text = InvoiceNumber.ToString();
            VoucherNumber.Text = voucherNumber.ToString();
        }

        //this method will clear/reset form values
        private void CleanUp()
        {
            totalVal = 0.0;
            //totalValBeforeItemDis = 0.0;
            oldtotalVal = 0.0;
            totalTaxAmount = 0.0;
            SaleAcctName = "";
            discounttotalCommon = 0.0;
            discounttotalByItem = 0.0;
            labourTotal = 0.0;
            makingTotalCharge = 0.0;
            totalInvValues = 0.0;
            totalTaxableValues = 0.0;
            totalSGSTTax = 0.0;
            totalCGSTTax = 0.0;
            totalIGSTTax = 0.0;
            totalQuanty = 0.0;
            totalPaid = 0.0;
            PackCharge.Text = "";
            GSTTaxVa.Text = ""; 
            txtCTN.Clear();
            txtPrice.Clear();
            txtQty.Text = "0";
            autocompltCustName.autoTextBox.Clear();
            CashCustName.Clear();
            EwayNumbertxt.Clear();
            //VoucherNumber.Clear();
            invDate.SelectedDate = DateTime.Now;
            receivedCash.Clear();
            receivedCard.Clear();
            flatOff.Clear();
            receivedOffer.Clear();
            receivedLoyalty.Clear();
            receivedPaytm.Clear();

            //shopping cart = a new empty list
            ShoppingCart = new List<Product>();
            OldCart = new List<Product>();
            //Textboxes and labels are set to defaults
            autocompleteItemName.autoTextBox1.Text = string.Empty;
            autocompleteItemName.autoTextBox1.Text = string.Empty;
            txtQty.Text = string.Empty;
            lbTotal.Content = "Total: ₹ 0.00";
            //lbOldTotal.Content = "Total: ₹ 0.00";
            lbGrandTotal.Content = "Total: ₹ 0.00";
            lbGrandSum.Content = "Grand SUM ₹ 0.00";
            //DataGrid items are set to null
            CartGrid.ItemsSource = null;
            //OldGoldGrid.ItemsSource = null;
            CartGrid.Items.Refresh();
            //Tmp variable is erased using null
            tmpProduct = null;

        }

        private void CartGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }


        //fires on Grid item click (Button delete)
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            //We ask the user if really wants to delete the item
            if (
                MessageBox.Show("Are you sure you want to remove this product from Cart", "Confirmation",
                    MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                var row = GetParent<DataGridRow>((Button)sender);
                var index = CartGrid.Items.IndexOf(row.Item);
                if (ShoppingCart.Count > index)
                {
                    MessageBoxResult result = MessageBox.Show("Are you sure want to delete?", "Delete Record", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                        ShoppingCart.RemoveAt(index);
                }



                ////if Result is OK we get the Button that was click
                ////Button deleteButton = (Button)sender;
                //////We get the record id binded using the commandParameter attribute {Binding Id}
                ////int id = (int)deleteButton.CommandParameter;
                //var rowl = CartGrid.SelectedItem.GetType().GetProperties();
                ////DataRowView row = (DataRowView)CartGrid.SelectedItems[0];
                ////string ItemName = row["ItemName"].ToString();
                ////string customerName = row["PartyName"].ToString();

                ////Remove the product from the Array
                //ShoppingCart.RemoveAll(s => s.ItemName == "Jabbar");

                //OldCart.RemoveAll(s => s.ItemName == "Jabbar");
                //Update the DataGrid
                BindDataGrid();
            }
        }


        //fires on Grid item click (Button delete)
        //private void OldButtonBase_OnClick(object sender, RoutedEventArgs e)
        //{
        //    //We ask the user if really wants to delete the item
        //    if (
        //        MessageBox.Show("Are you sure you want to remove this product from Cart", "Confirmation",
        //            MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
        //    {
        //        var row = GetParent<DataGridRow>((Button)sender);
        //        var index = OldGoldGrid.Items.IndexOf(row.Item);
        //        if (OldCart.Count > index)
        //        {
        //            MessageBoxResult result = MessageBox.Show("Are you sure want to delete?", "Delete Record", MessageBoxButton.YesNo);
        //            if (result == MessageBoxResult.Yes)
        //                OldCart.RemoveAt(index);
        //        }
        //        BindDataOldGridGrid();
        //        //BindDataGrid();
        //    }
        //}
        private TargetType GetParent<TargetType>(DependencyObject o) where TargetType : DependencyObject
        {
            if (o == null || o is TargetType) return (TargetType)o;
            return GetParent<TargetType>(VisualTreeHelper.GetParent(o));
        }

        private void Window_KeyDown_Invoice(object sender, KeyEventArgs e)
        {
            //e.Handled = true;

            if ((e.Key == Key.Enter) || (e.Key == Key.Tab))
            {
                e.Handled = true;
                //Get Invoice Details to view in the Screen
            }

            //// below for Shift Tab Backward/reversal 
            //if (e.Key == Key.Tab && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
            //{
            //    var btn = e.OriginalSource as TextBox;


            //    e.Handled = true;
            //}
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

        private void PrintInvBtn_GotFocus(object sender, RoutedEventArgs e)
        {
            var btn = e.OriginalSource as Button;
            btn.Background = Brushes.BlueViolet;
            btn.Foreground = Brushes.White;
        }

        private void PrintInvBtn_LostFocus(object sender, RoutedEventArgs e)
        {
            var btn = e.OriginalSource as Button;
            btn.Background = Brushes.Orange;
            btn.Foreground = Brushes.Black;
        }



        private void WindowFlatOff_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = true;
                return;
            }

            if (e.Key == Key.Enter)
            {
                PrintInvBtn.Focus();
                //this.PrintInvBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                //TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                //UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                //if (keyboardFocus != null)
                //{
                //    keyboardFocus.MoveFocus(tRequest);
                //}

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



        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            //If a product code is not empty we search the database
            if (Regex.IsMatch(autocompleteItemName.autoTextBox1.Text.Trim(), @"^\d+$"))
            {
                //DBInvoiceSample db = new DBInvoiceSample();
                ////parse the product code as int from the TextBox
                //int id = int.Parse(autocompleteItemName.autoTextBox1.Text);
                ////We query the database for the product
                //Product p = db.Products.SingleOrDefault(x => x.Id == id);
                //if (p != null) //if product was found
                //{
                //    //store in a temp variable (if user clicks on add we will need this for the Array)
                //    tmpProduct = p;
                //    //We display the product information on a label 
                //    cprod.Content = string.Format("ID: {0}, Name: {1}, Price: {2}, InStock (Qty): {3}", p.Id, p.Name, p.Price, p.Qty);
                //}
                //else
                //{
                //    //if product was not found we display a user notification window
                //    MessageBox.Show("Product not found. (Only numbers allowed)", "Product code error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //}

            }
        }

        private void textBoxItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (autocompltCustName.autoTextBox.Text.Trim() == "Card")
            {
                receivedCash.Clear();
                receivedCard.Text = Math.Round((totalTaxableValues - oldtotalVal), 0).ToString();
            }
            if (autocompltCustName.autoTextBox.Text.Trim() == "Cash")
            {
                receivedCard.Clear();
                receivedCash.Text = Math.Round((totalTaxableValues - oldtotalVal), 0).ToString();
            }

            if (autocompltCustName.autoTextBox.Text.Trim() != "Cash")
            {
                CashCustName.Visibility = Visibility.Collapsed;
                //CashName.Visibility = Visibility.Collapsed;

            }

            //invoiceNumber.Text = InvoiceNumber.ToString();
            //VoucherNumber.Text = voucherNumber.ToString();
            //If a product code is not empty we search the database
            if (Regex.IsMatch(autocompleteItemName.autoTextBox1.Text.Trim(), @"^\d+$") || 1 == 1)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select * from StockItemsByPC where ItemName = '" + autocompleteItemName.autoTextBox1.Text + "' and CompID = '" + CompID + "'";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                tmpProduct = new Product();

                while (reader.Read())
                {


                    //var CustID = reader.GetValue(0).ToString();

                    tmpProduct.ItemName = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                    tmpProduct.PrintName = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                    tmpProduct.UnitID = (reader["UnitID"] != DBNull.Value) ? (reader.GetString(4)) : "Pc";
                    tmpProduct.ItemCode = (reader["ItemCode"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";

                    tmpProduct.HSN = "9503";  //HSN

                    tmpProduct.ItemDesc = (reader["ItemDesc"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                    tmpProduct.ItemBarCode = (reader["ItemBarCode"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
                    tmpProduct.ItemPrice = (reader["ItemPrice"] != DBNull.Value) ? (reader.GetDouble(9)) : 0;
                    tmpProduct.SetCriticalLevel = (reader["SetCriticalLevel"] != DBNull.Value) ? (reader.GetBoolean(12)) : false;
                    tmpProduct.SetDefaultStorageID = (reader["SetDefaultStorageID"] != DBNull.Value) ? (reader.GetInt32(14)) : 0;
                    tmpProduct.DecimalPlaces = (reader["DecimalPlaces"] != DBNull.Value) ? (reader.GetInt32(17)) : 0;
                    tmpProduct.IsBarcodeCreated = (reader["IsBarcodeCreated"] != DBNull.Value) ? (reader.GetBoolean(18)) : false;
                    tmpProduct.ItemPurchPrice = (reader["ItemPurchPrice"] != DBNull.Value) ? (reader.GetDouble(23)) : 0;
                    tmpProduct.ItemAlias = (reader["ItemAlias"] != DBNull.Value) ? (reader.GetString(30).Trim()) : "";
                    tmpProduct.UnderGroupID = (reader["UnderGroupID"] != DBNull.Value) ? (reader.GetInt64(32)) : 0;
                    tmpProduct.UnderSubGroupID = (reader["UnderSubGroupID"] != DBNull.Value) ? (reader.GetInt64(34)) : 0;
                    tmpProduct.ActualQty = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)) : 0;
                    tmpProduct.HSN = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
                    tmpProduct.GSTRate = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)) : 0;
                    tmpProduct.StorageID = (reader["StorageID"] != DBNull.Value) ? (reader.GetInt32(38)) : 0;
                    tmpProduct.TrayID = (reader["TrayID"] != DBNull.Value) ? (reader.GetInt32(39)) : 0;
                    tmpProduct.CounterID = (reader["CounterID"] != DBNull.Value) ? (reader.GetInt32(40)) : 0;
                    //tmpProduct.UpdateDate = reader.GetDateTime(44); //reader["UpdateDate"] != DBNull.Value) ? (reader.GetDateTime(44)) : "";  
                    tmpProduct.ActualWt = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)) : 0;
                    //tmpProduct.LastBuyDate = reader.GetDateTime(47); //(reader["LastBuyDate"] != DBNull.Value) ? (reader.GetDateTime(47) : "";
                    //tmpProduct.LastSaleDate = reader.GetDateTime(48);//(reader["LastSaleDate"] != DBNull.Value) ? (reader.GetDateTime(48) : "";
                    tmpProduct.LastSalePrice = (reader["LastSalePrice"] != DBNull.Value) ? (reader.GetDouble(50)) : 0;
                    tmpProduct.LastBuyPrice = (reader["LastBuyPrice"] != DBNull.Value) ? (reader.GetDouble(51)) : 0;

                    HSN.Text = tmpProduct.HSN.ToString();
                    txtPrice.Text = tmpProduct.ItemPrice.ToString();
                    //cmbUnits.Text = tmpProduct.UnitID.ToString();
                    cmbUnits.Text = (tmpProduct.UnitID.ToString() != "") ? tmpProduct.UnitID.ToString() : "Pc";
                    //txtGSTRate.Text = tmpProduct.GSTRate.ToString();
                    autocompleteItemName.autoTextBox1.Text = tmpProduct.ItemBarCode.ToString();
                    //Get Counter , Tray and Storage Name by another call, get all count by sp or direct call for inventory 

                    BindStorageComboBox(tmpProduct.ItemName);
                }

                reader.Close();
            }
        }


        private void textBoxCustName_TextChanged(object sender, TextChangedEventArgs e)
        {


            string GSTINAcct = "";
            string GSTINCompany = "";
            if (autocompltCustName.autoTextBox.Text.Trim() != "Cash")
            {
                CashCustName.Clear();
                CashCustName.Visibility = Visibility.Collapsed;
                //CashName.Visibility = Visibility.Collapsed;

            }
            else
            {
                //CashCustName.Text = "Customer Name";
                //CashName.Visibility = Visibility.Visible;
                CashCustName.Visibility = Visibility.Visible;
            }

            //invoiceNumber.Text = InvoiceNumber.ToString();
            //VoucherNumber.Text = voucherNumber.ToString();
            //If a product code is not empty we search the database
            if (Regex.IsMatch(autocompltCustName.autoTextBox.Text.Trim(), @"^\d+$") || 1 == 1)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select AcctName,GSTIN,* from AccountsList where LTRIM(RTRIM(AcctName)) = '" + autocompltCustName.autoTextBox.Text + "' and CompID = '" + CompID + "'";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                tmpProduct = new Product();

                while (reader.Read())
                {


                    //var CustID = reader.GetValue(0).ToString();

                    //tmpProduct.ItemName = (reader["AcctName"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "";
                    GSTINAcct = (reader["GSTIN"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "";

                }
                reader.Close();
            }

            SqlConnection conCmp = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conCmp.Open();
            string sqlCmp = "select top 1  CompanyName,GSTIN,* from Company where   CompanyID = '" + CompID + "'";
            SqlCommand cmdCmp = new SqlCommand(sqlCmp);
            cmdCmp.Connection = conCmp;
            SqlDataReader readerCmp = cmdCmp.ExecuteReader();

            while (readerCmp.Read())
            {


                //var CustID = reader.GetValue(0).ToString();

                //tmpProduct.ItemName = (reader["AcctName"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "";
                GSTINCompany = (readerCmp["GSTIN"] != DBNull.Value) ? (readerCmp.GetString(1).Trim()) : "";

            }
            readerCmp.Close();

            if (GSTINAcct != "")
            {
                GSTINAcct = GSTINAcct.Substring(0, 2);
            }
            GSTINCompany = GSTINCompany.Substring(0, 2);
            if (GSTINAcct != GSTINCompany)
            {
                IState = false;
                stateCodeVal = GSTINAcct;
            }
            else
                IState = true;


        }

        private void textBoxInvoiceNumber_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        public void BindStorageComboBox(string comboBoxName)
        {
            //var custAdpt = new StockItemsStorageWiseTableAdapter();
            //var custInfoVal = custAdpt.GetData();
            ////var LinqRes = from UserRec in custInfoVal
            ////              select UserRec.CustomerName;
            //var LinqRes = (from UserRec in custInfoVal
            //               orderby UserRec.StorageName ascending
            //               select (UserRec.StorageName).Distinct().ToList());
            //cmbStorage.ItemsSource = LinqRes;



        }

        private void TxtProdCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            //If a product code is not empty we search the database
            //if (Regex.IsMatch(autocompleteItemName.autoTextBox1.Text.Trim(), @"^\d+$") || 1==1)
            //{
            //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            //    con.Open();
            //    string sql = "select * from StockItems where ItemName = '" + autocompleteItemName.autoTextBox1.Text + "'";
            //    SqlCommand cmd = new SqlCommand(sql);
            //    cmd.Connection = con;
            //    SqlDataReader reader = cmd.ExecuteReader();

            //    tmpProduct = new Product();

            //    while (reader.Read())
            //    {
            //        //var CustID = reader.GetValue(0).ToString();

            //        tmpProduct.ItemName = reader.GetString(2);

            //        tmpProduct.ItemPrice = 5;
            //        tmpProduct.ActualQty = 15;

            //    }
            //    reader.Close();



            //DBInvoiceSample db = new DBInvoiceSample();
            ////parse the product code as int from the TextBox
            //int id = int.Parse(autocompleteItemName.autoTextBox1.Text);
            ////We query the database for the product
            //Product p = db.Products.SingleOrDefault(x => x.Id == id);
            //if (p != null) //if product was found
            //{
            //    //store in a temp variable (if user clicks on add we will need this for the Array)
            //    tmpProduct = p;
            //    //We display the product information on a label 
            //    cprod.Content = string.Format("ID: {0}, Name: {1}, Price: {2}, InStock (Qty): {3}", p.Id, p.Name, p.Price, p.Qty);
            //}
            //else
            //{
            //    //if product was not found we display a user notification window
            //    MessageBox.Show("Product not found. (Only numbers allowed)", "Product code error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            //}
            //}

        }

        /*
         * There will be 2 account in sales 1 Cash Sales  2 Credit Sales
         * 
         * */

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            //////Direct send pdf to Printer from the saved pdf location.
            ////ProcessStartInfo info = new ProcessStartInfo();
            ////info.Verb = "print";
            ////info.FileName=@"C:\output.pdf";
            ////info.CreateNoWindow = true;
            ////info.WindowStyle = ProcessWindowStyle.Hidden;

            ////Process p = new Process();
            ////p.StartInfo=info;
            ////p.Start();
            ////p.WaitForInputIdle();
            ////System.Threading.Thread.Sleep(10000);
            ////if (false == p.CloseMainWindow())
            ////{
            ////    p.Kill();
            ////}


            //
            //




            /*Write code to save the sale voucher details
             * impacted tables are below
             * SalesVouchers(Not required), 
          
             * Accounts Tables(AccountsList, SundryDebtorsAccountsLedgers, Cash,PayTM,CGST,SGST,IGST, GSTR1Table,HSNTable,Discount, Packing, RoundOff,TransportDetails , BankAccountsLedgers, CashFlow, DraftVouchers,DutyAndTaxesAccountsLedgers, ErrorLogs,POSVouchers,SalesAccountsLedgers
             * Inventory Tables  StockItems,SalesVoucherInventory,StockItemsCounterWise,StockItemsHistory,StockItemsStorageWise,StockItemsTrayWise, StorageLocations, 
             * Taxes Tables
             *  on succeessful saved -->AutoIncrement VoucherNumber also
             *  
             * */


            //Bill is already generated and saved and user click againt then delete all existing data and add new , but for stock items do reverse process 
            try
            {
                string BillDateInv = invDate.SelectedDate.ToString();

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


                //Reset SalesVoucherInventory
                SqlConnection myConnSVEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                myConnSVEntryStr.Open();
                string CountSVEntryStr = "SELECT COUNT(*) From SalesVoucherInventory where InvoiceNumber='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "'";
                // string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                SqlCommand myCommandDel = new SqlCommand(CountSVEntryStr, myConnSVEntryStr);
                myCommandDel.Connection = myConnSVEntryStr;

                //int countRec = myCommand.ExecuteNonQuery();
                int countRecDelDel = (int)myCommandDel.ExecuteScalar();
                myCommandDel.Connection.Close();
                if (countRecDelDel != 0)
                {
                    // MessageBox.Show("Item Name is already Exist, Please delete existing", "Add Record");


                    SqlCommand myCommandDeleteDel = new SqlCommand("SPUpdateStockOnSalesVoucherChangeOrDeleteEstimationsQty", myConnSVEntryStr);
                    myCommandDeleteDel.CommandType = CommandType.StoredProcedure;
                    myCommandDeleteDel.Parameters.Add(new SqlParameter("@VoucherNumber", Convert.ToInt64(VoucherNumber.Text.Trim())));
                    myCommandDeleteDel.Parameters.Add(new SqlParameter("@InvoiceNumber", invoiceNumber.Text.Trim()));
                    myCommandDeleteDel.Parameters.Add(new SqlParameter("@CompID", CompID));
                    myCommandDeleteDel.Connection.Open();
                    int countRecDelDelDel = myCommandDeleteDel.ExecuteNonQuery();
                    if (countRecDelDelDel != 0)
                    {
                        //  MessageBox.Show("Record Successfully Deleted....", "Delete Record");
                    }


                    //string DeleteExisting = "DELETE From SalesVoucherInventory where  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                    ////string DeleteExisting = "DELETE From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                    //SqlCommand myCommandDeleteDel = new SqlCommand(DeleteExisting, myConnSVEntryStr);
                    //myCommandDeleteDel.Connection.Open();
                    //int countRecDelDelDel = (int)myCommandDeleteDel.ExecuteNonQuery();
                    //if (countRecDelDelDel != 0)
                    //{
                    //    // MessageBox.Show("Deleted", "Add Record");
                    //}
                    myCommandDeleteDel.Connection.Close();
                }
                //myCommandDel.Connection.Close();




                IEnumerable itemsSource = CartGrid.ItemsSource as IEnumerable;

                for (int k = 0; k < CartGrid.Items.Count; ++k)
                {
                    DataGridRow row = CartGrid.ItemContainerGenerator.ContainerFromItem(itemsSource) as DataGridRow;

                    row = CartGrid.ItemContainerGenerator.ContainerFromItem(itemsSource) as DataGridRow;

                    if (row == null)
                    {
                        CartGrid.UpdateLayout();
                        CartGrid.ScrollIntoView(CartGrid.Items[k]);
                        row = (DataGridRow)CartGrid.ItemContainerGenerator.ContainerFromIndex(k);
                    }

                    if (row != null)
                    {
                        DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(row);

                        //============
                        if (presenter == null)
                        {

                            CartGrid.UpdateLayout();
                            CartGrid.ScrollIntoView(CartGrid.Items[k]);
                            row = (DataGridRow)CartGrid.ItemContainerGenerator.ContainerFromIndex(k);
                            DataGridCellsPresenter prsnter = FindVisualChild<DataGridCellsPresenter>(row);
                            presenter = prsnter;
                        }
                        //============
                        // FOR iTEMnAME 2
                        DataGridCell cellItemName = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(1);
                        //TextBlock txtItemNam = cellItemName.Content as TextBlock;
                        TextBlock txtItemNam = cellItemName.Content as TextBlock;

                        //DataGridCell cellHSN = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(2);
                        //TextBlock hsnText = cellHSN.Content as TextBlock;


                        // for Qty

                        DataGridCell cellCTN = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(2);
                        TextBlock qtyCTN = cellCTN.Content as TextBlock;


                        DataGridCell cellQty = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(3);
                        TextBlock qtyText = cellQty.Content as TextBlock;

                        DataGridCell cellUnitID = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(4);
                        TextBlock txtcellUnitID = cellUnitID.Content as TextBlock;

                        DataGridCell cellPrice = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(5);
                        TextBlock priceText = cellPrice.Content as TextBlock;


                        DataGridCell cellAmount = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(6);
                        TextBlock txtCellAmount = cellAmount.Content as TextBlock;


                        DataGridCell discRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(7);
                        TextBlock txtdiscRate = discRate.Content as TextBlock;

                        DataGridCell cellTaxableAmt = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(8);
                        TextBlock txtTaxableAmt = cellTaxableAmt.Content as TextBlock;

                        //DataGridCell gstRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(8);
                        //TextBlock txtgstRate = gstRate.Content as TextBlock;

                        //DataGridCell gstTax = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(9);
                        //TextBlock txtgsTax = gstTax.Content as TextBlock;


                        //DataGridCell cellTotal = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(10);
                        //TextBlock totalText = cellTotal.Content as TextBlock;


                        //DataGridCell cellStoreID = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(11);
                        //TextBlock txtcellStoreID = cellStoreID.Content as TextBlock;

                        //DataGridCell cellCounterID = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(11);
                        //TextBlock txtcellCounterID = cellCounterID.Content as TextBlock;

                        //DataGridCell cellTrayID = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(11);
                        //TextBlock txtcellTrayID = cellTrayID.Content as TextBlock;

                        //Get Voucher Number




                        //Insert into SalesInventory 
                        SqlConnection myConSVInventoryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                        myConSVInventoryStr.Open();


                        string querySalesInventory = "";
                        querySalesInventory = "insert into SalesVoucherInventory(VoucherNumber, InvoiceNumber,ItemName,HSN,SalePrice,GSTRate,GSTTax,Discount,TaxablelAmount,TotalAmount, BilledQty,TransactionDate,FromConsumedStorageID,FromConsumedTrayID,FromConsumedCounterID,CompID,Amount,UnitID,Itemdesc) Values ( '" + VoucherNumber.Text + "','" + invoiceNumber.Text.Trim() + "','" + txtItemNam.Text + "','','" + priceText.Text + "','','','" + txtdiscRate.Text + "', '" + txtTaxableAmt.Text + "','" + txtTaxableAmt.Text + "','" + qtyText.Text + "', '" + InvdateValue + "','1','1','1', '" + CompID + "','" + txtCellAmount.Text + "','" + txtcellUnitID.Text + "','" + qtyCTN.Text.Trim() + "')";



                        SqlCommand myCommandSVInventory = new SqlCommand(querySalesInventory, myConSVInventoryStr);
                        myCommandSVInventory.Connection = myConSVInventoryStr;
                        //myCommandInvEntry.Connection.Open();
                        int NumPI = myCommandSVInventory.ExecuteNonQuery();
                        myCommandSVInventory.Connection.Close();


                        //StockItems: CRUD Start
                        if ((txtItemNam != null) && (priceText != null))
                        {
                            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                            SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                            myConnSalesInvEntryStr.Open();
                            string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPc where ItemName ='" + txtItemNam.Text.Trim() + "'  and CompID = '" + CompID + "' ";
                            //string CountSalesInvEntryStr = "SELECT COUNT(*) From StockItems where ItemName ='" + autocompleteItemName.autoTextBox1.Text + "' and  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                            //// string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                            SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
                            myCommand.Connection = myConnSalesInvEntryStr;

                            //int countRec = myCommand.ExecuteNonQuery();
                            int countRec = (int)myCommand.ExecuteScalar();
                            myCommand.Connection.Close();


                            if (countRec != 0)
                            {

                                string queryStrStockCheck = "";

                                string balanceStk = "";
                                string balanceStkWt = "";

                                // write code to update stocktable directly 
                                queryStrStockCheck = "select * from StockItemsByPc where ItemName = '" + txtItemNam.Text.Trim() + "' and CompID = '" + CompID + "'";
                                //OleDbCommand command = new OleDbCommand(queryStr, con);
                                // myConnStock.Open();
                                SqlCommand myCommandStkCheck = new SqlCommand(queryStrStockCheck, myConnSalesInvEntryStr);
                                myCommandStkCheck.Connection.Open();
                                SqlDataReader reader = myCommandStkCheck.ExecuteReader();



                                while (reader.Read())
                                {
                                    // var CustID = reader.GetValue(0).ToString();
                                    string ItemName = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                                    string PrintName = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                                    double invQty = (qtyText.Text != "") ? (Convert.ToDouble(qtyText.Text)) : 0;
                                    double actualQty = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)) : 0;
                                    //double invWt = (qtyWt.Text != "") ? (Convert.ToDouble(qtyWt.Text)) : 0;
                                    double actualWt = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)) : 0;
                                    //if (ItemName == "Old Gold" || ItemName == "Old Silver")
                                    //{
                                    //    balanceStk = Math.Round((actualQty + invQty), 2).ToString();
                                    //    balanceStkWt = Math.Round((actualWt + invWt), 2).ToString();
                                    //}
                                    //else
                                    //{
                                    balanceStk = Math.Round((actualQty - invQty), 2).ToString();
                                    //balanceStkWt = Math.Round((actualWt - invWt), 2).ToString();
                                    //}

                                }
                                reader.Close();
                                myCommandStkCheck.Connection.Close();

                                string queryStrStockUpdate = "";
                                queryStrStockUpdate = "update StockItemsByPc  set UpdateDate='" + InvdateValue + "', IsSoldFlag='1', UnitID='" + txtcellUnitID.Text + "'  ,ActualQty='" + balanceStk + "',ActualWt='" + balanceStkWt + "',LastSalePrice='" + priceText.Text + "'  where ItemName ='" + txtItemNam.Text + "'   and CompID = '" + CompID + "' ";
                                if (txtItemNam.Text == "Old Gold" || txtItemNam.Text == "Old Silver")
                                {
                                    queryStrStockUpdate = "update StockItemsByPc  set UpdateDate='" + InvdateValue + "' , ActualQty='" + balanceStk + "',ActualWt='" + balanceStkWt + "',LastBuyPrice='" + priceText.Text + "'  where ItemName ='" + txtItemNam.Text + "'   and CompID = '" + CompID + "' ";
                                }
                                SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                                myCommandStkUpdate.Connection.Open();
                                myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                                if (txtItemNam.Text.Trim() != "")
                                {
                                    // myCommandStk.Connection.Open();
                                    int Num = myCommandStkUpdate.ExecuteNonQuery();
                                    if (Num != 0)
                                    {
                                        // MessageBox.Show("Record Successfully Updated....", "Update Record");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Stock is not Updated....", "Update Record Error");
                                    }
                                    // myCommandStk.Connection.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Stock can not be updated....", "Update Record Error");
                                }
                                myCommandStkUpdate.Connection.Close();
                            }
                            else
                            {

                                string querySalesInvEntry = "";
                                querySalesInvEntry = "insert into StockItemsByPc(ItemName, ActualQty,ActualWt,ItemPrice,GSTRate,LastSalePrice,CompID,UnitID) Values ( '" + txtItemNam.Text + "','" + 0 + "','" + 0 + "','" + priceText.Text + "','','" + priceText.Text + "', '" + CompID + "','" + txtcellUnitID.Text + "')";
                                //if (txtItemNam.Text == "Old Gold" || txtItemNam.Text == "Old Silver")
                                //{
                                //    querySalesInvEntry = "insert into StockItems(ItemName, ActualQty,ActualWt,ItemPrice,GSTRate,LastBuyPrice,CompID) Values ( '" + txtItemNam.Text + "','" + 0 + "','" + 0 + "','" + priceText.Text + "','" + txtgstRate.Text + "','" + priceText.Text + "', '" + CompID + "')";
                                //}

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

                        //    string DeleteExisting = "DELETE From SalesInventory where ItemName ='" + txtItemNam.Text + "' and GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                        //    //string DeleteExisting = "DELETE From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                        //    SqlCommand myCommandDelete = new SqlCommand(DeleteExisting, myConnSalesInvEntryStr);
                        //    myCommandDelete.Connection.Open();
                        //    int countRecDel = (int)myCommandDelete.ExecuteNonQuery();
                        //    if (countRecDel != 0)
                        //    {
                        //        // MessageBox.Show("Deleted", "Add Record");
                        //    }
                        //    myCommandDelete.Connection.Close();

                        //}




                    }
                }



                //IEnumerable itemsSourceOld = OldGoldGrid.ItemsSource as IEnumerable;

                //for (int k = 0; k < OldGoldGrid.Items.Count; ++k)
                //{
                //    DataGridRow row = OldGoldGrid.ItemContainerGenerator.ContainerFromItem(itemsSourceOld) as DataGridRow;

                //    row = OldGoldGrid.ItemContainerGenerator.ContainerFromItem(itemsSourceOld) as DataGridRow;

                //    if (row == null)
                //    {
                //        OldGoldGrid.UpdateLayout();
                //        OldGoldGrid.ScrollIntoView(OldGoldGrid.Items[k]);
                //        row = (DataGridRow)OldGoldGrid.ItemContainerGenerator.ContainerFromIndex(k);
                //    }

                //    if (row != null)
                //    {
                //        DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(row);

                //        //============
                //        if (presenter == null)
                //        {

                //            OldGoldGrid.UpdateLayout();
                //            OldGoldGrid.ScrollIntoView(OldGoldGrid.Items[k]);
                //            row = (DataGridRow)OldGoldGrid.ItemContainerGenerator.ContainerFromIndex(k);
                //            DataGridCellsPresenter prsnter = FindVisualChild<DataGridCellsPresenter>(row);
                //            presenter = prsnter;
                //        }
                //        //============
                //        // FOR iTEMnAME 2
                //        DataGridCell cellItemName = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(1);
                //        //TextBlock txtItemNam = cellItemName.Content as TextBlock;
                //        TextBlock txtItemNam = cellItemName.Content as TextBlock;
                //        // for Qty
                //        DataGridCell cellQty = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(2);
                //        TextBlock qtyText = cellQty.Content as TextBlock;

                //        DataGridCell cellQtyWt = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(3);
                //        TextBlock qtyWt = cellQtyWt.Content as TextBlock;

                //        DataGridCell cellHSN = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(2);
                //        TextBlock hsnText = cellHSN.Content as TextBlock;

                //        DataGridCell cellUnit = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(3);
                //        ComboBox unitText = cellUnit.Content as ComboBox;

                //        DataGridCell cellPrice = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(7);
                //        TextBlock priceText = cellPrice.Content as TextBlock;

                //        DataGridCell cellTotal = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(7);
                //        TextBlock totalText = cellTotal.Content as TextBlock;

                //        DataGridCell gstRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(11);
                //        TextBlock txtgstRate = gstRate.Content as TextBlock;

                //        DataGridCell cellsgstRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(9);
                //        TextBlock txtsgstRate = cellsgstRate.Content as TextBlock;



                //        DataGridCell cellIgstRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(9);
                //        TextBlock txtIgstRate = cellIgstRate.Content as TextBlock;

                //        DataGridCell cellStock = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(10);
                //        TextBlock txtStock = cellStock.Content as TextBlock;


                //        //StockItems: CRUD Start
                //        if ((txtItemNam != null) && (priceText != null))
                //        {
                //            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //            SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //            myConnSalesInvEntryStr.Open();
                //            string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItems where ItemName ='" + txtItemNam.Text + "' and CompID = '" + CompID + "'";
                //            //string CountSalesInvEntryStr = "SELECT COUNT(*) From StockItems where ItemName ='" + autocompleteItemName.autoTextBox1.Text + "' and  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                //            //// string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                //            SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
                //            myCommand.Connection = myConnSalesInvEntryStr;

                //            //int countRec = myCommand.ExecuteNonQuery();
                //            int countRec = (int)myCommand.ExecuteScalar();
                //            myCommand.Connection.Close();


                //            if (countRec != 0)
                //            {

                //                string queryStrStockCheck = "";

                //                string balanceStk = "";
                //                string balanceStkWt = "";

                //                // write code to update stocktable directly 
                //                queryStrStockCheck = "select * from StockItems where ItemName = '" + txtItemNam.Text + "'";
                //                //OleDbCommand command = new OleDbCommand(queryStr, con);
                //                // myConnStock.Open();
                //                SqlCommand myCommandStkCheck = new SqlCommand(queryStrStockCheck, myConnSalesInvEntryStr);
                //                myCommandStkCheck.Connection.Open();
                //                SqlDataReader reader = myCommandStkCheck.ExecuteReader();



                //                while (reader.Read())
                //                {
                //                    // var CustID = reader.GetValue(0).ToString();
                //                    string ItemName = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                //                    string PrintName = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                //                    double invQty = (qtyText.Text != "") ? (Convert.ToDouble(qtyText.Text)) : 0;
                //                    double actualQty = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)) : 0;
                //                    double invWt = (qtyWt.Text != "") ? (Convert.ToDouble(qtyWt.Text)) : 0;
                //                    double actualWt = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)) : 0;
                //                    if (ItemName == "Old Gold" || ItemName == "Old Silver")
                //                    {
                //                        balanceStk = Math.Round((actualQty + invQty), 2).ToString();
                //                        balanceStkWt = Math.Round((actualWt + invWt), 2).ToString();
                //                    }
                //                    else
                //                    {
                //                        balanceStk = Math.Round((actualQty - invQty), 2).ToString();
                //                        balanceStkWt = Math.Round((actualWt - invWt), 2).ToString();
                //                    }

                //                }
                //                reader.Close();
                //                myCommandStkCheck.Connection.Close();

                //                string queryStrStockUpdate = "";
                //                queryStrStockUpdate = "update StockItems  set UpdateDate='" + InvdateValue + "',  IsSoldFlag='1',  ActualQty='" + balanceStk + "',ActualWt='" + balanceStkWt + "',LastSalePrice='" + priceText.Text + "'  where ItemName ='" + txtItemNam.Text + "'  and CompID = '" + CompID + "' ";
                //                if (txtItemNam.Text == "Old Gold" || txtItemNam.Text == "Old Silver")
                //                {
                //                    queryStrStockUpdate = "update StockItems  set UpdateDate='" + InvdateValue + "', ActualQty='" + balanceStk + "',ActualWt='" + balanceStkWt + "',LastBuyPrice='" + priceText.Text + "'  where ItemName ='" + txtItemNam.Text + "'  and CompID = '" + CompID + "' ";
                //                }
                //                SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                //                myCommandStkUpdate.Connection.Open();
                //                myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                //                if (txtItemNam.Text.Trim() != "")
                //                {
                //                    // myCommandStk.Connection.Open();
                //                    int Num = myCommandStkUpdate.ExecuteNonQuery();
                //                    if (Num != 0)
                //                    {
                //                        // MessageBox.Show("Record Successfully Updated....", "Update Record");
                //                    }
                //                    else
                //                    {
                //                        MessageBox.Show("Stock is not Updated....", "Update Record Error");
                //                    }
                //                    // myCommandStk.Connection.Close();
                //                }
                //                else
                //                {
                //                    MessageBox.Show("Stock can not be updated....", "Update Record Error");
                //                }
                //                myCommandStkUpdate.Connection.Close();
                //            }
                //            else
                //            {

                //                string querySalesInvEntry = "";
                //                querySalesInvEntry = "insert into StockItems(ItemName, ActualQty,ActualWt,ItemPrice,GSTRate,LastSalePrice,CompID) Values ( '" + txtItemNam.Text + "','" + 0 + "','" + 0 + "','" + priceText.Text + "','" + txtgstRate.Text + "','" + priceText.Text + "' ,  '" + CompID + "')";
                //                if (txtItemNam.Text == "Old Gold" || txtItemNam.Text == "Old Silver")
                //                {
                //                    querySalesInvEntry = "insert into StockItems(ItemName, ActualQty,ActualWt,ItemPrice,GSTRate,LastBuyPrice,CompID) Values ( '" + txtItemNam.Text + "','" + 0 + "','" + 0 + "','" + priceText.Text + "','" + txtgstRate.Text + "','" + priceText.Text + "', '" + CompID + "')";
                //                }

                //                SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                //                myCommandInvEntry.Connection.Open();
                //                int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                //                if (NumPInv != 0)
                //                {
                //                    // MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                //                }
                //                else
                //                {
                //                    MessageBox.Show("Stock is not Inserted....", "Insert Record Error");
                //                }
                //                myCommandInvEntry.Connection.Close();

                //                // myConnStock.Close();

                //            }


                //        }

                //        //    string DeleteExisting = "DELETE From SalesInventory where ItemName ='" + txtItemNam.Text + "' and GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                //        //    //string DeleteExisting = "DELETE From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                //        //    SqlCommand myCommandDelete = new SqlCommand(DeleteExisting, myConnSalesInvEntryStr);
                //        //    myCommandDelete.Connection.Open();
                //        //    int countRecDel = (int)myCommandDelete.ExecuteNonQuery();
                //        //    if (countRecDel != 0)
                //        //    {
                //        //        // MessageBox.Show("Deleted", "Add Record");
                //        //    }
                //        //    myCommandDelete.Connection.Close();

                //        //}






                //StockItems End
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            SqlConnection conStrCommon = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conStrCommon.Open();
            //string sql = "SELECT COUNT(*) From AccountsList where AcctName='" + textBoxAcctName.Text.Trim() + "'";
            SqlCommand cmdCommon;//= new SqlCommand(sql, con);
            //long debitacctnumber = 0;
            //long creditacctnumber = 0;
            //string againstinvnumber = "";
            cmdCommon = new SqlCommand("[SPUpdateAccountsForSaleVoucher]", conStrCommon);
            cmdCommon.CommandType = CommandType.StoredProcedure;
            cmdCommon.Parameters.Add(new SqlParameter("@SundryDebtorName", autocompltCustName.autoTextBox.Text));
            cmdCommon.Parameters.Add(new SqlParameter("@SalesAcctName", SaleAcctName));
            cmdCommon.Parameters.Add(new SqlParameter("@IsNewSundryDebtor", "No"));
            if (CashCustName.Text.Trim() != "")
            {
                cmdCommon.Parameters.Add(new SqlParameter("@CashCustomerName", CashCustName.Text));
                cmdCommon.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Cash"));
            }
            else
            {
                cmdCommon.Parameters.Add(new SqlParameter("@CashCustomerName", ""));
                cmdCommon.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Credit"));
            }
            cmdCommon.Parameters.Add(new SqlParameter("@InvoiceNumber", invoiceNumber.Text));
            cmdCommon.Parameters.Add(new SqlParameter("@SaleVoucherNumber", Convert.ToInt64(VoucherNumber.Text.Trim())));
            cmdCommon.Parameters.Add(new SqlParameter("@SaleVoucherType", "Sale Voucher"));
            cmdCommon.Parameters.Add(new SqlParameter("@EwayNumber", EwayNumbertxt.Text));

            string BillDateInvVal = invDate.SelectedDate.ToString();

            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dtinval = Convert.ToDateTime(BillDateInvVal);
            //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            int yearsinval = dtinval.Year;
            string monthsinval = dtinval.Month.ToString();
            if (dtinval.Month < 10)
            {
                monthsinval = "0" + monthsinval;
            }
            string daysinval = dtinval.Day.ToString();
            if (dtinval.Day < 10)
            {
                daysinval = "0" + daysinval;
            }

            string BillDateInvValval = yearsinval + "/" + monthsinval + "/" + daysinval;


            cmdCommon.Parameters.Add(new SqlParameter("@InvDate", BillDateInvValval));

            //check isState or central with company statecode            
            cmdCommon.Parameters.Add(new SqlParameter("@IsState", IState.ToString()));
            discounttotalCommon = (discountTxt.Text.Trim() == "") ? 0 : Convert.ToDouble(discountTxt.Text.Trim());
            cmdCommon.Parameters.Add(new SqlParameter("@Discount", discounttotalCommon)); //gettotal Discount-Common 
            if (IState)
            {
                double outputigstval = 0.0;
                cmdCommon.Parameters.Add(new SqlParameter("@OutputCGST", totalTaxAmount / 2));
                cmdCommon.Parameters.Add(new SqlParameter("@OutputSGST", totalTaxAmount / 2));
                cmdCommon.Parameters.Add(new SqlParameter("@OutputIGST", outputigstval));
            }
            else
            {
                double outputsgstval = 0.0;


                cmdCommon.Parameters.Add(new SqlParameter("@OutputCGST", outputsgstval));
                cmdCommon.Parameters.Add(new SqlParameter("@OutputSGST", outputsgstval));
                cmdCommon.Parameters.Add(new SqlParameter("@OutputIGST", totalTaxAmount));
            }
            // Get all common details on global var and pas to sp
            //receivedOffer = (receivedOffer.Text.Trim() == "") ? 0: Convert.ToDouble(receivedOffer.Text);
            //discounttotalCommon = Convert.ToDouble(receivedLoyalty.Text);
            //discounttotalCommon = Convert.ToDouble(receivedPaytm.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCash.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCard.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCard.Text);
            BalanceCRorDR = Convert.ToDouble(((dueBal.Content.ToString()).Replace("₹", "").Split(':')[1]).Trim());

            double cashreceived = (receivedCash.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCash.Text.Trim());
            double cardreceived = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());
            double paytmreceived = (receivedPaytm.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedPaytm.Text.Trim());
            double flatoff = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());
            double txtAdvAmt = (txtAdvanceAmt.Text.Trim() == "") ? 0 : Convert.ToDouble(txtAdvanceAmt.Text.Trim());
            double RoundOff = (txtRoundOff.Text.Trim() == "") ? 0 : Convert.ToDouble(txtRoundOff.Text.Trim());
            double txtPackForwd = (PackCharge.Text.Trim() == "") ? 0 : Convert.ToDouble(PackCharge.Text.Trim());
            int totalParcl = (totalParcel.Text.Trim() == "") ? 0 : Convert.ToInt32(totalParcel.Text.Trim());
            double offerzone = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
            double loyaltycard = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());

            double zeroValval = 0.0;

            cmdCommon.Parameters.Add(new SqlParameter("@Labour", labourTotal));
            cmdCommon.Parameters.Add(new SqlParameter("@MakingCharges", makingTotalCharge));
            cmdCommon.Parameters.Add(new SqlParameter("@TotalInvValue", totalInvValues - oldtotalVal));
            cmdCommon.Parameters.Add(new SqlParameter("@TotalTaxableValue", totalTaxableValues));
            cmdCommon.Parameters.Add(new SqlParameter("@TotalQuantities", totalQuanty));
            cmdCommon.Parameters.Add(new SqlParameter("@OfferAmount", offerzone));
            cmdCommon.Parameters.Add(new SqlParameter("@LoyaltyCard", loyaltycard));
            cmdCommon.Parameters.Add(new SqlParameter("@TotalPaidAmt", totalInvValues - oldtotalVal));
            cmdCommon.Parameters.Add(new SqlParameter("@PayModeGateway", ""));
            cmdCommon.Parameters.Add(new SqlParameter("@PaidCardSwipe", cardreceived));
            cmdCommon.Parameters.Add(new SqlParameter("@PaidCash", cashreceived));
            cmdCommon.Parameters.Add(new SqlParameter("@PaidChequeBank", zeroValval));
            cmdCommon.Parameters.Add(new SqlParameter("@PaidOtherGateway", paytmreceived));
            cmdCommon.Parameters.Add(new SqlParameter("@PaidOnlineBank", zeroValval));
            cmdCommon.Parameters.Add(new SqlParameter("@FlatOffTM", flatoff));
            cmdCommon.Parameters.Add(new SqlParameter("@RoundOff", RoundOff));
            cmdCommon.Parameters.Add(new SqlParameter("@AdvanceAmt", txtAdvAmt));
            cmdCommon.Parameters.Add(new SqlParameter("@BalanceCRorDR", BalanceCRorDR));
            cmdCommon.Parameters.Add(new SqlParameter("@TotalParcels", totalParcl));
            cmdCommon.Parameters.Add(new SqlParameter("@PackingAndForwarding", txtPackForwd));
            cmdCommon.Parameters.Add(new SqlParameter("@Freight", zeroValval));
            cmdCommon.Parameters.Add(new SqlParameter("@IsDraftVoucher", "false"));
            cmdCommon.Parameters.Add(new SqlParameter("@DispatchedThrough", dispatchedThrough.Text));
            cmdCommon.Parameters.Add(new SqlParameter("@TransportNameOrID", transportName.Text));
            cmdCommon.Parameters.Add(new SqlParameter("@CompID", Convert.ToInt32(CompID)));

            //cmdCommon.Connection.Open();
            cmdCommon.ExecuteNonQuery();
            cmdCommon.Connection.Close();


            //Extra Voucher Details Entry



            SqlConnection conStrCommonExtra = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conStrCommonExtra.Open();
            //string sql = "SELECT COUNT(*) From AccountsList where AcctName='" + textBoxAcctName.Text.Trim() + "'";
            SqlCommand cmdCommonExtra;//= new SqlCommand(sql, con);
            //long debitacctnumber = 0;
            //long creditacctnumber = 0;
            //string againstinvnumber = "";
            cmdCommonExtra = new SqlCommand("[SPUpdateAccountsForSaleVoucherExtra]", conStrCommonExtra);
            cmdCommonExtra.CommandType = CommandType.StoredProcedure;
            cmdCommonExtra.Parameters.Add(new SqlParameter("@SundryDebtorName", autocompltCustName.autoTextBox.Text));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@SalesAcctName", SaleAcctName));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@IsNewSundryDebtor", "No"));
            if (CashCustName.Text.Trim() != "")
            {
                cmdCommonExtra.Parameters.Add(new SqlParameter("@CashCustomerName", CashCustName.Text));
                cmdCommonExtra.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Cash"));
            }
            else
            {
                cmdCommonExtra.Parameters.Add(new SqlParameter("@CashCustomerName", ""));
                cmdCommonExtra.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Credit"));
            }
            cmdCommonExtra.Parameters.Add(new SqlParameter("@InvoiceNumber", invoiceNumber.Text));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@SaleVoucherNumber", Convert.ToInt64(VoucherNumber.Text.Trim())));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@SaleVoucherType", "Sale Voucher"));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@EwayNumber", EwayNumbertxt.Text));

            string BillDateInvValExtra = invDate.SelectedDate.ToString();

            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dtinvalExtra = Convert.ToDateTime(BillDateInvValExtra);
            //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            int yearsinvalExtra = dtinvalExtra.Year;
            string monthsinvalExtra = dtinvalExtra.Month.ToString();
            if (dtinvalExtra.Month < 10)
            {
                monthsinvalExtra = "0" + monthsinvalExtra;
            }
            string daysinvalExtra = dtinvalExtra.Day.ToString();
            if (dtinvalExtra.Day < 10)
            {
                daysinvalExtra = "0" + daysinvalExtra;
            }

            string BillDateInvValvalExtra = yearsinvalExtra + "/" + monthsinvalExtra + "/" + daysinvalExtra;


            cmdCommonExtra.Parameters.Add(new SqlParameter("@InvDate", BillDateInvValvalExtra));

            //check isState or central with company statecode            
            cmdCommonExtra.Parameters.Add(new SqlParameter("@IsState", IState.ToString()));
            discounttotalCommon = (discountTxt.Text.Trim() == "") ? 0 : Convert.ToDouble(discountTxt.Text.Trim());
            cmdCommonExtra.Parameters.Add(new SqlParameter("@Discount", discounttotalCommon)); //gettotal Discount-Common 
            if (IState)
            {
                double outputigstvalExtra = 0.0;
                cmdCommonExtra.Parameters.Add(new SqlParameter("@OutputCGST", totalTaxAmount / 2));
                cmdCommonExtra.Parameters.Add(new SqlParameter("@OutputSGST", totalTaxAmount / 2));
                cmdCommonExtra.Parameters.Add(new SqlParameter("@OutputIGST", outputigstvalExtra));
            }
            else
            {
                double outputsgstval = 0.0;


                cmdCommonExtra.Parameters.Add(new SqlParameter("@OutputCGST", outputsgstval));
                cmdCommonExtra.Parameters.Add(new SqlParameter("@OutputSGST", outputsgstval));
                cmdCommonExtra.Parameters.Add(new SqlParameter("@OutputIGST", totalTaxAmount));
            }
            // Get all common details on global var and pas to sp
            //receivedOffer = (receivedOffer.Text.Trim() == "") ? 0: Convert.ToDouble(receivedOffer.Text);
            //discounttotalCommon = Convert.ToDouble(receivedLoyalty.Text);
            //discounttotalCommon = Convert.ToDouble(receivedPaytm.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCash.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCard.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCard.Text);
            BalanceCRorDR = Convert.ToDouble(((dueBal.Content.ToString()).Replace("₹", "").Split(':')[1]).Trim());

            double cashreceivedExtra = (receivedCash.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCash.Text.Trim());
            double cardreceivedExtra = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());
            double paytmreceivedExtra = (receivedPaytm.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedPaytm.Text.Trim());
            double flatoffExtra = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());
            double txtAdvAmtExtra = (txtAdvanceAmt.Text.Trim() == "") ? 0 : Convert.ToDouble(txtAdvanceAmt.Text.Trim());
            double RoundOffExtra = (txtRoundOff.Text.Trim() == "") ? 0 : Convert.ToDouble(txtRoundOff.Text.Trim());
            double txtPackForwdExtra = (PackCharge.Text.Trim() == "") ? 0 : Convert.ToDouble(PackCharge.Text.Trim());
            double txtGSttaxvals = (GSTTaxVa.Text.Trim() == "") ? 0 : Convert.ToDouble(GSTTaxVa.Text.Trim());
            int totalParclExtra = (totalParcel.Text.Trim() == "") ? 0 : Convert.ToInt32(totalParcel.Text.Trim());
            double offerzoneExtra = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
            double loyaltycardExtra = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());

            double zeroValvalExtra = 0.0;

            cmdCommonExtra.Parameters.Add(new SqlParameter("@Labour", labourTotal));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@MakingCharges", makingTotalCharge));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@TotalInvValue", totalInvValues - oldtotalVal));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@TotalTaxableValue", totalTaxableValues));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@TotalQuantities", totalQuanty));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@OfferAmount", offerzone));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@LoyaltyCard", loyaltycard));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@TotalPaidAmt", totalInvValues - oldtotalVal));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@PayModeGateway", ""));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@PaidCardSwipe", cardreceived));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@PaidCash", cashreceived));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@PaidChequeBank", zeroValvalExtra));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@PaidOtherGateway", paytmreceived));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@PaidOnlineBank", zeroValvalExtra));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@FlatOffTM", flatoff));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@RoundOff", RoundOff));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@AdvanceAmt", txtAdvAmt));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@BalanceCRorDR", BalanceCRorDR));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@TotalParcels", totalParcl));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@PackingAndForwarding", txtPackForwd));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@Freight", zeroValvalExtra));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@GSTTax", txtGSttaxvals));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@IsDraftVoucher", "false"));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@DispatchedThrough", dispatchedThrough.Text));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@TransportNameOrID", transportName.Text));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@CompID", Convert.ToInt32(CompID)));

            //cmdCommon.Connection.Open();
            cmdCommonExtra.ExecuteNonQuery();
            cmdCommonExtra.Connection.Close();





















            ///////////////////////////////--------------------------------------------SaleVoucherOtherDetails Entry
            SqlConnection conStrCommonother = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conStrCommonother.Open();
            //string sql = "SELECT COUNT(*) From AccountsList where AcctName='" + textBoxAcctName.Text.Trim() + "'";
            SqlCommand cmdCommonother;//= new SqlCommand(sql, con);
            //long debitacctnumber = 0;
            //long creditacctnumber = 0;
            //string againstinvnumber = "";
            cmdCommonother = new SqlCommand("SPUpdateAccountsForSaleVoucherOtherDetails", conStrCommon);
            cmdCommonother.CommandType = CommandType.StoredProcedure;
            cmdCommonother.Parameters.Add(new SqlParameter("@SundryDebtorName", autocompltCustName.autoTextBox.Text));
            cmdCommonother.Parameters.Add(new SqlParameter("@SalesAcctName", SaleAcctName));
            cmdCommonother.Parameters.Add(new SqlParameter("@IsNewSundryDebtor", "No"));
            if (CashCustName.Text != "")
            {
                cmdCommonother.Parameters.Add(new SqlParameter("@CashCustomerName", CashCustName.Text));
                cmdCommonother.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Cash"));
            }
            else
            {
                cmdCommonother.Parameters.Add(new SqlParameter("@CashCustomerName", ""));
                cmdCommonother.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Credit"));
            }
            cmdCommonother.Parameters.Add(new SqlParameter("@InvoiceNumber", invoiceNumber.Text));
            cmdCommonother.Parameters.Add(new SqlParameter("@SaleVoucherNumber", Convert.ToInt64(VoucherNumber.Text.Trim())));
            cmdCommonother.Parameters.Add(new SqlParameter("@SaleVoucherType", "Sale Voucher"));
            cmdCommonother.Parameters.Add(new SqlParameter("@EwayNumber", EwayNumbertxt.Text));

            cmdCommonother.Parameters.Add(new SqlParameter("@InvDate", BillDateInvValval));

            //check isState or central with company statecode            
            cmdCommonother.Parameters.Add(new SqlParameter("@IsState", IState.ToString()));
            discounttotalCommon = (discountTxt.Text.Trim() == "") ? 0 : Convert.ToDouble(discountTxt.Text.Trim());
            cmdCommonother.Parameters.Add(new SqlParameter("@Discount", discounttotalCommon)); //gettotal Discount-Common 
            if (IState)
            {
                double outputigstval = 0.0;
                cmdCommonother.Parameters.Add(new SqlParameter("@OutputCGST", totalTaxAmount / 2));
                cmdCommonother.Parameters.Add(new SqlParameter("@OutputSGST", totalTaxAmount / 2));
                cmdCommonother.Parameters.Add(new SqlParameter("@OutputIGST", outputigstval));
            }
            else
            {
                double outputsgstval = 0.0;


                cmdCommonother.Parameters.Add(new SqlParameter("@OutputCGST", outputsgstval));
                cmdCommonother.Parameters.Add(new SqlParameter("@OutputSGST", outputsgstval));
                cmdCommonother.Parameters.Add(new SqlParameter("@OutputIGST", totalTaxAmount));
            }
            // Get all common details on global var and pas to sp
            //receivedOffer = (receivedOffer.Text.Trim() == "") ? 0: Convert.ToDouble(receivedOffer.Text);
            //discounttotalCommon = Convert.ToDouble(receivedLoyalty.Text);
            //discounttotalCommon = Convert.ToDouble(receivedPaytm.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCash.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCard.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCard.Text);
            BalanceCRorDR = Convert.ToDouble(((dueBal.Content.ToString()).Replace("₹", "").Split(':')[1]).Trim());

            //double cashreceived = (receivedCash.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCash.Text.Trim());
            //double cardreceived = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());
            //double paytmreceived = (receivedPaytm.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedPaytm.Text.Trim());
            //double flatoff = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());
            //double txtAdvAmt = (txtAdvanceAmt.Text.Trim() == "") ? 0 : Convert.ToDouble(txtAdvanceAmt.Text.Trim());
            //double RoundOff = (txtRoundOff.Text.Trim() == "") ? 0 : Convert.ToDouble(txtRoundOff.Text.Trim());
            //double txtPackForwd = (txtPackForward.Text.Trim() == "") ? 0 : Convert.ToDouble(txtPackForward.Text.Trim());
            //int totalParcl = (totalParcel.Text.Trim() == "") ? 0 : Convert.ToInt32(totalParcel.Text.Trim());
            //double offerzone = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
            //double loyaltycard = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());

            //double zeroValval = 0.0;

            cmdCommonother.Parameters.Add(new SqlParameter("@Labour", labourTotal));
            cmdCommonother.Parameters.Add(new SqlParameter("@MakingCharges", makingTotalCharge));
            cmdCommonother.Parameters.Add(new SqlParameter("@TotalInvValue", totalInvValues - oldtotalVal));
            cmdCommonother.Parameters.Add(new SqlParameter("@TotalTaxableValue", totalTaxableValues));
            cmdCommonother.Parameters.Add(new SqlParameter("@TotalQuantities", totalQuanty));
            cmdCommonother.Parameters.Add(new SqlParameter("@OfferAmount", offerzone));
            cmdCommonother.Parameters.Add(new SqlParameter("@LoyaltyCard", loyaltycard));
            cmdCommonother.Parameters.Add(new SqlParameter("@TotalPaidAmt", totalInvValues - oldtotalVal));
            cmdCommonother.Parameters.Add(new SqlParameter("@PayModeGateway", ""));
            cmdCommonother.Parameters.Add(new SqlParameter("@PaidCardSwipe", cardreceived));
            cmdCommonother.Parameters.Add(new SqlParameter("@PaidCash", cashreceived));
            cmdCommonother.Parameters.Add(new SqlParameter("@PaidChequeBank", zeroValval));
            cmdCommonother.Parameters.Add(new SqlParameter("@PaidOtherGateway", paytmreceived));
            cmdCommonother.Parameters.Add(new SqlParameter("@PaidOnlineBank", zeroValval));
            cmdCommonother.Parameters.Add(new SqlParameter("@FlatOffTM", flatoff));
            cmdCommonother.Parameters.Add(new SqlParameter("@RoundOff", RoundOff));
            cmdCommonother.Parameters.Add(new SqlParameter("@AdvanceAmt", txtAdvAmt));
            cmdCommonother.Parameters.Add(new SqlParameter("@BalanceCRorDR", BalanceCRorDR));
            cmdCommonother.Parameters.Add(new SqlParameter("@TotalParcels", totalParcl));
            cmdCommonother.Parameters.Add(new SqlParameter("@PackingAndForwarding", txtPackForwd));
            cmdCommonother.Parameters.Add(new SqlParameter("@Freight", zeroValval));
            cmdCommonother.Parameters.Add(new SqlParameter("@IsDraftVoucher", "false"));
            cmdCommonother.Parameters.Add(new SqlParameter("@DispatchedThrough", dispatchedThrough.Text));
            cmdCommonother.Parameters.Add(new SqlParameter("@TransportNameOrID", transportName.Text));
            cmdCommonother.Parameters.Add(new SqlParameter("@CompID", Convert.ToInt32(CompID)));
            cmdCommonother.Connection.Open();
            cmdCommonother.ExecuteNonQuery();
            cmdCommonother.Connection.Close();



            if (InvoiceNumber == Convert.ToInt64(invoiceNumber.Text))
            {
                string currentInvNumber = "";
                SqlConnection conCurrentInv = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                conCurrentInv.Open();
                string sqlCurrentInv = "select number from AutoIncrement where Name = 'SaleInvoice' and CompID = '" + CompID + "'";
                SqlCommand cmdCurrentInv = new SqlCommand(sqlCurrentInv);
                cmdCurrentInv.Connection = conCurrentInv;
                SqlDataReader readerCurrentInv = cmdCurrentInv.ExecuteReader();

                //tmpProduct = new Product();

                while (readerCurrentInv.Read())
                {
                    currentInvNumber = readerCurrentInv.GetInt64(0).ToString().Trim();

                }
                readerCurrentInv.Close();

                if (currentInvNumber == invoiceNumber.Text.Trim())
                {

                    SqlConnection consrauto = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                    consrauto.Open();
                    string updateVoucher = "";
                    string updateInvoice = "";
                    updateVoucher = "update AutoIncrement  set  Number='" + (Convert.ToInt64(VoucherNumber.Text) + 1) + "' where Name ='SaleVoucher' and Type='Sale Voucher'  and CompID = '" + CompID + "' ";
                    updateInvoice = "update AutoIncrement  set  Number='" + (Convert.ToInt64(invoiceNumber.Text) + 1) + "' where Name ='SaleInvoice' and Type='Sale Invoice'  and CompID = '" + CompID + "' ";
                    SqlCommand myCommandStkUpdateauto = new SqlCommand(updateVoucher, consrauto);
                    myCommandStkUpdateauto.Connection = consrauto;
                    int Numauto = myCommandStkUpdateauto.ExecuteNonQuery();

                    SqlCommand myCommandStkUpdateautoInv = new SqlCommand(updateInvoice, consrauto);
                    myCommandStkUpdateautoInv.Connection = consrauto;
                    int Numautoinv = myCommandStkUpdateautoInv.ExecuteNonQuery();

                    myCommandStkUpdateauto.Connection.Close();

                    myCommandStkUpdateautoInv.Connection.Close();
                }
            }


            SaleVoucherQtyEstimationA5SizePdf sv = new SaleVoucherQtyEstimationA5SizePdf();
            //SaleVoucherBarcode sv = new SaleVoucherBarcode();
            this.NavigationService.Navigate(sv);

        }



        private void PrintSimpleTextButton_Click(object sender, RoutedEventArgs e)
        {

            //////Direct send pdf to Printer from the saved pdf location.
            ////ProcessStartInfo info = new ProcessStartInfo();
            ////info.Verb = "print";
            ////info.FileName=@"C:\output.pdf";
            ////info.CreateNoWindow = true;
            ////info.WindowStyle = ProcessWindowStyle.Hidden;

            ////Process p = new Process();
            ////p.StartInfo=info;
            ////p.Start();
            ////p.WaitForInputIdle();
            ////System.Threading.Thread.Sleep(10000);
            ////if (false == p.CloseMainWindow())
            ////{
            ////    p.Kill();
            ////}


            //
            //




            /*Write code to save the sale voucher details
             * impacted tables are below
             * SalesVouchers(Not required), 
          
             * Accounts Tables(AccountsList, SundryDebtorsAccountsLedgers, Cash,PayTM,CGST,SGST,IGST, GSTR1Table,HSNTable,Discount, Packing, RoundOff,TransportDetails , BankAccountsLedgers, CashFlow, DraftVouchers,DutyAndTaxesAccountsLedgers, ErrorLogs,POSVouchers,SalesAccountsLedgers
             * Inventory Tables  StockItems,SalesVoucherInventory,StockItemsCounterWise,StockItemsHistory,StockItemsStorageWise,StockItemsTrayWise, StorageLocations, 
             * Taxes Tables
             *  on succeessful saved -->AutoIncrement VoucherNumber also
             *  
             * */


            //Bill is already generated and saved and user click againt then delete all existing data and add new , but for stock items do reverse process 
            try
            {
                string BillDateInv = invDate.SelectedDate.ToString();

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


                //Reset SalesVoucherInventory
                SqlConnection myConnSVEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                myConnSVEntryStr.Open();
                string CountSVEntryStr = "SELECT COUNT(*) From SalesVoucherInventory where InvoiceNumber='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "'";
                // string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                SqlCommand myCommandDel = new SqlCommand(CountSVEntryStr, myConnSVEntryStr);
                myCommandDel.Connection = myConnSVEntryStr;

                //int countRec = myCommand.ExecuteNonQuery();
                int countRecDelDel = (int)myCommandDel.ExecuteScalar();
                myCommandDel.Connection.Close();
                if (countRecDelDel != 0)
                {
                    // MessageBox.Show("Item Name is already Exist, Please delete existing", "Add Record");


                    SqlCommand myCommandDeleteDel = new SqlCommand("SPUpdateStockOnSalesVoucherChangeOrDeleteEstimationsQty", myConnSVEntryStr);
                    myCommandDeleteDel.CommandType = CommandType.StoredProcedure;
                    myCommandDeleteDel.Parameters.Add(new SqlParameter("@VoucherNumber", Convert.ToInt64(VoucherNumber.Text.Trim())));
                    myCommandDeleteDel.Parameters.Add(new SqlParameter("@InvoiceNumber", invoiceNumber.Text.Trim()));
                    myCommandDeleteDel.Parameters.Add(new SqlParameter("@CompID", CompID));
                    myCommandDeleteDel.Connection.Open();
                    int countRecDelDelDel = myCommandDeleteDel.ExecuteNonQuery();
                    if (countRecDelDelDel != 0)
                    {
                        //  MessageBox.Show("Record Successfully Deleted....", "Delete Record");
                    }


                    //string DeleteExisting = "DELETE From SalesVoucherInventory where  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                    ////string DeleteExisting = "DELETE From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                    //SqlCommand myCommandDeleteDel = new SqlCommand(DeleteExisting, myConnSVEntryStr);
                    //myCommandDeleteDel.Connection.Open();
                    //int countRecDelDelDel = (int)myCommandDeleteDel.ExecuteNonQuery();
                    //if (countRecDelDelDel != 0)
                    //{
                    //    // MessageBox.Show("Deleted", "Add Record");
                    //}
                    myCommandDeleteDel.Connection.Close();
                }
                //myCommandDel.Connection.Close();




                IEnumerable itemsSource = CartGrid.ItemsSource as IEnumerable;

                for (int k = 0; k < CartGrid.Items.Count; ++k)
                {
                    DataGridRow row = CartGrid.ItemContainerGenerator.ContainerFromItem(itemsSource) as DataGridRow;

                    row = CartGrid.ItemContainerGenerator.ContainerFromItem(itemsSource) as DataGridRow;

                    if (row == null)
                    {
                        CartGrid.UpdateLayout();
                        CartGrid.ScrollIntoView(CartGrid.Items[k]);
                        row = (DataGridRow)CartGrid.ItemContainerGenerator.ContainerFromIndex(k);
                    }

                    if (row != null)
                    {
                        DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(row);

                        //============
                        if (presenter == null)
                        {

                            CartGrid.UpdateLayout();
                            CartGrid.ScrollIntoView(CartGrid.Items[k]);
                            row = (DataGridRow)CartGrid.ItemContainerGenerator.ContainerFromIndex(k);
                            DataGridCellsPresenter prsnter = FindVisualChild<DataGridCellsPresenter>(row);
                            presenter = prsnter;
                        }
                        //============
                        // FOR iTEMnAME 2
                        DataGridCell cellItemName = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(1);
                        //TextBlock txtItemNam = cellItemName.Content as TextBlock;
                        TextBlock txtItemNam = cellItemName.Content as TextBlock;

                        //DataGridCell cellHSN = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(2);
                        //TextBlock hsnText = cellHSN.Content as TextBlock;


                        // for Qty

                        DataGridCell cellCTN = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(2);
                        TextBlock qtyCTN = cellCTN.Content as TextBlock;


                        DataGridCell cellQty = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(3);
                        TextBlock qtyText = cellQty.Content as TextBlock;

                        DataGridCell cellUnitID = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(4);
                        TextBlock txtcellUnitID = cellUnitID.Content as TextBlock;

                        DataGridCell cellPrice = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(5);
                        TextBlock priceText = cellPrice.Content as TextBlock;


                        DataGridCell cellAmount = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(6);
                        TextBlock txtCellAmount = cellAmount.Content as TextBlock;


                        DataGridCell discRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(7);
                        TextBlock txtdiscRate = discRate.Content as TextBlock;

                        DataGridCell cellTaxableAmt = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(8);
                        TextBlock txtTaxableAmt = cellTaxableAmt.Content as TextBlock;

                        //DataGridCell gstRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(8);
                        //TextBlock txtgstRate = gstRate.Content as TextBlock;

                        //DataGridCell gstTax = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(9);
                        //TextBlock txtgsTax = gstTax.Content as TextBlock;


                        //DataGridCell cellTotal = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(10);
                        //TextBlock totalText = cellTotal.Content as TextBlock;


                        //DataGridCell cellStoreID = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(11);
                        //TextBlock txtcellStoreID = cellStoreID.Content as TextBlock;

                        //DataGridCell cellCounterID = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(11);
                        //TextBlock txtcellCounterID = cellCounterID.Content as TextBlock;

                        //DataGridCell cellTrayID = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(11);
                        //TextBlock txtcellTrayID = cellTrayID.Content as TextBlock;

                        //Get Voucher Number




                        //Insert into SalesInventory 
                        SqlConnection myConSVInventoryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                        myConSVInventoryStr.Open();


                        string querySalesInventory = "";
                        querySalesInventory = "insert into SalesVoucherInventory(VoucherNumber, InvoiceNumber,ItemName,HSN,SalePrice,GSTRate,GSTTax,Discount,TaxablelAmount,TotalAmount, BilledQty,TransactionDate,FromConsumedStorageID,FromConsumedTrayID,FromConsumedCounterID,CompID,Amount,UnitID,Itemdesc) Values ( '" + VoucherNumber.Text + "','" + invoiceNumber.Text.Trim() + "','" + txtItemNam.Text + "','','" + priceText.Text + "','','','" + txtdiscRate.Text + "', '" + txtTaxableAmt.Text + "','" + txtTaxableAmt.Text + "','" + qtyText.Text + "', '" + InvdateValue + "','1','1','1', '" + CompID + "','" + txtCellAmount.Text + "','" + txtcellUnitID.Text + "','" + qtyCTN.Text.Trim() + "')";



                        SqlCommand myCommandSVInventory = new SqlCommand(querySalesInventory, myConSVInventoryStr);
                        myCommandSVInventory.Connection = myConSVInventoryStr;
                        //myCommandInvEntry.Connection.Open();
                        int NumPI = myCommandSVInventory.ExecuteNonQuery();
                        myCommandSVInventory.Connection.Close();


                        //StockItems: CRUD Start
                        if ((txtItemNam != null) && (priceText != null))
                        {
                            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                            SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                            myConnSalesInvEntryStr.Open();
                            string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPc where ItemName ='" + txtItemNam.Text.Trim() + "'  and CompID = '" + CompID + "'";
                            //string CountSalesInvEntryStr = "SELECT COUNT(*) From StockItems where ItemName ='" + autocompleteItemName.autoTextBox1.Text + "' and  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                            //// string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                            SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
                            myCommand.Connection = myConnSalesInvEntryStr;

                            //int countRec = myCommand.ExecuteNonQuery();
                            int countRec = (int)myCommand.ExecuteScalar();
                            myCommand.Connection.Close();


                            if (countRec != 0)
                            {

                                string queryStrStockCheck = "";

                                string balanceStk = "";
                                string balanceStkWt = "";

                                // write code to update stocktable directly 
                                queryStrStockCheck = "select * from StockItemsByPc where ItemName = '" + txtItemNam.Text.Trim() + "' and CompID = '" + CompID + "'";
                                //OleDbCommand command = new OleDbCommand(queryStr, con);
                                // myConnStock.Open();
                                SqlCommand myCommandStkCheck = new SqlCommand(queryStrStockCheck, myConnSalesInvEntryStr);
                                myCommandStkCheck.Connection.Open();
                                SqlDataReader reader = myCommandStkCheck.ExecuteReader();



                                while (reader.Read())
                                {
                                    // var CustID = reader.GetValue(0).ToString();
                                    string ItemName = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                                    string PrintName = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                                    double invQty = (qtyText.Text != "") ? (Convert.ToDouble(qtyText.Text)) : 0;
                                    double actualQty = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)) : 0;
                                    //double invWt = (qtyWt.Text != "") ? (Convert.ToDouble(qtyWt.Text)) : 0;
                                    double actualWt = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)) : 0;
                                    //if (ItemName == "Old Gold" || ItemName == "Old Silver")
                                    //{
                                    //    balanceStk = Math.Round((actualQty + invQty), 2).ToString();
                                    //    balanceStkWt = Math.Round((actualWt + invWt), 2).ToString();
                                    //}
                                    //else
                                    //{
                                    balanceStk = Math.Round((actualQty - invQty), 2).ToString();
                                    //balanceStkWt = Math.Round((actualWt - invWt), 2).ToString();
                                    //}

                                }
                                reader.Close();
                                myCommandStkCheck.Connection.Close();

                                string queryStrStockUpdate = "";
                                queryStrStockUpdate = "update StockItemsByPc  set UpdateDate='" + InvdateValue + "', IsSoldFlag='1', UnitID='" + txtcellUnitID.Text + "'  ,ActualQty='" + balanceStk + "',ActualWt='" + balanceStkWt + "',LastSalePrice='" + priceText.Text + "'  where ItemName ='" + txtItemNam.Text + "'   and CompID = '" + CompID + "' ";
                                if (txtItemNam.Text == "Old Gold" || txtItemNam.Text == "Old Silver")
                                {
                                    queryStrStockUpdate = "update StockItemsByPc  set UpdateDate='" + InvdateValue + "' , ActualQty='" + balanceStk + "',ActualWt='" + balanceStkWt + "',LastBuyPrice='" + priceText.Text + "'  where ItemName ='" + txtItemNam.Text + "'   and CompID = '" + CompID + "' ";
                                }
                                SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                                myCommandStkUpdate.Connection.Open();
                                myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                                if (txtItemNam.Text.Trim() != "")
                                {
                                    // myCommandStk.Connection.Open();
                                    int Num = myCommandStkUpdate.ExecuteNonQuery();
                                    if (Num != 0)
                                    {
                                        // MessageBox.Show("Record Successfully Updated....", "Update Record");
                                    }
                                    else
                                    {
                                        MessageBox.Show("Stock is not Updated....", "Update Record Error");
                                    }
                                    // myCommandStk.Connection.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Stock can not be updated....", "Update Record Error");
                                }
                                myCommandStkUpdate.Connection.Close();
                            }
                            else
                            {

                                string querySalesInvEntry = "";
                                querySalesInvEntry = "insert into StockItemsByPc(ItemName, ActualQty,ActualWt,ItemPrice,GSTRate,LastSalePrice,CompID,UnitID) Values ( '" + txtItemNam.Text + "','" + 0 + "','" + 0 + "','" + priceText.Text + "','','" + priceText.Text + "', '" + CompID + "','" + txtcellUnitID.Text + "')";
                                //if (txtItemNam.Text == "Old Gold" || txtItemNam.Text == "Old Silver")
                                //{
                                //    querySalesInvEntry = "insert into StockItems(ItemName, ActualQty,ActualWt,ItemPrice,GSTRate,LastBuyPrice,CompID) Values ( '" + txtItemNam.Text + "','" + 0 + "','" + 0 + "','" + priceText.Text + "','" + txtgstRate.Text + "','" + priceText.Text + "', '" + CompID + "')";
                                //}

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

                        //    string DeleteExisting = "DELETE From SalesInventory where ItemName ='" + txtItemNam.Text + "' and GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                        //    //string DeleteExisting = "DELETE From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                        //    SqlCommand myCommandDelete = new SqlCommand(DeleteExisting, myConnSalesInvEntryStr);
                        //    myCommandDelete.Connection.Open();
                        //    int countRecDel = (int)myCommandDelete.ExecuteNonQuery();
                        //    if (countRecDel != 0)
                        //    {
                        //        // MessageBox.Show("Deleted", "Add Record");
                        //    }
                        //    myCommandDelete.Connection.Close();

                        //}




                    }
                }



                //IEnumerable itemsSourceOld = OldGoldGrid.ItemsSource as IEnumerable;

                //for (int k = 0; k < OldGoldGrid.Items.Count; ++k)
                //{
                //    DataGridRow row = OldGoldGrid.ItemContainerGenerator.ContainerFromItem(itemsSourceOld) as DataGridRow;

                //    row = OldGoldGrid.ItemContainerGenerator.ContainerFromItem(itemsSourceOld) as DataGridRow;

                //    if (row == null)
                //    {
                //        OldGoldGrid.UpdateLayout();
                //        OldGoldGrid.ScrollIntoView(OldGoldGrid.Items[k]);
                //        row = (DataGridRow)OldGoldGrid.ItemContainerGenerator.ContainerFromIndex(k);
                //    }

                //    if (row != null)
                //    {
                //        DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(row);

                //        //============
                //        if (presenter == null)
                //        {

                //            OldGoldGrid.UpdateLayout();
                //            OldGoldGrid.ScrollIntoView(OldGoldGrid.Items[k]);
                //            row = (DataGridRow)OldGoldGrid.ItemContainerGenerator.ContainerFromIndex(k);
                //            DataGridCellsPresenter prsnter = FindVisualChild<DataGridCellsPresenter>(row);
                //            presenter = prsnter;
                //        }
                //        //============
                //        // FOR iTEMnAME 2
                //        DataGridCell cellItemName = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(1);
                //        //TextBlock txtItemNam = cellItemName.Content as TextBlock;
                //        TextBlock txtItemNam = cellItemName.Content as TextBlock;
                //        // for Qty
                //        DataGridCell cellQty = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(2);
                //        TextBlock qtyText = cellQty.Content as TextBlock;

                //        DataGridCell cellQtyWt = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(3);
                //        TextBlock qtyWt = cellQtyWt.Content as TextBlock;

                //        DataGridCell cellHSN = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(2);
                //        TextBlock hsnText = cellHSN.Content as TextBlock;

                //        DataGridCell cellUnit = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(3);
                //        ComboBox unitText = cellUnit.Content as ComboBox;

                //        DataGridCell cellPrice = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(7);
                //        TextBlock priceText = cellPrice.Content as TextBlock;

                //        DataGridCell cellTotal = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(7);
                //        TextBlock totalText = cellTotal.Content as TextBlock;

                //        DataGridCell gstRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(11);
                //        TextBlock txtgstRate = gstRate.Content as TextBlock;

                //        DataGridCell cellsgstRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(9);
                //        TextBlock txtsgstRate = cellsgstRate.Content as TextBlock;



                //        DataGridCell cellIgstRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(9);
                //        TextBlock txtIgstRate = cellIgstRate.Content as TextBlock;

                //        DataGridCell cellStock = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(10);
                //        TextBlock txtStock = cellStock.Content as TextBlock;


                //        //StockItems: CRUD Start
                //        if ((txtItemNam != null) && (priceText != null))
                //        {
                //            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //            SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //            myConnSalesInvEntryStr.Open();
                //            string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItems where ItemName ='" + txtItemNam.Text + "' and CompID = '" + CompID + "'";
                //            //string CountSalesInvEntryStr = "SELECT COUNT(*) From StockItems where ItemName ='" + autocompleteItemName.autoTextBox1.Text + "' and  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                //            //// string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                //            SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
                //            myCommand.Connection = myConnSalesInvEntryStr;

                //            //int countRec = myCommand.ExecuteNonQuery();
                //            int countRec = (int)myCommand.ExecuteScalar();
                //            myCommand.Connection.Close();


                //            if (countRec != 0)
                //            {

                //                string queryStrStockCheck = "";

                //                string balanceStk = "";
                //                string balanceStkWt = "";

                //                // write code to update stocktable directly 
                //                queryStrStockCheck = "select * from StockItems where ItemName = '" + txtItemNam.Text + "'";
                //                //OleDbCommand command = new OleDbCommand(queryStr, con);
                //                // myConnStock.Open();
                //                SqlCommand myCommandStkCheck = new SqlCommand(queryStrStockCheck, myConnSalesInvEntryStr);
                //                myCommandStkCheck.Connection.Open();
                //                SqlDataReader reader = myCommandStkCheck.ExecuteReader();



                //                while (reader.Read())
                //                {
                //                    // var CustID = reader.GetValue(0).ToString();
                //                    string ItemName = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                //                    string PrintName = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                //                    double invQty = (qtyText.Text != "") ? (Convert.ToDouble(qtyText.Text)) : 0;
                //                    double actualQty = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)) : 0;
                //                    double invWt = (qtyWt.Text != "") ? (Convert.ToDouble(qtyWt.Text)) : 0;
                //                    double actualWt = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)) : 0;
                //                    if (ItemName == "Old Gold" || ItemName == "Old Silver")
                //                    {
                //                        balanceStk = Math.Round((actualQty + invQty), 2).ToString();
                //                        balanceStkWt = Math.Round((actualWt + invWt), 2).ToString();
                //                    }
                //                    else
                //                    {
                //                        balanceStk = Math.Round((actualQty - invQty), 2).ToString();
                //                        balanceStkWt = Math.Round((actualWt - invWt), 2).ToString();
                //                    }

                //                }
                //                reader.Close();
                //                myCommandStkCheck.Connection.Close();

                //                string queryStrStockUpdate = "";
                //                queryStrStockUpdate = "update StockItems  set UpdateDate='" + InvdateValue + "',  IsSoldFlag='1',  ActualQty='" + balanceStk + "',ActualWt='" + balanceStkWt + "',LastSalePrice='" + priceText.Text + "'  where ItemName ='" + txtItemNam.Text + "'  and CompID = '" + CompID + "' ";
                //                if (txtItemNam.Text == "Old Gold" || txtItemNam.Text == "Old Silver")
                //                {
                //                    queryStrStockUpdate = "update StockItems  set UpdateDate='" + InvdateValue + "', ActualQty='" + balanceStk + "',ActualWt='" + balanceStkWt + "',LastBuyPrice='" + priceText.Text + "'  where ItemName ='" + txtItemNam.Text + "'  and CompID = '" + CompID + "' ";
                //                }
                //                SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                //                myCommandStkUpdate.Connection.Open();
                //                myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                //                if (txtItemNam.Text.Trim() != "")
                //                {
                //                    // myCommandStk.Connection.Open();
                //                    int Num = myCommandStkUpdate.ExecuteNonQuery();
                //                    if (Num != 0)
                //                    {
                //                        // MessageBox.Show("Record Successfully Updated....", "Update Record");
                //                    }
                //                    else
                //                    {
                //                        MessageBox.Show("Stock is not Updated....", "Update Record Error");
                //                    }
                //                    // myCommandStk.Connection.Close();
                //                }
                //                else
                //                {
                //                    MessageBox.Show("Stock can not be updated....", "Update Record Error");
                //                }
                //                myCommandStkUpdate.Connection.Close();
                //            }
                //            else
                //            {

                //                string querySalesInvEntry = "";
                //                querySalesInvEntry = "insert into StockItems(ItemName, ActualQty,ActualWt,ItemPrice,GSTRate,LastSalePrice,CompID) Values ( '" + txtItemNam.Text + "','" + 0 + "','" + 0 + "','" + priceText.Text + "','" + txtgstRate.Text + "','" + priceText.Text + "' ,  '" + CompID + "')";
                //                if (txtItemNam.Text == "Old Gold" || txtItemNam.Text == "Old Silver")
                //                {
                //                    querySalesInvEntry = "insert into StockItems(ItemName, ActualQty,ActualWt,ItemPrice,GSTRate,LastBuyPrice,CompID) Values ( '" + txtItemNam.Text + "','" + 0 + "','" + 0 + "','" + priceText.Text + "','" + txtgstRate.Text + "','" + priceText.Text + "', '" + CompID + "')";
                //                }

                //                SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                //                myCommandInvEntry.Connection.Open();
                //                int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                //                if (NumPInv != 0)
                //                {
                //                    // MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                //                }
                //                else
                //                {
                //                    MessageBox.Show("Stock is not Inserted....", "Insert Record Error");
                //                }
                //                myCommandInvEntry.Connection.Close();

                //                // myConnStock.Close();

                //            }


                //        }

                //        //    string DeleteExisting = "DELETE From SalesInventory where ItemName ='" + txtItemNam.Text + "' and GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                //        //    //string DeleteExisting = "DELETE From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                //        //    SqlCommand myCommandDelete = new SqlCommand(DeleteExisting, myConnSalesInvEntryStr);
                //        //    myCommandDelete.Connection.Open();
                //        //    int countRecDel = (int)myCommandDelete.ExecuteNonQuery();
                //        //    if (countRecDel != 0)
                //        //    {
                //        //        // MessageBox.Show("Deleted", "Add Record");
                //        //    }
                //        //    myCommandDelete.Connection.Close();

                //        //}






                //StockItems End
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            SqlConnection conStrCommon = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conStrCommon.Open();
            //string sql = "SELECT COUNT(*) From AccountsList where AcctName='" + textBoxAcctName.Text.Trim() + "'";
            SqlCommand cmdCommon;//= new SqlCommand(sql, con);
            //long debitacctnumber = 0;
            //long creditacctnumber = 0;
            //string againstinvnumber = "";
            cmdCommon = new SqlCommand("[SPUpdateAccountsForSaleVoucher]", conStrCommon);
            cmdCommon.CommandType = CommandType.StoredProcedure;
            cmdCommon.Parameters.Add(new SqlParameter("@SundryDebtorName", autocompltCustName.autoTextBox.Text));
            cmdCommon.Parameters.Add(new SqlParameter("@SalesAcctName", SaleAcctName));
            cmdCommon.Parameters.Add(new SqlParameter("@IsNewSundryDebtor", "No"));
            if (CashCustName.Text.Trim() != "")
            {
                cmdCommon.Parameters.Add(new SqlParameter("@CashCustomerName", CashCustName.Text));
                cmdCommon.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Cash"));
            }
            else
            {
                cmdCommon.Parameters.Add(new SqlParameter("@CashCustomerName", ""));
                cmdCommon.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Credit"));
            }
            cmdCommon.Parameters.Add(new SqlParameter("@InvoiceNumber", invoiceNumber.Text));
            cmdCommon.Parameters.Add(new SqlParameter("@SaleVoucherNumber", Convert.ToInt64(VoucherNumber.Text.Trim())));
            cmdCommon.Parameters.Add(new SqlParameter("@SaleVoucherType", "Sale Voucher"));
            cmdCommon.Parameters.Add(new SqlParameter("@EwayNumber", EwayNumbertxt.Text));

            string BillDateInvVal = invDate.SelectedDate.ToString();

            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dtinval = Convert.ToDateTime(BillDateInvVal);
            //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            int yearsinval = dtinval.Year;
            string monthsinval = dtinval.Month.ToString();
            if (dtinval.Month < 10)
            {
                monthsinval = "0" + monthsinval;
            }
            string daysinval = dtinval.Day.ToString();
            if (dtinval.Day < 10)
            {
                daysinval = "0" + daysinval;
            }

            string BillDateInvValval = yearsinval + "/" + monthsinval + "/" + daysinval;


            cmdCommon.Parameters.Add(new SqlParameter("@InvDate", BillDateInvValval));

            //check isState or central with company statecode            
            cmdCommon.Parameters.Add(new SqlParameter("@IsState", IState.ToString()));
            discounttotalCommon = (discountTxt.Text.Trim() == "") ? 0 : Convert.ToDouble(discountTxt.Text.Trim());
            cmdCommon.Parameters.Add(new SqlParameter("@Discount", discounttotalCommon)); //gettotal Discount-Common 
            if (IState)
            {
                double outputigstval = 0.0;
                cmdCommon.Parameters.Add(new SqlParameter("@OutputCGST", totalTaxAmount / 2));
                cmdCommon.Parameters.Add(new SqlParameter("@OutputSGST", totalTaxAmount / 2));
                cmdCommon.Parameters.Add(new SqlParameter("@OutputIGST", outputigstval));
            }
            else
            {
                double outputsgstval = 0.0;


                cmdCommon.Parameters.Add(new SqlParameter("@OutputCGST", outputsgstval));
                cmdCommon.Parameters.Add(new SqlParameter("@OutputSGST", outputsgstval));
                cmdCommon.Parameters.Add(new SqlParameter("@OutputIGST", totalTaxAmount));
            }
            // Get all common details on global var and pas to sp
            //receivedOffer = (receivedOffer.Text.Trim() == "") ? 0: Convert.ToDouble(receivedOffer.Text);
            //discounttotalCommon = Convert.ToDouble(receivedLoyalty.Text);
            //discounttotalCommon = Convert.ToDouble(receivedPaytm.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCash.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCard.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCard.Text);
            BalanceCRorDR = Convert.ToDouble(((dueBal.Content.ToString()).Replace("₹", "").Split(':')[1]).Trim());

            double cashreceived = (receivedCash.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCash.Text.Trim());
            double cardreceived = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());
            double paytmreceived = (receivedPaytm.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedPaytm.Text.Trim());
            double flatoff = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());
            double txtAdvAmt = (txtAdvanceAmt.Text.Trim() == "") ? 0 : Convert.ToDouble(txtAdvanceAmt.Text.Trim());
            double RoundOff = (txtRoundOff.Text.Trim() == "") ? 0 : Convert.ToDouble(txtRoundOff.Text.Trim());
            double txtPackForwd = (PackCharge.Text.Trim() == "") ? 0 : Convert.ToDouble(PackCharge.Text.Trim());
            int totalParcl = (totalParcel.Text.Trim() == "") ? 0 : Convert.ToInt32(totalParcel.Text.Trim());
            double offerzone = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
            double loyaltycard = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());

            double zeroValval = 0.0;

            cmdCommon.Parameters.Add(new SqlParameter("@Labour", labourTotal));
            cmdCommon.Parameters.Add(new SqlParameter("@MakingCharges", makingTotalCharge));
            cmdCommon.Parameters.Add(new SqlParameter("@TotalInvValue", totalInvValues - oldtotalVal));
            cmdCommon.Parameters.Add(new SqlParameter("@TotalTaxableValue", totalTaxableValues));
            cmdCommon.Parameters.Add(new SqlParameter("@TotalQuantities", totalQuanty));
            cmdCommon.Parameters.Add(new SqlParameter("@OfferAmount", offerzone));
            cmdCommon.Parameters.Add(new SqlParameter("@LoyaltyCard", loyaltycard));
            cmdCommon.Parameters.Add(new SqlParameter("@TotalPaidAmt", totalInvValues - oldtotalVal));
            cmdCommon.Parameters.Add(new SqlParameter("@PayModeGateway", ""));
            cmdCommon.Parameters.Add(new SqlParameter("@PaidCardSwipe", cardreceived));
            cmdCommon.Parameters.Add(new SqlParameter("@PaidCash", cashreceived));
            cmdCommon.Parameters.Add(new SqlParameter("@PaidChequeBank", zeroValval));
            cmdCommon.Parameters.Add(new SqlParameter("@PaidOtherGateway", paytmreceived));
            cmdCommon.Parameters.Add(new SqlParameter("@PaidOnlineBank", zeroValval));
            cmdCommon.Parameters.Add(new SqlParameter("@FlatOffTM", flatoff));
            cmdCommon.Parameters.Add(new SqlParameter("@RoundOff", RoundOff));
            cmdCommon.Parameters.Add(new SqlParameter("@AdvanceAmt", txtAdvAmt));
            cmdCommon.Parameters.Add(new SqlParameter("@BalanceCRorDR", BalanceCRorDR));
            cmdCommon.Parameters.Add(new SqlParameter("@TotalParcels", totalParcl));
            cmdCommon.Parameters.Add(new SqlParameter("@PackingAndForwarding", txtPackForwd));
            cmdCommon.Parameters.Add(new SqlParameter("@Freight", zeroValval));
            cmdCommon.Parameters.Add(new SqlParameter("@IsDraftVoucher", "false"));
            cmdCommon.Parameters.Add(new SqlParameter("@DispatchedThrough", dispatchedThrough.Text));
            cmdCommon.Parameters.Add(new SqlParameter("@TransportNameOrID", transportName.Text));
            cmdCommon.Parameters.Add(new SqlParameter("@CompID", Convert.ToInt32(CompID)));

            //cmdCommon.Connection.Open();
            cmdCommon.ExecuteNonQuery();
            cmdCommon.Connection.Close();

            //Extra Voucher Details Entry



            SqlConnection conStrCommonExtra = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conStrCommonExtra.Open();
            //string sql = "SELECT COUNT(*) From AccountsList where AcctName='" + textBoxAcctName.Text.Trim() + "'";
            SqlCommand cmdCommonExtra;//= new SqlCommand(sql, con);
            //long debitacctnumber = 0;
            //long creditacctnumber = 0;
            //string againstinvnumber = "";
            cmdCommonExtra = new SqlCommand("[SPUpdateAccountsForSaleVoucherExtra]", conStrCommonExtra);
            cmdCommonExtra.CommandType = CommandType.StoredProcedure;
            cmdCommonExtra.Parameters.Add(new SqlParameter("@SundryDebtorName", autocompltCustName.autoTextBox.Text));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@SalesAcctName", SaleAcctName));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@IsNewSundryDebtor", "No"));
            if (CashCustName.Text.Trim() != "")
            {
                cmdCommonExtra.Parameters.Add(new SqlParameter("@CashCustomerName", CashCustName.Text));
                cmdCommonExtra.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Cash"));
            }
            else
            {
                cmdCommonExtra.Parameters.Add(new SqlParameter("@CashCustomerName", ""));
                cmdCommonExtra.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Credit"));
            }
            cmdCommonExtra.Parameters.Add(new SqlParameter("@InvoiceNumber", invoiceNumber.Text));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@SaleVoucherNumber", Convert.ToInt64(VoucherNumber.Text.Trim())));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@SaleVoucherType", "Sale Voucher"));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@EwayNumber", EwayNumbertxt.Text));

            string BillDateInvValExtra = invDate.SelectedDate.ToString();

            // DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", null);
            DateTime dtinvalExtra = Convert.ToDateTime(BillDateInvValExtra);
            //DateTime dt = DateTime.ParseExact(sdt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            int yearsinvalExtra = dtinvalExtra.Year;
            string monthsinvalExtra = dtinvalExtra.Month.ToString();
            if (dtinvalExtra.Month < 10)
            {
                monthsinvalExtra = "0" + monthsinvalExtra;
            }
            string daysinvalExtra = dtinvalExtra.Day.ToString();
            if (dtinvalExtra.Day < 10)
            {
                daysinvalExtra = "0" + daysinvalExtra;
            }

            string BillDateInvValvalExtra = yearsinvalExtra + "/" + monthsinvalExtra + "/" + daysinvalExtra;


            cmdCommonExtra.Parameters.Add(new SqlParameter("@InvDate", BillDateInvValvalExtra));

            //check isState or central with company statecode            
            cmdCommonExtra.Parameters.Add(new SqlParameter("@IsState", IState.ToString()));
            discounttotalCommon = (discountTxt.Text.Trim() == "") ? 0 : Convert.ToDouble(discountTxt.Text.Trim());
            cmdCommonExtra.Parameters.Add(new SqlParameter("@Discount", discounttotalCommon)); //gettotal Discount-Common 
            if (IState)
            {
                double outputigstvalExtra = 0.0;
                cmdCommonExtra.Parameters.Add(new SqlParameter("@OutputCGST", totalTaxAmount / 2));
                cmdCommonExtra.Parameters.Add(new SqlParameter("@OutputSGST", totalTaxAmount / 2));
                cmdCommonExtra.Parameters.Add(new SqlParameter("@OutputIGST", outputigstvalExtra));
            }
            else
            {
                double outputsgstval = 0.0;


                cmdCommonExtra.Parameters.Add(new SqlParameter("@OutputCGST", outputsgstval));
                cmdCommonExtra.Parameters.Add(new SqlParameter("@OutputSGST", outputsgstval));
                cmdCommonExtra.Parameters.Add(new SqlParameter("@OutputIGST", totalTaxAmount));
            }
            // Get all common details on global var and pas to sp
            //receivedOffer = (receivedOffer.Text.Trim() == "") ? 0: Convert.ToDouble(receivedOffer.Text);
            //discounttotalCommon = Convert.ToDouble(receivedLoyalty.Text);
            //discounttotalCommon = Convert.ToDouble(receivedPaytm.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCash.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCard.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCard.Text);
            BalanceCRorDR = Convert.ToDouble(((dueBal.Content.ToString()).Replace("₹", "").Split(':')[1]).Trim());

            double cashreceivedExtra= (receivedCash.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCash.Text.Trim());
            double cardreceivedExtra = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());
            double paytmreceivedExtra = (receivedPaytm.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedPaytm.Text.Trim());
            double flatoffExtra = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());
            double txtAdvAmtExtra = (txtAdvanceAmt.Text.Trim() == "") ? 0 : Convert.ToDouble(txtAdvanceAmt.Text.Trim());
            double RoundOffExtra = (txtRoundOff.Text.Trim() == "") ? 0 : Convert.ToDouble(txtRoundOff.Text.Trim());
            double txtPackForwdExtra = (PackCharge.Text.Trim() == "") ? 0 : Convert.ToDouble(PackCharge.Text.Trim());
            double txtGSttaxvals = (GSTTaxVa.Text.Trim() == "") ? 0 : Convert.ToDouble(GSTTaxVa.Text.Trim());
            int totalParclExtra = (totalParcel.Text.Trim() == "") ? 0 : Convert.ToInt32(totalParcel.Text.Trim());
            double offerzoneExtra = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
            double loyaltycardExtra = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());

            double zeroValvalExtra = 0.0;

            cmdCommonExtra.Parameters.Add(new SqlParameter("@Labour", labourTotal));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@MakingCharges", makingTotalCharge));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@TotalInvValue", totalInvValues - oldtotalVal));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@TotalTaxableValue", totalTaxableValues));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@TotalQuantities", totalQuanty));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@OfferAmount", offerzone));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@LoyaltyCard", loyaltycard));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@TotalPaidAmt", totalInvValues - oldtotalVal));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@PayModeGateway", ""));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@PaidCardSwipe", cardreceived));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@PaidCash", cashreceived));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@PaidChequeBank", zeroValvalExtra));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@PaidOtherGateway", paytmreceived));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@PaidOnlineBank", zeroValvalExtra));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@FlatOffTM", flatoff));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@RoundOff", RoundOff));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@AdvanceAmt", txtAdvAmt));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@BalanceCRorDR", BalanceCRorDR));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@TotalParcels", totalParcl));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@PackingAndForwarding", txtPackForwd));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@Freight", zeroValvalExtra));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@GSTTax", txtGSttaxvals));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@IsDraftVoucher", "false"));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@DispatchedThrough", dispatchedThrough.Text));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@TransportNameOrID", transportName.Text));
            cmdCommonExtra.Parameters.Add(new SqlParameter("@CompID", Convert.ToInt32(CompID)));

            //cmdCommon.Connection.Open();
            cmdCommonExtra.ExecuteNonQuery();
            cmdCommonExtra.Connection.Close();





            ///////////////////////////////--------------------------------------------SaleVoucherOtherDetails Entry
            SqlConnection conStrCommonother = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conStrCommonother.Open();
            //string sql = "SELECT COUNT(*) From AccountsList where AcctName='" + textBoxAcctName.Text.Trim() + "'";
            SqlCommand cmdCommonother;//= new SqlCommand(sql, con);
            //long debitacctnumber = 0;
            //long creditacctnumber = 0;
            //string againstinvnumber = "";
            cmdCommonother = new SqlCommand("SPUpdateAccountsForSaleVoucherOtherDetails", conStrCommon);
            cmdCommonother.CommandType = CommandType.StoredProcedure;
            cmdCommonother.Parameters.Add(new SqlParameter("@SundryDebtorName", autocompltCustName.autoTextBox.Text));
            cmdCommonother.Parameters.Add(new SqlParameter("@SalesAcctName", SaleAcctName));
            cmdCommonother.Parameters.Add(new SqlParameter("@IsNewSundryDebtor", "No"));
            if (CashCustName.Text != "")
            {
                cmdCommonother.Parameters.Add(new SqlParameter("@CashCustomerName", CashCustName.Text));
                cmdCommonother.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Cash"));
            }
            else
            {
                cmdCommonother.Parameters.Add(new SqlParameter("@CashCustomerName", ""));
                cmdCommonother.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Credit"));
            }
            cmdCommonother.Parameters.Add(new SqlParameter("@InvoiceNumber", invoiceNumber.Text));
            cmdCommonother.Parameters.Add(new SqlParameter("@SaleVoucherNumber", Convert.ToInt64(VoucherNumber.Text.Trim())));
            cmdCommonother.Parameters.Add(new SqlParameter("@SaleVoucherType", "Sale Voucher"));
            cmdCommonother.Parameters.Add(new SqlParameter("@EwayNumber", EwayNumbertxt.Text));

            cmdCommonother.Parameters.Add(new SqlParameter("@InvDate", BillDateInvValval));

            //check isState or central with company statecode            
            cmdCommonother.Parameters.Add(new SqlParameter("@IsState", IState.ToString()));
            discounttotalCommon = (discountTxt.Text.Trim() == "") ? 0 : Convert.ToDouble(discountTxt.Text.Trim());
            cmdCommonother.Parameters.Add(new SqlParameter("@Discount", discounttotalCommon)); //gettotal Discount-Common 
            if (IState)
            {
                double outputigstval = 0.0;
                cmdCommonother.Parameters.Add(new SqlParameter("@OutputCGST", totalTaxAmount / 2));
                cmdCommonother.Parameters.Add(new SqlParameter("@OutputSGST", totalTaxAmount / 2));
                cmdCommonother.Parameters.Add(new SqlParameter("@OutputIGST", outputigstval));
            }
            else
            {
                double outputsgstval = 0.0;


                cmdCommonother.Parameters.Add(new SqlParameter("@OutputCGST", outputsgstval));
                cmdCommonother.Parameters.Add(new SqlParameter("@OutputSGST", outputsgstval));
                cmdCommonother.Parameters.Add(new SqlParameter("@OutputIGST", totalTaxAmount));
            }
            // Get all common details on global var and pas to sp
            //receivedOffer = (receivedOffer.Text.Trim() == "") ? 0: Convert.ToDouble(receivedOffer.Text);
            //discounttotalCommon = Convert.ToDouble(receivedLoyalty.Text);
            //discounttotalCommon = Convert.ToDouble(receivedPaytm.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCash.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCard.Text);
            //discounttotalCommon = Convert.ToDouble(receivedCard.Text);
            BalanceCRorDR = Convert.ToDouble(((dueBal.Content.ToString()).Replace("₹", "").Split(':')[1]).Trim());

            //double cashreceived = (receivedCash.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCash.Text.Trim());
            //double cardreceived = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());
            //double paytmreceived = (receivedPaytm.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedPaytm.Text.Trim());
            //double flatoff = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());
            //double txtAdvAmt = (txtAdvanceAmt.Text.Trim() == "") ? 0 : Convert.ToDouble(txtAdvanceAmt.Text.Trim());
            //double RoundOff = (txtRoundOff.Text.Trim() == "") ? 0 : Convert.ToDouble(txtRoundOff.Text.Trim());
            //double txtPackForwd = (txtPackForward.Text.Trim() == "") ? 0 : Convert.ToDouble(txtPackForward.Text.Trim());
            //int totalParcl = (totalParcel.Text.Trim() == "") ? 0 : Convert.ToInt32(totalParcel.Text.Trim());
            //double offerzone = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
            //double loyaltycard = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());

            //double zeroValval = 0.0;

            cmdCommonother.Parameters.Add(new SqlParameter("@Labour", labourTotal));
            cmdCommonother.Parameters.Add(new SqlParameter("@MakingCharges", makingTotalCharge));
            cmdCommonother.Parameters.Add(new SqlParameter("@TotalInvValue", totalInvValues - oldtotalVal));
            cmdCommonother.Parameters.Add(new SqlParameter("@TotalTaxableValue", totalTaxableValues));
            cmdCommonother.Parameters.Add(new SqlParameter("@TotalQuantities", totalQuanty));
            cmdCommonother.Parameters.Add(new SqlParameter("@OfferAmount", offerzone));
            cmdCommonother.Parameters.Add(new SqlParameter("@LoyaltyCard", loyaltycard));
            cmdCommonother.Parameters.Add(new SqlParameter("@TotalPaidAmt", totalInvValues - oldtotalVal));
            cmdCommonother.Parameters.Add(new SqlParameter("@PayModeGateway", ""));
            cmdCommonother.Parameters.Add(new SqlParameter("@PaidCardSwipe", cardreceived));
            cmdCommonother.Parameters.Add(new SqlParameter("@PaidCash", cashreceived));
            cmdCommonother.Parameters.Add(new SqlParameter("@PaidChequeBank", zeroValval));
            cmdCommonother.Parameters.Add(new SqlParameter("@PaidOtherGateway", paytmreceived));
            cmdCommonother.Parameters.Add(new SqlParameter("@PaidOnlineBank", zeroValval));
            cmdCommonother.Parameters.Add(new SqlParameter("@FlatOffTM", flatoff));
            cmdCommonother.Parameters.Add(new SqlParameter("@RoundOff", RoundOff));
            cmdCommonother.Parameters.Add(new SqlParameter("@AdvanceAmt", txtAdvAmt));
            cmdCommonother.Parameters.Add(new SqlParameter("@BalanceCRorDR", BalanceCRorDR));
            cmdCommonother.Parameters.Add(new SqlParameter("@TotalParcels", totalParcl));
            cmdCommonother.Parameters.Add(new SqlParameter("@PackingAndForwarding", txtPackForwd));
            cmdCommonother.Parameters.Add(new SqlParameter("@Freight", zeroValval));
            cmdCommonother.Parameters.Add(new SqlParameter("@IsDraftVoucher", "false"));
            cmdCommonother.Parameters.Add(new SqlParameter("@DispatchedThrough", dispatchedThrough.Text));
            cmdCommonother.Parameters.Add(new SqlParameter("@TransportNameOrID", transportName.Text));
            cmdCommonother.Parameters.Add(new SqlParameter("@CompID", Convert.ToInt32(CompID)));
            cmdCommonother.Connection.Open();
            cmdCommonother.ExecuteNonQuery();
            cmdCommonother.Connection.Close();



            if (InvoiceNumber == Convert.ToInt64(invoiceNumber.Text))
            {
                string currentInvNumber = "";
                SqlConnection conCurrentInv = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                conCurrentInv.Open();
                string sqlCurrentInv = "select number from AutoIncrement where Name = 'SaleInvoice' and CompID = '" + CompID + "'";
                SqlCommand cmdCurrentInv = new SqlCommand(sqlCurrentInv);
                cmdCurrentInv.Connection = conCurrentInv;
                SqlDataReader readerCurrentInv = cmdCurrentInv.ExecuteReader();

                //tmpProduct = new Product();

                while (readerCurrentInv.Read())
                {
                    currentInvNumber = readerCurrentInv.GetInt64(0).ToString().Trim();

                }
                readerCurrentInv.Close();

                if (currentInvNumber == invoiceNumber.Text.Trim())
                {

                    SqlConnection consrauto = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                    consrauto.Open();
                    string updateVoucher = "";
                    string updateInvoice = "";
                    updateVoucher = "update AutoIncrement  set  Number='" + (Convert.ToInt64(VoucherNumber.Text) + 1) + "' where Name ='SaleVoucher' and Type='Sale Voucher'  and CompID = '" + CompID + "' ";
                    updateInvoice = "update AutoIncrement  set  Number='" + (Convert.ToInt64(invoiceNumber.Text) + 1) + "' where Name ='SaleInvoice' and Type='Sale Invoice'  and CompID = '" + CompID + "' ";
                    SqlCommand myCommandStkUpdateauto = new SqlCommand(updateVoucher, consrauto);
                    myCommandStkUpdateauto.Connection = consrauto;
                    int Numauto = myCommandStkUpdateauto.ExecuteNonQuery();

                    SqlCommand myCommandStkUpdateautoInv = new SqlCommand(updateInvoice, consrauto);
                    myCommandStkUpdateautoInv.Connection = consrauto;
                    int Numautoinv = myCommandStkUpdateautoInv.ExecuteNonQuery();

                    myCommandStkUpdateauto.Connection.Close();

                    myCommandStkUpdateautoInv.Connection.Close();
                }
            }

            CreateFlowDocumentReadyMadeWholeSale();


            //// Create a PrintDialog
            //PrintDialog printDlg = new PrintDialog();

            ////Create a FlowDocument dynamically.
            //FlowDocument doc = CreateFlowDocumentQtyEsimation();
            //doc.ColumnWidth = 1024;
            //doc.Name = "FlowDoc";

            ////A5, A6, in B/w A5 and A6
            //doc.PageHeight = 800;
            //doc.PageWidth = 520; //550
            //doc.MinPageWidth = 520; //550

            //// Create IDocumentPaginatorSource from FlowDocument
            //IDocumentPaginatorSource idpSource = doc;

            //// Call PrintDocument method to send document to printer
            ////Uncomment for Print
            //printDlg.PrintDocument(idpSource.DocumentPaginator, "Receipt Printing.");


            SaleVoucherQtyEstimationA5SizePdf sv = new SaleVoucherQtyEstimationA5SizePdf();
            this.NavigationService.Navigate(sv);



        }

        /// <summary>
        /// This method creates a dynamic FlowDocument. You can add anything to this
        /// FlowDocument that you would like to send to the printer
        /// </summary>
        /// <returns></returns>
        private FlowDocument CreateFlowDocumentQtyEsimation()
        {
            //  Get Confirmation that data saved successfull, 


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
            //doc.ColumnWidth = 1024;
            doc.Name = "FlowDoc";
            //doc.PageHeight = 600;
            //doc.PageWidth = 800;
            //doc.MinPageWidth = 800;

            //doc.PageHeight = 400;
            //doc.PageWidth = 250;
            //doc.MinPageWidth = 250;


            /* style for products table header, assigned via type + class selectors */

            System.Windows.Documents.Paragraph p = new System.Windows.Documents.Paragraph();

            Span s = new Span();

            s = new Span(new Run(CompanyName + "\n"));
            s.FontWeight = FontWeights.Bold;

            s.FontSize = 18;
            //s.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a1 = new Span();
            a1 = new Span(new Run("GSTIN: " + GSTIN));
            a1.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a2 = new Span();
            a2 = new Span(new Run(Address + "," + Address2 + "," + City + "-" + PinCode + "," + State));
            a2.FontSize = 10;
            a2.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a2Mob = new Span();
            a2Mob = new Span(new Run("Mob: " + Mob + "," + Phone));
            a2Mob.FontSize = 10;
            a2Mob.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a3 = new Span();
            a3 = new Span(new Run("Estimation"));
            a3.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a4 = new Span();
            a4 = new Span(new Run("EST# " + invoiceNumber.Text));
            a4.FontWeight = FontWeights.Bold;
            a4.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a4acc = new Span();
            a4acc = new Span(new Run(autocompltCustName.autoTextBox.Text + " : " + CashCustName.Text));
            a4acc.FontWeight = FontWeights.Bold;
            a4acc.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span a4date = new Span();
            a4date = new Span(new Run("Date: " + invDate.Text));
            a4date.FontWeight = FontWeights.Bold;
            //a4date.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span a4trans = new Span();
            a4trans = new Span(new Run("Through: " + deliveryBy.Text + "-" + transportName.Text));
            a4trans.Inlines.Add(new LineBreak());//Line break is used for next line.  


            Span a5 = new Span();
            a5 = new Span(new Run("-----------------------------------------------------------"));
            //a5.Inlines.Add(new LineBreak());//Line break is used for next line.  
            p.FontSize = 11;
            p.Inlines.Add(a3);// Add the span content into paragraph.  
            p.Inlines.Add(s);// Add the span content into paragraph.  
            //p.Inlines.Add(a1);// Add the span content into paragraph.  
            p.Inlines.Add(a2);// Add the span content into paragraph. 
            p.Inlines.Add(a2Mob);
            //p.Inlines.Add(a1);// Add the span content into paragraph. 

            //p.Inlines.Add(a3);// Add the span content into paragraph.  
            p.Inlines.Add(a4);// Add the span content into paragraph.  
            p.Inlines.Add(a4acc);// Add the span content into paragraph.  
            p.Inlines.Add(a4date);// Add the span content into paragraph.  
            if (transportName.Text.Trim() != "")
            {
                p.Inlines.Add(a4trans);// Add the span content into paragraph. 
            }
            //If we have some dynamic text the span in flow document does not under "    " as space and we need to use "\t"  for space.  
            // s = new Span(new Run(s1 + "\t" + s2));//we need to use \t for space between s1 and s2 content.  
            //s.Inlines.Add(new LineBreak());
            //p.Inlines.Add(s);
            //Give style and formatting to paragraph content.  
            p.FontSize = 11;
            p.FontStyle = FontStyles.Normal;
            p.TextAlignment = TextAlignment.Center;
            p.FontFamily = new FontFamily("Century Gothic");
            p.BorderBrush = Brushes.Black;

            ThicknessConverter tc10010 = new ThicknessConverter();
            p.BorderThickness = (Thickness)tc10010.ConvertFromString("0.0002in");
            p.Margin = new Thickness(0);
            doc.Blocks.Add(p);

            System.Windows.Documents.Table t5 = new System.Windows.Documents.Table();
            t5.Margin = new Thickness(0);

            for (int i = 0; i < CartGrid.Items.Count; i++)
            {
                //TableColumn tc = new TableColumn();

                t5.Columns.Add(new TableColumn() { Width = GridLength.Auto });

            }

            ThicknessConverter tc1 = new ThicknessConverter();
            //// Create Table Borders
            t5.BorderThickness = (Thickness)tc1.ConvertFromString("0.02in");

            int count1 = CartGrid.Items.Count;
            var rg1 = new TableRowGroup();

            TableRow rowheadertable1 = new TableRow();



            rowheadertable1.Background = Brushes.Silver;
            rowheadertable1.FontSize = 11;

            rowheadertable1.FontFamily = new FontFamily("Century Gothic");
            rowheadertable1.FontWeight = FontWeights.Bold;

            ThicknessConverter tc222 = new ThicknessConverter();

            TableCell tcellfirstsR = new TableCell(new System.Windows.Documents.Paragraph(new Run("Sr")));
            //tcellfirstsR.ColumnSpan = 3;
            tcellfirstsR.BorderBrush = Brushes.Black;
            tcellfirstsR.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcellfirstsR);

            TableCell tcellfirst = new TableCell(new System.Windows.Documents.Paragraph(new Run("Product")));
            tcellfirst.ColumnSpan = 4;
            tcellfirst.BorderBrush = Brushes.Black;
            tcellfirst.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcellfirst);

            //TableCell tcell2 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Desc")));
            ////tcell2.ColumnSpan = 3;
            //tcell2.BorderBrush = Brushes.Black;
            //tcell2.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell2);

            TableCell tcell3 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Qty")));
            //tcell3.ColumnSpan = 3;
            tcell3.BorderBrush = Brushes.Black;
            tcell3.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell3);

            TableCell tcell4 = new TableCell(new System.Windows.Documents.Paragraph(new Run("UOM")));
            //tcell4.ColumnSpan = 3;
            tcell4.BorderBrush = Brushes.Black;
            tcell4.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell4);

            //TableCell tcell5 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Waste(%)")));
            ////tcell5.ColumnSpan = 3;
            //tcell5.BorderBrush = Brushes.Black;
            //tcell5.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell5);

            //TableCell tcell6 = new TableCell(new System.Windows.Documents.Paragraph(new Run("TotalWt")));
            ////tcell6.ColumnSpan = 3;
            //tcell6.BorderBrush = Brushes.Black;
            //tcell6.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell6);

            //TableCell tcell7 = new TableCell(new System.Windows.Documents.Paragraph(new Run("MC")));
            ////tcell7.ColumnSpan = 3;
            //tcell7.BorderBrush = Brushes.Black;
            //tcell7.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell7);

            TableCell tcell8 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Price")));
            //tcell8.ColumnSpan = 3;
            tcell8.BorderBrush = Brushes.Black;
            tcell8.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell8);

            TableCell tcell9 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Amt")));
            //tcell9.ColumnSpan = 3;
            tcell9.BorderBrush = Brushes.Black;
            tcell9.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell9);

            //TableCell tcell10 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Disc%"))); //Disc%
            ////tcell10.ColumnSpan = 3;
            //tcell10.BorderBrush = Brushes.Black;
            //tcell10.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell10);

            //TableCell tcell11 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Total")));
            ////tcell11.ColumnSpan = 3;
            //tcell11.BorderBrush = Brushes.Black;
            //tcell11.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell11);


            //rowheadertable1.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("HSN"))));

            //rowheadertable1.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Qty"))));

            //rowheadertable1.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Wt"))));

            //rowheadertable1.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Waste(%)"))));

            //rowheadertable1.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("TotalWt"))));

            //rowheadertable1.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("MC"))));

            //rowheadertable1.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Price"))));

            //rowheadertable1.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Amt"))));

            //rowheadertable1.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Disc%"))));

            //rowheadertable1.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Amount"))));

            //rowheadertable1.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("GST%"))));
            //rowheadertable1.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Tax"))));
            //rowheadertable1.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Total"))));

            rg1.Rows.Add(rowheadertable1);


            SqlConnection conpdf = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conpdf.Open();
            /// With Dscount ///string sqlpdf = "SELECT row_number() OVER (order by srnumber ) Sr ,(Ltrim(rtrim([ItemName]))+'-'+Ltrim(rtrim([Itemdesc]))) As [Item Name] , [BilledQty] As [Qty] ,[UnitID] As [UOM],[SalePrice] As [Price],Amount ,[Discount] As [Disc(%)] ,[TotalAmount] As [Total]   FROM [SalesVoucherInventory] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "'";

            //without Disc
            string sqlpdf = "SELECT row_number() OVER (order by srnumber ) Sr ,(Ltrim(rtrim([ItemName]))+'-'+Ltrim(rtrim([Itemdesc]))) As [Item Name] , [BilledQty] As [Qty] ,[UnitID] As [UOM],[SalePrice] As [Price],Amount  FROM [SalesVoucherInventory] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "'";

            SqlCommand cmdpdf = new SqlCommand(sqlpdf);
            cmdpdf.Connection = conpdf;
            SqlDataAdapter sda = new SqlDataAdapter(cmdpdf);
            DataTable dttable = new DataTable("Inv");
            sda.Fill(dttable);

            //foreach (DataColumn c in dttable.Columns)
            //{

            //    table.AddCell(new Phrase(c.ColumnName, tablefontsizeHeader));
            //}





            //for (int rows = 0; rows < dttable.Rows.Count; rows++)
            //{
            //    for (int column = 0; column < dttable.Columns.Count; column++)
            //    {
            //        //if (dttable.Rows[rows][column].ToString() != "0")
            //        //{
            //        PdfPCellhsn = new PdfPCell(new Phrase(new Chunk(dttable.Rows[rows][column].ToString(), tablefontsize)));
            //            table.AddCell(PdfPCellhsn);
            //    }

            //}







            IEnumerable itemsSource1 = CartGrid.ItemsSource as IEnumerable;
            if (itemsSource1 != null)
            {

                // foreach (var item in itemsSource)
                for (int k = 0; k < dttable.Rows.Count; ++k)
                {
                    TableRow rowone = new TableRow();

                    // rowone.Background = Brushes.Silver;
                    rowone.FontSize = 12;
                    rowone.FontWeight = FontWeights.Bold;

                    rowone.FontFamily = new FontFamily("Century Gothic");
                    // rowone.Background = Brushes.LightSalmon;
                    //for (int column = 0; column < dttable.Columns.Count; column++)
                    for (int i = 0; i < dttable.Columns.Count; i++)
                    {




                        if (i == 1)
                        {
                            TableCell firstcolproductcell = new TableCell(new System.Windows.Documents.Paragraph(new Run(dttable.Rows[k][i].ToString())));
                            firstcolproductcell.ColumnSpan = 4;
                            firstcolproductcell.BorderBrush = Brushes.Black;
                            firstcolproductcell.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
                            //rowone.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run((k + 1).ToString()))));
                            rowone.Cells.Add(firstcolproductcell);
                        }
                        else
                        {
                            TableCell txtcellall = new TableCell(new System.Windows.Documents.Paragraph(new Run(dttable.Rows[k][i].ToString())));
                            txtcellall.BorderBrush = Brushes.Black;
                            txtcellall.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
                            rowone.Cells.Add(txtcellall);
                            //table.AddCell(new Phrase(txt.Text, tablefontsize));
                        };
                    }
                    rg1.Rows.Add(rowone);
                }

            }











            //----------------

            t5.CellSpacing = 0;

            t5.RowGroups.Add(rg1);
            t5.TextAlignment = TextAlignment.Center;
            doc.Blocks.Add(t5);

            System.Windows.Documents.Paragraph totalHorizontalCTN = new System.Windows.Documents.Paragraph();
            Span tsctn = new Span();
            //tsctn = new Span(new Run("Total CTN: " + totalCTN.ToString() + ",   Total Qty:" + totalQuanty.ToString()));
            tsctn = new Span(new Run("Total Qty:" + totalQuanty.ToString() + "         "));

            totalHorizontalCTN.TextAlignment = TextAlignment.Right;
            totalHorizontalCTN.FontFamily = new FontFamily("Century Gothic");
            totalHorizontalCTN.FontSize = 11;
            totalHorizontalCTN.Inlines.Add(tsctn);// Add the span content into paragraph.  
            doc.Blocks.Add(totalHorizontalCTN);

            System.Windows.Documents.Paragraph totalValParag = new System.Windows.Documents.Paragraph();



            Span ts = new Span();
            //ts = new Span(new Run("\t" + " " + lbTotalTax.Content + "    " + lbTotal.Content));

            ts = new Span(new Run("\n Total Amount: ₹ " + totalBeforeItemDiscount.ToString() + "         "));


            Span ts1 = new Span();
            //ts = new Span(new Run("\t" + " " + lbTotalTax.Content + "    " + lbTotal.Content));

            ts1 = new Span(new Run("\n " + lblTotalDiscByItem.Content + "         "));

            //ts.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span totaltaxableamt1 = new Span();
            totaltaxableamt1 = new Span(new Run("\t ₹ " + "                          " + totalTaxableValues.ToString()));
            totaltaxableamt1.Inlines.Add(new LineBreak());//Line break is used for next line. 

            totalValParag.TextAlignment = TextAlignment.Right;
            totalValParag.FontFamily = new FontFamily("Century Gothic");
            totalValParag.FontSize = 12;
            //totalValParag.Inlines.Add(ts);// Add the span content into paragraph.  
            //totalValParag.Inlines.Add(ts1);// Add the span content into paragraph. 
            //totalValParag.Inlines.Add(totaltaxableamt1);// Add the span content into paragraph. 

            //totalVal.Inlines.Add(ali5);// Add the span content into paragraph.  

            // doc.Blocks.Add(totalValParag);






            System.Windows.Documents.Paragraph linedot = new System.Windows.Documents.Paragraph();

            System.Windows.Documents.Paragraph totalValold = new System.Windows.Documents.Paragraph();
            //totalValold.FontFamily 
            //Span ts1 = new Span();
            //ts1 = new Span(new Run("\t" + "(-) Old " + lbOldTotal.Content));

            //ts1.Inlines.Add(new LineBreak());//Line break is used for next line.  

            totalValold.FontSize = 12;

            //totalValold.Inlines.Add(ts1);// Add the span content into paragraph.  
            totalValold.FontFamily = new FontFamily("Century Gothic");
            //totalVal.Inlines.Add(ali5);// Add the span content into paragraph.  
            totalValold.TextAlignment = TextAlignment.Right;
            if (oldtotalVal > 0)
            {
                doc.Blocks.Add(totalValold);
            }
            Span linebrktble = new Span();
            linebrktble = new Span(new Run("-----------------------------------------------------"));
            // linebrktble.Inlines.Add(new LineBreak());//Line break is used for next line.  

            linedot.Inlines.Add(linebrktble);// Add the span content into paragraph. 
            linedot.TextAlignment = TextAlignment.Center;
            //doc.Blocks.Add(linedot);



            System.Windows.Documents.Paragraph totalVaGrand = new System.Windows.Documents.Paragraph();
            //totalValold.FontFamily 

            Span ts11g = new Span();

            ts11g = new Span(new Run("" + lbGrandTotal.Content + "         "));
            ts11g.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span ts111g = new Span();
            ts111g = new Span(new Run("\t" + "Flat Off: -₹" + flatOff.Text + "         "));
            ts111g.Inlines.Add(new LineBreak());//Line break is used for next line.  
            double flatoff = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());
            //string grandvalueafterDisc = Math.Round((totalVal - oldtotalVal - flatoff), 0).ToString();

            Span tspackcharge = new Span();
            tspackcharge = new Span(new Run("\t" + "Pack&GST: ₹" + PackCharge.Text + "         "));
            tspackcharge.Inlines.Add(new LineBreak());//Line break is used for next line.  
            double dPackCharge = (PackCharge.Text.Trim() == "") ? 0 : Convert.ToDouble(PackCharge.Text.Trim());
            string grandvalueafterDisc = Math.Round((totalVal - oldtotalVal - flatoff + dPackCharge), 0).ToString();



            Span ts1112g = new Span();
            ts1112g = new Span(new Run("\t" + "Pay: ₹ " + grandvalueafterDisc + "         "));
            ts1112g.Inlines.Add(new LineBreak());//Line break is used for next line.  

            totalVaGrand.FontSize = 12;
            totalVaGrand.FontFamily = new FontFamily("Century Gothic");
            totalVaGrand.Inlines.Add(ts11g);// Add the span content into paragraph. 
            if (flatoff > 0 && dPackCharge.Equals(0))
            {
                totalVaGrand.Inlines.Add(ts111g);
                totalVaGrand.Inlines.Add(ts1112g);
            }
            if (dPackCharge > 0 && flatoff.Equals(0))
            {
                totalVaGrand.Inlines.Add(tspackcharge);
                totalVaGrand.Inlines.Add(ts1112g);
            }
            if (dPackCharge > 0 && flatoff > 0)
            {
                totalVaGrand.Inlines.Add(ts111g);
                totalVaGrand.Inlines.Add(tspackcharge);
                totalVaGrand.Inlines.Add(ts1112g);
            }

            //totalVal.Inlines.Add(ali5);// Add the span content into paragraph.  
            totalVaGrand.TextAlignment = TextAlignment.Right;

            totalVaGrand.FontWeight = FontWeights.Bold;
            doc.Blocks.Add(totalVaGrand);


            //doc.Blocks.Add(linedot);

            System.Windows.Documents.Paragraph signpara = new System.Windows.Documents.Paragraph();

            Span linebrktble1 = new Span();
            linebrktble1 = new Span(new Run(narration.Text.Trim()));
            // linebrktble.Inlines.Add(new LineBreak());//Line break is used for next line.  

            signpara.FontSize = 11;

            //signpara.Inlines.Add(linebrktble1);// Add the span content into paragraph.  
            signpara.TextAlignment = TextAlignment.Center;

            //linedot.Inlines.Add(linebrktble1);// Add the span content into paragraph.  
            //doc.Blocks.Add(linedot);
            // doc.Blocks.Add(signpara);


            doc.Name = "FlowDoc";
            ////doc.PageWidth = 900;
            // doc.PageWidth = 380;// 400;
            doc.PagePadding = new Thickness(15, 15, 10, 5); //v4 new Thickness(20, 20, 20, 5);//v3 //  doc.PagePadding = new Thickness(50, 30, 10, 5); //v3

            //doc.PagePadding = new Thickness(30, 20, 10, 5); //V2 
            // Create IDocumentPaginatorSource from FlowDocument
            // IDocumentPaginatorSource idpSource = doc;
            // Call PrintDocument method to send document to printer



            return doc;


        }

        public void CreateFlowDocumentReadyMadeWholeSale()
        {
            MessageBoxResult genResult = MessageBox.Show("Do you want to generate PDf invoice?", "PDF Invoice", MessageBoxButton.YesNo);
            if (genResult == MessageBoxResult.Yes)
            {

                DriveInfo[] allDrives = DriveInfo.GetDrives();
                foreach (DriveInfo d in allDrives)
                {
                    if (d.DriveType == DriveType.Removable)
                    {
                       

                //int firmGSTN = RTSJewelERP.ConfigClass.firmId;

                string BillDateInv = invDate.SelectedDate.ToString();

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

                // firmGSTIN = firmGSTIN.Trim().Substring(0, 2);
                // firmStateCode.Text = firmGSTIN;

                //add background image 
                string imageFilePath = @"c:\ViewBill\Logo\Logo4.jpg";
                iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageFilePath);
                //Resize image depend upon your need
                //For give the size to image
                jpg.ScaleToFit(100, 100);

                //If you want to choose image as background then,

                jpg.Alignment = iTextSharp.text.Image.UNDERLYING;
                //If you want to give absolute/specified fix position to image.
                jpg.SetAbsolutePosition(120, 320); // to set the logo at left top 


                //string imageFilePathLogo2 = @"c:\ViewBill\Logo\Logo2.jpg";
                //iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance(imageFilePathLogo2);
                ////Resize image depend upon your need
                ////For give the size to image
                //jpg2.ScaleToFit(50, 50);

                ////If you want to choose image as background then,

                //jpg2.Alignment = iTextSharp.text.Image.UNDERLYING;
                ////If you want to give absolute/specified fix position to image.
                //jpg2.SetAbsolutePosition(340, 535); // to set the logo at left top 



                ///
                // Font headerFONT = new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.BOLD, BaseColor.BLACK);
                Font allFONTsize = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD,  BaseColor.BLACK);
                Font forFontSize = new Font(Font.FontFamily.COURIER, 10f, Font.BOLD, BaseColor.BLACK);
                Font allFONTsizetotal = new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.BOLD, BaseColor.BLACK);
                // Font tinfont = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.NORMAL, BaseColor.BLACK);
                // Font dateInv = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.BLACK);
                //for table font 
                Font tablefontsize = new Font(Font.FontFamily.TIMES_ROMAN, 11f, Font.BOLD, BaseColor.BLACK);
                Font tablefontsizeHeader = new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.BOLD, BaseColor.BLACK);

                Font taxslabAmtFont = new Font(Font.FontFamily.TIMES_ROMAN, 6.5f, Font.BOLD, BaseColor.BLACK);
                Font termsFont = new Font(Font.FontFamily.TIMES_ROMAN, 4f, Font.BOLD, BaseColor.BLACK);
                Font BankDetailFont = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.BLACK);

                //PdfPTable table = new iTextSharp.text.pdf.PdfPTable(CartGrid.Columns.Count) { TotalWidth = 390, LockedWidth = true };




                Font smallfont = new Font(Font.FontFamily.TIMES_ROMAN, 5.5f, Font.BOLD, BaseColor.BLACK);

                double packCharge1 = (PackCharge.Text.Trim() == "") ? 0 : Convert.ToDouble(PackCharge.Text.Trim());
                double gsttaxVCharge1 = (GSTTaxVa.Text.Trim() == "") ? 0 : Convert.ToDouble(GSTTaxVa.Text.Trim());

                totalGrandInvValues = Math.Round((totalTaxableValues - oldtotalVal + packCharge1 + gsttaxVCharge1), 0);



                long rupeesFig = Convert.ToInt64(Math.Round((Convert.ToDouble(totalGrandInvValues)), 0));
                //long rupeesFig = Convert.ToInt64(Math.Round((Convert.ToDouble(totalInvValues)), 0));

                string reupeesWords = ConvertNumbertoWords(rupeesFig);

                Font WwordsFormat = new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.BOLD, BaseColor.BLACK);






                PdfPTable totalTableHorizontal = new iTextSharp.text.pdf.PdfPTable(5) { TotalWidth = 390, LockedWidth = true };

                float[] widthsTotalTableHzl = new float[] { 218, 28, 34, 38, 72 };
                totalTableHorizontal.SetWidths(widthsTotalTableHzl);
                totalTableHorizontal.AddCell(new Phrase("                              Total", allFONTsize));
                totalTableHorizontal.AddCell(new Phrase(Math.Round(totalQuanty, 2).ToString(), allFONTsize));
                totalTableHorizontal.AddCell("");
                totalTableHorizontal.AddCell(new Phrase(Math.Round(totalVal, 2).ToString(), allFONTsize));
                totalTableHorizontal.AddCell(new Phrase(Math.Round(discounttotalByItem, 2).ToString(), allFONTsize));
                totalTableHorizontal.AddCell(new Phrase(Math.Round(totalTaxableValues, 2).ToString(), allFONTsize));
                totalTableHorizontal.AddCell(new Phrase(Math.Round(Convert.ToDouble(totalTaxAmount), 2).ToString(), allFONTsize));


                // TotalTable Start Here 
                PdfPTable totalTable = new iTextSharp.text.pdf.PdfPTable(3) { TotalWidth = 390, LockedWidth = true };


                float[] widthsTotalTable = new float[] { 223, 97, 70 };
                totalTable.SetWidths(widthsTotalTable);

                string packingchargeVal = "";
                //Convert.ToInt32(shipValText.Text)
                if (PackCharge.Text.Trim() != "")
                {
                    packingchargeVal = Convert.ToInt32(PackCharge.Text).ToString();

                } 

                 string gsttaxchargeVal = "";
                //Convert.ToInt32(shipValText.Text)
                 if (GSTTaxVa.Text.Trim() != "")
                {
                    gsttaxchargeVal = Convert.ToInt32(GSTTaxVa.Text).ToString();

                }

                long rupeesFigVal = Convert.ToInt64(Math.Round((Convert.ToDouble(totalInvValues)), 0));

                string reupeesWordsVal = ConvertNumbertoWords(rupeesFig);

                Font WwordsFormatVal = new Font(Font.FontFamily.TIMES_ROMAN, 6f, Font.BOLD, BaseColor.BLACK);


                PdfPCell totalCellAlign = new PdfPCell();
                totalCellAlign.BorderWidthLeft = 0;
                PdfPCell totalCellAmtAlign = new PdfPCell();
                totalCellAmtAlign.BorderWidthRight = 0;
                PdfPCell bankInvTotal = new PdfPCell();
                // bankInvTotal.Colspan
                PdfPTable bankWordsAmtTbl = new iTextSharp.text.pdf.PdfPTable(1) { TotalWidth = 220, LockedWidth = true };
                bankWordsAmtTbl.DefaultCell.Border = 0;


                PdfPTable banktaxslabDetailsTable = new iTextSharp.text.pdf.PdfPTable(2) { TotalWidth = 220, LockedWidth = true };
                float[] banktaxslabwidths = new float[] { 70, 150 };
                banktaxslabDetailsTable.SetWidths(banktaxslabwidths);
                banktaxslabDetailsTable.DefaultCell.Border = 0;

                PdfPTable taxslavtbl = new iTextSharp.text.pdf.PdfPTable(3);
                taxslavtbl.DefaultCell.Border = 0;
                float[] widthtaxslabs = new float[] { 60, 45, 45 };
                taxslavtbl.SetWidths(widthtaxslabs);


                banktaxslabDetailsTable.AddCell(new Phrase("E. & O.E" + "\n", BankDetailFont));


                bankWordsAmtTbl.AddCell(new Phrase(" Amount Chargeable(in words): Indian Rupees " + reupeesWordsVal + " Only." + "\n", forFontSize));

                bankWordsAmtTbl.AddCell(banktaxslabDetailsTable);
                //bankWordsAmtTbl.AddCell(taxslavtbl);

                // bankWordsAmtTbl.AddCell(new Phrase("OUR BANK DETAILS" + "\n" + "A/C#: " + firAcccountNumb.Trim() + "\n" + firmBankName.Trim() + "\n" + "IFSC: " + firmIFSC.Trim() + "\n" + firmBankAddress.Trim(), BankDetailFont));
                bankWordsAmtTbl.DefaultCell.Rowspan = 2;
                totalTable.AddCell(bankWordsAmtTbl);

                PdfPTable totaltableVerticalalign = new iTextSharp.text.pdf.PdfPTable(1);
                totaltableVerticalalign.DefaultCell.Border = 0;
                totaltableVerticalalign.AddCell(new Phrase("Total:", allFONTsizetotal));
                if (!Math.Round(discounttotalByItem, 2).Equals(0.0))
                {
                    totaltableVerticalalign.AddCell(new Phrase("Discount:", allFONTsizetotal));
                }
               // totaltableVerticalalign.AddCell(new Phrase("Taxable Value:", allFONTsizetotal));
                //if (IState)
                //{
                //    totaltableVerticalalign.AddCell(new Phrase("CGST:", allFONTsizetotal));
                //}
                //if (IState)
                //{
                //    totaltableVerticalalign.AddCell(new Phrase("SGST:", allFONTsizetotal));
                //}
                //if (!IState)
                //{
                //    totaltableVerticalalign.AddCell(new Phrase("IGST:", allFONTsizetotal));
                //}
                if (gsttaxchargeVal != "")
                {
                    totaltableVerticalalign.AddCell(new Phrase("GST:", allFONTsizetotal));
                }


                if (packingchargeVal != "")
                {
                    totaltableVerticalalign.AddCell(new Phrase("Packing:", allFONTsizetotal));
                }


                Font colorHighlight = new Font(Font.FontFamily.TIMES_ROMAN, 11f, Font.BOLD, BaseColor.BLACK);

                totaltableVerticalalign.AddCell(new Phrase("Total Value:", colorHighlight));
                //if (oldtotalVal > 0)
                //{
                //    totaltableVerticalalign.AddCell(new Phrase("Old Item Value:", allFONTsizetotal));
                //    totaltableVerticalalign.AddCell(new Phrase("Grand Total:", allFONTsizetotal));
                //}
                totaltableVerticalalign.DefaultCell.Rowspan = 2;
                totaltableVerticalalign.DefaultCell.BorderWidthRight = 0;
                totaltableVerticalalign.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
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
                totalsumrupee.Add(new Phrase(Math.Round(totalVal, 0).ToString(), allFONTsizetotal));

                //Phrase discSumrupee = new Phrase("-" + " \u20B9 ", font3);
                //discSumrupee.Add(new Phrase(Math.Round(discounttotalByItem, 2).ToString(), allFONTsizetotal));

                //Phrase discSumrupee = new Phrase(Math.Round(discSum, 2).ToString(), allFONTsizetotal);
                //discSumrupee.Add(chunkRupee);
                //Phrase taxableSumrupee = new Phrase(" \u20B9 ", font3);
                //taxableSumrupee.Add(new Phrase(Math.Round(totalTaxableValues, 2).ToString(), allFONTsizetotal));

                //Phrase taxableSumrupee = new Phrase(Math.Round(taxableSum, 2).ToString(), allFONTsizetotal);
                //taxableSumrupee.Add(chunkRupee);


                //Phrase cGSTSumrupee = new Phrase(" \u20B9 ", font3);
                //Phrase sGSTSumrupee = new Phrase(" \u20B9 ", font3);
                //Phrase iGSTSumrupee = new Phrase(" \u20B9 ", font3);
                //if (IState)
                //{

                //    cGSTSumrupee.Add(new Phrase(Math.Round(totalTaxAmount / 2, 2).ToString(), allFONTsizetotal));
                //    sGSTSumrupee.Add(new Phrase(Math.Round(totalTaxAmount / 2, 2).ToString(), allFONTsizetotal));
                //}
                //else
                //{
                //    iGSTSumrupee.Add(new Phrase(Math.Round(totalTaxAmount, 2).ToString(), allFONTsizetotal));
                //}

                Phrase gstTaxValrupee = new Phrase(" \u20B9 ", font3);
                gstTaxValrupee.Add(new Phrase(gsttaxchargeVal, allFONTsizetotal));

                Phrase packingchargeValrupee = new Phrase(" \u20B9 ", font3);
                packingchargeValrupee.Add(new Phrase(packingchargeVal, allFONTsizetotal));



                double discountamount12 = (discountTxt.Text == "") ? 0.0 : (Convert.ToDouble(discountTxt.Text) * totalVal / 100);
                Phrase discAmountvalues = new Phrase(" \u20B9 ", font3);
                discAmountvalues.Add(new Phrase(Math.Round(discountamount12, 2).ToString(), allFONTsizetotal));

                Phrase totalInvValuerupee = new Phrase(" \u20B9 ", font3);
                //totalInvValuerupee.Add(new Phrase(Math.Round(Convert.ToDouble(totalInvValue), 0).ToString(), allFONTsizetotal));
                double dgsttaxams = (GSTTaxVa.Text.Trim() == "") ? 0 : Convert.ToDouble(GSTTaxVa.Text.Trim());
                double dpackchargeds = (PackCharge.Text.Trim() == "") ? 0 : Convert.ToDouble(PackCharge.Text.Trim());


                totalInvValuerupee.Add(new Phrase(Math.Round(totalInvValues + dgsttaxams + dpackchargeds, 0).ToString(), colorHighlight));

                Phrase totaloldrupees = new Phrase("-" + " \u20B9 ", font3);
                Phrase totalgrandtotalwithOld = new Phrase(" \u20B9 ", font3);

                //Phrase totalInvValuerupee = new Phrase(Math.Round((Convert.ToDouble(totalInvValue)), 0).ToString(), allFONTsizetotal);
                //totalInvValuerupee.Add(chunkRupee);


                totaltableVerticalalign1.AddCell(totalsumrupee);
                //if (!Math.Round(discounttotalByItem, 2).Equals(0.0))
                //{
                //    totaltableVerticalalign1.AddCell(discSumrupee);
                //}
                //totaltableVerticalalign1.AddCell(taxableSumrupee);
                //if (IState)
                //{
                //    totaltableVerticalalign1.AddCell(cGSTSumrupee);
                //}
                //if (IState)
                //{
                //    totaltableVerticalalign1.AddCell(sGSTSumrupee);
                //}
                //if (!IState)
                //{
                //    totaltableVerticalalign1.AddCell(iGSTSumrupee);
                //}
                if (gsttaxchargeVal != "")
                {
                    totaltableVerticalalign1.AddCell(gstTaxValrupee);
                }

                if (packingchargeVal != "")
                {
                    totaltableVerticalalign1.AddCell(packingchargeValrupee);
                }



                totaltableVerticalalign1.AddCell(totalInvValuerupee);
                //if (oldtotalVal > 0)
                //{
                //    totaltableVerticalalign1.AddCell(totaloldrupees);
                //    totaltableVerticalalign1.AddCell(totalgrandtotalwithOld);
                //}
                totaltableVerticalalign1.DefaultCell.Rowspan = 2;
                totaltableVerticalalign1.DefaultCell.BorderWidthLeft = 0;
                totaltableVerticalalign1.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                PdfPCell totaltableVerticalalignCell1 = new PdfPCell();
                totaltableVerticalalignCell1.BorderWidthLeft = 0;

                totaltableVerticalalignCell1.AddElement(totaltableVerticalalign1);

                //totalCellAmtAlign.AddElement(phrtt0l);
                //totalTable.AddCell(totalCellAmtAlign);
                totalTable.AddCell(totaltableVerticalalignCell);
                totalTable.AddCell(totaltableVerticalalignCell1);
                //totalTable.AddCell(new Phrase("Total:  " + "\n" + "Discount:  " + "\n" + "Taxable Value:  " + "\n" + "CGST:  "  + "\n" +"SGST:  "  + "\n" + "IGST:  "  + "\n" + "Pack&Ship Charge:  "  + "\n" + "Total Invoice Value:  " + "\n" + "", allFONTsize));
                Phrase phrttl = new Phrase(new Phrase(Math.Round(totalVal, 2).ToString() + "\n" + Math.Round(Convert.ToDouble(totalCGSTTax), 2).ToString() + "\n" + Math.Round(Convert.ToDouble(totalSGSTTax), 2).ToString() + "\n" + Math.Round(Convert.ToDouble(totalIGSTTax), 2).ToString() + "\n" + packingchargeVal + "\n" + Math.Round((Convert.ToDouble(totalInvValues)), 0) + "\n" + "", allFONTsize));
                totalCellAlign.AddElement(phrttl);



                PdfPTable bankseparateTax = new iTextSharp.text.pdf.PdfPTable(2) { TotalWidth = 390, LockedWidth = true };
                //PdfPCell separatetabletaxCell = new PdfPCell();

                float[] widthsBankTable = new float[] { 223, 167 };
                bankseparateTax.SetWidths(widthsBankTable);


                iTextSharp.text.Paragraph termdetails = new iTextSharp.text.Paragraph();
                Phrase term1phT = new Phrase("E. & O.E" + "\n", BankDetailFont);
                //termdetails.Add(term1phT); 
                // termdetails.Add(ourbankdetails1cell);
                Phrase term1ph = new Phrase(" ->All disbutes are subject to Chennai Jurisdiction" + "\n" + "->Goods once sold will not be taken back" + "\n" + "->Goods are despatched at buyers risk " + "\n" + "->GST Rules and Regulation are applicable" + "\n", termsFont);
                // termdetails.Add(term1ph);

                PdfPTable ourbankdetails1 = new iTextSharp.text.pdf.PdfPTable(1) { TotalWidth = 85, LockedWidth = true };
                //ourbankdetails1.DefaultCell.Border = 0;
                PdfPCell ourbankdetails1Cell = new PdfPCell();
                //ourbankdetails1Cell.Border = 0;
                ourbankdetails1.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                ourbankdetails1Cell.AddElement(new Phrase("Bank Details", taxslabAmtFont));
                ourbankdetails1Cell.AddElement(new Phrase("A/C#:" + AccNumber.Trim(), taxslabAmtFont));
                ourbankdetails1Cell.AddElement(new Phrase(BankName.Trim(), taxslabAmtFont));
                ourbankdetails1Cell.AddElement(new Phrase("IFSC:" + IFSC.Trim(), taxslabAmtFont));
                ourbankdetails1Cell.AddElement(new Phrase(BAddress.Trim(), taxslabAmtFont));
                ourbankdetails1Cell.HorizontalAlignment = Element.ALIGN_LEFT;
                ourbankdetails1.AddCell(ourbankdetails1Cell);
                //ourbankdetails1.DefaultCell.Rowspan = 2;
                ourbankdetails1.DefaultCell.BorderWidthRight = 0;
                ourbankdetails1.DefaultCell.BorderWidthBottom = 0;
                ourbankdetails1.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                PdfPCell ourbankdetails1cell = new PdfPCell();
                // ourbankdetails1cell.Border = 0;


                ourbankdetails1cell.AddElement(ourbankdetails1);





                PdfPTable ForFirm = new iTextSharp.text.pdf.PdfPTable(1) { TotalWidth = 120, LockedWidth = true };
                ForFirm.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                ForFirm.DefaultCell.BorderWidth = 0;
                PdfPCell ForFirmCell = new PdfPCell();
                Phrase FirmPhrs = new Phrase("for " + CompanyName + "\n" + " " + "\n" + "\n" + "\n" + "\n" + "Authorised Signatory", forFontSize);

                ForFirmCell.AddElement(FirmPhrs);
                ForFirmCell.BorderWidth = 0;
                ForFirmCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                ForFirm.AddCell(ForFirmCell);



                PdfPTable Firmdatetable = new iTextSharp.text.pdf.PdfPTable(1) { TotalWidth = 390, LockedWidth = true };


                PdfPTable dateInvoice = new iTextSharp.text.pdf.PdfPTable(1) { TotalWidth = 390, LockedWidth = true };
                //headerTable.WidthPercentage = 100;
                //dateInvoice.DefaultCell.Border = 0;
                dateInvoice.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                //PdfPCell FirmTitCell = new PdfPCell();

                iTextSharp.text.Paragraph dtinv = new iTextSharp.text.Paragraph();
                Font chunkInvDateInv = new Font(Font.FontFamily.TIMES_ROMAN, 11f, Font.BOLD, BaseColor.BLACK);

                Phrase pdtInv = new Phrase("ESTIMATE                                                                                    Date: " + invDate.Text + "\n     " + "No: " + invoiceNumber.Text.Trim() + "", chunkInvDateInv);
                dtinv.Add(pdtInv);
                dateInvoice.AddCell(dtinv);

                //Font colorHighlight = new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.BOLD, BaseColor.BLACK);// from 8

                Font colorHighlightfIRM = new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.BOLD, BaseColor.BLACK);// from 8

                PdfPTable headerTable = new iTextSharp.text.pdf.PdfPTable(2) { TotalWidth = 390, LockedWidth = true };

                //float[] widthsHeaderTbles = new float[] { 300,90 };
                //headerTable.SetWidths(widthsHeaderTbles);

                headerTable.DefaultCell.BorderWidthTop = 0;
                headerTable.DefaultCell.BorderWidthBottom = 0;
                //headerTable.DefaultCell.BorderWidthRight = 0;

                //headerTable.WidthPercentage = 100;
                //headerTable.DefaultCell.Border = 0;
                //var selectedValueDelivery = ((ComboBoxItem)deliveryBy.SelectedItem).Content.ToString();
                iTextSharp.text.Paragraph p2 = new iTextSharp.text.Paragraph();
                Font dateInv = new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.BOLD, BaseColor.BLACK);
                Phrase billedTo = new Phrase("Details of Receiver(Bill To/Ship To)" + "\n", dateInv);
                p2.Add(billedTo);
                //if (CashCustName != "")
                //{
                Phrase Sphpcust1 = new Phrase(autocompltCustName.AutoCompletTextCustNameText + "\n", colorHighlightfIRM);
                    p2.Add(Sphpcust1);

                    Phrase Sphpcust2 = new Phrase(CustAddress.Text + "," + CustMobNumber.Text + "\n", colorHighlightfIRM);
                    p2.Add(Sphpcust2);


                Phrase pfirm5emp = new Phrase("");
                p2.Add(pfirm5emp);

                iTextSharp.text.Paragraph p3 = new iTextSharp.text.Paragraph();
                //if (CashCustName == "")
                //{
                    Phrase phpcust2 = new Phrase("ORIGINAL/TRANSPORT/SUPPLIER COPY" + "\n", allFONTsize);
                    p3.Add(phpcust2);
                //}


                    Phrase shippedTo = new Phrase("Your Order: " + YourOrder.Text + "\n", allFONTsize);
                    p3.Add(shippedTo);

                    Phrase phpcust1 = new Phrase("Through: " + deliveryBy.Text + "-" + transportName.Text + "\n", allFONTsize);
                    p3.Add(phpcust1);

                    Phrase BStateCode = new Phrase("Total Parcels:" + totalParcel.Text + "\n", allFONTsize);
                    p3.Add(BStateCode);

                //}



                Phrase pfirmemp = new Phrase("");
                p3.Add(pfirmemp);




                headerTable.AddCell(p2);
                headerTable.AddCell(p3);


                float[] widthsTab = new float[] { 250, 140 };
                headerTable.SetWidths(widthsTab);





                FileStream fs = File.Open(d.Name + @"\" + "Bill-" + (invoiceNumber.Text).Trim() + "-" + autocompltCustName.autoTextBox.Text + ".pdf", FileMode.Create);
                //Remove all special character from textBoxCustName
                //FileStream fs = File.Open(@"C:\ViewBill\" + "Bill-" + (invoiceNumber.Text).Trim() + "-" + autocompltCustName.autoTextBox.Text + ".pdf", FileMode.Create);


                using (MemoryStream output = new MemoryStream())
                {
                    //uncomment back 
                    //Document document = new Document(iTextSharp.text.PageSize.A5, 2f, 2f, 15, 2f);
                    //1 inch = 72 points in itextsharp
                    Document document = new Document(iTextSharp.text.PageSize.A4, 90,90, 5, 245);


                    //PageSize.A5.rotate()
                    //    Document document = new Document(PageSize.A5.rotate(), 10, 10, 10, 10);
                    //PageSize.A5 generate a page which size is vertical half of A4

                    //PageSize.A5.rotate() generate a page which size is horizontal half of A4


                    //commented below for memort=y stream
                    PdfWriter writer = PdfWriter.GetInstance(document, fs);
                    //PdfWriter writer = PdfWriter.GetInstance(document, output);


                    //below line for header footer POC
                    //writer.PageEvent = new RTSJewelERP.ITextEvents()
                    //{

                    //    custName = autocompltCustName.autoTextBox.Text,
                    //    SelectedValueDelivery = "Van",
                    //    cashCredit = "Cash",
                    //    selecteValueParcels = totalParcel.Text,
                    //    transportName = transportName.Text,
                    //    printName = autocompltCustName.autoTextBox.Text,
                    //    mobCust = "8978634244",
                    //    addressCust = "23/263 , Perya Street, Chennai, tamilnadu ",
                    //    invoiceNumber = (invoiceNumber.Text).Trim(),
                    //    BillDate = invDate.Text,
                    //    //BillDate = InvdateValue,
                    //    //BillDate = invDate.SelectedDate.Value.ToString("dd/MM/yyyy"),
                    //    GSTIN = "33LJKDHJDHHFJF7J",
                    //    State = "TN",
                    //    StateCode = "33",
                    //    YourOrder = "",
                    //    CashCustName = CashCustName.Text

                    //};

                    //float sethght = document.PageSize.Height;

                    document.Open();



                    IEnumerable itemsSource = CartGrid.ItemsSource as IEnumerable;
                    if (itemsSource != null)
                    {
 

                        ///////////////Commented above code\\\

                        SqlConnection conpdf = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                        //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                        conpdf.Open();
                        string sqlpdf = "SELECT row_number() OVER (order by srnumber ) Sr ,[ItemName] As [Item Name],[Itemdesc] As Description ,BilledQty As Qty, UnitID As [UOM]   ,[SalePrice] As [Price],[TotalAmount] As [Total]   FROM [SalesVoucherInventory] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text + "'";
                        SqlCommand cmdpdf = new SqlCommand(sqlpdf);
                        cmdpdf.Connection = conpdf;
                        SqlDataAdapter sda = new SqlDataAdapter(cmdpdf);
                        DataTable dttable = new DataTable("Inv");
                        sda.Fill(dttable);

                        PdfPTable table = new iTextSharp.text.pdf.PdfPTable(dttable.Columns.Count) { TotalWidth = 390, LockedWidth = true };
                        float[] widths = new float[] { 20, 120, 120, 50,35, 45, 65}; //remove disc and taxable
                        table.SetWidths(widths);
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

                                if ((rows == dttable.Rows.Count - 1) && (column == dttable.Columns.Count - 1))
                                {


                                    //float totaltblHorizntal = totalTableHorizontal.TotalHeight;
                                    //float totalTableHight = totalTable.TotalHeight;
                                    //float ttlhght = table.TotalHeight;
                                    //// float footerTblehght = footerTable.TotalHeight;
                                    ////float bankseparateTaxheght = bankseparateTax.TotalHeight; 
                                    //float bankseparateTaxheght = 60;//60;
                                    //float footertablehght = 135;//189;
                                    //float maxhght = document.PageSize.Height;
                                    //float balancehght = maxhght - (ttlhght + footertablehght + bankseparateTaxheght + totalTableHight + totaltblHorizntal);

                                    //Phrase newPhrase = new Phrase("");
                                    //iTextSharp.text.pdf.PdfPCell newCell = new iTextSharp.text.pdf.PdfPCell(newPhrase);
                                    //newCell.FixedHeight = balancehght;
                                    ////table.AddCell(newCell);

                                    //PdfPCellhsn.FixedHeight = balancehght;
                                    //table.AddCell(PdfPCellhsn);



                                    /// below for testing
                                    float totaltblHorizntal = totalTableHorizontal.TotalHeight;
                                    float totalTableHight = totalTable.TotalHeight;
                                    float ttlhght = table.TotalHeight;
                                  
                                    float bankseparateTaxheght = 60;//60;
                                    float footertablehght = 180;//189;
                                    //float maxhght = document.PageSize.Height;
                                    float maxhght = 598; // uncomment above 
                                     float balancehght = maxhght - (ttlhght + footertablehght + bankseparateTaxheght + totalTableHight + totaltblHorizntal);
                                    //uncomment above for rework

                                    //float balancehght = maxhght - (ttlhght + bankseparateTaxheght + bankseparateTaxheght + totalTableHight + totaltblHorizntal + );

                                    Phrase newPhrase = new Phrase("");
                                    iTextSharp.text.pdf.PdfPCell newCell = new iTextSharp.text.pdf.PdfPCell(newPhrase);
                                    newCell.FixedHeight = balancehght;
                                    //table.AddCell(newCell);

                                    PdfPCellhsn.FixedHeight = balancehght;
                                    table.AddCell(PdfPCellhsn);
                                    /// 






















                                    ////

                                }
                                else
                                    table.AddCell(PdfPCellhsn);

                                //table.AddCell(PdfPCellhsn);
                                //}
                            }
                        }



                        bankseparateTax.AddCell("");
                        //bankseparateTax.AddCell("");
                        //bankseparateTax.AddCell(PdfTableHSN); //commented for Hitesh
                        // bankseparateTax.AddCell(PdfTableHSNcell);
                        //bankseparateTax.AddCell(bankDetails);
                        bankseparateTax.AddCell(ForFirm);


                        //Auto Increment invoice/quotation number
                        //int billquoteNo = Convert.ToInt32(billQuoteNumber) + 1;
                        //File.WriteAllText(@"c:\RTSProSoft\Database\BillNumber.txt", billquoteNo.ToString(), Encoding.UTF8);
                        document.Add(dateInvoice);
                        document.Add(headerTable);

                        document.Add(jpg);
                        //document.Add(jpg2);

                        document.Add(table);
                        //document.Add(totalTableHorizontal);

                        // document.Add(p);
                        document.Add(totalTable);

                        // document.Add(footerTable);
                        document.Add(bankseparateTax);

                        document.Close();

                        //commented for memory stream
                        writer.Close();

                        fs.Close();


                        //string fPath = @"C:\ViewBill\" + "Bill-" + invoiceNumber.Text + "-" + custName.Text + ".pdf";
                        //try
                        //{
                        //    using (Stream stream = new FileStream(fPath, FileMode.Open))
                        //    {
                        //        Process process = new Process();
                        //        process.StartInfo.UseShellExecute = true;
                        //        process.StartInfo.FileName = @"C:\ViewBill\" + "Bill-" + invoiceNumber.Text + "-" + custName.Text + ".pdf";
                        //        process.Start();
                        //        process.Close();
                        //    }
                        //}
                        //catch
                        //{
                        //    MessageBox.Show("PDf Bill is already opened, please close and try again");
                        //    //check here why it failed and ask user to retry if the file is in use.
                        //}



                        try
                        {
                            //Open RTSProSoft Folder On PDf button Click
                            Process process = new Process();
                            process.StartInfo.UseShellExecute = true;

                            process.StartInfo.FileName = d.Name + @"\" + "Bill-" + (invoiceNumber.Text).Trim() + "-" + autocompltCustName.autoTextBox.Text + ".pdf";
                            //process.StartInfo.FileName = @"C:\ViewBill\" + "Bill-" + (invoiceNumber.Text).Trim() + "-" + autocompltCustName.autoTextBox.Text + ".pdf";

                            process.Start();


                            ////Printer setting Change to A5
                            //PrinterSettings ps = new PrinterSettings();
                            //PrintDocument recordDoc = new PrintDocument();
                            //recordDoc.PrinterSettings = ps;


                            //IEnumerable<PaperSize> paperSizes = ps.PaperSizes.Cast<PaperSize>();
                            //PaperSize sizeA5 = paperSizes.First<PaperSize>(size => size.Kind == PaperKind.A5); // setting paper size to A5 size
                            //recordDoc.DefaultPageSettings.PaperSize = sizeA5;

                            //recordDoc.Print();
                            ////Printer setting  Change to A5 Close


                            process.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("In Procees. Start");
                        }


                    }
                    // return output.ToArray();
                }


            }

        }
                //}// main try close
                //catch (Exception exc)
                //{
                //    MessageBox.Show("Please check bill in RTSProSoft folder");
                //}
            } //confirmation message to generate PDF
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

        private void textBoxCustName_LostFocus(object sender, RoutedEventArgs e)
        {




            //if (Regex.IsMatch(textBoxCustName.Text.Trim(), @"^\d+$") || 1 == 1)
            //{


            //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //    SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            //    con.Open();
            //    string sql = "SELECT COUNT(*) From AccountsList where AcctName='" + textBoxCustName.Text.Trim() + "'";
            //    SqlCommand cmd = new SqlCommand(sql, con);
            //    cmd.Connection = con;
            //    cmd.Connection = con;
            //    int countRecDelDel = (int)cmd.ExecuteScalar();
            //    cmd.Connection.Close();
            //    if (countRecDelDel == 0)
            //    {
            //        MessageBoxResult result = MessageBox.Show("Customer Does Not Exist, Do you want to Add?", "Add Record", MessageBoxButton.YesNo);
            //        if (result == MessageBoxResult.Yes)
            //            MessageBox.Show("Show Popup");
            //    }
            //}
        }

        //private void resultStack_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    txtQty.Focus();
        //}

        private void txtDueBal_LostFocus(object sender, RoutedEventArgs e)
        {
            double cashreceived = (receivedCash.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCash.Text.Trim());
            double cardreceived = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());
            double paytmreceived = (receivedPaytm.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedPaytm.Text.Trim());
            double flatoff = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());

            double offerzone = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
            double loyaltycard = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());

            dueBal.Content = string.Format("Balance:  {0}", Math.Round((totalVal - oldtotalVal - (cashreceived + cardreceived + paytmreceived + flatoff + offerzone + loyaltycard)), 0)).ToString();
        }

        private void PackCharge_LostFocus(object sender, RoutedEventArgs e)
        {
            double packCharge = (PackCharge.Text.Trim() == "") ? 0 : Convert.ToDouble(PackCharge.Text.Trim());
            double gstPCharge = (GSTTaxVa.Text.Trim() == "") ? 0 : Convert.ToDouble(GSTTaxVa.Text.Trim());
            //double cardreceived = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());
            //double paytmreceived = (receivedPaytm.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedPaytm.Text.Trim());
            //double flatoff = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());

            //double offerzone = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
            //double loyaltycard = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());

            //lbGrandTotal.Content = string.Format("Grand Total: {0}", (Math.Round((totalTaxableValues - oldtotalVal), 0)).ToString("C"));
            lbGrandSum.Content = string.Format("Grand Sum: {0}", (Math.Round((totalTaxableValues - oldtotalVal + packCharge + gstPCharge), 0)).ToString("C"));
        }

        private void MoveToBill(string invnumbertxt)
        {
            CleanUp();

            //autocompltCustName.autoTextBox.Clear();
            //CashCustName.Clear();
            EwayNumbertxt.Clear();
            //VoucherNumber.Clear();
            invDate.SelectedDate = DateTime.Now;
            receivedCash.Clear();
            receivedCard.Clear();
            flatOff.Clear();
            receivedOffer.Clear();
            receivedLoyalty.Clear();
            receivedPaytm.Clear();

            //load data from DB into CartGrid
            //invoiceNumber.Text
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            conn.Open();
            string sqlother = "";

            sqlother = "select * from SalesVouchersOtherDetails where LTRIM(RTRIM(InvoiceNumber))='" + invnumbertxt + "' and CompID = '" + CompID + "'";


            //sqlother = "select * from SalesVouchersOtherDetails where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text + "' and CompID = '" + CompID + "'";

            SqlCommand cmdother = new SqlCommand(sqlother);
            cmdother.Connection = conn;
            SqlDataReader readerother = cmdother.ExecuteReader();

            long dVoucherNumber = 0;
            string AccountName = "Cash";
            string InvoiceNumber = "";
            string CashCustomerName = "";
            string EwayNumber = "";
            double CashPaid = 0;
            double CardPaid = 0;
            double FlatOff = 0;
            double Offer = 0;
            double LoyaltyAmt = 0;
            double PaytmOther = 0;
            string TransactionDate = "";
            double TotalBox = 0;
            double TotalQty = 0;
            double DueBalance = 0;
            double RoundOff = 0;
            double DiscountOnTotal = 0;
            double packingCharges = 0;


            while (readerother.Read())
            {
                AccountName = readerother.GetString(3).Trim();
                CashCustomerName = readerother.GetString(7).Trim();
                EwayNumber = readerother.GetString(8).Trim();
                dVoucherNumber = readerother.GetInt64(1);
                InvoiceNumber = readerother.GetString(5);
                CashPaid = readerother.GetDouble(9);
                CardPaid = readerother.GetDouble(10);
                FlatOff = readerother.GetDouble(11);
                Offer = readerother.GetDouble(12);
                LoyaltyAmt = readerother.GetDouble(13);
                PaytmOther = readerother.GetDouble(14);
                TransactionDate = readerother.GetDateTime(15).ToString();
                TotalBox = readerother.GetDouble(16);
                TotalQty = readerother.GetDouble(17);
                DueBalance = readerother.GetDouble(18);
                RoundOff = readerother.GetDouble(19);
                DiscountOnTotal = readerother.GetDouble(20);
                packingCharges = readerother.GetDouble(21);

                PackCharge.Text = packingCharges.ToString();
                autocompltCustName.autoTextBox.Text = AccountName;
                CashCustName.Text = CashCustomerName;
                EwayNumbertxt.Text = EwayNumber;
                VoucherNumber.Text = dVoucherNumber.ToString();
                invDate.Text = TransactionDate;
                receivedCash.Text = CashPaid.ToString();
                receivedCard.Text = CardPaid.ToString();
                flatOff.Text = FlatOff.ToString();
                receivedOffer.Text = Offer.ToString();
                receivedLoyalty.Text = LoyaltyAmt.ToString();
                receivedPaytm.Text = PaytmOther.ToString();

                dueBal.Content = string.Format("Balance: {0}", (DueBalance).ToString("C"));

                //we add the product to the Cart
                //ShoppingCart.Add(new Product()
                //{
                //    BilledWt = dbilledWts,
                //    ItemName = reader.GetString(0).Trim(),
                //    ItemPrice = dsaleprice,
                //    BilledQty = dbilledQty,
                //    WastagePerc = dWastePerc,
                //    MC = dmakingcharge,
                //    SaleDiscountPerc = ddisperc,
                //    GSTRate = dgstrate
                //});
                //BindDataGrid();

            }
            readerother.Close();



            //load data from DB into CartGrid
            //invoiceNumber.Text
            SqlConnection connExtra = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            connExtra.Open();
            string sqlotherExtra = "";

            sqlotherExtra = "select GSTTax from SalesVouchersExtraDetails where LTRIM(RTRIM(InvoiceNumber))='" + invnumbertxt + "' and CompID = '" + CompID + "'";


            //sqlother = "select * from SalesVouchersOtherDetails where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text + "' and CompID = '" + CompID + "'";

            SqlCommand cmdotherExtra = new SqlCommand(sqlotherExtra);
            cmdotherExtra.Connection = connExtra;
            SqlDataReader readerotherExtra = cmdotherExtra.ExecuteReader();



            double gstTaxvaExtra = 0;
            //double OfferExtra = 0;
            //double LoyaltyAmtExtra = 0;


            while (readerotherExtra.Read())
            {

                gstTaxvaExtra = readerotherExtra.GetDouble(0);

                GSTTaxVa.Text = gstTaxvaExtra.ToString();

            }
            readerotherExtra.Close();





            //string sql = "select ItemName,HSN,BilledQty,BilledWt,WastePerc,TotalBilledWt,MakingCharge,SalePrice,TotalAmount,Discount,TaxablelAmount,TotalAmount,GSTRate,GSTTax,TotalAmount from SalesVoucherInventory where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text + "' and CompID = '" + CompID + "'";
            string sql = "select ItemName,HSN,BilledQty,SalePrice,TotalAmount,Discount,TaxablelAmount,GSTRate,GSTTax,Amount,UnitID,Itemdesc from SalesVoucherInventory where LTRIM(RTRIM(InvoiceNumber))='" + invnumbertxt + "' and CompID = '" + CompID + "' order by srnumber ";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = conn;
            SqlDataReader reader = cmd.ExecuteReader();

            double dbilledQty = 0;
            //double dbilledWts = 0;
            //double dWastePerc = 0;
            //double dmakingcharge = 0;
            double dsaleprice = 0;
            double ddisperc = 0;
            string dCTNqty = "";
            int dgstrate = 0;

            while (reader.Read())
            {
                string itemnme = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "";
                dbilledQty = (reader["BilledQty"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                //dbilledWts = reader.GetDouble(3);
                //dWastePerc = reader.GetDouble(4);
                //dmakingcharge = reader.GetDouble(6);
                dsaleprice = (reader["SalePrice"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                ddisperc = (reader["Discount"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;
                dgstrate = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(7)) : 0;
                dCTNqty = (reader["Itemdesc"] != DBNull.Value) ? (reader.GetString(11).Trim()) : "";
                //we add the product to the Cart
                ShoppingCart.Add(new Product()
                {
                    ItemDesc = dCTNqty,
                    UnitID = (reader["UnitID"] != DBNull.Value) ? (reader.GetString(10).Trim()) : "Pc",
                    ItemName = reader.GetString(0).Trim(),
                    ItemPrice = dsaleprice,
                    BilledQty = dbilledQty,
                    //WastagePerc = dWastePerc,
                    //MC = dmakingcharge,
                    SaleDiscountPerc = ddisperc
                    //GSTRate = dgstrate
                });
                BindDataGrid();

            }
            reader.Close();

            autocompltCustName.autoTextBox.Focus();

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

            MoveToBill(invoiceNumber.Text.Trim());
        }

        public string ConvertNumbertoWords(long number)
        {
            if (number == 0) return "Zero";
            if (number < 0) return "minus " + ConvertNumbertoWords(Math.Abs(number));
            string words = "";
            if ((number / 100000) > 0)
            {
                words += ConvertNumbertoWords(number / 100000) + " Lakh ";
                number %= 100000;
            }
            if ((number / 1000000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000000) + " Lakhs ";
                number %= 1000000;
            }
            if ((number / 1000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000) + " Thousand ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += ConvertNumbertoWords(number / 100) + " Hundred ";
                number %= 100;
            }
            //if ((number / 10) > 0)  
            //{  
            // words += ConvertNumbertoWords(number / 10) + " Rupees ";  
            // number %= 10;  
            //}  
            if (number > 0)
            {
                if (words != "") words += "And ";
                var unitsMap = new[]   
        {  
            "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"  
        };
                var tensMap = new[]   
        {  
            "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"  
        };
                if (number < 20) words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0) words += " " + unitsMap[number % 10];
                }
            }
            return words;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            AddSundryDebtor asd = new AddSundryDebtor();
            asd.ShowDialog();
            autocompltCustName.autoTextBox.Focus();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            AddItem ai = new AddItem();
            ai.ShowDialog();
            autocompleteItemName.autoTextBox1.Focus();
        }

        private void infoitem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowItemInfo si = new ShowItemInfo();
            si.ShowDialog();
        }

        private void txtGSTRate_LostFocus(object sender, RoutedEventArgs e)
        {
            //txtGSTRate.Background = Brushes.BlueViolet;
            //txtGSTRate.Foreground = Brushes.White;
            //AddItemRow.Focus();
        }

        private void autocompleteItemName_LostFocus(object sender, RoutedEventArgs e)
        {
            autocompleteItemName.autoTextBox1.Background = Brushes.White;
            autocompleteItemName.autoTextBox1.Foreground = Brushes.Black;

            if (autocompltCustName.autoTextBox.Text.Trim() == "Card")
            {
                receivedCash.Clear();
                receivedCard.Text = Math.Round((totalTaxableValues - oldtotalVal), 0).ToString();
            }
            if (autocompltCustName.autoTextBox.Text.Trim() == "Cash")
            {
                receivedCard.Clear();
                receivedCash.Text = Math.Round((totalTaxableValues - oldtotalVal), 0).ToString();
            }

            if (autocompltCustName.autoTextBox.Text.Trim() != "Cash")
            {
                CashCustName.Visibility = Visibility.Collapsed;
                //CashName.Visibility = Visibility.Collapsed;

            }

            //invoiceNumber.Text = InvoiceNumber.ToString();
            //VoucherNumber.Text = voucherNumber.ToString();
            //If a product code is not empty we search the database
            if (Regex.IsMatch(autocompleteItemName.autoTextBox1.Text.Trim(), @"^\d+$") || 1 == 1)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select * from StockItemsByPC where ItemName = '" + autocompleteItemName.autoTextBox1.Text.Trim() + "' and CompID = '" + CompID + "'";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                tmpProduct = new Product();

                while (reader.Read())
                {
                    //string isSoldAlert = (reader["IsSoldFlag"] != DBNull.Value) ? (reader.GetBoolean(72).ToString()) : "False";
                    //if (isSoldAlert == "True")
                    //{
                    //    //MessageBox.Show("Item is Sold Out !");
                    //}
                    //else
                    //{

                    //var CustID = reader.GetValue(0).ToString();

                    tmpProduct.ItemName = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                    tmpProduct.PrintName = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                    tmpProduct.UnitID = (reader["UnitID"] != DBNull.Value) ? (reader.GetString(4)) : "Pc";
                    tmpProduct.ItemCode = (reader["ItemCode"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";

                    tmpProduct.HSN = "9503";  //HSN

                    tmpProduct.ItemDesc = (reader["ItemDesc"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                    tmpProduct.ItemBarCode = (reader["ItemBarCode"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
                    tmpProduct.ItemPrice = (reader["ItemPrice"] != DBNull.Value) ? (reader.GetDouble(9)) : 0;
                    tmpProduct.SetCriticalLevel = (reader["SetCriticalLevel"] != DBNull.Value) ? (reader.GetBoolean(12)) : false;
                    tmpProduct.SetDefaultStorageID = (reader["SetDefaultStorageID"] != DBNull.Value) ? (reader.GetInt32(14)) : 0;
                    tmpProduct.DecimalPlaces = (reader["DecimalPlaces"] != DBNull.Value) ? (reader.GetInt32(17)) : 0;
                    tmpProduct.IsBarcodeCreated = (reader["IsBarcodeCreated"] != DBNull.Value) ? (reader.GetBoolean(18)) : false;
                    tmpProduct.ItemPurchPrice = (reader["ItemPurchPrice"] != DBNull.Value) ? (reader.GetDouble(23)) : 0;
                    tmpProduct.ItemAlias = (reader["ItemAlias"] != DBNull.Value) ? (reader.GetString(30).Trim()) : "";
                    tmpProduct.UnderGroupID = (reader["UnderGroupID"] != DBNull.Value) ? (reader.GetInt64(32)) : 0;
                    tmpProduct.UnderSubGroupID = (reader["UnderSubGroupID"] != DBNull.Value) ? (reader.GetInt64(34)) : 0;
                    tmpProduct.ActualQty = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)) : 0;
                    tmpProduct.HSN = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
                    tmpProduct.GSTRate = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)) : 0;
                    tmpProduct.StorageID = (reader["StorageID"] != DBNull.Value) ? (reader.GetInt32(38)) : 0;
                    tmpProduct.TrayID = (reader["TrayID"] != DBNull.Value) ? (reader.GetInt32(39)) : 0;
                    tmpProduct.CounterID = (reader["CounterID"] != DBNull.Value) ? (reader.GetInt32(40)) : 0;
                    //tmpProduct.UpdateDate = reader.GetDateTime(44); //reader["UpdateDate"] != DBNull.Value) ? (reader.GetDateTime(44)) : "";  
                    tmpProduct.ActualWt = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)) : 0;
                    //tmpProduct.LastBuyDate = reader.GetDateTime(47); //(reader["LastBuyDate"] != DBNull.Value) ? (reader.GetDateTime(47) : "";
                    //tmpProduct.LastSaleDate = reader.GetDateTime(48);//(reader["LastSaleDate"] != DBNull.Value) ? (reader.GetDateTime(48) : "";
                    tmpProduct.LastSalePrice = (reader["LastSalePrice"] != DBNull.Value) ? (reader.GetDouble(50)) : 0;
                    tmpProduct.LastBuyPrice = (reader["LastBuyPrice"] != DBNull.Value) ? (reader.GetDouble(51)) : 0;

                    //HSN.Text = tmpProduct.HSN.ToString();
                    //txtPrice.Text = tmpProduct.ItemPrice.ToString();
                    //txtGSTRate.Text = tmpProduct.GSTRate.ToString();
                    //txtWeight.Text = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)).ToString().Trim() : "";
                    //cmbUnits.Text = = (reader["UnderGroupName"] != DBNull.Value) ? (reader.GetString(31)).ToString().Trim() : "";
                    HSN.Text = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
                    cmbUnits.Text = (tmpProduct.UnitID.ToString() != "") ? tmpProduct.UnitID.ToString() : "Pc";
                    //txtGSTRate.Text = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)).ToString().Trim() : "";
                    //autocompleteItemName.autoTextBox1.Text = tmpProduct.ItemBarCode.ToString();
                    //Get Counter , Tray and Storage Name by another call, get all count by sp or direct call for inventory 
                    cmbStorage.Text = (reader["StorageName"] != DBNull.Value) ? (reader.GetString(79).Trim()) : "";
                    //CounterName.Text = (reader["CounterName"] != DBNull.Value) ? (reader.GetString(80).Trim()) : "";
                    cmbTray.Text = (reader["TrayName"] != DBNull.Value) ? (reader.GetString(81).Trim()) : "";

                    //txtMC.Text = (reader["MakingCharge"] != DBNull.Value) ? (reader.GetDouble(94)).ToString().Trim() : "";
                    //txtPrice.Text = (reader["RatePerGm"] != DBNull.Value) ? (reader.GetDouble(95)).ToString().Trim() : "";
                    txtPrice.Text = (reader["ItemPrice"] != DBNull.Value) ? (reader.GetDouble(9)).ToString().Trim() : "";
                    //txtWaste.Text = (reader["WastagePerc"] != DBNull.Value) ? (reader.GetDouble(96)).ToString().Trim() : "";
                    BindStorageComboBox(tmpProduct.ItemName);
                }
                //}
                reader.Close();
            }
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

        //private void Barcode_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    autocompleteItemName.autoTextBox1.Clear();
        //    txtPrice.Clear();
        //    txtQty.Text = "1";
        //    HSN.Clear();
        //    cmbStorage.Clear();
        //    cmbTray.Clear();
        //    txtWeight.Clear();
        //    txtWaste.Clear();
        //    txtMC.Clear();
        //    txtDiscPerc.Clear();
        //    txtGSTRate.Clear();
        //    //string custnme = txtBarcode.Text;
        //    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
        //    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
        //    conn.Open();
        //    string sql = "select * from StockItems where LTRIM(RTRIM(ItemName)) = '" + txtBarCode.Text.Trim() + "'  and CompID = '" + CompID + "'";
        //    //string sql = "select * from AccountsMaster where Barcode = '" + txtBarcode.Text + "'";
        //    SqlCommand cmd = new SqlCommand(sql);
        //    cmd.Connection = conn;
        //    SqlDataReader reader = cmd.ExecuteReader();

        //    //string constr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\RTSProSoft\Database\InvWpf-Enhanced.accdb;";
        //    //OleDbConnection con = new OleDbConnection(constr);
        //    //string queryStr = @"select * from PurchaseInvoices where PartyName = '" + custnme + "'";
        //    //OleDbCommand command = new OleDbCommand(queryStr, con);
        //    //con.Open();
        //    //OleDbDataReader reader = command.ExecuteReader();

        //    while (reader.Read())
        //    {

        //        autocompleteItemName.autoTextBox1.Text = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
        //        //PrintName.Text = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
        //        //UnitID.Text = (reader["UnitID"] != DBNull.Value) ? (reader.GetInt32(4)).ToString().Trim() : "";
        //        //ItemCode.Text = (reader["ItemCode"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";
        //        string isSoldAlert = (reader["IsSoldFlag"] != DBNull.Value) ? (reader.GetBoolean(72).ToString()) : "False";
        //        if (isSoldAlert == "True")
        //        {
        //            //MessageBox.Show("Item is Sold Out !");

        //        }
        //        else
        //        {


        //            //ItemDesc.Text = (reader["ItemDesc"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
        //            //ItemBarCode.Text = (reader["ItemBarCode"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
        //            //txtPrice.Text = (reader["ItemPrice"] != DBNull.Value) ? (reader.GetDouble(9)).ToString().Trim() : "";
        //            //SetCriticalLevel.Text = (reader["SetCriticalLevel"] != DBNull.Value) ? (reader.GetBoolean(12)).ToString().Trim() : "false";
        //            //SetDefaultStorageID.Text = (reader["SetDefaultStorageID"] != DBNull.Value) ? (reader.GetInt32(14)).ToString().Trim() : "";
        //            //DecimalPlaces.Text = (reader["DecimalPlaces"] != DBNull.Value) ? (reader.GetInt32(17)).ToString().Trim() : "";
        //            //HSN.Text = (reader["IsBarcodeCreated"] != DBNull.Value) ? (reader.GetBoolean(18)).ToString().Trim() : "false";
        //            //ItemPurchPrice.Text = (reader["ItemPurchPrice"] != DBNull.Value) ? (reader.GetDouble(23)).ToString().Trim() : "";
        //            //ItemAlias.Text = (reader["ItemAlias"] != DBNull.Value) ? (reader.GetString(30).Trim()) : "";
        //            //get Group Name 
        //            //autocompleteItemNameStockGroup.autoTextBoxStockGroup.Text = (reader["UnderGroupID"] != DBNull.Value) ? (reader.GetInt64(32)).ToString().Trim() : "";
        //            //autocompleteItemNameStockSubGroup.autoTextBoxStockSubGroup.Text = (reader["UnderSubGroupID"] != DBNull.Value) ? (reader.GetInt64(34)).ToString().Trim() : "";
        //            //txtQty.Text = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)).ToString().Trim() : "";
        //            HSN.Text = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
        //            txtGSTRate.Text = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)).ToString().Trim() : "";
        //            //Get Name instead ID
        //            //cmbStorage.Text = (reader["StorageID"] != DBNull.Value) ? (reader.GetInt32(38)).ToString().Trim() : "";
        //            //cmbTray.Text = (reader["TrayID"] != DBNull.Value) ? (reader.GetInt32(39)).ToString().Trim() : "";
        //            //CounterName.Text = (reader["CounterID"] != DBNull.Value) ? (reader.GetInt32(40)).ToString().Trim() : "";
        //            //OpeningStock.Text = (reader["OpeningStock"] != DBNull.Value) ? (reader.GetDouble(41)).ToString().Trim() : "";
        //            //OpeningStockValue.Text = (reader["OpeningStockValue"] != DBNull.Value) ? (reader.GetDouble(42)).ToString().Trim() : "";
        //            //tmpProduct.UpdateDate = reader.GetDateTime(44); //reader["UpdateDate"] != DBNull.Value) ? (reader.GetDateTime(44)) : "";  
        //            txtWeight.Text = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)).ToString().Trim() : "";
        //            //tmpProduct.LastBuyDate = reader.GetDateTime(47); //(reader["LastBuyDate"] != DBNull.Value) ? (reader.GetDateTime(47) : "";
        //            //tmpProduct.LastSaleDate = reader.GetDateTime(48);//(reader["LastSaleDate"] != DBNull.Value) ? (reader.GetDateTime(48) : "";
        //            //CurrentStockValue.Text = (reader["CurrentStockValue"] != DBNull.Value) ? (reader.GetDouble(49)).ToString().Trim() : "";
        //            //LastSalePrice.Text = (reader["LastSalePrice"] != DBNull.Value) ? (reader.GetDouble(50)).ToString().Trim() : "";
        //            //LastBuyPrice.Text = (reader["LastBuyPrice"] != DBNull.Value) ? (reader.GetDouble(51)).ToString().Trim() : "";

        //            //OpeningStockWt.Text = (reader["OpeningStockWt"] != DBNull.Value) ? (reader.GetDouble(52)).ToString().Trim() : "";

        //            //HSN.Text = tmpProduct.HSN.ToString();
        //            //txtPrice.Text = tmpProduct.ItemPrice.ToString();
        //            //txtGSTRate.Text = tmpProduct.GSTRate.ToString();
        //            //autocompleteItemName.autoTextBox1.Text = tmpProduct.ItemBarCode.ToString();
        //            //Get Counter , Tray and Storage Name by another call, get all count by sp or direct call for inventory 

        //            cmbStorage.Text = (reader["StorageName"] != DBNull.Value) ? (reader.GetString(79).Trim()) : "";
        //            //CounterName.Text = (reader["CounterName"] != DBNull.Value) ? (reader.GetString(80).Trim()) : "";
        //            cmbTray.Text = (reader["TrayName"] != DBNull.Value) ? (reader.GetString(81).Trim()) : "";

        //            txtMC.Text = (reader["MakingCharge"] != DBNull.Value) ? (reader.GetDouble(94)).ToString().Trim() : "";
        //            txtPrice.Text = (reader["RatePerGm"] != DBNull.Value) ? (reader.GetDouble(95)).ToString().Trim() : "";
        //            txtWaste.Text = (reader["WastagePerc"] != DBNull.Value) ? (reader.GetDouble(96)).ToString().Trim() : "";
        //        }


        //        //autocompleteItemName.autoTextBox1.Focus();
        //    }
        //    reader.Close();
        //}

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

        private void autocompltCustName_LostFocus(object sender, RoutedEventArgs e)
        {
            CustAddress.Text = "";

            string GSTINAcct = "";
            string GSTINCompany = "";
            if (autocompltCustName.autoTextBox.Text.Trim() != "Cash")
            {
                CashCustName.Clear();
                CashCustName.Visibility = Visibility.Collapsed;
                //CashName.Visibility = Visibility.Collapsed;

            }
            else
            {
                //CashCustName.Text = "Customer Name";
                //CashName.Visibility = Visibility.Visible;
                CashCustName.Visibility = Visibility.Visible;
            }



            if (autocompltCustName.autoTextBox.Text == "Card")
            {
                receivedCash.Clear();
                receivedCard.Text = Math.Round((totalTaxableValues - oldtotalVal), 0).ToString();
            }
            if (autocompltCustName.autoTextBox.Text == "Cash")
            {
                receivedCard.Clear();
                receivedCash.Text = Math.Round((totalTaxableValues - oldtotalVal), 0).ToString();
            }


            SqlConnection myConnCustExistr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            myConnCustExistr.Open();
            string CountSVCustExts = "SELECT COUNT(*) From AccountsList  where AcctName = '" + autocompltCustName.autoTextBox.Text.Trim() + "' and CompID = '" + CompID + "'";
            // string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
            SqlCommand myCommandCustEx = new SqlCommand(CountSVCustExts, myConnCustExistr);
            myCommandCustEx.Connection = myConnCustExistr;

            //int countRec = myCommand.ExecuteNonQuery();
            int countRecCustEx = (int)myCommandCustEx.ExecuteScalar();
            myCommandCustEx.Connection.Close();
            if (countRecCustEx < 1)
            {
                MessageBoxResult result = MessageBox.Show("Do you want to add new customer ?", "Close Page", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    string custNameEntered = autocompltCustName.autoTextBox.Text.Trim();
                    AddInstantAccount hp = new AddInstantAccount(custNameEntered);
                    hp.ShowDialog();
                    autocompltCustName.autoTextBox.Focus();

                }
                if (result == MessageBoxResult.No)
                {
                    CustAddress.Clear();
                    //txtState.Clear();
                    //CustMobNumber.Clear();
                    //autocompltCustName.autoTextBoxCustNameBarcode.Focus();
                }
            }

            else
            {

                if (Regex.IsMatch(autocompltCustName.autoTextBox.Text.Trim(), @"^\d+$") || 1 == 1)
                {
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                    con.Open();
                    string sql = "select LTRIM(RTRIM(Address1))  + ' '+  LTRIM(RTRIM(Address2)) + '' +LTRIM(RTRIM(City)) +' ' + LTRIM(RTRIM(State)) + ' '+ LTRIM(RTRIM(Mobile1)) As [Address],AcctName,GSTIN,State,Mobile1,* from AccountsList where LTRIM(RTRIM(AcctName)) = '" + autocompltCustName.autoTextBox.Text.Trim() + "' and CompID = '" + CompID + "'";
                    SqlCommand cmd = new SqlCommand(sql);
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();

                    tmpProduct = new Product();

                    while (reader.Read())
                    {


                        //var CustID = reader.GetValue(0).ToString();

                        //tmpProduct.ItemName = (reader["AcctName"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "";
                        //string GSTINAcct = (reader["GSTIN"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "";
                        //txtGSTIN.Text = GSTINAcct;
                        //txtState.Text = (reader["State"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                        CustAddress.Text = (reader["Address"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "";

                    }
                    reader.Close();
                }


            }





            ////invoiceNumber.Text = InvoiceNumber.ToString();
            ////VoucherNumber.Text = voucherNumber.ToString();
            ////If a product code is not empty we search the database
            //if (Regex.IsMatch(autocompltCustName.autoTextBox.Text.Trim(), @"^\d+$") || 1 == 1)
            //{
            //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            //    con.Open();
            //    string sql = "select AcctName,GSTIN,* from AccountsList where LTRIM(RTRIM(AcctName)) = '" + autocompltCustName.autoTextBox.Text + "' and CompID = '" + CompID + "'";
            //    SqlCommand cmd = new SqlCommand(sql);
            //    cmd.Connection = con;
            //    SqlDataReader reader = cmd.ExecuteReader();

            //    tmpProduct = new Product();

            //    while (reader.Read())
            //    {


            //        //var CustID = reader.GetValue(0).ToString();

            //        //tmpProduct.ItemName = (reader["AcctName"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "";
            //        GSTINAcct = (reader["GSTIN"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "";

            //    }
            //    reader.Close();
            //}

            //SqlConnection conCmp = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            ////SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            //conCmp.Open();
            //string sqlCmp = "select top 1  CompanyName,GSTIN,* from Company where   CompanyID = '" + CompID + "'";
            //SqlCommand cmdCmp = new SqlCommand(sqlCmp);
            //cmdCmp.Connection = conCmp;
            //SqlDataReader readerCmp = cmdCmp.ExecuteReader();

            //while (readerCmp.Read())
            //{


            //    //var CustID = reader.GetValue(0).ToString();

            //    //tmpProduct.ItemName = (reader["AcctName"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "";
            //    GSTINCompany = (readerCmp["GSTIN"] != DBNull.Value) ? (readerCmp.GetString(1).Trim()) : "";

            //}
            //readerCmp.Close();

            //if (GSTINAcct != "")
            //{
            //    GSTINAcct = GSTINAcct.Substring(0, 2);
            //}
            //GSTINCompany = GSTINCompany.Substring(0, 2);
            //if (GSTINAcct != GSTINCompany)
            //{
            //    IState = false;
            //    stateCodeVal = GSTINAcct;
            //}
            //else
                IState = true;

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

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            HomePage hp = new HomePage();
            this.NavigationService.Navigate(hp);
        }

        private void invoiceNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.PageUp)
            {
                if (Convert.ToInt64(invoiceNumber.Text.Trim()) < InvoiceNumber)
                {
                    Int64 inpageup = (invoiceNumber.Text.Trim() != "") ? (Convert.ToInt64(invoiceNumber.Text.Trim()) + 1) : 0;
                    invoiceNumber.Text = inpageup.ToString();
                    VoucherNumber.Text = voucherNumber.ToString();
                    MoveToBill(inpageup.ToString());

                }
                if (Convert.ToInt64(invoiceNumber.Text.Trim()) == InvoiceNumber)
                {
                    autocompltCustName.autoTextBox.Text = "Cash";
                    autocompltCustName.autoTextBox.Focus();
                }
                e.Handled = true;
            }
            if (e.Key == Key.PageDown)
            {
                Int64 inpageup = (invoiceNumber.Text.Trim() != "") ? (Convert.ToInt64(invoiceNumber.Text.Trim()) - 1) : 0;
                invoiceNumber.Text = inpageup.ToString();
                MoveToBill(inpageup.ToString());
                e.Handled = true;
            }
        }


    }
}
