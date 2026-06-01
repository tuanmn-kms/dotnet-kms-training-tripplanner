# Trip Planner React UI

A modern, responsive React application for planning trips, destinations, activities, and budgets. Built with React, TypeScript, Tailwind CSS, and DaisyUI.

## 🚀 Features

- **User Authentication** - Register, login, and secure JWT-based authentication
- **Trip Management** - Create, view, update, and delete trips
- **Destination Planning** - Add destinations to trips with dates
- **Activity Scheduling** - Plan activities at each destination
- **Budget Tracking** - Track planned vs actual spending by category
- **Responsive Design** - Beautiful UI that works on all devices
- **Modern Stack** - React 19, TypeScript, Vite, Tailwind CSS, DaisyUI

## 📋 Prerequisites

- Node.js 18+ and npm
- Trip Planner API running on `https://localhost:5001`

## 🔧 Installation

1. **Install dependencies:**
   ```bash
   npm install
   ```

2. **Configure environment:**
   ```bash
   cp .env.example .env
   ```

   Update `.env` if your API URL is different:
   ```
   VITE_API_URL=https://localhost:5001/api
   ```

3. **Run the development server:**
   ```bash
   npm run dev
   ```

   The app will be available at `http://localhost:5173`

## 🏗️ Project Structure

```
src/
├── components/           # Reusable components
│   ├── Layout/          # Layout components (Navbar, Layout)
│   └── PrivateRoute.tsx # Protected route component
├── contexts/            # React contexts
│   └── AuthContext.tsx  # Authentication context
├── pages/               # Page components
│   ├── Home.tsx        # Landing page
│   ├── Login.tsx       # Login page
│   ├── Register.tsx    # Registration page
│   ├── TripList.tsx    # Trips list page
│   ├── TripForm.tsx    # Create/edit trip form
│   └── TripDetail.tsx  # Trip details with destinations/budgets
├── services/            # API services
│   ├── api.ts          # Axios instance with interceptors
│   ├── authService.ts  # Authentication API calls
│   └── tripService.ts  # Trip/destination/activity/budget API calls
├── App.tsx             # Main app with routing
├── main.tsx            # App entry point
└── index.css           # Global styles with Tailwind
```

## 🎨 Technologies Used

### Core
- **React 19** - UI library
- **TypeScript** - Type safety
- **Vite** - Build tool and dev server
- **React Router 6** - Client-side routing

### UI & Styling
- **Tailwind CSS** - Utility-first CSS framework
- **DaisyUI** - Tailwind CSS component library
- **React Icons** - Icon library

### Data & State
- **Axios** - HTTP client
- **React Context** - State management
- **date-fns** - Date formatting

## 🌐 Available Pages

### Public Pages
- `/` - Home/Landing page
- `/login` - User login
- `/register` - User registration

### Protected Pages (Requires Authentication)
- `/trips` - List all user trips
- `/trips/new` - Create new trip
- `/trips/:id` - View trip details
- `/trips/:id/edit` - Edit trip

## 🔐 Authentication

The app uses JWT token-based authentication:

1. User logs in or registers
2. API returns JWT token
3. Token stored in localStorage
4. Token sent with every API request via Authorization header
5. Protected routes redirect to login if no token

## 🎨 UI Components

The app uses DaisyUI components which include:

- **Cards** - For trip, destination, and budget items
- **Modals** - For creating destinations, activities, and budgets
- **Forms** - For all input forms
- **Tables** - For budget breakdown
- **Badges** - For trip status
- **Tabs** - For switching between destinations and budgets
- **Stats** - For displaying trip statistics
- **Hero** - For landing page sections

## 📱 Responsive Design

The application is fully responsive with breakpoints:

- **Mobile** - Single column layouts
- **Tablet** - 2-column grids
- **Desktop** - 3-column grids and expanded layouts

## 🔨 Build Commands

```bash
# Development
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Lint code
npm run lint
```

## 🌈 DaisyUI Themes

The app includes multiple themes. You can switch themes by adding the `data-theme` attribute to the HTML element:

Available themes:
- light (default)
- dark
- cupcake
- corporate
- synthwave
- retro
- cyberpunk
- valentine
- halloween
- and more...

## 🔗 API Integration

The app connects to the Trip Planner API with the following endpoints:

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user

### Trips
- `GET /api/trips` - Get all trips
- `GET /api/trips/:id` - Get trip details
- `POST /api/trips` - Create trip
- `PUT /api/trips/:id` - Update trip
- `DELETE /api/trips/:id` - Delete trip

### Destinations
- `GET /api/destinations?tripId=:id` - Get destinations
- `POST /api/destinations` - Create destination
- `DELETE /api/destinations/:id` - Delete destination

### Activities
- `GET /api/activities?destinationId=:id` - Get activities
- `POST /api/activities` - Create activity
- `DELETE /api/activities/:id` - Delete activity

### Budgets
- `GET /api/budgets?tripId=:id` - Get budgets
- `POST /api/budgets` - Create budget
- `PUT /api/budgets/:id` - Update budget
- `DELETE /api/budgets/:id` - Delete budget

## 🐛 Troubleshooting

### API Connection Issues

If you see CORS errors:
1. Ensure the API is running on `https://localhost:5001`
2. Check that CORS is enabled in the API's `Program.cs`
3. Verify the API URL in `.env` file

### Build Errors

If you encounter build errors:
```bash
# Clear node_modules and reinstall
rm -rf node_modules package-lock.json
npm install

# Clear Vite cache
rm -rf .vite
npm run dev
```

## 🎯 Key Features Walkthrough

### 1. Create a Trip
1. Login or register
2. Click "New Trip" button
3. Fill in trip details (name, description, dates)
4. Click "Create Trip"

### 2. Add Destinations
1. Open a trip
2. Click "Add Destination"
3. Enter destination details
4. Arrival and departure dates must be within trip dates

### 3. Plan Activities
1. In trip details, click "+ Activity" on a destination
2. Fill in activity details (name, time, duration, cost)
3. Activities must be scheduled within destination dates

### 4. Track Budget
1. Switch to "Budgets" tab
2. Click "Add Budget Item"
3. Select category and enter amounts
4. View budget summary and tracking

## 📄 License

MIT License

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📧 Support

For issues or questions, please create an issue in the repository.

---

Built with ❤️ using React, TypeScript, Tailwind CSS, and DaisyUI
