using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using ElegantIFTTT;

namespace ElegantIFTTT
{
    public class AnalogModule : ModuleBase
    {
        public NumericType NumType { get; set; }
        public int Multiplier { get; set; }

        public delegate void ValueUpdatedEvent(ushort termID, ushort value);
        public ValueUpdatedEvent ValueUpdated { get; set; }

        public delegate void SignedValueUpdatedEvent(ushort termID, short value);
        public SignedValueUpdatedEvent SignedValueUpdated { get; set; }

        public AnalogModule()
        {
            NumType = NumericType.Raw;
        }

        public void Register(string managerID, string eventName, ushort numericType)
        {
            base.Register(managerID, eventName);
            if (numericType == 1)
            {
                NumType = NumericType.Percent;
                Multiplier = 65535 / 100;
            }
            else if (numericType == 2)
            {
                NumType = NumericType.Signed;
            }
        }

        public override void ProcessEventData(EventData data)
        {
            if (!IsEnabled || data.Name != EventName) { return; }
            for (var i = 0; i < Terms.Count; i++)
            {
                if (Terms[i].SearchString != "" &&
                    ParseSearchData(Terms[i].SearchString, data) == false)
                {
                    continue;
                }
                if (NumType == NumericType.Percent ||
                    NumType == NumericType.Raw)
                {
                    ushort val = 0;
                    if (data.Data2 != "")
                    {
                        var data2 = data.Data2.ToUshort();
                        if (data2 != null)
                        {
                            val = data2.Value;
                        }
                        else { continue; }
                    }
                    else if (data.Data1 != "")
                    {
                        var data1 = data.Data1.ToUshort();
                        if (data1 != null)
                        {
                            val = data1.Value;
                        }
                        else { continue; }
                    }
                    if (NumType == NumericType.Raw)
                    {
                        ValueUpdated(Terms[i].ID, val);
                    }
                    else if (NumType == NumericType.Percent)
                    {
                        int percent = Math.Min(val * Multiplier, 65535);
                        ValueUpdated(Terms[i].ID, (ushort)percent);
                    }
                }
                else if (NumType == NumericType.Signed)
                {
                    short val = 0;
                    if (data.Data2 != "")
                    {
                        var data2 = data.Data2.ToShort();
                        if (data2 != null)
                        {
                            val = data2.Value;
                        }
                        else { continue; }
                    }
                    else if (data.Data1 != "")
                    {
                        var data1 = data.Data1.ToShort();
                        if (data1 != null)
                        {
                            val = data1.Value;
                        }
                        else { continue; }
                    }
                        SignedValueUpdated(Terms[i].ID, val);
                }
            }
        }

        private bool ParseSearchData(string search, EventData data)
        {
            try
            {
                if (search.StartsWith("!"))
                {
                    search = search.Substring(1, search.Length - 1);
                    return data.Data1 != search;
                }
                else if (search.StartsWith("+"))
                {
                    search = search.Substring(1, search.Length - 1);
                    return data.Data1.Contains(search);
                }
                else if (search.StartsWith("-"))
                {
                    search = search.Substring(1, search.Length - 1);
                    return !data.Data1.Contains(search);
                }
                else if (search.StartsWith(">"))
                {
                    search = search.Substring(1, search.Length - 1);
                    try
                    {
                        var val = data.ParseInteger(search);
                        if (val != null)
                        {
                            var comp = data.ParseInteger(data.Data1);
                            if (val != null && val > comp) { return true; }
                        }
                    }
                    catch
                    {
                        CrestronConsole.PrintLine("IFTTT: Error parsing value as integer for greater than comparison.");
                    }
                    return false;
                }
                else if (search.StartsWith("<"))
                {
                    search = search.Substring(1, search.Length - 1);
                    try
                    {
                        var val = data.ParseInteger(search);
                        if (val != null)
                        {
                            var comp = data.ParseInteger(data.Data1);
                            if (val != null && val < comp) { return true; }
                        }
                    }
                    catch
                    {
                        CrestronConsole.PrintLine("IFTTT: Error parsing value as integer for greater than comparison.");
                    }
                    return false;
                }
                else if (search.StartsWith(">="))
                {
                    search = search.Substring(2, search.Length - 2);
                    try
                    {
                        var val = data.ParseInteger(search);
                        if (val != null)
                        {
                            var comp = data.ParseInteger(data.Data1);
                            if (val != null && val >= comp) { return true; }
                        }
                    }
                    catch
                    {
                        CrestronConsole.PrintLine("IFTTT: Error parsing value as integer for greater than comparison.");
                    }
                    return false;
                }
                else if (search.StartsWith("<="))
                {
                    search = search.Substring(2, search.Length - 2);
                    try
                    {
                        var val = data.ParseInteger(search);
                        if (val != null)
                        {
                            var comp = data.ParseInteger(data.Data1);
                            if (val != null && val <= comp) { return true; }
                        }
                    }
                    catch
                    {
                        CrestronConsole.PrintLine("IFTTT: Error parsing value as integer for greater than comparison.");
                    }
                    return false;
                }
                else
                {
                    if (search.StartsWith("="))
                    {
                        search = search.Substring(1, search.Length - 1);
                    }
                    return data.Data1 == search;
                }
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("IFTTT: Error occurred while parsing search string: " + search);
                CrestronConsole.PrintLine("IFTTT: Message is " + ex.Message);
                CrestronConsole.PrintLine("IFTTT: Exception is: " + ex.ToString());
                return false;
            }

        }
    }
}