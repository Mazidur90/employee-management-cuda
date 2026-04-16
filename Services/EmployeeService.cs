using System;
using System.Collections.Generic;
using System.Linq;
using SchoolMangementSystem.Data;
using SchoolMangementSystem.Models;

namespace SchoolMangementSystem.Services
{
    internal class EmployeeService
    {
        public List<EmployeeRecord> GetActiveEmployees()
        {
            return LoadAll()
                .Where(employee => employee.DeletedAtUtc == null)
                .OrderBy(employee => employee.FullName)
                .ToList();
        }

        public List<EmployeeRecord> GetRecentEmployees(int count)
        {
            return GetActiveEmployees()
                .OrderByDescending(employee => employee.UpdatedAtUtc)
                .Take(count)
                .ToList();
        }

        public bool EmployeeIdExists(string employeeId, string excludedEmployeeId = null)
        {
            return LoadAll().Any(employee =>
                employee.DeletedAtUtc == null &&
                string.Equals(employee.EmployeeId, employeeId, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(employee.EmployeeId, excludedEmployeeId, StringComparison.OrdinalIgnoreCase));
        }

        public void Save(EmployeeRecord record, string importedImagePath)
        {
            var employees = LoadAll();
            var existing = employees.Find(employee =>
                string.Equals(employee.EmployeeId, record.EmployeeId, StringComparison.OrdinalIgnoreCase));
            var now = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(importedImagePath))
            {
                record.PhotoPath = JsonFileStore.SaveImageCopy(importedImagePath, AppPaths.EmployeeImageDirectory, record.EmployeeId);
            }
            else if (existing != null)
            {
                record.PhotoPath = existing.PhotoPath;
            }

            if (existing == null)
            {
                record.CreatedAtUtc = now;
                record.UpdatedAtUtc = now;
                employees.Add(record);
            }
            else
            {
                existing.FullName = record.FullName;
                existing.Gender = record.Gender;
                existing.Address = record.Address;
                existing.JobTitle = record.JobTitle;
                existing.Department = record.Department;
                existing.Status = record.Status;
                existing.PhotoPath = record.PhotoPath;
                existing.UpdatedAtUtc = now;
                existing.DeletedAtUtc = null;
            }

            JsonFileStore.SaveList(AppPaths.EmployeesFile, employees);
        }

        public void SoftDelete(string employeeId)
        {
            var employees = LoadAll();
            var existing = employees.Find(employee =>
                string.Equals(employee.EmployeeId, employeeId, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                return;
            }

            existing.DeletedAtUtc = DateTime.UtcNow;
            existing.UpdatedAtUtc = DateTime.UtcNow;
            JsonFileStore.SaveList(AppPaths.EmployeesFile, employees);
        }

        private List<EmployeeRecord> LoadAll()
        {
            return JsonFileStore.LoadList<EmployeeRecord>(AppPaths.EmployeesFile);
        }
    }
}
