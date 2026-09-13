using System;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Domain;

namespace ShoppingCart.Infrastructure;

public sealed class ShoppingCartDbContext : DbContext
{
    public ShoppingCartDbContext(DbContextOptions<ShoppingCartDbContext> options) : base(options) { }
    public DbSet<Product> Products { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasKey(x => x.Id); modelBuilder.Entity<Product>().Property(x => x.Name).HasMaxLength(200).IsRequired(); modelBuilder.Entity<Product>().Property(x => x.Price).HasColumnType("decimal(18,2)"); modelBuilder.Entity<Product>().HasIndex(x => new { x.IsActive, x.Name });
        modelBuilder.Entity<Cart>().HasKey(x => x.Id); modelBuilder.Entity<Cart>().HasIndex(x => x.UserId).IsUnique(); modelBuilder.Entity<Cart>().HasMany(x => x.Items).WithOne().HasForeignKey("CartId").OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<CartItem>().HasKey(x => x.Id); modelBuilder.Entity<CartItem>().Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Order>().HasKey(x => x.Id); modelBuilder.Entity<Order>().Property(x => x.Total).HasColumnType("decimal(18,2)"); modelBuilder.Entity<Order>().HasIndex(x => new { x.UserId, x.CreatedUtc }); modelBuilder.Entity<Order>().HasMany(x => x.Items).WithOne().HasForeignKey("OrderId").OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<OrderItem>().HasKey(x => x.Id); modelBuilder.Entity<OrderItem>().Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
    }
}
