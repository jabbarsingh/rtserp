
using RTSJewelERP.CompanyTableAdapters;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Controls.DataVisualization.Charting;
using System.Data;
using System.Net;
using HtmlAgilityPack;
namespace RTSJewelERP
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        
        System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();

        private string estimateHomeIcon = "";
        private string saleHomeIcon = "";
        private string purchaseHomeIcon = "";
        private string ItemHomeIcon = "";
        private string stockEtryBarcodeHomeIcon = "";
        private string stockentryHomeIcon = "";
        private string stockentryWatchHomeIcon = "";
        private string DayReportHomeIcon = "";
        private string AccountHomeIcon = "";
        private string ReceiptHomeIcon = "";
        private string PaymentHomeIcon = "";
        private string JournalHomeIcon = "";
        private string BarcodeHomeIcon = "";
        private string CloseHomeIcon = "";
        private string BackupShortcutHome = "";
        private string AddAccountShortcutHome = "";
        private string AddItemShortcutHome = "";
        private string AddSaleShortcutHome = "";
        private string AddPurchaseShortcutHome = "";
        private string AddReceiptShortcutHome = "";
        private string AddPaymentShortcutHome = "";
        private string AddJournalShortcutHome = "";
        private string StockEntryShortcutHome = "";
        private string StockTransferShortcutHome = "";

        private string EstimationShortcutHome = "";
        private string EstimationA6ShortcutHome = "";
        private string WatchShortcutHome = "";
        private string DayBookShortcutHome = "";
  



        public HomePage()
        {
            InitializeComponent();                                 
            Timer.Tick += new EventHandler(Timer_Click);
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();


            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window
            //SaleInvoiceImg.Focus();

            //WindowState.Maximized;
            BindComboBox(CompName);
            //SaleInvoiceImg.Focus();


            
            
            
            
            //var companynameids = CompName.SelectedItem.ToString();
            ////ConfigClass.CompID = ((ComboBoxItem)CompName.SelectedItem).ToString();
            //string companyidsSelect = companynameids.Split('-')[1];


            List<SaleList> list = new List<SaleList>();
            SqlConnection connDelete = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            connDelete.Open();
            string query = "";
            SqlCommand cmdDelete;
            SqlDataReader dr;
            //PurchaseInvoices
            if (1 == 1)
            {
                //query = "Select  AccountName,Sum(CAST(InvoiceAmt AS float)) AS TotalInvValue from SalesVouchers group by AccountName";
                query = "Select top 12 (SELECT MONTH(TransactionDate)),Sum(CAST(InvoiceAmt AS float)) AS TotalInvValue from SalesVouchers where  (SELECT MONTH(TransactionDate) ) > (SELECT MONTH(TransactionDate) - 12)  and CompID = '1'  group by Month(TransactionDate)  order by Month(TransactionDate) desc";

                cmdDelete = new SqlCommand(query, connDelete);

                dr = cmdDelete.ExecuteReader();
                while (dr.Read())
                {
                    var reading = new SaleList
                    {
                        Month = dr[0].ToString().Trim(),// I have a error here because I didn't convert type 'string' to 'float'
                        Value = Convert.ToDouble(dr[1])
                    };
                    list.Add(reading);
                }
                dr.Close();

            }


            cmdDelete.Connection.Close();

            ((ColumnSeries)SaleChart.Series[0]).ItemsSource = list;
            //((System.Windows.Controls.DataVisualization.Charting.ColumnSeries)SaleChart.Series[0]).ItemsSource = list;

            List<PurList> listPurchase = new List<PurList>();
            SqlConnection connDeletePur = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            connDeletePur.Open();
            string queryPur = "";
            SqlCommand cmdDeletePur;
            SqlDataReader drPur;
            //PurchaseInvoices
            if (1 == 1)
            {
                //query = "Select  AccountName,Sum(CAST(InvoiceAmt AS float)) AS TotalInvValue from SalesVouchers group by AccountName";
                queryPur = "Select top 12 (SELECT MONTH(TransactionDate)),Sum(CAST(InvoiceAmt AS float)) AS TotalInvValue from PurchaseVouchers where  (SELECT MONTH(TransactionDate) ) > (SELECT MONTH(TransactionDate) - 12)  and CompID = '1'   group by Month(TransactionDate)  order by Month(TransactionDate) desc";

                cmdDeletePur = new SqlCommand(queryPur, connDeletePur);

                drPur = cmdDeletePur.ExecuteReader();
                while (drPur.Read())
                {
                    var readingpur = new PurList
                    {
                        PurMonth = drPur[0].ToString().Trim(),// I have a error here because I didn't convert type 'string' to 'float'
                        PurValue = Convert.ToDouble(drPur[1])
                    };
                    listPurchase.Add(readingpur);
                }
                drPur.Close();

            }


            cmdDeletePur.Connection.Close();


            ((System.Windows.Controls.DataVisualization.Charting.ColumnSeries)PurchaseChart.Series[0]).ItemsSource = listPurchase;



            SqlConnection conn2 = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conn2.Open();
          
            string sql = "select * from ConfigTable";

            SqlCommand cmdconfig = new SqlCommand(sql);
            cmdconfig.Connection = conn2;
            SqlDataReader readerConfig = cmdconfig.ExecuteReader();


            if (readerConfig.HasRows)
            {
                while (readerConfig.Read())
                {
                    string nameconf = (readerConfig["Name"] != DBNull.Value) ? (readerConfig.GetString(0).Trim()) : "";
                    //IDNo.Text = (readerConfig["ID"] != DBNull.Value) ? (readerConfig.GetString(1).Trim()) : "";
                    string parentconf = (readerConfig["Parent"] != DBNull.Value) ? (readerConfig.GetString(2).Trim()) : "";
                    string grandparentconf = (readerConfig["GrandParent"] != DBNull.Value) ? (readerConfig.GetString(3).Trim()) : "";
                    string pagenameconf = (readerConfig["MapTo"] != DBNull.Value) ? (readerConfig.GetString(4).Trim()) : "";

                    if (nameconf.Trim() == "Estimate" && parentconf.Trim() == "HomeIcon")
                    {
                        estimateHomeIcon = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "Sale" && parentconf.Trim() == "HomeIcon")
                    {
                        saleHomeIcon = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "Purchase" && parentconf.Trim() == "HomeIcon")
                    {
                        purchaseHomeIcon = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "Item" && parentconf.Trim() == "HomeIcon")
                    {
                        ItemHomeIcon = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "StockEntry-Barcode" && parentconf.Trim() == "HomeIcon")
                    {
                        stockEtryBarcodeHomeIcon = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "StockEntry" && parentconf.Trim() == "HomeIcon")
                    {
                        stockentryHomeIcon = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "StockEntry-Watch" && parentconf.Trim() == "HomeIcon")
                    {
                        stockentryWatchHomeIcon = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "DayReport" && parentconf.Trim() == "HomeIcon")
                    {
                        DayReportHomeIcon = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "Account" && parentconf.Trim() == "HomeIcon")
                    {
                        AccountHomeIcon = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "Receipt" && parentconf.Trim() == "HomeIcon")
                    {
                        ReceiptHomeIcon = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "Payment" && parentconf.Trim() == "HomeIcon")
                    {
                        PaymentHomeIcon = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "Journal" && parentconf.Trim() == "HomeIcon")
                    {
                        JournalHomeIcon = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "Barcode" && parentconf.Trim() == "HomeIcon")
                    {
                        BarcodeHomeIcon = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "Close" && parentconf.Trim() == "HomeIcon")
                    {
                        CloseHomeIcon = pagenameconf.Trim();
                    }


                    if (nameconf.Trim() == "AddAccountShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        AddAccountShortcutHome = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "AddItemShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        AddItemShortcutHome = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "AddSaleShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        AddSaleShortcutHome = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "AddPurchaseShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        AddPurchaseShortcutHome = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "ReceiptShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        AddReceiptShortcutHome = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "PaymentShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        AddPaymentShortcutHome = pagenameconf.Trim();
                    }

                    if (nameconf.Trim() == "AddJournalShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        AddJournalShortcutHome = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "StockEntryShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        StockEntryShortcutHome = pagenameconf.Trim();
                    }


                    if (nameconf.Trim() == "StockTransferShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        StockTransferShortcutHome = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "BackupShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        BackupShortcutHome = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "EstimationShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        EstimationShortcutHome = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "Estimation-A6ShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        EstimationA6ShortcutHome = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "WatchShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        WatchShortcutHome = pagenameconf.Trim();
                    }
                    if (nameconf.Trim() == "DayBookShortcutHome" && parentconf.Trim() == "ShortcutHome")
                    {
                        DayBookShortcutHome = pagenameconf.Trim();

                    }


                   
                }
            }

            readerConfig.Close();


            ///







        }



        private void Timer_Click(object sender, EventArgs e)
        {
            DateTime d;
            d = DateTime.Now;
            timer.Content = d.Hour + " : " + d.Minute + " : " + d.Second;
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
        }

        public void BindComboBox(ComboBox company)
        {
            var custAdpt = new CompanyTableAdapter();
            var custInfoVal = custAdpt.GetData();

//            CompName.ItemsSource = custInfoVal.Where(c => (c.Alias.Trim() == "GST"))
//.Select(x => (x.CompanyName.Trim() + "-" + x.CompanyID)).Distinct().ToList();

            //Background="#f5bd88"  set for GST else Light Blue

            CompName.ItemsSource = custInfoVal.Where(c => (c.Alias.Trim() == "EST"))
.Select(x => (x.CompanyName.Trim() + "-" + x.CompanyID)).Distinct().ToList();



        }


        class SaleList
        {
            public string Month { get; set; }
            public double Value { get; set; }
        }

        class PurList
        {
            public string PurMonth { get; set; }
            public double PurValue { get; set; }
        }


        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    SaleVoucher sv = new SaleVoucher();
        //    this.NavigationService.Navigate(sv);
        //}


        private void StockEntry_Click(object sender, RoutedEventArgs e)
        {
            StockEntry sv = new StockEntry();
            sv.ShowDialog();
        }

        private void Sale_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (saleHomeIcon == "SaleVoucherQtyGSTRetailRegular")
            {
                SaleVoucherQtyGSTRetailRegular sv = new SaleVoucherQtyGSTRetailRegular("");
                this.NavigationService.Navigate(sv);
            }


            if (saleHomeIcon == "SaleVoucherEstimationJewelleryConsolidatedLatha")
            {
                SaleVoucherEstimationJewelleryConsolidatedLatha sv = new SaleVoucherEstimationJewelleryConsolidatedLatha();
                this.NavigationService.Navigate(sv);
            }

            if (saleHomeIcon == "SaleVoucherEstimationBarcodeJewellConsolidated")
            {
                SaleVoucherEstimationBarcodeJewellConsolidated sv = new SaleVoucherEstimationBarcodeJewellConsolidated();
                this.NavigationService.Navigate(sv);
            } 

            if (saleHomeIcon == "SaleVoucherBarcodeConsolidated")
            {
                SaleVoucherBarcodeConsolidated sv = new SaleVoucherBarcodeConsolidated();
                this.NavigationService.Navigate(sv);
            }

            if (saleHomeIcon == "SaleVoucherJewellLathaConsolidated")
            {
                SaleVoucherJewellLathaConsolidated sv = new SaleVoucherJewellLathaConsolidated();
                this.NavigationService.Navigate(sv);
            }
            if (saleHomeIcon == "SaleQtyEstimationVoucherSizeItemDesc")
            {
                SaleQtyEstimationVoucherSizeItemDesc sv = new SaleQtyEstimationVoucherSizeItemDesc();
                this.NavigationService.Navigate(sv);
            }
            if (saleHomeIcon == "SaleVoucherQtyEstimationA5SizePdf")
            {
                SaleVoucherQtyEstimationA5SizePdf sv = new SaleVoucherQtyEstimationA5SizePdf();
                this.NavigationService.Navigate(sv);
            }

            //GetInstance(saleHomeIcon);
            if (saleHomeIcon == "SaleQtyEstimationVoucherSizeA6")
            {
                SaleQtyEstimationVoucherSizeA6 sv = new SaleQtyEstimationVoucherSizeA6();
                this.NavigationService.Navigate(sv);
            }
            if (saleHomeIcon == "SaleVoucher")
            {
                SaleVoucher sv = new SaleVoucher();
                this.NavigationService.Navigate(sv);
            }
            if (saleHomeIcon == "SaleVoucherBarcode")
            {
                SaleVoucherBarcode sv = new SaleVoucherBarcode();
                this.NavigationService.Navigate(sv);
            }
            if (saleHomeIcon == "SaleVoucherEstimationBarcodeJewell")
            {
                SaleVoucherEstimationBarcodeJewell sv = new SaleVoucherEstimationBarcodeJewell();
                this.NavigationService.Navigate(sv);
            }

            if (saleHomeIcon == "SaleVoucherEstimationBarcodeJewellUpdate")
            {
                SaleVoucherEstimationBarcodeJewellUpdate sv = new SaleVoucherEstimationBarcodeJewellUpdate();
                this.NavigationService.Navigate(sv);
            }
            if (saleHomeIcon == "SaleQtyGSTRetailComposition")
            {
                SaleQtyGSTRetailComposition sv = new SaleQtyGSTRetailComposition();
                this.NavigationService.Navigate(sv);
            }
            if (saleHomeIcon == "SaleQtyGSTRetailComposition")
            {
                SaleQtyGSTRetailComposition sv = new SaleQtyGSTRetailComposition();
                this.NavigationService.Navigate(sv);
            }
            if (saleHomeIcon == "SaleVoucherClothKiran")
            {
                SaleVoucherClothKiran sv = new SaleVoucherClothKiran();
                this.NavigationService.Navigate(sv);
            }
            if (saleHomeIcon == "SaleVoucherAllInOneQtyGST")
            {
                SaleVoucherAllInOneQtyGST sv = new SaleVoucherAllInOneQtyGST();
                this.NavigationService.Navigate(sv);
            }
            if (saleHomeIcon == "SaleVoucherAllInOneQtyGSTSteel")
            {
                SaleVoucherAllInOneQtyGSTSteel sv = new SaleVoucherAllInOneQtyGSTSteel("");
                this.NavigationService.Navigate(sv);
            }

            if (saleHomeIcon == "SaleestimationVoucherJewellLatha")
            {
                SaleestimationVoucherJewellLatha sv = new SaleestimationVoucherJewellLatha("");
                this.NavigationService.Navigate(sv);
            }
            if (saleHomeIcon == "SaleVoucherJewellLatha")
            {
                SaleVoucherJewellLatha sv = new SaleVoucherJewellLatha("");
                this.NavigationService.Navigate(sv);
            }

            if (saleHomeIcon == "SaleQtyEstimationVoucherSizeA6NoDisc")
            {
                SaleQtyEstimationVoucherSizeA6NoDisc sv = new SaleQtyEstimationVoucherSizeA6NoDisc();
                this.NavigationService.Navigate(sv);
            }

            if (saleHomeIcon == "SaleVoucherEstimationBarcodeWatch")
            {
                SaleVoucherEstimationBarcodeWatch sv = new SaleVoucherEstimationBarcodeWatch();
                this.NavigationService.Navigate(sv);
            }
            if (saleHomeIcon == "SaleVoucherClothHitesh")
            {
                SaleVoucherClothHitesh sv = new SaleVoucherClothHitesh();
                this.NavigationService.Navigate(sv);
            }

            if (saleHomeIcon == "SaleVoucherQtyGhansyam")
            {
                SaleVoucherQtyGhansyam sv = new SaleVoucherQtyGhansyam("");
                this.NavigationService.Navigate(sv);
            }
            //SaleVoucherBarcode sv = new SaleVoucherBarcode(); 
            //SaleVoucherEstimationBarcodeJewell sv = new SaleVoucherEstimationBarcodeJewell(); 
            //SaleQtyGSTRetailComposition sv = new SaleQtyGSTRetailComposition();
            //SaleVoucherClothKiran sv = new SaleVoucherClothKiran(); 
            //SaleVoucherAllInOneQtyGST sv = new SaleVoucherAllInOneQtyGST();
            //SaleVoucherJewellLatha sv = new SaleVoucherJewellLatha(); 
            
        }


        private void Estimate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (estimateHomeIcon == "SaleQtyEstimationVoucherSizeA6")
            {
                SaleQtyEstimationVoucherSizeA6 sv = new SaleQtyEstimationVoucherSizeA6();
                this.NavigationService.Navigate(sv);
            }

            if (estimateHomeIcon == "SaleVoucherEstimationJewelleryConsolidatedLatha")
            {
                SaleVoucherEstimationJewelleryConsolidatedLatha sv = new SaleVoucherEstimationJewelleryConsolidatedLatha();
                this.NavigationService.Navigate(sv);
            }

            if (estimateHomeIcon == "SaleVoucherEstimationBarcodeJewellConsolidated")
            {
                SaleVoucherEstimationBarcodeJewellConsolidated sv = new SaleVoucherEstimationBarcodeJewellConsolidated();
                this.NavigationService.Navigate(sv);
            }

            if (estimateHomeIcon == "SaleVoucherBarcodeConsolidated")
            {
                SaleVoucherBarcodeConsolidated sv = new SaleVoucherBarcodeConsolidated();
                this.NavigationService.Navigate(sv);
            }

            if (estimateHomeIcon == "SaleVoucherJewellLathaConsolidated")
            {
                SaleVoucherJewellLathaConsolidated sv = new SaleVoucherJewellLathaConsolidated();
                this.NavigationService.Navigate(sv);
            }


            if (estimateHomeIcon == "SaleVoucherQtyEstimationA5SizePdf")
            {
                SaleVoucherQtyEstimationA5SizePdf sv = new SaleVoucherQtyEstimationA5SizePdf();
                this.NavigationService.Navigate(sv);
            }

            if (estimateHomeIcon == "SaleQtyEstimationVoucherSizeItemDesc")
            {
                SaleQtyEstimationVoucherSizeItemDesc sv = new SaleQtyEstimationVoucherSizeItemDesc();
                this.NavigationService.Navigate(sv);
            }

            if (estimateHomeIcon == "SaleVoucherEstimationBarcodeJewell")
            {
                SaleVoucherEstimationBarcodeJewell sv = new SaleVoucherEstimationBarcodeJewell();
                this.NavigationService.Navigate(sv);
            }

            if (estimateHomeIcon == "SaleVoucherEstimationBarcodeJewellUpdate")
            {
                SaleVoucherEstimationBarcodeJewellUpdate sv = new SaleVoucherEstimationBarcodeJewellUpdate();
                this.NavigationService.Navigate(sv);
            }

            

            if (estimateHomeIcon == "SaleestimationVoucherJewellLatha")
            {
                SaleestimationVoucherJewellLatha sv = new SaleestimationVoucherJewellLatha("");
                this.NavigationService.Navigate(sv);
            }


            if (estimateHomeIcon == "SaleQtyEstimationVoucherSizeA6NoDisc")
            {
                SaleQtyEstimationVoucherSizeA6NoDisc sv = new SaleQtyEstimationVoucherSizeA6NoDisc();
                this.NavigationService.Navigate(sv);
            }

            if (estimateHomeIcon == "SaleVoucherEstimationBarcodeWatch")
            {
                SaleVoucherEstimationBarcodeWatch sv = new SaleVoucherEstimationBarcodeWatch();
                this.NavigationService.Navigate(sv);
            }

            if (estimateHomeIcon == "SaleQtyEstimationVoucher")
            {
                SaleQtyEstimationVoucher sv = new SaleQtyEstimationVoucher();
                this.NavigationService.Navigate(sv);
            }
            //SaleQtyEstimationVoucherSizeA6NoDisc sv = new SaleQtyEstimationVoucherSizeA6NoDisc();
            //SaleestimationVoucherJewellLatha sv = new SaleestimationVoucherJewellLatha();
            //SaleVoucherBarcode sv = new SaleVoucherBarcode(); 
            //SaleVoucherEstimationBarcodeJewell sv = new SaleVoucherEstimationBarcodeJewell(); 
            //SaleQtyGSTRetailComposition sv = new SaleQtyGSTRetailComposition();
            //SaleVoucherClothKiran sv = new SaleVoucherClothKiran(); 
            //SaleVoucherAllInOneQtyGST sv = new SaleVoucherAllInOneQtyGST(); 
            //SaleVoucherJewellLatha sv = new SaleVoucherJewellLatha();
            //this.NavigationService.Navigate(sv);
        }

        private void DayReport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("Coming Soon...");
            CashPothabaki sv = new CashPothabaki();
            sv.ShowDialog();
            //SaleVoucherBarcode sv = new sa();
            //this.NavigationService.Navigate(sv);
        }

        private void Purchase_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (purchaseHomeIcon == "PurchaseVoucher")
            {
                PurchaseVoucher sv = new PurchaseVoucher("");
                this.NavigationService.Navigate(sv);
            }

            if (purchaseHomeIcon == "PurchaseVoucherGroupEntryQtyCloths")
            {
                PurchaseVoucherGroupEntryQtyCloths sv = new PurchaseVoucherGroupEntryQtyCloths();
                this.NavigationService.Navigate(sv);
            }
            if (purchaseHomeIcon == "PurchaseQtyGSTVoucherxaml")
            {
                PurchaseQtyGSTVoucherxaml sv = new PurchaseQtyGSTVoucherxaml("");
                this.NavigationService.Navigate(sv);
            }



            if (purchaseHomeIcon == "SaleestimationVoucherJewellLatha")
            {
                SaleestimationVoucherJewellLatha sv = new SaleestimationVoucherJewellLatha("");
                this.NavigationService.Navigate(sv);
            }


            if (purchaseHomeIcon == "SaleQtyEstimationVoucherSizeA6NoDisc")
            {
                SaleQtyEstimationVoucherSizeA6NoDisc sv = new SaleQtyEstimationVoucherSizeA6NoDisc();
                this.NavigationService.Navigate(sv);
            }

            if (purchaseHomeIcon == "SaleVoucherEstimationBarcodeWatch")
            {
                SaleVoucherEstimationBarcodeWatch sv = new SaleVoucherEstimationBarcodeWatch();
                this.NavigationService.Navigate(sv);
            }


            //PurchaseVoucher sv = new PurchaseVoucher();
            //PurchaseQtyGSTVoucherxaml sv = new PurchaseQtyGSTVoucherxaml();
            //NavigationWindow navWIN = new NavigationWindow();
            //navWIN.Content = new PurchaseVoucher();
            //navWIN.Show();
            //this.NavigationService.Navigate(sv);
        }

        private void Rokad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rokad sv = new Rokad();
            sv.ShowDialog();
        }

        private void Receipt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Receipt sv = new Receipt("");
            this.NavigationService.Navigate(sv);
        }

        private void Pay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Payment sv = new Payment("");
            this.NavigationService.Navigate(sv);
        }

        private void Journal_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Journal sv = new Journal();
            this.NavigationService.Navigate(sv);
        }

        private void BarCode_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Bracode sv = new Bracode();          
            this.NavigationService.Navigate(sv);
        }

        private void Account_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AccountMaster sv = new AccountMaster();
            this.NavigationService.Navigate(sv);
        } 
        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {

            if (ItemHomeIcon == "ItemMasterJewell")
            {
                ItemMasterJewell sv = new ItemMasterJewell();
                sv.ShowDialog();
            }
            if (ItemHomeIcon == "ItemMaster")
            {
                ItemMaster sv = new ItemMaster();
                this.NavigationService.Navigate(sv);
            }
          
           


            //ItemMaster sv = new ItemMaster();
            //this.NavigationService.Navigate(sv);
        }

        private void StockEntry_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (stockEtryBarcodeHomeIcon == "StockEntry")
            {
                StockEntry sv = new StockEntry();
                sv.ShowDialog();
            }

            if (stockEtryBarcodeHomeIcon == "StockEntryMain")
            {
                StockEntryMain sv = new StockEntryMain();
                sv.ShowDialog();
            }

            if (stockEtryBarcodeHomeIcon == "StockEntryWatchBarCode")
            {
                StockEntryWatchBarCode sv = new StockEntryWatchBarCode();
                this.NavigationService.Navigate(sv);
            }
            if (stockEtryBarcodeHomeIcon == "ItemMasterJewell")
            {
                ItemMasterJewell sv = new ItemMasterJewell();
                sv.ShowDialog();
            }

        }
        private void Backup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Backup bkp = new Backup();
            bkp.ShowDialog();
        }

        private void ListBoxBackup_Selected(object sender, RoutedEventArgs e)
        {
            //SaleVoucher sv = new SaleVoucher();
            //this.NavigationService.Navigate(sv);
            Backup bkp = new Backup();
            bkp.ShowDialog();
        }

        private void ListBoxSale_Selected(object sender, RoutedEventArgs e)
        {
           
            //NavigationWindow navWIN = new NavigationWindow();
            //navWIN.Content = new SaleVoucher();
            //navWIN.Show();

            NavigationWindow navWIN = new NavigationWindow();
            navWIN.Content = new SaleVoucherAllInOneQtyGST();
            navWIN.Show(); 
        }


        private void ListBoxPurchase_Selected(object sender, RoutedEventArgs e)
        {
            //PurchaseVoucher sv = new PurchaseVoucher();
            //this.NavigationService.Navigate(sv);
            NavigationWindow navWIN = new NavigationWindow();
            navWIN.Content = new PurchaseVoucher("");
            navWIN.Show(); 
        }


        private void ListBoxReceipt_Selected(object sender, RoutedEventArgs e)
        {

            //Receipt sv = new Receipt();
            //this.NavigationService.Navigate(sv);

            NavigationWindow navWIN = new NavigationWindow();
            navWIN.Content = new Receipt("");
            navWIN.Show(); 
        }


        private void ListBoxPayment_Selected(object sender, RoutedEventArgs e)
        {
            //Payment sv = new Payment();
            //this.NavigationService.Navigate(sv);
            NavigationWindow navWIN = new NavigationWindow();
            navWIN.Content = new Payment("");
            navWIN.Show(); 
        }


        private void ListBoxJournal_Selected(object sender, RoutedEventArgs e)
        {
            //Journal sv = new Journal();
            //this.NavigationService.Navigate(sv);
            NavigationWindow navWIN = new NavigationWindow();
            navWIN.Content = new Journal();
            navWIN.Show(); 
        }

        private void ListBoxAccount_Selected(object sender, RoutedEventArgs e)
        {
            //AccountMaster sv = new AccountMaster();
            //this.NavigationService.Navigate(sv);
            NavigationWindow navWIN = new NavigationWindow();
            navWIN.Content = new AccountMaster();
            navWIN.Show(); 
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            //ItemMaster sv = new ItemMaster();
            //this.NavigationService.Navigate(sv);
            NavigationWindow navWIN = new NavigationWindow();
            navWIN.Content = new AccountMaster();
            navWIN.Show(); 
        }

        //private void ListBoxJournal_Selected(object sender, RoutedEventArgs e)
        //{
        //    Journal sv = new Journal();
        //    this.NavigationService.Navigate(sv);
        //}
        private void MenuItem_NewCompanyClick(object sender, RoutedEventArgs e)
        {
            CompanyMaster sv = new CompanyMaster();
            this.NavigationService.Navigate(sv);
        }

        private void MenuItem_NewSaleClick(object sender, RoutedEventArgs e)
        {
            //SaleVoucherBarcode sv = new SaleVoucherBarcode();
            //this.NavigationService.Navigate(sv);

            //SaleVoucher sv = new SaleVoucher();
            //this.NavigationService.Navigate(sv);

            NavigationWindow navWIN = new NavigationWindow();
            navWIN.Content = new SaleVoucherBarcode();
            navWIN.Show(); 

            //NavigationWindow navWIN = new NavigationWindow();
            //navWIN.Content = new SaleVoucherClothHitesh();
            //navWIN.Show(); 

            //NavigationWindow navWIN = new NavigationWindow();
            //navWIN.Content = new SaleQtyEstimationVoucherSizeA6();
            //navWIN.Show();

            //NavigationWindow navWIN = new NavigationWindow();
            //navWIN.Content = new SaleVoucherAllInOneQtyGST();
            //navWIN.Show();  

        }

        private void MenuItem_NewPurchaseClick(object sender, RoutedEventArgs e)
        {
            PurchaseVoucher sv = new PurchaseVoucher("");
            this.NavigationService.Navigate(sv);
        }

        private void MenuItem_NewPaymentClick(object sender, RoutedEventArgs e)
        {
            Payment sv = new Payment("");
            this.NavigationService.Navigate(sv);
        }

        private void MenuItem_NewReceiptClick(object sender, RoutedEventArgs e)
        {
            Receipt sv = new Receipt("");
            this.NavigationService.Navigate(sv);
        }

        private void MenuItem_NewJournalClick(object sender, RoutedEventArgs e)
        {
            Receipt sv = new Receipt("");
            this.NavigationService.Navigate(sv);
        }

        private void MenuItem_CreditNoteClick(object sender, RoutedEventArgs e)
        {
            CreditNote sv = new CreditNote();
            this.NavigationService.Navigate(sv);
        }

        private void NewItem_Click(object sender, RoutedEventArgs e)
        {
            ItemMaster sv = new ItemMaster();
            this.NavigationService.Navigate(sv);
        } 

        private void NewGroup_Click(object sender, RoutedEventArgs e)
        {
            AddStockGroup sv = new AddStockGroup();
            sv.ShowDialog();
        }

        private void NewAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountMaster sv = new AccountMaster();
            this.NavigationService.Navigate(sv);
        }

        private void MenuItem_Sheet(object sender, RoutedEventArgs e)
        {
            //SheetHome sv = new SheetHome();
            //sv.ShowDialog();
            NavigationWindow navWIN = new NavigationWindow();
            navWIN.Content = new SheetHome();
            navWIN.Show(); 

        }

        private void NewConfig_Click(object sender, RoutedEventArgs e)
        {
            ConfigurationalWindow sv = new ConfigurationalWindow();
            //this.NavigationService.Navigate(sv);
            sv.ShowDialog();
        }

        private void LetterPad_Click(object sender, RoutedEventArgs e)
        {
            LetterPad sv = new LetterPad();
            //this.NavigationService.Navigate(sv);
            sv.ShowDialog();
        }


        private void DetachAttach_Click(object sender, RoutedEventArgs e)
        {
            AttachDetach sv = new AttachDetach();
            //this.NavigationService.Navigate(sv);
            sv.ShowDialog();
        }

        private void NewSMS_Click(object sender, RoutedEventArgs e)
        {
            SendBulkSMS sv = new SendBulkSMS();
            //this.NavigationService.Navigate(sv);
            sv.ShowDialog();
        }


        private void ResetFactory_Click(object sender, RoutedEventArgs e)
        {
            FactoryResetDatabase sv = new FactoryResetDatabase();
            //this.NavigationService.Navigate(sv);
            sv.ShowDialog();
        }

        private void ResetInventory_Click(object sender, RoutedEventArgs e)
        {

            ResetStock sv = new ResetStock();
            //this.NavigationService.Navigate(sv);
            sv.ShowDialog();
        }

        private void SelectBusiness_Click(object sender, RoutedEventArgs e)
        {

            ResetStock sv = new ResetStock();
            //this.NavigationService.Navigate(sv);
            sv.ShowDialog();
        }


        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {

            DeleteAccount sv = new DeleteAccount();
            //this.NavigationService.Navigate(sv);
            sv.ShowDialog();
        }


        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {

            DeleteItem sv = new DeleteItem();
            //this.NavigationService.Navigate(sv);
            sv.ShowDialog();
        }


        private void ConsolidateDayReport_Click(object sender, RoutedEventArgs e)
        {
            ProtectedWindow sv = new ProtectedWindow();
            sv.ShowDialog();

        }
        private void RokadBook_Click(object sender, RoutedEventArgs e)
        {
            Rokad sv = new Rokad();
            sv.ShowDialog();

        }
        private void BankBook_Click(object sender, RoutedEventArgs e)
        {
            BankBook sv = new BankBook();
            sv.ShowDialog();

        }


        private void WhatUWant_Click(object sender, RoutedEventArgs e)
        {
            WhatUWant sv = new WhatUWant();
            //this.NavigationService.Navigate(sv);
            sv.ShowDialog();
        }

        private void GSTR_Click(object sender, RoutedEventArgs e)
        {
            GSTReports sv = new GSTReports();
            //this.NavigationService.Navigate(sv);
            sv.ShowDialog();
        }

        private void CompName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConfigClass.CompID = "1";
            
            var companyname = CompName.SelectedItem.ToString();
            //ConfigClass.CompID = ((ComboBoxItem)CompName.SelectedItem).ToString();
            string companyid = companyname.Split('-')[1];
            //ConfigClass.CompID = CompName.Text;
            ConfigClass.CompID = companyid;

            string aliasname = "GST";
            ///ICON Sets based on Company Type
            SqlConnection conCmp = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conCmp.Open();
            string sqlCmp = "select top 1  CompanyName,Alias,GSTIN,* from Company where   CompanyID = '" + companyid + "'";
            SqlCommand cmdCmp = new SqlCommand(sqlCmp);
            cmdCmp.Connection = conCmp;
            SqlDataReader readerCmp = cmdCmp.ExecuteReader();

            while (readerCmp.Read())
            {


                //var CustID = reader.GetValue(0).ToString();

                //tmpProduct.ItemName = (reader["AcctName"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "";
                //GSTINCompany = (readerCmp["GSTIN"] != DBNull.Value) ? (readerCmp.GetString(1).Trim()) : "";
                 aliasname  = (readerCmp["Alias"] != DBNull.Value) ? (readerCmp.GetString(1).Trim()) : "";

            }
            readerCmp.Close();
            if (aliasname == "EST")            
            {
                SaleInvoiceImg.Visibility = Visibility.Collapsed;
                lblIconSale.Visibility = Visibility.Collapsed;

                EstimateInvoiceImg.Visibility = Visibility.Visible;
                lblEstIcon.Visibility = Visibility.Visible;

                //MainWindow mw = (MainWindow)Application.Current.MainWindow;
                //mw.Background = "Red";
            }

            if (aliasname == "GST")
            {
                SaleInvoiceImg.Visibility = Visibility.Visible;
                lblIconSale.Visibility = Visibility.Visible;

                EstimateInvoiceImg.Visibility = Visibility.Collapsed;
                lblEstIcon.Visibility = Visibility.Collapsed; 
            }

        }

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountMaster sv = new AccountMaster();
            this.NavigationService.Navigate(sv);
        }

        private void AddItem_Click(object sender, RoutedEventArgs e)
        {
            if (AddItemShortcutHome == "ItemMaster")
            {
                ItemMaster sv = new ItemMaster();
                this.NavigationService.Navigate(sv);
            }
            if (AddItemShortcutHome == "ItemMasterJewell")
            {
                ItemMasterJewell sv = new ItemMasterJewell();
                sv.ShowDialog();
            }
            if (AddItemShortcutHome == "AddItem")
            {
                AddItem sv = new AddItem();
                sv.ShowDialog();
            }

        }

        private void AddSale_Click(object sender, RoutedEventArgs e)
        {



            if (AddSaleShortcutHome == "SaleVoucherQtyGSTRetailRegular")
            {
                SaleVoucherQtyGSTRetailRegular sv = new SaleVoucherQtyGSTRetailRegular("");
                this.NavigationService.Navigate(sv);
            }

            if (AddSaleShortcutHome == "SaleQtyEstimationVoucherSizeItemDesc")
            {
                SaleQtyEstimationVoucherSizeItemDesc sv = new SaleQtyEstimationVoucherSizeItemDesc();
                this.NavigationService.Navigate(sv);
            }

            if (AddSaleShortcutHome == "SaleVoucherQtyEstimationA5SizePdf")
            {
                SaleVoucherQtyEstimationA5SizePdf sv = new SaleVoucherQtyEstimationA5SizePdf();
                this.NavigationService.Navigate(sv);
            }


             
            if (AddSaleShortcutHome == "SaleVoucherQtyGhansyam")
            {
                SaleVoucherQtyGhansyam sv = new SaleVoucherQtyGhansyam("");
                this.NavigationService.Navigate(sv);
            }

            if (AddSaleShortcutHome == "SaleVoucherEstimationJewelleryConsolidatedLatha")
            {
                SaleVoucherEstimationJewelleryConsolidatedLatha sv = new SaleVoucherEstimationJewelleryConsolidatedLatha();
                this.NavigationService.Navigate(sv);
            }
            if (AddSaleShortcutHome == "SaleVoucherBarcodeConsolidated")
            {
                SaleVoucherBarcodeConsolidated sv = new SaleVoucherBarcodeConsolidated();
                this.NavigationService.Navigate(sv);
            }

            if (AddSaleShortcutHome == "SaleVoucherJewellLathaConsolidated")
            {
                SaleVoucherJewellLathaConsolidated sv = new SaleVoucherJewellLathaConsolidated();
                this.NavigationService.Navigate(sv);
            }

            if (AddSaleShortcutHome == "SaleVoucherEstimationBarcodeJewellConsolidated")
            {
                SaleVoucherEstimationBarcodeJewellConsolidated sv = new SaleVoucherEstimationBarcodeJewellConsolidated();
                this.NavigationService.Navigate(sv);
            } 

            if (AddSaleShortcutHome == "SaleQtyEstimationVoucherSizeA6")
            {
                SaleQtyEstimationVoucherSizeA6 sv = new SaleQtyEstimationVoucherSizeA6();
                this.NavigationService.Navigate(sv);
            }
            if (AddSaleShortcutHome == "SaleVoucher")
            {
                SaleVoucher sv = new SaleVoucher();
                this.NavigationService.Navigate(sv);
            }
            if (AddSaleShortcutHome == "SaleVoucherBarcode")
            {
                SaleVoucherBarcode sv = new SaleVoucherBarcode();
                this.NavigationService.Navigate(sv);
            }
            if (AddSaleShortcutHome == "SaleVoucherEstimationBarcodeJewell")
            {
                SaleVoucherEstimationBarcodeJewell sv = new SaleVoucherEstimationBarcodeJewell();
                this.NavigationService.Navigate(sv);
            }

            if (AddSaleShortcutHome == "SaleVoucherEstimationBarcodeJewellUpdate")
            {
                SaleVoucherEstimationBarcodeJewellUpdate sv = new SaleVoucherEstimationBarcodeJewellUpdate();
                this.NavigationService.Navigate(sv);
            }

            if (AddSaleShortcutHome == "SaleQtyGSTRetailComposition")
            {
                SaleQtyGSTRetailComposition sv = new SaleQtyGSTRetailComposition();
                this.NavigationService.Navigate(sv);
            }
            if (AddSaleShortcutHome == "SaleQtyGSTRetailComposition")
            {
                SaleQtyGSTRetailComposition sv = new SaleQtyGSTRetailComposition();
                this.NavigationService.Navigate(sv);
            }
            if (AddSaleShortcutHome == "SaleVoucherClothKiran")
            {
                SaleVoucherClothKiran sv = new SaleVoucherClothKiran();
                this.NavigationService.Navigate(sv);
            }
            if (AddSaleShortcutHome == "SaleVoucherAllInOneQtyGST")
            {
                SaleVoucherAllInOneQtyGST sv = new SaleVoucherAllInOneQtyGST();
                this.NavigationService.Navigate(sv);
            }
            if (AddSaleShortcutHome == "SaleVoucherAllInOneQtyGSTSteel")
            {
                SaleVoucherAllInOneQtyGSTSteel sv = new SaleVoucherAllInOneQtyGSTSteel("");
                this.NavigationService.Navigate(sv);
            }

            if (AddSaleShortcutHome == "SaleestimationVoucherJewellLatha")
            {
                SaleestimationVoucherJewellLatha sv = new SaleestimationVoucherJewellLatha("");
                this.NavigationService.Navigate(sv);
            }
            if (AddSaleShortcutHome == "SaleVoucherJewellLatha")
            {
                SaleVoucherJewellLatha sv = new SaleVoucherJewellLatha("");
                this.NavigationService.Navigate(sv);
            }

            if (AddSaleShortcutHome == "SaleQtyEstimationVoucherSizeA6NoDisc")
            {
                SaleQtyEstimationVoucherSizeA6NoDisc sv = new SaleQtyEstimationVoucherSizeA6NoDisc();
                this.NavigationService.Navigate(sv);
            }

            if (AddSaleShortcutHome == "SaleVoucherEstimationBarcodeWatch")
            {
                SaleVoucherEstimationBarcodeWatch sv = new SaleVoucherEstimationBarcodeWatch();
                this.NavigationService.Navigate(sv);
            }


            //SaleVoucherClothHitesh sv = new SaleVoucherClothHitesh();
            //SaleVoucherJewellLatha sv = new SaleVoucherJewellLatha();
            //SaleVoucherBarcode sv = new SaleVoucherBarcode();
            //SaleQtyGSTRetailComposition sv = new SaleQtyGSTRetailComposition(); 
            //SaleVoucherClothKiran sv = new SaleVoucherClothKiran();
            //SaleQtyEstimationVoucherSizeA6 sv = new SaleQtyEstimationVoucherSizeA6();
            //SaleVoucherEstimationBarcodeJewell sv = new SaleVoucherEstimationBarcodeJewell();
            //SaleVoucherAllInOneQtyGST sv = new SaleVoucherAllInOneQtyGST();
            //SaleQtyGSTRetailComposition sv = new SaleQtyGSTRetailComposition();
            //this.NavigationService.Navigate(sv);
        }

        private void AddPurchase_Click(object sender, RoutedEventArgs e)
        {
            if (AddPurchaseShortcutHome == "PurchaseVoucher")
            {
                PurchaseVoucher sv = new PurchaseVoucher("");
                this.NavigationService.Navigate(sv);
            }


            if (AddPurchaseShortcutHome == "PurchaseQtyGSTVoucherxaml")
            {
                PurchaseQtyGSTVoucherxaml sv = new PurchaseQtyGSTVoucherxaml("");
                this.NavigationService.Navigate(sv);
            }


            ////PurchaseVoucher sv = new PurchaseVoucher();
            //PurchaseQtyGSTVoucherxaml sv = new PurchaseQtyGSTVoucherxaml();
            //this.NavigationService.Navigate(sv);
        }

        private void AddReceipt_Click(object sender, RoutedEventArgs e)
        {
            if (AddReceiptShortcutHome == "Receipt")
            {
                Receipt sv = new Receipt("");
                this.NavigationService.Navigate(sv);
            }


        }

        private void AddPayment_Click(object sender, RoutedEventArgs e)
        {
            if (AddPaymentShortcutHome == "Payment")
            {
                Payment sv = new Payment("");
                this.NavigationService.Navigate(sv);
            }

            
        }

        private void AddJournal_Click(object sender, RoutedEventArgs e)
        {
            if (AddJournalShortcutHome == "Journal")
            {
                Journal sv = new Journal();
                this.NavigationService.Navigate(sv);
            }

        }

        private void AddStockEntry_Click(object sender, RoutedEventArgs e)
        {
            if (StockEntryShortcutHome == "StockEntry")
            {
                StockEntry sv = new StockEntry();
                this.NavigationService.Navigate(sv);
            }
            if (StockEntryShortcutHome == "StockEntryWatchBarCode")
            {
                StockEntryWatchBarCode sv = new StockEntryWatchBarCode();
                this.NavigationService.Navigate(sv);
            }

            if (StockEntryShortcutHome == "ItemMasterJewell")
            {
                ItemMasterJewell sv = new ItemMasterJewell();
                sv.ShowDialog();
            }

            if (StockEntryShortcutHome == "StockBoxEntryByPurchase")
            {
                StockBoxEntryByPurchase sv = new StockBoxEntryByPurchase();
                sv.ShowDialog();
            }

            if (StockEntryShortcutHome == "StockEntryMain")
            {
                StockEntryMain sv = new StockEntryMain();
                sv.ShowDialog();
            }

            if (StockEntryShortcutHome == "StockEntryTransferBoxTray")
            {
                StockEntryTransferBoxTray sv = new StockEntryTransferBoxTray();
                sv.ShowDialog();
            }
        }

        private void AddStockTransfer_Click(object sender, RoutedEventArgs e)
        {
            StockEntryTransferBoxTray sv = new StockEntryTransferBoxTray();
            sv.ShowDialog();
        }

        private void TakeBackup_Click(object sender, RoutedEventArgs e)
        {
            Backup sv = new Backup();
            sv.ShowDialog();
        }

        private void Rokad_Click(object sender, RoutedEventArgs e)
        {
            Rokad sv = new Rokad();
            sv.ShowDialog();
        }

        private void Pawn_Click(object sender, RoutedEventArgs e)
        {
            SaleVoucherQtyEstimationA5SizePdf sv = new SaleVoucherQtyEstimationA5SizePdf();
            //PawnHome sv = new PawnHome();
            this.NavigationService.Navigate(sv);
        }

        private void OneClick_Click(object sender, RoutedEventArgs e)
        {

            //SaleVoucherEstimationBarcodeJewell sv = new SaleVoucherEstimationBarcodeJewell();
            //SaleQtyGSTRetailComposition sv = new SaleQtyGSTRetailComposition();
            SaleVoucherAllInOneQtyGST sv = new SaleVoucherAllInOneQtyGST();

            //ItemMaster sv = new ItemMaster();
            this.NavigationService.Navigate(sv);
        }
        private void Estimation_Click(object sender, RoutedEventArgs e)
        {
            if (EstimationShortcutHome == "SaleQtyEstimationVoucherSizeItemDesc")
            {
                SaleQtyEstimationVoucherSizeItemDesc sv = new SaleQtyEstimationVoucherSizeItemDesc();
                this.NavigationService.Navigate(sv);
            }

            if (EstimationShortcutHome == "SaleVoucherQtyEstimationA5SizePdf")
            {
                SaleVoucherQtyEstimationA5SizePdf sv = new SaleVoucherQtyEstimationA5SizePdf();
                this.NavigationService.Navigate(sv);
            }


            if (EstimationShortcutHome == "SaleQtyEstimationVoucherSizeA6")
            {
                SaleQtyEstimationVoucherSizeA6 sv = new SaleQtyEstimationVoucherSizeA6();
                this.NavigationService.Navigate(sv);
            }
            if (EstimationShortcutHome == "SaleQtyEstimationVoucher")
            {
                SaleQtyEstimationVoucher sv = new SaleQtyEstimationVoucher();
                this.NavigationService.Navigate(sv);
            }

            if (EstimationShortcutHome == "SaleVoucherEstimationBarcodeJewell")
            {
                SaleVoucherEstimationBarcodeJewell sv = new SaleVoucherEstimationBarcodeJewell();
                this.NavigationService.Navigate(sv);
            }

            if (EstimationShortcutHome == "SaleVoucherEstimationBarcodeJewellUpdate")
            {
                SaleVoucherEstimationBarcodeJewellUpdate sv = new SaleVoucherEstimationBarcodeJewellUpdate();
                this.NavigationService.Navigate(sv);
            }

            

            if (EstimationShortcutHome == "SaleestimationVoucherJewellLatha")
            {
                SaleestimationVoucherJewellLatha sv = new SaleestimationVoucherJewellLatha("");
                this.NavigationService.Navigate(sv);
            }


            if (EstimationShortcutHome == "SaleQtyEstimationVoucherSizeA6NoDisc")
            {
                SaleQtyEstimationVoucherSizeA6NoDisc sv = new SaleQtyEstimationVoucherSizeA6NoDisc();
                this.NavigationService.Navigate(sv);
            }

            if (EstimationShortcutHome == "SaleVoucherEstimationBarcodeWatch")
            {
                SaleVoucherEstimationBarcodeWatch sv = new SaleVoucherEstimationBarcodeWatch();
                this.NavigationService.Navigate(sv);
            }


            //SaleVoucherEstimationBarcodeJewell sv = new SaleVoucherEstimationBarcodeJewell();
            ////SaleQtyGSTRetailComposition sv = new SaleQtyGSTRetailComposition();
            ////SaleestimationVoucherJewellLatha sv = new SaleestimationVoucherJewellLatha();
            ////SaleQtyEstimationVoucherSizeA6 sv = new SaleQtyEstimationVoucherSizeA6();
            ////ItemMaster sv = new ItemMaster();
            //this.NavigationService.Navigate(sv);
        }
        private void EstimationA6_Click(object sender, RoutedEventArgs e)
        {
            if (EstimationA6ShortcutHome == "SaleQtyEstimationVoucherSizeA6")
            {
                SaleQtyEstimationVoucherSizeA6 sv = new SaleQtyEstimationVoucherSizeA6();
                this.NavigationService.Navigate(sv);
            }
            if (EstimationA6ShortcutHome == "SaleQtyEstimationVoucher")
            {
                SaleQtyEstimationVoucher sv = new SaleQtyEstimationVoucher();
                this.NavigationService.Navigate(sv);
            }

            if (EstimationA6ShortcutHome == "SaleVoucherEstimationBarcodeJewell")
            {
                SaleVoucherEstimationBarcodeJewell sv = new SaleVoucherEstimationBarcodeJewell();
                this.NavigationService.Navigate(sv);
            }

            if (EstimationA6ShortcutHome == "SaleVoucherEstimationBarcodeJewellUpdate")
            {
                SaleVoucherEstimationBarcodeJewellUpdate sv = new SaleVoucherEstimationBarcodeJewellUpdate();
                this.NavigationService.Navigate(sv);
            }


            if (EstimationA6ShortcutHome == "SaleestimationVoucherJewellLatha")
            {
                SaleestimationVoucherJewellLatha sv = new SaleestimationVoucherJewellLatha("");
                this.NavigationService.Navigate(sv);
            }


            if (EstimationA6ShortcutHome == "SaleQtyEstimationVoucherSizeA6NoDisc")
            {
                SaleQtyEstimationVoucherSizeA6NoDisc sv = new SaleQtyEstimationVoucherSizeA6NoDisc();
                this.NavigationService.Navigate(sv);
            }

            if (EstimationA6ShortcutHome == "SaleVoucherEstimationBarcodeWatch")
            {
                SaleVoucherEstimationBarcodeWatch sv = new SaleVoucherEstimationBarcodeWatch();
                this.NavigationService.Navigate(sv);
            }

            if (EstimationA6ShortcutHome == "SaleVoucherBarcodeWatch")
            {
                SaleVoucherBarcodeWatch sv = new SaleVoucherBarcodeWatch();
                this.NavigationService.Navigate(sv);
            }

            if (EstimationA6ShortcutHome == "SaleVoucherAllInOneQtyGST")
            {
                SaleVoucherAllInOneQtyGST sv = new SaleVoucherAllInOneQtyGST();
                this.NavigationService.Navigate(sv);
            }


        }
        private void Daybook_Click(object sender, RoutedEventArgs e)
        {
            if (DayBookShortcutHome == "CashPothabaki")
            {
                CashPothabaki sv = new CashPothabaki();
                sv.ShowDialog();
                //this.NavigationService.Navigate(sv);
            }

            //SaleVoucherEstimationBarcodeJewell sv = new SaleVoucherEstimationBarcodeJewell();
            //SaleQtyGSTRetailComposition sv = new SaleQtyGSTRetailComposition();
            //CashPothabaki sv = new CashPothabaki();

            ////ItemMaster sv = new ItemMaster();
            //sv.ShowDialog();
        }

        private void Sheet_Click(object sender, RoutedEventArgs e)
        {
            NavigationWindow navWIN = new NavigationWindow();
            navWIN.Content = new SheetHome();
            navWIN.Show(); 
        }
        private void ShutDown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure want to Close RTS ERP?", "Close Page", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
              
           
            }
        }

        private void AddStockEntry_ClickBox(object sender, RoutedEventArgs e)
        {
            StockBoxEntryByPurchase sv = new StockBoxEntryByPurchase();
            sv.ShowDialog();
        }

    }
}
