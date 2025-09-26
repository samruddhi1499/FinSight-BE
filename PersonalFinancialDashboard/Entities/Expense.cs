namespace PersonalFinancialDashboard.Entities
{
    public class Expense
    {
        public int Id { get; set; }

        public int ExpenseCategoriesId { get; set; }

        public ExpenseCategories ExpenseCategories { get; set; }

        public double Amount { get; set; }

        public DateOnly ExpenseDate {  get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
