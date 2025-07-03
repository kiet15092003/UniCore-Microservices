using System.ComponentModel.DataAnnotations;

namespace EnrollmentService.Business.Dtos.StudentResult
{
    public class ImportScoreDto
    {
        [Required]
        public Guid ClassId { get; set; }
        
        [Required]
        public IFormFile ExcelFile { get; set; }
    }

    public class ImportScoreResultDto
    {
        public int TotalRows { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<ImportScoreError> Errors { get; set; } = new List<ImportScoreError>();
        public List<ImportScoreSuccess> Successes { get; set; } = new List<ImportScoreSuccess>();
    }

    public class ImportScoreError
    {
        public int RowNumber { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string ScoreTypeName { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class ImportScoreSuccess
    {
        public int RowNumber { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string ScoreTypeName { get; set; } = string.Empty;
        public double Score { get; set; }
        public Guid StudentResultId { get; set; }
    }

    public class ExcelScoreRow
    {
        public string StudentCode { get; set; } = string.Empty;
        public double? TheoryScore { get; set; }      // Score Type 1
        public double? PracticeScore { get; set; }    // Score Type 2
        public double? MidtermScore { get; set; }     // Score Type 3
        public double? FinalScore { get; set; }       // Score Type 4
    }

    public class UpdateScoreBatchDto
    {
        public Guid ClassId { get; set; }
        public List<UpdateScoreItemDto> Scores { get; set; } = new List<UpdateScoreItemDto>();
    }

    public class UpdateScoreItemDto
    {
        public string StudentCode { get; set; } = string.Empty;
        public double? TheoryScore { get; set; }      // Score Type 1
        public double? PracticeScore { get; set; }    // Score Type 2
        public double? MidtermScore { get; set; }     // Score Type 3
        public double? FinalScore { get; set; }       // Score Type 4
    }

    public class UpdateScoreBatchSimpleDto
    {
        public List<UpdateScoreItemDto> Scores { get; set; } = new List<UpdateScoreItemDto>();
    }
} 