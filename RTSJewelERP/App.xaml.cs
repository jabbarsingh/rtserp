using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace RTSJewelERP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //Process proc = Process.GetCurrentProcess();
            //int count = Process.GetProcesses().Where(p =>
            //    p.ProcessName == proc.ProcessName).Count();

            //if (count > 1)
            //{
            //    MessageBox.Show("Already an instance is running...");
            //    App.Current.Shutdown();
            //}
            //else
            //{
                Loginpage f = new Loginpage();
                f.ShowDialog();
            //}

        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            //uncomment below code to stop multi instance, to be taken after separate source code for Estimation
            //Process proc = Process.GetCurrentProcess();
            //int count = Process.GetProcesses().Where(p =>
            //    p.ProcessName == proc.ProcessName).Count();

            //if (count > 1)
            //{
            //    MessageBox.Show("यह सॉफ्टवेयर पहले से चल रहा है, कृपया नीचे देखें...It's Already Running");
            //    App.Current.Shutdown();
            //}
            //else
            //{
                // Select the text in a TextBox when it receives focus.
                EventManager.RegisterClassHandler(typeof(TextBox), TextBox.PreviewMouseLeftButtonDownEvent,
                    new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
                EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotKeyboardFocusEvent,
                    new RoutedEventHandler(SelectAllText));
                EventManager.RegisterClassHandler(typeof(TextBox), TextBox.MouseDoubleClickEvent,
                    new RoutedEventHandler(SelectAllText));
                base.OnStartup(e);
            //}
        }

        void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent != null)
            {
                var textBox = (TextBox)parent;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    // If the text box is not yet focused, give it the focus and
                    // stop further processing of this click event.
                    textBox.Focus();

                    e.Handled = true;
                }
            }
        }

        void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
                textBox.SelectAll();
        }



    }
}
