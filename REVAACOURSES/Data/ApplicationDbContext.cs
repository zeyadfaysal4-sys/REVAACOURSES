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
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Question> Questions { get; set; }

        public DbSet<StudentProgress> StudentProgresses { get; set; }

        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Instructor>()
                .HasOne(i => i.User)
                .WithOne(u => u.Instructor)
                .HasForeignKey<Instructor>(i => i.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Assistant>()
                .HasOne(a => a.User)
                .WithOne(u => u.Assistant)
                .HasForeignKey<Assistant>(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(i => i.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
