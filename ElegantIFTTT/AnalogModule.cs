using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using ElegantIFTTT;

namespace ElegantIFTTT
{
    /// <summary>
    /// Used to parse analog values from IFTTT messages sent to the Crestron system.
    /// </summary>
    public class AnalogModule : ModuleBase
    {
        /// <summary>
        /// Determines how the value is processed, unsigned, signed, or percentage.
        /// </summary>
        internal NumericType NumType { get; set; }

        /// <summary>
        /// Multiplier used for percentages.
        /// </summary>
        internal int Multiplier { get; set; }

        /// <summary>
        /// Used to pass an unsigned or percentage value back to Simpl+.
        /// </summary>
        public delegate void ValueUpdatedEvent(ushort termID, ushort value);
        public ValueUpdatedEvent ValueUpdated { get; set; }

        /// <summary>
        /// Used to pass a signed value back to Simpl+.
        /// </summary>
        public delegate void SignedValueUpdatedEvent(ushort termID, short value);
        public SignedValueUpdatedEvent SignedValueUpdated { get; set; }

        /// <summary>
        /// Provided for Simpl+. Use the register method to ready this class.
        /// </summary>
        [Obsolete("Provided for Simpl+. Use the Register method to ready this class.")]
        public AnalogModule() { }

        /// <summary>
        /// Registers the class with the Manager class.
        /// </summary>
        /// <param name="managerID">The ID of the primary IFTTT manager module.</param>
        /// <param name="eventName">The Event Name to look for in communications from the IFTTT service.</param>
        /// <param name="numericType">How to process the analog value. 0: Unsigned, 1: Percent, 2: Signed</param>
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

        /// <summary>
        /// Processes the various data received from the IFTTT service. Only works if IsEnabled is true and the event's name matches the expected name.
        /// </summary>
        internal override void ProcessEventData(EventData data)
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

        /// <summary>
        /// Parses a string to see if it matches any of the EventData's data parts.
        /// </summary>
        /// <param name="search">The string to look for matches using. Can include prefixes to control how matching is performed.</param>
        /// <param name="data">The EventData object to examine for matches.</param>
        /// <returns>True if a match is found, false if no match is found.</returns>
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