import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { FaArrowLeft, FaEnvelope, FaIdBadge, FaLock, FaUser, FaUserPlus } from 'react-icons/fa';
import { useAuth } from '../contexts/AuthContext';
import authService, { RegisterDto } from '../services/authService';
import BrandMark from '../components/ui/BrandMark';
import { asApiError, getApiMessage, getValidationMessages } from '../utils/errors';

const getRegisterErrorMessage = (err: unknown): string => {
  const apiMessage = getApiMessage(err);
  if (apiMessage) {
    return apiMessage;
  }

  const validationMessages = getValidationMessages(err);
  if (validationMessages.length > 0) {
    return validationMessages.join(' ');
  }

  if (asApiError(err).code === 'ERR_NETWORK') {
    return 'Cannot reach the API server. Please make sure https://localhost:7777 is running.';
  }

  return 'Registration failed. Please try again.';
};

const Register: React.FC = () => {
  const [formData, setFormData] = useState<RegisterDto>({
    username: '',
    email: '',
    password: '',
    firstName: '',
    lastName: '',
  });
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (formData.password !== confirmPassword) {
      setError('Passwords do not match');
      return;
    }

    if (formData.password.length < 6) {
      setError('Password must be at least 6 characters long');
      return;
    }

    setLoading(true);

    try {
      const payload: RegisterDto = {
        username: formData.username.trim(),
        email: formData.email.trim(),
        password: formData.password,
        firstName: formData.firstName?.trim() || undefined,
        lastName: formData.lastName?.trim() || undefined,
      };

      const response = await authService.register(payload);
      login(response.token, response);
      navigate('/trips');
    } catch (err: unknown) {
      setError(getRegisterErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  return (
    <div className="flex min-h-screen items-center justify-center px-4 py-8">
      <div className="w-full max-w-2xl">
        <div className="mb-6 flex justify-center">
          <BrandMark />
        </div>

        <div className="form-panel p-6 sm:p-8">
          <div className="mb-6 text-center">
            <p className="page-kicker mb-2">New account</p>
            <h1 className="text-3xl font-black text-slate-900 sm:text-4xl">Create Your Account</h1>
            <p className="mt-2 text-slate-600">Start building your travel plans in minutes.</p>
          </div>

          <form className="space-y-4" onSubmit={handleSubmit}>
            {error && (
              <div className="alert alert-error">
                <span>{error}</span>
              </div>
            )}

            <div className="form-control">
              <label className="label">
                <span className="label-text font-semibold text-slate-700">Username *</span>
              </label>
              <div className="relative">
                <FaUser className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" aria-hidden="true" />
                <input
                  type="text"
                  name="username"
                  placeholder="Choose a username"
                  className="input input-bordered field-control pl-11"
                  value={formData.username}
                  onChange={handleChange}
                  required
                />
              </div>
            </div>

            <div className="form-control">
              <label className="label">
                <span className="label-text font-semibold text-slate-700">Email *</span>
              </label>
              <div className="relative">
                <FaEnvelope className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" aria-hidden="true" />
                <input
                  type="email"
                  name="email"
                  placeholder="you@example.com"
                  className="input input-bordered field-control pl-11"
                  value={formData.email}
                  onChange={handleChange}
                  required
                />
              </div>
            </div>

            <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
              <div className="form-control">
                <label className="label">
                  <span className="label-text font-semibold text-slate-700">First Name</span>
                </label>
                <div className="relative">
                  <FaIdBadge className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" aria-hidden="true" />
                  <input
                    type="text"
                    name="firstName"
                    placeholder="John"
                    className="input input-bordered field-control pl-11"
                    value={formData.firstName}
                    onChange={handleChange}
                  />
                </div>
              </div>

              <div className="form-control">
                <label className="label">
                  <span className="label-text font-semibold text-slate-700">Last Name</span>
                </label>
                <div className="relative">
                  <FaIdBadge className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" aria-hidden="true" />
                  <input
                    type="text"
                    name="lastName"
                    placeholder="Doe"
                    className="input input-bordered field-control pl-11"
                    value={formData.lastName}
                    onChange={handleChange}
                  />
                </div>
              </div>
            </div>

            <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
              <div className="form-control">
                <label className="label">
                  <span className="label-text font-semibold text-slate-700">Password *</span>
                </label>
                <div className="relative">
                  <FaLock className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" aria-hidden="true" />
                  <input
                    type="password"
                    name="password"
                    placeholder="Min 6 characters"
                    className="input input-bordered field-control pl-11"
                    value={formData.password}
                    onChange={handleChange}
                    required
                  />
                </div>
              </div>

              <div className="form-control">
                <label className="label">
                  <span className="label-text font-semibold text-slate-700">Confirm Password *</span>
                </label>
                <div className="relative">
                  <FaLock className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" aria-hidden="true" />
                  <input
                    type="password"
                    placeholder="Re-enter password"
                    className="input input-bordered field-control pl-11"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    required
                  />
                </div>
              </div>
            </div>

            <button type="submit" className={`btn w-full primary-action ${loading ? 'loading' : ''}`} disabled={loading}>
              {loading ? (
                'Creating account...'
              ) : (
                <>
                  <FaUserPlus className="mr-2" aria-hidden="true" />
                  Sign Up
                </>
              )}
            </button>

            <div className="pt-2 text-center">
              <span className="text-sm text-slate-600">Already have an account? </span>
              <Link to="/login" className="text-sm font-semibold text-cyan-700 hover:text-cyan-800">Login</Link>
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

export default Register;
