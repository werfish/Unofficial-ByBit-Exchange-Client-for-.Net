using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace ByBitClientLib
{
    public static class CryptoGraFX
    {
        internal static string CreateSignature(string secret, string message)
        {
            //Declaration of the needed variables
            string sign;
            HMACSHA256 hash;
            byte[] SecretKeyBytes;
            byte[] MessegeBytes;
            byte[] signatureBytes;

            //1st Lets start with converting Secret key and Message Strings to byte Arrays
            SecretKeyBytes = Encoding.UTF8.GetBytes(secret);
            MessegeBytes = Encoding.UTF8.GetBytes(message);

            //2nd get the HMACSHA256 Hash out of the Secret and the Message
            hash = new HMACSHA256(SecretKeyBytes); //Intialize the HMAC SHA256 with the KeyBytes
            signatureBytes = hash.ComputeHash(MessegeBytes); //Generate the Signature Message Byte Array

            //3rd Step is to convert the signature Bytes into a String of Hexadecimal Format
            sign = ByteArrayToString(signatureBytes);

            return sign; 
        }


        //The below is the function which converts to Hex String
        private static string ByteArrayToString(byte[] ba)
        {
            var hex = new StringBuilder(ba.Length * 2);

            foreach (var b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
    }
}
