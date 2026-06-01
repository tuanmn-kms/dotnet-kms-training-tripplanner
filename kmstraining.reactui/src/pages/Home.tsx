import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { FaPlane, FaMapMarkedAlt, FaMoneyBillWave, FaCalendarCheck, FaRocket, FaGlobe, FaUsers, FaStar } from 'react-icons/fa';

const Home: React.FC = () => {
  const { isAuthenticated } = useAuth();

  return (
    <div className="min-h-screen">
      {/* Hero Section with Gradient */}
      <div className="hero min-h-[700px] bg-gradient-to-br from-blue-600 via-purple-600 to-pink-500 relative overflow-hidden">
        <div className="absolute inset-0 bg-black opacity-20"></div>
        <div className="hero-content text-center text-white z-10 max-w-6xl">
          <div>
            <div className="flex justify-center mb-6">
              <FaPlane className="text-9xl drop-shadow-2xl animate-pulse" />
            </div>
            <h1 className="text-7xl font-extrabold mb-6 drop-shadow-lg">
              Your Dream Trip Starts Here
            </h1>
            <p className="text-2xl mb-8 font-light max-w-3xl mx-auto leading-relaxed">
              Plan, organize, and manage your perfect journey with our all-in-one trip planning platform.
              From destinations to budgets, we've got you covered.
            </p>
            {!isAuthenticated ? (
              <div className="flex gap-4 justify-center flex-wrap">
                <Link to="/register" className="btn btn-lg bg-white text-purple-600 hover:bg-gray-100 border-0 shadow-xl">
                  <FaRocket className="mr-2" />
                  Start Planning Free
                </Link>
                <Link to="/login" className="btn btn-lg btn-outline border-white text-white hover:bg-white hover:text-purple-600">
                  Sign In
                </Link>
              </div>
            ) : (
              <Link to="/trips" className="btn btn-lg bg-white text-purple-600 hover:bg-gray-100 border-0 shadow-xl">
                <FaMapMarkedAlt className="mr-2" />
                View My Trips
              </Link>
            )}
          </div>
        </div>
      </div>

      {/* Stats Section */}
      <div className="bg-gradient-to-r from-purple-50 to-pink-50 py-16">
        <div className="container mx-auto px-4">
          <div className="stats stats-vertical lg:stats-horizontal shadow-lg w-full bg-white">
            <div className="stat place-items-center">
              <div className="stat-figure text-primary">
                <FaUsers className="text-5xl" />
              </div>
              <div className="stat-title">Active Travelers</div>
              <div className="stat-value text-primary">10K+</div>
              <div className="stat-desc">Planning amazing trips</div>
            </div>

            <div className="stat place-items-center">
              <div className="stat-figure text-secondary">
                <FaGlobe className="text-5xl" />
              </div>
              <div className="stat-title">Destinations</div>
              <div className="stat-value text-secondary">150+</div>
              <div className="stat-desc">Countries covered</div>
            </div>

            <div className="stat place-items-center">
              <div className="stat-figure text-accent">
                <FaStar className="text-5xl" />
              </div>
              <div className="stat-title">Trips Planned</div>
              <div className="stat-value text-accent">50K+</div>
              <div className="stat-desc">And counting</div>
            </div>
          </div>
        </div>
      </div>

      {/* Features Section */}
      <div className="py-24 bg-white">
        <div className="container mx-auto px-4">
          <div className="text-center mb-16">
            <h2 className="text-5xl font-bold mb-4 bg-gradient-to-r from-purple-600 to-pink-600 bg-clip-text text-transparent">
              Powerful Features
            </h2>
            <p className="text-xl text-gray-600 max-w-2xl mx-auto">
              Everything you need to plan the perfect trip, all in one place
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
            {/* Feature 1 */}
            <div className="card bg-gradient-to-br from-blue-50 to-blue-100 shadow-xl hover:shadow-2xl transition-all duration-300 hover:-translate-y-2">
              <figure className="px-10 pt-10">
                <div className="bg-blue-500 p-6 rounded-full">
                  <FaMapMarkedAlt className="text-5xl text-white" />
                </div>
              </figure>
              <div className="card-body items-center text-center">
                <h3 className="card-title text-2xl text-blue-700">Plan Destinations</h3>
                <p className="text-gray-700">
                  Add multiple destinations with dates, create your perfect itinerary
                </p>
              </div>
            </div>

            {/* Feature 2 */}
            <div className="card bg-gradient-to-br from-purple-50 to-purple-100 shadow-xl hover:shadow-2xl transition-all duration-300 hover:-translate-y-2">
              <figure className="px-10 pt-10">
                <div className="bg-purple-500 p-6 rounded-full">
                  <FaCalendarCheck className="text-5xl text-white" />
                </div>
              </figure>
              <div className="card-body items-center text-center">
                <h3 className="card-title text-2xl text-purple-700">Schedule Activities</h3>
                <p className="text-gray-700">
                  Detailed itineraries with activities, times, and locations
                </p>
              </div>
            </div>

            {/* Feature 3 */}
            <div className="card bg-gradient-to-br from-pink-50 to-pink-100 shadow-xl hover:shadow-2xl transition-all duration-300 hover:-translate-y-2">
              <figure className="px-10 pt-10">
                <div className="bg-pink-500 p-6 rounded-full">
                  <FaMoneyBillWave className="text-5xl text-white" />
                </div>
              </figure>
              <div className="card-body items-center text-center">
                <h3 className="card-title text-2xl text-pink-700">Track Budgets</h3>
                <p className="text-gray-700">
                  Monitor spending, set budgets, stay within your limits
                </p>
              </div>
            </div>

            {/* Feature 4 */}
            <div className="card bg-gradient-to-br from-green-50 to-green-100 shadow-xl hover:shadow-2xl transition-all duration-300 hover:-translate-y-2">
              <figure className="px-10 pt-10">
                <div className="bg-green-500 p-6 rounded-full">
                  <FaPlane className="text-5xl text-white" />
                </div>
              </figure>
              <div className="card-body items-center text-center">
                <h3 className="card-title text-2xl text-green-700">Trip Status</h3>
                <p className="text-gray-700">
                  Track progress from planning to completion
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* How It Works */}
      <div className="bg-gradient-to-br from-purple-100 to-pink-100 py-24">
        <div className="container mx-auto px-4">
          <div className="text-center mb-16">
            <h2 className="text-5xl font-bold mb-4 text-purple-800">How It Works</h2>
            <p className="text-xl text-gray-700">Simple steps to plan your perfect trip</p>
          </div>

          <div className="max-w-4xl mx-auto">
            <ul className="steps steps-vertical lg:steps-horizontal w-full">
              <li className="step step-primary">
                <div className="text-left mt-4">
                  <div className="badge badge-lg badge-primary mb-2">Step 1</div>
                  <h3 className="font-bold text-xl mb-2">Create a Trip</h3>
                  <p className="text-gray-600">Set your trip name, dates, and description</p>
                </div>
              </li>
              <li className="step step-primary">
                <div className="text-left mt-4">
                  <div className="badge badge-lg badge-primary mb-2">Step 2</div>
                  <h3 className="font-bold text-xl mb-2">Add Destinations</h3>
                  <p className="text-gray-600">Plan where you want to go and when</p>
                </div>
              </li>
              <li className="step step-primary">
                <div className="text-left mt-4">
                  <div className="badge badge-lg badge-primary mb-2">Step 3</div>
                  <h3 className="font-bold text-xl mb-2">Schedule Activities</h3>
                  <p className="text-gray-600">Fill your days with amazing experiences</p>
                </div>
              </li>
              <li className="step step-primary">
                <div className="text-left mt-4">
                  <div className="badge badge-lg badge-primary mb-2">Step 4</div>
                  <h3 className="font-bold text-xl mb-2">Track Budget</h3>
                  <p className="text-gray-600">Monitor your spending and stay on budget</p>
                </div>
              </li>
            </ul>
          </div>
        </div>
      </div>

      {/* CTA Section */}
      {!isAuthenticated && (
        <div className="bg-gradient-to-r from-purple-600 to-pink-600 py-20">
          <div className="container mx-auto px-4 text-center">
            <h2 className="text-5xl font-bold text-white mb-6">Ready to Start Planning?</h2>
            <p className="text-2xl text-white mb-8 opacity-90">
              Join thousands of travelers organizing their dream trips
            </p>
            <Link 
              to="/register" 
              className="btn btn-lg bg-white text-purple-600 hover:bg-gray-100 border-0 shadow-xl text-xl px-12"
            >
              <FaRocket className="mr-2" />
              Get Started Now - It's Free
            </Link>
          </div>
        </div>
      )}

      {/* Footer */}
      <footer className="footer footer-center p-10 bg-base-200 text-base-content">
        <div>
          <FaPlane className="text-5xl text-primary mb-4" />
          <p className="font-bold text-2xl">KMS Trip Planner</p>
          <p className="text-lg">Your journey to amazing adventures starts here</p>
          <p className="text-sm opacity-70">Copyright © 2026 - All rights reserved</p>
        </div>
      </footer>
    </div>
  );
};

export default Home;
