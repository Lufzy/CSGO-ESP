using Memorys;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSGOESP
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Process[] p = Process.GetProcessesByName("csgo");
            if(p.Length != 0)
            {
                bufferByte.Initialize("csgo");
                Globals.Client = bufferByte.GetModuleAddress("client.dll");
                Globals.Engine = bufferByte.GetModuleAddress("engine.dll");
                GameOverlay.InıtOverlay();
            }
            else
            {
                MessageBox.Show("Please, open CSGO", "CSGOESP", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(0);
            }
        }

        private void cbTeammate_CheckedChanged(object sender, EventArgs e)
        {
            Globals.Teammate = cbTeammate.Checked;
        }

        private void cbEnemy_CheckedChanged(object sender, EventArgs e)
        {
            Globals.Enemy = cbEnemy.Checked;
        }

        private void cbESP_CheckedChanged(object sender, EventArgs e)
        {
            Globals.ESP = cbESP.Checked;
        }

        private void pbTeammateColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();

            pbTeammateColor.BackColor = cd.Color;
            Globals.teammateColor = cd.Color;
        }

        private void pbEnemyColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.ShowDialog();

            pbEnemyColor.BackColor = cd.Color;
            Globals.enemyColor = cd.Color;
        }

        private void cbBoxESP_CheckedChanged(object sender, EventArgs e)
        {
            Globals.BoxESP = cbBoxESP.Checked;
        }

        private void cbSkeletonESP_CheckedChanged(object sender, EventArgs e)
        {
            Globals.SkeletonESP = cbSkeletonESP.Checked;
        }

        private void cbWatermark_CheckedChanged(object sender, EventArgs e)
        {
            Globals.Watermark = cbWatermark.Checked;

            if(Globals.Watermark)
            {
                DialogResult result = MessageBox.Show("Add Background for Watermark?", "CSGOESP", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    ColorDialog cd = new ColorDialog();
                    cd.ShowDialog();

                    Globals.WatermarkBG = true;
                    Globals.WatermarkBGColor = cd.Color;
                }
                else
                {
                    Globals.WatermarkBG = false;
                }
            }
        }
    }
}
