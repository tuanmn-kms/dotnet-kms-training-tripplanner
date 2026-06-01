# Docker Setup for KMSTraining.ReactUI

This guide explains how to run the React UI using Docker.

## 📦 Available Docker Configurations

### 1. Production Build (Nginx)
- **File**: `Dockerfile`
- **Purpose**: Optimized production build served by Nginx
- **Port**: 80 (mapped to 3000 on host)
- **Features**: Multi-stage build, gzip compression, health checks

### 2. Development Build (Hot Reload)
- **File**: `Dockerfile.dev`
- **Purpose**: Development server with hot reload
- **Port**: 63452
- **Features**: Volume mounting, live code changes

### 3. Full Stack (API + UI + Database)
- **File**: `docker-compose.full-stack.yml`
- **Purpose**: Complete application stack
- **Services**: SQL Server, .NET API, React UI

---

## 🚀 Quick Start

### Option 1: Production Build (Standalone UI)

```powershell
# Navigate to ReactUI directory
cd KMSTraining.ReactUI

# Build the Docker image
docker build -t kmstraining-reactui:latest .

# Run the container
docker run -d `
  -p 3000:80 `
  --name kmstraining-reactui `
  kmstraining-reactui:latest

# Access the application
# Open browser to: http://localhost:3000
```

### Option 2: Development Build with Hot Reload

```powershell
# Build development image
docker build -t kmstraining-reactui:dev -f Dockerfile.dev .

# Run with volume mounting for live changes
docker run -d `
  -p 63452:63452 `
  -v ${PWD}:/app `
  -v /app/node_modules `
  --name kmstraining-reactui-dev `
  kmstraining-reactui:dev

# Access the application
# Open browser to: http://localhost:63452
```

### Option 3: Using Docker Compose (Recommended)

#### Production Mode
```powershell
# Start the UI only
docker-compose up -d

# View logs
docker-compose logs -f

# Stop
docker-compose down
```

#### Development Mode
```powershell
# Start development server
docker-compose --profile development up -d reactui-dev

# View logs
docker-compose --profile development logs -f reactui-dev
```

### Option 4: Full Stack (API + UI + Database)

```powershell
# Navigate to ReactUI directory
cd KMSTraining.ReactUI

# Start all services
docker-compose -f docker-compose.full-stack.yml up -d

# View logs
docker-compose -f docker-compose.full-stack.yml logs -f

# Stop all services
docker-compose -f docker-compose.full-stack.yml down

# Stop and remove volumes (database data)
docker-compose -f docker-compose.full-stack.yml down -v
```

**Full Stack URLs:**
- React UI: http://localhost:3000
- API: http://localhost:5001
- SQL Server: localhost:1433

---

## 🔧 Configuration

### Environment Variables

Create a `.env` file in the `KMSTraining.ReactUI` directory:

```env
VITE_API_URL=http://localhost:5001/api
```

For Docker:
```env
VITE_API_URL=http://api:8080/api
```

### Build-time Arguments

You can pass environment variables at build time:

```powershell
docker build `
  --build-arg VITE_API_URL=http://localhost:5001/api `
  -t kmstraining-reactui:latest .
```

---

## 📋 Docker Commands Reference

### Build Commands
```powershell
# Production build
docker build -t kmstraining-reactui:latest .

# Development build
docker build -t kmstraining-reactui:dev -f Dockerfile.dev .

# Build with custom API URL
docker build --build-arg VITE_API_URL=https://api.example.com/api -t kmstraining-reactui:latest .
```

### Run Commands
```powershell
# Run production container
docker run -d -p 3000:80 --name reactui kmstraining-reactui:latest

# Run with custom environment
docker run -d -p 3000:80 -e VITE_API_URL=http://api:8080/api --name reactui kmstraining-reactui:latest

# Run development container with volume
docker run -d -p 63452:63452 -v ${PWD}:/app -v /app/node_modules --name reactui-dev kmstraining-reactui:dev
```

### Container Management
```powershell
# View running containers
docker ps

# View logs
docker logs -f kmstraining-reactui

# Stop container
docker stop kmstraining-reactui

# Remove container
docker rm kmstraining-reactui

# Restart container
docker restart kmstraining-reactui
```

### Health Check
```powershell
# Check container health
docker inspect --format='{{json .State.Health}}' kmstraining-reactui

