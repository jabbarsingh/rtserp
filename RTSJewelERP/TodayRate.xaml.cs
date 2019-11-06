using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace RTSJewelERP
{
    /// <summary>
    /// Interaction logic for TodayRate.xaml
    /// </summary>
    public partial class TodayRate : Window
    {
        public TodayRate()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(HandleEsc); // Esc Key Close Window
            txtgold916RateToday.Focus();

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                string sql = "select * from TodayRate";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();

                //tmpProduct = new Product();

                while (reader.Read())
                {
                    txtgold916RateToday.Text = (reader["Gold"] != DBNull.Value) ? (reader.GetString(0).Trim()) : "";
                    txtgoldRateToday.Text = (reader["GoldSada"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "";
                    txtsilverRateToday.Text = (reader["Silver"] != DBNull.Value) ? (reader.GetString(2).Trim()) : "";
                    txtsilversadaRateToday.Text = (reader["SilverSada"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                    txtOldGold916RateToday.Text = (reader["OldGold"] != DBNull.Value) ? (reader.GetString(4).Trim()) : "";
                    txtOldGoldSadaRateToday.Text = (reader["OldGoldSada"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";
                    txtOldSilverRateToday.Text = (reader["OldSilver"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";


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
                    this.Close();
                    //this.NavigationService.GoBack();
                    //this.NavigationService.RemoveBackEntry();
                }
            }
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


        private void Window_LastKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConfigClass.Gold916Rate = (txtgold916RateToday.Text.Trim() != "") ? (txtgold916RateToday.Text.Trim()) : "0";
                ConfigClass.GoldSadaRate = txtgoldRateToday.Text.Trim() != "" ? (txtgoldRateToday.Text.Trim()) : "0";
                ConfigClass.SilverPureRate = (txtsilverRateToday.Text.Trim() != "") ? (txtsilverRateToday.Text.Trim()) : "0";
                ConfigClass.SilverSadaRate = txtsilversadaRateToday.Text.Trim() != "" ? (txtsilversadaRateToday.Text.Trim()) : "0";
                ConfigClass.OldGoldRate = txtOldGold916RateToday.Text.Trim() != "" ? (txtOldGold916RateToday.Text.Trim()) : "0";
                ConfigClass.OldGoldSadaRate = txtOldGoldSadaRateToday.Text.Trim() != "" ? (txtOldGoldSadaRateToday.Text.Trim()) : "0";
                ConfigClass.OldSilverRate = txtOldSilverRateToday.Text.Trim() != "" ? (txtOldSilverRateToday.Text.Trim()) : "0";

                SqlConnection myConnSalesInvEntryStr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                myConnSalesInvEntryStr.Open();
                    string queryStrStockUpdateC = "";
                    queryStrStockUpdateC = "update TodayRate  set  Gold='" + txtgold916RateToday.Text.Trim() + "', GoldSada='" + txtgoldRateToday.Text.Trim() + "',Silver = '" + txtsilverRateToday.Text.Trim() + "',SilverSada='" + txtsilversadaRateToday.Text.Trim() + "',OldGold='" + txtOldGold916RateToday.Text + "' ,OldGoldSada='" + txtOldGoldSadaRateToday.Text + "',OldSilver='" + txtOldSilverRateToday.Text + "',OldSilverSada='33'";
                    SqlCommand myCommandStkUpdate = new SqlCommand(queryStrStockUpdateC, myConnSalesInvEntryStr);
                    //myCommandStkUpdate.Connection.Open();
                    myCommandStkUpdate.Connection = myConnSalesInvEntryStr;

                        // myCommandStk.Connection.Open();
                        int Num = myCommandStkUpdate.ExecuteNonQuery();
                        if (Num != 0)
                        {
                            //MessageBox.Show("Record Successfully Updated....", "Update Record");
                        }
                        else
                        {
                            MessageBox.Show("Stock is not Updated....", "Update Record Error");
                        }
                       // myCommandStk.Connection.Close();
                   
                    myCommandStkUpdate.Connection.Close();
                




                this.Close();
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

        private void TextBoxHighlight_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                textBox.Background = Brushes.BlueViolet;
                textBox.Foreground = Brushes.White;
            }

            //SolidColorBrush brush = (sender as TextBox).Foreground as SolidColorBrush;
            //if (null != brush)
            //{
            //Brush brush = Brushes.Black;
            //if (brush.IsFrozen)
            //{
            //    brush = brush.Clone();
            //}
            //brush.Opacity = 0.2;

            //Background = Brushes.PaleGoldenrod;

            //}
        }
        private void TextBoxHighlight_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            textBox.Background = Brushes.White;
            textBox.Foreground = Brushes.Black;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }


    }
}
