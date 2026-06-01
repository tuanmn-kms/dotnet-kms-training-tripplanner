import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import tripService, { TripDetail, Destination, Activity, Budget, CreateDestinationDto, CreateActivityDto, CreateBudgetDto } from '../services/tripService';
import { FaMapMarkedAlt, FaCalendarAlt, FaMoneyBillWave, FaPlus, FaEdit, FaTrash, FaClock, FaMapMarkerAlt } from 'react-icons/fa';
import { format } from 'date-fns';
import Layout from '../components/Layout/Layout';

const TripDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [trip, setTrip] = useState<TripDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'destinations' | 'budgets'>('destinations');

  // Modal states
  const [showDestinationModal, setShowDestinationModal] = useState(false);
  const [showActivityModal, setShowActivityModal] = useState(false);
  const [showBudgetModal, setShowBudgetModal] = useState(false);
  const [selectedDestination, setSelectedDestination] = useState<number | null>(null);

  // Form states
  const [destinationForm, setDestinationForm] = useState<CreateDestinationDto>({
    name: '',
    country: '',
    city: '',
    description: '',
    arrivalDate: '',
    departureDate: '',
    tripId: parseInt(id!),
  });

  const [activityForm, setActivityForm] = useState<CreateActivityDto>({
    name: '',
    description: '',
    scheduledDateTime: '',
    durationMinutes: 60,
    location: '',
    estimatedCost: 0,
    destinationId: 0,
  });

  const [budgetForm, setBudgetForm] = useState<CreateBudgetDto>({
    category: '',
    plannedAmount: 0,
    actualAmount: 0,
    notes: '',
    tripId: parseInt(id!),
  });

  useEffect(() => {
    loadTrip();
  }, [id]);

  const loadTrip = async () => {
    try {
      const data = await tripService.getTrip(parseInt(id!));
      setTrip(data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateDestination = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await tripService.createDestination(destinationForm);
      setShowDestinationModal(false);
      setDestinationForm({
        name: '',
        country: '',
        city: '',
        description: '',
        arrivalDate: '',
        departureDate: '',
        tripId: parseInt(id!),
      });
      loadTrip();
    } catch (err: any) {
      alert(err.response?.data?.message || 'Failed to create destination');
    }
  };

  const handleCreateActivity = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await tripService.createActivity(activityForm);
      setShowActivityModal(false);
      setActivityForm({
        name: '',
        description: '',
        scheduledDateTime: '',
        durationMinutes: 60,
        location: '',
        estimatedCost: 0,
        destinationId: 0,
      });
      loadTrip();
    } catch (err: any) {
      alert(err.response?.data?.message || 'Failed to create activity');
    }
  };

  const handleCreateBudget = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await tripService.createBudget(budgetForm);
      setShowBudgetModal(false);
      setBudgetForm({
        category: '',
        plannedAmount: 0,
        actualAmount: 0,
        notes: '',
        tripId: parseInt(id!),
      });
      loadTrip();
    } catch (err: any) {
      alert(err.response?.data?.message || 'Failed to create budget');
    }
  };

  const handleDeleteDestination = async (destId: number) => {
    if (!window.confirm('Are you sure? This will delete all activities in this destination.')) return;
    try {
      await tripService.deleteDestination(destId);
      loadTrip();
    } catch (err) {
      alert('Failed to delete destination');
    }
  };

  const handleDeleteActivity = async (activityId: number) => {
    if (!window.confirm('Delete this activity?')) return;
    try {
      await tripService.deleteActivity(activityId);
      loadTrip();
    } catch (err) {
      alert('Failed to delete activity');
    }
  };

  const handleDeleteBudget = async (budgetId: number) => {
    if (!window.confirm('Delete this budget item?')) return;
    try {
      await tripService.deleteBudget(budgetId);
      loadTrip();
    } catch (err) {
      alert('Failed to delete budget');
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

  const getTotalBudget = () => {
    return trip?.budgets.reduce((sum, b) => sum + b.plannedAmount, 0) || 0;
  };

  const getTotalSpent = () => {
    return trip?.budgets.reduce((sum, b) => sum + b.actualAmount, 0) || 0;
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

  if (!trip) {
    return (
      <Layout>
        <div className="alert alert-error">
          <span>Trip not found</span>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="space-y-6">
        {/* Trip Header */}
        <div className="card bg-base-100 shadow-xl">
          <div className="card-body">
            <div className="flex justify-between items-start">
              <div>
                <h1 className="text-3xl font-bold mb-2">{trip.name}</h1>
                <span className={getStatusBadge(trip.status)}>{trip.status}</span>
              </div>
              <Link to={`/trips/${trip.id}/edit`} className="btn btn-ghost">
                <FaEdit /> Edit
              </Link>
            </div>

            {trip.description && (
              <p className="text-lg mt-4">{trip.description}</p>
            )}

            <div className="flex items-center mt-4 text-lg">
              <FaCalendarAlt className="mr-2 text-primary" />
              <span>{format(new Date(trip.startDate), 'MMM dd, yyyy')}</span>
              <span className="mx-3">→</span>
              <span>{format(new Date(trip.endDate), 'MMM dd, yyyy')}</span>
            </div>

            {/* Stats */}
            <div className="stats stats-vertical lg:stats-horizontal shadow mt-6">
              <div className="stat">
                <div className="stat-figure text-primary">
                  <FaMapMarkedAlt className="text-3xl" />
                </div>
                <div className="stat-title">Destinations</div>
                <div className="stat-value text-primary">{trip.destinations.length}</div>
              </div>

              <div className="stat">
                <div className="stat-figure text-secondary">
                  <FaMoneyBillWave className="text-3xl" />
                </div>
                <div className="stat-title">Budget</div>
                <div className="stat-value text-secondary">${getTotalBudget().toFixed(2)}</div>
                <div className="stat-desc">Spent: ${getTotalSpent().toFixed(2)}</div>
              </div>
            </div>
          </div>
        </div>

        {/* Tabs */}
        <div className="tabs tabs-boxed bg-base-100 shadow">
          <a
            className={`tab tab-lg ${activeTab === 'destinations' ? 'tab-active' : ''}`}
            onClick={() => setActiveTab('destinations')}
          >
            <FaMapMarkedAlt className="mr-2" />
            Destinations & Activities
          </a>
          <a
            className={`tab tab-lg ${activeTab === 'budgets' ? 'tab-active' : ''}`}
            onClick={() => setActiveTab('budgets')}
          >
            <FaMoneyBillWave className="mr-2" />
            Budgets
          </a>
        </div>

        {/* Destinations Tab */}
        {activeTab === 'destinations' && (
          <div className="space-y-6">
            <div className="flex justify-between items-center">
              <h2 className="text-2xl font-bold">Destinations</h2>
              <button
                onClick={() => setShowDestinationModal(true)}
                className="btn btn-primary"
              >
                <FaPlus className="mr-2" />
                Add Destination
              </button>
            </div>

            {trip.destinations.length === 0 ? (
              <div className="card bg-base-100 shadow">
                <div className="card-body text-center">
                  <p className="text-lg">No destinations yet. Add your first destination!</p>
                </div>
              </div>
            ) : (
              trip.destinations.map((destination) => (
                <div key={destination.id} className="card bg-base-100 shadow-xl">
                  <div className="card-body">
                    <div className="flex justify-between items-start">
                      <div>
                        <h3 className="card-title text-2xl">{destination.name}</h3>
                        <p className="text-sm opacity-70">
                          {destination.city && `${destination.city}, `}{destination.country}
                        </p>
                      </div>
                      <div className="flex gap-2">
                        <button
                          onClick={() => {
                            setSelectedDestination(destination.id);
                            setActivityForm({ ...activityForm, destinationId: destination.id });
                            setShowActivityModal(true);
                          }}
                          className="btn btn-sm btn-primary"
                        >
                          <FaPlus /> Activity
                        </button>
                        <button
                          onClick={() => handleDeleteDestination(destination.id)}
                          className="btn btn-sm btn-ghost text-error"
                        >
                          <FaTrash />
                        </button>
                      </div>
                    </div>

                    {destination.description && (
                      <p className="mt-2">{destination.description}</p>
                    )}

                    <div className="flex items-center mt-2 text-sm">
                      <FaCalendarAlt className="mr-2" />
                      <span>{format(new Date(destination.arrivalDate), 'MMM dd, yyyy')}</span>
                      <span className="mx-2">→</span>
                      <span>{format(new Date(destination.departureDate), 'MMM dd, yyyy')}</span>
                    </div>

                    {/* Activities for this destination */}
                    {trip.destinations.find(d => d.id === destination.id) && (
                      <div className="mt-4">
                        <h4 className="font-semibold mb-2">Activities:</h4>
                        <div className="space-y-2">
                          {/* Since we don't have activities in the destination object, we'll fetch them */}
                          {/* This is a simplified version - in production, you'd fetch activities per destination */}
                          <p className="text-sm opacity-70">Add activities using the "+ Activity" button above</p>
                        </div>
                      </div>
                    )}
                  </div>
                </div>
              ))
            )}
          </div>
        )}

        {/* Budgets Tab */}
        {activeTab === 'budgets' && (
          <div className="space-y-6">
            <div className="flex justify-between items-center">
              <h2 className="text-2xl font-bold">Budget Breakdown</h2>
              <button
                onClick={() => setShowBudgetModal(true)}
                className="btn btn-primary"
              >
                <FaPlus className="mr-2" />
                Add Budget Item
              </button>
            </div>

            {trip.budgets.length === 0 ? (
              <div className="card bg-base-100 shadow">
                <div className="card-body text-center">
                  <p className="text-lg">No budget items yet. Start planning your expenses!</p>
                </div>
              </div>
            ) : (
              <div className="overflow-x-auto">
                <table className="table table-zebra">
                  <thead>
                    <tr>
                      <th>Category</th>
                      <th>Planned</th>
                      <th>Actual</th>
                      <th>Difference</th>
                      <th>Notes</th>
                      <th>Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {trip.budgets.map((budget) => (
                      <tr key={budget.id}>
                        <td className="font-semibold">{budget.category}</td>
                        <td>${budget.plannedAmount.toFixed(2)}</td>
                        <td>${budget.actualAmount.toFixed(2)}</td>
                        <td className={budget.actualAmount > budget.plannedAmount ? 'text-error' : 'text-success'}>
                          ${(budget.plannedAmount - budget.actualAmount).toFixed(2)}
                        </td>
                        <td>{budget.notes || '-'}</td>
                        <td>
                          <button
                            onClick={() => handleDeleteBudget(budget.id)}
                            className="btn btn-sm btn-ghost text-error"
                          >
                            <FaTrash />
                          </button>
                        </td>
                      </tr>
                    ))}
                    <tr className="font-bold">
                      <td>TOTAL</td>
                      <td>${getTotalBudget().toFixed(2)}</td>
                      <td>${getTotalSpent().toFixed(2)}</td>
                      <td className={getTotalSpent() > getTotalBudget() ? 'text-error' : 'text-success'}>
                        ${(getTotalBudget() - getTotalSpent()).toFixed(2)}
                      </td>
                      <td colSpan={2}></td>
                    </tr>
                  </tbody>
                </table>
              </div>
            )}
          </div>
        )}
      </div>

      {/* Destination Modal */}
      {showDestinationModal && (
        <dialog className="modal modal-open">
          <form method="dialog" className="modal-box" onSubmit={handleCreateDestination}>
            <h3 className="font-bold text-lg mb-4">Add Destination</h3>

            <div className="space-y-4">
              <div className="form-control">
                <label className="label">
                  <span className="label-text">Destination Name *</span>
                </label>
                <input
                  type="text"
                  className="input input-bordered"
                  value={destinationForm.name}
                  onChange={(e) => setDestinationForm({ ...destinationForm, name: e.target.value })}
                  required
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="form-control">
                  <label className="label">
                    <span className="label-text">Country *</span>
                  </label>
                  <input
                    type="text"
                    className="input input-bordered"
                    value={destinationForm.country}
                    onChange={(e) => setDestinationForm({ ...destinationForm, country: e.target.value })}
                    required
                  />
                </div>

                <div className="form-control">
                  <label className="label">
                    <span className="label-text">City</span>
                  </label>
                  <input
                    type="text"
                    className="input input-bordered"
                    value={destinationForm.city}
                    onChange={(e) => setDestinationForm({ ...destinationForm, city: e.target.value })}
                  />
                </div>
              </div>

              <div className="form-control">
                <label className="label">
                  <span className="label-text">Description</span>
                </label>
                <textarea
                  className="textarea textarea-bordered"
                  value={destinationForm.description}
                  onChange={(e) => setDestinationForm({ ...destinationForm, description: e.target.value })}
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="form-control">
                  <label className="label">
                    <span className="label-text">Arrival Date *</span>
                  </label>
                  <input
                    type="date"
                    className="input input-bordered"
                    value={destinationForm.arrivalDate}
                    onChange={(e) => setDestinationForm({ ...destinationForm, arrivalDate: e.target.value })}
                    required
                  />
                </div>

                <div className="form-control">
                  <label className="label">
                    <span className="label-text">Departure Date *</span>
                  </label>
                  <input
                    type="date"
                    className="input input-bordered"
                    value={destinationForm.departureDate}
                    onChange={(e) => setDestinationForm({ ...destinationForm, departureDate: e.target.value })}
                    required
                  />
                </div>
              </div>
            </div>

            <div className="modal-action">
              <button type="button" className="btn" onClick={() => setShowDestinationModal(false)}>
                Cancel
              </button>
              <button type="submit" className="btn btn-primary">
                Add Destination
              </button>
            </div>
          </form>
        </dialog>
      )}

      {/* Activity Modal */}
      {showActivityModal && (
        <dialog className="modal modal-open">
          <form method="dialog" className="modal-box" onSubmit={handleCreateActivity}>
            <h3 className="font-bold text-lg mb-4">Add Activity</h3>

            <div className="space-y-4">
              <div className="form-control">
                <label className="label">
                  <span className="label-text">Activity Name *</span>
                </label>
                <input
                  type="text"
                  className="input input-bordered"
                  value={activityForm.name}
                  onChange={(e) => setActivityForm({ ...activityForm, name: e.target.value })}
                  required
                />
              </div>

              <div className="form-control">
                <label className="label">
                  <span className="label-text">Description</span>
                </label>
                <textarea
                  className="textarea textarea-bordered"
                  value={activityForm.description}
                  onChange={(e) => setActivityForm({ ...activityForm, description: e.target.value })}
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="form-control">
                  <label className="label">
                    <span className="label-text">Date & Time *</span>
                  </label>
                  <input
                    type="datetime-local"
                    className="input input-bordered"
                    value={activityForm.scheduledDateTime}
                    onChange={(e) => setActivityForm({ ...activityForm, scheduledDateTime: e.target.value })}
                    required
                  />
                </div>

                <div className="form-control">
                  <label className="label">
                    <span className="label-text">Duration (minutes)</span>
                  </label>
                  <input
                    type="number"
                    className="input input-bordered"
                    value={activityForm.durationMinutes}
                    onChange={(e) => setActivityForm({ ...activityForm, durationMinutes: parseInt(e.target.value) })}
                  />
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="form-control">
                  <label className="label">
                    <span className="label-text">Location</span>
                  </label>
                  <input
                    type="text"
                    className="input input-bordered"
                    value={activityForm.location}
                    onChange={(e) => setActivityForm({ ...activityForm, location: e.target.value })}
                  />
                </div>

                <div className="form-control">
                  <label className="label">
                    <span className="label-text">Estimated Cost ($)</span>
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    className="input input-bordered"
                    value={activityForm.estimatedCost}
                    onChange={(e) => setActivityForm({ ...activityForm, estimatedCost: parseFloat(e.target.value) })}
                  />
                </div>
              </div>
            </div>

            <div className="modal-action">
              <button type="button" className="btn" onClick={() => setShowActivityModal(false)}>
                Cancel
              </button>
              <button type="submit" className="btn btn-primary">
                Add Activity
              </button>
            </div>
          </form>
        </dialog>
      )}

      {/* Budget Modal */}
      {showBudgetModal && (
        <dialog className="modal modal-open">
          <form method="dialog" className="modal-box" onSubmit={handleCreateBudget}>
            <h3 className="font-bold text-lg mb-4">Add Budget Item</h3>

            <div className="space-y-4">
              <div className="form-control">
                <label className="label">
                  <span className="label-text">Category *</span>
                </label>
                <select
                  className="select select-bordered"
                  value={budgetForm.category}
                  onChange={(e) => setBudgetForm({ ...budgetForm, category: e.target.value })}
                  required
                >
                  <option value="">Select category</option>
                  <option value="Accommodation">Accommodation</option>
                  <option value="Transportation">Transportation</option>
                  <option value="Food">Food</option>
                  <option value="Activities">Activities</option>
                  <option value="Shopping">Shopping</option>
                  <option value="Other">Other</option>
                </select>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div className="form-control">
                  <label className="label">
                    <span className="label-text">Planned Amount ($) *</span>
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    className="input input-bordered"
                    value={budgetForm.plannedAmount}
                    onChange={(e) => setBudgetForm({ ...budgetForm, plannedAmount: parseFloat(e.target.value) })}
                    required
                  />
                </div>

                <div className="form-control">
                  <label className="label">
                    <span className="label-text">Actual Amount ($)</span>
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    className="input input-bordered"
                    value={budgetForm.actualAmount}
                    onChange={(e) => setBudgetForm({ ...budgetForm, actualAmount: parseFloat(e.target.value) })}
                  />
                </div>
              </div>

              <div className="form-control">
                <label className="label">
                  <span className="label-text">Notes</span>
                </label>
                <textarea
                  className="textarea textarea-bordered"
                  value={budgetForm.notes}
                  onChange={(e) => setBudgetForm({ ...budgetForm, notes: e.target.value })}
                />
              </div>
            </div>

            <div className="modal-action">
              <button type="button" className="btn" onClick={() => setShowBudgetModal(false)}>
                Cancel
              </button>
              <button type="submit" className="btn btn-primary">
                Add Budget
              </button>
            </div>
          </form>
        </dialog>
      )}
    </Layout>
  );
};

export default TripDetail;
