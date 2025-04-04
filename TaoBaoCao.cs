using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportAppServer;
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
    public partial class TaoBaoCao : Form
    {
        public TaoBaoCao()
        {
            InitializeComponent();
            LoadChiNhanh();
        }
        public class ChiNhanh
        {
            public string MACN { get; set; }
            public string TENCN { get; set; }

            public override string ToString()
            {
                return TENCN; 
            }
        }

        private void LoadChiNhanh()
        {
            KetNoi ketNoi = new KetNoi();
            string query = "SELECT MACN, TENCN FROM CHINHANH";
            using (SqlDataReader reader = ketNoi.ExecuteReader(query, null))
            {
                while (reader.Read())
                {
                    var chiNhanh = new ChiNhanh
                    {
                        MACN = reader["MACN"].ToString(),
                        TENCN = reader["TENCN"].ToString()
                    };
                    cboCN.Items.Add(chiNhanh);
                }
            }
        }

        private void chkNam_CheckedChanged(object sender, EventArgs e)
        {
            txtNam.Enabled = chkNam.Checked;
            txtThang.Enabled = false;
            chkThang.Checked = false;
        }

        private void chkThang_CheckedChanged(object sender, EventArgs e)
        {
            txtThang.Enabled = chkThang.Checked;
            txtNam.Enabled = chkThang.Checked; 
            if (!chkThang.Checked) 
            {
                txtThang.Clear();
                txtNam.Clear();
            }
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkNam.Checked && string.IsNullOrEmpty(txtNam.Text))
                {
                    MessageBox.Show("Vui lòng nhập năm.");
                    return;
                }

                if (chkThang.Checked)
                {
                    if (string.IsNullOrEmpty(txtThang.Text) || string.IsNullOrEmpty(txtNam.Text))
                    {
                        MessageBox.Show("Vui lòng nhập tháng và năm.");
                        return;
                    }
                }
                var selectedCN = (ChiNhanh)cboCN.SelectedItem;
                var maCN = selectedCN.MACN;
                int totalTickets = GetTotalTickets(maCN);
                decimal totalRevenue = GetTotalRevenue(maCN);
                ReportClass report;

                if (chkThang.Checked) 
                {
                    report = new Report1();
                    report.SetParameterValue("@Thang", int.Parse(txtThang.Text));
                    report.SetParameterValue("@Nam", int.Parse(txtNam.Text));
                }
                else if (chkNam.Checked) 
                {
                    report = new Report2();
                    report.SetParameterValue("@Nam", int.Parse(txtNam.Text));
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một tùy chọn (Tháng hoặc Năm).");
                    return;
                }
                report.SetParameterValue("@MaCN", maCN);
                report.SetParameterValue("@SLVe", totalTickets);
                report.SetParameterValue("@DoanhThu", totalRevenue);
                crystalReportViewer1.ReportSource = report;
                crystalReportViewer1.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }
        }
        private decimal GetTotalRevenue(string maCN)
        {
            decimal totalRevenue = 0;
            KetNoi ketNoi = new KetNoi();
            string query = @"
        SELECT SUM(CHUYENXE.GIAVE) 
        FROM VE 
        JOIN NHANVIEN ON VE.MANV = NHANVIEN.MANV 
        JOIN CHUYENXE ON VE.MACHUYEN = CHUYENXE.MACHUYEN 
        WHERE NHANVIEN.MACN = @MaCN";
            if (chkThang.Checked)
            {
                query += " AND MONTH(VE.GIOIN) = @Thang AND YEAR(VE.GIOIN) = @Nam";
            }
            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@MaCN", maCN),
        new SqlParameter("@Nam", int.Parse(txtNam.Text))
    };

            if (chkThang.Checked)
            {
                parameters.Add(new SqlParameter("@Thang", int.Parse(txtThang.Text)));
            }
            var result = ketNoi.ExecuteScalar(query, parameters.ToArray());
            totalRevenue = result != DBNull.Value ? Convert.ToDecimal(result) : 0;

            return totalRevenue;
        }

        private int GetTotalTickets(string maCN)
        {
            int totalTickets = 0;
            KetNoi ketNoi = new KetNoi();
            string query = @"
        SELECT COUNT(*) 
        FROM VE 
        JOIN NHANVIEN ON VE.MANV = NHANVIEN.MANV 
        WHERE NHANVIEN.MACN = @MaCN";
            if (chkThang.Checked)
            {
                query += " AND MONTH(VE.GIOIN) = @Thang AND YEAR(VE.GIOIN) = @Nam";
            }
            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@MaCN", maCN),
        new SqlParameter("@Nam", int.Parse(txtNam.Text))
    };

            if (chkThang.Checked)
            {
                parameters.Add(new SqlParameter("@Thang", int.Parse(txtThang.Text)));
            }
            var result = ketNoi.ExecuteScalar(query, parameters.ToArray());
            totalTickets = result != DBNull.Value ? Convert.ToInt32(result) : 0;

            return totalTickets;
        }
    }
}
