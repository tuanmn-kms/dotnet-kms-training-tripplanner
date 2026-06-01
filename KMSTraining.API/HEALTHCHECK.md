# Health Check API

The KMSTraining.API includes comprehensive health check endpoints for monitoring and diagnostics.

## Endpoints

### 1. Basic Health Check (Minimal Response)
```
GET /health
```
Returns a simple health status (200 OK or 503 Service Unavailable).

**Example Response (Healthy):**
```
HTTP/1.1 200 OK
Healthy
```

### 2. Readiness Check
```
GET /health/ready
```
Checks if the API is ready to handle requests (database connection is available).

**Example Response (Ready):**
```
HTTP/1.1 200 OK
Healthy
```

### 3. Detailed Health Status (Recommended)
```
GET /api/health
```
Returns detailed health information including all dependencies.

**Example Response (Healthy):**
```json
{
  "status": "Healthy",
  "duration": "00:00:00.0234567",
  "checks": [
	{
	  "name": "database",
	  "status": "Healthy",
	  "description": null,
	  "duration": "00:00:00.0123456",
	  "exception": null,
	  "data": {}
	}
  ]
}
```

**Example Response (Unhealthy):**
```json
HTTP/1.1 503 Service Unavailable

{
  "status": "Unhealthy",
  "duration": "00:00:00.0234567",
  "checks": [
	{
	  "name": "database",
	  "status": "Unhealthy",
	  "description": null,
	  "duration": "00:00:00.0123456",
	  "exception": "Unable to connect to database",
	  "data": {}
	}
  ]
}
```

### 4. Liveness Probe
```
GET /api/health/live
```
Simple check to verify the API process is running (always returns 200 if the service is up).

**Example Response:**
```json
{
  "status": "Alive",
  "timestamp": "2026-06-01T10:30:45.1234567Z"
}
```

### 5. Readiness Probe (Detailed)
```
GET /api/health/ready
```
Checks if the API is ready to serve requests (includes dependency checks).

**Example Response:**
```json
{
  "status": "Ready",
  "timestamp": "2026-06-01T10:30:45.1234567Z"
}
```

## Health Check Components

The health checks monitor:
- **Database Connectivity**: Validates connection to SQL Server (TripPlannerDB)

## Use Cases

### Local Development
```bash
# Quick check
curl http://localhost:5000/health

# Detailed status
curl http://localhost:5000/api/health
```

### Docker/Kubernetes
Use these endpoints for container orchestration:

**Docker Compose:**
```yaml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:80/health/ready"]
  interval: 30s
  timeout: 10s
  retries: 3
  start_period: 40s
```

**Kubernetes:**
```yaml
livenessProbe:
  httpGet:
	path: /api/health/live
	port: 80
  initialDelaySeconds: 30
  periodSeconds: 10

readinessProbe:
  httpGet:
	path: /api/health/ready
	port: 80
  initialDelaySeconds: 5
  periodSeconds: 10
```

### Azure Application Insights
Health check endpoints can be monitored with availability tests:
- Configure URL ping test to `/health`
- Set up multi-step web test for detailed checks

### Load Balancer Health Probes
Configure your load balancer to use:
- **Health Check Path**: `/health/ready`
- **Interval**: 30 seconds
- **Timeout**: 10 seconds
- **Unhealthy threshold**: 3 consecutive failures

## Status Codes

| Status Code | Meaning |
|------------|---------|
| 200 OK | All health checks passed |
| 503 Service Unavailable | One or more health checks failed |

## Monitoring Best Practices

1. **Use `/health/ready` for readiness probes** - Checks all dependencies
2. **Use `/api/health/live` for liveness probes** - Lightweight process check
3. **Use `/api/health` for detailed diagnostics** - Includes timing and error details
4. **Monitor response times** - Slow health checks may indicate performance issues
5. **Alert on consecutive failures** - Single failures might be transient

## Adding Custom Health Checks

To add additional health checks (e.g., external API, cache, message queue), modify `Program.cs`:

```csharp
builder.Services.AddHealthChecks()
	.AddDbContextCheck<TripPlannerDbContext>("database")
	.AddUrlGroup(new Uri("https://api.example.com/health"), "external-api")
	.AddCheck("custom-check", () => 
	{
		// Custom logic here
		return HealthCheckResult.Healthy("Everything is OK");
	});
```
