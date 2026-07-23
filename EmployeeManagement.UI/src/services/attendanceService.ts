import api from './api';
import { ApiResponse, Attendance } from '../types';

export const attendanceService = {
  async getAttendanceHistory(startDate?: string, endDate?: string, employeeId?: number): Promise<Attendance[]> {
    const params: any = {};
    if (startDate) params.startDate = startDate;
    if (endDate) params.endDate = endDate;
    if (employeeId) params.employeeId = employeeId;

    const response = await api.get<ApiResponse<Attendance[]>>('/attendance', { params });
    return response.data.data;
  },

  async checkIn(employeeId: number): Promise<Attendance> {
    const response = await api.post<ApiResponse<Attendance>>('/attendance', { employeeId });
    return response.data.data;
  },

  async checkOut(attendanceId: number): Promise<Attendance> {
    const response = await api.put<ApiResponse<Attendance>>(`/attendance/${attendanceId}`);
    return response.data.data;
  }
};
