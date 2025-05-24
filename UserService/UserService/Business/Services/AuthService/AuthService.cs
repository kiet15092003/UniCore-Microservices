using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Business.Dtos.Auth;
using UserService.CommunicationTypes.Grpc.GrpcClient;
using UserService.CommunicationTypes.KafkaService.KafkaProducer;
using UserService.DataAccess.Repositories.TrainingManagerRepo;
using UserService.CommunicationTypes.Http.HttpClient;

namespace UserService.Business.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)

        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                throw new UnauthorizedAccessException("Invalid email or password.");

            var userRoles = await _userManager.GetRolesAsync(user);            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var role in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            var token = GenerateJwtToken(authClaims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }    public async Task<bool> ChangePasswordAsync(ChangePasswordDto model)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
                throw new UnauthorizedAccessException("Current password is incorrect.");
            
            // DTO validation now handles these checks:
            // - New password not matching confirmation
            // - New password not being the same as old password
            // - Password complexity requirements

            var result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
            if (!result.Succeeded)
                throw new InvalidOperationException($"Failed to change password: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return true;
        }
        catch (UnauthorizedAccessException)
        {
            // Rethrow authentication errors as is
            throw;
        }
        catch (InvalidOperationException)
        {
            // Rethrow validation errors as is
            throw;
        }
        catch (Exception ex)
        {
            // Log unexpected errors and rethrow with a user-friendly message
            // TODO: Add proper logging here
            throw new InvalidOperationException("An error occurred while changing the password. Please try again later.", ex);
        }
    }private JwtSecurityToken GenerateJwtToken(List<Claim> claims)
        {
            var jwtKey = _configuration["JWT:Key"] ?? throw new InvalidOperationException("JWT:Key is not configured");
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var jwtIssuer = _configuration["Jwt:Issuer"] ?? "DefaultIssuer";
            var jwtAudience = _configuration["Jwt:Audience"] ?? "DefaultAudience";

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                expires: DateTime.UtcNow.AddHours(2),
                claims: claims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}


