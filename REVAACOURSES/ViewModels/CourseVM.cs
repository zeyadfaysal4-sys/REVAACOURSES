using REVAACOURSES.Models;

namespace REVAACOURSES.ViewModels
{
    public class CourseVM
    {
        public Course? Course { get; set; }
        public List<Category> Categories { get; set; }

    }
}
