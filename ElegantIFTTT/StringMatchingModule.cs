using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace ElegantIFTTT
{
    public class StringMatchingModule : ModuleBase
    {

        public delegate void TermFoundEvent(ushort termID);
        public TermFoundEvent TermFound { get; set; }

        public override void ProcessEventData(EventData data)
        {
            try
            {
                if (!IsEnabled || EventName != data.Name) { return; }

                for (var i = 0; i < Terms.Count; i++)
                {
                    CrestronConsole.PrintLine("IFTTT: Parsing search data for: " + Terms[i].SearchString);
                    if (ParseSearchData(Terms[i].SearchString, data))
                    {
                        CrestronConsole.PrintLine("IFTTT: Sending term found alert.");
                        TermFound(Terms[i].ID);
                    }
                }
            }
            catch (Exception ex)
            {
                CrestronConsole.PrintLine("IFTTT: Error occurred while parsing data for: " + EventName);
                CrestronConsole.PrintLine("IFTTT: Message is " + ex.Message);
            }
        }

        private bool ParseSearchData(string search, EventData data)
        {
            try
            {
                if (search.StartsWith("!"))
                {
                    search = search.Substring(1, search.Length - 1);
                    return data.DataNotEquals(search);
                }
                else if (search.StartsWith("+"))
                {
                    search = search.Substring(1, search.Length - 1);
                    return data.DataIncludes(search);
                }
                else if (search.StartsWith("-"))
                {
                    search = search.Substring(1, search.Length - 1);
                    return data.DataExcludes(search);
                }
                else if (search.StartsWith(">"))
                {
                    search = search.Substring(1, search.Length - 1);
                    return data.DataGreaterThan(search);
                }
                else if (search.StartsWith("<"))
                {
                    search = search.Substring(1, search.Length - 1);
                    return data.DataLessThan(search);
                }
                else if (search.StartsWith(">="))
                {
                    search = search.Substring(2, search.Length - 2);
                    return data.DataGreaterThanOrEqual(search);
                }
                else if (search.StartsWith("<="))
                {
                    search = search.Substring(2, search.Length - 2);
                    return data.DataLessThanOrEqual(search);
                }
                else
                {
                    if (search.StartsWith("="))
                    {
                        search = search.Substring(1, search.Length - 1);
                    }
                    return data.DataEquals(search);
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