using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using FYP_Blood.Models;
using System.Security.Cryptography;

namespace FYP_Blood.Controllers
{

    public class ProfileController : Controller
    {       
        [HttpGet]
        public ActionResult Profile()
        {
            var userId = Session["userid"];
            var userDetail = new User();
            String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = "SELECT * FROM [User] WHERE userId = @userId";
                SqlCommand sqlcomm = new SqlCommand(query, con);
                sqlcomm.Parameters.AddWithValue("@userId", userId);
                con.Open();
                SqlDataReader rdr = sqlcomm.ExecuteReader();

                while (rdr.Read())
                {
                    userDetail.name = rdr["name"].ToString();
                    userDetail.nricNo = Decrypt(rdr["nricNo"].ToString());
                    userDetail.dob = Convert.ToDateTime(rdr["dob"]);
                    userDetail.contactNo = Decrypt(rdr["contactNo"].ToString());
                    userDetail.email = rdr["email"].ToString();
                    userDetail.address = Decrypt(rdr["address"].ToString());
                    userDetail.gender = rdr["gender"].ToString();
                    userDetail.bloodType = rdr["bloodType"].ToString();
                }
                rdr.Close();

                //Total quantity of donated blood
                string query2 = "SELECT COUNT(*) FROM [Appointment] WHERE userId = @userID AND status = 'Completed'";
                SqlCommand sqlcomm1 = new SqlCommand(query2, con);
                sqlcomm1.Parameters.AddWithValue("@userID", userId);
                //con.Open();
                int rowsAmount = (int)sqlcomm1.ExecuteScalar();
                //donation count
                userDetail.donationCount = rowsAmount;

                //To get the information of upcoming donation, status with pending(0) mean ongoing
                string query3 = "SELECT * FROM [Appointment] INNER JOIN [Event] ON Event.e_id = Appointment.e_id WHERE Appointment.userId = @userID AND status = 'Pending'";
                SqlCommand sqlcomm2 = new SqlCommand(query3, con);
                sqlcomm2.Parameters.AddWithValue("@userID", userId);
                SqlDataReader rdr1 = sqlcomm2.ExecuteReader();

                while (rdr1.Read())
                {
                    var appointment = new Appointment();
                    appointment.e_id = Convert.ToInt32(rdr1["e_id"]);
                    userDetail.eventTime = rdr1["apptTimeSlot"].ToString();
                    userDetail.appt_Id = Convert.ToInt32(rdr1["apptId"]);
                    userDetail.eventlocation = rdr1["e_location"].ToString();
                    userDetail.eventdate = Convert.ToDateTime(rdr1["e_date"]);

                }
                rdr1.Close();
                                     
                con.Close();
                return View(userDetail);
            }
        }

        public ActionResult DeleteAppt(int id)
        {
            var userId = Session["userid"];
            var userDetail = new User();
            String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = "DELETE FROM [Appointment] WHERE apptId = @apptId AND userId = @userId";
                SqlCommand sqlcomm = new SqlCommand(query, con);
                sqlcomm.Parameters.AddWithValue("@apptId", id);
                sqlcomm.Parameters.AddWithValue("@userId", userId);
                con.Open();
                sqlcomm.ExecuteNonQuery();
            }

