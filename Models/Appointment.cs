using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Web.Mvc;

namespace FYP_Blood.Models
{
    public class Appointment
    {
        public int apptId { get; set; }
        public int userId { get; set; }
        public int e_id { get; set; }
        public string status { get; set; }     
        public int donorId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh:mm tt}")]
        public string apptTimeSlot { get; set; }

        //eligibility form section
        public string form_age { get; set; }
        public string form_weight { get; set; }
        public string form_sleepHour { get; set; }
        public string form_meal { get; set; }
        public string form_illness { get; set; }
        public string form_highRisk { get; set; }
        public string form_latestDonation { get; set; }
        public string form_femaleReq { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime eventdate { get; set; }
        public string e_location { get; set; }
        public int donationCount { get; set; }




        //testing for donor list info
        //public string name { get; set; }
        //public string nricNo { get; set; }
        //public string contactNo { get; set; }
        //public String gender { get; set; }
        //public string bloodType { get; set; }
    }

    public class Appt
    {
        public int apptId { get; set; }
        public int userId { get; set; }
        public int e_id { get; set; }
        public string status { get; set; }
        public int donorId { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh:mm tt}")]
        public string apptTimeSlot { get; set; }

        //eligibility form section
        [Required(ErrorMessage = "*")]
        public string form_age { get; set; }

        [Required(ErrorMessage = "*")]
        public string form_weight { get; set; }

        [Required(ErrorMessage = "*")]
        public string form_sleepHour { get; set; }

        [Required(ErrorMessage = "*")]
        public string form_meal { get; set; }

        [Required(ErrorMessage = "*")]
        public string form_illness { get; set; }

        [Required(ErrorMessage = "*")]
        public string form_highRisk { get; set; }

        [Required(ErrorMessage = "*")]
        public string form_latestDonation { get; set; }

        [Required(ErrorMessage = "*")]
        public string form_femaleReq { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime eventdate { get; set; }

        public string e_location { get; set; }

        public int donationCount { get; set; }

        

    }
}