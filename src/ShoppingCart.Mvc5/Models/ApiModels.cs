namespace ShoppingCart.Mvc5.Models;
public sealed class ProductModel { public System.Guid Id { get; set; } public string Name { get; set; } public string Description { get; set; } public decimal Price { get; set; } public int Stock { get; set; } }
public sealed class LoginModel { public string Username { get; set; } public string Password { get; set; } }
public sealed class LoginResponse { public string token { get; set; } public System.Guid userId { get; set; } }
public sealed class AddCartModel { public System.Guid ProductId { get; set; } public int Quantity { get; set; } }
