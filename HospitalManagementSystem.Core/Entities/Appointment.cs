using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalManagementSystem.Core.Entities
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime ScheduledAt { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled

        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public User Patient { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }
        public User Doctor { get; set; }
    }
}
