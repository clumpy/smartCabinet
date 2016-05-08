using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UIShell.OSGi;

namespace UIShell.WpfShellPlugin.ExtensionModel
{
    public class LinkData : LinkBase
    {
        public const string SourceAttribute = "Source";
        public string Source { get; set; }

        public LinkData(IBundle owner)
            : base(owner)
        {
        }

        public override void FromXml(XmlNode node)
        {
            base.FromXml(node);
            Source = GetAttributeAndThrowIfEmpty(SourceAttribute, node);
        }
    }
}
