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
using System.Windows.Media.Animation;



namespace UIShell.WpfShellPlugin.Pages
{
    /// <summary>
    /// singleStore.xaml 的交互逻辑
    /// </summary>
    public delegate void storeEventHandler(object sender, EventArgs e);
    public class bom1 : INotifyPropertyChanged
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
    public partial class singleStore : UserControl 
    {
        bom1 mbom1;
        dbList takeList = new dbList();
        public singleStore()
        {
            InitializeComponent();
            GetData();
            mbom1 = new bom1 { Num = 0, TakeNum = 0 };
            this.DataContext = mbom1;

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

            var bom1 = takeList.db.Table.Where(c => c.CabID == id).OrderBy(c => c.CabID).FirstOrDefault();
            if (true)
            if (bom1.Num >= mbom1.TakeNum && mbom1.TakeNum > 0)
            {
                bom1.Num = bom1.Num + mbom1.TakeNum;


                if (bom1.Num == null)
                    bom1.Num = mbom1.TakeNum;
                else
                    bom1.Num = bom1.Num + mbom1.TakeNum;
                bom1.DateCreated = DateTime.Now;
                bom1.DateUpdated = DateTime.Now;

                takeList.db.SaveChanges();

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("TakeBom"));
                GetData();

                mbom1.TakeNum = 0;
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

        public FrameworkElement Parent
        {
            get;
            set;
        }

       
       // 将创建的委托和特定事件关联,在这里特定的事件为
       public static event EventHandler store;

       private void button1_Click(object sender, RoutedEventArgs e)
       {
           EventArgs storeEvent = new EventArgs();
           if (store != null)
           {
               store(sender, e);//执行委托实例  
           }

       }      
    }

}
