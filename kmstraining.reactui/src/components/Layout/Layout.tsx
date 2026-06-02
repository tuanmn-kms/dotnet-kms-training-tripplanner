import React, { ReactNode } from 'react';
import Navbar from './Navbar';

interface LayoutProps {
  children: ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  return (
    <div className="min-h-screen">
      <Navbar />
      <main className="page-container py-6 sm:py-8">
        {children}
      </main>
      <footer className="mt-8 border-t border-slate-200/70 bg-white/70 backdrop-blur">
        <div className="page-container py-4 sm:py-5 text-center text-sm sm:text-base text-slate-600">
          Copyright © 2026 - KMS Trip Planner. All rights reserved.
        </div>
      </footer>
    </div>
  );
};

export default Layout;
