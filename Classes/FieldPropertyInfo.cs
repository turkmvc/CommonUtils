using System;
using System.Reflection;

namespace CommonUtils.Classes
{	public class FieldPropertyInfo
	{
		private MemberInfo Member { get; set; }
		public FieldPropertyInfo(MemberInfo member)
		{
			this.Member = member;
		}
		public Type ObjectType
		{
			get
			{
				PropertyInfo property;
				Type result;
				if ((property = (this.Member as PropertyInfo)) != null)
				{
					result = property.PropertyType;
				}
				else
				{
					FieldInfo field;
					if ((field = (this.Member as FieldInfo)) != null)
					{
						result = field.FieldType;
					}
					else
					{
						result = null;
					}
				}
				return result;
			}
		}
		public bool CanRead
		{
			get
			{
				PropertyInfo property;
				bool flag = (property = (this.Member as PropertyInfo)) != null;
				return !flag || property.CanRead;
			}
		}
		public bool CanWrite
		{
			get
			{
				PropertyInfo property;
				bool flag = (property = (this.Member as PropertyInfo)) != null;
				return !flag || property.CanWrite;
			}
		}
		public object GetValue(object item)
		{
			PropertyInfo info;
			object result;
			if ((info = (this.Member as PropertyInfo)) != null)
			{
				result = info.GetValue(item);
			}
			else
			{
				FieldInfo field;
				bool flag2 = (field = (this.Member as FieldInfo)) != null;
				if (flag2)
				{
					result = field.GetValue(item);
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		public void SetValue(object item, object value)
		{
			PropertyInfo info;
			if ((info = (this.Member as PropertyInfo)) != null)
			{
				info.SetValue(item, value);
			}
			else
			{
				FieldInfo field;
				if ((field = (this.Member as FieldInfo)) != null)
				{
					field.SetValue(item, value);
				}
			}
		}
		public ParameterInfo[] GetIndexParameters()
		{
			PropertyInfo info;
			ParameterInfo[] result;
			if ((info = (this.Member as PropertyInfo)) != null)
			{
				result = info.GetIndexParameters();
			}
			else
			{
				result = new ParameterInfo[0];
			}
			return result;
		}
		public bool HasIndexParameters()
		{
			PropertyInfo info;
			return (info = (this.Member as PropertyInfo)) != null && info.GetIndexParameters().Length != 0;
		}
	}
}
