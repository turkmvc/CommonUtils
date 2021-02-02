using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CommonUtils.Util
{
    public static partial class Cipher
    {
        public static byte[] AES_GenerateIV()
        {
            using (Aes des = Aes.Create())
            {
                return des.IV;
            }
        }

        public static byte[] AES_EncryptByte(byte[] data, byte[] key, byte[] IV)
        {
            if (data == null || data.Length <= 0 || key == null || (key.Length < 8 || key.Length > 16)) return new byte[0];
            byte[] encrypted = new byte[0];
            using (Aes des = Aes.Create())
            {
                if (IV == null) IV = des.IV;
                des.Key = key;
                des.IV = IV;
                ICryptoTransform cryptoTransform = des.CreateEncryptor(key, des.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, 0, data.Length);
                    }
                    encrypted = memoryStream.ToArray();
                    memoryStream.Close();
                }
            }

            return encrypted;
        }
        public static byte[] AES_DecryptByte(byte[] data, byte[] key, byte[] IV)
        {
            if (data == null || data.Length <= 0 || key == null || (key.Length < 8 || key.Length > 16) || IV == null || IV.Length != 16) return new byte[0];
            byte[] decrytped;
            byte[] ibuffer = new byte[1024];
            List<byte> totalBytesRead = new List<byte>();
            using (Aes des = Aes.Create())
            {
                des.Key = key;
                des.IV = IV;
                ICryptoTransform cryptoTransform = des.CreateDecryptor(key, IV);
                using (MemoryStream memoryStream = new MemoryStream(data))
                {
                    try
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Read))
                        {
                            int readed = 0;
                            while ((readed = cryptoStream.Read(ibuffer, 0, ibuffer.Length)) > 0)
                            {
                                for (int i = 0; i < readed; i++)
                                {
                                    totalBytesRead.Add(ibuffer[i]);
                                }
                            }

                        }
                    }
                    catch (Exception)
                    {

                        return new byte[0];
                    }

                    decrytped = totalBytesRead.ToArray();
                    memoryStream.Close();
                }
            }
            return decrytped;
        }
    }
}
