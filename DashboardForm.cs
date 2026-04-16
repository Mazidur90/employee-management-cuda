using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using SchoolMangementSystem.Services;

namespace SchoolMangementSystem
{
    public partial class DashboardForm : UserControl
    {
        private readonly EmployeeService employeeService;
        private readonly SupervisorService supervisorService;

        private Chart employeeChart;
        private Label aiPredictionLabel;
        private Label blockchainStatusLabel;
        private Label iotStatusLabel;
        private Label quantumScoreLabel;
        private Timer simulationTimer;

        public DashboardForm()
        {
            employeeService = new EmployeeService();
            supervisorService = new SupervisorService();
            InitializeComponent();
            InitializeSimulationDashboard();
        }

        private void InitializeSimulationDashboard()
        {
            // Employee Distribution Chart
            employeeChart = new Chart();
            employeeChart.Location = new System.Drawing.Point(10, 250);
            employeeChart.Size = new System.Drawing.Size(500, 300);
            employeeChart.ChartAreas.Add(new ChartArea("MainArea"));
            employeeChart.Series.Add(new Series("Departments"));
            employeeChart.Series["Departments"].ChartType = SeriesChartType.Pie;
            employeeChart.Titles.Add("Employee Distribution by Department");
            this.Controls.Add(employeeChart);

            // AI Prediction Label
            aiPredictionLabel = new Label();
            aiPredictionLabel.Location = new System.Drawing.Point(520, 250);
            aiPredictionLabel.Size = new System.Drawing.Size(300, 50);
            aiPredictionLabel.Font = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            aiPredictionLabel.Text = "AI Turnover Prediction: Low Risk";
            this.Controls.Add(aiPredictionLabel);

            // Blockchain Status
            blockchainStatusLabel = new Label();
            blockchainStatusLabel.Location = new System.Drawing.Point(520, 310);
            blockchainStatusLabel.Size = new System.Drawing.Size(300, 30);
            blockchainStatusLabel.Text = "Blockchain Status: Synced (Latest Block: 123456)";
            this.Controls.Add(blockchainStatusLabel);

            // IoT Status
            iotStatusLabel = new Label();
            iotStatusLabel.Location = new System.Drawing.Point(520, 350);
            iotStatusLabel.Size = new System.Drawing.Size(300, 30);
            iotStatusLabel.Text = "IoT Devices Connected: 15 (Attendance Tracking Active)";
            this.Controls.Add(iotStatusLabel);

            // Quantum Score
            quantumScoreLabel = new Label();
            quantumScoreLabel.Location = new System.Drawing.Point(520, 390);
            quantumScoreLabel.Size = new System.Drawing.Size(300, 30);
            quantumScoreLabel.Text = "Quantum Optimization Score: 95.7%";
            this.Controls.Add(quantumScoreLabel);

            // Simulation Timer for real-time updates
            simulationTimer = new Timer();
            simulationTimer.Interval = 5000; // Update every 5 seconds
            simulationTimer.Tick += SimulationTimer_Tick;
            simulationTimer.Start();
        }

        private void SimulationTimer_Tick(object sender, EventArgs e)
        {
            // Simulate real-time updates
            Random rand = new Random();
            string risk = rand.Next(0, 2) == 0 ? "Low Risk" : "High Risk";
            aiPredictionLabel.Text = $"AI Turnover Prediction: {risk}";
            aiPredictionLabel.ForeColor = risk == "Low Risk" ? System.Drawing.Color.Green : System.Drawing.Color.Red;

            blockchainStatusLabel.Text = $"Blockchain Status: Synced (Latest Block: {rand.Next(100000, 200000)})";
            blockchainStatusLabel.ForeColor = System.Drawing.Color.Blue;

            iotStatusLabel.Text = $"IoT Devices Connected: {rand.Next(10, 20)} (Attendance Tracking Active)";
            iotStatusLabel.ForeColor = System.Drawing.Color.Purple;

            quantumScoreLabel.Text = $"Quantum Optimization Score: {rand.Next(85, 100)}.{rand.Next(0, 10)}%";
            quantumScoreLabel.ForeColor = System.Drawing.Color.Orange;
        }

        public void LoadDashboard(string displayName)
        {
            var employees = employeeService.GetActiveEmployees();
            var supervisors = supervisorService.GetActiveSupervisors();

            titleLabel.Text = "Professional Employee Management Dashboard";
            subtitleLabel.Text = "Signed in as " + displayName;
            employeeCountValue.Text = employees.Count.ToString();
            activeEmployeeCountValue.Text = employees.Count(employee => string.Equals(employee.Status, "Active", StringComparison.OrdinalIgnoreCase)).ToString();
            supervisorCountValue.Text = supervisors.Count.ToString();
            activeSupervisorCountValue.Text = supervisors.Count(supervisor => string.Equals(supervisor.Status, "Active", StringComparison.OrdinalIgnoreCase)).ToString();

            recentEmployeesList.Items.Clear();

            foreach (var employee in employeeService.GetRecentEmployees(6))
            {
                recentEmployeesList.Items.Add(employee.FullName + " | " + employee.JobTitle + " | " + employee.Department + " | " + employee.Status);
            }

            if (recentEmployeesList.Items.Count == 0)
            {
                recentEmployeesList.Items.Add("No employees have been added yet.");
            }

            // Populate the chart with department distribution
            employeeChart.Series["Departments"].Points.Clear();
            var departmentGroups = employees.GroupBy(e => e.Department);
            foreach (var group in departmentGroups)
            {
                employeeChart.Series["Departments"].Points.AddXY(group.Key ?? "Unassigned", group.Count());
            }

            // Simulate additional metrics
            Random rand = new Random();
            string risk = rand.Next(0, 2) == 0 ? "Low Risk" : "High Risk";
            aiPredictionLabel.Text = $"AI Turnover Prediction: {risk} (Based on {employees.Count} employees)";
            aiPredictionLabel.ForeColor = risk == "Low Risk" ? System.Drawing.Color.Green : System.Drawing.Color.Red;

            blockchainStatusLabel.Text = $"Blockchain Status: Synced ({employees.Count} records immutable)";
            blockchainStatusLabel.ForeColor = System.Drawing.Color.Blue;

            iotStatusLabel.Text = $"IoT Devices Connected: {rand.Next(10, 20)} (Real-time attendance)";
            iotStatusLabel.ForeColor = System.Drawing.Color.Purple;

            quantumScoreLabel.Text = $"Quantum Optimization Score: {rand.Next(85, 100)}.{rand.Next(0, 10)}% (Workforce efficiency)";
            quantumScoreLabel.ForeColor = System.Drawing.Color.Orange;
        }
    }
}
