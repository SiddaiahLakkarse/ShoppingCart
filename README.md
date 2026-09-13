# Shopping Cart API

Shopping Cart is an ASP.NET Core Web API for product management, shopping cart operations, JWT authentication, checkout, and order history.

## Technology Stack

- ASP.NET Core Web API
- C#
- .NET Core 3.1 for the API
- .NET Standard 2.0 for the infrastructure project
- .NET 8 for automated tests
- Entity Framework Core 3.1
- SQL Server
- JWT bearer authentication
- Swagger/OpenAPI
- xUnit
- Clean architecture-style project separation

> **Important:** .NET Core 3.1 is out of support. For production use, plan to migrate the application to a supported version such as .NET 8 or later.

## Solution Structure

```text
ShoppingCart.slnx
├── src
│   ├── ShoppingCart.Api             # HTTP API, controllers, authentication, Swagger
│   ├── ShoppingCart.Application     # Commands, queries, handlers, DTOs, contracts
│   ├── ShoppingCart.Domain          # Entities and business rules
│   └── ShoppingCart.Infrastructure   # EF Core DbContext, repositories, seed data
└── tests
	└── ShoppingCart.Tests           # xUnit domain tests
```

## Features

- JWT-based login and bearer-token authentication.
- Product listing and product creation.
- Product validation for name, price, and stock.
- Product stock tracking and stock reduction during checkout.
- Automatic shopping cart creation per user.
- Add products to a cart, including quantity merging for duplicate products.
- Cart total calculation.
- Checkout processing and paid order creation.
- User-specific order history.
- SQL Server persistence through Entity Framework Core.
- Automatic database creation on startup.
- Automatic seed data for initial products.
- Swagger/OpenAPI documentation.
- Asynchronous API operations with cancellation-token support.
- Domain unit tests using xUnit.

## Prerequisites

Install the following software:

1. Visual Studio 2022 or Visual Studio 2026 with the **ASP.NET and web development** workload.
2. A compatible .NET SDK/runtime:
   - .NET Core 3.1 SDK/runtime for running the API.
   - .NET 8 SDK for running the test project.
3. SQL Server Developer, Express, or SQL Server running in Docker.
4. Optional tools: SQL Server Management Studio, Azure Data Studio, Postman, curl, and Docker Desktop.

## Database Installation

The API uses SQL Server. The default connection string is configured in `src/ShoppingCart.Api/appsettings.json`:

```text
Server=localhost,1433;Database=ShoppingCart;User Id=sa;Password=Your_strong_password123;MultipleActiveResultSets=true;TrustServerCertificate=True
```

### Local SQL Server

Make sure SQL Server is running, TCP connections are enabled, port `1433` is available, the `sa` login is enabled, and the password matches the connection string.

The `ShoppingCart` database does not need to be created manually. The API calls Entity Framework Core `EnsureCreated()` during startup and creates the database when it does not exist.

### SQL Server with Docker

Run this PowerShell command:

```powershell
docker run --name shoppingcart-sql `
  -e "ACCEPT_EULA=Y" `
  -e "MSSQL_SA_PASSWORD=Your_strong_password123" `
  -p 1433:1433 `
  -d mcr.microsoft.com/mssql/server:2019-latest
```

Verify that the container is running:

```powershell
docker ps
```

## Configuration

The primary configuration file is `src/ShoppingCart.Api/appsettings.json`.

```json
{
  "ConnectionStrings": {
	"Default": "Server=localhost,1433;Database=ShoppingCart;User Id=sa;Password=Your_strong_password123;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "Jwt": {
	"Key": "development-only-change-this-key-123456"
  }
}
```

The JWT key signs authentication tokens. The login endpoint currently accepts any non-empty username and password and creates a development token; it is not connected to a user database.

For production, replace the default JWT key with a long random secret and store it in user secrets, environment variables, Azure Key Vault, or another secure secret store. Do not commit database passwords or JWT keys to source control.

Example PowerShell environment variable:

```powershell
$env:Jwt__Key = "replace-with-a-secure-production-key"
```

## Running the Application

### Visual Studio

1. Open `D:\Projects\Shopping-Cart\ShoppingCart.slnx`.
2. Set `ShoppingCart.Api` as the startup project.
3. Press `F5`, or select **Debug > Start Debugging**.
4. The configured launch URLs are:

   ```text
   https://localhost:52671
   http://localhost:52672
   ```

