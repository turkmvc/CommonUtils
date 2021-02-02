using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CommonUtils.Attributes;
using CommonUtils.Extensions;

namespace CommonUtils.Util
{
	public class JsonDecoder
	{
		public bool DecodeUnicode
		{
			get
			{
				return this.decodeUnicode;
			}
			set
			{
				this.decodeUnicode = value;
			}
		}

		private JsonDecoder()
		{
		}
		private JsonItem DecodeJson(string inputText)
		{
			this.input = inputText;
			JsonItem jsonItem = new JsonItem
			{
				NameQuot = '\0',
				ValueQuot = '\0',
				IsParse = true
			};
			char quotchar = '\0';
			bool quoted = false;
			StringBuilder builder = new StringBuilder();
			int i = 0;
			while (i < this.input.Length)
			{
				char cur = this.input[i];
				bool flag = quoted;
				if (flag)
				{
					bool flag2 = quotchar == cur;
					if (flag2)
					{
						jsonItem.ObjectType = JsonItemType.OBJ_SINGLE;
						jsonItem.Value = builder.ToString();
						quoted = false;
						break;
					}
					builder.Append(cur);
				}
				char c = cur;
				if (c <= '"')
				{
					switch (c)
					{
						case '\t':
						case '\n':
						case '\r':
							break;
						case '\v':
						case '\f':
							goto IL_127;
						default:
							if (c != ' ')
							{
								if (c != '"')
								{
									goto IL_127;
								}
								goto IL_D8;
							}
							break;
					}
				}
				else
				{
					if (c == '\'')
					{
						goto IL_D8;
					}
					if (c == '[')
					{
						this.startid = i;
						jsonItem.ObjectType = JsonItemType.OBJ_ARRAY;
						jsonItem.SubItems = this.JsonDecode(JsonItemType.OBJ_NONE, null);
						return jsonItem;
					}
					if (c != '{')
					{
						goto IL_127;
					}
					this.startid = i;
					jsonItem.ObjectType = JsonItemType.OBJ_OBJECT;
					jsonItem.SubItems = this.JsonDecode(JsonItemType.OBJ_NONE, null);
					return jsonItem;
				}
			IL_12D:
				i++;
				continue;
			IL_D8:
				quoted = true;
				quotchar = cur;
				goto IL_12D;
			IL_127:
				return null;
			}
			bool flag3 = quoted;
			if (flag3)
			{
				return null;
			}
			return jsonItem;
		}
		private List<JsonItem> JsonDecode(JsonItemType objtype, JsonItem parent)
		{
			StringBuilder key = new StringBuilder();
			StringBuilder value = new StringBuilder();
			StringBuilder comment = new StringBuilder();
			bool inquot = false;
			bool quoted = false;
			bool isspec = false;
			bool splitterFound = false;
			bool inlinecomment = false;
			bool multilinecomment = false;
			int lineoffset = 0;
			char quotchar = '\0';
			JsonItem lastitem = new JsonItem
			{
				Parent = parent,
				NameQuot = '\0',
				ValueQuot = '\0'
			};
			List<JsonItem> items = new List<JsonItem>();
			List<JsonItem> comments = new List<JsonItem>();
			for (int i = this.startid; i < this.input.Length; i++)
			{
				char cur = this.input[i];
				char next = (i + 1 < this.input.Length) ? this.input[i + 1] : '\0';
				bool flag = inlinecomment || multilinecomment;
				if (flag)
				{
					bool flag2 = inlinecomment;
					if (flag2)
					{
						bool flag3 = cur == '\r' && next == '\n';
						if (flag3)
						{
							goto IL_87B;
						}
					}
					bool flag4 = (inlinecomment && cur == '\n') || (multilinecomment && cur == '*' && next == '/');
					if (flag4)
					{
						bool flag5 = multilinecomment;
						if (flag5)
						{
							i++;
						}
						bool flag6 = !this.skipComment;
						if (flag6)
						{
							JsonItem commentItem = new JsonItem
							{
								LineOffset = lineoffset,
								Value = comment.ToString(),
								ObjectType = (inlinecomment ? JsonItemType.OBJ_COMMENT_SINGLELINE : JsonItemType.OBJ_COMMENT_MULTILINE),
								Parent = parent,
								NameQuot = '\0',
								ValueQuot = '\0',
								Index = items.Count
							};
							bool flag7 = key.Length > 0 || splitterFound;
							if (flag7)
							{
								comments.Add(commentItem);
							}
							else
							{
								items.Add(commentItem);
							}
							comment.Clear();
						}
						inlinecomment = false;
						multilinecomment = false;
					}
					else
					{
						bool flag8 = !this.skipComment;
						if (flag8)
						{
							comment.Append(cur);
						}
					}
				}
				else
				{
					bool flag9 = isspec;
					if (flag9)
					{
						bool flag10 = this.DecodeUnicode && (cur == 'u' || cur == 'U') && i + 5 < this.input.Length;
						if (flag10)
						{
							bool contn = true;
							int[] nums = new int[4];
							for (int j = 1; j < 5; j++)
							{
								char ucur = this.input[i + j];
								bool flag11 = !Uri.IsHexDigit(ucur);
								if (flag11)
								{
									contn = false;
									break;
								}
								nums[j - 1] = Uri.FromHex(ucur);
							}
							bool flag12 = contn;
							if (flag12)
							{
								int formula = nums[0] * 16 * 16 * 16 + nums[1] * 16 * 16 + nums[2] * 16 + nums[3];
								i += 4;
								cur = (char)formula;
							}
						}
						bool flag13 = splitterFound;
						if (flag13)
						{
							value.Append(cur);
						}
						else
						{
							key.Append(cur);
						}
						isspec = false;
					}
					else
					{
						bool flag14 = cur == '\\' && !isspec;
						if (flag14)
						{
							isspec = true;
						}
						else
						{
							bool flag15 = cur == '/';
							if (flag15)
							{
								bool flag16 = next == '/';
								if (flag16)
								{
									inlinecomment = true;
									i++;
									goto IL_87B;
								}
								bool flag17 = next == '*';
								if (flag17)
								{
									multilinecomment = true;
									i++;
									goto IL_87B;
								}
							}
							bool flag18 = inquot;
							if (flag18)
							{
								bool flag19 = cur == quotchar;
								if (flag19)
								{
									inquot = false;
									quoted = true;
									bool flag20 = objtype == JsonItemType.OBJ_ARRAY;
									if (flag20)
									{
										lastitem.ValueQuot = quotchar;
									}
									else
									{
										bool flag21 = objtype == JsonItemType.OBJ_OBJECT;
										if (flag21)
										{
											bool flag22 = splitterFound;
											if (flag22)
											{
												lastitem.ValueQuot = quotchar;
											}
											else
											{
												lastitem.NameQuot = quotchar;
											}
										}
									}
									goto IL_87B;
								}
								bool flag23 = !splitterFound;
								if (flag23)
								{
									key.Append(cur);
								}
								else
								{
									value.Append(cur);
								}
							}
							else
							{
								bool flag24 = cur == '"' || cur == '\'';
								if (flag24)
								{
									quotchar = cur;
									inquot = true;
									goto IL_87B;
								}
							}
							bool flag25 = !inquot;
							if (flag25)
							{
								char c = cur;
								if (c <= ':')
								{
									if (c <= ' ')
									{
										switch (c)
										{
											case '\t':
											case '\n':
											case '\r':
												break;
											case '\v':
											case '\f':
												goto IL_842;
											default:
												if (c != ' ')
												{
													goto IL_842;
												}
												break;
										}
										bool flag26 = cur == '\n' || (cur == '\r' && next != '\n');
										if (flag26)
										{
											lineoffset++;
										}
									}
									else if (c != ',')
									{
										if (c != ':')
										{
											goto IL_842;
										}
										bool flag27 = splitterFound || objtype == JsonItemType.OBJ_ARRAY || objtype == JsonItemType.OBJ_NONE;
										if (flag27)
										{
											this.stopFailure = true;
											return null;
										}
										lastitem.Name = key.ToString();
										splitterFound = true;
										quoted = false;
										inquot = false;
									}
									else
									{
										bool flag28 = !splitterFound && objtype == JsonItemType.OBJ_OBJECT;
										if (flag28)
										{
											this.stopFailure = true;
											return null;
										}
										bool flag29 = objtype == JsonItemType.OBJ_ARRAY;
										if (flag29)
										{
											bool flag30 = key.Length > 0 || quoted;
											if (flag30)
											{
												lastitem.Value = key.ToString();
												lastitem.ObjectType = JsonItemType.OBJ_ARRAYITEM;
											}
										}
										else
										{
											bool flag31 = objtype == JsonItemType.OBJ_OBJECT;
											if (flag31)
											{
												bool flag32 = value.Length > 0 || quoted;
												if (flag32)
												{
													lastitem.Value = value.ToString();
													lastitem.ObjectType = JsonItemType.OBJ_VARIANT;
												}
											}
										}
										lastitem.Index = items.Count;
										items.Add(lastitem);
										bool flag33 = comments.Count > 0;
										if (flag33)
										{
											items.AddRange(comments);
											comments.Clear();
										}
										key.Clear();
										value.Clear();
										inquot = false;
										quoted = false;
										splitterFound = false;
										lastitem = new JsonItem
										{
											NameQuot = '\0',
											ValueQuot = '\0',
											Parent = parent
										};
									}
								}
								else
								{
									if (c <= ']')
									{
										if (c != '[')
										{
											if (c != ']')
											{
												goto IL_842;
											}
											goto IL_570;
										}
									}
									else if (c != '{')
									{
										if (c != '}')
										{
											goto IL_842;
										}
										goto IL_570;
									}
									bool flag34 = !splitterFound && key.Length > 0;
									if (flag34)
									{
										this.stopFailure = true;
										return null;
									}
									bool flag35 = value.Length > 0;
									if (flag35)
									{
										this.stopFailure = true;
										return null;
									}
									this.startid = i + 1;
									this.depth++;
									bool flag36 = cur == '{';
									if (flag36)
									{
										bool flag37 = !splitterFound && objtype == JsonItemType.OBJ_OBJECT;
										if (flag37)
										{
											this.stopFailure = true;
											return null;
										}
										this.depth_object++;
										lastitem.ObjectType = JsonItemType.OBJ_OBJECT;
										lastitem.SubItems = this.JsonDecode(JsonItemType.OBJ_OBJECT, lastitem);
									}
									else
									{
										this.depth_array++;
										lastitem.ObjectType = JsonItemType.OBJ_ARRAY;
										lastitem.SubItems = this.JsonDecode(JsonItemType.OBJ_ARRAY, lastitem);
									}
									bool flag38 = this.stopFailure;
									if (flag38)
									{
										return null;
									}
									i = this.startid;
									goto IL_879;
								IL_570:
									bool flag39 = (objtype == JsonItemType.OBJ_OBJECT && !splitterFound && key.Length > 0) || (value.Length > 0 && objtype == JsonItemType.OBJ_ARRAY);
									if (flag39)
									{
										this.stopFailure = true;
										return null;
									}
									this.startid = i;
									bool flag40 = cur == '}';
									if (flag40)
									{
										this.depth_object--;
									}
									else
									{
										this.depth_array--;
									}
									this.depth--;
									bool flag41 = this.depth_array < 0 || this.depth_object < 0 || this.depth < 0;
									if (flag41)
									{
										this.stopFailure = true;
										return null;
									}
									bool flag42 = objtype == JsonItemType.OBJ_ARRAY;
									if (flag42)
									{
										bool flag43 = key.Length > 0;
										if (flag43)
										{
											lastitem.ObjectType = JsonItemType.OBJ_ARRAYITEM;
											lastitem.Value = key.ToString();
										}
									}
									else
									{
										bool flag44 = objtype == JsonItemType.OBJ_OBJECT;
										if (flag44)
										{
											bool flag45 = value.Length > 0;
											if (flag45)
											{
												lastitem.ObjectType = JsonItemType.OBJ_VARIANT;
												lastitem.Value = value.ToString();
											}
										}
									}
									lastitem.Index = items.Count;
									items.Add(lastitem);
									bool flag46 = comments.Count > 0;
									if (flag46)
									{
										items.AddRange(comments);
										comments.Clear();
									}
									return items;
								}
							IL_879:
								goto IL_87A;
							IL_842:
								if (!quoted)
								{
									if (splitterFound)
									{
										value.Append(cur);
									}
									else
									{
										key.Append(cur);
									}
									goto IL_879;
								}
								this.stopFailure = true;
								return null;
							}
						IL_87A:;
						}
					}
				}
			IL_87B:;
			}
			return lastitem.SubItems;
		}
		private List<JsonItem> JsonDecodeFromArray(IList array, JsonItem parent, HashSet<object> recursion)
		{
			List<JsonItem> objects = new List<JsonItem>();
			for (int i = 0; i < array.Count; i++)
			{
				object value = array[i];
				JsonItem jitem = this.GetJsonItemByNameValue(null, value, parent, recursion);
				if (jitem != null)
				{
					objects.Add(jitem);
				}
			}
			return objects;
		}
		private bool CheckOrAddRecursion(object value, HashSet<object> recursion)
		{
			if (value != null && value.IsObject())
			{
				if (recursion.Contains(value))
				{
					return true;
				}
				recursion.Add(value);
			}
			return false;
		}
		private JsonItem GetJsonItemByNameValue(string name, object value, JsonItem parent, HashSet<object> recursion)
		{
			JsonItem result;
			if (this.CheckOrAddRecursion(value, recursion))
			{
				result = null;
			}
			else
			{
				JsonItem jitem = new JsonItem
				{
					Name = name,
					NameQuot = '"',
					Parent = parent
				};
				if (value.IsArray())
				{
					jitem.ObjectType = JsonItemType.OBJ_ARRAY;
					jitem.SubItems = this.JsonDecodeFromArray((IList)value, jitem, recursion);
				}
				else
				{
					if (value.IsObject())
					{
						jitem.ObjectType = JsonItemType.OBJ_OBJECT;
						if (value is IDictionary || value is IDictionary<string, object>)
						{
							jitem.SubItems = this.JsonDecodeFromDictionary((IEnumerable)value, jitem, recursion);
						}
						else
						{
							jitem.SubItems = this.JsonDecodeFromObject(value, jitem, recursion);
						}
					}
					else
					{
						if (value == null)
						{
							jitem.ObjectType = ((parent.ObjectType == JsonItemType.OBJ_OBJECT) ? JsonItemType.OBJ_VARIANT : JsonItemType.OBJ_ARRAYITEM);
							jitem.Value = null;
						}
						else
						{
							jitem.ObjectType = ((parent.ObjectType == JsonItemType.OBJ_OBJECT) ? JsonItemType.OBJ_VARIANT : JsonItemType.OBJ_ARRAYITEM);
							if (value.IsNumericType() || value is bool)
							{
								jitem.ValueQuot = '\0';
								jitem.Value = value;
							}
							else
							{
								jitem.ValueQuot = '"';
								jitem.Value = value.ToString();
							}
						}
					}
				}
				result = jitem;
			}
			return result;
		}
		private List<JsonItem> JsonDecodeFromDictionary(IEnumerable item, JsonItem parent, HashSet<object> recursion)
		{
			List<JsonItem> objects = new List<JsonItem>();
			foreach (object key in item)
			{
				object value = null;
				string name = null;
				Type type = key.GetType();
				if (key is DictionaryEntry entry)
				{
					value = entry.Value;
					name = entry.Key.ToString();
				}
				else
				{
					if(key is KeyValuePair<string, object> pair)
					{
						value = pair.Value;
						name = pair.Key.ToString();
					}
				}
				JsonItem jitem = this.GetJsonItemByNameValue(name, value, parent, recursion);
				if (jitem != null)
				{
					objects.Add(jitem);
				}
			}
			return objects;
		}
		private List<JsonItem> JsonDecodeFromObject(object item, JsonItem parent, HashSet<object> recursion)
		{
			Type type = item.GetType();
			MemberInfo[] members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public);
			List<JsonItem> objects = new List<JsonItem>();
			foreach (MemberInfo member in members)
			{
				bool flag = member.MemberType != MemberTypes.Field && member.MemberType != MemberTypes.Property;
				if (!flag)
				{
					object value = null;
					string name = member.Name;
					JsonAttribute jattribut;
					bool flag2 = (jattribut = (member.GetCustomAttribute(typeof(JsonAttribute)) as JsonAttribute)) != null;
					if (flag2)
					{
						bool notMapped = jattribut.NotMapped;
						if (notMapped)
						{
							goto IL_13B;
						}
						bool flag3 = !string.IsNullOrEmpty(jattribut.TagName);
						if (flag3)
						{
							name = jattribut.TagName;
						}
					}
					PropertyInfo property;
					bool flag4 = (property = (member as PropertyInfo)) != null;
					if (flag4)
					{
						bool flag5 = !property.CanRead;
						if (flag5)
						{
							goto IL_13B;
						}
						bool flag6 = property.GetIndexParameters().Length != 0;
						if (flag6)
						{
							goto IL_13B;
						}
						value = property.GetValue(item);
					}
					else
					{
						FieldInfo field;
						bool flag7 = (field = (member as FieldInfo)) != null;
						if (flag7)
						{
							value = field.GetValue(item);
						}
					}
					JsonItem jitem = this.GetJsonItemByNameValue(name, value, parent, recursion);
					bool flag8 = jitem == null;
					if (!flag8)
					{
						objects.Add(jitem);
					}
				}
			IL_13B:;
			}
			return objects;
		}
		public static JsonItem DecodeFrom(object item, JsonItem baseitem = null)
		{
			JsonDecoder jparser = new JsonDecoder();
			JsonItem jitem = baseitem;
			if (jitem == null)
			{
				jitem = new JsonItem();
			}
			HashSet<object> recursion = new HashSet<object>();
			JsonItem parent = jitem.Parent;
			if (parent != null && parent.ObjectType == JsonItemType.OBJ_ARRAY)
			{
				jitem.ObjectType = JsonItemType.OBJ_ARRAYITEM;
			}
			else
			{
				JsonItem parent2 = jitem.Parent;
				if (parent2 != null && parent2.ObjectType == JsonItemType.OBJ_OBJECT)
				{
					jitem.ObjectType = JsonItemType.OBJ_VARIANT;
				}
			}
			if (jitem == null)
			{
				jitem = new JsonItem();
			}
			if (item.IsArray())
			{
				jitem.ObjectType = JsonItemType.OBJ_ARRAY;
				jitem.SubItems = jparser.JsonDecodeFromArray((IList)item, jitem, recursion);
			}
			else
			{
				if (item.IsObject() || item.IsDictionary())
				{
					jitem.ObjectType = JsonItemType.OBJ_OBJECT;
					if (item is IDictionary || item is IDictionary<string, object>)
					{
						jitem.SubItems = jparser.JsonDecodeFromDictionary((IEnumerable)item, jitem, recursion);
					}
					else
					{
						jitem.SubItems = jparser.JsonDecodeFromObject(item, jitem, recursion);
					}
				}
				else
				{
					jitem.Value = item;
				}
			}
			recursion.Clear();
			return jitem;
		}
		public static JsonItem Decode(string input, bool skipcomment = false)
		{
			JsonDecoder jparser = new JsonDecoder
			{
				skipComment = skipcomment,
				DecodeUnicode = true
			};
			return jparser.DecodeJson(input);
		}

		private string input;
		private int depth = 0;
		private int startid = 0;
		private int depth_array = 0;
		private int depth_object = 0;
		private bool decodeUnicode;
		private bool stopFailure = false;
		private bool skipComment = false;
	}
}
