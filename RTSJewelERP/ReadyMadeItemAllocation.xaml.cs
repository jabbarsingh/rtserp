using iTextSharp.text;
using iTextSharp.text.pdf;
using RTSJewelERP.StockItemsListTableAdapters;
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
    public partial class ReadyMadeItemAllocation : Window
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
        private Double totalPriceSum = 0.0;



        //Array of Cart items 
        private List<Product> ShoppingCart;
        private List<Product> OldCart;
        public ReadyMadeItemAllocation()
        {
        }
        public ReadyMadeItemAllocation(string invoiceNumberpara, string vouchernumberpara, string invdatepara, string groupNamePara)
        {

        //public ReadyMadeItemAllocation()
        //{
            InitializeComponent();


            //autocompleteItemName.autoTextBoxGroup.Text = 
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window
            BindComboBoxUnits(cmbUnits);
            //dueBal.Content = string.Format("Balance: {0}", (BalanceCRorDR).ToString("C"));
            invoiceNumber.Text = invoiceNumberpara;
            VoucherNumber.Text = vouchernumberpara;
            autocompleteItemName.autoTextBoxGroup.Text = groupNamePara;
            autocompleteItemName.autoTextBoxGroup.Focus();
            //on the constructor of the class we create a new instance of the shooping cart
            ShoppingCart = new List<Product>();
            OldCart = new List<Product>();
            //autocompleteItemName.autoTextBoxGroup.Focus();
            //Pattern.Focus();
            MoveToBill(vouchernumberpara);
            //txtBarCode.Focus();


            //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            ////SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            //con.Open();
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

            //string sqlvoucher = "select number from AutoIncrement where Name = 'SaleVoucher' and CompID = '" + CompID + "'";
            //SqlCommand cmdvoucher = new SqlCommand(sqlvoucher);
            //cmdvoucher.Connection = con;
            //SqlDataReader readerVoucher = cmdvoucher.ExecuteReader();

            ////tmpProduct = new Product();

            //while (readerVoucher.Read())
            //{
            //    voucherNumber = readerVoucher.GetInt64(0);
            //    VoucherNumber.Text = voucherNumber.ToString();
            //}
            //readerVoucher.Close();

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
            //    if (Convert.ToInt64(invoiceNumber.Text.Trim()) < InvoiceNumber)
            //    {
            //        Int64 inpageup = (invoiceNumber.Text.Trim() != "") ? (Convert.ToInt64(invoiceNumber.Text.Trim()) + 1) : 0;
            //        invoiceNumber.Text = inpageup.ToString();
            //        VoucherNumber.Text = voucherNumber.ToString();
            //        MoveToBill(inpageup.ToString());

            //    }
            //    if (Convert.ToInt64(invoiceNumber.Text.Trim()) == InvoiceNumber)
            //    {
            //        autocompltCustName.autoTextBox.Text = "Cash";
            //        autocompltCustName.autoTextBox.Focus();
            //    }
            //    e.Handled = true;
            //}
            //if (e.Key == Key.PageDown)
            //{
            //    if (Convert.ToInt64(invoiceNumber.Text.Trim()) > 1)
            //    {
            //        Int64 inpageup = (invoiceNumber.Text.Trim() != "") ? (Convert.ToInt64(invoiceNumber.Text.Trim()) - 1) : 0;
            //        invoiceNumber.Text = inpageup.ToString();
            //        MoveToBill(inpageup.ToString());
            //        e.Handled = true;
            //    }
            //}
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


        void CartGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()).ToString();
            //CartGrid.Items.Refresh();
        }
        private int i = 1;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //if (autocompltCustName.autoTextBox.Text == "Card")
            //{
            //    receivedCash.Clear();
            //    receivedCard.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            //}
            //if (autocompltCustName.autoTextBox.Text == "Cash")
            //{
            //    receivedCard.Clear();
            //    receivedCash.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            //}

            if (autocompleteItemName.autoTextBoxGroup.Text.Trim() != "")
            {

                //product quantity
                double qty;
                //double wtqty;

                // we try to parse the number of the textbox if the number is invalid 
                double.TryParse(txtQty.Text, out qty);
                //double.TryParse(txtWeight.Text, out wtqty);
                //if qty is 0 we assign 0 otherwise we assign the actual parsed value
                qty = qty == 0 ? 1 : qty;
                //really basic validation that checks inventory
                if (qty <= tmpProduct.ActualQty)
                {

                    //we check if product is not already in the cart if it is we remove the old one
                    var isexistItem = ShoppingCart.Where(s => s.DesignNumberPattern == tmpProduct.DesignNumberPattern);
                    if (isexistItem.Count() == 1)
                    {

                    }
                    //ShoppingCart.RemoveAll(s => s.ItemName == tmpProduct.ItemName); // Remove Existing item if same name
                    //ShoppingCart.RemoveAll(s => s.ItemName == tmpProduct.ItemName); // Remove Existing item if same barcode
                    //we add the product to the Cart
                    ShoppingCart.Add(new Product()
                    {
                        //Sr = i,
                        DesignNumberPattern = tmpProduct.DesignNumberPattern,
                        Size = tmpProduct.Size,
                        Color = tmpProduct.Color,
                        //Mediium = tmpProduct.Mediium,
                        //Large = tmpProduct.Large,
                        //XL = tmpProduct.XL,
                        //XL2 = tmpProduct.XL2,
                        //XL3 = tmpProduct.XL3,
                        //XL4 = tmpProduct.XL4,
                        //XL5 = tmpProduct.XL5,
                        //XL6 = tmpProduct.XL6,

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
                        UnderGroupName = tmpProduct.UnderGroupName
                        //GSTRate = (txtGSTRate.Text == "") ? 0 : Convert.ToInt16(txtGSTRate.Text)
                    });

                    //perform  query on Shopping Cart to select certain fields and perform subtotal operation 
                    BindDataGrid();
                    i++;
                    //<----------------------
                    //cleanup variables
                    tmpProduct = null;
                    //once the products had been added we clear the textbox of code and quantity.
                    //autocompleteItemName.autoTextBoxGroup.Text = string.Empty;
                    //autocompleteItemName.autoTextBoxGroup.Text = string.Empty;
                    txtQty.Text = "1";
                    txtDiscPerc.Text = string.Empty;
                    //txtGSTRate.Text = string.Empty;
                    //txtMC.Text = string.Empty;
                    //txtWeight.Text = string.Empty;
                    //txtWaste.Text = string.Empty;
                    txtPrice.Text = string.Empty;
                    autocompleteItemName.autoTextBoxGroup.Focus();
                    //Pattern.Clear();
                    //Size.Clear();
                    //Color.Clear();
                    //L.Clear();
                    //XL.Clear();
                    //XL2.Clear();
                    //XL3.Clear();
                    //XL4.Clear();
                    //XL5.Clear();
                    //XL6.Clear();




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
                        DesignNumberPattern = DesignPattern.autoTextBoxDesignPattern.Text.Trim(),
                        Size = (Size.Text.Trim() != "") ? (Size.Text.Trim()) : "",
                        Color = (Color.Text.Trim() != "") ? (Color.Text.Trim()) : "",
                        //Large = (L.Text.Trim() != "") ? Convert.ToDouble(L.Text.Trim()) : 0,
                        //XL = (XL.Text.Trim() != "") ? Convert.ToDouble(XL.Text.Trim()) : 0,
                        //XL2 = (XL2.Text.Trim() != "") ? Convert.ToDouble(XL2.Text.Trim()) : 0,
                        //XL3 = (XL3.Text.Trim() != "") ? Convert.ToDouble(XL3.Text.Trim()) : 0,
                        //XL4 = (XL4.Text.Trim() != "") ? Convert.ToDouble(XL4.Text.Trim()) : 0,
                        //XL5 = (XL5.Text.Trim() != "") ? Convert.ToDouble(XL5.Text.Trim()) : 0,
                        //XL6 = (XL6.Text.Trim() != "") ? Convert.ToDouble(XL6.Text.Trim()) : 0,
                        UnitID = (cmbUnits.Text != "") ? cmbUnits.Text : "Pc",
                        //HSN = HSN.Text,
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
                        UnderGroupName = autocompleteItemName.autoTextBoxGroup.Text
                        //GSTRate = (txtGSTRate.Text == "") ? 0 : Convert.ToInt16(txtGSTRate.Text)
                    });

                    //perform  query on Shopping Cart to select certain fields and perform subtotal operation 
                    BindDataGrid();
                    i++;
                    //<----------------------
                    //cleanup variables
                    tmpProduct = null;
                    //once the products had been added we clear the textbox of code and quantity.
                    //autocompleteItemName.autoTextBoxGroup.Text = string.Empty;
                    //autocompleteItemName.autoTextBoxGroup.Text = string.Empty;
                    txtQty.Text = "1";
                    txtDiscPerc.Text = string.Empty;
                    //txtGSTRate.Text = string.Empty;
                    //txtMC.Text = string.Empty;
                    //txtWeight.Text = string.Empty;
                    //txtWaste.Text = string.Empty;
                    txtPrice.Text = string.Empty;
                    autocompleteItemName.autoTextBoxGroup.Focus();

                    //DesignPattern.autoTextBoxDesignPattern.Clear();
                    //Size.Clear();
                    //Color.Clear();
                    //L.Clear();
                    //XL.Clear();
                    //XL2.Clear();
                    //XL3.Clear();
                    //XL4.Clear();
                    //XL5.Clear();
                    //XL6.Clear();

                    //---------------Write Code Below to Add Item in StockItems-Cloths Dynamically with minimum data, if some data not provided then send the item to Pending tasks

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
                                //Product = s.ItemName,
                                DesignNumberPattern = s.DesignNumberPattern,
                                
                                HSN = s.HSN,
                                Size = s.Size,
                                Color = s.Color,
                                //Large = s.Large,
                                //XL = s.XL,
                                //XL2 = s.XL2,
                                //XL3 = s.XL3,
                                //XL4 = s.XL4,
                                //XL5 = s.XL5,
                                //XL6 = s.XL6,
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
                                Total = Math.Round((((s.BilledQty * s.ItemPrice)) - (((s.BilledQty * s.ItemPrice)) * s.SaleDiscountPerc / 100)) + ((((s.BilledQty * s.ItemPrice)) - (((s.BilledQty * s.ItemPrice)) * s.SaleDiscountPerc / 100)) * s.GSTRate / 100), 2),
                                UnderGroupName = s.UnderGroupName
                                

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
            totalPriceSum = cartItems.Sum(x => x.Price); 
            totalTaxableValues = cartItems.Sum(x => x.TaxableAmount);
            discounttotalByItem = cartItems.Sum(x => (x.Disc * x.Amount / 100));
            //makingTotalCharge = cartItems.Sum(x => x.MC);



            //discounttotalval = cartItems.Sum(x => x.Disc);
            lbTotalTax.Content = string.Format("Tax: {0}", cartItems.Sum(x => x.Tax).ToString("C"));
            lblTotalQtyItem.Content = string.Format("Total Qty: {0}", (totalQuanty).ToString("C"));
            lblTotalDiscByItem.Content = string.Format("Discount: {0}", (discounttotalByItem).ToString("C"));
            lblAverageRate.Content = string.Format("Avg. Rate: {0}", (totalBeforeItemDiscount / totalQuanty).ToString("C"));
            //if (autocompltCustName.autoTextBox.Text == "Card")
            //{
            //    receivedCash.Clear();
            //    receivedCard.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            //}
            //if (autocompltCustName.autoTextBox.Text == "Cash")
            //{
            //    receivedCard.Clear();
            //    receivedCash.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            //}

            //double cashreceived = (receivedCash.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCash.Text.Trim());
            //double cardreceived = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());
            //double paytmreceived = (receivedPaytm.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedPaytm.Text.Trim());
            //double flatoff = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());

            //double offerzone = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
            //double loyaltycard = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());

            //dueBal.Content = string.Format("Balance:  {0}", Math.Round((totalVal - oldtotalVal - (cashreceived + cardreceived + paytmreceived + flatoff + offerzone + loyaltycard)), 0)).ToString();


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
            //VoucherNumber.Text = voucherNumber.ToString();
        }

        //this method will clear/reset form values
        private void CleanUp()
        {
            //autocompltCustName.autoTextBox.Clear();
            //CashCustName.Clear();
            //EwayNumbertxt.Clear();
            ////VoucherNumber.Clear();
            //invDate.SelectedDate = DateTime.Now;
            //receivedCash.Clear();
            //receivedCard.Clear();
            //flatOff.Clear();
            //receivedOffer.Clear();
            //receivedLoyalty.Clear();
            //receivedPaytm.Clear();

            ////shopping cart = a new empty list
            //ShoppingCart = new List<Product>();
            //OldCart = new List<Product>();
            ////Textboxes and labels are set to defaults
            //autocompleteItemName.autoTextBoxGroup.Text = string.Empty;
            //autocompleteItemName.autoTextBoxGroup.Text = string.Empty;
            //txtQty.Text = string.Empty;
            //lbTotal.Content = "Total: ₹ 0.00";
            ////lbOldTotal.Content = "Total: ₹ 0.00";
            //lbGrandTotal.Content = "Total: ₹ 0.00";
            ////lbGrandTotalSum.Content = "Total: ₹ 0.00";
            ////DataGrid items are set to null
            //CartGrid.ItemsSource = null;
            ////OldGoldGrid.ItemsSource = null;
            //CartGrid.Items.Refresh();
            ////Tmp variable is erased using null
            //tmpProduct = null;

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



        private void textBoxItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (autocompltCustName.autoTextBox.Text == "Card")
            //{
            //    receivedCash.Clear();
            //    receivedCard.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            //}
            //if (autocompltCustName.autoTextBox.Text == "Cash")
            //{
            //    receivedCard.Clear();
            //    receivedCash.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            //}

            //if (autocompltCustName.autoTextBox.Text != "Cash")
            //{
            //    CashCustName.Visibility = Visibility.Collapsed;
            //    //CashName.Visibility = Visibility.Collapsed;

            //}

            ////invoiceNumber.Text = InvoiceNumber.ToString();
            ////VoucherNumber.Text = voucherNumber.ToString();
            ////If a product code is not empty we search the database
            //if (Regex.IsMatch(autocompleteItemName.autoTextBoxGroup.Text.Trim(), @"^\d+$") || 1 == 1)
            //{
            //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            //    con.Open();
            //    string sql = "select * from StockItemsCloths where ItemName = '" + autocompleteItemName.autoTextBoxGroup.Text + "' and CompID = '" + CompID + "'";
            //    SqlCommand cmd = new SqlCommand(sql);
            //    cmd.Connection = con;
            //    SqlDataReader reader = cmd.ExecuteReader();

            //    tmpProduct = new Product();

            //    while (reader.Read())
            //    {


            //        //var CustID = reader.GetValue(0).ToString();

            //        tmpProduct.ItemName = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
            //        tmpProduct.PrintName = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
            //        tmpProduct.UnitID = (reader["UnitID"] != DBNull.Value) ? (reader.GetString(4)) : "Pc";
            //        tmpProduct.ItemCode = (reader["ItemCode"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";

            //        tmpProduct.HSN = "9503";  //HSN

            //        tmpProduct.ItemDesc = (reader["ItemDesc"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
            //        tmpProduct.ItemBarCode = (reader["ItemBarCode"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
            //        tmpProduct.ItemPrice = (reader["ItemPrice"] != DBNull.Value) ? (reader.GetDouble(9)) : 0;
            //        tmpProduct.SetCriticalLevel = (reader["SetCriticalLevel"] != DBNull.Value) ? (reader.GetBoolean(12)) : false;
            //        tmpProduct.SetDefaultStorageID = (reader["SetDefaultStorageID"] != DBNull.Value) ? (reader.GetInt32(14)) : 0;
            //        tmpProduct.DecimalPlaces = (reader["DecimalPlaces"] != DBNull.Value) ? (reader.GetInt32(17)) : 0;
            //        tmpProduct.IsBarcodeCreated = (reader["IsBarcodeCreated"] != DBNull.Value) ? (reader.GetBoolean(18)) : false;
            //        tmpProduct.ItemPurchPrice = (reader["ItemPurchPrice"] != DBNull.Value) ? (reader.GetDouble(23)) : 0;
            //        tmpProduct.ItemAlias = (reader["ItemAlias"] != DBNull.Value) ? (reader.GetString(30).Trim()) : "";
            //        tmpProduct.UnderGroupID = (reader["UnderGroupID"] != DBNull.Value) ? (reader.GetInt64(32)) : 0;
            //        tmpProduct.UnderSubGroupID = (reader["UnderSubGroupID"] != DBNull.Value) ? (reader.GetInt64(34)) : 0;
            //        tmpProduct.ActualQty = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)) : 0;
            //        tmpProduct.HSN = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
            //        tmpProduct.GSTRate = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)) : 0;
            //        tmpProduct.StorageID = (reader["StorageID"] != DBNull.Value) ? (reader.GetInt32(38)) : 0;
            //        tmpProduct.TrayID = (reader["TrayID"] != DBNull.Value) ? (reader.GetInt32(39)) : 0;
            //        tmpProduct.CounterID = (reader["CounterID"] != DBNull.Value) ? (reader.GetInt32(40)) : 0;
            //        //tmpProduct.UpdateDate = reader.GetDateTime(44); //reader["UpdateDate"] != DBNull.Value) ? (reader.GetDateTime(44)) : "";  
            //        tmpProduct.ActualWt = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)) : 0;
            //        //tmpProduct.LastBuyDate = reader.GetDateTime(47); //(reader["LastBuyDate"] != DBNull.Value) ? (reader.GetDateTime(47) : "";
            //        //tmpProduct.LastSaleDate = reader.GetDateTime(48);//(reader["LastSaleDate"] != DBNull.Value) ? (reader.GetDateTime(48) : "";
            //        tmpProduct.LastSalePrice = (reader["LastSalePrice"] != DBNull.Value) ? (reader.GetDouble(50)) : 0;
            //        tmpProduct.LastBuyPrice = (reader["LastBuyPrice"] != DBNull.Value) ? (reader.GetDouble(51)) : 0;

            //        HSN.Text = tmpProduct.HSN.ToString();
            //        txtPrice.Text = tmpProduct.ItemPrice.ToString();
            //        txtGSTRate.Text = tmpProduct.GSTRate.ToString();
            //        autocompleteItemName.autoTextBoxGroup.Text = tmpProduct.ItemBarCode.ToString();
            //        //Get Counter , Tray and Storage Name by another call, get all count by sp or direct call for inventory 
            //        //cmbUnits.Text = tmpProduct.UnitID.ToString();
            //        cmbUnits.Text = (tmpProduct.UnitID.ToString() != "") ? tmpProduct.UnitID.ToString() : "Pc";
            //        BindStorageComboBox(tmpProduct.ItemName);
            //    }

            //    reader.Close();
            //}
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


        private void PrintInvBtn_Click(object sender, RoutedEventArgs e)
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


                    //Do same for StockitemsByPc 
                    SqlConnection myConnSVEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                    myConnSVEntryStr.Open();
                    string CountSVEntryStr = "SELECT COUNT(*) From PurchaseVoucherInventoryByItemAllocation where VoucherNumber= '" + VoucherNumber.Text.Trim() + "' and InvoiceNumber='" + invoiceNumber.Text.Trim() + "' and CompID = '" + CompID + "'";
                    // string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                    SqlCommand myCommandDel = new SqlCommand(CountSVEntryStr, myConnSVEntryStr);
                    myCommandDel.Connection = myConnSVEntryStr;

                    //int countRec = myCommand.ExecuteNonQuery();
                    int countRecDelDel = (int)myCommandDel.ExecuteScalar();
                    myCommandDel.Connection.Close();
                    if (countRecDelDel != 0)
                    {
                        // MessageBox.Show("Item Name is already Exist, Please delete existing", "Add Record");


                        SqlCommand myCommandDeleteDel = new SqlCommand("SPUpdateStockOnPurchaseVoucherItemAllocationChangeOrDelete", myConnSVEntryStr);
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

                            DataGridCell cellSize= (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(4);
                            TextBlock sizeText = cellSize.Content as TextBlock;

                            DataGridCell cellColor = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(5);
                            TextBlock colorText = cellColor.Content as TextBlock;


                            // for Qty

                            DataGridCell cellQty = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(6);
                            TextBlock qtyText = cellQty.Content as TextBlock;

                            DataGridCell cellUnitID = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(7);
                            TextBlock txtcellUnitID = cellUnitID.Content as TextBlock;

                            DataGridCell cellPrice = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(8);
                            TextBlock priceText = cellPrice.Content as TextBlock;


                            DataGridCell cellAmount = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(9);
                            TextBlock txtCellAmount = cellAmount.Content as TextBlock;


                            DataGridCell discRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(10);
                            TextBlock txtdiscRate = discRate.Content as TextBlock;

                            DataGridCell cellTaxableAmt = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(11);
                            TextBlock txtTaxableAmt = cellTaxableAmt.Content as TextBlock;

                            DataGridCell gstRate = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(12);
                            TextBlock txtgstRate = gstRate.Content as TextBlock;

                            DataGridCell gstTax = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(13);
                            TextBlock txtgsTax = gstTax.Content as TextBlock;


                            DataGridCell cellTotal = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(14);
                            TextBlock totalText = cellTotal.Content as TextBlock;

                            DataGridCell cellGroupNamee = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(15);
                            //TextBlock txtItemNam = cellItemName.Content as TextBlock;
                            TextBlock txtgroupNamee = cellGroupNamee.Content as TextBlock;

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
                            querySalesInventory = "insert into PurchaseVoucherInventoryByItemAllocation(VoucherNumber, InvoiceNumber,GroupName,DesignNumberPattern,HSN,Size, Color,BuyPrice,GSTRate,GSTTax,Discount,TaxablelAmount,TotalAmount, BilledQty,UnitID,TransactionDate,FromConsumedStorageID,FromConsumedTrayID,FromConsumedCounterID,CompID,Amount) Values ( '" + VoucherNumber.Text + "','" + invoiceNumber.Text.Trim() + "','" + txtgroupNamee.Text + "','" + txtItemNam.Text + "','" + hsnText.Text + "','" + sizeText.Text + "','" + colorText.Text + "','" + priceText.Text + "','" + txtgstRate.Text + "','" + txtgsTax.Text + "','" + txtdiscRate.Text + "', '" + txtTaxableAmt.Text + "','" + totalText.Text + "','" + qtyText.Text + "', '" + txtcellUnitID.Text + "','" + InvdateValue + "','1','1','1', '" + CompID + "','" + txtCellAmount.Text + "')";



                            SqlCommand myCommandSVInventory = new SqlCommand(querySalesInventory, myConSVInventoryStr);
                            myCommandSVInventory.Connection = myConSVInventoryStr;
                            //myCommandInvEntry.Connection.Open();
                            int NumPI = myCommandSVInventory.ExecuteNonQuery();
                            myCommandSVInventory.Connection.Close();


                            ////StockItems: CRUD Start
                            //if ((txtItemNam != null) && (priceText != null))
                            //{
                            //    //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                            //    SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                            //    myConnSalesInvEntryStr.Open();
                            //    string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPC where DesignNumberPattern ='" + txtItemNam.Text.Trim() + "'";
                            //    //string CountSalesInvEntryStr = "SELECT COUNT(*) From StockItems where ItemName ='" + autocompleteItemName.autoTextBox1.Text + "' and  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                            //    //// string CountSalesInvEntryStr = "SELECT COUNT(*) From PurchaseInventory where  GSTIN ='" + GSTCust.Text + "' and  InvoiceNumber='" + invoiceNumber.Text.Trim() + "'";
                            //    SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
                            //    myCommand.Connection = myConnSalesInvEntryStr;

                            //    //int countRec = myCommand.ExecuteNonQuery();
                            //    int countRec = (int)myCommand.ExecuteScalar();
                            //    myCommand.Connection.Close();


                            //    if (countRec != 0)
                            //    {

                            //        string queryStrStockCheck = "";

                            //        string balanceStk = "";
                            //        string balanceStkWt = "";

                            //        // write code to update stocktable directly 
                            //        queryStrStockCheck = "select * from StockItemsByPC where DesignNumberPattern = '" + txtItemNam.Text.Trim() + "' and CompID = '" + CompID + "'";
                            //        //OleDbCommand command = new OleDbCommand(queryStr, con);
                            //        // myConnStock.Open();
                            //        SqlCommand myCommandStkCheck = new SqlCommand(queryStrStockCheck, myConnSalesInvEntryStr);
                            //        myCommandStkCheck.Connection.Open();
                            //        SqlDataReader reader = myCommandStkCheck.ExecuteReader();



                            //        while (reader.Read())
                            //        {
                            //            // var CustID = reader.GetValue(0).ToString();
                            //            string ItemName = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                            //            string PrintName = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                            //            double invQty = (qtyText.Text != "") ? (Convert.ToDouble(qtyText.Text)) : 0;
                            //            double actualQty = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)) : 0;
                            //            //double invWt = (qtyWt.Text != "") ? (Convert.ToDouble(qtyWt.Text)) : 0;
                            //            double actualWt = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)) : 0;
                            //            //if (ItemName == "Old Gold" || ItemName == "Old Silver")
                            //            //{
                            //            //    balanceStk = Math.Round((actualQty + invQty), 2).ToString();
                            //            //    balanceStkWt = Math.Round((actualWt + invWt), 2).ToString();
                            //            //}
                            //            //else
                            //            //{
                            //            balanceStk = Math.Round((actualQty + invQty), 2).ToString();
                            //            //balanceStkWt = Math.Round((actualWt - invWt), 2).ToString();
                            //            //}

                            //        }
                            //        reader.Close();
                            //        myCommandStkCheck.Connection.Close();

                            //        string queryStrStockUpdate = "";
                            //        queryStrStockUpdate = "update StockItemsByPC  set UpdateDate='" + InvdateValue + "', ActualQty='" + balanceStk + "',ActualWt='" + balanceStkWt + "',LastSalePrice='" + priceText.Text + "'  where DesignNumberPattern ='" + txtItemNam.Text + "'   and CompID = '" + CompID + "' ";
                            //        if (txtItemNam.Text == "Old Gold" || txtItemNam.Text == "Old Silver")
                            //        {
                            //            queryStrStockUpdate = "update StockItemsByPC  set UpdateDate='" + InvdateValue + "' , ActualQty='" + balanceStk + "',ActualWt='" + balanceStkWt + "',LastBuyPrice='" + priceText.Text + "'  where DesignNumberPattern ='" + txtItemNam.Text + "'   and CompID = '" + CompID + "' ";
                            //        }
                            //        SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                            //        myCommandStkUpdate.Connection.Open();
                            //        myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                            //        if (txtItemNam.Text.Trim() != "")
                            //        {
                            //            // myCommandStk.Connection.Open();
                            //            int Num = myCommandStkUpdate.ExecuteNonQuery();
                            //            if (Num != 0)
                            //            {
                            //                // MessageBox.Show("Record Successfully Updated....", "Update Record");
                            //            }
                            //            else
                            //            {
                            //                MessageBox.Show("Stock is not Updated....", "Update Record Error");
                            //            }
                            //            // myCommandStk.Connection.Close();
                            //        }
                            //        else
                            //        {
                            //            MessageBox.Show("Stock can not be updated....", "Update Record Error");
                            //        }
                            //        myCommandStkUpdate.Connection.Close();
                            //    }
                            //    else
                            //    {
                            //        //double qtyStkEntry = (txtQtyStockEntry.Text.Trim() == "") ? 0 : Convert.ToDouble(txtQtyStockEntry.Text.Trim());
                            //        //double qtyEntryInsertOpen = (txtQtyStockEntry.Text.Trim() == "") ? 0 : Convert.ToDouble(txtQtyStockEntry.Text.Trim());
                            //        //double qtyEntryInsertBill = (txtQty.Text.Trim() == "") ? 0 : Convert.ToDouble(txtQty.Text.Trim());
                            //        //string hsnentryinsert = HSN.Text.Trim();
                            //        string querySalesInvEntry = "";
                            //        querySalesInvEntry = "insert into StockItemsByPC(ItemName,DesignNumberPattern, ActualQty,UnitID,ActualWt,ItemPrice,GSTRate,LastSalePrice,HSN,Size, Color,CompID) Values ('" + autocompleteItemName.autoTextBoxGroup.Text + "', '" + txtItemNam.Text + "','0','" + txtcellUnitID.Text + "','" + 0 + "','" + priceText.Text + "','" + txtgstRate.Text + "','" + priceText.Text + "','" + hsnText.Text + "','" + sizeText.Text + "','" + colorText.Text + "', '" + CompID + "')";
                            //        //if (txtItemNam.Text == "Old Gold" || txtItemNam.Text == "Old Silver")
                            //        //{
                            //        //    querySalesInvEntry = "insert into StockItems(ItemName, ActualQty,ActualWt,ItemPrice,GSTRate,LastBuyPrice,CompID) Values ( '" + txtItemNam.Text + "','" + 0 + "','" + 0 + "','" + priceText.Text + "','" + txtgstRate.Text + "','" + priceText.Text + "', '" + CompID + "')";
                            //        //}

                            //        SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                            //        myCommandInvEntry.Connection.Open();
                            //        int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                            //        if (NumPInv != 0)
                            //        {
                            //            // MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                            //        }
                            //        else
                            //        {
                            //            MessageBox.Show("Stock is not Inserted....", "Insert Record Error");
                            //        }
                            //        myCommandInvEntry.Connection.Close();

                            //        // myConnStock.Close();

                            //    }


                            //}


                        }
                    }




                    this.Close();

                    //StockItems End
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

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

        //private void resultStack_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    txtQty.Focus();
        //}

        private void txtDueBal_LostFocus(object sender, RoutedEventArgs e)
        {
            ////double roundoffamt = (txtRoundOff.Text.Trim() == "") ? 0 : Convert.ToDouble(txtRoundOff.Text.Trim());
            //double cashreceived = (receivedCash.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCash.Text.Trim());
            //double cardreceived = (receivedCard.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedCard.Text.Trim());
            //double paytmreceived = (receivedPaytm.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedPaytm.Text.Trim());
            //double flatoff = (flatOff.Text.Trim() == "") ? 0 : Convert.ToDouble(flatOff.Text.Trim());

            //double offerzone = (receivedOffer.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedOffer.Text.Trim());
            //double loyaltycard = (receivedLoyalty.Text.Trim() == "") ? 0 : Convert.ToDouble(receivedLoyalty.Text.Trim());

            //dueBal.Content = string.Format("Balance:  {0}", Math.Round((totalVal - oldtotalVal - (cashreceived + cardreceived + paytmreceived + flatoff + offerzone + loyaltycard)), 0)).ToString();
        }

     

        private void MoveToBill(string invnumbertxt)
        {
            CleanUp();
            //isShipping.IsChecked = false;
            //autocompltCustName.autoTextBox.Clear();
            //CashCustName.Clear();
            //EwayNumbertxt.Clear();
            //VoucherNumber.Clear();
            invDate.SelectedDate = DateTime.Now;
            //receivedCash.Clear();
            //receivedCard.Clear();
            //flatOff.Clear();
            //receivedOffer.Clear();
            //receivedLoyalty.Clear();
            //receivedPaytm.Clear();

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


                //autocompltCustName.autoTextBox.Text = AccountName;
                //CashCustName.Text = CashCustomerName;
                //EwayNumbertxt.Text = EwayNumber;
                VoucherNumber.Text = dVoucherNumber.ToString();
                invDate.Text = TransactionDate;
                //receivedCash.Text = CashPaid.ToString();
                //receivedCard.Text = CardPaid.ToString();
                //flatOff.Text = FlatOff.ToString();
                //receivedOffer.Text = Offer.ToString();
                //receivedLoyalty.Text = LoyaltyAmt.ToString();
                //receivedPaytm.Text = PaytmOther.ToString();

                //dueBal.Content = string.Format("Balance: {0}", (DueBalance).ToString("C"));

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
            string sql = "select GroupName,DesignNumberPattern, Size,Color,HSN,BilledQty,BuyPrice,TotalAmount,Discount,TaxablelAmount,GSTRate,GSTTax,Amount,UnitID,TransactionDate from PurchaseVoucherInventoryByItemAllocation where LTRIM(RTRIM(VoucherNumber))='" + invnumbertxt + "'and CompID = '" + CompID + "'";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = conn;
            SqlDataReader reader = cmd.ExecuteReader();

            double dbilledQty = 0;
            //double dbilledQtySmall = 0;
            //double dbilledQtyMedium = 0;
            //double dbilledQtyLarge = 0;
            //double dbilledQtyXL = 0;
            //double dbilledQtyXL2 = 0;
            //double dbilledQtyXL3 = 0;
            //double dbilledQtyXL4 = 0;
            //double dbilledQtyXL5 = 0;
            //double dbilledQtyXL6 = 0;
            //double dbilledWts = 0;
            //double dWastePerc = 0;
            //double dmakingcharge = 0;
            double dsaleprice = 0;
            double ddisperc = 0;
            int dgstrate = 0;

            while (reader.Read())
            {
                string itemnme = (reader["GroupName"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "";
                //dbilledQtySmall = (reader["Small"] != DBNull.Value) ? (reader.GetDouble(3)) : 0;
                //dbilledQtyMedium = (reader["Mediium"] != DBNull.Value) ? (reader.GetDouble(4)) : 0;
                //dbilledQtyLarge = (reader["Large"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;
                //dbilledQtyXL = (reader["XL"] != DBNull.Value) ? (reader.GetDouble(6)) : 0;
                //dbilledQtyXL2 = (reader["XL2"] != DBNull.Value) ? (reader.GetDouble(7)) : 0;
                //dbilledQtyXL3 = (reader["XL3"] != DBNull.Value) ? (reader.GetDouble(8)) : 0;
                //dbilledQtyXL4 = (reader["XL4"] != DBNull.Value) ? (reader.GetDouble(9)) : 0;
                //dbilledQtyXL5 = (reader["XL5"] != DBNull.Value) ? (reader.GetDouble(10)) : 0;
                //dbilledQtyXL6 = (reader["XL6"] != DBNull.Value) ? (reader.GetDouble(11)) : 0;

                dbilledQty = (reader["BilledQty"] != DBNull.Value) ? (reader.GetDouble(5)) : 0;
                //dbilledWts = reader.GetDouble(3);
                //dWastePerc = reader.GetDouble(4);
                //dmakingcharge = reader.GetDouble(6);
                dsaleprice = (reader["BuyPrice"] != DBNull.Value) ? (reader.GetDouble(6)) : 0;
                ddisperc = (reader["Discount"] != DBNull.Value) ? (reader.GetDouble(8)) : 0;
                dgstrate = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(10)) : 0;
                //we add the product to the Cart
                ShoppingCart.Add(new Product()
                {
                    DesignNumberPattern = (reader["DesignNumberPattern"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "",
                    Size = (reader["Size"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "",
                    Color = (reader["Color"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "",
                    //Large = dbilledQtyLarge,
                    //XL = dbilledQtyXL,
                    //XL2 = dbilledQtyXL2,
                    //XL3 = dbilledQtyXL3,
                    //XL4 = dbilledQtyXL4,
                    //XL5 = dbilledQtyXL5,
                    //XL6 = dbilledQtyXL6,


                    HSN = (reader["HSN"] != DBNull.Value) ? (reader.GetString(4).Trim()) : "",
                    //BilledWt = dbilledWts,
                    UnitID = (reader["UnitID"] != DBNull.Value) ? (reader.GetString(13).Trim()) : "Pc",
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

            //autocompltCustName.autoTextBox.Focus();

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
            //AddSundryDebtor asd = new AddSundryDebtor();
            //asd.ShowDialog();
            //autocompltCustName.autoTextBox.Focus();
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
            //txtGSTRate.Background = Brushes.White;
            //txtGSTRate.Foreground = Brushes.Black;
            AddItemRow.Focus();
        }

        private void autocompleteGroupName_LostFocus(object sender, RoutedEventArgs e)
        {
            autocompleteItemName.autoTextBoxGroup.Background = Brushes.White;
            autocompleteItemName.autoTextBoxGroup.Foreground = Brushes.Black;

       
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select * from StockItemsByPc where DesignNumberPattern = '" + DesignPattern.autoTextBoxDesignPattern.Text.Trim() + "' and CompID = '" + CompID + "'";
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


                tmpProduct.DesignNumberPattern = (reader["DesignNumberPattern"] != DBNull.Value) ? (reader.GetString(85)) : "";
                tmpProduct.Size = (reader["Size"] != DBNull.Value) ? (reader.GetString(83)) : "";
                tmpProduct.Color = (reader["Color"] != DBNull.Value) ? (reader.GetString(84)) : "";
                Size.Text = (reader["Size"] != DBNull.Value) ? (reader.GetString(83)) : "";
                Color.Text = (reader["Color"] != DBNull.Value) ? (reader.GetString(84)) : "";

            }
            //}
            reader.Close();

        }

        private void autocompleteItemName_LostFocus(object sender, RoutedEventArgs e)
        {
            autocompleteItemName.autoTextBoxGroup.Background = Brushes.White;
            autocompleteItemName.autoTextBoxGroup.Foreground = Brushes.Black;

            //if (autocompltCustName.autoTextBox.Text == "Card")
            //{
            //    receivedCash.Clear();
            //    receivedCard.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            //}
            //if (autocompltCustName.autoTextBox.Text == "Cash")
            //{
            //    receivedCard.Clear();
            //    receivedCash.Text = Math.Round((totalVal - oldtotalVal), 0).ToString();
            //}

            //if (autocompltCustName.autoTextBox.Text != "Cash")
            //{
            //    CashCustName.Visibility = Visibility.Collapsed;
            //    //CashName.Visibility = Visibility.Collapsed;

            //}

            ////invoiceNumber.Text = InvoiceNumber.ToString();
            ////VoucherNumber.Text = voucherNumber.ToString();
            ////If a product code is not empty we search the database
            //if (Regex.IsMatch(autocompleteItemName.autoTextBoxGroup.Text.Trim(), @"^\d+$") || 1 == 1)
            //{



                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select * from StockItemsByPc where DesignNumberPattern = '" + DesignPattern.autoTextBoxDesignPattern.Text.Trim() + "' and CompID = '" + CompID + "'";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                tmpProduct = new Product();

                while (reader.Read())
                {
                    //string isSoldAlert = (reader["IsSoldFlag"] != DBNull.Value) ? (reader.GetBoolean(72).ToString()) : "False";
                    ////if (isSoldAlert == "True")
                    ////{
                    ////    //MessageBox.Show("Item is Sold Out !");
                    ////}
                    ////else
                    ////{

                    ////var CustID = reader.GetValue(0).ToString();

                    //tmpProduct.ItemName = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                    //tmpProduct.PrintName = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                    //tmpProduct.UnitID = (reader["UnitID"] != DBNull.Value) ? (reader.GetString(4)) : "Pc";
                    //tmpProduct.ItemCode = (reader["ItemCode"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";

                    ////tmpProduct.HSN = "9503";  //HSN

                    //tmpProduct.ItemDesc = (reader["ItemDesc"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                    //tmpProduct.ItemBarCode = (reader["ItemBarCode"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
                    //tmpProduct.ItemPrice = (reader["ItemPrice"] != DBNull.Value) ? (reader.GetDouble(9)) : 0;
                    //tmpProduct.SetCriticalLevel = (reader["SetCriticalLevel"] != DBNull.Value) ? (reader.GetBoolean(12)) : false;
                    //tmpProduct.SetDefaultStorageID = (reader["SetDefaultStorageID"] != DBNull.Value) ? (reader.GetInt32(14)) : 0;
                    //tmpProduct.DecimalPlaces = (reader["DecimalPlaces"] != DBNull.Value) ? (reader.GetInt32(17)) : 0;
                    //tmpProduct.IsBarcodeCreated = (reader["IsBarcodeCreated"] != DBNull.Value) ? (reader.GetBoolean(18)) : false;
                    //tmpProduct.ItemPurchPrice = (reader["ItemPurchPrice"] != DBNull.Value) ? (reader.GetDouble(23)) : 0;
                    //tmpProduct.ItemAlias = (reader["ItemAlias"] != DBNull.Value) ? (reader.GetString(30).Trim()) : "";
                    //tmpProduct.UnderGroupID = (reader["UnderGroupID"] != DBNull.Value) ? (reader.GetInt64(32)) : 0;
                    //tmpProduct.UnderSubGroupID = (reader["UnderSubGroupID"] != DBNull.Value) ? (reader.GetInt64(34)) : 0;
                    //tmpProduct.ActualQty = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)) : 0;
                    //tmpProduct.HSN = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
                    //tmpProduct.GSTRate = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)) : 0;
                    //tmpProduct.StorageID = (reader["StorageID"] != DBNull.Value) ? (reader.GetInt32(38)) : 0;
                    //tmpProduct.TrayID = (reader["TrayID"] != DBNull.Value) ? (reader.GetInt32(39)) : 0;
                    //tmpProduct.CounterID = (reader["CounterID"] != DBNull.Value) ? (reader.GetInt32(40)) : 0;
                    ////tmpProduct.UpdateDate = reader.GetDateTime(44); //reader["UpdateDate"] != DBNull.Value) ? (reader.GetDateTime(44)) : "";  
                    //tmpProduct.ActualWt = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)) : 0;
                    ////tmpProduct.LastBuyDate = reader.GetDateTime(47); //(reader["LastBuyDate"] != DBNull.Value) ? (reader.GetDateTime(47) : "";
                    ////tmpProduct.LastSaleDate = reader.GetDateTime(48);//(reader["LastSaleDate"] != DBNull.Value) ? (reader.GetDateTime(48) : "";
                    //tmpProduct.LastSalePrice = (reader["LastSalePrice"] != DBNull.Value) ? (reader.GetDouble(50)) : 0;
                    //tmpProduct.LastBuyPrice = (reader["LastBuyPrice"] != DBNull.Value) ? (reader.GetDouble(51)) : 0;


                    //tmpProduct.DesignNumberPattern = (reader["DesignNumberPattern"] != DBNull.Value) ? (reader.GetString(85)) : "";
                    //tmpProduct.Size = (reader["Size"] != DBNull.Value) ? (reader.GetString(83)) : "";
                    //tmpProduct.Color = (reader["Color"] != DBNull.Value) ? (reader.GetString(84)) : "";
                    //Size.Text = (reader["Size"] != DBNull.Value) ? (reader.GetString(83)) : "";
                    //Color.Text = (reader["Color"] != DBNull.Value) ? (reader.GetString(84)) : "";

                }
                //}
                reader.Close();

                BindComboBoxSizes(DesignPattern.autoTextBoxDesignPattern.Text.Trim(), autocompleteItemName.autoTextBoxGroup.Text.Trim());
            
        }

        public void BindComboBoxSizes(string designPatname, string groupnamepara)
        {
            try
            {
                var custAdpt = new StockItemsByPcTableAdapter();
                var custInfoVal = custAdpt.GetData();
                //var LinqRes = (from UserRec in custInfoVal
                //               orderby UserRec.GroupName ascending
                //               //select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
                //               select (UserRec.GroupName.Trim())).Distinct();
                //GroupName.ItemsSource = LinqRes;

                Size.ItemsSource = custInfoVal.Where(c => ((c.UnderGroupName.Trim() == groupnamepara) && (c.DesignNumberPattern.Trim() == designPatname)))
             .Select(x => x.Size.Trim()).Distinct().ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please Check Group and Pattern/Style ");
            }
            // comboBoxName.SelectedValueBinding = new Binding("Col6");


        }

        private void Size_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Size.SelectedItem != null)
            {
                string sizenameselected = Size.SelectedItem.ToString();
                BindComboBoxColors(DesignPattern.autoTextBoxDesignPattern.Text.Trim(), autocompleteItemName.autoTextBoxGroup.Text.Trim(), sizenameselected.Trim());
            }
        }

        public void BindComboBoxColors(string designPatname, string groupnamepara, string sizeparam)
        {
            var custAdpt = new StockItemsByPcTableAdapter();
            var custInfoVal = custAdpt.GetData();
            Color.ItemsSource = custInfoVal.Where(c => ((c.UnderGroupName.Trim() == groupnamepara) && (c.DesignNumberPattern.Trim() == designPatname) && (c.Size.Trim() == sizeparam)))
         .Select(x => x.Color.Trim()).Distinct().ToList();
        }

        private void Color_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Color.SelectedItem != null)
            {
                string sizenameselected = Color.SelectedItem.ToString();

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select * from StockItemsByPc where DesignNumberPattern = '" + DesignPattern.autoTextBoxDesignPattern.Text.Trim() + "' and Size = '" + Size.Text.Trim() + "' and Color = '" + Color.Text.Trim() + "' and UnderGroupName = '" + autocompleteItemName.autoTextBoxGroup.Text.Trim() + "' and CompID = '" + CompID + "'";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                tmpProduct = new Product();

                while (reader.Read())
                {
                    //string isSoldAlert = (reader["IsSoldFlag"] != DBNull.Value) ? (reader.GetBoolean(72).ToString()) : "False";
                    ////if (isSoldAlert == "True")
                    ////{
                    ////    //MessageBox.Show("Item is Sold Out !");
                    ////}
                    ////else
                    ////{

                    ////var CustID = reader.GetValue(0).ToString();

                    //tmpProduct.ItemName = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                    //tmpProduct.PrintName = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                    //tmpProduct.UnitID = (reader["UnitID"] != DBNull.Value) ? (reader.GetString(4)) : "Pc";
                    //tmpProduct.ItemCode = (reader["ItemCode"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";

                    ////tmpProduct.HSN = "9503";  //HSN

                    //tmpProduct.ItemDesc = (reader["ItemDesc"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                    //tmpProduct.ItemBarCode = (reader["ItemBarCode"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
                    //tmpProduct.ItemPrice = (reader["ItemPrice"] != DBNull.Value) ? (reader.GetDouble(9)) : 0;
                    //tmpProduct.SetCriticalLevel = (reader["SetCriticalLevel"] != DBNull.Value) ? (reader.GetBoolean(12)) : false;
                    //tmpProduct.SetDefaultStorageID = (reader["SetDefaultStorageID"] != DBNull.Value) ? (reader.GetInt32(14)) : 0;
                    //tmpProduct.DecimalPlaces = (reader["DecimalPlaces"] != DBNull.Value) ? (reader.GetInt32(17)) : 0;
                    //tmpProduct.IsBarcodeCreated = (reader["IsBarcodeCreated"] != DBNull.Value) ? (reader.GetBoolean(18)) : false;
                    //tmpProduct.ItemPurchPrice = (reader["ItemPurchPrice"] != DBNull.Value) ? (reader.GetDouble(23)) : 0;
                    //tmpProduct.ItemAlias = (reader["ItemAlias"] != DBNull.Value) ? (reader.GetString(30).Trim()) : "";
                    //tmpProduct.UnderGroupID = (reader["UnderGroupID"] != DBNull.Value) ? (reader.GetInt64(32)) : 0;
                    //tmpProduct.UnderSubGroupID = (reader["UnderSubGroupID"] != DBNull.Value) ? (reader.GetInt64(34)) : 0;
                    //tmpProduct.ActualQty = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)) : 0;
                    //tmpProduct.HSN = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
                    //tmpProduct.GSTRate = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)) : 0;
                    //tmpProduct.StorageID = (reader["StorageID"] != DBNull.Value) ? (reader.GetInt32(38)) : 0;
                    //tmpProduct.TrayID = (reader["TrayID"] != DBNull.Value) ? (reader.GetInt32(39)) : 0;
                    //tmpProduct.CounterID = (reader["CounterID"] != DBNull.Value) ? (reader.GetInt32(40)) : 0;
                    ////tmpProduct.UpdateDate = reader.GetDateTime(44); //reader["UpdateDate"] != DBNull.Value) ? (reader.GetDateTime(44)) : "";  
                    //tmpProduct.ActualWt = (reader["ActualWt"] != DBNull.Value) ? (reader.GetDouble(46)) : 0;
                    ////tmpProduct.LastBuyDate = reader.GetDateTime(47); //(reader["LastBuyDate"] != DBNull.Value) ? (reader.GetDateTime(47) : "";
                    ////tmpProduct.LastSaleDate = reader.GetDateTime(48);//(reader["LastSaleDate"] != DBNull.Value) ? (reader.GetDateTime(48) : "";
                    //tmpProduct.LastSalePrice = (reader["LastSalePrice"] != DBNull.Value) ? (reader.GetDouble(50)) : 0;
                    //tmpProduct.LastBuyPrice = (reader["LastBuyPrice"] != DBNull.Value) ? (reader.GetDouble(51)) : 0;


                    //tmpProduct.DesignNumberPattern = (reader["DesignNumberPattern"] != DBNull.Value) ? (reader.GetString(85)) : "";
                    //tmpProduct.Size = (reader["Size"] != DBNull.Value) ? (reader.GetString(83)) : "";
                    //tmpProduct.Color = (reader["Color"] != DBNull.Value) ? (reader.GetString(84)) : "";

                    txtQty.Text = "0";
                   // tmpProduct.HSN = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
                    //tmpProduct.GSTRate = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)) : 0;
                    cmbUnits.Text = "Pc";
                    txtPrice.Text = "0";
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

        }

        private void TextBoxHighlight_GotFocusQTYCloth(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                textBox.Background = Brushes.BlueViolet;
                textBox.Foreground = Brushes.White;
            }

            //double dbilledQty = 0;
            //double dbilledQtySmall = 0;
            //double dbilledQtyMedium = 0;
            //double dbilledQtyLarge = 0;
            //double dbilledQtyXL = 0;
            //double dbilledQtyXL2 = 0;
            //double dbilledQtyXL3 = 0;
            //double dbilledQtyXL4 = 0;
            //double dbilledQtyXL5 = 0;
            //double dbilledQtyXL6 = 0;


            //dbilledQtySmall = (S.Text.Trim() == "") ? 0 : Convert.ToDouble(S.Text.Trim());
            //dbilledQtyMedium = (M.Text.Trim() == "") ? 0 : Convert.ToDouble(M.Text.Trim());
            //dbilledQtyLarge = (L.Text.Trim() == "") ? 0 : Convert.ToDouble(L.Text.Trim());
            //dbilledQtyXL = (XL.Text.Trim() == "") ? 0 : Convert.ToDouble(XL.Text.Trim());
            //dbilledQtyXL2 = (XL2.Text.Trim() == "") ? 0 : Convert.ToDouble(XL2.Text.Trim());
            //dbilledQtyXL3 = (XL3.Text.Trim() == "") ? 0 : Convert.ToDouble(XL3.Text.Trim());
            //dbilledQtyXL4 = (XL4.Text.Trim() == "") ? 0 : Convert.ToDouble(XL4.Text.Trim());
            //dbilledQtyXL5 = (XL5.Text.Trim() == "") ? 0 : Convert.ToDouble(XL5.Text.Trim());
            //dbilledQtyXL6 = (XL6.Text.Trim() == "") ? 0 : Convert.ToDouble(XL6.Text.Trim());

            //dbilledQty = dbilledQtySmall + dbilledQtyMedium + dbilledQtyLarge + dbilledQtyXL + dbilledQtyXL2 + dbilledQtyXL3 + dbilledQtyXL4 + dbilledQtyXL5 + dbilledQtyXL6;
            //txtQty.Text = dbilledQty.ToString();
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
                    //autocompltCustName.autoTextBox.Text = "Cash";
                    //autocompltCustName.autoTextBox.Focus();
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

        private void addDesignPattern_Click(object sender, RoutedEventArgs e)
        {
            string GroupnamePara = autocompleteItemName.autoTextBoxGroup.Text.Trim();
            string designpattPara = DesignPattern.autoTextBoxDesignPattern.Text.Trim();
            AddDesignPatternClothItem viewBillObj = new AddDesignPatternClothItem(GroupnamePara, designpattPara);
            viewBillObj.ShowDialog();
            autocompleteItemName.autoTextBoxGroup.Focus();

        }








    }
}
