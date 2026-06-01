# 🚀 How to Execute KMSTraining.ReactUI

## ✅ Current Status

**The React UI is now running!**

📱 **Access the application at:**
- **Local**: http://localhost:63452/
- **Network**: http://10.206.10.10:63452/
- **Network**: http://192.168.1.186:63452/

---

## 🎯 Quick Start (Current Session)

The development server is already running in the background!

### Open in Browser
```
http://localhost:63452/
```

### Stop the Server
Press `Ctrl+C` in the terminal where it's running, or:
```powershell
# Find the process
Get-Process node | Where-Object {$_.Path -like "*node.exe*"}

# Kill it
Stop-Process -Name node -Force
```

---

## 📋 All Execution Methods

### Method 1: NPM Development Server (Recommended for Development) ⭐

```powershell
# Navigate to the ReactUI directory
cd KMSTraining.ReactUI

# Install dependencies (first time only)
npm install

# Start development server
npm run dev

# Access at: http://localhost:63452/
```

**Features:**
- ✅ Hot Module Replacement (HMR) - instant updates
- ✅ TypeScript compilation
- ✅ Fast refresh
- ✅ Source maps for debugging
- ✅ Development-optimized builds

**Stop Server:**
- Press `Ctrl+C` in the terminal

---

### Method 2: NPM Production Build

```powershell
# Navigate to the ReactUI directory
cd KMSTraining.ReactUI

# Build for production
npm run build

# Preview the production build
npm run preview

# Access at: http://localhost:63452/
```

**Features:**
- ✅ Optimized for production
- ✅ Minified JavaScript/CSS
- ✅ Tree-shaking
- ✅ Code splitting
- ✅ Asset optimization

---

### Method 3: Docker (Production-like Environment)

#### Using Helper Script (Easiest)
```powershell
cd KMSTraining.ReactUI
.\docker-start.ps1
```

#### Manual Docker Commands

**Development with Hot Reload:**
```powershell
docker build -t kmstraining-reactui:dev -f Dockerfile.dev .
docker run -d -p 63452:63452 -v ${PWD}:/app -v /app/node_modules --name reactui-dev kmstraining-reactui:dev

# Access at: http://localhost:63452/
```

**Production Build:**
```powershell
docker build -t kmstraining-reactui:latest .
docker run -d -p 3000:80 --name reactui kmstraining-reactui:latest

# Access at: http://localhost:3000/
```

**Stop Docker:**
```powershell
docker stop reactui
# or
docker stop reactui-dev
```

---

### Method 4: Visual Studio (if .esproj is supported)

1. Open the solution in Visual Studio
2. Right-click `KMSTraining.ReactUI` project
3. Select **"Set as Startup Project"**
4. Press **F5** or click the green play button

---

### Method 5: Full Stack (API + UI + Database)

```powershell
cd KMSTraining.ReactUI
docker-compose -f docker-compose.full-stack.yml up -d

# Access UI at: http://localhost:3000/
# Access API at: http://localhost:5001/
```

**Stop:**
```powershell
docker-compose -f docker-compose.full-stack.yml down
```

---

## 🔧 Configuration

### Environment Variables

The React UI uses Vite environment variables. Check `.env` file:

```env
VITE_API_URL=http://localhost:5001/api
```

**To change the API URL:**

1. Edit `.env` file
2. Or create `.env.local` (not committed to git):
```env
VITE_API_URL=https://your-api-url.com/api
```

3. Restart the dev server

---

## 🌐 Available URLs

### Development Server
```
Local:    http://localhost:63452/
Network:  http://10.206.10.10:63452/
Network:  http://192.168.1.186:63452/
Network:  http://172.24.0.1:63452/
```

### Production Docker
```
http://localhost:3000/
```

### Health Check
```
http://localhost:3000/health (Docker only)
```

---

## 📱 Available Routes in the UI

Once running, you can access these pages:

