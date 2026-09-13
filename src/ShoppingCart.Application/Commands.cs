using System;
using System.Collections.Generic;
using ShoppingCart.Domain;

namespace ShoppingCart.Application;

public record CreateProductCommand(string Name, string Description, decimal Price, int Stock) : ICommand<ProductDto>;
public record AddToCartCommand(Guid UserId, Guid ProductId, int Quantity) : ICommand<CartDto>;
public record CheckoutCommand(Guid UserId) : ICommand<OrderDto>;
public record ListProductsQuery : ICommand<IReadOnlyList<ProductDto>>;
public record ListOrdersQuery(Guid UserId) : ICommand<IReadOnlyList<OrderDto>>;
