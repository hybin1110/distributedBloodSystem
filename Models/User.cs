using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FYP_Blood.Models
{
    public class User
    {
        public int userId { get; set; }
        public string name { get; set; }

        [Required(ErrorMessage = "*Required!")]
        public string contactNo { get; set; }

        [Required(ErrorMessage = "*Required!")]
        public string gender { get; set; }

        [Required(ErrorMessage = "*Required!")]
        public string email { get; set; }
        public string password { get; set; }
        public string role { get; set; }
        public string status { get; set; }
        public string department { get; set; }
        public string message { get; set; }
        public string resetcode { get; set; }

        [Required(ErrorMessage = "*Required!")]
        public string nricNo { get; set; }

        [Required(ErrorMessage = "*Required!")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime dob { get; set; }

        [Required(ErrorMessage = "*Required!")]
        public string bloodType { get; set; }

        [Required(ErrorMessage = "*Required!")]
        public string address { get; set; }

        public int donationCount { get; set; }

        public string seKey { get; set; }

        public int appt_Id { get; set; }

        //testing for merge class
        public string eventlocation { get; set; }

        public DateTime eventdate { get; set; }

        public string eventTime { get; set; }
    }
}