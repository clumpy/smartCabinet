using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Controls;

namespace UIShell.WpfAppCenterPlugin
{
    /// <summary>
    /// Interaction logic for AppCenterWindow.xaml
    /// </summary>
    public partial class AppCenterWindow : ModernWindow
    {
        public AppCenterWindow()
        {
            InitializeComponent();
            Style = (Style)Application.Current.Resources["EmptyWindow"];
        }
    }
}
