namespace PersonalFinancialDashboard.DTOs
{
    public class ProfileDto
    {
       public double SalaryPerMonth { get; set; }
       public double CurrentBalance { get; set; }
       public int Id { get; set; } 
       public int UserId { get; set; }
       public string Username { get; set; }

       public bool IsAlreadyExists { get; set; }

        public ProfileDto(double salary, double balance, int id, int userId, string username, bool isExist) { 
            
            SalaryPerMonth = salary;
            CurrentBalance = balance;
            Id = id;
            UserId = userId;
            Username = username;
            IsAlreadyExists = isExist;
            
        }
    }
}
