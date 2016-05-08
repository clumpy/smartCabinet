using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UIShell.OSGi;
using System.Windows;
using UIShell.OSGi.Core.Service;
using UIShell.BundleManagementService;

namespace UIShell.WpfAppCenterPlugin
{
    public class BundleActivator : IBundleActivator
    {
        public static IBundle Bundle { get; private set; }
        public static IBundleInstallerService BundleInstallerService { get; private set; }
        public static ServiceTracker<IBundleManagementService> BundleManagementServiceTracker { get; private set; }

        public void Start(IBundleContext context)
        {
            Bundle = context.Bundle;
            BundleManagementServiceTracker = new ServiceTracker<IBundleManagementService>(context);
            BundleInstallerService = context.GetFirstOrDefaultService<IBundleInstallerService>();

            var app = context.GetFirstOrDefaultService<Application>();
            if (app != null)
            {
                try
                {
                    ResourceDictionary r = new ResourceDictionary();
                    r.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/FirstFloor.ModernUI,Version=1.0.3.0;component/Assets/ModernUI.xaml", UriKind.RelativeOrAbsolute) });
                    r.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/FirstFloor.ModernUI,Version=1.0.3.0;component/Assets/ModernUI.Light.xaml", UriKind.RelativeOrAbsolute) });
                    app.Resources = r;
                }
                catch
                {
                }
            }
        }

        public void Stop(IBundleContext context)
        {
            
        }
    }
}
