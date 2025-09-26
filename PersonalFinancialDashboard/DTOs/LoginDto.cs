namespace PersonalFinancialDashboard.DTOs
{
    public class LoginDto
    {

        public string Message { get; set; }

        public CookieOptions CookieOptions { get; set; }

        public string Token { get; set; }

        public bool IsOnboarded { get; set; }


        public LoginDto(string v1, CookieOptions value, string v2, bool v3)
        {
            this.Message = v1;
            this.CookieOptions = value;
            this.Token = v2;
            this.IsOnboarded = v3;
        }

        
    }
}
