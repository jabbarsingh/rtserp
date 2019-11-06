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
    /// Interaction logic for WhatUWant.xaml
    /// </summary>
    public partial class WhatUWant : Window
    {
        public WhatUWant()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {               
                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                myConnSalesInvEntryStr.Open();

                string querySalesInvEntry = "";
                querySalesInvEntry = "insert into RTSTasksList(UserStory, Description,CustomerName)  Values ( '" + Story.Text + "','" + Description.Text + "','" + CustomerName.Text + "')";
                SqlCommand myCommandInvEntry = new SqlCommand(querySalesInvEntry, myConnSalesInvEntryStr);

                //myCommandInvEntry.Connection.Open();
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
            
        }
    }
}
