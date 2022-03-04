namespace Raytracer
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.statistics = new System.Windows.Forms.Label();
            this.DepthSelector = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.SceneSelector = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ResolutionSelector = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.AAMultiplierSelector = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.DepthSelector)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SceneSelector)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(61, 61);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(312, 162);
            this.button1.TabIndex = 1;
            this.button1.Text = "Start Render";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // statistics
            // 
            this.statistics.AutoSize = true;
            this.statistics.Location = new System.Drawing.Point(446, 144);
            this.statistics.Name = "statistics";
            this.statistics.Size = new System.Drawing.Size(145, 80);
            this.statistics.TabIndex = 2;
            this.statistics.Text = "Renderdauer   :  0s\nAnzeigedauer  :  0s\nAnzahl Rays    :  0\nAnzahl Vectors:  0";
            // 
            // DepthSelector
            // 
            this.DepthSelector.Location = new System.Drawing.Point(262, 379);
            this.DepthSelector.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.DepthSelector.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DepthSelector.Name = "DepthSelector";
            this.DepthSelector.Size = new System.Drawing.Size(111, 26);
            this.DepthSelector.TabIndex = 7;
            this.DepthSelector.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(57, 385);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 20);
            this.label5.TabIndex = 8;
            this.label5.Text = "Depth";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(57, 292);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(123, 20);
            this.label6.TabIndex = 9;
            this.label6.Text = "Scene to render";
            // 
            // SceneSelector
            // 
            this.SceneSelector.Location = new System.Drawing.Point(261, 286);
            this.SceneSelector.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.SceneSelector.Name = "SceneSelector";
            this.SceneSelector.Size = new System.Drawing.Size(112, 26);
            this.SceneSelector.TabIndex = 10;
            this.SceneSelector.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(57, 341);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(85, 20);
            this.label7.TabIndex = 12;
            this.label7.Text = "Resolution";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(57, 430);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(158, 20);
            this.label8.TabIndex = 13;
            this.label8.Text = "AntiAliasing Multiplier";
            // 
            // ResolutionSelector
            // 
            this.ResolutionSelector.FormattingEnabled = true;
            this.ResolutionSelector.Items.AddRange(new object[] {
            "360p",
            "720p",
            "1080p",
            "1440p",
            "4k",
            "8k"});
            this.ResolutionSelector.Location = new System.Drawing.Point(261, 333);
            this.ResolutionSelector.Name = "ResolutionSelector";
            this.ResolutionSelector.Size = new System.Drawing.Size(112, 28);
            this.ResolutionSelector.TabIndex = 15;
            this.ResolutionSelector.Text = "360p";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(440, 61);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(181, 46);
            this.label9.TabIndex = 16;
            this.label9.Text = "Statistics";
            // 
            // AAMultiplierSelector
            // 
            this.AAMultiplierSelector.FormattingEnabled = true;
            this.AAMultiplierSelector.Items.AddRange(new object[] {
            "1",
            "2",
            "4",
            "8",
            "16",
            "32"});
            this.AAMultiplierSelector.Location = new System.Drawing.Point(261, 422);
            this.AAMultiplierSelector.Name = "AAMultiplierSelector";
            this.AAMultiplierSelector.Size = new System.Drawing.Size(112, 28);
            this.AAMultiplierSelector.TabIndex = 17;
            this.AAMultiplierSelector.Text = "1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 526);
            this.Controls.Add(this.AAMultiplierSelector);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.ResolutionSelector);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.SceneSelector);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.DepthSelector);
            this.Controls.Add(this.statistics);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DepthSelector)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SceneSelector)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label statistics;
        private System.Windows.Forms.NumericUpDown DepthSelector;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown SceneSelector;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox ResolutionSelector;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox AAMultiplierSelector;
    }
}

