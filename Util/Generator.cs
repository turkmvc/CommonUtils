using System;
using System.Collections.Generic;
using System.Text;

namespace CommonUtils.Util
{
    public class Generator
    {
        static char[] IdChar;
        public static void GeneratorInit()
        {
            List<char> allCharUpper = new List<char>();
            List<char> allCharLower = new List<char>();
            List<char> allCharNum = new List<char>();
            List<char> allChar = new List<char>(); ;
            for (int i = 48; i <= 122; i++)
            {
                if (i == 94 || i == 96) i++;
                char nchar = (char)i;
                if (!char.IsLetterOrDigit(nchar))
                {
                    continue;
                }
                if (char.IsDigit(nchar))
                {
                    allCharNum.Add(nchar);
                    continue;
                }
                if (char.IsUpper(nchar))
                {
                    allCharUpper.Add(nchar);
                    continue;
                }
                if (char.IsLower(nchar))
                {
                    allCharLower.Add(nchar);
                    continue;
                }
            }
            int itotal = 0;
            for (int i = 0; i < allCharLower.Count; i++)
            {
                allChar.Add(allCharLower[i]);
                allChar.Add(allCharUpper[i]);
                if (i % 3 == 0 && itotal < allCharNum.Count)
                {
                    allChar.Add(allCharNum[itotal]);
                    itotal++;
                }
            }

            for (int i = itotal; i < allCharNum.Count; i++)
            {
                allChar.Add(allCharNum[i]);
            }
            allCharUpper.Clear();
            allCharLower.Clear();
            allCharNum.Clear();
            IdChar = allChar.ToArray();
        }
        public enum GeneratorType
        {
            AllowAll = 0,
            AllowUpperCaseChar = 1 << 0,
            AllowLowerCaseChar = 1 << 1,
            AllowNumeric = 1 << 2
        }
        public static string ConvertToIdChar(int input)
        {
            if (input < 1) return IdChar[0].ToString();
            StringBuilder stringBuilder = new StringBuilder();

            int num = 0;
            while (input > 0)
            {
                num = (input % IdChar.Length);
                input /= IdChar.Length;
                stringBuilder.Insert(0, IdChar[num]);
            }
            return stringBuilder.ToString();
        }
        public static string GenerateRndNumber(int uzunluk = 4)
        {
            Random Rnd = new Random();
            StringBuilder stringBuilder = new StringBuilder();
            int totalwritten = 0;
            while (totalwritten < uzunluk)
            {
                int skey = Rnd.Next(0, 9);
                stringBuilder.Append(skey);
                totalwritten++;
            }
            return stringBuilder.ToString();
        }
        public static string GenerateRndString(int uzunluk = 4, GeneratorType generatorType = GeneratorType.AllowAll)
        {
            Random Rnd = new Random();
            StringBuilder stringBuilder = new StringBuilder();

            int totalwritten = 0;
            while (totalwritten < uzunluk)
            {
                int skey = Rnd.Next(48, 122);
                if (skey == 94 || skey == 96) skey++;
                if (!char.IsLetterOrDigit((char)skey))
                {
                    continue;
                }
                if (generatorType != GeneratorType.AllowAll)
                {
                    if ((generatorType & GeneratorType.AllowNumeric) == 0)
                    {
                        if (char.IsDigit((char)skey))
                        {
                            continue;
                        }
                    }
                    if ((generatorType & GeneratorType.AllowUpperCaseChar) == 0)
                    {
                        if (char.IsUpper((char)skey))
                        {
                            continue;
                        }
                    }
                    if ((generatorType & GeneratorType.AllowLowerCaseChar) == 0)
                    {
                        if (char.IsLower((char)skey))
                        {
                            continue;
                        }
                    }
                }
                stringBuilder.Append(((char)skey));
                totalwritten++;
            }
            return stringBuilder.ToString();
        }
        public static string GenerateFileName(string extension)
        {
            DateTime date = DateTime.Now;
            return string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}.{7}", date.Day, date.Month, date.Year, date.Hour, date.Minute, date.Second, date.Millisecond, extension);
        }

        public static string GenerateFileNameFromBase64(string base64String)
        {
            var startIndex = base64String.IndexOf('/') + 1;
            var endIndex = base64String.LastIndexOf(';');
            var extension = base64String.Substring(startIndex, endIndex - startIndex);
            return GenerateFileName(extension);
        }
    }
}
