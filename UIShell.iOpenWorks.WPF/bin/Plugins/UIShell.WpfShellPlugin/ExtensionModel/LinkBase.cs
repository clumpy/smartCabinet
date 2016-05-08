using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UIShell.OSGi;

namespace UIShell.WpfShellPlugin.ExtensionModel
{
    public class LinkBase : XmlObject
    {
        public Guid LinkId = Guid.NewGuid();
        public const string DisplayNameAttribute = "DisplayName";
        public string DisplayName { get; set; }
        public IBundle Owner { get; set; }

        public LinkBase(IBundle owner)
        {
            Owner = owner;
        }

        public string FormatSource(string source)
        {
            if (source.Contains('@') || source.ToLower().Contains(".xaml") || source.Contains('/') || source.Contains('\\'))
            {
                return source;
            }
            else
            {
                return string.Format("{0}@{1}", Owner.SymbolicName, source);
            }
        }

        public override void FromXml(System.Xml.XmlNode node)
        {
            ThrowIfXmlTypeNotMatch(node);
            DisplayName = GetAttributeAndThrowIfEmpty(DisplayNameAttribute, node);
        }
    }
}
