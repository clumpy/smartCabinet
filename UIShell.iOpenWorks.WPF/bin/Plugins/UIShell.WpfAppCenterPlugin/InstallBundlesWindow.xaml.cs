using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Controls;
using UIShell.BundleManagementService;
using UIShell.iOpenWorks.BundleRepository.OpenAPI;

namespace UIShell.WpfAppCenterPlugin
{
    /// <summary>
    /// InstallBundlesUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class InstallBundlesWindow : ModernWindow, IInstallationProgressReport
    {
        private List<RequestBundle> RequestBundles { get; set; }
        private bool IsInstallationInProgress { get; set; }

        public InstallBundlesWindow(List<RequestBundle> requestBundles)
        {
            InitializeComponent();
            Style = (Style)Application.Current.Resources["EmptyWindow"];
            RequestBundles = requestBundles;
        }

        public void AppendProgressItem(ProgressReportItem item)
        {
            Action invokeDel = ()=>
            {
                if (!string.IsNullOrEmpty(item.Title))
                {
                    TitleTextBlock.Text = item.Title;
                }
                if (!string.IsNullOrEmpty(item.Message))
                {
                    MessageTextBlock.Inlines.Add(item.Message);
                    MessageTextBlock.Inlines.Add(Environment.NewLine);
                    ScrollViewer.LineDown();
                    ScrollViewer.LineDown();
                }
                InstallProgressBar.Value = item.Percentage;
            };

            Dispatcher.Invoke(invokeDel);
        }

        public void ClearProgressReport()
        {
            
        }
        
        private void ModernWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (RequestBundles != null && RequestBundles.Count > 0)
            {
                var bundleManagementService = BundleActivator.BundleManagementServiceTracker.DefaultOrFirstService;
                if (bundleManagementService != null)
                {
                    IsInstallationInProgress = true;
                    new Thread(() =>
                    {
                        bundleManagementService.InstallBundles(RequestBundles, this);
                        AppendProgressItem(new ProgressReportItem { Title = "操作完成，需要重启", Message = "系统要求重启，请点击'重启系统'按钮完成重启。", Percentage = 100 });
                        IsInstallationInProgress = false;
                    }).Start();
                }
            }
        }

        private void ModernWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = IsInstallationInProgress;
        }
    }
}
