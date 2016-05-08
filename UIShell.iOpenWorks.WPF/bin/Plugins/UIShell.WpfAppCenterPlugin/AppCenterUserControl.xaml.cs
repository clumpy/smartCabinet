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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using UIShell.iOpenWorks.BundleRepository.OpenAPI;
using System.Collections.ObjectModel;
using FirstFloor.ModernUI.Windows.Controls;

namespace UIShell.WpfAppCenterPlugin
{
    /// <summary>
    /// Interaction logic for AppCenterUserControl.xaml
    /// </summary>
    public partial class AppCenterUserControl : UserControl
    {
        public AppCenterUserControl()
        {
            InitializeComponent();
            LoadBundles();
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            if (BundlesDataGrid.SelectedItems.Count > 0)
            {
                var requestBundles = new List<RequestBundle>();
                foreach (BundleDetails bundle in BundlesDataGrid.SelectedItems)
                {
                    requestBundles.Add(new RequestBundle()
                    {
                        InputBundleID = bundle.BundleID.ToString(),
                        SymbolicName = bundle.SymbolicName,
                        Version = bundle.Version,
                        Name = bundle.Name,
                        Upgrade = bundle.HasNewVersion
                    });
                }
                var win = new InstallBundlesWindow(requestBundles);
                win.Closed += InstallBundlesWindowClosed;
                win.ShowDialog();
            }
            else
            {
                ModernDialog.ShowMessage("请选择需要安装的插件。", "选择插件", MessageBoxButton.OK);
            }
        }

        private void InstallBundlesWindowClosed(object sender, EventArgs e)
        {
            RefreshButton_Click(null, null);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Key.Text = string.Empty;
            LoadBundles();
        }

        private void LoadBundles()
        {
            if (BundleActivator.BundleManagementServiceTracker.IsServiceAvailable)
            {
                BundlesDataGrid.DataContext = null;
                RefreshButton.IsEnabled = false;
                InstallButton.IsEnabled = false;
                LoadBundlesProgressBar.IsIndeterminate = true;
                LoadBundlesProgressBar.Visibility = System.Windows.Visibility.Visible;
                BundlesDataGrid.Visibility = System.Windows.Visibility.Hidden;
                new Thread(() =>
                {
                    var service = BundleActivator.BundleManagementServiceTracker.DefaultOrFirstService;
                    List<BundleDetails> bundles = null;
                    try
                    {
                        bundles = service.GetRepositoryBundles();
                    }
                    catch
                    {
                        // TODO: Hanel errors here.
                    }
                    Action action = () =>
                    {
                        LoadBundlesProgressBar.IsIndeterminate = false;
                        LoadBundlesProgressBar.Visibility = System.Windows.Visibility.Hidden;
                        Bundles = bundles;
                        BindBundles();
                        BundlesDataGrid.Visibility = System.Windows.Visibility.Visible;
                        RefreshButton.IsEnabled = true;
                        InstallButton.IsEnabled = true;
                    };
                    Dispatcher.Invoke(action);
                }).Start();
            }
        }

        private string KeyWord { get; set; }
        private List<BundleDetails> Bundles { get; set; }

        private void Key_TextChanged(object sender, TextChangedEventArgs e)
        {
            KeyWord = Key.Text.ToLower();
            BindBundles();
        }

        private void BindBundles()
        {
            if (Bundles == null)
            {
                return;
            }

            BundlesDataGrid.DataContext = string.IsNullOrEmpty(KeyWord) ? Bundles : Bundles.FindAll(b => b.Name.ToLower().Contains(KeyWord) || b.SymbolicName.ToLower().Contains(KeyWord));
        }

        private void BundlesDataGrid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;

            // iteratively traverse the visual tree
            while ((dep != null) &&
                !(dep is DataGridRow))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            if (dep is DataGridRow)
            {
                var row = dep as DataGridRow;
                row.IsSelected = !row.IsSelected;
                e.Handled = true;
            }
        }

        //private void ckbSelectedAll_Click(object sender, RoutedEventArgs e)
        //{
        //    BundlesDataGrid.SelectAll();
        //}

        //private void ckbSelectedAll_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    BundlesDataGrid.UnselectAll();
        //}
    }
}
