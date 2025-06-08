using AutoMapper;
using CourseService.Business.Dtos.Semester;
using CourseService.DataAccess.Repositories;
using CourseService.Entities;
using CourseService.Utils.Filter;
using CourseService.Utils.Pagination;

namespace CourseService.Business.Services
{
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _semesterRepository;
        private readonly IMapper _mapper;

        public SemesterService(
            ISemesterRepository semesterRepository,
            IMapper mapper)
        {
            _semesterRepository = semesterRepository;
            _mapper = mapper;
        }

        public async Task<SemesterReadDto> CreateSemesterAsync(SemesterCreateDto semesterCreateDto)
        {
            // Check if a semester with the same number and year already exists
            var exists = await _semesterRepository.SemesterExistsAsync(
                semesterCreateDto.SemesterNumber,
                semesterCreateDto.Year);

            if (exists)
            {
                throw new InvalidOperationException($"A semester with number {semesterCreateDto.SemesterNumber} and year {semesterCreateDto.Year} already exists.");
            }

            var semester = _mapper.Map<Semester>(semesterCreateDto);
            semester.CreatedAt = DateTime.Now;
            semester.UpdatedAt = DateTime.Now;
            semester.IsActive = true;

            var createdSemester = await _semesterRepository.CreateSemesterAsync(semester);
            return _mapper.Map<SemesterReadDto>(createdSemester);
        }

        public async Task<SemesterReadDto> DeactivateSemesterAsync(Guid id)
        {
            var semester = await _semesterRepository.GetSemesterByIdAsync(id);
            if (semester == null)
            {
                throw new KeyNotFoundException("Semester not found");
            }

            semester.IsActive = false;
            semester.UpdatedAt = DateTime.Now;
            
            var updatedSemester = await _semesterRepository.UpdateSemesterAsync(semester);
            return _mapper.Map<SemesterReadDto>(updatedSemester);
        }

        public async Task<SemesterReadDto> ActivateSemesterAsync(Guid id)
        {
            var semester = await _semesterRepository.GetSemesterByIdAsync(id);
            if (semester == null)
            {
                throw new KeyNotFoundException("Semester not found");
            }

            semester.IsActive = true;
            semester.UpdatedAt = DateTime.Now;
            
            var updatedSemester = await _semesterRepository.UpdateSemesterAsync(semester);
            return _mapper.Map<SemesterReadDto>(updatedSemester);
        }

        public async Task<PaginationResult<SemesterReadDto>> GetByPaginationAsync(Pagination pagination, SemesterFilterParams filter, Order? order)
        {
            var result = await _semesterRepository.GetAllSemestersPaginationAsync(pagination, filter, order);
            return _mapper.Map<PaginationResult<SemesterReadDto>>(result);
        }

        public async Task<SemesterReadDto> GetSemesterByIdAsync(Guid id)
        {
            var semester = await _semesterRepository.GetSemesterByIdAsync(id);
            if (semester == null)
            {
                throw new KeyNotFoundException("Semester not found");
            }
            
            return _mapper.Map<SemesterReadDto>(semester);
        }

        public async Task<SemesterReadDto> UpdateSemesterAsync(Guid id, SemesterUpdateDto semesterUpdateDto)
        {
            var semester = await _semesterRepository.GetSemesterByIdAsync(id);
            if (semester == null)
            {
                throw new KeyNotFoundException("Semester not found");
            }

            // Check if another semester with the same number and year exists
            var exists = await _semesterRepository.SemesterExistsAsync(
                semesterUpdateDto.SemesterNumber,
                semesterUpdateDto.Year,
                id);

            if (exists)
            {
                throw new InvalidOperationException($"Another semester with number {semesterUpdateDto.SemesterNumber} and year {semesterUpdateDto.Year} already exists.");
            }

            _mapper.Map(semesterUpdateDto, semester);
            semester.UpdatedAt = DateTime.Now;
            
            var updatedSemester = await _semesterRepository.UpdateSemesterAsync(semester);
            return _mapper.Map<SemesterReadDto>(updatedSemester);
        }
    }
}
