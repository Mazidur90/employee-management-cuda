using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SchoolMangementSystem.Models;
using SchoolMangementSystem.Services;

namespace SchoolMangementSystem
{
    public partial class AddTeachersForm : UserControl
    {
        private readonly SupervisorService supervisorService;
        private string importedImagePath;
        private string persistedImagePath;
        private string selectedSupervisorId;

        public AddTeachersForm()
        {
            supervisorService = new SupervisorService();
            InitializeComponent();
            ConfigureUi();
            RefreshSupervisors();
        }

        public void RefreshSupervisors()
        {
            var supervisors = supervisorService.GetActiveSupervisors()
                .Select(supervisor => new SupervisorGridRow
                {
                    SupervisorId = supervisor.SupervisorId,
                    FullName = supervisor.FullName,
                    Gender = supervisor.Gender,
                    Address = supervisor.Address,
                    Status = supervisor.Status,
                    UpdatedAt = supervisor.UpdatedAtUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm"),
                    PhotoPath = supervisor.PhotoPath
                })
                .ToList();

            teacher_gridData.DataSource = supervisors;
            teacher_gridData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (teacher_gridData.Columns["PhotoPath"] != null)
            {
                teacher_gridData.Columns["PhotoPath"].Visible = false;
            }
        }

        private void ConfigureUi()
        {
            label1.Text = "Supervisor Directory";
            label2.Text = "Supervisor ID:";
            teacher_addBtn.Text = "Create";
            teacher_updateBtn.Text = "Save";
            teacher_deleteBtn.Text = "Archive";
            teacher_status.Items.Clear();
            teacher_status.Items.AddRange(new object[] { "Active", "On Leave", "Inactive" });
            teacher_gridData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            teacher_gridData.MultiSelect = false;
        }

        private void teacher_addBtn_Click(object sender, EventArgs e)
        {
            SaveSupervisor(isUpdate: false);
        }

        private void teacher_updateBtn_Click(object sender, EventArgs e)
        {
            SaveSupervisor(isUpdate: true);
        }

        private void SaveSupervisor(bool isUpdate)
        {
            var supervisor = BuildSupervisor();

            if (supervisor == null)
            {
                return;
            }

            if (!isUpdate && supervisorService.SupervisorIdExists(supervisor.SupervisorId))
            {
                MessageBox.Show("That supervisor ID already exists.", "Duplicate supervisor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (isUpdate && string.IsNullOrWhiteSpace(selectedSupervisorId))
            {
                MessageBox.Show("Select a supervisor from the grid before saving changes.", "Supervisor not selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            supervisor.PhotoPath = persistedImagePath;
            supervisorService.Save(supervisor, importedImagePath);
            RefreshSupervisors();
            MessageBox.Show(isUpdate ? "Supervisor updated successfully." : "Supervisor created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearFields();
        }

        private SupervisorRecord BuildSupervisor()
        {
            if (string.IsNullOrWhiteSpace(teacher_id.Text)
                || string.IsNullOrWhiteSpace(teacher_name.Text)
                || string.IsNullOrWhiteSpace(teacher_gender.Text)
                || string.IsNullOrWhiteSpace(teacher_address.Text)
                || string.IsNullOrWhiteSpace(teacher_status.Text))
            {
                MessageBox.Show("Complete all supervisor fields before saving.", "Missing information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            if (teacher_image.Image == null && string.IsNullOrWhiteSpace(persistedImagePath) && string.IsNullOrWhiteSpace(importedImagePath))
            {
                MessageBox.Show("Import a profile image before saving the supervisor.", "Image required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            return new SupervisorRecord
            {
                SupervisorId = teacher_id.Text.Trim(),
                FullName = teacher_name.Text.Trim(),
                Gender = teacher_gender.Text.Trim(),
                Address = teacher_address.Text.Trim(),
                Status = teacher_status.Text.Trim()
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
                teacher_image.ImageLocation = importedImagePath;
            }
        }

        private void teacher_clearBtn_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void teacher_gridData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            var row = teacher_gridData.Rows[e.RowIndex].DataBoundItem as SupervisorGridRow;

            if (row == null)
            {
                return;
            }

            teacher_id.Text = row.SupervisorId;
            teacher_name.Text = row.FullName;
            teacher_gender.Text = row.Gender;
            teacher_address.Text = row.Address;
            teacher_status.Text = row.Status;
            importedImagePath = string.Empty;
            persistedImagePath = row.PhotoPath;
            selectedSupervisorId = row.SupervisorId;
            teacher_id.Enabled = false;
            LoadImage(row.PhotoPath, teacher_image);
        }

        private void teacher_deleteBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(teacher_id.Text))
            {
                MessageBox.Show("Select a supervisor first.", "Nothing selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Archive supervisor " + teacher_id.Text.Trim() + "?", "Confirm archive", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
            {
                return;
            }

            supervisorService.SoftDelete(teacher_id.Text.Trim());
            RefreshSupervisors();
            ClearFields();
            MessageBox.Show("Supervisor archived successfully.", "Archived", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ClearFields()
        {
            teacher_id.Text = string.Empty;
            teacher_name.Text = string.Empty;
            teacher_gender.SelectedIndex = -1;
            teacher_address.Text = string.Empty;
            teacher_status.SelectedIndex = -1;
            teacher_image.Image = null;
            importedImagePath = string.Empty;
            persistedImagePath = string.Empty;
            selectedSupervisorId = string.Empty;
            teacher_id.Enabled = true;
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

        private class SupervisorGridRow
        {
            public string SupervisorId { get; set; }
            public string FullName { get; set; }
            public string Gender { get; set; }
            public string Address { get; set; }
            public string Status { get; set; }
            public string UpdatedAt { get; set; }
            public string PhotoPath { get; set; }
        }
    }
}
