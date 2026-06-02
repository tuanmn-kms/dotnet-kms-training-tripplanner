import React, { ReactNode } from 'react';
import Navbar from './Navbar';

interface LayoutProps {
  children: ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  return (
    <div className="page-shell">
      <Navbar />
      <main className="page-container page-main py-6 sm:py-8">
        {children}
      </main>
      <footer className="app-footer">
        <div className="page-container py-4 text-center text-sm text-slate-600 sm:py-5 sm:text-base">
          Copyright (c) 2026 - KMS Trip Planner. All rights reserved.
        </div>
      </footer>
    </div>
  );
};

export default Layout;
