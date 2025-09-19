using HospitalManagementSystem.Core.Entities;

public class Patient
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
    public string MedicalHistory { get; set; }

    public string ContactNumber { get; set; }
    public string Address { get; set; }

    public bool IsAdmitted { get; set; }

    public Vitals Vitals { get; set; }

    public string BloodPressure { get; set; }
    public string HeartRate { get; set; }
    public string Temperature { get; set; }

    public int? AssignedDoctorId { get; set; }
    public User AssignedDoctor { get; set; }
}
