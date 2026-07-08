namespace REVAACOURSES.Models
{
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }
    public class Payment
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public double Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
        public int courseId { get; set; }
        public Course course { get; set; }
    }
}
