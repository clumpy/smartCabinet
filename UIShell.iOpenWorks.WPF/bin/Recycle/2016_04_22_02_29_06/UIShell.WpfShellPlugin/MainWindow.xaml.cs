using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UIShell.WpfShellPlugin.ExtensionModel;
using FirstFloor.ModernUI.Presentation;

namespace UIShell.WpfShellPlugin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public static ShellExtensionPointHandler ShellExtensionPointHandler { get; set; }
        private List<Tuple<LinkGroupData, LinkGroup>> LinkGroupTuples { get; set; }
        
        public MainWindow()
        {
            InitializeComponent();
            LinkGroupTuples = new List<Tuple<LinkGroupData, LinkGroup>>();
            ShellExtensionPointHandler = new ShellExtensionPointHandler(BundleActivator.Bundle);
            ShellExtensionPointHandler.LinkGroups.CollectionChanged += LinkGroups_CollectionChanged;
            InitializeLinkGroupsForExtensions();
        }

        void InitializeLinkGroupsForExtensions()
        {
            foreach (var linkGroupData in ShellExtensionPointHandler.LinkGroups)
            {
                CreateLinkGroupForData(linkGroupData);
            }

            //// 添加设置菜单
            //var settingsLinkGroup = new LinkGroup { DisplayName = "外观设置", GroupName="settings" };
            //var settingsLink = new Link { DisplayName = "外观设置", Source = new Uri("/UIShell.WpfShellPlugin;component/Pages/Settings.xaml", UriKind.RelativeOrAbsolute) };
            //settingsLinkGroup.Links.Add(settingsLink);
            //MenuLinkGroups.Add(settingsLinkGroup);

            //TitleLinks.Add(settingsLink);

            // 设置第一个页面
            if (ShellExtensionPointHandler.LinkGroups.Count > 0)
            {
                var first = ShellExtensionPointHandler.LinkGroups[0];
                ContentSource = new Uri(first.FormatSource(first.DefaultContentSource), UriKind.RelativeOrAbsolute);
            }
        }

        void LinkGroups_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Action action = () =>
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    // 新加了LinkGroupData
                    foreach (LinkGroupData item in e.NewItems)
                    {
                        CreateLinkGroupForData(item);
                    }
                }
                else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    // 删除了LinkGroupData
                    foreach (LinkGroupData item in e.OldItems)
                    {
                        RemoveLinkGroupForData(item);
                    }
                }
            };
            
            Dispatcher.Invoke(action);
        }

        void CreateLinkGroupForData(LinkGroupData linkGroupData)
        {
            var linkGroup = new LinkGroup { DisplayName = linkGroupData.DisplayName, GroupName = linkGroupData.GroupName };
            foreach (var linkData in linkGroupData.Links)
            {
                if (linkData is LinkData)
                {
                    
                    linkGroup.Links.Add(new Link { DisplayName = linkData.DisplayName, Source = new Uri(linkData.FormatSource((linkData as LinkData).Source), UriKind.RelativeOrAbsolute) });
                }
                else if (linkData is TabLinkData)
                {
                    linkGroup.Links.Add(new Link { DisplayName = linkData.DisplayName, Source = new Uri("UIShell.WpfShellPlugin@UIShell.WpfShellPlugin.Pages.ContentPlaceHolder?LinkId=" + linkData.LinkId.ToString(), UriKind.RelativeOrAbsolute) });
                }
            }
            if (linkGroupData.IsTitleLink)
            {
                TitleLinks.Add(new Link { DisplayName = linkGroupData.DisplayName, Source = new Uri(linkGroupData.FormatSource(linkGroupData.DefaultContentSource), UriKind.RelativeOrAbsolute) });
            }
            MenuLinkGroups.Add(linkGroup);
            LinkGroupTuples.Add(new Tuple<LinkGroupData, LinkGroup>(linkGroupData, linkGroup));
        }

        void RemoveLinkGroupForData(LinkGroupData linkGroupData)
        {
            var tuple = LinkGroupTuples.Find(t => t.Item1.Equals(linkGroupData));
            if (tuple != null)
            {
                MenuLinkGroups.Remove(tuple.Item2);
                LinkGroupTuples.Remove(tuple);
            }
        }
    }
}
