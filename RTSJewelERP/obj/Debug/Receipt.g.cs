﻿#pragma checksum "..\..\Receipt.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "43E15C834DB4E52F839E23BE1A435DCD38CF8BD8"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using RTSJewelERP.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace RTSJewelERP {
    
    
    /// <summary>
    /// Receipt
    /// </summary>
    public partial class Receipt : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\Receipt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker invDate;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\Receipt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button dateShortcut;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\Receipt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox VoucherNumber;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\Receipt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox DEBIT_Account;
        
        #line default
        #line hidden
        
        
        #line 35 "..\..\Receipt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtAmount;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\Receipt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Narration;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\Receipt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox AgainstInv;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\Receipt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox InvoiceNumberCmb;
        
        #line default
        #line hidden
        
        
        #line 53 "..\..\Receipt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Mode;
        
        #line default
        #line hidden
        
        
        #line 66 "..\..\Receipt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal RTSJewelERP.Controls.AutoCompleteComboBox autocompltCustName;
        
        #line default
        #line hidden
        
        
        #line 70 "..\..\Receipt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid VoucherEntryHistory;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\Receipt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblCustBalance;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\Receipt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblCRBalance;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\Receipt.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblInvoiceBalance;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/RTSJewelERP;component/receipt.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Receipt.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 25 "..\..\Receipt.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 2:
            this.invDate = ((System.Windows.Controls.DatePicker)(target));
            
            #line 27 "..\..\Receipt.xaml"
            this.invDate.AddHandler(System.Windows.Input.Keyboard.PreviewKeyUpEvent, new System.Windows.Input.KeyEventHandler(this.DatePicker_PreviewKeyUp));
            
            #line default
            #line hidden
            
            #line 27 "..\..\Receipt.xaml"
            this.invDate.KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 3:
            this.dateShortcut = ((System.Windows.Controls.Button)(target));
            
            #line 28 "..\..\Receipt.xaml"
            this.dateShortcut.Click += new System.Windows.RoutedEventHandler(this.dateShortcut_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.VoucherNumber = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.DEBIT_Account = ((System.Windows.Controls.ComboBox)(target));
            
            #line 32 "..\..\Receipt.xaml"
            this.DEBIT_Account.LostFocus += new System.Windows.RoutedEventHandler(this.DEBIT_Account_LostFocus);
            
            #line default
            #line hidden
            
            #line 32 "..\..\Receipt.xaml"
            this.DEBIT_Account.KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 6:
            this.txtAmount = ((System.Windows.Controls.TextBox)(target));
            
            #line 35 "..\..\Receipt.xaml"
            this.txtAmount.KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            
            #line 35 "..\..\Receipt.xaml"
            this.txtAmount.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.NumberValidationTextBox);
            
            #line default
            #line hidden
            return;
            case 7:
            this.Narration = ((System.Windows.Controls.TextBox)(target));
            
            #line 39 "..\..\Receipt.xaml"
            this.Narration.KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 8:
            this.AgainstInv = ((System.Windows.Controls.ComboBox)(target));
            
            #line 40 "..\..\Receipt.xaml"
            this.AgainstInv.KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            
            #line 40 "..\..\Receipt.xaml"
            this.AgainstInv.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.AgainstInv_SelectionChanged_1);
            
            #line default
            #line hidden
            return;
            case 9:
            this.InvoiceNumberCmb = ((System.Windows.Controls.ComboBox)(target));
            
            #line 51 "..\..\Receipt.xaml"
            this.InvoiceNumberCmb.KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            
            #line 51 "..\..\Receipt.xaml"
            this.InvoiceNumberCmb.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.InvoiceNumberCmb_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 52 "..\..\Receipt.xaml"
            ((System.Windows.Controls.Label)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 11:
            this.Mode = ((System.Windows.Controls.ComboBox)(target));
            
            #line 53 "..\..\Receipt.xaml"
            this.Mode.KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 12:
            this.autocompltCustName = ((RTSJewelERP.Controls.AutoCompleteComboBox)(target));
            return;
            case 13:
            
            #line 69 "..\..\Receipt.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_4);
            
            #line default
            #line hidden
            return;
            case 14:
            this.VoucherEntryHistory = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 15:
            this.lblCustBalance = ((System.Windows.Controls.Label)(target));
            return;
            case 16:
            this.lblCRBalance = ((System.Windows.Controls.Label)(target));
            return;
            case 17:
            this.lblInvoiceBalance = ((System.Windows.Controls.Label)(target));
            return;
            case 18:
            
            #line 76 "..\..\Receipt.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_6);
            
            #line default
            #line hidden
            return;
            case 19:
            
            #line 77 "..\..\Receipt.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_10);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

