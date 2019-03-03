using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace ElegantIFTTT
{
    /// <summary>
    /// Base class for all parsing modules.
    /// </summary>
    public abstract class ModuleBase
    {
        /// <summary>
        /// The EventName the module will use to parse data from.
        /// </summary>
        internal string EventName { get; set; }

        /// <summary>
        /// If true this module will process event data. If false nothing will happen.
        /// </summary>
        internal bool IsEnabled { get; set; }

        /// <summary>
        /// A list of terms (strings) to examine when processing event data received from the IFTTT service.
        /// </summary>
        internal List<Term> Terms { get; set; }

        /// <summary>
        /// The ID of the Manager module that this parsing module will communicate with to receive information from IFTTT.
        /// </summary>
        internal string ManagerID { get; set; }

        /// <summary>
        /// Must be overridden in child classes. Used to process the event data, parsing it how required to provide matches.
        /// </summary>
        /// <param name="data"></param>
        internal abstract void ProcessEventData(EventData data);

        /// <summary>
        /// Registers the class with the Manager.
        /// </summary>
        /// <param name="managerID">The ID of the manager module this this parsing module will communicate with to receive information from IFTTT.</param>
        /// <param name="eventName">The EventName the module will use to parse data from.</param>
        public void Register(string managerID, string eventName)
        {
            EventName = eventName.ToLower();
            ManagerID = managerID;
            Manager.RegisterModule(this, managerID);
            Terms = new List<Term>();
        }

        /// <summary>
        /// A helper method used to check to see if the manager is ready.
        /// Wait for this to return true, before registering the module, otherwise the registration could fail.
        /// </summary>
        /// <param name="managerID"></param>
        /// <returns></returns>
        public ushort IsManagerReady(string managerID)
        {
            if (Manager.IsManagerRegistered(managerID))
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Adds a search term to the list of terms to use while parsing data.
        /// </summary>
        /// <param name="id">The ID of the term. This should match the 1-based number in the parameter array in Simpl+.</param>
        /// <param name="term">The string provided on the parameter in Simpl+.</param>
        public void AddSearchTerm(ushort id, string term)
        {
            if (Terms == null)
            {
                Terms = new List<Term>();
            }
            var t = new Term();
            t.ID = id;
            t.SearchString = term.ToLower();
            Terms.Add(t);
            CrestronConsole.PrintLine("IFTTT: Added search term " + term.ToLower());
        }

        /// <summary>
        /// Call to enable the module.
        /// </summary>
        public void Enable()
        {
            IsEnabled = true;
        }

        /// <summary>
        /// Call to disable the module.
        /// </summary>
        public void Disable()
        {
            IsEnabled = false;
        }



    }
}