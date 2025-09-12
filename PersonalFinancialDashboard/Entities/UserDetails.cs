namespace PersonalFinancialDashboard.Entities
{
    public class UserDetails
    {
        public int Id { get; set; }
        public double CurrentBalance {  get; set; } 
        public double SalaryPerMonth { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
