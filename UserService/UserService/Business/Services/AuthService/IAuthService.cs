using Microsoft.AspNetCore.Mvc;
using UserService.Business.Dtos.Auth;

namespace UserService.Business.Services.AuthService
{
    public interface IAuthService
    {
        Task<IActionResult> RegisterStudentAsync(RegisterStudentDto registerStudentDto);
        Task<IActionResult> RegisterTrainingManagerAsync(RegisterTrainingManagerDto registerTrainingManagerDto);
        Task<string> LoginAsync(LoginDto model);
        Task<List<UserReadDto>> GetAllUsersAsync();
        Task<IActionResult> RegisterStudentsFromExcelAsync(RegisterStudentByExcelDto registerStudentByExcelDto);
    }
}
