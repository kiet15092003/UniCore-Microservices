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
using UserService.CommunicationTypes.KafkaService.KafkaProducer;
using UserService.CommunicationTypes.KafkaService.KafkaProducer.Templates;
using UserService.DataAccess.Repositories.AddressRepo;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace UserService.Business.Services.LecturerService
{
    public class LecturerService : ILecturerService
    {
        private readonly ILecturerRepo _lecturerRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GrpcDepartmentClientService _departmentService;
        private readonly ILogger<LecturerService> _logger;
        private readonly IKafkaProducerService _kafkaProducer;
        private readonly IAddressRepo _addressRepo;
        private readonly Cloudinary _cloudinary;
        public LecturerService(
            ILecturerRepo lecturerRepo,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            GrpcDepartmentClientService departmentService,
            ILogger<LecturerService> logger,
            IKafkaProducerService kafkaProducer,
            IAddressRepo addressRepo,
            IConfiguration configuration)
        {
            _lecturerRepo = lecturerRepo;
            _mapper = mapper;
            _userManager = userManager;
            _departmentService = departmentService;
            _logger = logger;
            _kafkaProducer = kafkaProducer;
            _addressRepo = addressRepo;

            // Setup Cloudinary
            var cloudinaryAccount = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]);
            _cloudinary = new Cloudinary(cloudinaryAccount);
        }


        public async Task<LecturerDto> UpdateLecturerAsync(Guid id, UpdateLecturerDto updateLecturerDto)
        {
            try
            {
                // Check if department exists, only if provided
                if (updateLecturerDto.DepartmentId.HasValue)
                {
                    var departmentResponse = await _departmentService.GetDepartmentByIdAsync(updateLecturerDto.DepartmentId.ToString());
                    if (!departmentResponse.Success || departmentResponse.Data == null)
                    {
                        throw new Exception("Invalid department ID");
                    }
                }

                var lecturer = await _lecturerRepo.GetLecturerByIdAsync(id);
                if (lecturer == null)
                {
                    return null;
                }

                // Update lecturer properties only if provided
                if (!string.IsNullOrEmpty(updateLecturerDto.Degree))
                    lecturer.Degree = updateLecturerDto.Degree;
                
                if (updateLecturerDto.Salary.HasValue)
                    lecturer.Salary = updateLecturerDto.Salary.Value;
                
                if (updateLecturerDto.DepartmentId.HasValue)
                    lecturer.DepartmentId = updateLecturerDto.DepartmentId.Value;
                
                if (updateLecturerDto.WorkingStatus.HasValue)
                    lecturer.WorkingStatus = updateLecturerDto.WorkingStatus.Value;
                
                if (!string.IsNullOrEmpty(updateLecturerDto.MainMajor))
                    lecturer.MainMajor = updateLecturerDto.MainMajor;

                // Update user properties only if provided
                var user = lecturer.ApplicationUser;
                if (!string.IsNullOrEmpty(updateLecturerDto.FirstName))
                    user.FirstName = updateLecturerDto.FirstName;
                
                if (!string.IsNullOrEmpty(updateLecturerDto.LastName))
                    user.LastName = updateLecturerDto.LastName;
                
                if (!string.IsNullOrEmpty(updateLecturerDto.PhoneNumber))
                    user.PhoneNumber = updateLecturerDto.PhoneNumber;
                
                if (!string.IsNullOrEmpty(updateLecturerDto.PersonId))
                    user.PersonId = updateLecturerDto.PersonId;
                
                if (!string.IsNullOrEmpty(updateLecturerDto.Dob) && DateTime.TryParse(updateLecturerDto.Dob, out DateTime dob))
                {
                    user.Dob = dob;
                }

                // Map address if provided (repo sẽ xử lý logic xóa/tạo Address)
                if (updateLecturerDto.Address != null)
                {
                    user.Address = _mapper.Map<Address>(updateLecturerDto.Address);
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
            var imageUrl = await UploadImageToCloudinary(updateUserImageDto.ImageFile);
            return await _lecturerRepo.UpdateLecturerImageAsync(updateUserImageDto.Id, imageUrl);
        }

        private async Task<string> UploadImageToCloudinary(IFormFile imageFile)
        {
            using var stream = imageFile.OpenReadStream();
            
            // Create upload parameters
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imageFile.FileName, stream),
                Folder = "lecturer_profile_images",
                Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
            };
            
            // Upload to Cloudinary
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            
            if (uploadResult.Error != null)
            {
                throw new Exception($"Failed to upload image: {uploadResult.Error.Message}");
            }
            
            // Return the secure URL
            return uploadResult.SecureUrl.ToString();
        }

        public async Task<LecturerDto> GetLecturerByEmailAsync(string email)
        {
            var lecturer = await _lecturerRepo.GetLecturerByEmailAsync(email);
            return _mapper.Map<LecturerDto>(lecturer);
        }

        public async Task<IActionResult> CreateLecturerAsync(CreateLecturerDto createLecturerDto)
        {
            try
            {
                // Verify that the department exists
                var departmentResponse = await _departmentService.GetDepartmentByIdAsync(createLecturerDto.DepartmentId.ToString());
                if (!departmentResponse.Success || departmentResponse.Data == null)
                {
                    return new BadRequestObjectResult("Invalid department ID");
                }

                // Check if a user with the same PersonId already exists
                var existingUserWithPersonId = await _userManager.Users.FirstOrDefaultAsync(u => u.PersonId == createLecturerDto.PersonId);
                if (existingUserWithPersonId != null)
                {
                    return new BadRequestObjectResult($"A user with Person ID '{createLecturerDto.PersonId}' already exists");
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
                    return new BadRequestObjectResult("Invalid date of birth format");
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
                    LecturerCode = generatedEmail.Split("@")[0],
                    DepartmentId = createLecturerDto.DepartmentId,
                    JoinDate = DateTime.UtcNow
                };
                lecturer.Degree = "";
                lecturer.Salary = 0;
                lecturer.MainMajor = "";

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
                var password = PasswordGenerator.GenerateSecurePassword();
                _logger.LogInformation("269---------------Password: {Password}", password);
                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new BadRequestObjectResult($"Failed to create user: {errors}");
                }

                // Add lecturer role
                await _userManager.AddToRoleAsync(user, "Lecturer");

                // Link the lecturer to the created user
                lecturer.ApplicationUserId = user.Id;
                lecturer.ApplicationUser = user;

                // Save the lecturer
                var createdLecturer = await _lecturerRepo.CreateAsync(lecturer);

                // Publish Kafka event for user import
                var userImportedEvent = new UserImportedEventDTO
                {
                    Data = new UserImportedEventData
                    {
                        Users = new List<UserImportedEventDataSingleData>
                        {
                            new UserImportedEventDataSingleData
                            {
                                UserEmail = user.Email ?? string.Empty,
                                Password = password,
                                PrivateEmail = createLecturerDto.PersonEmail ?? string.Empty
                            }
                        }
                    }
                };
                
                await _kafkaProducer.PublishMessageAsync("UserImportedEvent", userImportedEvent);

                // Map to DTO and return
                return new OkObjectResult(_mapper.Map<LecturerDto>(createdLecturer));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lecturer");
                return new BadRequestObjectResult(ex.Message);
            }
        }

        private async Task<string> GenerateEmailFromName(string lastName, string firstName)
        {
            // Remove accents and convert to lowercase
            string normalizedLastName = RemoveAccents(lastName).ToLower().Trim().Replace(" ", "");
            string normalizedFirstName = RemoveAccents(firstName).ToLower().Trim().Replace(" ", "");
            
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

        public async Task<List<LecturerDto>> GetLecturersByMajorsDepartmentAsync(List<string> majorIds)
        {
            var departmentIds = new HashSet<Guid>();
            foreach (var majorId in majorIds)
            {
                var majorResponse = await _departmentService.GetDepartmentByMajorIdAsync(majorId);
                if (majorResponse.Success && majorResponse.Data != null)
                {
                    if (Guid.TryParse(majorResponse.Data.Id, out var depId))
                        departmentIds.Add(depId);
                }
            }

            var lecturers = await _lecturerRepo.GetLecturersByDepartmentIdsAsync(departmentIds.ToList());
            return _mapper.Map<List<LecturerDto>>(lecturers);
        }
    }
} 