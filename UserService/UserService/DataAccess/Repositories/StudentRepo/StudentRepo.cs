using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Entities;
using UserService.CommunicationTypes.Http.HttpClient;

namespace UserService.DataAccess.Repositories.StudentRepo
{
    public class StudentRepo : IStudentRepo
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SmtpClientService _smtpClient;

        public StudentRepo(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            SmtpClientService smtpClient)
        {
            _context = context;
            _userManager = userManager;
            _smtpClient = smtpClient;
        }

        public async Task<Student> GetStudentByIdAsync(Guid id)
        {
            var result = await _context.Students.FirstOrDefaultAsync(d => d.Id == id);
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
                    // Create user
                    var result = await _userManager.CreateAsync(user, $"Student@{user.Email.Split('@')[0]}");
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
                    student.ApplicationUserId = Guid.Parse(user.Id);

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

        public async Task<List<Student>> GetAllAsync()
        {
            return await _context.Students
                .Include(s => s.ApplicationUser)
                .Include(s => s.Batch)
                .ToListAsync();
        }

        public async Task<Student> UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}