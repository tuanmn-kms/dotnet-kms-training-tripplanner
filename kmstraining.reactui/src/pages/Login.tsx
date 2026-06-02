import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { FaArrowLeft, FaLock, FaSignInAlt, FaUser } from 'react-icons/fa';
import { useAuth } from '../contexts/AuthContext';
import authService, { LoginDto } from '../services/authService';
import BrandMark from '../components/ui/BrandMark';
import { asApiError, getApiMessage } from '../utils/errors';

const getErrorMessage = (err: unknown): string => {
  const apiMessage = getApiMessage(err);
  if (apiMessage) {
    return apiMessage;
  }

  const apiError = asApiError(err);
  if (apiError.code === 'ERR_NETWORK') {
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
    } catch (err: unknown) {
      setError(getErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  return (
    <div className="flex min-h-screen items-center justify-center px-4 py-8">
      <div className="w-full max-w-md">
        <div className="mb-6 flex justify-center">
          <BrandMark />
        </div>

        <div className="form-panel p-6 sm:p-8">
          <div className="mb-7 text-center">
            <p className="page-kicker mb-2">Welcome back</p>
            <h1 className="text-3xl font-black text-slate-900 sm:text-4xl">Sign In</h1>
            <p className="mt-2 text-slate-600">Continue planning your next trip.</p>
          </div>

          <form onSubmit={handleSubmit} className="space-y-5">
            {error && (
              <div className="alert alert-error text-sm shadow-sm">
                <span>{error}</span>
              </div>
            )}

            <div className="form-control">
              <label className="label">
                <span className="label-text font-semibold text-slate-700">Username or Email</span>
              </label>
              <div className="relative">
                <div className="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-4">
                  <FaUser className="text-slate-400" aria-hidden="true" />
                </div>
                <input
                  type="text"
                  name="usernameOrEmail"
                  value={formData.usernameOrEmail}
                  onChange={handleChange}
                  className="input input-bordered field-control pl-11"
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
                <div className="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-4">
                  <FaLock className="text-slate-400" aria-hidden="true" />
                </div>
                <input
                  type="password"
                  name="password"
                  value={formData.password}
                  onChange={handleChange}
                  className="input input-bordered field-control pl-11"
                  placeholder="Enter your password"
                  required
                />
              </div>
            </div>

            <button
              type="submit"
              className={`btn w-full primary-action ${loading ? 'loading' : ''}`}
              disabled={loading}
            >
              {loading ? (
                'Signing in...'
              ) : (
                <>
                  <FaSignInAlt className="mr-2" aria-hidden="true" />
                  Sign In
                </>
              )}
            </button>

            <div className="divider text-slate-400">OR</div>

            <div className="text-center text-sm text-slate-600 sm:text-base">
              Don't have an account?{' '}
              <Link to="/register" className="font-semibold text-cyan-700 hover:text-cyan-800">
                Create one now
              </Link>
            </div>
          </form>
        </div>

        <div className="mt-5 text-center">
          <Link to="/" className="inline-flex items-center gap-2 text-sm text-slate-600 hover:text-slate-900 sm:text-base">
            <FaArrowLeft aria-hidden="true" />
            Back to Home
          </Link>
        </div>
      </div>
    </div>
  );
};

export default Login;
