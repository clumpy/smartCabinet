using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UIShell.WpfShellPlugin.ExtensionModel
{
    public abstract class XmlObject
    {
        public void ThrowIfXmlTypeNotMatch(XmlNode node)
        {
            if (!node.LocalName.Equals(GetType().Name.Replace("Data", string.Empty)))
            {
                throw new Exception(string.Format("The type '{0}' can not be parsed from xml '{1}'.", GetType().Name, node.InnerXml));
            }
        }

        public string GetAttribute(string attr, XmlNode node)
        {
            string value = string.Empty;
            if (node.Attributes[attr] != null)
            {
                value = node.Attributes[attr].Value;
            }
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return value;
        }

        public string GetAttributeAndThrowIfEmpty(string attr, XmlNode node)
        {
            string value = GetAttribute(attr, node);

            if (string.IsNullOrEmpty(value))
            {
                throw new Exception(string.Format("The attribute '{0}' can not be empty in xml node '{1}'.", attr, node.InnerXml));
            }

            return value;
        }

        public abstract void FromXml(XmlNode node);
    }
}
