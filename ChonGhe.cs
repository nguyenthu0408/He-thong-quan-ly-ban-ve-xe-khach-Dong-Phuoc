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
    public partial class ChonGhe : Form
    {
        private string maChuyen;
        private DateTime gioKhoiHanh;
        private DateTime ngayKhoiHanh; 
        KetNoi ketnoi;
        public ChonGhe(string maChuyen, DateTime gioKhoiHanh, DateTime ngayKhoiHanh)
        {
            ketnoi = new KetNoi();
            InitializeComponent();
            this.maChuyen = maChuyen;
            this.gioKhoiHanh = gioKhoiHanh;
            this.ngayKhoiHanh = ngayKhoiHanh;
            LoadGheTrong();
            dgvDSGheTrong.CellContentClick += dgvDSGheTrong_CellContentClick;
        }
        public DataGridView dgvDSGheTrongPublic => dgvDSGheTrong;
        private void LoadGheTrong()
        {
                try
                {
                    string query = @"
            SELECT COGHE.SOGHE, GHE.VITRI, CHUYENXE.GIAVE
            FROM COGHE 
            JOIN GHE ON COGHE.SOGHE = GHE.SOGHE
            JOIN CHUYENXE ON COGHE.MACHUYEN = CHUYENXE.MACHUYEN
            WHERE COGHE.MACHUYEN = @maChuyen
            AND COGHE.TINHTRANG = N'Ghế trống'";
                    SqlParameter[] parameters = {
            new SqlParameter("@maChuyen", maChuyen)
        };

                    DataTable dt = ketnoi.ExecuteDataTable(query, parameters);

                    if (dt.Rows.Count > 0)
                    {
                        DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn
                        {
                            HeaderText = "Chọn",
                            Name = "CheckBoxColumn",
                            Width = 50,
                            ReadOnly = false,
                            FillWeight = 20
                        };

                        dgvDSGheTrong.Columns.Clear();
                        dgvDSGheTrong.Columns.Add(checkBoxColumn); 
                        dgvDSGheTrong.DataSource = dt; 
                        dgvDSGheTrong.Columns["SOGHE"].HeaderText = "Số ghế";
                        dgvDSGheTrong.Columns["VITRI"].HeaderText = "Vị trí";
                        dgvDSGheTrong.Columns["GIAVE"].HeaderText = "Giá vé";
                        dgvDSGheTrong.Columns["GIAVE"].Visible = false; 
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy ghế trống cho chuyến xe này.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải ghế: " + ex.Message);
                }
            }
        private void btnChon_Click(object sender, EventArgs e)
        {
            List<string> selectedSeats = new List<string>();
            decimal totalAmount = 0;

            foreach (DataGridViewRow row in dgvDSGheTrong.Rows)
            {
                if (Convert.ToBoolean(row.Cells["CheckBoxColumn"].Value))
                {
                    selectedSeats.Add(row.Cells["SOGHE"].Value.ToString());
                    totalAmount += Convert.ToDecimal(row.Cells["GIAVE"].Value);
                }
            }

            if (selectedSeats.Count > 0)
            {
                string maChuyen = TraCuuChuyen.MaChuyen;
                DateTime gioKhoiHanh = TraCuuChuyen.GioKhoiHanh;
                DateTime ngayKhoiHanh = TraCuuChuyen.NgayKhoiHanh;

                string maNV = MainForm.CurrentUserId;

                ThanhToan paymentForm = new ThanhToan(maChuyen, totalAmount, gioKhoiHanh, ngayKhoiHanh, selectedSeats, maNV);
                this.Hide();
                paymentForm.ShowDialog();
                this.Show();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn ít nhất một ghế.");
            }
        }

        private void dgvDSGheTrong_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvDSGheTrong.Columns["CheckBoxColumn"].Index && e.RowIndex >= 0)
            {
                var checkBoxCell = dgvDSGheTrong.Rows[e.RowIndex].Cells["CheckBoxColumn"];
                checkBoxCell.Value = !(Convert.ToBoolean(checkBoxCell.Value));
                dgvDSGheTrong.EndEdit();
                UpdateSelectedSeatsAndTotal();
            }
        }
        private void UpdateSelectedSeatsAndTotal()
        {
            var selectedSeatNumbers = new List<string>();
            decimal total = 0;

            foreach (DataGridViewRow row in dgvDSGheTrong.Rows)
            {
                if (Convert.ToBoolean(row.Cells["CheckBoxColumn"].Value))
                {
                    string seatNumber = row.Cells["SOGHE"].Value.ToString();
                    decimal ticketPrice = Convert.ToDecimal(row.Cells["GIAVE"].Value);
                    if (!selectedSeatNumbers.Contains(seatNumber))
                    {
                        selectedSeatNumbers.Add(seatNumber);
                    }

                    total += ticketPrice;
                }
            }
            txtGhe.Text = string.Join(", ", selectedSeatNumbers);
            txtTongTien.Text = total.ToString("C");
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
