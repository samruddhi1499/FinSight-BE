using Microsoft.EntityFrameworkCore;

namespace PersonalFinancialDashboard.Entities
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }

        public DbSet<RecurringCategories> RecurringCategories { get; set; }

        public DbSet<ExpenseCategories> ExpenseCategories { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Optional fallback connection or leave empty
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.Id)
                      .ValueGeneratedOnAdd()
                      .HasColumnName("id");
                entity.Property(e => e.Username).HasColumnName("username");
                entity.Property(e => e.PasswordHash)
                    .HasColumnName("password_hash");
               
            });
            modelBuilder.Entity<UserDetails>(entity =>
            {
                entity.ToTable("UserDetails");

                entity.Property(e => e.Id)
                      .ValueGeneratedOnAdd()
                      .HasColumnName("id");
                entity.Property(e => e.CurrentBalance).HasColumnName("current_balance");
                entity.Property(e => e.SalaryPerMonth)
                    .HasColumnName("salary_per_month");
                entity.Property(e => e.UserId)
                    .HasColumnName("userId");

            });
            modelBuilder.Entity<RecurringCategories>(entity =>
            {
                entity.ToTable("RecurringCategories");

                entity.Property(e => e.ExpenseCategoriesId).HasColumnName("ex_category_id");
                entity.Property(e => e.UserId).HasColumnName("userId");

                // Composite primary key
                entity.HasKey(e => new { e.ExpenseCategoriesId, e.UserId });
            });

               
                

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
