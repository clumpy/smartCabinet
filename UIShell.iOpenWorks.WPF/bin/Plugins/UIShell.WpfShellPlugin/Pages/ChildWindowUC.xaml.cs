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
using System.Windows.Navigation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UIShell.WpfShellPlugin.Pages
{
    /// <summary>
    /// ChildWindowUC.xaml 的交互逻辑
    /// </summary>
    public partial class ChildWindowUC : UserControl
    {
        public event EventHandler Close;

        public ChildWindowUC()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close(sender, e);
        }
    }
}

