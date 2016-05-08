using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UIShell.OSGi;

namespace UIShell.WpfShellPlugin.ExtensionModel
{
    public class TabLinkData : LinkBase
    {
        public const string LayoutAttribute = "Layout";
        public const string DefaultContentSourceAttribute = "DefaultContentSource";

        public string Layout { get; set; }
        public string DefaultContentSource { get; set; }

        public List<LinkData> Links { get; set; }

        public TabLinkData(IBundle owner)
            : base(owner)
        {
        }

        public override void FromXml(XmlNode node)
        {
            base.FromXml(node);
            Layout = GetAttributeAndThrowIfEmpty(LayoutAttribute, node);
            DefaultContentSource = GetAttributeAndThrowIfEmpty(DefaultContentSourceAttribute, node);

            Links = new List<LinkData>();
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child is XmlComment)
                {
                    continue;
                }

                var link = new LinkData(Owner);
                link.FromXml(child);
                Links.Add(link);
            }
        }
    }
}
