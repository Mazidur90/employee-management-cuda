using SchoolMangementSystem.Models;
using SchoolMangementSystem.Repositories;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SchoolMangementSystem.Services
{
    public class HRIntegrationService
    {
        private readonly IEmployeeRepository _repository;

        public HRIntegrationService()
        {
            _repository = new EmployeeRepositoryEF();
        }

        public async Task ImportEmployeesFromHRAsync()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigurationManager.AppSettings["HRApiKey"]);
                var response = await client.GetAsync(ConfigurationManager.AppSettings["HRApiUrl"] + "/employees");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var employees = JsonConvert.DeserializeObject<List<EmployeeRecord>>(json);
                    foreach (var emp in employees)
                    {
                        if (!_repository.EmployeeIdExists(emp.EmployeeId))
                        {
                            _repository.Save(emp, null);
                        }
                    }
                }
            }
        }

        public async Task SyncEmployeeDataAsync(EmployeeRecord record)
        {
            // Example: Send update to HR system
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + ConfigurationManager.AppSettings["HRApiKey"]);
                var json = JsonConvert.SerializeObject(record);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(ConfigurationManager.AppSettings["HRApiUrl"] + "/employees/sync", content);
                // Handle response
            }
        }
    }
}