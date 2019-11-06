using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Globalization;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;


namespace RTSJewelERP
{
    public class ITextEvents : PdfPageEventHelper
    {

        string CompID = RTSJewelERP.ConfigClass.CompID; 
        public string custName
        {
            get;
            set;
        }

        public string CashCustName
        {
            get;
            set;
        } 

            
        public string cashCredit
        {
            get;
            set;
        } 

        public string BillDate
        {
            get;
            set;
        }
        public string invoiceNumber
        {
            get;
            set;
        }

        public string SelectedValueDelivery
        {
            get;
            set;
        }

        public string selecteValueParcels
        {
            get;
            set;
        } 
        

        public string transportName
        {
            get;
            set;
        }

        public string printName
        {
            get;
            set;
        }

        public string addressCust
        {
            get;
            set;
        }
        public string mobCust
        {
            get;
            set;
        }
                    
        public string YourOrder
        {
            get;
            set;
        } 
        public string GSTIN
        {
            get;
            set;
        }
        public string State
        {
            get;
            set;
        }


        public string StateCode
        {
            get;
            set;
        }

        public string PONumber
        {
            get;
            set;
        }

        public string ShippingAddress
        {
            get;
            set;
        } 

        public string EwayNumber
        {
            get;
            set;
        }

        public string GSTINCompany
        {
            get;
            set;
        }

        // This is the contentbyte object of the writer
        PdfContentByte cb;

        // we will put the final number of pages in a template
        PdfTemplate headerTemplate, footerTemplate;

        // this is the BaseFont we are going to use for the header / footer
        BaseFont bf = null;

        // This keeps track of the creation time
        DateTime PrintTime = DateTime.Now;


        #region Fields
        private string _header;
        #endregion

        #region Properties
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        #endregion


        public override void OnOpenDocument(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            try
            {
                PrintTime = DateTime.Now;
                bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb = writer.DirectContent;
                headerTemplate = cb.CreateTemplate(100, 120);
                footerTemplate = cb.CreateTemplate(50, 50);
            }
            catch (DocumentException de)
            {

            }
            catch (System.IO.IOException ioe)
            {

            }
        }

        public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document document)
        {
            base.OnEndPage(writer, document);


            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
            //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
            con.Open();
            string sql = "select  * from Company where CompanyID =  " + CompID + "";
            SqlCommand cmd = new SqlCommand(sql);
            cmd.Connection = con;
            SqlDataReader reader = cmd.ExecuteReader();

            //tmpProduct = new Product();
            string CompanyName = "";
            //string GSTIN = "";
            string Address = "";
            string Address2 = "";
            string City = "";
            string StateComp = "";
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
                GSTINCompany = (reader["GSTIN"] != DBNull.Value) ? (reader.GetString(3).Trim()) : "";
                Address = (reader["Address1"] != DBNull.Value) ? (reader.GetString(5).Trim()) : "";
                Address2 = (reader["Address2"] != DBNull.Value) ? (reader.GetString(6).Trim()) : "";
                City = (reader["City"] != DBNull.Value) ? (reader.GetString(7).Trim()) : "";

                StateComp = (reader["State"] != DBNull.Value) ? (reader.GetString(8).Trim()) : "";
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

            ///
            
            PdfPTable pdfTabTitleFirm = new PdfPTable(1) { TotalWidth = 390, LockedWidth = true }; ;
            pdfTabTitleFirm.DefaultCell.Border = 0;

            iTextSharp.text.Font baseFontNormal = new iTextSharp.text.Font();

            iTextSharp.text.Font baseFontBig = new iTextSharp.text.Font();

            //Phrase p1Header = new Phrase("Sample Header Here " + custName+"", baseFontNormal);

            Font headerFONT = new Font(Font.FontFamily.HELVETICA, 15.5f, Font.BOLD, BaseColor.BLACK);// from 14
            Font allFONTsize = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.NORMAL, BaseColor.BLACK); //from 8

            Font allFONTsizeCustomer = new Font(Font.FontFamily.TIMES_ROMAN, 9f, Font.NORMAL, BaseColor.BLACK); //from 8

            //Font allFONTsize = new Font(Font.FontFamily.TIMES_ROMAN, 8.5f, Font.NORMAL, BaseColor.BLACK); //from 8


            Font tinfont = new Font(Font.FontFamily.TIMES_ROMAN, 7.5f, Font.NORMAL, BaseColor.BLACK); //from 7

            PdfPTable titleTable = new iTextSharp.text.pdf.PdfPTable(2) { TotalWidth = 390, LockedWidth = true };
            titleTable.DefaultCell.Border = 0;

            float[] widthsTabTitle = new float[] { 290, 100 };
            titleTable.SetWidths(widthsTabTitle);
            //Create PdfTable object
            // String text = "     Page " + writer.PageNumber +"                                                   **This is Computer Generated Invoice**                                     Authorised Signatory ";
             String text = "     Page " + writer.PageNumber + "                                                   **This is Computer Generated Invoice**                                                           ";

