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
    <div className="min-h-screen flex items-center justify-center px-4 py-8">
      <div className="w-full max-w-md">
        <div className="surface-card rounded-3xl p-6 sm:p-8">
          <div className="text-center mb-7">
            <div className="inline-flex items-center justify-center w-16 h-16 bg-cyan-100 rounded-full mb-4 text-cyan-700">
              <FaPlane className="text-2xl" />
            </div>
            <h1 className="text-3xl sm:text-4xl font-black text-slate-900 mb-2">Welcome Back</h1>
            <p className="text-slate-600">Sign in to continue planning your next trip.</p>
          </div>

          <form onSubmit={handleSubmit} className="space-y-5">
            {error && (
              <div className="alert alert-error shadow-sm text-sm">
                <svg xmlns="http://www.w3.org/2000/svg" className="stroke-current shrink-0 h-6 w-6" fill="none" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <span>{error}</span>
              </div>
            )}

            <div className="form-control">
              <label className="label">
                <span className="label-text font-semibold text-slate-700">Username or Email</span>
              </label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                  <FaUser className="text-slate-400" />
                </div>
                <input
                  type="text"
                  name="usernameOrEmail"
                  value={formData.usernameOrEmail}
                  onChange={handleChange}
                  className="input input-bordered w-full pl-11 border-slate-200 focus:border-cyan-500"
                  placeholder="Enter your username or email"
                  required
                />
              </div>
            </div>

            <div className="form-control">
              <label className="label">
                <span className="label-text font-semibold text-slate-700">Password</span>
              </label>
              <div className="relative">
                <div className="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
                  <FaLock className="text-slate-400" />
                </div>
                <input
                  type="password"
                  name="password"
                  value={formData.password}
                  onChange={handleChange}
                  className="input input-bordered w-full pl-11 border-slate-200 focus:border-cyan-500"
                  placeholder="Enter your password"
                  required
                />
              </div>
            </div>

            <div className="form-control pt-2">
              <button
                type="submit"
                className={`btn border-0 bg-gradient-to-r from-teal-500 to-cyan-500 hover:from-teal-600 hover:to-cyan-600 text-white ${loading ? 'loading' : ''}`}
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

            <div className="divider text-slate-400">OR</div>

            <div className="text-center text-sm sm:text-base text-slate-600">
              Don't have an account?{' '}
              <Link to="/register" className="text-cyan-700 hover:text-cyan-800 font-semibold">
                Create one now
              </Link>
            </div>
          </form>
        </div>

        <div className="text-center mt-5">
          <Link to="/" className="text-slate-600 hover:text-slate-900 text-sm sm:text-base">
            ← Back to Home
          </Link>
        </div>
      </div>
    </div>
  );
};

export default Login;
