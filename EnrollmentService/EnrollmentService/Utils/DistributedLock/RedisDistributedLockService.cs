using StackExchange.Redis;

namespace EnrollmentService.Utils.DistributedLock
{
    public interface IDistributedLockService
    {
        Task<IDistributedLock?> AcquireLockAsync(string lockKey, TimeSpan expiration, TimeSpan timeout);
    }

    public interface IDistributedLock : IDisposable
    {
        string LockKey { get; }
    }
    public class RedisDistributedLockService : IDistributedLockService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisDistributedLockService> _logger;

        public RedisDistributedLockService(IConnectionMultiplexer redis, ILogger<RedisDistributedLockService> logger)
        {
            _redis = redis;
            _logger = logger;
        }

        public async Task<IDistributedLock?> AcquireLockAsync(string lockKey, TimeSpan expiration, TimeSpan timeout)
        {
            try
            {
                if (!_redis.IsConnected)
                {
                    _logger.LogWarning("Redis is not connected. Unable to acquire lock for key: {LockKey}", lockKey);
                    return null;
                }

                var database = _redis.GetDatabase();
                var lockValue = Guid.NewGuid().ToString();
                var startTime = DateTime.UtcNow;

                while (DateTime.UtcNow - startTime < timeout)
                {
                    var acquired = await database.StringSetAsync(lockKey, lockValue, expiration, When.NotExists);
                    if (acquired)
                    {
                        _logger.LogDebug("Lock acquired for key: {LockKey}", lockKey);
                        return new RedisDistributedLock(database, lockKey, lockValue, _logger);
                    }

                    // Wait before retry
                    await Task.Delay(50);
                }

                _logger.LogWarning("Failed to acquire lock for key: {LockKey} within timeout: {Timeout}", lockKey, timeout);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error acquiring lock for key: {LockKey}", lockKey);
                return null;
            }
        }
    }

    public class RedisDistributedLock : IDistributedLock
    {
        private readonly IDatabase _database;
        private readonly string _lockValue;
        private readonly ILogger _logger;
        private bool _disposed = false;

        public string LockKey { get; }

        public RedisDistributedLock(IDatabase database, string lockKey, string lockValue, ILogger logger)
        {
            _database = database;
            LockKey = lockKey;
            _lockValue = lockValue;
            _logger = logger;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                try
                {
                    // Use Lua script to ensure atomic check and delete
                    const string script = @"
                        if redis.call('GET', KEYS[1]) == ARGV[1] then
                            return redis.call('DEL', KEYS[1])
                        else
                            return 0
                        end";

                    _database.ScriptEvaluate(script, new RedisKey[] { LockKey }, new RedisValue[] { _lockValue });
                    _logger.LogDebug("Lock released for key: {LockKey}", LockKey);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error releasing lock for key: {LockKey}", LockKey);
                }
                finally
                {
                    _disposed = true;
                }
            }
        }
    }

    // Fallback implementation when Redis is not available
    public class FallbackDistributedLockService : IDistributedLockService
    {
        private readonly ILogger<FallbackDistributedLockService> _logger;

        public FallbackDistributedLockService(ILogger<FallbackDistributedLockService> logger)
        {
            _logger = logger;
        }

        public Task<IDistributedLock?> AcquireLockAsync(string lockKey, TimeSpan expiration, TimeSpan timeout)
        {
            _logger.LogWarning("Using fallback lock service for key: {LockKey}. Redis not available.", lockKey);
            // Return a dummy lock that always succeeds
            return Task.FromResult<IDistributedLock?>(new FallbackDistributedLock(lockKey, _logger));
        }
    }

    public class FallbackDistributedLock : IDistributedLock
    {
        private readonly ILogger _logger;
        private bool _disposed = false;

        public string LockKey { get; }

        public FallbackDistributedLock(string lockKey, ILogger logger)
        {
            LockKey = lockKey;
            _logger = logger;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _logger.LogDebug("Fallback lock released for key: {LockKey}", LockKey);
                _disposed = true;
            }
        }
    }
}
