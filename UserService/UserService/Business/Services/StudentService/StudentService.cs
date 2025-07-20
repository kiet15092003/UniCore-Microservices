using Microsoft.AspNetCore.Mvc;
using UserService.Entities;
using ClosedXML.Excel;
using UserService.Business.Dtos.Student;
using UserService.CommunicationTypes.Grpc.GrpcClient;
using UserService.CommunicationTypes.KafkaService.KafkaProducer;
using UserService.CommunicationTypes.KafkaService.KafkaProducer.Templates;
using UserService.DataAccess.Repositories.StudentRepo;
using UserService.DataAccess.Repositories.BatchRepo;
using UserService.Utils.Pagination;
using UserService.Utils.Filter;
using AutoMapper;
using System.Text.Json;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using MajorService;
namespace UserService.Business.Services.StudentService
{    
    public class StudentService : IStudentService
    {
        private readonly IStudentRepo _studentRepository;
        private readonly ILogger<StudentService> _logger;
        private readonly IMapper _mapper;
        private readonly GrpcMajorClientService _grpcMajorClient;
        private readonly Cloudinary _cloudinary;        
        private readonly IKafkaProducerService _kafkaProducer;
        private readonly IBatchRepo _batchRepository;

        public StudentService(
            IStudentRepo studentRepository,
            ILogger<StudentService> logger,
            IMapper mapper,
            IConfiguration configuration,
            GrpcMajorClientService grpcMajorClient,
            IKafkaProducerService kafkaProducer,
            IBatchRepo batchRepository)
        {
            _studentRepository = studentRepository;
            _logger = logger;
            _mapper = mapper;
            _grpcMajorClient = grpcMajorClient;

            // Setup Cloudinary
            var cloudinaryAccount = new Account(
                configuration["Cloudinary:CloudName"],
                configuration["Cloudinary:ApiKey"],
                configuration["Cloudinary:ApiSecret"]);
            _cloudinary = new Cloudinary(cloudinaryAccount);
            _kafkaProducer = kafkaProducer;
            _batchRepository = batchRepository;
        }

