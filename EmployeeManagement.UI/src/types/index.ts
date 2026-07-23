export interface User {
  token: string;
  username: string;
  role: string;
  employeeId?: number;
}

export interface Employee {
  employeeId: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  department: string;
  designation: string;
  salary: number;
  joiningDate: string;
  status: string;
  createdAt: string;
  updatedAt: string;
}

export interface Attendance {
  attendanceId: number;
  employeeId: number;
  employeeName: string;
  date: string;
  checkIn: string;
  checkOut?: string;
  status: string;
}

export interface PaginatedResponse<T> {
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  data: T[];
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}
