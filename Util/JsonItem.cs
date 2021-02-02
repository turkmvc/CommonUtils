using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using CommonUtils.Attributes;
using CommonUtils.Classes;
using CommonUtils.Extensions;

namespace CommonUtils.Util
{
    public class JsonItem : DynamicObject
    {
        public void ExportTo<T>(ref T obj) where T : class
        {
            bool flag = obj == null;
            if (!flag)
            {
                if (!obj.IsObject())
                {
                    bool flag3 = this.ObjectType == JsonItemType.OBJ_ARRAY || this.ObjectType == JsonItemType.OBJ_OBJECT;
                    if (flag3)
                    {
                        object tempobj = null;
                        if (this.ObjectType == JsonItemType.OBJ_ARRAY)
                        {
                            bool flag5 = obj.IsArray();
                            if (flag5)
                            {
                                bool flag6 = obj is Array;
                                if (flag6)
                                {
                                    Array array = obj as Array;
                                    bool flag7 = array.Length < this.SubItems.Count;
                                    if (flag7)
                                    {
                                        JsonItem.Resize(ref array, this.SubItems.Count);
                                    }
                                    obj = (array as T);
                                }
                            }
                            IDictionary idict;
                            bool flag8 = (idict = (obj as IDictionary)) != null;
                            if (flag8)
                            {
                                object[] nobj = new object[this.SubItems.Count];
                                idict.Add(this.Name, nobj);
                                tempobj = nobj;
                            }
                            else
                            {
                                IDictionary<string, object> ndict;
                                if ((ndict = (obj as IDictionary<string, object>)) != null)
                                {
                                    Dictionary<string, object> nobj2 = new Dictionary<string, object>();
                                    ndict.Add(this.Name, nobj2);
                                    tempobj = nobj2;
                                }
                            }
                        }
                        else
                        {
                            if (obj is Array)
                            {
                                IList arr = obj as IList;
                                Dictionary<string, object> nobj3 = new Dictionary<string, object>();
                                tempobj = nobj3;
                                arr[this.Index] = ReflectUtil.MatchType(nobj3, obj.GetType().GetElementType());
                            }
                            bool flag11 = this.Parent != null;
                            if (flag11)
                            {
                                IDictionary idict2;
                                if ((idict2 = (obj as IDictionary)) != null)
                                {
                                    Type otypee = obj.GetType();
                                    Type gentype = otypee.GetGenericArguments().ElementAt(1);
                                    bool flag13 = gentype != typeof(object);
                                    if (flag13)
                                    {
                                        return;
                                    }
                                    Type listType = typeof(Dictionary<,>);
                                    Type constructedListType = listType.MakeGenericType(otypee.GetGenericArguments());
                                    object nobj4 = Activator.CreateInstance(constructedListType);
                                    idict2.Add(this.Name, nobj4);
                                    tempobj = nobj4;
                                }
                                else
                                {
                                    IDictionary<string, object> ndict2;
                                    bool flag14 = (ndict2 = (obj as IDictionary<string, object>)) != null;
                                    if (flag14)
                                    {
                                        Dictionary<string, object> nobj5 = new Dictionary<string, object>();
                                        ndict2.Add(this.Name, nobj5);
                                        tempobj = nobj5;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < this.SubItems.Count; i++)
                        {
                            JsonItem item = this.SubItems[i];
                            bool flag15 = tempobj != null;
                            if (flag15)
                            {
                                item.ExportTo<object>(ref tempobj);
                            }
                            else
                            {
                                item.ExportTo<T>(ref obj);
                            }
                        }
                    }
                    else
                    {
                        bool flag16 = this.ObjectType == JsonItemType.OBJ_ARRAYITEM;
                        if (flag16)
                        {
                            IList list;
                            bool flag17 = (list = (obj as IList)) != null;
                            if (flag17)
                            {
                                Type ntype = list.GetType();
                                bool isGenericType = ntype.IsGenericType;
                                Type ltype;
                                if (isGenericType)
                                {
                                    ltype = ntype.GetGenericArguments().FirstOrDefault<Type>();
                                }
                                else
                                {
                                    ltype = list.GetType().GetElementType();
                                }
                                bool isFixedSize = list.IsFixedSize;
                                if (isFixedSize)
                                {
                                    list[this.Index] = ReflectUtil.MatchType(this.Value, ltype);
                                }
                                else
                                {
                                    list.Add(ReflectUtil.MatchType(this.Value, ltype));
                                }
                            }
                        }
                        else
                        {
                            if (this.ObjectType == JsonItemType.OBJ_VARIANT)
                            {
                                T t = obj;
                                Type otype = t?.GetType();
                                IDictionary idict3;
                                bool flag19 = (idict3 = (obj as IDictionary)) != null;
                                if (flag19)
                                {
                                    Type gentype2 = otype.GetGenericArguments().ElementAt(1);
                                    idict3.Add(this.Name, ReflectUtil.MatchType(this.Value, gentype2));
                                }
                                else
                                {
                                    IDictionary<string, object> sdict;
                                    if ((sdict = (obj as IDictionary<string, object>)) != null)
                                    {
                                        sdict.Add(this.Name, this.Value);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    bool flag21 = this.ObjectType != JsonItemType.OBJ_OBJECT;
                    if (!flag21)
                    {
                        Type otype2 = obj.GetType();
                        MemberInfo[] members = otype2.GetMembers(BindingFlags.Instance | BindingFlags.Public);
                        foreach (MemberInfo member in members)
                        {
                            bool flag22 = member.MemberType != MemberTypes.Field && member.MemberType != MemberTypes.Property;
                            if (!flag22)
                            {
                                string name = member.Name;
                                JsonAttribute jattribut;
                                if ((jattribut = (member.GetCustomAttribute(typeof(JsonAttribute)) as JsonAttribute)) != null)
                                {
                                    bool notMapped = jattribut.NotMapped;
                                    if (notMapped)
                                    {
                                        goto IL_7E6;
                                    }
                                    bool flag24 = !string.IsNullOrEmpty(jattribut.TagName);
                                    if (flag24)
                                    {
                                        name = jattribut.TagName;
                                    }
                                }
                                JsonItem item2 = this.Find(name);
                                bool flag25 = item2 == null;
                                if (!flag25)
                                {
                                    FieldPropertyInfo fprop = new FieldPropertyInfo(member);
                                    bool flag26 = !fprop.CanWrite || !fprop.CanRead || fprop.HasIndexParameters();
                                    if (!flag26)
                                    {
                                        if (item2.ObjectType == JsonItemType.OBJ_OBJECT)
                                        {
                                            bool flag28 = !fprop.ObjectType.IsObject() && !fprop.ObjectType.IsDictionary();
                                            if (!flag28)
                                            {
                                                object curvalue = fprop.GetValue(obj);
                                                if (curvalue == null)
                                                {
                                                    if (fprop.ObjectType.IsDictionary())
                                                    {
                                                        Type listType2 = typeof(Dictionary<,>);
                                                        Type constructedListType2 = listType2.MakeGenericType(fprop.ObjectType.GetGenericArguments());
                                                        curvalue = Activator.CreateInstance(constructedListType2);
                                                        for (int j = 0; j < item2.SubItems.Count; j++)
                                                        {
                                                            item2.SubItems[j].ExportTo<object>(ref curvalue);
                                                        }
                                                        fprop.SetValue(obj, curvalue);
                                                        break;
                                                    }
                                                    curvalue = Activator.CreateInstance(fprop.ObjectType);
                                                }
                                                item2.ExportTo<object>(ref curvalue);
                                                fprop.SetValue(obj, curvalue);
                                            }
                                        }
                                        else
                                        {
                                            if (item2.ObjectType == JsonItemType.OBJ_ARRAY)
                                            {
                                                bool flag32 = !fprop.ObjectType.IsArray();
                                                if (!flag32)
                                                {
                                                    object curvalue2 = fprop.GetValue(obj);
                                                    if (curvalue2 == null)
                                                    {
                                                        Type listType3 = typeof(List<>);
                                                        bool isGenericType2 = fprop.ObjectType.IsGenericType;
                                                        if (isGenericType2)
                                                        {
                                                            Type constructedListType3 = listType3.MakeGenericType(fprop.ObjectType.GetGenericArguments());
                                                            curvalue2 = Activator.CreateInstance(constructedListType3);
                                                        }
                                                        else
                                                        {
                                                            Type elemtype = fprop.ObjectType.GetElementType();
                                                            if (elemtype != null)
                                                            {
                                                                curvalue2 = Array.CreateInstance(elemtype, item2.SubItems.Count);
                                                            }
                                                        }
                                                    }
                                                    item2.ExportTo<object>(ref curvalue2);
                                                    fprop.SetValue(obj, curvalue2);
                                                }
                                            }
                                            else
                                            {
                                                if (!fprop.ObjectType.IsObject() && !member.MemberType.IsArray())
                                                {
                                                    fprop.SetValue(obj, ReflectUtil.MatchType(item2.Value, fprop.ObjectType));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            IL_7E6:;
                        }
                    }
                }
            }
        }
        private static void Resize(ref Array array, int newSize)
        {
            Type elementType = array.GetType().GetElementType();
            Array newArray = Array.CreateInstance(elementType, newSize);
            Array.Copy(array, newArray, Math.Min(array.Length, newArray.Length));
            array = newArray;
        }
        public object this[int index]
        {
            get
            {
                if (this.ObjectType == JsonItemType.OBJ_ARRAY)
                {
                    if (index < 0 || index > this.SubItems.Count)
                    {
                        return null;
                    }
                    JsonItem jitem = this.SubItems[index];
                    bool flag3 = jitem != null;
                    if (flag3)
                    {
                        return jitem.Value;
                    }
                }
                return null;
            }
            set
            {
                if (this.ObjectType == JsonItemType.OBJ_ARRAY)
                {
                    bool flag2 = index < 0 || index > this.SubItems.Count;
                    if (!flag2)
                    {
                        JsonItem jitem = this.SubItems[index];
                        bool flag3 = jitem != null;
                        if (flag3)
                        {
                            jitem.Value = value;
                        }
                    }
                }
            }
        }
        public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result)
        {
            return base.TryCreateInstance(binder, args, out result);
        }
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            return base.TryGetIndex(binder, indexes, out result);
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            JsonItem jitem = this.Find(binder.Name);
            bool flag = jitem != null;
            if (flag)
            {
                jitem.Value = value;
            }
            else
            {
                if(value is JsonItem jsitem)
                {
                    jsitem.Name = binder.Name;
                    jsitem.Parent = this;
                    jsitem.Index = this.SubItems.Count;
                    this.SubItems.Add(jsitem);
                }
                else
                {
                    jitem = new JsonItem
                    {
                        Parent = this,
                        Name = binder.Name
                    };
                    JsonDecoder.DecodeFrom(value, jitem);
                    jitem.Index = this.SubItems.Count;
                    this.SubItems.Add(jitem);
                }

            }
            return true;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            JsonItem jitem = this.Find(binder.Name);
            bool flag = jitem == null;
            bool result2;
            if (flag)
            {
                result = null;
                result2 = true;
            }
            else
            {
                bool flag2 = jitem.ObjectType == JsonItemType.OBJ_ARRAY || jitem.ObjectType == JsonItemType.OBJ_OBJECT;
                if (flag2)
                {
                    result = jitem;
                }
                else
                {
                    result = jitem.Value;
                }
                result2 = true;
            }
            return result2;
        }
        public JsonItem()
        {
            this.SubItems = new List<JsonItem>();
            this.ObjectType = JsonItemType.OBJ_OBJECT;
        }
        public bool IsParse { get; set; }
        public JsonItem Parent { get; set; }
        public JsonItemType ObjectType { get; set; }
        public string Name { get; set; }
        public object Value
        {
            get
            {
                return this.jvalue;
            }
            set
            {
                if (this.ObjectType != JsonItemType.OBJ_ARRAY && this.ObjectType != JsonItemType.OBJ_OBJECT)
                {
                    this.SubItems.Clear();
                }
                string str;
                bool flag2 = (str = (value as string)) != null && this.ValueQuot == '\0';
                if (flag2)
                {
                    bool flag3 = str.ToLower() == "true";
                    if (flag3)
                    {
                        value = true;
                    }
                    else
                    {
                        bool flag4 = str.ToLower() == "false";
                        if (flag4)
                        {
                            value = false;
                        }
                        else
                        {
                            bool flag5 = str == "null";
                            if (flag5)
                            {
                                value = null;
                            }
                            else
                            {
                                if (double.TryParse(str, out double dbl))
                                {
                                    value = dbl;
                                }
                            }
                        }
                    }
                }
                if (value is IDictionary || value.IsObject() || value.IsArray())
                {
                    JsonDecoder.DecodeFrom(value, this);
                    value = null;
                }
                this.jvalue = value;
            }
        }
        public int LineOffset { get; set; }
        public int Index { get; set; }
        public List<JsonItem> SubItems { get; set; }
        public char NameQuot { get; set; } = '"';
        public char ValueQuot { get; set; } = '"';
        public string ToJson()
        {
            return JsonEncoder.Encode(this, 2, true);
        }
        public void AddSubItem(JsonItem item)
        {

            if(item.ObjectType == JsonItemType.OBJ_OBJECT)
            {
                for (int i = 0; i < item.SubItems.Count; i++)
                {
                    item.SubItems[i].Index = this.SubItems.Count;
                    this.SubItems.Add(item.SubItems[i]);
                    item.SubItems[i].Parent = this;
                }
            }
            else
            {
                item.Parent = this;
                this.SubItems.Add(item);
                item.Index = this.SubItems.Count - 1;
            }
         
        }
        public string GetValueWithVars()
        {
            object value = this.Value;
            string result = value?.ToString().Replace("\\", "\\\\");
            bool flag = this.ValueQuot > '\0';
            if (flag)
            {
                result = result.Replace(this.NameQuot.ToString(), "\\" + this.ValueQuot.ToString());
            }
            return result;
        }
        public string GetNameWithVars()
        {
            string result = this.Name.Replace("\\", "\\\\");
            bool flag = this.NameQuot > '\0';
            if (flag)
            {
                result = result.Replace(this.NameQuot.ToString(), "\\" + this.NameQuot.ToString());
            }
            return result;
        }
        public override string ToString()
        {
            bool flag = this.ObjectType == JsonItemType.OBJ_ARRAYITEM || this.ObjectType == JsonItemType.OBJ_SINGLE;
            string result;
            if (flag)
            {
                result = "\"" + this.Value + "\"";
            }
            else
            {
                bool flag2 = this.ObjectType == JsonItemType.OBJ_VARIANT;
                if (flag2)
                {
                    result = string.Concat(new object[]
                    {
                        "\"",
                        this.Name,
                        "\": \"",
                        this.Value,
                        "\""
                    });
                }
                else
                {
                    bool flag3 = this.ObjectType == JsonItemType.OBJ_ARRAY || this.ObjectType == JsonItemType.OBJ_OBJECT;
                    if (flag3)
                    {
                        result = this.ToJson();
                    }
                    else
                    {
                        bool flag4 = this.ObjectType == JsonItemType.OBJ_COMMENT_SINGLELINE;
                        if (flag4)
                        {
                            result = "//" + this.Value;
                        }
                        else
                        {
                            bool flag5 = this.ObjectType == JsonItemType.OBJ_COMMENT_MULTILINE;
                            if (flag5)
                            {
                                result = "/*" + this.Value + "*/";
                            }
                            else
                            {
                                result = "Undefined";
                            }
                        }
                    }
                }
            }
            return result;
        }
        public JsonItem Evulate(string evulator)
        {
            string[] alls = evulator.Split(new char[]
            {
                '.'
            });
            JsonItem item = this;
            for (int i = 0; i < alls.Length; i++)
            {
                item = item.Find(alls[i]);
                bool flag = item == null;
                if (flag)
                {
                    return null;
                }
            }
            return item;
        }
        public JsonItem Find(string name)
        {
            bool flag = this.ObjectType != JsonItemType.OBJ_ARRAY && this.ObjectType != JsonItemType.OBJ_OBJECT;
            JsonItem result2;
            if (flag)
            {
                result2 = null;
            }
            else
            {
                Match match = Regex.Match(name, "([\\w\\d]+)?\\[(\\d+|l|f|L|F)\\]$");
                int arrayIndex = -1;
                bool success = match.Success;
                string newname;
                if (success)
                {
                    string result = match.Result("${2}");
                    if (result != "f" && result != "F" && result != "l" && result != "L")
                    {
                        arrayIndex = int.Parse(result);
                    }
                    else
                    {
                        if (result == "f" || result == "F")
                        {
                            arrayIndex = 0;
                        }
                        else
                        {
                            arrayIndex = -2;
                        }
                    }
                    newname = match.Result("${1}");
                    if (newname == "${1}")
                    {
                        newname = null;
                    }
                }
                else
                {
                    newname = name;
                }
                if (newname == null)
                {
                    if (this.ObjectType != JsonItemType.OBJ_ARRAY || arrayIndex >= this.SubItems.Count)
                    {
                        result2 = null;
                    }
                    else
                    {
                        if (arrayIndex == -2)
                        {
                            arrayIndex = this.SubItems.Count - 1;
                        }
                        if (arrayIndex < 0)
                        {
                            result2 = null;
                        }
                        else
                        {
                            result2 = this.SubItems[arrayIndex];
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < this.SubItems.Count; i++)
                    {
                        JsonItem sub = this.SubItems[i];
                        bool flag9 = sub.ObjectType != JsonItemType.OBJ_ARRAYITEM && sub.ObjectType != JsonItemType.OBJ_OBJECT && sub.ObjectType != JsonItemType.OBJ_ARRAY && sub.ObjectType != JsonItemType.OBJ_VARIANT;
                        if (!flag9)
                        {
                            if (sub.ObjectType == JsonItemType.OBJ_ARRAY && sub.Name == newname)
                            {
                                if (arrayIndex == -2)
                                {
                                    arrayIndex = sub.SubItems.Count - 1;
                                }
                                bool flag12 = arrayIndex >= 0;
                                if (!flag12)
                                {
                                    return sub;
                                }
                                if (arrayIndex >= sub.SubItems.Count)
                                {
                                    return null;
                                }
                                return sub.SubItems[arrayIndex];
                            }
                            else
                            {
                                if (sub.Name == name)
                                {
                                    return sub;
                                }
                            }
                        }
                    }
                    result2 = null;
                }
            }
            return result2;
        }
        public static implicit operator JsonItem(string v)
        {
            return JsonDecoder.Decode(v, false);
        }
        public static implicit operator string(JsonItem v)
        {
            string result;
            if (v == null)
            {
                result = null;
            }
            else
            {
                result = v.ToString();
            }
            return result;
        }
        private object jvalue;
    }
}
