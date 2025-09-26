namespace PersonalFinancialDashboard.DTOs
{
    public class ExpenseSummaryDto
    {
        public int Id {  get; set; }
        public string Category {get; set;}
        public int ExpenseYear {get; set;}
        public int ExpenseMonth { get; set; }
        public double CapAmount { get; set; }
        public double TotalAmount { get; set; }
        
        public string IsOverBudget { get; set; }
    }
}
