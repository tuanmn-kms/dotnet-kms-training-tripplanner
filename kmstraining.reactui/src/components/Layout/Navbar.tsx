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
    <div className="navbar bg-gradient-to-r from-purple-600 to-pink-600 text-white shadow-lg">
      <div className="flex-1">
        <Link to="/" className="btn btn-ghost normal-case text-xl text-white hover:bg-purple-700">
          <FaPlane className="mr-2 text-2xl" />
          <span className="font-bold">Trip Planner</span>
        </Link>
      </div>
      <div className="flex-none">
        {isAuthenticated ? (
          <>
            <Link to="/trips" className="btn btn-ghost text-white hover:bg-purple-700 mr-2">
              <FaMapMarkedAlt className="mr-2" />
              My Trips
            </Link>
            <div className="dropdown dropdown-end">
              <label tabIndex={0} className="btn btn-ghost btn-circle avatar">
                <div className="bg-white text-purple-600 rounded-full w-10 h-10 flex items-center justify-center">
                  <FaUser className="text-xl" />
                </div>
              </label>
              <ul tabIndex={0} className="menu menu-sm dropdown-content mt-3 z-[1] p-2 shadow bg-white rounded-box w-52 text-gray-800">
                <li className="menu-title">
                  <span className="text-purple-600 font-semibold">{user?.username}</span>
                </li>
                <li>
                  <a onClick={handleLogout} className="hover:bg-purple-100">
                    <FaSignOutAlt className="text-red-500" /> 
                    <span className="text-red-500">Logout</span>
                  </a>
                </li>
              </ul>
            </div>
          </>
        ) : (
          <div className="flex gap-2">
            <Link to="/login" className="btn btn-ghost text-white hover:bg-purple-700">Login</Link>
            <Link to="/register" className="btn bg-white text-purple-600 hover:bg-gray-100 border-0">
              Sign Up
            </Link>
          </div>
        )}
      </div>
    </div>
  );
};

export default Navbar;
