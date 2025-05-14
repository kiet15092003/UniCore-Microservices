using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace UserService.CommunicationTypes.Http.HttpClient
{
    public class SmtpClientService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpClientService> _logger;
        private readonly System.Net.Http.HttpClient _httpClient;

        public SmtpClientService(IConfiguration configuration, ILogger<SmtpClientService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = new System.Net.Http.HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", _configuration["SmtpSettings:ApiKey"]);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<bool> CreateEmailAccountAsync(string address, string password)
        {
            try
            {
                var request = new
                {
                    address,
                    password
                };

                var response = await _httpClient.PostAsJsonAsync(
                    _configuration["SmtpSettings:SmtpServliceUr"] + "/accounts",
                    request
                );

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to create email account: {error}");
                    return false;
                }

                _logger.LogInformation($"Email account created successfully for {address}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating email account");
                return false;
            }
        }
    }
} 