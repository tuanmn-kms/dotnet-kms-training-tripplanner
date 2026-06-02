import React, { useCallback, useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { format } from 'date-fns';
import {
  FaArrowLeft,
  FaArrowRight,
  FaCalendarAlt,
  FaEdit,
  FaMapMarkedAlt,
  FaMapMarkerAlt,
  FaMoneyBillWave,
  FaPlus,
  FaTrash,
} from 'react-icons/fa';
import Layout from '../components/Layout/Layout';
import StatusBadge from '../components/ui/StatusBadge';
import tripService, { CreateActivityDto, CreateBudgetDto, CreateDestinationDto, TripDetail } from '../services/tripService';
import { getApiMessage } from '../utils/errors';

const currency = (value: number) => `$${value.toFixed(2)}`;

const toNumber = (value: string) => {
  const parsed = Number(value);
  return Number.isFinite(parsed) ? parsed : 0;
};

const TripDetailPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const tripId = Number(id);

  const [trip, setTrip] = useState<TripDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'destinations' | 'budgets'>('destinations');
  const [showDestinationModal, setShowDestinationModal] = useState(false);
  const [showActivityModal, setShowActivityModal] = useState(false);
  const [showBudgetModal, setShowBudgetModal] = useState(false);

  const [destinationForm, setDestinationForm] = useState<CreateDestinationDto>({
    name: '',
    country: '',
    city: '',
    description: '',
    arrivalDate: '',
    departureDate: '',
    tripId,
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
    tripId,
  });

  const refreshTrip = useCallback(async () => {
    try {
      const data = await tripService.getTrip(tripId);
      setTrip(data);
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  }, [tripId]);

  useEffect(() => {
    let isMounted = true;

    tripService.getTrip(tripId)
      .then((data) => {
        if (isMounted) {
          setTrip(data);
        }
      })
      .catch((err: unknown) => {
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
  }, [tripId]);

  const resetDestinationForm = () => {
    setDestinationForm({
      name: '',
      country: '',
      city: '',
      description: '',
      arrivalDate: '',
      departureDate: '',
      tripId,
    });
  };

  const resetActivityForm = () => {
    setActivityForm({
      name: '',
      description: '',
      scheduledDateTime: '',
      durationMinutes: 60,
      location: '',
      estimatedCost: 0,
      destinationId: 0,
    });
  };

  const resetBudgetForm = () => {
    setBudgetForm({
      category: '',
      plannedAmount: 0,
      actualAmount: 0,
      notes: '',
      tripId,
    });
  };

  const handleCreateDestination = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await tripService.createDestination(destinationForm);
      setShowDestinationModal(false);
      resetDestinationForm();
      refreshTrip();
    } catch (err: unknown) {
      alert(getApiMessage(err) || 'Failed to create destination');
    }
  };

  const handleCreateActivity = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await tripService.createActivity(activityForm);
      setShowActivityModal(false);
      resetActivityForm();
      refreshTrip();
    } catch (err: unknown) {
      alert(getApiMessage(err) || 'Failed to create activity');
    }
  };

  const handleCreateBudget = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await tripService.createBudget(budgetForm);
      setShowBudgetModal(false);
      resetBudgetForm();
      refreshTrip();
    } catch (err: unknown) {
      alert(getApiMessage(err) || 'Failed to create budget');
    }
  };

  const handleDeleteDestination = async (destId: number) => {
    if (!window.confirm('Are you sure? This will delete all activities in this destination.')) return;
    try {
      await tripService.deleteDestination(destId);
      refreshTrip();
    } catch {
      alert('Failed to delete destination');
    }
  };

  const handleDeleteBudget = async (budgetId: number) => {
    if (!window.confirm('Delete this budget item?')) return;
    try {
      await tripService.deleteBudget(budgetId);
      refreshTrip();
    } catch {
      alert('Failed to delete budget');
    }
  };

  const getTotalBudget = () => {
    return trip?.budgets.reduce((sum, budget) => sum + budget.plannedAmount, 0) || 0;
  };

  const getTotalSpent = () => {
    return trip?.budgets.reduce((sum, budget) => sum + budget.actualAmount, 0) || 0;
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

  if (!trip) {
    return (
      <Layout>
        <div className="alert alert-error">
          <span>Trip not found</span>
        </div>
      </Layout>
    );
  }

  const totalBudget = getTotalBudget();
  const totalSpent = getTotalSpent();
  const remainingBudget = totalBudget - totalSpent;

  return (
    <Layout>
      <div className="space-y-6">
        <button type="button" className="btn btn-sm quiet-action" onClick={() => navigate('/trips')}>
          <FaArrowLeft aria-hidden="true" />
          Back to Trips
        </button>

        <header className="border-b border-slate-200 pb-6">
          <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
            <div>
              <p className="page-kicker mb-2">Trip Overview</p>
              <div className="flex flex-wrap items-center gap-3">
                <h1 className="section-title">{trip.name}</h1>
                <StatusBadge status={trip.status} />
              </div>
              {trip.description && (
                <p className="mt-3 max-w-3xl text-slate-600">{trip.description}</p>
              )}
            </div>

            <Link to={`/trips/${trip.id}/edit`} className="btn quiet-action w-full sm:w-auto">
              <FaEdit aria-hidden="true" />
              Edit
            </Link>
          </div>

          <div className="mt-5 flex flex-wrap items-center gap-2 text-sm text-slate-600 sm:text-base">
            <FaCalendarAlt className="text-cyan-700" aria-hidden="true" />
            <span>{format(new Date(trip.startDate), 'MMM dd, yyyy')}</span>
            <FaArrowRight className="text-slate-400" aria-hidden="true" />
            <span>{format(new Date(trip.endDate), 'MMM dd, yyyy')}</span>
          </div>

          <div className="mt-6 grid grid-cols-1 gap-4 md:grid-cols-3">
            <div className="metric-tile p-4">
              <div className="stat p-0">
                <div className="stat-figure text-cyan-700">
                  <FaMapMarkedAlt className="text-2xl" aria-hidden="true" />
                </div>
                <div className="stat-title">Destinations</div>
                <div className="stat-value text-cyan-700">{trip.destinations.length}</div>
              </div>
            </div>
            <div className="metric-tile p-4">
              <div className="stat p-0">
                <div className="stat-figure text-emerald-700">
                  <FaMoneyBillWave className="text-2xl" aria-hidden="true" />
                </div>
                <div className="stat-title">Planned Budget</div>
                <div className="stat-value text-emerald-700">{currency(totalBudget)}</div>
              </div>
            </div>
            <div className="metric-tile p-4">
              <div className="stat p-0">
                <div className="stat-figure text-amber-600">
                  <FaMoneyBillWave className="text-2xl" aria-hidden="true" />
                </div>
                <div className="stat-title">Remaining</div>
                <div className={`stat-value ${remainingBudget < 0 ? 'text-error' : 'text-amber-600'}`}>{currency(remainingBudget)}</div>
                <div className="stat-desc">Spent: {currency(totalSpent)}</div>
              </div>
            </div>
          </div>
        </header>

        <div className="tabs tabs-boxed w-full rounded-lg border border-slate-200 bg-white/85 p-1 shadow-sm sm:w-auto">
          <button
            type="button"
            className={`tab h-auto min-h-11 flex-1 gap-2 px-3 sm:flex-none ${activeTab === 'destinations' ? 'tab-active' : ''}`}
            onClick={() => setActiveTab('destinations')}
          >
            <FaMapMarkedAlt aria-hidden="true" />
            <span>Destinations</span>
          </button>
          <button
            type="button"
            className={`tab h-auto min-h-11 flex-1 gap-2 px-3 sm:flex-none ${activeTab === 'budgets' ? 'tab-active' : ''}`}
            onClick={() => setActiveTab('budgets')}
          >
            <FaMoneyBillWave aria-hidden="true" />
            <span>Budgets</span>
          </button>
        </div>

        {activeTab === 'destinations' && (
          <section className="space-y-5">
            <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <p className="page-kicker mb-2">Itinerary</p>
                <h2 className="section-title">Destinations</h2>
              </div>
              <button
                onClick={() => setShowDestinationModal(true)}
                className="btn primary-action w-full sm:w-auto"
              >
                <FaPlus className="mr-2" aria-hidden="true" />
                Add Destination
              </button>
            </div>

            {trip.destinations.length === 0 ? (
              <div className="empty-state p-8 text-center">
                <span className="icon-chip mx-auto mb-4 bg-cyan-50 text-cyan-700">
                  <FaMapMarkedAlt aria-hidden="true" />
                </span>
                <p className="text-lg font-semibold text-slate-900">No destinations yet</p>
                <p className="mt-2 text-slate-600">Add your first stop to start shaping the itinerary.</p>
              </div>
            ) : (
              <div className="space-y-4">
                {trip.destinations.map((destination) => (
                  <article key={destination.id} className="surface-card p-5">
                    <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                      <div>
                        <h3 className="text-xl font-bold text-slate-900 sm:text-2xl">{destination.name}</h3>
                        <p className="mt-1 flex flex-wrap items-center gap-2 text-sm text-slate-600">
                          <FaMapMarkerAlt className="text-amber-600" aria-hidden="true" />
                          <span>{destination.city && `${destination.city}, `}{destination.country}</span>
                        </p>
                      </div>
                      <div className="grid grid-cols-2 gap-2 sm:flex">
                        <button
                          onClick={() => {
                            setActivityForm({ ...activityForm, destinationId: destination.id });
                            setShowActivityModal(true);
                          }}
                          className="btn btn-sm primary-action"
                        >
                          <FaPlus aria-hidden="true" />
                          Activity
                        </button>
                        <button
                          onClick={() => handleDeleteDestination(destination.id)}
                          className="btn btn-sm quiet-action danger-action"
                          aria-label={`Delete destination ${destination.name}`}
                        >
                          <FaTrash aria-hidden="true" />
                        </button>
                      </div>
                    </div>

                    {destination.description && (
                      <p className="mt-3 text-slate-600">{destination.description}</p>
                    )}

                    <div className="mt-4 flex flex-wrap items-center gap-2 text-sm text-slate-600">
                      <FaCalendarAlt className="text-cyan-700" aria-hidden="true" />
                      <span>{format(new Date(destination.arrivalDate), 'MMM dd, yyyy')}</span>
                      <FaArrowRight className="text-slate-400" aria-hidden="true" />
                      <span>{format(new Date(destination.departureDate), 'MMM dd, yyyy')}</span>
                    </div>

                    <div className="mt-4 border-t border-dashed border-slate-200 pt-4">
                      <h4 className="font-semibold text-slate-900">Activities</h4>
                      <p className="mt-1 text-sm text-slate-600">Add activities from this destination card.</p>
                    </div>
                  </article>
                ))}
              </div>
            )}
          </section>
        )}

        {activeTab === 'budgets' && (
          <section className="space-y-5">
            <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <p className="page-kicker mb-2">Spending</p>
                <h2 className="section-title">Budget Breakdown</h2>
              </div>
              <button
                onClick={() => setShowBudgetModal(true)}
                className="btn primary-action w-full sm:w-auto"
              >
                <FaPlus className="mr-2" aria-hidden="true" />
                Add Budget Item
              </button>
            </div>

            {trip.budgets.length === 0 ? (
              <div className="empty-state p-8 text-center">
                <span className="icon-chip mx-auto mb-4 bg-emerald-50 text-emerald-700">
                  <FaMoneyBillWave aria-hidden="true" />
                </span>
                <p className="text-lg font-semibold text-slate-900">No budget items yet</p>
                <p className="mt-2 text-slate-600">Add planned expenses to see how the trip is tracking.</p>
              </div>
            ) : (
              <div className="data-panel overflow-x-auto">
                <table className="table">
                  <thead>
                    <tr>
                      <th>Category</th>
                      <th>Planned</th>
                      <th>Actual</th>
                      <th>Difference</th>
                      <th>Notes</th>
                      <th className="text-right">Actions</th>
                    </tr>
                  </thead>
                  <tbody>
                    {trip.budgets.map((budget) => {
                      const difference = budget.plannedAmount - budget.actualAmount;

                      return (
                        <tr key={budget.id}>
                          <td className="font-semibold">{budget.category}</td>
                          <td>{currency(budget.plannedAmount)}</td>
                          <td>{currency(budget.actualAmount)}</td>
                          <td className={difference < 0 ? 'text-error' : 'text-success'}>{currency(difference)}</td>
                          <td>{budget.notes || '-'}</td>
                          <td className="text-right">
                            <button
                              onClick={() => handleDeleteBudget(budget.id)}
                              className="btn btn-sm btn-ghost danger-action"
                              aria-label={`Delete budget item ${budget.category}`}
                            >
                              <FaTrash aria-hidden="true" />
                            </button>
                          </td>
                        </tr>
                      );
                    })}
                    <tr className="font-bold">
                      <td>TOTAL</td>
                      <td>{currency(totalBudget)}</td>
                      <td>{currency(totalSpent)}</td>
                      <td className={remainingBudget < 0 ? 'text-error' : 'text-success'}>{currency(remainingBudget)}</td>
                      <td colSpan={2}></td>
                    </tr>
                  </tbody>
                </table>
              </div>
            )}
          </section>
        )}
      </div>

      {showDestinationModal && (
        <dialog className="modal modal-open">
          <form className="modal-box modal-panel max-w-2xl" onSubmit={handleCreateDestination}>
            <h3 className="mb-4 text-lg font-bold text-slate-900">Add Destination</h3>

            <div className="space-y-4">
              <div className="form-control">
                <label className="label">
                  <span className="label-text font-semibold text-slate-700">Destination Name *</span>
                </label>
                <input
                  type="text"
                  className="input input-bordered field-control"
                  value={destinationForm.name}
                  onChange={(e) => setDestinationForm({ ...destinationForm, name: e.target.value })}
                  required
                />
              </div>

              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <div className="form-control">
                  <label className="label">
                    <span className="label-text font-semibold text-slate-700">Country *</span>
                  </label>
                  <input
                    type="text"
                    className="input input-bordered field-control"
                    value={destinationForm.country}
                    onChange={(e) => setDestinationForm({ ...destinationForm, country: e.target.value })}
                    required
                  />
                </div>

                <div className="form-control">
                  <label className="label">
                    <span className="label-text font-semibold text-slate-700">City</span>
                  </label>
                  <input
                    type="text"
                    className="input input-bordered field-control"
                    value={destinationForm.city}
                    onChange={(e) => setDestinationForm({ ...destinationForm, city: e.target.value })}
                  />
                </div>
              </div>

              <div className="form-control">
                <label className="label">
                  <span className="label-text font-semibold text-slate-700">Description</span>
                </label>
                <textarea
                  className="textarea textarea-bordered field-control"
                  value={destinationForm.description}
                  onChange={(e) => setDestinationForm({ ...destinationForm, description: e.target.value })}
                />
              </div>

              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <div className="form-control">
                  <label className="label">
                    <span className="label-text font-semibold text-slate-700">Arrival Date *</span>
                  </label>
                  <input
                    type="date"
                    className="input input-bordered field-control"
                    value={destinationForm.arrivalDate}
                    onChange={(e) => setDestinationForm({ ...destinationForm, arrivalDate: e.target.value })}
                    required
                  />
                </div>

                <div className="form-control">
                  <label className="label">
                    <span className="label-text font-semibold text-slate-700">Departure Date *</span>
                  </label>
                  <input
                    type="date"
                    className="input input-bordered field-control"
                    value={destinationForm.departureDate}
                    onChange={(e) => setDestinationForm({ ...destinationForm, departureDate: e.target.value })}
                    required
                  />
                </div>
              </div>
            </div>

            <div className="modal-action">
              <button type="button" className="btn quiet-action" onClick={() => setShowDestinationModal(false)}>
                Cancel
              </button>
              <button type="submit" className="btn primary-action">
                Add Destination
              </button>
            </div>
          </form>
        </dialog>
      )}

      {showActivityModal && (
        <dialog className="modal modal-open">
          <form className="modal-box modal-panel max-w-2xl" onSubmit={handleCreateActivity}>
            <h3 className="mb-4 text-lg font-bold text-slate-900">Add Activity</h3>

            <div className="space-y-4">
              <div className="form-control">
                <label className="label">
                  <span className="label-text font-semibold text-slate-700">Activity Name *</span>
                </label>
                <input
                  type="text"
                  className="input input-bordered field-control"
                  value={activityForm.name}
                  onChange={(e) => setActivityForm({ ...activityForm, name: e.target.value })}
                  required
                />
              </div>

              <div className="form-control">
                <label className="label">
                  <span className="label-text font-semibold text-slate-700">Description</span>
                </label>
                <textarea
                  className="textarea textarea-bordered field-control"
                  value={activityForm.description}
                  onChange={(e) => setActivityForm({ ...activityForm, description: e.target.value })}
                />
              </div>

              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <div className="form-control">
                  <label className="label">
                    <span className="label-text font-semibold text-slate-700">Date & Time *</span>
                  </label>
                  <input
                    type="datetime-local"
                    className="input input-bordered field-control"
                    value={activityForm.scheduledDateTime}
                    onChange={(e) => setActivityForm({ ...activityForm, scheduledDateTime: e.target.value })}
                    required
                  />
                </div>

                <div className="form-control">
                  <label className="label">
                    <span className="label-text font-semibold text-slate-700">Duration (minutes)</span>
                  </label>
                  <input
                    type="number"
                    className="input input-bordered field-control"
                    value={activityForm.durationMinutes}
                    onChange={(e) => setActivityForm({ ...activityForm, durationMinutes: toNumber(e.target.value) })}
                  />
                </div>
              </div>

              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <div className="form-control">
                  <label className="label">
                    <span className="label-text font-semibold text-slate-700">Location</span>
                  </label>
                  <input
                    type="text"
                    className="input input-bordered field-control"
                    value={activityForm.location}
                    onChange={(e) => setActivityForm({ ...activityForm, location: e.target.value })}
                  />
                </div>

                <div className="form-control">
                  <label className="label">
                    <span className="label-text font-semibold text-slate-700">Estimated Cost ($)</span>
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    className="input input-bordered field-control"
                    value={activityForm.estimatedCost}
                    onChange={(e) => setActivityForm({ ...activityForm, estimatedCost: toNumber(e.target.value) })}
                  />
                </div>
              </div>
            </div>

            <div className="modal-action">
              <button type="button" className="btn quiet-action" onClick={() => setShowActivityModal(false)}>
                Cancel
              </button>
              <button type="submit" className="btn primary-action">
                Add Activity
              </button>
            </div>
          </form>
        </dialog>
      )}

      {showBudgetModal && (
        <dialog className="modal modal-open">
          <form className="modal-box modal-panel max-w-2xl" onSubmit={handleCreateBudget}>
            <h3 className="mb-4 text-lg font-bold text-slate-900">Add Budget Item</h3>

            <div className="space-y-4">
              <div className="form-control">
                <label className="label">
                  <span className="label-text font-semibold text-slate-700">Category *</span>
                </label>
                <select
                  className="select select-bordered field-control"
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

              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <div className="form-control">
                  <label className="label">
                    <span className="label-text font-semibold text-slate-700">Planned Amount ($) *</span>
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    className="input input-bordered field-control"
                    value={budgetForm.plannedAmount}
                    onChange={(e) => setBudgetForm({ ...budgetForm, plannedAmount: toNumber(e.target.value) })}
                    required
                  />
                </div>

                <div className="form-control">
                  <label className="label">
                    <span className="label-text font-semibold text-slate-700">Actual Amount ($)</span>
                  </label>
                  <input
                    type="number"
                    step="0.01"
                    className="input input-bordered field-control"
                    value={budgetForm.actualAmount}
                    onChange={(e) => setBudgetForm({ ...budgetForm, actualAmount: toNumber(e.target.value) })}
                  />
                </div>
              </div>

              <div className="form-control">
                <label className="label">
                  <span className="label-text font-semibold text-slate-700">Notes</span>
                </label>
                <textarea
                  className="textarea textarea-bordered field-control"
                  value={budgetForm.notes}
                  onChange={(e) => setBudgetForm({ ...budgetForm, notes: e.target.value })}
                />
              </div>
            </div>

            <div className="modal-action">
              <button type="button" className="btn quiet-action" onClick={() => setShowBudgetModal(false)}>
                Cancel
              </button>
              <button type="submit" className="btn primary-action">
                Add Budget
              </button>
            </div>
          </form>
        </dialog>
      )}
    </Layout>
  );
};

export default TripDetailPage;
