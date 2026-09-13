using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShoppingCart.Application;
using ShoppingCart.Infrastructure;
using System.Text;

namespace ShoppingCart.Api;
public sealed class Startup
{
    public IConfiguration Configuration { get; } public Startup(IConfiguration configuration) => Configuration = configuration;
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<ShoppingCartDbContext>(o => o.UseSqlServer(Configuration.GetConnectionString("Default")));
        services.AddScoped<IProductRepository, ProductRepository>(); services.AddScoped<ICartRepository, CartRepository>(); services.AddScoped<IOrderRepository, OrderRepository>(); services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICommandHandler<CreateProductCommand, ProductDto>, CreateProductHandler>(); services.AddScoped<ICommandHandler<ListProductsQuery, IReadOnlyList<ProductDto>>, ListProductsHandler>(); services.AddScoped<ICommandHandler<AddToCartCommand, CartDto>, AddToCartHandler>(); services.AddScoped<ICommandHandler<CheckoutCommand, OrderDto>, CheckoutHandler>(); services.AddScoped<ICommandHandler<ListOrdersQuery, IReadOnlyList<OrderDto>>, ListOrdersHandler>();
        var key = Encoding.UTF8.GetBytes(Configuration["Jwt:Key"] ?? "development-only-change-this-key-123456"); services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters { ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(key), ValidateIssuer = false, ValidateAudience = false });
        services.AddAuthorization(); services.AddControllers(); services.AddSwaggerGen();
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    { using (var scope = app.ApplicationServices.CreateScope()) { var db = scope.ServiceProvider.GetRequiredService<ShoppingCartDbContext>(); db.Database.EnsureCreated(); SeedData.Seed(db).GetAwaiter().GetResult(); } app.UseRouting(); app.UseAuthentication(); app.UseAuthorization(); app.UseSwagger(); app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopping Cart API v1")); app.UseEndpoints(e => e.MapControllers()); }
}
