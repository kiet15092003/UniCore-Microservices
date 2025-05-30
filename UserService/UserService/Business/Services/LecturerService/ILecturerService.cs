using Microsoft.AspNetCore.Mvc;
using UserService.Business.Dtos.Lecturer;
using UserService.Utils.Pagination;
using UserService.Utils.Filter;
using UserService.Entities;

namespace UserService.Business.Services.LecturerService
{
    public interface ILecturerService
    {
        Task<LecturerDto> UpdateLecturerAsync(Guid id, UpdateLecturerDto updateLecturerDto);
        Task<bool> DeleteLecturerAsync(Guid id);  
        Task<LecturerListResponse> GetAllLecturersAsync(Pagination pagination, LecturerListFilterParams filter, Order? order);
        Task<LecturerDto> GetLecturerByIdAsync(Guid id);
        Task<LecturerDetailDto> GetLecturerDetailByIdAsync(Guid id);
        Task<string> UpdateUserImageAsync(UpdateUserImageDto updateUserImageDto);
        Task<LecturerDto> GetLecturerByEmailAsync(string email);
    }
} 