using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace ElegantIFTTT
{

    
    /// <summary>
    /// Provides string extensions for parsing safely to numeric values.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns a nullable UShort object parsed from a string.
        /// </summary>
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

        /// <summary>
        /// Returns a nullable Short object, parsed from a string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
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