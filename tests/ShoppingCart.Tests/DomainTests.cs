using System;
using ShoppingCart.Domain;
using Xunit;

namespace ShoppingCart.Tests;
public sealed class DomainTests
{
    [Fact] public void Cart_Add_merges_same_product() { var product = new Product("Keyboard", "", 10m, 5); var cart = new Cart(Guid.NewGuid()); cart.Add(product, 1); cart.Add(product, 2); Assert.Single(cart.Items); Assert.Equal(3, cart.Items[0].Quantity); Assert.Equal(30m, cart.Total); }
    [Fact] public void Product_DecreaseStock_rejects_overdraw() { var product = new Product("Keyboard", "", 10m, 1); Assert.Throws<InvalidOperationException>(() => product.DecreaseStock(2)); }
    [Fact] public void Order_snapshots_total_and_items() { var item = new CartItem(Guid.NewGuid(), 2, 12.5m); var order = new Order(Guid.NewGuid(), new[] { item }); order.MarkPaid(); Assert.Equal(25m, order.Total); Assert.Equal(OrderStatus.Paid, order.Status); }
}