| Route | Description |
|-------|-------------|
| `/` | Home page |
| `/login` | Login page |
| `/register` | User registration |
| `/trips` | Trip list (requires auth) |
| `/trips/new` | Create new trip (requires auth) |
| `/trips/:id` | Trip details (requires auth) |
| `/trips/:id/edit` | Edit trip (requires auth) |

---

## 🛠️ NPM Scripts Reference

```powershell
# Development server (hot reload)
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Run linter
npm run lint
```

---

## 🐛 Troubleshooting

### Issue: Port 63452 already in use

**Solution:**
```powershell
# Find process using the port
netstat -ano | findstr :63452

# Kill the process (replace PID with actual process ID)
taskkill /PID <PID> /F

# Or change the port in vite.config.ts
```

### Issue: "Cannot find module" errors

**Solution:**
```powershell
# Delete node_modules and reinstall
Remove-Item -Recurse -Force node_modules
npm install
```

### Issue: Hot reload not working

**Solution:**
```powershell
# Restart the dev server
# Press Ctrl+C, then run again:
npm run dev
```

### Issue: API calls failing (CORS errors)

**Solution:**
1. Make sure the API is running on http://localhost:5001
2. Check `.env` file has correct `VITE_API_URL`
3. Verify CORS is enabled in the API (it should be)

### Issue: Build fails

**Solution:**
```powershell
# Clear Vite cache
Remove-Item -Recurse -Force node_modules\.vite
npm run dev
```

---

## 🔍 Monitoring & Logs

### View Development Server Logs
The logs appear in the terminal where you ran `npm run dev`

### View Browser Console
1. Open the app in browser
2. Press `F12` to open DevTools
3. Check **Console** tab for errors
4. Check **Network** tab for API calls

### View Docker Logs
```powershell
# Development container
docker logs -f reactui-dev

# Production container
docker logs -f reactui
```

---

## 🚀 Production Deployment Checklist

Before deploying to production:

- [ ] Run `npm run build` successfully
- [ ] Test the production build with `npm run preview`
- [ ] Verify all environment variables are set correctly
- [ ] Update `VITE_API_URL` to production API URL
- [ ] Test API connectivity from production build
- [ ] Verify routing works in production build
- [ ] Check browser console for errors
- [ ] Test on different browsers (Chrome, Firefox, Edge)

---

## 📊 Performance Tips

### Development
```powershell
# Clear cache if slow
Remove-Item -Recurse -Force node_modules\.vite
npm run dev
```

### Production
```powershell
# Build with production optimizations
npm run build

# Analyze bundle size
npm run build -- --mode analyze
```

---

## 🔄 Switching Between Development and Production

### Development Mode
```powershell
npm run dev
# Access: http://localhost:63452/
```

### Production Preview
```powershell
npm run build
npm run preview
# Access: http://localhost:63452/
```

### Production Docker
```powershell
docker build -t kmstraining-reactui:latest .
docker run -d -p 3000:80 --name reactui kmstraining-reactui:latest
# Access: http://localhost:3000/
```

---

## 📚 Additional Resources

- **Vite Documentation**: https://vitejs.dev/
- **React Router**: https://reactrouter.com/
- **Tailwind CSS**: https://tailwindcss.com/
- **DaisyUI**: https://daisyui.com/

---

## ✅ Quick Verification

After starting the server, verify:

1. **Server is running**
   - Check terminal output for "ready in XXXms"
   - See URL: http://localhost:63452/

2. **UI loads in browser**
   - Open http://localhost:63452/
   - Should see the home page

3. **No console errors**
   - Press F12 in browser
   - Check Console tab

4. **Routing works**
   - Click navigation links
   - URL should change

5. **API connectivity** (if API is running)
   - Try login/register
   - Check Network tab in DevTools

---

## 🎯 Next Steps

1. ✅ **Server is already running** at http://localhost:63452/
2. ✅ Open the URL in your browser
3. ✅ Start the API if you want to test full functionality:
   ```powershell
   cd KMSTraining.API
   dotnet run
   ```
4. ✅ Test the application features
5. ✅ Check the browser console for any errors

**Your React UI is ready to use! 🎉**
