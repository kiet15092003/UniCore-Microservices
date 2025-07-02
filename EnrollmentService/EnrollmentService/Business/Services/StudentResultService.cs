using AutoMapper;
using EnrollmentService.DataAccess.Repositories;
using EnrollmentService.Business.Dtos.StudentResult;
using EnrollmentService.Utils.Filter;
using EnrollmentService.Utils.Pagination;
using Microsoft.Extensions.Logging;
using EnrollmentService.Entities;
using EnrollmentService.CommunicationTypes.Grpc.GrpcClient;
using ClosedXML.Excel;
using System.Data;
using Microsoft.AspNetCore.Http;

namespace EnrollmentService.Business.Services
{
    public class StudentResultService : IStudentResultService
    {
        private readonly IStudentResultRepository _studentResultRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentResultService> _logger;
        private readonly GrpcStudentClientService _studentClient;

        public StudentResultService(
            IStudentResultRepository studentResultRepository,
            IMapper mapper,
            ILogger<StudentResultService> logger,
            GrpcStudentClientService studentClient)
        {
            _studentResultRepository = studentResultRepository;
            _mapper = mapper;
            _logger = logger;
            _studentClient = studentClient;
        }

        public async Task<StudentResultDto?> GetStudentResultByIdAsync(Guid id)
        {
            try
            {
                var studentResult = await _studentResultRepository.GetStudentResultByIdAsync(id);
                return studentResult != null ? _mapper.Map<StudentResultDto>(studentResult) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student result with ID {Id}", id);
                throw;
            }
        }

