using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShoppingCart.Application;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ShoppingCart.Api;
[ApiController, Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IConfiguration config; public AuthController(IConfiguration config) => this.config = config;
    [HttpPost("login")] public IActionResult Login(LoginRequest request) { if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password)) return Unauthorized(); var id = Guid.NewGuid(); var claims = new[] { new Claim(ClaimTypes.NameIdentifier, id.ToString()), new Claim(ClaimTypes.Name, request.Username) }; var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])); var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddHours(8), signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)); return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token), userId = id }); }
}
public record LoginRequest(string Username, string Password);

[ApiController, Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly ICommandHandler<CreateProductCommand, ProductDto> create; private readonly ICommandHandler<ListProductsQuery, IReadOnlyList<ProductDto>> list;
    public ProductsController(ICommandHandler<CreateProductCommand, ProductDto> create, ICommandHandler<ListProductsQuery, IReadOnlyList<ProductDto>> list) { this.create = create; this.list = list; }
    [HttpGet] public Task<IReadOnlyList<ProductDto>> Get(CancellationToken ct) => list.Handle(new ListProductsQuery(), ct);
    [HttpPost] public Task<ProductDto> Post(CreateProductCommand command, CancellationToken ct) => create.Handle(command, ct);
}

[ApiController, Authorize, Route("api/cart")]
public sealed class CartController : ControllerBase
{
    private readonly ICommandHandler<AddToCartCommand, CartDto> add; private readonly ICommandHandler<CheckoutCommand, OrderDto> checkout;
    public CartController(ICommandHandler<AddToCartCommand, CartDto> add, ICommandHandler<CheckoutCommand, OrderDto> checkout) { this.add = add; this.checkout = checkout; }
    [HttpPost("items")] public Task<CartDto> Add(AddCartRequest request, CancellationToken ct) => add.Handle(new AddToCartCommand(UserId(), request.ProductId, request.Quantity), ct);
    [HttpPost("checkout")] public Task<OrderDto> Checkout(CancellationToken ct) => checkout.Handle(new CheckoutCommand(UserId()), ct);
    private Guid UserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
}

[ApiController, Authorize, Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly ICommandHandler<ListOrdersQuery, IReadOnlyList<OrderDto>> list; public OrdersController(ICommandHandler<ListOrdersQuery, IReadOnlyList<OrderDto>> list) => this.list = list;
    [HttpGet] public Task<IReadOnlyList<OrderDto>> Get(CancellationToken ct) => list.Handle(new ListOrdersQuery(Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))), ct);
}
public record AddCartRequest(Guid ProductId, int Quantity);
