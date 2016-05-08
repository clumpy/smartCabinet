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
using UIShell.OSGi;

namespace UIShell.WpfAppCenterPlugin
{
    /// <summary>
    /// BundleManagementUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class BundleManagementUserControl : UserControl
    {
        public BundleManagementUserControl()
        {
            InitializeComponent();
        }

        private void BindBundlesData()
        {
            var bundleManagementService = BundleActivator.BundleManagementServiceTracker.DefaultOrFirstService;
            if (bundleManagementService != null)
            {
                var bundles = bundleManagementService.GetLocalBundles();
                BundlesDataGrid.DataContext = null;
                BundlesDataGrid.DataContext = bundles;
                if (bundles.Count > 0)
                {
                    BundlesDataGrid.SelectedIndex = 0;
                }
                else
                {
                    BundlesDataGrid.SelectedItem = null;
                }
                SelectBundle();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            BindBundlesData();
        }

        private void BundlesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectBundle();
        }

        private void SelectBundle()
        {
            var item = BundlesDataGrid.SelectedItem;
            var bundleManagementService = BundleActivator.BundleManagementServiceTracker.DefaultOrFirstService;
            if (item != null && bundleManagementService != null)
            {
                var bundle = item as IBundle;
                bool allowStarted = bundleManagementService.IsBundleAllowedToStart(bundle.BundleID);
                bool allowStopped = bundleManagementService.IsBundleAllowedToStop(bundle.BundleID);
                bool allowUninstalled = bundleManagementService.IsBundleAllowedToUninstall(bundle.BundleID);
                //string operation = allowStarted ? " 启动" : string.Empty;
                //operation += allowStopped ? " 停止" : string.Empty;
                //operation += allowUninstalled ? " 卸载" : string.Empty;
                //if (string.IsNullOrEmpty(operation))
                //{
                //    operation = " 无";
                //}
                StartButton.IsEnabled = allowStarted;
                StopButton.IsEnabled = allowStopped;
                UninstallButton.IsEnabled = allowUninstalled;

                StartButton.Visibility = bundle.State != BundleState.Active ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                StopButton.Visibility = bundle.State == BundleState.Active ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                UninstallButton.Visibility = System.Windows.Visibility.Visible;

                SelectedBundleTextBlock.Text = string.Format("当前插件：{0}", bundle.Name);
            }
            else
            {
                SelectedBundleTextBlock.Text = string.Empty;
                StartButton.Visibility = System.Windows.Visibility.Hidden;
                StopButton.Visibility = System.Windows.Visibility.Hidden;
                UninstallButton.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            var item = BundlesDataGrid.SelectedItem;
            var bundleManagementService = BundleActivator.BundleManagementServiceTracker.DefaultOrFirstService;
            if (item != null && bundleManagementService != null)
            {
                try
                {
                    bundleManagementService.StartBundle((item as IBundle).BundleID);
                }
                catch (BundleException ex)
                {
                }
                BindBundlesData();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            var item = BundlesDataGrid.SelectedItem;
            var bundleManagementService = BundleActivator.BundleManagementServiceTracker.DefaultOrFirstService;
            if (item != null && bundleManagementService != null)
            {
                try
                {
                    bundleManagementService.StopBundle((item as IBundle).BundleID);
                }
                catch (BundleException ex)
                {
                }
                BindBundlesData();
            }
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            var item = BundlesDataGrid.SelectedItem;
            var bundleManagementService = BundleActivator.BundleManagementServiceTracker.DefaultOrFirstService;
            if (item != null && bundleManagementService != null)
            {
                try
                {
                    bundleManagementService.UninstallBundle((item as IBundle).BundleID);
                }
                catch (BundleException ex)
                {
                }
                BindBundlesData();
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            BindBundlesData();
        }
    }
}
