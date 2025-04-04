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
using static _25_NguyenLeMinhThu_9401_QuanLyBanVe.TraCuuChuyen;

namespace _25_NguyenLeMinhThu_9401_QuanLyBanVe
{
    public partial class QuanLyChuyenXe : Form
    {
        public QuanLyChuyenXe()
        {
            InitializeComponent();
            LoadData();
            dtpNgay.Format = DateTimePickerFormat.Short; 
            dtpGio.Format = DateTimePickerFormat.Custom;
            dtpGio.CustomFormat = "dd/MM/yyyy HH:mm"; 
            dtpGioDen.Format = DateTimePickerFormat.Custom;
            dtpGioDen.CustomFormat = "dd/MM/yyyy HH:mm"; 
            dgvDS.CellClick += dgvDS_CellClick;
            dtpNgay.ValueChanged += dtpNgay_ValueChanged;
        }
        private void LoadData()
        {
            KetNoi ketNoi = new KetNoi();
            cboGia.Items.Clear();
            cboGia.Items.Add(100000);
            cboGia.Items.Add(120000);

            try
            {
                string queryNoiDi = "SELECT MaNoiDi, TenNoiDi FROM NoiDi";
                using (SqlDataReader readerDeparture = ketNoi.ExecuteReader(queryNoiDi, null))
                {
                    while (readerDeparture.Read())
                    {
                        cboNoiDi.Items.Add(new ComboBoxItem
                        {
                            Value = readerDeparture["MaNoiDi"].ToString(),
                            Text = readerDeparture["TenNoiDi"].ToString()
                        });
                    }
                }
                string queryNoiDen = "SELECT MaNoiDen, TenNoiDen FROM NoiDen";
                using (SqlDataReader readerDestination = ketNoi.ExecuteReader(queryNoiDen, null))
                {
                    while (readerDestination.Read())
                    {
                        cboNoiDen.Items.Add(new ComboBoxItem
                        {
                            Value = readerDestination["MaNoiDen"].ToString(),
                            Text = readerDestination["TenNoiDen"].ToString()
                        });
                    }
                }
                string queryLoai = "SELECT MALOAI, TENLOAI from LOAIXE";
                using (SqlDataReader readerLoai = ketNoi.ExecuteReader(queryLoai, null))
                {
                    while (readerLoai.Read())
                    {
                        cboLoai.Items.Add(new ComboBoxItem
                        {
                            Value = readerLoai["MALOAI"].ToString(),
                            Text = readerLoai["TENLOAI"].ToString()
                        });
                    }
                }
                string queryChuyenXe = @"
            SELECT cx.MACHUYEN, lx.TENLOAI, cx.NGAYKHOIHANH, cx.GIOKHOIHANH, cx.GIODENDK, cx.SOGHETRONG, cx.GIAVE, ndn.TENNOIDEN, nd.TENNOIDI
            FROM CHUYENXE cx
            JOIN LOAIXE lx ON cx.MALOAI = lx.MALOAI
            JOIN NoiDi nd ON cx.MANOIDI = nd.MANOIDI
            JOIN NoiDen ndn ON cx.MANOIDEN = ndn.MANOIDEN";

                DataTable dataTable = ketNoi.ExecuteDataTable(queryChuyenXe, null);
                dgvDS.DataSource = dataTable;
                dgvDS.Columns["MACHUYEN"].HeaderText = "Mã chuyến";
                dgvDS.Columns["TENLOAI"].HeaderText = "Loại xe";
                dgvDS.Columns["NGAYKHOIHANH"].HeaderText = "Ngày khởi hành";
                dgvDS.Columns["GIOKHOIHANH"].HeaderText = "Giờ khởi hành";
                dgvDS.Columns["GIODENDK"].HeaderText = "Giờ đến";
                dgvDS.Columns["SOGHETRONG"].HeaderText = "Ghế trống";
                dgvDS.Columns["GIAVE"].HeaderText = "Giá";
                dgvDS.Columns["TENNOIDEN"].HeaderText = "Nơi đến";
                dgvDS.Columns["TENNOIDI"].HeaderText = "Nơi đi";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }
        public class ComboBoxItem
        {
            public string Value { get; set; }
            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string maChuyen = txtMaChuyen.Text;
            string maLoai = ((ComboBoxItem)cboLoai.SelectedItem).Value;
            string maNoiDi = ((ComboBoxItem)cboNoiDi.SelectedItem).Value;
            string maNoiDen = ((ComboBoxItem)cboNoiDen.SelectedItem).Value;
            DateTime ngayKhoiHanh = dtpNgay.Value.Date;
            TimeSpan gioKhoiHanh = dtpGio.Value.TimeOfDay;
            TimeSpan gioDenDuKien = dtpGioDen.Value.TimeOfDay;
            int slGhe = int.Parse(txtSL.Text);
            int giaVe = int.Parse(cboGia.SelectedItem.ToString());
            KetNoi kn = new KetNoi();
            using (SqlConnection conn = kn.GetConnection())
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    string insertChuyenXeQuery = @"
                INSERT INTO CHUYENXE (MACHUYEN, MALOAI, MANOIDI, MANOIDEN, NGAYKHOIHANH, GIOKHOIHANH, GIODENDK, SLGHE, SOGHETRONG, GIAVE) 
                VALUES (@MACHUYEN, @MALOAI, @MANOIDI, @MANOIDEN, @NGAYKHOIHANH, @GIOKHOIHANH, @GIODENDK, @SLGHE, @SOGHETRONG, @GIAVE)";
                    SqlParameter[] chuyenXeParams = new SqlParameter[]
                    {
                new SqlParameter("@MACHUYEN", maChuyen),
                new SqlParameter("@MALOAI", maLoai),
                new SqlParameter("@MANOIDI", maNoiDi),
                new SqlParameter("@MANOIDEN", maNoiDen),
                new SqlParameter("@NGAYKHOIHANH", ngayKhoiHanh),
                new SqlParameter("@GIOKHOIHANH", ngayKhoiHanh + gioKhoiHanh),
                new SqlParameter("@GIODENDK", ngayKhoiHanh + gioDenDuKien),
                new SqlParameter("@SLGHE", slGhe),
                new SqlParameter("@SOGHETRONG", slGhe),
                new SqlParameter("@GIAVE", giaVe)
                    };
                    kn.ExecuteNonQuery(insertChuyenXeQuery, chuyenXeParams);
                    string insertCoGheQuery = @"
                INSERT INTO COGHE (MACHUYEN, SOGHE, TINHTRANG) 
                VALUES (@MACHUYEN, @SOGHE, N'Ghế trống')";
                    for (int i = 1; i <= slGhe; i++)
                    {
                        SqlParameter[] coGheParams = new SqlParameter[]
                        {
                    new SqlParameter("@MACHUYEN", maChuyen),
                    new SqlParameter("@SOGHE", i)
                        };
                        kn.ExecuteNonQuery(insertCoGheQuery, coGheParams);
                    }

                    transaction.Commit();
                    MessageBox.Show("Thêm chuyến xe thành công!");
                    LoadData();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
                }
            }
        }

