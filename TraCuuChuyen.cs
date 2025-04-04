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
    public partial class TraCuuChuyen : Form
    {
        public static DateTime GioKhoiHanh { get; private set; }
        public static DateTime NgayKhoiHanh { get; private set; }
        public static string MaChuyen { get; private set; } 
        public DataGridViewRow SelectedRow { get; private set; }
        private KetNoi ketNoi;
        public TraCuuChuyen()
        {
            InitializeComponent();
            ketNoi = new KetNoi();
            LoadPlaces();
        }
        public string GetMaChuyen()
        {
            using (TraCuuChuyen formTraCuuChuyen = new TraCuuChuyen())
            {
                if (formTraCuuChuyen.ShowDialog() == DialogResult.OK) 
                {
                    return MaChuyen;
                }
                else
                {
                    return null;
                }
            }
        }
        public DateTime GetGioKhoiHanh()
        {
            using (TraCuuChuyen formTraCuuChuyen = new TraCuuChuyen())
            {
                if (formTraCuuChuyen.ShowDialog() == DialogResult.OK) 
                {
                    return GioKhoiHanh; 
                }
                else
                {
                    return DateTime.Now;
                }
            }
        }
        public DateTime GetNgayKhoiHanh()
        {
            using (TraCuuChuyen formTraCuuChuyen = new TraCuuChuyen())
            {
                if (formTraCuuChuyen.ShowDialog() == DialogResult.OK) 
                {
                    return NgayKhoiHanh;
                }
                else
                {
                    return DateTime.Now;
                }
            }
        }
        private void LoadPlaces()
        {
            LoadComboBox("SELECT MaNoiDi, TenNoiDi FROM NoiDi", cmbDeparture, "MaNoiDi", "TenNoiDi");
            LoadComboBox("SELECT MaNoiDen, TenNoiDen FROM NoiDen", cmbDestination, "MaNoiDen", "TenNoiDen");
        }
        private void LoadComboBox(string query, ComboBox comboBox, string valueField, string textField)
        {

            using (var reader = ketNoi.ExecuteReader(query, null))
            {
                while (reader.Read())
                {
                    comboBox.Items.Add(new ComboBoxItem
                    {
                        Value = reader[valueField].ToString(),
                        Text = reader[textField].ToString()
                    });
                }
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
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbDeparture.SelectedItem == null || cmbDestination.SelectedItem == null)
                {
                    MessageBox.Show("Please select both departure and destination.");
                    return;
                }
                string maNoiDi = ((ComboBoxItem)cmbDeparture.SelectedItem).Value;
                string maNoiDen = ((ComboBoxItem)cmbDestination.SelectedItem).Value;
                DateTime ngayKhoiHanh = dtpDepartureDate.Value.Date;
                string query = @"
            SELECT ChuyenXe.MaChuyen, ChuyenXe.NgayKhoiHanh, NoiDi.TenNoiDi, NoiDen.TenNoiDen, ChuyenXe.MaLoai, ChuyenXe.GioKhoiHanh  
            FROM ChuyenXe 
            JOIN NoiDi ON ChuyenXe.MaNoiDi = NoiDi.MaNoiDi 
            JOIN NoiDen ON ChuyenXe.MaNoiDen = NoiDen.MaNoiDen 
            WHERE ChuyenXe.MaNoiDi = @maNoiDi 
            AND ChuyenXe.MaNoiDen = @maNoiDen 
            AND CONVERT(date, ChuyenXe.NgayKhoiHanh) = @ngayKhoiHanh";
                SqlParameter[] parameters = {
            new SqlParameter("@maNoiDi", maNoiDi),
            new SqlParameter("@maNoiDen", maNoiDen),
            new SqlParameter("@ngayKhoiHanh", ngayKhoiHanh)
        };
                DataTable dt = ketNoi.ExecuteDataTable(query, parameters);
                dgvTripList.DataSource = dt;
                dgvTripList.Columns["MaChuyen"].HeaderText = "Mã chuyến";
                dgvTripList.Columns["TenNoiDi"].HeaderText = "Nơi đi";
                dgvTripList.Columns["TenNoiDen"].HeaderText = "Nơi đến";
                dgvTripList.Columns["GioKhoiHanh"].HeaderText = "Giờ khởi hành";
                dgvTripList.Columns["NgayKhoiHanh"].HeaderText = "Ngày khởi hành";
                dgvTripList.Columns["MaLoai"].HeaderText = "Loại xe";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
    }


        private void dgvTripList_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SelectedRow = dgvTripList.Rows[e.RowIndex];
                MaChuyen = SelectedRow.Cells["MaChuyen"].Value.ToString();
                GioKhoiHanh = Convert.ToDateTime(SelectedRow.Cells["GioKhoiHanh"].Value);
                NgayKhoiHanh = Convert.ToDateTime(SelectedRow.Cells["NgayKhoiHanh"].Value); 
                ChonGhe formChonGhe = new ChonGhe(MaChuyen, GioKhoiHanh, NgayKhoiHanh);
                formChonGhe.ShowDialog();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
