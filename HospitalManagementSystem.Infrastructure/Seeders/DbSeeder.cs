using HospitalManagementSystem.Core.Entities;
using HospitalManagementSystem.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace HospitalManagementSystem.Infrastructure.Seeders
{
    public static class DbSeeder
    {
        public static void SeedAdminUser(HospitalDbContext context)
        {
            // Only create admin if there are no users yet
            if (!context.Users.Any())
            {
                var admin = CreateUser("myusername", "MyStrongPassword123!", "Admin");
                context.Users.Add(admin);
                context.SaveChanges();

                Console.WriteLine("Hardcoded admin created!");
            }
        }

        private static User CreateUser(string username, string password, string role)
        {
            using var hmac = new HMACSHA512();
            return new User
            {
                Username = username,
                Role = role,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                PasswordSalt = hmac.Key
            };
        }
    }
}
