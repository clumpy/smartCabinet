using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using UIShell.OSGi;

namespace UIShell.WpfShellPlugin
{
    public class BundleActivator : IBundleActivator
    {
        public static IBundle Bundle { get; private set; }
        public void Start(IBundleContext context)
        {
            Bundle = context.Bundle;
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
