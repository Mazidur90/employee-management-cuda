using System;
using System.Windows.Forms;

namespace SchoolMangementSystem
{
    public partial class MainForm : Form
    {
        private readonly string displayName;

        public MainForm(string userDisplayName)
        {
            displayName = string.IsNullOrWhiteSpace(userDisplayName) ? "Operations Admin" : userDisplayName;
            InitializeComponent();
            label1.Text = "Welcome, " + displayName;
            label2.Text = "Employee Management System | Control Center";
            Dashboard_btn.Text = "Dashboard";
            AddEmployee_btn.Text = "Employees";
            AddTeacher_btn.Text = "Supervisors";
            ShowDashboard();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowDashboard();
        }

        private void AddEmployee_btn_Click(object sender, EventArgs e)
        {
            dashboardForm1.Visible = false;
            addTeachersForm1.Visible = false;
            addEmployeeForm1.Visible = true;
            addEmployeeForm1.RefreshEmployees();
            addEmployeeForm1.BringToFront();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dashboardForm1.Visible = false;
            addEmployeeForm1.Visible = false;
            addTeachersForm1.Visible = true;
            addTeachersForm1.RefreshSupervisors();
            addTeachersForm1.BringToFront();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Sign out of the current session?", "Confirm logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            var loginForm = new LoginForm();
            loginForm.Show();
            Close();
        }

        private void ShowDashboard()
        {
            addEmployeeForm1.Visible = false;
            addTeachersForm1.Visible = false;
            dashboardForm1.Visible = true;
            dashboardForm1.LoadDashboard(displayName);
            dashboardForm1.BringToFront();
        }
    }
}
