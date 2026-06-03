# KMSTraining.API - Clean Architecture Refactoring

## Overview

The KMSTraining.API has been successfully refactored from a monolithic structure to **Domain-Centric Clean Architecture** with four distinct layers:

1. **Domain Layer** - Core business entities and interfaces
2. **Application Layer** - Use cases, application services, and DTOs  
3. **Infrastructure Layer** - Data access, repositories, and external services
4. **Presentation Layer** - Controllers and HTTP endpoints

## Architecture Principles

### Dependency Rule
> **Inner layers should never depend on outer layers. Dependencies flow inward.**

- **Domain** ← Depends on nothing
- **Application** ← Depends on Domain only
- **Infrastructure** ← Depends on Application and Domain
- **Presentation** ← Depends on all layers (entry point)

### Layer Responsibilities

#### Domain Layer (`KMSTraining.API.Domain`)
**Purpose:** Define core business entities, domain logic, and contracts

**Contains:**
- **Entities/** - Pure domain objects with business logic
  - `User.cs` - User entity with factory methods
  - `Trip.cs` - Trip entity with status management
  - `Destination.cs` - Destination entity
  - `Activity.cs` - Activity entity
  - `Budget.cs` - Budget entity with expense tracking
  
- **Interfaces/** - Repository and UoW contracts
  - `IRepository<T>` - Generic repository interface
  - `IUserRepository` - User-specific queries
  - `ITripRepository` - Trip-specific queries
  - `IDestinationRepository` - Destination-specific queries
  - `IActivityRepository` - Activity-specific queries
  - `IBudgetRepository` - Budget-specific queries
  - `IUnitOfWork` - Transaction management
  
- **Exceptions/** - Domain-specific exceptions
  - `DomainException` - Base domain exception
  - `EntityNotFoundException` - When entity not found
  - `DuplicateEntityException` - When duplicate exists
  - `AuthenticationException` - Auth failures
  - `AuthorizationException` - Authorization failures

**Key Features:**
- Entities contain factory methods (e.g., `Trip.Create()`)
- Domain validation methods (e.g., `Trip.ChangeStatus()`)
- Rich domain models with business logic

#### Application Layer (`KMSTraining.API.Application`)
**Purpose:** Orchestrate domain logic and handle use cases

**Contains:**
- **DTOs/** - Data transfer objects for API contracts
  - `AuthDtos.cs` - `RegisterDto`, `LoginDto`, `AuthResponseDto`, `UserDto`
  - `TripDtos.cs` - `CreateTripDto`, `UpdateTripDto`, `TripDto`
  - `DestinationDtos.cs` - `CreateDestinationDto`, `UpdateDestinationDto`, `DestinationDto`
  - `ActivityDtos.cs` - `CreateActivityDto`, `UpdateActivityDto`, `ActivityDto`
  - `BudgetDtos.cs` - `CreateBudgetDto`, `UpdateBudgetDto`, `BudgetDto`
  
- **Mapping/** - DTO ↔ Entity conversion
  - `Mapper.cs` - Handles all mapping logic
  - `IMapper` interface - Mapper contract
  
- **Services/** - Application services implementing use cases
  - `IAuthService` / `AuthService` - Registration and login
  - `ITripService` / `TripService` - Trip management
  - `IDestinationService` / `DestinationService` - Destination management
  - `IActivityService` / `ActivityService` - Activity management
  - `IBudgetService` / `BudgetService` - Budget management
  - `ITokenService` - Token generation interface

**Key Features:**
- Services depend on repositories via `IUnitOfWork`
- All business logic flows through application services
- DTOs ensure API contracts are independent of entity models
- Mappers handle all entity-to-DTO conversions

#### Infrastructure Layer (`KMSTraining.API.Infrastructure`)
**Purpose:** Implement data access and external dependencies

**Contains:**
- **Data/**
  - `TripPlannerDbContext.cs` - EF Core DbContext with model configuration
  
- **Repositories/** - Repository implementations
  - `Repository<T>` - Generic repository base class
  - `UserRepository` - User-specific queries
  - `TripRepository` - Trip-specific queries (with eager loading)
  - `DestinationRepository` - Destination-specific queries
  - `ActivityRepository` - Activity-specific queries
  - `BudgetRepository` - Budget-specific queries
  
- **UnitOfWork/** - Transaction management
  - `UnitOfWork` - Implements `IUnitOfWork` pattern
  - Manages all repositories
  - Provides transaction support
  
- **Services/**
  - `TokenService` - JWT token generation (implements `ITokenService`)
  
- **Extensions/**
  - `ServiceCollectionExtensions` - Dependency injection setup
  - `AddInfrastructureServices()` - Registers DbContext, repositories, UoW
  - `AddApplicationServices()` - Registers mappers and application services

**Key Features:**
- DbContext isolated in infrastructure
- Repository pattern abstracts data access
- Unit of Work manages multiple repositories
- Lazy-loaded repository instances for efficiency
- Extensible service registration

#### Presentation Layer (`KMSTraining.API`)
**Purpose:** Handle HTTP requests and responses

**Contains:**
- **Controllers/** - Existing controllers refactored to use new services
  - `AuthController` - Uses `IAuthService`
  - `TripsController` - Uses `ITripService`
  - `DestinationsController` - Uses `IDestinationService`
  - `ActivitiesController` - Uses `IActivityService`
  - `BudgetsController` - Uses `IBudgetService`
  
- **Program.cs** - Startup configuration
  - Registers infrastructure services
  - Configures authentication/authorization
  - Configures middleware pipeline

**Key Features:**
- Controllers are thin - they orchestrate application services
- No direct database access
- Proper separation of concerns

## Data Flow Example: User Registration

```
HTTP POST /api/auth/register
    ↓
AuthController.Register(RegisterDto)
    ↓
AuthService.RegisterAsync(RegisterDto)
    ├─ Check UsernameRepository.UsernameExistsAsync() → via IUnitOfWork
    ├─ Check EmailRepository.EmailExistsAsync() → via IUnitOfWork
    ├─ Hash password with BCrypt
    ├─ Mapper.MapRegisterDtoToUser() → Domain Entity
    ├─ UserRepository.AddAsync(User) → via IUnitOfWork
    ├─ UnitOfWork.SaveChangesAsync()
    ├─ TokenService.GenerateToken()
    └─ Return AuthResponseDto
    ↓
HTTP 200 OK with AuthResponseDto
```

## Benefits of This Architecture

### 1. **Testability**
- Unit test repositories, services, and entities independently
- Interfaces enable easy mocking
- Domain entities have no external dependencies

### 2. **Maintainability**
- Clear responsibility separation
- Easy to locate functionality
- Business logic isolated in domain

### 3. **Scalability**
- New features added without modifying core layers
- Repositories can be swapped (e.g., SQL Server → MongoDB)
- Application services remain unchanged

### 4. **Flexibility**
- Can add new presentation formats (gRPC, SignalR) without changing business logic
- Infrastructure changes don't affect application/domain
- Easy to refactor, optimize, or replace components

### 5. **Dependency Inversion**
- High-level modules don't depend on low-level modules
- Both depend on abstractions (interfaces)
- Controllers depend on services, not implementation details

## Project References

```
KMSTraining.API (Presentation)
├─ references → KMSTraining.API.Domain
├─ references → KMSTraining.API.Application  
├─ references → KMSTraining.API.Infrastructure

KMSTraining.API.Application
└─ references → KMSTraining.API.Domain

KMSTraining.API.Infrastructure
├─ references → KMSTraining.API.Application
└─ references → KMSTraining.API.Domain

KMSTraining.API.Domain
└─ (no external references)
```

## Migration Guide: Old Code → New Code

### Before (Monolithic)
```csharp
// Direct DbContext access
var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
```

### After (Clean Architecture)
```csharp
// Through repository interface
var user = await unitOfWork.Users.GetByUsernameAsync(username);
```

### Before (Service registration)
```csharp
builder.Services.AddDbContext<TripPlannerDbContext>(options => ...);
builder.Services.AddScoped<IAuthService, AuthService>();
```

### After (Clean Architecture)
```csharp
builder.Services.AddInfrastructureServices(configuration);
builder.Services.AddApplicationServices();
```

## Patterns Implemented

1. **Repository Pattern** - Abstracts data access
2. **Unit of Work Pattern** - Manages multiple repositories and transactions
3. **Dependency Injection** - Loose coupling through interfaces
4. **Factory Methods** - Encapsulated entity creation (e.g., `Trip.Create()`)
5. **DTO Pattern** - API contracts independent from domain models
6. **Mapper Pattern** - Centralized DTO ↔ Entity conversion

## Future Enhancements

1. **CQRS** - Separate read and write models for complex queries
2. **Domain Events** - Publish domain events for cross-cutting concerns
3. **Specifications** - Encapsulate complex query logic
4. **Auto-Mapper** - Use AutoMapper or similar for large DTOs
5. **Mediator Pattern** - Add MediatR for command/query handlers
6. **Value Objects** - Create strongly-typed values (e.g., Money, Email)

## Project Structure

```
KMSTraining.API/
├── KMSTraining.API/ (Presentation)
│   ├── Controllers/
│   ├── Program.cs
│   └── KMSTraining.API.csproj
│
├── KMSTraining.API.Domain/ (Domain)
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Trip.cs
│   │   ├── Destination.cs
│   │   ├── Activity.cs
│   │   └── Budget.cs
│   ├── Interfaces/
│   │   ├── IRepository.cs
│   │   ├── IUserRepository.cs
│   │   ├── ITripRepository.cs
│   │   ├── IDestinationRepository.cs
│   │   ├── IActivityRepository.cs
│   │   ├── IBudgetRepository.cs
│   │   └── IUnitOfWork.cs
│   ├── Exceptions/
│   │   └── DomainExceptions.cs
│   └── KMSTraining.API.Domain.csproj
│
├── KMSTraining.API.Application/ (Application)
│   ├── DTOs/
│   │   ├── AuthDtos.cs
│   │   ├── TripDtos.cs
│   │   ├── DestinationDtos.cs
│   │   ├── ActivityDtos.cs
│   │   └── BudgetDtos.cs
│   ├── Mapping/
│   │   └── Mapper.cs
│   ├── Services/
│   │   ├── IAuthService.cs
│   │   ├── AuthService.cs
│   │   ├── IDomainServices.cs
│   │   └── DomainServices.cs
│   └── KMSTraining.API.Application.csproj
│
└── KMSTraining.API.Infrastructure/ (Infrastructure)
    ├── Data/
    │   └── TripPlannerDbContext.cs
    ├── Repositories/
    │   └── Repository.cs
    ├── UnitOfWork/
    │   └── UnitOfWork.cs
    ├── Services/
    │   └── TokenService.cs
    ├── Extensions/
    │   └── ServiceCollectionExtensions.cs
    └── KMSTraining.API.Infrastructure.csproj
```

## Getting Started

### Building the Project
```bash
dotnet build
```

### Running the Application
```bash
dotnet run --project KMSTraining.API/KMSTraining.API.csproj
```

### Adding a New Feature

1. **Create Domain Entity** in `Domain/Entities/`
2. **Create Interfaces** in `Domain/Interfaces/`
3. **Create DTOs** in `Application/DTOs/`
4. **Implement Application Service** in `Application/Services/`
5. **Implement Repository** in `Infrastructure/Repositories/`
6. **Create Controller** in `KMSTraining.API/Controllers/`
7. **Register Services** in `Infrastructure/Extensions/ServiceCollectionExtensions.cs`

## References

- [Microsoft Clean Architecture Documentation](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [Clean Architecture: A Craftsman's Guide to Software Structure and Design](https://www.pearson.com/en-us/subject-catalog/p/clean-architecture-a-craftsman-s-guide-to-software-structure-and-design/P200000009529/9780136341802)
- [Domain-Driven Design: Tackling Complexity in the Heart of Software](https://www.pearson.com/en-us/subject-catalog/p/domain-driven-design-tackling-complexity-in-the-heart-of-software/P200000005248)
