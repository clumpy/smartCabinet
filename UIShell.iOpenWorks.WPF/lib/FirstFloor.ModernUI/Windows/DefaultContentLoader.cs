using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UIShell.OSGi;

namespace FirstFloor.ModernUI.Windows
{
    /// <summary>
    /// Loads XAML files using Application.LoadComponent.
    /// </summary>
    public class DefaultContentLoader
        : IContentLoader
    {
        /// <summary>
        /// Asynchronously loads content from specified uri.
        /// </summary>
        /// <param name="uri">The content uri.</param>
        /// <param name="cancellationToken">The token used to cancel the load content task.</param>
        /// <returns>The loaded content.</returns>
        public Task<object> LoadContentAsync(Uri uri, CancellationToken cancellationToken)
        {
            if (!Application.Current.Dispatcher.CheckAccess()) {
               throw new InvalidOperationException(Resources.UIThreadRequired);
            }
            
            // scheduler ensures LoadContent is executed on the current UI thread
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            return Task.Factory.StartNew(() => LoadContent(uri), cancellationToken, TaskCreationOptions.None, scheduler);
        }

        /// <summary>
        /// Loads the content from specified uri.
        /// </summary>
        /// <param name="uri">The content uri</param>
        /// <returns>The loaded content.</returns>
        protected virtual object LoadContent(Uri uri)
        {
            // don't do anything in design mode
            if (ModernUIHelper.IsInDesignMode)
            {
                return null;
            }
            string uriString = string.Empty;
            string paraString = string.Empty;
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (uri.OriginalString.Contains('?'))
            {
                var uriPara = uri.OriginalString.Split('?');
                uriString = uriPara[0];
                paraString = uriPara[1];
                var parameterStrs = paraString.Split('&');
                string[] parameterStrSplitted;
                foreach (var parameterStr in parameterStrs)
                {
                    parameterStrSplitted = parameterStr.Split('=');
                    parameters.Add(parameterStrSplitted[0], parameterStrSplitted[1]);
                }
            }
            else
            {
                uriString = uri.OriginalString;
            }

            object result = null;
            // 1st Format: [BundleSymbolicName]@[Class Full Name]
            if (uriString.Contains('@'))
            {
                var bundleSymbolicNameAndClass = uriString.Split('@');
                if (bundleSymbolicNameAndClass.Length != 2 || string.IsNullOrEmpty(bundleSymbolicNameAndClass[0]) || string.IsNullOrEmpty(bundleSymbolicNameAndClass[1]))
                {
                    throw new Exception("The uri must be in format of '[BundleSymbolicName]@[Class Full Name]'");
                }
                var bundle = BundleRuntime.Instance.Framework.Bundles.GetBundleBySymbolicName(bundleSymbolicNameAndClass[0]);
                if (bundle == null)
                {
                    throw new Exception(string.Format("The uri is not correct since the bunde '{0}' does not exist.", bundleSymbolicNameAndClass[0]));
                }
                var type = bundle.LoadClass(bundleSymbolicNameAndClass[1]);
                if (type == null)
                {
                    throw new Exception(string.Format("The class '{0}' is not found in bunle '{1}'.", bundleSymbolicNameAndClass[1], bundleSymbolicNameAndClass[0]));
                }
                result = Activator.CreateInstance(type);
            }
            // 2nd Format: /[AssemblyName],Version=[Version];component/[XAML relative path]
            else if (string.IsNullOrEmpty(paraString))
            {
                //result = Application.LoadComponent(uri);
            }
            else
            {
                result = Application.LoadComponent(new Uri(uriString, UriKind.RelativeOrAbsolute));
            }

            ApplyProperties(result, parameters);

            return result;
        }

        private void ApplyProperties(object o, Dictionary<string, string> parameters)
        {
            if (o == null || parameters.Count == 0)
            {
                return;
            }

            Type oType = o.GetType();
            PropertyInfo pi = null;
            foreach (var keyPair in parameters)
            {
                pi = oType.GetProperty(keyPair.Key);
                pi.SetValue(o, keyPair.Value, null);
            }
        }
    }
}
