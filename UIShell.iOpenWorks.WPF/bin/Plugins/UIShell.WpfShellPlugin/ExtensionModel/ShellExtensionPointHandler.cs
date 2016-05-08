using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Xml;
using UIShell.OSGi;

namespace UIShell.WpfShellPlugin.ExtensionModel
{
    public class ShellExtensionPointHandler
    {
        public const string ExtensionPointName = "UIShell.WpfShellPlugin.LinkGroups";

        public IBundle Bundle { get; private set; }
        public ObservableCollection<LinkGroupData> LinkGroups { get; private set; }

        public ShellExtensionPointHandler(IBundle bundle)
        {
            Bundle = bundle;
            InitExtensions();
            if (Bundle.Context != null)
            {
                Bundle.Context.ExtensionChanged += Context_ExtensionChanged;
            }
        }

        void InitExtensions() // Init
        {
            if (Bundle.Context == null)
            {
                return;
            }
            // Get all extensions.
            var extensions = Bundle.Context.GetExtensions(ExtensionPointName);
            LinkGroups = new ObservableCollection<LinkGroupData>();

            // Convert extensions to LinkGroupData collection.
            foreach (var extension in extensions)
            {
                AddExtension(extension);
            }
        }

        // Handle ExtensionChanged event.
        void Context_ExtensionChanged(object sender, ExtensionEventArgs e)
        {
            if (e.ExtensionPoint.Equals(ExtensionPointName))
            {
                // Create LinkGroupData objects for new Extension.
                if (e.Action == CollectionChangedAction.Add)
                {
                    AddExtension(e.Extension);
                }
                else // Remove LinkGroupData objects respond to the Extension.
                {
                    RemoveExtension(e.Extension);
                }
            }
        }

        // Convert Extension to LinkGroupData instances.
        void AddExtension(Extension extension)
        {
            LinkGroupData linkGroup;
            foreach (XmlNode node in extension.Data)
            {
                if (node is XmlComment)
                {
                    continue;
                }
                linkGroup = new LinkGroupData(extension);
                linkGroup.FromXml(node);
                LinkGroups.Add(linkGroup);
            }
        }
        // Remove LinkGroupData instances of the Extension.
        void RemoveExtension(Extension extension)
        {
            var toBeRemoved = new List<LinkGroupData>();
            foreach (var linkGroup in LinkGroups)
            {
                if (linkGroup.Extension.Equals(extension))
                {
                    toBeRemoved.Add(linkGroup);
                }
            }
            foreach (var linkGroup in toBeRemoved)
            {
                LinkGroups.Remove(linkGroup);
            }
        }
    }
}
