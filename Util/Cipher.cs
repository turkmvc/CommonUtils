using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace CommonUtils.Util
{
    public static partial class Cipher
    {
        public static string ConvertToMD5(string variant)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                string hash = GetMd5Hash(md5Hash, variant);
                return hash;
            }
        }
    }
}
