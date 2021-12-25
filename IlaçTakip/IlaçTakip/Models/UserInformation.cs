using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IlaçTakip.Models
{
    public class UserInformation
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserTelNumber { get; set; }
        public string UserEmail { get; set; }
        public string Password { get; set; }
        public DateTime UserRegisterDate { get; set; }
        public bool UserStatus { get; set; }
        public UserInformation()
        {
            this.UserRegisterDate = DateTime.Now;

        }

        public ICollection<PatientInformation> PatientInformations { get; set; }


    }
}