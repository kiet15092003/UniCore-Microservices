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
using System.Text;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using UserService.Utils;

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

        public async Task<LecturerDto> CreateLecturerAsync(CreateLecturerDto createLecturerDto)
        {
            try
            {
                // Verify that the department exists
                var departmentResponse = await _departmentService.GetDepartmentByIdAsync(createLecturerDto.DepartmentId.ToString());
                if (!departmentResponse.Success || departmentResponse.Data == null)
                {
                    throw new Exception("Invalid department ID");
                }

                // Check if a user with the same PersonId already exists
                var existingUserWithPersonId = await _userManager.Users.FirstOrDefaultAsync(u => u.PersonId == createLecturerDto.PersonId);
                if (existingUserWithPersonId != null)
                {
                    throw new Exception($"A user with Person ID '{createLecturerDto.PersonId}' already exists");
                }

                // Generate email from lastname + firstname without accents
                string generatedEmail = await GenerateEmailFromName(createLecturerDto.LastName, createLecturerDto.FirstName);

                // Create user
                var user = new ApplicationUser
                {
                    UserName = generatedEmail,
                    Email = generatedEmail,
                    FirstName = createLecturerDto.FirstName,
                    LastName = createLecturerDto.LastName,
                    PhoneNumber = createLecturerDto.PhoneNumber,
                    PersonId = createLecturerDto.PersonId,
                    Status = 1,
                    EmailConfirmed = true
                };

                // Parse date of birth
                if (DateTime.TryParse(createLecturerDto.Dob, out DateTime dob))
                {
                    user.Dob = dob;
                }
                else
                {
                    throw new Exception("Invalid date of birth format");
                }

                // Create the address if provided
                if (createLecturerDto.Address != null)
                {
                    user.Address = new Address
                    {
                        Id = Guid.NewGuid(),
                        Country = createLecturerDto.Address.Country,
                        City = createLecturerDto.Address.City,
                        District = createLecturerDto.Address.District,
                        Ward = createLecturerDto.Address.Ward,
                        AddressDetail = createLecturerDto.Address.AddressDetail
                    };
                    user.AddressId = user.Address.Id;
                }

                // Create lecturer entity
                var lecturer = new Lecturer
                {
                    Id = Guid.NewGuid(),
                    LecturerCode = "155555555",
                    DepartmentId = createLecturerDto.DepartmentId,
                    JoinDate = DateTime.UtcNow,
                    WorkingStatus = 1,
                    MainMajor = "",
                    Degree = "",
                    Salary = 0
                };

                if(createLecturerDto.Degree != null)
                {
                    lecturer.Degree = createLecturerDto.Degree;
                }
                if(createLecturerDto.Salary != null)
                {
                    lecturer.Salary = createLecturerDto.Salary.Value; 
                }
                if(createLecturerDto.MainMajor != null)
                {
                    lecturer.MainMajor = createLecturerDto.MainMajor;
                }


                // Create user with password
                var result = await _userManager.CreateAsync(user, PasswordGenerator.GenerateSecurePassword());
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to create user: {errors}");
                }

                // Add lecturer role
                await _userManager.AddToRoleAsync(user, "Lecturer");

                // Link the lecturer to the created user
                lecturer.ApplicationUserId = user.Id;
                lecturer.ApplicationUser = user;

                // Save the lecturer
                var createdLecturer = await _lecturerRepo.CreateAsync(lecturer);

                // Map to DTO and return
                return _mapper.Map<LecturerDto>(createdLecturer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lecturer");
                throw;
            }
        }

        private async Task<string> GenerateEmailFromName(string lastName, string firstName)
        {
            // Remove accents and convert to lowercase
            string normalizedLastName = RemoveAccents(lastName).ToLower();
            string normalizedFirstName = RemoveAccents(firstName).ToLower();
            
            // Create base email
            string baseEmail = $"{normalizedLastName}{normalizedFirstName}@unicore.edu.vn";
            
            // Check if email already exists
            var existingUser = await _userManager.FindByEmailAsync(baseEmail);
            if (existingUser == null)
            {
                return baseEmail;
            }
            
            // If email exists, add a sequential number
            int counter = 1;
            string newEmail;
            do
            {
                newEmail = $"{normalizedLastName}{normalizedFirstName}{counter}@unicore.edu.vn";
                existingUser = await _userManager.FindByEmailAsync(newEmail);
                counter++;
            } while (existingUser != null);
            
            return newEmail;
        }

        private string RemoveAccents(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;
            
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();
            
            foreach (char c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }
            
            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
} 