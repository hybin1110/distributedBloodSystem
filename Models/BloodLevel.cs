using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FYP_Blood.Models
{
    public class BloodLevel
    {
        public string bloodlvlid { get; set; }
        public string userid { get; set; }
        public string datecollected { get; set; }
        public int bloodqty { get; set; }
        public string hosname { get; set; }
        public string department { get; set; }

    }
}