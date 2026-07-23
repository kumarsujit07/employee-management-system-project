import api from './api';
import { ApiResponse, Employee, PaginatedResponse } from '../types';

export const employeeService = {
  async getEmployees(
    search?: string,
    sortBy?: string,
    sortOrder?: string,
    page = 1,
    pageSize = 10
  ): Promise<PaginatedResponse<Employee>> {
    const params: any = { page, pageSize };
    if (search) params.search = search;
    if (sortBy) params.sortBy = sortBy;
    if (sortOrder) params.sortOrder = sortOrder;

    const response = await api.get<ApiResponse<PaginatedResponse<Employee>>>('/employees', { params });
    return response.data.data;
  },

  async getEmployeeById(id: number): Promise<Employee> {
    const response = await api.get<ApiResponse<Employee>>(`/employees/${id}`);
    return response.data.data;
  },

  async createEmployee(employee: Omit<Employee, 'employeeId' | 'createdAt' | 'updatedAt'>): Promise<Employee> {
    const response = await api.post<ApiResponse<Employee>>('/employees', employee);
    return response.data.data;
  },

  async updateEmployee(id: number, employee: Omit<Employee, 'createdAt' | 'updatedAt'>): Promise<Employee> {
    const response = await api.put<ApiResponse<Employee>>(`/employees/${id}`, employee);
    return response.data.data;
  },

  async deleteEmployee(id: number): Promise<void> {
    await api.delete<ApiResponse<any>>(`/employees/${id}`);
  },

  async deleteMultipleEmployees(ids: number[]): Promise<void> {
    await api.delete<ApiResponse<any>>('/employees/delete-multiple', { data: ids });
  }
};
