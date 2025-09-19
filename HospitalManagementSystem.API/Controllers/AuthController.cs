using HospitalManagementSystem.Core.Entities;
using HospitalManagementSystem.Core.DTOs;
using HospitalManagementSystem.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace HospitalManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(HospitalDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ------------------ LOGIN ------------------
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == dto.Username);
            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
                return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        // ------------------ SELF-REGISTER ------------------
        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegisterDto dto)
        {
            if (_context.Users.Any(u => u.Username == dto.Username))
                return BadRequest("Username already exists");

            // Force normal users to have role "User"
            dto.Role = "User";

            CreatePasswordHash(dto.Password, out byte[] hash, out byte[] salt);

            var newUser = new User
            {
                Username = dto.Username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = dto.Role
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok("User registered successfully");
        }

        // ------------------ ADMIN: CREATE STAFF USERS ------------------
        [HttpPost("create-user")]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser([FromBody] UserRegisterDto dto)
        {
            if (_context.Users.Any(u => u.Username == dto.Username))
                return BadRequest("Username already exists");

            var allowedRoles = new List<string> { "Doctor", "Nurse", "Receptionist" };
            if (!allowedRoles.Contains(dto.Role))
                return BadRequest("Invalid role. Allowed roles: Doctor, Nurse, Receptionist");

            CreatePasswordHash(dto.Password, out byte[] hash, out byte[] salt);

            var newUser = new User
            {
                Username = dto.Username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Role = dto.Role
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok($"User {dto.Username} created as {dto.Role}");
        }

        // ------------------ ADMIN: PROMOTE USER TO ADMIN ------------------
        [HttpPost("make-admin/{userId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult MakeAdmin(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null) return NotFound("User not found");

            if (user.Role == "Admin")
                return BadRequest("User is already an Admin");

            user.Role = "Admin";
            _context.SaveChanges();

            return Ok($"{user.Username} is now an Admin");
        }

        // ------------------ HELPER METHODS ------------------
        private bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
