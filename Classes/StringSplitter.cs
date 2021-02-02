using CommonUtils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonUtils.Classes
{


    [Flags]
    public enum StringSplitOption
    {
        None = 0,
        PrintSpecialCharacter = 1 << 0,
        AddToAll = 1 << 1,
        CrossEmptyValue = 1 << 2,
        TrimPerElement = 1 << 3,
        AllowSpecialChar = 1 << 4
    }
    public class StringSplitHandler
    {
        public StringSplitHandler(string text, string splitstring, int index)
        {
            this.Text = text;
            this.SplitString = splitstring;
            this.Index = index;
        }
        public string Text { get; set; }
        public string SplitString { get; private set; }
        public int Index { get; private set; }
        public bool Cancel { get; set; }
        public bool Stop { get; set; }
    }
    public class StringSplitter : IDisposable
    {
        private StringTokenizer Tokenizer { get; set; } = new StringTokenizer();
        public StringSplitter()
        {
            
        }
        public StringSplitter(string text)
        {
            this.Text = text;
        }
        public StringSplitter(string text, params string[] splitters)
        {
            this.Text = text;
            this.Splitters = splitters;
        }
        public StringSplitter(string text, int count, params string[] splitters)
        {
            this.Count = count;
            this.Text = text;
            this.Splitters = splitters;
        }
        public Action<StringSplitHandler> OnSplit { get; set; }
        public string Text 
        { 
            get


            {
                return this.Tokenizer.Text;
            } 
            set
            {
                this.Tokenizer.Text = value;
            }
        }
        public int Count { get; set; }
        private StringSplitOption splitOptions;
        public StringSplitOption SplitOptions
        {
            get
            {
                return splitOptions;
            }
            set
            {
                this.splitOptions = value;
                this.Tokenizer.SetSettingsFrom(value);
            }
        }
        public StringQuoteOption SplitQuoteOption
        {
            get
            {
                return this.Tokenizer.StringQuoteOption;
            }
            set
            {
                this.Tokenizer.StringQuoteOption = value;
            }
        }
        public string[] Splitters
        {
            get
            {
                return this.Tokenizer.Tokens;
            }
            set
            {
                this.Tokenizer.Tokens = value;
            }
        }
        public string[] Split()
        {

            if (string.IsNullOrEmpty(this.Text) || this.Splitters == null || this.Splitters.Length == 0) return new string[0];
            this.Tokenizer.ResetPosition();
            List<string> splitted = new List<string>();
            while (!this.Tokenizer.Finish)
            {
                StringTokenResult tokenResult = this.Tokenizer.Tokenize();
                if (this.SplitOptions.HasFlag(StringSplitOption.CrossEmptyValue) && string.IsNullOrEmpty(tokenResult.TokenText))
                {
                    continue;
                }
                if(this.OnSplit != null)
                {
                    var handler = new StringSplitHandler(tokenResult.TokenText, tokenResult.TokenKey, splitted.Count);
                    this.OnSplitEvent(handler);
                    if (!handler.Cancel)
                    {
                        splitted.Add(handler.Text);
                    }
                    if (handler.Stop)
                    {
                        break;
                    }
                }
                if (this.Count > 0 && splitted.Count >= this.Count) break;
            }
            return splitted.ToArray();
        }
       
        /* OldStyle
        private string[] SplitPrivate()
        {
            if (string.IsNullOrEmpty(this.Text) || this.Splitters == null || this.Splitters.Length == 0) return new string[0];
            int maxlen = this.Splitters.First().Length;
            List<string> splitted = new List<string>();
            StringBuilder currentKey = new StringBuilder();
            StringBuilder currentValue = new StringBuilder();
            bool specialchar = false;
            bool isquote = false;
            bool quoted = false;
            char quotchar = '\0';
            for (int i = 0; i < this.Text.Length; i++)
            {
                char current = this.Text[i];
                if (current == '\\' && !specialchar)
                {
                    if (this.SplitOptions.HasFlag(StringSplitOption.PrintSpecialCharacter))
                    {
                        currentValue.Append(current);
                    }
                    specialchar = true;
                    continue;
                }
                bool continueNext = false;
                if(this.SplitQuoteOption != StringQuoteOption.None)
                {
                    if((this.SplitQuoteOption == StringQuoteOption.SingleQuote && current == '\'') || (this.SplitQuoteOption == StringQuoteOption.DoubleQuote && current == '"'))
                    {
                        continueNext = true;
                    }
                    else if(this.SplitQuoteOption == StringQuoteOption.All)
                    {
                        if(isquote)
                        {
                            if(current == quotchar)
                            {
                                continueNext = true;
                            }
                        }
                        else
                        {
                            continueNext = true;
                            quotchar = current;
                        }
                    }
                }
                if (continueNext && !specialchar)
                {
                    if (isquote)
                    {
                        quoted = true;
                    }
                    isquote = !isquote;

                    if (this.SplitOptions.HasFlag(StringSplitOption.PrintSpecialCharacter))
                    {
                        currentValue.Append(current);
                    }


                    continue;
                }
                else
                {
                    if (!isquote)
                    {
                        currentKey.Append(current);
                    }

                    if (currentKey.Length > maxlen)
                    {
                        currentKey.Remove(0, 1);
                    }
                    if (currentKey.Length >= maxlen && !isquote)
                    {
                        bool next = false;
                        for (int j = 0; j < this.Splitters.Length; j++)
                        {
                            if (currentKey.ToString(0, this.Splitters[j].Length) == this.Splitters[j])
                            {
                                if (currentValue.Length == 0 && this.SplitOptions.HasFlag(StringSplitOption.CrossEmptyValue))
                                {
                                    next = true;
                                    break;
                                }
                                currentKey.Remove(0, this.Splitters[j].Length);
                                var value = "";
                                if (quoted && !this.SplitOptions.HasFlag(StringSplitOption.AddToAll))
                                {
                                    value = currentValue.ToString();
                                }
                                else
                                {
                                    value = currentValue.ToString(0, currentValue.Length - (maxlen - 1));
                                }
                                if (this.SplitOptions.HasFlag(StringSplitOption.TrimPerElement))
                                {
                                    value = value.Trim();
                                }
                                var handler = new StringSplitHandler(value, this.Splitters[j], splitted.Count);
                                this.OnSplitEvent(handler);
                                if (!handler.Cancel)
                                {
                                    splitted.Add(handler.Text);
                                }
                                if (handler.Stop)
                                {
                                    return splitted.ToArray();
                                }
                                next = true;
                                currentValue.Clear();
                                currentValue.Append(currentKey.ToString());
                                quoted = false;
                                if (this.Count > 0 && splitted.Count >= this.Count)
                                {
                                    int minus = maxlen - 1;

                                    currentValue.Append(this.Text.SubstringEx(i + 1));
                                    string text = ((this.SplitOptions.HasFlag(StringSplitOption.TrimPerElement)) ? currentValue.ToString().Trim() : currentValue.ToString());
                                    handler = new StringSplitHandler(text, "", splitted.Count);
                                    if (!handler.Cancel)
                                    {
                                        splitted.Add(handler.Text);
                                    }
                                    return splitted.ToArray();
                                }
                                break;
                            }
                        }
                        if (next) continue;
                    }
                    if ((!quoted || this.SplitOptions.HasFlag(StringSplitOption.AddToAll)))
                    {
                        currentValue.Append(current);
                    }

                }
                if (specialchar)
                {
                    specialchar = false;
                    continue;
                }
            }
            if (!(this.Count > 0 && splitted.Count < this.Count) && currentValue.Length > 0)
            {
                string text = ((this.SplitOptions.HasFlag(StringSplitOption.TrimPerElement)) ? currentValue.ToString().Trim() : currentValue.ToString());
                var handler = new StringSplitHandler(text, "", splitted.Count);
                if (!handler.Cancel)
                {
                    splitted.Add(handler.Text);
                }
            }
            return splitted.ToArray();
        }
        */
        protected void OnSplitEvent(StringSplitHandler handler)
        {
            this.OnSplit?.Invoke(handler);
        }
        private bool disposed;
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                this.Tokenizer.Dispose();
                this.Tokenizer = null;
            }
            disposed = true;
        }
    }
}
