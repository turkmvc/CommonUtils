using CommonUtils.Attributes;
using CommonUtils.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommonUtils.Util
{
    public static class ReflectUtil
    {
        public static Func<Type, Type, bool> CompareType { get; set; }
        //source target, member
        public static Func<object, object, MemberInfo, bool> OnMemberSet { get; set; }

        public static void SetSameClassValueWithVariants(object source, object target, params string[] variats)
        {
            SetSameClassValue(source, target, true, true, false, false, variats);
        }
        public static void SetSameClassValue(object source, object target, bool setvirtualvariable = false, bool setsubclassvariable = false, bool listisdisallow = true, bool crossnullvalue = true, string[] variants = null)
        {
            if (source == null || target == null) return;
            if(CompareType != null)
            {
                if (!CompareType(source.GetType(), target.GetType())) return;
            }
            Type sourceType = source.GetType();
            MemberInfo[] allMembers = sourceType.GetMembers().Where(e => e.MemberType == MemberTypes.Property || e.MemberType == MemberTypes.Field).ToArray();
            List<MemberInfo> memberInfos = new List<MemberInfo>();
            for (int i = 0; i < allMembers.Length; i++)
            {
                if (variants != null)
                {
                    int index = Array.FindIndex(variants, (e) =>
                    {
                        if (e == allMembers[i].Name || e.StartsWith(allMembers[i].Name + "."))
                            return true;
                        return false;
                    });
                    //index = Array.IndexOf(variants, searchnamepref + allMembers[i].Name) ;
                    if (index >= 0)
                    {
                        if (listisdisallow)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!listisdisallow)
                        {
                            continue;
                        }
                    }
                }

                memberInfos.Add(allMembers[i]);
            }
            Array.Clear(allMembers, 0, allMembers.Length);


            foreach (var member in memberInfos)
            {
                if (member.MemberType != MemberTypes.Field && member.MemberType != MemberTypes.Property) continue;
                if (OnMemberSet != null)
                {
                    if (!OnMemberSet(source, target, member)) continue;
                }
                var attrib = member.GetCustomAttributes(typeof(TransferToAttribute), true)?.FirstOrDefault() as TransferToAttribute ?? TransferToAttribute.GetDefault();
                if (attrib.IsDefault)
                {
                    attrib.CrossNullValue = crossnullvalue;
                }
                if (attrib.NoTransfer) continue;
                bool canwrite = false;
                object curvalue = null;
                object targtvalue = null;
                PropertyInfo pi = null;
                FieldInfo fi = null;

                if (member.MemberType == MemberTypes.Property)
                {
                    pi = member as PropertyInfo;
                    if (pi.CanRead)
                    {
                        if (pi.CanWrite)
                        {
                            if (pi.GetGetMethod().IsVirtual && !setvirtualvariable)
                            {
                                continue;
                            }
                            canwrite = true;

                        }
                        curvalue = pi.GetValue(source, null);
                        targtvalue = pi.GetValue(target, null);


                    }
                }
                else if (member.MemberType == MemberTypes.Field)
                {
                    fi = member as FieldInfo;
                    curvalue = fi.GetValue(source);
                    targtvalue = fi.GetValue(target);
                    canwrite = true;
                }

                if (curvalue != null && targtvalue != null && setsubclassvariable && (!curvalue.GetType().IsValueType) && curvalue.GetType().Name != "String")
                {
                    List<string> SubClsList = new List<string>();
                    for (int i = 0; i < variants.Length; i++)
                    {
                        if (variants[i].StartsWith(member.Name + "."))
                        {
                            SubClsList.Add(variants[i].Split('.')[1]);
                        }
                    }
                    SetSameClassValue(curvalue, targtvalue, setvirtualvariable, setsubclassvariable, listisdisallow, crossnullvalue, SubClsList.ToArray());
                }
                else
                {
                    if (canwrite)
                    {
                        if (curvalue == null && attrib.CrossNullValue)
                        {
                            continue;
                        }
                        if (pi != null)
                        {
                            pi.SetValue(target, curvalue, null);
                        }
                        if (fi != null)
                        {
                            fi.SetValue(target, curvalue);
                        }

                    }

                }
            }
        }
        public static IDictionary<string, object> AnonymToDictionary(object anonymObject)
        {
            IDictionary<string, object> list;
            if (anonymObject != null)
            {
                list = anonymObject.GetType()
                    .GetProperties()
                    .ToDictionary(e => e.Name, e => e.GetValue(anonymObject, null));
            }
            else
            {
                list = new Dictionary<string, object>();
            }
            return list;
        }
        public static string GetEnumDescription(object value)
        {
           
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            else
                return value.ToString();
        }
        public static bool EnumCanShow(object value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            EditorBrowsableAttribute[] attributes = (EditorBrowsableAttribute[])fi.GetCustomAttributes(typeof(EditorBrowsableAttribute), false);

            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].State != EditorBrowsableState.Never;
            else
                return true;
        }
        public static List<T> GetEnumerableOfType<T>(Type[] objecttypes, params object[] constructorArgs) where T : class
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                if (objecttypes != null && type.GetConstructor(objecttypes) == null)
                {
                    continue;
                }
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            return objects;
        }
		// Token: 0x0600005B RID: 91 RVA: 0x00005BF0 File Offset: 0x00003DF0
		public static object MatchType(object obj, Type type)
		{
			Type otype = typeof(object);

			if (obj != null)
			{
				otype = obj.GetType();
			}
			bool flag2 = type == typeof(object) || type == otype;
			object result;
			if (flag2)
			{
				result = obj;
			}
			else
			{
				bool flag3 = type == typeof(string);
				if (flag3)
				{
					result = obj.ToString();
				}
				else
				{
					bool flag4 = type == typeof(bool);
					if (flag4)
					{
						string str;
						bool flag5 = (str = (obj as string)) != null;
						if (flag5)
						{
							bool flag6 = str == "true";
							if (flag6)
							{
								return true;
							}
							bool flag7 = str == "false";
							if (flag7)
							{
								return false;
							}
							bool flag8 = str.IsNumeric();
							if (flag8)
							{
								double numobj = double.Parse(str);
								bool flag9 = numobj > 0.0;
								if (flag9)
								{
									return true;
								}
								return false;
							}
						}
						else
						{
							bool flag10 = ReflectUtil.IsNumericType(obj);
							if (flag10)
							{
								double numboj = (double)Convert.ChangeType(obj, TypeCode.Double);
								bool flag11 = numboj > 0.0;
								if (flag11)
								{
									return true;
								}
								return false;
							}
						}
					}
					TypeConverter converter = TypeDescriptor.GetConverter(otype);
					bool flag12 = converter.CanConvertTo(type);
					if (flag12)
					{
						result = converter.ConvertTo(obj, type);
					}
					else
					{
						bool isValueType = type.IsValueType;
						if (isValueType)
						{
							result = Activator.CreateInstance(type);
						}
						else
						{
							result = null;
						}
					}
				}
			}
			return result;
		}
		public static object[] MatchParams(object[] @params, ParameterInfo[] method_Params)
		{
			List<object> convertedParams = new List<object>();
			for (int i = 0; i < method_Params.Length; i++)
			{
				ParameterInfo cparam = method_Params[i];
				object callingParam = @params.ElementAtOrDefault(i);
				convertedParams.Add(ReflectUtil.MatchType(callingParam, cparam.ParameterType));
			}
			return convertedParams.ToArray();
		}
		public static bool IsArray(object obj)
		{
			Type type;
			bool flag = (type = (obj as Type)) != null;
			bool result;
			if (flag)
			{
				result = (type == typeof(IList) || type.GetInterface("IList") != null || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)));
			}
			else
			{
				result = (obj is IList);
			}
			return result;
		}
		public static bool IsObject(object obj)
		{
			bool flag = obj == null || ReflectUtil.IsArray(obj) || ReflectUtil.IsDictionary(obj) || ReflectUtil.IsEnum(obj);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Type type;
				bool flag2 = (type = (obj as Type)) != null;
				TypeCode code;
				if (flag2)
				{
					code = Type.GetTypeCode(type);
				}
				else
				{
					code = Type.GetTypeCode(obj.GetType());
				}
				bool flag3 = code == TypeCode.Object;
				result = flag3;
			}
			return result;
		}
		public static bool IsNumericType(object obj)
		{
			bool flag = obj == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Type type;
				bool flag2 = (type = (obj as Type)) != null;
				TypeCode code;
				if (flag2)
				{
					code = Type.GetTypeCode(type);
				}
				else
				{
					code = Type.GetTypeCode(obj.GetType());
				}
				TypeCode typeCode = code;
				result = (typeCode - TypeCode.SByte <= 10);
			}
			return result;
		}
		public static bool IsEnum(object obj)
		{
			bool flag = obj == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				Type type;
				bool flag2 = (type = (obj as Type)) != null;
				if (flag2)
				{
					result = type.IsEnum;
				}
				else
				{
					Type otype = obj.GetType();
					result = otype.IsEnum;
				}
			}
			return result;
		}
		public static bool IsDictionary(object obj)
		{
			Type type;
			bool flag = (type = (obj as Type)) != null;
			bool result;
			if (flag)
			{
				result = (type == typeof(IDictionary) || type.GetInterface("IDictionary") != null || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>)));
			}
			else
			{
				result = (obj is IDictionary || obj is IDictionary<string, object>);
			}
			return result;
		}
	}
}
