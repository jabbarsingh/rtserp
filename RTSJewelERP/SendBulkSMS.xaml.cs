using RestSharp;
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
    /// Interaction logic for SendBulkSMS.xaml
    /// </summary>
    public partial class SendBulkSMS : Window
    {
        string MobileList = "";
        public SendBulkSMS()
        {
            InitializeComponent();

            //get comma separated list of all mobile numbers of customer as supplied Main Account type parameter
           
                    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                    //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                    con.Open();
                    //string sql = "select * from AccountsList where CompID = '" + CompID + "'";
                    string sql = "select * from AccountsList where PrimaryAcctName = 'Sundry Debtors'";
                    SqlCommand cmd = new SqlCommand(sql);
                    cmd.Connection = con;
                    SqlDataReader reader = cmd.ExecuteReader();

                    //tmpProduct = new Product();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                           string mob1 = (reader["Mobile1"] != DBNull.Value) ? (reader.GetString(11).Trim()) : "";
                           string mob2 = (reader["Mobile2"] != DBNull.Value) ? (reader.GetString(12).Trim()) : "";

                           string mobno = mob1.Trim(); // +"," + mob2.Trim();

                           if (mobno != "")
                           {
                               if (MobileList.Trim() == "")
                               {
                                   MobileList = mobno;
                               }
                               else
                                   MobileList = MobileList + "," + mobno;
                           }
                            //var acctID = (reader["AcctID"] != DBNull.Value) ? (reader.GetInt64(0)).ToString().Trim() : "";
                            //PrintName.Text = (reader["AcctName"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "";
                            //var PrimaryAcctID = (reader["PrimaryAcctID"] != DBNull.Value) ? (reader.GetInt32(2)).ToString().Trim() : "";
                            //MainAccounts.Text = ((reader["PrimaryAcctName"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "") + "-" + ((reader["PrimaryAcctID"] != DBNull.Value) ? (reader.GetInt32(2)).ToString().Trim() : "");

                            //Address1.Text = (reader["Address1"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                            //Address2.Text = (reader["Address2"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";

                            //City.Text = (reader["City"] != DBNull.Value) ? (reader.GetString(8).Trim()) : "";
                            //State.Text = (reader["State"] != DBNull.Value) ? (reader.GetString(9).Trim()) : "";

                           
                        }
                    }

                    mobLists.Text = MobileList;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

           //Your authentication key
            string authKey = authKeyVal.Text.Trim();// "279284AW3QGxTmmkeI5cf28957"; //279284AW3QGxTmmkeI5cf28957
            //Multiple mobiles numbers separated by comma
            //string mobileNumber = "7506376936";
            string mobileNumber = mobLists.Text;
            //Sender ID,While using route4 sender id should be 6 characters long.
            string senderId = senderID.Text.Trim();  //GSTERP
            //Your message to send, Add URL encoding here.
            string message = smsTexts.Text;


            try
            {

                var client = new RestClient("https://api.msg91.com/api/v2/sendsms?country=91");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authkey", authKey);
                request.AddParameter("application/json", "{ \"sender\": \"" + senderId + "\", \"route\": \"4\", \"country\": \"91\", \"sms\": [ { \"message\": \"" + message + "\", \"to\": [ \"" + mobileNumber + "\" ] } ] }", ParameterType.RequestBody);
              
//{  "sender": "SOCKET",  "route": "4",  "country": "91",  "sms": [    {      "message": "Month End Offer, regards, OM Ji Ambika Jewllery",      "to": [        "7506376936"      ]    }  ]}

        

                
                
                IRestResponse response = client.Execute(request);

            }
            catch (SystemException ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }






        }

    }
}
