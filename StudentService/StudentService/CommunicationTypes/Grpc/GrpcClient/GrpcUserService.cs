using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;
using StudentService.CommunicationTypes.Grpc.GrpcClient.Protos;

namespace StudentService.CommunicationTypes.Grpc.GrpcClient
{
    public class GrpcUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions;
        private readonly GrpcChannel _channel;
        private readonly GrpcUser.GrpcUserClient _client;

        public GrpcUserService(IConfiguration configuration, IMemoryCache cache)
        {
            _configuration = configuration;
            _cache = cache;
            _cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            var grpcUrl = _configuration["GrpcSettings:UserServiceUrl"];
            _channel = GrpcChannel.ForAddress(grpcUrl);
            _client = new GrpcUser.GrpcUserClient(_channel);
        }

        public async Task<GetUserByIdResponse> GetUserByIdAsync(string userId)
        {
            // Try to get from cache first
            if (_cache.TryGetValue($"user_{userId}", out UserData cachedUser))
            {
                return new GetUserByIdResponse
                {
                    Success = true,
                    Data = cachedUser
                };
            }

            try
            {
                var request = new GetUserByIdRequest { UserId = userId };
                var response = await _client.GetUserByIdAsync(request);

                if (response.Success)
                {
                    // Cache the successful response
                    _cache.Set($"user_{userId}", response.Data, _cacheOptions);
                }

                return response;
            }
            catch (Exception ex)
            {
                return new GetUserByIdResponse
                {
                    Success = false,
                    Errors = { $"gRPC call failed: {ex.Message}" }
                };
            }
        }

        public async Task<GetUsersByIdsResponse> GetUsersByIdsAsync(IEnumerable<string> userIds)
        {
            var uncachedUserIds = new List<string>();
            var cachedUsers = new List<UserData>();

            // Check cache for each user
            foreach (var userId in userIds)
            {
                if (_cache.TryGetValue($"user_{userId}", out UserData cachedUser))
                {
                    cachedUsers.Add(cachedUser);
                }
                else
                {
                    uncachedUserIds.Add(userId);
                }
            }

            // If all users were in cache, return them
            if (!uncachedUserIds.Any())
            {
                return new GetUsersByIdsResponse
                {
                    Success = true,
                    Data = { cachedUsers }
                };
            }

            try
            {
                var request = new GetUsersByIdsRequest { UserIds = { uncachedUserIds } };
                var response = await _client.GetUsersByIdsAsync(request);

                if (response.Success)
                {
                    // Cache the new users
                    foreach (var user in response.Data)
                    {
                        _cache.Set($"user_{user.Id}", user, _cacheOptions);
                    }

                    // Combine cached and newly fetched users
                    return new GetUsersByIdsResponse
                    {
                        Success = true,
                        Data = { cachedUsers.Concat(response.Data) }
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                return new GetUsersByIdsResponse
                {
                    Success = false,
                    Errors = { $"gRPC call failed: {ex.Message}" }
                };
            }
        }
    }
}
