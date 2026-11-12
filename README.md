# Product Management API – Backend

## Overview

The Product Management API is a RESTful backend application built using **ASP.NET Core, C#, Dapper, and SQL Server**.

The application follows a layered architecture inspired by Clean Architecture principles. It separates HTTP request handling, business logic, and database operations to maintain a loosely coupled, maintainable, and testable codebase.

The backend implements CRUD operations for product management, custom business validation, centralized exception handling, and server-side pagination.

## Technology Stack

| Technology | Purpose |
|---|---|
| ASP.NET Core Web API | REST API development |
| C# | Backend programming language |
| Dapper | Lightweight ORM for database operations |
| SQL Server | Relational database |
| Dependency Injection | Managing application dependencies |
| ADO.NET / Microsoft.Data.SqlClient | SQL Server connectivity |
| ProblemDetails | Standardized API error responses |
| Swagger / OpenAPI | API documentation |

## Backend Architecture

The backend is organized into three main projects:

```text
ProductManagement/
│
├── TestProduct/
│   ├── Controllers/
│   ├── Middleware/
│   ├── Program.cs
│   └── appsettings.json
│
├── ProductManagement.Application/
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Services/
│   └── Exceptions/
│
└── ProductManagement.Infrastructure/
    └── Repositories/
```

### 1. API Layer

Responsible for handling incoming HTTP requests and returning appropriate HTTP responses.

Responsibilities:
- Exposes RESTful API endpoints through controllers.
- Receives and processes HTTP requests.
- Calls application services through dependency injection.
- Returns appropriate HTTP status codes.
- Registers application and infrastructure dependencies.
- Configures global exception handling.

### 2. Application Layer

Contains the application's business logic and defines contracts for communication between layers.

Responsibilities:
- Implements service interfaces and business operations.
- Performs custom business validation.
- Defines repository interfaces.
- Contains request and response DTOs.
- Handles application-specific exceptions.
- Maintains independence from database implementation details.

### 3. Infrastructure Layer

Responsible for implementing external dependencies and database operations.

Responsibilities:
- Implements repository interfaces.
- Uses Dapper for SQL Server communication.
- Executes parameterized SQL queries.
- Handles database CRUD operations.
- Supports asynchronous database operations.
- Retrieves paginated records and total record counts.

## Request Processing Flow

The application follows this execution flow:

```text
HTTP Request
     |
     v
API Controller
     |
     v
Application Service
     |
     |-- Business Validation
     |
     v
Repository Interface
     |
     v
Dapper Repository
     |
     v
SQL Server
     |
     v
Response DTO
     |
     v
HTTP Response
```

This separation ensures that controllers do not directly communicate with the database and business logic remains independent of the persistence technology.

## Design Patterns

The backend uses the following design patterns and architectural practices:

### 1. Repository Pattern

The Repository Pattern abstracts database operations behind an interface.

Example:

```csharp
public interface IProductRepository
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<ProductDto?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<int> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken);

    Task<bool> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken);
}
```

Benefits:
- Separates data access from business logic.
- Makes database implementations replaceable.
- Improves unit testing and maintainability.

### 2. Dependency Injection

Dependencies are registered centrally and injected through constructors.

Example:

```csharp
builder.Services.AddScoped<
    IProductRepository,
    ProductRepository>();

builder.Services.AddScoped<
    IProductService,
    ProductService>();
```

Services depend on abstractions rather than concrete repository implementations.

### 3. Service Layer Pattern

The service layer centralizes business rules and validation.

For example, product creation validates:
- Product name is required.
- Product name must contain between 2 and 150 characters.
- Description cannot exceed 1000 characters.
- Price must be greater than zero.
- Stock quantity cannot be negative.

This keeps controllers lightweight and prevents business validation from being duplicated across endpoints.

### 4. DTO Pattern

Data Transfer Objects are used to control the data exchanged between API clients and the backend.

Examples:
- `CreateProductRequest`
- `UpdateProductRequest`
- `ProductDto`
- `PagedResponse<T>`

DTOs prevent direct exposure of database implementation details and allow API contracts to evolve independently.

## SOLID Principles

The backend follows SOLID principles to improve maintainability, extensibility, and testability.

| Principle | Implementation |
|---|---|
| Single Responsibility (SRP) | Controllers, services, repositories, and exception handlers have separate responsibilities. |
| Open/Closed (OCP) | Repository implementations can be extended or replaced without changing service business logic. |
| Liskov Substitution (LSP) | Different repository implementations can be substituted as long as they honor the repository contract. |
| Interface Segregation (ISP) | Application services and repositories use separate interfaces. |
| Dependency Inversion (DIP) | High-level services depend on repository abstractions rather than concrete database implementations. |

## Exception Handling

The application uses centralized exception handling through ASP.NET Core's `IExceptionHandler`.

Custom exceptions are used to represent application-level errors:

- `BadRequestException` – Invalid request or business validation failure.
- `NotFoundException` – Requested resource does not exist.
- `ConflictException` – Resource conflict.

The global exception handler maps exceptions to appropriate HTTP status codes and returns standardized ProblemDetails responses.

Example error response:

```json
{
  "title": "Bad Request",
  "status": 400,
  "detail": "Product name is required.",
  "instance": "/api/Products",
  "errors": {},
  "traceId": "..."
}
```

Unexpected exceptions are logged, while internal exception details are not exposed to API consumers.

## Pagination

The API supports server-side pagination to retrieve records efficiently.

Example endpoint:

```http
GET /api/Products?page=1&pageSize=10
```

The paginated response includes:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 10,
  "totalRecords": 25,
  "totalPages": 3,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

Pagination is implemented using SQL Server's `OFFSET` and `FETCH NEXT` clauses.

Benefits:
- Reduces the number of records transferred per request.
- Improves API response efficiency for larger datasets.
- Provides metadata for client-side pagination controls.

## Backend Best Practices

The implementation incorporates the following practices:

- **Asynchronous programming:** Uses async/await for database operations.
- **Cancellation support:** Passes `CancellationToken` through controller, service, and repository layers.
- **Parameterized SQL:** Uses Dapper parameters to prevent SQL injection.
- **Custom validation:** Keeps business validation in the application service layer without relying on Data Annotations.
- **Separation of concerns:** Keeps API, application, and infrastructure responsibilities independent.
- **Dependency Injection:** Uses interface-based dependency registration.
- **Centralized exception handling:** Maintains consistent error responses.
- **DTO-based communication:** Separates API contracts from database structures.
- **Database constraints:** Enforces important data integrity rules at the database level.
- **Server-side pagination:** Retrieves only the requested records.

## API Operations

| HTTP Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Products` | Retrieve paginated products |
| GET | `/api/Products/{id}` | Retrieve a product by ID |
| POST | `/api/Products` | Create a new product |
| PUT | `/api/Products/{id}` | Update an existing product |
| DELETE | `/api/Products/{id}` | Delete a product |

## Architecture Benefits

This backend architecture provides:

1. **Maintainability:** Clear separation of responsibilities makes changes easier.
2. **Extensibility:** New features and repository implementations can be added with minimal impact on existing code.
3. **Testability:** Business logic can be tested independently of the database.
4. **Scalability:** Asynchronous operations and pagination support efficient request processing.
5. **Security:** Parameterized SQL queries and controlled API contracts reduce common security risks.
6. **Consistency:** Centralized exception handling provides predictable API error responses.

---

**Architecture Summary:** ASP.NET Core Web API + Layered Architecture + Dapper Repository Pattern + Dependency Injection + SOLID Principles + SQL Server.
