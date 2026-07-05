using Microsoft.AspNetCore.Identity;

namespace REVAACOURSES.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public Student Student { get; set; }
        public Instructor Instructor { get; set; }
        public Assistant Assistant { get; set; }

    }
}
