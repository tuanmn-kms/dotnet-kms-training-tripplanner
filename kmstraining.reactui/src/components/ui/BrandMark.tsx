import React from 'react';
import { Link } from 'react-router-dom';
import { FaPlane } from 'react-icons/fa';

interface BrandMarkProps {
  compact?: boolean;
  className?: string;
  to?: string;
}

const BrandMark: React.FC<BrandMarkProps> = ({ compact = false, className = '', to = '/' }) => {
  return (
    <Link to={to} className={`app-brand ${className}`.trim()} aria-label="KMS Trip Planner home">
      <span className="app-brand-icon">
        <FaPlane aria-hidden="true" />
      </span>
      <span>
        <span className="block text-base sm:text-lg leading-tight">KMS Trip Planner</span>
        {!compact && <span className="app-brand-subtitle">Travel planning workspace</span>}
      </span>
    </Link>
  );
};

export default BrandMark;
