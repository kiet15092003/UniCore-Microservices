using CourseService.DataAccess;
using CourseService.DataAccess.Repositories;
using CourseService.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseService.Services
{
    public class RegistrationTimeCheckService : BackgroundService
    {
        private readonly ILogger<RegistrationTimeCheckService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10);

        public RegistrationTimeCheckService(
            ILogger<RegistrationTimeCheckService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RegistrationTimeCheckService is starting.");
            Console.WriteLine($"[DEBUG] RegistrationTimeCheckService started at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checking academic classes for registration time...");
                Console.WriteLine($"[DEBUG] Starting registration time check at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

                try
                {
                    await CheckAndUpdateRegistrableClasses(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking registration times");
                    Console.WriteLine($"[DEBUG ERROR] Exception occurred at {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {ex.Message}");
                }

                Console.WriteLine($"[DEBUG] Waiting {_checkInterval.TotalSeconds} seconds before next check...");
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }        private async Task CheckAndUpdateRegistrableClasses(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var academicClassRepository = scope.ServiceProvider.GetRequiredService<IAcademicClassRepository>();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var currentTime = DateTime.UtcNow;
            Console.WriteLine($"[DEBUG] Current UTC time: {currentTime:yyyy-MM-dd HH:mm:ss}");            // Find classes to open: IsRegistrable is false AND RegistrationOpenTime has passed AND RegistrationOpenTime is valid
            // AND current time is still before close time (or no close time set)
            var classesToOpen = await academicClassRepository.GetQuery()
                .Where(ac => !ac.IsRegistrable && 
                           ac.RegistrationOpenTime.HasValue && 
                           ac.RegistrationOpenTime.Value <= currentTime &&
                           ac.RegistrationOpenTime.Value != DateTime.MinValue &&
                           (!ac.RegistrationCloseTime.HasValue || ac.RegistrationCloseTime.Value > currentTime)) // Don't open if already past close time
                .ToListAsync(stoppingToken);

            Console.WriteLine($"[DEBUG] Found {classesToOpen.Count} classes to open registration");
            
            // Find classes to close: IsRegistrable is true AND RegistrationCloseTime has passed AND RegistrationCloseTime is valid
            var classesToClose = await academicClassRepository.GetQuery()
                .Where(ac => ac.IsRegistrable && 
                           ac.RegistrationCloseTime.HasValue && 
                           ac.RegistrationCloseTime.Value <= currentTime &&
                           ac.RegistrationCloseTime.Value != DateTime.MinValue) // Ensure valid close time
                .ToListAsync(stoppingToken);

            Console.WriteLine($"[DEBUG] Found {classesToClose.Count} classes to close registration");

            int actualChanges = 0;

            // Open registrations for classes that reached their opening time
            if (classesToOpen.Any())
            {
                foreach (var academicClass in classesToOpen)
                {
                    // Double-check that we're actually changing the state
                    if (!academicClass.IsRegistrable)
                    {
                        academicClass.IsRegistrable = true;
                        actualChanges++;
                        _logger.LogInformation("Opening registration for academic class: {AcademicClassName} (ID: {AcademicClassId}) - Open time: {OpenTime}",
                            academicClass.Name, academicClass.Id, academicClass.RegistrationOpenTime);
                        Console.WriteLine($"[DEBUG] Opening registration for class: {academicClass.Name} (ID: {academicClass.Id}) - Open: {academicClass.RegistrationOpenTime:yyyy-MM-dd HH:mm:ss}, Close: {(academicClass.RegistrationCloseTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "No close time")}");
                    }
                    else
                    {
                        Console.WriteLine($"[DEBUG] Class {academicClass.Name} (ID: {academicClass.Id}) is already open, skipping");
                    }
                }
            }

            // Close registrations for classes that reached their closing time
            if (classesToClose.Any())
            {
                foreach (var academicClass in classesToClose)
                {
                    // Double-check that we're actually changing the state
                    if (academicClass.IsRegistrable)
                    {
                        academicClass.IsRegistrable = false;
                        actualChanges++;
                        _logger.LogInformation("Closing registration for academic class: {AcademicClassName} (ID: {AcademicClassId}) - Close time: {CloseTime}",
                            academicClass.Name, academicClass.Id, academicClass.RegistrationCloseTime);
                        Console.WriteLine($"[DEBUG] Closing registration for class: {academicClass.Name} (ID: {academicClass.Id}) - Close time: {academicClass.RegistrationCloseTime:yyyy-MM-dd HH:mm:ss}");
                    }
                    else
                    {
                        Console.WriteLine($"[DEBUG] Class {academicClass.Name} (ID: {academicClass.Id}) is already closed, skipping");
                    }
                }
            }
            
            if (actualChanges > 0)
            {
                // Save all changes at once
                await academicClassRepository.SaveChangesAsync();

                _logger.LogInformation("Updated registration status for {ActualChanges} classes (from {OpenCount} to open, {CloseCount} to close)",
                    actualChanges, classesToOpen.Count, classesToClose.Count);
                Console.WriteLine($"[DEBUG] Successfully made {actualChanges} actual changes to database");
            }
            else
            {
                _logger.LogInformation("No academic classes need updating at this time.");
                Console.WriteLine("[DEBUG] No classes need updating at this time");
            }
        }
    }
}
