using Microsoft.EntityFrameworkCore;
using Stripe;

namespace REVAACOURSES.Models
{
    [PrimaryKey(nameof(CourseId), nameof(ApplicationUserId))]

    public class Cart
    {
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public int? Count { get; set; }
        public double Price { get; set; }
    }
}
