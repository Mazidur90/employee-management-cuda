using System;
using System.Windows.Forms;
using SchoolMangementSystem.Models;
using SchoolMangementSystem.Services;

namespace SchoolMangementSystem
{
    public partial class LoginForm : Form
    {
        private readonly AuthService authService;

        public LoginForm()
        {
            authService = new AuthService();
            InitializeComponent();
            label2.Text = "Employee Management System | Secure Login";
            username.Text = authService.DefaultUsername;
            password.Text = authService.DefaultPasswordHint;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void showPass_CheckedChanged(object sender, EventArgs e)
        {
            password.PasswordChar = showPass.Checked ? '\0' : '*';
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            AppUser user;

            if (!authService.TryLogin(username.Text, password.Text, out user))
            {
                MessageBox.Show("The username or password is incorrect.", "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var mainForm = new MainForm(user.DisplayName);
            mainForm.FormClosed += MainForm_FormClosed;
            mainForm.Show();
            Hide();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
        }
    }
}
