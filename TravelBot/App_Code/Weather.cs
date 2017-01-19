using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelBot.Dialogs
{
    [Serializable]
    public class Weather
    {
        public string Location { get; set; }
        public DateTime Date { get; set; }
    }
}