using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserService.Entities;
using ClosedXML.Excel;
using UserService.Business.Services.StudentService;
using UserService.Business.Dtos.Student;

using UserService.CommunicationTypes.Http.HttpClient;
using UserService.CommunicationTypes.Grpc.GrpcClient;
using UserService.DataAccess.Repositories.StudentRepo;

namespace UserService.Business.Services.StudentService
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepo _studentRepository;
        private readonly ILogger<StudentService> _logger;
        private readonly SmtpClientService _smtpClient;
        private readonly GrpcMajorClientService _grpcMajorClient;

        public StudentService(
            IStudentRepo studentRepository,
            ILogger<StudentService> logger,
            SmtpClientService smtpClient,
            GrpcMajorClientService grpcMajorClient)
        {
            _studentRepository = studentRepository;
            _logger = logger;
            _smtpClient = smtpClient;
            _grpcMajorClient = grpcMajorClient;
        }

        public async Task<IActionResult> CreateStudentByExcelAsync(CreateStudentByExcelDto createStudentByExcelDto)
        {
            try
            {
                var file = createStudentByExcelDto.ExcelFile;
                var batchId = createStudentByExcelDto.BatchId;
                var majorId = createStudentByExcelDto.MajorId;

                if (file == null || file.Length == 0)
                {
                    return new BadRequestObjectResult("File is empty");
                }

                if (!file.FileName.EndsWith(".xlsx"))
                {
                    return new BadRequestObjectResult("Only Excel files (.xlsx) are allowed");
                }

                var major = await _grpcMajorClient.GetMajorByIdAsync(majorId.ToString());
                if (!major.Success)
                {
                    return new BadRequestObjectResult("Major not found");
                }

                var results = new List<string>();
                var userStudentPairs = new List<(ApplicationUser User, Entities.Student Student)>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var workbook = new XLWorkbook(stream))
                    {
                        var worksheet = workbook.Worksheet(1);
                        var rowCount = worksheet.LastRowUsed().RowNumber();

                        // Validate headers
                        var headers = new Dictionary<string, int>
                        {
                            { "FirstName", 0 },
                            { "LastName", 0 },
                            { "Dob", 0 },
                            { "PersonId", 0 },
                            { "PhoneNumber", 0 },
                            { "StudentCode", 0 }
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
                                var studentCode = worksheet.Cell(row, headers["StudentCode"]).Value.ToString();
                                var email = $"{studentCode.ToLower()}@unicore.edu";
                                var password = $"Student@{studentCode}";
                                var firstName = worksheet.Cell(row, headers["FirstName"]).Value.ToString();
                                var lastName = worksheet.Cell(row, headers["LastName"]).Value.ToString();
                                var dob = DateTime.Parse(worksheet.Cell(row, headers["Dob"]).Value.ToString());
                                var personId = worksheet.Cell(row, headers["PersonId"]).Value.ToString();
                                var phoneNumber = worksheet.Cell(row, headers["PhoneNumber"]).Value.ToString();

                                // Create ApplicationUser
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

                                userStudentPairs.Add((user, student));

                                results.Add($"Prepared student {studentCode} with email: {email}");
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error processing row {Row}: {Message}", row, ex.Message);
                                results.Add($"Error processing row {row}: {ex.Message}");
                            }
                        }
                    }
                }

                if (!userStudentPairs.Any())
                {
                    return new BadRequestObjectResult("No valid students found in the file");
                }

                try
                {
                    var (createdUsers, createdStudents) = await _studentRepository.AddStudentsWithUsersAsync(userStudentPairs);
                    _logger.LogInformation("Successfully created {Count} students", createdStudents.Count);
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
    }
}

