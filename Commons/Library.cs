﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Commons
{
    public class Library
    {
        /// <summary>
        /// Mã hóa chuỗi thành chuỗi MD5
        /// Input: sToEncrypt (Chuỗi cần mã hóa)
        /// Output: Chuỗi sau khi mã hóa
        /// </summary>
        public static string EncryptMD5(string sToEncrypt)
        {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(sToEncrypt);

            // encrypt bytes
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');

        }
        public static string ConvertToUnSign(string text)
        {

            for (int i = 33; i < 48; i++)
            {

                text = text.Replace(((char)i).ToString(), "");

            }



            for (int i = 58; i < 65; i++)
            {

                text = text.Replace(((char)i).ToString(), "");

            }



            for (int i = 91; i < 97; i++)
            {

                text = text.Replace(((char)i).ToString(), "");

            }

            for (int i = 123; i < 127; i++)
            {

                text = text.Replace(((char)i).ToString(), "");

            }

            text = text.Replace(" ", "-");
            text = text.Replace("\"", "'");
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");

            string strFormD = text.Normalize(System.Text.NormalizationForm.FormD);

            return regex.Replace(strFormD, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');

        }
        public static DateTime GetServerDateTime()
        {
            return DateTime.Now.ToUniversalTime().AddHours(7);
        }
    }
}
