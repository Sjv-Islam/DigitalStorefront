using DigitalStorefront.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DigitalStorefront.Data
{
    public class DSDbContext : DbContext
    {
        public DSDbContext(DbContextOptions<DSDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductDetail> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProductDetail>()
                .Property(pd => pd.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProductDetail>()
                .Property(pd => pd.TotalPrice)
                .HasColumnType("decimal(18,2)");
        }
    }
}
