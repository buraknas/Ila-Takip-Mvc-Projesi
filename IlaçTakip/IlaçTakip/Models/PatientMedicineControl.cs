using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IlaçTakip.Models
{
    public class PatientMedicineControl
    {
        [Key]
        public int RegistrationId { get; set; }
        public PatientInformation PatientInformation { get; set; }
        public int PatientId { get; set; }
        public string PatientNameSurname { get; set; }
        public string MedicineName { get; set; }
        public DateTime MedicineStartTime { get; set; }
        public string TakeTheMedicine { get; set; }
    }
}