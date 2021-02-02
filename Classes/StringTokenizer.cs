using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonUtils.Classes
{
    [Flags]
    public enum StringQuoteOption
    {
        None,
        SingleQuote,
        DoubleQuote,
        All
    }
    public class StringTokenResult
    {
        public string TokenText { get; set; }
        public string TokenKey { get; set; }
        public bool TokenFound { get; set; }
        public int TokenIndex { get; set; }
    }
    public class StringTokenizer : IDisposable
    {
        public StringTokenizer()
        {

        }
        public StringTokenizer(string text)
        {
            this.Text = text;
        }
        public StringTokenizer(string text, string[] tokens)
        {
            this.Text = text;
            this.Tokens = tokens;
        }
        private string text;
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
                this.ResetPosition();
            }
        }
        public bool Finish { get; private set; }
        private string[] tokens;
        public string[] Tokens
        {
            get
            {
                return tokens;
            }
            set
            {
                this.Maxlen = 0;
                if (value == null)
                {
                    tokens = null;
                    return;
                }
                if(value.Length > 1)
                {
                    tokens = value.OrderByDescending(m => m.Length).ToArray();
                }
                else
                {
                    tokens = value;
                }
                if(tokens != null && tokens.Length > 0)
                {
                    this.Maxlen = tokens.First().Length;
                }
                
            }
        }
        public void ResetPosition()
        {
            this.Finish = false;
            this.CurrentPosition = 0;
        }
        private int currentPosition;
        public int CurrentPosition
        {
            get
            {
                return this.currentPosition;
            }
            set
            {
                this.Finish = false;
                this.currentPosition = value;
                if(this.Text != null && this.currentPosition >= this.Text.Length)
                {
                    this.Finish = true;
                }
            }
        }
        private int Maxlen
        {
            get;set;
        }
        public bool PrintSpecialCharacter { get; set; }
        public bool  AddToAll { get; set; }
        public bool AutoTrim { get; set; }
        public bool AllowSpecialChar { get; set; }


        public void SetSettingsFrom(StringSplitOption stringSplitOption)
        {
            this.AddToAll = stringSplitOption.HasFlag(StringSplitOption.AddToAll);
            this.AutoTrim = stringSplitOption.HasFlag(StringSplitOption.TrimPerElement);
            this.PrintSpecialCharacter = stringSplitOption.HasFlag(StringSplitOption.PrintSpecialCharacter);
            this.AllowSpecialChar = stringSplitOption.HasFlag(StringSplitOption.AllowSpecialChar);
        }
        public StringQuoteOption StringQuoteOption { get; set; } = StringQuoteOption.DoubleQuote;
        public string GetRemainText()
        {
            if(this.Finish || string.IsNullOrEmpty(this.Text))
            {
                return null;
            }
            return this.Text.Substring(this.CurrentPosition);
        }
        public StringTokenResult Tokenize(params string[] token)
        {
            StringTokenResult tokenResult = new StringTokenResult();
            tokenResult.TokenIndex = -1;
            if (token != null &&token.Length > 0) this.Tokens = token;
            if (string.IsNullOrEmpty(this.Text) || this.Tokens == null || this.Tokens.Length == 0) return tokenResult;
            if(this.CurrentPosition >= this.Text.Length)
            {
                return tokenResult;
            }
            StringBuilder currentKey = new StringBuilder();
            StringBuilder currentValue = new StringBuilder();
            bool specialchar = false;
            bool isquote = false;
            bool quoted = false;
            char quotchar = '\0';
            for (int i = this.CurrentPosition; i < this.Text.Length; i++)
            {
                char current = this.Text[i];
                if (this.AllowSpecialChar && current == '\\' && !specialchar)
                {
                    if (this.PrintSpecialCharacter)
                    {
                        currentValue.Append(current);
                    }
                    specialchar = true;
                    continue;
                }
                bool continueNext = false;
                if (this.StringQuoteOption != StringQuoteOption.None)
                {
                    if ((this.StringQuoteOption == StringQuoteOption.SingleQuote && current == '\'') || (this.StringQuoteOption == StringQuoteOption.DoubleQuote && current == '"'))
                    {
                        continueNext = true;
                    }
                    else if (this.StringQuoteOption == StringQuoteOption.All)
                    {
                        if (isquote)
                        {
                            if (current == quotchar)
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

                    if (this.PrintSpecialCharacter)
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

                    if (currentKey.Length > this.Maxlen)
                    {
                        currentKey.Remove(0, 1);
                    }
                    if (currentKey.Length >= this.Maxlen && !isquote && !specialchar)
                    {
                        bool next = false;
                        for (int j = 0; j < this.Tokens.Length; j++)
                        {
                            if (currentKey.ToString(0, this.Tokens[j].Length) == this.Tokens[j])
                            {
                                currentKey.Remove(0, this.Tokens[j].Length);
                                var value = "";
                                if (quoted && !this.AddToAll)
                                {
                                    value = currentValue.ToString();
                                }
                                else
                                {
                                    value = currentValue.ToString(0, currentValue.Length - (this.Maxlen - 1));
                                }
                                if (this.AutoTrim)
                                {
                                    value = value.Trim();
                                }
                                currentValue.Clear();
                                currentValue.Append(currentKey.ToString());
                                this.CurrentPosition = i + 1 - (this.Maxlen - this.Tokens[j].Length);
                                tokenResult.TokenFound = true;
                                tokenResult.TokenText = value;
                                tokenResult.TokenKey = this.Tokens[j];
                                tokenResult.TokenIndex = this.CurrentPosition - (tokenResult.TokenKey.Length);
                                return tokenResult;
                            }
                        }
                        if (next) continue;
                    }
                    if ((!quoted || AddToAll))
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
            if (currentValue.Length > 0)
            {
                string text = this.AutoTrim ? currentValue.ToString().Trim() : currentValue.ToString();
                tokenResult.TokenText = text;
            }
            this.CurrentPosition = this.text.Length;
            this.Finish = true;
            return tokenResult;
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
                this.Text = null;
                this.Tokens = null;
            }
            disposed = true;
        }
    }
}
