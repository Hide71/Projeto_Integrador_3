
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
           .HasOne(a => a.user)
           .WithMany(b => b.expenses)
           .HasForeignKey(b => b.userId);

           modelBuilder.Entity<Expense>()
           .HasOne(a => a.category)
           .WithMany(b => b.expenses)
           .HasForeignKey(b => b.categoryId);
           //modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
