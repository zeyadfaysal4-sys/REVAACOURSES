using System.ComponentModel.DataAnnotations;

namespace REVAACOURSES.ViewModels
{
    public class RegisterVM
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password),Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