            return Redirect("/Profile/Profile");
        }

        [HttpGet]
        public ActionResult EditProfile()
        {
            var userId = Session["userid"];
            var userDetail = new User();

            String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = "SELECT * FROM [User] WHERE userId = @userId";
                SqlCommand sqlcomm = new SqlCommand(query, con);
                sqlcomm.Parameters.AddWithValue("@userId", userId);
                con.Open();

                SqlDataReader rdr = sqlcomm.ExecuteReader();

                while (rdr.Read())
                {
                    userDetail.userId = Convert.ToInt32(rdr["userId"]);
                    userDetail.nricNo = Decrypt(rdr["nricNo"].ToString());
                    userDetail.dob = Convert.ToDateTime(rdr["dob"].ToString());
                    userDetail.contactNo = Decrypt(rdr["contactNo"].ToString());
                    userDetail.email = rdr["email"].ToString();
                    userDetail.address = Decrypt(rdr["address"].ToString());
                    userDetail.gender = rdr["gender"].ToString();
                    userDetail.bloodType = rdr["bloodType"].ToString();

                    //password use for current only as without login part
                    //userDetail.password = Decrypt(rdr["password"].ToString());
                    //userDetail.seKey = rdr["seKey"].ToString();
                }
                rdr.Close();
                con.Close();
                return View(userDetail);

            }
        }


        [HttpPost]
        public ActionResult EditProfile(User user)
        {
            if (ModelState.IsValid)//if no this one ,event got error visual studio will show u also,need this
            {
                String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                string test = Convert.ToString(Session["userid"]);

                using (SqlConnection con = new SqlConnection(CS))
                {
                    string query = "Update [User] set nricNo = @nricNo , dob = @dob, contactNo = @contactNo, address = @address,gender = @gender,bloodType = @bloodType WHERE userId = @userId";
                    SqlCommand sqlcomm = new SqlCommand(query, con);
                    sqlcomm.Parameters.AddWithValue("@userId", test);
                    sqlcomm.Parameters.AddWithValue("@nricNo", Encrypt(user.nricNo));
                    sqlcomm.Parameters.AddWithValue("@dob", user.dob);
                    sqlcomm.Parameters.AddWithValue("@contactNo", Encrypt(user.contactNo));
                    sqlcomm.Parameters.AddWithValue("@email", Encrypt(user.email));
                    sqlcomm.Parameters.AddWithValue("@gender", user.gender);
                    sqlcomm.Parameters.AddWithValue("@bloodType", user.bloodType);
                    sqlcomm.Parameters.AddWithValue("@address", Encrypt(user.address));

                    //testing for passowrd encryption, use for current only as without login part
                    //sqlcomm.Parameters.AddWithValue("@password", Encrypt(user.password));
                    //sqlcomm.Parameters.AddWithValue("@seKey", user.seKey);
                    con.Open();
                    sqlcomm.ExecuteNonQuery();

                    //new code work can redirect to profile already
                    ViewBag.Message = "Congratulations, your information have been updated!";
                    con.Close();
                    return View();
                }
            }
            else
            {
                EditProfile();
                return View();
            }
        }

        [HttpGet]
        public ActionResult completeProfile()
        {
            var useremail = Session["useremail"];
            var userDetail = new User();

            String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                string query = "SELECT userId FROM [User] WHERE Email = @email";
                SqlCommand sqlcomm = new SqlCommand(query, con);
                sqlcomm.Parameters.AddWithValue("@email", useremail);
                con.Open();

                SqlDataReader rdr = sqlcomm.ExecuteReader();

                while (rdr.Read())
                {
                    userDetail.userId = Convert.ToInt32(rdr["userId"]);                  
                }
                Session["userid"] = userDetail.userId;
                rdr.Close();
                con.Close();
                return View(userDetail);

            }
        }

        [HttpPost]
        public ActionResult completeProfile(User user)
        {
            //if (ModelState.IsValid)//if no this one ,event got error visual studio will show u also,need this
            //{
                String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    string query = "Update [User] set nricNo = @nricNo , dob = @dob, contactNo = @contactNo, address = @address,gender = @gender,bloodType = @bloodType WHERE userId = @userId";
                    SqlCommand sqlcomm = new SqlCommand(query, con);
                    sqlcomm.Parameters.AddWithValue("@userId", user.userId);
                    sqlcomm.Parameters.AddWithValue("@nricNo", Encrypt(user.nricNo));
                    sqlcomm.Parameters.AddWithValue("@dob", user.dob);
                    sqlcomm.Parameters.AddWithValue("@contactNo", Encrypt(user.contactNo));
                    //sqlcomm.Parameters.AddWithValue("@email", Encrypt(user.email));
                    sqlcomm.Parameters.AddWithValue("@gender", user.gender);
                    sqlcomm.Parameters.AddWithValue("@bloodType", user.bloodType);
                    sqlcomm.Parameters.AddWithValue("@address", Encrypt(user.address));

                    //testing for passowrd encryption, use for current only as without login part
                    //sqlcomm.Parameters.AddWithValue("@password", Encrypt(user.password));
                    //sqlcomm.Parameters.AddWithValue("@seKey", user.seKey);
                    con.Open();
                    sqlcomm.ExecuteNonQuery();

                    //new code work can redirect to profile already
                    ViewBag.Message = "Congratulations, profile is updated! Please proceed to login.";
                    con.Close();
                    return View();
                }
            //}
            //else
            //{
            //    EditProfile();
            //    return View();
            //}
        }

        public ActionResult donationHistory()
        {
            var userId = Session["userid"];

            List<Appointment> appointmentList = new List<Appointment>();
            String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                //if need count need to change to SELECT COUNT(*)
                string query0 = "SELECT * FROM [Appointment] INNER JOIN [Event] ON Appointment.e_id = Event.e_id WHERE Appointment.userId = @userId AND Appointment.status = 'Completed'";
                SqlCommand sqlcomm0 = new SqlCommand(query0, con);
                sqlcomm0.Parameters.AddWithValue("@userId", userId);
                con.Open();
                SqlDataReader rdr1 = sqlcomm0.ExecuteReader();

                while (rdr1.Read())
                {
                    var appointment = new Appointment();
                    appointment.e_id = Convert.ToInt32(rdr1["e_id"]);
                    appointment.apptTimeSlot = rdr1["apptTimeSlot"].ToString();
                    appointment.e_location = rdr1["e_location"].ToString();
                    appointment.eventdate = Convert.ToDateTime(rdr1["e_date"]);
                    appointmentList.Add(appointment);
                }
                rdr1.Close();             
                con.Close();
            }
            return View(appointmentList);
        }

        
        private string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
                //create cyptographic obj that use to perform symmetric algorithm
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                //use this class to create 1 identical key for the aes class
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                //getBytes() encode given string into sql of bytes, return array of bytes
                //IV:Initialization vector = random num used in combination with a secret key as a mean,to encrypt data
                using (MemoryStream ms = new MemoryStream())
                //create stream that have memory as a backing store
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    //initialize new instance of cryptostream class with a target stream
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        private string Decrypt(string cipherText)
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

      

    }
    
}