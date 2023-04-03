using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using FYP_Blood.Models;
using System.Security.Cryptography;
using System.Text;

namespace FYP_Blood.Controllers
{
    public class AuthorityController : Controller
    {
        [HttpGet]
        public ActionResult CreateEvent()
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult CreateEvent(Event events)
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority")
            {
                //ADDED 16Nov2021 [WORK]
                var userId = Session["userid"];
                //

                if (ModelState.IsValid)
                { 
                    String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(CS))
                    {
                        //Added 16/11 [WORK]
                        string query = "Insert into Event(e_date,e_location,e_startTime,e_endTime,e_address,e_locationPic,e_slotQty,userId) Values (@e_date,@e_location, @e_startTime, @e_endTime, @e_address,@e_locationPic,@e_slotQty,@userId)";
                        //

                        //ori code
                        //string query = "Insert into Event(e_date,e_location,e_startTime,e_endTime,e_address,e_locationPic,e_slotQty) Values (@e_date,@e_location, @e_startTime, @e_endTime, @e_address,@e_locationPic,@e_slotQty)";
                        query += "SELECT SCOPE_IDENTITY()";
                        SqlCommand sqlcomm = new SqlCommand(query, con);
                        con.Open();

                        //added code 16nov [WORK]
                        sqlcomm.Parameters.AddWithValue("@userId", userId);
                        //
                        sqlcomm.Parameters.AddWithValue("@e_date", events.e_date);
                        sqlcomm.Parameters.AddWithValue("@e_location", events.e_location);
                        sqlcomm.Parameters.AddWithValue("@e_startTime", events.e_startTime);
                        sqlcomm.Parameters.AddWithValue("@e_endTime", events.e_endTime);
                        sqlcomm.Parameters.AddWithValue("@e_address", events.e_address);
                        sqlcomm.Parameters.AddWithValue("@e_slotQty", events.e_slotQty);
                        string filename = Path.GetFileNameWithoutExtension(events.ImageFile.FileName);
                        string extension = Path.GetExtension(events.ImageFile.FileName);
                        filename = filename + DateTime.Now.ToString("yymmssfff") + extension;
                        events.e_locationPic = "/Content/images/LocationUpload/" + filename;
                        filename = Path.Combine(Server.MapPath("~/Content/images/LocationUpload/"), filename);
                        events.ImageFile.SaveAs(filename);
                        sqlcomm.Parameters.AddWithValue("@e_locationPic", events.e_locationPic);
                    

                        if(events.e_endTime<events.e_startTime)
                        {
                            ViewBag.Alert = "WARNING: Start time shall be earlier than end time!";
                            con.Close();
                            return View();
                        }
                        else 
                        {
                            sqlcomm.ExecuteNonQuery();
                            ViewBag.Message = "REMINDER: New event has been successfully inserted!";
                            con.Close();
                            return View();
                        }
                    }

                }
                else
                {               
                    return View();               
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult ModifyEvent(int id)
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority")
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
                        eventDetail.e_slotQty = Convert.ToInt32(rdr["e_slotQty"]);
                    }
                    rdr.Close();
                    con.Close();
                    return View(eventDetail);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public ActionResult ModifyEvent(Event events)
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority")
            {
                if (ModelState.IsValid)//if no this one ,event got error visual studio will show u also,need this
                {
                    String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(CS))
                    {
                        string query = "Update Event set e_date = @e_date , e_startTime = @e_startTime, e_endTime = @e_endTime, e_address = @e_address, e_slotQty=@e_slotQty WHERE e_id = @e_id";
                        SqlCommand sqlcomm = new SqlCommand(query, con);
                        sqlcomm.Parameters.AddWithValue("@e_id", events.e_id);
                        sqlcomm.Parameters.AddWithValue("@e_date", events.e_date);
                        sqlcomm.Parameters.AddWithValue("@e_startTime", events.e_startTime);
                        sqlcomm.Parameters.AddWithValue("@e_endTime", events.e_endTime);
                        sqlcomm.Parameters.AddWithValue("@e_address", events.e_address);
                        sqlcomm.Parameters.AddWithValue("@e_slotQty", events.e_slotQty);
                        con.Open();

                        if (events.e_endTime < events.e_startTime)
                        {
                            ViewBag.Alert = "WARNING: Start time shall be earlier than end time!";
                            con.Close();
                            return View();
                        }
                        else
                        {
                            sqlcomm.ExecuteNonQuery();
                            ViewBag.Message = "REMINDER: Event information have been updated!";
                            con.Close();
                            return View();
                        }
                    }
                }
                else
                {
                    ModifyEvent(events.e_id);
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult ViewEvent()
        {

            if (Convert.ToString(Session["userrole"]) == "Hospital Authority")
            {
                //latest 16nov [WORK]
                var userId = Session["userid"];
                //

                List<Event> eventList = new List<Event>();
                String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    //new code [WORK]
                    string query = "SELECT * FROM Event WHERE userId=@userId";
                    SqlCommand sqlcomm = new SqlCommand(query, con);
                    sqlcomm.Parameters.AddWithValue("@userId", userId);
                    //

                    //old
                    //SqlCommand cmd = new SqlCommand("SELECT * FROM Event", con);
                    //cmd.CommandType = CommandType.Text;
                    con.Open();

                    SqlDataReader rdr = sqlcomm.ExecuteReader();
                    while (rdr.Read())
                    {
                        var eventDetail = new Event();
                        eventDetail.e_id = Convert.ToInt32(rdr["e_id"]);
                        eventDetail.e_date = Convert.ToDateTime(rdr["e_date"]);
                        eventDetail.e_location = rdr["e_location"].ToString();
                        eventDetail.e_startTime = Convert.ToDateTime(rdr["e_startTime"].ToString());
                        eventDetail.e_endTime = Convert.ToDateTime(rdr["e_endTime"].ToString());
                        eventDetail.e_slotQty = Convert.ToInt32(rdr["e_slotQty"]);
                        eventDetail.e_address = rdr["e_address"].ToString();
                        eventList.Add(eventDetail);
                    }
                    rdr.Close();
                    con.Close();
                }
                return View(eventList);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult ManageApptList()
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority")
            {
                //latest 16nov [WORK]
                var userId = Session["userid"];
                //

                List<Appointment> apptList = new List<Appointment>();
                String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    //latest 16nov [WORK]
                    string query = "SELECT * FROM [Appointment] INNER JOIN [Event] ON Appointment.e_id = Event.e_id WHERE Event.userId = @userId";
                    SqlCommand sqlcomm = new SqlCommand(query, con);
                    sqlcomm.Parameters.AddWithValue("userId", userId);
                    con.Open();

                    //old code
                    //string query = "SELECT* FROM [Appointment] INNER JOIN [Event] ON APPOINTMENT.e_id = Event.e_id";
                    //con.Open();
                    //SqlCommand sqlcomm = new SqlCommand(query, con);
                    SqlDataReader rdr = sqlcomm.ExecuteReader();

                    while (rdr.Read())
                    {
                        var apptDetail = new Appointment();
                        apptDetail.apptId = Convert.ToInt32(rdr["apptId"]);
                        //apptDetail.donorId = Convert.ToInt32(rdr["donorId"]);
                        apptDetail.e_id = Convert.ToInt32(rdr["e_id"]);
                        apptDetail.apptTimeSlot = rdr["apptTimeSlot"].ToString();
                        apptDetail.status = rdr["status"].ToString();
                        apptDetail.eventdate = Convert.ToDateTime(rdr["e_date"]);
                        apptDetail.e_location = rdr["e_location"].ToString();

                        //add on for user id
                        //apptDetail.userId = Convert.ToInt32(rdr["userId"]);

                        apptList.Add(apptDetail);
                    }
                    rdr.Close();
                    con.Close();
                }
                return View(apptList);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult UpdateDonationStatus(int id)
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority")
            {

                var donationDetail = new Appointment();
                string CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    string query = "SELECT * FROM [Appointment] INNER JOIN [Event] ON APPOINTMENT.e_id = Event.e_id WHERE apptId = @apptId";
                    SqlCommand sqlcomm = new SqlCommand(query, con);
                    sqlcomm.Parameters.AddWithValue("@apptId", id);
                    con.Open();
                    SqlDataReader rdr = sqlcomm.ExecuteReader();

                    while (rdr.Read())
                    {
                        donationDetail.apptId = Convert.ToInt32(rdr["apptId"]);
                        donationDetail.apptTimeSlot = rdr["apptTimeSlot"].ToString();
                        //donationDetail.donorId = Convert.ToInt32(rdr["donorId"]);
                        donationDetail.e_id = Convert.ToInt32(rdr["e_id"]);
                        donationDetail.e_location = rdr["e_location"].ToString();
                        donationDetail.status = rdr["status"].ToString();
                        donationDetail.eventdate = Convert.ToDateTime(rdr["e_date"]);
                    }
                    rdr.Close();
                    con.Close();
                    return View(donationDetail);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }


        [HttpPost]
        public ActionResult UpdateDonationStatus(Appointment appointments)
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority")
            {
                if (ModelState.IsValid)
                {
                    string CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(CS))
                    {
                        string query = "Update Appointment set status = @status where apptId=@apptId";
                        SqlCommand sqlcomm = new SqlCommand(query, con);
                        sqlcomm.Parameters.AddWithValue("@apptId", appointments.apptId);
                        sqlcomm.Parameters.AddWithValue("@status", appointments.status);

                        con.Open();
                        sqlcomm.ExecuteNonQuery();

                        ViewBag.Message = "Congratulation! Appointment status has been updated!";
                        con.Close();
                        return View();

                    }
                }
                else
                {
                    UpdateDonationStatus(appointments.donorId);
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }




        //ori
        public ActionResult TotalUserReport(Event events)
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority")
            {
                //latest 16nov [WORK]
                var userId = Session["userid"];
                //

                string startdate = events.ev_startdate.ToString("MM/dd/yyyy");
                string enddate = events.ev_enddate.ToString("MM/dd/yyyy");

                List<Event> eventList = new List<Event>();
                var multipleModelEvent = new MultipleModelReportEvent();
                multipleModelEvent.EventDate = events;

                String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    string query = "";
                    if (startdate == "01/01/0001")
                    {
                        //old code
                        //query = "SELECT * FROM Event ";

                        //newcode
                        query = "SELECT * FROM Event WHERE userId = @userId";
                    }
                    else
                    {
                        query = "SELECT * FROM Event Where e_date BETWEEN @ev_startdate AND @ev_enddate";
                    }

                    SqlCommand sqlcomm = new SqlCommand(query, con);
                    sqlcomm.Parameters.AddWithValue("@userId", userId);
                    con.Open();
                    if (startdate != "01/01/0001")
                    {
                        string newDate = DateTime.Parse(startdate).ToString();
                        sqlcomm.Parameters.AddWithValue("@ev_startdate", DateTime.Parse(startdate));
                        sqlcomm.Parameters.AddWithValue("@ev_enddate", DateTime.Parse(enddate));

                    }
                    SqlDataReader rdr = sqlcomm.ExecuteReader();
                    while (rdr.Read())
                    {
                        var eventDetail = new Event();
                        eventDetail.e_id = Convert.ToInt32(rdr["e_id"]);
                        eventDetail.e_date = Convert.ToDateTime(rdr["e_date"]);
                        eventDetail.e_location = rdr["e_location"].ToString();
                        eventDetail.e_startTime = Convert.ToDateTime(rdr["e_startTime"].ToString());
                        eventDetail.e_endTime = Convert.ToDateTime(rdr["e_endTime"].ToString());
                        eventDetail.e_address = rdr["e_address"].ToString();

                        string query1 = "SELECT COUNT(userId) FROM Appointment WHERE e_id = @e_id";
                        SqlCommand sqlcomm1 = new SqlCommand(query1, con);
                        sqlcomm1.Parameters.AddWithValue("e_id", eventDetail.e_id);
                        int rowsAmount = (int)sqlcomm1.ExecuteScalar();
                        //****set the webconfig MultipleActiveResultSets=True; already to allow new sql in one reader
                        eventDetail.registeredUserCount = rowsAmount;

                        string query2 = "SELECT COUNT([User].userId) FROM [User] INNER JOIN [Appointment] ON [Appointment].userId = [User].userId " +
                                        "INNER JOIN [Event] ON [Event].e_id=[Appointment].e_id WHERE [Event].e_id = @e_id AND [User].bloodType = 'AB'";
                        SqlCommand sqlcomm2 = new SqlCommand(query2, con);
                        sqlcomm2.Parameters.AddWithValue("e_id", eventDetail.e_id);
                        int ABTypeCount = (int)sqlcomm2.ExecuteScalar();
                        eventDetail.ABcount = ABTypeCount;

                        string query3 = "SELECT COUNT([User].userId) FROM [User] INNER JOIN [Appointment] ON [Appointment].userId = [User].userId " +
                                        "INNER JOIN [Event] ON [Event].e_id=[Appointment].e_id WHERE [Event].e_id = @e_id AND [User].bloodType = 'A'";
                        SqlCommand sqlcomm3 = new SqlCommand(query3, con);
                        sqlcomm3.Parameters.AddWithValue("e_id", eventDetail.e_id);
                        int ATypeCount = (int)sqlcomm3.ExecuteScalar();
                        eventDetail.Acount = ATypeCount;

                        string query4 = "SELECT COUNT([User].userId) FROM [User] INNER JOIN [Appointment] ON [Appointment].userId = [User].userId " +
                                        "INNER JOIN [Event] ON [Event].e_id=[Appointment].e_id WHERE [Event].e_id = @e_id AND [User].bloodType = 'B'";
                        SqlCommand sqlcomm4 = new SqlCommand(query4, con);
                        sqlcomm4.Parameters.AddWithValue("e_id", eventDetail.e_id);
                        int BTypeCount = (int)sqlcomm4.ExecuteScalar();
                        eventDetail.Bcount = BTypeCount;

                        string query5 = "SELECT COUNT([User].userId) FROM [User] INNER JOIN [Appointment] ON [Appointment].userId = [User].userId " +
                                        "INNER JOIN [Event] ON [Event].e_id=[Appointment].e_id WHERE [Event].e_id = @e_id AND [User].bloodType = 'O'";
                        SqlCommand sqlcomm5 = new SqlCommand(query5, con);
                        sqlcomm5.Parameters.AddWithValue("e_id", eventDetail.e_id);
                        int OTypeCount = (int)sqlcomm5.ExecuteScalar();
                        eventDetail.Ocount = OTypeCount;

                        eventList.Add(eventDetail);
                    }
                    multipleModelEvent.EventDetails = eventList;
                    rdr.Close();
                    con.Close();
                }
                return View(multipleModelEvent);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult DashboardOverview()
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority")
            {

                List<Event> eventList = new List<Event>();

                String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {

                    //old code
                    string query = "SELECT * FROM Event";
                    SqlCommand sqlcomm = new SqlCommand(query, con);
                    con.Open();

                    SqlDataReader rdr = sqlcomm.ExecuteReader();
                    while (rdr.Read())
                    {
                        var eventDetail = new Event();
                        eventDetail.e_id = Convert.ToInt32(rdr["e_id"]);
                        eventDetail.e_date = Convert.ToDateTime(rdr["e_date"]);
                        eventDetail.e_location = rdr["e_location"].ToString();
                        eventDetail.e_startTime = Convert.ToDateTime(rdr["e_startTime"].ToString());
                        eventDetail.e_endTime = Convert.ToDateTime(rdr["e_endTime"].ToString());
                        eventDetail.e_address = rdr["e_address"].ToString();
                        string query1 = "SELECT COUNT(userId) FROM Appointment WHERE e_id = @e_id";
                        SqlCommand sqlcomm1 = new SqlCommand(query1, con);
                        sqlcomm1.Parameters.AddWithValue("e_id", eventDetail.e_id);
                        int rowsAmount = (int)sqlcomm1.ExecuteScalar();
                        //****set the webconfig MultipleActiveResultSets=True; already to allow new sql in one reader
                        eventDetail.registeredUserCount = rowsAmount;

                        string query2 = "SELECT COUNT(*) FROM Event";
                        SqlCommand sqlcomm2 = new SqlCommand(query2, con);
                        int eventCount = (int)sqlcomm2.ExecuteScalar();
                        eventDetail.eventCount = eventCount;

                        eventList.Add(eventDetail);
                    }
                    rdr.Close();
                    con.Close();
                }
                return View(eventList);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult donorlist()
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority")
            {
                //latest 16nov [WORK]
                var userId = Session["userid"];
                //

                List<User> donorList = new List<User>();
                String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    //new code
                    string query =  "SELECT [User].userId, [User].name,[User].nricNo,[User].contactNo,[User].email,[User].gender,[User].bloodType " +
                                    "FROM [User] " +
                                    "INNER JOIN [Appointment] ON [Appointment].userId = [User].userId " +
                                    "INNER JOIN [Event] ON [Event].e_id = [Appointment].e_id " +
                                    "WHERE [Event].userId = 1009" +
                                    "GROUP BY [User].userId,[User].name,[User].nricNo,[User].contactNo,[User].email,[User].gender,[User].bloodType";

                    //old code
                    //string query = "SELECT [User].userId, [User].name,[User].nricNo,[User].contactNo,[User].email,[User].gender,[User].bloodType FROM [User] INNER JOIN [Appointment] ON [Appointment].userId = [User].userId " +
                    //               "GROUP BY [User].userId,[User].name,[User].nricNo,[User].contactNo,[User].email,[User].gender,[User].bloodType";
                    con.Open();
                    SqlCommand sqlcomm = new SqlCommand(query, con);
                    sqlcomm.Parameters.AddWithValue("@userId", userId);
                    SqlDataReader rdr = sqlcomm.ExecuteReader();

                    while (rdr.Read())
                    {
                        var donorDetail = new User();
                        donorDetail.userId = Convert.ToInt32(rdr["userId"]);
                        donorDetail.name = rdr["name"].ToString();
                        donorDetail.nricNo = rdr["nricNo"].ToString();
                        donorDetail.contactNo = rdr["contactNo"].ToString();
                        donorDetail.email = rdr["email"].ToString();
                        donorDetail.gender = rdr["gender"].ToString();
                        donorDetail.bloodType = rdr["bloodType"].ToString();

                        donorList.Add(donorDetail);
                    }
                    rdr.Close();
                    con.Close();
                }
                return View(donorList);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }      

        public string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public ActionResult popoutformresult(string seKey)
        {
          
            var userId = Session["userid"];

            List<User> donorList = new List<User>();
            String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            SqlConnection con = new SqlConnection(CS);

            if (string.IsNullOrEmpty(seKey))
            {
                //tempdata can show
                TempData["Feedback"] = "*This field is required";
                return Redirect("/Authority/donorlist");
            }
            else
            {

                string query = "SELECT COUNT(*) FROM [User] WHERE seKey = @seKey AND userId = @userId";
                SqlCommand sqlcomm = new SqlCommand(query, con);
                con.Open();
                sqlcomm.Parameters.AddWithValue("@seKey", seKey);
                sqlcomm.Parameters.AddWithValue("@userId", userId);
                int count = (int)sqlcomm.ExecuteScalar();

                if (count > 0)
                {
                    //new code
                    string query1 = "SELECT [User].userId, [User].name,[User].nricNo,[User].contactNo,[User].email,[User].gender,[User].bloodType " +
                                    "FROM [User] " +
                                    "INNER JOIN [Appointment] ON [Appointment].userId = [User].userId " +
                                    "INNER JOIN [Event] ON [Event].e_id = [Appointment].e_id " +
                                    "WHERE [Event].userId = 1009" +
                                    "GROUP BY [User].userId,[User].name,[User].nricNo,[User].contactNo,[User].email,[User].gender,[User].bloodType";
                    SqlCommand sqlcomm1 = new SqlCommand(query1, con);
                    SqlDataReader rdr1 = sqlcomm1.ExecuteReader();

                    //old code
                    //string query1 = "SELECT [User].userId, [User].name,[User].nricNo,[User].contactNo,[User].email,[User].gender,[User].bloodType FROM [User] INNER JOIN [Appointment] ON [Appointment].userId = [User].userId " +
                    //                "GROUP BY [User].userId,[User].name,[User].nricNo,[User].contactNo,[User].email,[User].gender,[User].bloodType";
                    //SqlCommand sqlcomm1 = new SqlCommand(query1, con);
                    //SqlDataReader rdr1 = sqlcomm1.ExecuteReader();

                    while (rdr1.Read())
                    {
                        var donorDetail = new User();
                        donorDetail.userId = Convert.ToInt32(rdr1["userId"]);
                        donorDetail.name = rdr1["name"].ToString();
                        donorDetail.nricNo = Decrypt(rdr1["nricNo"].ToString());
                        donorDetail.contactNo = Decrypt(rdr1["contactNo"].ToString());
                        donorDetail.email = rdr1["email"].ToString();
                        donorDetail.gender = rdr1["gender"].ToString();
                        donorDetail.bloodType = rdr1["bloodType"].ToString();

                        donorList.Add(donorDetail);
                    }
                    rdr1.Close();
                    con.Close();
                    return View(donorList);
                }
                else
                {
                    ViewBag.Alert = "ALERT: Secret key entered incorrect!";
                    con.Close();
                    return View();

                }

                
                //return View(donorList);
            }
        }


        public ActionResult ReportSelection()
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }





    }
}