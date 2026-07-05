using System.ComponentModel.DataAnnotations;

namespace REVAACOURSES.ViewModels
{
    public class ApplicationUserVM
    {
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }

        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }


    }
}