            //Add paging to footer
            {
                cb.BeginText();
                cb.SetFontAndSize(bf, 6);
                cb.SetTextMatrix(document.PageSize.GetRight(390), document.PageSize.GetBottom(1));
                cb.ShowText(text);
                cb.EndText();
                float len = bf.GetWidthPoint(text, 6);
                // cb.AddTemplate(footerTemplate, document.PageSize.GetRight(180) + len, document.PageSize.GetBottom(30));
            }
            //Row 2



            iTextSharp.text.Paragraph p4 = new iTextSharp.text.Paragraph();
            Phrase pht1 = new Phrase("GSTIN:" + GSTINCompany + "\n", tinfont);

            Font chunkguru = new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.BOLD, BaseColor.BLACK);

            Font chunkInvDateInv = new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.BOLD, BaseColor.BLACK);

            //Phrase pht2 = new Phrase("          --om--  " + "\n" + "       INVOICE", chunkguru);
            //p4.Add(pht2);

            p4.Add(pht1);
            Phrase pht3 = new Phrase("               ", allFONTsize);
            p4.Add(pht3);

            titleTable.AddCell(pht1);
            // titleTable.AddCell(pht2);
            titleTable.AddCell(pht3);
            titleTable.DefaultCell.Border = 0;
            //document.Add(titleTable);

            Font dateInv = new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.BOLD, BaseColor.BLACK);

            Font gstInv = new Font(Font.FontFamily.TIMES_ROMAN, 7f, Font.BOLD, BaseColor.BLACK);

            //Mahendra G 
            PdfPTable firmtitletable = new iTextSharp.text.pdf.PdfPTable(1) { TotalWidth = 390, LockedWidth = true };
            //headerTable.WidthPercentage = 100;
           firmtitletable.DefaultCell.Border = 0;
            firmtitletable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;
            //PdfPCell FirmTitCell = new PdfPCell();

            iTextSharp.text.Paragraph p11 = new iTextSharp.text.Paragraph();
            if (CashCustName != "")
            {
                Phrase pfirm1CashCredit = new Phrase("Cash Memo" + "\n", gstInv);
                p11.Add(pfirm1CashCredit);
            }
            else
            {
                Phrase pfirm1CashCredit = new Phrase("GST Invoice" + "\n", gstInv);
                p11.Add(pfirm1CashCredit);
            }

            Phrase pfirm1 = new Phrase(CompanyName + "\n", headerFONT);
            p11.Add(pfirm1);

            if (SubTitle.Trim() != "")
            {
                Phrase pfirm5 = new Phrase(SubTitle + "\n", tinfont);
                p11.Add(pfirm5);
            }

            Phrase pfirm2 = new Phrase(Address + "\n", allFONTsize);
            p11.Add(pfirm2);
            
            //if (false)
            //{
            Phrase pfirmgd = new Phrase(Address2 + " " + City + "-" + PinCode + " " + StateComp + "\n", allFONTsize);
                p11.Add(pfirmgd);
            //}

            if (Email.Trim() != "")
            {
                Phrase pfirm3 = new Phrase("Email: " + Email + "\n", allFONTsize);
                p11.Add(pfirm3);
            }

            if (Web.Trim() != "")
            {
                Phrase pfirm3web = new Phrase("Web: " + Web + "\n", allFONTsize);
                p11.Add(pfirm3web);
            }

            Phrase pPhonenumber = new Phrase("Contact: " + Phone + "  " + Mob + "\n", allFONTsize);
            p11.Add(pPhonenumber);

            Phrase pfirm5gstin = new Phrase("GSTIN: " + GSTINCompany + "\n", dateInv);
            p11.Add(pfirm5gstin);

            //Phrase pfirmempty4 = new Phrase("" + "\n");
            //p11.Add(pfirmempty4);


            string dtddmmmyyyy = DateTime.Parse(BillDate.Trim()).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture);

            //Phrase pfirm4 = new Phrase("Date: " + dtddmmmyyyy + "  " + "Invoice: " + invoiceNumber + "", chunkguru);
            //p11.Add(pfirm4);

            string invFormatDigit = invoiceNumber.ToString();
            if (invoiceNumber.Length == 1)
            {
                invFormatDigit = "00" + invFormatDigit;
            }
            if (invoiceNumber.Length == 2)
            {
                invFormatDigit = "0" + invFormatDigit;
            }

           


            //Phrase pfirm4 = new Phrase("                                                                                                            Date: " + BillDate + "  " + "Invoice: " + invFormatDigit + "", chunkguru);
            //p11.Add(pfirm4);

            //Phrase pfirm4 = new Phrase("Date: " + BillDate + "  " + "Invoice: " + invFormatDigit + "", chunkguru);
            //p11.Add(pfirm4);

            //p11.Alignment = Element.ALIGN_CENTER;
            //FirmTitCell.AddElement(p11);
            p11.Alignment = Element.ALIGN_CENTER;
            //FirmTitCell.HorizontalAlignment = Element.ALIGN_CENTER;
            firmtitletable.AddCell(p11);
            //document.Add(firmtitletable);

            PdfPTable Firmdatetable = new iTextSharp.text.pdf.PdfPTable(1) { TotalWidth = 390, LockedWidth = true };


            PdfPTable dateInvoice = new iTextSharp.text.pdf.PdfPTable(1) { TotalWidth = 390, LockedWidth = true };
            //headerTable.WidthPercentage = 100;
            dateInvoice.DefaultCell.Border = 0;
            dateInvoice.DefaultCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            //PdfPCell FirmTitCell = new PdfPCell();

            iTextSharp.text.Paragraph dtinv = new iTextSharp.text.Paragraph();

            Phrase pdtInv = new Phrase("Date: " + BillDate + "     " + "Invoice: " + invFormatDigit + "", chunkInvDateInv);
            dtinv.Add(pdtInv);
            dateInvoice.AddCell(dtinv);

            Font colorHighlight = new Font(Font.FontFamily.TIMES_ROMAN, 10f, Font.BOLD, BaseColor.BLACK);// from 8

            Font colorHighlightfIRM = new Font(Font.FontFamily.TIMES_ROMAN, 8f, Font.BOLD, BaseColor.BLACK);// from 8

            PdfPTable headerTable = new iTextSharp.text.pdf.PdfPTable(2) { TotalWidth = 390, LockedWidth = true };
          
            //float[] widthsHeaderTbles = new float[] { 300,90 };
            //headerTable.SetWidths(widthsHeaderTbles);

            headerTable.DefaultCell.BorderWidthTop = 0;
            headerTable.DefaultCell.BorderWidthBottom = 0;
            headerTable.DefaultCell.BorderWidthRight = 0;

            //headerTable.WidthPercentage = 100;
            //headerTable.DefaultCell.Border = 0;
            //var selectedValueDelivery = ((ComboBoxItem)deliveryBy.SelectedItem).Content.ToString();
            iTextSharp.text.Paragraph p2 = new iTextSharp.text.Paragraph();
            Phrase billedTo = new Phrase("Details of Receiver(Billed To/Shipped To)" + "\n", dateInv);
            p2.Add(billedTo);
            if (CashCustName != "")
            {
                Phrase Sphpcust1 = new Phrase("Name: " + CashCustName + "\n", colorHighlight);
                p2.Add(Sphpcust1);
            }
            else
            {
                Phrase Sphpcust1 = new Phrase("Name: " + printName + "\n", colorHighlight);
                //Phrase Sphpcust1 = new Phrase("Name: " + custName + "\n", allFONTsize);
                p2.Add(Sphpcust1);
            }
            if (addressCust != "")
            {
                Phrase Sphpcust2 = new Phrase("Address: " + addressCust + "," + "Contact: " + mobCust + "\n", colorHighlight);
                p2.Add(Sphpcust2);
            }
            //Phrase Sphpcont = new Phrase("Contact: " + mobCust + "\n", allFONTsize);
            //p2.Add(Sphpcont);
            Phrase Sphpcust3 = new Phrase("State: " + State + "," + "State Code: " + StateCode + "\n", colorHighlightfIRM);
            p2.Add(Sphpcust3);
            //Phrase SStateCode = new Phrase("State Code: " + StateCode + "\n", allFONTsize);
            //p2.Add(SStateCode);
            if (CashCustName == "")
            {
                //Phrase SGSTIN = new Phrase("GSTIN: " + GSTIN + "\n", colorHighlight);
                Phrase SGSTIN = new Phrase("GSTIN: " + GSTIN , colorHighlight);
                p2.Add(SGSTIN);
            }

            Phrase pfirm5emp = new Phrase("");
            p2.Add(pfirm5emp);

            iTextSharp.text.Paragraph p3 = new iTextSharp.text.Paragraph();
            if (CashCustName == "")
            {
                Phrase phpcust2 = new Phrase("ORIGINAL/TRANSPORT/SUPPLIER COPY" + "\n", allFONTsize);
                p3.Add(phpcust2);
            }
            Phrase phpcust3 = new Phrase("Payment Mode :"+ cashCredit + "\n", allFONTsize);
            //p3.Add(phpcust3);
            if (ShippingAddress != "")
            {
                Phrase bshipaddre = new Phrase("Shipping To: " + ShippingAddress + "\n", allFONTsize);
                p3.Add(bshipaddre);
            }
            else if (CashCustName == "")
            {
                Phrase ewayNo = new Phrase("EWay#: " + EwayNumber + "\n", allFONTsize);
                p3.Add(ewayNo);

                Phrase shippedTo = new Phrase("Your Order: " + YourOrder + "\n", allFONTsize);
                p3.Add(shippedTo);

                Phrase phpcust1 = new Phrase("Through: " + SelectedValueDelivery + "-" + transportName + "\n", allFONTsize);
                p3.Add(phpcust1);

                Phrase BStateCode = new Phrase("Total Parcels:" + selecteValueParcels + "\n", allFONTsize);
                p3.Add(BStateCode);

            }


          
            Phrase pfirmemp = new Phrase("");
            p3.Add(pfirmemp);

    


            headerTable.AddCell(p2);
            headerTable.AddCell(p3);


            float[] widthsTab = new float[] { 250, 140 };
            headerTable.SetWidths(widthsTab);
            // document.Add(headerTable);


            PdfPCell pdfCell3 = new PdfPCell(headerTable);


            pdfCell3.HorizontalAlignment = Element.ALIGN_CENTER;

            pdfCell3.VerticalAlignment = Element.ALIGN_MIDDLE;

            pdfCell3.Border = 0;


            PdfPTable pdfTab = new PdfPTable(1) { TotalWidth = 390, LockedWidth = true }; ;

            pdfTab.AddCell(pdfCell3);

            pdfTab.TotalWidth = 390;
            //pdfTab.WidthPercentage = 70;
            pdfTab.HorizontalAlignment = Element.ALIGN_CENTER;

            Firmdatetable.AddCell(firmtitletable);
            Firmdatetable.AddCell(dateInvoice);
           
            PdfPCell balancehtCell = new PdfPCell(new Phrase(""));
            balancehtCell.AddElement(pdfTab);


           // Firmdatetable.AddCell(balancehtCell);
            //Firmdatetable.TotalHeight = 159;
           // float balancehght = 157 - Firmdatetable.TotalHeight;
            //float balancehght = 155 - Firmdatetable.TotalHeight; // Apr 2019 Backup
            float balancehght = 166 - Firmdatetable.TotalHeight;


            if (balancehght >= pdfTab.TotalHeight +2) //from 2
            {

                balancehtCell.FixedHeight =  balancehght;
                Firmdatetable.AddCell(balancehtCell);
            }
            else
            {
               // Firmdatetable.AddCell(balancehtCell);
                System.Windows.MessageBox.Show("Your address has reached to maximum limit, Please shrink address", "Address Exceed");
            }
           
            Firmdatetable.DefaultCell.Rowspan = 2;

            PdfPTable spcacetab = new iTextSharp.text.pdf.PdfPTable(1) { TotalWidth = 390, LockedWidth = true };
            //headerTable.WidthPercentage = 100;
            spcacetab.DefaultCell.Border = 0;
            iTextSharp.text.Paragraph spcacetabpara = new iTextSharp.text.Paragraph();
            Phrase phrspace = new Phrase("                                                ");
            spcacetabpara.Add(phrspace);
            spcacetab.AddCell(spcacetabpara);


            //call WriteSelectedRows of PdfTable. This writes rows from PdfWriter in PdfTable
            //first param is start row. -1 indicates there is no end row and all the rows to be included to write
            //Third and fourth param is x and y position to start writing
          // // pdfTabTitleFirm.WriteSelectedRows(0, -1, 8, document.PageSize.Height - 1, writer.DirectContent);
           // firmtitletable.WriteSelectedRows(0, -1, 12, document.PageSize.Height - 4, writer.DirectContent);
           // dateInvoice.WriteSelectedRows(0, -1, 12, document.PageSize.Height - 70, writer.DirectContent);
            Firmdatetable.WriteSelectedRows(0, -1, 15, document.PageSize.Height - 4, writer.DirectContent);
            // pdfTab.WriteSelectedRows(0, -1, 12, document.PageSize.Height - 90, writer.DirectContent);
          //  spcacetab.WriteSelectedRows(0, -1, 12, document.PageSize.Height - 150, writer.DirectContent);
            float titlehgtf = pdfTabTitleFirm.TotalHeight;


            float firmehgtf = firmtitletable.TotalHeight;

            float pdfTabhgtf = pdfTab.TotalHeight;

        }

        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);

            headerTemplate.BeginText();
            headerTemplate.SetFontAndSize(bf, 12);
            headerTemplate.SetTextMatrix(0, 0);
            headerTemplate.ShowText((writer.PageNumber - 1).ToString());
            headerTemplate.EndText();

            footerTemplate.BeginText();
            footerTemplate.SetFontAndSize(bf, 12);
            footerTemplate.SetTextMatrix(0, 0);
            footerTemplate.ShowText((writer.PageNumber - 1).ToString());
            footerTemplate.EndText();


        }
    }
}
