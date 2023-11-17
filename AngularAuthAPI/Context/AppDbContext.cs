using AngularAuthAPI.Controllers;
using AngularAuthAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AngularAuthAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        } 

        public DbSet<User> Users { get; set; }
        public DbSet<UserSearchResult> UserSearchResults { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<getCustomerNotes> CustomerNotes { get; set; }
        public object AddCustomerNotes { get; internal set; }
        public DbSet<GDPRCustomer> GDPRCustomers { get; set; }

        public DbSet<PasswordResetRequest> PasswordResetRequest { get; set; }
       



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<UserSearchResult>().HasNoKey();
            modelBuilder.Entity<Customer>().ToTable("ASPNETUSERS");
            modelBuilder.Entity<getCustomerNotes>().ToTable("TBL_CUSTOMER_NOTES");
            modelBuilder.Entity<GDPRCustomer>().ToTable("TBL_GDPR_CUSTOMER");
            modelBuilder.Entity<PasswordResetRequest>().ToTable("USERS");
            

        }
    }
}
