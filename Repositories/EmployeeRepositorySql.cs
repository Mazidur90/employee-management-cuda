using SchoolMangementSystem.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System;

namespace SchoolMangementSystem.Repositories
{
    public class EmployeeRepositorySql : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepositorySql()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["EmployeeDbConnectionString"].ConnectionString;
        }

        public List<EmployeeRecord> GetActiveEmployees()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<EmployeeRecord>("SELECT * FROM Employees WHERE DeletedAtUtc IS NULL ORDER BY FullName").ToList();
            }
        }

        public List<EmployeeRecord> GetRecentEmployees(int count)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                return connection.Query<EmployeeRecord>("SELECT TOP (@Count) * FROM Employees WHERE DeletedAtUtc IS NULL ORDER BY UpdatedAtUtc DESC", new { Count = count }).ToList();
            }
        }

        public bool EmployeeIdExists(string employeeId, string excludedEmployeeId = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "SELECT COUNT(*) FROM Employees WHERE DeletedAtUtc IS NULL AND EmployeeId = @EmployeeId";
                var parameters = new { EmployeeId = employeeId };
                if (!string.IsNullOrEmpty(excludedEmployeeId))
                {
                    sql += " AND EmployeeId != @ExcludedEmployeeId";
                    parameters = new { EmployeeId = employeeId, ExcludedEmployeeId = excludedEmployeeId };
                }
                return connection.ExecuteScalar<int>(sql, parameters) > 0;
            }
        }

        public void Save(EmployeeRecord record, string importedImagePath)
        {
            var now = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(importedImagePath))
            {
                record.PhotoPath = SchoolMangementSystem.Data.JsonFileStore.SaveImageCopy(importedImagePath, SchoolMangementSystem.Data.AppPaths.EmployeeImageDirectory, record.EmployeeId);
            }
            else
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var existing = connection.QueryFirstOrDefault<EmployeeRecord>("SELECT PhotoPath FROM Employees WHERE EmployeeId = @EmployeeId", new { EmployeeId = record.EmployeeId });
                    if (existing != null)
                    {
                        record.PhotoPath = existing.PhotoPath;
                    }
                }
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var existing = connection.QueryFirstOrDefault<EmployeeRecord>("SELECT * FROM Employees WHERE EmployeeId = @EmployeeId", new { EmployeeId = record.EmployeeId });
                if (existing == null)
                {
                    record.CreatedAtUtc = now;
                    record.UpdatedAtUtc = now;
                    connection.Execute("INSERT INTO Employees (EmployeeId, FullName, Gender, Address, JobTitle, Department, Status, PhotoPath, CreatedAtUtc, UpdatedAtUtc, DeletedAtUtc) VALUES (@EmployeeId, @FullName, @Gender, @Address, @JobTitle, @Department, @Status, @PhotoPath, @CreatedAtUtc, @UpdatedAtUtc, @DeletedAtUtc)", record);
                }
                else
                {
                    record.CreatedAtUtc = existing.CreatedAtUtc;
                    record.UpdatedAtUtc = now;
                    record.DeletedAtUtc = null;
                    connection.Execute("UPDATE Employees SET FullName = @FullName, Gender = @Gender, Address = @Address, JobTitle = @JobTitle, Department = @Department, Status = @Status, PhotoPath = @PhotoPath, UpdatedAtUtc = @UpdatedAtUtc, DeletedAtUtc = @DeletedAtUtc WHERE EmployeeId = @EmployeeId", record);
                }
            }
        }

        public void SoftDelete(string employeeId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Execute("UPDATE Employees SET DeletedAtUtc = @DeletedAtUtc, UpdatedAtUtc = @UpdatedAtUtc WHERE EmployeeId = @EmployeeId", new { EmployeeId = employeeId, DeletedAtUtc = DateTime.UtcNow, UpdatedAtUtc = DateTime.UtcNow });
            }
        }
    }
}