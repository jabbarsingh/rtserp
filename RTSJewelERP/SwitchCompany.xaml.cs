using RTSJewelERP.CompanyTableAdapters;
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
    /// Interaction logic for SwitchCompany.xaml
    /// </summary>

    public partial class SwitchCompany : Window
    {
        public SwitchCompany()
        {
            InitializeComponent();
            BindComboBox(CompName);
        }

        private void CompName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConfigClass.CompID = "1";

            var companyname = CompName.SelectedItem.ToString();
            //ConfigClass.CompID = ((ComboBoxItem)CompName.SelectedItem).ToString();
            string companyid = companyname.Split('-')[1];
            //ConfigClass.CompID = CompName.Text;
            ConfigClass.CompID = companyid;

        }

        public void BindComboBox(ComboBox company)
        {
            var custAdpt = new CompanyTableAdapter();
            var custInfoVal = custAdpt.GetData();

            //            CompName.ItemsSource = custInfoVal.Where(c => (c.Alias.Trim() == "GST"))
            //.Select(x => (x.CompanyName.Trim() + "-" + x.CompanyID)).Distinct().ToList();


            CompName.ItemsSource = custInfoVal.Where(c => (c.Alias.Trim() == "GST"))
.Select(x => (x.CompanyName.Trim() + "-" + x.CompanyID)).Distinct().ToList();


            //var LinqRes = (from UserRec in custInfoVal
            //               orderby UserRec.CompSrNumber ascending
            //               select (UserRec.CompanyName.Trim() + "-" + UserRec.CompanyID)).Distinct();
            //CompName.ItemsSource = LinqRes;
            // comboBoxName.SelectedValueBinding = new Binding("Col6");
        }

        private void CompName_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

    }
}