            private void btnXoa_Click(object sender, EventArgs e)
        {
            string maChuyen = txtMaChuyen.Text;
            if (string.IsNullOrEmpty(maChuyen))
            {
                MessageBox.Show("Vui lòng nhập mã chuyến xe cần xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa chuyến xe này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            KetNoi ketNoi = new KetNoi(); 
            using (SqlConnection connection = ketNoi.GetConnection())
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    string deleteCogheQuery = "DELETE FROM COGHE WHERE MACHUYEN = @MaChuyen";
                    SqlParameter[] cogheParameters = new SqlParameter[] { new SqlParameter("@MaChuyen", maChuyen) };
                    ketNoi.ExecuteNonQuery(deleteCogheQuery, cogheParameters);
                    string deleteChuyenXeQuery = "DELETE FROM CHUYENXE WHERE MACHUYEN = @MaChuyen";
                    SqlParameter[] chuyenXeParameters = new SqlParameter[] { new SqlParameter("@MaChuyen", maChuyen) };
                    ketNoi.ExecuteNonQuery(deleteChuyenXeQuery, chuyenXeParameters);

                    transaction.Commit();
                    MessageBox.Show("Xóa chuyến xe thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Có lỗi xảy ra khi xóa chuyến xe: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
    }

        private void btnSua_Click(object sender, EventArgs e)
        {
            string maChuyen = txtMaChuyen.Text;
            string maLoai = ((ComboBoxItem)cboLoai.SelectedItem).Value;
            string maNoiDi = ((ComboBoxItem)cboNoiDi.SelectedItem).Value;
            string maNoiDen = ((ComboBoxItem)cboNoiDen.SelectedItem).Value;
            DateTime ngayKhoiHanh = dtpNgay.Value.Date;
            TimeSpan gioKhoiHanh = dtpGio.Value.TimeOfDay;
            TimeSpan gioDenDuKien = dtpGioDen.Value.TimeOfDay;
            int slGhe = int.Parse(txtSL.Text);
            int giaVe = int.Parse(cboGia.SelectedItem.ToString());

            if (string.IsNullOrEmpty(maChuyen))
            {
                MessageBox.Show("Vui lòng nhập mã chuyến xe muốn sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn sửa chuyến xe này không?", "Xác nhận sửa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            using (SqlConnection connection = new KetNoi().GetConnection())
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    string updateChuyenXeQuery = @"
            UPDATE CHUYENXE 
            SET MALOAI = @MaLoai, MANOIDI = @MaNoiDi, MANOIDEN = @MaNoiDen, 
                NGAYKHOIHANH = @NgayKhoiHanh, GIOKHOIHANH = @GioKhoiHanh, 
                GIODENDK = @GioDenDuKien, SLGHE = @SLGhe, SOGHETRONG = @SOGHETRONG, GIAVE = @GiaVe 
            WHERE MACHUYEN = @MaChuyen";

                    SqlParameter[] updateParameters = new SqlParameter[]
                    {
                new SqlParameter("@MaChuyen", maChuyen),
                new SqlParameter("@MaLoai", maLoai),
                new SqlParameter("@MaNoiDi", maNoiDi),
                new SqlParameter("@MaNoiDen", maNoiDen),
                new SqlParameter("@NgayKhoiHanh", ngayKhoiHanh), 
                new SqlParameter("@GioKhoiHanh", ngayKhoiHanh + gioKhoiHanh), 
                new SqlParameter("@GioDenDuKien", ngayKhoiHanh + gioDenDuKien), 
                new SqlParameter("@SLGhe", slGhe),
                new SqlParameter("@SOGHETRONG", slGhe), 
                new SqlParameter("@GiaVe", giaVe)
                    };

                    new KetNoi().ExecuteNonQuery(updateChuyenXeQuery, updateParameters);
                    string deleteCoGheQuery = "DELETE FROM COGHE WHERE MACHUYEN = @MaChuyen";
                    SqlParameter[] deleteParameters = new SqlParameter[]
                    {
                new SqlParameter("@MaChuyen", maChuyen)
                    };
                    new KetNoi().ExecuteNonQuery(deleteCoGheQuery, deleteParameters);
                    string insertCoGheQuery = "INSERT INTO COGHE (MACHUYEN, SOGHE, TINHTRANG) VALUES (@MACHUYEN, @SOGHE, N'Ghế trống')";

                    for (int i = 1; i <= slGhe; i++)
                    {
                        SqlParameter[] insertParameters = new SqlParameter[]
                        {
                    new SqlParameter("@MACHUYEN", maChuyen),
                    new SqlParameter("@SOGHE", i)
                        };
                        new KetNoi().ExecuteNonQuery(insertCoGheQuery, insertParameters);
                    }

                    transaction.Commit();
                    MessageBox.Show("Cập nhật chuyến xe thành công!");
                    LoadData();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message);
                }
            }
        }

        private void dgvDS_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDS.Rows[e.RowIndex];

                txtMaChuyen.Text = row.Cells["MACHUYEN"].Value.ToString();
                cboLoai.SelectedIndex = cboLoai.FindStringExact(row.Cells["TENLOAI"].Value.ToString());
                cboNoiDi.SelectedIndex = cboNoiDi.FindStringExact(row.Cells["TENNOIDI"].Value.ToString());
                cboNoiDen.SelectedIndex = cboNoiDen.FindStringExact(row.Cells["TENNOIDEN"].Value.ToString());
                if (DateTime.TryParse(row.Cells["NGAYKHOIHANH"].Value.ToString(), out DateTime ngayKhoiHanh))
                {
                    dtpNgay.Value = ngayKhoiHanh;
                }

                if (DateTime.TryParse(row.Cells["GIOKHOIHANH"].Value.ToString(), out DateTime gioKhoiHanh))
                {
                    dtpGio.Value = gioKhoiHanh;
                }

                if (DateTime.TryParse(row.Cells["GIODENDK"].Value.ToString(), out DateTime gioDen))
                {
                    dtpGioDen.Value = gioDen;
                }
                if (decimal.TryParse(row.Cells["GIAVE"].Value.ToString(), out decimal giaVeDecimal))
                {
                    int giaVe = Convert.ToInt32(giaVeDecimal);
                    cboGia.SelectedIndex = cboGia.FindStringExact(giaVe.ToString());
                }

                txtSL.Text = row.Cells["SOGHETRONG"].Value.ToString();
            }
        }

        private void btnTim_Click(object sender, EventArgs e)
        {
            string maChuyen = txtMaChuyen.Text;

            if (string.IsNullOrEmpty(maChuyen))
            {
                MessageBox.Show("Vui lòng nhập mã chuyến xe.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            KetNoi ketNoi = new KetNoi();
            string query = "SELECT * FROM CHUYENXE WHERE MACHUYEN = @MaChuyen";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@MaChuyen", maChuyen)
            };

            DataTable result = ketNoi.ExecuteDataTable(query, parameters);

            if (result.Rows.Count > 0)
            {
                dgvDS.DataSource = result;
            }
            else
            {
                MessageBox.Show("Không tìm thấy chuyến xe với mã đã nhập.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dtpNgay_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = dtpNgay.Value.Date;
            dtpGio.Value = selectedDate.Add(dtpGio.Value.TimeOfDay);
            dtpGioDen.Value = selectedDate.Add(dtpGioDen.Value.TimeOfDay);
        }
    }
}
