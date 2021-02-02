using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace CommonUtils.Extensions
{
    public static class XElementExtensions
    {
        public static int GetValueInt(this XElement element, string elementName, int defaultValue = 0)
        {
            if (element.Element(elementName) is XElement xElem && xElem.Value != null)
            {
                return xElem.Value.ToInt32(defaultValue);
            }
            return defaultValue;
        }
        public static string GetValue(this XElement element, string elementName, string defaultValue = null)
        {
            if (element.Element(elementName) is XElement xElem)
            {
                return xElem.Value;
            }
            return defaultValue;
        }
        public static int GetAttributeInt(this XElement element, string elementName, int defaultValue = 0)
        {
            if (element.Attribute(elementName) is XAttribute xAttr && xAttr.Value != null)
            {
                return xAttr.Value.ToInt32(defaultValue);
            }
            return defaultValue;
        }
        public static string GetAttribute(this XElement element, string elementName, string defaultValue = null)
        {
            if (element.Attribute(elementName) is XAttribute xAttr)
            {
                return xAttr.Value;
            }
            return defaultValue;
        }
    }
}
