# KMS Training API - Complete API Reference

## Base URL
```
Development: https://localhost:5001
Production: https://your-domain.com
```

## Swagger UI
When running in Development mode, access interactive API documentation at:
```
https://localhost:5001/
```

---

## Authentication
Most endpoints require JWT Bearer token authentication.

**Header Format:**
```
Authorization: Bearer <your_jwt_token>
```

---

## API Endpoints Summary

### 🔐 Authentication (No Auth Required)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login and receive JWT token |

### 🏥 Health Checks (No Auth Required)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/health` | Basic health check |
| GET | `/health/ready` | Readiness check |
| GET | `/api/health` | Detailed health status |
| GET | `/api/health/live` | Liveness probe |
| GET | `/api/health/ready` | Readiness probe (detailed) |

### ✈️ Trips (Auth Required)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/trips` | Get all trips for authenticated user |
| GET | `/api/trips/{id}` | Get trip details with destinations and budgets |
| POST | `/api/trips` | Create a new trip |
| PUT | `/api/trips/{id}` | Update a trip |
| DELETE | `/api/trips/{id}` | Delete a trip |

### 🌍 Destinations (Auth Required)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/destinations?tripId={tripId}` | Get all destinations for a trip |
| GET | `/api/destinations/{id}` | Get destination details with activities |
| POST | `/api/destinations` | Create a new destination |
| PUT | `/api/destinations/{id}` | Update a destination |
| DELETE | `/api/destinations/{id}` | Delete a destination |

### 🎯 Activities (Auth Required)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/activities?destinationId={destinationId}` | Get all activities for a destination |
| GET | `/api/activities/{id}` | Get activity details |
| POST | `/api/activities` | Create a new activity |
| PUT | `/api/activities/{id}` | Update an activity |
| DELETE | `/api/activities/{id}` | Delete an activity |

### 💰 Budgets (Auth Required)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/budgets?tripId={tripId}` | Get all budgets for a trip |
| GET | `/api/budgets/{id}` | Get budget details |
| POST | `/api/budgets` | Create a new budget |
| PUT | `/api/budgets/{id}` | Update a budget |
| DELETE | `/api/budgets/{id}` | Delete a budget |

---

## Detailed API Documentation

### Authentication APIs

#### Register User
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePassword123!",
  "fullName": "John Doe"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "username": "john_doe",
  "email": "john@example.com",
  "expiresAt": "2026-06-01T12:00:00Z"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "john_doe",
  "password": "SecurePassword123!"
}
```

**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": 1,
  "username": "john_doe",
  "email": "john@example.com",
  "expiresAt": "2026-06-01T12:00:00Z"
}
```

---

### Trip APIs

#### Get All Trips
```http
GET /api/trips
Authorization: Bearer <token>
```

**Response (200 OK):**
```json
[
  {
	"id": 1,
	"name": "Summer Vacation 2026",
	"description": "Trip to Europe",
	"startDate": "2026-07-01T00:00:00Z",
	"endDate": "2026-07-15T00:00:00Z",
	"status": "Planning",
	"userId": 1,
	"createdAt": "2026-06-01T10:00:00Z",
	"updatedAt": null
  }
]
```

#### Get Trip by ID
```http
GET /api/trips/1
Authorization: Bearer <token>
```

**Response (200 OK):**
```json
{
  "id": 1,
  "name": "Summer Vacation 2026",
  "description": "Trip to Europe",
  "startDate": "2026-07-01T00:00:00Z",
  "endDate": "2026-07-15T00:00:00Z",
  "status": "Planning",
  "userId": 1,
  "createdAt": "2026-06-01T10:00:00Z",
  "updatedAt": null,
  "destinations": [
	{
	  "id": 1,
	  "name": "Paris",
	  "country": "France",
	  "city": "Paris",
	  "description": "City of lights",
	  "arrivalDate": "2026-07-01T00:00:00Z",
	  "departureDate": "2026-07-05T00:00:00Z",
	  "tripId": 1,
	  "createdAt": "2026-06-01T10:00:00Z",
	  "updatedAt": null
	}
  ],
  "budgets": [
	{
	  "id": 1,
	  "category": "Accommodation",
	  "plannedAmount": 2000.00,
	  "actualAmount": 0.00,
	  "notes": "Hotels in Paris",
	  "tripId": 1,
	  "createdAt": "2026-06-01T10:00:00Z",
	  "updatedAt": null
	}
  ]
}
```

#### Create Trip
```http
POST /api/trips
Authorization: Bearer <token>
Content-Type: application/json

{
  "name": "Summer Vacation 2026",
  "description": "Trip to Europe",
  "startDate": "2026-07-01",
  "endDate": "2026-07-15"
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "name": "Summer Vacation 2026",
  "description": "Trip to Europe",
  "startDate": "2026-07-01T00:00:00Z",
  "endDate": "2026-07-15T00:00:00Z",
  "status": "Planning",
  "userId": 1,
  "createdAt": "2026-06-01T10:00:00Z",
  "updatedAt": null
}
```

