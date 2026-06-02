import React from 'react';

const statusMeta: Record<string, { className: string; label: string }> = {
  Planning: { className: 'badge-info', label: 'Planning' },
  Confirmed: { className: 'badge-success', label: 'Confirmed' },
  InProgress: { className: 'badge-warning', label: 'In Progress' },
  Completed: { className: 'badge-neutral', label: 'Completed' },
  Cancelled: { className: 'badge-error', label: 'Cancelled' },
};

interface StatusBadgeProps {
  status: string;
  size?: 'sm' | 'md';
}

const StatusBadge: React.FC<StatusBadgeProps> = ({ status, size = 'md' }) => {
  const meta = statusMeta[status] || { className: 'badge-ghost', label: status };
  const sizeClass = size === 'sm' ? 'badge-sm' : 'badge-md';

  return <span className={`badge ${meta.className} ${sizeClass}`}>{meta.label}</span>;
};

export default StatusBadge;
