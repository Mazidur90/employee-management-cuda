using System;
using System.IO;

namespace SchoolMangementSystem.Data
{
    internal static class AppPaths
    {
        private const string ProductFolder = "EmployeeManagementCuda";

        static AppPaths()
        {
            Directory.CreateDirectory(Root);
            Directory.CreateDirectory(DataDirectory);
            Directory.CreateDirectory(EmployeeImageDirectory);
            Directory.CreateDirectory(SupervisorImageDirectory);
        }

        public static string Root
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    ProductFolder);
            }
        }

        public static string DataDirectory => Path.Combine(Root, "Data");

        public static string EmployeesFile => Path.Combine(DataDirectory, "employees.json");

        public static string SupervisorsFile => Path.Combine(DataDirectory, "supervisors.json");

        public static string UsersFile => Path.Combine(DataDirectory, "users.json");

        public static string EmployeeImageDirectory => Path.Combine(Root, "EmployeeImages");

        public static string SupervisorImageDirectory => Path.Combine(Root, "SupervisorImages");
    }
}
