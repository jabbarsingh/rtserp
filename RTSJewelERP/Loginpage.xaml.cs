using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
using System.Windows.Forms;
using Microsoft.Win32;
using WinForms = System.Windows.Forms;

namespace RTSJewelERP
{
    /// <summary>
    /// Interaction logic for Loginpage.xaml
    /// </summary>
    public partial class Loginpage : Window
    {
        public Loginpage()
        {
            InitializeComponent();
            password.Focus();

            SqlConnection conbackup = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            conbackup.Open();
            string sqlbackup = "select * from BakupSettings";
            SqlCommand cmdBackup = new SqlCommand(sqlbackup);
            cmdBackup.Connection = conbackup;
            SqlDataReader readerBackup = cmdBackup.ExecuteReader();
            while (readerBackup.Read())
            {
                backupPath.Text = (readerBackup["Path"] != DBNull.Value) ? (readerBackup.GetString(0).Trim()) : "";

                string lastBackupDate = readerBackup.GetDateTime(1).ToString();

            }
            readerBackup.Close();




            //var myBrush = new ImageBrush();
            //var image = new Image
            //{
            //    Source = new BitmapImage(
            //        new Uri(
            //            @"../Sale.png"))
            //};
            //myBrush.ImageSource = image.Source;
            //Background = myBrush;


        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EnterLogin();
        }

        private void password_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                EnterLogin();
        }
        private void EnterLogin()
        {
            SqlConnection conn2 = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conn2.Open();

            string sql = "select top 1 * from Users where UserPassword='" + password.Password.ToString().Trim() + "'";

            SqlCommand cmdconfig = new SqlCommand(sql);
            cmdconfig.Connection = conn2;
            SqlDataReader readerConfig = cmdconfig.ExecuteReader();


            if (readerConfig.HasRows)
            {
                while (readerConfig.Read())
                {
                    string nameconf = (readerConfig["UserName"] != DBNull.Value) ? (readerConfig.GetString(2).Trim()) : "";
                    string parentconf = (readerConfig["UserID"] != DBNull.Value) ? (readerConfig.GetString(3).Trim()) : "";
                    string grandparentconf = (readerConfig["UserPassword"] != DBNull.Value) ? (readerConfig.GetString(4).Trim()) : "";


                    MainWindow mainwin = new MainWindow();
                    mainwin.Show();
                    System.Windows.Application.Current.Windows[0].Close();

                }
            }


            //if (password.Password.ToString() == "0000")
            //{


            //    MainWindow mainwin = new MainWindow();
            //    mainwin.Show();
            //    Application.Current.Windows[0].Close();
            //}
            //else if (password.Password.ToString() == "Colors#1986")
            //{
            //    //AdminWindow adminwin = new AdminWindow();
            //    //adminwin.Show();
            //    //Application.Current.Windows[0].Close();
            //}
            else
                System.Windows.MessageBox.Show("Please enter correct PIN");    
        }

        private void password_PasswordChanged(object sender, RoutedEventArgs e)
       {
            SqlConnection conn2 = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            conn2.Open();

            string sql = "select top 1 * from Users where UserPassword='" + password.Password.ToString().Trim() + "'";

            SqlCommand cmdconfig = new SqlCommand(sql);
            cmdconfig.Connection = conn2;
            SqlDataReader readerConfig = cmdconfig.ExecuteReader();

            //string pathbackup = ""; 

            if (readerConfig.HasRows)
            {
                while (readerConfig.Read())
                {
                    string nameconf = (readerConfig["UserName"] != DBNull.Value) ? (readerConfig.GetString(2).Trim()) : "";
                    string parentconf = (readerConfig["UserID"] != DBNull.Value) ? (readerConfig.GetString(3).Trim()) : "";
                    string grandparentconf = (readerConfig["UserPassword"] != DBNull.Value) ? (readerConfig.GetString(4).Trim()) : "";
                   





                    //Take database backup----------------------------------------------
    

                    SqlConnection myConnSVEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErpMaster"].ConnectionString);
                    myConnSVEntryStr.Open();
                     //pathbackup = @"E:\GST-Backup\SQLBackups\"; 
                    
                        SqlCommand myCommandDeleteDel = new SqlCommand("sp_BackupDatabases", myConnSVEntryStr);
                        myCommandDeleteDel.CommandType = CommandType.StoredProcedure;
                        myCommandDeleteDel.Parameters.Add(new SqlParameter("@backupLocation", backupPath.Text.Trim()));
                        myCommandDeleteDel.Parameters.Add(new SqlParameter("@backupType", "F"));                       
              
                        int countRecDelDelDel = myCommandDeleteDel.ExecuteNonQuery();
                        if (countRecDelDelDel != 0)
                        {
                              System.Windows.MessageBox.Show("Backup Successfull....", "Backup Data");
                        }

                        myCommandDeleteDel.Connection.Close();                    
                        //--------------------------------------------------------------------------





                    MainWindow mainwin = new MainWindow();
                    mainwin.Show();
                    System.Windows.Application.Current.Windows[0].Close();

                }
            }
          
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.SelectedPath;
                backupPath.Text = path + "\\";
               //System.Windows.MessageBox.Show(path);
            }

 




            //Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            //dlg.InitialDirectory = "C:\\";
            //// Set filter for file extension and default file extension 
            ////dlg.DefaultExt = ".txt";
            ////// dlg.Filter = "Text documents (.txt)|*.txt";
            ////dlg.Filter = "EXCEL Files (*.xls)|*.xlsx";

            //// Display OpenFileDialog by calling ShowDialog method 
            //Nullable<bool> result = dlg.ShowDialog();

          
        
          

            //// Get the selected file name and display in a TextBox 
            //if (result == true)
            //{
            //    // Open document 
            //    string filename = dlg.FileName;
            //    backupPath.Text = filename;
            //}




        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            
            string pathname = backupPath.Text.Trim();
            //Take database backup----------------------------------------------
            SqlConnection conbackup = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            conbackup.Open();
            string sqlbackup = "update BakupSettings  set Path='" + pathname + "', IsSuccess='1'";
            SqlCommand cmdBackup = new SqlCommand(sqlbackup);
            cmdBackup.Connection = conbackup;

            int Num = cmdBackup.ExecuteNonQuery();
            if (Num != 0)
            {
                System.Windows.MessageBox.Show("Backup Path Successfully Set....", "Update Record");
            }


            cmdBackup.Connection.Close();
        }
    }
}
