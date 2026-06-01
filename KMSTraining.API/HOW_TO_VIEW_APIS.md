# Quick Start - Viewing All APIs

## Method 1: Swagger UI (Recommended) ⭐

1. **Run the API**:
   - Press **F5** in Visual Studio, or
   - Run `dotnet run --project KMSTraining.API\KMSTraining.API.csproj`

2. **Open your browser** and navigate to:
   ```
   https://localhost:5001/
   ```

3. **You'll see an interactive Swagger UI** with:
   - All endpoints listed by controller
   - Try-it-out functionality
   - Request/response examples
   - Schema definitions

4. **To test authenticated endpoints**:
   - First, use `/api/auth/register` or `/api/auth/login` to get a token
   - Click the **"Authorize"** button (🔒) at the top right
   - Enter: `Bearer <your_token_here>`
   - Click "Authorize"
   - Now you can test all protected endpoints

---

## Method 2: Visual Studio API Explorer

1. Open **Solution Explorer**
2. Expand **Controllers** folder
3. You'll see all controller files:
   - `AuthController.cs` - Authentication
   - `TripsController.cs` - Trip management
   - `DestinationsController.cs` - Destination management
   - `ActivitiesController.cs` - Activity management
   - `BudgetsController.cs` - Budget management
   - `HealthController.cs` - Health checks

4. Open any controller to see the endpoints (methods marked with `[HttpGet]`, `[HttpPost]`, etc.)

---

## Method 3: API Reference Documentation

See the complete API documentation in:
```
KMSTraining.API/API_REFERENCE.md
```

This file contains:
- All endpoints with full details
- Request/response examples
- Authentication guide
- Error handling
- cURL examples

---

## All Available APIs at a Glance

### Authentication (Public)
- `POST /api/auth/register` - Register
- `POST /api/auth/login` - Login

### Health Checks (Public)
- `GET /health` - Basic health
- `GET /health/ready` - Readiness
- `GET /api/health` - Detailed health
- `GET /api/health/live` - Liveness
- `GET /api/health/ready` - Readiness (detailed)

### Trips (Protected)
- `GET /api/trips` - List all trips
- `GET /api/trips/{id}` - Get trip details
- `POST /api/trips` - Create trip
- `PUT /api/trips/{id}` - Update trip
- `DELETE /api/trips/{id}` - Delete trip

### Destinations (Protected)
- `GET /api/destinations?tripId={id}` - List destinations
- `GET /api/destinations/{id}` - Get destination
- `POST /api/destinations` - Create destination
- `PUT /api/destinations/{id}` - Update destination
- `DELETE /api/destinations/{id}` - Delete destination

### Activities (Protected)
- `GET /api/activities?destinationId={id}` - List activities
- `GET /api/activities/{id}` - Get activity
- `POST /api/activities` - Create activity
- `PUT /api/activities/{id}` - Update activity
- `DELETE /api/activities/{id}` - Delete activity

### Budgets (Protected)
- `GET /api/budgets?tripId={id}` - List budgets
- `GET /api/budgets/{id}` - Get budget
- `POST /api/budgets` - Create budget
- `PUT /api/budgets/{id}` - Update budget
- `DELETE /api/budgets/{id}` - Delete budget

---

## Quick Test

```powershell
# 1. Start the API
dotnet run --project KMSTraining.API\KMSTraining.API.csproj

# 2. In another terminal, test health endpoint
curl https://localhost:5001/health

# 3. Register a user
curl -k -X POST https://localhost:5001/api/auth/register `
  -H "Content-Type: application/json" `
  -d '{\"username\":\"testuser\",\"email\":\"test@example.com\",\"password\":\"Test123!\",\"fullName\":\"Test User\"}'

# 4. Open Swagger UI for interactive testing
# Navigate to: https://localhost:5001/
```

---

## Swagger UI Features

✅ **Interactive Testing** - Try endpoints directly from the browser  
✅ **Auto Documentation** - Automatically generated from code  
✅ **Schema Explorer** - View all DTOs and models  
✅ **JWT Authentication** - Test protected endpoints  
✅ **Request/Response Examples** - See expected formats  
✅ **Export OpenAPI Spec** - Download JSON/YAML specification  

---

## Next Steps

1. ✅ Run the API with `F5`
2. ✅ Open https://localhost:5001/ in browser
3. ✅ Explore all endpoints in Swagger UI
4. ✅ Test authentication and protected endpoints
5. ✅ Review full API documentation in `API_REFERENCE.md`
