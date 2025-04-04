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
    public partial class Ve : Form
    {
        string maVe;
        public Ve(string maVe)
        {
            InitializeComponent();
            this.maVe = maVe;
        }

        private void Ve_Load(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            try
            {
                KetNoi ketNoi = new KetNoi();
                string query = "SELECT MAVE, MACHUYEN, GIOCHAY, NGAY, GHE, GIOIN FROM VE WHERE MAVE = @maVe";
                SqlParameter[] parameters = {
            new SqlParameter("@maVe", maVe)
        };

                using (SqlDataReader reader = ketNoi.ExecuteReader(query, parameters))
                {
                    if (reader.Read())
                    {
                        lblMaVe.Text = reader["MAVE"].ToString();
                        lblGhe.Text = reader["GHE"].ToString();
                        lblChuyenXe.Text = reader["MACHUYEN"].ToString();
                        lblGio.Text = reader["GIOCHAY"].ToString();
                        lblNgay.Text = reader["NGAY"].ToString();
                        lblIn.Text = reader["GIOIN"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin vé.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
    }
}

