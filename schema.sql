-- MySQL Schema for Employee Management System
CREATE DATABASE IF NOT EXISTS employee_db;
USE employee_db;

-- 1. Employees Table
CREATE TABLE IF NOT EXISTS Employees (
    EmployeeId INT AUTO_INCREMENT PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Phone VARCHAR(20) NOT NULL,
    Department VARCHAR(50) NOT NULL,
    Designation VARCHAR(50) NOT NULL,
    Salary DECIMAL(18, 2) NOT NULL,
    JoiningDate DATE NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Active',
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- 2. Users Table (Authentication and Authorization)
CREATE TABLE IF NOT EXISTS Users (
    UserId INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL, -- Stored as plain-text as per comment feedback
    Role VARCHAR(20) NOT NULL DEFAULT 'HR', -- Roles: 'Admin', 'HR'
    EmployeeId INT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId) ON DELETE SET NULL
);

-- 3. Attendance Table
CREATE TABLE IF NOT EXISTS Attendance (
    AttendanceId INT AUTO_INCREMENT PRIMARY KEY,
    EmployeeId INT NOT NULL,
    Date DATE NOT NULL,
    CheckIn DATETIME NOT NULL,
    CheckOut DATETIME NULL,
    Status VARCHAR(20) NOT NULL, -- 'Present', 'Late', 'Absent'
    FOREIGN KEY (EmployeeId) REFERENCES Employees(EmployeeId) ON DELETE CASCADE,
    UNIQUE KEY UQ_Employee_Date (EmployeeId, Date)
);

-- Seed Data (Default Admin User)
-- Username: admin, Password: admin123
INSERT INTO Users (Username, Password, Role, EmployeeId)
VALUES ('admin', 'admin123', 'Admin', NULL)
ON DUPLICATE KEY UPDATE Username=Username;
