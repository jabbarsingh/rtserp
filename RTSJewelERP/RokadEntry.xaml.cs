using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for RokadEntry.xaml
    /// </summary>
    public partial class RokadEntry : Window
    {
        public RokadEntry()
        {
            InitializeComponent();
        }

        private void DataGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (e.AddedCells.Count == 0)
                this.textBox.SetBinding(TextBox.TextProperty, (string)null);
            else
            {
                var selectedCell = e.AddedCells.First();

                // Assumes your header is the same name as the field it's bound to
                var binding = new Binding(selectedCell.Column.Header.ToString())
                {
                    Mode = BindingMode.TwoWay,
                    Source = selectedCell.Item,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                this.textBox.SetBinding(TextBox.TextProperty, binding);
            }
        }

        //private ObservableCollection<SimpleClass> _simpleCollection;
        //public ObservableCollection<SimpleClass> SimpleCollection
        //{

        //    get { return _simpleCollection ?? (_simpleCollection = new ObservableCollection<SimpleClass>()); }

        //    set { _simpleCollection = value; }
        //}

    }


    public class SimpleClass
    {
        public string A { get; set; }
        public string B { get; set; }
        public string C { get; set; }
    }
}
