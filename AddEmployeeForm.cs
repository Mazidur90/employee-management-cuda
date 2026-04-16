using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SchoolMangementSystem.Models;
using SchoolMangementSystem.Services;

namespace SchoolMangementSystem
{
    public partial class AddEmployeeForm : UserControl
    {
        private readonly EmployeeService employeeService;
        private string importedImagePath;
        private string persistedImagePath;
        private string selectedEmployeeId;

        public AddEmployeeForm()
        {
            employeeService = new EmployeeService();
            InitializeComponent();
            ConfigureUi();
            RefreshEmployees();
        }

        public void RefreshEmployees()
        {
            var employees = employeeService.GetActiveEmployees()
                .Select(employee => new EmployeeGridRow
                {
                    EmployeeId = employee.EmployeeId,
                    FullName = employee.FullName,
                    Gender = employee.Gender,
                    Address = employee.Address,
                    JobTitle = employee.JobTitle,
                    Department = employee.Department,
                    Status = employee.Status,
                    UpdatedAt = employee.UpdatedAtUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                    PhotoPath = employee.PhotoPath
                })
                .ToList();

            student_studentData.DataSource = employees;
            student_studentData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (student_studentData.Columns["PhotoPath"] != null)
            {
                student_studentData.Columns["PhotoPath"].Visible = false;
            }
        }

        private void ConfigureUi()
        {
            label1.Text = "Employee Directory";
            label2.Text = "Employee ID:";
            label6.Text = "Job Title:";
            label7.Text = "Department:";
            student_addBtn.Text = "Create";
            student_updateBtn.Text = "Save";
            student_deleteBtn.Text = "Archive";
            student_clearBtn.Click += student_clearBtn_Click;
            student_studentData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            student_studentData.MultiSelect = false;

            student_grade.Items.Clear();
            student_grade.Items.AddRange(new object[]
            {
                "Software Engineer",
                "Product Manager",
                "QA Analyst",
                "Designer",
                "HR Specialist",
                "Finance Analyst",
                "Operations Lead"
            });

            student_section.Items.Clear();
            student_section.Items.AddRange(new object[]
            {
                "Engineering",
                "Product",
                "Design",
                "Operations",
                "Finance",
                "People"
            });

            student_status.Items.Clear();
            student_status.Items.AddRange(new object[]
            {
                "Active",
                "On Leave",
                "Inactive"
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveEmployee(isUpdate: false);
        }

        private void student_updateBtn_Click(object sender, EventArgs e)
        {
            SaveEmployee(isUpdate: true);
        }

        private void SaveEmployee(bool isUpdate)
        {
            var employee = BuildEmployee();

            if (employee == null)
            {
                return;
            }

            if (!isUpdate && employeeService.EmployeeIdExists(employee.EmployeeId))
            {
                MessageBox.Show("That employee ID already exists.", "Duplicate employee", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (isUpdate && string.IsNullOrWhiteSpace(selectedEmployeeId))
            {
                MessageBox.Show("Select an employee from the grid before saving changes.", "Employee not selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            employee.PhotoPath = persistedImagePath;
            employeeService.Save(employee, importedImagePath);
            RefreshEmployees();
            MessageBox.Show(isUpdate ? "Employee updated successfully." : "Employee created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearFields();
        }

        private EmployeeRecord BuildEmployee()
        {
            if (string.IsNullOrWhiteSpace(student_id.Text)
                || string.IsNullOrWhiteSpace(student_name.Text)
                || string.IsNullOrWhiteSpace(student_gender.Text)
                || string.IsNullOrWhiteSpace(student_address.Text)
                || string.IsNullOrWhiteSpace(student_grade.Text)
                || string.IsNullOrWhiteSpace(student_section.Text)
                || string.IsNullOrWhiteSpace(student_status.Text))
            {
                MessageBox.Show("Complete all employee fields before saving.", "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            if (student_image.Image == null && string.IsNullOrWhiteSpace(persistedImagePath) && string.IsNullOrWhiteSpace(importedImagePath))
            {
                MessageBox.Show("Import a profile image before saving the employee.", "Image required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            return new EmployeeRecord
            {
                EmployeeId = student_id.Text.Trim(),
                FullName = student_name.Text.Trim(),
                Gender = student_gender.Text.Trim(),
                Address = student_address.Text.Trim(),
                JobTitle = student_grade.Text.Trim(),
                Department = student_section.Text.Trim(),
                Status = student_status.Text.Trim()
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var open = new OpenFileDialog())
            {
                open.Filter = "Image files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";

                if (open.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                importedImagePath = open.FileName;
                persistedImagePath = string.Empty;
                student_image.ImageLocation = importedImagePath;
            }
        }

        private void student_studentData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            var row = student_studentData.Rows[e.RowIndex].DataBoundItem as EmployeeGridRow;

            if (row == null)
            {
                return;
            }

            student_id.Text = row.EmployeeId;
            student_name.Text = row.FullName;
            student_gender.Text = row.Gender;
            student_address.Text = row.Address;
            student_grade.Text = row.JobTitle;
            student_section.Text = row.Department;
            student_status.Text = row.Status;
            importedImagePath = string.Empty;
            persistedImagePath = row.PhotoPath;
            selectedEmployeeId = row.EmployeeId;
            student_id.Enabled = false;
            LoadImage(row.PhotoPath, student_image);
        }

        private void student_deleteBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(student_id.Text))
            {
                MessageBox.Show("Select an employee first.", "Nothing selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Archive employee " + student_id.Text.Trim() + "?", "Confirm archive", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            employeeService.SoftDelete(student_id.Text.Trim());
            RefreshEmployees();
            ClearFields();
            MessageBox.Show("Employee archived successfully.", "Archived", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void student_clearBtn_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void ClearFields()
        {
            student_id.Text = string.Empty;
            student_name.Text = string.Empty;
            student_gender.SelectedIndex = -1;
            student_address.Text = string.Empty;
            student_grade.SelectedIndex = -1;
            student_section.SelectedIndex = -1;
            student_status.SelectedIndex = -1;
            student_image.Image = null;
            importedImagePath = string.Empty;
            persistedImagePath = string.Empty;
            selectedEmployeeId = string.Empty;
            student_id.Enabled = true;
        }

        private static void LoadImage(string path, PictureBox pictureBox)
        {
            pictureBox.Image = null;

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return;
            }

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var image = Image.FromStream(stream))
            {
                pictureBox.Image = new Bitmap(image);
            }
        }

        private class EmployeeGridRow
        {
            public string EmployeeId { get; set; }
            public string FullName { get; set; }
            public string Gender { get; set; }
            public string Address { get; set; }
            public string JobTitle { get; set; }
            public string Department { get; set; }
            public string Status { get; set; }
            public string UpdatedAt { get; set; }
            public string PhotoPath { get; set; }
        }
    }
}
