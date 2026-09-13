# Database Design

## Engine and deployment
SQL Server 2019+ is used. Run `docker compose -f docker/docker-compose.yml up -d`, then execute `database/001_initial.sql` against `localhost,1433` (or allow API startup `EnsureCreated` for a development-only database).

## Schema

| Table | Purpose | Key columns |
|---|---|---|
| Products | Sellable catalog items | Id, Name, Price, Stock, IsActive |
| Carts | One active cart per user | Id, UserId |
| CartItems | Current cart lines | CartId, ProductId, Quantity, UnitPrice |
| Orders | Immutable checkout header | Id, UserId, CreatedUtc, Status, Total |
| OrderItems | Price snapshot at checkout | OrderId, ProductId, Quantity, UnitPrice |

User credentials are intentionally represented by the JWT demo identity in this starter. A production deployment should add `Users`, `Roles`, password hash, refresh-token and audit tables, or connect an external identity provider.

## Relationships
- Cart 1-to-many CartItems; CartItems reference Products.
- Order 1-to-many OrderItems; OrderItems reference Products.
- Product is not deleted when referenced; deactivate with `IsActive = 0`.

## Query optimization
- `Products(IsActive, Name)` supports the active catalog filter and deterministic ordering.
- `Carts(UserId)` is unique, making cart lookup a single indexed seek.
- `CartItems(CartId, ProductId)` supports aggregate loading and duplicate-line checks.
- `Orders(UserId, CreatedUtc DESC)` supports the order-history endpoint without a sort.
- Product lists use `AsNoTracking`; only checkout/product mutation paths track entities.
- All repository queries are async and use cancellation tokens.
- Avoid `SELECT *`, return DTOs, and inspect execution plans for catalog and order-history queries.

## Transaction and consistency
Checkout is one unit of work: validate stock, decrement stock, create the order, clear the cart, and commit. For high-concurrency production use, add a row-version/concurrency token to Products and retry on `DbUpdateConcurrencyException`; consider an outbox for payment events.

## Migration policy
The checked-in SQL is a reviewable baseline. For a production EF workflow, install `dotnet-ef` 3.1, generate an EF migration, review the generated SQL, and apply it through CI/CD. Never run destructive migrations automatically on production.
