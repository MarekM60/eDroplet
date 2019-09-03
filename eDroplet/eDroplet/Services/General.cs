using System;
using System.Collections.Generic;
using System.Text;

namespace eDroplet.Services
{
    public class General
    {
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:X2} ", b);
            return hex.ToString();
        }

        public static string ByteArrayToStringCont(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:X2}", b);
            return hex.ToString();
        }
        public static string ByteArrayToStringCont1(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            int count = 0;
            foreach (byte b in ba)
            {
                if (count > 0 && count < 17)
                {
                    hex.AppendFormat("{0:X2}", b);
                }
                count++;
            }
            return hex.ToString();
        }
    }
}
