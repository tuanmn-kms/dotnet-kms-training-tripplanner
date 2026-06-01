# 🐳 Docker Deployment - Quick Reference

## 📋 Prerequisites

- Docker Desktop installed and running
- (Optional) Docker Compose installed
- At least 2GB free disk space

---

## ⚡ Quick Start (Choose One)

### Option 1: Using Helper Script (Easiest) ⭐

**Windows (PowerShell):**
```powershell
cd KMSTraining.ReactUI
.\docker-start.ps1
```

**Linux/Mac:**
```bash
cd KMSTraining.ReactUI
chmod +x docker-start.sh
./docker-start.sh
```

The interactive menu will guide you through all options!

### Option 2: Manual Commands

#### Production Build
```powershell
cd KMSTraining.ReactUI
docker build -t kmstraining-reactui:latest .
docker run -d -p 3000:80 --name kmstraining-reactui kmstraining-reactui:latest
```
**Access at:** http://localhost:3000

#### Development Build (Hot Reload)
```powershell
cd KMSTraining.ReactUI
docker build -t kmstraining-reactui:dev -f Dockerfile.dev .
docker run -d -p 63452:63452 -v ${PWD}:/app -v /app/node_modules --name kmstraining-reactui-dev kmstraining-reactui:dev
```
**Access at:** http://localhost:63452

#### Full Stack (API + UI + DB)
```powershell
cd KMSTraining.ReactUI
docker-compose -f docker-compose.full-stack.yml up -d
```
**Access:**
- UI: http://localhost:3000
- API: http://localhost:5001
- Database: localhost:1433

---

## 📊 Container Status

### Check Running Containers
```powershell
docker ps
```

### View Logs
```powershell
# Production UI
docker logs -f kmstraining-reactui

# Development UI
docker logs -f kmstraining-reactui-dev

# Full Stack
docker-compose -f docker-compose.full-stack.yml logs -f
```

### Health Check
```powershell
# Check health status
docker inspect --format='{{.State.Health.Status}}' kmstraining-reactui

# Manual health check
curl http://localhost:3000/health
```

---

## 🛑 Stop & Clean Up

### Stop Containers
```powershell
# Stop specific container
docker stop kmstraining-reactui

# Stop all
docker stop kmstraining-reactui kmstraining-reactui-dev

# Stop full stack
docker-compose -f docker-compose.full-stack.yml down
```

### Remove Containers
```powershell
docker rm kmstraining-reactui
docker rm kmstraining-reactui-dev
```

### Remove Images
```powershell
docker rmi kmstraining-reactui:latest
docker rmi kmstraining-reactui:dev
```

### Complete Cleanup (Everything)
```powershell
docker-compose -f docker-compose.full-stack.yml down -v
docker rmi kmstraining-reactui:latest kmstraining-reactui:dev
docker system prune -a
```

---

## 🔧 Configuration

### Environment Variables

Create `.env` file:
```env
VITE_API_URL=http://localhost:5001/api
```

For Docker networking:
```env
VITE_API_URL=http://api:8080/api
```

### Custom API URL at Build Time
```powershell
docker build --build-arg VITE_API_URL=https://api.example.com/api -t kmstraining-reactui:latest .
```

---

## 📁 Files Created

| File | Purpose |
|------|---------|
| `Dockerfile` | Production build (Nginx) |
| `Dockerfile.dev` | Development build (hot reload) |
| `docker-compose.yml` | UI-only orchestration |
| `docker-compose.full-stack.yml` | Complete stack |
| `nginx.conf` | Nginx server configuration |
| `.dockerignore` | Exclude files from build |
| `docker-start.ps1` | Helper script (Windows) |
| `docker-start.sh` | Helper script (Linux/Mac) |
| `DOCKER.md` | Comprehensive documentation |

---

## 🎯 Use Cases

### Local Development
```powershell
# Use development build with hot reload
docker build -f Dockerfile.dev -t kmstraining-reactui:dev .
docker run -d -p 63452:63452 -v ${PWD}:/app --name reactui-dev kmstraining-reactui:dev
```

### Production Testing
```powershell
# Use production build
docker build -t kmstraining-reactui:latest .
docker run -d -p 3000:80 --name reactui kmstraining-reactui:latest
```

### Full Application Testing
```powershell
# Use full stack compose
docker-compose -f docker-compose.full-stack.yml up -d
```

---

## 🐛 Troubleshooting

### Port Already in Use
```powershell
# Find process using port
netstat -ano | findstr :3000

# Use different port
docker run -d -p 8080:80 --name reactui kmstraining-reactui:latest
```

### Container Won't Start
```powershell
# Check logs
docker logs kmstraining-reactui

# Inspect container
docker inspect kmstraining-reactui
```

### Build Fails
```powershell
# Clean Docker cache
docker builder prune

# Rebuild without cache
docker build --no-cache -t kmstraining-reactui:latest .
```

### Cannot Connect to API
```powershell
# Verify network
docker network ls
docker network inspect kmstraining-network

# Check API container
docker logs kmstraining-api
```

---

## 📚 Documentation

For detailed information, see:
- **`DOCKER.md`** - Complete Docker guide
- **`docker-compose.yml`** - Service definitions
- **`nginx.conf`** - Web server configuration

---

## ✅ Verification Checklist

After deployment:

- [ ] Container is running: `docker ps | grep reactui`
- [ ] Health check passes: `curl http://localhost:3000/health`
- [ ] UI loads: Open http://localhost:3000 in browser
- [ ] API connectivity works: Check browser console
- [ ] Routing works: Navigate between pages

---

## 🚀 Next Steps

1. ✅ Choose your deployment method (script or manual)
2. ✅ Build and run the container
3. ✅ Verify the application works
4. ✅ Test with the API (if using full stack)
5. ✅ Review logs for any issues

**Need help?** Check `DOCKER.md` for comprehensive documentation!