        public async Task<PaginationResult<StudentResultDto>> GetAllStudentResultsPaginationAsync(
            Pagination pagination,
            StudentResultListFilterParams filterParams,
            Order? order)
        {
            try
            {
                var result = await _studentResultRepository.GetAllStudentResultsPaginationAsync(
                    pagination, filterParams, order);

                return new PaginationResult<StudentResultDto>
                {
                    Data = _mapper.Map<List<StudentResultDto>>(result.Data),
                    Total = result.Total,
                    PageSize = result.PageSize,
                    PageIndex = result.PageIndex
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting paginated student results");
                throw;
            }
        }

        public async Task<StudentResultDto> UpdateStudentResultAsync(Guid id, UpdateStudentResultDto updateDto)
        {
            try
            {
                var studentResult = await _studentResultRepository.GetStudentResultByIdAsync(id);
                if (studentResult == null)
                {
                    throw new KeyNotFoundException($"Student result with ID {id} not found");
                }

                _mapper.Map(updateDto, studentResult);
                var updatedResult = await _studentResultRepository.UpdateStudentResultAsync(studentResult);
                return _mapper.Map<StudentResultDto>(updatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating student result with ID {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteStudentResultAsync(Guid id)
        {
            try
            {
                return await _studentResultRepository.DeleteStudentResultAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting student result with ID {Id}", id);
                throw;
            }
        }

        public async Task<List<StudentResultDto>> GetStudentResultsByEnrollmentIdAsync(Guid enrollmentId)
        {
            try
            {
                var results = await _studentResultRepository.GetStudentResultsByEnrollmentIdAsync(enrollmentId);
                return _mapper.Map<List<StudentResultDto>>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student results for enrollment ID {EnrollmentId}", enrollmentId);
                throw;
            }
        }

        public async Task<List<StudentResultDto>> GetStudentResultsByEnrollmentIdsAsync(List<Guid> enrollmentIds)
        {
            try
            {
                var results = await _studentResultRepository.GetStudentResultsByEnrollmentIdsAsync(enrollmentIds);
                return _mapper.Map<List<StudentResultDto>>(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student results for enrollment IDs {EnrollmentIds}", enrollmentIds);
                throw;
            }
        }

        public async Task<List<StudentResultByStudentDto>> GetStudentResultsByClassIdAsync(Guid classId)
        {
            try
            {
                var results = await _studentResultRepository.GetStudentResultsByClassIdAsync(classId);
                // Group by studentId
                var groups = results.GroupBy(r => r.Enrollment.StudentId).ToList();
                var dtos = new List<StudentResultByStudentDto>();
                foreach (var group in groups)
                {
                    var studentId = group.Key;
                    string studentCode = string.Empty;
                    string studentName = string.Empty;
                    // Call gRPC to get student info
                    try
                    {
                        var studentResponse = await _studentClient.GetStudentById(studentId.ToString());
                        if (studentResponse?.Success == true && studentResponse.Data != null)
                        {
                            studentCode = studentResponse.Data.StudentCode;
                            var user = studentResponse.Data.User;
                            if (user != null)
                                studentName = $"{user.LastName} {user.FirstName}".Trim();
                        }
                    }
                    catch { }
                    // Map results for this student
                    var resultDtos = group.Select(sr => {
                        var dto = _mapper.Map<StudentResultDto>(sr);
                        dto.StudentCode = studentCode;
                        dto.StudentName = studentName;
                        return dto;
                    }).ToList();
                    dtos.Add(new StudentResultByStudentDto
                    {
                        StudentId = studentId,
                        StudentCode = studentCode,
                        StudentName = studentName,
                        Results = resultDtos
                    });
                }
                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting student results for class ID {ClassId}", classId);
                throw;
            }
        }

        public async Task<ImportScoreResultDto> ImportScoresFromExcelAsync(Guid classId, IFormFile excelFile)
        {
            var result = new ImportScoreResultDto();
            var errors = new List<ImportScoreError>();
            var successes = new List<ImportScoreSuccess>();
            var studentResultsToUpdate = new List<StudentResult>();

            try
            {
                // Validate file
                if (excelFile == null || excelFile.Length == 0)
                {
                    throw new ArgumentException("Excel file is required");
                }

                if (!excelFile.FileName.EndsWith(".xlsx") && !excelFile.FileName.EndsWith(".xls"))
                {
                    throw new ArgumentException("File must be an Excel file (.xlsx or .xls)");
                }

                // Read Excel file
                var excelRows = await ReadExcelFileAsync(excelFile);
                result.TotalRows = excelRows.Count;

                // Get existing student results for the class
                var existingStudentResults = await _studentResultRepository.GetStudentResultsByClassIdWithEnrollmentAsync(classId);

                // Process each row
                for (int i = 0; i < excelRows.Count; i++)
                {
                    var row = excelRows[i];
                    var rowNumber = i + 2; // Excel rows start from 1, and we skip header

                    try
                    {
                        // Validate student code
                        if (string.IsNullOrWhiteSpace(row.StudentCode))
                        {
                            errors.Add(new ImportScoreError
                            {
                                RowNumber = rowNumber,
                                StudentCode = row.StudentCode,
                                ScoreTypeName = "N/A",
                                ErrorMessage = "Student code is required"
                            });
                            continue;
                        }

                        // Find student by code
                        var studentId = await GetStudentIdByCodeAsync(row.StudentCode, classId);
                        if (!studentId.HasValue)
                        {
                            errors.Add(new ImportScoreError
                            {
                                RowNumber = rowNumber,
                                StudentCode = row.StudentCode,
                                ScoreTypeName = "N/A",
                                ErrorMessage = "Student not found"
                            });
                            continue;
                        }

                        // Find enrollment for this student and class
                        var enrollment = existingStudentResults
                            .FirstOrDefault(sr => sr.Enrollment.StudentId == studentId.Value)
                            ?.Enrollment;

                        if (enrollment == null)
                        {
                            errors.Add(new ImportScoreError
                            {
                                RowNumber = rowNumber,
                                StudentCode = row.StudentCode,
                                ScoreTypeName = "N/A",
                                ErrorMessage = "Student is not enrolled in this class"
                            });
                            continue;
                        }

                        // Process each score type
                        var scoreTypes = new[]
                        {
                            new { Score = row.TheoryScore, TypeName = "1", DisplayName = "Theory" },
                            new { Score = row.PracticeScore, TypeName = "2", DisplayName = "Practice" },
                            new { Score = row.MidtermScore, TypeName = "3", DisplayName = "Midterm" },
                            new { Score = row.FinalScore, TypeName = "4", DisplayName = "Final" }
                        };

                        foreach (var scoreType in scoreTypes)
                        {
                            if (!scoreType.Score.HasValue)
                                continue;

                            var score = scoreType.Score.Value;

                            // Validate score range
                            if (score < 0 || score > 10)
                            {
                                errors.Add(new ImportScoreError
                                {
                                    RowNumber = rowNumber,
                                    StudentCode = row.StudentCode,
                                    ScoreTypeName = scoreType.DisplayName,
                                    ErrorMessage = "Score must be between 0 and 10"
                                });
                                continue;
                            }

                            // Find score type by name
                            var scoreTypeEntity = existingStudentResults
                                .FirstOrDefault(sr => sr.ScoreType.Type.ToString() == scoreType.TypeName)
                                ?.ScoreType;

                            if (scoreTypeEntity == null)
                            {
                                errors.Add(new ImportScoreError
                                {
                                    RowNumber = rowNumber,
                                    StudentCode = row.StudentCode,
                                    ScoreTypeName = scoreType.DisplayName,
                                    ErrorMessage = "Score type not found"
                                });
                                continue;
                            }

                            // Find existing student result
                            var studentResult = existingStudentResults
                                .FirstOrDefault(sr => sr.EnrollmentId == enrollment.Id && sr.ScoreTypeId == scoreTypeEntity.Id);

                            if (studentResult == null)
                            {
                                errors.Add(new ImportScoreError
                                {
                                    RowNumber = rowNumber,
                                    StudentCode = row.StudentCode,
                                    ScoreTypeName = scoreType.DisplayName,
                                    ErrorMessage = "Student result record not found"
                                });
                                continue;
                            }

                            // Update score
                            studentResult.Score = score;
                            studentResult.UpdatedAt = DateTime.UtcNow;
                            studentResultsToUpdate.Add(studentResult);

                            successes.Add(new ImportScoreSuccess
                            {
                                RowNumber = rowNumber,
                                StudentCode = row.StudentCode,
                                ScoreTypeName = scoreType.DisplayName,
                                Score = score,
                                StudentResultId = studentResult.Id
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new ImportScoreError
                        {
                            RowNumber = rowNumber,
                            StudentCode = row.StudentCode,
                            ScoreTypeName = "N/A",
                            ErrorMessage = $"Error processing row: {ex.Message}"
                        });
                    }
                }

                // Bulk update scores
                if (studentResultsToUpdate.Any())
                {
                    await _studentResultRepository.BulkUpdateScoresAsync(studentResultsToUpdate);
                }

                result.SuccessCount = successes.Count;
                result.FailureCount = errors.Count;
                result.Errors = errors;
                result.Successes = successes;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while importing scores from Excel for class ID {ClassId}", classId);
                throw;
            }
        }

        private async Task<List<ExcelScoreRow>> ReadExcelFileAsync(IFormFile file)
        {
            var rows = new List<ExcelScoreRow>();

            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1); // First worksheet

            var usedRange = worksheet.RangeUsed();
            if (usedRange == null) return rows;

            // Skip header row, start from row 2
            for (int row = 2; row <= usedRange.RowCount(); row++)
            {
                var studentCode = worksheet.Cell(row, 1).GetValue<string>()?.Trim();
                var theoryScore = worksheet.Cell(row, 2).GetValue<string>()?.Trim();
                var practiceScore = worksheet.Cell(row, 3).GetValue<string>()?.Trim();
                var midtermScore = worksheet.Cell(row, 4).GetValue<string>()?.Trim();
                var finalScore = worksheet.Cell(row, 5).GetValue<string>()?.Trim();

                // Skip empty rows (no student code)
                if (string.IsNullOrWhiteSpace(studentCode))
                    continue;

                var excelRow = new ExcelScoreRow
                {
                    StudentCode = studentCode ?? ""
                };

                // Parse scores if they exist
                if (double.TryParse(theoryScore, out double theory))
                    excelRow.TheoryScore = theory;
                
                if (double.TryParse(practiceScore, out double practice))
                    excelRow.PracticeScore = practice;
                
                if (double.TryParse(midtermScore, out double midterm))
                    excelRow.MidtermScore = midterm;
                
                if (double.TryParse(finalScore, out double final))
                    excelRow.FinalScore = final;

                rows.Add(excelRow);
            }

            return rows;
        }

        private async Task<Guid?> GetStudentIdByCodeAsync(string studentCode, Guid classId)
        {
            try
            {
                // Get all unique student IDs from enrollments for this class
                var studentResults = await _studentResultRepository.GetStudentResultsByClassIdWithEnrollmentAsync(classId);
                var uniqueStudentIds = studentResults.Select(sr => sr.Enrollment.StudentId).Distinct().ToList();

                // For each student ID, check if it matches the student code
                foreach (var studentId in uniqueStudentIds)
                {
                    try
                    {
                        var studentResponse = await _studentClient.GetStudentById(studentId.ToString());
                        
                        if (studentResponse?.Success == true && 
                            studentResponse.Data != null && 
                            studentResponse.Data.StudentCode.Equals(studentCode, StringComparison.OrdinalIgnoreCase))
                        {
                            return studentId;
                        }
                    }
                    catch (Exception)
                    {
                        // If gRPC call fails, skip this student
                        continue;
                    }
                }

                return null;
            }
            catch (Exception)
            {
                // If any error occurs, return null
                return null;
            }
        }

        public async Task<ImportScoreResultDto> UpdateScoresBatchAsync(UpdateScoreBatchDto batchDto)
        {
            var result = new ImportScoreResultDto();
            var errors = new List<ImportScoreError>();
            var successes = new List<ImportScoreSuccess>();
            var studentResultsToUpdate = new List<StudentResult>();

            try
            {
                if (batchDto == null || batchDto.Scores == null || !batchDto.Scores.Any())
                {
                    throw new ArgumentException("Score list is required");
                }

                // Lấy tất cả StudentResult của class này
                var existingStudentResults = await _studentResultRepository.GetStudentResultsByClassIdAsync(batchDto.ClassId);

                for (int i = 0; i < batchDto.Scores.Count; i++)
                {
                    var row = batchDto.Scores[i];
                    var rowNumber = i + 1;

                    try
                    {
                        // Validate student code
                        if (string.IsNullOrWhiteSpace(row.StudentCode))
                        {
                            errors.Add(new ImportScoreError
                            {
                                RowNumber = rowNumber,
                                StudentCode = row.StudentCode,
                                ScoreTypeName = "N/A",
                                ErrorMessage = "Student code is required"
                            });
                            continue;
                        }

                        var studentId = await GetStudentIdByCodeAsync(row.StudentCode, batchDto.ClassId);
                        if (!studentId.HasValue)
                        {
                            errors.Add(new ImportScoreError
                            {
                                RowNumber = rowNumber,
                                StudentCode = row.StudentCode,
                                ScoreTypeName = "N/A",
                                ErrorMessage = "Student not found"
                            });
                            continue;
                        }

                        // Tìm enrollment cho studentId này trong class
                        var enrollment = existingStudentResults
                            .FirstOrDefault(sr => sr.Enrollment.StudentId == studentId.Value)?.Enrollment;
                        if (enrollment == null)
                        {
                            errors.Add(new ImportScoreError
                            {
                                RowNumber = rowNumber,
                                StudentCode = row.StudentCode,
                                ScoreTypeName = "N/A",
                                ErrorMessage = "Student is not enrolled in this class"
                            });
                            continue;
                        }

                        // Xử lý từng loại điểm
                        var scoreTypes = new[]
                        {
                            new { Score = row.TheoryScore, Type = 1, DisplayName = "Theory" },
                            new { Score = row.PracticeScore, Type = 2, DisplayName = "Practice" },
                            new { Score = row.MidtermScore, Type = 3, DisplayName = "Midterm" },
                            new { Score = row.FinalScore, Type = 4, DisplayName = "Final" }
                        };

                        foreach (var scoreType in scoreTypes)
                        {
                            if (!scoreType.Score.HasValue)
                                continue;

                            var score = scoreType.Score.Value;
                            if (score < 0 || score > 10)
                            {
                                errors.Add(new ImportScoreError
                                {
                                    RowNumber = rowNumber,
                                    StudentCode = row.StudentCode,
                                    ScoreTypeName = scoreType.DisplayName,
                                    ErrorMessage = "Score must be between 0 and 10"
                                });
                                continue;
                            }

                            // Tìm scoreTypeEntity phù hợp
                            var scoreTypeEntity = existingStudentResults
                                .FirstOrDefault(sr => sr.ScoreType.Type == scoreType.Type)?.ScoreType;
                            if (scoreTypeEntity == null)
                            {
                                errors.Add(new ImportScoreError
                                {
                                    RowNumber = rowNumber,
                                    StudentCode = row.StudentCode,
                                    ScoreTypeName = scoreType.DisplayName,
                                    ErrorMessage = "Score type not found for this class"
                                });
                                continue;
                            }

                            // Tìm studentResult
                            var studentResult = existingStudentResults
                                .FirstOrDefault(sr => sr.EnrollmentId == enrollment.Id && sr.ScoreTypeId == scoreTypeEntity.Id);
                            if (studentResult == null)
                            {
                                errors.Add(new ImportScoreError
                                {
                                    RowNumber = rowNumber,
                                    StudentCode = row.StudentCode,
                                    ScoreTypeName = scoreType.DisplayName,
                                    ErrorMessage = "Student result record not found"
                                });
                                continue;
                            }

                            studentResult.Score = score;
                            studentResult.UpdatedAt = DateTime.UtcNow;
                            studentResultsToUpdate.Add(studentResult);

                            successes.Add(new ImportScoreSuccess
                            {
                                RowNumber = rowNumber,
                                StudentCode = row.StudentCode,
                                ScoreTypeName = scoreType.DisplayName,
                                Score = score,
                                StudentResultId = studentResult.Id
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(new ImportScoreError
                        {
                            RowNumber = rowNumber,
                            StudentCode = row.StudentCode,
                            ScoreTypeName = "N/A",
                            ErrorMessage = $"Error processing row: {ex.Message}"
                        });
                    }
                }

                // Bulk update
                if (studentResultsToUpdate.Any())
                {
                    await _studentResultRepository.BulkUpdateScoresAsync(studentResultsToUpdate);
                }

                result.SuccessCount = successes.Count;
                result.FailureCount = errors.Count;
                result.Errors = errors;
                result.Successes = successes;
                result.TotalRows = batchDto.Scores.Count;
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while batch updating scores for class {ClassId}", batchDto?.ClassId);
                throw;
            }
        }
    }
} 