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
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

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

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Checking academic classes for registration time...");
                
                try
                {
                    await CheckAndUpdateRegistrableClasses(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking registration times");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }        
        private async Task CheckAndUpdateRegistrableClasses(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var academicClassRepository = scope.ServiceProvider.GetRequiredService<IAcademicClassRepository>();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            
            var currentTime = DateTime.UtcNow;
            
            // Find classes where:
            // 1. IsRegistrable is false AND RegistrationOpenTime has passed
            var classesToOpen = await academicClassRepository.GetQuery()
                .Where(ac => !ac.IsRegistrable && ac.RegistrationOpenTime <= currentTime)
                .ToListAsync(stoppingToken);
                  // Find classes where: 
            // 1. IsRegistrable is true AND RegistrationCloseTime has passed
            var classesToClose = await academicClassRepository.GetQuery()
                .Where(ac => ac.IsRegistrable && ac.RegistrationCloseTime.HasValue && ac.RegistrationCloseTime.Value <= currentTime)
                .ToListAsync(stoppingToken);

            bool anyChanges = false;

            // Open registrations for classes that reached their opening time
            if (classesToOpen.Any())
            {
                foreach (var academicClass in classesToOpen)
                {
                    academicClass.IsRegistrable = true;
                    _logger.LogInformation("Opening registration for academic class: {AcademicClassName} (ID: {AcademicClassId})", 
                        academicClass.Name, academicClass.Id);
                }
                anyChanges = true;
            }

            // Close registrations for classes that reached their closing time
            if (classesToClose.Any())
            {
                foreach (var academicClass in classesToClose)
                {
                    academicClass.IsRegistrable = false;
                    _logger.LogInformation("Closing registration for academic class: {AcademicClassName} (ID: {AcademicClassId})", 
                        academicClass.Name, academicClass.Id);
                }
                anyChanges = true;
            }            if (anyChanges)
            {
                // Save all changes at once
                await academicClassRepository.SaveChangesAsync();
                
                _logger.LogInformation("Updated registration status for {OpenCount} classes (opened) and {CloseCount} classes (closed)",
                    classesToOpen.Count, classesToClose.Count);
            }
            else
            {
                _logger.LogInformation("No academic classes need updating at this time.");
            }
        }
    }
}
