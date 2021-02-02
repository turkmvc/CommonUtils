using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.Attributes
{
    public class TransferToAttribute : Attribute
    {
        public TransferToAttribute()
        {
            CrossNullValue = false;
            IsDefault = false;
        }
        public bool NoTransfer { get; set; }
        public bool CrossNullValue { get; set; }
        public bool IsDefault { get; private set; }
        public static TransferToAttribute GetDefault()
        {
            TransferToAttribute transferTo = new TransferToAttribute
            {
                IsDefault = true
            };
            return transferTo;
        }
    }
}
