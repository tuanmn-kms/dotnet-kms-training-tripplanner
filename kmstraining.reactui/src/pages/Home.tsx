import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { FaPlane, FaMapMarkedAlt, FaMoneyBillWave, FaCalendarCheck, FaRocket, FaGlobe, FaUsers, FaStar } from 'react-icons/fa';

const Home: React.FC = () => {
  const { isAuthenticated } = useAuth();

  return (
    <div className="min-h-screen">
      <header className="page-container pt-4 sm:pt-6">
        <div className="navbar bg-white/80 backdrop-blur-lg rounded-2xl border border-slate-200/70 px-3 sm:px-5">
          <div className="flex-1">
            <div className="flex items-center gap-2 font-black text-slate-800 tracking-tight text-base sm:text-lg">
              <span className="inline-flex w-8 h-8 items-center justify-center rounded-full bg-cyan-100 text-cyan-700">
                <FaPlane />
              </span>
              KMS Trip Planner
            </div>
          </div>
          <div className="flex-none gap-2">
            <Link to="/login" className="btn btn-sm sm:btn-md border-0 bg-slate-100 text-slate-700 hover:bg-slate-200">Login</Link>
            <Link to="/register" className="btn btn-sm sm:btn-md border-0 bg-gradient-to-r from-teal-500 to-cyan-500 text-white hover:from-teal-600 hover:to-cyan-600">Get Started</Link>
          </div>
        </div>
      </header>

      <section className="page-container py-8 sm:py-16">
        <div className="surface-card rounded-3xl p-6 sm:p-10 lg:p-14 relative overflow-hidden">
          <div className="absolute -top-10 -right-10 w-44 h-44 rounded-full bg-cyan-200/35 blur-3xl"></div>
          <div className="absolute -bottom-16 -left-8 w-52 h-52 rounded-full bg-amber-200/35 blur-3xl"></div>

          <div className="relative z-10 grid lg:grid-cols-2 gap-10 items-center">
            <div>
              <p className="badge border-0 bg-teal-100 text-teal-700 mb-4">Smart Planning Platform</p>
              <h1 className="text-4xl sm:text-5xl lg:text-6xl font-black leading-tight text-slate-900">
                Plan Better.
                <span className="heading-gradient block">Travel Smarter.</span>
              </h1>
              <p className="mt-4 text-slate-600 text-base sm:text-lg max-w-xl">
                Build trips with destinations, activities, and budgets in one focused workspace designed for modern travelers.
              </p>
              <div className="mt-8 flex flex-col sm:flex-row gap-3">
                {!isAuthenticated ? (
                  <>
                    <Link to="/register" className="btn btn-md sm:btn-lg border-0 bg-slate-900 text-white hover:bg-slate-800 w-full sm:w-auto">
                      <FaRocket className="mr-2" />
                      Start Free
                    </Link>
                    <Link to="/login" className="btn btn-md sm:btn-lg border-slate-300 text-slate-700 bg-white/70 hover:bg-slate-100 w-full sm:w-auto">
                      Sign In
                    </Link>
                  </>
                ) : (
                  <Link to="/trips" className="btn btn-md sm:btn-lg border-0 bg-slate-900 text-white hover:bg-slate-800 w-full sm:w-auto">
                    <FaMapMarkedAlt className="mr-2" />
                    Open My Trips
                  </Link>
                )}
              </div>
            </div>

            <div className="grid grid-cols-2 gap-3 sm:gap-4">
              <div className="surface-card rounded-2xl p-4 sm:p-5">
                <FaUsers className="text-teal-600 text-2xl mb-3" />
                <p className="text-2xl sm:text-3xl font-black text-slate-900">10K+</p>
                <p className="text-sm text-slate-600">Active planners</p>
              </div>
              <div className="surface-card rounded-2xl p-4 sm:p-5">
                <FaGlobe className="text-cyan-600 text-2xl mb-3" />
                <p className="text-2xl sm:text-3xl font-black text-slate-900">150+</p>
                <p className="text-sm text-slate-600">Countries</p>
              </div>
              <div className="surface-card rounded-2xl p-4 sm:p-5">
                <FaStar className="text-amber-500 text-2xl mb-3" />
                <p className="text-2xl sm:text-3xl font-black text-slate-900">50K+</p>
                <p className="text-sm text-slate-600">Trips created</p>
              </div>
              <div className="surface-card rounded-2xl p-4 sm:p-5">
                <FaMoneyBillWave className="text-emerald-600 text-2xl mb-3" />
                <p className="text-2xl sm:text-3xl font-black text-slate-900">100%</p>
                <p className="text-sm text-slate-600">Budget visibility</p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="page-container pb-10 sm:pb-16">
        <h2 className="text-2xl sm:text-4xl font-black text-slate-900 mb-6 sm:mb-8">What You Can Do</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 sm:gap-6">
          <article className="surface-card rounded-2xl p-5 sm:p-6">
            <FaMapMarkedAlt className="text-cyan-600 text-3xl mb-4" />
            <h3 className="text-xl font-bold text-slate-900">Destination Mapping</h3>
            <p className="text-slate-600 mt-2">Organize countries and cities in a clean timeline for each trip.</p>
          </article>
          <article className="surface-card rounded-2xl p-5 sm:p-6">
            <FaCalendarCheck className="text-teal-600 text-3xl mb-4" />
            <h3 className="text-xl font-bold text-slate-900">Activity Scheduling</h3>
            <p className="text-slate-600 mt-2">Plan every day with activities and time windows you can edit fast.</p>
          </article>
          <article className="surface-card rounded-2xl p-5 sm:p-6">
            <FaMoneyBillWave className="text-amber-600 text-3xl mb-4" />
            <h3 className="text-xl font-bold text-slate-900">Budget Control</h3>
            <p className="text-slate-600 mt-2">Track planned vs actual costs by category before spending surprises hit.</p>
          </article>
        </div>
      </section>

      <footer className="border-t border-slate-200/70 bg-white/70">
        <div className="page-container py-6 sm:py-8 text-center text-sm sm:text-base text-slate-600">
          Copyright © 2026 - KMS Trip Planner. All rights reserved.
        </div>
      </footer>
    </div>
  );
};

export default Home;
