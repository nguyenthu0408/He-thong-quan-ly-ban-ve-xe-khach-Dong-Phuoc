using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _25_NguyenLeMinhThu_9401_QuanLyBanVe
{
    public partial class MainForm : Form
    {
        public static string CurrentUserId { get; private set; }
        private KetNoi ketNoi = new KetNoi();
        public MainForm()
        {
            InitializeComponent();
        }
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string username = txtuser.Text;
            string password = txtpassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập tên đăng nhập và mật khẩu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string query = "SELECT MANV, ChucVu FROM NhanVien WHERE TenDN = @username AND MatKhau = @password";
            SqlParameter[] parameters = {
        new SqlParameter("@username", username),
        new SqlParameter("@password", password)
    };

            try
            {
                using (SqlDataReader reader = ketNoi.ExecuteReader(query, parameters))
                {
                    if (reader.Read())
                    {
                        CurrentUserId = reader["MANV"].ToString();
                        string role = reader["ChucVu"].ToString();

                        if (role == "Nhân viên")
                        {
                            NhanVienForm nhanVienForm = new NhanVienForm();
                            this.Hide();
                            nhanVienForm.ShowDialog();
                            this.Show();
                        }
                        else if (role == "Quản lý")
                        {
                            QuanLyForm quanLyForm = new QuanLyForm();
                            this.Hide();
                            quanLyForm.ShowDialog();
                            this.Show();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Tên đăng nhập hoặc mật khẩu không đúng!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
