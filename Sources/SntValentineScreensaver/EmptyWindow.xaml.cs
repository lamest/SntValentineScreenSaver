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

namespace SntValentineScreensaver
{
    /// <summary>
    /// Interaction logic for УьзенЦштвщц.xaml
    /// </summary>
    public partial class EmptyWindow : Window
    {
        public string BGColor { get; set; } = "#a23a61";
        public EmptyWindow()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            App.ShutdownIfTimeout();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Application.Current.Shutdown();
        }
    }
}
