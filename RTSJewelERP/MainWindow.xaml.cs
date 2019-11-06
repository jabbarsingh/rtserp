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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RTSJewelERP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select CompanyType from CompanyBusiness where LTRIM(RTRIM(Applicable)) = 1 ";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = con;
            SqlDataReader reader = cmd.ExecuteReader();

            //tmpProduct = new Product();
            string companytype = "";
            while (reader.Read())
            {
                companytype = reader.GetString(0).ToString();

            }
      
            reader.Close();


            if (companytype.Trim() == "Jewellery")
            {
                TodayRate dialogs = new TodayRate();
                dialogs.ShowDialog();
            }
            
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
                    string backupPath ="C:\\ViewBill\\Backup\\";
                    SqlConnection conn2Bkp = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                    conn2Bkp.Open();

                    string sqlBkp = "select top 1 * from BakupSettings  order by LastBackupDate desc";

                    SqlCommand cmdconfigBkp = new SqlCommand(sqlBkp);
                    cmdconfigBkp.Connection = conn2Bkp;
                    SqlDataReader readerConfigBkp = cmdconfigBkp.ExecuteReader(); 


                    //string pathbackup = ""; 

                    if (readerConfigBkp.HasRows)
                    {
                        while (readerConfigBkp.Read())
                        {
                             backupPath = (readerConfigBkp["Path"] != DBNull.Value) ? (readerConfigBkp.GetString(0).Trim()) : "";

                        }
                    }

                    //Take database backup----------------------------------------------


                    SqlConnection myConnSVEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErpMaster"].ConnectionString);
                    myConnSVEntryStr.Open();
                    //pathbackup = @"E:\GST-Backup\SQLBackups\"; 

                    SqlCommand myCommandDeleteDel = new SqlCommand("sp_BackupDatabases", myConnSVEntryStr);
                    myCommandDeleteDel.CommandType = CommandType.StoredProcedure;
                    myCommandDeleteDel.Parameters.Add(new SqlParameter("@backupLocation", backupPath.Trim()));
                    myCommandDeleteDel.Parameters.Add(new SqlParameter("@backupType", "F"));

                    int countRecDelDelDel = myCommandDeleteDel.ExecuteNonQuery();
                    if (countRecDelDelDel != 0)
                    {
                        System.Windows.MessageBox.Show("Backup Successfull....", "Backup Data");
                    }

                    myCommandDeleteDel.Connection.Close();
                    //--------------------------------------------------------------------------




                    this.Close();
                    //this.NavigationService.GoBack();
                    //this.NavigationService.RemoveBackEntry();
                }
            }
        }

        private void NavigationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
                 MessageBoxResult result = MessageBox.Show("Sure you want to close Software?", "Close Page", MessageBoxButton.YesNo);
                 if (result == MessageBoxResult.Yes)
                 {
                     string backupPath = "C:\\ViewBill\\Backup\\";
                     SqlConnection conn2Bkp = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                     //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                     conn2Bkp.Open();

                     string sqlBkp = "select top 1 * from BakupSettings  order by LastBackupDate desc";

                     SqlCommand cmdconfigBkp = new SqlCommand(sqlBkp);
                     cmdconfigBkp.Connection = conn2Bkp;
                     SqlDataReader readerConfigBkp = cmdconfigBkp.ExecuteReader();


                     //string pathbackup = ""; 

                     if (readerConfigBkp.HasRows)
                     {
                         while (readerConfigBkp.Read())
                         {
                             backupPath = (readerConfigBkp["Path"] != DBNull.Value) ? (readerConfigBkp.GetString(0).Trim()) : "";

                         }
                     }

                     //Take database backup----------------------------------------------


                     SqlConnection myConnSVEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErpMaster"].ConnectionString);
                     myConnSVEntryStr.Open();
                     //pathbackup = @"E:\GST-Backup\SQLBackups\"; 

                     SqlCommand myCommandDeleteDel = new SqlCommand("sp_BackupDatabases", myConnSVEntryStr);
                     myCommandDeleteDel.CommandType = CommandType.StoredProcedure;
                     myCommandDeleteDel.Parameters.Add(new SqlParameter("@backupLocation", backupPath.Trim()));
                     myCommandDeleteDel.Parameters.Add(new SqlParameter("@backupType", "F"));

                     int countRecDelDelDel = myCommandDeleteDel.ExecuteNonQuery();
                     if (countRecDelDelDel != 0)
                     {
                         System.Windows.MessageBox.Show("Backup Successfull....", "Backup Data");
                     }

                     myCommandDeleteDel.Connection.Close();
                     //--------------------------------------------------------------------------


                 }


        }
    }
}
