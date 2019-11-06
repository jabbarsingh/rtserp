using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
    /// Interaction logic for FactoryResetDatabase.xaml
    /// </summary>
    public partial class ResetStock : Window
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;

        public ResetStock()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            con.Open();
            SqlCommand cmd;//= new SqlCommand(sql, con);

            cmd = new SqlCommand("SPResetRefreshStockAndAccounts", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@CompID", CompID));

            //con.Open();
            //cmd.ExecuteNonQuery();
            int countRecPay = cmd.ExecuteNonQuery();
            if (countRecPay != 0)
            {
                MessageBox.Show("Success....", "Added Record");
            }

        }
    }
}
