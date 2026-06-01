import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import tripService, { Trip } from '../services/tripService';
import { FaPlus, FaMapMarkedAlt, FaCalendarAlt, FaTrash, FaEdit } from 'react-icons/fa';
import { format } from 'date-fns';
import Layout from '../components/Layout/Layout';

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

  const getStatusBadge = (status: string) => {
    const statusColors: Record<string, string> = {
      Planning: 'badge-info',
      Confirmed: 'badge-success',
      InProgress: 'badge-warning',
      Completed: 'badge-neutral',
      Cancelled: 'badge-error',
    };
    return `badge ${statusColors[status] || 'badge-ghost'}`;
  };

  if (loading) {
    return (
      <Layout>
        <div className="flex justify-center items-center min-h-[400px]">
          <span className="loading loading-spinner loading-lg"></span>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="space-y-6">
        <div className="flex justify-between items-center">
          <h1 className="text-3xl font-bold">My Trips</h1>
          <Link to="/trips/new" className="btn btn-primary">
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
          <div className="hero bg-base-100 rounded-lg shadow-lg min-h-[400px]">
            <div className="hero-content text-center">
              <div className="max-w-md">
                <FaMapMarkedAlt className="text-6xl mx-auto mb-4 text-primary" />
                <h2 className="text-2xl font-bold mb-4">No trips yet</h2>
                <p className="mb-6">Start planning your next adventure by creating your first trip!</p>
                <Link to="/trips/new" className="btn btn-primary">
                  <FaPlus className="mr-2" />
                  Create Your First Trip
                </Link>
              </div>
            </div>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {trips.map((trip) => (
              <div key={trip.id} className="card bg-base-100 shadow-xl hover:shadow-2xl transition-shadow">
                <div className="card-body">
                  <div className="flex justify-between items-start">
                    <h2 className="card-title">{trip.name}</h2>
                    <span className={getStatusBadge(trip.status)}>{trip.status}</span>
                  </div>

                  {trip.description && (
                    <p className="text-sm opacity-70">{trip.description}</p>
                  )}

                  <div className="space-y-2 mt-4">
                    <div className="flex items-center text-sm">
                      <FaCalendarAlt className="mr-2 text-primary" />
                      <span>{format(new Date(trip.startDate), 'MMM dd, yyyy')}</span>
                      <span className="mx-2">→</span>
                      <span>{format(new Date(trip.endDate), 'MMM dd, yyyy')}</span>
                    </div>
                  </div>

                  <div className="card-actions justify-end mt-4">
                    <Link to={`/trips/${trip.id}`} className="btn btn-sm btn-primary">
                      View Details
                    </Link>
                    <Link to={`/trips/${trip.id}/edit`} className="btn btn-sm btn-ghost">
                      <FaEdit />
                    </Link>
                    <button onClick={() => handleDelete(trip.id)} className="btn btn-sm btn-ghost text-error">
                      <FaTrash />
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </Layout>
  );
};

export default TripList;
