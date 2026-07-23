import React, { useState, useEffect } from 'react';
import { employeeService } from '../services/employeeService';
import { attendanceService } from '../services/attendanceService';
import { Employee, Attendance } from '../types';

export const Dashboard: React.FC = () => {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [attendanceToday, setAttendanceToday] = useState<Attendance[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedEmpId, setSelectedEmpId] = useState<number | ''>('');
  const [actionLoading, setActionLoading] = useState(false);
  const [time, setTime] = useState(new Date());

  // Clock ticks every second
  useEffect(() => {
    const timer = setInterval(() => setTime(new Date()), 1000);
    return () => clearInterval(timer);
  }, []);

  const loadData = async () => {
    try {
      setLoading(true);
      // Fetch employees (get first 100 for stats)
      const empData = await employeeService.getEmployees('', '', '', 1, 100);
      setEmployees(empData.data);

      // Fetch today's attendance
      const todayStr = new Date().toISOString().split('T')[0];
      const attData = await attendanceService.getAttendanceHistory(todayStr, todayStr);
      setAttendanceToday(attData);
    } catch (err) {
      console.error('Error loading dashboard stats:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleCheckIn = async () => {
    if (!selectedEmpId) return;
    try {
      setActionLoading(true);
      await attendanceService.checkIn(selectedEmpId);
      alert('Check-in recorded successfully.');
      loadData();
    } catch (err: any) {
      alert(err.response?.data?.message || 'Check-in failed.');
    } finally {
      setActionLoading(false);
    }
  };

  const handleCheckOut = async (attendanceId: number) => {
    try {
      setActionLoading(true);
      await attendanceService.checkOut(attendanceId);
      alert('Check-out recorded successfully.');
      loadData();
    } catch (err: any) {
      alert(err.response?.data?.message || 'Check-out failed.');
    } finally {
      setActionLoading(false);
    }
  };

  // Stats calculations
  const totalEmployees = employees.length;
  const activeEmployees = employees.filter((e) => e.status === 'Active').length;
  const totalMonthlyPayroll = employees.reduce((sum, e) => sum + Number(e.salary), 0);
  const uniqueDepartments = Array.from(new Set(employees.map((e) => e.department))).length;
  const presentToday = attendanceToday.length;

  if (loading) {
    return <div style={{ textAlign: 'center', padding: '40px' }}>Loading dashboard data...</div>;
  }

  return (
    <div>
      <div className="stats-grid">
        <div className="stat-card">
          <div className="stat-icon" style={{ backgroundColor: 'var(--primary-light)', color: 'var(--primary)' }}>👤</div>
          <div className="stat-info">
            <span className="stat-label">Total Employees</span>
            <span className="stat-value">{totalEmployees} ({activeEmployees} Active)</span>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon" style={{ backgroundColor: '#ccfbf1', color: '#14b8a6' }}>💵</div>
          <div className="stat-info">
            <span className="stat-label">Monthly Payroll</span>
            <span className="stat-value">${totalMonthlyPayroll.toLocaleString('en-US', { minimumFractionDigits: 2 })}</span>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon" style={{ backgroundColor: '#fef3c7', color: '#f59e0b' }}>🏢</div>
          <div className="stat-info">
            <span className="stat-label">Departments</span>
            <span className="stat-value">{uniqueDepartments} Active</span>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon" style={{ backgroundColor: 'var(--success-bg)', color: 'var(--success)' }}>✅</div>
          <div className="stat-info">
            <span className="stat-label">Present Today</span>
            <span className="stat-value">{presentToday} Checked In</span>
          </div>
        </div>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(320px, 1fr))', gap: '24px' }}>
        {/* Clock In/Out panel */}
        <div className="attendance-widget">
          <div className="attendance-title">⏱️ Shift Timeclock</div>
          <div className="attendance-time">{time.toLocaleTimeString()}</div>
          <div className="attendance-date">{time.toLocaleDateString(undefined, { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}</div>
          
          <div style={{ marginBottom: '20px' }}>
            <label className="form-label" style={{ color: '#94a3b8' }}>Select Employee to Clock In:</label>
            <select 
              className="form-control" 
              style={{ backgroundColor: '#1e293b', borderColor: '#334155', color: '#ffffff' }}
              value={selectedEmpId}
              onChange={(e) => setSelectedEmpId(Number(e.target.value) || '')}
              disabled={actionLoading}
            >
              <option value="">-- Choose Employee --</option>
              {employees
                .filter(e => e.status === 'Active' && !attendanceToday.some(att => att.employeeId === e.employeeId))
                .map(e => (
                  <option key={e.employeeId} value={e.employeeId}>
                    {e.firstName} {e.lastName} ({e.designation})
                  </option>
                ))}
            </select>
          </div>

          <div className="attendance-buttons">
            <button 
              className="btn btn-primary" 
              onClick={handleCheckIn}
              disabled={!selectedEmpId || actionLoading}
              style={{ flexGrow: 1 }}
            >
              Check In (Start Shift)
            </button>
          </div>
        </div>

        {/* Present Today Panel */}
        <div className="card">
          <h2 className="card-title">👥 Today's Presence</h2>
          {attendanceToday.length === 0 ? (
            <div style={{ color: 'var(--text-muted)', fontSize: '14px' }}>No employees have clocked in today yet.</div>
          ) : (
            <div className="table-responsive" style={{ maxHeight: '220px' }}>
              <table className="table">
                <thead>
                  <tr>
                    <th>Employee</th>
                    <th>In</th>
                    <th>Out</th>
                    <th>Status</th>
                    <th>Action</th>
                  </tr>
                </thead>
                <tbody>
                  {attendanceToday.map((att) => (
                    <tr key={att.attendanceId}>
                      <td style={{ fontWeight: '500' }}>{att.employeeName}</td>
                      <td>{new Date(att.checkIn).toLocaleTimeString(undefined, { hour: '2-digit', minute: '2-digit' })}</td>
                      <td>
                        {att.checkOut 
                          ? new Date(att.checkOut).toLocaleTimeString(undefined, { hour: '2-digit', minute: '2-digit' })
                          : '-'
                        }
                      </td>
                      <td>
                        <span className={`badge badge-${att.status === 'Present' ? 'success' : 'warning'}`}>
                          {att.status}
                        </span>
                      </td>
                      <td>
                        {!att.checkOut && (
                          <button 
                            className="btn btn-secondary" 
                            style={{ padding: '4px 8px', fontSize: '12px' }}
                            onClick={() => handleCheckOut(att.attendanceId)}
                            disabled={actionLoading}
                          >
                            Out
                          </button>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};
