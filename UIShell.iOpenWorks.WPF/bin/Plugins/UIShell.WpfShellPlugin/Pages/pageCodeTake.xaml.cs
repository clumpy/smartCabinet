using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using UIShell.WpfShellPlugin.DAL;




namespace UIShell.WpfShellPlugin.Pages
{
    /// <summary>
    /// pageCodeTake.xaml 的交互逻辑
    /// </summary>
    public partial class pageCodeTake : UserControl
    {
        public pageCodeTake()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            GetData();
        }

        cabinetEntities db = new cabinetEntities();

        protected void GetData()
        {



            List<DAL.Table> list = db.Table.ToList<DAL.Table>();

            //listView1.ItemsSource = list;

        }
        private void add_Click(object sender, RoutedEventArgs e)
        {
            Int16 i = 0;
            for (i = 1; i <= 20; i++)
            {
                dbinsert test = new dbinsert();
                test.Insert(i, null, null, 0);
            }

        }

    }
}

