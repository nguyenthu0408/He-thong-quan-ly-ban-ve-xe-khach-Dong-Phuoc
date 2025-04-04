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
    public partial class ThanhToan : Form
    {
        private decimal totalAmount; 
        private string maChuyen;
        private DateTime gioKhoiHanh;
        private DateTime ngayKhoiHanh;
        private List<string> selectedSeats;
        private string maNV;
        KetNoi ketnoi;
        public ThanhToan(string maChuyen, decimal totalAmount, DateTime gioKhoiHanh, DateTime ngayKhoiHanh, List<string> selectedSeats, string maNV)
        {
            InitializeComponent();
            this.totalAmount = totalAmount;
            this.maChuyen = maChuyen;
            this.gioKhoiHanh = gioKhoiHanh;
            this.ngayKhoiHanh = ngayKhoiHanh;
            this.selectedSeats = selectedSeats;
            this.maNV = maNV; 
            txtTongTien.Text = totalAmount.ToString("C");
            ketnoi = new KetNoi();
        }

        private void btnTinh_Click(object sender, EventArgs e)
        {
            decimal customerAmount;
            if (decimal.TryParse(txtDua.Text, out customerAmount))
            {
                decimal change = customerAmount - totalAmount;
                txtThoi.Text = change >= 0 ? change.ToString("C") : "Số tiền không đủ";
            }
            else
            {
                MessageBox.Show("Vui lòng nhập số tiền hợp lệ.");
            }
        }

        private void rdoCK_CheckedChanged(object sender, EventArgs e)
        {
            txtDua.Enabled = !rdoCK.Checked;
            txtThoi.Enabled = !rdoCK.Checked;
        }
        private void UpdateSeatStatusAndReduceSeats(string machuyen, string ghe)
        {
            try
            {
                KetNoi ketNoi = new KetNoi();
                string updateSeatStatusQuery = @"
            UPDATE COGHE
            SET TINHTRANG = N'Ghế đã bán'
            WHERE MACHUYEN = @maChuyen AND SOGHE = @ghe";

                SqlParameter[] seatStatusParameters = {
            new SqlParameter("@maChuyen", machuyen),
            new SqlParameter("@ghe", ghe)
        };

                ketNoi.ExecuteNonQuery(updateSeatStatusQuery, seatStatusParameters);
                string reduceSeatsQuery = @"
            UPDATE CHUYENXE
            SET SOGHETRONG = SOGHETRONG - 1
            WHERE MACHUYEN = @maChuyen";

                SqlParameter[] reduceSeatsParameters = {
            new SqlParameter("@maChuyen", machuyen)
        };

                ketNoi.ExecuteNonQuery(reduceSeatsQuery, reduceSeatsParameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật ghế: " + ex.Message);
            }
        }

        private void btnIn_Click(object sender, EventArgs e)
        {
            string maChuyen = TraCuuChuyen.MaChuyen;
            DateTime gioChay = TraCuuChuyen.GioKhoiHanh;
            DateTime ngay = TraCuuChuyen.NgayKhoiHanh;
            string maNV = MainForm.CurrentUserId;

            foreach (string seat in selectedSeats)
            {
                string maVeMoi = GenerateRandomTicketCode();
                InsertTicket(maVeMoi, maChuyen, maNV, gioChay, ngay, seat);
                UpdateSeatStatusAndReduceSeats(maChuyen, seat);
                Ve veForm = new Ve(maVeMoi);
                veForm.ShowDialog();
            }
        }
        private string GenerateRandomTicketCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void InsertTicket(string maVe, string maChuyen, string maNV, DateTime gioChay, DateTime ngay, string ghe)
        {
            try
            {
                KetNoi ketNoi = new KetNoi();

                string query = @"
            INSERT INTO VE (MAVE, MACHUYEN, MANV, GIOCHAY, NGAY, GHE, GIOIN)
            VALUES (@maVe, @maChuyen, @maNV, @gioChay, @ngay, @ghe, @gioIn)";

                SqlParameter[] parameters = {
            new SqlParameter("@maVe", maVe),
            new SqlParameter("@maChuyen", maChuyen),
            new SqlParameter("@maNV", maNV),
            new SqlParameter("@gioChay", gioChay),
            new SqlParameter("@ngay", ngay),
            new SqlParameter("@ghe", ghe),
            new SqlParameter("@gioIn", DateTime.Now)
        };

                ketNoi.ExecuteNonQuery(query, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm vé: " + ex.Message);
            }
    }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
    }

