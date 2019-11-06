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
    /// Interaction logic for Loginpage.xaml
    /// </summary>
    public partial class ProtectedWindow : Window
    {
        public ProtectedWindow()
        {
            InitializeComponent();
            password.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            EnterLogin();
        }

        private void password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                EnterLogin();
        }
        private void EnterLogin()
        {
            string todayDt = DateTime.Now.ToString("d/M");
            todayDt = todayDt.Replace("-", "");
            if (password.Password.ToString().Trim() == todayDt)
            {
                this.Close();
                CashPothabakiConsolidated sv = new CashPothabakiConsolidated();
                //this.NavigationService.Navigate(sv);
                sv.ShowDialog();
                
            }
            else 
            {
                //MessageBox.Show("Incorrect Attempt");
            }

        }

        private void password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            string todayDt = DateTime.Now.ToString("d/M");
            todayDt = todayDt.Replace("-", "");
            if (password.Password.ToString().Trim() == todayDt)
            {
                this.Close();
                CashPothabakiConsolidated sv = new CashPothabakiConsolidated();
                //this.NavigationService.Navigate(sv);
                sv.ShowDialog();
               
            }
            else
            {
                //MessageBox.Show("Incorrect Attempt");
            }

        }
    }
}
