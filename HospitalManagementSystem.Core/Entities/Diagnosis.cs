using HospitalManagementSystem.Core.Entities;

public class Diagnosis
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public Patient Patient { get; set; }

    public int DoctorId { get; set; }
    public User Doctor { get; set; }

    public string Notes { get; set; }
    public DateTime DateCreated { get; set; }
}
