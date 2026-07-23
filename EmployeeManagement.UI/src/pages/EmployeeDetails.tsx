import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { employeeService } from '../services/employeeService';
import { attendanceService } from '../services/attendanceService';
import { Employee, Attendance } from '../types';

export const EmployeeDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [employee, setEmployee] = useState<Employee | null>(null);
  const [attendance, setAttendance] = useState<Attendance[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadEmployeeDetails = async () => {
      if (!id) return;
      try {
        setLoading(true);
        const empId = Number(id);
        const empData = await employeeService.getEmployeeById(empId);
        setEmployee(empData);

        const attData = await attendanceService.getAttendanceHistory(undefined, undefined, empId);
        setAttendance(attData);
      } catch (err) {
        console.error(err);
        alert('Failed to load employee profile details.');
      } finally {
        setLoading(false);
      }
    };
    loadEmployeeDetails();
  }, [id]);

  if (loading) {
    return <div style={{ textAlign: 'center', padding: '40px' }}>Loading profile details...</div>;
  }

  if (!employee) {
    return (
      <div className="card" style={{ textAlign: 'center', padding: '40px' }}>
        <h2>Employee Profile Not Found</h2>
        <p style={{ margin: '15px 0', color: 'var(--text-muted)' }}>The requested profile does not exist.</p>
        <Link to="/employees" className="btn btn-primary">
          Back to Directory
        </Link>
      </div>
    );
  }

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px' }}>
        <Link to="/employees" className="btn btn-secondary">
          ⬅️ Back to Directory
        </Link>
        <Link to={`/employees/${employee.employeeId}/edit`} className="btn btn-primary">
          ✏️ Edit Profile
        </Link>
      </div>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))', gap: '24px', alignItems: 'start' }}>
        
        {/* Profile Card */}
        <div className="card">
          <h2 className="card-title">👤 Employee Profile Summary</h2>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', borderBottom: '1px solid var(--border)', paddingBottom: '8px' }}>
              <span style={{ color: 'var(--text-muted)', fontWeight: '500' }}>ID:</span>
              <span style={{ fontWeight: '600' }}>{employee.employeeId}</span>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', borderBottom: '1px solid var(--border)', paddingBottom: '8px' }}>
              <span style={{ color: 'var(--text-muted)', fontWeight: '500' }}>Full Name:</span>
              <span style={{ fontWeight: '600' }}>{employee.firstName} {employee.lastName}</span>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', borderBottom: '1px solid var(--border)', paddingBottom: '8px' }}>
              <span style={{ color: 'var(--text-muted)', fontWeight: '500' }}>Email Address:</span>
              <span style={{ fontWeight: '600' }}>{employee.email}</span>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', borderBottom: '1px solid var(--border)', paddingBottom: '8px' }}>
              <span style={{ color: 'var(--text-muted)', fontWeight: '500' }}>Phone Number:</span>
              <span style={{ fontWeight: '600' }}>{employee.phone}</span>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', borderBottom: '1px solid var(--border)', paddingBottom: '8px' }}>
              <span style={{ color: 'var(--text-muted)', fontWeight: '500' }}>Department:</span>
              <span style={{ fontWeight: '600' }}>{employee.department}</span>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', borderBottom: '1px solid var(--border)', paddingBottom: '8px' }}>
              <span style={{ color: 'var(--text-muted)', fontWeight: '500' }}>Designation:</span>
              <span style={{ fontWeight: '600' }}>{employee.designation}</span>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', borderBottom: '1px solid var(--border)', paddingBottom: '8px' }}>
              <span style={{ color: 'var(--text-muted)', fontWeight: '500' }}>Salary (Monthly):</span>
              <span style={{ fontWeight: '600', color: 'var(--success)' }}>${Number(employee.salary).toLocaleString(undefined, { minimumFractionDigits: 2 })}</span>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', borderBottom: '1px solid var(--border)', paddingBottom: '8px' }}>
              <span style={{ color: 'var(--text-muted)', fontWeight: '500' }}>Joining Date:</span>
              <span style={{ fontWeight: '600' }}>{new Date(employee.joiningDate).toLocaleDateString(undefined, { dateStyle: 'long' })}</span>
            </div>
            <div style={{ display: 'flex', justifyContent: 'space-between', paddingBottom: '8px' }}>
              <span style={{ color: 'var(--text-muted)', fontWeight: '500' }}>Employment Status:</span>
              <span className={`badge badge-${employee.status.toLowerCase() === 'active' ? 'success' : 'danger'}`}>
                {employee.status}
              </span>
            </div>
          </div>
        </div>

        {/* Attendance Card */}
        <div className="card">
          <h2 className="card-title">⏱️ Attendance Logs ({attendance.length})</h2>
          {attendance.length === 0 ? (
            <div style={{ color: 'var(--text-muted)', fontSize: '14px' }}>No attendance records found for this employee.</div>
          ) : (
            <div className="table-responsive" style={{ maxHeight: '420px' }}>
              <table className="table">
                <thead>
                  <tr>
                    <th>Date</th>
                    <th>Check In</th>
                    <th>Check Out</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  {attendance.map((att) => (
                    <tr key={att.attendanceId}>
                      <td style={{ fontWeight: '500' }}>{new Date(att.date).toLocaleDateString(undefined, { dateStyle: 'medium' })}</td>
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
