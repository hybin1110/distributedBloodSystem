using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;


namespace FYP_Blood.Models
{
    public class MultipleModelReportEvent
    {
        public IEnumerable<FYP_Blood.Models.Event> EventDetails { get; set; }
        public Event EventDate { get; set; }
    }





}