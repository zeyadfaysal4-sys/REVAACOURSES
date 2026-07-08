using REVAACOURSES.Models;

namespace REVAACOURSES.ViewModels
{
    public class FilterCourseVM
    {
        public string? Title { get; set; }
        public double? Price { get; set; }
        public int? CategoryId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int Page { get; set; } = 1;
    }
}