        public async Task<StudentListResponse> GetAllStudentsAsync(Pagination pagination, StudentListFilterParams filter, Order? order)
        {
            try
            {
                var students = await _studentRepository.GetAllPaginationAsync(pagination, filter, order);
                return new StudentListResponse
                {
                    Data = students.Data,
                    Total = students.Total,
                    PageIndex = students.PageIndex,
                    PageSize = students.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all students");
                throw;
            }
        }

        public async Task<StudentDto> GetStudentByIdAsync(Guid id)
        {
            try
            {
                var student = await _studentRepository.GetStudentByIdAsync(id);
                if (student == null)
                {
                    return null;
                }
                return _mapper.Map<StudentDto>(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student with id {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteStudentAsync(Guid id)
        {
            try
            {
                var result = await _studentRepository.DeleteAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student with id {Id}", id);
                throw;
            }
        }

        public async Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentDto updateStudentDto)
        {
            try
            {
                // Check if updateStudentDto is null
                if (updateStudentDto == null)
                {
                    throw new ArgumentNullException(nameof(updateStudentDto), "Update data cannot be null");
                }
                
                // Get existing student from database
                var existingStudent = await _studentRepository.GetStudentByIdAsync(id);
                if (existingStudent == null)
                {
                    return null;
                }

                // Create a Student object with updated properties
                var studentToUpdate = new Entities.Student
                {
                    Id = existingStudent.Id,
                    StudentCode = existingStudent.StudentCode,
                    AccumulateCredits = updateStudentDto.AccumulateCredits ?? existingStudent.AccumulateCredits,
                    AccumulateScore = updateStudentDto.AccumulateScore ?? existingStudent.AccumulateScore,
                    AccumulateActivityScore = updateStudentDto.AccumulateActivityScore ?? existingStudent.AccumulateActivityScore,
                    MajorId = updateStudentDto.MajorId ?? existingStudent.MajorId,
                    BatchId = updateStudentDto.BatchId ?? existingStudent.BatchId,
                    ApplicationUser = new ApplicationUser
                    {
                        Id = existingStudent.ApplicationUserId,
                        FirstName = updateStudentDto.FirstName ?? existingStudent.ApplicationUser.FirstName,
                        LastName = updateStudentDto.LastName ?? existingStudent.ApplicationUser.LastName,
                        PhoneNumber = updateStudentDto.PhoneNumber ?? existingStudent.ApplicationUser.PhoneNumber,
                        Status = updateStudentDto.Status ?? existingStudent.ApplicationUser.Status,
                        ImageUrl = existingStudent.ApplicationUser.ImageUrl
                    }
                };

                // Map guardians if provided
                if (updateStudentDto.Guardians != null && updateStudentDto.Guardians.Any())
                {
                    studentToUpdate.Guardians = _mapper.Map<List<Guardian>>(updateStudentDto.Guardians);
                }

                // Map address if provided
                if (updateStudentDto.Address != null)
                {
                    // Handle empty string or empty Guid for AddressId
                    if (updateStudentDto.Address.Id.HasValue && 
                        (updateStudentDto.Address.Id.Value == Guid.Empty || 
                         updateStudentDto.Address.Id.Value.ToString() == ""))
                    {
                        updateStudentDto.Address.Id = null;
                    }
                    
                    studentToUpdate.ApplicationUser.Address = _mapper.Map<Address>(updateStudentDto.Address);
                }

                

                // Update the student in the database
                var updatedStudent = await _studentRepository.UpdateAsync(studentToUpdate);
                return _mapper.Map<StudentDto>(updatedStudent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student with id {Id}", id);
                throw;
            }
        }

        public async Task<string> UpdateUserImageAsync(UpdateUserImageDto updateUserImageDto)
        {
            var imageUrl = await UploadImageToCloudinary(updateUserImageDto.ImageFile);
            return await _studentRepository.UpdateStudentImageAsync(updateUserImageDto.StudentId, imageUrl);
        }

        private async Task<string> UploadImageToCloudinary(IFormFile imageFile)
        {
            using var stream = imageFile.OpenReadStream();
            
            // Create upload parameters
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(imageFile.FileName, stream),
                Folder = "student_profile_images",
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

        public async Task<IActionResult> CreateStudentByExcelAsync(CreateStudentByExcelDto createStudentByExcelDto)
        {
            MajorResponse major = await _grpcMajorClient.GetMajorByIdAsync(createStudentByExcelDto.MajorId.ToString());

            if (!major.Success)
            {
                throw new KeyNotFoundException("Major not found");
            }

            try
            {                
                var file = createStudentByExcelDto.ExcelFile;
                var batchId = createStudentByExcelDto.BatchId;
                var majorId = createStudentByExcelDto.MajorId;

                // Get batch to extract StartYear
                var batch = await _batchRepository.GetByIdAsync(batchId);
                if (batch == null)
                {
                    return new BadRequestObjectResult("Batch not found");
                }

                // Get major code from the response
                string majorCode = major.Data.Code;
                if (string.IsNullOrEmpty(majorCode) || majorCode.Length < 3)
                {
                    return new BadRequestObjectResult("Invalid major code");
                }

                // Extract the first 3 digits of the major code
                string majorPrefix = majorCode.Substring(0, 3);

                // Extract the last 2 digits of the batch year
                string batchSuffix = (batch.StartYear % 100).ToString("00");

                if (file == null || file.Length == 0)
                {
                    return new BadRequestObjectResult("File is empty");
                }

                if (!file.FileName.EndsWith(".xlsx"))
                {
                    return new BadRequestObjectResult("Only Excel files (.xlsx) are allowed");
                }                var results = new List<string>();
                var userStudentPairs = new List<(ApplicationUser User, Entities.Student Student)>();
                var userPrivateEmails = new Dictionary<string, string>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rowCount = worksheet.LastRowUsed().RowNumber();                        // Validate headers
                        var headers = new Dictionary<string, int>
                        {
                            { "FirstName", 0 },
                            { "LastName", 0 },
                            { "Dob", 0 },
                            { "PersonId", 0 },
                            { "PhoneNumber", 0 },
                            { "PrivateEmail", 0 }
                            // StudentCode is removed as it will be generated
                        };

                        // Get header positions
                        var headerRow = worksheet.Row(1);
                        for (int col = 1; col <= worksheet.LastColumnUsed().ColumnNumber(); col++)
                        {
                            var header = headerRow.Cell(col).Value.ToString();
                            if (headers.ContainsKey(header))
                            {
                                headers[header] = col;
                            }
                        }

                        // Validate all headers are present
                        if (headers.Any(h => h.Value == 0))
                        {
                            return new BadRequestObjectResult($"Missing required columns: {string.Join(", ", headers.Where(h => h.Value == 0).Select(h => h.Key))}");
                        }

                        // Process each row
                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                // Generate student code based on formula:
                                // 3 first digits of major code + 2 last digits of batch year + 4 digits for row order
                                string orderNumber = (row - 1).ToString("0000"); // Convert row index to 4-digit format (0001, 0002, etc.)
                                string studentCode = $"{majorPrefix}{batchSuffix}{orderNumber}";                                var firstName = worksheet.Cell(row, headers["FirstName"]).Value.ToString();
                                var lastName = worksheet.Cell(row, headers["LastName"]).Value.ToString();
                                var dob = DateTime.Parse(worksheet.Cell(row, headers["Dob"]).Value.ToString());                                var personId = worksheet.Cell(row, headers["PersonId"]).Value.ToString();
                                var phoneNumber = worksheet.Cell(row, headers["PhoneNumber"]).Value.ToString();
                                var privateEmail = worksheet.Cell(row, headers["PrivateEmail"]).Value.ToString();                                var email = $"{studentCode.ToLower()}@unicore.edu.vn";                                // Create ApplicationUser
                                var user = new ApplicationUser
                                {
                                    UserName = email,
                                    Email = email,
                                    FirstName = firstName,
                                    LastName = lastName,
                                    Dob = dob,
                                    PersonId = personId,
                                    PhoneNumber = phoneNumber,
                                    Status = 1
                                };

                                // Create Student (ApplicationUserId sẽ được gán sau khi tạo user)
                                var student = new Entities.Student
                                {
                                    StudentCode = studentCode,
                                    AccumulateCredits = 0,
                                    AccumulateScore = 0,
                                    AccumulateActivityScore = 0,
                                    MajorId = majorId,
                                    BatchId = batchId
                                };                                userStudentPairs.Add((user, student));
                                userPrivateEmails[email] = privateEmail;

                                results.Add($"Prepared student {studentCode} with email: {email}");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error processing row {Row}: {Message}", row, ex.Message);
                                results.Add($"Error processing row {row}: {ex.Message}");
                            }
                        }
                    }
                }                if (!userStudentPairs.Any())
                {
                    return new BadRequestObjectResult("No valid students found in the file");
                }

                // Generate and store passwords for each user
                var userPasswords = new Dictionary<string, string>();
                foreach (var (user, _) in userStudentPairs)
                {
                    // Store the passwords based on user email for later use
                    userPasswords[user.Email] = Utils.PasswordGenerator.GenerateSecurePassword();
                }

                try
                {
                    var (createdUsers, createdStudents) = await _studentRepository.AddStudentsWithUsersAsync(userStudentPairs, userPasswords);
                    _logger.LogInformation("Successfully created {Count} students", createdStudents.Count);
                      // Publish Kafka event for user import
                    var userImportedEvent = new UserImportedEventDTO
                    {
                        Data = new UserImportedEventData
                        {                            Users = createdUsers.Select(user => new UserImportedEventDataSingleData
                            {
                                UserEmail = user.Email ?? string.Empty,
                                Password = user.Email != null && userPasswords.ContainsKey(user.Email)
                                    ? userPasswords[user.Email]
                                    : string.Empty,
                                PrivateEmail = user.Email != null && userPrivateEmails.ContainsKey(user.Email)
                                    ? userPrivateEmails[user.Email]
                                    : string.Empty
                            }).ToList()
                        }
                    };
                    
                    await _kafkaProducer.PublishMessageAsync("UserImportedEvent", userImportedEvent);
                    
                    return new OkObjectResult(new { Results = results });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating students");
                    return new BadRequestObjectResult($"Error creating students: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CreateStudentByExcelAsync");
                return new BadRequestObjectResult($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<IActionResult> CreateStudentAsync(CreateStudentDto createStudentDto)
        {
            MajorResponse major = await _grpcMajorClient.GetMajorByIdAsync(createStudentDto.MajorId.ToString());

            if (!major.Success)
            {
                throw new KeyNotFoundException("Major not found");
            }

            try
            {
                var batchId = createStudentDto.BatchId;
                var majorId = createStudentDto.MajorId;

                // Get batch to extract StartYear
                var batch = await _batchRepository.GetByIdAsync(batchId);
                if (batch == null)
                {
                    return new BadRequestObjectResult("Batch not found");
                }

                // Get major code from the response
                string majorCode = major.Data.Code;
                if (string.IsNullOrEmpty(majorCode) || majorCode.Length < 3)
                {
                    return new BadRequestObjectResult("Invalid major code");
                }

                // Extract the first 3 digits of the major code
                string majorPrefix = majorCode.Substring(0, 3);

                // Extract the last 2 digits of the batch year
                string batchSuffix = (batch.StartYear % 100).ToString("00");

                // Generate student code - we'll use a timestamp-based approach for single student
                string timestamp = DateTime.Now.ToString("HHmmss");
                string studentCode = $"{majorPrefix}{batchSuffix}{timestamp}";

                var email = $"{studentCode.ToLower()}@unicore.edu.vn";

                // Create ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = createStudentDto.FirstName,
                    LastName = createStudentDto.LastName,
                    Dob = createStudentDto.Dob,
                    PersonId = createStudentDto.PersonId,
                    PhoneNumber = createStudentDto.PhoneNumber,
                    Status = 1
                };

                // Create Student
                var student = new Entities.Student
                {
                    StudentCode = studentCode,
                    AccumulateCredits = 0,
                    AccumulateScore = 0,
                    AccumulateActivityScore = 0,
                    MajorId = majorId,
                    BatchId = batchId
                };

                var userStudentPair = new List<(ApplicationUser User, Entities.Student Student)>
                {
                    (user, student)
                };

                var userPrivateEmails = new Dictionary<string, string>
                {
                    [email] = createStudentDto.PrivateEmail
                };

                // Generate password for the user
                var userPasswords = new Dictionary<string, string>
                {
                    [email] = Utils.PasswordGenerator.GenerateSecurePassword()
                };

                try
                {
                    var (createdUsers, createdStudents) = await _studentRepository.AddStudentsWithUsersAsync(userStudentPair, userPasswords);
                    _logger.LogInformation("Successfully created student {StudentCode}", studentCode);

                    // Publish Kafka event for user import
                    var userImportedEvent = new UserImportedEventDTO
                    {
                        Data = new UserImportedEventData
                        {
                            Users = createdUsers.Select(user => new UserImportedEventDataSingleData
                            {
                                UserEmail = user.Email ?? string.Empty,
                                Password = user.Email != null && userPasswords.ContainsKey(user.Email)
                                    ? userPasswords[user.Email]
                                    : string.Empty,
                                PrivateEmail = user.Email != null && userPrivateEmails.ContainsKey(user.Email)
                                    ? userPrivateEmails[user.Email]
                                    : string.Empty
                            }).ToList()
                        }
                    };

                    await _kafkaProducer.PublishMessageAsync("UserImportedEvent", userImportedEvent);

                    return new OkObjectResult(new { 
                        StudentCode = studentCode,
                        Email = email,
                        Message = "Student created successfully"
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating student");
                    return new BadRequestObjectResult($"Error creating student: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CreateStudentAsync");
                return new BadRequestObjectResult($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<StudentDetailDto> GetStudentDetailByIdAsync(Guid id)
        {
            try
            {
                var student = await _studentRepository.GetStudentDetailByIdAsync(id);
                if (student == null)
                {
                    return null;
                }
                var studentDetailDto = _mapper.Map<StudentDetailDto>(student);
                
                // Get major information
                var majorResponse = await _grpcMajorClient.GetMajorByIdAsync(student.MajorId.ToString());
                
                if (majorResponse != null && majorResponse.Success && majorResponse.Data != null)
                {
                    studentDetailDto.MajorName = majorResponse.Data.Name;
                }
                
                return studentDetailDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student with id {Id}", id);
                throw;
            }
        }

        public async Task<StudentDto> GetStudentByEmailAsync(string email)
        {
            try
            {
                var student = await _studentRepository.GetStudentByEmailAsync(email);
                return _mapper.Map<StudentDto>(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student with email {Email}", email);
                throw;
            }
        }
    }
}            
