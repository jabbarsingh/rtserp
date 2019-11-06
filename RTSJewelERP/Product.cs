using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTSJewelERP
{
    public class Product
    {
        public long Sr { get; set; }
        public long ItemUniqNumber { get; set; }

        public string ItemName { get; set; }

        public string PrintName { get; set; }

        public string ItemCode { get; set; }

        public string ItemBarCode { get; set; }

        public double ItemPrice { get; set; }
        
        public int DecimalPlaces { get; set; }

        public string UnitID { get; set; }

        public bool IsBarcodeCreated { get; set; }

        public double SaleDiscountPerc { get; set; }

        public double ActualQty { get; set; }

        public double ActualWt { get; set; }

        public double BilledQty { get; set; }

        public double BilledWt { get; set; }

        public double WastagePerc { get; set; }

        public double MC { get; set; }

        public double OpeningStock { get; set; }

        public string HSN { get; set; }

        public int GSTRate { get; set; }

        public int StorageID { get; set; }

        public int TrayID { get; set; }

        public int CounterID { get; set; }

        public DateTime UpdateDate { get; set; }

        public string ItemDesc { get; set; }

        public bool SetCriticalLevel { get; set; }

        public int SetDefaultStorageID { get; set; }
  
    
        public string ItemAlias { get; set; }
        public long UnderGroupID { get; set; }

        public string UnderGroupName { get; set; }


        public long UnderSubGroupID { get; set; }

        public DateTime LastBuyDate { get; set; }

        public DateTime LastSaleDate { get; set; }

        public double LastSalePrice { get; set; }

        public double LastBuyPrice { get; set; }

        public double CurrentStockValue { get; set; }

        public double ItemPurchPrice { get; set; }

        public string Size { get; set; }

        public string Color { get; set; }


        public double Small { get; set; }

        public double Mediium { get; set; }

        public double Large { get; set; }

        public double XL { get; set; }
        public double XL2 { get; set; }
        public double XL3 { get; set; }
        public double XL4 { get; set; }
        public double XL5 { get; set; }
        public double XL6 { get; set; }
        public string DesignNumberPattern { get; set; }

        public double CTN { get; set; }

        public double ItemMRP { get; set; }


    //[ItemUniqNumber] [nchar](300) NOT NULL,
    //[ItemName] [nchar](300) NOT NULL,
    //[PrintName] [nchar](300) NOT NULL,
    //[UnitID] [int] NULL,
    //[ItemCode] [nchar](200) NULL,
    //[ItemDesc] [nchar](200) NULL,
    //[ItemBarCode] [nchar](200) NULL,
    //[ItemMRP] [float] NULL,
    //[ItemPrice] [float] NULL,
    //[ItemMinSalePrice] [float] NULL,
    //[ItemSelfValPrice] [float] NULL,
    //[SetCriticalLevel] [bit] NULL,
    //[SetImageUrl] [nchar](200) NULL,
    //[SetDefaultStorageID] [int] NULL,
    //[SetDefaultSundryCreditor] [int] NULL,
    //[SetDefaultSundryDebtor] [int] NULL,
    //[DecimalPlaces] [int] NULL,

    //[IsBarcodeCreated] [bit] NULL,

    //[SaleDiscount] [float] NULL,
    //[PurchaseDiscount] [float] NULL,
    //[SaleDiscountStructure] [nchar](200) NULL,
    //[PurchaseDiscountStructure] [nchar](200) NULL,
    //[ItemPurchPrice] [float] NULL,
    //[ItemAvgRate] [float] NULL,
    //[ItemStdRate] [float] NULL,
    //[ItemPriceLevel1] [float] NULL,
    //[ItemPriceLevel2] [float] NULL,
    //[ItemPriceLevel3] [float] NULL,
    //[ItemQRCode] [nchar](200) NULL,
    //[ItemAlias] [nchar](200) NULL,

    //[UnderGroupName] [nchar](200) NULL,
    //[UnderGroupID] [bigint] NULL,
    //[UnderSubGroupName] [nchar](200) NULL,
    //[UnderSubGroupID] [bigint] NULL,
    //[ActualQty] [float] NOT NULL,
    //[HSN] [nchar](200) NULL,
    //[GSTRate] [int] NOT NULL,
    //[StorageID] [int] NOT NULL,
    //[TrayID] [int] NOT NULL,
    //[CounterID] [int] NOT NULL,
    //[OpeningStock] [float] NULL,
    //[OpeningStockValue] [float] NULL,
        //[CurrentStockValue] [float] NULL,
    //[CreationDate] [datetime] NOT NULL,
    //[UpdateDate] [datetime] NULL,
    //[CreatedBy] [nchar](100) NULL

        //lastPurchaseDate
        //LastSaleDate
        //LastSalePrice
        //LastBuyPrice
      
    }
}
