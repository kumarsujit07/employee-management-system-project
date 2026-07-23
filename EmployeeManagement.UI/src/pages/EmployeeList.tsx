import React, { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { employeeService } from '../services/employeeService';
import { Employee } from '../types';

export const EmployeeList: React.FC = () => {
  const [employees, setEmployees] = useState<Employee[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [sortBy, setSortBy] = useState('EmployeeId');
  const [sortOrder, setSortOrder] = useState('asc');
  const [loading, setLoading] = useState(true);

  // Selection state for multi-delete
  const [selectedIds, setSelectedIds] = useState<number[]>([]);

  const loadEmployees = async () => {
    try {
      setLoading(true);
      const result = await employeeService.getEmployees(search, sortBy, sortOrder, page, 10);
      setEmployees(result.data);
      setTotalCount(result.totalCount);
      setTotalPages(result.totalPages);
    } catch (err) {
      console.error('Error fetching employees:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadEmployees();
  }, [page, sortBy, sortOrder]);

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
    loadEmployees();
  };

  const handleSort = (field: string) => {
    if (sortBy === field) {
      setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
    } else {
      setSortBy(field);
      setSortOrder('asc');
    }
    setPage(1);
  };

  const handleDeleteSingle = async (id: number) => {
    if (!window.confirm(`Are you sure you want to delete employee with ID: ${id}?`)) {
      return;
    }
    try {
      await employeeService.deleteEmployee(id);
      alert('Employee deleted successfully.');
      loadEmployees();
      setSelectedIds((prev) => prev.filter((selectedId) => selectedId !== id));
    } catch (err) {
      console.error(err);
      alert('Delete failed.');
    }
  };

  const handleSelectAll = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.checked) {
      const allIds = employees.map((emp) => emp.employeeId);
      setSelectedIds(allIds);
    } else {
      setSelectedIds([]);
    }
  };

  const handleSelectRow = (id: number, checked: boolean) => {
    if (checked) {
      setSelectedIds((prev) => [...prev, id]);
    } else {
      setSelectedIds((prev) => prev.filter((x) => x !== id));
    }
  };

  const handleDeleteMultiple = async () => {
    if (selectedIds.length === 0) return;
    if (!window.confirm(`Are you sure you want to delete the ${selectedIds.length} selected employees?`)) {
      return;
    }
    try {
      await employeeService.deleteMultipleEmployees(selectedIds);
      alert('Selected employees deleted successfully.');
      setSelectedIds([]);
      loadEmployees();
    } catch (err) {
      console.error(err);
      alert('Batch delete failed.');
    }
  };

  const renderSortIndicator = (field: string) => {
    if (sortBy !== field) return null;
    return sortOrder === 'asc' ? ' 🔼' : ' 🔽';
  };

  return (
    <div className="card">
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '20px', flexWrap: 'wrap', gap: '15px' }}>
        <h2 style={{ fontSize: '18px', fontWeight: '600' }}>Directory Directory</h2>
        <div style={{ display: 'flex', gap: '10px' }}>
          {selectedIds.length > 0 && (
            <button className="btn btn-danger" onClick={handleDeleteMultiple}>
              🗑️ Delete Selected ({selectedIds.length})
            </button>
          )}
          <Link to="/employees/new" className="btn btn-primary">
            ➕ Add Employee
          </Link>
        </div>
      </div>

      {/* Toolbar (Search) */}
      <div className="toolbar">
        <form onSubmit={handleSearchSubmit} className="search-input-wrapper">
          <input
            type="text"
            className="form-control"
            placeholder="Search by name, email, department..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
          <button type="submit" className="btn btn-secondary" style={{ position: 'absolute', right: '4px', top: '4px', padding: '6px 12px', fontSize: '12px' }}>
            Search
          </button>
        </form>
      </div>

      {loading ? (
        <div style={{ textAlign: 'center', padding: '40px' }}>Loading employees list...</div>
      ) : (
        <div className="table-responsive">
          <table className="table">
            <thead>
              <tr>
                <th style={{ width: '40px' }}>
                  <input
                    type="checkbox"
                    className="checkbox-custom"
                    checked={employees.length > 0 && selectedIds.length === employees.length}
                    onChange={handleSelectAll}
                  />
                </th>
                <th onClick={() => handleSort('employeeid')}>ID {renderSortIndicator('employeeid')}</th>
                <th onClick={() => handleSort('firstname')}>First Name {renderSortIndicator('firstname')}</th>
                <th onClick={() => handleSort('lastname')}>Last Name {renderSortIndicator('lastname')}</th>
                <th onClick={() => handleSort('email')}>Email {renderSortIndicator('email')}</th>
                <th>Phone</th>
                <th>Department</th>
                <th>Designation</th>
                <th onClick={() => handleSort('salary')}>Salary {renderSortIndicator('salary')}</th>
                <th onClick={() => handleSort('joiningdate')}>Joining Date {renderSortIndicator('joiningdate')}</th>
                <th onClick={() => handleSort('status')}>Status {renderSortIndicator('status')}</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {employees.length === 0 ? (
                <tr>
                  <td colSpan={12} style={{ textAlign: 'center', color: 'var(--text-muted)', padding: '20px' }}>
                    No employees found.
                  </td>
                </tr>
              ) : (
                employees.map((emp) => (
                  <tr key={emp.employeeId}>
                    <td>
                      <input
                        type="checkbox"
                        className="checkbox-custom"
                        checked={selectedIds.includes(emp.employeeId)}
                        onChange={(e) => handleSelectRow(emp.employeeId, e.target.checked)}
                      />
                    </td>
                    <td>{emp.employeeId}</td>
                    <td>
                      <Link to={`/employees/${emp.employeeId}`} style={{ color: 'var(--primary)', fontWeight: '500', textDecoration: 'none' }}>
                        {emp.firstName}
                      </Link>
                    </td>
                    <td>{emp.lastName}</td>
                    <td>{emp.email}</td>
                    <td>{emp.phone}</td>
                    <td>{emp.department}</td>
                    <td>{emp.designation}</td>
                    <td>${Number(emp.salary).toLocaleString(undefined, { minimumFractionDigits: 2 })}</td>
                    <td>{new Date(emp.joiningDate).toLocaleDateString()}</td>
                    <td>
                      <span className={`badge badge-${emp.status.toLowerCase() === 'active' ? 'success' : 'danger'}`}>
                        {emp.status}
                      </span>
                    </td>
                    <td>
                      <div style={{ display: 'flex', gap: '8px' }}>
                        <Link to={`/employees/${emp.employeeId}/edit`} className="btn btn-secondary" style={{ padding: '6px 10px', fontSize: '12px' }}>
                          Edit
                        </Link>
                        <button className="btn btn-secondary" style={{ padding: '6px 10px', fontSize: '12px', color: 'var(--danger)' }} onClick={() => handleDeleteSingle(emp.employeeId)}>
                          Delete
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      )}

      {/* Pagination Footer */}
      {!loading && totalPages > 1 && (
        <div className="pagination">
          <div className="pagination-info">
            Showing Page <strong>{page}</strong> of <strong>{totalPages}</strong> (Total: {totalCount} records)
          </div>
          <div className="pagination-buttons">
            <button className="btn btn-secondary" onClick={() => setPage((p) => Math.max(p - 1, 1))} disabled={page === 1}>
              Previous
            </button>
            <button className="btn btn-secondary" onClick={() => setPage((p) => Math.min(p + 1, totalPages))} disabled={page === totalPages}>
              Next
            </button>
          </div>
        </div>
      )}
    </div>
  );
};
