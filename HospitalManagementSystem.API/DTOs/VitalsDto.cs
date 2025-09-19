using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.API.DTOs
{
    public class VitalsDto
    {
        [Required(ErrorMessage = "Blood Pressure is required")]
        public string BloodPressure { get; set; }

        [Required(ErrorMessage = "Heart Rate is required")]
        public string HeartRate { get; set; }

        [Required(ErrorMessage = "Temperature is required")]
        public string Temperature { get; set; }
    }
}
