# 🚀 Quick Start - Trip Planner UI

## Get Started in 3 Steps

### Step 1: Install Dependencies
```bash
cd KMSTraining.ReactUI
npm install
```

### Step 2: Start the Development Server
```bash
npm run dev
```

### Step 3: Open in Browser
Navigate to `http://localhost:5173`

## ✅ What You'll See

### 1. Home Page
- Beautiful landing page with features
- Sign up or log in buttons

### 2. Register
1. Click "Sign Up" or "Get Started Free"
2. Fill in your details
3. Click "Sign Up"
4. You'll be automatically logged in and redirected to trips

### 3. Create Your First Trip
1. Click "New Trip"
2. Enter trip name and dates
3. Click "Create Trip"

### 4. Add Destinations
1. Open your trip
2. Click "Add Destination"
3. Fill in destination details
4. Click "Add Destination"

### 5. Plan Activities
1. Click "+ Activity" on a destination
2. Enter activity details
3. Set date, time, and duration

### 6. Track Budget
1. Switch to "Budgets" tab
2. Click "Add Budget Item"
3. Select category
4. Enter planned amount

## 🎯 Sample Data to Try

### Trip Example
- **Name**: Summer European Tour
- **Start Date**: 2024-07-01
- **End Date**: 2024-07-15

### Destination Examples
1. **Paris, France**
   - Arrival: 2024-07-01
   - Departure: 2024-07-05

2. **Rome, Italy**
   - Arrival: 2024-07-06
   - Departure: 2024-07-10

### Activity Examples
- **Eiffel Tower Visit**
  - Date: 2024-07-02 14:00
  - Duration: 120 minutes
  - Cost: $30

- **Colosseum Tour**
  - Date: 2024-07-07 09:00
  - Duration: 180 minutes
  - Cost: $45

### Budget Examples
- **Accommodation**: $2000 planned
- **Food**: $800 planned
- **Transportation**: $500 planned
- **Activities**: $400 planned

## 📱 Features

✅ **Responsive Design** - Works on mobile, tablet, desktop
✅ **Beautiful UI** - Modern design with DaisyUI
✅ **Dark Mode Ready** - Multiple theme support
✅ **Real-time Updates** - Instant feedback
✅ **Form Validation** - Error handling
✅ **Secure Auth** - JWT-based authentication

## 🎨 UI Highlights

- **Cards** - Clean, modern trip cards
- **Modals** - Smooth popup forms
- **Tables** - Budget tracking tables
- **Stats** - Visual trip statistics
- **Icons** - React Icons throughout
- **Animations** - Smooth transitions

## ⚠️ Prerequisites

Make sure the API is running:
```bash
cd KMSTraining.API
dotnet run
```

API should be at: `https://localhost:5001`

## 🔧 Configuration

Edit `.env` if your API URL is different:
```
VITE_API_URL=https://localhost:5001/api
```

## 🐛 Common Issues

**Issue**: API connection failed
- **Solution**: Check if API is running on port 5001

**Issue**: CORS error
- **Solution**: Ensure API has CORS configured for `http://localhost:5173`

**Issue**: Page not found
- **Solution**: Refresh the page or check routes in App.tsx

## 📚 Tech Stack

- React 19 + TypeScript
- Vite (build tool)
- React Router (navigation)
- Tailwind CSS (styling)
- DaisyUI (components)
- Axios (API calls)

## 🎉 You're Ready!

Start creating trips and planning your adventures! 🌍✈️

---

For detailed documentation, see [README_UI.md](README_UI.md)
