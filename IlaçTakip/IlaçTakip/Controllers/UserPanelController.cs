using IlaçTakip.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace IlaçTakip.Controllers
{

    public class UserPanelController : Controller
    {

        Context db = new Context();
        DateTime _now = DateTime.Now;
        TimeSpan result;
        DateTime hour;

        // GET: UserPanel

        [Authorize]
        [HttpGet]
        public ActionResult Index()
        {
            var userinfo = db.UserInformations.FirstOrDefault(x => x.UserEmail == UserController.email);
            ViewBag.UserInformation = userinfo.UserName + ' ' + userinfo.UserSurname;

            var fill = db.PatientInformations.Where(x => x.UserId == userinfo.UserId).ToList();

            for (int i = 0; i < fill.Count; i++)
            {
                hour = DateTime.Now.AddHours(-(fill[i].MedicineRepetetionTime));
                result = fill[i].MedicineStartTime - hour;
                int PatientIdHolder = fill[i].PatientId;
                var holder = db.PatientMedicineControls.Where(x => x.PatientId == PatientIdHolder).ToList();

                if (fill[i].MedicineStartTime.AddHours(fill[i].MedicineRepetetionTime) < DateTime.Now)
                {
                    if (i < holder.Count)
                    {

                        db.PatientMedicineControls.Add(holder[i]);
                        holder[i].MedicineStartTime = fill[i].MedicineStartTime.AddHours(fill[i].MedicineRepetetionTime);
                        db.SaveChanges();
                    }
                    fill[i].MedicineStartTime = fill[i].MedicineStartTime.AddHours(fill[i].MedicineRepetetionTime);
                    db.SaveChanges();
                    if ((result.TotalHours < 0 && result.TotalHours > -1) || (result.TotalHours < 1 && result.TotalHours > 0))
                    {
                        db.SaveChanges();
                        MailMessage mymailaddress = new MailMessage();
                        mymailaddress.To.Add(userinfo.UserEmail);
                        mymailaddress.From = new MailAddress("buraknas577@gmail.com");
                        mymailaddress.Subject = fill[i].PatientNameSurname + " Adlı Kişinin " + fill[i].MedicineName + " İlacını İçme Vakti Geldi";
                        mymailaddress.Body = "Sayın " + userinfo.UserName + " " + fill[i].PatientNameSurname + " Adlı Hastanın " + fill[i].MedicineName + " İlacını İçme Vakti Geldi";
                        SmtpClient smtp = new SmtpClient();
                        smtp.Credentials = new NetworkCredential("buraknas577@gmail.com", "yakinda1muhendis");
                        smtp.Port = 587;
                        smtp.Host = "smtp.gmail.com";
                        smtp.EnableSsl = true;
                        smtp.Send(mymailaddress);
                    }
                }
            }
            return View();
        }


        [HttpPost]
        [Authorize]
        public ActionResult AddMedicine(PatientInformation patientInformation, PatientMedicineControl patientMedicineControl)
        {
            var holder = db.UserInformations.FirstOrDefault(x => x.UserEmail == UserController.email);
            ViewBag.UserId = holder.UserId;
            var data = db.PatientInformations.FirstOrDefault(x => x.PatientNameSurname == patientInformation.PatientNameSurname && x.MedicineName == patientInformation.MedicineName && x.UserId == patientInformation.UserId);
            if (data == null)
            {
                db.PatientInformations.Add(patientInformation);
                db.PatientMedicineControls.Add(patientMedicineControl);
                db.SaveChanges();
                return RedirectToAction("MedicineList");
            }
            ViewBag.Error = "This patient has a record for this medicine.";


            return View();
        }
        [HttpGet]
        [Authorize]
        public ActionResult AddMedicine()
        {
            var holder = db.UserInformations.FirstOrDefault(x => x.UserEmail == UserController.email);
            ViewBag.UserId = holder.UserId;

            return View();
        }
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");

        }
        [Authorize]
        public ActionResult MedicineList()
        {

            var holder = db.UserInformations.Where(x => x.UserEmail == UserController.email).FirstOrDefault();
            var fill = db.PatientInformations.Where(x => x.UserId == holder.UserId).ToList();
            db.SaveChanges();
            return View(fill);
        }
        public ActionResult DeleteUser(int id)
        {
            var holderPatientInformations = db.PatientInformations.Find(id);
            db.PatientInformations.Remove(holderPatientInformations);
            var holderMedicineControl = db.PatientMedicineControls.Find(id);
            db.PatientMedicineControls.Remove(holderMedicineControl);
            db.SaveChanges();
            return RedirectToAction("MedicineList");
        }
        public ActionResult UpdateUser(int id)
        {
            var holder = db.PatientInformations.Find(id);
            return View("UpdateUser", holder);
        }
        public ActionResult UpdateUserCommand(PatientInformation values)
        {
            var holder = db.PatientInformations.Find(values.PatientId);
            holder.PatientNameSurname = values.PatientNameSurname;
            holder.MedicineName = values.MedicineName;
            holder.MedicineStartTime = values.MedicineStartTime;
            holder.MedicineRepetetionTime = values.MedicineRepetetionTime;
            holder.Statament = values.Statament;

            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [Authorize]
        public ActionResult MedicineControl()
        {
            var holder = db.PatientMedicineControls.ToList();
            return View(holder);
        }
        public ActionResult UpdateMedicineControl(int id)
        {
            var holder = db.PatientMedicineControls.Find(id);
            return View("UpdateMedicineControl", holder);
        }
        public ActionResult UpdateMedicineControlCommand(PatientMedicineControl values)
        {
            var holder = db.PatientMedicineControls.Find(values.RegistrationId);
            holder.PatientNameSurname = values.PatientNameSurname;
            holder.MedicineName = values.MedicineName;
            holder.MedicineStartTime = values.MedicineStartTime;
            holder.TakeTheMedicine = values.TakeTheMedicine;


            db.SaveChanges();
            return RedirectToAction("MedicineControl");
        }
    }
}