using Microsoft.AspNetCore.Mvc;
using UserService.Business.Dtos.Auth;

namespace UserService.Business.Services.AuthService
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginDto model);
    }
}
