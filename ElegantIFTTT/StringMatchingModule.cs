using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace ElegantIFTTT
{
    /// <summary>
    /// Used to perform string matching to trigger digital output events from IFTTT messages sent to the Crestron system.
    /// </summary>
    public class StringMatchingModule : ModuleBase
    {
        /// <summary>
        /// Used to notify that a match was found.
        /// </summary>
        public delegate void TermFoundEvent(ushort termID);
        public TermFoundEvent TermFound { get; set; }

        /// <summary>
        /// Processes the various data received from the IFTTT service. Only works if IsEnabled is true and the event's name matches the expected name.
        /// </summary>
        internal override void ProcessEventData(EventData data)
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