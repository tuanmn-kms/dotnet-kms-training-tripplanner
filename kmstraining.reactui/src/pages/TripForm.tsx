import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { format } from 'date-fns';
import { FaArrowLeft, FaCalendarAlt, FaMapMarkedAlt, FaSave } from 'react-icons/fa';
import Layout from '../components/Layout/Layout';
import tripService, { CreateTripDto } from '../services/tripService';
import { getApiMessage } from '../utils/errors';

const emptyTripForm: CreateTripDto = {
  name: '',
  description: '',
  startDate: '',
  endDate: '',
};

const toDateInputValue = (dateValue?: string) => {
  if (!dateValue) return '';
  return format(new Date(dateValue), 'yyyy-MM-dd');
};

const TripForm: React.FC = () => {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const tripId = id ? Number(id) : null;
  const isEditing = Number.isFinite(tripId);

  const [formData, setFormData] = useState<CreateTripDto>(emptyTripForm);
  const [initialLoading, setInitialLoading] = useState(isEditing);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!isEditing || !tripId) {
      return;
    }

    let isMounted = true;

    const loadTrip = async () => {
      setInitialLoading(true);
      setError('');

      try {
        const trip = await tripService.getTrip(tripId);
        if (!isMounted) return;

        setFormData({
          name: trip.name,
          description: trip.description || '',
          startDate: toDateInputValue(trip.startDate),
          endDate: toDateInputValue(trip.endDate),
        });
      } catch (err: unknown) {
        if (isMounted) {
          setError(getApiMessage(err) || 'Failed to load trip');
        }
      } finally {
        if (isMounted) {
          setInitialLoading(false);
        }
      }
    };

    loadTrip();

    return () => {
      isMounted = false;
    };
  }, [isEditing, tripId]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      if (isEditing && tripId) {
        await tripService.updateTrip(tripId, formData);
        navigate(`/trips/${tripId}`);
      } else {
        await tripService.createTrip(formData);
        navigate('/trips');
      }
    } catch (err: unknown) {
      setError(getApiMessage(err) || `Failed to ${isEditing ? 'update' : 'create'} trip`);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  if (initialLoading) {
    return (
      <Layout>
        <div className="flex min-h-[320px] items-center justify-center sm:min-h-[400px]">
          <span className="loading loading-spinner loading-lg"></span>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="mx-auto max-w-2xl">
        <button
          type="button"
          className="btn btn-sm quiet-action mb-5"
          onClick={() => navigate(isEditing && tripId ? `/trips/${tripId}` : '/trips')}
        >
          <FaArrowLeft aria-hidden="true" />
          Back
        </button>

        <div className="mb-5 sm:mb-7">
          <p className="page-kicker mb-2">{isEditing ? 'Trip details' : 'New trip'}</p>
          <h1 className="section-title">{isEditing ? 'Edit Trip' : 'Create New Trip'}</h1>
          <p className="mt-2 text-slate-600">
            {isEditing ? 'Update the destination, timeline, and travel notes.' : 'Set your destination, timeline, and travel notes.'}
          </p>
        </div>

        <div className="form-panel p-5 sm:p-6">
          <form onSubmit={handleSubmit} className="space-y-4">
            {error && (
              <div className="alert alert-error">
                <span>{error}</span>
              </div>
            )}

            <div className="form-control">
              <label className="label">
                <span className="label-text font-semibold text-slate-700">Trip Name *</span>
              </label>
              <div className="relative">
                <FaMapMarkedAlt className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" aria-hidden="true" />
                <input
                  type="text"
                  name="name"
                  placeholder="e.g., Summer Vacation 2026"
                  className="input input-bordered field-control pl-11"
                  value={formData.name}
                  onChange={handleChange}
                  required
                />
              </div>
            </div>

            <div className="form-control">
              <label className="label">
                <span className="label-text font-semibold text-slate-700">Description</span>
              </label>
              <textarea
                name="description"
                placeholder="Describe your trip..."
                className="textarea textarea-bordered field-control min-h-28"
                value={formData.description}
                onChange={handleChange}
              />
            </div>

            <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
              <div className="form-control">
                <label className="label">
                  <span className="label-text font-semibold text-slate-700">Start Date *</span>
                </label>
                <div className="relative">
                  <FaCalendarAlt className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" aria-hidden="true" />
                  <input
                    type="date"
                    name="startDate"
                    className="input input-bordered field-control pl-11"
                    value={formData.startDate}
                    onChange={handleChange}
                    required
                  />
                </div>
              </div>

              <div className="form-control">
                <label className="label">
                  <span className="label-text font-semibold text-slate-700">End Date *</span>
                </label>
                <div className="relative">
                  <FaCalendarAlt className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-slate-400" aria-hidden="true" />
                  <input
                    type="date"
                    name="endDate"
                    className="input input-bordered field-control pl-11"
                    value={formData.endDate}
                    onChange={handleChange}
                    required
                  />
                </div>
              </div>
            </div>

            <div className="flex flex-col-reverse justify-end gap-2 pt-4 sm:flex-row">
              <button
                type="button"
                className="btn quiet-action w-full sm:w-auto"
                onClick={() => navigate(isEditing && tripId ? `/trips/${tripId}` : '/trips')}
              >
                Cancel
              </button>
              <button
                type="submit"
                className={`btn primary-action w-full sm:w-auto ${loading ? 'loading' : ''}`}
                disabled={loading}
              >
                {loading ? (
                  isEditing ? 'Saving...' : 'Creating...'
                ) : (
                  <>
                    <FaSave className="mr-2" aria-hidden="true" />
                    {isEditing ? 'Save Changes' : 'Create Trip'}
                  </>
                )}
              </button>
            </div>
          </form>
        </div>
      </div>
    </Layout>
  );
};

export default TripForm;
