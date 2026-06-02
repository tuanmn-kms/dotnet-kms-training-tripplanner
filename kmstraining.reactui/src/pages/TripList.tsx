import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { format } from 'date-fns';
import { FaArrowRight, FaCalendarAlt, FaEdit, FaMapMarkedAlt, FaPlus, FaTrash } from 'react-icons/fa';
import Layout from '../components/Layout/Layout';
import StatusBadge from '../components/ui/StatusBadge';
import tripService, { Trip } from '../services/tripService';

const TripCard: React.FC<{ trip: Trip; onDelete: (id: number) => Promise<void> }> = ({ trip, onDelete }) => {
  return (
    <article className="trip-card surface-card">
      <div className="trip-card-body p-4 sm:p-5">
        <div className="flex flex-col gap-2 sm:flex-row sm:items-start sm:justify-between">
          <h2 className="text-lg font-bold text-slate-900 sm:text-xl">{trip.name}</h2>
          <StatusBadge status={trip.status} size="sm" />
        </div>

        {trip.description && (
          <p className="mt-3 line-clamp-3 text-sm text-slate-600">{trip.description}</p>
        )}

        <div className="trip-date-row mt-4 flex flex-wrap items-center gap-2 text-sm text-slate-600">
          <FaCalendarAlt className="text-cyan-700" aria-hidden="true" />
          <span>{format(new Date(trip.startDate), 'MMM dd, yyyy')}</span>
          <FaArrowRight className="text-slate-400" aria-hidden="true" />
          <span>{format(new Date(trip.endDate), 'MMM dd, yyyy')}</span>
        </div>

        <div className="trip-card-actions mt-4 grid grid-cols-1 gap-2 sm:flex sm:justify-end">
          <Link to={`/trips/${trip.id}`} className="btn btn-sm primary-action w-full sm:w-auto">
            View Details
          </Link>
          <Link
            to={`/trips/${trip.id}/edit`}
            className="btn btn-sm quiet-action w-full sm:w-auto"
            aria-label={`Edit trip ${trip.name}`}
          >
            <FaEdit aria-hidden="true" />
            <span className="sm:hidden">Edit</span>
          </Link>
          <button
            onClick={() => onDelete(trip.id)}
            className="btn btn-sm quiet-action danger-action w-full sm:w-auto"
            aria-label={`Delete trip ${trip.name}`}
          >
            <FaTrash aria-hidden="true" />
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
    let isMounted = true;

    tripService.getTrips()
      .then((data) => {
        if (isMounted) {
          setTrips(data);
        }
      })
      .catch((err: unknown) => {
        if (isMounted) {
          setError('Failed to load trips');
        }
        console.error(err);
      })
      .finally(() => {
        if (isMounted) {
          setLoading(false);
        }
      });

    return () => {
      isMounted = false;
    };
  }, []);

  const handleDelete = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this trip?')) return;

    try {
      await tripService.deleteTrip(id);
      setTrips((currentTrips) => currentTrips.filter((trip) => trip.id !== id));
    } catch {
      alert('Failed to delete trip');
    }
  };

  if (loading) {
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
      <div className="space-y-5 sm:space-y-6">
        <div className="flex flex-col gap-4 sm:flex-row sm:items-end sm:justify-between">
          <div>
            <p className="page-kicker mb-2">Trips</p>
            <h1 className="section-title">My Trips</h1>
            <p className="mt-1 text-sm text-slate-600 sm:text-base">
              {trips.length} total {trips.length === 1 ? 'trip' : 'trips'}
            </p>
          </div>
          <Link to="/trips/new" className="btn primary-action w-full sm:w-auto">
            <FaPlus className="mr-2" aria-hidden="true" />
            New Trip
          </Link>
        </div>

        {error && (
          <div className="alert alert-error">
            <span>{error}</span>
          </div>
        )}

        {trips.length === 0 ? (
          <div className="empty-state flex min-h-[320px] items-center justify-center p-6 text-center sm:min-h-[400px]">
            <div className="max-w-md">
              <span className="icon-chip mx-auto mb-4 bg-cyan-50 text-cyan-700">
                <FaMapMarkedAlt className="text-xl" aria-hidden="true" />
              </span>
              <h2 className="mb-3 text-xl font-bold text-slate-900 sm:text-2xl">No trips yet</h2>
              <p className="mb-5 text-sm text-slate-600 sm:text-base">
                Start planning your next adventure by creating your first trip.
              </p>
              <Link to="/trips/new" className="btn primary-action w-full sm:w-auto">
                <FaPlus className="mr-2" aria-hidden="true" />
                Create Your First Trip
              </Link>
            </div>
          </div>
        ) : (
          <div className="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3 sm:gap-6">
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
