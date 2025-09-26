namespace PersonalFinancialDashboard.DTOs
{
    public class ExpenseDetailDto
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public double Amount { get; set; }
        public DateOnly ExpenseDate { get; set; }
    }
}
