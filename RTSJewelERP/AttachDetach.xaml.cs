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
    /// Interaction logic for AttachDetach.xaml
    /// </summary>
    public partial class AttachDetach : Window
    {
        public AttachDetach()
        {
            InitializeComponent();
        }

        private void attach_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection conStrCommon = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conStrCommon.Open();

            //string sql = "SELECT COUNT(*) From AccountsList where AcctName='" + textBoxAcctName.Text.Trim() + "'";
            SqlCommand cmdTaxTable;//= new SqlCommand(sql, con);

            cmdTaxTable = new SqlCommand("SPAttachDetachDatabase", conStrCommon);
            cmdTaxTable.CommandType = CommandType.StoredProcedure;
            cmdTaxTable.Parameters.Add(new SqlParameter("@Flag", "Attach"));

            //cmdTaxTable.Connection.Open();
            int countRecPayTrans =  cmdTaxTable.ExecuteNonQuery();
            if (countRecPayTrans != 0)
            {
                attach.IsEnabled =  false;
                detach.IsEnabled = true;
            }
            cmdTaxTable.Connection.Close();
            //conStrCommon.Close();
        }

        private void detach_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection conStrCommonDet = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conStrCommonDet.Open();

            SqlCommand cmdTaxTable;//= new SqlCommand(sql, con);

            cmdTaxTable = new SqlCommand("SPAttachDetachDatabase", conStrCommonDet);
            cmdTaxTable.CommandType = CommandType.StoredProcedure;
            cmdTaxTable.Parameters.Add(new SqlParameter("@Flag", "Detach"));

            //cmdTaxTable.Connection.Open();
            int countRecPayTrans = cmdTaxTable.ExecuteNonQuery();
            if (countRecPayTrans != 0)
            {
                detach.IsEnabled = false;
                attach.IsEnabled = true;
            }

            
            cmdTaxTable.Connection.Close();
        
        }
    }
}