5. Open Swagger at `https://localhost:52671/swagger` or `http://localhost:52672/swagger`.

### PowerShell

From the solution root:

```powershell
cd D:\Projects\Shopping-Cart
dotnet restore
dotnet run --project .\src\ShoppingCart.Api\ShoppingCart.Api.csproj
```

Open the URL displayed in the terminal followed by `/swagger`.

At startup, the following products are inserted when the database is empty:

| Product | Description | Price | Stock |
|---|---|---:|---:|
| Mechanical Keyboard | Compact USB keyboard | 89.99 | 100 |
| Wireless Mouse | Ergonomic 2.4 GHz mouse | 39.99 | 150 |
| USB-C Hub | Seven-port aluminum hub | 49.99 | 75 |

## API Endpoints

### Login

```http
POST /api/auth/login
Content-Type: application/json
```

```json
{
  "username": "demo-user",
  "password": "demo-password"
}
```

The response contains a JWT token and generated user ID. Use the token on protected endpoints:

```http
Authorization: Bearer <token>
```

### Products

```http
GET /api/products
POST /api/products
```

Example product request:

```json
{
  "name": "Laptop Stand",
  "description": "Adjustable aluminum laptop stand",
  "price": 59.99,
  "stock": 25
}
```

The product name must not be empty, and price and stock must not be negative.

### Cart

Cart endpoints require a bearer token.

```http
POST /api/cart/items
POST /api/cart/checkout
```

Example add-to-cart request:

```json
{
  "productId": "00000000-0000-0000-0000-000000000000",
  "quantity": 2
}
```

Checkout verifies that the cart is not empty, verifies product stock, decreases stock, creates and pays the order, and clears the cart.

### Orders

```http
GET /api/orders
```

This returns orders for the authenticated user in descending creation-date order.

## Typical Usage Flow

1. Start SQL Server.
2. Start the API.
3. Open Swagger at `/swagger`.
4. Call `POST /api/auth/login` with any non-empty username and password.
5. Copy the returned token and authorize Swagger with `Bearer <token>`.
6. Call `GET /api/products` and select a product ID.
7. Call `POST /api/cart/items` with the product ID and quantity.
8. Call `POST /api/cart/checkout`.
9. Call `GET /api/orders` to view the created order.

## Running Tests

The test project targets .NET 8 and uses xUnit:

```powershell
dotnet test .\tests\ShoppingCart.Tests\ShoppingCart.Tests.csproj
```

The current tests cover combining duplicate cart items, rejecting stock overdraw, and calculating order totals and item snapshots. Tests can also be run from Visual Studio Test Explorer with **Test > Run All Tests**.

## Architecture Overview

- **Domain:** Entities and business rules for products, carts, cart items, orders, and order status.
- **Application:** Commands, queries, handlers, DTOs, and repository abstractions.
- **Infrastructure:** EF Core `DbContext`, SQL Server configuration, repositories, unit of work, and seed data.
- **API:** Controllers, dependency injection, JWT authentication, Swagger, and HTTP endpoint routing.

The application uses command-handler classes to coordinate use cases while keeping the domain independent from HTTP and Entity Framework concerns.

## Troubleshooting

### SQL Server connection failure

Check that SQL Server or the Docker container is running, port `1433` is available, the connection string credentials are correct, TCP connections are enabled, and the firewall is not blocking the port.

### HTTPS certificate warning

The local development certificate may not be trusted. Trust the development certificate or use the HTTP URL `http://localhost:52672/swagger`.

### Port already in use

Change the ports in `src/ShoppingCart.Api/Properties/launchSettings.json`.

### Old seed data remains

Seed data is inserted only when the `Products` table is empty. To reset local development data, remove the `ShoppingCart` database and restart the API.

## Security and Production Checklist

- Upgrade from .NET Core 3.1 to a supported .NET version.
- Replace the demo login with ASP.NET Identity or an external identity provider.
- Replace the development JWT key and secure all secrets.
- Use HTTPS in every environment.
- Add role-based authorization for product administration.
- Add consistent request validation and error handling.
- Use EF Core migrations instead of relying only on `EnsureCreated()`.
- Add logging, monitoring, rate limiting, and exception handling.
- Review checkout concurrency and stock consistency requirements.
- Add integration, API contract, and end-to-end tests.
