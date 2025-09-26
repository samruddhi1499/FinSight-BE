namespace PersonalFinancialDashboard.DTOs
{
    public class RecurringCatMessageDto
    {
        public bool Val {  get; set; }

        public string Message { get; set; }

        public RecurringCatMessageDto( string message, bool val) {
            
            Val = val;
            Message = message;
        }
    }
}
