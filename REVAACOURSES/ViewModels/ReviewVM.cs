using System.ComponentModel.DataAnnotations;

namespace REVAACOURSES.ViewModels
{
    public class ReviewVM
    {
        public int CourseId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }
    }
}
