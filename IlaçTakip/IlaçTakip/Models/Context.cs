using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
namespace IlaçTakip.Models
{
    public class Context:DbContext
    {
        public DbSet<UserInformation> UserInformations { get; set; }
        public DbSet<PatientInformation> PatientInformations { get; set; }
        public DbSet<PatientMedicineControl> PatientMedicineControls { get; set; }
    }
}