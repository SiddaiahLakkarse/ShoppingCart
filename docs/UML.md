# UML diagrams

The diagrams use Mermaid and render in GitHub, Azure DevOps, and compatible Markdown viewers.

## Component diagram
```mermaid
graph TD
  MVC[ASP.NET MVC 5 Client] -->|HTTP/JWT| API[ShoppingCart.Api]
  API --> APP[ShoppingCart.Application]
  APP --> DOMAIN[ShoppingCart.Domain]
  API --> INFRA[ShoppingCart.Infrastructure]
  INFRA --> DB[(SQL Server)]
  INFRA -.implements. APP
```

## Domain class diagram
```mermaid
classDiagram
  Product { Guid Id; string Name; decimal Price; int Stock; DecreaseStock() }
  Cart { Guid UserId; Add(); Remove(); Total }
  CartItem { Guid ProductId; int Quantity; decimal UnitPrice }
  Order { Guid UserId; OrderStatus Status; decimal Total; MarkPaid() }
  OrderItem { Guid ProductId; int Quantity; decimal UnitPrice }
  Cart "1" *-- "many" CartItem
  Order "1" *-- "many" OrderItem
  CartItem --> Product
  OrderItem --> Product
```

## Checkout sequence
```mermaid
sequenceDiagram
  actor User
  participant MVC as MVC 5
  participant API as Web API
  participant Handler as CheckoutHandler
  participant DB as SQL Server
  User->>MVC: Checkout
  MVC->>API: POST /api/cart/checkout (Bearer JWT)
  API->>Handler: CheckoutCommand(UserId)
  Handler->>DB: Load cart and products
  Handler->>Handler: Validate stock and decrement
  Handler->>DB: Insert order, clear cart, commit
  API-->>MVC: OrderDto
  MVC-->>User: Order history
```

## Command-handler flow
```mermaid
flowchart LR
  Request --> Controller --> Command[Command DTO] --> Handler --> Repository --> Db[(SQL Server)]
  Handler --> DTO[Response DTO]
```
