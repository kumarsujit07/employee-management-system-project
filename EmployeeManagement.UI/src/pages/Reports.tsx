import React, { useState } from 'react';
import { reportService } from '../services/reportService';

export const Reports: React.FC = () => {
  const [downloading, setDownloading] = useState<string | null>(null);

  const handleDownload = async (reportName: string, downloadFn: () => Promise<void>) => {
    try {
      setDownloading(reportName);
      await downloadFn();
    } catch (err) {
      console.error(err);
      alert(`Failed to download report: ${reportName}`);
    } finally {
      setDownloading(null);
    }
  };

  return (
    <div>
      <h2 style={{ fontSize: '18px', fontWeight: '600', marginBottom: '20px' }}>📈 Management Reports Center</h2>

      <div className="report-grid">
        {/* Employee Directory */}
        <div className="report-card">
          <div>
            <h3 className="report-title">👥 Employee Directory</h3>
            <p className="report-desc">Full directory of active and inactive staff, contact details, designations, and joining records.</p>
          </div>
          <div className="report-actions">
            <button
              className="btn btn-primary"
              onClick={() => handleDownload('Employee PDF', () => reportService.downloadEmployeeDirectoryPdf())}
              disabled={downloading !== null}
            >
              {downloading === 'Employee PDF' ? 'Downloading...' : '📄 PDF'}
            </button>
            <button
              className="btn btn-secondary"
              onClick={() => handleDownload('Employee Excel', () => reportService.downloadEmployeeDirectoryExcel())}
              disabled={downloading !== null}
            >
              {downloading === 'Employee Excel' ? 'Downloading...' : '📊 Excel'}
            </button>
          </div>
        </div>

        {/* Attendance Summary */}
        <div className="report-card">
          <div>
            <h3 className="report-title">⏱️ Attendance Registry</h3>
            <p className="report-desc">Shift check-in/out records, delay metrics, working hours logs, and presence status.</p>
          </div>
          <div className="report-actions">
            <button
              className="btn btn-primary"
              onClick={() => handleDownload('Attendance PDF', () => reportService.downloadAttendanceReportPdf())}
              disabled={downloading !== null}
            >
              {downloading === 'Attendance PDF' ? 'Downloading...' : '📄 PDF'}
            </button>
            <button
              className="btn btn-secondary"
              onClick={() => handleDownload('Attendance Excel', () => reportService.downloadAttendanceReportExcel())}
              disabled={downloading !== null}
            >
              {downloading === 'Attendance Excel' ? 'Downloading...' : '📊 Excel'}
            </button>
          </div>
        </div>

        {/* Salary & Payroll brackets */}
        <div className="report-card">
          <div>
            <h3 className="report-title">💵 Salary & Payroll Summary</h3>
            <p className="report-desc">Provides total monthly payroll metrics, average salaries, payroll summaries, and payscale distributions.</p>
          </div>
          <div className="report-actions">
            <button
              className="btn btn-primary"
              onClick={() => handleDownload('Salary PDF', () => reportService.downloadSalaryReportPdf())}
              disabled={downloading !== null}
            >
              {downloading === 'Salary PDF' ? 'Downloading...' : '📄 PDF'}
            </button>
          </div>
        </div>

        {/* Department stats */}
        <div className="report-card">
          <div>
            <h3 className="report-title">🏢 Department Audits</h3>
            <p className="report-desc">Overview of headcounts by department, total budget expense per division, and average compensation levels.</p>
          </div>
          <div className="report-actions">
            <button
              className="btn btn-primary"
              onClick={() => handleDownload('Department PDF', () => reportService.downloadDepartmentReportPdf())}
              disabled={downloading !== null}
            >
              {downloading === 'Department PDF' ? 'Downloading...' : '📄 PDF'}
            </button>
          </div>
        </div>

      </div>
    </div>
  );
};
