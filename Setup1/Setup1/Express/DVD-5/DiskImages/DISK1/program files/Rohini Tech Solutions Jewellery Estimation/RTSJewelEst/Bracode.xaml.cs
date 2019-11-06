using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RTSJewelERP
{
    /// <summary>
    /// Interaction logic for Bracode.xaml
    /// </summary>
    public partial class Bracode : Page
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public Bracode()
        {
            InitializeComponent();

            string autobarcodeNumber = "";

            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select number from AutoIncrement where LTRIM(RTRIM(Name)) = 'BarCode'  and CompID = '" + CompID + "'";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = con;
            SqlDataReader reader = cmd.ExecuteReader();

            //tmpProduct = new Product();

            while (reader.Read())
            {
                autobarcodeNumber = reader.GetInt64(0).ToString();

            }
            AutoBarCodeNumber.Text = autobarcodeNumber;
            reader.Close();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {


            iTextSharp.text.pdf.Barcode128 bc = new iTextSharp.text.pdf.Barcode128();
            bc.TextAlignment = Element.ALIGN_CENTER;
            bc.Code = "anything";
            bc.StartStopText = false;
            bc.CodeType = iTextSharp.text.pdf.Barcode128.CODE128;
            bc.Extended = true;
            //bc.Font = null;

            var retangulo = new iTextSharp.text.Rectangle(400, 80);
            //var documento = new Document(retangulo);
            //var writer = PdfWriter.GetInstance(documento, new FileStream(@"C:\output1.pdf", FileMode.Create));
            //FileStream fs = File.Open(@"C:\ViewBill\" + "Bill-" + (invoiceNumber.Text).Trim() + "-" + custName.Text + ".pdf", FileMode.Create);

            string barcodenumber = "";
            if (ScannedCode.Text.Trim() == "")
            {
                barcodenumber = AutoBarCodeNumber.Text;
            }
            else
                barcodenumber = ScannedCode.Text;


            FileStream fs = File.Open(@"C:\BarCode\barcode-" +barcodenumber+".pdf", FileMode.Create);


            Document document = new Document(retangulo);
            //commented below for memort=y stream
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            document.Open();



    //        var imagemDoTopo = iTextSharp.text.Image.GetInstance(@"C:\ViewBill\bc.png");
    //imagemDoTopo.SetAbsolutePosition(0, 5);
    //document.Add(imagemDoTopo);
 
    PdfContentByte cb = writer.DirectContent;
    BaseFont outraFonte = BaseFont.CreateFont(BaseFont.COURIER, BaseFont.CP1252, false, false);
 
    //cb.BeginText();
    //cb.SetFontAndSize(outraFonte, 12);
    //cb.SetColorFill(new BaseColor(51, 51, 51));
    //cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "TESTE", 50, 35, 50);
    //cb.EndText();

    var paramQR = new Dictionary<EncodeHintType, object>();
    paramQR.Add(EncodeHintType.CHARACTER_SET, CharacterSetECI.GetCharacterSetECIByName("UTF-8"));
    BarcodeQRCode qrCodigo = new BarcodeQRCode("<a  />", 10, 10, paramQR);
    iTextSharp.text.Image imgBarCode = qrCodigo.GetImage();
    imgBarCode.SetAbsolutePosition(10, 20);
    document.Add(imgBarCode);

    Barcode128 codeEAN13 = null;
    codeEAN13 = new Barcode128();
    codeEAN13.CodeType = Barcode.CODE128;
    codeEAN13.ChecksumText = true;
    codeEAN13.GenerateChecksum = true;
    codeEAN13.BarHeight = 10;

    if (ScannedCode.Text.Trim() != "")
    {
        codeEAN13.Code = ScannedCode.Text.Trim();
    }
    else
    {
        codeEAN13.Code = AutoBarCodeNumber.Text;
    }
            //check standrad format of barcode
    
    iTextSharp.text.Image imgBarCode1 = codeEAN13.CreateImageWithBarcode(cb, null, null);
    imgBarCode1.SetAbsolutePosition(80, 20);
    imgBarCode1.Alignment = iTextSharp.text.Image.TEXTWRAP;
    document.Add(imgBarCode1);
 
    document.Close();
    if (ScannedCode.Text.Trim() == "")
    {
        SqlConnection consr = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
        //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
        consr.Open();
        string update = "";
        update = "update AutoIncrement  set  Number='" + (Convert.ToInt64(AutoBarCodeNumber.Text) + 1) + "' where Name ='BarCodeStart' and Type='BarCode'   and CompID = '" + CompID + "' ";
        SqlCommand myCommandStkUpdate = new SqlCommand(update, consr);
        //myCommandStkUpdate.Connection.Open();
        myCommandStkUpdate.Connection = consr;
        // myCommandStk.Connection.Open();
        int Num = myCommandStkUpdate.ExecuteNonQuery();

        myCommandStkUpdate.Connection.Close();
    }

    try
    {
        //Open RTSProSoft Folder On PDf button Click
        Process process = new Process();
        process.StartInfo.UseShellExecute = true;
        if (ScannedCode.Text.Trim() == "")
        {
            process.StartInfo.FileName = @"C:\BarCode\barcode-" + AutoBarCodeNumber.Text + ".pdf";
            //process.StartInfo.FileName = @"C:\ViewBill\" + "Bill-" + (invoiceNumber.Text).Trim() + "-" + custName.Text + ".pdf";
            //process.StartInfo.FileName = @"C:\RTSProSoft\";
        }
        else
            process.StartInfo.FileName = @"C:\BarCode\barcode-" + ScannedCode.Text + ".pdf";

        process.Start();
        process.Close();
    }
    catch (Exception ex)
    {
        MessageBox.Show("In Procees. Start");
    }



        }
    }
}
