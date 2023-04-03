using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FYP_Blood.Models
{
    public class Event
    {
        public int userId { get; set; }
        public int e_id { get; set; }

        [Required(ErrorMessage = "*This field is required")]
        public DateTime e_date { get; set; }

        [Required(ErrorMessage = "*This field is required")]
        public string e_location { get; set; }

        //startTime
        [Required(ErrorMessage = "*This field is required")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh:mm tt}")]
        public DateTime e_startTime { get; set; }

        //endTime
        [Required(ErrorMessage = "*This field is required")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh:mm tt}")]
        public DateTime e_endTime { get; set; }

 
        [Required(ErrorMessage = "*This field is required")]
        public string e_address { get; set; }

        public string e_locationPic { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }


        [Required(ErrorMessage = "*This field is required")]
        public int e_slotQty { get; set; }
        public int registeredUserCount { get; set; }
         public int eventCount { get; set; }

        //report
        public int ABcount { get; set; }
        public int Acount { get; set; }
        public int Bcount { get; set; }
        public int Ocount { get; set; }

        public DateTime ev_startdate { get; set; }
        public DateTime ev_enddate { get; set; }

        //// user infor
        //public int user_id { get; set; }
        //public string user_name { get; set; }
        //public string user_nricNo { get; set; }
        //public string user_contactNo { get; set; }
        //public string user_gender { get; set; }
        //public string user_bloodType { get; set; }
    }

}