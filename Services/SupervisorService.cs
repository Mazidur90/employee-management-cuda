using System;
using System.Collections.Generic;
using System.Linq;
using SchoolMangementSystem.Data;
using SchoolMangementSystem.Models;

namespace SchoolMangementSystem.Services
{
    internal class SupervisorService
    {
        public List<SupervisorRecord> GetActiveSupervisors()
        {
            return LoadAll()
                .Where(supervisor => supervisor.DeletedAtUtc == null)
                .OrderBy(supervisor => supervisor.FullName)
                .ToList();
        }

        public bool SupervisorIdExists(string supervisorId, string excludedSupervisorId = null)
        {
            return LoadAll().Any(supervisor =>
                supervisor.DeletedAtUtc == null &&
                string.Equals(supervisor.SupervisorId, supervisorId, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(supervisor.SupervisorId, excludedSupervisorId, StringComparison.OrdinalIgnoreCase));
        }

        public void Save(SupervisorRecord record, string importedImagePath)
        {
            var supervisors = LoadAll();
            var existing = supervisors.Find(supervisor =>
                string.Equals(supervisor.SupervisorId, record.SupervisorId, StringComparison.OrdinalIgnoreCase));
            var now = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(importedImagePath))
            {
                record.PhotoPath = JsonFileStore.SaveImageCopy(importedImagePath, AppPaths.SupervisorImageDirectory, record.SupervisorId);
            }
            else if (existing != null)
            {
                record.PhotoPath = existing.PhotoPath;
            }

            if (existing == null)
            {
                record.CreatedAtUtc = now;
                record.UpdatedAtUtc = now;
                supervisors.Add(record);
            }
            else
            {
                existing.FullName = record.FullName;
                existing.Gender = record.Gender;
                existing.Address = record.Address;
                existing.Status = record.Status;
                existing.PhotoPath = record.PhotoPath;
                existing.UpdatedAtUtc = now;
                existing.DeletedAtUtc = null;
            }

            JsonFileStore.SaveList(AppPaths.SupervisorsFile, supervisors);
        }

        public void SoftDelete(string supervisorId)
        {
            var supervisors = LoadAll();
            var existing = supervisors.Find(supervisor =>
                string.Equals(supervisor.SupervisorId, supervisorId, StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                return;
            }

            existing.DeletedAtUtc = DateTime.UtcNow;
            existing.UpdatedAtUtc = DateTime.UtcNow;
            JsonFileStore.SaveList(AppPaths.SupervisorsFile, supervisors);
        }

        private List<SupervisorRecord> LoadAll()
        {
            return JsonFileStore.LoadList<SupervisorRecord>(AppPaths.SupervisorsFile);
        }
    }
}
