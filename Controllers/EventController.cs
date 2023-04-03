using FYP_Blood.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using PagedList.Mvc;
//using PagedList;

namespace FYP_Blood.Controllers
{
    public class EventController : Controller
    {
        public ActionResult Event()
        {
            List<Event> eventList = new List<Event>();
            String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Event", con);
                cmd.CommandType = CommandType.Text;
                con.Open();

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    var eventDetail = new Event();
                    eventDetail.e_id = Convert.ToInt32(rdr["e_id"]);
                    eventDetail.e_date = Convert.ToDateTime(rdr["e_date"]);
                    eventDetail.e_location = rdr["e_location"].ToString();
                    //ori code can work 
                    eventDetail.e_startTime = Convert.ToDateTime(rdr["e_startTime"].ToString());
                    eventDetail.e_endTime = Convert.ToDateTime(rdr["e_endTime"].ToString());


                    eventDetail.e_address = rdr["e_address"].ToString();
                    eventList.Add(eventDetail);
                }
                rdr.Close();
                con.Close();
            }
            return View(eventList);
        }

        public ActionResult SearchResult(string eventSearch)
        {
            String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            SqlConnection con = new SqlConnection(CS);

            if (string.IsNullOrEmpty(eventSearch))
            {
                return Redirect("/Home/Index");
            }
            else
            {
                string query = "Select * from Event WHERE e_address like '%" + eventSearch + "%'";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                List<Event> eventList = new List<Event>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    eventList.Add(new Event
                    {
                        e_id = Convert.ToInt32(dr["e_id"]),
                        e_location = Convert.ToString(dr["e_location"]),
                        e_date = Convert.ToDateTime(dr["e_date"]),
                        e_startTime = Convert.ToDateTime(dr["e_startTime"].ToString()),
                        e_endTime = Convert.ToDateTime(dr["e_endTime"].ToString()),
                        e_address = Convert.ToString(dr["e_address"])
                    });
                }

                con.Close();
                ModelState.Clear();
                return View(eventList);
            }


        }


        public ActionResult SearchResultLetter(string eventSearch)
        {
            String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            SqlConnection con = new SqlConnection(CS);

            if (string.IsNullOrEmpty(eventSearch))
            {

                return Redirect("/Event/Event");
            }
            else
            {
                string query = "Select * from Event WHERE e_location like '%" + eventSearch + "%'";
                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                List<Event> eventList = new List<Event>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    eventList.Add(new Event
                    {
                        e_id = Convert.ToInt32(dr["e_id"]),
                        e_location = Convert.ToString(dr["e_location"]),
                        e_date = Convert.ToDateTime(dr["e_date"]),
                        e_startTime = Convert.ToDateTime(dr["e_startTime"].ToString()),
                        e_endTime = Convert.ToDateTime(dr["e_endTime"].ToString()),
                        e_address = Convert.ToString(dr["e_address"])
                    });
                }

                con.Close();
                ModelState.Clear();
                return View(eventList);
            }


        }

    }
}