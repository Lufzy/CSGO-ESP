namespace CSGOESP
{
    partial class MainForm
    {
        /// <summary>
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer üretilen kod

        /// <summary>
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.cbESP = new System.Windows.Forms.CheckBox();
            this.cbTeammate = new System.Windows.Forms.CheckBox();
            this.cbEnemy = new System.Windows.Forms.CheckBox();
            this.pbTeammateColor = new System.Windows.Forms.PictureBox();
            this.pbEnemyColor = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbTeammateColor)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbEnemyColor)).BeginInit();
            this.SuspendLayout();
            // 
            // cbESP
            // 
            this.cbESP.AutoSize = true;
            this.cbESP.Checked = true;
            this.cbESP.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbESP.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.cbESP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.cbESP.Location = new System.Drawing.Point(12, 12);
            this.cbESP.Name = "cbESP";
            this.cbESP.Size = new System.Drawing.Size(44, 17);
            this.cbESP.TabIndex = 0;
            this.cbESP.Text = "ESP";
            this.cbESP.UseVisualStyleBackColor = true;
            this.cbESP.CheckedChanged += new System.EventHandler(this.cbESP_CheckedChanged);
            // 
            // cbTeammate
            // 
            this.cbTeammate.AutoSize = true;
            this.cbTeammate.Checked = true;
            this.cbTeammate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTeammate.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.cbTeammate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.cbTeammate.Location = new System.Drawing.Point(12, 55);
            this.cbTeammate.Name = "cbTeammate";
            this.cbTeammate.Size = new System.Drawing.Size(74, 17);
            this.cbTeammate.TabIndex = 1;
            this.cbTeammate.Text = "Teammate";
            this.cbTeammate.UseVisualStyleBackColor = true;
            this.cbTeammate.CheckedChanged += new System.EventHandler(this.cbTeammate_CheckedChanged);
            // 
            // cbEnemy
            // 
            this.cbEnemy.AutoSize = true;
            this.cbEnemy.Checked = true;
            this.cbEnemy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbEnemy.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.cbEnemy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.cbEnemy.Location = new System.Drawing.Point(12, 78);
            this.cbEnemy.Name = "cbEnemy";
            this.cbEnemy.Size = new System.Drawing.Size(56, 17);
            this.cbEnemy.TabIndex = 2;
            this.cbEnemy.Text = "Enemy";
            this.cbEnemy.UseVisualStyleBackColor = true;
            this.cbEnemy.CheckedChanged += new System.EventHandler(this.cbEnemy_CheckedChanged);
            // 
            // pbTeammateColor
            // 
            this.pbTeammateColor.BackColor = System.Drawing.Color.Blue;
            this.pbTeammateColor.Location = new System.Drawing.Point(92, 55);
            this.pbTeammateColor.Name = "pbTeammateColor";
            this.pbTeammateColor.Size = new System.Drawing.Size(17, 17);
            this.pbTeammateColor.TabIndex = 3;
            this.pbTeammateColor.TabStop = false;
            this.pbTeammateColor.Click += new System.EventHandler(this.pbTeammateColor_Click);
            // 
            // pbEnemyColor
            // 
            this.pbEnemyColor.BackColor = System.Drawing.Color.Red;
            this.pbEnemyColor.Location = new System.Drawing.Point(92, 78);
            this.pbEnemyColor.Name = "pbEnemyColor";
            this.pbEnemyColor.Size = new System.Drawing.Size(17, 17);
            this.pbEnemyColor.TabIndex = 4;
            this.pbEnemyColor.TabStop = false;
            this.pbEnemyColor.Click += new System.EventHandler(this.pbEnemyColor_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.ClientSize = new System.Drawing.Size(264, 106);
            this.Controls.Add(this.pbEnemyColor);
            this.Controls.Add(this.pbTeammateColor);
            this.Controls.Add(this.cbEnemy);
            this.Controls.Add(this.cbTeammate);
            this.Controls.Add(this.cbESP);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CSGOESP";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbTeammateColor)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbEnemyColor)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbESP;
        private System.Windows.Forms.CheckBox cbTeammate;
        private System.Windows.Forms.CheckBox cbEnemy;
        private System.Windows.Forms.PictureBox pbTeammateColor;
        private System.Windows.Forms.PictureBox pbEnemyColor;
    }
}

