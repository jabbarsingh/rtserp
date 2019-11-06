using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
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
    /// Interaction logic for LetterPad.xaml
    /// </summary>

    public partial class LetterPad : Window
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public LetterPad()
        {
            InitializeComponent();
        }
        public void ExportToPdf(object sender, RoutedEventArgs e)
        {

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select  * from Company where CompanyID =  " + CompID + "";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = con;
            SqlDataReader reader = cmd.ExecuteReader();

            //tmpProduct = new Product();
            string CompanyName = "";
            string GSTIN = "";
            string Address = "";
            string Address2 = "";
            string City = "";
            string State = "";
            string Mob = "";
            string Phone = "";
            string Email = "";
            string Web = "";
            string Branches = "";
            string LogoUrl = "";
            string SubTitle = "";
            string BankName = "";
            string BAddress = "";
            string IFSC = "";
            string AccNumber = "";
            string Holder = "";
            string PinCode = "";
            while (reader.Read())
            {

                //var CustID = reader.GetValue(0).ToString();
                CompanyName = (reader["CompanyName"] != DBNull.Value) ? (reader.GetString(1).Trim()) : "";
                GSTIN = (reader["GSTIN"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                Address = (reader["Address1"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";
                Address2 = (reader["Address2"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                City = (reader["City"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";

                State = (reader["State"] != DBNull.Value) ? (reader.GetString(8).Trim()) : "";
                PinCode = (reader["PINCode"] != DBNull.Value) ? (reader.GetString(9).Trim()) : "";
                Mob = (reader["Mobile1"] != DBNull.Value) ? (reader.GetString(10).Trim()) : "";
                Phone = (reader["Phone"] != DBNull.Value) ? (reader.GetString(12).Trim()) : "";

                Email = (reader["Email"] != DBNull.Value) ? (reader.GetString(13).Trim()) : "";
                //FinYeraStartDate  = (reader["FinYearStartDate"] != DBNull.Value) ? (reader.GetString(17).Trim()) : "";
                //BookStartDate  = (reader["BookStartDate"] != DBNull.Value) ? (reader.GetString(18).Trim()) : "";
                Web = (reader["Website"] != DBNull.Value) ? (reader.GetString(15).Trim()) : "";
                Branches = (reader["NumberOfBranches"] != DBNull.Value) ? (reader.GetInt32(16)).ToString() : "";
                LogoUrl = (reader["LogoPath"] != DBNull.Value) ? (reader.GetString(26).Trim()) : "";
                SubTitle = (reader["SubTitle"] != DBNull.Value) ? (reader.GetString(25).Trim()) : "";

                BankName = (reader["BankName"] != DBNull.Value) ? (reader.GetString(20).Trim()) : "";
                BAddress = (reader["BAddress"] != DBNull.Value) ? (reader.GetString(21).Trim()) : "";
                IFSC = (reader["IFSC"] != DBNull.Value) ? (reader.GetString(22).Trim()) : "";
                AccNumber = (reader["AccNumber"] != DBNull.Value) ? (reader.GetString(23).Trim()) : "";
                Holder = (reader["Holder"] != DBNull.Value) ? (reader.GetString(24).Trim()) : "";


            }
            reader.Close();




            //add background image 
            string imageFilePath = @"c:\RTSProSoft\Database\Logo1.jpg";
            iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageFilePath);
            //Resize image depend upon your need
            //For give the size to image
            jpg.ScaleToFit(50, 50);

            //If you want to choose image as background then,

            jpg.Alignment = iTextSharp.text.Image.UNDERLYING;
            //If you want to give absolute/specified fix position to image.
            jpg.SetAbsolutePosition(17, 540); // to set the logo at left top 


            string imageFilePathLogo2 = @"c:\RTSProSoft\Database\Logo2.jpg";
            iTextSharp.text.Image jpg2 = iTextSharp.text.Image.GetInstance(imageFilePathLogo2);
            //Resize image depend upon your need
            //For give the size to image
            jpg2.ScaleToFit(50, 50);

            //If you want to choose image as background then,

            jpg2.Alignment = iTextSharp.text.Image.UNDERLYING;
            //If you want to give absolute/specified fix position to image.
            jpg2.SetAbsolutePosition(340, 535); // to set the logo at left top 

            //string nowdt = DateTime.Now.ToString();
            //nowdt = nowdt.Replace(":", "").Replace(" ", "").Replace("-", "");

            //FileStream fs = File.Open(@"C:\ViewBill\LetterPad-" + nowdt + ".pdf", FileMode.Create);
            FileStream fs = File.Open(@"C:\ViewBill\LetterPad.pdf", FileMode.Create);
            Document document = new Document(iTextSharp.text.PageSize.A5, 15, 15, 2, 2);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);


            //float sethght = document.PageSize.Height;

            document.Open();

            Font headerFONT = new Font(Font.FontFamily.HELVETICA, 14f, Font.BOLD, BaseColor.BLACK);
            Font allFONTsize = new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.NORMAL, BaseColor.BLACK);
            Font tinfont = new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.NORMAL, BaseColor.BLACK);

            Font dateInv = new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.BOLD, BaseColor.BLACK);

            Font gstInv = new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.BOLDITALIC, BaseColor.BLACK);


            PdfPTable firmtitletableLP = new iTextSharp.text.pdf.PdfPTable(1) {TotalWidth = 390, LockedWidth = true };
            //headerTable.WidthPercentage = 100;
            firmtitletableLP.DefaultCell.Border = 0;
            firmtitletableLP.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            firmtitletableLP.DefaultCell.BorderWidthBottom = 1;
            //PdfPCell FirmTitCell = new PdfPCell();

            iTextSharp.text.Paragraph pltt = new iTextSharp.text.Paragraph();


            Phrase pfirm1 = new Phrase(CompanyName + "\n", headerFONT);
            pltt.Add(pfirm1);

            if (SubTitle.Trim() != "")
            {
                Phrase pfirm5 = new Phrase(SubTitle + "\n", tinfont);
                pltt.Add(pfirm5);
            }

            Phrase pfirm2 = new Phrase(Address +" " +Address2 + "\n" + City + " " + State + " " + PinCode + "\n", allFONTsize);
            pltt.Add(pfirm2);

            //if (firmGodownAdd.Trim() != "")
            //{
            //    Phrase pfirmgd = new Phrase(firmGodownAdd + "\n", allFONTsize);
            //    pltt.Add(pfirmgd);
            //}

            if (Email.Trim() != "")
            {
                Phrase pfirm3 = new Phrase("Email: " + Email + "\n", allFONTsize);
                pltt.Add(pfirm3);
            }
            if (Web.Trim() != "")
            {
                Phrase pfirm3web = new Phrase("Web: " + Web + "\n", allFONTsize);
                pltt.Add(pfirm3web);
            }
            Phrase pPhonenumber = new Phrase("Contact: " + Mob + "  " + Phone + "\n", allFONTsize);
            pltt.Add(pPhonenumber);

            Phrase pfirm5gstin = new Phrase("GSTIN: " + GSTIN + "\n", dateInv);
            pltt.Add(pfirm5gstin);

            pltt.Alignment = Element.ALIGN_CENTER;
            //FirmTitCell.HorizontalAlignment = Element.ALIGN_CENTER;


            firmtitletableLP.AddCell(pltt);

            document.Add(jpg);
            document.Add(jpg2);

            document.Add(firmtitletableLP);
            document.Add(new Phrase(BillDate.Text + "\n", dateInv));
            document.Add(new Phrase("\n" + tbMultiLine.Text, allFONTsize));
            document.Close();

            writer.Close();
            fs.Close();

            try
            {
                //Open RTSProSoft Folder On PDf button Click
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                //process.StartInfo.FileName = @"C:\ViewBill\LetterPad-" + nowdt + ".pdf"; 
                process.StartInfo.FileName = @"C:\ViewBill\LetterPad.pdf";
                //C:\ViewBill\LetterPad.pdf
                //process.StartInfo.FileName = @"C:\RTSProSoft\";

                process.Start();
                process.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("In Procees. Start");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}