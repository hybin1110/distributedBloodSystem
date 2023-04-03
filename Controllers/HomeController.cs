using System;
using System.Web.Mvc;
using FYP_Blood.Models;
using System.Web.Script.Serialization;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;

namespace FYP_Blood.Controllers
{
    public class HomeController : Controller
    {
        Database_Access_Layer.db dblayer = new Database_Access_Layer.db();
        static string resetcode = "";

        public ActionResult Index()
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority" || Convert.ToString(Session["userrole"]) == "Admin")
            {
                Session["user"] = null;
                Session["user"] = null;
                Session["userrole"] = null;
                Session["userdept"] = null;
                Session.Abandon();
            }
            return View();
        }

        public ActionResult BloodLevel()
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority" || Convert.ToString(Session["userrole"]) == "Admin")
            {
                Session["user"] = null;
                Session["user"] = null;
                Session["userrole"] = null;
                Session["userdept"] = null;
                Session.Abandon();
            }
            return View();
        }

        public ActionResult FAQ()
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority" || Convert.ToString(Session["userrole"]) == "Admin")
            {
                Session["user"] = null;
                Session["user"] = null;
                Session["userrole"] = null;
                Session["userdept"] = null;
                Session.Abandon();
            }
            return View();
        }

        public ActionResult Contact()
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority" || Convert.ToString(Session["userrole"]) == "Admin")
            {
                Session["user"] = null;
                Session["user"] = null;
                Session["userrole"] = null;
                Session["userdept"] = null;
                Session.Abandon();
            }
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult PageNotFound()
        {
            if (Convert.ToString(Session["userrole"]) == "Hospital Authority" || Convert.ToString(Session["userrole"]) == "Admin")
            {
                Session["user"] = null;
                Session["user"] = null;
                Session["userrole"] = null;
                Session["userdept"] = null;
                Session.Abandon();
            }
            Response.StatusCode = 404;
            return View();
        }

        public ActionResult PageUnauthorised()
        {
            return View();
        }
        /*-----------------------------------------------USER MODULE------------------------------------------ */
        public ActionResult UserLogOut()
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


        public ActionResult validateuser(string _email)
        {

            int result = dblayer.validateuser(_email);

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public string userlogin(User user)
        {
            user = dblayer.userlogin(user);

            if (user.message == "Login Successful")
            {
                Session["userid"] = user.userId;
                Session["user"] = user.name;
                Session["userrole"] = user.role;
                Session["userdept"] = user.department;

                var json = new JavaScriptSerializer().Serialize(user);
                return json;
            }
            else
            {
                //return new HttpUnauthorizedResult();
                var json = "Login Fail";
                return json;
            }

        }

        public ActionResult usersignup(User user)
        {
            try
            {
                dblayer.UserSignUp(user);
                Session["useremail"] = user.email;

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on Creating New User" + ex.Message);
            }
        }

        public ActionResult ForgotPassword(string _email)
        {
            try
            {
                string resetCode = Guid.NewGuid().ToString();
                var verifyUrl = "/Home/ResetPassword/" + resetCode;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

                dblayer.OTPCode(_email, resetCode);

                var subject = "Blood+ Password Reset Request";
                var body = " You recently requested to reset your password for your account. Click the link below to reset it." +
                     " <br/><br/><a href='" + link + "'>" + link + "</a> <br/><br/>" +
                     "If you did not request a password reset, please ignore this email or reply to let us know.<br/><br/> Thank you";

                SendEmail(_email, body, subject);

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on Reset Password" + ex.Message);
            }
        }

        [HandleError]
        public ActionResult ResetPassword(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                //throw new HttpException(404, "Your error message");
                return RedirectToAction("PageNotFound", "Home");
            }

            resetcode = id;
            int OTPvalid = dblayer.verifyOTPCode(id);

            if (OTPvalid == 1)
            {
                return View();
            }
            else
            {
                //new HttpNotFoundResult("Link Expired");
                //return HttpNotFound();
                //throw new HttpException(404, "Page Not Found");
                return RedirectToAction("PageNotFound", "Home");

            }
        }

        public ActionResult UpdateResetPassword(string _password)
        {
            try
            {
                dblayer.updateResetPassword(resetcode, _password);

                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on Update Reset Password" + ex.Message);
            }
        }

        private void SendEmail(string emailAddress, string body, string subject)
        {
            using (MailMessage mm = new MailMessage("bloodplusofficial@gmail.com", emailAddress))
            {
                mm.Subject = subject;
                mm.Body = body;

                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("bloodplusofficial@gmail.com", "lfrdxcftonoqkhnn");
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;
                smtp.Send(mm);

            }
        }

        /*-----------------------------------------------FEEDBACKS------------------------------------------ */

        public ActionResult SaveFeedback(string firstname, string lastname, string email, string message)
        {
            try
            {

                dblayer.SaveFeedbacks(firstname, lastname, email, message);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on Save New Feedbacks" + ex.Message);
            }
        }

        /*-----------------------------------------------BLOOD LEVEL------------------------------------------ */
        public ActionResult GetBloodLevel()
        {
            try
            {
                List<BloodLevel> bloodLvl = dblayer.GetBloodLevelRecord();

                return Json(bloodLvl, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception("Error on loading blood level listing..." + ex.Message);
            }
        }

    }
}