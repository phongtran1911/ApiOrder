using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace OrderAppAPITest.Models
{
    public class Security
    {
        public static string sha256(string password)
        {
            System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(password), 0, Encoding.UTF8.GetByteCount(password));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
        public static string CreateHmacSha256(string str1, string str2)
        {
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(str2);
            byte[] messageBytes = encoding.GetBytes(str1);
            byte[] hashmessage;
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                hashmessage = hmacsha256.ComputeHash(messageBytes);
            }
            string hex = BitConverter.ToString(hashmessage).Replace("-", "");
            return hex;
        }
    }
}