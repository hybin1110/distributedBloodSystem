using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using FYP_Blood.Models;

namespace FYP_Blood.Controllers
{
    public class AppointmentController : Controller
    {

        //the e_id pass when schedule button click
        [HttpGet]
        public ActionResult Appointment(int id)
        {
            if (Convert.ToString(Session["userrole"]) == "User")
            {
                var eventDetail = new Event();
                String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    string query = "SELECT * FROM Event WHERE e_id = @e_id";
                    SqlCommand sqlcomm = new SqlCommand(query, con);
                    sqlcomm.Parameters.AddWithValue("@e_id", id);
                    con.Open();
                    SqlDataReader rdr = sqlcomm.ExecuteReader();


                    while (rdr.Read())
                    {
                        eventDetail.e_id = Convert.ToInt32(rdr["e_id"]);
                        eventDetail.e_date = Convert.ToDateTime(rdr["e_date"]);
                        eventDetail.e_location = rdr["e_location"].ToString();
                        eventDetail.e_startTime = Convert.ToDateTime(rdr["e_startTime"].ToString());
                        eventDetail.e_endTime = Convert.ToDateTime(rdr["e_endTime"].ToString());
                        eventDetail.e_address = rdr["e_address"].ToString();
                        eventDetail.e_locationPic = rdr["e_locationPic"].ToString();

                    }
                    rdr.Close();
                    con.Close();
                    return View(eventDetail);
                }
            }
            else
            {
                ViewBag.Login = "REMINDER: Please login to perform your operation!";
                return View();
                //return RedirectToAction("Index", "Home");
            }
        }

        
        [HttpPost]
        public ActionResult Appointments(Event events)
        {
            if (Convert.ToString(Session["userrole"]) == "User")
            {
                string userId = Convert.ToString(Session["userid"]);

                String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    string query = "SELECT COUNT(*) FROM [Appointment] WHERE userId = @userID AND status = 'Pending'";
                    SqlCommand sqlcomm = new SqlCommand(query, con);
                    sqlcomm.Parameters.AddWithValue("@userId", userId);
                    con.Open();

                    int rowsAmount = (int)sqlcomm.ExecuteScalar();
                    if (rowsAmount > 0)
                    {
                        ViewBag.Fail = "WARNING: Sorry, you are not able to schedule appointment as you have an ongoing appointment!";
                        con.Close();
                        Appointment(events.e_id);
                        return View("Appointment");
                    }
                    else
                    {
                        string startTime = events.e_startTime.ToString("hh:mmtt");
                        //total quantity for current time slot
                        string query1 = "SELECT COUNT(*) FROM [Appointment] WHERE e_id = @e_id AND apptTimeSlot = @e_starttime";
                        SqlCommand sqlcomm1 = new SqlCommand(query1, con);
                        sqlcomm1.Parameters.AddWithValue("@e_id", events.e_id);
                        sqlcomm1.Parameters.AddWithValue("@e_starttime", startTime);
                        int slotCount = (int)sqlcomm1.ExecuteScalar();

                        string query2 = "Select e_slotQty FROM Event Where e_id = @e_id";
                        SqlCommand sqlcomm2 = new SqlCommand(query2, con);
                        sqlcomm2.Parameters.AddWithValue("@e_id",events.e_id);
                        int maxslot = (int)sqlcomm2.ExecuteScalar();
                        if (slotCount < maxslot)
                        {
                            var appt = new Appt();
                            appt.apptTimeSlot = startTime;
                            appt.e_id = events.e_id;
                            return View("EligibilityForm",appt);

                        
                        }
                        else
                        {
                            ViewBag.Message = "Sorry, this time slot have been occupied!";
                            con.Close();
                            Appointment(events.e_id);
                            return View("Appointment");
                        }

                    }
                }
                return View();
            }
            else
            {
                ViewBag.Login = "REMINDER: Please login to perform your operation!";
                return View();
                //return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EligibilityForm(Appointment appt)
        {
            var userId = Session["userid"];
            String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                if ((appt.form_age == "Yes"|| appt.form_age == "Yes1")&& appt.form_sleepHour == "Yes" && appt.form_weight == "Yes" && appt.form_meal == "Yes" && appt.form_illness == "Yes" &&
                appt.form_highRisk == "Yes" && appt.form_latestDonation == "Yes" && (appt.form_femaleReq == "Yes" || appt.form_femaleReq == "Yes1"))

                {
                    string query3 = "INSERT INTO [Appointment] (userId,e_id,apptTimeSlot) Values (@userId,@e_id,@apptTimeSlot)";
                    SqlCommand sqlcomm3 = new SqlCommand(query3, con);
                    con.Open();
                    sqlcomm3.Parameters.AddWithValue("@userId", userId);
                    sqlcomm3.Parameters.AddWithValue("@e_id", appt.e_id);
                    sqlcomm3.Parameters.AddWithValue("@apptTimeSlot", appt.apptTimeSlot);
                    sqlcomm3.ExecuteNonQuery();
                    //works
                    ViewBag.Success = "Congratulations! Your appointment have successfully booked!";
                    con.Close();
                    return View();
                }
                else
                {
                    //works
                    ViewBag.Fail = "Sorry! You are not eligible to donate blood! Kindly refer to Blood Donor Eligibility Criteria page as your reference.";
                    con.Close();
                    return View();
                }
            }      
           
        }

        public ActionResult DonationBenefit()
        {
            return View();
        }

        public ActionResult EligibilityCriteria()
        {
            return View();
        }


    }

}