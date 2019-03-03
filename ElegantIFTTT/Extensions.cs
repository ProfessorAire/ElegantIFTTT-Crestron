using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace ElegantIFTTT
{

    

    public static class Extensions
    {

        public static ushort? ToUshort(this string s)
        {
            try
            {
                if (s.Contains("%"))
                {
                    s = s.Replace("%", "");
                }
                if (s.ToLower().Contains(" percent"))
                {
                    s = s.ToLower().Replace(" percent", "");
                }
                return ushort.Parse(s);
            }
            catch
            {
                return null;
            }
        }

        public static short? ToShort(this string s)
        {
            try
            {

                return short.Parse(s);
            }
            catch
            {
                return null;
            }
        }

    }
}