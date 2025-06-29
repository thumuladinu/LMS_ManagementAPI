using System.ComponentModel.DataAnnotations;

namespace LMS_ManagementAPI.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required]
        public string CourseName { get; set; }

        [Required]
        public string Instructor { get; set; }

        [Range(1, 50)]
        public int DurationWeeks { get; set; }
    }
}
