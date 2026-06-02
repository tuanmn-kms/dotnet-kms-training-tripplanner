import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { FaMapMarkedAlt, FaSignOutAlt, FaUser } from 'react-icons/fa';
import { useAuth } from '../../contexts/AuthContext';
import BrandMark from '../ui/BrandMark';

const Navbar: React.FC = () => {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <header className="app-header">
      <div className="page-container flex min-h-16 flex-wrap items-center justify-between gap-3 py-3">
        <BrandMark compact />

        <nav className="w-full sm:w-auto" aria-label="Primary navigation">
          {isAuthenticated ? (
            <div className="flex w-full justify-end gap-2 sm:w-auto">
              <Link to="/trips" className="btn btn-sm sm:btn-md quiet-action">
                <FaMapMarkedAlt aria-hidden="true" className="sm:mr-2" />
                <span className="hidden sm:inline">My Trips</span>
              </Link>

              <div className="dropdown dropdown-end">
                <label tabIndex={0} className="btn btn-circle btn-sm sm:btn-md quiet-action" aria-label="Open account menu">
                  <FaUser aria-hidden="true" className="text-amber-600" />
                </label>
                <ul tabIndex={0} className="menu menu-sm dropdown-content z-[1] mt-3 w-56 rounded-lg border border-slate-200 bg-white p-2 text-slate-800 shadow-lg">
                  <li className="menu-title">
                    <span className="font-semibold text-cyan-700">{user?.username}</span>
                  </li>
                  <li>
                    <button onClick={handleLogout} className="danger-action">
                      <FaSignOutAlt aria-hidden="true" />
                      <span>Logout</span>
                    </button>
                  </li>
                </ul>
              </div>
            </div>
          ) : (
            <div className="flex w-full justify-end gap-2 sm:w-auto">
              <Link to="/login" className="btn btn-sm sm:btn-md quiet-action">Login</Link>
              <Link to="/register" className="btn btn-sm sm:btn-md primary-action">Sign Up</Link>
            </div>
          )}
        </nav>
      </div>
    </header>
  );
};

export default Navbar;
