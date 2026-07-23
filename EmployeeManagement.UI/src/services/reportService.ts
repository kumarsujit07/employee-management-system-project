import api from './api';

export const reportService = {
  async downloadReport(endpoint: string, defaultFileName: string): Promise<void> {
    const response = await api.get(endpoint, { responseType: 'blob' });
    const contentType = (response.headers['content-type'] as string) || 'application/octet-stream';
    const blob = new Blob([response.data], { type: contentType });
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = defaultFileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  },

  async downloadEmployeeDirectoryPdf(): Promise<void> {
    await this.downloadReport('/reports/employees/pdf', 'Employee_Directory.pdf');
  },

  async downloadEmployeeDirectoryExcel(): Promise<void> {
    await this.downloadReport('/reports/employees/excel', 'Employee_Directory.xlsx');
  },

  async downloadAttendanceReportPdf(): Promise<void> {
    await this.downloadReport('/reports/attendance/pdf', 'Attendance_Report.pdf');
  },

  async downloadAttendanceReportExcel(): Promise<void> {
    await this.downloadReport('/reports/attendance/excel', 'Attendance_Report.xlsx');
  },

  async downloadSalaryReportPdf(): Promise<void> {
    await this.downloadReport('/reports/salary/pdf', 'Salary_Payroll_Report.pdf');
  },

  async downloadDepartmentReportPdf(): Promise<void> {
    await this.downloadReport('/reports/departments/pdf', 'Department_Summary_Report.pdf');
  }
};
