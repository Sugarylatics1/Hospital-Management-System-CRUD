public class Vitals
{
    public int Id { get; set; }
    public string BloodPressure { get; set; }
    public string HeartRate { get; set; }
    public string Temperature { get; set; }

    public int PatientId { get; set; }
    public Patient Patient { get; set; }
}
