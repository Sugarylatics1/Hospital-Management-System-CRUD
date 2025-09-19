using HospitalManagementSystem.Core.DTOs;

namespace HospitalManagementSystem.Core.Interfaces
{
    public interface IAuthService
    {
        Task<string> Register(UserRegisterDto dto);
        Task<string> Login(UserLoginDto dto);
    }
}
