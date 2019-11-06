using RTSJewelERP.ConfigPageListTableAdapters;
using RTSJewelERP.ConfigTableListTableAdapters;
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
    /// Interaction logic for ConfigurationalWindow.xaml
    /// </summary>
    public partial class ConfigurationalWindow : Window
    {
        public ConfigurationalWindow()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window
            BindComboBoxConfigTable(cmbConfigList);
            BindComboBoxPageList(cmbListPage);

        }

        public void BindComboBoxConfigTable(ComboBox contable)
        {
            var custAdpt = new ConfigTableTableAdapter();
            var custInfoVal = custAdpt.GetData();
            var LinqRes = (from UserRec in custInfoVal
                           orderby UserRec.Name ascending
                           //select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
                           select (UserRec.Name.Trim()));
            cmbConfigList.ItemsSource = LinqRes;
            // comboBoxName.SelectedValueBinding = new Binding("Col6");
        }

        private void cmbConfigList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbConfigList.SelectedItem != null)
            {
                SqlConnection conn2 = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                conn2.Open();
                string selectedConfigname = cmbConfigList.SelectedItem.ToString();
                string sql = "select * from ConfigTable where Name= '" + selectedConfigname + "'";
               
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = conn2;
                SqlDataReader reader = cmd.ExecuteReader();

             
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        IDNo.Text = (reader["ID"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "";
                        ParentName.Text = (reader["Parent"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";

                        GrandParent.Text = (reader["GrandParent"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";

                        cmbListPage.Text = (reader["MapTo"] != DBNull.Value) ? (reader.GetString(4).Trim()) : "";
                        cmbConfigList.Focus();
                    }
                }

                reader.Close();
            }

        }

        public void BindComboBoxPageList(ComboBox pagelist)
        {
            var custAdpt = new ConfigPageListTableAdapter();
            var custInfoVal = custAdpt.GetData();
            //var LinqRes = (from UserRec in custInfoVal
            //               orderby UserRec.GroupName ascending
            //               //select (UserRec.StorageName + "- ID:" + UserRec.StorageID)).Distinct();
            //               select (UserRec.GroupName.Trim())).Distinct();
            //GroupName.ItemsSource = LinqRes;

            cmbListPage.ItemsSource = custInfoVal.Select(x => x.PageWindowName.Trim()).Distinct().ToList();


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

                    //this.NavigationService.GoBack();
                    //this.NavigationService.RemoveBackEntry();
                }
            }


        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {



            //StockItems: CRUD Start
            if (cmbConfigList.SelectedItem != null)
            {
                string selectedConfigname = cmbConfigList.SelectedItem.ToString();
                //SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                myConnSalesInvEntryStr.Open();
                string CountStockItemsEntryStr = "SELECT COUNT(*) From ConfigTable where Name ='" + selectedConfigname + "'";
                SqlCommand myCommand = new SqlCommand(CountStockItemsEntryStr, myConnSalesInvEntryStr);
                myCommand.Connection = myConnSalesInvEntryStr;

                //int countRec = myCommand.ExecuteNonQuery();
                int countRec = (int)myCommand.ExecuteScalar();
                myCommand.Connection.Close();


                if (countRec != 0)
                {

                    string queryStrStockUpdate = "";
                    queryStrStockUpdate = "update ConfigTable  set  Name='" + selectedConfigname + "',ID='" + IDNo.Text + "' ,Parent='" + ParentName.Text + "' ,GrandParent='" + GrandParent.Text + "' ,MapTo='" + cmbListPage.Text + "' where Name='" + selectedConfigname + "'";
                    SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdate, myConnSalesInvEntryStr);
                    myCommandStkUpdate.Connection.Open();
                    myCommandStkUpdate.Connection = myConnSalesInvEntryStr;
                    if (selectedConfigname != "")
                    {
                        // myCommandStk.Connection.Open();
                        int Num = myCommandStkUpdate.ExecuteNonQuery();
                        if (Num != 0)
                        {
                            MessageBox.Show("Record Successfully Updated....", "Update Record");
                           

                        }
                        else
                        {
                            MessageBox.Show("Record is not Updated....", "Update Record Error");
                        }
                        // myCommandStk.Connection.Close();
                    }
                    else
                    {
                        MessageBox.Show("Record can not be updated....", "Update Record Error");
                    }
                    myCommandStkUpdate.Connection.Close();
                }
                else
                {

                    string querySalesInvEntry = "";
                    querySalesInvEntry = "insert into ConfigTable(Name, ID,Parent,GrandParent,MapTo)  Values ( '" + selectedConfigname + "','" + IDNo.Text + "','" + ParentName.Text + "','" + GrandParent.Text + "','" + cmbListPage.Text + "')";
                    SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                    myCommandInvEntry.Connection.Open();
                    int NumPInv = myCommandInvEntry.ExecuteNonQuery();
                    if (NumPInv != 0)
                    {
                        MessageBox.Show("Record Successfully Inserted....", "Insert Record");
                      
                    }
                    else
                    {
                        MessageBox.Show("Record is not Inserted....", "Insert Record Error");
                    }
                    myCommandInvEntry.Connection.Close();

                    // myConnStock.Close();

                }


            }

        }



    }
}
