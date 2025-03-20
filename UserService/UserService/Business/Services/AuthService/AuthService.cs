using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Business.Dtos.Auth;
using UserService.CommunicationTypes.Grpc.GrpcClient;
using UserService.CommunicationTypes.KafkaService.KafkaProducer;
using UserService.CommunicationTypes.KafkaService.KafkaProducer.Templates;
using UserService.DataAccess.Repositories.TrainingManagerRepo;
using UserService.Entities;

namespace UserService.Business.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IKafkaProducerService _kafkaProducerService;
        private readonly ITrainingManagerRepo _trainingManagerRepo;
        private readonly GrpcMajorClientService _grpcClient;
        private readonly GrpcBatchClientService _grpcBatchClient;    
        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IMapper mapper,
            IKafkaProducerService kafkaProducerService,
            ITrainingManagerRepo trainingManagerRepo,
            GrpcMajorClientService grpcClient,
            GrpcBatchClientService grpcBatchClient)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _kafkaProducerService = kafkaProducerService;
            _trainingManagerRepo = trainingManagerRepo;
            _grpcClient = grpcClient;
            _grpcBatchClient = grpcBatchClient;
        }

        public async Task<IActionResult> RegisterStudentAsync(RegisterStudentDto registerStudentDto)
        {
            var userExists = await _userManager.FindByEmailAsync(registerStudentDto.Email);
            
            if (userExists != null)
                throw new KeyNotFoundException("User already exists.");

            var newUser = new ApplicationUser
            {
                UserName = registerStudentDto.Email,
                Email = registerStudentDto.Email,
                FirstName = registerStudentDto.FirstName,
                LastName = registerStudentDto.LastName,
                Dob = registerStudentDto.Dob,
                PersonId = registerStudentDto.PersonId,
                PhoneNumber = registerStudentDto.PhoneNumber
            };

            var major = await _grpcClient.GetMajorByIdAsync(registerStudentDto.MajorId.ToString());

            if (!major.Success)
            {
                throw new KeyNotFoundException("Major not found");
            }

            var batch = await _grpcBatchClient.GetBatchByIdAsync(registerStudentDto.BatchId.ToString());

            if (!batch.Success)
            {
                throw new KeyNotFoundException("Batch not found");
            }

            var result = await _userManager.CreateAsync(newUser, registerStudentDto.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(newUser, "Student");

            // Kafka send message
            var studentSendData = new StudentCreatedEventDTO
            {
                Data = new StudentCreatedEventData
                {
                    StudentCode = registerStudentDto.StudentCode,
                    ApplicationUserId = Guid.Parse(newUser.Id),
                    MajorId = registerStudentDto.MajorId,
                    BatchId = registerStudentDto.BatchId,
                }
            };

            await _kafkaProducerService.PublishMessageAsync("StudentCreatedEvent", studentSendData);

            Console.WriteLine($"--------Student Created");           

            return new OkObjectResult("Student registered successfully.");
        }

        public async Task<IActionResult> RegisterTrainingManagerAsync(RegisterTrainingManagerDto registerTrainingManagerDto)
        {
            var userExists = await _userManager.FindByEmailAsync(registerTrainingManagerDto.Email);

            if (userExists != null)
                throw new KeyNotFoundException("User already exists.");

            var newUser = new ApplicationUser
            {
                UserName = registerTrainingManagerDto.Email,
                Email = registerTrainingManagerDto.Email,
                FirstName = registerTrainingManagerDto.FirstName,
                LastName = registerTrainingManagerDto.LastName,
                Dob = registerTrainingManagerDto.Dob,
                PersonId = registerTrainingManagerDto.PersonId,
                PhoneNumber = registerTrainingManagerDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(newUser, registerTrainingManagerDto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(newUser, "TrainingManager");

            var trainingManager = new TrainingManager
            {
                ApplicationUserId = newUser.Id,
                ApplicationUser = newUser,
                TrainingManagerCode = registerTrainingManagerDto.TrainingManagerCode
            };

            await _trainingManagerRepo.CreateTrainingManagerAsync(trainingManager);

            // Kafka send message
            //var trainingManagerSendData = new TrainingManagerCreatedEventDTO
            //{
            //    Data = new TrainingManagerCreatedEventData
            //    {
            //        Id = trainingManager.Id,
            //        TrainingManagerCode = registerTrainingManagerDto.TrainingManagerCode,
            //        Email = newUser.Email,
            //        FullName = newUser.FullName,
            //    }
            //};
            //
            //await _kafkaProducerService.PublishMessageAsync("TrainingManagerCreatedEvent", trainingManagerSendData);

            Console.WriteLine($"--------TrainingManager Created: {trainingManager.Id} - {trainingManager.TrainingManagerCode}");

            return new OkObjectResult("TrainingManager registered successfully.");
        }

        public async Task<string> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                throw new UnauthorizedAccessException("Invalid email or password.");

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            var token = GenerateJwtToken(authClaims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private JwtSecurityToken GenerateJwtToken(List<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.UtcNow.AddHours(2),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
        public async Task<List<UserReadDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return _mapper.Map<List<UserReadDto>>(users);
        }
    }
}
