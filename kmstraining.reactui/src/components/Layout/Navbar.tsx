import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { FaPlane, FaUser, FaSignOutAlt, FaMapMarkedAlt } from 'react-icons/fa';

const Navbar: React.FC = () => {
  const { isAuthenticated, user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <header className="sticky top-0 z-50 border-b border-slate-200/80 bg-white/80 backdrop-blur-lg">
      <div className="page-container navbar min-h-[68px] px-0">
        <div className="flex-1">
          <Link to="/" className="btn btn-ghost normal-case text-base sm:text-lg text-slate-800 hover:bg-cyan-50">
            <span className="inline-flex items-center justify-center w-8 h-8 rounded-full bg-cyan-100 text-cyan-700 mr-2">
              <FaPlane className="text-sm" />
            </span>
            <span className="font-extrabold tracking-tight">KMS Trip Planner</span>
          </Link>
        </div>
        <div className="flex-none w-full sm:w-auto">
          {isAuthenticated ? (
            <div className="flex w-full sm:w-auto justify-end gap-2">
              <Link to="/trips" className="btn btn-sm sm:btn-md border-0 bg-slate-100 text-slate-700 hover:bg-cyan-100 hover:text-cyan-800">
                <FaMapMarkedAlt className="sm:mr-2" />
                <span className="hidden sm:inline">My Trips</span>
              </Link>
              <div className="dropdown dropdown-end">
                <label tabIndex={0} className="btn btn-circle avatar btn-sm sm:btn-md border-0 bg-slate-100 hover:bg-amber-100">
                  <div className="text-amber-700 rounded-full w-8 h-8 sm:w-10 sm:h-10 flex items-center justify-center">
                    <FaUser className="text-base sm:text-lg" />
                  </div>
                </label>
                <ul tabIndex={0} className="menu menu-sm dropdown-content mt-3 z-[1] p-2 shadow bg-white rounded-box w-56 text-slate-800 border border-slate-100">
                  <li className="menu-title">
                    <span className="text-cyan-700 font-semibold">{user?.username}</span>
                  </li>
                  <li>
                    <button onClick={handleLogout} className="hover:bg-red-50">
                      <FaSignOutAlt className="text-red-500" />
                      <span className="text-red-500">Logout</span>
                    </button>
                  </li>
                </ul>
              </div>
            </div>
          ) : (
            <div className="flex w-full sm:w-auto justify-end gap-2">
              <Link to="/login" className="btn btn-sm sm:btn-md border-0 bg-slate-100 text-slate-700 hover:bg-cyan-100">Login</Link>
              <Link to="/register" className="btn btn-sm sm:btn-md border-0 bg-gradient-to-r from-teal-500 to-cyan-500 text-white hover:from-teal-600 hover:to-cyan-600">
                Sign Up
              </Link>
            </div>
          )}
        </div>
      </div>
    </header>
  );
};

export default Navbar;
