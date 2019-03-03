using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace ElegantIFTTT
{
    /// <summary>
    /// A search term for parsing data.
    /// </summary>
    public class Term
    {
        /// <summary>
        /// The 1-based ID of the term, matching the Simpl+ parameter's index.
        /// </summary>
        public ushort ID { get; set; }
        /// <summary>
        /// The string from the matching Simpl+ parameter.
        /// </summary>
        public string SearchString { get; set; }
    }
}