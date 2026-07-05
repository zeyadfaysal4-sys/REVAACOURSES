using System.ComponentModel.DataAnnotations;

namespace REVAACOURSES.ViewModels
{
    public class NewPasswordPVM
    {
        public  int Id { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
        public string UserId { get; set; }

        public string Token { get; set; }

    }
}
