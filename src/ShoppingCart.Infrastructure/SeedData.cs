using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Domain;

namespace ShoppingCart.Infrastructure;
public static class SeedData
{
    public static async Task Seed(ShoppingCartDbContext db)
    {
        if (db.Products.Any()) return;
        db.Products.AddRange(new Product("Mechanical Keyboard", "Compact USB keyboard", 89.99m, 100), new Product("Wireless Mouse", "Ergonomic 2.4 GHz mouse", 39.99m, 150), new Product("USB-C Hub", "Seven-port aluminum hub", 49.99m, 75));
        await db.SaveChangesAsync();
    }
}
