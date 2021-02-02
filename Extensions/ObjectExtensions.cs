using CommonUtils.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.Extensions
{
    public static class ObjectExtensions
    {
        public static void TransferTo(this object source, object target)
        {

            TransferTo(source, target, false);
        }
        public static void TransferTo(this object source, object target, bool crossNullValue)
        {

            TransferTo(source, target, null, true, crossNullValue);
        }
        public static void TransferTo(this object source, object target, string[] excludingorincluding = null, bool listisexcluding = true, bool crossNullvalue = true)
        {
            ReflectUtil.SetSameClassValue(source, target, variants: excludingorincluding, listisdisallow: listisexcluding, crossnullvalue: crossNullvalue);
        }
        public static void TransferValuesTo(this object source, object target, params string[] including)
        {
            ReflectUtil.SetSameClassValue(source, target, variants: including, listisdisallow: false);
        }
		public static bool IsNumericType(this object source)
		{
			return ReflectUtil.IsNumericType(source);
		}
		public static bool IsObject(this object source)
		{
			return ReflectUtil.IsObject(source);
		}
		public static bool IsEnum(this object source)
		{
			return ReflectUtil.IsEnum(source);
		}
		public static bool IsArray(this object source)
		{
			return ReflectUtil.IsArray(source);
		}
		public static bool IsDictionary(this object source)
		{
			return ReflectUtil.IsDictionary(source);
		}
    }
}
