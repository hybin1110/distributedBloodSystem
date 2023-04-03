using FYP_Blood.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.ABI.FunctionEncoding.Attributes;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;
using ZXing;
using System.Drawing;
using System.Drawing.Imaging;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;

namespace FYP_Blood.Controllers
{
    public class HospitalController : Controller
    {
        Database_Access_Layer.db dblayer = new Database_Access_Layer.db();
        BloodLevel bl = new BloodLevel();

        public ActionResult HospitalLogOut()
        {
            if (!string.IsNullOrEmpty(Session["user"] as string))
            {
                Session["user"] = null;
                Session.Abandon();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("PageNotFound", "Home");
            }
        }

        public ActionResult Dashboard()
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority")
            {
                //latest 11nov
                var userId = Session["userid"];
                //


                List<Models.Event> eventList = new List<Models.Event>();

                String CS = ConfigurationManager.ConnectionStrings["con"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    string query = "SELECT * FROM Event WHERE userId = @userId";
                    SqlCommand sqlcomm = new SqlCommand(query, con);
                    sqlcomm.Parameters.AddWithValue("@userId", userId);
                    con.Open();

                    //string query = "SELECT * FROM Event ";
                    //SqlCommand sqlcomm = new SqlCommand(query, con);
                    //con.Open();

                    SqlDataReader rdr = sqlcomm.ExecuteReader();
                    while (rdr.Read())
                    {
                        var eventDetail = new Models.Event();
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

        /*-----------------------------------------------BLOOD LEVEL------------------------------------------ */

        public ActionResult BloodLevel()
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority")
            {
                return View();
            }
            else
            {
                return RedirectToAction("PageUnauthorised", "Home");
            }
                
        }

        //Blood Level Module
        public ActionResult GetBloodLevel()
        {
            try
            {
                string currentname = Convert.ToString(Session["user"]);
                string currentid = Convert.ToString(Session["userid"]);
                List<BloodLevel> bloodLvl = dblayer.GetBloodLevel(currentname);

                return Json(bloodLvl, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on loading blood level listing..." + ex.Message);
            }

        }

        public ActionResult UpdateBloodLevel(string bloodlvldate, int bloodlvlqty)
        {
            try
            {
                string currentid = Convert.ToString(Session["userid"]);
                dblayer.UpdateBloodLevel(bloodlvldate, bloodlvlqty, currentid);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on Updating Blood Level" + ex.Message);
            }
        }

        /*-----------------------------------------------APPOINTMENT------------------------------------------ */

        public ActionResult ValidateAptId(int _aptid)
        {
            int result = dblayer.ValidateAptId(_aptid);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        //Update Appointment ID Blood result status to completed
        public void UpdateAptResult(int _aptid)
        {
            try
            {
                dblayer.UpdateAptResult(_aptid);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on saving appointment update blood result " + ex.Message);
            }

        }

        //Generate QR Code 
        //[HttpPost]
        public void Generate(string _donorid)
        {
            QRCodeModel qrcode = new QRCodeModel();
            try
            {
                qrcode.QRCodeImagePath = GenerateQRCode(_donorid);
                //TempData["Imagename"] = Server.MapPath("~") + "image/QrCode.jpg"; //qrcode.QRCodeImagePath;

            }
            catch (Exception ex)
            {
                throw new Exception("Error on showing qrcode" + ex.Message);
            }
        }

        private string GenerateQRCode(string qrcodeText)
        {
            string folderPath = "~/image/";
            string imagePath = "~/image/QrCode.jpg";

            //if the directory doesnt exist then create it.
            if (!Directory.Exists(Server.MapPath(folderPath)))
            {
                Directory.CreateDirectory(Server.MapPath(folderPath));
            }

            var barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            //convert param string into image
            var result = barcodeWriter.Write(qrcodeText);

            string barcodePath = Server.MapPath(imagePath);
            var barcodeBitmap = new Bitmap(result);
            //copied into custom path "~/image/QrCode.jpg
            using (MemoryStream memory = new MemoryStream())
            {
                using (FileStream fs = new FileStream(barcodePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    barcodeBitmap.Save(memory, ImageFormat.Jpeg);
                    byte[] bytes = memory.ToArray();
                    fs.Write(bytes, 0, bytes.Length);
                }
            }
            return imagePath;
        }

        /*-----------------------------------------------REPORTING------------------------------------------ */
        public ActionResult BloodReport()
        {
                //SSRS Report Without Parameter
                //string ssrsUrl = ConfigurationManager.AppSettings["SSRSReportsUrl"].ToString();
                //ReportViewer viewer = new ReportViewer();
                //viewer.ProcessingMode = ProcessingMode.Remote;
                //viewer.SizeToReportContent = true;
                //viewer.AsyncRendering = true;
                //viewer.ServerReport.ReportServerUrl = new Uri(ssrsUrl);
                //viewer.ServerReport.ReportPath = "/ReportBlood";

                //ViewBag.ReportViewer = viewer;
                return View();
        }

        [HttpPost]
        public ActionResult BloodReport(string startDate, string endDate)
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority")
            {
                string ssrsUrl = ConfigurationManager.AppSettings["SSRSReportsUrl"].ToString();
                ReportViewer viewer = new ReportViewer();
                viewer.ProcessingMode = ProcessingMode.Remote;
                viewer.SizeToReportContent = true;
                viewer.AsyncRendering = true;
                viewer.ServerReport.ReportServerUrl = new Uri(ssrsUrl);
                viewer.ServerReport.ReportPath = "/ReportBlood2";

                //String[] userValue = new string[] { startDate, endDate };
                ReportParameter param = new ReportParameter("StartDate");
                param.Name = "StartDate";
                param.Values.Add(startDate);
                viewer.ServerReport.SetParameters(param);
                ReportParameter secondparam = new ReportParameter("EndDate");
                secondparam.Name = "EndDate";
                secondparam.Values.Add(endDate);
                viewer.ServerReport.SetParameters(secondparam);


                ViewBag.ReportViewer = viewer;
                 return View("BloodReport");
            }
            else
            {
                return RedirectToAction("PageUnauthorised", "Home");
            }
        }
    }

}