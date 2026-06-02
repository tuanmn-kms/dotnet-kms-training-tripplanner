import React from 'react';
import { Link } from 'react-router-dom';
import {
  FaCalendarCheck,
  FaGlobe,
  FaMapMarkedAlt,
  FaMoneyBillWave,
  FaRocket,
  FaStar,
  FaUsers,
} from 'react-icons/fa';
import { useAuth } from '../contexts/AuthContext';
import BrandMark from '../components/ui/BrandMark';
import heroImage from '../assets/hero.png';

const Home: React.FC = () => {
  const { isAuthenticated } = useAuth();

  return (
    <div className="min-h-screen">
      <header className="page-container py-4">
        <div className="flex min-h-14 flex-wrap items-center justify-between gap-3">
          <BrandMark compact />
          <nav className="flex gap-2" aria-label="Home navigation">
            {isAuthenticated ? (
              <Link to="/trips" className="btn btn-sm sm:btn-md primary-action">My Trips</Link>
            ) : (
              <>
                <Link to="/login" className="btn btn-sm sm:btn-md quiet-action">Login</Link>
                <Link to="/register" className="btn btn-sm sm:btn-md primary-action">Get Started</Link>
              </>
            )}
          </nav>
        </div>
      </header>

      <main>
        <section
          className="home-hero"
          style={{
            backgroundImage: `linear-gradient(90deg, rgb(248 250 252 / 0.98), rgb(248 250 252 / 0.9) 48%, rgb(248 250 252 / 0.68)), url(${heroImage})`,
          }}
        >
          <div className="page-container">
            <div className="max-w-2xl py-12 sm:py-16">
              <p className="page-kicker mb-3">Travel planning workspace</p>
              <h1 className="page-title">KMS Trip Planner</h1>
              <p className="page-subtitle mt-5 max-w-xl sm:text-lg">
                Plan destinations, activities, dates, and budgets in one calm workspace built for real trip decisions.
              </p>

              <div className="mt-8 flex flex-col gap-3 sm:flex-row">
                {!isAuthenticated ? (
                  <>
                    <Link to="/register" className="btn btn-md sm:btn-lg primary-action w-full sm:w-auto">
                      <FaRocket aria-hidden="true" className="mr-2" />
                      Start Planning
                    </Link>
                    <Link to="/login" className="btn btn-md sm:btn-lg quiet-action w-full sm:w-auto">
                      Sign In
                    </Link>
                  </>
                ) : (
                  <Link to="/trips" className="btn btn-md sm:btn-lg primary-action w-full sm:w-auto">
                    <FaMapMarkedAlt aria-hidden="true" className="mr-2" />
                    Open My Trips
                  </Link>
                )}
              </div>
            </div>
          </div>
        </section>

        <section className="page-container py-8 sm:py-10">
          <div className="grid grid-cols-2 gap-3 sm:grid-cols-4 sm:gap-4">
            <div className="metric-tile p-4">
              <FaUsers className="mb-3 text-2xl text-teal-600" aria-hidden="true" />
              <p className="text-2xl font-black text-slate-900 sm:text-3xl">10K+</p>
              <p className="text-sm text-slate-600">Active planners</p>
            </div>
            <div className="metric-tile p-4">
              <FaGlobe className="mb-3 text-2xl text-cyan-600" aria-hidden="true" />
              <p className="text-2xl font-black text-slate-900 sm:text-3xl">150+</p>
              <p className="text-sm text-slate-600">Countries</p>
            </div>
            <div className="metric-tile p-4">
              <FaStar className="mb-3 text-2xl text-amber-500" aria-hidden="true" />
              <p className="text-2xl font-black text-slate-900 sm:text-3xl">50K+</p>
              <p className="text-sm text-slate-600">Trips created</p>
            </div>
            <div className="metric-tile p-4">
              <FaMoneyBillWave className="mb-3 text-2xl text-emerald-600" aria-hidden="true" />
              <p className="text-2xl font-black text-slate-900 sm:text-3xl">100%</p>
              <p className="text-sm text-slate-600">Budget visibility</p>
            </div>
          </div>
        </section>

        <section className="page-container pb-12 sm:pb-16">
          <div className="mb-6">
            <p className="page-kicker mb-2">Core workflow</p>
            <h2 className="section-title">What You Can Do</h2>
          </div>

          <div className="grid grid-cols-1 gap-4 md:grid-cols-3 sm:gap-6">
            <article className="surface-card p-5 sm:p-6">
              <span className="icon-chip mb-4 bg-cyan-50 text-cyan-700">
                <FaMapMarkedAlt className="text-xl" aria-hidden="true" />
              </span>
              <h3 className="text-lg font-bold text-slate-900">Destination Mapping</h3>
              <p className="mt-2 text-slate-600">Organize countries and cities in a clean timeline for each trip.</p>
            </article>
            <article className="surface-card p-5 sm:p-6">
              <span className="icon-chip mb-4 bg-teal-50 text-teal-700">
                <FaCalendarCheck className="text-xl" aria-hidden="true" />
              </span>
              <h3 className="text-lg font-bold text-slate-900">Activity Scheduling</h3>
              <p className="mt-2 text-slate-600">Plan every day with activities and time windows you can edit fast.</p>
            </article>
            <article className="surface-card p-5 sm:p-6">
              <span className="icon-chip mb-4 bg-amber-50 text-amber-700">
                <FaMoneyBillWave className="text-xl" aria-hidden="true" />
              </span>
              <h3 className="text-lg font-bold text-slate-900">Budget Control</h3>
              <p className="mt-2 text-slate-600">Track planned vs actual costs by category before surprises hit.</p>
            </article>
          </div>
        </section>
      </main>

      <footer className="app-footer">
        <div className="page-container py-6 text-center text-sm text-slate-600 sm:py-8 sm:text-base">
          Copyright (c) 2026 - KMS Trip Planner. All rights reserved.
        </div>
      </footer>
    </div>
  );
};

export default Home;
