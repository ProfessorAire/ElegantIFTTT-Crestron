using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace ElegantIFTTT
{
    public class EventData
    {

        public string Name { get; set; }
        public string Data1 { get; set; }
        public string Data2 { get; set; }
        public string Data3 { get; set; }
        public string Data4 { get; set; }
        public string Data5 { get; set; }
        public string Data6 { get; set; }
        public string Data7 { get; set; }
        public string Data8 { get; set; }
        public string Data9 { get; set; }
        public string Data10 { get; set; }

        public static EventData Parse(string[] data)
        {
            var eventData = new EventData();
            eventData.Name = data[0].ToLower();
            if (data.Length > 1) { eventData.Data1 = data[1].ToLower(); } else { eventData.Data1 = ""; }
            if (data.Length > 2) { eventData.Data2 = data[2].ToLower(); } else { eventData.Data2 = ""; }
            if (data.Length > 3) { eventData.Data3 = data[3].ToLower(); } else { eventData.Data3 = ""; }
            if (data.Length > 4) { eventData.Data4 = data[4].ToLower(); } else { eventData.Data4 = ""; }
            if (data.Length > 5) { eventData.Data5 = data[5].ToLower(); } else { eventData.Data5 = ""; }
            if (data.Length > 6) { eventData.Data6 = data[6].ToLower(); } else { eventData.Data6 = ""; }
            if (data.Length > 7) { eventData.Data7 = data[7].ToLower(); } else { eventData.Data7 = ""; }
            if (data.Length > 8) { eventData.Data8 = data[8].ToLower(); } else { eventData.Data8 = ""; }
            if (data.Length > 9) { eventData.Data9 = data[9].ToLower(); } else { eventData.Data9 = ""; }
            if (data.Length > 10) { eventData.Data10 = data[10].ToLower(); } else { eventData.Data10 = ""; }
            return eventData;
        }

        public bool DataEquals(string search)
        {
            if (Data1 == search || Data2 == search || Data3 == search || Data4 == search || Data5 == search ||
                Data6 == search || Data7 == search || Data8 == search || Data9 == search || Data10 == search)
            {
                return true;
            }
            return false;
        }

        public bool DataNotEquals(string search)
        {
            if (Data1 == search || Data2 == search || Data3 == search || Data4 == search || Data5 == search ||
                Data6 == search || Data7 == search || Data8 == search || Data9 == search || Data10 == search)
            {
                return false;
            }
            return true;
        }

        public bool DataIncludes(string search)
        {
            if (Data1.Contains(search) || Data2.Contains(search) || Data3.Contains(search) || Data4.Contains(search) || Data5.Contains(search) ||
                Data6.Contains(search) || Data7.Contains(search) || Data8.Contains(search) || Data9.Contains(search) || Data10.Contains(search))
            {
                return true;
            }
            return false;
        }

        public bool DataExcludes(string search)
        {
            if (Data1.Contains(search) || Data2.Contains(search) || Data3.Contains(search) || Data4.Contains(search) || Data5.Contains(search) ||
                Data6.Contains(search) || Data7.Contains(search) || Data8.Contains(search) || Data9.Contains(search) || Data10.Contains(search))
            {
                return false;
            }
            return true;
        }

        public bool DataGreaterThan(string search)
        {
            try
            {
                var val = ParseInteger(search);
                if(val != null)
                {
                    var comp = ParseInteger(Data1);
                    if (val != null && val > comp) { return true; }
                    comp = ParseInteger(Data2);
                    if (val != null && val > comp) { return true; }
                    comp = ParseInteger(Data3);
                    if (val != null && val > comp) { return true; }
                    comp = ParseInteger(Data4);
                    if (val != null && val > comp) { return true; }
                    comp = ParseInteger(Data5);
                    if (val != null && val > comp) { return true; }
                    comp = ParseInteger(Data6);
                    if (val != null && val > comp) { return true; }
                    comp = ParseInteger(Data7);
                    if (val != null && val > comp) { return true; }
                    comp = ParseInteger(Data8);
                    if (val != null && val > comp) { return true; }
                    comp = ParseInteger(Data9);
                    if (val != null && val > comp) { return true; }
                    comp = ParseInteger(Data10);
                    if (val != null && val > comp) { return true; }
                }
            }
            catch
            {
                CrestronConsole.PrintLine("IFTTT: Error parsing value as integer for greater than comparison.");
            }
            return false;
        }

        public int? ParseInteger(string value)
        {
            try
            {
                return ParseInteger(value);
            }
            catch
            {
                return null;
            }
        }

        public bool DataLessThan(string search)
        {
            try
            {
                var val = ParseInteger(search);
                if (val != null)
                {
                    var comp = ParseInteger(Data1);
                    if (val != null && val < comp) { return true; }
                    comp = ParseInteger(Data2);
                    if (val != null && val < comp) { return true; }
                    comp = ParseInteger(Data3);
                    if (val != null && val < comp) { return true; }
                    comp = ParseInteger(Data4);
                    if (val != null && val < comp) { return true; }
                    comp = ParseInteger(Data5);
                    if (val != null && val < comp) { return true; }
                    comp = ParseInteger(Data6);
                    if (val != null && val < comp) { return true; }
                    comp = ParseInteger(Data7);
                    if (val != null && val < comp) { return true; }
                    comp = ParseInteger(Data8);
                    if (val != null && val < comp) { return true; }
                    comp = ParseInteger(Data9);
                    if (val != null && val < comp) { return true; }
                    comp = ParseInteger(Data10);
                    if (val != null && val < comp) { return true; }
                }
            }
            catch
            {
                CrestronConsole.PrintLine("IFTTT: Error parsing value as integer for greater than comparison.");
            }
            return false;
        }

        public bool DataGreaterThanOrEqual(string search)
        {
            try
            {
                var val = ParseInteger(search);
                if (val != null)
                {
                    var comp = ParseInteger(Data1);
                    if (val != null && val != null && val >= comp) { return true; }
                    comp = ParseInteger(Data2);
                    if (val != null && val >= comp) { return true; }
                    comp = ParseInteger(Data3);
                    if (val != null && val >= comp) { return true; }
                    comp = ParseInteger(Data4);
                    if (val != null && val >= comp) { return true; }
                    comp = ParseInteger(Data5);
                    if (val != null && val >= comp) { return true; }
                    comp = ParseInteger(Data6);
                    if (val != null && val >= comp) { return true; }
                    comp = ParseInteger(Data7);
                    if (val != null && val >= comp) { return true; }
                    comp = ParseInteger(Data8);
                    if (val != null && val >= comp) { return true; }
                    comp = ParseInteger(Data9);
                    if (val != null && val >= comp) { return true; }
                    comp = ParseInteger(Data10);
                    if (val != null && val >= comp) { return true; }
                }
            }
            catch
            {
                CrestronConsole.PrintLine("IFTTT: Error parsing value as integer for greater than comparison.");
            }
            return false;
        }

        public bool DataLessThanOrEqual(string search)
        {
            try
            {
                var val = ParseInteger(search);
                if (val != null)
                {
                    var comp = ParseInteger(Data1);
                    if (val != null && val <= comp) { return true; }
                    comp = ParseInteger(Data2);
                    if (val != null && val <= comp) { return true; }
                    comp = ParseInteger(Data3);
                    if (val != null && val <= comp) { return true; }
                    comp = ParseInteger(Data4);
                    if (val != null && val <= comp) { return true; }
                    comp = ParseInteger(Data5);
                    if (val != null && val <= comp) { return true; }
                    comp = ParseInteger(Data6);
                    if (val != null && val <= comp) { return true; }
                    comp = ParseInteger(Data7);
                    if (val != null && val <= comp) { return true; }
                    comp = ParseInteger(Data8);
                    if (val != null && val <= comp) { return true; }
                    comp = ParseInteger(Data9);
                    if (val != null && val <= comp) { return true; }
                    comp = ParseInteger(Data10);
                    if (val != null && val <= comp) { return true; }
                }
            }
            catch
            {
                CrestronConsole.PrintLine("IFTTT: Error parsing value as integer for greater than comparison.");
            }
            return false;
        }

    }
}