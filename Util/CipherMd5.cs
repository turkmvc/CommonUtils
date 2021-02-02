using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CommonUtils.Util
{
    public static partial class Cipher
    {
        public static bool Md5Matches(string source, string target)
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;
            if (comparer.Compare(source, target) == 0)
            {
                return true;
            }
            return false;
        }
        private static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
