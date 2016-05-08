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
    /// Introduction3.xaml 的交互逻辑
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
    public partial class Introduction3 : UserControl
    {
        bom mbom;
        cabinetEntities db = new cabinetEntities();
        

        public Introduction3()
        {
            InitializeComponent();
            GetData();
            List<DAL.Table> list = db.Table.ToList<DAL.Table>();
            mbom = new bom { Num = 0, TakeNum = 0 };
            this.DataContext = mbom;
          
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            GetData();
        }
        
        protected void GetData()
        {
            List<DAL.Table> list = db.Table.ToList<DAL.Table>();                   
            listView1.ItemsSource = list;

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            
            int id = int.Parse(textBox_CabID.Text);

            var bom = db.Table.Where(c => c.CabID == id).OrderBy(c => c.CabID).FirstOrDefault();
            if (bom.Num >= mbom.TakeNum && mbom.TakeNum > 0)
            {
                bom.Num = bom.Num - mbom.TakeNum;

                bom.DateUpdated = DateTime.Now;

                db.SaveChanges();

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

