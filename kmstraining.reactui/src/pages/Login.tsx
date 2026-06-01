import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import authService, { LoginDto } from '../services/authService';
import { FaPlane, FaUser, FaLock, FaSignInAlt } from 'react-icons/fa';

const getErrorMessage = (err: any): string => {
  const apiMessage = err?.response?.data?.message;
  if (apiMessage) {
    return apiMessage;
  }

  if (err?.code === 'ERR_NETWORK') {
    return 'Cannot reach the API server. Please ensure the backend is running.';
  }

  if (err instanceof Error && err.message) {
    return err.message;
  }

  return 'Login failed. Please check your credentials.';
};

const Login: React.FC = () => {
  const [formData, setFormData] = useState<LoginDto>({
    usernameOrEmail: '',
    password: '',
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    debugger;
    setError('');
    setLoading(true);

    try {
      const payload = {
        usernameOrEmail: formData.usernameOrEmail.trim(),
        password: formData.password,
      };

      const response = await authService.signIn(payload);
      login(response.token, response);
      navigate('/trips');
    } catch (err: any) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-500 via-purple-500 to-pink-500 p-4">
      <div className="max-w-md w-full">
        {/* Logo and Title */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-20 h-20 bg-white rounded-full shadow-lg mb-4">
            <FaPlane className="text-4xl text-purple-600" />
          </div>
          <h1 className="text-4xl font-bold text-white mb-2">Welcome Back! #</h1>
          <p className="text-white text-lg opacity-90">Sign in to continue your journey</p>
        </div>

        {/* Login Card */}
        <div className="card bg-white shadow-2xl">
          <div className="card-body">
            <form onSubmit={handleSubmit} className="space-y-6">
              {error && (
                <div className="alert alert-error shadow-lg">
                  <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
                  </svg>
                  <span>{error}</span>
                </div>
              )}

              {/* Username/Email Field */}
              <div className="form-control">
                <label className="label">
                  <span className="label-text font-semibold text-lg">Username or Email</span>
                </label>
                <div className="relative">
                  <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                    <FaUser className="text-gray-400 text-xl" />
                  </div>
                  <input
                    type="text"
                    name="usernameOrEmail"
                    value={formData.usernameOrEmail}
                    onChange={handleChange}
                    className="input input-bordered input-lg w-full pl-12 text-lg focus:ring-2 focus:ring-purple-500"
                    placeholder="Enter your username or email"
                    required
                  />
                </div>
              </div>

              {/* Password Field */}
              <div className="form-control">
                <label className="label">
                  <span className="label-text font-semibold text-lg">Password</span>
                </label>
                <div className="relative">
                  <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                    <FaLock className="text-gray-400 text-xl" />
                  </div>
                  <input
                    type="password"
                    name="password"
                    value={formData.password}
                    onChange={handleChange}
                    className="input input-bordered input-lg w-full pl-12 text-lg focus:ring-2 focus:ring-purple-500"
                    placeholder="Enter your password"
                    required
                  />
                </div>
              </div>

              {/* Submit Button */}
              <div className="form-control mt-8">
                <button 
                  type="submit" 
                  className={`btn btn-lg bg-gradient-to-r from-purple-600 to-pink-600 hover:from-purple-700 hover:to-pink-700 text-white border-0 shadow-lg ${loading ? 'loading' : ''}`}
                  disabled={loading}
                >
                  {loading ? (
                    'Signing in...'
                  ) : (
                    <>
                      <FaSignInAlt className="mr-2" />
                      Sign In
                    </>
                  )}
                </button>
              </div>
            </form>

            {/* Divider */}
            <div className="divider">OR</div>

            {/* Register Link */}
            <div className="text-center">
              <p className="text-gray-600 text-lg">
                Don't have an account?{' '}
                <Link to="/register" className="text-purple-600 hover:text-purple-800 font-semibold hover:underline">
                  Create one now
                </Link>
              </p>
            </div>
          </div>
        </div>

        {/* Back to Home */}
        <div className="text-center mt-6">
          <Link to="/" className="text-white hover:text-gray-200 text-lg hover:underline">
            ← Back to Home
          </Link>
        </div>
      </div>
    </div>
  );
};

export default Login;
