using RTSJewelERP.GroupListTableAdapters;
using RTSJewelERP.MainAccountsTableAdapters;
using RTSJewelERP.StateTableAdapters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AddInstantAccount.xaml
    /// </summary>
    public partial class UpdateItemInstantly : Window
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public UpdateItemInstantly(string srnumber,string itemname, string itembarcode,string group, string issold, string qty, string wt, string price,string gstrate, string compid)
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window

            BindComboBoxGroupName(GroupName);
            lblSrNumber.Content = srnumber;
            txtBarcode.Text = itembarcode;
            autocompleteItemNameStockEntry.Text = itemname.Trim();
            ItemPrice.Text = price;

            GroupName.Text = group;
            //SubGroupName.Text = (reader["UnderSubGroupName"] != DBNull.Value) ? (reader.GetString(33)).ToString().Trim() : "";
            ActualQty.Text = qty;
            ActualWt.Text = wt;
           // HSN.Text = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
            GSTRate.Text = gstrate;
            if (issold == "True")
            {
                isSoldOutChkb.IsChecked = true;
            }
            txtBarcode.Focus();
        }

        private void Barcode_TextChanged(object sender, TextChangedEventArgs e)
        {
            CleanUp();
            //string custnme = txtBarcode.Text.Trim();
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conn.Open();
            string sql = "select * from StockItemsByPc where LTRIM(RTRIM(ItemBarCode)) = '" + txtBarcode.Text.Trim() + "'  and CompID = '" + CompID + "'";
            //string sql = "select * from AccountsMaster where Barcode = '" + txtBarcode.Text.Trim() + "'";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = conn;
            SqlDataReader reader = cmd.ExecuteReader();

            //string constr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\RTSProSoft\Database\InvWpf-Enhanced.accdb;";
            //OleDbConnection con = new OleDbConnection(constr);
            //string queryStr = @"select * from PurchaseInvoices where PartyName = '" + custnme + "'";
            //OleDbCommand command = new OleDbCommand(queryStr, con);
            //con.Open();
            //OleDbDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    autocompleteItemNameStockEntry.Text = (reader["ItemName"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                    //HSN.Text = (reader["HSN"] != DBNull.Value) ? (reader.GetString(18).Trim()) : "";
                    //txtBarcode.Text = (reader["PrintName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                    //UnitID.Text = (reader["UnitID"] != DBNull.Value) ? (reader.GetInt32(4)).ToString().Trim() : "";
                    //ItemCode.Text = (reader["ItemCode"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";


                    //ItemDesc.Text = (reader["ItemDesc"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                    //ItemBarCode.Text = (reader["ItemBarCode"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";
                    ItemPrice.Text = (reader["ItemPrice"] != DBNull.Value) ? (reader.GetDouble(9)).ToString().Trim() : "";

                    GroupName.Text = (reader["UnderGroupName"] != DBNull.Value) ? (reader.GetString(31)).ToString().Trim() : "";
                    SubGroupName.Text = (reader["UnderSubGroupName"] != DBNull.Value) ? (reader.GetString(33)).ToString().Trim() : "";
                    ActualQty.Text = (reader["ActualQty"] != DBNull.Value) ? (reader.GetDouble(35)).ToString().Trim() : "";
                    HSN.Text = (reader["HSN"] != DBNull.Value) ? (reader.GetString(36).Trim()) : "";
                    GSTRate.Text = (reader["GSTRate"] != DBNull.Value) ? (reader.GetInt32(37)).ToString().Trim() : "";


                }
            }
            else
            {
                // MessageBox.Show("Item does not Found", "Not Found Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                // txtBarcode.Clear();
                //autocompleteItemNameStockEntry.Clear();
                //txtBarcode.Focus();

            }


            reader.Close();
        }


        public void BindComboBoxGroupName(ComboBox groupname)
        {
            var custAdpt = new StockGroupsTableAdapter();
            var custInfoVal = custAdpt.GetData();
            //var LinqRes = (from UserRec in custInfoVal
            //               orderby UserRec.GroupName ascending
            //               //select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
            //               select (UserRec.GroupName.Trim())).Distinct();
            //GroupName.ItemsSource = LinqRes;

            GroupName.ItemsSource = custInfoVal.Where(c => (c.ParentGroupName.Trim() == "Main"))
         .Select(x => x.GroupName.Trim()).Distinct().ToList();


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
                //MessageBoxResult result = MessageBox.Show("Are you sure want to Close?", "Close Page", MessageBoxButton.YesNo);
                //if (result == MessageBoxResult.Yes)
                //{

                this.Close();
                //this.NavigationService.RemoveBackEntry();
                //}
            }


        }



        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= TextBox_GotFocus;
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
            autocompleteItemNameStockEntry.Clear();
            ItemPrice.Clear();
            HSN.Clear();

            ActualQty.Clear();
            GSTRate.Clear();

            GroupName.Text = "";
            SubGroupName.Text = "";


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


        private void TextBoxHighlight_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                textBox.Background = Brushes.BlueViolet;
                textBox.Foreground = Brushes.White;
            }

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

        private void GroupName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GroupName.SelectedItem != null)
            {
                string storagenameselected = GroupName.SelectedItem.ToString();
                BindComboBoxSubGroup(storagenameselected);
            }

        }

        public void BindComboBoxSubGroup(string groupname)
        {
            var custAdpt = new StockGroupsTableAdapter();
            var custInfoVal = custAdpt.GetData();
            //var LinqRes = (from UserRec in custInfoVal
            //               orderby UserRec.StorageName ascending
            //               select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
            //StorageName.ItemsSource = LinqRes;


            SubGroupName.ItemsSource = custInfoVal.Where(c => (c.ParentGroupName.Trim() == groupname.Trim()))
                     .Select(x => x.GroupName.Trim()).Distinct().ToList();
            //TrayName.SelectedItem = "Cash";

            // comboBoxName.SelectedValueBinding = new Binding("Col6");
        }


        private void NumberValidationInvoiceTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }



        private void Button_Click_2(object sender, RoutedEventArgs e)
        {


            //if (autocompleteItemNameStockEntry.Text.Trim() == "" || GroupName.SelectedItem == null || txtBarcode.Text.Trim() == "")
            if (autocompleteItemNameStockEntry.Text.Trim() == "" || GroupName.SelectedItem == null )
            {
                MessageBox.Show("Item Name or Item Group is Empty", "Add Item Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                autocompleteItemNameStockEntry.Focus();
            }
            else
            {
                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                myConnSalesInvEntryStr.Open();
                string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPc where SrNumber = '" + lblSrNumber.Content.ToString().Trim() + "' and  LTRIM(RTRIM(ISNULL(ItemBarCode,''))) ='" + txtBarcode.Text.Trim() + "' and CompID = '" + CompID + "'";
                //string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPc where ( ItemName='" + txtBarcode.Text.Trim() + "'  OR  ItemBarCode ='" + txtBarcode.Text.Trim() + "') and CompID = '" + CompID + "'";
                SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
                myCommand.Connection = myConnSalesInvEntryStr;

                //int countRec = myCommand.ExecuteNonQuery();
                int countRec = (int)myCommand.ExecuteScalar();
                myCommand.Connection.Close();

                //double dUnitID = (UnitID.Text.Trim() == "") ? 1 : Convert.ToInt32(UnitID.Text);
                double dItemPrice = (ItemPrice.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemPrice.Text.Trim());
                //double dItemPurchPrice = (ItemPurchPrice.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemPurchPrice.Text);
                //double dItemMRP = (ItemMRP.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemMRP.Text.Trim());
                //double dItemMinSalePrice = (ItemMinSalePrice.Text.Trim() == "") ? 0 : Convert.ToDouble(ItemMinSalePrice.Text);
                double dSetDefaultStorageID = 1;
                //Int32 dDecimalPlaces = (DecimalPlaces.Text.Trim() == "") ? 0 : Convert.ToInt32(DecimalPlaces.Text);
                //double dSaleDiscount = (SaleDiscount.Text.Trim() == "") ? 0 : Convert.ToDouble(SaleDiscount.Text.Trim());
                double dActualQty = (ActualQty.Text.Trim() == "") ? 0 : Convert.ToDouble(ActualQty.Text.Trim());
                double dActualWt = (ActualWt.Text.Trim() == "") ? 0 : Convert.ToDouble(ActualWt.Text.Trim());
                double dGSTRate = (GSTRate.Text.Trim() == "") ? 0 : Convert.ToDouble(GSTRate.Text.Trim());
                bool isSoldFlgs = false;
                if (isSoldOutChkb.IsChecked==true)
                {
                    isSoldFlgs = true;
                }
                Int32 dStorageID = 1;
                Int32 dTrayID = 1;
                Int32 dCounterID = 1;

                //SetCriticalLevel,SetImageUrl,SetDefaultStorageID,SetDefaultSundryCreditor,SetDefaultSundryDebtor,DecimalPlaces,SaleDiscount,ItemPurchPrice,ItemAlias,UnderGroupName,UnderSubGroupName,
                //ActualQty,HSN,GSTRate,StorageID,TrayID,CounterID,OpeningStock,OpeningStockValue,ActualWt,CurrentStockValue,LastSalePrice,LastBuyPrice,OpeningStockWt) Values ( '" + textBoxItemname.Text + "','" + PrintName.Text + "','" + Convert.ToInt32(UnitID.Text) + "','" + ItemCode.Text + "','" + ItemDesc.Text + "','" + ItemBarCode.Text + "','" + ItemMRP.Text + "','" + ItemPrice.Text + "','" + ItemMinSalePrice.Text + "','" + Convert.ToBoolean(SetCriticalLevel.Text) + "','" + SetImageUrl.Text + "','" + Convert.ToInt32(SetDefaultStorageID.Text) + "','" + Convert.ToInt32(SetDefaultSundryCreditor.Text) + "','" + Convert.ToInt32(SetDefaultSundryDebtor.Text) + "','" + Convert.ToInt32(DecimalPlaces.Text) + "','" + Convert.ToDouble(SaleDiscount.Text) + "','" + Convert.ToDouble(ItemPurchPrice.Text) + "','" + ItemAlias.Text + "','" + UnderGroupName.Text + "','" + UnderSubGroupName.Text + "','" + Convert.ToDouble(ActualQty.Text) + "','" + HSN.Text + "','" + Convert.ToDouble(GSTRate.Text) + "','" + Convert.ToInt32(StorageName.Text) + "','" + Convert.ToInt32(TrayName.Text) + "','" + Convert.ToInt32(CounterName.Text) + "','" + Convert.ToDouble(OpeningStock.Text) + "','" + Convert.ToDouble(OpeningStockValue.Text) + "','" + Convert.ToDouble(ActualWt.Text) + "','" + Convert.ToDouble(CurrentStockValue.Text) + "','" + Convert.ToDouble(LastSalePrice.Text) + "','" + Convert.ToDouble(LastBuyPrice.Text) + "','" + Convert.ToDouble(OpeningStockWt.Text) + "')";
                if (countRec != 0)
                {

                    string queryStrStockUpdate = "";
                    queryStrStockUpdate = "update StockItemsByPc  set  ItemName='" + autocompleteItemNameStockEntry.Text.Trim() + "',PrintName='" + autocompleteItemNameStockEntry.Text + "', ItemBarCode ='" + (txtBarcode.Text.Trim()) + "' ,ItemPrice='" + dItemPrice + "' ,UnderGroupName='" + GroupName.Text + "' ,UnderSubGroupName='" + SubGroupName.Text + "' ,ActualQty='" + dActualQty + "',ActualWt='" + dActualWt + "' ,HSN='" + HSN.Text + "' ,GSTRate='" + dGSTRate + "' , IsSoldFlag= '" + isSoldFlgs + "' where SrNumber = '" + lblSrNumber.Content.ToString().Trim() + "' and LTRIM(RTRIM(ISNULL(ItemBarCode,''))) ='" + (txtBarcode.Text.Trim()) + "' and  LTRIM(RTRIM(ItemName)) ='" + autocompleteItemNameStockEntry.Text.Trim() + "'  and CompID = '" + CompID + "' ";


                    SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                    myCommandStkUpdate.Connection.Open();
                    myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                    //if autocompleteItemNameStockEntryy.Text.Trim() != "")
                    //{
                    // myCommandStk.Connection.Open();
                    int Num = myCommandStkUpdate.ExecuteNonQuery();
                    if (Num != 0)
                    {
                        MessageBox.Show("Record Successfully Updated....", "Update Record");
                        this.Close();

                    }
                    else
                    {
                        MessageBox.Show("Stock is not Updated....", "Update Record Error");
                    }

                    myCommandStkUpdate.Connection.Close();


                }
                else
                {
                    //string CountAutoBarCodeItems = "SELECT COUNT(*) From StockItemsByPc where  ItemBarCode ='" + txtBarcode.Text.Trim() + "' and CompID = '" + CompID + "'";
                    ////string CountStockItemsEntryStr = "SELECT COUNT(*) From StockItemsByPc where ( ItemName='" + txtBarcode.Text.Trim() + "'  OR  ItemBarCode ='" + txtBarcode.Text.Trim() + "') and CompID = '" + CompID + "'";
                    //SqlCommand myCommandauto = new SqlCommand(CountAutoBarCodeItems, myConnSalesInvEntryStr);
                    //myCommandauto.Connection.Open();
                    //myCommandauto.Connection = myConnSalesInvEntryStr;

                    ////int countRec = myCommand.ExecuteNonQuery();
                    //int countRecauto = (int)myCommandauto.ExecuteScalar();
                    //myCommandauto.Connection.Close();
                    //if (countRecauto < 1)
                    //{
                    //    string querySalesInvEntry = "";
                    //    //querySalesInvEntry = "insert into StockItems(ItemName, PrintName,ItemDesc,ItemPrice,ItemPurchPrice,UnderGroupName,UnderSubGroupName,ActualQty,HSN,GSTRate,OpeningStock,OpeningStockValue,ActualWt,CurrentStockValue,OpeningStockWt)   Values ( '" + textBoxItemname.Text + "','" + ItemDesc.Text + "','" + ItemBarCode.Text + "','" + ItemPrice.Text + "','" + Convert.ToDouble(ItemPurchPrice.Text) + "','" + UnderGroupName.Text + "','" + UnderSubGroupName.Text + "','" + Convert.ToDouble(ActualQty.Text) + "','" + HSN.Text + "','" + Convert.ToDouble(GSTRate.Text) + "','" + Convert.ToDouble(OpeningStock.Text) + "','" + Convert.ToDouble(OpeningStockValue.Text) + "','" + Convert.ToDouble(ActualWt.Text) + "','" + Convert.ToDouble(CurrentStockValue.Text) + "','" + Convert.ToDouble(OpeningStockWt.Text) + "')";
                    //    querySalesInvEntry = "insert into StockItemsByPc(ItemName, ItemBarCode,ItemPrice,UnderGroupName,UnderSubGroupName,ActualQty,HSN,GSTRate,CompID)  Values ( '" + autocompleteItemNameStockEntry.Text.Trim() + "', '" + txtBarcode.Text.Trim() + "','" + dItemPrice + "','" + GroupName.Text + "','" + SubGroupName.Text + "','" + dActualQty + "','" + HSN.Text + "','" + dGSTRate + "','" + CompID + "')";
                    //    SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                    //    myCommandInvEntry.Connection.Open();
                    //    int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                    //    if (NumPInv != 0)
                    //    {
                    //        MessageBox.Show("Record Successfully Inserted....", "Insert Record");

                    //        this.Close();

                    //    }
                    //    else
                    //    {
                    //        MessageBox.Show("Stock is not Inserted....", "Insert Record Error");
                    //    }
                    //    myCommandInvEntry.Connection.Close();

                    //    // myConnStock.Close();
                    //}
                    //else
                    //{
                    //    MessageBox.Show(" Barccode Item Already There , Please select another barcode number....", "Auto Barcode Insert Error");
                    //}
                }

            }

        }


    }
}
