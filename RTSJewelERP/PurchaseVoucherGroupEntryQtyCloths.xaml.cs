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
    public partial class PurchaseVoucherGroupEntryQtyCloths : Page
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
        public PurchaseVoucherGroupEntryQtyCloths()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window
            BindComboBoxUnits(cmbUnits);
            dueBal.Content = string.Format("Balance: {0}", (BalanceCRorDR).ToString("C"));


            //on the constructor of the class we create a new instance of the shooping cart
            ShoppingCart = new List<Product>();
            OldCart = new List<Product>();
            //autocompleteItemName.autoTextBoxGroup.Focus();
            autocompltCustName.autoTextBox.Focus();

            //txtBarCode.Focus();


            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            ////SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            //string sql = "select number from AutoIncrement where Name = 'SaleInvoice' and CompID = '" + CompID + "'";
            //SqlCommand cmd = new SqlCommand(sql);
            //cmd.Connection = con;
            //SqlDataReader reader = cmd.ExecuteReader();

            ////tmpProduct = new Product();

            //while (reader.Read())
            //{
            //    InvoiceNumber = reader.GetInt64(0);
            //    invoiceNumber.Text = InvoiceNumber.ToString();

            //}
            //reader.Close();

            string sqlvoucher = "select number from AutoIncrement where Name = 'PurchaseVoucher' and CompID = '" + CompID + "'";
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
                if (Convert.ToInt64(VoucherNumber.Text.Trim()) < voucherNumber)
                {
                    Int64 inpageup = (VoucherNumber.Text.Trim() != "") ? (Convert.ToInt64(VoucherNumber.Text.Trim()) + 1) : 0;
                    VoucherNumber.Text = inpageup.ToString();
                    // VoucherNumber.Text = voucherNumber.ToString();
                    MoveToBill(inpageup.ToString());

                }
                if (Convert.ToInt64(VoucherNumber.Text.Trim()) == voucherNumber)
                {
                    autocompltCustName.autoTextBox.Text = "Cash";
                    autocompltCustName.autoTextBox.Focus();
                }
                e.Handled = true;
            }
            if (e.Key == Key.PageDown)
            {
                if (Convert.ToInt64(VoucherNumber.Text.Trim()) > 1)
                {
                    Int64 inpageup = (VoucherNumber.Text.Trim() != "") ? (Convert.ToInt64(VoucherNumber.Text.Trim()) - 1) : 0;
                    VoucherNumber.Text = inpageup.ToString();
                    MoveToBill(inpageup.ToString());
                    e.Handled = true;
                }
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

        //private void TextBoxCust_KeyUp(object sender, KeyEventArgs e)
        //{
        //    if (autocompltCustName.autoTextBox.Text != "Cash")
        //    {
        //        CashCustName.Visibility = Visibility.Collapsed;
        //        //CashName.Visibility = Visibility.Collapsed;
        //    }
        //    else
        //    {
        //        //CashName.Visibility = Visibility.Visible;
        //        CashCustName.Visibility = Visibility.Visible;
        //    }

        //    bool found = false;
        //    var border = (resultStackCust.Parent as ScrollViewer).Parent as Border;
        //    //var data ;
        //    //= Model.GetData();

        //    //If a product code is not empty we search the database
        //    if (Regex.IsMatch(autocompltCustName.autoTextBox.Text.Trim(), @"^\d+$") || 1 == 1)
        //    {
        //        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
        //        //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
        //        con.Open();
        //        string sql = "select AcctName from AccountsList where AcctName like '%" + autocompltCustName.autoTextBox + "%' and CompID = '" + CompID + "'";
        //        SqlCommand cmd = new SqlCommand(sql);
        //        cmd.Connection = con;
        //        SqlDataReader reader = cmd.ExecuteReader();

        //        tmpProduct = new Product();

        //        string query = (sender as TextBox).Text;

        //        if (query.Length == 0)
        //        {
        //            // Clear    
        //            resultStackCust.Children.Clear();
        //            border.Visibility = System.Windows.Visibility.Collapsed;
        //        }
        //        else
        //        {
        //            border.Visibility = System.Windows.Visibility.Visible;
        //        }

        //        // Clear the list    
        //        resultStackCust.Children.Clear();

        //        while (reader.Read())
        //        {
        //            //var CustID = reader.GetValue(0).ToString();

        //            tmpProduct.ItemName = reader.GetString(0).Trim();
        //            if (tmpProduct.ItemName.ToLower().Contains(query.ToLower()))
        //            {
        //                // The word starts with this... Autocomplete must work    
        //                addCust(tmpProduct.ItemName);



        //                found = true;
        //            }
        //            //tmpProduct.PrintName = reader.GetString(3).Trim();
        //            //tmpProduct.ItemCode = reader.GetString(5).Trim();
        //            //tmpProduct.ItemBarCode = reader.GetString(7).Trim();

        //            //tmpProduct.ItemPrice = reader.GetDouble(9);
        //            //tmpProduct.ActualQty = reader.GetDouble(35);
        //            //tmpProduct.ActualWt = reader.GetDouble(46);

        //        }
        //        reader.Close();
        //    }









        //    // Add the result    
        //    //foreach (var obj in data)
        //    //{

        //    //}

        //    if (!found)
        //    {
        //        resultStackCust.Children.Add(new TextBlock() { Text = "No results found." });
        //    }
        //}

        //private void TextBox_KeyUp(object sender, KeyEventArgs e)
        //{
        //    bool found = false;
        //    var border = (resultStack.Parent as ScrollViewer).Parent as Border;
        //    //var data ;
        //    //= Model.GetData();

        //    //If a product code is not empty we search the database
        //    if (Regex.IsMatch(autocompleteItemName.autoTextBoxGroup.Text.Trim(), @"^\d+$") || 1 == 1)
        //    {
        //        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
        //        //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
        //        con.Open();
        //        string sql = "select ItemName from StockItemsByPC where ItemName like '%" + autocompleteItemName.autoTextBoxGroup.Text + "%' and CompID = '" + CompID + "'";
        //        SqlCommand cmd = new SqlCommand(sql);
        //        cmd.Connection = con;
        //        SqlDataReader reader = cmd.ExecuteReader();

        //        tmpProduct = new Product();

        //        string query = (sender as TextBox).Text;

        //        if (query.Length == 0)
        //        {
        //            // Clear    
        //            resultStack.Children.Clear();
        //            border.Visibility = System.Windows.Visibility.Collapsed;
        //        }
        //        else
        //        {
        //            border.Visibility = System.Windows.Visibility.Visible;
        //        }

        //        // Clear the list    
        //        resultStack.Children.Clear();

        //        while (reader.Read())
        //        {
        //            //var CustID = reader.GetValue(0).ToString();

        //            tmpProduct.ItemName = reader.GetString(0).Trim();
        //            if (tmpProduct.ItemName.ToLower().Contains(query.ToLower()))
        //            {
        //                // The word starts with this... Autocomplete must work    
        //                addItem(tmpProduct.ItemName);



        //                found = true;

        //            }
        //            //tmpProduct.PrintName = reader.GetString(3).Trim();
        //            //tmpProduct.ItemCode = reader.GetString(5).Trim();
        //            //tmpProduct.ItemBarCode = reader.GetString(7).Trim();

        //            //tmpProduct.ItemPrice = reader.GetDouble(9);
        //            //tmpProduct.ActualQty = reader.GetDouble(35);
        //            //tmpProduct.ActualWt = reader.GetDouble(46);

        //        }
        //        reader.Close();
        //    }









        //    // Add the result    
        //    //foreach (var obj in data)
        //    //{

        //    //}

        //    if (!found)
        //    {
        //        resultStack.Children.Add(new TextBlock() { Text = "No results found." });
        //    }
        //}

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

            if (autocompleteItemName.autoTextBoxGroup.Text.Trim() != "")
            {


                //product quantity
                double qty;
                double wtqty;

                // we try to parse the number of the textbox if the number is invalid 
                double.TryParse(txtQty.Text, out qty);
                //double.TryParse(txtWeight.Text, out wtqty);
                //if qty is 0 we assign 0 otherwise we assign the actual parsed value
                qty = qty == 0 ? 1 : qty;
                //really basic validation that checks inventory
                //if (tmpProduct.ItemName == "Old Gold" || tmpProduct.ItemName == "Old Silver" || autocompleteItemName.autoTextBoxGroup.Text == "Old Gold" || autocompleteItemName.autoTextBoxGroup.Text == "Old Silver")
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
                //        ItemName = tmpProduct.ItemName != null ? tmpProduct.ItemName : autocompleteItemName.autoTextBoxGroup.Text,
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
                //    autocompleteItemName.autoTextBoxGroup.Text = string.Empty;
                //    //autocompleteItemName.autoTextBoxGroup.Text = string.Empty;
                //    //txtQty.Text = string.Empty;
                //    txtQty.Text = "1";
                //    txtDiscPerc.Text = string.Empty;
                //    txtGSTRate.Text = string.Empty;
                //    txtMC.Text = string.Empty;
                //    txtWeight.Text = string.Empty;
                //    txtWaste.Text = string.Empty;
                //    txtPrice.Text = string.Empty;
                //    autocompleteItemName.autoTextBoxGroup.Focus(); // Uncomment for without Barcode
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
                    ShoppingCart.RemoveAll(s => s.ItemName == tmpProduct.ItemName); // Remove Existing item if same barcode
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
                    autocompleteItemName.autoTextBoxGroup.Text = string.Empty;
                    //autocompleteItemName.autoTextBoxGroup.Text = string.Empty;
                    txtQty.Text = "1";
                    txtDiscPerc.Text = string.Empty;
                    txtGSTRate.Text = string.Empty;
                    //txtMC.Text = string.Empty;
                    //txtWeight.Text = string.Empty;
                    //txtWaste.Text = string.Empty;
                    txtPrice.Text = string.Empty;
                    autocompleteItemName.autoTextBoxGroup.Focus();

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
                    ShoppingCart.RemoveAll(s => s.ItemName == tmpProduct.ItemName);
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
                        ItemName = autocompleteItemName.autoTextBoxGroup.Text,// tmpProduct.ItemName,                                              
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
                    autocompleteItemName.autoTextBoxGroup.Text = string.Empty;
                    //autocompleteItemName.autoTextBoxGroup.Text = string.Empty;
                    txtQty.Text = "1";
                    txtDiscPerc.Text = string.Empty;
                    txtGSTRate.Text = string.Empty;
                    //txtMC.Text = string.Empty;
                    //txtWeight.Text = string.Empty;
                    //txtWaste.Text = string.Empty;
                    txtPrice.Text = string.Empty;
                    autocompleteItemName.autoTextBoxGroup.Focus();

                    //---------------Write Code Below to Add Item in StockItems Dynamically with minimum data, if some data not provided then send the item to Pending tasks

                }



            }

            else
            {
                MessageBox.Show("Product is Empty");
                autocompleteItemName.autoTextBoxGroup.Focus();

            }

            //TxtProdCode.Focus();
        }


        private void BindDataGrid()
        {
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

            double packCharge = (PackCharge.Text.Trim() == "") ? 0 : Convert.ToDouble(PackCharge.Text.Trim());
            lbGrandSum.Content = string.Format("Grand Sum: {0}", (Math.Round((totalVal - oldtotalVal + packCharge), 0)).ToString("C"));


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

            //invoiceNumber.Text = InvoiceNumber.ToString();
            VoucherNumber.Text = voucherNumber.ToString();
        }

        //this method will clear/reset form values
        private void CleanUp()
        {
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
            autocompleteItemName.autoTextBoxGroup.Text = string.Empty;
            autocompleteItemName.autoTextBoxGroup.Text = string.Empty;
            txtQty.Text = string.Empty;
            lbTotal.Content = "Total: ₹ 0.00";
            //lbOldTotal.Content = "Total: ₹ 0.00";
            lbGrandTotal.Content = "Total: ₹ 0.00";
            lbGrandSum.Content = "Total: ₹ 0.00";
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
            if (Regex.IsMatch(autocompleteItemName.autoTextBoxGroup.Text.Trim(), @"^\d+$"))
            {
                //DBInvoiceSample db = new DBInvoiceSample();
                ////parse the product code as int from the TextBox
                //int id = int.Parse(autocompleteItemName.autoTextBoxGroup.Text);
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
            if (Regex.IsMatch(autocompleteItemName.autoTextBoxGroup.Text.Trim(), @"^\d+$") || 1 == 1)
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select * from StockItemsByPC where ItemName = '" + autocompleteItemName.autoTextBoxGroup.Text + "' and CompID = '" + CompID + "'";
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
                    autocompleteItemName.autoTextBoxGroup.Text = tmpProduct.ItemBarCode.ToString();
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
            //if (Regex.IsMatch(autocompleteItemName.autoTextBoxGroup.Text.Trim(), @"^\d+$") || 1==1)
            //{
            //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            //    con.Open();
            //    string sql = "select * from StockItems where ItemName = '" + autocompleteItemName.autoTextBoxGroup.Text + "'";
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
            //int id = int.Parse(autocompleteItemName.autoTextBoxGroup.Text);
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
        private void PrintSimpleTextButton_Click(object sender, RoutedEventArgs e)
        {

            if (invoiceNumber.Text.Trim() != "")
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
                    string CountSVEntryStr = "SELECT COUNT(*) From PurchaseVoucherInventoryByGroup where VoucherNumber= '" + VoucherNumber.Text.Trim() + "' and InvoiceNumber='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "'";
                    // string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                    SqlCommand myCommandDel = new SqlCommand(CountSVEntryStr, myConnSVEntryStr);
                    myCommandDel.Connection = myConnSVEntryStr;

                    //int countRec = myCommand.ExecuteNonQuery();
                    int countRecDelDel = (int)myCommandDel.ExecuteScalar();
                    myCommandDel.Connection.Close();
                    if (countRecDelDel != 0)
                    {
                        // MessageBox.Show("Item Name is already Exist, Please delete existing", "Add Record");


                        SqlCommand myCommandDeleteDel = new SqlCommand("SPUpdateStockOnPurchaseVoucherGroupChangeOrDelete", myConnSVEntryStr);
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
                            DataGridCell cellItemName = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(2);
                            //TextBlock txtItemNam = cellItemName.Content as TextBlock;
                            TextBlock txtItemNam = cellItemName.Content as TextBlock;

                            DataGridCell cellHSN = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(3);
                            TextBlock hsnText = cellHSN.Content as TextBlock;


                            // for Qty

                            DataGridCell cellQty = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(4);
                            TextBlock qtyText = cellQty.Content as TextBlock;

                            DataGridCell cellUnitID = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(5);
                            TextBlock txtcellUnitID = cellUnitID.Content as TextBlock;

                            DataGridCell cellPrice = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(6);
                            TextBlock priceText = cellPrice.Content as TextBlock;


                            DataGridCell cellAmount = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(7);
                            TextBlock txtCellAmount = cellAmount.Content as TextBlock;


                            DataGridCell discRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(8);
                            TextBlock txtdiscRate = discRate.Content as TextBlock;

                            DataGridCell cellTaxableAmt = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(9);
                            TextBlock txtTaxableAmt = cellTaxableAmt.Content as TextBlock;

                            DataGridCell gstRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(10);
                            TextBlock txtgstRate = gstRate.Content as TextBlock;

                            DataGridCell gstTax = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(11);
                            TextBlock txtgsTax = gstTax.Content as TextBlock;


                            DataGridCell cellTotal = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(12);
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
                            querySalesInventory = "insert into PurchaseVoucherInventoryByGroup(VoucherNumber, InvoiceNumber,GroupName,HSN,BuyPrice,GSTRate,GSTTax,Discount,TaxablelAmount,TotalAmount, BilledQty,UnitID,TransactionDate,FromConsumedStorageID,FromConsumedTrayID,FromConsumedCounterID,CompID,Amount) Values ( '" + VoucherNumber.Text + "','" + invoiceNumber.Text.Trim() + "','" + txtItemNam.Text + "','" + hsnText.Text + "','" + priceText.Text + "','" + txtgstRate.Text + "','" + txtgsTax.Text + "','" + txtdiscRate.Text + "', '" + txtTaxableAmt.Text + "','" + totalText.Text + "','" + qtyText.Text + "', '" + txtcellUnitID.Text + "','" + InvdateValue + "','1','1','1', '" + CompID + "','" + txtCellAmount.Text + "')";



                            SqlCommand myCommandSVInventory = new SqlCommand(querySalesInventory, myConSVInventoryStr);
                            myCommandSVInventory.Connection = myConSVInventoryStr;
                            //myCommandInvEntry.Connection.Open();
                            int NumPI = myCommandSVInventory.ExecuteNonQuery();
                            myCommandSVInventory.Connection.Close();


                            // Here Just write below the code to transfer data from PurchasevoucherItemAllocation table to update StockItemsByPc with actual stock and other details. this will be fetched in ReadymadeAllocation screen to fill Qty and GST Rate, Price etc.  


                            SqlCommand myCommandStkItemtblUpdate = new SqlCommand("SPUpdateStockOnStockItemsByPCfromPurchaseAllocationInventory", myConnSVEntryStr);
                            myCommandStkItemtblUpdate.CommandType = CommandType.StoredProcedure;
                            myCommandStkItemtblUpdate.Parameters.Add(new SqlParameter("@VoucherNumber", Convert.ToInt64(VoucherNumber.Text.Trim())));
                            myCommandStkItemtblUpdate.Parameters.Add(new SqlParameter("@InvoiceNumber", invoiceNumber.Text.Trim()));
                            myCommandStkItemtblUpdate.Parameters.Add(new SqlParameter("@CompID", CompID));
                            myCommandStkItemtblUpdate.Connection.Open();
                            int countReclStkItemtblUpdate = myCommandStkItemtblUpdate.ExecuteNonQuery();
                            if (countReclStkItemtblUpdate != 0)
                            {
                                //  MessageBox.Show("Record Successfully Deleted....", "Delete Record");
                            }
                            myCommandStkItemtblUpdate.Connection.Close();


                            

     

                        }
                    }






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
                cmdCommon = new SqlCommand("[SPUpdateAccountsForPurchaseVoucher]", conStrCommon);
                cmdCommon.CommandType = CommandType.StoredProcedure;
                cmdCommon.Parameters.Add(new SqlParameter("@SundryCreditorName", autocompltCustName.autoTextBox.Text));
                cmdCommon.Parameters.Add(new SqlParameter("@PurchaseAcctName", SaleAcctName));
                cmdCommon.Parameters.Add(new SqlParameter("@IsNewSundryCreditor", "No"));
                if (CashCustName.Text != "")
                {
                    cmdCommon.Parameters.Add(new SqlParameter("@CashPartyName", CashCustName.Text));
                    cmdCommon.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Cash"));
                }
                else
                {
                    cmdCommon.Parameters.Add(new SqlParameter("@CashPartyName", ""));
                    cmdCommon.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Credit"));
                }
                cmdCommon.Parameters.Add(new SqlParameter("@InvoiceNumber", invoiceNumber.Text));
                cmdCommon.Parameters.Add(new SqlParameter("@PurVoucherNumber", Convert.ToInt64(VoucherNumber.Text.Trim())));
                cmdCommon.Parameters.Add(new SqlParameter("@PurVoucherType", "Purchase Voucher"));
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
                    cmdCommon.Parameters.Add(new SqlParameter("@InputCGST", totalTaxAmount / 2));
                    cmdCommon.Parameters.Add(new SqlParameter("@InputSGST", totalTaxAmount / 2));
                    cmdCommon.Parameters.Add(new SqlParameter("@InputIGST", outputigstval));
                }
                else
                {
                    double outputsgstval = 0.0;


                    cmdCommon.Parameters.Add(new SqlParameter("@InputCGST", outputsgstval));
                    cmdCommon.Parameters.Add(new SqlParameter("@InputSGST", outputsgstval));
                    cmdCommon.Parameters.Add(new SqlParameter("@InputIGST", totalTaxAmount));
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
                double packCharge = (PackCharge.Text.Trim() == "") ? 0 : Convert.ToDouble(PackCharge.Text.Trim());
                cmdCommon.Parameters.Add(new SqlParameter("@TotalInvValue", totalInvValues - oldtotalVal + packCharge));
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
                cmdCommonother = new SqlCommand("SPUpdateAccountsForPurchaseVoucherOtherDetails", conStrCommon);
                cmdCommonother.CommandType = CommandType.StoredProcedure;
                cmdCommonother.Parameters.Add(new SqlParameter("@SundryCreditorName", autocompltCustName.autoTextBox.Text));
                cmdCommonother.Parameters.Add(new SqlParameter("@PurchaseAcctName", SaleAcctName));
                cmdCommonother.Parameters.Add(new SqlParameter("@IsNewSundryDebtor", "No"));
                if (CashCustName.Text != "")
                {
                    cmdCommonother.Parameters.Add(new SqlParameter("@CashPartyName", CashCustName.Text));
                    cmdCommonother.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Cash"));
                }
                else
                {
                    cmdCommonother.Parameters.Add(new SqlParameter("@CashPartyName", ""));
                    cmdCommonother.Parameters.Add(new SqlParameter("@IsCashOrCredit", "Credit"));
                }
                cmdCommonother.Parameters.Add(new SqlParameter("@InvoiceNumber", invoiceNumber.Text));
                cmdCommonother.Parameters.Add(new SqlParameter("@PurVoucherNumber", Convert.ToInt64(VoucherNumber.Text.Trim())));
                cmdCommonother.Parameters.Add(new SqlParameter("@PurVoucherType", "Purchase Voucher"));
                cmdCommonother.Parameters.Add(new SqlParameter("@EwayNumber", EwayNumbertxt.Text));

                cmdCommonother.Parameters.Add(new SqlParameter("@InvDate", BillDateInvValval));

                //check isState or central with company statecode            
                cmdCommonother.Parameters.Add(new SqlParameter("@IsState", IState.ToString()));
                discounttotalCommon = (discountTxt.Text.Trim() == "") ? 0 : Convert.ToDouble(discountTxt.Text.Trim());
                cmdCommonother.Parameters.Add(new SqlParameter("@Discount", discounttotalCommon)); //gettotal Discount-Common 
                if (IState)
                {
                    double outputigstval = 0.0;
                    cmdCommonother.Parameters.Add(new SqlParameter("@InputCGST", totalTaxAmount / 2));
                    cmdCommonother.Parameters.Add(new SqlParameter("@InputSGST", totalTaxAmount / 2));
                    cmdCommonother.Parameters.Add(new SqlParameter("@InputIGST", outputigstval));
                }
                else
                {
                    double outputsgstval = 0.0;


                    cmdCommonother.Parameters.Add(new SqlParameter("@InputCGST", outputsgstval));
                    cmdCommonother.Parameters.Add(new SqlParameter("@InputSGST", outputsgstval));
                    cmdCommonother.Parameters.Add(new SqlParameter("@InputIGST", totalTaxAmount));
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
                //double txtPackForwd = (PackCharge.Text.Trim() == "") ? 0 : Convert.ToDouble(PackCharge.Text.Trim());
                //int totalParcl = (totalParcel.Text.Trim() == "") ? 0 : Convert.ToInt32(totalParcel.Text.Trim());
                //double offerzone = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
                //double loyaltycard = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());

                //double zeroValval = 0.0;

                cmdCommonother.Parameters.Add(new SqlParameter("@Labour", labourTotal));
                cmdCommonother.Parameters.Add(new SqlParameter("@MakingCharges", makingTotalCharge));
                packCharge = (PackCharge.Text.Trim() == "") ? 0 : Convert.ToDouble(PackCharge.Text.Trim());
                cmdCommonother.Parameters.Add(new SqlParameter("@TotalInvValue", totalInvValues - oldtotalVal + packCharge));
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



                if (voucherNumber == Convert.ToInt64(VoucherNumber.Text))
                {
                    SqlConnection consrauto = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                    consrauto.Open();
                    string updateVoucher = "";
                    // string updateInvoice = "";
                    updateVoucher = "update AutoIncrement  set  Number='" + (Convert.ToInt64(VoucherNumber.Text) + 1) + "' where Name ='PurchaseVoucher' and Type='Purchase Voucher'  and CompID = '" + CompID + "' ";
                    // updateInvoice = "update AutoIncrement  set  Number='" + (Convert.ToInt64(invoiceNumber.Text) + 1) + "' where Name ='SaleInvoice' and Type='Sale Invoice'  and CompID = '" + CompID + "' ";
                    SqlCommand myCommandStkUpdateauto = new SqlCommand(updateVoucher, consrauto);
                    myCommandStkUpdateauto.Connection = consrauto;
                    int Numauto = myCommandStkUpdateauto.ExecuteNonQuery();
                    if (Numauto > 0)
                    {
                        MessageBox.Show("Added Successful");

                        PurchaseVoucherGroupEntryQtyCloths sv = new PurchaseVoucherGroupEntryQtyCloths();
                        //SaleVoucherBarcode sv = new SaleVoucherBarcode();
                        this.NavigationService.Navigate(sv);
                    }
                    //SqlCommand myCommandStkUpdateautoInv = new SqlCommand(updateInvoice, consrauto);
                    //myCommandStkUpdateautoInv.Connection = consrauto;
                    //int Numautoinv = myCommandStkUpdateautoInv.ExecuteNonQuery();

                    myCommandStkUpdateauto.Connection.Close();

                    //myCommandStkUpdateautoInv.Connection.Close();

                }
                else
                    MessageBox.Show("Updated Successful");


                // CreateFlowDocumentReadyMadeWholeSale();

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
            else
                MessageBox.Show("Enter Invoice Number");
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

            MoveToBill(VoucherNumber.Text.Trim());
        }

        private void MoveToBill(string invnumbertxt)
        {
            CleanUp();
            isShipping.IsChecked = false;
            autocompltCustName.autoTextBox.Clear();
            CashCustName.Clear();
            EwayNumbertxt.Clear();
            invoiceNumber.Clear();
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

            string sqlother = "select * from PurchaseVouchersOtherDetails where LTRIM(RTRIM(VoucherNumber))='" + invnumbertxt + "' and CompID = '" + CompID + "'";
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
                invoiceNumber.Text = readerother.GetString(5).Trim();
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
                //invoiceNumber.Text = InvoiceNumber.Trim();
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
            string sql = "select GroupName,HSN,BilledQty,BuyPrice,TotalAmount,Discount,TaxablelAmount,GSTRate,GSTTax,Amount,UnitID from PurchaseVoucherInventoryByGroup where LTRIM(RTRIM(VoucherNumber))='" + invnumbertxt + "' and CompID = '" + CompID + "'";
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
                string itemnme = (reader["GroupName"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "";
                dbilledQty = (reader["BilledQty"] != DBNull.Value) ? (reader.GetDouble(2)) : 0;
                //dbilledWts = reader.GetDouble(3);
                //dWastePerc = reader.GetDouble(4);
                //dmakingcharge = reader.GetDouble(6);
                dsaleprice = (reader["BuyPrice"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                ddisperc = (reader["Discount"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;
                dgstrate = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(7)) : 0;
                //we add the product to the Cart
                ShoppingCart.Add(new Product()
                {
                    HSN = (reader["HSN"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "",
                    //BilledWt = dbilledWts,
                    UnitID = (reader["UnitID"] != DBNull.Value) ? (reader.GetString(10).Trim()) : "Pc",
                    ItemName = (reader["GroupName"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "",
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
            AddSundryDebtor asd = new AddSundryDebtor();
            asd.ShowDialog();
            autocompltCustName.autoTextBox.Focus();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            AddItem ai = new AddItem();
            ai.ShowDialog();
        }

        private void infoitem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string itemName = autocompleteItemName.autoTextBoxGroup.Text.Trim();
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
            autocompleteItemName.autoTextBoxGroup.Background = Brushes.White;
            autocompleteItemName.autoTextBoxGroup.Foreground = Brushes.Black;

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
            if (Regex.IsMatch(autocompleteItemName.autoTextBoxGroup.Text.Trim(), @"^\d+$") || 1 == 1)
            {
                //SUM(CAST(DR AS float)) As DebtAmount
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select SUM(CAST(BilledQty AS float)) As Qty ,SUM(CAST(BilledWt AS float)) As Wt ,  (SUM(CAST(Amount AS float)) / SUM(CAST(BilledQty AS float))) As Rate  from PurchaseVoucherInventoryByItemAllocation where GroupName = '" + autocompleteItemName.autoTextBoxGroup.Text + "' and VoucherNumber='" + VoucherNumber.Text.Trim() + "' and InvoiceNumber='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "'";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                tmpProduct = new Product();

                while (reader.Read())
                {
                    tmpProduct.ItemName = autocompleteItemName.autoTextBoxGroup.Text.Trim();
                    txtQty.Text = (reader["Qty"] != DBNull.Value) ? (reader.GetDouble(0).ToString().Trim()) : "";
                    cmbUnits.Text = "Pc";
                    txtPrice.Text = (reader["Rate"] != DBNull.Value) ? (reader.GetDouble(2).ToString().Trim()) : "";
                    //HSN.Text = (reader["HSN"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";
                   // txtGSTRate.Text = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(6)).ToString() : "";
                    //tmpProduct.StorageID = (reader["StorageID"] != DBNull.Value) ? (reader.GetInt32(38)) : 0;

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

        private void PackCharge_LostFocus(object sender, RoutedEventArgs e)
        {
            double packCharge = (PackCharge.Text.Trim() == "") ? 0 : Convert.ToDouble(PackCharge.Text.Trim());
            //double cardreceived = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());
            //double paytmreceived = (receivedPaytm.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedPaytm.Text.Trim());
            //double flatoff = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());

            //double offerzone = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
            //double loyaltycard = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());

            //lbGrandTotal.Content = string.Format("Grand Total: {0}", (Math.Round((totalTaxableValues - oldtotalVal), 0)).ToString("C"));
            lbGrandSum.Content = string.Format("Grand Sum: {0}", (Math.Round((totalVal - oldtotalVal + packCharge), 0)).ToString("C"));
        }

        private void invoiceNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.PageUp)
            {
                if (Convert.ToInt64(VoucherNumber.Text.Trim()) < voucherNumber)
                {
                    Int64 inpageup = (VoucherNumber.Text.Trim() != "") ? (Convert.ToInt64(VoucherNumber.Text.Trim()) + 1) : 0;
                    VoucherNumber.Text = inpageup.ToString();
                    //VoucherNumber.Text = voucherNumber.ToString();
                    MoveToBill(inpageup.ToString());

                }
                if (Convert.ToInt64(VoucherNumber.Text.Trim()) == voucherNumber)
                {
                    autocompltCustName.autoTextBox.Text = "Cash";
                    autocompltCustName.autoTextBox.Focus();
                }
                e.Handled = true;
            }
            if (e.Key == Key.PageDown)
            {

                Int64 inpageup = (VoucherNumber.Text.Trim() != "") ? (Convert.ToInt64(VoucherNumber.Text.Trim()) - 1) : 0;
                VoucherNumber.Text = inpageup.ToString();
                MoveToBill(inpageup.ToString());
                e.Handled = true;
            }

        }

        private void btnItemAllocation_Click(object sender, RoutedEventArgs e)
        {
            //DataRowView row = (DataRowView)ViewSavedBills.SelectedItems[0];
            string invoiceNumberpara = invoiceNumber.Text.Trim();
            string vouchernumberPara = VoucherNumber.Text.Trim();
            string invDatePara = invDate.ToString();
            string GroupnamePara = autocompleteItemName.autoTextBoxGroup.Text.Trim();
            if (invoiceNumberpara != "")
            {
                ReadyMadeItemAllocation viewBillObj = new ReadyMadeItemAllocation(invoiceNumberpara, vouchernumberPara, invDatePara, GroupnamePara);
                viewBillObj.ShowDialog();
            }
            else
                MessageBox.Show("Enter Invvoice Number");
            autocompleteItemName.autoTextBoxGroup.Focus();
            //ReadyMadeItemAllocation sv = new ReadyMadeItemAllocation();
            //sv.ShowDialog();
        }

        private void ViewGroupInventory_OnClick(object sender, RoutedEventArgs e)
        {
            string invoiceNumberpara = invoiceNumber.Text.Trim();
            string vouchernumberPara = VoucherNumber.Text.Trim();
            string invDatePara = invDate.ToString();
            string GroupnamePara = autocompleteItemName.autoTextBoxGroup.Text.Trim();
            if (invoiceNumberpara != "")
            {
                ReadyMadeItemAllocation viewBillObj = new ReadyMadeItemAllocation(invoiceNumberpara, vouchernumberPara, invDatePara, GroupnamePara);
                viewBillObj.ShowDialog();
            }
            else
                MessageBox.Show("Enter Invvoice Number");
            autocompleteItemName.autoTextBoxGroup.Focus();
        }

    }
}
