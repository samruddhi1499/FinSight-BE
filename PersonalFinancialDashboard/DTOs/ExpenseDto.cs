namespace PersonalFinancialDashboard.DTOs
{
    public class ExpenseDto
    {
        public int ExpenseCategoriesId { get; set; }

        public DateOnly ExpenseDate { get; set; }

        public float Amount { get; set; }
    }
}
