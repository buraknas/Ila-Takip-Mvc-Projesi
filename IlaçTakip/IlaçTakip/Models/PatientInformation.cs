using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IlaçTakip.Models
{
    public class PatientInformation
    {
        [Key]
        public int PatientId { get; set; }
        public UserInformation UserInformation { get; set; }
        public int UserId { get; set; }
        public string PatientNameSurname { get; set; }
        public string MedicineName { get; set; }

        public int MedicineRepetetionTime { get; set; }
        public string Statament { get; set; }
        public DateTime MedicineStartTime { get; set; }
        public ICollection<PatientMedicineControl> PatientMedicineControls { get; set; }
    }
}