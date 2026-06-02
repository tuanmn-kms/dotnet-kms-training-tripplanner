import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import tripService, { Trip } from '../services/tripService';
import { FaPlus, FaMapMarkedAlt, FaCalendarAlt, FaTrash, FaEdit } from 'react-icons/fa';
import { format } from 'date-fns';
import Layout from '../components/Layout/Layout';
import './TripList.css';

const statusConfig: Record<string, { badge: string; label: string }> = {
  Planning: { badge: 'badge-info', label: 'Planning' },
  Confirmed: { badge: 'badge-success', label: 'Confirmed' },
  InProgress: { badge: 'badge-warning', label: 'In Progress' },
  Completed: { badge: 'badge-neutral', label: 'Completed' },
  Cancelled: { badge: 'badge-error', label: 'Cancelled' },
};

const getStatusMeta = (status: string) => {
  const fallback = { badge: 'badge-ghost', label: status };
  const meta = statusConfig[status] || fallback;
  return {
    className: `badge ${meta.badge} badge-sm sm:badge-md`,
    label: meta.label,
  };
};

const TripCard: React.FC<{ trip: Trip; onDelete: (id: number) => Promise<void> }> = ({ trip, onDelete }) => {
  const status = getStatusMeta(trip.status);

  return (
    <article className="trip-card card surface-card transition-all duration-300">
      <div className="card-body trip-card-body p-4 sm:p-6">
        <div className="flex flex-col sm:flex-row sm:justify-between sm:items-start gap-2">
          <h2 className="card-title text-lg sm:text-xl break-words">{trip.name}</h2>
          <span className={status.className}>{status.label}</span>
        </div>

        {trip.description && (
          <p className="text-sm opacity-70 break-words line-clamp-3">{trip.description}</p>
        )}

        <div className="space-y-2 mt-4">
          <div className="trip-date-row flex flex-wrap items-center text-sm gap-y-1">
            <FaCalendarAlt className="mr-2 text-primary" aria-hidden="true" />
            <span>{format(new Date(trip.startDate), 'MMM dd, yyyy')}</span>
            <span className="mx-2" aria-hidden="true">→</span>
            <span>{format(new Date(trip.endDate), 'MMM dd, yyyy')}</span>
          </div>
        </div>

        <div className="trip-card-actions card-actions mt-4 grid grid-cols-1 sm:flex sm:justify-end gap-2">
          <Link to={`/trips/${trip.id}`} className="btn btn-sm w-full sm:w-auto border-0 bg-gradient-to-r from-teal-500 to-cyan-500 hover:from-teal-600 hover:to-cyan-600 text-white">
            View Details
          </Link>
          <Link
            to={`/trips/${trip.id}/edit`}
            className="btn btn-sm btn-ghost w-full sm:w-auto"
            aria-label={`Edit trip ${trip.name}`}
          >
            <FaEdit className="sm:mr-0 mr-2" />
            <span className="sm:hidden">Edit</span>
          </Link>
          <button
            onClick={() => onDelete(trip.id)}
            className="btn btn-sm btn-ghost text-error w-full sm:w-auto"
            aria-label={`Delete trip ${trip.name}`}
          >
            <FaTrash className="sm:mr-0 mr-2" />
            <span className="sm:hidden">Delete</span>
          </button>
        </div>
      </div>
    </article>
  );
};

const TripList: React.FC = () => {
  const [trips, setTrips] = useState<Trip[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    loadTrips();
  }, []);

  const loadTrips = async () => {
    try {
      const data = await tripService.getTrips();
      setTrips(data);
    } catch (err: any) {
      setError('Failed to load trips');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this trip?')) return;

    try {
      await tripService.deleteTrip(id);
      setTrips(trips.filter(t => t.id !== id));
    } catch (err) {
      alert('Failed to delete trip');
    }
  };

  if (loading) {
    return (
      <Layout>
        <div className="flex justify-center items-center min-h-[320px] sm:min-h-[400px]">
          <span className="loading loading-spinner loading-lg"></span>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="trip-list-page space-y-4 sm:space-y-6 max-w-7xl mx-auto">
        <div className="trip-list-header flex flex-col sm:flex-row sm:justify-between sm:items-center gap-3">
          <div className="trip-list-title-wrap">
            <h1 className="text-2xl sm:text-3xl font-black text-slate-900">My Trips</h1>
            <p className="text-sm sm:text-base text-slate-600 mt-1">{trips.length} total {trips.length === 1 ? 'trip' : 'trips'}</p>
          </div>
          <Link to="/trips/new" className="trip-new-btn btn btn-sm sm:btn-md w-full sm:w-auto border-0 bg-gradient-to-r from-teal-500 to-cyan-500 hover:from-teal-600 hover:to-cyan-600 text-white">
            <FaPlus className="mr-2" />
            New Trip
          </Link>
        </div>

        {error && (
          <div className="alert alert-error">
            <span>{error}</span>
          </div>
        )}

        {trips.length === 0 ? (
          <div className="trip-empty-state hero rounded-2xl shadow-lg min-h-[320px] sm:min-h-[400px] px-3 sm:px-0">
            <div className="hero-content text-center">
              <div className="max-w-md">
                <FaMapMarkedAlt className="text-5xl sm:text-6xl mx-auto mb-4 text-primary" />
                <h2 className="text-xl sm:text-2xl font-bold mb-3 sm:mb-4">No trips yet</h2>
                <p className="mb-5 sm:mb-6 text-sm sm:text-base">Start planning your next adventure by creating your first trip!</p>
                <Link to="/trips/new" className="btn btn-primary btn-sm sm:btn-md w-full sm:w-auto">
                  <FaPlus className="mr-2" />
                  Create Your First Trip
                </Link>
              </div>
            </div>
          </div>
        ) : (
          <div className="trip-grid grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 sm:gap-6">
            {trips.map((trip) => (
              <TripCard key={trip.id} trip={trip} onDelete={handleDelete} />
            ))}
          </div>
        )}
      </div>
    </Layout>
  );
};

export default TripList;
