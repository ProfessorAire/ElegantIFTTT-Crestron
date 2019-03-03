using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace ElegantIFTTT
{
    public class SimplEventData
    {
        public SimplSharpString Name { get; set; }
        public SimplSharpString Data1 { get; set; }
        public SimplSharpString Data2 { get; set; }
        public SimplSharpString Data3 { get; set; }
        public SimplSharpString Data4 { get; set; }
        public SimplSharpString Data5 { get; set; }
        public SimplSharpString Data6 { get; set; }
        public SimplSharpString Data7 { get; set; }
        public SimplSharpString Data8 { get; set; }
        public SimplSharpString Data9 { get; set; }
        public SimplSharpString Data10 { get; set; }

        public static SimplEventData ParseEventData(EventData data)
        {
            var sdata = new SimplEventData();
            sdata.Name = data.Name;
            sdata.Data1 = data.Data1;
            sdata.Data2 = data.Data2;
            sdata.Data3 = data.Data3;
            sdata.Data4 = data.Data4;
            sdata.Data5 = data.Data5;
            sdata.Data6 = data.Data6;
            sdata.Data7 = data.Data7;
            sdata.Data8 = data.Data8;
            sdata.Data9 = data.Data9;
            sdata.Data10 = data.Data10;
            return sdata;
        }

    }
}