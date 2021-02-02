using CommonUtils.Classes;
using CommonUtils.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CommonUtils.Extensions
{
    public static class StringExtensions
    {

        public const string Regex_Web = @"([\r\n\t\s]|^)(((http|https|ftp|telnet|file|steam|ftps|sftp)(://))|(www|ftp)(\.))([a-zA-Z0-9çÇğĞıİöÖşŞüÜ_\-\+\=&!'\^é""<:>\./\\;~\,\?\@\[\]]+)";
        public const string Regex_Mail = @"\b([a-zA-Z0-9çÇğĞıİöÖşŞüÜ_\-\+\=&!'\^é""<:>\./\\;~\,\?]+)(\@)([\w\-_]+\.)([a-zA-Z0-9çÇğĞıİöÖşŞüÜ_\-\+\=&!'\^é""<:>\./\\;~\,\?\]]+)";
        public const string Regex_Ip = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        public const string Regex_Port = @"^([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])$";
        public static string RetDataPass = "_cwarge_";
        private static byte[] defaultIV = Encoding.UTF8.GetBytes("(WebUtils.Utils)");
        public static byte[] DefaultIV
        {
            get
            {
                return defaultIV;
            }
            set
            {
                defaultIV = value;
            }
        }

        public static string Escape(this string input)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var _char in input)
            {
                if (!char.IsLetterOrDigit(_char) || _char == ' ')
                {
                    builder.Append("_");
                    continue;
                }
                builder.Append(_char);
            }
            return builder.ToString();
        }
        public static string EncodeRetData(this string input)
        {
            return input.EncryptAES(RetDataPass);
        }
        public static string DecodeRetData(this string input)
        {
            return input.DecryptAES(RetDataPass);
        }
        public static bool IsAllNumeric(this string input)
        {
            return input.All(e => char.IsNumber(e));
        }
        public static bool IsPort(this string portaddr)
        {
            if (!int.TryParse(portaddr, out int nm))
            {
                return false;
            }
            if (nm < 0 || nm > 65535) return false;
            return true;
        }
        public static bool IsIp(this string ipaddr)
        {
            if (Regex.IsMatch(ipaddr, Regex_Ip))
            {
                return true;
            }
            return false;
        }
        public static bool IsUrl(this string url)
        {
            if (Regex.IsMatch(url, Regex_Web))
            {
                return true;
            }
            return false;
        }
        public static bool IsMail(this string url)
        {
            if (Regex.IsMatch(url, Regex_Mail))
            {
                return true;
            }
            return false;
        }
        public static bool IsNumeric(this string input)
        {
            return decimal.TryParse(input, out decimal d) ? true : false;
        }
        public static bool IsDateTime(this string input)
        {
            if (DateTime.TryParse(input, out DateTime d))
            {
                return true;
            }
            return false;
        }
        public static bool IsBool(this string input)
        {
            var acceptvalues = new string[]
            {
                "0",
                "1",
                "On",
                "Off",
                "Evet",
                "Hayır"

            };
            if (acceptvalues.Where(e => e == input).Any() || bool.TryParse(input, out bool d))
            {
                return true;
            }
            return false;
        }
      

        /// <summary>
        /// SoldanKırp: Cümlenin içerisinde sol taraftaki verilen harf veya kelimeyi kırpar.
        /// </summary>
        /// <param name="Cümle">Aranacak cümleyi girin</param>
        /// <param name="Harf">Cümle içerisinde sayılacak harfi girin</param>
        /// <param name="Toplam">Kaç defa kırpılacağını belirler</param>
        /// <returns></returns>
        /// 
        public static string TrimStartEx(this string input, string which, int total = 0)
        {
            string[] whichs = new string[] { which };
            return TrimStartEx(input, whichs, total, out int totalout);
        }
        public static string TrimStartEx(this string input, string[] whichs, int total = 0)
        {
            return TrimStartEx(input, whichs, total, out int totalout);
        }
        private static string TrimStartEx(this string input, string[] whichs, int total, out int totalout)
        {
            totalout = 0;
            StringBuilder sb = new StringBuilder(input);
            if (string.IsNullOrEmpty(input)) goto endsof;
            if (whichs == null || whichs.Length == 0) goto endsof;
            whichs = whichs.OrderByDescending(m => m.Length).ToArray();
            int maxlen = whichs.First().Length;
            int minlen = whichs.Last().Length;

            for (int i = 0; i < sb.Length; i++)
            {
                bool canloop = false;
                for (int j = 0; j < whichs.Length; j++)
                {
                    if (sb.Length >= whichs[j].Length && sb.ToString(0, whichs[j].Length) == whichs[j])
                    {
                        totalout++;
                        sb.Remove(0, whichs[j].Length);
                        canloop = true;
                        i = 0;
                        break;
                    }
                }
                if (!canloop) break;
            }
        endsof:
            return sb.ToString();
        }
        public static string TrimEx(this string input, string which, int total = 0)
        {
            string[] HarfC = new string[] { which };
            string returnValue;
            returnValue = TrimStartEx(input, HarfC, total, out int totalout);
            returnValue = TrimEndEx(returnValue, HarfC, total, out totalout);
            return returnValue;
        }
        public static string TrimEx(this string input, string[] whichs, int total = 0)
        {
            string returnValue;
            returnValue = TrimStartEx(input, whichs, total, out int totalout);
            returnValue = TrimEndEx(returnValue, whichs, total, out totalout);
            return returnValue;
        }
        /// <summary>
        /// SağdanKırp: Cümlenin içerisinde sağ taraftaki verilen harf veya kelimeyi kırpar.
        /// </summary>
        /// <param name="Cümle">Aranacak cümleyi girin</param>
        /// <param name="Harf">Cümle içerisinde sayılacak harfi girin</param>
        /// <param name="Toplam">Kaç defa kırpılacağını belirler</param>
        /// <returns>Cümlenin sağında belirtilen karakterleri kırpar.</returns>
        /// 
        public static string TrimEndEx(this string input, string which, int total = 0)
        {
            string[] whichs = new string[] { which };
            return TrimEndEx(input, whichs, total, out int totalout);
        }
        public static string TrimEndEx(this string input, string[] whichs, int total = 0)
        {
            int totalout = 0;
            return TrimEndEx(input, whichs, totalout, out totalout);
        }
        private static string TrimEndEx(this string input, string[] whichs, int total, out int totalout)
        {
            StringBuilder sb = new StringBuilder(input);
            totalout = 0;
            if (string.IsNullOrEmpty(input) || whichs == null || whichs.Length == 0) goto endsof;
            whichs = whichs.OrderByDescending(m => m.Length).ToArray();
            int maxlen = whichs.First().Length;
            int minlen = whichs.Last().Length;
            for (int i = sb.Length - 1; i >= 0; i--)
            {
                bool canloop = false;
                for (int j = 0; j < whichs.Length; j++)
                {
                    if (i >= whichs[j].Length && sb.ToString(i - whichs[j].Length, whichs[j].Length) == whichs[j])
                    {
                        totalout++;
                        sb.Remove(sb.Length, sb.Length - whichs[j].Length);
                        canloop = true;
                        i = sb.Length;
                        break;
                    }
                }
                if (!canloop) break;
            }
        endsof:
            return sb.ToString();
        }
        public static string Rot13(this string value)
        {
            char[] array = value.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                int number = (int)array[i];

                if (number >= 'a' && number <= 'z')
                {
                    if (number > 'm')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                else if (number >= 'A' && number <= 'Z')
                {
                    if (number > 'M')
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                array[i] = (char)number;
            }
            return new string(array);
        }
        public static string[] SplitEx(this string input, params string[] splitters)
        {
            return SplitEx(input, 0, splitters);
        }
        public static string[] SplitEx(this string input, int count, params string[] splitters)
        {
            return SplitEx(input, splitters, count);
        }
        public static string[] SplitEx(this string input, string splitter, int count = 0)
        {
            return SplitEx(input, new string[] { splitter }, count);
        }
        public static string[] SplitEx(this string input, string[] splitters, int count = 0, StringSplitOption options = StringSplitOption.None)
        {
            StringSplitter splitter = new StringSplitter(input)
            {
                Splitters = splitters,
                Count = count,
                SplitOptions = options
            };
            return splitter.Split();
        }
        public static byte[] SimpleCrypt(this string input)
        {
            byte[] B = ASCIIEncoding.Unicode.GetBytes(input);
            byte[] D = new Byte[B.Length];
            int GRM = 0;
            GRM = B.Length % 2;
            string ŞimdikiB = null;
            int Konum = 0;
            string ŞimdikiCH = null;
            bool İkinci = false;
            int k = 0;
            for (int i = 0; i < B.Length; i++)
            {

                ŞimdikiB = B[i].ToString("X2");
                if (İkinci && GRM == 0)
                {
                    ŞimdikiCH = ŞimdikiCH + ŞimdikiB[(k - 1) % 2].ToString();
                }
                else
                {
                    ŞimdikiCH = ŞimdikiCH + ŞimdikiB[k % 2].ToString();
                }

                if (k % 2 == 1)
                {
                    D[Konum] = Byte.Parse(ŞimdikiCH, System.Globalization.NumberStyles.HexNumber);
                    ŞimdikiCH = null;
                    Konum++;
                }
                if (i == B.Length - 1 && !İkinci)
                {
                    İkinci = true;
                    i = -1;
                }
                k++;
            }
            return D;
        }
        public static string OpenSimpleCrypt(this string Loc)
        {
            Byte[] şifreleneniçerik;
            string İçerikC = null;
            FileStream FileRd = new FileStream(Loc, FileMode.Open, FileAccess.Read);
            şifreleneniçerik = new byte[FileRd.Length];
            FileRd.Read(şifreleneniçerik, 0, şifreleneniçerik.Length);
            FileRd.Close();
            İçerikC = ASCIIEncoding.Unicode.GetString(şifreleneniçerik);
            return ASCIIEncoding.Unicode.GetString(DecrpytPassword(İçerikC)); ;
        }
        public static void SaveSimpleCrypt(this string input, string location)
        {
            //Key = ASCIIEncoding.Unicode.GetString(şifreleneniçerik);
            Byte[] şifreleneniçerik;
            şifreleneniçerik = SimpleCrypt(input);
            FileStream FileWrt = new FileStream(location, FileMode.Create, FileAccess.Write);
            FileWrt.Write(şifreleneniçerik, 0, şifreleneniçerik.Length);
            FileWrt.Close();
        }
        public static byte[] DecrpytPassword(string Kelime)
        {
            Byte[] B = ASCIIEncoding.Unicode.GetBytes(Kelime);
            Byte[] D = new Byte[B.Length];
            string ŞimdikiB = null;
            long f = 0;
            long m = 0;
            long md1 = 0;
            md1 = System.Convert.ToInt64(B.Length % 2);
            f = 1;
            if (md1 > 0)
            {
                m = System.Convert.ToInt64(B.Length + 1) / 2;
            }
            else
            {
                m = System.Convert.ToInt64(B.Length / 2 + 1);
            }
            long md2 = md1;
            for (int i = 1; i <= B.Length; i++)
            {
                if (md2 == 0)
                {
                    if (i % 2 == 1)
                    {
                        ŞimdikiB = B[f - 1].ToString("X2")[(i - 1) % 2].ToString() + B[m - 1].ToString("X2")[(i - 1) % 2].ToString();
                    }
                    else
                    {
                        ŞimdikiB = B[m - 1].ToString("X2")[(i - 1) % 2].ToString() + B[f - 1].ToString("X2")[(i - 1) % 2].ToString();
                    }
                }
                else
                {
                    if (i % 2 == 1)
                    {
                        ŞimdikiB = B[f - 1].ToString("X2")[0].ToString() + B[m - 1].ToString("X2")[1].ToString();
                    }
                    else
                    {
                        ŞimdikiB = B[m - 1].ToString("X2")[0].ToString() + B[f - 1].ToString("X2")[1].ToString();
                    }
                }

                D[i - 1] = Byte.Parse(ŞimdikiB, System.Globalization.NumberStyles.HexNumber);
                f = f + ((i - 1) % 2);
                m = m + (md1 % 2);
                md1++;
            }
            return D;
        }
        public static bool ToBool(this string Key)
        {
            if (Key == null) return false;
            if (IsNumeric(Key))
            {
                long nmbr = Convert.ToInt64(Key);
                if (nmbr > 0) return true;
                return false;
            }
            if (Key.ToString().ToUpper() == "DOĞRU" || Key.ToString().ToUpper() == "EVET" || Key.ToString().ToUpper() == "TRUE") return true;
            return false;
        }
        public static bool IncludingQuote(this string Key)
        {
            if (Key == null) return false;
            if (Key.StartsWith("\"") && Key.EndsWith("\""))
            {
                return true;
            }
            return false;
        }
        public static string RemoveQuota(this string input)
        {
            if (input.StartsWith("\"") && input.EndsWith("\""))
            {
                if (input.Length == 2)
                {
                    return string.Empty;
                }
                else
                {
                    return input.Substring(1, input.Length - 2);
                }
            }
            return input;
        }
        public static string SubstringEx(this string input, int start)
        {
            return SubstringEx(input, start, -1);
        }
        public static string SubstringEx(this string input, int start, int length)
        {
            if (string.IsNullOrEmpty(input) || start >= input.Length) return "";
            if (length == -1 || start + length > input.Length)
            {
                return input.Substring(start, input.Length - start);
            }
            return input.Substring(start, length);

        }
        public static string ToMD5(this string input)
        {
            return Cipher.ConvertToMD5(input);
        }
        public static string EncryptAES(this string input, string password)
        {
            if (password.Length > 16)
            {
                password = password.Substring(0, 16);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int i = password.Length; i < 16; i++)
                {
                    sb.Append("*");
                }
                password += sb;
            }
            byte[] inputbytes = Encoding.UTF8.GetBytes(input);
            byte[] passbytes = Encoding.UTF8.GetBytes(password);
            if (passbytes.Length > 17)
            {
                Array.Resize(ref passbytes, 16);
            }
            byte[] outbytes = Cipher.AES_EncryptByte(inputbytes, passbytes, DefaultIV);
            return Convert.ToBase64String(outbytes);
        }
        public static string DecryptAES(this string input, string password)
        {
            if (password.Length > 16)
            {
                password = password.Substring(0, 16);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int i = password.Length; i < 16; i++)
                {
                    sb.Append("*");
                }
                password += sb;
            }
            byte[] inputbytes = Convert.FromBase64String(input);
            byte[] passbytes = Encoding.UTF8.GetBytes(password);
            if (passbytes.Length > 17)
            {
                Array.Resize(ref passbytes, 16);
            }
            byte[] outbytes = Cipher.AES_DecryptByte(inputbytes, passbytes, DefaultIV);
            return Encoding.UTF8.GetString(outbytes);
        }
        public static int ToInt32(this string input, int defaultValue = 0)
        {
            return decimal.ToInt32(ToDecimal(input, defaultValue));
        }
        public static long ToInt64(this string input, long defaultValue = 0)
        {
            return decimal.ToInt64(ToDecimal(input, defaultValue));
        }
        public static float ToSingle(this string input, float defaultValue = 0)
        {
            return float.TryParse(input, out float results) ? results : defaultValue;
        }
        public static double ToDouble(this string input, double defaultValue = 0)
        {
            return double.TryParse(input, out double results) ? results : defaultValue;
        }
        public static decimal ToDecimal(this string input, decimal defaultValue = 0)
        {
            return decimal.TryParse(input, out decimal results) ? results : defaultValue;
        }
        public static bool ToBool(this string input, bool defaultValue = false)
        {
            var trueValues = new string[]
            {
                "1",
                "On",
                "Evet"

            };
            var falseValues = new string[]
            {
                    "0",
                    "Off",
                    "Hayır"
            };
            if (trueValues.Where(e => e == input).Any())
            {
                return true;
            }
            if (falseValues.Where(e => e == input).Any())
            {
                return false;
            }
            if (bool.TryParse(input, out bool results))
            {
                return results;
            }
            return defaultValue;
        }
        public static DateTime ToDateTime(this string input, DateTime defaultValue = default(DateTime))
        {
            if (DateTime.TryParse(input, out DateTime results))
            {
                return results;
            }
            return defaultValue;
        }
        public static string Fmt(this string input, params object[] args)
        {
            if (input == null) return null;
            return string.Format(input, args);
        }
        public static string GetFirst(this string input, int len, string suffix = "...")
        {
            if (string.IsNullOrEmpty(input)) return input;
            if (len >= input.Length) return input;
            return input.Substring(0, len) + suffix;
        }
        public static string Empty(this string input, string emptymsg = "")
        {
            if (string.IsNullOrEmpty(input)) return emptymsg;
            return input;
        }
        public static bool IsSoundFile(this string input)
        {
            if (input.ToLowerInvariant().EndsWith(".mp3"))
            {
                return true;
            }
            if (input.ToLowerInvariant().EndsWith(".wav"))
            {
                return true;
            }
            return false;
        }
        static string[] excludedPasswordChars = new string[]
        {
                   " "
        };
        public static bool IsValidUsername(this string input)
        {
            if (string.IsNullOrEmpty(input) || excludedPasswordChars.Contains(input) || input.Length < 3 || input.Length > 100)
            {
                return false;
            }
            return true;

        }
        public static bool IsValidPassword(this string input)
        {
            if (string.IsNullOrEmpty(input) || excludedPasswordChars.Contains(input) || input.Length < 3 || input.Length > 16)
            {
                return false;
            }
            return true;
        }
        public static string Tokenize(this string input, params string[] tokens)
        {
            return Tokenize(input, StringQuoteOption.None, tokens);
        }
        public static string Tokenize(this string input, StringQuoteOption quoteoption, params string[] tokens)
        {
            return Tokenize(input, quoteoption, StringSplitOption.None, tokens);
        }
        public static string Tokenize(this string input, StringQuoteOption quoteoption, StringSplitOption tokenOptions, params string[] tokens)
        {
            using (var tokenizer = new StringTokenizer(input, tokens))
            {
                tokenizer.StringQuoteOption = quoteoption;
                tokenizer.SetSettingsFrom(tokenOptions);
                return tokenizer.Tokenize().TokenText;
            }
        }
    }
}
