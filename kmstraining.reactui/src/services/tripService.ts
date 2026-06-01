import api from './api';

export interface Trip {
  id: number;
  name: string;
  description?: string;
  startDate: string;
  endDate: string;
  status: string;
  userId: number;
  createdAt: string;
  updatedAt?: string;
}

export interface TripDetail extends Trip {
  destinations: Destination[];
  budgets: Budget[];
}

export interface CreateTripDto {
  name: string;
  description?: string;
  startDate: string;
  endDate: string;
}

export interface UpdateTripDto {
  name?: string;
  description?: string;
  startDate?: string;
  endDate?: string;
  status?: string;
}

export interface Destination {
  id: number;
  name: string;
  country: string;
  city?: string;
  description?: string;
  arrivalDate: string;
  departureDate: string;
  tripId: number;
  createdAt: string;
  updatedAt?: string;
}

export interface DestinationDetail extends Destination {
  activities: Activity[];
}

export interface CreateDestinationDto {
  name: string;
  country: string;
  city?: string;
  description?: string;
  arrivalDate: string;
  departureDate: string;
  tripId: number;
}

export interface UpdateDestinationDto {
  name?: string;
  country?: string;
  city?: string;
  description?: string;
  arrivalDate?: string;
  departureDate?: string;
}

export interface Activity {
  id: number;
  name: string;
  description?: string;
  scheduledDateTime: string;
  durationMinutes: number;
  location?: string;
  estimatedCost?: number;
  destinationId: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateActivityDto {
  name: string;
  description?: string;
  scheduledDateTime: string;
  durationMinutes: number;
  location?: string;
  estimatedCost?: number;
  destinationId: number;
}

export interface UpdateActivityDto {
  name?: string;
  description?: string;
  scheduledDateTime?: string;
  durationMinutes?: number;
  location?: string;
  estimatedCost?: number;
}

export interface Budget {
  id: number;
  category: string;
  plannedAmount: number;
  actualAmount: number;
  notes?: string;
  tripId: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateBudgetDto {
  category: string;
  plannedAmount: number;
  actualAmount?: number;
  notes?: string;
  tripId: number;
}

export interface UpdateBudgetDto {
  category?: string;
  plannedAmount?: number;
  actualAmount?: number;
  notes?: string;
}

const tripService = {
  // Trips
  getTrips: async (): Promise<Trip[]> => {
    const response = await api.get<Trip[]>('/trips');
    return response.data;
  },

  getTrip: async (id: number): Promise<TripDetail> => {
    const response = await api.get<TripDetail>(`/trips/${id}`);
    return response.data;
  },

  createTrip: async (data: CreateTripDto): Promise<Trip> => {
    const response = await api.post<Trip>('/trips', data);
    return response.data;
  },

  updateTrip: async (id: number, data: UpdateTripDto): Promise<Trip> => {
    const response = await api.put<Trip>(`/trips/${id}`, data);
    return response.data;
  },

  deleteTrip: async (id: number): Promise<void> => {
    await api.delete(`/trips/${id}`);
  },

  // Destinations
  getDestinations: async (tripId?: number): Promise<Destination[]> => {
    const url = tripId ? `/destinations?tripId=${tripId}` : '/destinations';
    const response = await api.get<Destination[]>(url);
    return response.data;
  },

  getDestination: async (id: number): Promise<DestinationDetail> => {
    const response = await api.get<DestinationDetail>(`/destinations/${id}`);
    return response.data;
  },

  createDestination: async (data: CreateDestinationDto): Promise<Destination> => {
    const response = await api.post<Destination>('/destinations', data);
    return response.data;
  },

  updateDestination: async (id: number, data: UpdateDestinationDto): Promise<Destination> => {
    const response = await api.put<Destination>(`/destinations/${id}`, data);
    return response.data;
  },

  deleteDestination: async (id: number): Promise<void> => {
    await api.delete(`/destinations/${id}`);
  },

  // Activities
  getActivities: async (destinationId?: number): Promise<Activity[]> => {
    const url = destinationId ? `/activities?destinationId=${destinationId}` : '/activities';
    const response = await api.get<Activity[]>(url);
    return response.data;
  },

  getActivity: async (id: number): Promise<Activity> => {
    const response = await api.get<Activity>(`/activities/${id}`);
    return response.data;
  },

  createActivity: async (data: CreateActivityDto): Promise<Activity> => {
    const response = await api.post<Activity>('/activities', data);
    return response.data;
  },

  updateActivity: async (id: number, data: UpdateActivityDto): Promise<Activity> => {
    const response = await api.put<Activity>(`/activities/${id}`, data);
    return response.data;
  },

  deleteActivity: async (id: number): Promise<void> => {
    await api.delete(`/activities/${id}`);
  },

  // Budgets
  getBudgets: async (tripId?: number): Promise<Budget[]> => {
    const url = tripId ? `/budgets?tripId=${tripId}` : '/budgets';
    const response = await api.get<Budget[]>(url);
    return response.data;
  },

  getBudget: async (id: number): Promise<Budget> => {
    const response = await api.get<Budget>(`/budgets/${id}`);
    return response.data;
  },

  createBudget: async (data: CreateBudgetDto): Promise<Budget> => {
    const response = await api.post<Budget>('/budgets', data);
    return response.data;
  },

  updateBudget: async (id: number, data: UpdateBudgetDto): Promise<Budget> => {
    const response = await api.put<Budget>(`/budgets/${id}`, data);
    return response.data;
  },

  deleteBudget: async (id: number): Promise<void> => {
    await api.delete(`/budgets/${id}`);
  },
};

export default tripService;
