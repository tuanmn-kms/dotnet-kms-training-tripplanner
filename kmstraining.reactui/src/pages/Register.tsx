import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import authService, { RegisterDto } from '../services/authService';
import { FaPlane, FaUser, FaEnvelope, FaLock, FaUserPlus, FaUserCircle } from 'react-icons/fa';

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
    return 'Cannot reach the API server. Please make sure https://localhost:44391 is running.';
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
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-green-500 via-blue-500 to-purple-500 p-4">
      <div className="hero-content flex-col lg:flex-row-reverse">
        <div className="text-center lg:text-left lg:ml-8">
          <h1 className="text-5xl font-bold flex items-center justify-center lg:justify-start">
            <FaPlane className="mr-4 text-primary" />
            Trip Planner
          </h1>
          <p className="py-6">
            Join us today and start planning your dream adventures!
          </p>
        </div>
        <div className="card flex-shrink-0 w-full max-w-sm shadow-2xl bg-base-100">
          <form className="card-body" onSubmit={handleSubmit}>
            <h2 className="text-2xl font-bold text-center mb-4">Sign Up</h2>

            {error && (
              <div className="alert alert-error">
                <span>{error}</span>
              </div>
            )}

            <div className="form-control">
              <label className="label">
                <span className="label-text">Username *</span>
              </label>
              <input
                type="text"
                name="username"
                placeholder="Choose a username"
                className="input input-bordered"
                value={formData.username}
                onChange={handleChange}
                required
              />
            </div>

            <div className="form-control">
              <label className="label">
                <span className="label-text">Email *</span>
              </label>
              <input
                type="email"
                name="email"
                placeholder="your@email.com"
                className="input input-bordered"
                value={formData.email}
                onChange={handleChange}
                required
              />
            </div>

            <div className="grid grid-cols-2 gap-2">
              <div className="form-control">
                <label className="label">
                  <span className="label-text">First Name</span>
                </label>
                <input
                  type="text"
                  name="firstName"
                  placeholder="John"
                  className="input input-bordered"
                  value={formData.firstName}
                  onChange={handleChange}
                />
              </div>

              <div className="form-control">
                <label className="label">
                  <span className="label-text">Last Name</span>
                </label>
                <input
                  type="text"
                  name="lastName"
                  placeholder="Doe"
                  className="input input-bordered"
                  value={formData.lastName}
                  onChange={handleChange}
                />
              </div>
            </div>

            <div className="form-control">
              <label className="label">
                <span className="label-text">Password *</span>
              </label>
              <input
                type="password"
                name="password"
                placeholder="Min 6 characters"
                className="input input-bordered"
                value={formData.password}
                onChange={handleChange}
                required
              />
            </div>

            <div className="form-control">
              <label className="label">
                <span className="label-text">Confirm Password *</span>
              </label>
              <input
                type="password"
                placeholder="Re-enter password"
                className="input input-bordered"
                value={confirmPassword}
                onChange={(e) => setConfirmPassword(e.target.value)}
                required
              />
            </div>

            <div className="form-control mt-6">
              <button type="submit" className={`btn btn-primary ${loading ? 'loading' : ''}`} disabled={loading}>
                {loading ? 'Creating account...' : 'Sign Up'}
              </button>
            </div>

            <div className="text-center mt-4">
              <span className="text-sm">Already have an account? </span>
              <Link to="/login" className="link link-primary text-sm">Login</Link>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

export default Register;
