using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShoppingCart.Domain;

namespace ShoppingCart.Application;

public interface ICommand<TResult> { }
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult> { Task<TResult> Handle(TCommand command, CancellationToken cancellationToken); }
public interface IProductRepository { Task<Product?> Get(Guid id, CancellationToken ct); Task<IReadOnlyList<Product>> List(CancellationToken ct); Task Add(Product product, CancellationToken ct); }
public interface ICartRepository { Task<Cart> GetOrCreate(Guid userId, CancellationToken ct); Task Save(Cart cart, CancellationToken ct); }
public interface IOrderRepository { Task Add(Order order, CancellationToken ct); Task<IReadOnlyList<Order>> List(Guid userId, CancellationToken ct); }
public interface IUnitOfWork { Task<int> SaveChanges(CancellationToken ct); }
public record ProductDto(Guid Id, string Name, string Description, decimal Price, int Stock);
public record CartItemDto(Guid ProductId, int Quantity, decimal UnitPrice);
public record CartDto(Guid Id, IReadOnlyList<CartItemDto> Items, decimal Total);
public record OrderDto(Guid Id, decimal Total, OrderStatus Status, DateTime CreatedUtc, IReadOnlyList<CartItemDto> Items);
