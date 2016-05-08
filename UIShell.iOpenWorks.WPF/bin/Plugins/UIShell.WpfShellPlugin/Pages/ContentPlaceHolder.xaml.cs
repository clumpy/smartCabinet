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
using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using UIShell.WpfShellPlugin.ExtensionModel;

namespace UIShell.WpfShellPlugin.Pages
{
    /// <summary>
    /// ContentPlaceHolder.xaml 的交互逻辑
    /// </summary>
    public partial class ContentPlaceHolder : UserControl
    {
        private string _linkId = string.Empty;
        private FirstFloor.ModernUI.Windows.Controls.ModernTab _tab;
        public string LinkId
        {
            get
            {
                return _linkId;
            }
            set
            {
                _linkId = value;
                TabLinkData tabLinkData = null;
                foreach (var linkGroupData in MainWindow.ShellExtensionPointHandler.LinkGroups)
                {
                    foreach (var link in linkGroupData.Links)
                    {
                        if (link.LinkId.ToString().Equals(_linkId, StringComparison.OrdinalIgnoreCase))
                        {
                            tabLinkData = link as TabLinkData;
                            break;
                        }
                    }
                }

                if (tabLinkData != null)
                {
                    _tab.SelectedSource = new Uri(tabLinkData.FormatSource(tabLinkData.DefaultContentSource), UriKind.RelativeOrAbsolute);
                    _tab.Layout = (TabLayout)Enum.Parse(typeof(TabLayout), tabLinkData.Layout);
                    foreach(var linkData in tabLinkData.Links)
                    {
                        _tab.Links.Add(new Link { DisplayName = linkData.DisplayName, Source = new Uri(linkData.FormatSource(linkData.Source), UriKind.RelativeOrAbsolute) });
                    }
                }
            }
        }
        public ContentPlaceHolder()
        {
            InitializeComponent();
            _tab = FindName("ModernTab") as FirstFloor.ModernUI.Windows.Controls.ModernTab;
        }
    }
}
