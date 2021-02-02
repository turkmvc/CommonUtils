using System;
using System.Text;

namespace CommonUtils.Util
{	public class JsonEncoder
	{		
		private JsonEncoder()
		{
		}
		private string EncodeJson(JsonItem jsonItem)
		{
			string result2;
			if (jsonItem.ObjectType == JsonItemType.OBJ_SINGLE)
			{
				result2 = "\"" + jsonItem.Value + "\"";
			}
			else
			{
				if (jsonItem.ObjectType != JsonItemType.OBJ_ARRAY && jsonItem.ObjectType != JsonItemType.OBJ_OBJECT)
				{
					result2 = null;
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					int added = 0;
					if (this.formattingtype != 0)
					{
						if (this.formattingtype == 2 || this.depth2 == 1)
						{
							if (!this.preventfirstlineformat)
							{
								sb.Append('\t', this.depth2);
							}
						}
					}
					this.preventfirstlineformat = false;
					if (jsonItem.ObjectType == JsonItemType.OBJ_ARRAY)
					{
						sb.Append("[");
					}
					else
					{
						sb.Append("{");
					}
					if (this.formattingtype != 0)
					{
						if (this.formattingtype == 2 || this.depth2 == 0)
						{
							sb.Append("\r\n");
						}
					}
					bool donotadd = false;
					foreach (JsonItem jitem in jsonItem.SubItems)
					{
						bool flag9 = (jitem.ObjectType == JsonItemType.OBJ_COMMENT_SINGLELINE || jitem.ObjectType == JsonItemType.OBJ_COMMENT_MULTILINE) && !this.printComment;
						if (!flag9)
						{
							bool flag10 = (jitem.ObjectType == JsonItemType.OBJ_COMMENT_SINGLELINE || jitem.ObjectType == JsonItemType.OBJ_COMMENT_MULTILINE) && this.formattingtype != 2;
							if (!flag10)
							{
								if (added > 0)
								{
									bool flag12 = !donotadd;
									if (flag12)
									{
										sb.Append(", ");
									}
									bool flag13 = donotadd;
									if (flag13)
									{
										donotadd = false;
									}
									if (this.formattingtype != 0)
									{
										if (this.formattingtype == 2 || this.depth2 == 0)
										{
											if (this.formattingtype == 2)
											{
												if (jitem.ObjectType == JsonItemType.OBJ_COMMENT_SINGLELINE || jitem.ObjectType == JsonItemType.OBJ_COMMENT_MULTILINE || added == 0)
												{
													if (jitem.LineOffset > 0)
													{
														for (int i = 0; i < jitem.LineOffset; i++)
														{
															sb.Append("\r\n");
														}
													}
													else
													{
														sb.Append(" ");
													}
												}
												else
												{
													sb.Append("\r\n");
												}
											}
											else
											{
												sb.Append("\r\n");
											}
										}
									}
								}
								if (jsonItem.ObjectType == JsonItemType.OBJ_ARRAY)
								{
									if (jsonItem.ObjectType == JsonItemType.OBJ_NONE || jsonItem.ObjectType == JsonItemType.OBJ_SINGLE || jsonItem.ObjectType == JsonItemType.OBJ_VARIANT)
									{
										return null;
									}
								}
								else
								{
									if (jsonItem.ObjectType == JsonItemType.OBJ_NONE || jsonItem.ObjectType == JsonItemType.OBJ_SINGLE || jsonItem.ObjectType == JsonItemType.OBJ_ARRAYITEM)
									{
										return null;
									}
								}
								if (jitem.ObjectType == JsonItemType.OBJ_ARRAY || jitem.ObjectType == JsonItemType.OBJ_OBJECT)
								{
									this.depth2++;
									if (jsonItem.ObjectType == JsonItemType.OBJ_OBJECT)
									{
										this.preventfirstlineformat = true;
									}
									string result = this.EncodeJson(jitem);
									if (result == null)
									{
										return null;
									}
									if (jsonItem.ObjectType == JsonItemType.OBJ_ARRAY)
									{
										sb.Append(result);
									}
									else
									{
										if (this.formattingtype != 0)
										{
											if (this.formattingtype == 2 || this.depth2 == 1)
											{
												sb.Append('\t', this.depth2);
											}
										}
										bool isParse = this.rootItem.IsParse;
										string name;
										if (isParse)
										{
											if (jitem.NameQuot > '\0')
											{
												name = jitem.NameQuot.ToString() + jitem.Name + jitem.NameQuot.ToString();
											}
											else
											{
												name = jitem.Name;
											}
										}
										else
										{
											name = "\"" + jitem.Name + "\"";
										}
										sb.Append(name + ": ");
										sb.Append(result);
									}
									added++;
									this.depth2--;
								}
								else
								{
									if (this.formattingtype != 0)
									{
										if (this.formattingtype == 2 || this.depth2 == 0)
										{
											if (jitem.ObjectType == JsonItemType.OBJ_COMMENT_SINGLELINE || jitem.ObjectType == JsonItemType.OBJ_COMMENT_MULTILINE)
											{
												if (jitem.LineOffset > 0 || added == 0)
												{
													sb.Append('\t', this.depth2 + 1);
												}
											}
											else
											{
												sb.Append('\t', this.depth2 + 1);
											}
										}
									}
									if (jitem.ObjectType == JsonItemType.OBJ_COMMENT_SINGLELINE || jitem.ObjectType == JsonItemType.OBJ_COMMENT_MULTILINE)
									{
										if (jitem.ObjectType == JsonItemType.OBJ_COMMENT_SINGLELINE)
										{
											sb.Append("//");
											sb.Append(jitem.Value);
										}
										else
										{
											sb.Append("/*");
											sb.Append(jitem.Value);
											sb.Append("*/");
										}
										donotadd = true;
									}
									else
									{
										if (jsonItem.ObjectType == JsonItemType.OBJ_ARRAY)
										{
											string value;
											if (this.rootItem.IsParse || !(jitem.Value is string))
											{
												if (jitem.Value == null)
												{
													value = "null";
												}
												else
												{
													if (jitem.Value is bool)
													{
														value = jitem.Value.ToString().ToLower();
													}
													else
													{
														if (jitem.ValueQuot > '\0')
														{
															value = jitem.ValueQuot.ToString() + jitem.GetValueWithVars() + jitem.ValueQuot.ToString();
														}
														else
														{
															value = jitem.GetValueWithVars();
														}
													}
												}
											}
											else
											{
												value = "\"" + jitem.GetValueWithVars() + "\"";
											}
											sb.Append(value);
										}
										else
										{
											string name2;
											string value2;
											if (this.rootItem.IsParse || !(jitem.Value is string))
											{
												if (jitem.NameQuot > '\0')
												{
													name2 = jitem.NameQuot.ToString() + jitem.Name + jitem.NameQuot.ToString();
												}
												else
												{
													name2 = jitem.Name;
												}
												if (jitem.Value == null)
												{
													value2 = "null";
												}
												else
												{
													if (jitem.Value is bool)
													{
														value2 = jitem.Value.ToString().ToLower();
													}
													else
													{
														bool flag44 = jitem.ValueQuot > '\0';
														if (flag44)
														{
															value2 = jitem.ValueQuot.ToString() + jitem.GetValueWithVars() + jitem.ValueQuot.ToString();
														}
														else
														{
															value2 = jitem.GetValueWithVars();
														}
													}
												}
											}
											else
											{
												name2 = "\"" + jitem.Name + "\"";
												value2 = "\"" + jitem.GetValueWithVars() + "\"";
											}
											sb.Append(name2 + ": " + value2);
										}
									}
									added++;
								}
							}
						}
					}
					if (this.formattingtype != 0)
					{
						if (this.formattingtype == 2 || this.depth2 == 0)
						{
							sb.Append("\r\n");
							bool flag47 = this.formattingtype == 2;
							if (flag47)
							{
								sb.Append('\t', this.depth2);
							}
						}
					}
					if (jsonItem.ObjectType == JsonItemType.OBJ_ARRAY)
					{
						sb.Append("]");
					}
					else
					{
						sb.Append("}");
					}
					result2 = sb.ToString();
				}
			}
			return result2;
		}
		public static string Encode(JsonItem jsonItem, int formatting = 2, bool printcomment = true)
		{
			JsonEncoder jsonEncoder = new JsonEncoder
			{
				formattingtype = formatting
			};
			jsonEncoder.printComment = printcomment;
			jsonEncoder.rootItem = jsonItem;
			return jsonEncoder.EncodeJson(jsonItem);
		}
		private int depth2 = 0;
		private int formattingtype = 0;
		private bool preventfirstlineformat = false;
		public const int FORMATTING_SINGLELINE = 0;
		public const int FORMATTING_MULTILINE = 1;
		public const int FORMATTING_TABFORMAT = 2;
		private JsonItem rootItem = null;
		private bool printComment = true;
	}
}
