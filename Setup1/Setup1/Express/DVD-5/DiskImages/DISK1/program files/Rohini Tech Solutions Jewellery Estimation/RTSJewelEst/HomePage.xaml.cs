
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
namespace RTSJewelERP
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            BindComboBox(CompName);
            SaleInvoiceImg.Focus();

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
                query = "Select top 3 (SELECT MONTH(TransactionDate)),Sum(CAST(InvoiceAmt AS float)) AS TotalInvValue from SalesVouchers where  (SELECT MONTH(TransactionDate) ) > (SELECT MONTH(TransactionDate) - 3)  and CompID = '1'  group by Month(TransactionDate)  order by Month(TransactionDate) desc";

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
                queryPur = "Select top 3 (SELECT MONTH(TransactionDate)),Sum(CAST(InvoiceAmt AS float)) AS TotalInvValue from PurchaseVouchers where  (SELECT MONTH(TransactionDate) ) > (SELECT MONTH(TransactionDate) - 3)  group by Month(TransactionDate)  order by Month(TransactionDate) desc";

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


        }

        public void BindComboBox(ComboBox company)
        {
            var custAdpt = new CompanyTableAdapter();
            var custInfoVal = custAdpt.GetData();
            var LinqRes = (from UserRec in custInfoVal
                           orderby UserRec.CompSrNumber ascending
                           select (UserRec.CompanyName.Trim() + "-" + UserRec.CompanyID)).Distinct();
            CompName.ItemsSource = LinqRes;
            // comboBoxName.SelectedValueBinding = new Binding("Col6");
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


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SaleVoucher sv = new SaleVoucher();
            this.NavigationService.Navigate(sv);
        }

        private void Sale_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SaleVoucher sv = new SaleVoucher();
            this.NavigationService.Navigate(sv);
        }

        private void Purchase_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PurchaseVoucher sv = new PurchaseVoucher();
            //NavigationWindow navWIN = new NavigationWindow();
            //navWIN.Content = new PurchaseVoucher();
            //navWIN.Show();
            this.NavigationService.Navigate(sv);
        }

        private void Receipt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Receipt sv = new Receipt();
            this.NavigationService.Navigate(sv);
        }

        private void Pay_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Payment sv = new Payment();
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
            ItemMaster sv = new ItemMaster();
            this.NavigationService.Navigate(sv);
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
            //SaleVoucher sv = new SaleVoucher();
            //this.NavigationService.Navigate(sv);
            NavigationWindow navWIN = new NavigationWindow();
            navWIN.Content = new SaleVoucher();
            navWIN.Show(); 
        }


        private void ListBoxPurchase_Selected(object sender, RoutedEventArgs e)
        {
            //PurchaseVoucher sv = new PurchaseVoucher();
            //this.NavigationService.Navigate(sv);
            NavigationWindow navWIN = new NavigationWindow();
            navWIN.Content = new PurchaseVoucher();
            navWIN.Show(); 
        }


        private void ListBoxReceipt_Selected(object sender, RoutedEventArgs e)
        {

            //Receipt sv = new Receipt();
            //this.NavigationService.Navigate(sv);

            NavigationWindow navWIN = new NavigationWindow();
            navWIN.Content = new Receipt();
            navWIN.Show(); 
        }


        private void ListBoxPayment_Selected(object sender, RoutedEventArgs e)
        {
            //Payment sv = new Payment();
            //this.NavigationService.Navigate(sv);
            NavigationWindow navWIN = new NavigationWindow();
            navWIN.Content = new Payment();
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
            //SaleVoucher sv = new SaleVoucher();
            //this.NavigationService.Navigate(sv);

            NavigationWindow navWIN = new NavigationWindow();
            navWIN.Content = new SaleVoucher();
            navWIN.Show(); 

        }

        private void MenuItem_NewPurchaseClick(object sender, RoutedEventArgs e)
        {
            PurchaseVoucher sv = new PurchaseVoucher();
            this.NavigationService.Navigate(sv);
        }

        private void MenuItem_NewPaymentClick(object sender, RoutedEventArgs e)
        {
            Payment sv = new Payment();
            this.NavigationService.Navigate(sv);
        }

        private void MenuItem_NewReceiptClick(object sender, RoutedEventArgs e)
        {
            Receipt sv = new Receipt();
            this.NavigationService.Navigate(sv);
        }

        private void MenuItem_NewJournalClick(object sender, RoutedEventArgs e)
        {
            Receipt sv = new Receipt();
            this.NavigationService.Navigate(sv);
        }


        private void NewItem_Click(object sender, RoutedEventArgs e)
        {
            ItemMaster sv = new ItemMaster();
            this.NavigationService.Navigate(sv);
        }

        private void NewAccount_Click(object sender, RoutedEventArgs e)
        {
            AccountMaster sv = new AccountMaster();
            this.NavigationService.Navigate(sv);
        }

        private void CompName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConfigClass.CompID = "1";
            
            var companyname = CompName.SelectedItem.ToString();
            //ConfigClass.CompID = ((ComboBoxItem)CompName.SelectedItem).ToString();
            string companyid = companyname.Split('-')[1];
            //ConfigClass.CompID = CompName.Text;
            ConfigClass.CompID = companyid;
        }


        
    }
}
