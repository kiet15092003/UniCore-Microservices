using Grpc.Core;
using CourseService.Business.Services;
using Google.Protobuf.WellKnownTypes;
using System.Globalization;

namespace CourseService.CommunicationTypes.Grpc.GrpcServer
{
    public class GrpcAcademicClassService : GrpcAcademicClass.GrpcAcademicClassBase
    {
        private readonly IAcademicClassService _academicClassService;

        public GrpcAcademicClassService(IAcademicClassService academicClassService)
        {
            _academicClassService = academicClassService;
        }       
        public override async Task<AcademicClassResponse> GetAcademicClassById(AcademicClassRequest request, ServerCallContext context)
        {
            try
            {
                // Validate GUID format
                if (!Guid.TryParse(request.Id, out Guid academicClassId))
                {
                    return new AcademicClassResponse
                    {
                        Success = false,
                        Error = { "Invalid Academic Class ID format." }
                    };
                }

                // Get academic class from business service
                var academicClassDto = await _academicClassService.GetAcademicClassByIdAsync(academicClassId);

                // Create response
                var response = new AcademicClassResponse
                {
                    Success = true,
                    Data = new AcademicClassData
                    {
                        Id = academicClassDto.Id.ToString(),
                        Name = academicClassDto.Name,
                        GroupNumber = academicClassDto.GroupNumber,
                        Capacity = academicClassDto.Capacity,
                        IsRegistrable = academicClassDto.IsRegistrable,
                        SemesterId = academicClassDto.SemesterId.ToString(),
                        CourseId = academicClassDto.CourseId.ToString()
                    }
                };

                // Add list of weeks
                if (academicClassDto.ListOfWeeks != null)
                {
                    response.Data.ListOfWeeks.AddRange(academicClassDto.ListOfWeeks);
                }

                // Add Course data
                if (academicClassDto.Course != null)
                {
                    response.Data.Course = new CourseData
                    {
                        Id = academicClassDto.Course.Id.ToString(),
                        Code = academicClassDto.Course.Code,
                        Name = academicClassDto.Course.Name,
                        Description = academicClassDto.Course.Description,
                        IsActive = academicClassDto.Course.IsActive,
                        Credit = academicClassDto.Course.Credit,
                        PracticePeriod = academicClassDto.Course.PracticePeriod,
                        IsRequired = academicClassDto.Course.IsRequired,
                        Cost = academicClassDto.Course.Cost
                    };
                }

                // Add Semester data
                if (academicClassDto.Semester != null)
                {
                    response.Data.Semester = new SemesterData
                    {
                        Id = academicClassDto.Semester.Id.ToString(),
                        SemesterNumber = academicClassDto.Semester.SemesterNumber,
                        Year = academicClassDto.Semester.Year,
                        IsActive = academicClassDto.Semester.IsActive,
                        StartDate = academicClassDto.Semester.StartDate.ToString("yyyy-MM-dd"),
                        EndDate = academicClassDto.Semester.EndDate.ToString("yyyy-MM-dd"),
                        NumberOfWeeks = academicClassDto.Semester.NumberOfWeeks
                    };
                }

                // Add ScheduleInDays data
                if (academicClassDto.ScheduleInDays != null && academicClassDto.ScheduleInDays.Any())
                {
                    foreach (var schedule in academicClassDto.ScheduleInDays)
                    {
                        var scheduleData = new ScheduleInDayData
                        {
                            Id = schedule.Id.ToString(),
                            DayOfWeek = schedule.DayOfWeek,
                            RoomId = schedule.RoomId.ToString(),
                            ShiftId = schedule.ShiftId.ToString()
                        };

                        // Add Shift data
                        if (schedule.Shift != null)
                        {
                            scheduleData.Shift = new ShiftData
                            {
                                Id = schedule.Shift.Id.ToString(),
                                Name = schedule.Shift.Name,
                                StartTime = schedule.Shift.StartTime.ToString(@"hh\:mm"),
                                EndTime = schedule.Shift.EndTime.ToString(@"hh\:mm")
                            };
                        }

                        // Add Room data if available
                        if (schedule.Room != null)
                        {
                            scheduleData.Room = schedule.Room;
                        }
                        response.Data.ScheduleInDays.Add(scheduleData);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                return new AcademicClassResponse
                {
                    Success = false,
                    Error = { ex.Message }
                };
            }
        }
    }
}
