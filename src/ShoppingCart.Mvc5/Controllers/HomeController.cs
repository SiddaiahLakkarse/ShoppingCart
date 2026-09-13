using Newtonsoft.Json;
using ShoppingCart.Mvc5.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ShoppingCart.Mvc5.Controllers;
public sealed class HomeController : Controller
{
    private readonly string api = System.Configuration.ConfigurationManager.AppSettings["ApiBaseUrl"];
    public async Task<ActionResult> Index() { using (var client = new HttpClient()) { var json = await client.GetStringAsync(api + "/api/products"); return View(JsonConvert.DeserializeObject<List<ProductModel>>(json)); } }
    public ActionResult Login() => View(new LoginModel());
    [HttpPost] public async Task<ActionResult> Login(LoginModel model) { using (var client = new HttpClient()) { var response = await client.PostAsJsonAsync(api + "/api/auth/login", model); if (!response.IsSuccessStatusCode) { ModelState.AddModelError("", "Login failed."); return View(model); } var data = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync()); Session["Jwt"] = data.token; return RedirectToAction("Index"); } }
    [HttpPost] public async Task<ActionResult> AddToCart(AddCartModel model) { using (var client = Client()) { await client.PostAsJsonAsync(api + "/api/cart/items", model); return RedirectToAction("Index"); } }
    [HttpPost] public async Task<ActionResult> Checkout() { using (var client = Client()) { await client.PostAsync(api + "/api/cart/checkout", null); return RedirectToAction("Orders"); } }
    public async Task<ActionResult> Orders() { using (var client = Client()) { var json = await client.GetStringAsync(api + "/api/orders"); return Content(json, "application/json"); } }
    private HttpClient Client() { var client = new HttpClient(); client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["Jwt"] as string); return client; }
}
