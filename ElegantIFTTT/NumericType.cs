using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace ElegantIFTTT
{
    /// <summary>
    /// Determines how an analog value should be parsed.
    /// </summary>
    public enum NumericType
    {
        Raw = 0,
        Percent = 1,
        Signed = 2
    }
}