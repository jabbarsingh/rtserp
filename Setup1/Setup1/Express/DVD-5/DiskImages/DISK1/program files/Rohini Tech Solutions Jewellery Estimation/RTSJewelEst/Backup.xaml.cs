using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
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
    /// Interaction logic for Backup.xaml
    /// </summary>
    public partial class Backup : Window
    {
        public Backup()
        {
            InitializeComponent();
        }
        private void Backup_Click(object sender, RoutedEventArgs e)
        {

            SqlConnection myConnBkp = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            myConnBkp.Open();
            string queryStrBkp = "";
            SqlCommand myCommandBkp = new SqlCommand(queryStrBkp);
            myCommandBkp.Connection = myConnBkp;

            queryStrBkp = "Backup database RTSERPBasic to disk='C:/ViewBill/Database/RTSERPBasic.bak'";

            myCommandBkp = new SqlCommand(queryStrBkp, myConnBkp);


            // myCommandBkp.ExecuteNonQuery();


            int NumBkp = myCommandBkp.ExecuteNonQuery();
            if (NumBkp != 0)
            {
                MessageBox.Show("Database backup is taken , please wait a while....", "Backup");
            }
            else
            {
                MessageBox.Show("Record is not Added....", "Add Record Error");
            }
            myCommandBkp.Connection.Close();


            //Copy the C:\RTSProSoft\Database to the Pendrive Loccation in Date Folder  //Pendrive/26 Aug 2017/RTSProsSoft/Database/
            string sourceFolder = @"C:\ViewBill\";
            string destFolder = "";
            // DateTime now = new DateTime();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType == DriveType.Removable)
                {
                    destFolder = d.Name + @"\" + "RTSERP-" + (DateTime.Now).Day + "-" + (DateTime.Now).Month + "-" + (DateTime.Now).Year + "";
                }
            }
            int chk = CopyFolder(sourceFolder, destFolder);
            if (chk.Equals(1))
            {
                MessageBox.Show("Backup is successfully taken", "Success");
            }
        }
        static public int CopyFolder(string sourceFolder, string destFolder)
        {
            try
            {
                if (!Directory.Exists(destFolder))
                {
                    Directory.CreateDirectory(destFolder);
                }
                else
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(destFolder);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }

                    //Directory.Delete(destFolder);
                    Directory.CreateDirectory(destFolder);
                }
                string[] files = Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = System.IO.Path.GetFileName(file);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    File.Copy(file, dest);
                }
                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = System.IO.Path.GetFileName(folder);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    CopyFolder(folder, dest);
                }
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Backup is not taken successfully, please close window and re-try again....", "Backup Error");
                return 0;
            }

        }

        private void OnlineBackup_Click(object sender, RoutedEventArgs e)
        {
            SqlConnection myConnBkp = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            myConnBkp.Open();
            string queryStrBkp = "";
            SqlCommand myCommandBkp = new SqlCommand(queryStrBkp);
            myCommandBkp.Connection = myConnBkp;

            queryStrBkp = "Backup database RTSERPBasic to disk='C:/ViewBill/Database/RTSERPBasic.bak'";

            myCommandBkp = new SqlCommand(queryStrBkp, myConnBkp);


            // myCommandBkp.ExecuteNonQuery();


            int NumBkp = myCommandBkp.ExecuteNonQuery();
            if (NumBkp != 0)
            {
                MessageBox.Show("Database backup is taken , please wait a while....", "Backup");
            }
            else
            {
                MessageBox.Show("Record is not Added....", "Add Record Error");
            }
            myCommandBkp.Connection.Close();


            //Copy the C:\RTSProSoft\Database to the Pendrive Loccation in Date Folder  //Pendrive/26 Aug 2017/RTSProsSoft/Database/
            string sourceFolder = @"C:\ViewBill\";
            string destFolder = "";
            // DateTime now = new DateTime();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType == DriveType.Removable)
                {
                    destFolder = d.Name + @"\" + "RTSERP-" + (DateTime.Now).Day + "-" + (DateTime.Now).Month + "-" + (DateTime.Now).Year + "";
                }
            }
            int chk = CopyFolder(sourceFolder, destFolder);
            if (chk.Equals(1))
            {
                MessageBox.Show("Backup is successfully taken", "Success");
            }
        }

    }
}