# Manual health check
curl http://localhost:3000/health
```

---

## 🏗️ Multi-Stage Build Explained

The production `Dockerfile` uses a multi-stage build:

### Stage 1: Builder
- Uses Node.js 22 Alpine
- Installs dependencies
- Builds the React app
- Outputs to `/app/dist`

### Stage 2: Production
- Uses Nginx Alpine (lightweight)
- Copies built files from builder
- Serves static files
- ~30MB final image size (vs ~1GB with Node)

---

## 🔒 Security Best Practices

### Nginx Configuration (`nginx.conf`)
- ✅ Security headers (X-Frame-Options, X-Content-Type-Options, etc.)
- ✅ Gzip compression enabled
- ✅ Static asset caching (1 year)
- ✅ Hidden files access denied
- ✅ SPA routing support

### Docker Best Practices
- ✅ Non-root user (nginx runs as nginx user)
- ✅ Multi-stage build (smaller attack surface)
- ✅ `.dockerignore` to exclude sensitive files
- ✅ Health checks configured
- ✅ Minimal base image (Alpine)

---

## 🐛 Troubleshooting

### Issue: Container not starting
```powershell
# Check logs
docker logs kmstraining-reactui

# Check if port is already in use
netstat -ano | findstr :3000
```

### Issue: Cannot connect to API
```powershell
# Verify API URL in container
docker exec kmstraining-reactui cat /usr/share/nginx/html/assets/index-*.js | findstr VITE_API_URL

# Check network connectivity
docker network inspect kmstraining-network
```

### Issue: Hot reload not working in development
```powershell
# Ensure volume is mounted correctly
docker inspect kmstraining-reactui-dev | findstr Mounts

# Restart container
docker restart kmstraining-reactui-dev
```

### Issue: Build fails
```powershell
# Clean Docker cache
docker builder prune

# Rebuild without cache
docker build --no-cache -t kmstraining-reactui:latest .
```

---

## 📊 Container Stats

```powershell
# View resource usage
docker stats kmstraining-reactui

# View container size
docker images kmstraining-reactui
```

**Expected Sizes:**
- Production image: ~30-50 MB
- Development image: ~800 MB - 1 GB

---

## 🚢 Deployment

### Docker Hub
```powershell
# Tag image
docker tag kmstraining-reactui:latest yourusername/kmstraining-reactui:latest

# Push to Docker Hub
docker push yourusername/kmstraining-reactui:latest

# Pull and run
docker pull yourusername/kmstraining-reactui:latest
docker run -d -p 3000:80 yourusername/kmstraining-reactui:latest
```

### Azure Container Registry
```powershell
# Login to ACR
az acr login --name yourregistry

# Tag image
docker tag kmstraining-reactui:latest yourregistry.azurecr.io/kmstraining-reactui:latest

# Push
docker push yourregistry.azurecr.io/kmstraining-reactui:latest
```

### Azure Container Instances
```powershell
az container create `
  --resource-group myResourceGroup `
  --name kmstraining-reactui `
  --image yourregistry.azurecr.io/kmstraining-reactui:latest `
  --dns-name-label kmstraining-ui `
  --ports 80
```

---

## 🔄 CI/CD Integration

### GitHub Actions Example
```yaml
name: Build and Push Docker Image

on:
  push:
	branches: [main]

jobs:
  build:
	runs-on: ubuntu-latest
	steps:
	  - uses: actions/checkout@v3

	  - name: Build Docker image
		run: |
		  cd KMSTraining.ReactUI
		  docker build -t kmstraining-reactui:${{ github.sha }} .

	  - name: Push to registry
		run: |
		  docker push kmstraining-reactui:${{ github.sha }}
```

---

## 📝 Additional Resources

- [Docker Documentation](https://docs.docker.com/)
- [Nginx Configuration](https://nginx.org/en/docs/)
- [Vite Build Guide](https://vitejs.dev/guide/build.html)
- [Multi-stage Builds](https://docs.docker.com/build/building/multi-stage/)

---

## ✅ Verification Checklist

After running the container:

- [ ] Container is running: `docker ps | grep reactui`
- [ ] Health check passes: `docker inspect --format='{{.State.Health.Status}}' kmstraining-reactui`
- [ ] Web UI accessible: http://localhost:3000
- [ ] Health endpoint responds: http://localhost:3000/health
- [ ] API calls work (check browser console)
- [ ] Routing works (navigate to different pages)

---

## 🎯 Next Steps

1. ✅ Build and run the Docker container
2. ✅ Verify the application works
3. ✅ Test API connectivity
4. ✅ Consider using docker-compose for full stack
5. ✅ Set up CI/CD pipeline
6. ✅ Deploy to production environment
