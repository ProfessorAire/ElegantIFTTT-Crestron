using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace ElegantIFTTT
{
    public abstract class ModuleBase
    {
        public string EventName { get; set; }
        public bool IsEnabled { get; set; }
        public List<Term> Terms { get; set; }

        public string ManagerID { get; set; }

        public abstract void ProcessEventData(EventData data);
        public void Register(string managerID, string eventName)
        {
            EventName = eventName.ToLower();
            ManagerID = managerID;
            Manager.RegisterModule(this, managerID);
            Terms = new List<Term>();
        }

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

        public void Enable()
        {
            IsEnabled = true;
        }
        public void Disable()
        {
            IsEnabled = false;
        }



    }
}