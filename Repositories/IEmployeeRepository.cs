using SchoolMangementSystem.Models;
using System.Collections.Generic;

namespace SchoolMangementSystem.Repositories
{
    public interface IEmployeeRepository
    {
        List<EmployeeRecord> GetActiveEmployees();
        List<EmployeeRecord> GetRecentEmployees(int count);
        bool EmployeeIdExists(string employeeId, string excludedEmployeeId = null);
        void Save(EmployeeRecord record, string importedImagePath);
        void SoftDelete(string employeeId);
    }
}