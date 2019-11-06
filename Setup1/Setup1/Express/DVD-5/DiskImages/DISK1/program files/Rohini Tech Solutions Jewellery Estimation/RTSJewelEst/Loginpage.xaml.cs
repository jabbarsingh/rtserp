using System;
using System.Collections.Generic;
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
    public partial class Loginpage : Window
    {
        public Loginpage()
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
            if (password.Password.ToString() == "0000")
            {
                MainWindow mainwin = new MainWindow();
                mainwin.Show();
                Application.Current.Windows[0].Close();
            }
            else if (password.Password.ToString() == "Colors#1986")
            {
                //AdminWindow adminwin = new AdminWindow();
                //adminwin.Show();
                //Application.Current.Windows[0].Close();
            }
            else
                MessageBox.Show("Please enter correct password");    
        }
    }
}
