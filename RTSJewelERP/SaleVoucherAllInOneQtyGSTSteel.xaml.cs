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
    public partial class SaleVoucherAllInOneQtyGSTSteel : Page
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
        private Product tmpProduct = null;



        //Array of Cart items 
        private List<Product> ShoppingCart;
        private List<Product> OldCart;
        public SaleVoucherAllInOneQtyGSTSteel()
        {
        }
        public SaleVoucherAllInOneQtyGSTSteel(string invoiceNumberRef)
        {
            InitializeComponent();

            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window
            BindComboBoxUnits(cmbUnits);
            dueBal.Content = string.Format("Balance: {0}", (BalanceCRorDR).ToString("C"));


            //on the constructor of the class we create a new instance of the shooping cart
            ShoppingCart = new List<Product>();
            OldCart = new List<Product>();
            //autocompleteItemName.autoTextBox1.Focus();
            autocompltCustName.autoTextBox.Focus();

            //txtBarCode.Focus();


            if (invoiceNumberRef == "" || invoiceNumberRef == null)
            {
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
            else
            {
                invoiceNumber.Text = invoiceNumberRef.Trim();
                InvoiceNumber = Convert.ToInt64(invoiceNumber.Text.Trim());
                MoveToBill(invoiceNumberRef);

            }
        }

        /// <summary>
        /// Esc key close This window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleEsc(object sender, KeyEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show("Close the Voucher and Reopen if required");
            }
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

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
        }

        private void TextBoxCust_KeyUp(object sender, KeyEventArgs e)
        {
            if (autocompltCustName.autoTextBox.Text != "Cash")
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
            if (autocompltCustName.autoTextBox.Text != "Cash")
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


            string GSTINAcct = "";
            string GSTINCompany = "";
            if (autocompltCustName.autoTextBox.Text != "Cash")
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
                if (autocompltCustName.autoTextBox.Text.Trim() == "Cash")
                {
                    IState = true;
                }
            }
            else
                IState = true;




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
            if (autocompltCustName.autoTextBox.Text == "Card")
            {
                receivedCash.Clear();
                receivedCard.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            }
            if (autocompltCustName.autoTextBox.Text == "Cash")
            {
                receivedCard.Clear();
                receivedCash.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            }

            if (autocompleteItemName.autoTextBox1.Text.Trim() != "")
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
                    // ShoppingCart.RemoveAll(s => s.ItemName == tmpProduct.ItemName); // Remove Existing item if same barcode
                    //we add the product to the Cart
                    ShoppingCart.Add(new Product()
                    {
                        //Sr = i,
                        HSN = tmpProduct.HSN,
                        UnitID = tmpProduct.UnitID,
                        //ItemPrice = tmpProduct.ItemPrice,
                        //BilledQty = qty,
                        //BilledWt = (txtWeight.Text == "") ? 0.0 : Convert.ToDouble(txtWeight.Text),
                        ItemName = tmpProduct.ItemName,
                        ItemPrice = (txtPrice.Text == "") ? 0.0 : Convert.ToDouble(txtPrice.Text),//tmpProduct.ItemPrice, //Get from textbox if changed
                        BilledQty = (txtQty.Text == "") ? 0.0 : Convert.ToDouble(txtQty.Text),
                        //WastagePerc = (txtWaste.Text == "") ? 0.0 : Convert.ToDouble(txtWaste.Text),
                        //MC = (txtMC.Text == "") ? 0.0 : Convert.ToDouble(txtMC.Text),
                        SaleDiscountPerc = (txtDiscPerc.Text == "") ? 0.0 : Convert.ToDouble(txtDiscPerc.Text),
                        GSTRate = (txtGSTRate.Text == "") ? 0 : Convert.ToInt16(txtGSTRate.Text)
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
                    txtGSTRate.Text = string.Empty;
                    //txtMC.Text = string.Empty;
                    //txtWeight.Text = string.Empty;
                    //txtWaste.Text = string.Empty;
                    txtPrice.Text = string.Empty;
                    autocompleteItemName.autoTextBox1.Focus();

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
                        UnitID = (cmbUnits.Text != "") ? cmbUnits.Text : "Pc",
                        HSN = HSN.Text,
                        //Sr = i,
                        //ItemName = tmpProduct.ItemName,
                        //ItemPrice = tmpProduct.ItemPrice,
                        //BilledQty = qty,
                        //BilledWt = (txtWeight.Text == "") ? 0.0 : Convert.ToDouble(txtWeight.Text),
                        ItemName = autocompleteItemName.autoTextBox1.Text,// tmpProduct.ItemName,                                              
                        ItemPrice = (txtPrice.Text == "") ? 0.0 : Convert.ToDouble(txtPrice.Text),//tmpProduct.ItemPrice, //Get from textbox if changed
                        BilledQty = (txtQty.Text == "") ? 0.0 : Convert.ToDouble(txtQty.Text),
                        //WastagePerc = (txtWaste.Text == "") ? 0.0 : Convert.ToDouble(txtWaste.Text),
                        //MC = (txtMC.Text == "") ? 0.0 : Convert.ToDouble(txtMC.Text),
                        SaleDiscountPerc = (txtDiscPerc.Text == "") ? 0.0 : Convert.ToDouble(txtDiscPerc.Text),
                        GSTRate = (txtGSTRate.Text == "") ? 0 : Convert.ToInt16(txtGSTRate.Text)
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
                    txtGSTRate.Text = string.Empty;
                    //txtMC.Text = string.Empty;
                    //txtWeight.Text = string.Empty;
                    //txtWaste.Text = string.Empty;
                    txtPrice.Text = string.Empty;
                    autocompleteItemName.autoTextBox1.Focus();

                    //---------------Write Code Below to Add Item in StockItems Dynamically with minimum data, if some data not provided then send the item to Pending tasks

                }



            }

            else
            {
                MessageBox.Show("Product is Empty");
                autocompleteItemName.autoTextBox1.Focus();

            }

            //TxtProdCode.Focus();
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
                                HSN = s.HSN,
                                Qty = s.BilledQty,
                                UOM = s.UnitID,
                                //Wt = s.BilledWt,
                                //Wast = s.WastagePerc,
                                //TotalWt = Math.Round((s.BilledWt + (s.BilledWt * s.WastagePerc / 100)), 2),
                                //s.MC,
                                Price = s.ItemPrice,
                                Amount = Math.Round((s.BilledQty * s.ItemPrice), 2),
                                Disc = s.SaleDiscountPerc,
                                TaxableAmount = Math.Round(((s.BilledQty * s.ItemPrice)) - (((s.BilledQty * s.ItemPrice)) * s.SaleDiscountPerc / 100), 2),
                                GST = s.GSTRate,
                                Tax = Math.Round((((s.BilledQty * s.ItemPrice)) - (((s.BilledQty * s.ItemPrice)) * s.SaleDiscountPerc / 100)) * (s.GSTRate) / 100, 2),
                                Total = Math.Round((((s.BilledQty * s.ItemPrice)) - (((s.BilledQty * s.ItemPrice)) * s.SaleDiscountPerc / 100)) + ((((s.BilledQty * s.ItemPrice)) - (((s.BilledQty * s.ItemPrice)) * s.SaleDiscountPerc / 100)) * s.GSTRate / 100), 2)


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
            lbTotal.Content = string.Format("Total: {0}", ShoppingCart.Sum(x => (((x.BilledQty * x.ItemPrice)) - (((x.BilledQty * x.ItemPrice)) * x.SaleDiscountPerc / 100)) + ((((x.BilledQty * x.ItemPrice)) - (((x.BilledQty * x.ItemPrice)) * x.SaleDiscountPerc / 100)) * x.GSTRate / 100)).ToString("C"));
            totalVal = cartItems.Sum(x => x.Total);
            totalBeforeItemDiscount = cartItems.Sum(x => x.Amount);
            totalInvValues = cartItems.Sum(x => x.Total);
            totalTaxAmount = cartItems.Sum(x => x.Tax);
            totalQuanty = cartItems.Sum(x => x.Qty);
            totalTaxableValues = cartItems.Sum(x => x.TaxableAmount);
            discounttotalByItem = cartItems.Sum(x => (x.Disc * x.Amount / 100));
            //makingTotalCharge = cartItems.Sum(x => x.MC);



            //discounttotalval = cartItems.Sum(x => x.Disc);
            lbTotalTax.Content = string.Format("Tax: {0}", cartItems.Sum(x => x.Tax).ToString("C"));
            lbGrandTotal.Content = string.Format("Grand Total: {0}", (Math.Round((totalVal - oldtotalVal), 0)).ToString("C"));
            lblTotalDiscByItem.Content = string.Format("Discount: {0}", (discounttotalByItem).ToString("C"));
            if (autocompltCustName.autoTextBox.Text == "Card")
            {
                receivedCash.Clear();
                receivedCard.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            }
            if (autocompltCustName.autoTextBox.Text == "Cash")
            {
                receivedCard.Clear();
                receivedCash.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            }

            double cashreceived = (receivedCash.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCash.Text.Trim());
            double cardreceived = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());
            double paytmreceived = (receivedPaytm.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedPaytm.Text.Trim());
            double flatoff = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());

            double offerzone = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
            double loyaltycard = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());

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
            lblCustBalance.Content = "Balance ₹: ";
            //dueBal.Content = "Credit Bal:";
            dueBal.Content = string.Format("Balance: {0}", (0).ToString("C"));
            //lbTotalTax.Content = "Tax: ₹";
            lbTotalTax.Content = string.Format("Tax: {0}", (0).ToString("C"));
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
            //lbGrandTotalSum.Content = "Total: ₹ 0.00";
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
            //if (
            //    MessageBox.Show("Are you sure you want to remove this product from Cart", "Confirmation",
            //        MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            //{
            var row = GetParent<DataGridRow>((Button)sender);
            var index = CartGrid.Items.IndexOf(row.Item);
            if (ShoppingCart.Count > index)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure want to delete?", "Delete Record", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                    ShoppingCart.RemoveAt(index);
                autocompleteItemName.autoTextBox1.Focus();
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
            //}
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
                autocompltCustName.autoTextBox.Focus();
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
            if (autocompltCustName.autoTextBox.Text == "Card")
            {
                receivedCash.Clear();
                receivedCard.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            }
            if (autocompltCustName.autoTextBox.Text == "Cash")
            {
                receivedCard.Clear();
                receivedCash.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            }

            if (autocompltCustName.autoTextBox.Text != "Cash")
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
                    txtGSTRate.Text = tmpProduct.GSTRate.ToString();
                    autocompleteItemName.autoTextBox1.Text = tmpProduct.ItemBarCode.ToString();
                    //Get Counter , Tray and Storage Name by another call, get all count by sp or direct call for inventory 
                    //cmbUnits.Text = tmpProduct.UnitID.ToString();
                    cmbUnits.Text = (tmpProduct.UnitID.ToString() != "") ? tmpProduct.UnitID.ToString() : "Pc";
                    BindStorageComboBox(tmpProduct.ItemName);
                }

                reader.Close();
            }
        }


        private void textBoxCustName_TextChanged(object sender, TextChangedEventArgs e)
        {


            string GSTINAcct = "";
            string GSTINCompany = "";
            if (autocompltCustName.autoTextBox.Text != "Cash")
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

        private void SaveInv_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                string GSTINAcct = "";
                string GSTINCompany = "";
                if (autocompltCustName.autoTextBox.Text != "Cash")
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
                    if (autocompltCustName.autoTextBox.Text.Trim() == "Cash")
                    {
                        IState = true;
                    }
                }
                else
                    IState = true;



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

                    MessageBox.Show("Wrong Account Name, please select correct account name ");
                    //autocompltCustName.autoTextBoxCustNameBarcode.Focus();
                }

                else
                {
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
                        string CountSVEntryStr = "SELECT COUNT(*) From SalesVoucherInventoryByPC where InvoiceNumber='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "'";
                        // string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                        SqlCommand myCommandDel = new SqlCommand(CountSVEntryStr, myConnSVEntryStr);
                        myCommandDel.Connection = myConnSVEntryStr;

                        //int countRec = myCommand.ExecuteNonQuery();
                        int countRecDelDel = (int)myCommandDel.ExecuteScalar();
                        myCommandDel.Connection.Close();
                        if (countRecDelDel != 0)
                        {
                            // MessageBox.Show("Item Name is already Exist, Please delete existing", "Add Record");


                            SqlCommand myCommandDeleteDel = new SqlCommand("SPUpdateStockOnSalesVoucherChangeOrDelete", myConnSVEntryStr);
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

                                DataGridCell cellHSN = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(2);
                                TextBlock hsnText = cellHSN.Content as TextBlock;


                                // for Qty

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

                                DataGridCell gstRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(9);
                                TextBlock txtgstRate = gstRate.Content as TextBlock;

                                DataGridCell gstTax = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(10);
                                TextBlock txtgsTax = gstTax.Content as TextBlock;


                                DataGridCell cellTotal = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(11);
                                TextBlock totalText = cellTotal.Content as TextBlock;


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
                                querySalesInventory = "insert into SalesVoucherInventoryByPC(VoucherNumber, InvoiceNumber,ItemName,HSN,SalePrice,GSTRate,GSTTax,Discount,TaxablelAmount,TotalAmount, BilledQty,UnitID,TransactionDate,FromConsumedStorageID,FromConsumedTrayID,FromConsumedCounterID,CompID,Amount) Values ( '" + VoucherNumber.Text + "','" + invoiceNumber.Text.Trim() + "','" + txtItemNam.Text + "','" + hsnText.Text + "','" + priceText.Text + "','" + txtgstRate.Text + "','" + txtgsTax.Text + "','" + txtdiscRate.Text + "', '" + txtTaxableAmt.Text + "','" + totalText.Text + "','" + qtyText.Text + "', '" + txtcellUnitID.Text + "','" + InvdateValue + "','1','1','1', '" + CompID + "','" + txtCellAmount.Text + "')";



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
                                    string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPC where ItemName ='" + txtItemNam.Text.Trim() + "'  and CompID = '" + CompID + "'";
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
                                        queryStrStockCheck = "select * from StockItemsByPC where ItemName = '" + txtItemNam.Text.Trim() + "' and CompID = '" + CompID + "'";
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
                                        queryStrStockUpdate = "update StockItemsByPC  set UpdateDate='" + InvdateValue + "', IsSoldFlag='1'  ,ActualQty='" + balanceStk + "',ActualWt='" + balanceStkWt + "',LastSalePrice='" + priceText.Text + "'  where ItemName ='" + txtItemNam.Text + "'   and CompID = '" + CompID + "' ";
                                        if (txtItemNam.Text == "Old Gold" || txtItemNam.Text == "Old Silver")
                                        {
                                            queryStrStockUpdate = "update StockItemsByPC  set UpdateDate='" + InvdateValue + "' , ActualQty='" + balanceStk + "',ActualWt='" + balanceStkWt + "',LastBuyPrice='" + priceText.Text + "'  where ItemName ='" + txtItemNam.Text + "'   and CompID = '" + CompID + "' ";
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
                                        double qtyStkEntry = (txtQtyStockEntry.Text.Trim() == "") ? 0 : Convert.ToDouble(txtQtyStockEntry.Text.Trim());
                                        double qtyEntryInsertOpen = (txtQtyStockEntry.Text.Trim() == "") ? 0 : Convert.ToDouble(txtQtyStockEntry.Text.Trim());
                                        double qtyEntryInsertBill = (txtQty.Text.Trim() == "") ? 0 : Convert.ToDouble(txtQty.Text.Trim());
                                        //string hsnentryinsert = HSN.Text.Trim();
                                        string querySalesInvEntry = "";
                                        querySalesInvEntry = "insert into StockItemsByPC(ItemName, ActualQty,UnitID,ActualWt,ItemPrice,GSTRate,LastSalePrice,HSN,CompID) Values ( '" + txtItemNam.Text + "','" + (qtyEntryInsertOpen - qtyEntryInsertBill) + "','" + txtcellUnitID.Text + "','" + 0 + "','" + priceText.Text + "','" + txtgstRate.Text + "','" + priceText.Text + "','" + hsnText.Text + "', '" + CompID + "')";
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
                    if (CashCustName.Text != "")
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
                    double txtPackForwd = (txtPackForward.Text.Trim() == "") ? 0 : Convert.ToDouble(txtPackForward.Text.Trim());
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



                    SqlConnection conStrTaxTable = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                    conStrTaxTable.Open();
                    //string sql = "SELECT COUNT(*) From AccountsList where AcctName='" + textBoxAcctName.Text.Trim() + "'";
                    SqlCommand cmdTaxTable;//= new SqlCommand(sql, con);

                    cmdTaxTable = new SqlCommand("SPUpdateTaxDetailsForSaleVoucher", conStrCommon);
                    cmdTaxTable.CommandType = CommandType.StoredProcedure;
                    cmdTaxTable.Parameters.Add(new SqlParameter("@SundryDebtorName", autocompltCustName.autoTextBox.Text));

                    cmdTaxTable.Parameters.Add(new SqlParameter("@CustGSTIN", ""));
                    cmdTaxTable.Parameters.Add(new SqlParameter("@StatePlaceSupply", SaleAcctName));
                    if (CashCustName.Text != "")
                    {
                        cmdTaxTable.Parameters.Add(new SqlParameter("@CashCustomerName", CashCustName.Text));
                        //cmdTaxTable.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Cash"));
                    }
                    else
                    {
                        cmdTaxTable.Parameters.Add(new SqlParameter("@CashCustomerName", ""));
                        //cmdTaxTable.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Credit"));
                    }
                    cmdTaxTable.Parameters.Add(new SqlParameter("@InvoiceNumber", invoiceNumber.Text));
                    cmdTaxTable.Parameters.Add(new SqlParameter("@SaleVoucherNumber", Convert.ToInt64(VoucherNumber.Text.Trim())));
                    cmdTaxTable.Parameters.Add(new SqlParameter("@SaleVoucherType", "Sale Voucher"));

                    cmdTaxTable.Parameters.Add(new SqlParameter("@InvDate", BillDateInvValval));

                    //check isState or central with company statecode            
                    cmdTaxTable.Parameters.Add(new SqlParameter("@IsState", IState.ToString()));
                    discounttotalCommon = (discountTxt.Text.Trim() == "") ? 0 : Convert.ToDouble(discountTxt.Text.Trim());
                    //cmdTaxTable.Parameters.Add(new SqlParameter("@Discount", discounttotalCommon)); //gettotal Discount-Common 
                    if (IState)
                    {
                        double outputigstval = 0.0;
                        cmdTaxTable.Parameters.Add(new SqlParameter("@OutputCGST", totalTaxAmount / 2));
                        cmdTaxTable.Parameters.Add(new SqlParameter("@OutputSGST", totalTaxAmount / 2));
                        cmdTaxTable.Parameters.Add(new SqlParameter("@OutputIGST", outputigstval));
                    }
                    else
                    {
                        double outputsgstval = 0.0;


                        cmdTaxTable.Parameters.Add(new SqlParameter("@OutputCGST", outputsgstval));
                        cmdTaxTable.Parameters.Add(new SqlParameter("@OutputSGST", outputsgstval));
                        cmdTaxTable.Parameters.Add(new SqlParameter("@OutputIGST", totalTaxAmount));
                    }
                    BalanceCRorDR = Convert.ToDouble(((dueBal.Content.ToString()).Replace("₹", "").Split(':')[1]).Trim());

                    cmdTaxTable.Parameters.Add(new SqlParameter("@TotalInvValue", totalInvValues - oldtotalVal));
                    cmdTaxTable.Parameters.Add(new SqlParameter("@TotalTaxableValue", totalTaxableValues));
                    cmdTaxTable.Parameters.Add(new SqlParameter("@CompID", Convert.ToInt32(CompID)));
                    cmdTaxTable.Connection.Open();
                    cmdTaxTable.ExecuteNonQuery();
                    cmdTaxTable.Connection.Close();


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

                    //CreateFlowDocumentReadyMadeWholeSale();

                    SaleVoucherAllInOneQtyGSTSteel sv = new SaleVoucherAllInOneQtyGSTSteel("");
                    //SaleVoucherBarcode sv = new SaleVoucherBarcode();
                    this.NavigationService.Navigate(sv);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        /*
         * There will be 2 account in sales 1 Cash Sales  2 Credit Sales
         * 
         * */
        private void PrintSimpleTextButton_Click(object sender, RoutedEventArgs e)
        {

            string GSTINAcct = "";
            string GSTINCompany = "";
            if (autocompltCustName.autoTextBox.Text != "Cash")
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
                if (autocompltCustName.autoTextBox.Text.Trim() == "Cash")
                {
                    IState = true;
                }
            }
            else
                IState = true;



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

                MessageBox.Show("Wrong Account Name, please select correct account name ");
                //autocompltCustName.autoTextBoxCustNameBarcode.Focus();
            }

            else
            {
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
                    string CountSVEntryStr = "SELECT COUNT(*) From SalesVoucherInventoryByPC where InvoiceNumber='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "'";
                    // string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                    SqlCommand myCommandDel = new SqlCommand(CountSVEntryStr, myConnSVEntryStr);
                    myCommandDel.Connection = myConnSVEntryStr;

                    //int countRec = myCommand.ExecuteNonQuery();
                    int countRecDelDel = (int)myCommandDel.ExecuteScalar();
                    myCommandDel.Connection.Close();
                    if (countRecDelDel != 0)
                    {
                        // MessageBox.Show("Item Name is already Exist, Please delete existing", "Add Record");


                        SqlCommand myCommandDeleteDel = new SqlCommand("SPUpdateStockOnSalesVoucherChangeOrDelete", myConnSVEntryStr);
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

                            DataGridCell cellHSN = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(2);
                            TextBlock hsnText = cellHSN.Content as TextBlock;


                            // for Qty

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

                            DataGridCell gstRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(9);
                            TextBlock txtgstRate = gstRate.Content as TextBlock;

                            DataGridCell gstTax = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(10);
                            TextBlock txtgsTax = gstTax.Content as TextBlock;


                            DataGridCell cellTotal = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(11);
                            TextBlock totalText = cellTotal.Content as TextBlock;


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
                            querySalesInventory = "insert into SalesVoucherInventoryByPC(VoucherNumber, InvoiceNumber,ItemName,HSN,SalePrice,GSTRate,GSTTax,Discount,TaxablelAmount,TotalAmount, BilledQty,UnitID,TransactionDate,FromConsumedStorageID,FromConsumedTrayID,FromConsumedCounterID,CompID,Amount) Values ( '" + VoucherNumber.Text + "','" + invoiceNumber.Text.Trim() + "','" + txtItemNam.Text + "','" + hsnText.Text + "','" + priceText.Text + "','" + txtgstRate.Text + "','" + txtgsTax.Text + "','" + txtdiscRate.Text + "', '" + txtTaxableAmt.Text + "','" + totalText.Text + "','" + qtyText.Text + "', '" + txtcellUnitID.Text + "','" + InvdateValue + "','1','1','1', '" + CompID + "','" + txtCellAmount.Text + "')";



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
                                string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPC where ItemName ='" + txtItemNam.Text.Trim() + "'  and CompID = '" + CompID + "'";
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
                                    queryStrStockCheck = "select * from StockItemsByPC where ItemName = '" + txtItemNam.Text.Trim() + "' and CompID = '" + CompID + "'";
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
                                    queryStrStockUpdate = "update StockItemsByPC  set UpdateDate='" + InvdateValue + "', IsSoldFlag='1'  ,ActualQty='" + balanceStk + "',ActualWt='" + balanceStkWt + "',LastSalePrice='" + priceText.Text + "'  where ItemName ='" + txtItemNam.Text + "'   and CompID = '" + CompID + "' ";
                                    if (txtItemNam.Text == "Old Gold" || txtItemNam.Text == "Old Silver")
                                    {
                                        queryStrStockUpdate = "update StockItemsByPC  set UpdateDate='" + InvdateValue + "' , ActualQty='" + balanceStk + "',ActualWt='" + balanceStkWt + "',LastBuyPrice='" + priceText.Text + "'  where ItemName ='" + txtItemNam.Text + "'   and CompID = '" + CompID + "' ";
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
                                    double qtyStkEntry = (txtQtyStockEntry.Text.Trim() == "") ? 0 : Convert.ToDouble(txtQtyStockEntry.Text.Trim());
                                    double qtyEntryInsertOpen = (txtQtyStockEntry.Text.Trim() == "") ? 0 : Convert.ToDouble(txtQtyStockEntry.Text.Trim());
                                    double qtyEntryInsertBill = (txtQty.Text.Trim() == "") ? 0 : Convert.ToDouble(txtQty.Text.Trim());
                                    //string hsnentryinsert = HSN.Text.Trim();
                                    string querySalesInvEntry = "";
                                    querySalesInvEntry = "insert into StockItemsByPC(ItemName, ActualQty,UnitID,ActualWt,ItemPrice,GSTRate,LastSalePrice,HSN,CompID) Values ( '" + txtItemNam.Text + "','" + (qtyEntryInsertOpen - qtyEntryInsertBill) + "','" + txtcellUnitID.Text + "','" + 0 + "','" + priceText.Text + "','" + txtgstRate.Text + "','" + priceText.Text + "','" + hsnText.Text + "', '" + CompID + "')";
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
                if (CashCustName.Text != "")
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
                double txtPackForwd = (txtPackForward.Text.Trim() == "") ? 0 : Convert.ToDouble(txtPackForward.Text.Trim());
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



                SqlConnection conStrTaxTable = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                conStrTaxTable.Open();
                //string sql = "SELECT COUNT(*) From AccountsList where AcctName='" + textBoxAcctName.Text.Trim() + "'";
                SqlCommand cmdTaxTable;//= new SqlCommand(sql, con);

                cmdTaxTable = new SqlCommand("SPUpdateTaxDetailsForSaleVoucher", conStrCommon);
                cmdTaxTable.CommandType = CommandType.StoredProcedure;
                cmdTaxTable.Parameters.Add(new SqlParameter("@SundryDebtorName", autocompltCustName.autoTextBox.Text));

                cmdTaxTable.Parameters.Add(new SqlParameter("@CustGSTIN", ""));
                cmdTaxTable.Parameters.Add(new SqlParameter("@StatePlaceSupply", SaleAcctName));
                if (CashCustName.Text != "")
                {
                    cmdTaxTable.Parameters.Add(new SqlParameter("@CashCustomerName", CashCustName.Text));
                    //cmdTaxTable.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Cash"));
                }
                else
                {
                    cmdTaxTable.Parameters.Add(new SqlParameter("@CashCustomerName", ""));
                    //cmdTaxTable.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Credit"));
                }
                cmdTaxTable.Parameters.Add(new SqlParameter("@InvoiceNumber", invoiceNumber.Text));
                cmdTaxTable.Parameters.Add(new SqlParameter("@SaleVoucherNumber", Convert.ToInt64(VoucherNumber.Text.Trim())));
                cmdTaxTable.Parameters.Add(new SqlParameter("@SaleVoucherType", "Sale Voucher"));

                cmdTaxTable.Parameters.Add(new SqlParameter("@InvDate", BillDateInvValval));

                //check isState or central with company statecode            
                cmdTaxTable.Parameters.Add(new SqlParameter("@IsState", IState.ToString()));
                discounttotalCommon = (discountTxt.Text.Trim() == "") ? 0 : Convert.ToDouble(discountTxt.Text.Trim());
                //cmdTaxTable.Parameters.Add(new SqlParameter("@Discount", discounttotalCommon)); //gettotal Discount-Common 
                if (IState)
                {
                    double outputigstval = 0.0;
                    cmdTaxTable.Parameters.Add(new SqlParameter("@OutputCGST", totalTaxAmount / 2));
                    cmdTaxTable.Parameters.Add(new SqlParameter("@OutputSGST", totalTaxAmount / 2));
                    cmdTaxTable.Parameters.Add(new SqlParameter("@OutputIGST", outputigstval));
                }
                else
                {
                    double outputsgstval = 0.0;


                    cmdTaxTable.Parameters.Add(new SqlParameter("@OutputCGST", outputsgstval));
                    cmdTaxTable.Parameters.Add(new SqlParameter("@OutputSGST", outputsgstval));
                    cmdTaxTable.Parameters.Add(new SqlParameter("@OutputIGST", totalTaxAmount));
                }
                BalanceCRorDR = Convert.ToDouble(((dueBal.Content.ToString()).Replace("₹", "").Split(':')[1]).Trim());

                cmdTaxTable.Parameters.Add(new SqlParameter("@TotalInvValue", totalInvValues - oldtotalVal));
                cmdTaxTable.Parameters.Add(new SqlParameter("@TotalTaxableValue", totalTaxableValues));
                cmdTaxTable.Parameters.Add(new SqlParameter("@CompID", Convert.ToInt32(CompID)));
                cmdTaxTable.Connection.Open();
                cmdTaxTable.ExecuteNonQuery();
                cmdTaxTable.Connection.Close();


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

                //// Create a FlowDocument dynamically.
                ////FlowDocument doc = CreateFlowDocumentJewellery();
                ////FlowDocument doc = CreateFlowDocumentJewellery();
                //doc.ColumnWidth = 600;
                //doc.Name = "FlowDoc";
                //doc.PageHeight = 600;
                //doc.PageWidth = 800;
                //doc.MinPageWidth = 800;

                //// Create IDocumentPaginatorSource from FlowDocument
                //IDocumentPaginatorSource idpSource = doc;

                //// Call PrintDocument method to send document to printer
                ////Uncomment for Print
                //printDlg.PrintDocument(idpSource.DocumentPaginator, "Receipt Printing.");
            }

        }

        /// <summary>
        /// This method creates a dynamic FlowDocument. You can add anything to this
        /// FlowDocument that you would like to send to the printer
        /// </summary>
        /// <returns></returns>
        private FlowDocument CreateFlowDocumentJewellery()
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
            doc.ColumnWidth = 1024;
            doc.Name = "FlowDoc";
            doc.PageHeight = 600;
            doc.PageWidth = 800;
            doc.MinPageWidth = 800;


            /* style for products table header, assigned via type + class selectors */

            System.Windows.Documents.Paragraph p = new System.Windows.Documents.Paragraph();

            Span s = new Span();

            s = new Span(new Run(CompanyName));
            s.FontWeight = FontWeights.Bold;

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
            a4date.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span a5 = new Span();
            a5 = new Span(new Run("---------------------------------------------------------------------------------------------------------"));
            //a5.Inlines.Add(new LineBreak());//Line break is used for next line.  
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
            p.FontSize = 13;
            p.FontStyle = FontStyles.Normal;
            p.TextAlignment = TextAlignment.Center;
            p.FontFamily = new FontFamily("Century Gothic");
            doc.Blocks.Add(p);

            System.Windows.Documents.Table t5 = new System.Windows.Documents.Table();


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
            rowheadertable1.FontSize = 12;
            rowheadertable1.FontFamily = new FontFamily("Century Gothic");
            rowheadertable1.FontWeight = FontWeights.Bold;

            ThicknessConverter tc222 = new ThicknessConverter();

            TableCell tcellfirst = new TableCell(new System.Windows.Documents.Paragraph(new Run("Product")));
            tcellfirst.ColumnSpan = 3;
            tcellfirst.BorderBrush = Brushes.Black;
            tcellfirst.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcellfirst);

            TableCell tcell2 = new TableCell(new System.Windows.Documents.Paragraph(new Run("HSN")));
            //tcell2.ColumnSpan = 3;
            tcell2.BorderBrush = Brushes.Black;
            tcell2.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell2);

            TableCell tcell3 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Qty")));
            //tcell3.ColumnSpan = 3;
            tcell3.BorderBrush = Brushes.Black;
            tcell3.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell3);

            //TableCell tcell4 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Wt")));
            ////tcell4.ColumnSpan = 3;
            //tcell4.BorderBrush = Brushes.Black;
            //tcell4.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable1.Cells.Add(tcell4);

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

            TableCell tcell10 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Disc%")));
            //tcell10.ColumnSpan = 3;
            tcell10.BorderBrush = Brushes.Black;
            tcell10.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell10);

            TableCell tcell11 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Total")));
            //tcell11.ColumnSpan = 3;
            tcell11.BorderBrush = Brushes.Black;
            tcell11.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            rowheadertable1.Cells.Add(tcell11);


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

            IEnumerable itemsSource1 = CartGrid.ItemsSource as IEnumerable;
            if (itemsSource1 != null)
            {

                // foreach (var item in itemsSource)
                for (int k = 0; k < CartGrid.Items.Count; ++k)
                {
                    TableRow rowone = new TableRow();

                    // rowone.Background = Brushes.Silver;
                    rowone.FontSize = 11;
                    rowone.FontWeight = FontWeights.Regular;
                    rowone.FontFamily = new FontFamily("Century Gothic");
                    DataGridRow row = CartGrid.ItemContainerGenerator.ContainerFromItem(itemsSource1) as DataGridRow;

                    row = CartGrid.ItemContainerGenerator.ContainerFromItem(itemsSource1) as DataGridRow;

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



                        for (int i = 0; i < CartGrid.Columns.Count - 3; ++i)
                        {
                            DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(i);

                            TextBlock txt = cell.Content as TextBlock;
                            ComboBox ele = cell.Content as ComboBox;




                            ////TextBlock txt = new TextBlock();
                            //if (cell != null)
                            //{
                            //    txt = cell.Content as TextBlock;
                            //}

                            if (txt != null)
                            {
                                if (i == 0)
                                {

                                    // table.AddCell(new Phrase((k + 1).ToString(), tablefontsize));
                                    rowone.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run((k + 1).ToString()))));

                                }
                                else if (i == 1)
                                {
                                    TableCell firstcolproductcell = new TableCell(new System.Windows.Documents.Paragraph(new Run(txt.Text)));
                                    firstcolproductcell.ColumnSpan = 3;
                                    firstcolproductcell.BorderBrush = Brushes.Black;
                                    firstcolproductcell.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
                                    //rowone.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run((k + 1).ToString()))));
                                    rowone.Cells.Add(firstcolproductcell);
                                }
                                else
                                {
                                    TableCell txtcellall = new TableCell(new System.Windows.Documents.Paragraph(new Run(txt.Text)));
                                    txtcellall.BorderBrush = Brushes.Black;
                                    txtcellall.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
                                    rowone.Cells.Add(txtcellall);
                                    //table.AddCell(new Phrase(txt.Text, tablefontsize));
                                };
                            }




                            if (ele != null)
                            {

                                if (i == 1)
                                {

                                    // table.AddCell(new Phrase((k + 1).ToString(), tablefontsize));
                                    string[] txtnew = ele.Text.Split('-');
                                    rowone.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run(txtnew[1]))));

                                }
                                else
                                {
                                    rowone.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run(ele.Text))));
                                }
                            }





                        }

                    }
                    rg1.Rows.Add(rowone);
                }
            }



            //----------------

            t5.CellSpacing = 0;

            t5.RowGroups.Add(rg1);
            doc.Blocks.Add(t5);



            System.Windows.Documents.Paragraph totalValParag = new System.Windows.Documents.Paragraph();

            Span ts = new Span();
            //ts = new Span(new Run("\t" + " " + lbTotalTax.Content + "    " + lbTotal.Content));

            ts = new Span(new Run("\t" + lbTotal.Content));

            ts.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span cgsttax = new Span();
            cgsttax = new Span(new Run("\t" + "                          " + lbTotalTax.Content));
            cgsttax.Inlines.Add(new LineBreak());//Line break is used for next line.  

            totalValParag.TextAlignment = TextAlignment.Right;
            totalValParag.FontFamily = new FontFamily("Century Gothic");
            totalValParag.FontSize = 12;
            totalValParag.Inlines.Add(ts);// Add the span content into paragraph.  
            //totalVal.Inlines.Add(cgsttax);// Add the span content into paragraph. 
            //totalVal.Inlines.Add(sgsttax);// Add the span content into paragraph. 

            //totalVal.Inlines.Add(ali5);// Add the span content into paragraph.  

            doc.Blocks.Add(totalValParag);


            //System.Windows.Documents.Table t4 = new System.Windows.Documents.Table();

            //for (int i = 0; i < OldGoldGrid.Items.Count; i++)
            //{
            //    //TableColumn tc = new TableColumn();

            //    t4.Columns.Add(new TableColumn());

            //}

            //ThicknessConverter tc = new ThicknessConverter();
            ////// Create Table Borders
            //t4.BorderThickness = (Thickness)tc.ConvertFromString("0.0001in");
            //t4.CellSpacing = 0;
            //int count = OldGoldGrid.Items.Count;
            //var rg = new TableRowGroup();

            //TableRow rowheadertable = new TableRow();
            //rowheadertable.Background = Brushes.Silver;
            //rowheadertable.FontSize = 12;
            //rowheadertable.FontWeight = FontWeights.Bold;

            //TableCell tcellfirst1 = new TableCell(new System.Windows.Documents.Paragraph(new Run("OLD Item")));
            //tcellfirst1.ColumnSpan = 3;
            //tcellfirst1.BorderBrush = Brushes.Black;
            //tcellfirst1.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable.Cells.Add(tcellfirst1);


            //TableCell tcell31 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Qty")));
            ////tcell31.ColumnSpan = 3;
            //tcell31.BorderBrush = Brushes.Black;
            //tcell31.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable.Cells.Add(tcell31);

            //TableCell tcell41 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Wt")));
            ////tcell41.ColumnSpan = 3;
            //tcell41.BorderBrush = Brushes.Black;
            //tcell41.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable.Cells.Add(tcell41);

            //TableCell tcell51 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Waste(%)")));
            ////tcell51.ColumnSpan = 3;
            //tcell51.BorderBrush = Brushes.Black;
            //tcell51.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable.Cells.Add(tcell51);

            //TableCell tcell61 = new TableCell(new System.Windows.Documents.Paragraph(new Run("TotalWt")));
            ////tcell61.ColumnSpan = 3;
            //tcell61.BorderBrush = Brushes.Black;
            //tcell61.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable.Cells.Add(tcell61);

            //TableCell tcell71 = new TableCell(new System.Windows.Documents.Paragraph(new Run("MC")));
            ////tcell71.ColumnSpan = 3;
            //tcell71.BorderBrush = Brushes.Black;
            //tcell71.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable.Cells.Add(tcell71);

            //TableCell tcell81 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Price")));
            ////tcell81.ColumnSpan = 3;
            //tcell81.BorderBrush = Brushes.Black;
            //tcell81.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable.Cells.Add(tcell81);

            //TableCell tcell91 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Amt")));
            ////tcell91.ColumnSpan = 3;
            //tcell91.BorderBrush = Brushes.Black;
            //tcell91.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable.Cells.Add(tcell91);

            //TableCell tcell101 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Disc%")));
            ////tcell101.ColumnSpan = 3;
            //tcell101.BorderBrush = Brushes.Black;
            //tcell101.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable.Cells.Add(tcell101);

            //TableCell tcell111 = new TableCell(new System.Windows.Documents.Paragraph(new Run("Total")));
            ////tcell111.ColumnSpan = 3;
            //tcell111.BorderBrush = Brushes.Black;
            //tcell111.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //rowheadertable.Cells.Add(tcell111);


            ////rowheadertable.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Product"))));
            ////rowheadertable.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Qty"))));
            ////rowheadertable.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Wt"))));
            ////rowheadertable.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Waste(%)"))));
            ////rowheadertable.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("TotalWt"))));
            ////rowheadertable.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("MC"))));
            ////rowheadertable.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Price"))));
            ////rowheadertable.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Amt"))));
            ////rowheadertable.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Disc%"))));
            ////rowheadertable.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Amount"))));
            ////rowheadertable.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("GST%"))));
            ////rowheadertable.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run("Total"))));

            //rg.Rows.Add(rowheadertable);

            //IEnumerable itemsSource = OldGoldGrid.ItemsSource as IEnumerable;
            //if (itemsSource != null)
            //{

            //    // foreach (var item in itemsSource)
            //    for (int k = 0; k < OldGoldGrid.Items.Count; ++k)
            //    {
            //        TableRow rowone = new TableRow();

            //        // rowone.Background = Brushes.Silver;
            //        rowone.FontSize = 11;
            //        rowone.FontWeight = FontWeights.Regular;
            //        rowone.FontFamily = new FontFamily("Century Gothic");
            //        DataGridRow row = OldGoldGrid.ItemContainerGenerator.ContainerFromItem(itemsSource) as DataGridRow;

            //        row = OldGoldGrid.ItemContainerGenerator.ContainerFromItem(itemsSource) as DataGridRow;

            //        if (row == null)
            //        {
            //            OldGoldGrid.UpdateLayout();
            //            OldGoldGrid.ScrollIntoView(OldGoldGrid.Items[k]);
            //            row = (DataGridRow)OldGoldGrid.ItemContainerGenerator.ContainerFromIndex(k);
            //        }

            //        if (row != null)
            //        {
            //            DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(row);

            //            //============
            //            if (presenter == null)
            //            {

            //                OldGoldGrid.UpdateLayout();
            //                OldGoldGrid.ScrollIntoView(OldGoldGrid.Items[k]);
            //                row = (DataGridRow)OldGoldGrid.ItemContainerGenerator.ContainerFromIndex(k);
            //                DataGridCellsPresenter prsnter = FindVisualChild<DataGridCellsPresenter>(row);
            //                presenter = prsnter;
            //            }
            //            //============



            //            for (int i = 0; i < OldGoldGrid.Columns.Count - 2; ++i)
            //            {
            //                DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(i);

            //                TextBlock txt = cell.Content as TextBlock;
            //                ComboBox ele = cell.Content as ComboBox;




            //                ////TextBlock txt = new TextBlock();
            //                //if (cell != null)
            //                //{
            //                //    txt = cell.Content as TextBlock;
            //                //}

            //                if (txt != null)
            //                {
            //                    if (i == 0)
            //                    {

            //                        // table.AddCell(new Phrase((k + 1).ToString(), tablefontsize));
            //                        rowone.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run((k + 1).ToString()))));

            //                    }

            //                    else if (i == 1)
            //                    {
            //                        TableCell firstcolproductcell1 = new TableCell(new System.Windows.Documents.Paragraph(new Run(txt.Text)));
            //                        firstcolproductcell1.ColumnSpan = 3;
            //                        firstcolproductcell1.BorderBrush = Brushes.Black;
            //                        firstcolproductcell1.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //                        //rowone.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run((k + 1).ToString()))));
            //                        rowone.Cells.Add(firstcolproductcell1);
            //                    }
            //                    else
            //                    {
            //                        TableCell txtcellall1 = new TableCell(new System.Windows.Documents.Paragraph(new Run(txt.Text)));
            //                        txtcellall1.BorderBrush = Brushes.Black;
            //                        txtcellall1.BorderThickness = (Thickness)tc222.ConvertFromString("0.0001in");
            //                        rowone.Cells.Add(txtcellall1);
            //                        //table.AddCell(new Phrase(txt.Text, tablefontsize));
            //                    };


            //                    //else
            //                    //{
            //                    //    rowone.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run(txt.Text))));
            //                    //    //table.AddCell(new Phrase(txt.Text, tablefontsize));
            //                    //};
            //                }




            //                if (ele != null)
            //                {

            //                    if (i == 1)
            //                    {

            //                        // table.AddCell(new Phrase((k + 1).ToString(), tablefontsize));
            //                        string[] txtnew = ele.Text.Split('-');
            //                        rowone.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run(txtnew[1]))));

            //                    }
            //                    else
            //                    {
            //                        rowone.Cells.Add(new TableCell(new System.Windows.Documents.Paragraph(new Run(ele.Text))));
            //                    }
            //                }





            //            }

            //        }
            //        rg.Rows.Add(rowone);
            //    }
            //}



            ////----------------



            //t4.RowGroups.Add(rg);

            //if (oldtotalVal > 0)
            //{
            //    doc.Blocks.Add(t4);
            //}




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
            linebrktble = new Span(new Run("------------------------------------------------------------------------------------------- "));
            // linebrktble.Inlines.Add(new LineBreak());//Line break is used for next line.  

            linedot.Inlines.Add(linebrktble);// Add the span content into paragraph. 
            linedot.TextAlignment = TextAlignment.Center;
            doc.Blocks.Add(linedot);



            System.Windows.Documents.Paragraph totalVaGrand = new System.Windows.Documents.Paragraph();
            //totalValold.FontFamily 

            Span ts11g = new Span();

            ts11g = new Span(new Run("\t" + "" + lbGrandTotal.Content));
            ts11g.Inlines.Add(new LineBreak());//Line break is used for next line.  

            Span ts111g = new Span();
            ts111g = new Span(new Run("\t" + "Discount: -₹" + flatOff.Text));
            ts111g.Inlines.Add(new LineBreak());//Line break is used for next line.  
            double flatoff = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());
            string grandvalueafterDisc = Math.Round((totalVal - oldtotalVal - flatoff), 0).ToString();

            Span ts1112g = new Span();
            ts1112g = new Span(new Run("\t" + "Pay: ₹" + grandvalueafterDisc));
            ts1112g.Inlines.Add(new LineBreak());//Line break is used for next line.  

            totalVaGrand.FontSize = 14;
            totalVaGrand.FontFamily = new FontFamily("Century Gothic");
            totalVaGrand.Inlines.Add(ts11g);// Add the span content into paragraph.  
            totalVaGrand.Inlines.Add(ts111g);
            totalVaGrand.Inlines.Add(ts1112g);
            //totalVal.Inlines.Add(ali5);// Add the span content into paragraph.  
            totalVaGrand.TextAlignment = TextAlignment.Right;

            totalVaGrand.FontWeight = FontWeights.Bold;
            doc.Blocks.Add(totalVaGrand);


            doc.Blocks.Add(linedot);

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

        /// <summary>
        /// Export to PDf for Clothes Wholesalers
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="depObj"></param>
        /// <returns></returns>
        public void CreateFlowDocumentReadyMadeWholeSale()
        {
            try
            {
                MessageBoxResult genResult = MessageBox.Show("Do you want to generate PDf invoice?", "PDF Invoice", MessageBoxButton.YesNo);
                if (genResult == MessageBoxResult.Yes)
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
                    con.Close();

                    // firmGSTIN = firmGSTIN.Trim().Substring(0, 2);
                    // firmStateCode.Text = firmGSTIN;
                    string imageFilePath1 = @"c:\ViewBill\Logo\Logo1.jpg";
                    if (CompID == "2")
                    {
                        imageFilePath1 = @"c:\ViewBill\Logo\Logo3.jpg";
                    }
                    string imageFilePathLogo2 = @"c:\ViewBill\Logo\Logo2.jpg";
                    //add background image 

                        //string imageFilePath = @"c:\ViewBill\Logo\Logo1.jpg";
                        iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageFilePath1);
                        //Resize image depend upon your need
                        //For give the size to image
                        jpg.ScaleToFit(80, 80);

                        //If you want to choose image as background then,

                        jpg.Alignment = iTextSharp.text.Image.UNDERLYING;
                        //If you want to give absolute/specified fix position to image.
                        jpg.SetAbsolutePosition(20, 510); // to set the logo at left top 


                    //string imageFilePathLogo2 = @"c:\ViewBill\Logo\Logo2.jpg";
                    iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance(imageFilePathLogo2);
                    //Resize image depend upon your need
                    //For give the size to image
                    jpg2.ScaleToFit(50, 50);

                    //If you want to choose image as background then,

                    jpg2.Alignment = iTextSharp.text.Image.UNDERLYING;
                    //If you want to give absolute/specified fix position to image.
                    jpg2.SetAbsolutePosition(340, 535); // to set the logo at left top 



                    ///
                    // Font headerFONT = new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.BOLD, BaseColor.BLACK);
                    Font allFONTsize = new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.NORMAL, BaseColor.BLACK);
                    Font forFontSize = new Font(Font.FontFamily.COURIER, 7.5f, Font.BOLD, BaseColor.BLACK);
                    Font allFONTsizetotal = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.BLACK);
                    // Font tinfont = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.NORMAL, BaseColor.BLACK);
                    // Font dateInv = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.BLACK);
                    //for table font 
                    Font tablefontsize = new Font(Font.FontFamily.TIMES_ROMAN, 9.2f, Font.BOLD, BaseColor.BLACK);
                    Font tablefontsizeHeader = new Font(Font.FontFamily.TIMES_ROMAN, 6f, Font.NORMAL, BaseColor.BLACK);

                    Font taxslabAmtFont = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.NORMAL, BaseColor.BLACK);
                    Font termsFont = new Font(Font.FontFamily.TIMES_ROMAN, 4f, Font.BOLD, BaseColor.BLACK);
                    Font BankDetailFont = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.NORMAL, BaseColor.BLACK);

                    //PdfPTable table = new iTextSharp.text.pdf.PdfPTable(CartGrid.Columns.Count) { TotalWidth = 390, LockedWidth = true };




                    Font smallfont = new Font(Font.FontFamily.TIMES_ROMAN, 5.5f, Font.NORMAL, BaseColor.BLACK);



                    long rupeesFig = Convert.ToInt64(Math.Round((Convert.ToDouble(totalInvValues)), 0));

                    string reupeesWords = ConvertNumbertoWords(rupeesFig);

                    Font WwordsFormat = new Font(Font.FontFamily.TIMES_ROMAN, 7.5f, Font.NORMAL, BaseColor.BLACK);






                    PdfPTable totalTableHorizontal = new iTextSharp.text.pdf.PdfPTable(5) { TotalWidth = 390, LockedWidth = true };
                    //   [] { 13, 92, 30, 25, 30, 34, 40, 20, 40, 22, 22, 22 };
                    //new float[] { 13, 152, 30, 25,   30, 34, 40,  22, 22, 22 }; //remove disc and taxable
                    // 15, 148, 30, 25,   28, 34, 38 , 24, 24, 24 }
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


                    float[] widthsTotalTable = new float[] { 215, 100, 70 };
                    totalTable.SetWidths(widthsTotalTable);

                    string packingchargeVal = "";
                    //Convert.ToInt32(shipValText.Text)
                    if (txtPackForward.Text.Trim() != "")
                    {
                        packingchargeVal = Convert.ToInt32(txtPackForward.Text).ToString();

                    }

                    long rupeesFigVal = Convert.ToInt64(Math.Round((Convert.ToDouble(totalInvValues)), 0));

                    string reupeesWordsVal = ConvertNumbertoWords(rupeesFig);

                    Font WwordsFormatVal = new Font(Font.FontFamily.TIMES_ROMAN, 6f, Font.NORMAL, BaseColor.BLACK);


                    PdfPCell totalCellAlign = new PdfPCell();
                    totalCellAlign.BorderWidthLeft = 0;
                    PdfPCell totalCellAmtAlign = new PdfPCell();
                    totalCellAmtAlign.BorderWidthRight = 0;
                    PdfPCell bankInvTotal = new PdfPCell();
                    // bankInvTotal.Colspan
                    PdfPTable bankWordsAmtTbl = new iTextSharp.text.pdf.PdfPTable(1) { TotalWidth = 215, LockedWidth = true };
                    bankWordsAmtTbl.DefaultCell.Border = 0;


                    PdfPTable banktaxslabDetailsTable = new iTextSharp.text.pdf.PdfPTable(2) { TotalWidth = 215, LockedWidth = true };
                    float[] banktaxslabwidths = new float[] { 70, 150 };
                    banktaxslabDetailsTable.SetWidths(banktaxslabwidths);
                    banktaxslabDetailsTable.DefaultCell.Border = 0;

                    PdfPTable taxslavtbl = new iTextSharp.text.pdf.PdfPTable(3);
                    taxslavtbl.DefaultCell.Border = 0;
                    float[] widthtaxslabs = new float[] { 60, 45, 45 };
                    taxslavtbl.SetWidths(widthtaxslabs);

                    //PdfPTable taxslabtableVerticalalign = new iTextSharp.text.pdf.PdfPTable(1);
                    //taxslabtableVerticalalign.DefaultCell.Border = 0;
                    //taxslabtableVerticalalign.AddCell(new Phrase("Tax Slab", taxslabAmtFont));
                    //if (!CGSTSum5.Equals(0.0))
                    //{
                    //    taxslabtableVerticalalign.AddCell(new Phrase("CGST@2.5%:", taxslabAmtFont));
                    //}
                    //if (!CGSTSum5.Equals(0.0))
                    //{
                    //    taxslabtableVerticalalign.AddCell(new Phrase("SGST@2.5%:", taxslabAmtFont));
                    //}
                    //if (!CGSTSum12.Equals(0.0))
                    //{
                    //    taxslabtableVerticalalign.AddCell(new Phrase("CGST@6%:", taxslabAmtFont));
                    //}
                    //if (!CGSTSum12.Equals(0.0))
                    //{
                    //    taxslabtableVerticalalign.AddCell(new Phrase("SGST@6%:", taxslabAmtFont));
                    //}
                    //if (!CGSTSum18.Equals(0.0))
                    //{
                    //    taxslabtableVerticalalign.AddCell(new Phrase("CGST@9%:", taxslabAmtFont));
                    //}
                    //if (!CGSTSum18.Equals(0.0))
                    //{
                    //    taxslabtableVerticalalign.AddCell(new Phrase("SGST@9%:", taxslabAmtFont));
                    //}
                    //if (!SGSTSum28.Equals(0.0))
                    //{
                    //    taxslabtableVerticalalign.AddCell(new Phrase("CGST@14%:", taxslabAmtFont));
                    //}
                    //if (!SGSTSum28.Equals(0.0))
                    //{
                    //    taxslabtableVerticalalign.AddCell(new Phrase("SGST@14%:", taxslabAmtFont));
                    //}
                    //if (!IGSTSum28.Equals(0.0))
                    //{
                    //    taxslabtableVerticalalign.AddCell(new Phrase("IGST@28%:", taxslabAmtFont));
                    //}
                    //if (!IGSTSum18.Equals(0.0))
                    //{
                    //    taxslabtableVerticalalign.AddCell(new Phrase("IGST@18%:", taxslabAmtFont));
                    //}
                    //if (!IGSTSum12.Equals(0.0))
                    //{
                    //    taxslabtableVerticalalign.AddCell(new Phrase("IGST@12%:", taxslabAmtFont));
                    //}
                    //if (!IGSTSum5.Equals(0.0))
                    //{
                    //    taxslabtableVerticalalign.AddCell(new Phrase("IGST@5%:", taxslabAmtFont));
                    //}
                    //taxslabtableVerticalalign.DefaultCell.Rowspan = 2;
                    //taxslabtableVerticalalign.DefaultCell.Border = 0;
                    //taxslabtableVerticalalign.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //PdfPCell taxslabtableVerticalalignCell = new PdfPCell();
                    //taxslabtableVerticalalignCell.Border = 0;
                    //taxslabtableVerticalalignCell.AddElement(taxslabtableVerticalalign);

                    //PdfPTable taxtableVerticalalign = new iTextSharp.text.pdf.PdfPTable(1);
                    //taxtableVerticalalign.DefaultCell.Border = 0;
                    //taxtableVerticalalign.AddCell(new Phrase("Tax", taxslabAmtFont));

                    //if (!CGSTSum5.Equals(0.0))
                    //{
                    //    taxtableVerticalalign.AddCell(new Phrase(CGSTSum5.ToString(), taxslabAmtFont));
                    //}
                    //if (!CGSTSum5.Equals(0.0))
                    //{
                    //    taxtableVerticalalign.AddCell(new Phrase(CGSTSum5.ToString(), taxslabAmtFont));
                    //}

                    //if (!CGSTSum12.Equals(0.0))
                    //{
                    //    taxtableVerticalalign.AddCell(new Phrase(CGSTSum12.ToString(), taxslabAmtFont));
                    //}
                    //if (!CGSTSum12.Equals(0.0))
                    //{
                    //    taxtableVerticalalign.AddCell(new Phrase(CGSTSum12.ToString(), taxslabAmtFont));
                    //}
                    //if (!CGSTSum18.Equals(0.0))
                    //{
                    //    taxtableVerticalalign.AddCell(new Phrase(CGSTSum18.ToString(), taxslabAmtFont));
                    //}
                    //if (!CGSTSum18.Equals(0.0))
                    //{
                    //    taxtableVerticalalign.AddCell(new Phrase(CGSTSum18.ToString(), taxslabAmtFont));
                    //}
                    //if (!SGSTSum28.Equals(0.0))
                    //{
                    //    taxtableVerticalalign.AddCell(new Phrase(SGSTSum28.ToString(), taxslabAmtFont));
                    //}
                    //if (!SGSTSum28.Equals(0.0))
                    //{
                    //    taxtableVerticalalign.AddCell(new Phrase(CGSTSum28.ToString(), taxslabAmtFont));
                    //}
                    //if (!IGSTSum28.Equals(0.0))
                    //{
                    //    taxtableVerticalalign.AddCell(new Phrase(IGSTSum28.ToString(), taxslabAmtFont));
                    //}
                    //if (!IGSTSum18.Equals(0.0))
                    //{
                    //    taxtableVerticalalign.AddCell(new Phrase(IGSTSum18.ToString(), taxslabAmtFont));
                    //}
                    //if (!IGSTSum12.Equals(0.0))
                    //{
                    //    taxtableVerticalalign.AddCell(new Phrase(IGSTSum12.ToString(), taxslabAmtFont));
                    //}
                    //if (!IGSTSum5.Equals(0.0))
                    //{
                    //    taxtableVerticalalign.AddCell(new Phrase(IGSTSum5.ToString(), taxslabAmtFont));
                    //}


                    //taxtableVerticalalign.DefaultCell.Rowspan = 2;
                    //taxtableVerticalalign.DefaultCell.Border = 0;
                    //taxtableVerticalalign.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //PdfPCell taxtableVerticalalignCell = new PdfPCell();
                    //taxtableVerticalalignCell.Border = 0;
                    //taxtableVerticalalignCell.AddElement(taxtableVerticalalign);

                    //PdfPTable valuetableVerticalalign = new iTextSharp.text.pdf.PdfPTable(1);
                    //valuetableVerticalalign.DefaultCell.Border = 0;
                    //valuetableVerticalalign.AddCell(new Phrase("Value", taxslabAmtFont));
                    //if (!CGSTSum5.Equals(0.0))
                    //{
                    //    valuetableVerticalalign.AddCell(new Phrase(SGSTSum5Value.ToString(), taxslabAmtFont));
                    //}
                    //if (!CGSTSum5.Equals(0.0))
                    //{
                    //    valuetableVerticalalign.AddCell(new Phrase(SGSTSum5Value.ToString(), taxslabAmtFont));
                    //}

                    //if (!CGSTSum12.Equals(0.0))
                    //{
                    //    valuetableVerticalalign.AddCell(new Phrase(SGSTSum12Value.ToString(), taxslabAmtFont));
                    //}
                    //if (!CGSTSum12.Equals(0.0))
                    //{
                    //    valuetableVerticalalign.AddCell(new Phrase(SGSTSum12Value.ToString(), taxslabAmtFont));
                    //}
                    //if (!CGSTSum18.Equals(0.0))
                    //{
                    //    valuetableVerticalalign.AddCell(new Phrase(SGSTSum18Value.ToString(), taxslabAmtFont));
                    //}
                    //if (!CGSTSum18.Equals(0.0))
                    //{
                    //    valuetableVerticalalign.AddCell(new Phrase(SGSTSum18Value.ToString(), taxslabAmtFont));
                    //}
                    //if (!SGSTSum28.Equals(0.0))
                    //{
                    //    valuetableVerticalalign.AddCell(new Phrase(SGSTSum28Value.ToString(), taxslabAmtFont));
                    //}
                    //if (!SGSTSum28.Equals(0.0))
                    //{
                    //    valuetableVerticalalign.AddCell(new Phrase(SGSTSum28Value.ToString(), taxslabAmtFont));
                    //}
                    //if (!IGSTSum28.Equals(0.0))
                    //{
                    //    valuetableVerticalalign.AddCell(new Phrase(IGSTSum28Value.ToString(), taxslabAmtFont));
                    //}
                    //if (!IGSTSum18.Equals(0.0))
                    //{
                    //    valuetableVerticalalign.AddCell(new Phrase(IGSTSum18Value.ToString(), taxslabAmtFont));
                    //}
                    //if (!IGSTSum12.Equals(0.0))
                    //{
                    //    valuetableVerticalalign.AddCell(new Phrase(IGSTSum12Value.ToString(), taxslabAmtFont));
                    //}
                    //if (!IGSTSum5.Equals(0.0))
                    //{
                    //    valuetableVerticalalign.AddCell(new Phrase(IGSTSum5Value.ToString(), taxslabAmtFont));
                    //}

                    //valuetableVerticalalign.DefaultCell.Rowspan = 2;
                    //valuetableVerticalalign.DefaultCell.Border = 0;
                    //valuetableVerticalalign.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //PdfPCell valuetableVerticalalignCell = new PdfPCell();
                    //valuetableVerticalalignCell.Border = 0;
                    //valuetableVerticalalignCell.AddElement(valuetableVerticalalign);

                    //taxslavtbl.AddCell(taxslabtableVerticalalignCell);
                    //taxslavtbl.AddCell(taxtableVerticalalignCell);
                    //taxslavtbl.AddCell(valuetableVerticalalignCell);


                    //ourbankdetails1cell.AddElement(ourbankdetails1);

                    banktaxslabDetailsTable.AddCell(new Phrase("E. & O.E" + "\n", BankDetailFont));
                    // banktaxslabDetailsTable.AddCell(ourbankdetails1cell);
                    //banktaxslabDetailsTable.AddCell(taxslavtbl);




                    bankWordsAmtTbl.AddCell(new Phrase(" Amount Chargeable(in words): Indian Rupees " + reupeesWordsVal + " Only." + "\n", forFontSize));

                    bankWordsAmtTbl.AddCell(banktaxslabDetailsTable);
                    //bankWordsAmtTbl.AddCell(taxslavtbl);

                    // bankWordsAmtTbl.AddCell(new Phrase("OUR BANK DETAILS" + "\n" + "A/C#: " + firAcccountNumb.Trim() + "\n" + firmBankName.Trim() + "\n" + "IFSC: " + firmIFSC.Trim() + "\n" + firmBankAddress.Trim(), BankDetailFont));
                    bankWordsAmtTbl.DefaultCell.Rowspan = 2;
                    totalTable.AddCell(bankWordsAmtTbl);
                    //totalTable.AddCell(new Phrase("Total Invoice Value(In Figure): " + reupeesWordsVal + " Only." + "\n", forFontSize));
                    // Phrase phrtt0l = new Phrase(new Phrase("Total:  " + "\n" + "Discount:  " + "\n" + "Taxable Value:  " + "\n" + "CGST:  " + "\n" + "SGST:  " + "\n" + "IGST:  " + "\n" + "Pack&Ship Charge:  " + "\n" + "Total Invoice Value:  " + "\n" + "", allFONTsize));

                    PdfPTable totaltableVerticalalign = new iTextSharp.text.pdf.PdfPTable(1);
                    totaltableVerticalalign.DefaultCell.Border = 0;
                    //totaltableVerticalalign.AddCell(new Phrase("Total:", allFONTsizetotal));
                    if (!Math.Round(discounttotalByItem, 2).Equals(0.0))
                    {
                        totaltableVerticalalign.AddCell(new Phrase("Total:", allFONTsizetotal));
                        totaltableVerticalalign.AddCell(new Phrase("Discount:", allFONTsizetotal));
                    }
                    totaltableVerticalalign.AddCell(new Phrase("Taxable Value:", allFONTsizetotal));
                    if (IState)
                    {
                        totaltableVerticalalign.AddCell(new Phrase("CGST:", allFONTsizetotal));
                    }
                    if (IState)
                    {
                        totaltableVerticalalign.AddCell(new Phrase("SGST:", allFONTsizetotal));
                    }
                    if (!IState)
                    {
                        totaltableVerticalalign.AddCell(new Phrase("IGST:", allFONTsizetotal));
                    }
                    if (packingchargeVal != "")
                    {
                        totaltableVerticalalign.AddCell(new Phrase("Pack&Ship Charge:", allFONTsizetotal));
                    }

                    //if (disc.Text != "")
                    //{
                    //    totaltableVerticalalign.AddCell(new Phrase("Discount:@" + discountperc.Text + "%", allFONTsizetotal));
                    //}
                    Font colorHighlight = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.RED);

                    totaltableVerticalalign.AddCell(new Phrase("Total Invoice Value:", allFONTsizetotal));
                    if (oldtotalVal > 0)
                    {
                        totaltableVerticalalign.AddCell(new Phrase("Old Item Value:", allFONTsizetotal));
                        totaltableVerticalalign.AddCell(new Phrase("Grand Total:", allFONTsizetotal));
                    }
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

                    Phrase totalsumrupee = new Phrase("    \u20B9 ", font3);
                    totalsumrupee.Add(new Phrase(Math.Round(totalBeforeItemDiscount, 2).ToString(), allFONTsizetotal));

                    Phrase discSumrupee = new Phrase("-" + "    \u20B9 ", font3);
                    discSumrupee.Add(new Phrase(Math.Round(discounttotalByItem, 2).ToString(), allFONTsizetotal));

                    //Phrase discSumrupee = new Phrase(Math.Round(discSum, 2).ToString(), allFONTsizetotal);
                    //discSumrupee.Add(chunkRupee);
                    Phrase taxableSumrupee = new Phrase("    \u20B9 ", font3);
                    taxableSumrupee.Add(new Phrase(Math.Round(totalTaxableValues, 2).ToString(), allFONTsizetotal));

                    //Phrase taxableSumrupee = new Phrase(Math.Round(taxableSum, 2).ToString(), allFONTsizetotal);
                    //taxableSumrupee.Add(chunkRupee);


                    Phrase cGSTSumrupee = new Phrase("    \u20B9 ", font3);
                    Phrase sGSTSumrupee = new Phrase("    \u20B9 ", font3);
                    Phrase iGSTSumrupee = new Phrase("    \u20B9 ", font3);
                    if (IState)
                    {

                        cGSTSumrupee.Add(new Phrase(Math.Round(totalTaxAmount / 2, 2).ToString(), allFONTsizetotal));
                        sGSTSumrupee.Add(new Phrase(Math.Round(totalTaxAmount / 2, 2).ToString(), allFONTsizetotal));
                    }
                    else
                    {
                        iGSTSumrupee.Add(new Phrase(Math.Round(totalTaxAmount, 2).ToString(), allFONTsizetotal));
                    }

                    Phrase packingchargeValrupee = new Phrase("    \u20B9 ", font3);
                    packingchargeValrupee.Add(new Phrase(packingchargeVal, allFONTsizetotal));
                    double discountamount12 = (discountTxt.Text == "") ? 0.0 : (Convert.ToDouble(discountTxt.Text) * totalVal / 100);
                    Phrase discAmountvalues = new Phrase("    \u20B9 ", font3);
                    discAmountvalues.Add(new Phrase(Math.Round(discountamount12, 2).ToString(), allFONTsizetotal));

                    Phrase totalInvValuerupee = new Phrase("    \u20B9 ", font3);
                    //totalInvValuerupee.Add(new Phrase(Math.Round(Convert.ToDouble(totalInvValue), 0).ToString(), allFONTsizetotal));
                    totalInvValuerupee.Add(new Phrase(Math.Round(totalInvValues, 0).ToString(), allFONTsizetotal));

                    Phrase totaloldrupees = new Phrase("-" + "    \u20B9 ", font3);
                    Phrase totalgrandtotalwithOld = new Phrase("    \u20B9 ", font3);
                    if (oldtotalVal > 0)
                    {

                        //totalInvValuerupee.Add(new Phrase(Math.Round(Convert.ToDouble(totalInvValue), 0).ToString(), allFONTsizetotal));
                        totaloldrupees.Add(new Phrase(oldtotalVal.ToString(), allFONTsizetotal));
                        totalgrandtotalwithOld.Add(new Phrase((totalInvValues - oldtotalVal).ToString(), allFONTsizetotal));
                    }
                    //Phrase totalInvValuerupee = new Phrase(Math.Round((Convert.ToDouble(totalInvValue)), 0).ToString(), allFONTsizetotal);
                    //totalInvValuerupee.Add(chunkRupee);


                    //totaltableVerticalalign1.AddCell(totalsumrupee);
                    if (!Math.Round(discounttotalByItem, 2).Equals(0.0))
                    {
                        totaltableVerticalalign1.AddCell(totalsumrupee);
                        totaltableVerticalalign1.AddCell(discSumrupee);
                    }
                    totaltableVerticalalign1.AddCell(taxableSumrupee);
                    if (IState)
                    {
                        totaltableVerticalalign1.AddCell(cGSTSumrupee);
                    }
                    if (IState)
                    {
                        totaltableVerticalalign1.AddCell(sGSTSumrupee);
                    }
                    if (!IState)
                    {
                        totaltableVerticalalign1.AddCell(iGSTSumrupee);
                    }
                    if (packingchargeVal != "")
                    {
                        totaltableVerticalalign1.AddCell(packingchargeValrupee);
                    }

                    if (discountTxt.Text != "")
                    {
                        totaltableVerticalalign1.AddCell(discAmountvalues);
                    }

                    totaltableVerticalalign1.AddCell(totalInvValuerupee);
                    if (oldtotalVal > 0)
                    {
                        totaltableVerticalalign1.AddCell(totaloldrupees);
                        totaltableVerticalalign1.AddCell(totalgrandtotalwithOld);
                    }
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



                    PdfPTable bankseparateTax = new iTextSharp.text.pdf.PdfPTable(3) { TotalWidth = 390, LockedWidth = true };
                    //PdfPCell separatetabletaxCell = new PdfPCell();

                    float[] widthsBankTable = new float[] { 85, 185, 120 };
                    bankseparateTax.SetWidths(widthsBankTable);




                    //PdfPTable termCon = new iTextSharp.text.pdf.PdfPTable(1) { TotalWidth = 85, LockedWidth = true };
                    //termCon.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //termCon.DefaultCell.BorderWidth = 0;
                    //PdfPCell termCell = new PdfPCell();




                    iTextSharp.text.Paragraph termdetails = new iTextSharp.text.Paragraph();
                    Phrase term1phT = new Phrase("E. & O.E" + "\n", BankDetailFont);
                    //termdetails.Add(term1phT); 
                    // termdetails.Add(ourbankdetails1cell);
                    Phrase term1ph = new Phrase(" ->All disbutes are subject to Chennai Jurisdiction" + "\n" + "->Goods once sold will not be taken back" + "\n" + "->Goods are despatched at buyers risk " + "\n" + "->GST Rules and Regulation are applicable" + "\n", termsFont);
                    // termdetails.Add(term1ph);

                    PdfPTable ourbankdetails1 = new iTextSharp.text.pdf.PdfPTable(1) { TotalWidth = 85, LockedWidth = true };
                    ourbankdetails1.DefaultCell.BorderWidthTop = 0;
                    ourbankdetails1.DefaultCell.BorderWidthBottom = 0;
                    //ourbankdetails1.DefaultCell.BorderWidthRight = 0;
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
                    //ourbankdetails1cell.BorderWidthBottom = 0;

                    //termCell.AddElement(ourbankdetails1);
                    //termCell.BorderWidth = 0;
                    //termCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //termCon.AddCell(termCell);

                    // separatetabletaxCell.AddElement(taxslavtbl);
                    // separatetaxessum.AddCell(separatetabletaxCell);



                    PdfPTable ForFirm = new iTextSharp.text.pdf.PdfPTable(1) { TotalWidth = 120, LockedWidth = true };
                    ForFirm.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
                    ForFirm.DefaultCell.BorderWidth = 0;
                    PdfPCell ForFirmCell = new PdfPCell();
                    Phrase FirmPhrs = new Phrase("for " + CompanyName + "\n" + " " + "\n" + "\n" + "\n" + "\n" + "Authorised Signatory", forFontSize);

                    ForFirmCell.AddElement(FirmPhrs);
                    ForFirmCell.BorderWidth = 0;
                    ForFirmCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    ForFirm.AddCell(ForFirmCell);


                    //  //bankseparateTax.AddCell(termCon);
                    // bankseparateTax.AddCell("");
                    // bankseparateTax.AddCell("");
                    //// bankseparateTax.AddCell(PdfTableHSNcell);
                    // //bankseparateTax.AddCell(bankDetails);
                    // bankseparateTax.AddCell(ForFirm);



                    //totalTable.AddCell(totalCellAlign);
                    // totalTable.AddCell(new Phrase(Math.Round(totalSum, 2).ToString() + "\n" + Math.Round(discSum, 2).ToString() + "\n" + Math.Round(taxableSum, 2).ToString() + "\n" + Math.Round(Convert.ToDouble(cGSTSum), 2).ToString() + "\n" + Math.Round(Convert.ToDouble(sGSTSum), 2).ToString() + "\n"  + Math.Round(Convert.ToDouble(iGSTSum), 2).ToString() + "\n" + packingchargeVal + "\n" +  Math.Round((Convert.ToDouble(totalInvValue)), 0) + "\n" + "", allFONTsize));

                    //float[] widths = new float[] { 13, 92, 30, 25, 30, 34, 40, 20, 40, 22, 22, 22 };
                    //float[] widths = new float[] { 12, 50,30,30,30, 30, 25, 28, 34, 38, 24, 24, 24 }; //remove disc and taxable





                    //Remove all special character from textBoxCustName
                    FileStream fs = File.Open(@"C:\ViewBill\" + "Bill-" + (invoiceNumber.Text).Trim() + "-" + autocompltCustName.autoTextBox.Text + ".pdf", FileMode.Create);


                    using (MemoryStream output = new MemoryStream())
                    {

                        Document document = new Document(iTextSharp.text.PageSize.A5, 2f, 2f, 170f, 2f); // 159
                        //commented below for memort=y stream
                        PdfWriter writer = PdfWriter.GetInstance(document, fs);
                        //PdfWriter writer = PdfWriter.GetInstance(document, output);

                        ///
                        SqlConnection conCustDetails = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                        //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                        conCustDetails.Open();
                        string sqlCustDetails = "select * from AccountsList where LTRIM(RTRIM(AcctName)) = '" + autocompltCustName.autoTextBox.Text.Trim() + "' and CompID = '" + CompID + "'";
                        SqlCommand cmdCustDetails = new SqlCommand(sqlCustDetails, conCustDetails);

                        //cmdCustDetails.Connection = conCustDetails;
                        //cmdCustDetails.Connection.Open();
                        SqlDataReader readerCustDetails = cmdCustDetails.ExecuteReader();

                        //tmpProduct = new Product();
                        string CompanyNameCustomerDetails = "";
                        string PrintNameCustomerDetails = "";
                        string AliasNameCustomerDetails = "";
                        string GSTINCustomerDetails = "";
                        string AddressCustomerDetails = "";
                        string Address2CustomerDetails = "";
                        string CityCustomerDetails = "";
                        string StateCustomerDetails = "";
                        string MobCustomerDetails = "";
                        string PhoneCustomerDetails = "";
                        string PinCodeCustomerDetails = "";
                        string ShippinAddr1 = "";
                        string ShippinAddr2 = "";

                        while (readerCustDetails.Read())
                        {

                            //var CustID = reader.GetValue(0).ToString();
                            CompanyNameCustomerDetails = (readerCustDetails["AcctName"] != DBNull.Value) ? (readerCustDetails.GetString(1).Trim()) : "";
                            AliasNameCustomerDetails = (readerCustDetails["Alias"] != DBNull.Value) ? (readerCustDetails.GetString(5).Trim()) : "";
                            PrintNameCustomerDetails = (readerCustDetails["PrintName"] != DBNull.Value) ? (readerCustDetails.GetString(29).Trim()) : "";
                            AddressCustomerDetails = (readerCustDetails["Address1"] != DBNull.Value) ? (readerCustDetails.GetString(6).Trim()) : "";
                            Address2CustomerDetails = (readerCustDetails["Address2"] != DBNull.Value) ? (readerCustDetails.GetString(7).Trim()) : "";
                            CityCustomerDetails = (readerCustDetails["City"] != DBNull.Value) ? (readerCustDetails.GetString(8).Trim()) : "";

                            StateCustomerDetails = (readerCustDetails["State"] != DBNull.Value) ? (readerCustDetails.GetString(9).Trim()) : "";
                            PinCodeCustomerDetails = (readerCustDetails["PINCode"] != DBNull.Value) ? (readerCustDetails.GetString(10).Trim()) : "";
                            MobCustomerDetails = (readerCustDetails["Mobile1"] != DBNull.Value) ? (readerCustDetails.GetString(11).Trim()) : "";
                            PhoneCustomerDetails = (readerCustDetails["Phone"] != DBNull.Value) ? (readerCustDetails.GetString(13).Trim()) : "";

                            ShippinAddr1 = (readerCustDetails["ShippingAddr1"] != DBNull.Value) ? (readerCustDetails.GetString(26).Trim()) : "";
                            ShippinAddr2 = (readerCustDetails["ShippingAddr2"] != DBNull.Value) ? (readerCustDetails.GetString(27).Trim()) : "";

                            GSTINCustomerDetails = (readerCustDetails["GSTIN"] != DBNull.Value) ? (readerCustDetails.GetString(14).Trim()) : "";
                            //FinYeraStartDate  = (reader["FinYearStartDate"] != DBNull.Value) ? (reader.GetString(17).Trim()) : "";
                            //BookStartDate  = (reader["BookStartDate"] != DBNull.Value) ? (reader.GetString(18).Trim()) : "";
                            //WebCustomerDetails = (reader["Website"] != DBNull.Value) ? (reader.GetString(15).Trim()) : "";
                            //BranchesCustomerDetails = (reader["NumberOfBranches"] != DBNull.Value) ? (reader.GetInt32(16)).ToString() : "";
                            //LogoUrlCustomerDetails = (reader["LogoPath"] != DBNull.Value) ? (reader.GetString(25).Trim()) : "";
                            //SubTitleCustomerDetails = (reader["SubTitle"] != DBNull.Value) ? (reader.GetString(26).Trim()) : "";
                            //GSTINCustomerDetails = (reader["GSTIN"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                            //BankNameCustomerDetails = (reader["BankName"] != DBNull.Value) ? (reader.GetString(20).Trim()) : "";
                            //BAddressCustomerDetails = (reader["BAddress"] != DBNull.Value) ? (reader.GetString(21).Trim()) : "";
                            //IFSCCustomerDetails = (reader["IFSC"] != DBNull.Value) ? (reader.GetString(22).Trim()) : "";
                            //AccNumberCustomerDetails = (reader["AccNumber"] != DBNull.Value) ? (reader.GetString(23).Trim()) : "";
                            //HolderCustomerDetails = (reader["Holder"] != DBNull.Value) ? (reader.GetString(24).Trim()) : "";


                        }
                        reader.Close();
                        //cmdCustDetails.Connection.Close();
                        //////

                        //below line for header footer POC
                        writer.PageEvent = new RTSJewelERP.ITextEvents()
                        {
                            //custName = textBoxCustName.Text,
                            //SelectedValueDelivery = ((ComboBoxItem)deliveryBy.SelectedItem).Content.ToString(),
                            //cashCredit = ((ComboBoxItem)cashCredit.SelectedItem).Content.ToString(),
                            //selecteValueParcels = totalParcel.Text,
                            //transportName = transportName.Text,
                            //printName = printName.Text,
                            //mobCust = mobCust.Text,
                            //addressCust = addressCust.Text,
                            //invoiceNumber = (invoiceNumber.Text).Trim(),
                            //BillDate = BillDate.Text,
                            ////BillDate = InvdateValue,
                            ////BillDate = invDate.SelectedDate.Value.ToString("dd/MM/yyyy"),
                            //GSTIN = GSTCust.Text,
                            //State = State.Text,
                            //StateCode = StateCode.Text,
                            //YourOrder = YourOrder.Text,
                            //CashCustName = CashCustName.Text

                            custName = CompanyNameCustomerDetails,
                            SelectedValueDelivery = ((ComboBoxItem)deliveryBy.SelectedItem).Content.ToString(),

                            cashCredit = (CompanyNameCustomerDetails == "Cash") ? "Cash" : "Credit",

                            selecteValueParcels = totalParcel.Text,
                            transportName = transportName.Text,
                            printName = PrintNameCustomerDetails,
                            mobCust = MobCustomerDetails + "," + PhoneCustomerDetails,
                            addressCust = AddressCustomerDetails + "," + Address2CustomerDetails + "," + CityCustomerDetails,
                            invoiceNumber = (invoiceNumber.Text).Trim(),
                            BillDate = invDate.Text,
                            //BillDate = InvdateValue,
                            //BillDate = invDate.SelectedDate.Value.ToString("dd/MM/yyyy"),
                            GSTIN = GSTINCustomerDetails,
                            State = StateCustomerDetails,
                            StateCode = "",
                            YourOrder = YourOrder.Text.Trim(),
                            CashCustName = CashCustName.Text.Trim(),
                            ShippingAddress = (isShipping.IsChecked == true) ? (ShippinAddr1 + "," + ShippinAddr2) : "",
                            EwayNumber = EwayNumbertxt.Text.Trim()

                        };

                        //float sethght = document.PageSize.Height;

                        document.Open();


                        ///////////////comment below code
                        //for (int j = 0; j < CartGrid.Columns.Count; j++)
                        //{
                        //    if (j == 0)
                        //    {
                        //        CartGrid.Columns[0].Header = "S.N";
                        //    }


                        //    table.AddCell(new Phrase(CartGrid.Columns[j].Header.ToString(), tablefontsizeHeader));

                        //}

                        IEnumerable itemsSource = CartGrid.ItemsSource as IEnumerable;
                        if (itemsSource != null)
                        {
                            // foreach (var item in itemsSource)
                            //for (int k = 0; k < CartGrid.Items.Count - 1; ++k)  below line changed bcz grid data not coming when have single row
                            //for (int k = 0; k < CartGrid.Items.Count; ++k)
                            //{
                            //    DataGridRow row = CartGrid.ItemContainerGenerator.ContainerFromItem(itemsSource) as DataGridRow;

                            //    row = CartGrid.ItemContainerGenerator.ContainerFromItem(itemsSource) as DataGridRow;

                            //    if (row == null)
                            //    {
                            //        CartGrid.UpdateLayout();
                            //        CartGrid.ScrollIntoView(CartGrid.Items[k]);
                            //        row = (DataGridRow)CartGrid.ItemContainerGenerator.ContainerFromIndex(k);
                            //    }

                            //    if (row != null)
                            //    {
                            //        DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(row);

                            //        //============
                            //        if (presenter == null)
                            //        {

                            //            CartGrid.UpdateLayout();
                            //            CartGrid.ScrollIntoView(CartGrid.Items[k]);
                            //            row = (DataGridRow)CartGrid.ItemContainerGenerator.ContainerFromIndex(k);
                            //            DataGridCellsPresenter prsnter = FindVisualChild<DataGridCellsPresenter>(row);
                            //            presenter = prsnter;
                            //        }
                            //        //============




                            //        for (int i = 0; i < CartGrid.Columns.Count - 0; ++i)
                            //        {
                            //            DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(i);
                            //            TextBlock txt = cell.Content as TextBlock;
                            //            ComboBox ele = cell.Content as ComboBox;

                            //            if (txt != null)
                            //            {
                            //                if (i == 0)
                            //                {
                            //                    if (k == CartGrid.Items.Count - 0)
                            //                    {
                            //                        float totaltblHorizntal = totalTableHorizontal.TotalHeight;
                            //                        float totalTableHight = totalTable.TotalHeight;
                            //                        float ttlhght = table.TotalHeight;
                            //                        // float footerTblehght = footerTable.TotalHeight;
                            //                        //float bankseparateTaxheght = bankseparateTax.TotalHeight; 
                            //                        float bankseparateTaxheght = 60;
                            //                        float footertablehght = 189;
                            //                        float maxhght = document.PageSize.Height;
                            //                        float balancehght = maxhght - (ttlhght + footertablehght + bankseparateTaxheght + totalTableHight + totaltblHorizntal);

                            //                        Phrase newPhrase = new Phrase(new Phrase((k + 1).ToString(), tablefontsize));
                            //                        iTextSharp.text.pdf.PdfPCell newCell = new iTextSharp.text.pdf.PdfPCell(newPhrase);
                            //                        newCell.FixedHeight = balancehght;

                            //                        table.AddCell(newCell);
                            //                        //table.AddCell(new Phrase((k + 1).ToString(), tablefontsize));
                            //                    }
                            //                    else
                            //                    {
                            //                        table.AddCell(new Phrase((k + 1).ToString(), tablefontsize));
                            //                    }
                            //                }

                            //                else
                            //                {
                            //                    table.AddCell(new Phrase(txt.Text, tablefontsize));

                            //                };
                            //            }

                            //            if (ele != null)
                            //            {
                            //                table.AddCell(new Phrase(ele.Text, tablefontsize));

                            //            }
                            //        }
                            //    }
                            //}


                            ///////////////Commented above code\\\

                            SqlConnection conpdf = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                            conpdf.Open();
                            string sqlpdf = "SELECT row_number() OVER (order by srnumber ) Sr ,[ItemName] As [ITEM NAME]  ,[HSN] ,[BilledQty] As [Qty] ,[UnitID] As [UOM],[SalePrice] As [Price],Amount ,[Discount] As [Disc%] ,[TaxablelAmount] As [Taxable] ,[GSTRate] As [GST%] ,[TotalAmount] As [Total]   FROM [SalesVoucherInventoryByPC] where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "'";
                            SqlCommand cmdpdf = new SqlCommand(sqlpdf);
                            cmdpdf.Connection = conpdf;
                            SqlDataAdapter sda = new SqlDataAdapter(cmdpdf);
                            DataTable dttable = new DataTable("Inv");
                            sda.Fill(dttable);

                            PdfPTable table = new iTextSharp.text.pdf.PdfPTable(dttable.Columns.Count) { TotalWidth = 390, LockedWidth = true };
                            float[] widths = new float[] { 20, 75, 25, 40, 20, 35, 45, 21, 45, 21, 45 }; //remove disc and taxable
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

                                    //}

                                    if ((rows == dttable.Rows.Count - 1) && (column == dttable.Columns.Count - 1))
                                    {


                                        float totaltblHorizntal = totalTableHorizontal.TotalHeight;
                                        float totalTableHight = totalTable.TotalHeight;
                                        float ttlhght = table.TotalHeight;
                                        // float footerTblehght = footerTable.TotalHeight;
                                        //float bankseparateTaxheght = bankseparateTax.TotalHeight; 
                                        float bankseparateTaxheght = 60;//60;
                                        float footertablehght = 189;//189;
                                        float maxhght = document.PageSize.Height;
                                        float balancehght = maxhght - (ttlhght + footertablehght + bankseparateTaxheght + totalTableHight + totaltblHorizntal);

                                        Phrase newPhrase = new Phrase("");
                                        iTextSharp.text.pdf.PdfPCell newCell = new iTextSharp.text.pdf.PdfPCell(newPhrase);
                                        newCell.FixedHeight = balancehght;
                                        //table.AddCell(newCell);

                                        PdfPCellhsn.FixedHeight = balancehght;
                                        table.AddCell(PdfPCellhsn);


                                    }
                                    else
                                        table.AddCell(PdfPCellhsn);
                                }

                            }


                            //In HSN Entry
                            PdfPTable PdfTableHSN = new PdfPTable(8);
                            try
                            {

                                SqlConnection conn1 = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                                conn1.Open();

                                string sql1 = "SELECT HSN,CAST(([GSTRate])/2.0 AS float) As CGSTRate,CAST(([GSTRate])/2.0 AS float) As SGSTRate ,SUM(CAST([GSTTax]/2 as float)) [CGST Tax],SUM(CAST([GSTTax]/2 as float)) [SGST Tax], SUM(CAST([TaxablelAmount] as float)) [Value] from SalesVoucherInventoryByPC where CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "' group by HSN, [GSTRate] order by hsn ";
                                SqlCommand cmd1 = new SqlCommand(sql1);
                                cmd1.Connection = conn1;
                                SqlDataAdapter sda1 = new SqlDataAdapter(cmd1);
                                DataTable dtTemp = new DataTable("emp");
                                sda1.Fill(dtTemp);



                                PdfTableHSN.DefaultCell.Border = 0;
                                // DataTable dt = myDataTable;
                                if (dtTemp != null)
                                {
                                    if (IState)
                                    //if (StateCode.Text == "33") // get state code from firm  firmGSTIN
                                    {
                                        //Craete instance of the pdf table and set the number of column in that table
                                        PdfTableHSN = new PdfPTable(6) { TotalWidth = 185, LockedWidth = true };
                                        float[] widthshsn = new float[] { 35, 30, 30, 30, 30, 30 };
                                        PdfTableHSN.SetWidths(widthshsn);

                                        PdfPCell PdfPCellhsnE0 = null;

                                        PdfPCell PdfPCellhsnE1 = new PdfPCell(new Phrase(new Chunk("HSN/SAC", smallfont)));

                                        PdfPCell PdfPCellhsnE2 = new PdfPCell(new Phrase(new Chunk("CGST%", smallfont)));

                                        PdfPCell PdfPCellhsnE3 = new PdfPCell(new Phrase(new Chunk("SGST%", smallfont)));
                                        // PdfPCell PdfPCellhsn4 = new PdfPCell(new Phrase(new Chunk("IGST%", allFONTsize)));
                                        PdfPCell PdfPCellhsnE5 = new PdfPCell(new Phrase(new Chunk("CGST Tax", smallfont)));
                                        PdfPCell PdfPCellhsnE6 = new PdfPCell(new Phrase(new Chunk("SGST Tax", smallfont)));
                                        // PdfPCell PdfPCellhsn7 = new PdfPCell(new Phrase(new Chunk("IGST Tax", allFONTsize)));
                                        PdfPCell PdfPCellhsnE8 = new PdfPCell(new Phrase(new Chunk("Value", smallfont)));

                                        PdfTableHSN.AddCell(PdfPCellhsnE1);
                                        PdfTableHSN.AddCell(PdfPCellhsnE2);
                                        PdfTableHSN.AddCell(PdfPCellhsnE3);
                                        // PdfTableHSN.AddCell(PdfPCellhsn4);
                                        PdfTableHSN.AddCell(PdfPCellhsnE5);
                                        PdfTableHSN.AddCell(PdfPCellhsnE6);
                                        // PdfTableHSN.AddCell(PdfPCellhsn7);
                                        PdfTableHSN.AddCell(PdfPCellhsnE8);


                                        for (int rows = 0; rows < dtTemp.Rows.Count; rows++)
                                        {
                                            for (int column = 0; column < dtTemp.Columns.Count; column++)
                                            {
                                                if (dtTemp.Rows[rows][column].ToString() != "0")
                                                {
                                                    PdfPCellhsnE0 = new PdfPCell(new Phrase(new Chunk(dtTemp.Rows[rows][column].ToString(), smallfont)));
                                                    PdfTableHSN.AddCell(PdfPCellhsnE0);
                                                }
                                            }
                                        }
                                        //PdfTable.SpacingBefore = 15f; // Give some space after the text or it may overlap the table
                                        //pdfDoc.Add(PdfTable); // add pdf table to the document
                                    }

                                    SqlConnection conn1hs = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                                    conn1hs.Open();

                                    string sql1hs = "SELECT HSN,[GSTRate] As IGSTRate,SUM(CAST([GSTTax] as float)) [IGST Tax] ,SUM(CAST([TaxablelAmount] as float)) [Value] from SalesVoucherInventoryByPC where CompID = '" + CompID + "' and VoucherNumber= '" + VoucherNumber.Text.Trim() + "' group by HSN, [GSTRate] order by hsn ";
                                    SqlCommand cmd1hs = new SqlCommand(sql1hs);
                                    cmd1hs.Connection = conn1hs;
                                    SqlDataAdapter sda1hs = new SqlDataAdapter(cmd1hs);
                                    DataTable dtTemphs = new DataTable("emphs");
                                    sda1hs.Fill(dtTemphs);


                                    if (!IState)  // get state code from firm firmGSTIN
                                    {
                                        //Craete instance of the pdf table and set the number of column in that table
                                        PdfTableHSN = new PdfPTable(4) { TotalWidth = 185, LockedWidth = true };
                                        float[] widthshsnE = new float[] { 35, 30, 30, 120 };
                                        PdfTableHSN.SetWidths(widthshsnE);

                                        PdfPCell PdfPCellhsnE0 = null;

                                        PdfPCell PdfPCellhsnE1 = new PdfPCell(new Phrase(new Chunk("HSN/SAC", smallfont)));

                                        PdfPCell PdfPCellhsnE4 = new PdfPCell(new Phrase(new Chunk("IGST%", smallfont)));

                                        PdfPCell PdfPCellhsnE7 = new PdfPCell(new Phrase(new Chunk("IGST", smallfont)));
                                        PdfPCell PdfPCellhsnE8 = new PdfPCell(new Phrase(new Chunk("Value", smallfont)));

                                        PdfTableHSN.AddCell(PdfPCellhsnE1);
                                        //PdfTableHSN.AddCell(PdfPCellhsn2);
                                        //PdfTableHSN.AddCell(PdfPCellhsn3);
                                        PdfTableHSN.AddCell(PdfPCellhsnE4);
                                        //PdfTableHSN.AddCell(PdfPCellhsn5);
                                        //PdfTableHSN.AddCell(PdfPCellhsn6);
                                        PdfTableHSN.AddCell(PdfPCellhsnE7);
                                        PdfTableHSN.AddCell(PdfPCellhsnE8);


                                        for (int rows = 0; rows < dtTemphs.Rows.Count; rows++)
                                        {
                                            for (int column = 0; column < dtTemphs.Columns.Count; column++)
                                            {
                                                if (dtTemphs.Rows[rows][column].ToString() != "0")
                                                {
                                                    PdfPCellhsnE0 = new PdfPCell(new Phrase(new Chunk(dtTemphs.Rows[rows][column].ToString(), smallfont)));
                                                    PdfTableHSN.AddCell(PdfPCellhsnE0);
                                                }
                                            }
                                        }
                                        //PdfTable.SpacingBefore = 15f; // Give some space after the text or it may overlap the table
                                        //pdfDoc.Add(PdfTable); // add pdf table to the document
                                    }
                                }
                            } //try close

                            catch (Exception ex)
                            {
                                MessageBox.Show("In HSN Entry ");

                            }


                            bankseparateTax.AddCell(ourbankdetails1cell);
                            bankseparateTax.AddCell(PdfTableHSN);
                            //bankseparateTax.AddCell(PdfTableHSN); //commented for Hitesh
                            // bankseparateTax.AddCell(PdfTableHSNcell);
                            //bankseparateTax.AddCell(bankDetails);
                            bankseparateTax.AddCell(ForFirm);


                            //Auto Increment invoice/quotation number
                            //int billquoteNo = Convert.ToInt32(billQuoteNumber) + 1;
                            //File.WriteAllText(@"c:\RTSProSoft\Database\BillNumber.txt", billquoteNo.ToString(), Encoding.UTF8);

                            document.Add(jpg);
                            document.Add(jpg2);

                            document.Add(table);
                            //document.Add(totalTableHorizontal);

                            // document.Add(p);
                            document.Add(totalTable);

                            // document.Add(footerTable);
                            document.Add(bankseparateTax);
                            //document.Add(chunkRupee);

                            //document.Add(PdfTableHSN);

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



                            //try
                            //{
                            //    PrinterSettings ps = new PrinterSettings();
                            //    PrintDocument recordDoc = new PrintDocument();
                            //    recordDoc.PrinterSettings = ps;

                            //    IEnumerable<PaperSize> paperSizes = ps.PaperSizes.Cast<PaperSize>();
                            //    PaperSize sizeA4 = paperSizes.First<PaperSize>(size => size.Kind == PaperKind.A5); // setting paper size to A4 size
                            //    recordDoc.DefaultPageSettings.PaperSize = sizeA4;


                            //    Direct send pdf to Printer from the saved pdf location.
                            //    ProcessStartInfo info = new ProcessStartInfo();
                            //    info.Verb = "print";

                            //    process.StartInfo.FileName = @"C:\ViewBill\" + "Bill-" + (invoiceNumber.Text).Trim() + "-" + autocompltCustName.autoTextBox.Text + ".pdf";


                            //    info.FileName = @"C:\ViewBill\" + "Bill-" + (invoiceNumber.Text).Trim() + "-" + autocompltCustName.autoTextBox.Text + ".pdf";
                            //    }
                            //    else
                            //        info.FileName = @"C:\ViewBill\Barcode\barcode-" + txtBarcode.Text.Trim() + ".pdf";


                            //    info.CreateNoWindow = true;
                            //    info.WindowStyle = ProcessWindowStyle.Hidden;

                               
                            //    Process p = new Process();
                                
                            //    p.StartInfo = info;
                            //    p.Start();
                            //    p.WaitForInputIdle();
                            //    System.Threading.Thread.Sleep(5000);
                            //    if (false == p.CloseMainWindow())
                            //    {
                            //        p.Kill();
                            //    }

                            //}






                            try
                            {

                                //Open RTSProSoft Folder On PDf button Click
                                Process process = new Process();
                                process.StartInfo.UseShellExecute = true;
                                process.StartInfo.FileName = @"C:\ViewBill\" + "Bill-" + (invoiceNumber.Text).Trim() + "-" + autocompltCustName.autoTextBox.Text + ".pdf";
                                //process.StartInfo.FileName = @"C:\ViewBill\" + "Bill-" + (invoiceNumber.Text).Trim() + "-" + custName.Text + ".pdf";
                                //process.StartInfo.FileName = @"C:\RTSProSoft\";

                                process.Start();
                                process.Close();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("In Procees. Start");
                            }


                        }
                        // return output.ToArray();
                    }
                    //}// main try close
                    //catch (Exception exc)
                    //{
                    //    MessageBox.Show("Please check bill in RTSProSoft folder");
                    //}
                } //confirmation message to generate PDF

                SaleVoucherAllInOneQtyGSTSteel sv = new SaleVoucherAllInOneQtyGSTSteel("");
                //SaleVoucherBarcode sv = new SaleVoucherBarcode();
                this.NavigationService.Navigate(sv);
            }
            catch (Exception e)
            {
                MessageBox.Show("Close PDF Invoice and Re-Print");
            }
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
            if (autocompltCustName.autoTextBox.Text == "Card")
            {
                receivedCash.Clear();
                receivedCard.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            }
            if (autocompltCustName.autoTextBox.Text == "Cash")
            {
                receivedCard.Clear();
                receivedCash.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            }


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
            //double roundoffamt = (txtRoundOff.Text.Trim() == "") ? 0 : Convert.ToDouble(txtRoundOff.Text.Trim());
            double cashreceived = (receivedCash.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCash.Text.Trim());
            double cardreceived = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());
            double paytmreceived = (receivedPaytm.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedPaytm.Text.Trim());
            double flatoff = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());

            double offerzone = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
            double loyaltycard = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());

            dueBal.Content = string.Format("Balance:  {0}", Math.Round((totalVal - oldtotalVal - (cashreceived + cardreceived + paytmreceived + flatoff + offerzone + loyaltycard)), 0)).ToString();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {

            MoveToBill(invoiceNumber.Text.Trim());
        }

        private void MoveToBill(string invnumbertxt)
        {
            CleanUp();
            isShipping.IsChecked = false;
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

            //load data from DB into CartGrid
            //invoiceNumber.Text
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            conn.Open();

            string sqlother = "select * from SalesVouchersOtherDetails where LTRIM(RTRIM(InvoiceNumber))='" + invnumbertxt + "' and CompID = '" + CompID + "'";
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


            //string sql = "select ItemName,HSN,BilledQty,BilledWt,WastePerc,TotalBilledWt,MakingCharge,SalePrice,TotalAmount,Discount,TaxablelAmount,TotalAmount,GSTRate,GSTTax,TotalAmount from SalesVoucherInventory where LTRIM(RTRIM(InvoiceNumber))='" + invoiceNumber.Text + "' and CompID = '" + CompID + "'";
            string sql = "select ItemName,HSN,BilledQty,SalePrice,TotalAmount,Discount,TaxablelAmount,GSTRate,GSTTax,Amount,UnitID from SalesVoucherInventoryByPC where LTRIM(RTRIM(InvoiceNumber))='" + invnumbertxt + "' and CompID = '" + CompID + "'  order by srnumber";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = conn;
            SqlDataReader reader = cmd.ExecuteReader();

            double dbilledQty = 0;
            //double dbilledWts = 0;
            //double dWastePerc = 0;
            //double dmakingcharge = 0;
            double dsaleprice = 0;
            double ddisperc = 0;
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
                //we add the product to the Cart
                ShoppingCart.Add(new Product()
                {
                    HSN = (reader["HSN"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "",
                    //BilledWt = dbilledWts,
                    UnitID = (reader["UnitID"] != DBNull.Value) ? (reader.GetString(10).Trim()) : "Pc",
                    ItemName = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "",
                    ItemPrice = dsaleprice,
                    BilledQty = dbilledQty,
                    //WastagePerc = dWastePerc,
                    //MC = dmakingcharge,
                    SaleDiscountPerc = ddisperc,
                    GSTRate = dgstrate
                });
                BindDataGrid();

            }
            reader.Close();

            autocompltCustName.autoTextBox.Focus();

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
            //AddInstantAccoun tasd = new AddSundryDebtor();
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
            string itemName = autocompleteItemName.autoTextBox1.Text.Trim();
            //string customerName = row["CustomerName"].ToString();
            //string otherCharge = row["AnyotherCharges"].ToString();
            //string statecodeCust = row["GSTIN"].ToString();
            //statecodeCust = statecodeCust.Trim().Substring(0, 2);
            ShowItemInfo showItemInfor = new ShowItemInfo(itemName, CompID);
            showItemInfor.ShowDialog();
        }

        private void txtGSTRate_LostFocus(object sender, RoutedEventArgs e)
        {
            txtGSTRate.Background = Brushes.White;
            txtGSTRate.Foreground = Brushes.Black;
            AddItemRow.Focus();
        }

        private void autocompleteItemName_LostFocus(object sender, RoutedEventArgs e)
        {
            autocompleteItemName.autoTextBox1.Background = Brushes.White;
            autocompleteItemName.autoTextBox1.Foreground = Brushes.Black;

            if (autocompltCustName.autoTextBox.Text == "Card")
            {
                receivedCash.Clear();
                receivedCard.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            }
            if (autocompltCustName.autoTextBox.Text == "Cash")
            {
                receivedCard.Clear();
                receivedCash.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            }

            if (autocompltCustName.autoTextBox.Text != "Cash")
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
                    string isSoldAlert = (reader["IsSoldFlag"] != DBNull.Value) ? (reader.GetBoolean(72).ToString()) : "False";
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

                    //tmpProduct.HSN = "9503";  //HSN

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
                    HSN.Text = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
                    txtGSTRate.Text = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)).ToString().Trim() : "";
                    //autocompleteItemName.autoTextBox1.Text = tmpProduct.ItemBarCode.ToString();
                    //Get Counter , Tray and Storage Name by another call, get all count by sp or direct call for inventory 
                    cmbStorage.Text = (reader["StorageName"] != DBNull.Value) ? (reader.GetString(79).Trim()) : "";
                    //CounterName.Text = (reader["CounterName"] != DBNull.Value) ? (reader.GetString(80).Trim()) : "";
                    cmbTray.Text = (reader["TrayName"] != DBNull.Value) ? (reader.GetString(81).Trim()) : "";
                    //cmbUnits.Text = tmpProduct.UnitID.ToString();
                    cmbUnits.Text = (tmpProduct.UnitID.ToString() != "") ? tmpProduct.UnitID.ToString() : "Pc";
                    //txtMC.Text = (reader["MakingCharge"] != DBNull.Value) ? (reader.GetDouble(94)).ToString().Trim() : "";
                    //txtPrice.Text = (reader["RatePerGm"] != DBNull.Value) ? (reader.GetDouble(95)).ToString().Trim() : "";
                    txtPrice.Text = (reader["ItemPrice"] != DBNull.Value) ? (reader.GetDouble(9)).ToString().Trim() : "";

                    txtQtyStockEntry.Text = tmpProduct.ActualQty.ToString();
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
            string GSTINAcct = "";
            string GSTINCompany = "";
            if (autocompltCustName.autoTextBox.Text != "Cash")
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
                string sql = "select AcctName,GSTIN,State,Mobile1,* from AccountsList where LTRIM(RTRIM(AcctName)) = '" + autocompltCustName.autoTextBox.Text + "' and CompID = '" + CompID + "'";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                tmpProduct = new Product();

                while (reader.Read())
                {


                    //var CustID = reader.GetValue(0).ToString();

                    //tmpProduct.ItemName = (reader["AcctName"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "";
                    GSTINAcct = (reader["GSTIN"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "";
                    txtGSTIN.Text = GSTINAcct;
                    txtState.Text = (reader["State"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                    txtMob.Text = (reader["Mobile1"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";

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

        private void packingCharge_LostFocus(object sender, RoutedEventArgs e)
        {
            //double packandforward = (packingCharge.Text.Trim() != "") ? Convert.ToDouble(packingCharge.Text.Trim()) : 0.0;
            //lbGrandTotalSum.Content = string.Format("Grand Total: {0}", (Math.Round((totalVal - oldtotalVal + packandforward), 0)).ToString("C"));

            //var textBox = e.OriginalSource as TextBox;
            //textBox.Background = Brushes.White;
            //textBox.Foreground = Brushes.Black;
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


        private void invoiceShortcut_Click(object sender, RoutedEventArgs e)
        {
            invoiceNumber.Focus();
        }

        private void dateShortcut_Click(object sender, RoutedEventArgs e)
        {
            invDate.IsDropDownOpen = true;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
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
                autocompltCustName.autoTextBox.Focus();
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

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            HomePage hp = new HomePage();
            this.NavigationService.Navigate(hp);
        }
    }
}
