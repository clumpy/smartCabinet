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
using System.ComponentModel;
using UIShell.WpfShellPlugin;



namespace UIShell.WpfShellPlugin.Pages
{
    /// <summary>
    /// pageManualTake.xaml 的交互逻辑
    /// </summary>
    /// 

    public class bom : INotifyPropertyChanged
    {
        private int num;
        private int takenum;
        public event PropertyChangedEventHandler PropertyChanged;
        public int Num
        {
            get { return num; }
            set
            {
                num = value;
                if (this.PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Num"));
                }
            }
        }

        public int TakeNum
        {
            get { return takenum; }
            set
            {
                takenum = value;
                if (this.PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("TakeNum"));
                }
            }

        }


    }
    public partial class pageManualTake : UserControl
    {
        bom mbom;
        dbList takeList = new dbList();
        public pageManualTake()
        {
            InitializeComponent();
            GetData();
            mbom = new bom { Num = 0, TakeNum = 0 };
            this.DataContext = mbom;
          
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            GetData();
        }
        
        protected void GetData()
        {
            takeList.updateList();
            listView1.ItemsSource = takeList.list;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            
            int id = int.Parse(textBox_CabID.Text);

            var bom = takeList.db.Table.Where(c => c.CabID == id).OrderBy(c => c.CabID).FirstOrDefault();
            if (bom.Num >= mbom.TakeNum && mbom.TakeNum > 0)
            {
                bom.Num = bom.Num - mbom.TakeNum;

                bom.DateUpdated = DateTime.Now;

                takeList.db.SaveChanges();

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TakeBom"));
                GetData();

                mbom.TakeNum = 0;
                TcpClient.CabinetTCPServer.CabinetOpen(id);
            }

            else
                MessageBox.Show("没有足够的物料！");

        }

        private void menuPrism_Click(object sender, RoutedEventArgs e)
        {



            WindowPrismDemo1 win = new WindowPrismDemo1();

            win.Show();

        }

        private void listView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }

        

        

    }



}

