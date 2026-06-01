import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import PrivateRoute from './components/PrivateRoute';

// Pages
import Home from './pages/Home';
import Login from './pages/Login';
import Register from './pages/Register';
import TripList from './pages/TripList';
import TripForm from './pages/TripForm';
import TripDetail from './pages/TripDetail';

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />

          <Route path="/trips" element={
            <PrivateRoute>
              <TripList />
            </PrivateRoute>
          } />

          <Route path="/trips/new" element={
            <PrivateRoute>
              <TripForm />
            </PrivateRoute>
          } />

          <Route path="/trips/:id" element={
            <PrivateRoute>
              <TripDetail />
            </PrivateRoute>
          } />

          <Route path="/trips/:id/edit" element={
            <PrivateRoute>
              <TripForm />
            </PrivateRoute>
          } />

          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
