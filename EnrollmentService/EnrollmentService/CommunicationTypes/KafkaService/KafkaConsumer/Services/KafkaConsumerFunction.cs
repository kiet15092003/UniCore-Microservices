using EnrollmentService.CommunicationTypes.KafkaService.KafkaConsumer.Templates;
using EnrollmentService.DataAccess.Repositories;
using Microsoft.Extensions.Logging;

namespace EnrollmentService.CommunicationTypes.KafkaService.KafkaConsumer.Services
{
    public class KafkaConsumerFunction : IKafkaConsumerFunction
    {
        private readonly ILogger<KafkaConsumerFunction> _logger;
        private readonly IEnrollmentRepository _enrollmentRepository;

        public KafkaConsumerFunction(
            ILogger<KafkaConsumerFunction> logger,
            IEnrollmentRepository enrollmentRepository)
        {
            _logger = logger;
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<bool> HandleClassClosureAsync(ClassClosureEventData classClosureData)
        {
            try
            {
                _logger.LogInformation("Processing class closure event from CourseService");
                
                if (classClosureData?.Data?.ClassIds == null || !classClosureData.Data.ClassIds.Any())
                {
                    _logger.LogWarning("No class IDs provided in the ClassClosureEventData");
                    return false;
                }

                // Update enrollment status from 1 (active) to 2 (closed) for all enrollments in the closed classes
                var updatedCount = await _enrollmentRepository.UpdateEnrollmentStatusByClassIdsAsync(
                    classClosureData.Data.ClassIds, 
                    fromStatus: 1, 
                    toStatus: 2);

                _logger.LogInformation($"Successfully updated {updatedCount} enrollments to closed status for {classClosureData.Data.ClassIds.Count} closed classes");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing class closure event");
                return false;
            }
        }
    }
}
