using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using REVAACOURSES.Models;

namespace REVAACOURSES.Data
{
    public class ApplicationDbContext :IdentityDbContext<ApplicationUser>
    {
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<ApplicationUserOTP> ApplicationUserOTPs { get; set; }
        public DbSet<Assistant> Assistants { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Quiez> Quiezs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

    }
}
