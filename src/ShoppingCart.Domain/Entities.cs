using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Domain;

public enum OrderStatus { Pending, Paid, Cancelled }

public sealed class Product
{
    private Product() { }
    public Product(string name, string description, decimal price, int stock) { Id = Guid.NewGuid(); Name = name; Description = description; Price = price; Stock = stock; IsActive = true; }
    public Guid Id { get; private set; }
    public Guid CartId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int Stock { get; private set; }
    public bool IsActive { get; private set; }
    public void DecreaseStock(int quantity) { if (quantity <= 0 || quantity > Stock) throw new InvalidOperationException("Insufficient stock."); Stock -= quantity; }
}

public sealed class Cart
{
    private Cart() { }
    public Cart(Guid userId) { Id = Guid.NewGuid(); UserId = userId; Items = new List<CartItem>(); }
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public List<CartItem> Items { get; private set; }
    public void Add(Product product, int quantity) { if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity)); var item = Items.SingleOrDefault(x => x.ProductId == product.Id); if (item == null) Items.Add(new CartItem(product.Id, quantity, product.Price)); else item.Increase(quantity, product.Price); }
    public void Remove(Guid productId) => Items.RemoveAll(x => x.ProductId == productId);
    public decimal Total => Items.Sum(x => x.UnitPrice * x.Quantity);
}

public sealed class CartItem
{
    private CartItem() { }
    public CartItem(Guid productId, int quantity, decimal unitPrice) { ProductId = productId; Quantity = quantity; UnitPrice = unitPrice; }
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public void Increase(int quantity, decimal currentPrice) { Quantity += quantity; UnitPrice = currentPrice; }
}

public sealed class Order
{
    private Order() { }
    public Order(Guid userId, IEnumerable<CartItem> items) { Id = Guid.NewGuid(); UserId = userId; Status = OrderStatus.Pending; CreatedUtc = DateTime.UtcNow; Items = items.Select(x => new OrderItem(x.ProductId, x.Quantity, x.UnitPrice)).ToList(); Total = Items.Sum(x => x.UnitPrice * x.Quantity); }
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedUtc { get; private set; }
    public OrderStatus Status { get; private set; }
    public decimal Total { get; private set; }
    public List<OrderItem> Items { get; private set; }
    public void MarkPaid() => Status = OrderStatus.Paid;
}

public sealed class OrderItem
{
    private OrderItem() { }
    public OrderItem(Guid productId, int quantity, decimal unitPrice) { ProductId = productId; Quantity = quantity; UnitPrice = unitPrice; }
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
}
