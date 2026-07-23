# Employee Management System

A Full Stack Employee Management System built with **React.js, TypeScript, ASP.NET Core Web API, MySQL, JWT Authentication, and Layered Architecture**.

## Project Overview

This application is designed to replace manual HR record-keeping by providing a centralized system for managing employee information, attendance, and reports.

The system includes secure authentication, employee management, attendance tracking, and PDF/Excel report generation.

---

## Features

### Authentication
- JWT Authentication
- Password Hashing
- Secure Login
- Role-based Authorization (if implemented)

### Employee Management
- Add Employee
- Update Employee
- Delete Employee
- Delete Multiple Employees
- View Employee List
- Search Employees

### Attendance
- Mark Attendance
- Check-In / Check-Out
- Attendance History

### Reports
- Employee Report
- Attendance Report
- Department Report
- Salary Report
- Export to PDF
- Export to Excel

---

## Technology Stack

### Frontend
- React.js
- TypeScript
- React Router
- Axios

### Backend
- ASP.NET Core Web API
- C#
- JWT Authentication
- Repository Pattern
- Layered Architecture
- Dependency Injection

### Database
- MySQL

---

## Project Architecture

```
React + TypeScript
        │
      Axios
        │
ASP.NET Core Web API
        │
    Controllers
        │
      Services
        │
   Repositories
        │
       MySQL
```

---

## Folder Structure

```
employee-management-system

├── EmployeeManagement.API
├── EmployeeManagement.Client
├── Database
├── Docs
└── README.md
```

---

## API Modules

- Authentication
- Employee Management
- Attendance
- Reports

---

## Getting Started

### Backend

```bash
cd EmployeeManagement.API
dotnet restore
dotnet run
```

### Frontend

```bash
cd EmployeeManagement.Client
npm install
npm run dev
```

---

## Database

- MySQL
- SQL Script available in the `Database` folder.

---

## Project Status

🚧 Under Development

Current Progress:

- [ ] Authentication
- [ ] Employee CRUD
- [ ] Attendance
- [ ] Reports
- [ ] UI Integration

---

## Author

**Sujit Kumar Malik**

GitHub:
https://github.com/kumarsujit07
