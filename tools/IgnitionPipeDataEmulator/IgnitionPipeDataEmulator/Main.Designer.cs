namespace IgnitionPipeDataEmulator
{
    partial class Main
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
            this.tbLogFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btBrowse = new System.Windows.Forms.Button();
            this.opfLogFile = new System.Windows.Forms.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.btEmulate = new System.Windows.Forms.Button();
            this.btStop = new System.Windows.Forms.Button();
            this.pbMain = new System.Windows.Forms.ProgressBar();
            this.tbLog = new System.Windows.Forms.TextBox();
            this.bwMain = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // tbLogFile
            // 
            this.tbLogFile.Location = new System.Drawing.Point(12, 25);
            this.tbLogFile.Name = "tbLogFile";
            this.tbLogFile.Size = new System.Drawing.Size(302, 20);
            this.tbLogFile.TabIndex = 0;
            this.tbLogFile.TextChanged += new System.EventHandler(this.tbLogFile_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Log file:";
            // 
            // btBrowse
            // 
            this.btBrowse.Location = new System.Drawing.Point(320, 23);
            this.btBrowse.Name = "btBrowse";
            this.btBrowse.Size = new System.Drawing.Size(75, 23);
            this.btBrowse.TabIndex = 2;
            this.btBrowse.Text = "Browse...";
            this.btBrowse.UseVisualStyleBackColor = true;
            this.btBrowse.Click += new System.EventHandler(this.btBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Log:";
            // 
            // btEmulate
            // 
            this.btEmulate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btEmulate.Enabled = false;
            this.btEmulate.Location = new System.Drawing.Point(407, 226);
            this.btEmulate.Name = "btEmulate";
            this.btEmulate.Size = new System.Drawing.Size(98, 23);
            this.btEmulate.TabIndex = 5;
            this.btEmulate.Text = "Start Emulation";
            this.btEmulate.UseVisualStyleBackColor = true;
            this.btEmulate.Click += new System.EventHandler(this.btEmulate_Click);
            // 
            // btStop
            // 
            this.btStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btStop.Enabled = false;
            this.btStop.Location = new System.Drawing.Point(511, 226);
            this.btStop.Name = "btStop";
            this.btStop.Size = new System.Drawing.Size(75, 23);
            this.btStop.TabIndex = 6;
            this.btStop.Text = "Stop";
            this.btStop.UseVisualStyleBackColor = true;
            this.btStop.Click += new System.EventHandler(this.btStop_Click);
            // 
            // pbMain
            // 
            this.pbMain.Location = new System.Drawing.Point(12, 226);
            this.pbMain.Name = "pbMain";
            this.pbMain.Size = new System.Drawing.Size(389, 23);
            this.pbMain.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbMain.TabIndex = 7;
            this.pbMain.Visible = false;
            // 
            // tbLog
            // 
            this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLog.BackColor = System.Drawing.SystemColors.Window;
            this.tbLog.Location = new System.Drawing.Point(12, 79);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(574, 141);
            this.tbLog.TabIndex = 8;
            // 
            // bwMain
            // 
            this.bwMain.WorkerReportsProgress = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 261);
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.pbMain);
            this.Controls.Add(this.btStop);
            this.Controls.Add(this.btEmulate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btBrowse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbLogFile);
            this.Name = "Main";
            this.Text = "Ignition Pipe Emulator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbLogFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btBrowse;
        private System.Windows.Forms.OpenFileDialog opfLogFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btEmulate;
        private System.Windows.Forms.Button btStop;
        private System.Windows.Forms.ProgressBar pbMain;
        private System.Windows.Forms.TextBox tbLog;
        private System.ComponentModel.BackgroundWorker bwMain;
    }
}

