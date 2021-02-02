using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CommonUtils.Extensions
{
    public static class XmlExtensions
    {
        public static int GetValueInt(this XmlNode element, string elementName, int defaultValue = 0)
        {
            if (element.SelectSingleNode(elementName) is XmlNode node)
            {
                return node.Value.ToInt32();
            }
            return defaultValue;
        }
        public static string GetValue(this XmlNode element, string elementName, string defaultValue = null)
        {
            if (element.SelectSingleNode(elementName) is XmlNode node)
            {
                return node.Value;
            }
            return defaultValue;
        }
        public static int GetAttributeInt(this XmlNode element, string elementName, int defaultValue = 0)
        {
            var elem = element.Attributes?[elementName];
            if (elem == null)
            {
                return defaultValue;
            }
            return elem.Value.ToInt32();
        }
        public static string GetAttribute(this XmlNode element, string elementName, string defaultValue = null)
        {
            var elem = element.Attributes?[elementName];
            if (elem == null)
            {
                return defaultValue;
            }
            return elem.Value;
        }
        public static string GetAttributeUntilNotNull(this XmlNode element, string elementName, string defaultValue = null)
        {
            var nextElem = element;
            do
            {
                string val = nextElem.GetAttribute(elementName);
                if (val != null) return val;

            } while ((nextElem = nextElem.ParentNode) != null);
            return defaultValue;
        }
    }
}
