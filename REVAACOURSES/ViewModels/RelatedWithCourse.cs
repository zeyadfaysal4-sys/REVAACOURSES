using REVAACOURSES.Models;
using NuGet.Protocol.Core.Types;

namespace REVAACOURSES.ViewModels
{
    public class RelatedWithCourse
    {
        public Course Course { get; set; }
        public List<Course> RelatedCourses { get; set; }
    }
}
