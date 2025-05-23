﻿using System;
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
    public partial class NhanVienForm : Form
    {
        public NhanVienForm()
        {
            InitializeComponent();
        }

        private void BanVeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TraCuuChuyen traCuuChuyenForm = new TraCuuChuyen();
            traCuuChuyenForm.TopLevel = false; 
            traCuuChuyenForm.FormBorderStyle = FormBorderStyle.None; 
            traCuuChuyenForm.Dock = DockStyle.Fill;
            panelTraCuu.Controls.Clear();
            panelTraCuu.Controls.Add(traCuuChuyenForm);
            traCuuChuyenForm.Show();
        }

        private void ThoatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
