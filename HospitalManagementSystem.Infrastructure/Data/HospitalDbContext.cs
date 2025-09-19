using HospitalManagementSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Infrastructure.Data
{
    public class HospitalDbContext : DbContext
    {
        public HospitalDbContext(DbContextOptions<HospitalDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Vitals> Vitals { get; set; }

    }
}
