import React, { useEffect, useState } from 'react';
import { attendanceService } from '../services/attendanceService';
import { employeeService } from '../services/employeeService';
import { Attendance as AttendanceType, Employee } from '../types';

export const Attendance: React.FC = () => {
  const [history, setHistory] = useState<AttendanceType[]>([]);
  const [employees, setEmployees] = useState<Employee[]>([]);
  
  // Filter states
  const [startDate, setStartDate] = useState('');
  const [endDate, setEndDate] = useState('');
  const [selectedEmpId, setSelectedEmpId] = useState<number | ''>('');
  const [loading, setLoading] = useState(true);

  const loadFilterData = async () => {
    try {
      const empData = await employeeService.getEmployees('', '', '', 1, 100);
      setEmployees(empData.data);
    } catch (err) {
      console.error(err);
    }
  };

  const loadAttendanceHistory = async () => {
    try {
      setLoading(true);
      const data = await attendanceService.getAttendanceHistory(
        startDate || undefined,
        endDate || undefined,
        selectedEmpId || undefined
      );
      setHistory(data);
    } catch (err) {
      console.error(err);
      alert('Failed to load attendance logs.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadFilterData();
    loadAttendanceHistory();
  }, []);

  const handleFilterSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    loadAttendanceHistory();
  };

  const handleClearFilters = () => {
    setStartDate('');
    setEndDate('');
    setSelectedEmpId('');
    setTimeout(() => {
      loadAttendanceHistory();
    }, 50);
  };

  return (
    <div className="card">
      <h2 className="card-title">⏱️ Shift Attendance Administration</h2>

      {/* Filter Board */}
      <form onSubmit={handleFilterSubmit} style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: '16px', marginBottom: '24px', alignItems: 'end', backgroundColor: '#f8fafc', padding: '16px', borderRadius: '8px', border: '1px solid var(--border)' }}>
        <div className="form-group" style={{ marginBottom: 0 }}>
          <label className="form-label">Start Date</label>
          <input
            type="date"
            className="form-control"
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
          />
        </div>

        <div className="form-group" style={{ marginBottom: 0 }}>
          <label className="form-label">End Date</label>
          <input
            type="date"
            className="form-control"
            value={endDate}
            onChange={(e) => setEndDate(e.target.value)}
          />
        </div>

        <div className="form-group" style={{ marginBottom: 0 }}>
          <label className="form-label">Filter by Employee</label>
          <select
            className="form-control"
            value={selectedEmpId}
            onChange={(e) => setSelectedEmpId(Number(e.target.value) || '')}
          >
            <option value="">-- All Employees --</option>
            {employees.map((emp) => (
              <option key={emp.employeeId} value={emp.employeeId}>
                {emp.firstName} {emp.lastName}
              </option>
            ))}
          </select>
        </div>

        <div style={{ display: 'flex', gap: '8px' }}>
          <button type="submit" className="btn btn-primary" style={{ flexGrow: 1 }}>
            Filter
          </button>
          <button type="button" className="btn btn-secondary" onClick={handleClearFilters}>
            Clear
          </button>
        </div>
      </form>

      {/* Log list grid */}
      {loading ? (
        <div style={{ textAlign: 'center', padding: '40px' }}>Loading history records...</div>
      ) : (
        <div className="table-responsive">
          <table className="table">
            <thead>
              <tr>
                <th>Date</th>
                <th>Employee Name</th>
                <th>Check In</th>
                <th>Check Out</th>
                <th>Status</th>
              </tr>
            </thead>
            <tbody>
              {history.length === 0 ? (
                <tr>
                  <td colSpan={5} style={{ textAlign: 'center', color: 'var(--text-muted)', padding: '20px' }}>
                    No attendance records found for these settings.
                  </td>
                </tr>
              ) : (
                history.map((att) => (
                  <tr key={att.attendanceId}>
                    <td style={{ fontWeight: '500' }}>{new Date(att.date).toLocaleDateString(undefined, { dateStyle: 'medium' })}</td>
                    <td style={{ fontWeight: '600' }}>{att.employeeName}</td>
                    <td>{new Date(att.checkIn).toLocaleTimeString(undefined, { hour: '2-digit', minute: '2-digit' })}</td>
                    <td>
                      {att.checkOut 
                        ? new Date(att.checkOut).toLocaleTimeString(undefined, { hour: '2-digit', minute: '2-digit' })
                        : <span style={{ color: 'var(--text-muted)', fontStyle: 'italic' }}>Shift Active</span>
                      }
                    </td>
                    <td>
                      <span className={`badge badge-${att.status === 'Present' ? 'success' : 'warning'}`}>
                        {att.status}
                      </span>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
};
