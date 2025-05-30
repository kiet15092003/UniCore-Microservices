using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using UserService.Entities;
using System.Text.Json.Serialization;

namespace UserService.Entities
{
    public class Lecturer : BaseEntity
    {
        [Required]
        public string LecturerCode { get; set; }

        [Required]
        public string Degree { get; set; }

        [Required]
        public decimal Salary { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public Guid DepartmentId { get; set; }

        [Required]
        public int WorkingStatus { get; set; }

        [Required]
        public DateTime JoinDate { get; set; }

        public string MainMajor { get; set; }

    }
}
