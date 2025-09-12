namespace PersonalFinancialDashboard.Entities
{
    public class RecurringCategories
    {
        public int ExpenseCategoriesId{  get; set; }

        public double CapAmount { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        public ExpenseCategories ExpenseCategories { get; set; }
    }
}
