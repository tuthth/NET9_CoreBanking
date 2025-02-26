using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class AppContext : DbContext
    {
        public AppContext(DbContextOptions options) : base(options)
        {
        }

        public AppContext()
        {
        }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
        private string GetConnectionString()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .SetBasePath(Directory.GetCurrentDirectory()).Build();
            return config.GetConnectionString("DefaultConnection");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(GetConnectionString());
            //optionsBuilder.UseSqlServer("Server=(local);uid=sa;pwd=12345;Database=CoreBanking;Encrypt=false;TrustServerCertificate=true;");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); 
           
            modelBuilder.Entity<BankAccount>()
                .HasIndex(b => b.AccountNumber)
                .IsUnique(); 

            modelBuilder.Entity<BankAccount>()
                .HasOne(b => b.UserNavigation)
                .WithMany(u => u.BankAccountsNavigation)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade); 

            
            modelBuilder.Entity<Transaction>()
                .HasIndex(t => t.CreatedAt); 

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.BankAccountNavigation)
                .WithMany()
                .HasForeignKey(t => t.BankAccountId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.RelatedAccountNavigation)
                .WithMany()
                .HasForeignKey(t => t.RelatedBankAccountId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}
