import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import authService, { RegisterDto } from '../services/authService';
import { FaPlane } from 'react-icons/fa';

const getRegisterErrorMessage = (err: any): string => {
  const responseData = err?.response?.data;

  if (responseData?.message) {
    return responseData.message;
  }

  const errors = responseData?.errors;
  if (errors && typeof errors === 'object') {
    const messages = Object.values(errors)
      .flatMap((value) => (Array.isArray(value) ? value : [String(value)]))
      .filter(Boolean);

    if (messages.length > 0) {
      return messages.join(' ');
    }
  }

  if (err?.code === 'ERR_NETWORK') {
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
    } catch (err: any) {
      setError(getRegisterErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  return (
    <div className="min-h-screen flex items-center justify-center px-4 py-8">
      <div className="w-full max-w-2xl surface-card rounded-3xl p-6 sm:p-8">
        <div className="text-center mb-6">
          <span className="inline-flex items-center justify-center w-14 h-14 rounded-full bg-teal-100 text-teal-700 mb-3">
            <FaPlane className="text-xl" />
          </span>
          <h1 className="text-3xl sm:text-4xl font-black text-slate-900">Create Your Account</h1>
          <p className="text-slate-600 mt-2">Start building your travel plans in minutes.</p>
        </div>

        <form className="space-y-4" onSubmit={handleSubmit}>
          <h2 className="text-xl sm:text-2xl font-bold text-slate-900">Sign Up</h2>

          {error && (
            <div className="alert alert-error">
              <span>{error}</span>
            </div>
          )}

          <div className="form-control">
            <label className="label">
              <span className="label-text text-slate-700">Username *</span>
            </label>
            <input
              type="text"
              name="username"
              placeholder="Choose a username"
              className="input input-bordered border-slate-200"
              value={formData.username}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-control">
            <label className="label">
              <span className="label-text text-slate-700">Email *</span>
            </label>
            <input
              type="email"
              name="email"
              placeholder="your@email.com"
              className="input input-bordered border-slate-200"
              value={formData.email}
              onChange={handleChange}
              required
            />
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            <div className="form-control">
              <label className="label">
                <span className="label-text text-slate-700">First Name</span>
              </label>
              <input
                type="text"
                name="firstName"
                placeholder="John"
                className="input input-bordered border-slate-200"
                value={formData.firstName}
                onChange={handleChange}
              />
            </div>

            <div className="form-control">
              <label className="label">
                <span className="label-text text-slate-700">Last Name</span>
              </label>
              <input
                type="text"
                name="lastName"
                placeholder="Doe"
                className="input input-bordered border-slate-200"
                value={formData.lastName}
                onChange={handleChange}
              />
            </div>
          </div>

          <div className="form-control">
            <label className="label">
              <span className="label-text text-slate-700">Password *</span>
            </label>
            <input
              type="password"
              name="password"
              placeholder="Min 6 characters"
              className="input input-bordered border-slate-200"
              value={formData.password}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-control">
            <label className="label">
              <span className="label-text text-slate-700">Confirm Password *</span>
            </label>
            <input
              type="password"
              placeholder="Re-enter password"
              className="input input-bordered border-slate-200"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              required
            />
          </div>

          <div className="form-control pt-2">
            <button type="submit" className={`btn border-0 bg-gradient-to-r from-teal-500 to-cyan-500 hover:from-teal-600 hover:to-cyan-600 text-white ${loading ? 'loading' : ''}`} disabled={loading}>
              {loading ? 'Creating account...' : 'Sign Up'}
            </button>
          </div>

          <div className="text-center pt-2">
            <span className="text-sm text-slate-600">Already have an account? </span>
            <Link to="/login" className="link link-info text-sm">Login</Link>
          </div>
        </form>
      </div>
    </div>
  );
};

export default Register;
