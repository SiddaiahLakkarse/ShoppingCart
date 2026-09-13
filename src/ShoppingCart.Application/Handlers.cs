using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ShoppingCart.Domain;

namespace ShoppingCart.Application;

public sealed class CreateProductHandler : ICommandHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository products; private readonly IUnitOfWork unitOfWork;
    public CreateProductHandler(IProductRepository products, IUnitOfWork unitOfWork) { this.products = products; this.unitOfWork = unitOfWork; }
    public async Task<ProductDto> Handle(CreateProductCommand command, CancellationToken ct) { if (string.IsNullOrWhiteSpace(command.Name) || command.Price < 0 || command.Stock < 0) throw new ArgumentException("Invalid product data."); var product = new Product(command.Name.Trim(), command.Description ?? string.Empty, command.Price, command.Stock); await products.Add(product, ct); await unitOfWork.SaveChanges(ct); return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.Stock); }
}

public sealed class ListProductsHandler : ICommandHandler<ListProductsQuery, IReadOnlyList<ProductDto>>
{
    private readonly IProductRepository products; public ListProductsHandler(IProductRepository products) => this.products = products;
    public async Task<IReadOnlyList<ProductDto>> Handle(ListProductsQuery command, CancellationToken ct) => (await products.List(ct)).Select(x => new ProductDto(x.Id, x.Name, x.Description, x.Price, x.Stock)).ToList();
}

public sealed class AddToCartHandler : ICommandHandler<AddToCartCommand, CartDto>
{
    private readonly IProductRepository products; private readonly ICartRepository carts; private readonly IUnitOfWork unitOfWork;
    public AddToCartHandler(IProductRepository products, ICartRepository carts, IUnitOfWork unitOfWork) { this.products = products; this.carts = carts; this.unitOfWork = unitOfWork; }
    public async Task<CartDto> Handle(AddToCartCommand command, CancellationToken ct) { var product = await products.Get(command.ProductId, ct) ?? throw new KeyNotFoundException("Product not found."); if (!product.IsActive || product.Stock < command.Quantity) throw new InvalidOperationException("Product is unavailable."); var cart = await carts.GetOrCreate(command.UserId, ct); cart.Add(product, command.Quantity); await carts.Save(cart, ct); await unitOfWork.SaveChanges(ct); return ToDto(cart); }
    internal static CartDto ToDto(Cart cart) => new CartDto(cart.Id, cart.Items.Select(x => new CartItemDto(x.ProductId, x.Quantity, x.UnitPrice)).ToList(), cart.Total);
}

public sealed class CheckoutHandler : ICommandHandler<CheckoutCommand, OrderDto>
{
    private readonly ICartRepository carts; private readonly IProductRepository products; private readonly IOrderRepository orders; private readonly IUnitOfWork unitOfWork;
    public CheckoutHandler(ICartRepository carts, IProductRepository products, IOrderRepository orders, IUnitOfWork unitOfWork) { this.carts = carts; this.products = products; this.orders = orders; this.unitOfWork = unitOfWork; }
    public async Task<OrderDto> Handle(CheckoutCommand command, CancellationToken ct) { var cart = await carts.GetOrCreate(command.UserId, ct); if (!cart.Items.Any()) throw new InvalidOperationException("Cart is empty."); foreach (var item in cart.Items) { var product = await products.Get(item.ProductId, ct) ?? throw new KeyNotFoundException("Product not found."); product.DecreaseStock(item.Quantity); } var order = new Order(command.UserId, cart.Items); order.MarkPaid(); await orders.Add(order, ct); cart.Items.Clear(); await carts.Save(cart, ct); await unitOfWork.SaveChanges(ct); return ToDto(order); }
    internal static OrderDto ToDto(Order order) => new OrderDto(order.Id, order.Total, order.Status, order.CreatedUtc, order.Items.Select(x => new CartItemDto(x.ProductId, x.Quantity, x.UnitPrice)).ToList());
}

public sealed class ListOrdersHandler : ICommandHandler<ListOrdersQuery, IReadOnlyList<OrderDto>>
{
    private readonly IOrderRepository orders; public ListOrdersHandler(IOrderRepository orders) => this.orders = orders;
    public async Task<IReadOnlyList<OrderDto>> Handle(ListOrdersQuery command, CancellationToken ct) => (await orders.List(command.UserId, ct)).Select(CheckoutHandler.ToDto).ToList();
}
