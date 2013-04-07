namespace HDRControl
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.lblCameraName = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.prgBattery = new System.Windows.Forms.ProgressBar();
            this.lblISOSpeeds = new System.Windows.Forms.Label();
            this.lblExposureCompensation = new System.Windows.Forms.Label();
            this.lstISOSpeeds = new System.Windows.Forms.ComboBox();
            this.lstExposureCompensation = new System.Windows.Forms.ComboBox();
            this.lstShutterSpeeds = new System.Windows.Forms.ComboBox();
            this.lstBrackets = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRelease = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lstAperture = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lstMode = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.numDelay = new System.Windows.Forms.NumericUpDown();
            this.numMulti = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMulti)).BeginInit();
            this.SuspendLayout();
            // 
            // lblCameraName
            // 
            this.lblCameraName.AutoSize = true;
            this.lblCameraName.Location = new System.Drawing.Point(107, 12);
            this.lblCameraName.Name = "lblCameraName";
            this.lblCameraName.Size = new System.Drawing.Size(27, 13);
            this.lblCameraName.TabIndex = 3;
            this.lblCameraName.Text = "N/A";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Camera Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Battery Level";
            // 
            // prgBattery
            // 
            this.prgBattery.Location = new System.Drawing.Point(110, 35);
            this.prgBattery.Name = "prgBattery";
            this.prgBattery.Size = new System.Drawing.Size(157, 14);
            this.prgBattery.TabIndex = 0;
            // 
            // lblISOSpeeds
            // 
            this.lblISOSpeeds.AutoSize = true;
            this.lblISOSpeeds.Location = new System.Drawing.Point(12, 85);
            this.lblISOSpeeds.Name = "lblISOSpeeds";
            this.lblISOSpeeds.Size = new System.Drawing.Size(59, 13);
            this.lblISOSpeeds.TabIndex = 10;
            this.lblISOSpeeds.Text = "ISO Speed";
            // 
            // lblExposureCompensation
            // 
            this.lblExposureCompensation.AutoSize = true;
            this.lblExposureCompensation.Location = new System.Drawing.Point(12, 112);
            this.lblExposureCompensation.Name = "lblExposureCompensation";
            this.lblExposureCompensation.Size = new System.Drawing.Size(84, 13);
            this.lblExposureCompensation.TabIndex = 9;
            this.lblExposureCompensation.Text = "Exposure Comp.";
            // 
            // lstISOSpeeds
            // 
            this.lstISOSpeeds.FormattingEnabled = true;
            this.lstISOSpeeds.Location = new System.Drawing.Point(110, 82);
            this.lstISOSpeeds.Name = "lstISOSpeeds";
            this.lstISOSpeeds.Size = new System.Drawing.Size(157, 21);
            this.lstISOSpeeds.TabIndex = 1;
            this.lstISOSpeeds.SelectedIndexChanged += new System.EventHandler(this.ISOSpeeds_SelectedIndexChanged);
            // 
            // lstExposureCompensation
            // 
            this.lstExposureCompensation.FormattingEnabled = true;
            this.lstExposureCompensation.Location = new System.Drawing.Point(110, 109);
            this.lstExposureCompensation.Name = "lstExposureCompensation";
            this.lstExposureCompensation.Size = new System.Drawing.Size(157, 21);
            this.lstExposureCompensation.TabIndex = 2;
            this.lstExposureCompensation.SelectedIndexChanged += new System.EventHandler(this.ExposureCompensation_SelectedIndexChanged);
            // 
            // lstShutterSpeeds
            // 
            this.lstShutterSpeeds.FormattingEnabled = true;
            this.lstShutterSpeeds.Location = new System.Drawing.Point(110, 136);
            this.lstShutterSpeeds.Name = "lstShutterSpeeds";
            this.lstShutterSpeeds.Size = new System.Drawing.Size(157, 21);
            this.lstShutterSpeeds.TabIndex = 3;
            this.lstShutterSpeeds.SelectedIndexChanged += new System.EventHandler(this.ShutterSpeed_SlectedIndexChanged);
            // 
            // lstBrackets
            // 
            this.lstBrackets.FormattingEnabled = true;
            this.lstBrackets.Location = new System.Drawing.Point(110, 190);
            this.lstBrackets.Name = "lstBrackets";
            this.lstBrackets.Size = new System.Drawing.Size(157, 21);
            this.lstBrackets.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 193);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Bracketing";
            // 
            // btnRelease
            // 
            this.btnRelease.Location = new System.Drawing.Point(191, 279);
            this.btnRelease.Name = "btnRelease";
            this.btnRelease.Size = new System.Drawing.Size(76, 23);
            this.btnRelease.TabIndex = 8;
            this.btnRelease.Text = "Release";
            this.btnRelease.UseVisualStyleBackColor = true;
            this.btnRelease.Click += new System.EventHandler(this.btnRelease_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(110, 279);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 9;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 139);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Shutter Speed";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 166);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 13);
            this.label5.TabIndex = 13;
            this.label5.Text = "Aperture";
            // 
            // lstAperture
            // 
            this.lstAperture.FormattingEnabled = true;
            this.lstAperture.Location = new System.Drawing.Point(110, 163);
            this.lstAperture.Name = "lstAperture";
            this.lstAperture.Size = new System.Drawing.Size(157, 21);
            this.lstAperture.TabIndex = 4;
            this.lstAperture.SelectedIndexChanged += new System.EventHandler(this.ApertureValue_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 58);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(34, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Mode";
            // 
            // lstMode
            // 
            this.lstMode.FormattingEnabled = true;
            this.lstMode.Location = new System.Drawing.Point(110, 55);
            this.lstMode.Name = "lstMode";
            this.lstMode.Size = new System.Drawing.Size(157, 21);
            this.lstMode.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 245);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Delay in seconds";
            // 
            // numDelay
            // 
            this.numDelay.Location = new System.Drawing.Point(110, 243);
            this.numDelay.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numDelay.Name = "numDelay";
            this.numDelay.Size = new System.Drawing.Size(157, 20);
            this.numDelay.TabIndex = 7;
            this.numDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numDelay.ThousandsSeparator = true;
            // 
            // numMulti
            // 
            this.numMulti.Location = new System.Drawing.Point(110, 217);
            this.numMulti.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numMulti.Name = "numMulti";
            this.numMulti.Size = new System.Drawing.Size(157, 20);
            this.numMulti.TabIndex = 6;
            this.numMulti.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numMulti.ThousandsSeparator = true;
            this.numMulti.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 219);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Release Count";
            // 
            // frmMain
            // 
            this.AcceptButton = this.btnRelease;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 314);
            this.Controls.Add(this.numMulti);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.numDelay);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lstMode);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lstAperture);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lstBrackets);
            this.Controls.Add(this.lstShutterSpeeds);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblISOSpeeds);
            this.Controls.Add(this.lblCameraName);
            this.Controls.Add(this.lblExposureCompensation);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lstISOSpeeds);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lstExposureCompensation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.prgBattery);
            this.Controls.Add(this.btnRelease);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Canon Control";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.numDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMulti)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblISOSpeeds;
        private System.Windows.Forms.Label lblExposureCompensation;
        private System.Windows.Forms.Button btnRelease;
        private System.Windows.Forms.ComboBox lstISOSpeeds;
        private System.Windows.Forms.ComboBox lstExposureCompensation;
        private System.Windows.Forms.ComboBox lstBrackets;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar prgBattery;
        private System.Windows.Forms.Label lblCameraName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ComboBox lstShutterSpeeds;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox lstAperture;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox lstMode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numDelay;
        private System.Windows.Forms.NumericUpDown numMulti;
        private System.Windows.Forms.Label label8;
    }
}

