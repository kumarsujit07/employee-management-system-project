import React, { useEffect, useState } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { employeeService } from '../services/employeeService';

interface FormErrors {
  [key: string]: string;
}

export const EmployeeForm: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const isEditMode = !!id;
  const navigate = useNavigate();

  // Form Fields
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phone: '',
    department: '',
    designation: '',
    salary: 0,
    joiningDate: '',
    status: 'Active',
  });

  const [errors, setErrors] = useState<FormErrors>({});
  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(false);

  useEffect(() => {
    if (isEditMode) {
      const fetchEmployee = async () => {
        try {
          setFetching(true);
          const emp = await employeeService.getEmployeeById(Number(id));
          setFormData({
            firstName: emp.firstName,
            lastName: emp.lastName,
            email: emp.email,
            phone: emp.phone,
            department: emp.department,
            designation: emp.designation,
            salary: emp.salary,
            // Format joining date to YYYY-MM-DD for date inputs
            joiningDate: new Date(emp.joiningDate).toISOString().split('T')[0],
            status: emp.status,
          });
        } catch (err) {
          console.error(err);
          alert('Failed to load employee details for editing.');
          navigate('/employees');
        } finally {
          setFetching(false);
        }
      };
      fetchEmployee();
    }
  }, [id, isEditMode]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: name === 'salary' ? Number(value) || 0 : value,
    }));
  };

  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};

    if (!formData.firstName.trim()) newErrors.firstName = 'First name is required.';
    if (!formData.lastName.trim()) newErrors.lastName = 'Last name is required.';
    
    // Email Check
    if (!formData.email.trim()) {
      newErrors.email = 'Email address is required.';
    } else {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailRegex.test(formData.email)) {
        newErrors.email = 'Invalid email address format.';
      }
    }

    if (!formData.phone.trim()) newErrors.phone = 'Phone number is required.';
    if (!formData.department.trim()) newErrors.department = 'Department is required.';
    if (!formData.designation.trim()) newErrors.designation = 'Designation is required.';
    
    // Salary Check
    if (formData.salary <= 0) {
      newErrors.salary = 'Salary must be greater than zero.';
    }

    if (!formData.joiningDate) newErrors.joiningDate = 'Joining date is required.';
    if (!formData.status) newErrors.status = 'Status is required.';

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateForm()) return;

    setLoading(true);
    try {
      if (isEditMode) {
        await employeeService.updateEmployee(Number(id), {
          employeeId: Number(id),
          ...formData,
        });
        alert('Employee updated successfully.');
      } else {
        await employeeService.createEmployee(formData);
        alert('Employee created successfully.');
      }
      navigate('/employees');
    } catch (err: any) {
      if (err.response && err.response.data && err.response.data.message) {
        setErrors({ form: err.response.data.message });
      } else {
        setErrors({ form: 'An error occurred while saving the profile.' });
      }
    } finally {
      setLoading(false);
    }
  };

  if (fetching) {
    return <div style={{ textAlign: 'center', padding: '40px' }}>Fetching profile information...</div>;
  }

  return (
    <div className="card" style={{ maxWidth: '600px', margin: '0 auto' }}>
      <h2 className="card-title">{isEditMode ? '✏️ Edit Employee Details' : '👤 Add New Employee'}</h2>

      {errors.form && <div className="alert alert-danger">{errors.form}</div>}

      <form onSubmit={handleSubmit}>
        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
          <div className="form-group">
            <label className="form-label" htmlFor="firstName">First Name</label>
            <input
              type="text"
              id="firstName"
              name="firstName"
              className="form-control"
              value={formData.firstName}
              onChange={handleInputChange}
              disabled={loading}
            />
            {errors.firstName && <div className="form-error">{errors.firstName}</div>}
          </div>

          <div className="form-group">
            <label className="form-label" htmlFor="lastName">Last Name</label>
            <input
              type="text"
              id="lastName"
              name="lastName"
              className="form-control"
              value={formData.lastName}
              onChange={handleInputChange}
              disabled={loading}
            />
            {errors.lastName && <div className="form-error">{errors.lastName}</div>}
          </div>
        </div>

        <div className="form-group">
          <label className="form-label" htmlFor="email">Email Address</label>
          <input
            type="email"
            id="email"
            name="email"
            className="form-control"
            value={formData.email}
            onChange={handleInputChange}
            disabled={loading}
          />
          {errors.email && <div className="form-error">{errors.email}</div>}
        </div>

        <div className="form-group">
          <label className="form-label" htmlFor="phone">Phone Number</label>
          <input
            type="text"
            id="phone"
            name="phone"
            className="form-control"
            value={formData.phone}
            onChange={handleInputChange}
            disabled={loading}
          />
          {errors.phone && <div className="form-error">{errors.phone}</div>}
        </div>

        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
          <div className="form-group">
            <label className="form-label" htmlFor="department">Department</label>
            <input
              type="text"
              id="department"
              name="department"
              className="form-control"
              placeholder="e.g. Engineering"
              value={formData.department}
              onChange={handleInputChange}
              disabled={loading}
            />
            {errors.department && <div className="form-error">{errors.department}</div>}
          </div>

          <div className="form-group">
            <label className="form-label" htmlFor="designation">Designation</label>
            <input
              type="text"
              id="designation"
              name="designation"
              className="form-control"
              placeholder="e.g. Lead Engineer"
              value={formData.designation}
              onChange={handleInputChange}
              disabled={loading}
            />
            {errors.designation && <div className="form-error">{errors.designation}</div>}
          </div>
        </div>

        <div className="form-group">
          <label className="form-label" htmlFor="salary">Salary (Monthly USD)</label>
          <input
            type="number"
            id="salary"
            name="salary"
            className="form-control"
            min="0"
            step="0.01"
            value={formData.salary || ''}
            onChange={handleInputChange}
            disabled={loading}
          />
          {errors.salary && <div className="form-error">{errors.salary}</div>}
        </div>

        <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
          <div className="form-group">
            <label className="form-label" htmlFor="joiningDate">Joining Date</label>
            <input
              type="date"
              id="joiningDate"
              name="joiningDate"
              className="form-control"
              value={formData.joiningDate}
              onChange={handleInputChange}
              disabled={loading}
            />
            {errors.joiningDate && <div className="form-error">{errors.joiningDate}</div>}
          </div>

          <div className="form-group">
            <label className="form-label" htmlFor="status">Employment Status</label>
            <select
              id="status"
              name="status"
              className="form-control"
              value={formData.status}
              onChange={handleInputChange}
              disabled={loading}
            >
              <option value="Active">Active</option>
              <option value="Inactive">Inactive</option>
            </select>
            {errors.status && <div className="form-error">{errors.status}</div>}
          </div>
        </div>

        <div style={{ display: 'flex', gap: '12px', marginTop: '24px', justifyContent: 'flex-end' }}>
          <Link to="/employees" className="btn btn-secondary">
            Cancel
          </Link>
          <button type="submit" className="btn btn-primary" disabled={loading}>
            {loading ? 'Saving Changes...' : 'Save Profile'}
          </button>
        </div>
      </form>
    </div>
  );
};
