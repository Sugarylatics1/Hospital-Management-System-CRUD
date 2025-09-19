using HospitalManagementSystem.Core.Entities;
using HospitalManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using HospitalManagementSystem.API.DTOs;
using System.ComponentModel.DataAnnotations;


namespace HospitalManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Nurse")]
    public class NurseController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public NurseController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET: api/nurse/patients
        [HttpGet("patients")]
        public IActionResult GetAdmittedPatients()
        {
            var patients = _context.Patients
                .Where(p => p.IsAdmitted)
                .Select(p => new {
                    p.Id,
                    p.FullName,
                    p.Age,
                    p.Gender,
                    Vitals = p.Vitals != null ? new
                    {
                        p.Vitals.BloodPressure,
                        p.Vitals.HeartRate,
                        p.Vitals.Temperature
                    } : null
                })
                .ToList();

            return Ok(patients);
        }

        // PUT: api/nurse/patients/{id}/vitals
        [HttpPut("patients/{id}/vitals")]
        public IActionResult UpdatePatientVitals(int id, [FromBody] VitalsDto dto)
        {
            var patient = _context.Patients
                .FirstOrDefault(p => p.Id == id);

            if (patient == null)
                return NotFound("Patient not found.");

            if (patient.Vitals != null)
            {
                patient.Vitals.BloodPressure = dto.BloodPressure;
                patient.Vitals.HeartRate = dto.HeartRate;
                patient.Vitals.Temperature = dto.Temperature;
            }
            else
            {
                var newVitals = new Vitals
                {
                    BloodPressure = dto.BloodPressure,
                    HeartRate = dto.HeartRate,
                    Temperature = dto.Temperature,
                    PatientId = patient.Id // If there's a foreign key
                };

                patient.Vitals = newVitals;
                _context.Vitals.Add(newVitals);
            }

            _context.SaveChanges();

            return Ok("Vitals updated successfully.");
        }

        // GET: api/nurse/profile
        [HttpGet("profile")]
        public IActionResult GetNurseProfile()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var nurse = _context.Users
                .Where(u => u.Username == username)
                .Select(u => new {
                    u.Id,
                    u.Username,
                    u.Role
                })
                .FirstOrDefault();

            return Ok(nurse);
        }
    }

    public class VitalsDto
    {
        public string BloodPressure { get; set; }
        public string HeartRate { get; set; }
        public string Temperature { get; set; }
    }
}
