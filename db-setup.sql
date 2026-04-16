CREATE DATABASE EmployeeDB;

USE EmployeeDB;

CREATE TABLE Employees (
    EmployeeId NVARCHAR(50) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Gender NVARCHAR(10),
    Address NVARCHAR(200),
    JobTitle NVARCHAR(50),
    Department NVARCHAR(50),
    Status NVARCHAR(20),
    PhotoPath NVARCHAR(200),
    CreatedAtUtc DATETIME NOT NULL,
    UpdatedAtUtc DATETIME NOT NULL,
    DeletedAtUtc DATETIME NULL
);