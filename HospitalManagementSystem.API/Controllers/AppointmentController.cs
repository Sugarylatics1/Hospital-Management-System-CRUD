using HospitalManagementSystem.Core.Entities;
using HospitalManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public AppointmentController(HospitalDbContext context)
        {
            _context = context;
        }

        // Create new appointment (Receptionist)
        [HttpPost]
        [Authorize(Roles = "Receptionist,Admin")]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            var doctorExists = await _context.Users.AnyAsync(u => u.Id == appointment.DoctorId && u.Role == "Doctor");
            var patientExists = await _context.Users.AnyAsync(u => u.Id == appointment.PatientId && u.Role == "Patient");

            if (!doctorExists || !patientExists)
                return BadRequest("Invalid Doctor or Patient ID.");

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return Ok(appointment);
        }

        // View all appointments (Admin)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToListAsync();

            return Ok(appointments);
        }

        // View own appointments (Doctor)
        [HttpGet("my")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetOwn()
        {
            var username = User.Identity.Name;
            var doctor = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (doctor == null) return Unauthorized();

            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctor.Id)
                .Include(a => a.Patient)
                .ToListAsync();

            return Ok(appointments);
        }

        // Cancel appointment (Receptionist, Admin)
        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Receptionist,Admin")]
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return Ok(appointment);
        }
    }
}
