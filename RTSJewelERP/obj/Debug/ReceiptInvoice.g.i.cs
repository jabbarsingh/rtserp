﻿#pragma checksum "..\..\ReceiptInvoice.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "86CF2598BC789E0E69B3CEF1F431AEFD648027CD"
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
    /// ReceiptInvoice
    /// </summary>
    public partial class ReceiptInvoice : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\ReceiptInvoice.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox DEBIT_Account;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\ReceiptInvoice.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblInvNameVV;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\ReceiptInvoice.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox Mode;
        
        #line default
        #line hidden
        
        
        #line 24 "..\..\ReceiptInvoice.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtAmount;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\ReceiptInvoice.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DatePicker invDate;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\ReceiptInvoice.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox VoucherNumber;
        
        #line default
        #line hidden
        
        
        #line 33 "..\..\ReceiptInvoice.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblInvoiceBalance;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\ReceiptInvoice.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal RTSJewelERP.Controls.AutoCompleteComboBox autocompltCustName;
        
        #line default
        #line hidden
        
        
        #line 38 "..\..\ReceiptInvoice.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblCRBalance;
        
        #line default
        #line hidden
        
        
        #line 39 "..\..\ReceiptInvoice.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox Narration;
        
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
            System.Uri resourceLocater = new System.Uri("/RTSJewelERP;component/receiptinvoice.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\ReceiptInvoice.xaml"
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
            this.DEBIT_Account = ((System.Windows.Controls.ComboBox)(target));
            
            #line 8 "..\..\ReceiptInvoice.xaml"
            this.DEBIT_Account.LostFocus += new System.Windows.RoutedEventHandler(this.DEBIT_Account_LostFocus);
            
            #line default
            #line hidden
            
            #line 8 "..\..\ReceiptInvoice.xaml"
            this.DEBIT_Account.KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.lblInvNameVV = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.Mode = ((System.Windows.Controls.ComboBox)(target));
            
            #line 11 "..\..\ReceiptInvoice.xaml"
            this.Mode.KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 4:
            this.txtAmount = ((System.Windows.Controls.TextBox)(target));
            
            #line 24 "..\..\ReceiptInvoice.xaml"
            this.txtAmount.KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            
            #line 24 "..\..\ReceiptInvoice.xaml"
            this.txtAmount.PreviewTextInput += new System.Windows.Input.TextCompositionEventHandler(this.NumberValidationTextBox);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 25 "..\..\ReceiptInvoice.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            this.invDate = ((System.Windows.Controls.DatePicker)(target));
            
            #line 29 "..\..\ReceiptInvoice.xaml"
            this.invDate.AddHandler(System.Windows.Input.Keyboard.PreviewKeyUpEvent, new System.Windows.Input.KeyEventHandler(this.DatePicker_PreviewKeyUp));
            
            #line default
            #line hidden
            
            #line 29 "..\..\ReceiptInvoice.xaml"
            this.invDate.KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 7:
            this.VoucherNumber = ((System.Windows.Controls.TextBox)(target));
            return;
            case 8:
            this.lblInvoiceBalance = ((System.Windows.Controls.Label)(target));
            return;
            case 9:
            this.autocompltCustName = ((RTSJewelERP.Controls.AutoCompleteComboBox)(target));
            return;
            case 10:
            this.lblCRBalance = ((System.Windows.Controls.Label)(target));
            return;
            case 11:
            this.Narration = ((System.Windows.Controls.TextBox)(target));
            
            #line 39 "..\..\ReceiptInvoice.xaml"
            this.Narration.KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

