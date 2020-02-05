using System;
using System.Security.Cryptography;
using System.Text;

namespace Trigger.Utility
{
    public static class EncryptDecrypt
   {
        private static string cryptoKey = "TTDRB01";

        public static string Encrypt(string plainText)
        {
            byte[] buffer = Encryption(plainText, cryptoKey);
            string cipherText = Convert.ToBase64String(buffer);

            return cipherText;
        }

        private static byte[] Encryption(string plainText, string cryptoKey)
        {
            TripleDES des = CreateDES(cryptoKey);
            ICryptoTransform ct = des.CreateEncryptor();
            byte[] input = Encoding.Unicode.GetBytes(plainText);
            return ct.TransformFinalBlock(input, 0, input.Length);
        }

        public static string Decrypt(string cipherText)
        {
            return Decryption(cipherText, cryptoKey);

        }
        private static string Decryption(string CypherText, string cryptoKey)
        {
            byte[] b = Convert.FromBase64String(CypherText);
            TripleDES des = CreateDES(cryptoKey);
            ICryptoTransform ct = des.CreateDecryptor();
            byte[] output = ct.TransformFinalBlock(b, 0, b.Length);
            return Encoding.Unicode.GetString(output);
        }

        private static TripleDES CreateDES(string cryptoKey)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            TripleDES des = new TripleDESCryptoServiceProvider();
            des.Key = md5.ComputeHash(Encoding.Unicode.GetBytes(cryptoKey));
            des.IV = new byte[des.BlockSize / 8];
            return des;
        }


    }
}
