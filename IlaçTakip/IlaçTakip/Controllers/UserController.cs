using IlaçTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;

namespace IlaçTakip.Controllers
{
    public class UserController : Controller
    {
        Context db = new Context();
        public static string email;

        // GET: Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserInformation userInformation)
        {
            email = userInformation.UserEmail;


            var data = db.UserInformations.FirstOrDefault(x => x.UserEmail == userInformation.UserEmail && x.Password == userInformation.Password);
            if (data != null && data.UserStatus == true)
            {
                FormsAuthentication.SetAuthCookie(data.UserEmail.ToString(), false);
                Session["UserMail"] = data.UserEmail;

                return RedirectToAction("Index", "UserPanel");

            }
            else
            {
                ViewBag.Error = "Email or Password is incorrect.You may not have activated your account.";
                return View();
            }
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgetPassword(UserInformation userInformation)
        {
            var data = db.UserInformations.Where(i => i.UserEmail == userInformation.UserEmail).FirstOrDefault();
            if (data != null)
            {
                Guid _guid = Guid.NewGuid();
                data.Password = _guid.ToString().Substring(0, 8);
                db.SaveChanges();
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                client.EnableSsl = true;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("buraknas577@gmail.com", "New Password");
                mail.To.Add(userInformation.UserEmail);
                mail.IsBodyHtml = true;
                mail.Subject = "New Password";
                mail.Body += "The password of the " + data.UserEmail + "account has been reset.</br>" + "New password=" + data.Password;
                NetworkCredential net = new NetworkCredential("buraknas577@gmail.com", "yakinda1muhendis");
                client.Credentials = net;
                client.Send(mail);
                return RedirectToAction("Login");
            }
            ViewBag.Error = "Email address not found.";

            return View();
        }
        [HttpGet]
        public ActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(UserInformation userInformation)
        {
            var data = db.UserInformations.FirstOrDefault(x => x.UserEmail == userInformation.UserEmail);
            if (data == null)
            {
                userInformation.UserStatus = false;
                db.UserInformations.Add(userInformation);
                db.SaveChanges();
                BuildEmailTemplate(userInformation.UserId);
                return RedirectToAction("Login");
            }
            ViewBag.Error = "This e-mail address has already been registered.";

            return View();

        }
        public ActionResult RegisterConfirm(int userId)
        {
            DateTime now = DateTime.Now;
            TimeSpan result;

            UserInformation data = db.UserInformations.Where(x => x.UserId == userId).FirstOrDefault();
            result = now - data.UserRegisterDate;
            if (result.TotalSeconds <= 86400)
            {
                data.UserStatus = true;
                db.SaveChanges();
                return RedirectToAction("Login");

            }
            else
            {
                var deleteregister = db.UserInformations.Find(userId);

                db.UserInformations.Remove(deleteregister);
                db.SaveChanges();
                return PartialView("Error");
            }



        }
        public ActionResult Confirm(int userId)
        {
            ViewBag.userId = userId;
            return View();
        }

        public void BuildEmailTemplate(int userId)
        {
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/" + "Text" + ".cshtml"));
            var UserInfo = db.UserInformations.Where(x => x.UserId == userId).FirstOrDefault();
            var url = "https://localhost:44361/" + "User/Confirm?userId=" + userId;
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            body = body.ToString();
            BuildEmailTemplate("Your Account Is Succesfully Created", body, UserInfo.UserEmail);
        }
        public static void BuildEmailTemplate(string subjectText, string bodyText, string sendTo)
        {
            string from, to, bcc, cc, subject, body;
            from = "buraknas577@gmail.com";
            to = sendTo.Trim();
            bcc = "";
            cc = "";
            subject = subjectText;
            StringBuilder sb = new StringBuilder();
            sb.Append(bodyText);
            body = sb.ToString();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from);
            mail.To.Add(new MailAddress(to));
            if (!string.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(new MailAddress(bcc));
            }
            if (!string.IsNullOrEmpty(cc))
            {
                mail.Bcc.Add(new MailAddress(cc));
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendEmail(mail);
        }
        public static void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("buraknas577@gmail.com", "yakinda1muhendis");
            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}