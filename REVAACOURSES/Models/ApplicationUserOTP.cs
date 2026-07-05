namespace REVAACOURSES.Models
{
    public class ApplicationUserOTP
    {
        public string Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string OTP { get; set; }
        public bool IsValied { get; set; }
        public DateTime ValiedTo { get; set; }
        public DateTime CreatedAt { get; set; }

        public ApplicationUserOTP()
        {

        }
        public ApplicationUserOTP(string ApplicationUserId, string otp)
        {
            this.ApplicationUserId = ApplicationUserId;
            OTP = otp;
            Id = Guid.NewGuid().ToString();
            IsValied = true;
            ValiedTo = DateTime.UtcNow.AddMinutes(30);
            CreatedAt = DateTime.UtcNow;
        }

    }
}