#### Update Trip
```http
PUT /api/trips/1
Authorization: Bearer <token>
Content-Type: application/json

{
  "name": "Updated Trip Name",
  "description": "Updated description",
  "startDate": "2026-07-02",
  "endDate": "2026-07-16",
  "status": "Confirmed"
}
```

**Valid Status Values:**
- `Planning`
- `Confirmed`
- `InProgress`
- `Completed`
- `Cancelled`

**Response (200 OK):**
```json
{
  "id": 1,
  "name": "Updated Trip Name",
  "description": "Updated description",
  "startDate": "2026-07-02T00:00:00Z",
  "endDate": "2026-07-16T00:00:00Z",
  "status": "Confirmed",
  "userId": 1,
  "createdAt": "2026-06-01T10:00:00Z",
  "updatedAt": "2026-06-01T11:00:00Z"
}
```

#### Delete Trip
```http
DELETE /api/trips/1
Authorization: Bearer <token>
```

**Response (204 No Content)**

---

### Destination APIs

#### Get All Destinations for a Trip
```http
GET /api/destinations?tripId=1
Authorization: Bearer <token>
```

#### Create Destination
```http
POST /api/destinations
Authorization: Bearer <token>
Content-Type: application/json

{
  "name": "Paris",
  "country": "France",
  "city": "Paris",
  "description": "City of lights",
  "arrivalDate": "2026-07-01",
  "departureDate": "2026-07-05",
  "tripId": 1
}
```

**Validation:**
- Arrival date must be >= trip start date
- Departure date must be <= trip end date
- Departure date must be after arrival date

---

### Activity APIs

#### Get All Activities for a Destination
```http
GET /api/activities?destinationId=1
Authorization: Bearer <token>
```

#### Create Activity
```http
POST /api/activities
Authorization: Bearer <token>
Content-Type: application/json

{
  "name": "Eiffel Tower Visit",
  "description": "Visit to the iconic Eiffel Tower",
  "scheduledDateTime": "2026-07-02T14:00:00Z",
  "durationMinutes": 180,
  "location": "Champ de Mars, Paris",
  "estimatedCost": 50.00,
  "destinationId": 1
}
```

**Validation:**
- Scheduled date/time must be between destination arrival and departure dates

---

### Budget APIs

#### Get All Budgets for a Trip
```http
GET /api/budgets?tripId=1
Authorization: Bearer <token>
```

#### Create Budget
```http
POST /api/budgets
Authorization: Bearer <token>
Content-Type: application/json

{
  "category": "Accommodation",
  "plannedAmount": 2000.00,
  "actualAmount": 0.00,
  "notes": "Hotels in Paris and Rome",
  "tripId": 1
}
```

---

## Error Responses

### 400 Bad Request
```json
{
  "message": "End date must be after start date"
}
```

### 401 Unauthorized
```json
{
  "message": "Unauthorized"
}
```

### 404 Not Found
```json
{
  "message": "Trip not found"
}
```

### 500 Internal Server Error
```json
{
  "message": "An error occurred processing your request"
}
```

---

## Common Response Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 201 | Created |
| 204 | No Content (successful deletion) |
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (missing/invalid token) |
| 404 | Not Found |
| 500 | Internal Server Error |

---

## Testing with cURL

### Register and Login
```bash
# Register
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"username":"john_doe","email":"john@example.com","password":"SecurePassword123!","fullName":"John Doe"}'

# Login and save token
TOKEN=$(curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"john_doe","password":"SecurePassword123!"}' \
  | jq -r '.token')

# Use token to get trips
curl -X GET https://localhost:5001/api/trips \
  -H "Authorization: Bearer $TOKEN"
```

---

## Testing with Postman

1. Import the API collection (or create requests manually)
2. Set up environment variables:
   - `baseUrl`: `https://localhost:5001`
   - `token`: (will be set after login)
3. Use the Login endpoint and save the token to the environment
4. Use `{{baseUrl}}` and `Bearer {{token}}` in your requests

---

## Rate Limiting & Quotas

Currently, there are no rate limits applied. Consider adding rate limiting for production deployment.

---

## CORS Configuration

The API currently allows all origins (`AllowAll` policy). For production, restrict to specific domains:

```csharp
policy.WithOrigins("https://yourdomain.com")
	  .AllowAnyMethod()
	  .AllowAnyHeader();
```

---

## Additional Notes

- All dates are in ISO 8601 format (UTC)
- JWT tokens expire after 60 minutes (configurable in appsettings.json)
- Cascade delete is enabled (deleting a trip deletes all destinations, activities, and budgets)
- All authenticated endpoints validate user ownership
