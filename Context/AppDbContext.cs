
using Controle_Pessoal.Entities;
using Microsoft.EntityFrameworkCore;

namespace Controle_Pessoal.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           base.OnModelCreating(modelBuilder);

           modelBuilder.Entity<Expense>()
            .Property(x => x.Description).IsRequired();

            modelBuilder.Entity<Expense>()
            .Property(x => x.Date)
            .HasColumnType("date")
            .IsRequired();
           
           modelBuilder.Entity<Expense>()
            .HasOne(a => a.User)
            .WithMany(b => b.Expenses)
            .HasForeignKey(b => b.UserId);

           modelBuilder.Entity<Expense>()
            .HasOne(a => a.Category)
            .WithMany(b => b.Expenses)
            .HasForeignKey(b => b.CategoryId);
        }
    }
}
