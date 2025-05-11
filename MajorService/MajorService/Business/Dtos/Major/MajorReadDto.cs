namespace MajorService.Business.Dtos.Major
{
    public class MajorReadDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public double CostPerCredit { get; set; }
    }
}
