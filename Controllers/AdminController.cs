using FYP_Blood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace FYP_Blood.Controllers
{
    public class AdminController : Controller
    {
        Database_Access_Layer.db dblayer = new Database_Access_Layer.db();
        static int authorityCount;
        static int hospitalCount;
        static string randomKey;

        // GET: Admin
        public ActionResult AdminDashboard()
        {
            if (Convert.ToString(Session["userrole"]) == "Admin")
            {
                PartnerCount();
                GetAuthorityAccount();
                ViewBag.TotalAuthority = authorityCount;
                ViewBag.TotalPartner = hospitalCount;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult AdminLogOut()
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

        public ActionResult AdminManagement()
        {
            if (Convert.ToString(Session["userrole"]) == "Admin")
            {
                 return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
               
        }

        /*-----------------------------------------------HOSPITAL AUTHORITY MODULE------------------------------------------ */

        //New Authority Account
        public string RegisterAuthority(string name, string username, string dept, string password)
        {
            try
            {
                User user = new User();
                dblayer.RegisterHospitalAuthority(name, username, dept, password);
                user.seKey = randomKey;
                user.message = "Success";
                //return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
                var json = new JavaScriptSerializer().Serialize(user);
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("Error on Registering New Hospital Authority" + ex.Message);
            }
        }

        //Generate Secret Key
        public string GenSecretKey()
        {
            Random generator = new Random();
            randomKey = generator.Next(0, 1000000).ToString("D6");
            return randomKey;
        }

        //Authority Account Listing
        public ActionResult GetAuthorityAccount()
        {
            try
            {
                List<User> user = dblayer.GetAuthorityAccount();
                authorityCount = user.Count();
                return Json(user, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on loading authority listing..." + ex.Message);
            }
        }

        public ActionResult UpdateAuthorityAccount(string name, string dept, string status, string userid)
        {
            try
            {
                dblayer.UpdateAuthorityAccount(name, dept, status, userid);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on Registering New Hospital Authority" + ex.Message);
            }
        }

        public ActionResult ValidatePass(string currentPassword, int currentid)
        {
            try
            {
                 int result = dblayer.validatePass(currentPassword, currentid);
                 return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on validating password" + ex.Message);
            }
        }

        public ActionResult UpdateHosPass(int userid, string password)
        {
            try
            {
                dblayer.UpdateHosPassword(userid, password);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on Registering New Hospital Authority" + ex.Message);
            }
        }

        //Hospital Account Reset Password
        public string ValidateAdminPass(string _password)
        {
            try
            {
                string currentid = Convert.ToString(Session["userid"]);
                int result =  dblayer.validateAdminPass(_password, currentid);
                var json = new JavaScriptSerializer().Serialize(result);
                return json;
            }
            catch (Exception ex)
            {
                throw new Exception("Error on Validating Admin Password " + ex.Message);
            }
        }

        //Validate secret key - reset password
        public ActionResult ValidateSK (int currentSK, int currentid)
        {
            try
            {
                int result = dblayer.validateSK(currentSK, currentid);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on validating secret key" + ex.Message);
            }
        }

        public void PartnerCount()
        {
            hospitalCount = dblayer.CountPartner();
        }

        /*-----------------------------------------------FEEDBACKS MODULE------------------------------------------ */
        public ActionResult Feedbacks()
        {
            if (Convert.ToString(Session["userrole"]) == "Admin")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        //Feedbacks Listing
        public ActionResult GetFeedbacksListing()
        {
            try
            {
                List<Feedback> fb = dblayer.GetFeedbacks();
                return Json(fb, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on loading authority listing..." + ex.Message);
            }
        }

    }
}