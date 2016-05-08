using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UIShell.OSGi;

namespace UIShell.WpfShellPlugin.ExtensionModel
{
    /// <summary>
    /// 该类是以下扩展点对应的模型，这个界面模型实现了三级菜单。
    /// <Extension Point="***">
    ///   <LinkGroup DisplayName="" DefaultContentSource=""> // 一级菜单
    ///     <Link DisplayName="" Source="" /> // 二级菜单
    ///     <TabLink DisplayName="" DefaultContentSource="" Layout="Tab">
    ///       <Link DisplayName="" Source="" /> // 三级菜单
    ///     </TabLink>
    ///     <TabLink DisplayName="" DefaultContentSource="" Layout="List">
    ///       <Link DisplayName="" Source="" />
    ///     </TabLink>
    ///   </LinkGroup>
    /// </Extension>
    /// </summary>
    public class LinkGroupData : LinkBase
    {
        public const string DefaultContentSourceAttribute = "DefaultContentSource";
        public const string GroupNameAttribute = "GroupName";
        public const string IsTitleLinkAttribute = "IsTitleLink";
        public string DefaultContentSource { get; set; }
        public List<LinkBase> Links { get; set; }
        public Extension Extension { get; private set; }
        public bool IsTitleLink { get; set; }
        public string GroupName { get; set; }

        public LinkGroupData(Extension extension) 
            : base(extension.Owner)
        {
            Extension = extension;
            Links = new List<LinkBase>();
        }

        public override void FromXml(XmlNode node)
        {
            base.FromXml(node);
            GroupName = GetAttribute(GroupNameAttribute, node);
            DefaultContentSource = GetAttributeAndThrowIfEmpty(DefaultContentSourceAttribute, node);
            string isTitleLink = GetAttribute(IsTitleLinkAttribute, node);
            IsTitleLink = isTitleLink.Trim().ToLower().Equals("true");

            Links = new List<LinkBase>();
            LinkBase link;
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child is XmlComment)
                {
                    continue;
                }
                if (child.LocalName.Equals(typeof(LinkData).Name.Replace("Data",string.Empty)))
                {
                    link = new LinkData(Owner);
                }
                else if (child.LocalName.Equals(typeof(TabLinkData).Name.Replace("Data", string.Empty)))
                {
                    link = new TabLinkData(Owner);
                }
                else
                {
                    continue;
                }

                link.FromXml(child);
                Links.Add(link);
            }
        }
    }
}
