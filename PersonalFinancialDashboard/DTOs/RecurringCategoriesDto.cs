namespace PersonalFinancialDashboard.DTOs
{
    public class RecurringCategoriesDto
    {
        public int? CategoryId { get; set; }
        public string Category { get; set; }

        public double CapAmount { get; set; }

        public RecurringCategoriesDto() { }

        public RecurringCategoriesDto(int? categoryId, string category, double capAmount)
        {
            CategoryId = categoryId;
            Category = category;
            CapAmount = capAmount;
        }

        public RecurringCategoriesDto (string category, double capAmount)
        {
            Category = category;
            CapAmount = capAmount;
        }
    }
}
