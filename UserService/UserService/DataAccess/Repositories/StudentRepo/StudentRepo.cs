using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Entities;
using UserService.CommunicationTypes.Http.HttpClient;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using AutoMapper;
using UserService.Business.Dtos.Student;
using System.Collections.Concurrent;
using UserService.Utils.Pagination;
using UserService.Utils.Filter;
using UserService.DataAccess.Repositories.GuardianRepo;
using UserService.Business.Services.GuardianService;
using UserService.Business.Dtos.Guardian;

namespace UserService.DataAccess.Repositories.StudentRepo
{
    public class StudentRepo : IStudentRepo
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SmtpClientService _smtpClient;
        private readonly ILogger<StudentRepo> _logger;
        private readonly IMapper _mapper;
        private readonly IGuardianRepo _guardianRepo;
        private readonly IGuardianService _guardianService;

        public StudentRepo(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<StudentRepo> logger,
            SmtpClientService smtpClient,
            IMapper mapper,
            IGuardianRepo guardianRepo,
            IGuardianService guardianService)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _smtpClient = smtpClient;
            _mapper = mapper;
            _guardianRepo = guardianRepo;
            _guardianService = guardianService;
        }


        public async Task<Student> GetStudentByIdAsync(Guid id)
        {
            var result = await _context.Students
                .Include(s => s.ApplicationUser)
                .FirstOrDefaultAsync(d => d.Id == id);
            if (result == null)
            {
                throw new KeyNotFoundException("Student not found");
            }
            return result;
        }

        public async Task<(List<ApplicationUser> Users, List<Student> Students)> AddStudentsWithUsersAsync(
            List<(ApplicationUser User, Student Student)> userStudentPairs)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var createdUsers = new List<ApplicationUser>();
                var createdStudents = new List<Student>();

                foreach (var (user, student) in userStudentPairs)
                {
                    _logger.LogInformation("-----------------------------------48 {user}", JsonSerializer.Serialize(user));
                    // Create user
                    var result = await _userManager.CreateAsync(user, $"Student@{user.Email.Split('@')[0]}");
                    _logger.LogInformation("-----------------------------------49 {result}", JsonSerializer.Serialize(result));
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }

                    // Add student role
                    await _userManager.AddToRoleAsync(user, "Student");

                    // Create email account
                    try
                    {
                        var success = await _smtpClient.CreateEmailAccountAsync(
                            user.Email,
                            $"Student@{user.Email.Split('@')[0]}"
                        );

                        if (!success)
                        {
                            throw new Exception($"Failed to create email account for {user.Email}");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error creating email account: {ex.Message}");
                    }

                    // Set ApplicationUserId for student
                    student.ApplicationUserId = user.Id;

                    // Add to lists
                    createdUsers.Add(user);
                    createdStudents.Add(student);
                }

                // Add all students
                await _context.Students.AddRangeAsync(createdStudents);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return (createdUsers, createdStudents);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Student> CreateAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
            return student;
        }

        private IQueryable<Student> ApplyFilter(IQueryable<Student> query, StudentListFilterParams filter)
        {
            if (!string.IsNullOrEmpty(filter.SearchQuery))
            {
                query = query.Where(s => s.StudentCode.Contains(filter.SearchQuery) || s.ApplicationUser.Email.Contains(filter.SearchQuery) || s.ApplicationUser.FirstName.Contains(filter.SearchQuery) || s.ApplicationUser.LastName.Contains(filter.SearchQuery));
            }
            if (filter.BatchId != null)
            {
                query = query.Where(s => s.BatchId == filter.BatchId);
            }
            if (filter.MajorId != null)
            {
                query = query.Where(s => s.MajorId == filter.MajorId);
            }
            return query;
        }

        private IQueryable<Student> ApplyOrder(IQueryable<Student> query, Order order)
        {
            if (order.By == "StudentCode")
            {
                query = order.IsDesc ? query.OrderByDescending(s => s.StudentCode) : query.OrderBy(s => s.StudentCode);
            }
            else if (order.By == "Email")
            {
                query = order.IsDesc ? query.OrderByDescending(s => s.ApplicationUser.Email) : query.OrderBy(s => s.ApplicationUser.Email);
            }
            else if (order.By == "FirstName")
            {
                query = order.IsDesc ? query.OrderByDescending(s => s.ApplicationUser.FirstName) : query.OrderBy(s => s.ApplicationUser.FirstName);
            }
            else if (order.By == "LastName")
            {
                query = order.IsDesc ? query.OrderByDescending(s => s.ApplicationUser.LastName) : query.OrderBy(s => s.ApplicationUser.LastName);
            }
            else {
                query = query = order.IsDesc ? query.OrderByDescending(s => s.StudentCode) : query.OrderBy(s => s.StudentCode);
            }
            return query;
        }

        public async Task<PaginationResult<StudentDto>> GetAllPaginationAsync(Pagination pagination, StudentListFilterParams filter, Order order)
        {
            var query = _context.Students
                .Include(s => s.ApplicationUser)
                .AsQueryable();

            // Apply filter if needed
            if (filter != null)
            {
                query = ApplyFilter(query, filter);
            }

            // Apply ordering if needed
            if (order != null)
            {
               query = ApplyOrder(query, order);
            }

            var result = await query
                .Skip((pagination.PageNumber - 1) * pagination.ItemsPerpage)
                .Take(pagination.ItemsPerpage)
                .ToListAsync();

            // Get total count before applying pagination
            int total = await query.CountAsync();

            var mappedResult = _mapper.Map<List<StudentDto>>(result);

            return new PaginationResult<StudentDto>
            {
                Data = mappedResult,
                Total = total,
                PageIndex = pagination.PageNumber,
                PageSize = pagination.ItemsPerpage
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var student = await _context.Students
                    .Include(s => s.ApplicationUser)
                    .FirstOrDefaultAsync(s => s.Id == id);
                
                if (student == null)
                    return false;

                // Delete student first
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();

                // Delete associated user
                if (student.ApplicationUser != null)
                {
                    await _userManager.DeleteAsync(student.ApplicationUser);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting student with id {Id}", id);
                throw;
            }
        }

        public async Task<Student> UpdateAsync(Student student)
        {
            _logger.LogInformation("-----------------------------------243 {student}", JsonSerializer.Serialize(student.ApplicationUser.Address));

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingStudent = await _context.Students
                    .Include(s => s.ApplicationUser)
                        .ThenInclude(u => u.Address)
                    .Include(s => s.Guardians)
                    .FirstOrDefaultAsync(s => s.Id == student.Id);

                if (existingStudent == null)
                    throw new KeyNotFoundException("Student not found");

                // Update student properties
                existingStudent.StudentCode = student.StudentCode;
                existingStudent.AccumulateCredits = student.AccumulateCredits;
                existingStudent.AccumulateScore = student.AccumulateScore;
                existingStudent.AccumulateActivityScore = student.AccumulateActivityScore;
                existingStudent.MajorId = student.MajorId;
                existingStudent.BatchId = student.BatchId;

                // Copy guardians from input student
                if (student.Guardians != null)
                {
                    // Create a new list instead of direct assignment
                    existingStudent.Guardians = new List<Guardian>(student.Guardians);
                }

                // Update associated user if needed
                if (existingStudent.ApplicationUser != null && student.ApplicationUser != null)
                {
                    existingStudent.ApplicationUser.FirstName = student.ApplicationUser.FirstName;
                    existingStudent.ApplicationUser.LastName = student.ApplicationUser.LastName;
                    existingStudent.ApplicationUser.PhoneNumber = student.ApplicationUser.PhoneNumber;
                    existingStudent.ApplicationUser.Status = student.ApplicationUser.Status;
                    existingStudent.ApplicationUser.ImageUrl = student.ApplicationUser.ImageUrl;
                }

    
                // Update guardians
                if (student.Guardians != null && student.Guardians.Any())
                {
                    // Create a temporary list to store new guardians
                    var newGuardians = new List<Guardian>();

                    // Process guardians
                    foreach (var guardian in student.Guardians)
                    {
                        if (guardian.Id != Guid.Empty)
                        {
                            // Update existing guardian
                            var existingGuardian = await _guardianRepo.GetGuardianByIdAsync(guardian.Id);
                            if (existingGuardian != null)
                            {
                                existingGuardian.Name = guardian.Name;
                                existingGuardian.PhoneNumber = guardian.PhoneNumber;
                                existingGuardian.Relationship = guardian.Relationship;
                                existingGuardian.StudentId = existingStudent.Id;
                                await _guardianRepo.UpdateGuardianAsync(existingGuardian);
                                newGuardians.Add(existingGuardian);
                            }
                        }
                        else
                        {
                            // Create new guardian
                            guardian.StudentId = existingStudent.Id;
                            var newGuardian = await _guardianRepo.CreateGuardianAsync(guardian);
                            newGuardians.Add(newGuardian);
                        }
                    }

                    // Add all processed guardians to existingStudent
                    existingStudent.Guardians.AddRange(newGuardians);
                }

                if (student.ApplicationUser.Address != null)
                {
                    if (existingStudent.ApplicationUser.Address == null)
                    {
                        // Tạo mới Address
                        var newAddress = new Address
                        {
                            Country = student.ApplicationUser.Address.Country,
                            City = student.ApplicationUser.Address.City,
                            District = student.ApplicationUser.Address.District,
                            Ward = student.ApplicationUser.Address.Ward,
                            AddressDetail = student.ApplicationUser.Address.AddressDetail
                        };
                        
                        // Thêm vào context để theo dõi
                        _context.Addresses.Add(newAddress);
                        
                        // Lưu ngay để có Id
                        await _context.SaveChangesAsync();
                        
                        // Cập nhật mối quan hệ trong ApplicationUser
                        existingStudent.ApplicationUser.Address = newAddress;
                        existingStudent.ApplicationUser.AddressId = newAddress.Id;
                    }
                    else
                    {
                        // Lấy Address từ context để đảm bảo nó được theo dõi
                        var addressId = existingStudent.ApplicationUser.AddressId.Value;
                        var existingAddress = await _context.Set<Address>().FindAsync(addressId);
                        
                        if (existingAddress != null)
                        {
                            // Cập nhật thuộc tính cho đối tượng được theo dõi
                            existingAddress.Country = student.ApplicationUser.Address.Country;
                            existingAddress.City = student.ApplicationUser.Address.City;
                            existingAddress.District = student.ApplicationUser.Address.District;
                            existingAddress.Ward = student.ApplicationUser.Address.Ward;
                            existingAddress.AddressDetail = student.ApplicationUser.Address.AddressDetail;
                            
                            // Đánh dấu đã thay đổi
                            _context.Entry(existingAddress).State = EntityState.Modified;
                        }
                        else
                        {
                            // Address không tồn tại trong DB mặc dù có ID
                            // Tạo mới Address
                            var newAddress = new Address
                            {
                                Country = student.ApplicationUser.Address.Country,
                                City = student.ApplicationUser.Address.City,
                                District = student.ApplicationUser.Address.District,
                                Ward = student.ApplicationUser.Address.Ward,
                                AddressDetail = student.ApplicationUser.Address.AddressDetail
                            };
                            
                            _context.Addresses.Add(newAddress);
                            await _context.SaveChangesAsync();
                            
                            // Cập nhật mối quan hệ
                            existingStudent.ApplicationUser.Address = newAddress;
                            existingStudent.ApplicationUser.AddressId = newAddress.Id;
                        }
                    }
                }

                _context.Students.Update(existingStudent);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return existingStudent;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating student with id {Id}", student.Id);
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Student> GetStudentDetailByIdAsync(Guid id)
        {
            var result = await _context.Students
                .Include(s => s.ApplicationUser)
                    .ThenInclude(u => u.Address)
                .Include(s => s.Guardians)
                .Include(s => s.Batch)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (result == null)
            {
                throw new KeyNotFoundException("Student not found");
            }
            return result;
        }
    }
}