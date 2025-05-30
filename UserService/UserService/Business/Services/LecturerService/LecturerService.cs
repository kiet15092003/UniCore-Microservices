using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using UserService.Business.Dtos.Lecturer;
using UserService.Entities;
using UserService.DataAccess.Repositories.LecturerRepo;
using UserService.Utils.Pagination;
using UserService.Utils.Filter;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Identity;
using UserService.CommunicationTypes.Grpc.GrpcClient;

namespace UserService.Business.Services.LecturerService
{
    public class LecturerService : ILecturerService
    {
        private readonly ILecturerRepo _lecturerRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GrpcDepartmentClientService _departmentService;
        private readonly ILogger<LecturerService> _logger;

        public LecturerService(
            ILecturerRepo lecturerRepo,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            GrpcDepartmentClientService departmentService,
            ILogger<LecturerService> logger)
        {
            _lecturerRepo = lecturerRepo;
            _mapper = mapper;
            _userManager = userManager;
            _departmentService = departmentService;
            _logger = logger;
        }


        public async Task<LecturerDto> UpdateLecturerAsync(Guid id, UpdateLecturerDto updateLecturerDto)
        {
            try
            {
                // Check if department exists
                var departmentResponse = await _departmentService.GetDepartmentByIdAsync(updateLecturerDto.DepartmentId.ToString());
                if (!departmentResponse.Success || departmentResponse.Data == null)
                {
                    throw new Exception("Invalid department ID");
                }

                var lecturer = await _lecturerRepo.GetLecturerByIdAsync(id);
                if (lecturer == null)
                {
                    return null;
                }

                // Update lecturer properties
                lecturer.Degree = updateLecturerDto.Degree;
                lecturer.Salary = updateLecturerDto.Salary;
                lecturer.DepartmentId = updateLecturerDto.DepartmentId;
                lecturer.WorkingStatus = updateLecturerDto.WorkingStatus;
                lecturer.MainMajor = updateLecturerDto.MainMajor;

                // Update user properties
                var user = lecturer.ApplicationUser;
                user.FirstName = updateLecturerDto.FirstName;
                user.LastName = updateLecturerDto.LastName;
                user.PhoneNumber = updateLecturerDto.PhoneNumber;
                user.PersonId = updateLecturerDto.PersonId;
                
                if (DateTime.TryParse(updateLecturerDto.Dob, out DateTime dob))
                {
                    user.Dob = dob;
                }

                // Update address if provided
                if (updateLecturerDto.Address != null)
                {
                    // If the user doesn't have an address yet, create one
                    if (user.Address == null)
                    {
                        user.Address = new Address
                        {
                            Id = Guid.NewGuid(),
                            Country = updateLecturerDto.Address.Country,
                            City = updateLecturerDto.Address.City,
                            District = updateLecturerDto.Address.District,
                            Ward = updateLecturerDto.Address.Ward,
                            AddressDetail = updateLecturerDto.Address.AddressDetail
                        };
                        user.AddressId = user.Address.Id;
                    }
                    else
                    {
                        // Update existing address
                        user.Address.Country = updateLecturerDto.Address.Country;
                        user.Address.City = updateLecturerDto.Address.City;
                        user.Address.District = updateLecturerDto.Address.District;
                        user.Address.Ward = updateLecturerDto.Address.Ward;
                        user.Address.AddressDetail = updateLecturerDto.Address.AddressDetail;
                    }
                }

                await _lecturerRepo.UpdateAsync(lecturer);
                return _mapper.Map<LecturerDto>(lecturer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lecturer with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteLecturerAsync(Guid id)
        {
            return await _lecturerRepo.DeleteAsync(id);
        }

        public async Task<LecturerListResponse> GetAllLecturersAsync(Pagination pagination, LecturerListFilterParams filter, Order? order)
        {
            var result = await _lecturerRepo.GetAllPaginationAsync(pagination, filter, order ?? new Order());
            
            return new LecturerListResponse
            {
                Data = result.Data,
                Total = result.Total,
                PageSize = result.PageSize,
                PageIndex = result.PageIndex
            };
        }

        public async Task<LecturerDto> GetLecturerByIdAsync(Guid id)
        {
            var lecturer = await _lecturerRepo.GetLecturerByIdAsync(id);
            return _mapper.Map<LecturerDto>(lecturer);
        }

        public async Task<LecturerDetailDto> GetLecturerDetailByIdAsync(Guid id)
        {
            var lecturer = await _lecturerRepo.GetLecturerDetailByIdAsync(id);
            if (lecturer == null)
            {
                return null;
            }

            var lecturerDetail = _mapper.Map<LecturerDetailDto>(lecturer);

            // Get department name from gRPC service
            try
            {
                var departmentResponse = await _departmentService.GetDepartmentByIdAsync(lecturer.DepartmentId.ToString());
                if (departmentResponse.Success && departmentResponse.Data != null)
                {
                    lecturerDetail.DepartmentName = departmentResponse.Data.Name;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting department details for lecturer {Id}", id);
            }

            return lecturerDetail;
        }

        public async Task<string> UpdateUserImageAsync(UpdateUserImageDto updateUserImageDto)
        {
            return await _lecturerRepo.UpdateLecturerImageAsync(updateUserImageDto.Id, updateUserImageDto.ImageUrl);
        }

        public async Task<LecturerDto> GetLecturerByEmailAsync(string email)
        {
            var lecturer = await _lecturerRepo.GetLecturerByEmailAsync(email);
            return _mapper.Map<LecturerDto>(lecturer);
        }
    }
} 