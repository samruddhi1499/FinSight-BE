namespace PersonalFinancialDashboard.DTOs
{
    public class CardDto
    {
        public CardDto(string v, double salaryPerMonth)
        {
            Title = v;
            Amount = salaryPerMonth;
        }

        public string Title { get; set; }
        public double Amount { get; set; }
 
    }
}
