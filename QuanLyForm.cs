using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _25_NguyenLeMinhThu_9401_QuanLyBanVe
{
    public partial class QuanLyForm : Form
    {
        public QuanLyForm()
        {
            InitializeComponent();
        }

        private void ThoatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BanVeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TraCuuChuyen traCuuChuyenForm = new TraCuuChuyen();
            traCuuChuyenForm.TopLevel = false; 
            traCuuChuyenForm.FormBorderStyle = FormBorderStyle.None; 
            traCuuChuyenForm.Dock = DockStyle.Fill; 
            panelChucNang.Controls.Clear(); 
            panelChucNang.Controls.Add(traCuuChuyenForm);
            traCuuChuyenForm.Show();
        }

        private void ThoatToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void QLChuyenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuanLyChuyenXe baocaoForm = new QuanLyChuyenXe();
            baocaoForm.TopLevel = false;
            baocaoForm.FormBorderStyle = FormBorderStyle.None;
            baocaoForm.Dock = DockStyle.Fill;
            panelChucNang.Controls.Clear();
            panelChucNang.Controls.Add(baocaoForm);
            baocaoForm.Show();
        }

        private void BaoCaoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TaoBaoCao baocaoForm = new TaoBaoCao();
            baocaoForm.TopLevel = false;
            baocaoForm.FormBorderStyle = FormBorderStyle.None;
            baocaoForm.Dock = DockStyle.Fill;
            panelChucNang.Controls.Clear();
            panelChucNang.Controls.Add(baocaoForm);
            baocaoForm.Show();
        }
    }
}
