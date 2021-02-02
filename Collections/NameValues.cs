using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CommonUtils.Collections
{
    public class NameValue
    {
        HashSet<NameValueItem> innerItems;
        public NameValue()
        {
            innerItems = new HashSet<NameValueItem>();
        }

        public static implicit operator NameValue(string v)
        {
            throw new NotImplementedException();
        }

        public static implicit operator string(NameValue v)
        {
            throw new NotImplementedException();
        }
    } 
    public class NameValueItem
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

}
