﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RTSJewelERP.Controls
{
    /// <summary>
    /// Interaction logic for AutoCompleteComboBox.xaml
    /// </summary>
    public partial class AutoCompleteBarCodeItemNameComboBox : UserControl
    {
        string CompID = RTSJewelERP.ConfigClass.CompID;
        public string AutoCompletTextCustNameText
        {
            get { return autoTextBoxBarCodeItemName.Text.Trim(); }
        }

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoCompleteComboBox"/> class.
        /// </summary>
        public AutoCompleteBarCodeItemNameComboBox()
        {
            InitializeComponent();

            // Attach events to the controls
            autoTextBoxBarCodeItemName.TextChanged +=
                new TextChangedEventHandler(autoTextBox_TextChanged);
            autoTextBoxBarCodeItemName.PreviewKeyDown +=
                new KeyEventHandler(autoTextBox_PreviewKeyDown);
            suggestionListBox.SelectionChanged +=
                new SelectionChangedEventHandler(suggestionListBox_SelectionChanged);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        /// <value>The items source.</value>
        public IEnumerable<string> ItemsSourceBarItem
        {
            get { return (IEnumerable<string>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSourceBarItem"
                                , typeof(IEnumerable<string>)
                                , typeof(AutoCompleteComboBox)
                                , new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the selected value.
        /// </summary>
        /// <value>The selected value.</value>
        public string SelectedValueBarItem
        {
            get { return (string)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedValue.  
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValueBarItem"
                            , typeof(string)
                            , typeof(AutoCompleteComboBox)
                            , new UIPropertyMetadata(string.Empty));

        #endregion

        public List<string> CountryList { get; set; }

        #region Methods
        /// <summary>
        /// Handles the TextChanged event of the autoTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        void autoTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Only autocomplete when there is text
            if (autoTextBoxBarCodeItemName.Text.Trim().Length > 0)
            {


                CountryList = new List<string>();

                //If a product code is not empty we search the database

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConStrRTSErp"].ConnectionString);
                //SqlConnection conn = new SqlConnection(@"Data Source=.\SQLExpress;Database=RTSProSoft;Trusted_Connection=Yes;");
                con.Open();
                //string sql = "select AcctName from AccountsList where CompID = '" + CompID + "'";
                //string sql = "select Distinct  ItemName from StockItemsByPc where CompID = '" + CompID + "'";
                string sql = "select Distinct  LTRIM(RTRIM(ItemName)) from StockItemsByPc where CompID = '" + CompID + "'";
                SqlCommand cmd = new SqlCommand(sql);

                cmd.Connection = con;
                SqlDataReader reader = cmd.ExecuteReader();




                while (reader.Read())
                {

                    CountryList.Add(reader.GetValue(0).ToString().Trim());

                }
                reader.Close();
                ItemsSourceBarItem = CountryList;



                // Use Linq to Query ItemsSource for resultdata
                string condition = string.Format("%{0}%", autoTextBoxBarCodeItemName.Text.Trim());
                IEnumerable<string> results = ItemsSourceBarItem.Where(
                    delegate(string s) { return s.ToLower().Contains(autoTextBoxBarCodeItemName.Text.Trim().ToLower()); });

                if (results.Count() > 0)
                {
                    suggestionListBox.ItemsSource = results;
                    suggestionListBox.Visibility = Visibility.Visible;
                }
                else
                {
                    suggestionListBox.Visibility = Visibility.Collapsed;
                    suggestionListBox.ItemsSource = null;
                }
            }
            else
            {
                suggestionListBox.Visibility = Visibility.Collapsed;
                suggestionListBox.ItemsSource = null;
            }
        }

        /// <summary>
        /// Handles the PreviewKeyDown event of the autoTextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The instance containing the event data.</param>
        void autoTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                if (suggestionListBox.SelectedIndex < suggestionListBox.Items.Count)
                {
                    suggestionListBox.SelectedIndex = suggestionListBox.SelectedIndex + 1;
                }
            }
            if (e.Key == Key.Up)
            {
                if (suggestionListBox.SelectedIndex > -1)
                {
                    suggestionListBox.SelectedIndex = suggestionListBox.SelectedIndex - 1;
                }
            }
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                // Commit the selection
                suggestionListBox.Visibility = Visibility.Collapsed;

                TraversalRequest tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                UIElement keyboardFocus = Keyboard.FocusedElement as UIElement;

                if (keyboardFocus != null)
                {
                    keyboardFocus.MoveFocus(tRequest);
                }

                e.Handled = true;


                //e.Handled = (e.Key == Key.Enter);
            }

            if (e.Key == Key.Escape)
            {
                // Cancel the selection
                suggestionListBox.ItemsSource = null;
                suggestionListBox.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the suggestionListBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void suggestionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (suggestionListBox.ItemsSource != null)
            {
                autoTextBoxBarCodeItemName.TextChanged -= new TextChangedEventHandler(autoTextBox_TextChanged);
                if (suggestionListBox.SelectedIndex != -1)
                {
                    autoTextBoxBarCodeItemName.Text = suggestionListBox.SelectedItem.ToString();



                }
                autoTextBoxBarCodeItemName.TextChanged += new TextChangedEventHandler(autoTextBox_TextChanged);
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
        }


        #endregion

    }
}
