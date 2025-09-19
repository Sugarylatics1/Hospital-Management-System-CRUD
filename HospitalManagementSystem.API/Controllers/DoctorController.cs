using HospitalManagementSystem.Core.Entities;
using HospitalManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HospitalManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Doctor")]
    public class DoctorController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public DoctorController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET: api/doctor/patients
        [HttpGet("patients")]
        public IActionResult GetAssignedPatients()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var doctor = _context.Users.FirstOrDefault(u => u.Username == username);

            if (doctor == null)
                return Unauthorized();

            var patients = _context.Patients
                .Where(p => p.AssignedDoctorId == doctor.Id)
                .Select(p => new {
                    p.Id,
                    p.FullName,
                    p.Age,
                    p.Gender,
                    p.MedicalHistory
                }).ToList();

            return Ok(patients);
        }

        // POST: api/doctor/patients/{id}/diagnosis
        [HttpPost("patients/{id}/diagnosis")]
        public IActionResult AddDiagnosis(int id, [FromBody] DiagnosisDto dto)
        {
            var patient = _context.Patients.Find(id);
            if (patient == null)
                return NotFound("Patient not found.");

            var username = User.FindFirstValue(ClaimTypes.Name);
            var doctor = _context.Users.FirstOrDefault(u => u.Username == username);

            if (doctor == null)
                return Unauthorized();

            var diagnosis = new Diagnosis
            {
                PatientId = patient.Id,
                DoctorId = doctor.Id,
                Notes = dto.Notes,
                DateCreated = DateTime.UtcNow
            };

            _context.Diagnoses.Add(diagnosis);
            _context.SaveChanges();

            return Ok("Diagnosis added successfully.");
        }

        // GET: api/doctor/profile
        [HttpGet("profile")]
        public IActionResult GetDoctorProfile()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var doctor = _context.Users
                .Where(u => u.Username == username)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Role
                })
                .FirstOrDefault();

            return Ok(doctor);
        }
    }

    public class DiagnosisDto
    {
        public string Notes { get; set; }
    }
}
