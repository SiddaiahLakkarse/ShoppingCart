using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Application;
using ShoppingCart.Domain;

namespace ShoppingCart.Infrastructure;

public sealed class ProductRepository : IProductRepository
{
    private readonly ShoppingCartDbContext db; public ProductRepository(ShoppingCartDbContext db) => this.db = db;
    public Task<Product?> Get(Guid id, CancellationToken ct) => db.Products.SingleOrDefaultAsync(x => x.Id == id, ct);
    public async Task<IReadOnlyList<Product>> List(CancellationToken ct) => await db.Products.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Name).ToListAsync(ct);
    public Task Add(Product product, CancellationToken ct) => db.Products.AddAsync(product, ct).AsTask();
}
public sealed class CartRepository : ICartRepository
{
    private readonly ShoppingCartDbContext db; public CartRepository(ShoppingCartDbContext db) => this.db = db;
    public async Task<Cart> GetOrCreate(Guid userId, CancellationToken ct) => await db.Carts.Include(x => x.Items).SingleOrDefaultAsync(x => x.UserId == userId, ct) ?? new Cart(userId);
    public Task Save(Cart cart, CancellationToken ct) { if (db.Entry(cart).State == EntityState.Detached) db.Carts.Add(cart); return Task.CompletedTask; }
}
public sealed class OrderRepository : IOrderRepository
{
    private readonly ShoppingCartDbContext db; public OrderRepository(ShoppingCartDbContext db) => this.db = db;
    public Task Add(Order order, CancellationToken ct) => db.Orders.AddAsync(order, ct).AsTask();
    public async Task<IReadOnlyList<Order>> List(Guid userId, CancellationToken ct) => await db.Orders.AsNoTracking().Include(x => x.Items).Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedUtc).ToListAsync(ct);
}
public sealed class UnitOfWork : IUnitOfWork { private readonly ShoppingCartDbContext db; public UnitOfWork(ShoppingCartDbContext db) => this.db = db; public Task<int> SaveChanges(CancellationToken ct) => db.SaveChangesAsync(ct); }
