using System.ComponentModel.DataAnnotations;

namespace REVAACOURSES.ViewModels
{
    public class LoginVM
    {
        public int Id { get; set; }
        public string UserNameOrEmail { get; set; }
        [DataType(DataType.Password)]

        public string Password { get; set; }
        public bool RememberMe { get; set; }

    }
}
