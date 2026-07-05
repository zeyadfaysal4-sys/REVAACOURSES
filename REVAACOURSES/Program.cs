using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using REVAACOURSES.Data;
using REVAACOURSES.Models;
using REVAACOURSES.Repositories;
using REVAACOURSES.Utiltes;
using REVAACOURSES.Utiltes.DbSeeder;
using Stripe;

namespace REVAACOURSES
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            var connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string"
            + "'DefaultConnection' not found.");


            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {

                options.UseSqlServer(connectionString);
            });

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;

            })
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();


            builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
            builder.Services.AddScoped<IRepository<Assistant>, Repository<Assistant>>();
            builder.Services.AddScoped<IRepository<Course>, Repository<Course>>();
            builder.Services.AddScoped<IRepository<Enrollment>, Repository<Enrollment>>();
            builder.Services.AddScoped<IRepository<Instructor>, Repository<Instructor>>();
            builder.Services.AddScoped<IRepository<Payment>, Repository<Payment>>();
            builder.Services.AddScoped<IRepository<Quiez>, Repository<Quiez>>();
            builder.Services.AddScoped<IRepository<Cart>, Repository<Cart>>();
            builder.Services.AddScoped<IRepository<ApplicationUserOTP>, Repository<ApplicationUserOTP>>();
            builder.Services.AddScoped<IRepository<Student>, Repository<Student>>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();

            builder.Services.ConfigureApplicationCookie(option =>
            {

                option.LoginPath = "/Identity/Account/Login";
                option.AccessDeniedPath = "/Identity/Account/AccessDenied";
                option.SlidingExpiration = true;

            });
            //builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

            var app = builder.Build();


            using (var scope = app.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                await initializer.InitializeAsync();
            }


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
