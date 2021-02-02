using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.Attributes
{
    public class JsonAttribute : Attribute
    {
        public string TagName { get; set; }
        public bool NotMapped { get; set; }
    }
}
