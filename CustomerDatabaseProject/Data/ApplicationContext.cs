using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDatabaseProject.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderRow> OrderRows => Set<OrderRow>();
        public DbSet<Product> Products => Set<Product>();

        public DbSet<CustomerView> CustomerViews => Set<CustomerView>();
        public DbSet<OrderView> OrderViews => Set<OrderView>();
        public DbSet<ProductView> ProductViews => Set<ProductView>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppContext.BaseDirectory, "shop.db");
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerView>(cv =>
            {
                cv.HasNoKey();
                cv.ToView("OrdersMadeView");
            });
            modelBuilder.Entity<Customer>(c =>
            {
                c.HasKey(x => x.CustomerId);

                c.Property(x => x.Name).IsRequired().HasMaxLength(100);
                c.Property(x => x.Email).IsRequired().HasMaxLength(100);
                c.Property(x => x.City).IsRequired().HasMaxLength(100);
                c.Property(x => x.Password).IsRequired().HasMaxLength(100);

                c.HasIndex(x => x.CustomerId).IsUnique();

                c.HasMany(x => x.Orders).WithOne(x => x.Customer).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderView>(ov =>
            {
                ov.HasNoKey();
                ov.ToView("OrderSummaryView");
            });
            modelBuilder.Entity<Order>(o =>
            {
                o.HasKey(x => x.OrderId);

                o.Property(x => x.OrderDate).IsRequired();
                o.Property(x => x.Status).IsRequired().HasMaxLength(100);
                o.Property(x => x.TotalAmount).IsRequired();

                o.HasIndex(x => x.OrderId).IsUnique();

                o.HasOne(x => x.Customer).WithMany(x => x.Orders).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
                o.HasMany(x => x.OrderRows).WithOne(x => x.Order).HasForeignKey(x => x.OrderRowId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<OrderRow>(orw =>
            {
                orw.HasKey(x => x.OrderRowId);

                orw.Property(x => x.UnitPrice).IsRequired();
                orw.Property(x => x.Quantity).IsRequired();

                orw.HasIndex(x => x.OrderRowId).IsUnique();

                orw.HasOne(x => x.Order).WithMany(x => x.OrderRows).HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Restrict);
                orw.HasOne(x => x.Product).WithMany(x => x.OrderRows).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ProductView>(pv =>
            {
                pv.HasNoKey();
                pv.ToView("ProductsSoldView");
            });
            modelBuilder.Entity<Product>(p =>
            {
                p.HasKey(x => x.ProductId);

                p.Property(x => x.ProductName).IsRequired().HasMaxLength(100);
                p.Property(x => x.UnitPrice).IsRequired();

                p.HasIndex(x => x.ProductId).IsUnique();

                p.HasMany(x => x.OrderRows).WithOne(x => x.Product).OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Order>().HasIndex(o => o.OrderDate);
            modelBuilder.Entity<Order>().HasIndex(o => o.CustomerId);
        }
    }
}
