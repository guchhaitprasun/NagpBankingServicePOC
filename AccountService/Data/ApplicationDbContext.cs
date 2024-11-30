using AccountService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option) : base(option) { }


        #region DB Sets
        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<TransactionModel> Transactions { get; set; }

        #endregion

        #region Model Configuration 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AccountModel>()
                .HasIndex(account => account.EmailAddress)
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}
