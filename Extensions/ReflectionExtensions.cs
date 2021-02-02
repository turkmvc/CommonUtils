using CommonUtils.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CommonUtils.Extensions
{
    public static class ReflectionExtensions
    {
        public static Func<object, string> OnEnumGetDisplayName { get; set; }
        public static PropertyInfo GetMemberProtertyByPath(this Type type, string content)
        {
            content = content.Trim();
            if (string.IsNullOrEmpty(content)) return null;
            string[] splitContents = content.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitContents == null || splitContents.Length <= 0) return null;
            Type currenttype = type;
            PropertyInfo lastProp = null;
            for (int i = 0; i < splitContents.Length; i++)
            {
                lastProp = currenttype.GetProperty(splitContents[i]);
                if (lastProp == null) break;
                currenttype = lastProp.PropertyType;
            }
            return lastProp;
        }
        public static PropertyInfo GetMemberProtertyByPath(this object obj, string content)
        {
            return obj.GetType().GetMemberProtertyByPath(content);
        }
        public static object GetMemberValueByPath(this object obj, string content, bool returndisplayifenum = false)
        {
            content = content.Trim();
            if (string.IsNullOrEmpty(content)) return null;
            string[] splitContents = content.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitContents == null || splitContents.Length <= 0) return null;
            Object currentobj = obj;
            if (currentobj == null) return null;
            Type currentype = currentobj.GetType();
            for (int i = 0; i < splitContents.Length; i++)
            {
                PropertyInfo PI = currentype.GetProperty(splitContents[i]);
                if (PI != null)
                {
                    currentobj = PI.GetValue(currentobj, null);
                    if (currentobj == null) return null;
                    currentype = currentobj.GetType();
                }
                FieldInfo FI = null;
                if (PI == null)
                {

                    FI = currentype.GetField(splitContents[i]);
                    if (FI != null)
                    {
                        currentobj = FI.GetValue(currentobj);
                        if (currentobj == null) return null;
                        currentype = currentobj.GetType();
                    }
                }
                if (FI == null && PI == null) return null;
                if (i == splitContents.Length - 1)
                {
                    if (returndisplayifenum && currentobj is Enum)
                    {
                        string disp = null;
                        if(OnEnumGetDisplayName!= null)
                        {
                            disp = OnEnumGetDisplayName(currentobj);
                        }
                        if (disp != null)
                        {
                            return disp;
                        }
                        var desc = ReflectUtil.GetEnumDescription(currentobj);
                        if (desc != null)
                        {
                            return desc;
                        }
                    }
                    return currentobj;
                }
            }
            return null;
        }
    }
}
