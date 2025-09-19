using HospitalManagementSystem.Core.Entities;
using HospitalManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HospitalManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Receptionist")]
    public class ReceptionistController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public ReceptionistController(HospitalDbContext context)
        {
            _context = context;
        }

        // POST: api/receptionist/patients
        [HttpPost("patients")]
        public IActionResult RegisterPatient([FromBody] RegisterPatientDto dto)
        {
            var patient = new Patient
            {
                FullName = dto.FullName,
                Age = dto.Age,
                Gender = dto.Gender,
                ContactNumber = dto.ContactNumber,
                Address = dto.Address,
                MedicalHistory = dto.MedicalHistory,
                IsAdmitted = false
            };

            _context.Patients.Add(patient);
            _context.SaveChanges();

            return Ok("Patient registered successfully.");
        }

        // GET: api/receptionist/patients
        [HttpGet("patients")]
        public IActionResult GetAllPatients()
        {
            var patients = _context.Patients
                .Select(p => new {
                    p.Id,
                    p.FullName,
                    p.Age,
                    p.Gender,
                    p.ContactNumber,
                    p.Address,
                    p.IsAdmitted
                }).ToList();

            return Ok(patients);
        }

        // PUT: api/receptionist/patients/{id}/contact
        [HttpPut("patients/{id}/contact")]
        public IActionResult UpdateContact(int id, [FromBody] UpdateContactDto dto)
        {
            var patient = _context.Patients.Find(id);
            if (patient == null)
                return NotFound("Patient not found.");

            patient.ContactNumber = dto.ContactNumber;
            patient.Address = dto.Address;
            _context.SaveChanges();

            return Ok("Patient contact updated.");
        }

        // GET: api/receptionist/profile
        [HttpGet("profile")]
        public IActionResult GetReceptionistProfile()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var receptionist = _context.Users
                .Where(u => u.Username == username)
                .Select(u => new {
                    u.Id,
                    u.Username,
                    u.Role
                }).FirstOrDefault();

            return Ok(receptionist);
        }
    }

    public class RegisterPatientDto
    {
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public string MedicalHistory { get; set; }
    }

    public class UpdateContactDto
    {
        public string ContactNumber { get; set; }
        public string Address { get; set; }
    }
}
