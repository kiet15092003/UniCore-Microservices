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

namespace UserService.DataAccess.Repositories.StudentRepo
{
    public class StudentRepo : IStudentRepo
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SmtpClientService _smtpClient;
        private readonly ILogger<StudentRepo> _logger;
        private readonly IMapper _mapper;

        public StudentRepo(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<StudentRepo> logger,
            SmtpClientService smtpClient,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _smtpClient = smtpClient;
            _mapper = mapper;
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
        }        public async Task<(List<ApplicationUser> Users, List<Student> Students)> AddStudentsWithUsersAsync(
            List<(ApplicationUser User, Student Student)> userStudentPairs, 
            Dictionary<string, string>? passwords = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var createdUsers = new List<ApplicationUser>();
                var createdStudents = new List<Student>();                foreach (var (user, student) in userStudentPairs)
                {
                    // Get password from dictionary or generate a default one
                    string password;
                    if (passwords != null && user.Email != null && passwords.ContainsKey(user.Email))
                    {
                        password = passwords[user.Email];
                    }
                    else
                    {
                        password = Utils.PasswordGenerator.GenerateSecurePassword();
                    }

                    // Create user
                    var result = await _userManager.CreateAsync(user, password);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }

                    // Add student role
                    await _userManager.AddToRoleAsync(user, "Student");

                    // Create email account
                    //try
                    //{
                    //    var success = await _smtpClient.CreateEmailAccountAsync(
                    //        user.Email,
                    //        $"Student@{user.Email.Split('@')[0]}"
                    //    );

                    //    if (!success)
                    //    {
                    //        throw new Exception($"Failed to create email account for {user.Email}");
                    //    }
                    //}
                    //catch (Exception ex)
                    //{
                    //    throw new Exception($"Error creating email account: {ex.Message}");
                    //}

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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingStudent = await _context.Students
                    .Include(s => s.ApplicationUser)
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
                existingStudent.GuardianId = student.GuardianId;

                // Update associated user if needed
                if (existingStudent.ApplicationUser != null && student.ApplicationUser != null)
                {
                    existingStudent.ApplicationUser.FirstName = student.ApplicationUser.FirstName;
                    existingStudent.ApplicationUser.LastName = student.ApplicationUser.LastName;
                    existingStudent.ApplicationUser.PhoneNumber = student.ApplicationUser.PhoneNumber;
                    existingStudent.ApplicationUser.Status = student.ApplicationUser.Status;
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
    }
}