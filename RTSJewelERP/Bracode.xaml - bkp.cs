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
            bc.TextAlignment = Element.ALIGN_RIGHT;
            //bc.Code = "anything";
            bc.StartStopText = false;
            bc.CodeType = iTextSharp.text.pdf.Barcode128.CODE128;
            //bc.Extended = true;
            //bc.Font = null;

            var retangulo = new iTextSharp.text.Rectangle(100, 50);
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

            cb.BeginText();
            cb.SetFontAndSize(outraFonte, 25);
            cb.SetColorFill(new BaseColor(51, 51, 51));
            //cb.ShowTextAligned(PdfContentByte.ALIGN_LEFT, "TESTE", 50, 35, 50);
            cb.EndText();

    //var paramQR = new Dictionary<EncodeHintType, object>();
    //paramQR.Add(EncodeHintType.CHARACTER_SET, CharacterSetECI.GetCharacterSetECIByName("UTF-8"));
    //BarcodeQRCode qrCodigo = new BarcodeQRCode("<a  />", 10, 10, paramQR);
    //iTextSharp.text.Image imgBarCode = qrCodigo.GetImage();
    //imgBarCode.SetAbsolutePosition(5, 5);
    //document.Add(imgBarCode);

    Barcode128 codeEAN13 = null;
    codeEAN13 = new Barcode128();
    codeEAN13.CodeType = Barcode.CODE128;
    //codeEAN13.ChecksumText = true;
    //codeEAN13.GenerateChecksum = true;
    codeEAN13.BarHeight = 20;  //Set this Barcode height
    //codeEAN13.AltText = "";
      
    
            //////////////

            //PDf , Fit Screen, Portrait
            // Go to Printer Settings 
            //SetBinding Width  = 95, height 10




            ///////////

    if (ScannedCode.Text.Trim() != "")
    {
        codeEAN13.Code = ScannedCode.Text.Trim();
    }
    else
    {
        codeEAN13.Code = AutoBarCodeNumber.Text.Trim();
    }
            //check standrad format of barcode
   
    iTextSharp.text.Image imgBarCode1 = codeEAN13.CreateImageWithBarcode(cb, null, null);
    imgBarCode1.SetAbsolutePosition(15, 5);
    imgBarCode1.Alignment = iTextSharp.text.Image.ALIGN_RIGHT;
    

    PdfPTable barcodeTable = new iTextSharp.text.pdf.PdfPTable(2);
    //float[] widthsTotalTableHzl = new float[] { 50, 50 };
    barcodeTable.DefaultCell.Border = 0;
  

    //barcodeTable.SetWidths(widthsTotalTableHzl);
    barcodeTable.WidthPercentage = 100;
    //barcodeTable.TotalWidth = 100;
          
    //PdfPCell barcodeTableCell = new PdfPCell();
    //ourbankdetails1Cell.Border = 0;
    //barcodeTable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
    
    ////barcodeTableCell.AddElement(imgBarCode1);
    ////barcodeTableCell.AddElement(new Phrase("A/C#:"));
    ////barcodeTableCell.AddElement(new Phrase("2"));
    ////barcodeTableCell.AddElement(new Phrase("IFSC:"));
    ////barcodeTableCell.AddElement(new Phrase(firmBankAddress.Trim(), taxslabAmtFont));
    //barcodeTableCell.HorizontalAlignment = Element.ALIGN_LEFT;
    //barcodeTable.AddCell("");
    //barcodeTable.AddCell("wt");
    //barcodeTable.AddCell("quality");
    //ourbankdetails1.DefaultCell.Rowspan = 2;
    //barcodeTable.DefaultCell.BorderWidthRight = 0;
    //barcodeTable.DefaultCell.BorderWidthBottom = 0;
    //barcodeTable.DefaultCell.HorizontalAlignment = Element.ALIGN_LEFT;
    //PdfPCell ourbankdetails1cell = new PdfPCell();
    //document.Add(barcodeTable);
             Font barfont = new Font(Font.FontFamily.TIMES_ROMAN, 12f, Font.NORMAL, BaseColor.BLACK);
    barcodeTable.SplitLate = false;
    barcodeTable.SplitRows = true;
    barcodeTable.AddCell(new Phrase("A/C#:", barfont));
    PdfPCell ForFirmCell = new PdfPCell();
    ForFirmCell.AddElement(imgBarCode1); //imgBarCode1
    barcodeTable.AddCell(ForFirmCell);
    //ForFirmCell.MinimumHeight = 40f;
    document.Add(barcodeTable);

    //Font barfont = new Font(Font.FontFamily.TIMES_ROMAN, 4f, Font.NORMAL, BaseColor.BLACK);

    Phrase size = new Phrase("12*45", barfont);

    //document.Add(imgBarCode1);
    //iTextSharp.text.Paragraph wt = new iTextSharp.text.Paragraph("4.7gm") { Height:12,Width:10};

            
    //document.Add(size);
    //Phrase quality2 = new Phrase("916");
    //document.Add(quality2);             
 
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
