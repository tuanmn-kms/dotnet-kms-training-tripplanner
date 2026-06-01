import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import tripService, { CreateTripDto } from '../services/tripService';
import Layout from '../components/Layout/Layout';

const TripForm: React.FC = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<CreateTripDto>({
    name: '',
    description: '',
    startDate: '',
    endDate: '',
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      await tripService.createTrip(formData);
      navigate('/trips');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to create trip');
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  return (
    <Layout>
      <div className="max-w-2xl mx-auto">
        <h1 className="text-3xl font-bold mb-6">Create New Trip</h1>

        <div className="card bg-base-100 shadow-xl">
          <div className="card-body">
            <form onSubmit={handleSubmit} className="space-y-4">
              {error && (
                <div className="alert alert-error">
                  <span>{error}</span>
                </div>
              )}

              <div className="form-control">
                <label className="label">
                  <span className="label-text">Trip Name *</span>
                </label>
                <input
                  type="text"
                  name="name"
                  placeholder="e.g., Summer Vacation 2024"
                  className="input input-bordered"
                  value={formData.name}
                  onChange={handleChange}
                  required
                />
              </div>

              <div className="form-control">
                <label className="label">
                  <span className="label-text">Description</span>
                </label>
                <textarea
                  name="description"
                  placeholder="Describe your trip..."
                  className="textarea textarea-bordered h-24"
                  value={formData.description}
                  onChange={handleChange}
                />
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="form-control">
                  <label className="label">
                    <span className="label-text">Start Date *</span>
                  </label>
                  <input
                    type="date"
                    name="startDate"
                    className="input input-bordered"
                    value={formData.startDate}
                    onChange={handleChange}
                    required
                  />
                </div>

                <div className="form-control">
                  <label className="label">
                    <span className="label-text">End Date *</span>
                  </label>
                  <input
                    type="date"
                    name="endDate"
                    className="input input-bordered"
                    value={formData.endDate}
                    onChange={handleChange}
                    required
                  />
                </div>
              </div>

              <div className="card-actions justify-end pt-4">
                <button
                  type="button"
                  className="btn btn-ghost"
                  onClick={() => navigate('/trips')}
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className={`btn btn-primary ${loading ? 'loading' : ''}`}
                  disabled={loading}
                >
                  {loading ? 'Creating...' : 'Create Trip'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </Layout>
  );
};

export default TripForm;
