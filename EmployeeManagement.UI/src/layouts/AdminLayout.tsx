import React from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { authService } from '../services/authService';

export const AdminLayout: React.FC = () => {
  const navigate = useNavigate();
  const currentUser = authService.getCurrentUser();

  const handleLogout = () => {
    authService.logout();
    navigate('/login');
  };

  return (
    <div className="app-layout">
      <aside className="sidebar">
        <div className="sidebar-header">
          <span>💼 EMS Portal</span>
        </div>
        <nav className="sidebar-menu">
          <div className="sidebar-item">
            <NavLink to="/dashboard" className={({ isActive }) => `sidebar-link ${isActive ? 'active' : ''}`}>
              📊 Dashboard
            </NavLink>
          </div>
          <div className="sidebar-item">
            <NavLink to="/employees" className={({ isActive }) => `sidebar-link ${isActive ? 'active' : ''}`}>
              👥 Employees
            </NavLink>
          </div>
          <div className="sidebar-item">
            <NavLink to="/attendance" className={({ isActive }) => `sidebar-link ${isActive ? 'active' : ''}`}>
              ⏱️ Attendance
            </NavLink>
          </div>
          <div className="sidebar-item">
            <NavLink to="/reports" className={({ isActive }) => `sidebar-link ${isActive ? 'active' : ''}`}>
              📈 Reports
            </NavLink>
          </div>
        </nav>
        <div className="sidebar-footer">
          {currentUser && (
            <div className="user-info">
              <div className="user-name">{currentUser.username}</div>
              <div className="user-role">{currentUser.role}</div>
            </div>
          )}
          <button onClick={handleLogout} className="btn btn-secondary btn-block" style={{ fontSize: '13px', padding: '8px' }}>
            🚪 Logout
          </button>
        </div>
      </aside>

      <main className="main-content">
        <header className="navbar">
          <div className="navbar-title">Employee Management System</div>
          <div style={{ fontSize: '14px', color: 'var(--text-muted)' }}>
            Welcome, <strong>{currentUser?.username || 'User'}</strong>
          </div>
        </header>
        <div className="content-body">
          <Outlet />
        </div>
      </main>
    </div>
  );
};
