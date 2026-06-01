import React, { ReactNode } from 'react';
import Navbar from './Navbar';

interface LayoutProps {
  children: ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  return (
    <div className="min-h-screen bg-base-200">
      <Navbar />
      <main className="container mx-auto px-4 py-8">
        {children}
      </main>
      <footer className="footer footer-center p-4 bg-base-300 text-base-content mt-auto">
        <aside>
          <p>Copyright © 2024 - Trip Planner. All rights reserved.</p>
        </aside>
      </footer>
    </div>
  );
};

export default Layout;
