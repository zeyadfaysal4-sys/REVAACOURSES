using REVAACOURSES.Models;

namespace REVAACOURSES.ViewModels
{
    public class DashboardVM
    {
        public int Course { get; set; }
        public int User { get; set; }
        public int Categories { get; set; }
        public int Payment { get; set; }
        public double TotalPrice { get; set; }
    }
}
